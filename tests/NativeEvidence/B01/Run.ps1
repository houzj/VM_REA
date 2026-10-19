param([string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$output=Join-Path $workspace ('analysis/full-audit/batch1-20261019/B01/run-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $output | Out-Null
$exe=Join-Path $output 'TabAssemblyProbe.exe'
& C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /platform:x64 /r:System.Core.dll /r:System.Web.Extensions.dll "/r:$Applications/myLibs/VMControls.BaseInterface.dll" "/out:$exe" (Join-Path $PSScriptRoot 'TabAssemblyProbe.cs')
if($LASTEXITCODE -ne 0){throw 'Compile failed'}
Copy-Item -LiteralPath (Join-Path $Applications 'myLibs/VMControls.BaseInterface.dll') -Destination $output
$paths=@((Join-Path $PSScriptRoot 'TabAssemblyProbe.cs'),(Join-Path $Applications 'myLibs/Apps.XmlParser.dll'),(Join-Path $Applications 'myLibs/VM.Xml.dll'),(Join-Path $Applications 'Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModuleAlgorithmTab.xml'))
$paths | ForEach-Object {@{path=$_;sha256=(Get-FileHash -LiteralPath $_).Hash}} | ConvertTo-Json | Set-Content (Join-Path $output 'input-hashes.json')
$arguments=@($Applications,$output) | ForEach-Object {'"'+$_+'"'}
$process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output 'stdout.log') -RedirectStandardError (Join-Path $output 'stderr.log')
if(!$process.WaitForExit(45000)){$process.Kill();throw "Owned probe timeout: $output"}
$process.Refresh()
@{exitCode=$process.ExitCode;pid=$process.Id;output=$output} | ConvertTo-Json | Set-Content (Join-Path $output 'process.json')
Get-Content (Join-Path $output 'probe.json') -Raw | ConvertFrom-Json | Select-Object phase,error,passed
Write-Output $output
if($process.ExitCode -ne 0){throw "Probe failed: $($process.ExitCode)"}
