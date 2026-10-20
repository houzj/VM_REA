param([Parameter(Mandatory=$true)][ValidateSet('MapCalibration.GridRoiChecks','MapCalibration.RoiRuntimeChecks','MapCalibration.SourceRecognitionChecks')][string]$Project)
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root ('analysis/map-calibration/r04-map-roi-verification/'+$Project+'-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $out|Out-Null
$artifacts=Join-Path $out 'artifacts'
$reports=@()
foreach($name in @($Project)){
 $projectFile=Join-Path $root "tests/$name/$name.csproj"
 $watch=[Diagnostics.Stopwatch]::StartNew()
 & dotnet build $projectFile --artifacts-path $artifacts --ignore-failed-sources -m:1 -p:UseSharedCompilation=false -v:minimal *> "$out/$name-build.log"
 $buildExit=$LASTEXITCODE;$runExit=$null
 if($buildExit -eq 0){
  $dll=Get-ChildItem -LiteralPath "$artifacts/bin/$name" -Recurse -Filter "$name.dll" -File|Select-Object -First 1
  if($null -eq $dll){throw "No output DLL for $name"}
  & dotnet $dll.FullName *> "$out/$name-run.log"
  $runExit=$LASTEXITCODE
 }
 $watch.Stop();$reports+=@{project=$name;buildExit=$buildExit;runExit=$runExit;seconds=$watch.Elapsed.TotalSeconds}
 $reports|ConvertTo-Json -Depth 5|Set-Content "$out/results.json" -Encoding utf8
 Write-Output "$name build=$buildExit run=$runExit seconds=$($watch.Elapsed.TotalSeconds)"
 if($buildExit -eq 0){Get-Content "$out/$name-run.log" -Tail 2}else{Get-Content "$out/$name-build.log" -Tail 5}
}
if(@($reports|Where-Object {$_.buildExit -ne 0 -or $_.runExit -ne 0}).Count -gt 0){exit 1}


