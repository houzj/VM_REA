param([switch]$VerifyFailureExit)
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
Push-Location $root
try {
 dotnet build tests/VisionConfiguration.ControlChecks --no-restore -v:q
 if($LASTEXITCODE -ne 0){throw 'Standard control checks build failed'}
 $out=Join-Path $root ('analysis/full-audit/configuration-controls/run-'+[Guid]::NewGuid().ToString('N'))
 New-Item -ItemType Directory -Force $out|Out-Null
 $options=@{FilePath=(Join-Path $root 'tests/VisionConfiguration.ControlChecks/bin/Debug/net10.0-windows/VisionConfiguration.ControlChecks.exe');WindowStyle='Hidden';RedirectStandardOutput=(Join-Path $out 'stdout.txt');RedirectStandardError=(Join-Path $out 'stderr.txt');PassThru=$true;WorkingDirectory=$root}
 if($VerifyFailureExit){$options.ArgumentList='--verify-failure-exit'}
 $process=Start-Process @options
 if(!$process.WaitForExit(30000)){$process.Kill();throw 'Own standard-control checks process timed out'}
 $exitCode=$process.ExitCode;$stderr=Get-Content (Join-Path $out 'stderr.txt') -Raw
 Get-Content (Join-Path $out 'stdout.txt')
 if($VerifyFailureExit){if($exitCode -ne 1 -or $stderr -notmatch 'Controlled standard-control test failure' -or $stderr -notmatch 'Program.Main'){throw 'Failure exit policy verification failed'}}
 elseif($exitCode -ne 0){throw $stderr}
 Write-Output ('ExitCode='+$exitCode+'; log='+$out)
} finally {Pop-Location}
