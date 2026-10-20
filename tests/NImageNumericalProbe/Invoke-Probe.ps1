param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0',[int]$CaseIndex=-1)
$ErrorActionPreference='Stop'
$root=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$audit=Join-Path $root 'analysis/full-audit/n-image-calibration/numerical-probe'
New-Item -ItemType Directory -Force $audit | Out-Null
$alias=Join-Path $env:TEMP ('nimage-numerical-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $alias -Target $root | Out-Null
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10'
$kitVersion=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$kitVersion/ucrt" "/I$kit/Include/$kitVersion/um" "/I$kit/Include/$kitVersion/shared" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner" "$PSScriptRoot/probe.cpp" "/Fe:$audit/probe.exe" "/Fo:$audit/probe.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$kitVersion/ucrt/x64" "/LIBPATH:$kit/Lib/$kitVersion/um/x64" | Set-Content "$audit/build.log"
if($LASTEXITCODE -ne 0){throw 'NImage numerical probe build failed.'}
$runtime="$InstallRoot/MVDAlgorithmSDK/Runtime/x64"
$prefix="$alias/analysis/full-audit/n-image-calibration/solver-fixture/frame"
$arguments=@($runtime,$prefix) | ForEach-Object {'"'+$_+'"'}
$name='runtime';if($CaseIndex -ge 0){$arguments+=$CaseIndex.ToString();$name='case-'+$CaseIndex}
$process=Start-Process -FilePath "$audit/probe.exe" -ArgumentList $arguments -WindowStyle Hidden -PassThru -RedirectStandardOutput "$audit/$name.jsonl" -RedirectStandardError "$audit/$name.stderr.log"
@{pid=$process.Id;executable="$audit/probe.exe";runtime=$runtime;fixture=$prefix;deviceTests=$false} | ConvertTo-Json | Set-Content "$audit/process.json"
$process.Id
