$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'));Set-Location $root
$py='C:/Users/houzhongjie/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe'
& $py tests/DistortionCalibration.NativeProbe/ExportDistortionModuleExceptionScopes.py > analysis/distortion-calibration/c3-catch-source/metadata-export.log
if($LASTEXITCODE-ne0){throw 'Passive PE metadata export failed'}
$d='analysis/distortion-calibration/c3-catch-source';$m=Get-Content "$d/module-exception-scopes.json" -Raw|ConvertFrom-Json
$expected=@(@('20550',1,1,3,'18002074c','180020782'),@('20820',0,0,3,'1800209b9','1800209d3'),@('20820',2,3,4,'180020b06','180020b06'),@('20b70',0,3,4,'1800212ec','1800212ec'))
$handlers=@();foreach($e in $expected){$record=$m.records|Where-Object begin -eq "0x$($e[0])";$block=$record.tryBlocks|Where-Object {$_.tryLow-eq$e[1]-and$_.tryHigh-eq$e[2]};if(!$block-or$block.handlers.Count-ne4){throw 'Try metadata mismatch'};for($i=0;$i-lt4;$i++){$h=$block.handlers[$i];$addr='1800'+$h.handlerRva.Substring(2);$file="$d/module/output/$addr.c";$asm="$d/module/output/$addr-asm.tsv";if(!(Test-Path $asm)){throw 'Handler instructions absent'};$text=Get-Content $file -Raw;$cont=if($i-lt3){$e[4]}else{$e[5]};if(!$text.Contains("return &DAT_$cont")){throw "Wrong continuation $addr"};if($i-eq3-and!$text.Contains('0xe00000ff')){throw 'Wrong catch-all code'};$handlers+=@{rva=$h.handlerRva;type=$h.typeDescriptorName;continuation=$cont;source=$file}}}
$checks=@(
 @("$d/continuations/output/18002074c-continuation.tsv",'18002076c'+"`t"+'JNZ 0x1800207a7'),
 @("$d/continuations/output/180020782-continuation.tsv",'1800207a0'+"`t"+'JMP 0x1800207a7'),
 @("$d/continuations/output/1800209b9-continuation.tsv",'1800209c2'+"`t"+'MOV R14D,dword ptr [RSP + 0x40]'),
 @("$d/continuations/output/180020b06-continuation.tsv",'180020b2b'+"`t"+'MOV dword ptr [R8 + 0x4],R14D'),
 @("$d/continuations/output/180020b06-continuation.tsv",'180020b3b'+"`t"+'MOV dword ptr [R8],EAX'),
 @("$d/continuations/output/180020b06-continuation.tsv",'180020b3e'+"`t"+'MOV EAX,R14D'),
 @("$d/continuations/output/1800212ec-continuation.tsv",'1800212ec'+"`t"+'MOV EBX,dword ptr [RSP + 0x40]'),
 @("$d/module/output/180020550-asm.tsv",'1800207ba'+"`t"+'CALL 0x180020b70'),
 @("$d/module/output/180020550-asm.tsv",'180020807'+"`t"+'MOV EAX,EBX'),
 @("$d/module/output/180020b70-asm.tsv",'180020c43'+"`t"+'JNZ 0x180020c4c'),
 @("$d/module/output/180020b70-asm.tsv",'180020c1f'+"`t"+'CALL 0x18001f0e0'),
 @('analysis/distortion-calibration/c3-file-source/module/output/180020820-asm.tsv','180020a49'+"`t"+'MOV dword ptr [RSP + 0x50],ESI')
)
foreach($c in $checks){if(!(Get-Content $c[0] -Raw).Contains($c[1])){throw "Missing catch anchor $($c[0]):$($c[1])"}}
$between=@(Get-Content "$d/module/output/180020550-asm.tsv"|Where-Object {$_ -match '^1800207[b-f]|^18002080[0-6]'});if($between|Where-Object {$_-match '\t(MOV|XOR|LEA) EBX,'}){throw 'Unexpected overwrite of retained Process return'}
$files=@(Get-ChildItem $d -Recurse -File | Where-Object {$_.Extension-in@('.c','.tsv','.json','.txt')-and$_.Name-notin@('jobs.txt','ready.txt','stopped.txt','completed.txt')}|ForEach-Object{[IO.Path]::GetRelativePath($root,$_.FullName)})+@('analysis/distortion-calibration/c3-file-source/module/output/180020820-asm.tsv','tests/DistortionCalibration.NativeProbe/ExportDistortionModuleExceptionScopes.py','tests/DistortionCalibration.NativeProbe/DistortionCatchContinuations.java')
$r=[ordered]@{slice='C3 module EH to final publication finite';originalDllExecuted=$false;productChanged=$false;module=$m.target;sha256=$m.sha256;tryGroups=4;handlerCount=$handlers.Count;handlers=$handlers;checks=@($checks|ForEach-Object{@{path=$_[0];text=$_[1]}});sourceFiles=@($files|Sort-Object -Unique|ForEach-Object{@{path=$_;sha256=(Get-FileHash $_).Hash}});unknown=@('Exceptions within handlers or outside shown try scopes','Framework outer catch/return-code consumers','No new native exception injection or host recovery run')}
$r|ConvertTo-Json -Depth 12|Set-Content docs/plugins/distortion-calibration-c3-catch-evidence.json -Encoding utf8
@{tryGroups=4;handlers=$handlers.Count;continuations=6;anchors=$checks.Count;files=$r.sourceFiles.Count;originalDllExecuted=$false}|ConvertTo-Json
