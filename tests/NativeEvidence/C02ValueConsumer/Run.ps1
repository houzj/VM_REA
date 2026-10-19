param([string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$output=Join-Path $root ('analysis/full-audit/batch7-20261019/C02/run-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $output | Out-Null
$exe=Join-Path $output 'ValueConsumerProbe.exe'
& C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /platform:x64 /r:System.Core.dll /r:System.Web.Extensions.dll "/out:$exe" (Join-Path $PSScriptRoot 'Probe.cs')
if($LASTEXITCODE -ne 0){throw 'Compile failed'}
$dll=Join-Path $Applications 'myLibs/VM.Core.dll'
$before=(Get-FileHash -LiteralPath $dll).Hash
$arguments=@($Applications,$output) | ForEach-Object {'"'+$_+'"'}
$p=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output 'stdout.log') -RedirectStandardError (Join-Path $output 'stderr.log')
$ownedHandle=$p.Handle
if(!$p.WaitForExit(30000)){$p.Kill();throw "Owned probe timeout $output"}
$p.Refresh()
@{exitCode=$p.ExitCode;pid=$p.Id;beforeSHA256=$before;afterSHA256=(Get-FileHash -LiteralPath $dll).Hash;output=$output} | ConvertTo-Json | Set-Content (Join-Path $output 'process.json')
$hashInputs=@(Get-ChildItem -LiteralPath $output -File | Where-Object Name -ne 'output-sha256.csv')
$hashInputs | Get-FileHash | Export-Csv (Join-Path $output 'output-sha256.csv') -NoTypeInformation
Write-Output $output
if($p.ExitCode -ne 0){throw "Probe failed $($p.ExitCode)"}

