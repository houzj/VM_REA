#Requires -Version 7.0
[CmdletBinding()]
param([string]$OutputRoot = (Join-Path $PSScriptRoot '../../../analysis/full-audit/framework-numeric-ranges'))
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $false
$repo = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$dotnet = (Get-Command dotnet -CommandType Application).Source
$batch = (New-Item -ItemType Directory -Path (Join-Path $OutputRoot ('regression-' + [Guid]::NewGuid().ToString('N')))).FullName
$artifacts = Join-Path $batch 'build'
# Reviewed console/control-object suites. Window.Show, popups, host windows,
# vendor SDK probes and device tests are deliberately outside this runner.
$suites = @(
 @{ name='VisionConfiguration.NumericRangeChecks'; pattern='^PASS (\d+) headless numeric range checks'; minimum=142; audit=$true }
 @{ name='VisionConfiguration.ModelChecks'; pattern='^VisionConfiguration.ModelChecks: (\d+) checks passed'; minimum=1 }
 @{ name='VisionConfiguration.StateChecks'; pattern='^VisionConfiguration.StateChecks: (\d+) checks passed'; minimum=1692 }
 @{ name='VisionConfiguration.OperationChecks'; pattern='^(\d+) configuration operation/store/localization checks passed'; minimum=1 }
 @{ name='VisionConfiguration.TemplateChecks'; pattern='^Configuration template metadata: (\d+) checks passed'; minimum=1 }
 @{ name='VisionConfiguration.PointAliasChecks'; pattern='^PASS (\d+) POINT alias/persistence/graph checks'; minimum=1 }
 @{ name='VisionConfiguration.WorkflowSubscriptionChecks'; pattern='^VisionConfiguration.WorkflowSubscriptionChecks: (\d+) checks passed'; minimum=1 }
 @{ name='VisionConfiguration.ObjectChecks'; pattern='^PASS (\d+) object binding checks'; minimum=1 }
 @{ name='VisionConfiguration.NamedTextChecks'; pattern='^PASS (\d+) named text feature checks'; minimum=1 }
 @{ name='VisionConfiguration.DisplaySubscriptionChecks'; pattern='^PASS (\d+) display subscription checks'; minimum=1 }
 @{ name='ContourMatch.ResultChecks'; pattern='^PASS (\d+) production result/configuration checks'; minimum=1 }
)
$results = foreach ($suite in $suites) {
 $name = $suite.name
 $directory = (New-Item -ItemType Directory -Path (Join-Path $batch $name)).FullName
 $record = [ordered]@{ project=$name; buildExit=$null; testExit=$null; checks=$null; assemblySha256=$null; error=$null }
 try {
  & $dotnet build (Join-Path $repo "tests/$name/$name.csproj") -c Release --artifacts-path $artifacts --disable-build-servers -p:UseSharedCompilation=false -m:1 --nologo *> (Join-Path $directory 'build.log')
  $record.buildExit = $LASTEXITCODE
  if ($record.buildExit -ne 0) { throw 'Build failed; see build.log.' }
  $files = @(Get-ChildItem -LiteralPath (Join-Path $artifacts "bin/$name") -Recurse -File -Filter "$name.dll")
  if ($files.Count -ne 1) { throw 'Expected one test assembly.' }
  if ($name -eq 'VisionConfiguration.TemplateChecks') {
   $evidenceDirectory = (New-Item -ItemType Directory -Path (Join-Path $directory 'docs/plugins') -Force).FullName
   Copy-Item -LiteralPath (Join-Path $repo 'docs/plugins/configuration-template-catalog-evidence.json') -Destination $evidenceDirectory
  }
  $record.assemblySha256 = (Get-FileHash -LiteralPath $files[0].FullName).Hash
  $start = [Diagnostics.ProcessStartInfo]::new($dotnet)
  $start.UseShellExecute=$false; $start.CreateNoWindow=$true; $start.RedirectStandardOutput=$true; $start.RedirectStandardError=$true
  $start.WorkingDirectory=$directory; $start.ArgumentList.Add($files[0].FullName)
  if ($suite.audit) { $start.ArgumentList.Add((Join-Path $directory 'audit')) }
  $process = [Diagnostics.Process]::Start($start)
  try {
   $stdout=$process.StandardOutput.ReadToEndAsync(); $stderr=$process.StandardError.ReadToEndAsync()
   if (!$process.WaitForExit(60000)) { $process.Kill($true); $process.WaitForExit(); throw 'Test exceeded the 60-second bound.' }
   $record.testExit=$process.ExitCode
   $log=$stdout.GetAwaiter().GetResult()+$stderr.GetAwaiter().GetResult()
   [IO.File]::WriteAllText((Join-Path $directory 'run.log'), $log)
  } finally { $process.Dispose() }
  if ($record.testExit -ne 0) { throw 'Test failed; see run.log.' }
  $match=[regex]::Match($log, '(?m)'+$suite.pattern)
  if (!$match.Success) { throw 'Completion count is absent from the test log.' }
  $record.checks=[int]$match.Groups[1].Value
  if ($record.checks -lt $suite.minimum) { throw 'Observed count is below the required coverage baseline.' }
  Write-Host "PASS $name ($($record.checks))"
 } catch { $record.error=$_.Exception.Message; Write-Host "FAIL $name $($record.error)" }
 [pscustomobject]$record
}
$passed=@($results | Where-Object { !$_.error })
$summary=Join-Path $batch 'summary.json'
[ordered]@{ scope='Shared configuration headless regression; no Window.Show, host launch, vendor SDK or device execution'; runnerSha256=(Get-FileHash -LiteralPath $PSCommandPath).Hash; passedSuites=$passed.Count; passedChecks=($passed | Measure-Object checks -Sum).Sum; suites=@($results) } | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $summary -Encoding utf8
Write-Output $summary
if ($passed.Count -ne $suites.Count) { throw 'One or more suites failed.' }
