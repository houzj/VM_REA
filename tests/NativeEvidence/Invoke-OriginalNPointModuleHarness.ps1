#Requires -Version 7.0
param([string]$Install='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=[guid]::NewGuid().ToString('N')
$relative="analysis/full-audit/n-point-module/run-$run"
$output=Join-Path $workspace $relative
New-Item -ItemType Directory -Path $output | Out-Null
$alias=Join-Path $env:TEMP "n-point-module-$run"
New-Item -ItemType Junction -Path $alias -Target $workspace | Out-Null
$binary=Join-Path $Install 'Applications/Module(sp)/x64/Calibration/IMVSNPointCalibModu/IMVSNPointCalibModu.dll'
if((Get-FileHash -LiteralPath $binary).Hash -ne 'F8E07023A816C55FF07906E8889F45B93658F59CE88F040AD2204A70922F4839'){throw 'Original module identity changed'}
$runtime=Join-Path $Install 'MVDAlgorithmSDK/Runtime/x64'
$algorithm=Join-Path $runtime 'MVDNPointCalibCpp.dll'
$exe=Join-Path $alias "$relative/OriginalNPointModuleHarness.exe"
$source=Join-Path $alias 'tests/NativeEvidence/OriginalNPointModuleHarness.cpp'
$build=Join-Path $output build.cmd
@"
@echo off
call "C:/Program Files/Microsoft Visual Studio/18/Community/VC/Auxiliary/Build/vcvars64.bat" >nul
cl /nologo /EHsc /std:c++17 /I"$Install/MVDAlgorithmSDK/Includes/Algorithms" /I"$Install/MVDAlgorithmSDK/Includes/Common/VisionDesigner" /Fo"$alias/$relative/probe.obj" /Fe"$exe" "$source"
"@ | Set-Content -LiteralPath $build -Encoding ascii
& cmd.exe /c $build *> (Join-Path $output build.log)
if($LASTEXITCODE -ne 0){throw "Build failed: $output"}
@($binary,$algorithm,$source,$exe) | ForEach-Object { @{path=[IO.Path]::GetFullPath($_);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()} } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output provenance.json)
$taskPath=$env:PATH
try{
 $env:PATH="$runtime;$(Join-Path $Install 'Applications/PublicFile/x64');$(Join-Path $Install 'Applications/ModuleProxy/x64');$taskPath"
 $arguments=@($algorithm,$binary,(Join-Path $alias "$relative/module-calibration.xml")) | ForEach-Object {'"'+$_+'"'}
 $process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output probe.jsonl) -RedirectStandardError (Join-Path $output stderr.log)
 if(!$process.WaitForExit(60000)){$process.Kill();throw "Owned probe timed out: $output"}
 $process.Refresh()
 @{exitCode=$process.ExitCode;scope='Original exported methods on owned instance; manual input; no Platform Init or devices'} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output process.json)
 if($process.ExitCode -ne 0){throw "Owned probe exit $($process.ExitCode): $output"}
}finally{$env:PATH=$taskPath}
Get-Content (Join-Path $output probe.jsonl) | ForEach-Object {$_ | ConvertFrom-Json} | Where-Object rawMembers | Select-Object phase,code,@{N='current';E={$_.rawMembers.current}}
Write-Output $output
