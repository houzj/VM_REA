$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/b-fixture/native-format'
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$compiler=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory|Sort-Object Name -Descending|Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10'
$version=Get-ChildItem "$kit/Include" -Directory|Sort-Object Name -Descending|Select-Object -First 1 -ExpandProperty Name
& "$compiler/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$compiler/include" "/I$kit/Include/$version/ucrt" "/I$kit/Include/$version/um" "/I$kit/Include/$version/shared" "$PSScriptRoot/FormatProbe.cpp" "/Fe:$audit/probe.exe" "/Fo:$audit/probe.obj" /link "/LIBPATH:$compiler/lib/x64" "/LIBPATH:$kit/Lib/$version/ucrt/x64" "/LIBPATH:$kit/Lib/$version/um/x64" *> "$audit/build.log"
if($LASTEXITCODE -ne 0){Get-Content "$audit/build.log";exit 1}
& "$audit/probe.exe" *> "$audit/run.log"
$probeExit=$LASTEXITCODE
Get-Content "$audit/run.log"
exit $probeExit
