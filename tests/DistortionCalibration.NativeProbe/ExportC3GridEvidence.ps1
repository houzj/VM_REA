$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'));Set-Location $root
$d='analysis/distortion-calibration/c3-grid-source';$meta=Get-Content "$d/wrapper-constant.json" -Raw|ConvertFrom-Json
if((Get-FileHash $meta.binary).Hash-ne$meta.sha256){throw 'Wrapper changed'}
$b=[IO.File]::ReadAllBytes($meta.binary);$nt=[BitConverter]::ToInt32($b,60);$n=[BitConverter]::ToUInt16($b,$nt+6);$st=$nt+24+[BitConverter]::ToUInt16($b,$nt+20);$off=-1
for($i=0;$i-lt$n;$i++){$h=$st+40*$i;$va=[BitConverter]::ToUInt32($b,$h+12);$sz=[BitConverter]::ToUInt32($b,$h+16);if($meta.rva-ge$va-and$meta.rva-lt$va+$sz){$off=[BitConverter]::ToUInt32($b,$h+20)+$meta.rva-$va;break}}
if($off-ne$meta.fileOffset-or[BitConverter]::ToUInt32($b,$off)-ne0x3f800000){throw 'SDK scalar constant mismatch'}
$c=[IO.File]::ReadAllBytes((Join-Path $root 'analysis/distortion-calibration/c3-model-points/core/output/1816299a0.bin'));if([BitConverter]::ToUInt32($c,168)-ne0x3dcccccd-or[BitConverter]::ToUInt32($c,176)-ne0x3f000000){throw 'Grid thresholds mismatch'}
$src='analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0.c';$asm='analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv'
$checks=@(
 @($src,'if (_DAT_181629a48 < (fVar27 - fVar35) / fVar35)'),
 @($src,'FUN_1815729e0(param_13,param_2'),
 @($src,'if (auVar34._0_4_ <= auVar39._0_4_)'),
 @($src,'fVar27 = cosf(auVar39._0_4_)'),
 @($src,'fVar35 = sinf(auVar39._0_4_)'),
 @($src,'if ((int)uVar17 < (int)uVar12)'),
 @($src,'*(undefined4 *)(param_13 + lVar20 * 8) = 0'),
 @($asm,'1807ed301'+"`t"+'MOVSS XMM11,dword ptr [RBX + 0x10]'),
 @($asm,'1807ed79a'+"`t"+'PCMPEQD XMM12,XMM2'),
 @($asm,'1807ed7ec'+"`t"+'MOVD EDI,XMM5'),
 @($asm,'1807ed876'+"`t"+'CMP R9D,R11D'),
 @($asm,'1807ed880'+"`t"+'CMOVG RDI,R10'),
 @('analysis/distortion-calibration/c1-workspace-source/core/output/1807f7050.c','uStack_88 = *(undefined4 *)((longlong)param_1 + 0x24)'),
 @('analysis/distortion-calibration/c1-workspace-source/core/output/18001e330.c','uVar1 = param_2[3]'),
 @('analysis/distortion-calibration/c1-workspace-source/wrapper/output/180002a70.c','local_36c = DAT_180007db0')
)
foreach($p in $checks){if(!(Get-Content $p[0] -Raw).Contains($p[1])){throw "Missing anchor $($p[0]): $($p[1])"}}
$files=@($checks|ForEach-Object{$_[0]})+@("$d/wrapper-constant.json","$d/wrapper-180007db0.bin",'analysis/distortion-calibration/c3-model-points/core/output/1816299a0.bin')
$r=[ordered]@{slice='C3 grid scale and origin finite';originalDllExecuted=$false;productChanged=$false;wrapperConstant=$meta;checks=@($checks|ForEach-Object{@{path=$_[0];text=$_[1]}});distinctScales=@('SDK/core input gridScale','last matching edge distance for rotated quantization','model1/3 two-point image-to-ideal scale');ratioThresholdBits='3dcccccd';roundingHalfBits='3f000000';sources=@($files|Sort-Object -Unique|ForEach-Object{@{path=$_;sha256=(Get-FileHash $_).Hash}});unknown=@('Nonfinite/overflow/zero-edge actual execution','All error and lifecycle paths','Real host acceptance')}
$r|ConvertTo-Json -Depth 10|Set-Content docs/plugins/distortion-calibration-c3-grid-evidence.json -Encoding utf8
@{anchors=$checks.Count;files=$r.sources.Count;originalDllExecuted=$false;productChanged=$false}|ConvertTo-Json
