param([string]$OutputDirectory = 'analysis/full-audit/batch6-20261019/A03')
$ErrorActionPreference = 'Stop'
$out = [IO.Path]::GetFullPath($OutputDirectory)
[IO.Directory]::CreateDirectory($out) | Out-Null
$run = Join-Path $out ('run-' + [Guid]::NewGuid().ToString('N'))
[IO.Directory]::CreateDirectory($run) | Out-Null
$source = Join-Path $PSScriptRoot 'Probe.cs'
$exe = Join-Path $run 'A03ProcessIsolation.exe'
$csc = Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
& $csc /nologo /target:exe ("/out:" + $exe) $source *> (Join-Path $run 'compile.log')
if ($LASTEXITCODE -ne 0) { throw 'Probe compile failed' }
$key = 'SOFTWARE\WOW6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\VisionMaster'
function Read-HostValue {
  $k = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey($key)
  try { if ($k) { return [string]$k.GetValue('') } else { return $null } } finally { if ($k) { $k.Dispose() } }
}
$before = Read-HostValue
& $exe $run *> (Join-Path $run 'stdout-stderr.log')
$exitCode = $LASTEXITCODE
$after = Read-HostValue
@{ before=$before; after=$after; unchanged=($before -ceq $after); childExit=$exitCode; originalLoggerExecuted=$false } | ConvertTo-Json | Set-Content (Join-Path $run 'host-observation.json') -Encoding UTF8
Get-ChildItem $run -File | Get-FileHash -Algorithm SHA256 | Select-Object Path,Hash | ConvertTo-Json | Set-Content (Join-Path $run 'output-hashes.json') -Encoding UTF8
Write-Output $run
exit $exitCode
