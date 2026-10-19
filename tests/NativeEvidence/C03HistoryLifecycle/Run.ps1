param([string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$output=Join-Path $root ('analysis/full-audit/batch9-20261019/C03/replay-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $output | Out-Null
$exe=Join-Path $output 'ExportHistoryCil.exe'
$source=Join-Path $PSScriptRoot 'ExportCil.cs'
& C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /r:System.Core.dll /r:System.Web.Extensions.dll "/out:$exe" $source
if($LASTEXITCODE -ne 0){throw 'Compile failed; no target code was run'}
$inputs=@('VM.Core.dll','VMControls.WPF.dll','../VisionMaster.exe')
$paths=$inputs | ForEach-Object {[IO.Path]::GetFullPath((Join-Path (Join-Path $Applications 'myLibs') $_))}
$before=$paths | ForEach-Object {Get-FileHash -LiteralPath $_ -Algorithm SHA256}
foreach($inputName in $inputs){
  & $exe (Join-Path $Applications 'myLibs') $output $inputName 2>&1 | Tee-Object -FilePath (Join-Path $output ([IO.Path]::GetFileName($inputName)+'.export.log'))
  if($LASTEXITCODE -ne 0){throw "Metadata export failed: $inputName"}
}
$after=$paths | ForEach-Object {Get-FileHash -LiteralPath $_ -Algorithm SHA256}
@{before=$before;after=$after;execution='own metadata reader only; target constructors and static methods never invoked'} | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $output 'input-hashes.json')
if(Compare-Object ($before.Hash) ($after.Hash)){throw 'Input hash mismatch'}
Get-ChildItem -LiteralPath $output -File | Where-Object Name -ne output-hashes.csv | ForEach-Object {Get-FileHash -LiteralPath $_ -Algorithm SHA256} | Export-Csv (Join-Path $output 'output-hashes.csv') -NoTypeInformation
Write-Output $output
