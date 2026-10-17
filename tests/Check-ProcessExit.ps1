param([int]$TimeoutMs=60000,[int]$Repeat=2,[string]$EditorPath='dist/VisionEditor-WpfCanvas/VisionEditor.exe',[string]$ReportName='process-lifecycle',[string]$CoreChecksPath='tests/VisionEditor.Core.Checks/bin/Debug/net10.0/VisionEditor.Core.Checks.exe',[switch]$EditorOnly)
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
Set-Location -LiteralPath $root
$outDir=Join-Path $root (Join-Path 'analysis/full-audit' $ReportName)
New-Item -ItemType Directory -Path $outDir -Force | Out-Null
$cases=@(
 @{Name='CoreChecks';Path=$CoreChecksPath;Gui=$false},
 @{Name='PluginChecks';Path='tests/ImageSource.PluginChecks/bin/Debug/net10.0-windows/ImageSource.PluginChecks.exe';Gui=$false},
 @{Name='WpfChecks';Path='tests/VisionEditor.WpfChecks/bin/Debug/net10.0-windows/VisionEditor.WpfChecks.exe';Gui=$false},
 @{Name='EditorClose';Path=$EditorPath;Gui=$true}
)
$rows=@()
if($EditorOnly){$cases=@($cases|Where-Object {$_.Gui})}
foreach($case in $cases) {
 for($round=1;$round -le $Repeat;$round++) {
  $watch=[Diagnostics.Stopwatch]::StartNew()
  $launch=@{FilePath=(Join-Path $root $case.Path);WorkingDirectory=$root;PassThru=$true;WindowStyle='Hidden'}
  if($case.Path.EndsWith('.dll')) { $launch.ArgumentList='"'+$launch.FilePath+'"';$launch.FilePath=(Get-Command dotnet -ErrorAction Stop).Source }
  if(-not $case.Gui) { $launch.RedirectStandardOutput=Join-Path $outDir "$($case.Name)-$round.out.txt";$launch.RedirectStandardError=Join-Path $outDir "$($case.Name)-$round.err.txt" }
  $child=Start-Process @launch
  # Retain the native handle before a short-lived child exits. Windows PowerShell
  # may otherwise return a null ExitCode for a completed Start-Process -PassThru.
  $null=$child.Handle
  $closeSent=$null
  if($case.Gui) {
   for($n=0;$n -lt 100;$n++) {$child.Refresh();if($child.HasExited -or $child.MainWindowHandle -ne 0){break};Start-Sleep -Milliseconds 100}
   $closeSent=$child.CloseMainWindow()
  }
  $exited=$child.WaitForExit($TimeoutMs)
  if($exited){$child.WaitForExit();$child.Refresh()}
  $exitCode=if($exited){$child.ExitCode}else{$null}
  $rows += [pscustomobject]@{Case=$case.Name;Round=$round;PID=$child.Id;CloseSent=$closeSent;ExitedWithinTimeout=$exited;TimeoutMs=$TimeoutMs;ExitCode=$exitCode;ElapsedMs=$watch.ElapsedMilliseconds}
  if(-not $exited){$child.Kill();$child.WaitForExit(5000)|Out-Null}
  $child.Dispose()
 }
}
$rows | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $outDir 'exit-results.json') -Encoding utf8
$rows | Format-Table -AutoSize
if(@($rows|Where-Object {-not $_.ExitedWithinTimeout -or $_.ExitCode -ne 0}).Count){exit 1}

