param([string]$NativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis',[string]$InstallRoot='D:/Program Files/VisionMaster4.4.0',[string[]]$Jobs=@())
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run='analysis/calibration-board/core-live-'+[guid]::NewGuid().ToString('N')
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
 & "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/calibration-board/core-project" nimage-core -process MVD_Algorithm.dll -noanalysis -readOnly -scriptPath "$alias/work/tests/CalibrationBoard.NativeProbe" -postScript BoardCoreInteractive.java "$alias/work/$run/output" -analysisTimeoutPerFile 60 *> "$root/$run/headless.log"
 $evidenceFiles=@(Get-ChildItem "$root/$run/output" -File | Where-Object {$_.Extension -in '.c','.asm','.bin','.tsv'})
 $exportErrors=@(Get-ChildItem "$root/$run/output" -Filter 'error-*.txt')
 if($LASTEXITCODE -ne 0 -or !(Test-Path "$root/$run/output/ready.txt") -or !(Test-Path "$root/$run/output/stopped.txt") -or !$evidenceFiles.Count -or $exportErrors.Count){throw 'Owned native source/instruction export failed validation; inspect headless.log and error files'}
}finally{$env:JAVA_HOME=$oldJavaHome;$env:JAVA_TOOL_OPTIONS=$oldJavaOptions}





