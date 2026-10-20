param([ValidateSet('platform','manager','proxy','frame','camera')][string]$Project='manager',[string[]]$Entry=@('PF_M_MsgDeal'))
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$native='D:/MyWork/REA逆向测试/.tools/native-analysis'
$link=Join-Path $env:TEMP ('camera-b3-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $link|Out-Null
New-Item -ItemType Junction "$link/work" -Target $root|Out-Null
New-Item -ItemType Junction "$link/tools" -Target $native|Out-Null
$task="$link/work/analysis/camera-mapping/b3-reset-source-20261019"
$env:JAVA_HOME="$link/tools/jdk-21.0.12.1+1"
# All mutable Ghidra state belongs to this line, never global user settings.
$env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$task/settings -Dapplication.cachedir=$task/cache -Dapplication.tempdir=$task/temp"
$binary=if($Project -eq 'manager'){'PlatformManager.dll'}elseif($Project -eq 'proxy'){'ModuleProxy.dll'}elseif($Project -eq 'frame'){'ModuleFrame.dll'}elseif($Project -eq 'camera'){'IMVSCameraMapModu.dll'}else{'iMVS-6000PlatformSDK.dll'}
& "$link/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$task/project" $Project -process $binary -noanalysis -scriptPath "$link/work/tests/CameraMapping.NativeProbe" -postScript B3ResetSourceAudit.java "$task/$Project" @Entry -log "$task/$Project-replay.log"
exit $LASTEXITCODE


