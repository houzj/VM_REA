param([switch]$Module,[string]$SdkRoot='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
& "$env:LOCALAPPDATA/Programs/Python/Python314/python.exe" "$PSScriptRoot/ExportEvidence.py" --cases-only
if($LASTEXITCODE -ne 0){throw "Case generation failed"}
$output=Join-Path $root 'analysis/camera-mapping/native'
New-Item -ItemType Directory -Force $output|Out-Null
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$alias=Join-Path $env:TEMP ('camera-map-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/sdk" -Target $SdkRoot|Out-Null
$probe=if($Module){"ModuleProbe"}else{"Probe"}
$cmd=@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/camera-mapping/native/probe.obj" /Fe"$alias/work/analysis/camera-mapping/native/probe.exe" "$alias/work/tests/CameraMapping.NativeProbe/$probe.cpp"
"@
[IO.File]::WriteAllText("$output/build.cmd",$cmd,[Text.Encoding]::ASCII)
& cmd /c "$output/build.cmd" 2>&1|Tee-Object "$output/build.log"
if($LASTEXITCODE -ne 0){throw 'Native probe build failed'}
$env:PATH="$SdkRoot/Runtime/x64;"+$env:PATH
if($Module){ & "$output/probe.exe" "D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu/IMVSCameraMapModu.dll" "$alias/work/analysis/camera-mapping/native/module.iwcal" 2>"$output/module-stderr.log" | Set-Content "$output/module-results.jsonl" -Encoding utf8 }else{ & "$output/probe.exe" "$SdkRoot/Runtime/x64/MVDCameraMapCpp.dll" "$output/cases.txt" 2>"$output/stderr.log"|Set-Content "$output/results.jsonl" -Encoding utf8 }
if($LASTEXITCODE -ne 0){throw "Native probe failed: $LASTEXITCODE"}
