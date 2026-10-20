param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0',[string]$OutDir='')
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
if([string]::IsNullOrWhiteSpace($OutDir)){$OutDir="analysis/n-image-calibration/abandon-param-probe-$([Guid]::NewGuid().ToString('N'))"}
$audit=[IO.Path]::GetFullPath((Join-Path $root $OutDir))
if(!$audit.StartsWith($root,[StringComparison]::OrdinalIgnoreCase)){throw 'OutDir must stay under the current worktree.'}
if(Test-Path $audit){throw "OutDir already exists; refusing to overwrite: $audit"}
$null=New-Item -ItemType Directory -Force $audit
$module="$InstallRoot/Applications/Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModu.dll"
$sdk="$InstallRoot/MVDAlgorithmSDK/Runtime/x64"
if((Get-FileHash $module -Algorithm SHA256).Hash -ne '68C04FC9782E873A80776D6EA4BC1E0515B87D54E9D13B41DBD73799747525EC'){throw 'Original module hash mismatch; refusing to use its private frame-list RVA.'}
New-Item -ItemType Directory -Force $audit | Out-Null
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10';$kv=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
$inc="$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms";$common="$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner"
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$kv/ucrt" "/I$kit/Include/$kv/um" "/I$kit/Include/$kv/shared" "/I$inc" "/I$common" "$PSScriptRoot/OriginalNImageAbandonParamProbe.cpp" "/Fe:$audit/probe.exe" "/Fo:$audit/probe.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$kv/ucrt/x64" "/LIBPATH:$kit/Lib/$kv/um/x64" | Set-Content "$audit/build.log"
if($LASTEXITCODE -ne 0){throw 'Native probe build failed.'}
$oldPath=$env:PATH
try{
 $env:PATH="$InstallRoot/Applications/ModuleProxy/x64;$InstallRoot/Applications;$sdk;"+$oldPath
 Push-Location $root
 try{& "$audit/probe.exe" $sdk $module 'analysis/full-audit/n-image-calibration/board' | Set-Content "$audit/runtime.jsonl"}finally{Pop-Location}
 if($LASTEXITCODE -ne 0){throw 'Original module probe failed.'}
}finally{$env:PATH=$oldPath}
@{authority='hash-pinned original module + original SDK, controlled synthetic no-device input';moduleSha256=(Get-FileHash $module -Algorithm SHA256).Hash.ToLower();algorithmTabSha256=(Get-FileHash "$InstallRoot/Applications/Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModuAlgorithmTab.xml" -Algorithm SHA256).Hash.ToLower();probeSha256=(Get-FileHash "$PSScriptRoot/OriginalNImageAbandonParamProbe.cpp" -Algorithm SHA256).Hash.ToLower();runtimeSha256=(Get-FileHash "$audit/runtime.jsonl" -Algorithm SHA256).Hash.ToLower()}|ConvertTo-Json|Set-Content "$audit/provenance.json"
Write-Host "Original NImage retired-parameter probe saved to $audit"
