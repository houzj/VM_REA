param([string]$Validation='CM01 product/test prepared; managed build pending shared slot')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$dir=Join-Path $root 'analysis/camera-mapping/b3-input-lifecycle-source-20261019'
function Identity([string]$path){[ordered]@{path=$path;sha256=(Get-FileHash -LiteralPath $path).Hash}}
$identity=Get-Content -LiteralPath "$dir/camera/program-identity.txt"
$native=@(Get-ChildItem -LiteralPath "$dir/camera" -Filter '*.c'|ForEach-Object{
 [ordered]@{va=('0x'+$_.BaseName);rva=('0x{0:x}' -f ([Convert]::ToInt64($_.BaseName,16)-0x180000000));source=(Identity $_.FullName)}
})
$data=[ordered]@{
 provider='isolated Ghidra 12.1.4; existing identity-matched private camera project; original instructions; no new target runtime'
 binary=[ordered]@{path=$identity[0];sha256=$identity[1]}
 nativeBodies=$native
 instructionWindows=@(Get-ChildItem "$dir/*/*-linear.tsv"|ForEach-Object{Identity $_.FullName})
 managedMetadata=Identity "$dir/managed-metadata.json"
 managedSourceIndex=Identity "$root/docs/plugins/CAMERA-MAPPING-B3-POINT-INDEX.json"
 closureMatrix=Identity "$root/docs/plugins/CAMERA-MAPPING-B3-B4-CLOSURE-MATRIX.md"
 cm01Before=Get-Content -LiteralPath "$dir/cm01-before.json" -Raw|ConvertFrom-Json
 cm06Before=Get-Content -LiteralPath "$dir/cm06-before.json" -Raw|ConvertFrom-Json
 originalManagedSources=@(foreach($name in @('ValueBase','OpenFileForCalibDialog','SetModuleParamBindOperation','ButtonSelecter','RadioSelecter','AlgorithmTabRoot','Button_ResetAlgoParams','PointPair')){Identity "D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/$name.cs"})
 currentSources=@(foreach($name in @('CameraMappingPointPairControls.cs','CameraMappingPlugin.cs','CameraMappingInputReader.cs','CameraMappingConfiguration.cs')){Identity "$root/src/VisionPlugins.CameraMapping/$name"})
 currentTests=@(foreach($name in @('PluginChecks','InputChecks','ResetChecks')){Identity "$root/tests/CameraMapping.$name/Program.cs"})
 sharedReaderContract=Identity "$root/src/VisionPluginContracts/ToolNumericInputReader.cs"
 originalDefaults=Identity 'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu/IMVSCameraMapModuAlgorithm.xml'
 embeddedDefaults=Identity "$root/src/VisionPlugins.CameraMapping/Resources/CameraMappingAlgorithm.xml"
 automatedResults=@(foreach($name in @('verification/checks.json','input-verification/checks.json','cm0203-plugin-verification/checks.json','cm0203-24-runtime.json','reset-verification/checks.json','cm06-plugin-verification-fixed/checks.json')){Identity "$dir/$name"})
 validation=$Validation
 overallProgress=[ordered]@{completeTools=0;totalTools=10;note='root audits tool counts; human acceptance separately recorded and excluded'}
}
$target=Join-Path $root 'docs/plugins/CAMERA-MAPPING-B3-B4-CLOSURE-MATRIX.json'
$data|ConvertTo-Json -Depth 15|Set-Content -LiteralPath $target -Encoding utf8
$read=Get-Content -LiteralPath $target -Raw|ConvertFrom-Json
foreach($body in $read.nativeBodies){if((Get-FileHash -LiteralPath $body.source.path).Hash -ne $body.source.sha256){throw 'Native source hash mismatch'}}
[ordered]@{bodies=$native.Count;instructionWindows=$data.instructionWindows.Count;report=(Identity $target)}|ConvertTo-Json -Depth 5
