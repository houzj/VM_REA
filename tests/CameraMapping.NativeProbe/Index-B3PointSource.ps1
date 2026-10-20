$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$dir=Join-Path $root 'analysis/camera-mapping/b3-point-index-source-20261019'
function Identity([string]$path){[ordered]@{path=$path;sha256=(Get-FileHash -LiteralPath $path).Hash}}
$native=@(foreach($image in @('platform','manager','proxy','frame')){
 $identity=Get-Content -LiteralPath "$dir/$image/program-identity.txt"
 foreach($file in Get-ChildItem -LiteralPath "$dir/$image" -Filter '*.c'){
  if($file.BaseName -notmatch '^180[0-9a-f]+$'){throw "Unexpected body $file"}
  [ordered]@{image=$image;binaryPath=$identity[0];binarySha256=$identity[1];va=('0x'+$file.BaseName);rva=('0x{0:x}' -f ([Convert]::ToInt64($file.BaseName,16)-0x180000000));source=(Identity $file.FullName)}
 }
})
$instructions=@(Get-ChildItem -Path "$dir/*/*-linear.tsv"|ForEach-Object{Identity $_.FullName})
$managed=Get-Content -LiteralPath "$dir/managed-metadata.json" -Raw|ConvertFrom-Json
$resources=Get-Content -LiteralPath "$dir/resource-map.json" -Raw|ConvertFrom-Json
$report=[ordered]@{
 status='finite source slice; entire B3 S1/S2 remain open'
 scope='Data3.ObjectPoint -> ObjectInput3 -> ObjectPointX3/Y3 -> RelateIndex -> native subscription -> float selector'
 provider='passive PEReader/XML plus isolated Ghidra 12.1.4 decompilation and original instructions; no target runtime or product build'
 overallProgress=[ordered]@{completeTools=0;totalTools=10;percent=0;authority='user complete-tool metric; internal stages do not count as overall progress'}
 internalCameraStages=[ordered]@{B1=@('S0','S1','S2','S3','S4','S5','S7');B2=@('S0','S1','S2','S3','S4','S5','S7');B3=@('S0');B4=@('S0')}
 managedMetadata=$managed
 resources=$resources
 nativeBodies=$native
 originalInstructionWindows=$instructions
 reusedIndexes=@(Identity "$root/docs/plugins/CAMERA-MAPPING-B3-RESET-SOURCE.json";Identity "$root/docs/plugins/CAMERA-MAPPING-B3-RETURN-SOURCE.json")
 remaining=@('Module local GetFloat wrapper and aggregate count/failure propagation','Dynamic integer data leaf and non-float source branches beyond selected chain','Full initialization/persistence/mode failure lifecycle','Actual original UI reset and real host acceptance','B3 last frame/file reopening/release remainder')
}
$target=Join-Path $root 'docs/plugins/CAMERA-MAPPING-B3-POINT-INDEX.json'
$report|ConvertTo-Json -Depth 30|Set-Content -LiteralPath $target -Encoding utf8
$r=Get-Content -LiteralPath $target -Raw|ConvertFrom-Json
if($r.nativeBodies.Count -ne $native.Count){throw 'Native index roundtrip mismatch'}
foreach($body in $r.nativeBodies){if((Get-FileHash -LiteralPath $body.source.path).Hash -ne $body.source.sha256){throw 'Native source changed'}}
[ordered]@{nativeBodies=$native.Count;instructionWindows=$instructions.Count;metadataAssemblies=$managed.Count;resourceGroups=$resources.groups.Count;index=(Identity $target)}|ConvertTo-Json -Depth 5
