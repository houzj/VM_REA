$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/b1-solve-source-20261019'
$solver=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingSolver.cs'
$algebra=Join-Path $root 'src/VisionPlugins.CameraMapping/CameraMappingLinearAlgebra.cs'
$before=@((Get-FileHash $solver).Hash,(Get-FileHash $algebra).Hash)
$s=[IO.File]::ReadAllText($solver)
$s=$s.Replace('public static class CameraMappingSolver {','public static class CameraMappingSolver { public static Action<string, object>? Trace;')
$s=$s.Replace('var weights=Enumerable.Repeat(1d,points.Length).ToArray();', 'Trace?.Invoke("normalized",new { points=normalized.Select(p=>new[]{p.ObjectX,p.ObjectY,p.TargetX,p.TargetY}).ToArray(), sourceNormalize=new float[]{ss,0,-ss*mx,0,ss,-ss*my,0,0,1},targetNormalize=new float[]{st,0,-st*mu,0,st,-st*mv,0,0,1}}); var weights=Enumerable.Repeat(1d,points.Length).ToArray();')
$s=$s.Replace('var next=FitLinear(normalized,freedom,weights,true,true);','Trace?.Invoke("weights", new { iteration, errors, cutoff, weights=weights.ToArray(), matrix=h.ToArray() }); var next=FitLinear(normalized,freedom,weights,true,true);')
$s=$s.Replace('var result=CameraMappingLinearAlgebra.Solve(rows,targets,rowWeights,nested);return', 'var result=CameraMappingLinearAlgebra.Solve(rows,targets,rowWeights,nested);Trace?.Invoke("fit",new {nested, rows=rows.ToArray(), targets=targets.ToArray(), weights=rowWeights.ToArray(), result});return')
$s=$s.Replace('if(h[8]==0)return h;','Trace?.Invoke("roundtrip",new {denormalized=denormalized.ToArray(),renormalized=h.ToArray()}); if(h[8]==0)return h;')
$a=[IO.File]::ReadAllText($algebra)
$a=$a.Replace('var a = new float[n, n]; var b = new float[n];','CameraMappingSolver.Trace?.Invoke("normal",new { nested, dimension=n, matrix=Enumerable.Range(0,n*n).Select(i=>normal[i/n,i%n]).ToArray(), rhs=rhs.ToArray() }); var a = new float[n, n]; var b = new float[n];')
$wrapper=@'
public static class B1Snapshot {
 public static VisionPlugins.CameraMapping.CameraMappingSolution Run(double[][] points) => VisionPlugins.CameraMapping.CameraMappingSolver.Solve(points.Select(x=>new VisionPlugins.CameraMapping.CameraMappingPair(x[0],x[1],x[2],x[3])).ToArray());
 // Independent diagnostic transcription of recovered 7fcf60 -> 7fd4e0
 // transformation order. It does not alter the source solver execution above.
 public static double[][] RoundTrip(double[] hn, double[] ns, double[] nt) {
  double[] Product(double[] a,double[] b)=>Enumerable.Range(0,9).Select(i=>{float sum=0;for(int k=0;k<3;k++)sum+=(float)a[i/3*3+k]*(float)b[k*3+i%3];return(double)sum;}).ToArray();
  double[] Normalize(double[] h){float r=1f/(float)h[8];return h.Select(x=>(double)((float)x*r)).ToArray();}
  var denormalized=Normalize(Product(Product(VisionPlugins.CameraMapping.CameraMappingLinearAlgebra.Inverse(nt)!,hn),ns));
  var renormalized=Normalize(Product(Product(nt,denormalized),VisionPlugins.CameraMapping.CameraMappingLinearAlgebra.Inverse(ns)!));
  return new[]{denormalized,renormalized};
 }
}
'@
$code="using System;`nusing System.Linq;`nusing System.Collections.Generic;`n"+$s.Replace('namespace VisionPlugins.CameraMapping;','namespace VisionPlugins.CameraMapping {')+"`n}`n"+$a.Replace('namespace VisionPlugins.CameraMapping;','namespace VisionPlugins.CameraMapping {')+"`n}`n"+$wrapper
[IO.File]::WriteAllText((Join-Path $out 'instrumented-source-snapshot.cs'),$code)
Add-Type -TypeDefinition $code -CompilerOptions '/nullable:enable'
$events=[Collections.Generic.List[object]]::new()
$script:b1name=''
[VisionPlugins.CameraMapping.CameraMappingSolver]::Trace=[Action[string,object]]{param($phase,$data) $events.Add([ordered]@{case=$script:b1name;phase=$phase;data=$data})}
$cases=Get-Content (Join-Path $out 'cases.json') -Raw|ConvertFrom-Json
foreach($case in $cases){$script:b1name=$case.case;$solution=[B1Snapshot]::Run([double[][]]$case.points);$events.Add([ordered]@{case=$case.case;phase='result';data=$solution})}
$events|ForEach-Object{$_|ConvertTo-Json -Depth 14 -Compress}|Set-Content (Join-Path $out 'source-trace.jsonl') -Encoding utf8
$after=@((Get-FileHash $solver).Hash,(Get-FileHash $algebra).Hash)
if(($before -join '|') -ne ($after -join '|')){throw 'Product source changed during snapshot'}
[ordered]@{scope='Instrumented unchanged production source statements in isolated Add-Type, not plugin runtime';sourceHashes=$before;sourceChanged=$false;events=$events.Count}|ConvertTo-Json|Set-Content (Join-Path $out 'source-trace-summary.json') -Encoding utf8
$events|Group-Object phase|Select-Object Name,Count
$native=@(Get-Content (Join-Path $out 'module-results.jsonl')|ForEach-Object{$_|ConvertFrom-Json})
$source=@(Get-Content (Join-Path $out 'source-trace.jsonl')|ForEach-Object{$_|ConvertFrom-Json})
$roundTrips=@(foreach($case in $cases){
 $fit=@($source|Where-Object{$_.case -eq $case.case -and $_.phase -eq 'fit'})[0]
 $norm=$source|Where-Object{$_.case -eq $case.case -and $_.phase -eq 'normalized'}
 $r=@($native|Where-Object{$_.case -eq $case.case -and $_.phase -eq 'b1-residual-entry'})[0]
 $entry=$native|Where-Object{$_.case -eq $case.case -and $_.phase -eq 'b1-refine-entry'}
 $matrices=[B1Snapshot]::RoundTrip([double[]](@($fit.data.result)+@(1)),[double[]]$norm.data.sourceNormalize,[double[]]$norm.data.targetNormalize)
 $diff=@(for($i=0;$i -lt 8;$i++){if([BitConverter]::SingleToInt32Bits([float]$matrices[1][$i]) -ne [BitConverter]::SingleToInt32Bits([float]$r.coefficients[$i])){[ordered]@{index=$i;native=$r.coefficients[$i];diagnostic=$matrices[1][$i]}}})
 $denormDiff=@(for($i=0;$i -lt 9;$i++){if([BitConverter]::SingleToInt32Bits([float]$matrices[0][$i]) -ne [BitConverter]::SingleToInt32Bits([float]$entry.matrix[$i])){[ordered]@{index=$i;native=$entry.matrix[$i];diagnostic=$matrices[0][$i]}}})
 [ordered]@{case=$case.case;scope='Independent source-order roundtrip diagnostic, not product fix';denormalized=$matrices[0];renormalized=$matrices[1];residualInputDifferences=$diff;refineEntryDifferences=$denormDiff}
})
$roundTrips|ConvertTo-Json -Depth 10|Set-Content (Join-Path $out 'roundtrip-diagnostic.json') -Encoding utf8
$roundTrips|ConvertTo-Json -Depth 5
