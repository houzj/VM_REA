param([string]$Artifacts='.a/cmpk',[switch]$ExplicitUnload,[int]$TimeoutSeconds=20,[string]$Tag='bounded-native')
$ErrorActionPreference='Stop'
if($TimeoutSeconds -lt 1 -or $TimeoutSeconds -gt 45){throw 'Diagnostic deadline must remain1..45 seconds'}
$repo=(Resolve-Path "$PSScriptRoot/../..").Path
$dll=Join-Path $repo "$Artifacts/bin/ContourMatch.TrainingPlaneChecks/debug/ContourMatch.TrainingPlaneChecks.dll"
$dir=Join-Path $repo 'analysis/contour-match/training-plane'
if($Tag -notmatch '^[a-zA-Z0-9_-]+$'){throw 'Invalid diagnostic tag'}
$stdout=Join-Path $dir "$Tag.stdout.log";$stderr=Join-Path $dir "$Tag.stderr.log"
$argsList=@($dll,'--native-leaf','--quick-native');if(!$ExplicitUnload){$argsList+='--skip-native-free'}
$child=Start-Process -FilePath (Get-Command dotnet).Source -ArgumentList $argsList -WorkingDirectory $repo -WindowStyle Hidden -RedirectStandardOutput $stdout -RedirectStandardError $stderr -PassThru
$started=$child.StartTime.ToUniversalTime().ToString('o')
$natural=$child.WaitForExit($TimeoutSeconds*1000)
if(!$natural){$child.Kill();if(!$child.WaitForExit(5000)){throw 'Owned child did not exit after bounded stop'}}
$child.Refresh()
[ordered]@{pid=$child.Id;startedUtc=$started;executable='dotnet';arguments=$argsList;timeoutSeconds=$TimeoutSeconds;naturalExit=$natural;explicitNativeUnload=[bool]$ExplicitUnload;actualExitCode=$child.ExitCode;stoppedOnlyOwnChild=!$natural;stdout=$stdout;stderr=$stderr}|ConvertTo-Json -Depth 4|Set-Content (Join-Path $dir "$Tag.exit.json")
Get-Content -LiteralPath $stdout
if(!$natural -or $child.ExitCode -ne 0){exit 1}
