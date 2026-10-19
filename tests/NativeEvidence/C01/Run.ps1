param([string]$VendorDirectory='D:/Program Files/VisionMaster4.4.0/Applications/myLibs')
$ErrorActionPreference='Stop'
$repo=(Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$root=Join-Path $repo 'analysis/full-audit/batch1-20261019/C01'
$run=Join-Path $root ('run-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $run | Out-Null
Set-Content (Join-Path $run 'C01-owned-directory.marker') 'Exclusive C01 probe directory'
New-Item -ItemType Directory -Path (Join-Path $run 'TempFiles'),(Join-Path $run 'myLibs') | Out-Null
Set-Content (Join-Path $run 'TempFiles/owned-sentinel.txt') 'Owned C01 sentinel; original constructor may delete'
foreach($name in @('VM.UnRedo.dll','VMControls.Interface.dll','VM.Utility.dll','VM.Controls.dll')) {
 Copy-Item -LiteralPath (Join-Path $VendorDirectory $name) -Destination $run
}
$source=Join-Path $PSScriptRoot 'Probe.cs'
$exe=Join-Path $run 'Probe.exe'
& "$env:WINDIR/Microsoft.NET/Framework64/v4.0.30319/csc.exe" /nologo "/out:$exe" "/r:$run/VM.UnRedo.dll" "/r:$run/VMControls.Interface.dll" $source *> (Join-Path $run 'compile.log')
if($LASTEXITCODE -ne 0){throw "Probe compile failed; see $run"}
$paths=@($source,$exe)+@(Get-ChildItem -LiteralPath $run -Filter '*.dll' | ForEach-Object FullName)
$before=@($paths | Get-FileHash -Algorithm SHA256)
$before | Export-Csv (Join-Path $run 'hashes-before.csv') -NoTypeInformation
Push-Location $run
try { & $exe *> (Join-Path $run 'run.log'); $probeExit=$LASTEXITCODE } finally { Pop-Location }
$before | ForEach-Object { if((Get-FileHash -LiteralPath $_.Path).Hash -ne $_.Hash){throw "Artifact changed: $($_.Path)"} }
# CLR strong-name resolution may select GAC dependencies; hash what actually loaded.
Get-Content (Join-Path $run 'run.log') | Where-Object {$_ -like 'loaded=*'} | ForEach-Object {($_ -split '\|',2)[1]} | Get-FileHash | Export-Csv (Join-Path $run 'loaded-hashes.csv') -NoTypeInformation
Get-Content (Join-Path $run 'run.log')
Write-Output "Evidence directory: $run"
if($probeExit -ne 0){throw "Native service probe failed with exit $probeExit"}
