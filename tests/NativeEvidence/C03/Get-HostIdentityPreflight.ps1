#Requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Image,
    [string]$Solution,
    [string]$InstallRoot = 'C:/Program Files/VisionMaster4.4.0',
    [string]$OutputRoot = (Join-Path $PSScriptRoot '../../../analysis/full-audit/batch1-20261107/C03')
)

$ErrorActionPreference = 'Stop'
$baselinePath = Join-Path $PSScriptRoot 'sdk-region-baseline.json'
$baseline = Get-Content -Raw -LiteralPath $baselinePath | ConvertFrom-Json -AsHashtable
$blobDir = 'Applications/Module(sp)/x64/Location/IMVSBlobFindModu'
$hostSpec = @(
    @{ path = 'Applications/VisionMaster.exe' }
    @{ path = 'Applications/ServerApp/vServerApp.exe' }
    @{ path = 'Applications/myLibs/VM.Core.dll' }
    @{ path = 'Applications/myLibs/Apps.XmlParser.dll' }
    @{ path = 'Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll'; archived = 'B62D7E15588CCDA6671228AEBC11FEC3E27ACA878F0E03067E44B17D5A54A9A1' }
    @{ path = 'Applications/Server/PlatformManager.dll'; archived = '06D86F51BEBFEBA357D1B79297E20A3B0A70580A40AF7B77C8ABDD6C38CA0624' }
    @{ path = 'Applications/ModuleProxy/x64/ModuleFrame.dll'; archived = '45C57BC46F72FD1F9E0B3A6F9183FF974D14590CFD05ECF4E9FA97A4FF95AE9B' }
    @{ path = 'Applications/ModuleProxy/x64/ModuleProxy.dll'; archived = '76F09A40184B2B5F99B7173EDDE23A716D339E3C64955696D6927495EFDF68A9' }
    @{ path = "$blobDir/IMVSBlobFindModu.dll"; archived = '4DE695D8C3051045C9010739686F94C32D66A36EE486207FE749FB183FFF4C9A' }
    @{ path = "$blobDir/IMVSBlobFindModuAlgorithmTab.xml" }
)

function Get-FileIdentity([string]$Path, [string]$ReferenceHash, [string]$ReferenceScope) {
    $identity = [ordered]@{
        path = $null
        bytes = $null
        sha256 = $null
        fileVersion = $null
        referenceSha256 = $ReferenceHash
        referenceScope = $ReferenceScope
        status = 'missing'
    }
    if ($Path) {
        $identity.path = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($Path)
        if (Test-Path -LiteralPath $identity.path -PathType Leaf) {
            $file = Get-Item -LiteralPath $identity.path
            $identity.bytes = $file.Length
            $identity.sha256 = (Get-FileHash -LiteralPath $file.FullName).Hash
            if ($file.Extension -in '.exe', '.dll') { $identity.fileVersion = $file.VersionInfo.FileVersion }
            $identity.status = if (!$ReferenceHash) { 'reference-unavailable' }
                elseif ($identity.sha256 -eq $ReferenceHash) { 'match' } else { 'mismatch' }
        }
    }
    [pscustomobject]$identity
}

$hostFiles = foreach ($file in $hostSpec) {
    $scope = if ($file.archived) { 'archived original reverse-engineering artifact' }
        else { 'historical run hash unavailable' }
    Get-FileIdentity (Join-Path $InstallRoot $file.path) $file.archived $scope
}
$sdkFiles = foreach ($name in $baseline.runtimeDlls.Keys) {
    $path = Join-Path $InstallRoot "MVDAlgorithmSDK/Runtime/x64/$name"
    Get-FileIdentity $path $baseline.runtimeDlls[$name] 'direct SDK baseline, installed copy'
}
$imageRecord = Get-FileIdentity $Image $baseline.inputSha256 'archived direct SDK input'
$solutionRecord = Get-FileIdentity $Solution '' 'historical test1108.sol hash unavailable'
$hostMissing = @($hostFiles | Where-Object status -eq 'missing')
$hostMismatches = @($hostFiles | Where-Object status -eq 'mismatch')
$sdkMismatches = @($sdkFiles | Where-Object status -ne 'match')

$report = [ordered]@{
    scope = 'Read-only installed-file inventory. Runtime-loaded modules and host solution execution require independent evidence.'
    capturedUtc = [DateTime]::UtcNow.ToString('o')
    directSdkBaselineSha256 = (Get-FileHash -LiteralPath $baselinePath).Hash
    image = $imageRecord
    solution = $solutionRecord
    hostFiles = @($hostFiles)
    directSdkFiles = @($sdkFiles)
    hostMissing = $hostMissing.Count
    hostArchiveMismatches = $hostMismatches.Count
    directSdkPreflightPassed = $imageRecord.status -eq 'match' -and $sdkMismatches.Count -eq 0
}
$out = New-Item -ItemType Directory -Path (Join-Path $OutputRoot "host-identity-$([Guid]::NewGuid().ToString('N'))") -Force
$summary = Join-Path $out.FullName 'summary.json'
$report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $summary -Encoding utf8
Write-Output $summary
