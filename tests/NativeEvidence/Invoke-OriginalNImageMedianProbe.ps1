param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0',[string]$FixturePrefix='', [int]$Board=7)
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/full-audit/n-image-calibration/median-probe'
New-Item -ItemType Directory -Force $audit | Out-Null
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10'
$kitVersion=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$kitVersion/ucrt" "/I$kit/Include/$kitVersion/um" "/I$kit/Include/$kitVersion/shared" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner" "$PSScriptRoot/OriginalNImageMedianProbe.cpp" "/Fe:$audit/probe.exe" "/Fo:$audit/probe.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$kitVersion/ucrt/x64" "/LIBPATH:$kit/Lib/$kitVersion/um/x64" | Set-Content "$audit/build-native.log"
if($LASTEXITCODE -ne 0){throw 'Native probe build failed.'}
Push-Location $audit
try{& ./probe.exe "$InstallRoot/MVDAlgorithmSDK/Runtime/x64" | Set-Content runtime.jsonl;if($LASTEXITCODE -ne 0){throw 'Original median process failed.'}}finally{Pop-Location}
