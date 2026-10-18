param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/full-audit/n-image-calibration/numerical-probe'
# Build both SDK entry points first using Invoke-Probe.ps1 / Invoke-RgbProbe.ps1.
$taskAlias=Join-Path $env:TEMP ('nimage-color-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $taskAlias -Target $root | Out-Null
foreach($mode in @('rgb','gray')){
 $executable=if($mode -eq 'rgb'){'rgb-probe.exe'}else{'probe.exe'}
 $arguments=@('"'+$InstallRoot+'/MVDAlgorithmSDK/Runtime/x64"','"'+$taskAlias+'/analysis/full-audit/n-image-calibration/color-fixture/'+$mode+'"','6')
 $process=Start-Process -FilePath "$audit/$executable" -ArgumentList $arguments -WindowStyle Hidden -PassThru -RedirectStandardOutput "$audit/color-$mode.jsonl" -RedirectStandardError "$audit/color-$mode.stderr.log"
 Write-Output "$mode $($process.Id)"
}
