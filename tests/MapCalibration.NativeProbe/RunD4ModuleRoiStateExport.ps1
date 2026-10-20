param([string[]]$Addresses=@('180015660','180015830','18001f280','18001f390','18001fae0','1800203d0','180021340','180021690','180021c50','180022d50','180022ec0','180022f40','180023870'),[string]$EvidenceDirectory='analysis/map-calibration/d4-module-roi-state')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$evidencePath=Join-Path $root $EvidenceDirectory
New-Item -ItemType Directory -Force $evidencePath|Out-Null
$alias=Join-Path $env:TEMP ('map-d1-'+[Guid]::NewGuid().ToString('N'))
$originalCheckout=Split-Path (Split-Path $root -Parent) -Parent
$nativeTools=Join-Path $originalCheckout '.tools\native-analysis'
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/tools" -Target $nativeTools|Out-Null
$env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
$env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/$EvidenceDirectory/settings -Dapplication.cachedir=$alias/work/$EvidenceDirectory/cache -Dapplication.tempdir=$alias/work/$EvidenceDirectory/temp"
$previousErrorActionPreference=$ErrorActionPreference
$exportScript='D4ModuleRoiStateAudit.java'
$ErrorActionPreference='Continue' # Ghidra emits JAVA_TOOL_OPTIONS on stderr even when it succeeds.
& "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/map-calibration/ghidra/project" MapCalibmodule -process IMVSMapCalibModu.dll -noanalysis -readOnly -scriptPath "$alias/work/tests/MapCalibration.NativeProbe" -postScript $exportScript "$alias/work/$EvidenceDirectory/source" @Addresses -log "$alias/work/$EvidenceDirectory/headless.log" 2>&1|Set-Content "$evidencePath/console.log"
$ghidraExitCode=$LASTEXITCODE
$ErrorActionPreference=$previousErrorActionPreference
if($ghidraExitCode -ne 0){throw "Core CLI failed with exit code $ghidraExitCode"}
foreach($address in $Addresses){if(-not(Test-Path "$evidencePath/source/$address.c")){throw "Missing core output $address"}}





