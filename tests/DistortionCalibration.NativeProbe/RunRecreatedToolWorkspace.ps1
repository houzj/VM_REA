param([switch]$CheckerPrelude,[string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$expectedCore='85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549'
if((Get-FileHash -LiteralPath "$InstallRoot/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll" -Algorithm SHA256).Hash -ne $expectedCore){throw 'Original core hash differs; fixed hook RVAs require renewed evidence.'}
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/distortion-calibration/recreated-tool-workspace'
if ($CheckerPrelude) { $audit = Join-Path $audit 'checker-prelude' }
New-Item -ItemType Directory -Force $audit | Out-Null
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10'
$kitVersion=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$kitVersion/ucrt" "/I$kit/Include/$kitVersion/um" "/I$kit/Include/$kitVersion/shared" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner" "$PSScriptRoot/trace-recreated-tool-workspace-probe.cpp" "/Fe:$audit/probe.exe" "/Fo:$audit/probe.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$kitVersion/ucrt/x64" "/LIBPATH:$kit/Lib/$kitVersion/um/x64" | Set-Content "$audit/build-native.log"
if($LASTEXITCODE -ne 0){throw 'Native probe build failed.'}
$probeArguments=@("`"$InstallRoot/MVDAlgorithmSDK/Runtime/x64`""); if($CheckerPrelude){$probeArguments += "checker-prelude"}
$process=Start-Process -FilePath "$audit/probe.exe" -ArgumentList $probeArguments -WorkingDirectory $audit -WindowStyle Hidden -PassThru -RedirectStandardOutput "$audit/runtime.jsonl" -RedirectStandardError "$audit/stderr.log" -Wait
@{exitCode=$process.ExitCode;authority='owned documented SDK';deviceTests=$false;executable="$audit/probe.exe"}|ConvertTo-Json|Set-Content "$audit/process.json"
if($process.ExitCode -ne 0){throw "Native probe failed with exit code $($process.ExitCode)."}



