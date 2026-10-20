$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/b2-matrix-source-20261019'
$algebra=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingLinearAlgebra.cs'
$source=[IO.File]::ReadAllText($algebra).Replace('namespace VisionPlugins.CameraMapping;','namespace VisionPlugins.CameraMapping {')+"`n}"
$projectionInverse=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingMatrixToParamInverse.cs'
if(Test-Path $projectionInverse){$source+="`n"+[IO.File]::ReadAllText($projectionInverse).Replace('namespace VisionPlugins.CameraMapping;','namespace VisionPlugins.CameraMapping {')+"`n}"}
$code=@'
using System;
using System.Linq;
using System.Collections.Generic;
public static class B2Projection {
 public static double[][]? Production(double[] matrix){
  var inverse=VisionPlugins.CameraMapping.CameraMappingMatrixToParamInverse.Inverse(matrix);if(inverse is null)return null;
  float reciprocal=1f/(float)inverse[8];var normalized=inverse.Select(v=>(double)((float)v*reciprocal)).ToArray();
  return new[]{inverse,normalized,Project(normalized)};
 }
 public static float CurrentDeterminant(double[] matrix){
  var m=matrix.Select(v=>(float)v).ToArray();
  return (((m[8]*m[0]*m[4]+m[6]*m[1]*m[5]+m[7]*m[2]*m[3])-m[7]*m[0]*m[5])-m[8]*m[1]*m[3])-m[6]*m[4]*m[2];
 }
 public static double[][] NativeOrder(double[] matrix){
  var m=matrix.Select(v=>(float)v).ToArray();
  float p04=m[0]*m[4],p15=m[1]*m[5],p23=m[2]*m[3],p05=m[0]*m[5],p13=m[1]*m[3],p42=m[4]*m[2];
  float determinant=(((m[8]*p04+m[6]*p15+m[7]*p23)-m[7]*p05)-m[8]*p13)-m[6]*p42;
  float r=1f/determinant;
  double[] inverse=[(m[4]*m[8]-m[5]*m[7])*r,(m[2]*m[7]-m[8]*m[1])*r,(p15-p42)*r,
   (m[5]*m[6]-m[8]*m[3])*r,(m[8]*m[0]-m[2]*m[6])*r,(p23-p05)*r,
   (m[3]*m[7]-m[6]*m[4])*r,(m[1]*m[6]-m[0]*m[7])*r,(p04-p13)*r];
  float reciprocal=1f/(float)inverse[8];
  return new[]{inverse,inverse.Select(v=>(double)((float)v*reciprocal)).ToArray(),new double[]{determinant,reciprocal}};
 }
 public static double[] Project(double[] inv){
  const float epsilon=1.1920928955078125e-7f;float theta=MathF.Atan2((float)inv[3],(float)inv[0]);float scale=(float)inv[0]/(MathF.Cos(theta)+epsilon);float skew=MathF.Atan2(-(float)inv[1],(float)inv[4])-theta;if(skew< -MathF.PI)skew+=2*MathF.PI;if(skew>MathF.PI)skew-=2*MathF.PI;
  return new double[]{scale,inv[2],inv[5],theta,-(float)inv[1]/(MathF.Sin(theta+skew)*scale+epsilon),skew};
 }
 public static double[][] Evaluate(double[] matrix){
  var inverse=VisionPlugins.CameraMapping.CameraMappingLinearAlgebra.Inverse(matrix)!;
  float reciprocal=1f/(float)inverse[8];
  var divided=inverse.Select(v=>(double)((float)v/(float)inverse[8])).ToArray();
  var multiplied=inverse.Select(v=>(double)((float)v*reciprocal)).ToArray();
  return new[]{inverse,divided,multiplied,new double[]{reciprocal}};
 }
}
'@
Add-Type -TypeDefinition ($code+"`n"+$source) -CompilerOptions '/nullable:enable'
$native=@(Get-Content "$root/analysis/camera-mapping/reachable-matrix-20261019/results.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
$trace=@(Get-Content "$out/results.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
$rows=@(foreach($name in @('seed-33-extent-1000-grid-4','seed-15-extent-10000-grid-4','seed-16-extent-10-grid-4','seed-15-extent-1000-grid-4','seed-33-extent-1000-grid-2')){
 $record=$native|Where-Object{$_.case -eq $name};$steps=[B2Projection]::Evaluate([double[]]$record.matrix)
 $raw=$trace|Where-Object{$_.case -eq $name -and $_.phase -eq 'inverse-before-normalize'}
 $normalized=$trace|Where-Object{$_.case -eq $name -and $_.phase -eq 'inverse-after-normalize'}
 $rawEqual=@(for($i=0;$i -lt 9;$i++){[BitConverter]::SingleToInt32Bits([float]$raw.inverse[$i]) -eq [BitConverter]::SingleToInt32Bits([float]$steps[0][$i])})
 $divisionEqual=@(for($i=0;$i -lt 8;$i++){[BitConverter]::SingleToInt32Bits([float]$normalized.firstEight[$i]) -eq [BitConverter]::SingleToInt32Bits([float]$steps[1][$i])})
 $multiplyEqual=@(for($i=0;$i -lt 8;$i++){[BitConverter]::SingleToInt32Bits([float]$normalized.firstEight[$i]) -eq [BitConverter]::SingleToInt32Bits([float]$steps[2][$i])})
 $projected=[B2Projection]::Project($steps[2]);$fields=@('scale','offsetX','offsetY','rotate','aspect','skew')
 $sixEqual=@(for($i=0;$i -lt 6;$i++){[BitConverter]::SingleToInt32Bits([float]$record.result.($fields[$i])) -eq [BitConverter]::SingleToInt32Bits([float]$projected[$i])})
 $nativeOrder=[B2Projection]::NativeOrder([double[]]$record.matrix)
 $nativeOrderRawEqual=@(for($i=0;$i -lt 9;$i++){[BitConverter]::SingleToInt32Bits([float]$raw.inverse[$i]) -eq [BitConverter]::SingleToInt32Bits([float]$nativeOrder[0][$i])})
 $determinant=$trace|Where-Object{$_.case -eq $name -and $_.phase -eq 'determinant'}
 $candidateNativeOrder=[B2Projection]::Project($nativeOrder[1])
 $nativeOrderSixEqual=@(for($i=0;$i -lt 6;$i++){[BitConverter]::SingleToInt32Bits([float]$record.result.($fields[$i])) -eq [BitConverter]::SingleToInt32Bits([float]$candidateNativeOrder[$i])})
 [ordered]@{case=$name;scope='Independent generic-versus-instruction-order inverse comparison; production now uses its specialized inverse';matrix=$record.matrix;inverse=$steps[0];reciprocal=$steps[3][0];divisionNormalized=$steps[1];reciprocalMultiplicationNormalized=$steps[2];nativeOffsetX=$record.result.offsetX;nativeOffsetY=$record.result.offsetY;rawInverseBitsEqual=$rawEqual;divisionNormalizedBitsEqual=$divisionEqual;multiplicationNormalizedBitsEqual=$multiplyEqual;candidateProjection=$projected;candidateSixFieldsEqual=$sixEqual;nativeDeterminant=$determinant.value;currentDeterminant=[B2Projection]::CurrentDeterminant([double[]]$record.matrix);instructionOrderDeterminant=$nativeOrder[2][0];instructionOrderInverse=$nativeOrder[0];instructionOrderRawBitsEqual=$nativeOrderRawEqual;instructionOrderSixFieldsEqual=$nativeOrderSixEqual}
})
$rows|ConvertTo-Json -Depth 12|Tee-Object "$out/normalization-only-diagnostic.json"
