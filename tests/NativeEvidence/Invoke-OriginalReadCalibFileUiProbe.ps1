#Requires -Version 7.0
param([string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=[guid]::NewGuid().ToString('N')
$relative="analysis/full-audit/read-calib-file/ui/run-$run"
$output=Join-Path $workspace $relative
New-Item -ItemType Directory -Path $output | Out-Null
$evidence=Get-Content (Join-Path $workspace 'docs/plugins/read-calib-file-evidence.json') -Raw | ConvertFrom-Json
$nativeDescription=$evidence.events | Where-Object {$_.phase -EQ 'get' -and $_.key -EQ 'GetCalibInputParam' -and $_.value -Like '*TransNum*'} | Select-Object -First 1
if($nativeDescription){[IO.File]::WriteAllText((Join-Path $output 'native-description.txt'),$nativeDescription.value,[Text.UTF8Encoding]::new($false))}
$alias=Join-Path $env:TEMP "read-calib-file-ui-$run"
New-Item -ItemType Junction -Path $alias -Target $workspace | Out-Null
$exe=Join-Path $alias "$relative/OriginalReadCalibFileUiProbe.exe"
$wpf='C:/Windows/Microsoft.NET/Framework64/v4.0.30319/WPF'
& C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /platform:x64 /r:System.Core.dll /r:System.Web.Extensions.dll /r:System.Xml.dll "/r:$Applications/myLibs/VMControls.BaseInterface.dll" "/r:$wpf/PresentationCore.dll" "/r:$wpf/PresentationFramework.dll" "/r:$wpf/WindowsBase.dll" /r:System.Xaml.dll "/out:$exe" (Join-Path $alias 'tests/NativeEvidence/OriginalReadCalibFileUiProbe.cs')
if($LASTEXITCODE -ne 0){throw 'Compile failed'}
'<configuration><startup><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1"/></startup></configuration>' | Set-Content -LiteralPath ($exe+'.config')
Copy-Item -LiteralPath (Join-Path $Applications 'myLibs/VMControls.BaseInterface.dll') -Destination $output
@((Join-Path $PSScriptRoot 'OriginalReadCalibFileUiProbe.cs'),(Join-Path $Applications 'Module(sp)/x64/Calibration/ReadCalibFileModu/ReadCalibFileModuControl.dll'),(Join-Path $Applications 'Module(sp)/x64/Calibration/ReadCalibFileModu/ReadCalibFileModuAlgorithmTab.xml')) | ForEach-Object { @{path=[IO.Path]::GetFullPath($_);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()} } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output provenance.json)
$arguments=@($Applications,$output) | ForEach-Object {'"'+$_+'"'}
$process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output stdout.log) -RedirectStandardError (Join-Path $output stderr.log)
if(!$process.WaitForExit(60000)){$process.Kill();throw "Owned probe timed out: $output"}
$process.Refresh()
@{exitCode=$process.ExitCode;scope='Original controls with synthetic model; no native module created'} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output process.json)
if($process.ExitCode -ne 0){throw "Owned probe failed with exit code $($process.ExitCode): $output"}
if(!(Test-Path (Join-Path $output probe.json))){throw "Probe produced no report: $output"}
if(Test-Path (Join-Path $output probe.json)){Get-Content -LiteralPath (Join-Path $output probe.json) -Raw | ConvertFrom-Json | Select-Object fixture,error,screenshot}
Write-Output $output

