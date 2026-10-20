$ErrorActionPreference='Stop'
$root=(Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$out=Join-Path $root 'analysis/n-point-calibration/count4-fit-trace'
New-Item -ItemType Directory -Force -Path $out|Out-Null
$sdk='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK'
$runtime=Join-Path $sdk 'Runtime/x64'
if((Get-FileHash (Join-Path $runtime 'MVD_Algorithm.dll')).Hash -ne '85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549'){throw 'Original core DLL identity differs.'}
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory|Sort-Object Name -Descending|Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10';$kv=Get-ChildItem "$kit/Include" -Directory|Sort-Object Name -Descending|Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$kv/ucrt" "/I$kit/Include/$kv/um" "/I$kit/Include/$kv/shared" "/I$sdk/Includes/Algorithms" "/I$sdk/Includes/Common/VisionDesigner" "$PSScriptRoot/OriginalNPointCount4TraceProbe.cpp" "/Fe:$out/probe.exe" "/Fo:$out/probe.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$kv/ucrt/x64" "/LIBPATH:$kit/Lib/$kv/um/x64" | Set-Content (Join-Path $out 'build.log')
if($LASTEXITCODE -ne 0){throw 'Native trace probe build failed.'}
$proc=Start-Process -FilePath (Join-Path $out 'probe.exe') -ArgumentList @("`"$runtime`"") -WorkingDirectory $out -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $out 'result.json') -RedirectStandardError (Join-Path $out 'stderr.log')
if(-not $proc.WaitForExit(15000)){Stop-Process -Id $proc.Id -Force;throw 'Owned trace probe exceeded 15 seconds.'}
if($proc.ExitCode -ne 0){throw "Owned trace probe exited $($proc.ExitCode). See workspace-local stderr."}
Get-Content (Join-Path $out 'result.json');Get-Content (Join-Path $out 'fit-trace.jsonl')
