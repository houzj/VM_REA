param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/full-audit/n-image-calibration/numerical-probe'
$alias=Join-Path $env:TEMP ('nimage-board-fit-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $alias -Target $root | Out-Null
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10'
$version=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$version/ucrt" "/I$kit/Include/$version/um" "/I$kit/Include/$version/shared" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner" "$PSScriptRoot/board-fit.cpp" "/Fe:$audit/board-fit.exe" "/Fo:$audit/board-fit.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$version/ucrt/x64" "/LIBPATH:$kit/Lib/$version/um/x64" | Set-Content "$audit/board-fit-build.log"
if($LASTEXITCODE -ne 0){throw 'Board-fit probe build failed.'}
$arguments=@("$InstallRoot/MVDAlgorithmSDK/Runtime/x64","$alias/analysis/full-audit/n-image-calibration/numerical-probe/board-fit-input.txt") | ForEach-Object {'"'+$_+'"'}
$process=Start-Process -FilePath "$audit/board-fit.exe" -ArgumentList $arguments -WindowStyle Hidden -PassThru -RedirectStandardOutput "$audit/board-fit.jsonl" -RedirectStandardError "$audit/board-fit.stderr.log"
$process.Id
