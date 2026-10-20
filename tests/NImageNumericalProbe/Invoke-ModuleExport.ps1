param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/full-audit/n-image-calibration/numerical-probe'
$module="$InstallRoot/Applications/Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModu.dll"
if((Get-FileHash $module -Algorithm SHA256).Hash -ne '68C04FC9782E873A80776D6EA4BC1E0515B87D54E9D13B41DBD73799747525EC'){throw 'Original module identity differs; private frame-list helper RVA cannot be used.'}
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10'
$kv=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$kv/ucrt" "/I$kit/Include/$kv/um" "/I$kit/Include/$kv/shared" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner" "$PSScriptRoot/module-export.cpp" "/Fe:$audit/module-export.exe" "/Fo:$audit/module-export.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$kv/ucrt/x64" "/LIBPATH:$kit/Lib/$kv/um/x64" | Set-Content "$audit/build-module-native.log"
if($LASTEXITCODE -ne 0){throw 'Module harness build failed.'}
# The existing ASCII worktree alias prevents native narrow-string filename conversion from silently losing output.
$alias=Join-Path $env:TEMP ('nimage-export-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $alias -Target $root | Out-Null
$ascii="$alias/analysis/full-audit/n-image-calibration"
if(!(Test-Path "$ascii/solver-fixture/frame-0.bmp")){throw 'Missing solver fixtures.'}
$oldPath=$env:PATH
try{$env:PATH="$InstallRoot/Applications/ModuleProxy/x64;$InstallRoot/Applications;"+$oldPath;& "$audit/module-export.exe" "$InstallRoot/MVDAlgorithmSDK/Runtime/x64" $module "$ascii/solver-fixture/frame" "$ascii/numerical-probe/original-module-calibration.xml" | Set-Content "$audit/module-runtime.jsonl";if($LASTEXITCODE -ne 0){throw 'Original module harness failed.'}}finally{$env:PATH=$oldPath}
if(!(Test-Path "$audit/original-module-calibration.xml")){throw 'Native save reported no usable output file.'}
@{authority='synthetic-owned-instance';platformInit=$false;moduleSha256=(Get-FileHash $module -Algorithm SHA256).Hash.ToLower();harnessSha256=(Get-FileHash "$PSScriptRoot/module-export.cpp" -Algorithm SHA256).Hash.ToLower();outputSha256=(Get-FileHash "$audit/original-module-calibration.xml" -Algorithm SHA256).Hash.ToLower()}|ConvertTo-Json|Set-Content "$audit/module-provenance.json"
