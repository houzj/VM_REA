#Requires -Version 7.0
param([string]$Install='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=[guid]::NewGuid().ToString('N')
$relative="analysis/rotate-calibration/typed-input-oracle/run-$run"
$output=Join-Path $workspace $relative
New-Item -ItemType Directory -Path $output | Out-Null
$alias=Join-Path $env:TEMP "rotate-calibration-state-$run"
New-Item -ItemType Junction -Path $alias -Target $workspace | Out-Null
$binary=Join-Path $Install 'Applications/Module(sp)/x64/Calibration/RotateCalibModu/RotateCalibModu.dll'
if((Get-FileHash -LiteralPath $binary).Hash -ne 'B08486B8481ED6D7774634B30B5CAE167F85B911EBF394122663BAB400A9BB41'){throw 'Original module identity changed'}
$runtime=Join-Path $Install 'MVDAlgorithmSDK/Runtime/x64'
$algorithm=Join-Path $runtime 'MVDNPointCalibCpp.dll'
$exe=Join-Path $alias "$relative/RotateTypedInputOracle.exe"
$source=Join-Path $alias 'tests/RotateCalibration.NativeProbe/RotateTypedInputOracle.cpp'
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
 $arguments=@($binary,(Join-Path $alias $relative)) | ForEach-Object {'"'+$_+'"'}
 $process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output probe.jsonl) -RedirectStandardError (Join-Path $output stderr.log)
 if(!$process.WaitForExit(60000)){$process.Kill();throw "Owned probe timed out: $output"}
 $process.Refresh()
 @{exitCode=$process.ExitCode;scope='Original exported methods on owned instance; manual input; no Platform Init or devices'} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output process.json)
 if($process.ExitCode -ne 0){throw "Owned probe exit $($process.ExitCode): $output"}
}finally{$env:PATH=$taskPath}

Write-Output $output


