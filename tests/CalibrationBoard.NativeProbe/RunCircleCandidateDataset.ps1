param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$audit=Join-Path $root 'analysis/calibration-board/circle-candidates'
& "$PSScriptRoot/RunCircleCandidate.ps1" -InstallRoot $InstallRoot -Group 0
$results=@()
for($group=0;$group -lt 36;$group++){
 if($group -gt 0){
  $process=Start-Process -FilePath "$audit/probe.exe" -ArgumentList @("`"$InstallRoot/MVDAlgorithmSDK/Runtime/x64`"","$group") -WorkingDirectory $audit -WindowStyle Hidden -PassThru -RedirectStandardOutput "$audit/runtime-group-$group.jsonl" -RedirectStandardError "$audit/stderr-group-$group.log"
  if(!$process.WaitForExit(30000)){$process.Kill();throw "Owned candidate group $group timeout"}
  $process.Refresh();if($process.ExitCode -ne 0){throw "Candidate group $group exit $($process.ExitCode)"}
 }
 $rows=@(Get-Content "$audit/runtime-group-$group.jsonl" | ForEach-Object {$_|ConvertFrom-Json})
 if($rows.Count -ne 10){throw "Candidate group $group has $($rows.Count) rows"}
 $results+=@{group=$group;count=$rows.Count;sha256=(Get-FileHash "$audit/runtime-group-$group.jsonl").Hash.ToLowerInvariant()}
 Write-Output "Completed original candidate group $group (10 cases)"
}
@{groups=$results;sourceSha256=(Get-FileHash "$PSScriptRoot/CircleCandidateProbe.cpp").Hash.ToLowerInvariant();executableSha256=(Get-FileHash "$audit/probe.exe").Hash.ToLowerInvariant();deviceTests=$false}|ConvertTo-Json -Depth 5|Set-Content "$audit/dataset-summary.json"
