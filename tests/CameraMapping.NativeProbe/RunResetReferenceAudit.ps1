param([ValidateSet('module','platform')][string]$Target='module')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$nativeTools='D:/MyWork/REA逆向测试/.tools/native-analysis'
$alias=Join-Path $env:TEMP ('camera-map-reset-ghidra-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/tools" -Target $nativeTools|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
$env:JAVA_HOME="$alias/tools/jdk-21.0.12.1+1"
$out=Join-Path $root "analysis/camera-mapping/ghidra/$Target-reset-audit"
$binary=if($Target -eq 'module'){
 New-Item -ItemType Junction "$alias/module" -Target 'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu'|Out-Null
 "$alias/module/IMVSCameraMapModu.dll"
}else{'D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll'}
$addresses=if($Target -eq 'module'){@('18000caf0')}else{@('18064a2e0')}
$scriptArgs=@("$alias/work/analysis/camera-mapping/ghidra/$Target-reset-audit") + @($addresses | ForEach-Object {[string]$_})
$env:JAVA_TOOL_OPTIONS="-Dapplication.settingsdir=$alias/work/analysis/camera-mapping/ghidra/settings-$Target-reset -Dapplication.cachedir=$alias/work/analysis/camera-mapping/ghidra/cache-$Target-reset -Dapplication.tempdir=$alias/work/analysis/camera-mapping/ghidra/temp-$Target-reset"
New-Item -ItemType Directory -Force $out|Out-Null
& "$alias/tools/ghidra_12.1.4_PUBLIC/support/analyzeHeadless.bat" "$alias/work/analysis/camera-mapping/ghidra/project" ('CameraMapReset'+$Target+[Guid]::NewGuid().ToString('N')) -import $binary -scriptPath "$alias/work/tests/CameraMapping.NativeProbe" -postScript ResetReferenceAudit.java @scriptArgs -analysisTimeoutPerFile 150 -deleteProject -log "$out/headless.log" *> "$out/console.log"
if($LASTEXITCODE -ne 0){throw "Ghidra $Target audit failed; inspect $out"}
foreach($address in $addresses){if(!(Test-Path "$out/$address-refs.tsv")){throw "Ghidra did not emit reference results for $address"}}
Write-Output $out
