$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$base=Join-Path $root 'analysis/distortion-calibration/recreated-tool-workspace'
function LoadJsonLines($path){ @(Get-Content -LiteralPath $path | ForEach-Object { $_ | ConvertFrom-Json }) }
function Require($condition,$message){ if(!$condition){throw $message} }
function Digest($path){ (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash }
$same=LoadJsonLines "$base/writer-trace.jsonl"
$mixed=LoadJsonLines "$base/checker-prelude/writer-trace.jsonl"
$sameRuns=@(LoadJsonLines "$base/runs.jsonl" | Where-Object { $null -ne $_.repeat })
$mixedRuns=@(LoadJsonLines "$base/checker-prelude/runs.jsonl" | Where-Object { $null -ne $_.repeat })
$sameMain=@($same|Where-Object phase -eq main-entry)
$mixedMain=@($mixed|Where-Object phase -eq main-entry)
$circles=@($mixedMain|Where-Object board -eq 2)
$writes=@($mixed|Where-Object phase -eq write)
$registers=@($mixed|Where-Object phase -eq writer-registers)
Require ($sameRuns.Count -eq 48 -and $mixedRuns.Count -eq 48) 'Expected 48 successes in each scenario'
Require ($sameMain.Count -eq 48 -and $mixedMain.Count -eq 48) 'Missing observed main calls'
Require (@($sameMain.workspace|Select-Object -Unique).Count -eq 1) 'Baseline workspace did not reuse'
Require (@($mixedMain.workspace|Select-Object -Unique).Count -eq 1) 'Prelude workspace did not reuse'
Require (@($sameMain|Where-Object initialBits -ne 0).Count -eq 0) 'Baseline prefix changed'
Require (@($same|Where-Object phase -eq write).Count -eq 0) 'Baseline prefix writes observed'
Require ($circles.Count -eq 46 -and @($circles.initialBits|Select-Object -Unique).Count -eq 1 -and $circles[0].initialBits -ne 0) 'Prelude residue not persistent'
Require (@($writes|Where-Object board -ne 1).Count -eq 0 -and $writes.Count -gt 0) 'Unexpected residue writer'
Require ($writes[-1].bits -eq $circles[0].initialBits -and $writes[-1].address -eq $circles[0].watchAddress) 'Residue not linked to earlier exact address'
Require ($registers.Count -eq $writes.Count) 'Missing writer register evidence'
foreach($w in $registers){
 Require ($w.address -eq $w.designBase + $w.rowStrideIndex*4 + 60) 'Address not design coefficient field15'
 Require ($w.pointIndex -eq 83 -and $w.rowStrideIndex -eq 83*16 -and $w.bits -eq $w.xmm4Bits) 'Writer register mismatch'
}
$baseline=($sameRuns[0].matrix|ConvertTo-Json -Compress)
foreach($r in $sameRuns+@($mixedRuns|Where-Object instance -ne 0)){
 Require ($r.count -eq 80 -and ($r.matrix|ConvertTo-Json -Compress) -ceq $baseline -and $r.error -eq $sameRuns[0].error) 'Circle output differs'
}
$maskStream=[IO.File]::OpenRead('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll')
try{$maskStream.Position=0x162a240;$maskBytes=[byte[]]::new(4);Require ($maskStream.Read($maskBytes,0,4) -eq 4 -and [BitConverter]::ToUInt32($maskBytes,0) -eq [Convert]::ToUInt32('80000000',16)) 'Source sign mask differs'}finally{$maskStream.Dispose()}
$files=@('tests/DistortionCalibration.NativeProbe/trace-recreated-tool-workspace-probe.cpp','tests/DistortionCalibration.NativeProbe/trace-recreated-workspace-writer-owned.h','tests/DistortionCalibration.NativeProbe/RunRecreatedToolWorkspace.ps1','tests/DistortionCalibration.NativeProbe/DistortionWorkspaceWriterAudit.java','tests/DistortionCalibration.NativeProbe/RunRecreatedWorkspaceWriterAudit.ps1')
$hashes=@{};foreach($file in $files){$hashes[$file]=Digest (Join-Path $root $file)}
foreach($file in @('writer-trace.jsonl','runs.jsonl','probe.exe','checker-prelude/writer-trace.jsonl','checker-prelude/runs.jsonl','checker-prelude/probe.exe','writer-source/output/writer.c','writer-source/output/writer-asm.tsv','writer-source/output/identity.txt')){$hashes["analysis/distortion-calibration/recreated-tool-workspace/$file"]=Digest (Join-Path $base $file)}
$result=[ordered]@{
 authority='controlled original SDK replay; private read-only Ghidra; no devices';date='2026-10-19';hashes=$hashes
 observations=@{baselineFirstEntry=$sameMain[0];preludeFirstEntries=@($mixedMain|Select-Object -First 3);writes=$writes;writerRegisters=$registers;baselineOutput=$sameRuns[0];firstCircleAfterPrelude=$mixedRuns[2]}
 sourceBinary=@{path='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll';sha256=Digest 'D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll'}
 scenarios=@(@{name='same-circle';instances=24;runs=48;uniqueWorkspace=1;prefixBits='0x00000000';changedWrites=0},@{name='checker-prelude';instances=24;runs=48;checkerRuns=2;circleRuns=46;uniqueWorkspace=1;circlePrefixBits=('0x{0:x8}' -f [uint32]$circles[0].initialBits);changedWrites=$writes.Count;registerEvidenceCount=$registers.Count})
 layout=@{align=128;checkerCount=99;circleCount=80;checkerOuterPrefixBytes=4480;circleOuterPrefixBytes=3200;circleMedianPrefixFromInner=10236;circleMedianPrefixFromOuter=13436;checkerDesignFromOuter=8064;checkerDesignRow=83;checkerDesignField=15;checkerDesignByteOffset=5372;writerInstructionRva='0x8003da';hardwareTrapNextRva='0x8003e0';instruction='MOVSS [RAX+RBX*4+0x3c],XMM4';meaning='negative normalized target Y times normalized source Y';signMaskRva='0x162b640';signMaskFileOffset='0x162a240';signMaskBits='0x80000000'}
 confirmed=@('24 identical-input instances reuse native workspace but do not generate nonzero prefix','Earlier 99-point checker design coefficient writes exactly the future 80-point median prefix address','After checker destruction the 23 circle instances retain this exact bit pattern across 46 calls','All 46 circle outputs remain identical to zero-prefix baseline in this bounded negative-prefix case')
 unknown=@('Production VisionMaster host allocator, workspace scope and intervening writes','Deterministic initialization/default value for arbitrary sessions','Whether positive-prefix differing fixtures have the same immediate write source','Complete ABI/function boundaries: existing Ghidra identifies 1808002fe fragment with live-in registers')
 productChange='none; no hardcoded workspace defaults or copied solver'
}
$result|ConvertTo-Json -Depth 12|Set-Content (Join-Path $root 'docs/plugins/distortion-calibration-recreated-workspace-evidence.json')
$result.scenarios|ConvertTo-Json -Depth 5



