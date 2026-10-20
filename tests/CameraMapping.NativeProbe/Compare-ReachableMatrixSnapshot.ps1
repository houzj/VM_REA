$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/reachable-matrix-20261019'
$plugin=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingPlugin.cs'
$algebra=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingLinearAlgebra.cs'
$solver=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingSolver.cs'
$before=@((Get-FileHash $plugin).Hash,(Get-FileHash $algebra).Hash,(Get-FileHash $solver).Hash)
$linear=[IO.File]::ReadAllText($algebra)
$scalar=([IO.File]::ReadAllLines($plugin)|Where-Object{$_ -match 'const float epsilon='}) -join "`n"
if(!$scalar){throw 'Missing production scalar expression'}
$wrapper=@"
public static class MatrixProjectionSnapshot {
 public static VisionPlugins.CameraMapping.CameraMappingSolution Solve(double[][] p) => VisionPlugins.CameraMapping.CameraMappingSolver.Solve(p.Select(x=>new VisionPlugins.CameraMapping.CameraMappingPair(x[0],x[1],x[2],x[3])).ToArray());
 public static double[]? Evaluate(double[] h) {
  var inv=VisionPlugins.CameraMapping.CameraMappingLinearAlgebra.Inverse(h);
  if(inv is null||inv[8]==0)return null;
  float normalization=(float)inv[8];inv=inv.Select(v=>(double)((float)v/normalization)).ToArray();
  $scalar
  return new double[]{scale,inv[2],inv[5],theta,-(float)inv[1]/(MathF.Sin(theta+skew)*scale+epsilon),skew};
 }
}
"@
$source="using System;`nusing System.Linq;`nusing System.Collections.Generic;`n"+$linear.Replace('namespace VisionPlugins.CameraMapping;','namespace VisionPlugins.CameraMapping {')+"`n}`n"+$wrapper
$source+="`n"+[IO.File]::ReadAllText($solver).Replace('namespace VisionPlugins.CameraMapping;','namespace VisionPlugins.CameraMapping {')+"`n}"
[IO.File]::WriteAllText((Join-Path $out 'projection-snapshot.cs'),$source)
Add-Type -TypeDefinition $source -CompilerOptions '/nullable:enable'
$names=@('scale','offsetX','offsetY','rotate','aspect','skew')
$rows=@(Get-Content (Join-Path $out 'results.jsonl')|ForEach-Object{
 $record=$_|ConvertFrom-Json
 $actual=[MatrixProjectionSnapshot]::Evaluate([double[]]$record.matrix)
 $deltas=[ordered]@{}; $bits=[ordered]@{}
 if($actual -and $record.result.event -eq 'result'){
  for($i=0;$i -lt 6;$i++){$deltas[$names[$i]]=[Math]::Abs($actual[$i]-$record.result.($names[$i]));$bits[$names[$i]]=[BitConverter]::SingleToInt32Bits([float]$actual[$i]) -eq [BitConverter]::SingleToInt32Bits([float]$record.result.($names[$i]))}
 }
 [ordered]@{case=$record.case;native=$record.result;snapshot=$actual;absoluteDelta=$deltas;equalFloatBits=$bits;exceeds003=@($deltas.Values|Where-Object{$_ -gt 0.003}).Count -gt 0}
})
$rows|ConvertTo-Json -Depth 10|Set-Content (Join-Path $out 'comparison.json') -Encoding utf8
$after=@((Get-FileHash $plugin).Hash,(Get-FileHash $algebra).Hash,(Get-FileHash $solver).Hash)
if(($before -join '|') -ne ($after -join '|')){throw 'Source changed during snapshot comparison'}
[ordered]@{diagnostic='Source formula snapshot; not actual plugin execution';sourceHashes=$before;sourceChanged=$false;cases=$rows.Count;nativeSuccess=@($rows|Where-Object{$_.native.event -eq 'result'}).Count;exceeds003=@($rows|Where-Object{$_.exceeds003}).Count;allSixFloatBitsEqual=@($rows|Where-Object{$_.native.event -eq 'result' -and @($_.equalFloatBits.Values|Where-Object{!$_}).Count -eq 0}).Count;nativeFailures=@($rows|Where-Object{$_.native.event -ne 'result'}).Count}|ConvertTo-Json|Tee-Object (Join-Path $out 'summary.json')
$rows|Where-Object{$_.exceeds003}|ConvertTo-Json -Depth 6

