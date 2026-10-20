$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/b1-solve-source-20261019'
$native=@(Get-Content "$out/module-results.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
$managed=@(Get-Content "$out/source-trace.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
function Compare-Floats($name,$a,$b){
 $diff=@(for($i=0;$i -lt [Math]::Min($a.Count,$b.Count);$i++){
  if([BitConverter]::SingleToInt32Bits([float]$a[$i]) -ne [BitConverter]::SingleToInt32Bits([float]$b[$i])){
   [ordered]@{index=$i;native=[double][float]$a[$i];managed=[double][float]$b[$i]}
  }
 })
 [ordered]@{stage=$name;nativeCount=$a.Count;managedCount=$b.Count;diffCount=$diff.Count;firstDiff=($diff|Select-Object -First 1)}
}
$results=@(foreach($case in @('seed-15-extent-10000-grid-2','seed-15-extent-10-grid-4')){
 $nn=@($native|Where-Object{$_.case -eq $case});$mm=@($managed|Where-Object{$_.case -eq $case})
 $norm=$nn|Where-Object{$_.phase -eq 'b1-normalized'};$mn=$mm|Where-Object{$_.phase -eq 'normalized'}
 $source=@();$target=@();foreach($p in $mn.data.points){$source+=@($p[0],$p[1]);$target+=@($p[2],$p[3])}
 $stages=@(Compare-Floats 'source-normalized-points' $norm.object $source;Compare-Floats 'target-normalized-points' $norm.target $target)
 $fits=@($mm|Where-Object{$_.phase -eq 'fit'});$entries=@($nn|Where-Object{$_.phase -eq 'b1-linear-entry'});$returns=@($nn|Where-Object{$_.phase -eq 'b1-linear-return'})
 $rows=@();foreach($row in $fits[0].data.rows){$rows+=@($row)}
 $stages+=@(Compare-Floats 'initial-design-a' $entries[0].a $rows;Compare-Floats 'initial-target-b' $entries[0].b $fits[0].data.targets;Compare-Floats 'initial-linear-output' $returns[0].solution $fits[0].data.result)
 $residualEntries=@($nn|Where-Object{$_.phase -eq 'b1-residual-entry'});$residualReturns=@($nn|Where-Object{$_.phase -eq 'b1-residual-return'});$weights=@($mm|Where-Object{$_.phase -eq 'weights'})
 $stages+=@(Compare-Floats 'first-residual-input-coefficients' $residualEntries[0].coefficients @($weights[0].data.matrix|Select-Object -First 8);Compare-Floats 'first-residual-errors' $residualReturns[0].errors $weights[0].data.errors)
 $normals=@($mm|Where-Object{$_.phase -eq 'normal'})
 $stages+=@(Compare-Floats 'first-weighted-normal-a' $entries[1].a $normals[1].data.matrix;Compare-Floats 'first-weighted-normal-b' $entries[1].b $normals[1].data.rhs;Compare-Floats 'first-weighted-linear-output' $returns[1].solution $fits[1].data.result)
 for($iteration=2;$iteration -lt [Math]::Min($entries.Count,$fits.Count);$iteration++){
  $stages+=@(Compare-Floats "weighted-$iteration-normal-a" $entries[$iteration].a $normals[$iteration].data.matrix;Compare-Floats "weighted-$iteration-normal-b" $entries[$iteration].b $normals[$iteration].data.rhs;Compare-Floats "weighted-$iteration-linear-output" $returns[$iteration].solution $fits[$iteration].data.result)
 }
 [ordered]@{case=$case;nativeLinearCalls=$entries.Count;managedLinearCalls=$fits.Count;stages=$stages;nativeFirstSigma=$residualReturns[0].sigma;managedFirstCutoff=$weights[0].data.cutoff;warning='Native sigma must still be multiplied by factor 2; sigma and managed cutoff are different stages.'}
})
$results|ConvertTo-Json -Depth 12|Set-Content "$out/first-divergence.json" -Encoding utf8
$results|ConvertTo-Json -Depth 6
