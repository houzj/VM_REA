$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'));Set-Location $root
$dir='analysis/distortion-calibration/c3-schema-source'
$meta=Get-Content "$dir/resources-rea.json" -Raw|ConvertFrom-Json
$core='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll'
if((Get-FileHash $core).Hash-ne$meta.subject.digest.sha256){throw 'Core changed'}
foreach($r in $meta.resources){if((Get-FileHash "$dir/resource-$($r.name.id).xml").Hash-ne$r.payload.sha256){throw 'Resource bytes changed'}}
[xml]$x=Get-Content "$dir/resource-206.xml" -Raw
$expected=@(@('HomoFreedom','0x40001','0x2'),@('CalibBoardWeightFun','0x40002','0x3'),@('WeightFactor','0x40003','0x14'),@('CorrectModel','0x40401','0x1'))
$fields=@();foreach($v in $expected){$n=$x.SelectSingleNode("//*[@Name='$($v[0])']");if($n.AlgorithmIndex-ne$v[1]-or$n.DefaultValue-ne$v[2]){throw 'Schema mismatch'};$fields+=@{name=$v[0];id=$n.AlgorithmIndex;default=$n.DefaultValue;type=$n.LocalName;min=$n.MinValue;max=$n.MaxValue;entries=@($n.EnumEntry|ForEach-Object{@{name=$_.Name;value=$_.Value}})}}
if(($x.SelectSingleNode("//*[@Name='CalibBoardWeightFun']/EnumEntry[@Name='Ransac']")).Value-ne'0x4'){throw 'Ransac missing'}
$checks=@(
 @('core/output/18001d6c0.c','puVar3[uVar6 * 0x26 + 0x21]'),
 @('core/output/180002af0.c','FUN_180001830'),
 @('core/output/180001830.c','FUN_180013320'),
 @('core/output/180001830.c','FUN_1800014d0'),
 @('core/output/180006ec0-asm.tsv','MOV R8D,0x6'),
 @('core/output/180007790.c','*puVar2 = uVar1'),
 @('core/output/180007790.c','*(undefined1 *)(param_1 + 0x110) = 1'),
 @('core/output/180007120.c','plVar9,param_2,1,plVar10 + 8,8'),
 @('core/output/180007120.c','*(undefined1 *)(param_1 + 0x110) = 0'),
 @('core/output/1807c7c20.c','if (3 < uVar3 - 1)'),
 @('core/output/1807c7c20.c','if (199 < uVar3 - 1)'),
 @('core/output/1807c7c20.c','return 0x1010303d'),
 @('core/output/180002e70.c','*(int *)(lVar3 + 400) = param_3'),
 @('core/output/180012ee0-asm.tsv','MOV RDX,RSI'),
 @('managed/ImageCalibModuParams.cs','weightFun = new ParamItem(this, "WeightFun")'),
 @('managed/ParamItem.cs','Value = value.ToString()')
)
foreach($c in $checks){if(!(Get-Content "$dir/$($c[0])" -Raw).Contains($c[1])){throw "Missing anchor $($c[0]):$($c[1])"}}
$charset=[Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes((Join-Path $root "$dir/core/output/181ea78b8.bin"))).Split([char]0)[0];if($charset-ne'0123456789.-'){throw 'Character set mismatch'}
$files=@(Get-ChildItem $dir -Recurse -File | Where-Object { $_.Extension-in@('.c','.cs','.tsv','.bin','.xml','.json') } | Sort-Object FullName | ForEach-Object {@{path=[IO.Path]::GetRelativePath($root,$_.FullName);sha256=(Get-FileHash $_.FullName).Hash}})
$result=[ordered]@{slice='C3-2 schema and delayed state';originalDllExecuted=$false;productChanged=$false;reaResourceEvidence=$meta.evidence_id;fields=$fields;anchors=@($checks|ForEach-Object{@{path="$dir/$($_[0])";text=$_[1]}});numericTokenCharacters=$charset;stateChain=@('parse original ID/value','operation6 validates only','cache original pair and dirty','schema DOM original value','Process operation1 applies algorithm context; Weight4 to3','getter cache retains original successful value');sourceFiles=$files;runtimeLimitation='Getter4/context3 is source-derived conditional normal path, not a new runtime observation';remaining=@('Enum splitter and conversion exceptional tokens','DOM failure versus cache rollback','Full persistence transaction','Full model grid center/scale/initial-H selection')}
$result|ConvertTo-Json -Depth 12|Set-Content docs/plugins/distortion-calibration-c3-schema-evidence.json -Encoding utf8
@{anchors=$checks.Count;files=$files.Count;fields=$fields.Count;resource206Sha256=(Get-FileHash "$dir/resource-206.xml").Hash;originalDllExecuted=$false}|ConvertTo-Json
