param([string]$NativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis',[string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run='analysis/calibration-board/mask-sdk-decompile-rotation'
New-Item -ItemType Directory -Force "$root/$run/project","$root/$run/settings","$root/$run/cache","$root/$run/temp","$root/$run/output" | Out-Null
$alias=Join-Path $env:TEMP ('board-audit-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $alias | Out-Null
New-Item -ItemType Junction -Path "$alias/tools" -Target $NativeTools | Out-Null
New-Item -ItemType Junction -Path "$alias/work" -Target $root | Out-Null
New-Item -ItemType Junction -Path "$alias/target" -Target "$InstallRoot/MVDAlgorithmSDK/Runtime/x64" | Out-Null
$oldJavaHome=$env:JAVA_HOME;$oldJavaOptions=$env:JAVA_TOOL_OPTIONS
try {
 $env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
 $env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/$run/settings -Dapplication.cachedir=$alias/work/$run/cache -Dapplication.tempdir=$alias/work/$run/temp"
 & "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/$run/project" board -import "$alias/target/MVDPreproMaskCpp.dll" -scriptPath "$alias/work/tests/CalibrationBoard.NativeProbe" -postScript BoardMaskAudit.java "$alias/work/$run/output" -analysisTimeoutPerFile 60 -deleteProject *> "$root/$run/headless.log"
 if($LASTEXITCODE -ne 0 -or !(Test-Path "$root/$run/output/functions.tsv") -or !(Get-ChildItem "$root/$run/output" -Filter '*.c')){throw 'Owned native decompilation produced no validated source; inspect headless.log'}
}finally{$env:JAVA_HOME=$oldJavaHome;$env:JAVA_TOOL_OPTIONS=$oldJavaOptions}


