param([ValidateSet('platform','manager','proxy','frame','camera')][string]$Project='manager',[string[]]$Entry=@('IMVS_SubscribeResultEx'))
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$link=Join-Path $env:TEMP ('camera-b3-point-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $link|Out-Null
New-Item -ItemType Junction "$link/work" -Target $root|Out-Null
New-Item -ItemType Junction "$link/tools" -Target 'D:/MyWork/REA逆向测试/.tools/native-analysis'|Out-Null
$task="$link/work/analysis/camera-mapping/b3-point-index-source-20261019"
New-Item -ItemType Directory $task -Force|Out-Null
$env:JAVA_HOME="$link/tools/jdk-21.0.12.1+1"
$env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$task/settings -Dapplication.cachedir=$task/cache -Dapplication.tempdir=$task/temp"
$binary=@{platform='iMVS-6000PlatformSDK.dll';manager='PlatformManager.dll';proxy='ModuleProxy.dll';frame='ModuleFrame.dll';camera='IMVSCameraMapModu.dll'}[$Project]
& "$link/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$link/work/analysis/camera-mapping/b3-reset-source-20261019/project" $Project -process $binary -noanalysis -scriptPath "$link/work/tests/CameraMapping.NativeProbe" -postScript B3PointSourceAudit.java "$task/$Project" @Entry -log "$task/$Project-replay.log"
exit $LASTEXITCODE

