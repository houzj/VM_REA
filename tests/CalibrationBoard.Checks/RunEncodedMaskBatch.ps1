param([Parameter(Mandatory)][string]$BuiltRun,[Parameter(Mandatory)][ValidateSet(6,7,8,9)][int]$BoardType)
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$build=[IO.Path]::GetFullPath($BuiltRun)
if(!$build.StartsWith($root+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw 'Frozen build must belong to this worktree'}
$exe=Join-Path $build 'build/CalibrationBoard.Checks.exe'
if(!(Test-Path -LiteralPath $exe)){throw 'Missing frozen diagnostic executable'}
$run=Join-Path $root ('analysis/calibration-board/encoded-mask-batch-'+$BoardType+'-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force "$run/docs/plugins","$run/analysis/calibration-board" | Out-Null
$evidence=Get-Content "$root/docs/plugins/calibration-board-encoded-roi-rotated-evidence.json" -Raw | ConvertFrom-Json
$source=@($evidence.sources | Where-Object boardType -eq $BoardType)
foreach($item in $source){$item.image=[IO.Path]::GetFullPath((Join-Path $root $item.image))}
$evidence.sources=$source
$evidence | ConvertTo-Json -Depth 12 | Set-Content "$run/docs/plugins/calibration-board-encoded-roi-rotated-evidence.json"
$rows=@(Get-Content "$root/docs/plugins/calibration-board-encoded-roi-rotated-fixtures.jsonl" | Where-Object {($_|ConvertFrom-Json).boardType -eq $BoardType})
if($rows.Count -ne 21){throw 'Expected exactly 21 original cases'}
$rows | Set-Content "$run/docs/plugins/calibration-board-encoded-roi-rotated-fixtures.jsonl"
Copy-Item -LiteralPath "$build/sources.json" -Destination "$run/sources.json"
$process=Start-Process -FilePath $exe -ArgumentList '--encoded-mask-only' -WorkingDirectory $run -WindowStyle Hidden -PassThru -RedirectStandardOutput "$run/stdout.log" -RedirectStandardError "$run/stderr.log"
$seconds=0
while(!$process.WaitForExit(10000)){$seconds+=10;if($seconds -ge 1200){$process.Kill();throw "Owned diagnostic batch timeout; preserved $run"}}
$process.Refresh()
@{scope='encoded-mask-batch-diagnostic';boardType=$BoardType;exitCode=$process.ExitCode;builtRun=$build;executableSha256=(Get-FileHash "$build/build/CalibrationBoard.Checks.dll").Hash.ToLowerInvariant();cases=21;productParity=$false;devices=$false}|ConvertTo-Json|Set-Content "$run/summary.json"
if($process.ExitCode -ne 0){Get-Content "$run/stderr.log";throw 'Batch diagnostic failed'}
Get-Content "$run/stdout.log"
Write-Output $run
