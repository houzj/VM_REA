param()
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=Join-Path $root ('analysis/board-host-acceptance/verified-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $run | Out-Null
# Existing checks use relative src/ and analysis/ paths. Isolate their generated files.
New-Item -ItemType Junction -Path (Join-Path $run 'src') -Target (Join-Path $root 'src') | Out-Null
function Snapshot {
 @(& rg --files (Join-Path $root 'src') $PSScriptRoot -g '*.cs' | Sort-Object | ForEach-Object {
  @{path=[IO.Path]::GetRelativePath($root,$_);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()}
 })
}
$before=@(Snapshot)
$before | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $run 'sources.json')
& dotnet build (Join-Path $PSScriptRoot 'VisionConfiguration.RoiHostChecks.csproj') -o "$run/build" --artifacts-path "$run/artifacts" --nologo -v:q -m:1 -p:BuildInParallel=false *> "$run/build.log"
if($LASTEXITCODE -ne 0){throw "Host checks build failed: $run/build.log"}
$process=Start-Process -FilePath "$run/build/VisionConfiguration.RoiHostChecks.exe" -WorkingDirectory $run -WindowStyle Hidden -PassThru -RedirectStandardOutput "$run/stdout.log" -RedirectStandardError "$run/stderr.log"
$elapsed=0
while(!$process.WaitForExit(10000)){$elapsed+=10;if($elapsed -ge 300){$process.Kill();throw "Owned host checks timeout: $run"}}
$process.Refresh();$after=@(Snapshot)
$changed=(@($before|ForEach-Object {$_.path+':'+$_.sha256}) -join '|') -ne (@($after|ForEach-Object {$_.path+':'+$_.sha256}) -join '|')
$result=Get-Content "$run/stdout.log" -Raw
@{exitCode=$process.ExitCode;sourceChangedDuringRun=$changed;result=$result;devices=$false;nativeDesktopParity=$false;manualAccepted=$false;executableSha256=(Get-FileHash "$run/build/VisionConfiguration.RoiHostChecks.exe").Hash.ToLowerInvariant()} | ConvertTo-Json -Depth 4 | Set-Content "$run/summary.json"
Write-Output $result
Write-Output $run
if($process.ExitCode -ne 0 -or $changed){throw 'Host checks failed or source snapshot changed'}
