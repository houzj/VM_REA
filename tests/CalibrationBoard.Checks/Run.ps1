param([switch]$AllCandidates,[switch]$RoiOnly,[switch]$GridOnly,[switch]$RecognitionOnly,[switch]$CheckerMaskOnly,[switch]$EncodedMaskOnly,[switch]$EncodedOutputFitOnly,[switch]$EncodedFullProductOnly,[switch]$EnclosingCircleOnly,[switch]$AbandonedParametersOnly,[switch]$BoundaryCandidatesOnly,[switch]$BoundaryRuntimeOnly,[switch]$FailureOutputOnly,[switch]$Circle80FitDiagnosticOnly,[int]$EncodedMaskType=-1,[int]$EncodedMaskStart=0,[switch]$EncodedMaskStrict,[switch]$CheckerRuntimeOnly,[switch]$EncodedMaskRuntimeOnly,[switch]$EncodedAxisRuntimeOnly)
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=Join-Path $root ('analysis/calibration-board/verification-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $run | Out-Null
function SourceSnapshot {
 @(Get-ChildItem (Join-Path $root 'src/VisionPlugins.CalibrationBoard') -File -Filter '*.cs';Get-ChildItem $PSScriptRoot -File -Filter '*.cs';Get-ChildItem (Join-Path $root 'src/VisionPlugins.Calibration') -File -Filter '*.cs') | Sort-Object FullName | ForEach-Object {@{path=[IO.Path]::GetRelativePath($root,$_.FullName).Replace('\','/');sha256=(Get-FileHash $_.FullName).Hash.ToLowerInvariant()}}
}
$sourceSnapshot=@(SourceSnapshot)
$sourceSnapshot | ConvertTo-Json -Depth 5 | Set-Content "$run/sources.json"
& dotnet build (Join-Path $PSScriptRoot 'CalibrationBoard.Checks.csproj') -o "$run/build" --artifacts-path "$run/artifacts" --nologo -v:q -m:1 -p:BuildInParallel=false *> "$run/build.log"
if($LASTEXITCODE -ne 0){Get-Content "$run/build.log";throw 'Build failed'}
Push-Location $root
try {
 $arguments=@();if($EncodedAxisRuntimeOnly){$arguments+="--encoded-axis-runtime-only"};if($EncodedMaskRuntimeOnly){$arguments+="--encoded-mask-runtime-only"};if($CheckerRuntimeOnly){$arguments+="--checker-runtime-only"};if($EncodedMaskStrict){$arguments+="--encoded-mask-strict"};if($EncodedMaskStart -gt 0){$arguments+="--encoded-mask-start=$EncodedMaskStart"};if($EncodedMaskType -ge 0){$arguments+="--encoded-mask-type=$EncodedMaskType"};if($BoundaryRuntimeOnly){$arguments+="--boundary-runtime-only"};if($FailureOutputOnly){$arguments+="--failure-output-only"};if($Circle80FitDiagnosticOnly){$arguments+="--circle80-fit-diagnostic-only"};if($BoundaryCandidatesOnly){$arguments+="--boundary-candidates-only"};if($EnclosingCircleOnly){$arguments+="--enclosing-circle-only"};if($AbandonedParametersOnly){$arguments+="--abandoned-parameters-only"};if($EncodedFullProductOnly){$arguments+="--encoded-full-product-only"};if($EncodedOutputFitOnly){$arguments+="--encoded-output-fit-only"};if($EncodedMaskOnly){$arguments+="--encoded-mask-only"};if($CheckerMaskOnly){$arguments+="--checker-mask-only"};if($RecognitionOnly){$arguments+="--recognition-only"};if($GridOnly){$arguments+="--grid-only"};if($RoiOnly){$arguments+="--roi-only"};if($AllCandidates){$arguments+='--all-candidates'}
 $start=@{FilePath="$run/build/CalibrationBoard.Checks.exe";WorkingDirectory=$root;WindowStyle='Hidden';PassThru=$true;RedirectStandardOutput="$run/stdout.log";RedirectStandardError="$run/stderr.log"};if($arguments.Count){$start.ArgumentList=$arguments}
 $process=Start-Process @start
 $timeout=if($EncodedMaskOnly){900}elseif($AllCandidates){600}else{300};$elapsed=0
 while(!$process.WaitForExit(10000)){$elapsed+=10;if($elapsed -ge $timeout){$process.Kill();throw 'Owned check timeout'}}
 $process.Refresh()
 $scope=if($EncodedAxisRuntimeOnly){"encoded-axis-runtime-only"}elseif($EncodedMaskRuntimeOnly){"encoded-mask-runtime-only"}elseif($CheckerRuntimeOnly){"checker-runtime-only"}elseif($BoundaryRuntimeOnly){"boundary-runtime-only"}elseif($FailureOutputOnly){"failure-output-only"}elseif($Circle80FitDiagnosticOnly){"circle80-fit-diagnostic-only"}elseif($BoundaryCandidatesOnly){"boundary-candidates-only"}elseif($AbandonedParametersOnly){"abandoned-parameters-only"}elseif($EnclosingCircleOnly){"enclosing-circle-only"}elseif($EncodedFullProductOnly){"encoded-full-product-only"}elseif($EncodedOutputFitOnly){"encoded-output-fit-only"}elseif($EncodedMaskOnly){"encoded-mask-only"}elseif($CheckerMaskOnly){"checker-mask-only"}elseif($RecognitionOnly){"recognition-only"}elseif($GridOnly){"grid-only"}elseif($RoiOnly){"roi-only"}elseif($AllCandidates){"all-candidates"}else{"routine"};$summary=@{scope=$scope;encodedMaskType=$EncodedMaskType;encodedMaskStart=$EncodedMaskStart;encodedMaskStrict=[bool]$EncodedMaskStrict;exitCode=$process.ExitCode;devices=$false;desktopParity=$false;manualAccepted=$false;sourceChangedDuringBuild=((@($sourceSnapshot|ForEach-Object {$_.path+":"+$_.sha256}) -join "|") -ne (@(SourceSnapshot|ForEach-Object {$_.path+":"+$_.sha256}) -join "|"));executableSha256=(Get-FileHash "$run/build/CalibrationBoard.Checks.dll").Hash.ToLowerInvariant();sourceEvidence='docs/plugins/calibration-board-evidence.json';result=(Get-Content "$run/stdout.log" -Raw)}
 $summary|ConvertTo-Json|Set-Content "$run/summary.json"
 if($process.ExitCode -ne 0){Get-Content "$run/stderr.log";throw 'Checks failed'}
 Get-Content "$run/stdout.log"
 Write-Output $run
}finally{Pop-Location}
