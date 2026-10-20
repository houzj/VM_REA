$ErrorActionPreference='Stop'
$null=. "$PSScriptRoot/B2ProjectionSnapshot.ps1"
$records=@(Get-Content "$root/analysis/camera-mapping/matrix-differential-20261019/results.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
$fields=@('scale','offsetX','offsetY','rotate','aspect','skew')
$reports=@(foreach($record in $records){
 if($record.result.event -ne 'result'){
  [ordered]@{case=$record.case;native=$record.result;status='Native failure boundary retained; formula diagnostic does not implement SDK exceptions'}
  continue
 }
 $steps=[B2Projection]::Production([double[]]$record.matrix)
 if($null -eq $steps){throw "Production inverse unexpectedly rejected native-success case $($record.case)"}
 $projected=$steps[2];$deltas=[ordered]@{};$bits=[ordered]@{}
 for($i=0;$i -lt 6;$i++){
  $deltas[$fields[$i]]=[Math]::Abs($projected[$i]-$record.result.($fields[$i]))
  $bits[$fields[$i]]=[BitConverter]::SingleToInt32Bits([float]$projected[$i]) -eq [BitConverter]::SingleToInt32Bits([float]$record.result.($fields[$i]))
 }
 [ordered]@{case=$record.case;status='Compared successful matrix';native=$record.result;projection=$projected;absoluteDelta=$deltas;floatBitsEqual=$bits;passed=@($deltas.Values|Where-Object{!($_ -lt 0.003)}).Count -eq 0;allSixBitsEqual=@($bits.Values|Where-Object{!$_}).Count -eq 0}
})
$summary=[ordered]@{scope='Current production inverse source and unchanged Runtime scalar expressions in isolated Add-Type; actual plugin runtime is separately recorded';inputCases=$reports.Count;successCompared=@($reports|Where-Object{$_.status -eq 'Compared successful matrix'}).Count;matching=@($reports|Where-Object{$_.passed}).Count;allSixBitsEqual=@($reports|Where-Object{$_.allSixBitsEqual}).Count;nativeFailuresNotImplemented=2;tolerance=0.003}
[ordered]@{summary=$summary;comparisons=$reports}|ConvertTo-Json -Depth 14|Set-Content "$out/direct95-candidate-diagnostic.json" -Encoding utf8
$summary|ConvertTo-Json
