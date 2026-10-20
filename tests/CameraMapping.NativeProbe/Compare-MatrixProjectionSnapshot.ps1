$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/matrix-differential-20261019'
$plugin=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingPlugin.cs'
$algebra=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingLinearAlgebra.cs'
$before=@((Get-FileHash $plugin).Hash,(Get-FileHash $algebra).Hash)
$linear=[IO.File]::ReadAllText($algebra)
$scalar=([IO.File]::ReadAllLines($plugin)|Where-Object{$_ -match 'const float epsilon='}) -join "`n"
if(!$scalar){throw 'Missing production scalar expression'}
$wrapper=@"
public static class MatrixProjectionSnapshot {
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
$after=@((Get-FileHash $plugin).Hash,(Get-FileHash $algebra).Hash)
if(($before -join '|') -ne ($after -join '|')){throw 'Source changed during snapshot comparison'}
[ordered]@{diagnostic='Source formula snapshot; not actual plugin execution';sourceHashes=$before;sourceChanged=$false;cases=$rows.Count;nativeSuccess=@($rows|Where-Object{$_.native.event -eq 'result'}).Count;exceeds003=@($rows|Where-Object{$_.exceeds003}).Count;allSixFloatBitsEqual=@($rows|Where-Object{$_.native.event -eq 'result' -and @($_.equalFloatBits.Values|Where-Object{!$_}).Count -eq 0}).Count;nativeFailures=@($rows|Where-Object{$_.native.event -ne 'result'}).Count}|ConvertTo-Json|Tee-Object (Join-Path $out 'summary.json')
$rows|Where-Object{$_.exceeds003}|ConvertTo-Json -Depth 6
