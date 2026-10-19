param([string]$LibraryRoot='D:/Program Files/VisionMaster4.4.0/Applications/myLibs')
$ErrorActionPreference='Stop'
$output=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot ('../../../analysis/full-audit/batch7-20261019/C02/static-'+[guid]::NewGuid().ToString('N'))))
New-Item -ItemType Directory -Path $output | Out-Null
& C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /r:System.Web.Extensions.dll "/out:$output/Export.exe" (Join-Path $PSScriptRoot 'ExportCil.cs')
if($LASTEXITCODE -ne 0){throw 'Compile failed'}
& "$output/Export.exe" $LibraryRoot $output VM.Core.dll
if($LASTEXITCODE -ne 0){throw 'Metadata export failed'}
$inputs=@(Get-ChildItem -LiteralPath $output -File)
$inputs | Get-FileHash | Export-Csv (Join-Path $output 'output-sha256.csv') -NoTypeInformation
Write-Output $output
