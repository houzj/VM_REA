#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$SummaryPath,
    [string]$DesktopDirectory,
    [string[]]$AdditionalAssembly = @(),
    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$summaryFile = Get-Item -LiteralPath $SummaryPath
$batch = $summaryFile.Directory.FullName
$summary = Get-Content -Raw -LiteralPath $summaryFile.FullName | ConvertFrom-Json
$runnerPath = Join-Path $PSScriptRoot 'Run-LifecycleRegression.ps1'
if ($summary.runnerSha256 -ne (Get-FileHash -LiteralPath $runnerPath).Hash) {
    throw 'The regression runner has changed since this batch.'
}
$expectedSuites = @([regex]::Matches((Get-Content -Raw -LiteralPath $runnerPath), "name = '([^']+)'") | ForEach-Object { $_.Groups[1].Value } | Sort-Object)
$actualSuites = @($summary.suites.project | Sort-Object)
if (!$expectedSuites.Count -or $actualSuites.Count -ne $expectedSuites.Count -or
    (Compare-Object $expectedSuites $actualSuites)) {
    throw 'The summary does not contain the complete regression suite set.'
}
if ($summary.passedSuites -ne $summary.suites.Count -or
    $summary.passedChecks -ne ($summary.suites | Measure-Object checks -Sum).Sum) {
    throw 'Regression totals disagree with suite records.'
}

function Read-SourceIdentity([string]$AssemblyPath) {
    $pdbPath = [IO.Path]::ChangeExtension($AssemblyPath, '.pdb')
    $peStream = [IO.File]::OpenRead($AssemblyPath)
    $pdbStream = [IO.File]::OpenRead($pdbPath)
    $pe = [System.Reflection.PortableExecutable.PEReader]::new($peStream)
    $provider = [System.Reflection.Metadata.MetadataReaderProvider]::FromPortablePdbStream(
        $pdbStream, [System.Reflection.Metadata.MetadataStreamOptions]::Default, 0)
    try {
        $reader = $provider.GetMetadataReader([System.Reflection.Metadata.MetadataReaderOptions]::Default, $null)
        $codeViews = @($pe.ReadDebugDirectory() | Where-Object Type -eq CodeView)
        if ($codeViews.Count -ne 1) { throw "Expected one CodeView entry: $AssemblyPath" }
        $codeView = $pe.ReadCodeViewDebugDirectoryData($codeViews[0])
        $pdbId = [System.Reflection.Metadata.BlobContentId]::new($reader.DebugMetadataHeader.Id)
        if ($codeView.Guid -ne $pdbId.Guid -or $codeViews[0].Stamp -ne $pdbId.Stamp) {
            throw "Assembly/PDB identity mismatch: $AssemblyPath"
        }
        $documents = @(
            foreach ($handle in $reader.Documents) {
                $document = $reader.GetDocument($handle)
                $path = $reader.GetString($document.Name)
                # Generated compiler inputs are build artifacts, not repository source.
                if ($path -notmatch '[/\\](src|tests)[/\\]' -or $path -match '[/\\]obj[/\\]') { continue }
                if (!$path.StartsWith($repo + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
                    throw "Source is outside the current checkout: $path"
                }
                $expected = [Convert]::ToHexString($reader.GetBlobBytes($document.Hash))
                $actual = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
                if ($expected -ne $actual) { throw "Source differs from tested assembly: $path" }
                [ordered]@{ path = [IO.Path]::GetRelativePath($repo, $path); sha256 = $actual }
            }
        )
        if (!$documents.Count) { throw "No repository source documents found: $AssemblyPath" }
        [ordered]@{ assembly = $AssemblyPath; sha256 = (Get-FileHash -LiteralPath $AssemblyPath).Hash; sources = $documents }
    } finally {
        $provider.Dispose(); $pe.Dispose(); $pdbStream.Dispose(); $peStream.Dispose()
    }
}

$assemblies = @(
    foreach ($suite in $summary.suites) {
        if ($suite.buildExit -ne 0 -or $suite.testExit -ne 0 -or $suite.error) { throw "Suite failed: $($suite.project)" }
        $files = @(Get-ChildItem -LiteralPath (Join-Path $batch "artifacts/bin/$($suite.project)") -Recurse -File -Filter "$($suite.project).dll")
        if ($files.Count -ne 1) { throw "Expected one suite assembly: $($suite.project)" }
        if ((Get-FileHash -LiteralPath $files[0].FullName).Hash -ne $suite.assemblySha256) {
            throw "Suite assembly changed: $($suite.project)"
        }
        Read-SourceIdentity $files[0].FullName
    }
    Read-SourceIdentity (Join-Path $batch 'artifacts/bin/VisionFramework/release/VisionFramework.dll')
    if ($DesktopDirectory) { Read-SourceIdentity (Join-Path $DesktopDirectory 'VisionFramework.dll') }
    foreach ($assembly in $AdditionalAssembly) { Read-SourceIdentity (Get-Item -LiteralPath $assembly).FullName }
)
if (!$OutputPath) { $OutputPath = Join-Path $batch 'source-identity-audit.json' }
[ordered]@{
    scope = 'Build identity and current repository source only; does not establish original VM parity or add test cases'
    auditorSha256 = (Get-FileHash -LiteralPath $PSCommandPath).Hash
    summarySha256 = (Get-FileHash -LiteralPath $summaryFile.FullName).Hash
    suites = $summary.passedSuites
    checks = $summary.passedChecks
    assemblies = $assemblies
} | ConvertTo-Json -Depth 7 | Set-Content -LiteralPath $OutputPath -Encoding utf8
Write-Output "Verified $($assemblies.Count) assembly/PDB pairs and $(($assemblies | ForEach-Object { $_.sources.Count } | Measure-Object -Sum).Sum) source documents. $OutputPath"
