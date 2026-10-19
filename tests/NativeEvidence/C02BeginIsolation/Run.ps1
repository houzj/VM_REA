param([string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications')
$ErrorActionPreference='Stop'
$output=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot ('../../../analysis/full-audit/batch8-20261019/C02/static-'+[guid]::NewGuid().ToString('N'))))
New-Item -ItemType Directory -Path $output | Out-Null
& C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /r:System.Web.Extensions.dll "/out:$output/Export.exe" (Join-Path $PSScriptRoot 'ExportCil.cs')
if($LASTEXITCODE -ne 0){throw 'Compile failed'}
$artifacts=@((Join-Path $Applications 'myLibs/VM.Core.dll'),(Join-Path $Applications 'PublicFile/x64/VM.PlatformSDKCS.dll'))
$before=@($artifacts|ForEach-Object {Get-FileHash -LiteralPath $_})
& "$output/Export.exe" (Join-Path $Applications 'myLibs') $output VM.Core.dll
if($LASTEXITCODE -ne 0){throw 'Core metadata export failed'}
& "$output/Export.exe" (Join-Path $Applications 'PublicFile/x64') $output VM.PlatformSDKCS.dll
if($LASTEXITCODE -ne 0){throw 'SDK metadata export failed'}
$after=@($artifacts|ForEach-Object {Get-FileHash -LiteralPath $_})
@{before=$before;after=$after;targetExecution=$false}|ConvertTo-Json -Depth 5|Set-Content (Join-Path $output 'artifact-integrity.json')
$hashInputs=@(Get-ChildItem -LiteralPath $output -File|Where-Object Name -ne 'output-sha256.csv')
$hashInputs|ForEach-Object {Get-FileHash -LiteralPath $_.FullName}|Export-Csv (Join-Path $output 'output-sha256.csv') -NoTypeInformation
Write-Output $output

