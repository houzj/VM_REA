#Requires -Version 7.0
param([string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications',[string]$Solution='D:/Program Files/VisionMaster4.4.0/Development/V4.x/Samples/C#/PlatformSDKSampleCS/CircleFind/CircleFind.sol')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=[guid]::NewGuid().ToString('N')
$relative="analysis/full-audit/n-point-calibration/runtime/run-$run"
$output=Join-Path $workspace $relative
New-Item -ItemType Directory -Path $output | Out-Null
$binary=Join-Path $Applications 'Module(sp)/x64/Calibration/IMVSNPointCalibModu/IMVSNPointCalibModu.dll'
if((Get-FileHash -LiteralPath $binary).Hash -ne 'F8E07023A816C55FF07906E8889F45B93658F59CE88F040AD2204A70922F4839'){throw 'Original binary changed'}
$alias=Join-Path $env:TEMP "acquisition-probe-$run"
New-Item -ItemType Junction -Path $alias -Target $workspace | Out-Null
$exe=Join-Path $alias "$relative/OriginalNPointCalibrationProbe.exe"
$wpf='C:/Windows/Microsoft.NET/Framework64/v4.0.30319/WPF'
& C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /platform:x64 /r:System.Core.dll /r:System.Web.Extensions.dll "/r:$wpf/PresentationCore.dll" "/r:$wpf/PresentationFramework.dll" "/r:$wpf/WindowsBase.dll" /r:System.Xaml.dll "/out:$exe" (Join-Path $alias 'tests/NativeEvidence/OriginalNPointCalibrationProbe.cs')
if($LASTEXITCODE -ne 0){throw 'Compile failed'}
'<configuration><appSettings><add key="StartServerByExe" value="0"/><add key="ServerPath" value=""/><add key="StartClientMode" value="1"/></appSettings><startup><supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1"/></startup></configuration>' | Set-Content -LiteralPath ($exe+'.config')
@($binary,$exe,(Join-Path $PSScriptRoot 'OriginalNPointCalibrationProbe.cs')) | ForEach-Object { @{path=[IO.Path]::GetFullPath($_);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()} } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output provenance.json)
$probeArguments=@($Applications,$Solution,$output);$arguments=$probeArguments | ForEach-Object {if($_.Contains('"')){throw 'Invalid quote'};'"'+$_+'"'}
$process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output stdout.log) -RedirectStandardError (Join-Path $output stderr.log)
if(!$process.WaitForExit(60000)){$process.Kill();throw "Owned probe timed out: $output"}
Get-Content -LiteralPath (Join-Path $output probe.json) -Raw | ConvertFrom-Json | Select-Object phase,code,count,interval,error,message
Write-Output $output
