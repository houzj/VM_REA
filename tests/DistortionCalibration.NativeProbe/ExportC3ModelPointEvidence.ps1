$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'));Set-Location $root
$d='analysis/distortion-calibration/c3-model-points/core/output'
$checks=@(
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eeb54'+"`t"+'JZ 0x1807eebe9'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eec35'+"`t"+'JMP 0x1807eeb62'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eeb7b'+"`t"+'JZ 0x1807eecf9'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eed45'+"`t"+'JMP 0x1807eeb89'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eeba2'+"`t"+'JZ 0x1807eec4d'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eec99'+"`t"+'JMP 0x1807eebb0'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eebc7'+"`t"+'JZ 0x1807eecb1'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eecf4'+"`t"+'JMP 0x1807eebd7'),
 @('analysis/distortion-calibration/c1-workspace-source/wrapper/output/180002a70-asm.tsv','180002f0e'+"`t"+'CVTTSS2SI ECX,dword ptr [RAX]'),
 @('analysis/distortion-calibration/c1-workspace-source/wrapper/output/180002a70-asm.tsv','180002f29'+"`t"+'CVTTSS2SI ECX,dword ptr [RAX + 0x4]'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eeb0d'+"`t"+'JBE 0x1807eeb12'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eec1a'+"`t"+'JBE 0x1807eec3a'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eed2a'+"`t"+'JBE 0x1807eed4a'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eec7e'+"`t"+'JBE 0x1807eec9e'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807eece6'+"`t"+'CMOVA RAX,RDI'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807edd6f'+"`t"+'JBE 0x1807edd77'),
 @("$d/18089ec80-asm.tsv",'SQRTSS XMM1,XMM1'),
 @("$d/180853820-asm.tsv",'180853fdf'+"`t"+'MOVSD XMM3,qword ptr [0x181643f50]'),
 @("$d/180853820-asm.tsv",'CALL 0x1808345e0'),
 @("$d/180853820-asm.tsv",'CALL 0x1807ff3c0')
)
foreach($c in $checks){if(!(Get-Content $c[0] -Raw).Contains($c[1])){throw "Missing anchor $($c[0]):$($c[1])"}}
$b=[IO.File]::ReadAllBytes((Join-Path $root "$d/1816299a0.bin"));foreach($p in @(@(160,0x34000000),@(164,0x7f7fffff),@(184,0x3f800000))){if([BitConverter]::ToUInt32($b,$p[0])-ne$p[1]){throw 'Point constants changed'}}
$r=[IO.File]::ReadAllBytes((Join-Path $root "$d/181643ec0.bin"));$radial=@(16,24,32,40|ForEach-Object{[BitConverter]::ToDouble($r,$_)})
if(($radial-join',')-ne'0,0,1,0'){throw 'Radial initial values changed'}
$half=[IO.File]::ReadAllBytes((Join-Path $root "$d/181643f50.bin"));if([BitConverter]::ToDouble($half,0)-ne.5){throw 'Half constant changed'}
$core=(Get-Content docs/plugins/distortion-calibration-c1-workspace-source-index.json -Raw|ConvertFrom-Json).binaryIdentity.core
if((Get-FileHash $core.path).Hash-ne$core.sha256){throw 'Original core changed'}
$paths=@($checks|ForEach-Object{$_[0]})+@(Get-ChildItem $d -File | Where-Object {$_.Extension-in@('.c','.tsv','.bin')}|ForEach-Object{[IO.Path]::GetRelativePath($root,$_.FullName)})+@('analysis/distortion-calibration/c1-workspace-source/core/output/18001e330.c','analysis/distortion-calibration/c1-workspace-source/core/output/1807f7050.c','analysis/distortion-calibration/ghidra/module/180020550.c','analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0.c')
$result=[ordered]@{slice='C3 model1/3 point selection finite';binary=$core;originalDllExecuted=$false;productChanged=$false;anchors=@($checks|ForEach-Object{@{path=$_[0];text=$_[1]}});a='first strict minimum float squared distance to truncated CorrectCenterPoint';bDirections=@('gx-1,gy','gx+1,gy','gx,gy-1','gx,gy+1');b='first matching index per direction; strict shorter replaces; initial index0';scaleThresholdBits='34000000';radialInitial=$radial;radialCenterInitial='double image width/height multiplied by .5; distinct from selection input';sources=@($paths|Sort-Object -Unique|ForEach-Object{@{path=$_;sha256=(Get-FileHash $_).Hash}});remaining=@('Rotated grid/origin inference full branches','Grid scale source full branches','Nonfinite/int32 overflow runtime contract','Full LM mathematics and real host acceptance')}
$result|ConvertTo-Json -Depth 10|Set-Content docs/plugins/distortion-calibration-c3-model-point-evidence.json -Encoding utf8
@{anchors=$checks.Count;files=$result.sources.Count;originalDllExecuted=$false;productChanged=$false}|ConvertTo-Json

