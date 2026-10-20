param([switch]$Wrapper,[string[]]$Addresses=@())
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/map-calibration/ghidra'
New-Item -ItemType Directory -Force "$out/project"|Out-Null
$alias=Join-Path $env:TEMP ('map-calib-ghidra-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/tools" -Target 'D:/MyWork/REA逆向测试/.tools/native-analysis'|Out-Null
New-Item -ItemType Junction "$alias/module" -Target 'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu'|Out-Null
$target=if($Wrapper){"wrapper"}else{"module"}
$binary=if($Wrapper){"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDMapCalibCpp.dll"}else{"$alias/module/IMVSMapCalibModu.dll"}
$env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
$env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/analysis/map-calibration/ghidra/settings -Dapplication.cachedir=$alias/work/analysis/map-calibration/ghidra/cache -Dapplication.tempdir=$alias/work/analysis/map-calibration/ghidra/temp"
& "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/map-calibration/ghidra/project" ("MapCalib"+$target) -import $binary -scriptPath "$alias/work/tests/MapCalibration.NativeProbe" -postScript MapCalibrationAudit.java "$alias/work/analysis/map-calibration/ghidra/$target" @Addresses -loader-loadExternalLibs false -noanalysis -log "$alias/work/analysis/map-calibration/ghidra/$target.log" 2>&1|Set-Content "$out/$target-console.log"
if($LASTEXITCODE -ne 0){throw 'Ghidra CLI failed'}
if(-not(Test-Path "$out/$target/functions.tsv")){throw 'Ghidra postscript did not execute'}






