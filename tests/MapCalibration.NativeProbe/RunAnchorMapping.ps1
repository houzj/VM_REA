$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/map-calibration/anchor-mapping-native'
New-Item -ItemType Directory -Force $out|Out-Null
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$alias=Join-Path $env:TEMP ('map-anchor-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/sdk" -Target 'D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK'|Out-Null
$cmd=@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /Fo"$alias/work/analysis/map-calibration/anchor-mapping-native/probe.obj" /Fe"$alias/work/analysis/map-calibration/anchor-mapping-native/probe.exe" "$alias/work/tests/MapCalibration.NativeProbe/AnchorMappingProbe.cpp"
"@
[IO.File]::WriteAllText("$out/build.cmd",$cmd,[Text.Encoding]::ASCII)
& cmd /c "$out/build.cmd" 2>&1|Tee-Object "$out/build.log"
if($LASTEXITCODE -ne 0){throw 'Native build failed'}
$env:PATH="$alias/sdk/Runtime/x64;"+$env:PATH
& "$out/probe.exe" "$alias/sdk/Runtime/x64" 2>"$out/stderr.log"|Set-Content "$out/results.jsonl" -Encoding utf8
@{exitCode=$LASTEXITCODE;binary=(Get-FileHash 'D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll').Hash;source=(Get-FileHash "$root/analysis/map-calibration/ghidra/core/180813020.c").Hash}|ConvertTo-Json|Set-Content "$out/process-status.json"
if($LASTEXITCODE -ne 0){throw "SDK probe failed: $LASTEXITCODE"}
