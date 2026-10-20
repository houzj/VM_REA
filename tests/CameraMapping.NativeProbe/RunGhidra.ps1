param([ValidateSet('wrapper','core','module','matrix')][string]$Target='wrapper',[string[]]$Addresses)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$nativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis'
$output=Join-Path $root 'analysis/camera-mapping/ghidra'
$addressesToUse=if($PSBoundParameters.ContainsKey('Addresses')){$Addresses}elseif($Target -eq 'matrix'){@()}else{@('180002620')}
New-Item -ItemType Directory -Force "$output/project"|Out-Null
$alias=Join-Path $env:TEMP ('camera-map-ghidra-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/tools" -Target $nativeTools|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
$env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
$env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/analysis/camera-mapping/ghidra/settings-$Target -Dapplication.cachedir=$alias/work/analysis/camera-mapping/ghidra/cache-$Target -Dapplication.tempdir=$alias/work/analysis/camera-mapping/ghidra/temp-$Target"
$dll=if($Target -eq 'wrapper'){'MVDCameraMapCpp.dll'}elseif($Target -eq 'matrix'){'MVDMatrixToParamCpp.dll'}else{'MVD_Algorithm.dll'}
if($Target -eq 'module'){New-Item -ItemType Junction "$alias/module" -Target 'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu'|Out-Null}
$binary=if($Target -eq 'module'){"$alias/module/IMVSCameraMapModu.dll"}else{"D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/$dll"}
& "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/camera-mapping/ghidra/project" ('CameraMap'+[Guid]::NewGuid().ToString('N')) -import $binary -scriptPath "$alias/work/tests/CameraMapping.NativeProbe" -postScript CameraMapAudit.java "$alias/work/analysis/camera-mapping/ghidra/$Target" @addressesToUse -analysisTimeoutPerFile 120 -deleteProject -log "$alias/work/analysis/camera-mapping/ghidra/$Target.log" 2>&1|Set-Content "$output/$Target-console.log"
if($LASTEXITCODE -ne 0){throw 'Ghidra CLI failed; inspect retained log.'}
foreach($address in $addressesToUse){if(-not(Test-Path -LiteralPath "$output/$Target/$address.c")){throw "Ghidra postscript did not emit $address.c; inspect retained log even when CLI exit code is zero."}}
