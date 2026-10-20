param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$directory=Join-Path $root 'analysis/distortion-calibration/trace-fit'
$events=Get-Content (Join-Path $directory 'fixed-trace.jsonl')|ForEach-Object{$_|ConvertFrom-Json}
$cases=@()
foreach($entry in $events|Where-Object phase -eq 'fixed-fit-input'){
 $sequence=$entry.sequence
 $returned=@($events|Where-Object {$_.phase -eq 'fixed-fit-output' -and $_.sequence -eq $sequence})
 if($returned.Count -ne 1 -or $returned[0].matrix.Count -ne 9){throw 'Native return is missing or non-unique.'}
 $binary=[IO.File]::ReadAllBytes((Join-Path $directory ('capture-{0:000}.bin' -f $sequence)))
 if($binary.Length -ne $entry.count*16){throw 'Captured point-pair length mismatch.'}
 $cases+=[ordered]@{sequence=$sequence;board=$entry.board;model=$entry.model;warp=$entry.warp;count=$entry.count;options=$entry.options;matrix=$returned[0].matrix;returnRax=$returned[0].returnRax;meanError=$returned[0].meanError;pointPairsBase64=[Convert]::ToBase64String($binary)}
}
[ordered]@{authority='Original 1807ff3c0 entry/return recorded in owned SDK process with restored single-step breakpoints';source='tests/DistortionCalibration.NativeProbe/trace-fit-owned.h';storage='count source float32 xy pairs then count destination float32 xy pairs, little endian';coreSha256=(Get-FileHash "$InstallRoot/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll").Hash;deviceTests=$false;cases=$cases}|ConvertTo-Json -Depth 12|Set-Content (Join-Path $root 'docs/plugins/distortion-calibration-fixed-trace-evidence.json')
Write-Output "Captured $($cases.Count) unique input/return pairs."
