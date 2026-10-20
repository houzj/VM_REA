$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'));Set-Location $root
$checks=@(
 @('analysis/distortion-calibration/ghidra/wrapper/180002860.c','if (*(int *)(param_1 + 0x40) != 0)'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0-asm.tsv','1807ee396'+"`t"+'MOV dword ptr [R9],0x40000000'),
 @('analysis/distortion-calibration/c3-dispatch-source/core/output/1807ed0c0.c','*(float *)((longlong)auStack_118 + 0x48) = (float)(int)uVar3'),
 @('analysis/distortion-calibration/c1-workspace-source/core/output/18001e330.c','memcpy_s(param_3 + 10,0x4c'),
 @('analysis/distortion-calibration/c1-workspace-source/wrapper/output/180002a70.c','+ 0x2c),local_1a0,0x4c)'),
 @('analysis/distortion-calibration/c3-file-source/wrapper/output/180001ed0.c','memcpy(param_2,(void *)(param_1 + 0x2c),0x4c)'),
 @('analysis/distortion-calibration/c3-file-source/wrapper/output/180003b20.c','fwrite(local_4c8,0x4c,1,_File)'),
 @('analysis/distortion-calibration/c3-file-source/wrapper/output/180003b20-asm.tsv','180003f57'+"`t"+'MOV EDX,0x4c'),
 @('analysis/distortion-calibration/c3-file-source/wrapper/output/180003b20-asm.tsv','180003f6e'+"`t"+'MOV RCX,RDI'),
 @('analysis/distortion-calibration/c3-file-source/module/output/180021560-asm.tsv','180021706'+"`t"+'MOV RDX,RSI'),
 @('analysis/distortion-calibration/c3-file-source/module/output/180021560-asm.tsv','180021709'+"`t"+'CALL qword ptr [RAX + 0x70]'),
 @('analysis/distortion-calibration/c3-file-source/module/output/180020820-asm.tsv','180020af2'+"`t"+'LEA RDX,[RBX + 0x15da1]'),
 @('analysis/distortion-calibration/c3-file-source/module/output/180020820.c',"if ((cVar3 != '\0') && (this[0x15da0] == (CIMVSImageCalibModu)0x0))"),
 @('analysis/distortion-calibration/c3-file-source/module/output/180021560.c','strncmp(param_2,"True",4)'),
 @('analysis/distortion-calibration/c3-file-source/module/output/18001b720.c','return 0xe0000204'),
 @('analysis/distortion-calibration/core-live-20559644c178452b8ab9c8bcf092d5a5/output/18083cef0.c','dVar22 = (double)((int)lVar10 + 1)')
)
foreach($c in $checks){if(!(Get-Content $c[0] -Raw).Contains($c[1])){throw "Missing anchor $($c[0]):$($c[1])"}}
$ids=(Get-Content docs/plugins/distortion-calibration-c1-workspace-source-index.json -Raw|ConvertFrom-Json).binaryIdentity
foreach($name in @('core','wrapper','module')){if((Get-FileHash $ids.$name.path).Hash-ne$ids.$name.sha256){throw "Binary identity changed $name"}}
$paths=@($checks|ForEach-Object{$_[0]})+@(Get-ChildItem analysis/distortion-calibration/c3-file-source -Recurse -File | Where-Object{$_.Extension-in@('.c','.tsv')} | ForEach-Object{[IO.Path]::GetRelativePath($root,$_.FullName)})+@('analysis/distortion-calibration/ghidra/module/1800020c0.c','analysis/distortion-calibration/ghidra/wrapper/1800047e0.c','src/VisionPlugins.DistortionCalibration/DistortionCalibrationFile.cs')
$r=[ordered]@{slice='C3 file fields and save gates finite';binaryIdentity=$ids;originalDllExecuted=$false;productChanged=$false;payloadFloatCount=19;payloadBytes=76;fieldGroups=@('header2','fileH[9]','center[2]','radial[4]','imageDimensions[2]','model');direction='output pixel to input image sampling coordinate; model3 homography then radial';checks=@($checks|ForEach-Object{@{path=$_[0];text=$_[1]}});sources=@($paths|Sort-Object -Unique|ForEach-Object{@{path=$_;sha256=(Get-FileHash $_).Hash}});remaining=@('Module outer catch output mapping','All inner-fit error propagation','Path encoding/permission/disk-full actual runs and concurrency')}
$r|ConvertTo-Json -Depth 12|Set-Content docs/plugins/distortion-calibration-c3-file-evidence.json -Encoding utf8
@{anchors=$checks.Count;files=$r.sources.Count;originalDllExecuted=$false;productChanged=$false}|ConvertTo-Json

