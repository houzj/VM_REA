param([string]$VendorDirectory='D:/Program Files/VisionMaster4.4.0/Applications/myLibs')
$ErrorActionPreference='Stop'
$repo=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$run=Join-Path $repo ('analysis/full-audit/batch3-20261019/C02/run-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $run | Out-Null
$originals=@((Join-Path (Split-Path $VendorDirectory) 'VisionMaster.cfg'),(Join-Path (Split-Path $VendorDirectory) 'VisionMaster.exe.config'),(Join-Path $VendorDirectory 'VM.Utility.dll'))
$before=@($originals | Get-FileHash)
$before | Export-Csv (Join-Path $run 'original-before.csv') -NoTypeInformation
foreach($name in @('VM.Utility.dll','Apps.Log.dll','log4net.dll')) {$path=Join-Path $VendorDirectory $name;if(Test-Path -LiteralPath $path){Copy-Item -LiteralPath $path -Destination $run}}
$exe=Join-Path $run 'Probe.exe'
& "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe" /nologo /r:System.Xml.Linq.dll "/out:$exe" (Join-Path $PSScriptRoot 'Probe.cs') *> (Join-Path $run 'compile.log')
if($LASTEXITCODE -ne 0){throw "Compile failed: $run"}
$config='<configuration><appSettings><add key="ApplicationName" value="Probe"/><add key="Flag" value="old"/></appSettings></configuration>'
Set-Content ($exe+'.config') $config
# Executable base differs from CWD: string save must stay at base, cfg saves at CWD.
$cwd=Join-Path $run 'working'
New-Item -ItemType Directory -Path $cwd | Out-Null
Set-Content (Join-Path $cwd 'Probe.cfg') '<Config><IsClearLastExecuteResult><CurValue>0</CurValue></IsClearLastExecuteResult></Config>'
Push-Location $cwd
try {
 foreach($mode in @('write','restart','duplicate')) {
  if($mode -eq 'duplicate') {
   Set-Content 'Probe.cfg' '<Config><IsClearLastExecuteResult><CurValue>0</CurValue></IsClearLastExecuteResult><IsClearLastExecuteResult><CurValue>1</CurValue></IsClearLastExecuteResult></Config>'
   Set-Content ($exe+'.config') '<configuration><appSettings><add key="ApplicationName" value="Probe"/><add key="Flag" value="old"/><add key="Flag" value="other"/></appSettings></configuration>'
  }
  & $exe $mode *> (Join-Path $run ($mode+'.log'))
  if($LASTEXITCODE -ne 0){throw "Probe failed: $mode ; $run"}
  Get-Content (Join-Path $run ($mode+'.log')) | Where-Object {$_ -like 'PASS *' -or $_ -like 'assertions=*'}
 }
} finally {Pop-Location}
$after=@($originals | Get-FileHash)
$after | Export-Csv (Join-Path $run 'original-after.csv') -NoTypeInformation
for($i=0;$i -lt $before.Count;$i++){if($before[$i].Hash -ne $after[$i].Hash){throw 'Original changed'}}
$loaded=@(Get-ChildItem -LiteralPath $run -Filter '*.log' | Get-Content | Where-Object {$_ -like 'loaded=*'} | ForEach-Object {($_ -split '\|',2)[1]} | Sort-Object -Unique)
$loaded | Get-FileHash | Export-Csv (Join-Path $run 'loaded-hashes.csv') -NoTypeInformation
$files=@(Get-ChildItem -LiteralPath $run -File -Recurse)
$files | Get-FileHash | Export-Csv (Join-Path $run 'artifact-hashes.csv') -NoTypeInformation
Write-Output "Evidence directory: $run"
