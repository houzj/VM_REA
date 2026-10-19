param(
 [string]$LibraryRoot='D:\Program Files\VisionMaster4.4.0\Applications\myLibs',
 [string]$OutputRoot=(Join-Path $PSScriptRoot '..\..\..\analysis\full-audit\batch2-20261019\A03')
)
$ErrorActionPreference='Stop'
$OutputRoot=[IO.Path]::GetFullPath($OutputRoot)
New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
$dll=Join-Path $LibraryRoot 'VMControls.WPF.dll'
$before=Get-FileHash -LiteralPath $dll -Algorithm SHA256
$framework=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319'
$exe=Join-Path $OutputRoot 'ExportTabsCil.exe'
& (Join-Path $framework 'csc.exe') /nologo /target:exe "/out:$exe" /r:System.Web.Extensions.dll (Join-Path $PSScriptRoot 'ExportTabsCil.cs')
if($LASTEXITCODE -ne 0){throw 'Metadata exporter compilation failed'}
& $exe $LibraryRoot $OutputRoot *> (Join-Path $OutputRoot 'stdout-stderr.log')
$code=$LASTEXITCODE
$after=Get-FileHash -LiteralPath $dll -Algorithm SHA256
if($before.Hash -ne $after.Hash){throw 'Original assembly hash changed'}
Get-Content (Join-Path $OutputRoot 'stdout-stderr.log')
if($code -ne 0){throw "Metadata export failed: $code"}
Get-ChildItem -LiteralPath $OutputRoot -File | Where-Object Name -ne 'output-sha256.csv' | Get-FileHash -Algorithm SHA256 | Export-Csv (Join-Path $OutputRoot 'output-sha256.csv') -NoTypeInformation -Encoding utf8
