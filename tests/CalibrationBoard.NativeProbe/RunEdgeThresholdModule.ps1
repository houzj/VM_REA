param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=Join-Path $root ('analysis/calibration-board/edge-threshold-module-run-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $run | Out-Null
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10'
$kitVersion=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$kitVersion/ucrt" "/I$kit/Include/$kitVersion/um" "/I$kit/Include/$kitVersion/shared" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner" "$PSScriptRoot/ModuleProbe.cpp" "/Fe:$run/probe.exe" "/Fo:$run/probe.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$kitVersion/ucrt/x64" "/LIBPATH:$kit/Lib/$kitVersion/um/x64" *> "$run/build.log"
if($LASTEXITCODE -ne 0){Get-Content "$run/build.log";throw 'Native probe build failed.'}
$alias=Join-Path $env:TEMP ('board-edge-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $alias -Target $root | Out-Null
try {
 $relative=[IO.Path]::GetRelativePath($root,$run).Replace('\','/')
 $arguments=@("$InstallRoot/MVDAlgorithmSDK/Runtime/x64","$InstallRoot/Applications","$InstallRoot/Applications/Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu/IMVSCalibBoardCalibModu.dll","$alias/$relative")|ForEach-Object{'"'+$_+'"'}
 $process=Start-Process -FilePath "$run/probe.exe" -ArgumentList $arguments -WorkingDirectory $run -WindowStyle Hidden -PassThru -RedirectStandardOutput "$run/runtime.jsonl" -RedirectStandardError "$run/stderr.log"
 if(!$process.WaitForExit(30000)){$process.Kill();throw 'Owned module probe timeout.'}
 $process.Refresh()
 if($process.ExitCode -ne 0){Get-Content "$run/runtime.jsonl";Get-Content "$run/stderr.log";throw "Owned module probe failed: $($process.ExitCode)"}
 $records=@(Get-Content "$run/runtime.jsonl" | ForEach-Object {ConvertFrom-Json $_})
 $expected=@(@('EdgeThreshLow','20'),@('EdgeThreshHigh','40'),@('EdgeThreshLow','0'),@('EdgeThreshHigh','40'),@('EdgeThreshLow','20'),@('EdgeThreshHigh','80'))
 $gets=@($records | Where-Object {$_.phase -eq 'get'})
 if($gets.Count -ne $expected.Count){throw "Expected $($expected.Count) GetParam observations, got $($gets.Count)."}
 for($i=0;$i -lt $expected.Count;$i++){if($gets[$i].code -ne 0 -or $gets[$i].key -ne $expected[$i][0] -or $gets[$i].value -ne $expected[$i][1]){throw "GetParam mismatch at index $i."}}
 $outputs=@($records | Where-Object {$_.phase -like 'edge-*-output'})
 if($outputs.Count -ne 3 -or @($outputs.fnv1a64 | Select-Object -Unique).Count -ne 1){throw 'The three controlled module outputs differ or are missing.'}
 foreach($result in $outputs){$count=($result.writes | Where-Object {$_.name -eq 'CalibPointNum'}).values[0];$estimation=($result.writes | Where-Object {$_.name -eq 'EstimationError'}).values[0];if($result.code -ne 0 -or $result.writeCount -ne 23 -or $count -ne 99 -or [Math]::Abs($estimation-0.00435560197) -gt 1e-10){throw "Unexpected module result at $($result.phase)."}}
 Write-Output "PASS original module EdgeThresh Low/High SetParam+GetParam and 3 identical native output replays. JSONL: $run/runtime.jsonl"
 Write-Output $run
} finally { Remove-Item -LiteralPath $alias -Force }
