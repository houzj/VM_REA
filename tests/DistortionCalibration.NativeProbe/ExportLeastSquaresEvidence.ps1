param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$cases=@(Get-Content (Join-Path $root 'analysis/distortion-calibration/least-squares/runtime.jsonl')|ForEach-Object{$_|ConvertFrom-Json})
if($cases.Count-ne252-or@($cases|Where-Object {$_.matrix.Count-ne9}).Count){throw 'Expected 84 captured point sets across all three freedoms.'}
[ordered]@{
 authority='Original 1808345e0 WeightFun1 in owned process; RAX 0 is success'
 source='tests/DistortionCalibration.NativeProbe/least-squares-probe.cpp'
 pointPairsEvidence='distortion-calibration-fixed-trace-evidence.json'
 coreSha256=(Get-FileHash "$InstallRoot/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll").Hash
 deviceTests=$false
 cases=$cases
}|ConvertTo-Json -Depth 10|Set-Content (Join-Path $root 'docs/plugins/distortion-calibration-least-squares-evidence.json')
Write-Output 'Exported 252 original WeightFun1 calls.'
