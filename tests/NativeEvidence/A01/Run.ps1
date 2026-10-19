param(
    [string]$LibraryRoot = 'D:\Program Files\VisionMaster4.4.0\Applications\myLibs',
    [string]$OutputRoot = (Join-Path $PSScriptRoot '..\..\..\analysis\full-audit\batch1-20261019\A01')
)
$ErrorActionPreference = 'Stop'
$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
# Every replay gets its own app base and fixture directory. Do not load installation Librarys.Config.
$runRoot = Join-Path $OutputRoot ('replay-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $runRoot 'myLibs') -Force | Out-Null
[IO.File]::WriteAllText((Join-Path $runRoot 'myLibs\Librarys.Config'), '')
$exe = Join-Path $runRoot 'LoaderStateProbe.exe'
$framework = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319'
& (Join-Path $framework 'csc.exe') /nologo /target:exe "/out:$exe" "/r:$framework\WPF\WindowsBase.dll" "/r:$framework\WPF\PresentationFramework.dll" /r:System.Web.Extensions.dll (Join-Path $PSScriptRoot 'LoaderStateProbe.cs')
if ($LASTEXITCODE -ne 0) { throw "Compiler failed: $LASTEXITCODE" }
$before = Get-ChildItem -LiteralPath $LibraryRoot -File -Filter '*.dll' | Get-FileHash -Algorithm SHA256
$before | Export-Csv (Join-Path $runRoot 'original-before-sha256.csv') -NoTypeInformation -Encoding utf8
Push-Location $runRoot
try {
    & $exe $LibraryRoot (Join-Path $runRoot 'fixtures') (Join-Path $runRoot 'observations.json') *> (Join-Path $runRoot 'stdout-stderr.log')
    $probeExit = $LASTEXITCODE
} finally { Pop-Location }
$after = Get-ChildItem -LiteralPath $LibraryRoot -File -Filter '*.dll' | Get-FileHash -Algorithm SHA256
$after | Export-Csv (Join-Path $runRoot 'original-after-sha256.csv') -NoTypeInformation -Encoding utf8
$changes = @(Compare-Object $before $after -Property Path, Hash)
if ($changes.Count -ne 0) { throw 'Installed DLL hashes changed during replay' }
$observation = Get-Content -LiteralPath (Join-Path $runRoot 'observations.json') -Raw | ConvertFrom-Json
$loaded = foreach ($assembly in $observation.loadedAssemblies) {
    if ($assembly.path) {
        $actual = Get-FileHash -LiteralPath $assembly.path -Algorithm SHA256
        $candidate = Join-Path $LibraryRoot ([IO.Path]::GetFileName($assembly.path))
        $installedHash = if (Test-Path -LiteralPath $candidate) { (Get-FileHash -LiteralPath $candidate -Algorithm SHA256).Hash } else { $null }
        [pscustomobject]@{ Name = $assembly.name; LoadedPath = $assembly.path; SHA256 = $actual.Hash; InstalledCandidateSHA256 = $installedHash; MatchesCandidate = if ($installedHash) { $actual.Hash -eq $installedHash } else { $null } }
    }
}
$loaded | Export-Csv (Join-Path $runRoot 'loaded-assemblies-sha256.csv') -NoTypeInformation -Encoding utf8
if (@($loaded | Where-Object { $_.MatchesCandidate -eq $false }).Count) { throw 'A loaded dependency differs from the installed candidate' }
$outputHashes = Get-ChildItem -LiteralPath $runRoot -File -Recurse | Where-Object Name -ne 'output-sha256.csv' | Get-FileHash -Algorithm SHA256
$outputHashes | Export-Csv (Join-Path $runRoot 'output-sha256.csv') -NoTypeInformation -Encoding utf8
Get-Content -LiteralPath (Join-Path $runRoot 'stdout-stderr.log')
Write-Output "Evidence: $runRoot"
if ($probeExit -ne 0) { throw "Probe failed: $probeExit" }
