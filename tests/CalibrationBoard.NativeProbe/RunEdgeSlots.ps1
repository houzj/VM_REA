param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=Join-Path $root ('analysis/calibration-board/edge-slots-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $run | Out-Null
$sdk=Join-Path $InstallRoot 'MVDAlgorithmSDK/Runtime/x64/MVDCalibBoardCalibCpp.dll'
if((Get-FileHash $sdk).Hash.ToLowerInvariant() -ne '0a65d7a9e84a3ec6f662b4f05daa5a88351690ae4fbecd1748427c13c56fa308'){throw 'SDK identity mismatch for observed offsets'}
$module=Join-Path $InstallRoot 'Applications/Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu/IMVSCalibBoardCalibModu.dll'
if((Get-FileHash $module).Hash.ToLowerInvariant() -ne 'bd62ee940a2e299c0c47e41909e64ca78c09d7717e7cefc629dbfb9e21105c1d'){throw 'Module identity mismatch for observed offsets'}
$core=Join-Path $InstallRoot 'MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll'
if((Get-FileHash $core).Hash.ToLowerInvariant() -ne '85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549'){throw 'Core identity mismatch for observed offsets'}
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10'
$kitVersion=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$kitVersion/ucrt" "/I$kit/Include/$kitVersion/um" "/I$kit/Include/$kitVersion/shared" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner" "$PSScriptRoot/EdgeSlotsProbe.cpp" "/Fe:$run/probe.exe" "/Fo:$run/probe.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$kitVersion/ucrt/x64" "/LIBPATH:$kit/Lib/$kitVersion/um/x64" *> "$run/build.log"
if($LASTEXITCODE -ne 0){Get-Content "$run/build.log";throw 'Native probe build failed'}
$arguments=@("$InstallRoot/MVDAlgorithmSDK/Runtime/x64","$InstallRoot/Applications",$module,$run)|ForEach-Object{'"'+$_+'"'}
$process=Start-Process -FilePath "$run/probe.exe" -ArgumentList $arguments -WorkingDirectory $run -WindowStyle Hidden -PassThru -RedirectStandardOutput "$run/runtime.jsonl" -RedirectStandardError "$run/stderr.log"
if(!$process.WaitForExit(45000)){$process.Kill();throw 'Owned edge slot probe timeout'}
$process.Refresh()
$records=@(Get-Content "$run/runtime.jsonl" | ForEach-Object {ConvertFrom-Json $_})
$entries=@($records | Where-Object {$_.phase -eq 'candidate-entry'})
$calls=@($records | Where-Object {$_.phase -eq 'run'})
$sources=@($sdk,$module,$core,"$PSScriptRoot/EdgeSlotsProbe.cpp","$PSScriptRoot/ModuleProbe.cpp",$PSCommandPath | ForEach-Object {@{path=$_;sha256=(Get-FileHash $_).Hash.ToLowerInvariant()}})
@{scope='same original module and SDK instance; hardware register observation only';exitCode=$process.ExitCode;source=$sources;candidateEntries=$entries;runCount=$calls.Count;devices=$false;nativeDesktopCompared=$false} | ConvertTo-Json -Depth 7 | Set-Content "$run/summary.json"
if($process.ExitCode -ne 0){Get-Content "$run/runtime.jsonl" -Tail 8;throw "Native edge slots failed: $($process.ExitCode)"}
if($calls.Count -ne 6 -or $entries.Count -ne 6){throw "Expected six runs/entries; got $($calls.Count)/$($entries.Count)"}
$expected=@(@(20,40),@(0,40),@(20,80),@(20,40),@(0,40),@(20,80))
for($i=0;$i -lt 6;$i++){
 $entry=@($entries | Where-Object {$_.stage -eq $i})
 if($entry.Count -ne 1 -or $entry[0].low -ne $expected[$i][0] -or $entry[0].high -ne $expected[$i][1]){throw "Candidate options differ at stage $i"}
 if($calls[$i].code -ne 0 -or $calls[$i].status -ne 1){throw "Native process failed at stage $i"}
 $sdkCalls=@($records | Where-Object {$_.phase -eq 'sdk-setter-entry' -and $_.stage -eq $i})
 $algorithmCalls=@($records | Where-Object {$_.phase -eq 'algorithm-setter-entry' -and $_.stage -eq $i})
 if($sdkCalls.Count -ne 2 -or $algorithmCalls.Count -ne 2){throw "Missing setter observation at stage $i"}
 for($j=0;$j -lt 2;$j++){
  $name=if($j -eq 0){'EdgeThreshLow'}else{'EdgeThreshHigh'}
  if($sdkCalls[$j].name -ne $name -or $sdkCalls[$j].value -ne [string]$expected[$i][$j] -or $algorithmCalls[$j].name -ne $name -or $algorithmCalls[$j].value -ne [string]$expected[$i][$j] -or $sdkCalls[$j].rdx -ne $algorithmCalls[$j].rdx -or $sdkCalls[$j].r8 -ne $algorithmCalls[$j].r8){throw "ABI name/value passthrough differs at stage $i field $j"}
 }
}
Write-Output 'PASS six original module/SDK runs; 24 setter ABI observations and six exact circle candidate threshold pairs.'
Get-Content "$run/runtime.jsonl" | Where-Object {$_ -match 'candidate-entry|"phase":"run"'}
Write-Output $run
