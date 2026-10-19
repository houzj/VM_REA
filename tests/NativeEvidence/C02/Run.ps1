param([string]$VendorDirectory='D:/Program Files/VisionMaster4.4.0/Applications/myLibs')
$ErrorActionPreference='Stop'
$repo=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$root=Join-Path $repo 'analysis/full-audit/batch2-20261019/C02'
$run=Join-Path $root ('run-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $run | Out-Null
foreach($name in @('VM.Utility.dll','VM.Util.dll','Apps.Log.dll','log4net.dll')) {
 $path=Join-Path $VendorDirectory $name
 if(Test-Path -LiteralPath $path){Copy-Item -LiteralPath $path -Destination $run}
}
$exe=Join-Path $run 'Probe.exe'
& "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe" /nologo "/out:$exe" (Join-Path $PSScriptRoot 'Probe.cs') *> (Join-Path $run 'compile.log')
if($LASTEXITCODE -ne 0){throw 'C02 compile failed'}
Set-Content -LiteralPath ($exe+'.config') -Value '<configuration><appSettings><add key="ApplicationName" value="Probe"/></appSettings></configuration>'
$original=Join-Path (Split-Path $VendorDirectory) 'VisionMaster.cfg'
$before=Get-FileHash -LiteralPath $original
$before | Export-Csv (Join-Path $run 'original-before.csv') -NoTypeInformation
Push-Location $run
try {
 foreach($mode in @('installed','one','missing','duplicate')) {
  if($mode -eq 'installed'){Copy-Item -LiteralPath $original -Destination 'Probe.cfg';$expected='0'}
  elseif($mode -eq 'one'){Set-Content 'Probe.cfg' '<Config><IsClearLastExecuteResult><CurValue>1</CurValue></IsClearLastExecuteResult></Config>';$expected='1'}
  elseif($mode -eq 'missing'){Set-Content 'Probe.cfg' '<Config/>';$expected='null'}
  else {Set-Content 'Probe.cfg' '<Config><IsClearLastExecuteResult><CurValue>1</CurValue></IsClearLastExecuteResult><IsClearLastExecuteResult><CurValue>0</CurValue></IsClearLastExecuteResult></Config>';$expected='null'}
  Copy-Item 'Probe.cfg' ($mode+'-input.cfg')
  & $exe $expected *> ($mode+'.log')
  if($LASTEXITCODE -ne 0){throw "C02 mode failed: $mode"}
  Get-Content ($mode+'.log') | Where-Object {$_ -notlike 'loaded=*'}
 }
} finally {Pop-Location}
if((Get-FileHash -LiteralPath $original).Hash -ne $before.Hash){throw 'Original configuration changed'}
$loaded=@(Get-ChildItem -LiteralPath $run -Filter '*.log' | Get-Content | Where-Object {$_ -like 'loaded=*'} | ForEach-Object {($_ -split '\|',2)[1]} | Sort-Object -Unique)
$loaded | Get-FileHash | Export-Csv (Join-Path $run 'loaded-hashes.csv') -NoTypeInformation
$hashes=@(Get-ChildItem -LiteralPath $run -File | Get-FileHash)
$hashes | Export-Csv (Join-Path $run 'artifact-hashes.csv') -NoTypeInformation
Write-Output "Evidence directory: $run"
