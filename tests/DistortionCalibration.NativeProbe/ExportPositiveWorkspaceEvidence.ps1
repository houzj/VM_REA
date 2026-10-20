$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$dir=Join-Path $root 'analysis/distortion-calibration/positive-workspace'
function Need($ok,$why){if(!$ok){throw $why}}
function Lines($file){@(Get-Content -LiteralPath $file|ForEach-Object {$_|ConvertFrom-Json})}
function Hash($file){(Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash}
$writer=Lines "$dir/positive-writer.jsonl";$median=Lines "$dir/median.jsonl";$fixed=Lines "$dir/fixed-trace.jsonl";$runs=Lines "$dir/runtime.jsonl"
Need (@($runs|Where-Object code -eq 0).Count -eq 12) 'Expected 12 original SDK successes'
Need (@($fixed|Where-Object phase -eq fixed-fit-input).Count -eq 84) 'Expected original 84-call sequence'
$main=@($writer|Where-Object phase -eq main-entry)
Need ($main.Count -eq 12 -and @($main.outer|Select-Object -Unique).Count -eq 1) 'Original outer workspace did not reuse'
$old=Get-Content (Join-Path $root 'docs/plugins/distortion-calibration-workspace-evidence.json') -Raw|ConvertFrom-Json
$results=@()
foreach($seq in 56,67){
 $fixture=$old.cases|Where-Object sequence -eq $seq
 $input=$median|Where-Object {$_.phase -eq 'median-input' -and $_.sequence -eq $seq -and $_.stage -eq 0}
 $output=$median|Where-Object {$_.phase -eq 'median-output' -and $_.sequence -eq $seq -and $_.stage -eq 0}
 $context=$fixed|Where-Object {$_.phase -eq 'fixed-fit-context' -and $_.sequence -eq $seq}
 $source=[Convert]::FromBase64String($fixture.entryBase64)
 $live=[IO.File]::ReadAllBytes((Join-Path $dir ('median-entry-{0:d3}-00.bin' -f $seq)))
 $count=[BitConverter]::ToInt32($source,0);$prefixCount=[BitConverter]::ToInt32($source,8);$start=44+16*$count
 Need ([Convert]::ToBase64String($source,0,$start) -ceq [Convert]::ToBase64String($live,0,$start)) "Seq $seq changed fit input/coefficients"
 $prefixBits=[BitConverter]::ToUInt32($source,$start+4*($prefixCount-1))
 Need ($input.sentinelBits -eq $prefixBits -and $output.sigma -eq $fixture.capturedContextSigma) "Seq $seq did not reproduce captured sigma/prefix"
 $offset=if($seq -eq 56){10108}else{10236}
 Need ($input.prefixAddress -eq $context.workspaceAddress+$offset) "Seq $seq unexpected median address"
 $slot=if($seq -eq 56){1}else{0}
 $last=@($writer|Where-Object {$_.phase -eq 'write' -and $_.slot -eq $slot -and $_.sequence -lt $seq})[-1]
 Need ($last.address -eq $input.prefixAddress -and $last.bits -eq $input.sentinelBits) "Seq $seq no exact preceding writer"
 if($seq -eq 56){
  Need ($last.sequence -eq 46 -and $last.nextRva -eq 0x7fdd02 -and $last.address -eq $last.r9+$last.rsi*4+4 -and $last.rsi -eq 94) 'Seq56 weight writer changed'
  $prior=$fixed|Where-Object {$_.phase -eq 'fixed-fit-context' -and $_.sequence -eq 46}
  Need ($last.address -eq $prior.workspaceAddress+9340 -and $last.r9 -eq $prior.workspaceAddress+8960) 'Seq46 weights layout differs'
 }else{
  Need ($last.sequence -eq 63 -and $last.nextRva -eq 0x7ff188 -and $last.r10 -eq $last.address+4 -and $last.bits -eq $last.r9) 'Seq67 insertion writer changed'
  Need ($last.rcx.ToString() -eq '18446744073709551614') 'Seq63 source index was not -2'
 }
 $model=2;$warp=if($seq -eq 56){0}else{1}
 $life=@($writer|Where-Object {$_.board -eq 2 -and $_.model -eq $model -and $_.warp -eq $warp -and $_.phase -in 'destroy-begin','destroy-end'})
 Need ($life.Count -eq 2 -and $life[0].slot0Bits -eq $life[1].slot0Bits -and $life[0].slot1Bits -eq $life[1].slot1Bits -and $life[1].slot0Readable -eq 1 -and $life[1].slot1Readable -eq 1) 'Observed bytes changed or unmapped at destroy'
 $results+= [ordered]@{sequence=$seq;count=$count;prefixBits=('0x{0:x8}' -f [uint32]$prefixBits);sigma=$output.sigma;zeroContextSigma=$fixture.zeroContextSigma;medianInput=$input;fitContext=$context;lastWriter=$last;destroyEvidence=$life}
}
$hashes=@{}
$files=@('trace-positive-workspace-probe.cpp','trace-positive-workspace-owned.h','trace-positive-median-owned.h','RunPositiveWorkspaceTrace.ps1','PositiveWorkspaceWriterAudit.java','RunPositiveWorkspaceWriterAudit.ps1','ExportPositiveWorkspaceEvidence.ps1')
foreach($f in $files){$rel="tests/DistortionCalibration.NativeProbe/$f";$hashes[$rel]=Hash (Join-Path $root $rel)}
foreach($f in @('positive-writer.jsonl','median.jsonl','fixed-trace.jsonl','runtime.jsonl','probe.exe','median-entry-056-00.bin','median-entry-067-00.bin','source/output/1807fdcff.c','source/output/1807fdcff-asm.tsv','source/output/1807ff184.c','source/output/1807ff184-asm.tsv')){$hashes["analysis/distortion-calibration/positive-workspace/$f"]=Hash (Join-Path $dir $f)}
$asmWeights=@(Get-Content "$dir/source/output/1807fdcff-asm.tsv"|Where-Object {$_ -match '^1807fdc(e|f)|^1807fdd02'})
$asmSort=@(Get-Content "$dir/source/output/1807ff184-asm.tsv"|Where-Object {$_ -match '^1807ff17|^1807ff18|^1807ff19'})
[ordered]@{date='2026-10-19';authority='original SDK owned-process controlled replay; no devices';coreSha256='85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549';hashes=$hashes;SDKRuns=12;fixedCalls=84;mainEntries=12;uniqueOuterWorkspace=1;cases=$results;weightsAssembly=$asmWeights;sortAssembly=$asmSort;confirmed=@('Seq46 duplicated native weight95 at inner+9340 becomes seq56 median prefix at different inner+10108; observed inner starts differ by768','Seq63 out-of-bounds insertion writes median[-1] which seq67 consumes after tool destruction and same-address reuse','Both original positive-prefix sigma differences reproduced with identical normalized points and coefficients','Observed target bytes remain readable and unchanged immediately before/after the intervening tool destruction on the observed thread');unknown=@('Original host allocator/free/arena lifetime and other-thread writes','Generic scratch initialization/default values','Whether the underlying allocation is freed or retained by an allocator during Destroy; same committed address does not establish that','Portable safe emulation policy for native underflow writes');productChanges='none'}|ConvertTo-Json -Depth 14|Set-Content (Join-Path $root 'docs/plugins/distortion-calibration-positive-workspace-evidence.json')
$results|ForEach-Object{[pscustomobject]@{sequence=$_.sequence;writer=$_.lastWriter.sequence;prefix=$_.prefixBits;sigma=$_.sigma;zero=$_.zeroContextSigma}}|ConvertTo-Json
