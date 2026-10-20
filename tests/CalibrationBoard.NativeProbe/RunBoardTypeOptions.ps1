param([string]$InstallRoot='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=Join-Path $root ('analysis/calibration-board/board-type-options-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $run | Out-Null
$sdk=Join-Path $InstallRoot 'MVDAlgorithmSDK/Runtime/x64/MVDCalibBoardCalibCpp.dll'
$module=Join-Path $InstallRoot 'Applications/Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu/IMVSCalibBoardCalibModu.dll'
$core=Join-Path $InstallRoot 'MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll'
$identities=@{ $sdk='0a65d7a9e84a3ec6f662b4f05daa5a88351690ae4fbecd1748427c13c56fa308';$module='bd62ee940a2e299c0c47e41909e64ca78c09d7717e7cefc629dbfb9e21105c1d';$core='85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549'}
foreach($path in $identities.Keys){if((Get-FileHash $path).Hash.ToLowerInvariant() -ne $identities[$path]){throw "Original binary identity mismatch: $path"}}
$vs=& 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
$cl=Get-ChildItem "$vs/VC/Tools/MSVC" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$kit='C:/Program Files (x86)/Windows Kits/10';$version=Get-ChildItem "$kit/Include" -Directory | Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty Name
& "$cl/bin/Hostx64/x64/cl.exe" /nologo /EHsc /std:c++17 "/I$cl/include" "/I$kit/Include/$version/ucrt" "/I$kit/Include/$version/um" "/I$kit/Include/$version/shared" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Algorithms" "/I$InstallRoot/MVDAlgorithmSDK/Includes/Common/VisionDesigner" "$PSScriptRoot/BoardTypeOptionsProbe.cpp" "/Fe:$run/probe.exe" "/Fo:$run/probe.obj" /link "/LIBPATH:$cl/lib/x64" "/LIBPATH:$kit/Lib/$version/ucrt/x64" "/LIBPATH:$kit/Lib/$version/um/x64" *> "$run/build.log"
if($LASTEXITCODE -ne 0){Get-Content "$run/build.log";throw 'Native board options build failed'}
$alias=Join-Path $env:TEMP ('board-options-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $alias -Target $root | Out-Null
try{
 $evidence=Get-Content "$root/docs/plugins/calibration-board-encoded-roi-rotated-evidence.json" -Raw | ConvertFrom-Json
 $images=@();$imageSources=@()
 foreach($board in 6,7,8,9){$source=$evidence.sources|Where-Object {$_.boardType -eq $board};$full=Join-Path $root $source.image;if((Get-FileHash $full).Hash.ToLowerInvariant() -ne $source.imageSha256){throw "Image identity mismatch for board $board"};$images+=Join-Path $alias $source.image;$imageSources+=@{board=$board;path=$source.image;sha256=$source.imageSha256}}
 $arguments=@("$InstallRoot/MVDAlgorithmSDK/Runtime/x64","$InstallRoot/Applications",$module,$run)+$images
 $arguments=$arguments|ForEach-Object{'"'+$_+'"'}
 $process=Start-Process -FilePath "$run/probe.exe" -ArgumentList $arguments -WorkingDirectory $run -WindowStyle Hidden -PassThru -RedirectStandardOutput "$run/runtime.jsonl" -RedirectStandardError "$run/stderr.log"
 if(!$process.WaitForExit(120000)){$process.Kill();throw 'Owned board options timeout'}
 $process.Refresh();$records=@(Get-Content "$run/runtime.jsonl"|ForEach-Object {ConvertFrom-Json $_})
 $runs=@($records|Where-Object {$_.phase -eq 'run'});$pipelines=@($records|Where-Object {$_.phase -eq 'pipeline-entry'});$dispatch=@($records|Where-Object {$_.phase -eq 'dispatch-entry'})
 $sources=@($sdk,$module,$core,"$PSScriptRoot/BoardTypeOptionsProbe.cpp","$PSScriptRoot/EdgeSlotsProbe.cpp","$PSScriptRoot/ModuleProbe.cpp",$PSCommandPath|ForEach-Object {@{path=$_;sha256=(Get-FileHash $_).Hash.ToLowerInvariant()}})
 $summary=@{scope='six XML-exposed board types, three Low/High pairs; same original module and SDK';exitCode=$process.ExitCode;verified=$false;sources=$sources;images=$imageSources;runs=$runs;pipelineEntries=$pipelines;dispatchEntries=$dispatch;devices=$false;nativeDesktopCompared=$false}
 $summary|ConvertTo-Json -Depth 8|Set-Content "$run/summary.json"
 if($process.ExitCode -ne 0){Get-Content "$run/runtime.jsonl" -Tail 8;throw "Board options native exit $($process.ExitCode)"}
 if($runs.Count -ne 18 -or $pipelines.Count -ne 18 -or $dispatch.Count -ne 18){throw "Expected 18 each run/pipeline/dispatch, got $($runs.Count)/$($pipelines.Count)/$($dispatch.Count)"}
 foreach($stage in @($records|Where-Object {$_.phase -eq 'stage'})){
  foreach($phase in 'pipeline-entry','dispatch-entry'){$item=@($records|Where-Object {$_.phase -eq $phase -and $_.stage -eq $stage.stage});if($item.Count -ne 1 -or $item[0].low -ne $stage.low -or $item[0].high -ne $stage.high){throw "Threshold mismatch $phase stage $($stage.stage)"}}
  if($runs[$stage.stage].code -ne 0 -or $runs[$stage.stage].status -ne 1){throw "Board runtime failed at stage $($stage.stage)"}
 }
 $summary.verified=$true;$summary.runtimeSha256=(Get-FileHash "$run/runtime.jsonl").Hash.ToLowerInvariant();$summary|ConvertTo-Json -Depth 8|Set-Content "$run/summary.json"
 Write-Output 'PASS 18 same-instance runs and 36 exact cross-board pipeline/dispatch threshold observations.'
 $pipelines|Select-Object stage,board,coreKind,low,high|Format-Table
 Write-Output $run
}finally{Remove-Item -LiteralPath $alias -Force}
