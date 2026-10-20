$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/b1-solve-source-20261019'
$baseline=Join-Path $root 'analysis/camera-mapping/reachable-matrix-20261019'
$solver=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingSolver.cs';$algebra=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingLinearAlgebra.cs'
$hashes=@((Get-FileHash $solver).Hash,(Get-FileHash $algebra).Hash)
$wrapper=@'
public static class B1Solver24 {
 public static VisionPlugins.CameraMapping.CameraMappingSolution Run(double[][] points) => VisionPlugins.CameraMapping.CameraMappingSolver.Solve(points.Select(p=>new VisionPlugins.CameraMapping.CameraMappingPair(p[0],p[1],p[2],p[3])).ToArray());
}
'@
$code="using System;`nusing System.Linq;`nusing System.Collections.Generic;`n"+[IO.File]::ReadAllText($solver).Replace('namespace VisionPlugins.CameraMapping;','namespace VisionPlugins.CameraMapping {')+"`n}`n"+[IO.File]::ReadAllText($algebra).Replace('namespace VisionPlugins.CameraMapping;','namespace VisionPlugins.CameraMapping {')+"`n}`n"+$wrapper
Add-Type -TypeDefinition $code -CompilerOptions '/nullable:enable'
$cases=Get-Content "$baseline/cases.json" -Raw|ConvertFrom-Json
$native=@(Get-Content "$baseline/results.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
$frames=@(Get-Content "$baseline/module-results.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
$rows=@(foreach($case in $cases){
 $solution=[B1Solver24]::Run([double[][]]$case.points);$n=$native|Where-Object{$_.case -eq $case.case};$f=$frames|Where-Object{$_.phase -eq $case.case}
 $equal=@(for($i=0;$i -lt 9;$i++){[BitConverter]::SingleToInt32Bits([float]$solution.Matrix[$i]) -eq [BitConverter]::SingleToInt32Bits([float]$n.matrix[$i])})
 $sx=($f.writes|Where-Object{$_.name -eq 'ScaleX'}).values[0];$sy=($f.writes|Where-Object{$_.name -eq 'ScaleY'}).values[0]
 [ordered]@{case=$case.case;error=$solution.ErrorCode;matrix=$solution.Matrix;nativeMatrix=$n.matrix;allMatrixFloatBitsEqual=@($equal|Where-Object{!$_}).Count -eq 0;scaleXEqual=[BitConverter]::SingleToInt32Bits([float]$solution.RatioX) -eq [BitConverter]::SingleToInt32Bits([float]$sx);scaleYEqual=[BitConverter]::SingleToInt32Bits([float]$solution.RatioY) -eq [BitConverter]::SingleToInt32Bits([float]$sy)}
})
$summary=[ordered]@{scope='Exact current solver source, isolated Add-Type; actual runtime output is separately recorded';cases=$rows.Count;success=@($rows|Where-Object{$_.error -eq 0}).Count;matrixBitMatches=@($rows|Where-Object{$_.allMatrixFloatBitsEqual}).Count;ratioBitMatches=@($rows|Where-Object{$_.scaleXEqual -and $_.scaleYEqual}).Count;sourceHashes=$hashes}
if(($hashes -join '|') -ne (@((Get-FileHash $solver).Hash,(Get-FileHash $algebra).Hash) -join '|')){throw 'Source changed during solver comparison'}
[ordered]@{summary=$summary;comparisons=$rows}|ConvertTo-Json -Depth 12|Set-Content "$out/solver24-after.json" -Encoding utf8
$summary|ConvertTo-Json
