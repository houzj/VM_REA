param([string[]]$Addresses=@('180001ab0','180001b70','180001c00','1800019f0'),[string]$EvidenceDirectory='analysis/map-calibration/d4-roi-ownership')
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
$exportScript='D4RoiOwnershipAudit.java'
$ErrorActionPreference='Continue' # Ghidra emits JAVA_TOOL_OPTIONS on stderr even when it succeeds.
& "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/map-calibration/ghidra/project" MapCalibwrapper -process MVDMapCalibCpp.dll -noanalysis -readOnly -scriptPath "$alias/work/tests/MapCalibration.NativeProbe" -postScript $exportScript "$alias/work/$EvidenceDirectory/source" @Addresses -log "$alias/work/$EvidenceDirectory/headless.log" 2>&1|Set-Content "$evidencePath/console.log"
$ghidraExitCode=$LASTEXITCODE
$ErrorActionPreference=$previousErrorActionPreference
if($ghidraExitCode -ne 0){throw "Core CLI failed with exit code $ghidraExitCode"}
foreach($address in $Addresses){if(-not(Test-Path "$evidencePath/source/$address.c")){throw "Missing core output $address"}}



