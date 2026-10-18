param([switch]$AllowOriginalPlatformStart,[string]$Applications='D:/Program Files/VisionMaster4.4.0/Applications')
$ErrorActionPreference='Stop'
if(!$AllowOriginalPlatformStart){throw 'Original SDK starts a platform and was observed attempting shared ImageTemp cleanup. Establish isolated original runtime/temp before explicitly allowing this probe.'}
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$output=Join-Path $root 'analysis/camera-mapping/parser'
New-Item -ItemType Directory -Force $output | Out-Null
$alias=Join-Path $env:TEMP ('camera-platform-index-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $alias -Target $root | Out-Null
$exe=Join-Path $alias 'analysis\camera-mapping\parser\PlatformIndexEntryProbe.exe'
& C:/Windows/Microsoft.NET/Framework64/v4.0.30319/csc.exe /nologo /target:exe /platform:x64 /r:System.Web.Extensions.dll "/r:$Applications/PublicFile/x64/VM.PlatformSDKCS.dll" "/out:$exe" (Join-Path $alias 'tests\CameraMapping.NativeProbe\PlatformIndexEntryProbe.cs')
if($LASTEXITCODE -ne 0){throw 'Platform entry probe compilation failed.'}
Copy-Item "$Applications/PublicFile/x64/VM.PlatformSDKCS.dll" $output
$arguments=@($Applications) | ForEach-Object {'"'+$_+'"'}
$process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput "$output/platform-entry.jsonl" -RedirectStandardError "$output/platform-entry.stderr.log"
if(!$process.WaitForExit(30000)){$process.Kill();throw 'Owned platform entry probe timeout'}
Get-Content "$output/platform-entry.jsonl"
if($process.ExitCode -ne 0){throw "Owned platform entry probe failed: $($process.ExitCode)"}
