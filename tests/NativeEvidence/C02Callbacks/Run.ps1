param(
 [string]$LibraryRoot='D:\Program Files\VisionMaster4.4.0\Applications\myLibs',
 [string]$OutputRoot=(Join-Path $PSScriptRoot '..\..\..\analysis\full-audit\batch4-20261019\C02')
)
$ErrorActionPreference='Stop'
$OutputRoot=Join-Path ([IO.Path]::GetFullPath($OutputRoot)) ('run-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
$framework=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319'
$exe=Join-Path $OutputRoot 'ExportCallbacksCil.exe'
& (Join-Path $framework 'csc.exe') /nologo /target:exe "/out:$exe" /r:System.Web.Extensions.dll (Join-Path $PSScriptRoot 'ExportCallbacksCil.cs')
if($LASTEXITCODE -ne 0){throw 'Metadata exporter compilation failed'}
$integrity=@()
foreach($name in @('VMControls.WPF.dll','VM.Core.dll','VM.PlatformSDKCS.dll')) {
 $targetRoot=$LibraryRoot
 if($name -eq 'VM.PlatformSDKCS.dll'){$targetRoot=[IO.Path]::GetFullPath((Join-Path $LibraryRoot '../PublicFile/x64'))}
 $dll=Join-Path $targetRoot $name
 $before=Get-FileHash -LiteralPath $dll -Algorithm SHA256
 & $exe $targetRoot $OutputRoot $name *> (Join-Path $OutputRoot ($name+'.stdout-stderr.log'))
 $code=$LASTEXITCODE
 $after=Get-FileHash -LiteralPath $dll -Algorithm SHA256
 $integrity += [pscustomobject]@{Path=$dll;BeforeSHA256=$before.Hash;AfterSHA256=$after.Hash;Equal=($before.Hash -eq $after.Hash)}
 if($before.Hash -ne $after.Hash){throw 'Original assembly hash changed'}
 Get-Content (Join-Path $OutputRoot ($name+'.stdout-stderr.log'))
 if($code -ne 0){throw "Metadata export failed: $name $code"}
}
$integrity | ConvertTo-Json | Set-Content (Join-Path $OutputRoot 'artifact-integrity.json') -Encoding utf8
Get-ChildItem -LiteralPath $OutputRoot -File | Where-Object Name -ne 'output-sha256.csv' | Get-FileHash -Algorithm SHA256 | Export-Csv (Join-Path $OutputRoot 'output-sha256.csv') -NoTypeInformation -Encoding utf8



