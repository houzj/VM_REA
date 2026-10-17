#Requires -Version 7.0
param([string]$Install='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=[guid]::NewGuid().ToString('N')
$relative="analysis/full-audit/n-image-solver/run-$run"
$output=Join-Path $workspace $relative
New-Item -ItemType Directory -Path $output | Out-Null
$alias=Join-Path $env:TEMP "n-image-solver-$run"
New-Item -ItemType Junction -Path $alias -Target $workspace | Out-Null
$runtime=Join-Path $Install 'MVDAlgorithmSDK/Runtime/x64'
$algorithm=Join-Path $runtime 'MVD_Algorithm.dll'
if((Get-FileHash -LiteralPath $algorithm).Hash -ne '85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549'){throw 'Original core identity changed; internal function offsets cannot be reused'}
$exe=Join-Path $alias "$relative/OriginalNImageSolverProbe.exe"
$source=Join-Path $alias 'tests/NativeEvidence/OriginalNImageSolverProbe.cpp'
$build=Join-Path $output build.cmd
@"
@echo off
call "C:/Program Files/Microsoft Visual Studio/18/Community/VC/Auxiliary/Build/vcvars64.bat" >nul
cl /nologo /EHsc /std:c++17 /I"$Install/MVDAlgorithmSDK/Includes/Algorithms" /I"$Install/MVDAlgorithmSDK/Includes/Common/VisionDesigner" /Fo"$alias/$relative/probe.obj" /Fe"$exe" "$source"
"@ | Set-Content -LiteralPath $build -Encoding ascii
& cmd.exe /c $build *> (Join-Path $output build.log)
if($LASTEXITCODE -ne 0){throw "Build failed: $output"}
@($algorithm,$source,$exe) | ForEach-Object { @{path=[IO.Path]::GetFullPath($_);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()} } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output provenance.json)
$taskPath=$env:PATH
try{
 $env:PATH="$runtime;$(Join-Path $Install 'Applications/PublicFile/x64');$(Join-Path $Install 'Applications/ModuleProxy/x64');$taskPath"
 $arguments=@($runtime,(Join-Path $alias "analysis/full-audit/n-image-calibration/solver-fixture/frame")) | ForEach-Object {'"'+$_+'"'}
 $process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output probe.jsonl) -RedirectStandardError (Join-Path $output stderr.log)
 if(!$process.WaitForExit(60000)){$process.Kill();throw "Owned probe timed out: $output"}
 $process.Refresh()
 @{exitCode=$process.ExitCode;scope='Official NImage SDK with synthetic translations and rotations; no devices'} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output process.json)
 if($process.ExitCode -ne 0){throw "Owned probe exit $($process.ExitCode): $output"}
}finally{$env:PATH=$taskPath}
Get-Content (Join-Path $output probe.jsonl) | ForEach-Object {$_ | ConvertFrom-Json} | Select-Object phase,board,count,code
Write-Output $output
