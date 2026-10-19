param([string]$OutputDirectory='analysis/full-audit/batch8-20261019/A03')
$ErrorActionPreference='Stop'
$out=[IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($out) | Out-Null
$runtime=Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319'
$original='D:/Program Files/VisionMaster4.4.0/Applications/myLibs'
$inputs=@((Join-Path $original 'Apps.BaseLog.dll'),(Join-Path $runtime 'System.Configuration.dll'),(Join-Path $runtime 'mscorlib.dll'))
$before=@($inputs | ForEach-Object {Get-FileHash -LiteralPath $_ -Algorithm SHA256} | Select-Object Path,Hash)
$exe=Join-Path $out 'ExportConfigCil.exe'
& (Join-Path $runtime 'csc.exe') /nologo /r:System.Web.Extensions.dll ("/out:"+$exe) (Join-Path $PSScriptRoot 'ExportConfigCil.cs') *> (Join-Path $out 'compile.log')
if($LASTEXITCODE -ne 0){throw 'Metadata exporter compilation failed'}
foreach($p in $inputs) {
 & $exe ([IO.Path]::GetDirectoryName($p)) $out ([IO.Path]::GetFileName($p)) *> (Join-Path $out ([IO.Path]::GetFileName($p)+'.export.log'))
 if($LASTEXITCODE -ne 0){throw 'Metadata export failed'}
}
$after=@($inputs | ForEach-Object {Get-FileHash -LiteralPath $_ -Algorithm SHA256} | Select-Object Path,Hash)
$same=($before | ConvertTo-Json -Compress) -ceq ($after | ConvertTo-Json -Compress)
@{before=$before;after=$after;unchanged=$same;targetExecution=$false;registryAccess=$false} | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $out 'artifact-integrity.json') -Encoding utf8
if(!$same){throw 'Input identity changed'}
