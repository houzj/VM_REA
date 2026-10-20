param([string]$NativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis',[string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run='analysis/calibration-board/core-live-'+[guid]::NewGuid().ToString('N')
New-Item -ItemType Directory -Force "$root/$run/project","$root/$run/settings","$root/$run/cache","$root/$run/temp","$root/$run/output" | Out-Null
Write-Output "Owned read-only threshold source audit: $root/$run/output"
$boardCore=Join-Path $InstallRoot 'MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll'
if((Get-FileHash $boardCore).Hash.ToLowerInvariant() -ne '85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549'){throw 'Original core identity mismatch'}
$alias=Join-Path $env:TEMP ('board-audit-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $alias | Out-Null
New-Item -ItemType Junction -Path "$alias/tools" -Target $NativeTools | Out-Null
New-Item -ItemType Junction -Path "$alias/work" -Target $root | Out-Null
New-Item -ItemType Junction -Path "$alias/target" -Target "$InstallRoot/MVDAlgorithmSDK/Runtime/x64" | Out-Null
$oldJavaHome=$env:JAVA_HOME;$oldJavaOptions=$env:JAVA_TOOL_OPTIONS
try {
 $env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
 $env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/$run/settings -Dapplication.cachedir=$alias/work/$run/cache -Dapplication.tempdir=$alias/work/$run/temp"
 & "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/calibration-board/core-project" nimage-core -process MVD_Algorithm.dll -noanalysis -readOnly -scriptPath "$alias/work/tests/CalibrationBoard.NativeProbe" -postScript BoardThresholdSourceAudit.java "$alias/work/$run/output" -analysisTimeoutPerFile 60 *> "$root/$run/headless.log"
 if($LASTEXITCODE -ne 0 -or !(Test-Path "$root/$run/output/done.txt")){throw 'Owned native decompilation produced no validated source; inspect headless.log'}
}finally{$env:JAVA_HOME=$oldJavaHome;$env:JAVA_TOOL_OPTIONS=$oldJavaOptions}






