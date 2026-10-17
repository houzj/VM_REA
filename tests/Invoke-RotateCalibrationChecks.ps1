#Requires -Version 7.0
param([switch]$VerifyFailureExit)
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$output=Join-Path $workspace ('analysis/full-audit/rotate-calibration/checks/run-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $output | Out-Null
& dotnet build (Join-Path $PSScriptRoot 'RotateCalibration.Checks/RotateCalibration.Checks.csproj') --no-restore *> (Join-Path $output build.log)
if($LASTEXITCODE -ne 0){throw "Build failed; $output"}
$exe=Join-Path $PSScriptRoot 'RotateCalibration.Checks/bin/Debug/net10.0-windows/RotateCalibration.Checks.exe'
$start=@{FilePath=$exe;WorkingDirectory=$workspace;WindowStyle='Hidden';PassThru=$true;RedirectStandardOutput=(Join-Path $output stdout.log);RedirectStandardError=(Join-Path $output stderr.log)}
if($VerifyFailureExit){$start.ArgumentList='--verify-failure-exit'}
$process=Start-Process @start
if(!$process.WaitForExit(60000)){$process.Kill();throw "Owned check process timed out; $output"}
$process.Refresh()
@{exitCode=$process.ExitCode;ownedProcessId=$process.Id;hasExited=$process.HasExited;failureExitVerification=[bool]$VerifyFailureExit;scope='Independent implementation tests; all windows owned and positioned offscreen'} | ConvertTo-Json | Set-Content (Join-Path $output process.json)
Get-Content (Join-Path $output stdout.log)
if($VerifyFailureExit){$errorText=Get-Content (Join-Path $output stderr.log) -Raw;if($process.ExitCode -ne 1 -or !$errorText.Contains('CONTROLLED_ROTATE_CHECK_FAILURE') -or !$errorText.Contains('Program.cs:line')){throw "Failure route did not return captured stack; $output"};Write-Output "Controlled failure captured, exit 1, owned process exited; $output";exit 0}
if($process.ExitCode -ne 0){Get-Content (Join-Path $output stderr.log);throw "Checks failed; $output"}
Write-Output $output