$raw=@(Get-Content (Join-Path $out 'module-results.jsonl')|ForEach-Object{$_|ConvertFrom-Json})
$points=Get-Content (Join-Path $out 'cases.json') -Raw|ConvertFrom-Json
$order=[ordered]@{TranslateX=1;TranslateY=2;Rotate=3;Scale=0;Skew=5;Aspect=4}
$nativeNames=@('scale','offsetX','offsetY','rotate','aspect','skew')
$details=@(foreach($record in $points){
 $row=$rows|Where-Object{$_.case -eq $record.case};$frame=$raw|Where-Object{$_.phase -eq $record.case};$sdk=$raw|Where-Object{$_.case -eq $record.case}
 $writeEqual=$true;$firstDifferent=$null;$firstOver003=$null
 foreach($name in $order.Keys){
  $i=$order[$name];$write=$frame.writes|Where-Object{$_.name -eq $name}
  if([BitConverter]::SingleToInt32Bits([float]$write.values[0]) -ne [BitConverter]::SingleToInt32Bits([float]$row.native.($nativeNames[$i]))){$writeEqual=$false}
  if(!$row.equalFloatBits.($nativeNames[$i]) -and !$firstDifferent){$firstDifferent=$name}
  if($row.absoluteDelta.($nativeNames[$i]) -gt 0.003 -and !$firstOver003){$firstOver003=$name}
 }
 $solved=[MatrixProjectionSnapshot]::Solve([double[][]]$record.points)
 $matrixDelta=if($solved.Success){@(0..8|ForEach-Object{[Math]::Abs($solved.Matrix[$_]-$sdk.matrix[$_])})}else{@()}
 $projection=if($solved.Success){[MatrixProjectionSnapshot]::Evaluate($solved.Matrix)}else{$null}
 $wholeDeltas=[ordered]@{};if($projection){for($i=0;$i -lt 6;$i++){$wholeDeltas[$nativeNames[$i]]=[Math]::Abs($projection[$i]-$row.native.($nativeNames[$i]))}}
 [ordered]@{case=$record.case;moduleCode=$frame.code;moduleWrites=$frame.writes;moduleWritesEqualSdkGetters=$writeEqual;firstDifferentProjectionField=$firstDifferent;firstProjectionFieldOver003=$firstOver003;sourceSolverError=$solved.ErrorCode;sourceSolverMatrix=$solved.Matrix;sourceSolverMatrixDelta=$matrixDelta;sourceSolverProjection=$projection;sourceSolverProjectionDeltas=$wholeDeltas}
})
$details|ConvertTo-Json -Depth 10|Set-Content (Join-Path $out 'module-and-solver-comparison.json') -Encoding utf8
[ordered]@{cases=$details.Count;moduleSuccess=@($details|Where-Object{$_.moduleCode -eq 0}).Count;moduleWritesEqualSdkGetters=@($details|Where-Object{$_.moduleWritesEqualSdkGetters}).Count;sourceSolverSuccess=@($details|Where-Object{$_.sourceSolverError -eq 0}).Count;sourceSolverMatrixOver003=@($details|Where-Object{@($_.sourceSolverMatrixDelta|Where-Object{$_ -gt 0.003}).Count -gt 0}).Count;sourceSolverProjectionOver003=@($details|Where-Object{@($_.sourceSolverProjectionDeltas.Values|Where-Object{$_ -gt 0.003}).Count -gt 0}).Count}|ConvertTo-Json|Tee-Object (Join-Path $out 'module-and-solver-summary.json')