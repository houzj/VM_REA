param([string[]]$Addresses=@('1807d3e90'))
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$alias=Join-Path $env:TEMP ('map-core-'+[Guid]::NewGuid().ToString('N'))
$originalCheckout=Split-Path (Split-Path $root -Parent) -Parent
$nativeTools=Join-Path $originalCheckout '.tools\native-analysis'
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/tools" -Target $nativeTools|Out-Null
$env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
$env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/analysis/map-calibration/ghidra/core-settings -Dapplication.cachedir=$alias/work/analysis/map-calibration/ghidra/core-cache -Dapplication.tempdir=$alias/work/analysis/map-calibration/ghidra/core-temp"
$previousErrorActionPreference=$ErrorActionPreference
$ErrorActionPreference='Continue' # Ghidra emits JAVA_TOOL_OPTIONS on stderr even when it succeeds.
& "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/map-calibration/ghidra/core-project" core -process MVD_Algorithm.dll -noanalysis -readOnly -scriptPath "$alias/work/tests/MapCalibration.NativeProbe" -postScript MapCalibrationAudit.java "$alias/work/analysis/map-calibration/ghidra/core" @Addresses -log "$alias/work/analysis/map-calibration/ghidra/core.log" 2>&1|Set-Content "$root/analysis/map-calibration/ghidra/core-console.log"
$ghidraExitCode=$LASTEXITCODE
$ErrorActionPreference=$previousErrorActionPreference
if($ghidraExitCode -ne 0){throw "Core CLI failed with exit code $ghidraExitCode"}
foreach($address in $Addresses){if(-not(Test-Path "$root/analysis/map-calibration/ghidra/core/$address.c")){throw "Missing core output $address"}}
