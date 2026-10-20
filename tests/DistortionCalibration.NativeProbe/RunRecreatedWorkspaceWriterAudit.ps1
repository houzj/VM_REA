param([string]$NativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis',[string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'

$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run='analysis/distortion-calibration/recreated-tool-workspace/writer-source'
if(!(Test-Path "$root/analysis/distortion-calibration/core-project/nimage-core.gpr")){throw 'Existing private core project is required; use the established core evidence setup first.'}
New-Item -ItemType Directory -Force "$root/$run/project","$root/$run/settings","$root/$run/cache","$root/$run/temp","$root/$run/output" | Out-Null
Write-Output "Owned read-only writer audit: $root/$run/output"

$alias=Join-Path $env:TEMP ('board-audit-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $alias | Out-Null
New-Item -ItemType Junction -Path "$alias/tools" -Target $NativeTools | Out-Null
New-Item -ItemType Junction -Path "$alias/work" -Target $root | Out-Null
New-Item -ItemType Junction -Path "$alias/target" -Target "$InstallRoot/MVDAlgorithmSDK/Runtime/x64" | Out-Null
$oldJavaHome=$env:JAVA_HOME;$oldJavaOptions=$env:JAVA_TOOL_OPTIONS
try {
 $env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
 $env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/$run/settings -Dapplication.cachedir=$alias/work/$run/cache -Dapplication.tempdir=$alias/work/$run/temp"
 $ErrorActionPreference="Continue"
 & "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/distortion-calibration/core-project" nimage-core -process MVD_Algorithm.dll -noanalysis -readOnly -scriptPath "$alias/work/tests/DistortionCalibration.NativeProbe" -postScript DistortionWorkspaceWriterAudit.java "$alias/work/$run/output" -analysisTimeoutPerFile 60 *> "$root/$run/headless.log"
 $nativeExit=$LASTEXITCODE
 $ErrorActionPreference="Stop"
 if($nativeExit -ne 0 -or !(Get-ChildItem "$root/$run/output" -Filter '*.c')){throw 'Owned native decompilation produced no validated source; inspect headless.log'}
}finally{$env:JAVA_HOME=$oldJavaHome;$env:JAVA_TOOL_OPTIONS=$oldJavaOptions}







