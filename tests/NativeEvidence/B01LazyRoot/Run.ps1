param([string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$output=Join-Path $workspace ('analysis/full-audit/batch4-20261019/B01/run-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $output | Out-Null
$csc='C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe'
$paths=@((Join-Path $Applications 'myLibs/Apps.XmlParser.dll'),(Join-Path $Applications 'myLibs/VM.Core.dll'),(Join-Path $Applications 'myLibs/VM.Util.dll'),(Join-Path $Applications 'myLibs/VM.Utility.dll'))+@(Get-ChildItem -LiteralPath $PSScriptRoot -File | ForEach-Object FullName)
function Save-Hashes($name){$paths | ForEach-Object {@{path=$_;sha256=(Get-FileHash -LiteralPath $_).Hash}} | ConvertTo-Json | Set-Content (Join-Path $output $name)}
Save-Hashes 'input-hashes-before.json'
$export=Join-Path $output 'ExportLazyCil.exe'
& $csc /nologo /target:exe /r:System.Web.Extensions.dll "/out:$export" (Join-Path $PSScriptRoot 'ExportLazyCil.cs')
if($LASTEXITCODE -ne 0){throw 'Metadata exporter compile failed'}
foreach($dll in @('Apps.XmlParser.dll','VM.Core.dll')){
 $argsExport=@((Join-Path $Applications 'myLibs'),$output,$dll) | ForEach-Object {'"'+$_+'"'}
 $ep=Start-Process -FilePath $export -ArgumentList $argsExport -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output ($dll+'.export.log')) -RedirectStandardError (Join-Path $output ($dll+'.export.stderr.log'))
 $ownedExportHandle=$ep.Handle
 if(!$ep.WaitForExit(45000)){$ep.Kill();throw 'Owned metadata export timeout'}
 $ep.Refresh()
 if($ep.ExitCode -ne 0){throw "Metadata export failed: $dll exit $($ep.ExitCode), $output"}
}
$exe=Join-Path $output 'LazyRootProbe.exe'
& $csc /nologo /target:exe /platform:x64 /r:System.Core.dll /r:System.Web.Extensions.dll "/out:$exe" (Join-Path $PSScriptRoot 'LazyRootProbe.cs')
if($LASTEXITCODE -ne 0){throw 'Probe compile failed'}
$arguments=@($Applications,$output) | ForEach-Object {'"'+$_+'"'}
$process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output 'stdout.log') -RedirectStandardError (Join-Path $output 'stderr.log')
$ownedHandle=$process.Handle
if(!$process.WaitForExit(45000)){$process.Kill();throw "Owned probe timeout: $output"}
$process.Refresh()
@{exitCode=$process.ExitCode;pid=$process.Id;output=$output} | ConvertTo-Json | Set-Content (Join-Path $output 'process.json')
Save-Hashes 'input-hashes-after.json'
Write-Output $output
if($process.ExitCode -ne 0){throw "Probe failed: $($process.ExitCode)"}




