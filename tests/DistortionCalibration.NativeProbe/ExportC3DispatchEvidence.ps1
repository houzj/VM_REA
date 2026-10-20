$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
Set-Location $root
$checks=@(
 @('analysis/distortion-calibration/ghidra/module/180021560.c','strncmp("4",param_2,1)'),
 @('analysis/distortion-calibration/ghidra/module/180021560.c','pcVar12 = "CalibBoardWeightFun"'),
 @('analysis/distortion-calibration/c3-dispatch-source/wrapper/output/vtables.tsv','50'+"`t"+'1800028e0'),
 @('analysis/distortion-calibration/c3-dispatch-source/wrapper-methods/output/1800028e0-asm.tsv','180002934'+"`t"+'MOV RCX,qword ptr [RBX + 0x20]'),
 @('analysis/distortion-calibration/c3-dispatch-source/wrapper-methods/output/180002870-asm.tsv','MOV R8D,0x100'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/180022ab0.c','FUN_180007020'),
 @('analysis/distortion-calibration/c1-workspace-source/core/output/1807c6830.c','if (iVar4 == 4)'),
 @('analysis/distortion-calibration/c1-workspace-source/core/output/1807c6830.c','if (iVar5 != 4)'),
 @('analysis/distortion-calibration/c1-workspace-source/core/output/1807c6830.c','param_1[0xa0] = 3'),
 @('analysis/distortion-calibration/c1-workspace-source/core/output/1807f7050-asm.tsv','MULSS XMM3,dword ptr [0x18162b610]'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807edaa9'+"`t"+'CMP R13D,0x2'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807edfc9'+"`t"+'CALL 0x1807ff3c0'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807edf0c'+"`t"+'CALL 0x1808345e0'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807ede72'+"`t"+'CALL 0x180832830'),
 @('analysis/distortion-calibration/c1-workspace-source/core/output/1807ff3c0.c','fVar25 = fVar31 / (fVar19 * fVar17)')
)
foreach($c in $checks){if(!(Get-Content $c[0] -Raw).Contains($c[1])){throw "Missing source anchor $($c[0]): $($c[1])"}}
$constant=[IO.File]::ReadAllBytes((Join-Path $root 'analysis/distortion-calibration/c3-dispatch-source/core/output/18162b600.bin'))
if([BitConverter]::ToUInt32($constant,12)-ne0x3dcccccd -or [BitConverter]::ToUInt32($constant,16)-ne0x3c23d70a){throw 'Scale constants mismatch'}
[xml]$xml=Get-Content src/VisionPlugins.DistortionCalibration/Resources/DistortionAlgorithm.xml -Raw
$defaults=@{};foreach($item in $xml.AlgorithmParamList.ParamItem){$defaults[$item.Name]=$item.DefaultValue}
foreach($pair in @(@('HomoFreedom','1'),@('CalibBoardWeightFun','2'),@('WeightFactor','20'),@('CorrectModel','1'))){if($defaults[$pair[0]]-ne$pair[1]){throw 'Default mismatch'}}
$files=@($checks|ForEach-Object{$_[0]})+@('analysis/distortion-calibration/c3-dispatch-source/core/output/1808345e0.c','analysis/distortion-calibration/c3-dispatch-source/core/output/180832830.c','analysis/distortion-calibration/c3-dispatch-source/core/output/180022890.c','analysis/distortion-calibration/c3-dispatch-source/core/output/180067790.c','analysis/distortion-calibration/c3-dispatch-source/core/output/18162b600.bin','src/VisionPlugins.DistortionCalibration/Resources/DistortionAlgorithm.xml','src/VisionPlugins.DistortionCalibration/Resources/DistortionAlgorithmTab.xml')
$files += @(Get-ChildItem analysis/distortion-calibration/c3-dispatch-source -Recurse -File | Where-Object { $_.Extension -in @('.c','.tsv','.bin') } | ForEach-Object { [IO.Path]::GetRelativePath($root,$_.FullName) })
$identities=(Get-Content docs/plugins/distortion-calibration-c1-workspace-source-index.json -Raw|ConvertFrom-Json).binaryIdentity
foreach($name in @('core','wrapper','module')){if((Get-FileHash $identities.$name.path).Hash-ne$identities.$name.sha256){throw "Binary identity changed: $name"}}
$hashes=@($files|Sort-Object -Unique|ForEach-Object{@{path=$_;sha256=(Get-FileHash $_ -Algorithm SHA256).Hash}})
$result=[ordered]@{slice='C3-config-dispatch-first';binaryIdentity=$identities;sourceConfirmed=$true;originalDllExecuted=$false;productChanged=$false;provider='Existing private Ghidra read-only project; REA Ghidra not_configured';defaults=$defaults;anchors=@($checks|ForEach-Object{@{path=$_[0];text=$_[1]}});sourceFiles=$hashes;remaining=@('Exact schema name/id/range and string conversion bodies','Full model point/center/scale/initial-H expressions','OLS and sample-kernel internal math','Real host binding and acceptance')}
$result|ConvertTo-Json -Depth 12|Set-Content docs/plugins/distortion-calibration-c3-dispatch-evidence.json -Encoding utf8
@{anchors=$checks.Count;sourceFiles=$hashes.Count;originalDllExecuted=$false;productChanged=$false}|ConvertTo-Json


