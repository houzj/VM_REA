param([string]$NativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis',[string]$InstallRoot='D:/Program Files/VisionMaster4.4.0',[string[]]$Jobs=@('1807f7050','1807f7450','1807f8590','stop'),[string]$JobList='')
$ErrorActionPreference='Stop'
if($JobList){$Jobs=$JobList.Split(',')+@('stop')}
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run='analysis/distortion-calibration/c3-file-source/module'
if(!(Test-Path "$root/analysis/distortion-calibration/core-project/nimage-core.gpr")){
 New-Item -ItemType Directory -Force "$root/analysis/distortion-calibration/core-project"|Out-Null
 Copy-Item -LiteralPath "$root/analysis/calibration-board/core-project/nimage-core.gpr" -Destination "$root/analysis/distortion-calibration/core-project/nimage-core.gpr"
 Copy-Item -LiteralPath "$root/analysis/calibration-board/core-project/nimage-core.rep" -Destination "$root/analysis/distortion-calibration/core-project/nimage-core.rep" -Recurse
}
New-Item -ItemType Directory -Force "$root/$run/project","$root/$run/settings","$root/$run/cache","$root/$run/temp","$root/$run/output" | Out-Null
Write-Output "Owned read-only decompile queue: $root/$run/output/jobs.txt"
if($Jobs.Count){Set-Content -LiteralPath "$root/$run/output/jobs.txt" -Value $Jobs}
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
 & "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/distortion-calibration/ghidra/project" Distortioncd3ce30e84574209a65b4374238381e6 -process IMVSImageCalibModu.dll -noanalysis -readOnly -scriptPath "$alias/work/tests/DistortionCalibration.NativeProbe" -postScript DistortionCoreInteractive.java "$alias/work/$run/output" -analysisTimeoutPerFile 60 *> "$root/$run/headless.log"
 $nativeExit=$LASTEXITCODE
 $ErrorActionPreference="Stop"
 if($nativeExit -ne 0 -or !(Test-Path "$root/$run/output/ready.txt") -or !(Get-ChildItem "$root/$run/output" -Filter '*.c')){throw 'Owned native decompilation produced no validated source; inspect headless.log'}
}finally{$env:JAVA_HOME=$oldJavaHome;$env:JAVA_TOOL_OPTIONS=$oldJavaOptions}








