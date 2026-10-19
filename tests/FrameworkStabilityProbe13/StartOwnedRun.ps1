param(
    [Parameter(Mandatory=$true)][string]$OutputParent,
    [Parameter(Mandatory=$true)][string]$BaselineSha,
    [ValidateRange(1800,86400)][int]$DurationSeconds=1800
)
$ErrorActionPreference='Stop'
# A new explicitly requested run only. Never reuse or retry a failed directory.
$taskParent=[System.IO.Path]::GetFullPath($OutputParent)
if(Test-Path -LiteralPath $taskParent){throw 'OutputParent must be a new owned directory.'}
$taskExe=(Resolve-Path -LiteralPath (Join-Path $PSScriptRoot 'bin/Debug/net10.0/FrameworkStabilityProbe13.exe')).Path
New-Item -ItemType Directory -Path $taskParent | Out-Null
$taskLaunch=[ordered]@{executable=$taskExe;requestedSeconds=$DurationSeconds;baseline=$BaselineSha;launchUtc=[DateTime]::UtcNow.ToString('o')}
try {
    $taskProcess=Start-Process -FilePath $taskExe -ArgumentList @(('"'+$taskParent+'"'),$DurationSeconds,$BaselineSha) -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $taskParent 'stdout.log') -RedirectStandardError (Join-Path $taskParent 'stderr.log')
    $taskLaunch.pid=$taskProcess.Id
    $taskLaunch.processStartUtc=$taskProcess.StartTime.ToUniversalTime().ToString('o')
    $taskLaunch | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $taskParent 'launch.json') -Encoding utf8
    # Run this launcher as an asynchronous exec job; only its own child is awaited.
    $taskProcess.WaitForExit()
    $taskProcess.Refresh()
    [ordered]@{pid=$taskProcess.Id;processStartUtc=$taskLaunch.processStartUtc;exitCode=$taskProcess.ExitCode;observedUtc=[DateTime]::UtcNow.ToString('o')} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $taskParent 'exit.json') -Encoding utf8
    exit $taskProcess.ExitCode
} catch {
    [ordered]@{error=$_.Exception.ToString();hresult=$_.Exception.HResult;observedUtc=[DateTime]::UtcNow.ToString('o')} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $taskParent 'launcher-failure.json') -Encoding utf8
    throw
}
