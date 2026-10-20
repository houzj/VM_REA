param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$directory=Join-Path $root 'analysis/distortion-calibration/e2e-fit-median-context'
$native=@(Get-Content (Join-Path $root 'analysis/distortion-calibration/median-context-replay/runtime.jsonl')|ForEach-Object{$_|ConvertFrom-Json})
$cases=@()
foreach($sequence in @(4,56,63,67,76,83)){
 $entry=[IO.File]::ReadAllBytes((Join-Path $directory ('median-entry-{0:000}-00.bin'-f $sequence)))
 $count=[BitConverter]::ToInt32($entry,0)
 $output=[IO.File]::ReadAllBytes((Join-Path $directory ('median-{0:000}-00.bin'-f $sequence)))
 $zero=@($native|Where-Object {$_.sequence-eq$sequence-and !$_.capturedContext})
 $captured=@($native|Where-Object {$_.sequence-eq$sequence-and $_.capturedContext})
 if($zero.Count-ne1-or$captured.Count-ne1-or$output.Length-ne$count*8){throw 'Missing or non-unique native workspace replay.'}
 $cases+=[ordered]@{sequence=$sequence;entryBase64=[Convert]::ToBase64String($entry);errorsBase64=[Convert]::ToBase64String($output,0,$count*4);zeroContextSigma=$zero[0].sigma;capturedContextSigma=$captured[0].sigma}
}
[ordered]@{
 authority='Original 7fec40 owned-process replay of same parameters with zero versus captured workspace prefix'
 source='tests/DistortionCalibration.NativeProbe/median-context-replay.cpp'
 deviceTests=$false
 entryLayout='int32 count,freedom,prefixCount; float32 coefficients[8],source[2count],target[2count],prefix[prefixCount],initialSorted[count]'
 coreSha256=(Get-FileHash "$InstallRoot/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll").Hash
 cases=$cases
}|ConvertTo-Json -Depth 10|Set-Content (Join-Path $root 'docs/plugins/distortion-calibration-workspace-evidence.json')
Write-Output 'Exported six native contexts with twelve median results.'
