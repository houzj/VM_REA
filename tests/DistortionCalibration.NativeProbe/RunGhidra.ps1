param([ValidateSet('wrapper','core','module')][string]$Target='wrapper',[string[]]$Addresses=@(),[string]$ProjectName='')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$nativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis'
$output=Join-Path $root 'analysis/distortion-calibration/ghidra'
New-Item -ItemType Directory -Force "$output/project"|Out-Null
$alias=Join-Path $env:TEMP ('distortion-ghidra-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/tools" -Target $nativeTools|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
$env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
$env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/analysis/distortion-calibration/ghidra/settings-$Target -Dapplication.cachedir=$alias/work/analysis/distortion-calibration/ghidra/cache-$Target -Dapplication.tempdir=$alias/work/analysis/distortion-calibration/ghidra/temp-$Target"
$dll=if($Target -eq 'wrapper'){'MVDImageCalibCpp.dll'}elseif($Target -eq 'module'){'IMVSImageCalibModu.dll'}else{'MVD_Algorithm.dll'}
if($Target -eq 'module'){New-Item -ItemType Junction "$alias/module" -Target 'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSImageCalibModu'|Out-Null}
$binary=if($Target -eq 'module'){"$alias/module/IMVSImageCalibModu.dll"}else{"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/$dll"}
$ErrorActionPreference="Continue"
$importOrProcess=if($ProjectName){@($ProjectName,'-process',$dll,'-noanalysis','-readOnly')}else{@(('Distortion'+[Guid]::NewGuid().ToString('N')),'-import',$binary)}
& "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/distortion-calibration/ghidra/project" @importOrProcess -scriptPath "$alias/work/tests/DistortionCalibration.NativeProbe" -postScript DistortionAudit.java "$alias/work/analysis/distortion-calibration/ghidra/$Target" @Addresses -analysisTimeoutPerFile 120 -log "$alias/work/analysis/distortion-calibration/ghidra/$Target.log" 2>&1|Set-Content "$output/$Target-console.log"
$nativeExit=$LASTEXITCODE
$ErrorActionPreference="Stop"
if($nativeExit -ne 0){throw 'Ghidra CLI failed; inspect retained log.'}
foreach($address in $Addresses){if(-not(Test-Path -LiteralPath "$output/$Target/$address.c")){throw "Ghidra postscript did not emit $address.c; inspect retained log even when CLI exit code is zero."}}
if(!(Test-Path "$output/$Target/functions.tsv")){throw "Postscript emitted no function inventory; CLI exit code alone is insufficient."}
