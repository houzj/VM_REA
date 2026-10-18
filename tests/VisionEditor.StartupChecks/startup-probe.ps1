param(
    [Parameter(Mandatory=$true)][string]$Executable,
    [int]$TimeoutSeconds = 45
)

$resolved = (Resolve-Path -LiteralPath $Executable).Path
$watch = [Diagnostics.Stopwatch]::StartNew()
$child = Start-Process -FilePath $resolved -WorkingDirectory (Split-Path $resolved) -WindowStyle Normal -PassThru
try {
    do {
        Start-Sleep -Milliseconds 250
        $process = Get-Process -Id $child.Id -ErrorAction SilentlyContinue
        if ($null -eq $process) { throw "程序在显示主窗口前退出：PID $($child.Id)" }
        $process.Refresh()
        if ($process.MainWindowHandle -ne [IntPtr]::Zero) {
            [pscustomobject]@{
                Pid = $child.Id
                SessionId = $process.SessionId
                WindowHandle = $process.MainWindowHandle.ToInt64()
                WindowTitle = $process.MainWindowTitle
                StartupMilliseconds = $watch.ElapsedMilliseconds
                Path = $resolved
            } | ConvertTo-Json
            exit 0
        }
    } while ($watch.Elapsed.TotalSeconds -lt $TimeoutSeconds)
    throw "在 $TimeoutSeconds 秒内未创建主窗口：PID $($child.Id)"
}
finally {
    # 仅清理由本脚本创建的进程，不触及用户或其它任务的实例。
    $owned = Get-Process -Id $child.Id -ErrorAction SilentlyContinue
    if ($null -ne $owned) {
        if ($owned.MainWindowHandle -ne [IntPtr]::Zero) { $owned.CloseMainWindow() | Out-Null }
        if (-not $owned.WaitForExit(3000)) { Stop-Process -Id $child.Id -Force }
    }
}
