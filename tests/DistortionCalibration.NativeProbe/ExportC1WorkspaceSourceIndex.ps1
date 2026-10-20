$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$base=Join-Path $root 'analysis/distortion-calibration/c1-workspace-source'
function Need($ok,$why){if(!$ok){throw $why}}
function Digest($p){(Get-FileHash -LiteralPath $p -Algorithm SHA256).Hash}
$binPaths=@{
 wrapper='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDImageCalibCpp.dll'
 core='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll'
 common='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDCommon.dll'
 module='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSImageCalibModu/IMVSImageCalibModu.dll'
}
$expected=@{wrapper='AE4D64C5505E25E192EC39A5DF105DA16A2C69DE51D23F8E5B66C97A771FA120';core='85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549';common='9C3E9C40305E9E892BE97FE1C5428474B2885D6FE8AC5DAFAF7EC1395486970B';module='0A6BAC886F1AA1CCC41433EF9D35FC1091B3D43354DE5794563454D8BBA03BCB'}
$identities=@{};foreach($kind in $binPaths.Keys){$hash=Digest $binPaths[$kind];Need ($hash -ceq $expected[$kind]) "$kind binary differs";$identities[$kind]=@{path=$binPaths[$kind];sha256=$hash}}
$required=@{wrapper=@('180001000','180001070','180002590','180003fe0','1800041c0','180002a70');module=@('18001f400','18001f470','18001f9f0','180021530','1800203f0');core=@('180014930','180015270','1800152b0','1800156c0','1800156e0','18001bff0','18001c140','18001c170','18001c210','18001c4e0','18001cdc0','18001d550','180022c90','1800061f0','1800062e0','180073030','1807c4750','1807c5340','1807c7470','1807f7050','1807ecea0','1807ff3c0','1807ecd50');common=@('180008150','1800081a0','1800018f0','180001e40','180002630','1800024f0','1800015f0','180001710','180001570','180013020','180013070','180001800','180002110')}
foreach($kind in $required.Keys){foreach($address in $required[$kind]){Need (Test-Path "$base/$kind/output/$address.c") "$kind $address body missing";Need (Test-Path "$base/$kind/output/$address-asm.tsv") "$kind $address assembly missing"}}
$moduleAsm=Get-Content "$base/module/output/1800203f0-asm.tsv" -Raw
Need ($moduleAsm.Contains('LEA RCX,[RBX + 0x15d98]')) 'Module Create ABI unresolved'
$wrapperAsm=Get-Content "$base/wrapper/output/1800041c0-asm.tsv" -Raw
Need ($wrapperAsm.Contains('MOV RCX,qword ptr [RCX + 0x20]')) 'SDK destroy ABI unresolved'
$processAsm=Get-Content "$base/core/output/18001c4e0-asm.tsv" -Raw
Need ($processAsm.Contains('18001c6ab'+"`t"+'MOV R9D,0x1')) 'Run release mask differs'
$cleanupAsm=Get-Content "$base/core/output/18001d550-asm.tsv" -Raw
Need ($cleanupAsm.Contains('MOV R9D,0x6')) 'Destroy release mask differs'
$freeBody=Get-Content "$base/common/output/180001e40.c" -Raw
Need ($freeBody.Contains('*(undefined1 *)(puVar14 + 2) = 0;') -and !$freeBody.Contains('free(')) 'Pool return body differs'
$allocator=Get-Content "$base/common/output/1800018f0.c" -Raw
Need ($allocator.Contains('*(undefined1 *)(puVar17 + 2) = 1;') -and !$allocator.Contains('memset(')) 'Pool reuse semantics differ'
$index=@();foreach($kind in $binPaths.Keys){foreach($f in Get-ChildItem "$base/$kind/output" -Filter '*.c'){$stem=$f.BaseName;$asm=Join-Path $f.DirectoryName ($stem+'-asm.tsv');$calls=Join-Path $f.DirectoryName ($stem+'-calls.tsv');$entry=[ordered]@{binary=$kind;address=('0x'+$stem);provenance='private Ghidra export; native pseudocode is not original source';path=$f.FullName;sha256=Digest $f.FullName;status=if($required[$kind] -contains $stem){'body and assembly acquired; audited finite lifecycle path'}else{'acquired; not all branches audited'}};if(Test-Path $asm){$entry.assemblyPath=$asm;$entry.assemblySha256=Digest $asm};if(Test-Path $calls){$entry.callsPath=$calls;$entry.callsSha256=Digest $calls};$index+=$entry}}
$bytes=[IO.File]::ReadAllBytes("$base/core/output/181ead878.bin");$vtable=@();for($i=0;$i -lt 11;$i++){$vtable+=@{slot=('0x{0:x}' -f ($i*8));target=('0x{0:x}' -f [BitConverter]::ToUInt64($bytes,$i*8))}}
$scripts=@{};foreach($f in @('RunC1WorkspaceSource.ps1','RunC1ModuleSource.ps1','RunC1WrapperSource.ps1','RunC1CommonSource.ps1','RunC1CommonFollowupSource.ps1','DistortionC1CommonAudit.java','ExportC1WorkspaceSourceIndex.ps1')){$scripts[$f]=Digest (Join-Path $PSScriptRoot $f)}
[ordered]@{date='2026-10-19';task='C1 finite S1/S2 workspace allocator lifecycle';binaryIdentity=$identities;sourceCount=$index.Count;sources=$index;vtable=@{address='0x181ead878';capturedPath="$base/core/output/181ead878.bin";sha256=Digest "$base/core/output/181ead878.bin";knownSlots=$vtable};scripts=$scripts;existingReuse=@{path=(Join-Path $root 'analysis/distortion-calibration/core-live-eb19412b1b45482281d7ce5d0c087aa4/output/1807ed0c0.c');sha256=Digest (Join-Path $root 'analysis/distortion-calibration/core-live-eb19412b1b45482281d7ce5d0c087aa4/output/1807ed0c0.c')};rea=@{sessionOpen=$false;deepProvider='Ghidra not_configured; GHIDRA_INSTALL_DIR missing';targetSwitched=$false;evidenceId=$null;fallback='root-authorized private Ghidra; no global configuration writes'};sourceConclusions=@('Actual module DeInit/destructor destroys SDK tool, ResetMember does not','SDK core object owns 8 memory descriptors; temporary type0 returned at normal/operation-error Process tail mask1','Destruction returns persistent descriptors mask6 and clears owner fields','Pool return marks free without payload clear or CRT free; reuse returns same busy-marked block without clear','CRT free occurs on idle-block aging/pressure cleanup, malloc-failure retry, and global pool shutdown');runtime='no new runtime probe; previous captured prefixes only corroborate source interpretation';implementation='no product change; deterministic policies are independent options awaiting user decision';unknown=@('All Init capacity-growth/failure/exception rollback branches','Unexpanded size-query helpers and every feature combination','All consumers and concurrency effects in real VisionMaster host','CRT/OS heap release beyond CRT free','C2 complete sort index/odd-even boundaries') }|ConvertTo-Json -Depth 12|Set-Content (Join-Path $root 'docs/plugins/distortion-calibration-c1-workspace-source-index.json')
[pscustomobject]@{sourceCount=$index.Count;binaryCount=$identities.Count;validatedRequiredBodies=($required.Values|ForEach-Object{$_.Count}|Measure-Object -Sum).Sum;vtableSlots=$vtable.Count;productChanged=$false}|ConvertTo-Json
