param([string]$OutputName="state-lifecycle-replay")
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root ("analysis/map-calibration/"+$OutputName)
if(Test-Path -LiteralPath $out){throw "Output already exists; choose a fresh OutputName: $out"}
New-Item -ItemType Directory -Force $out|Out-Null
$badImage=Join-Path $out 'blank-640x480-24.bmp'
$stream=[IO.File]::Open($badImage,[IO.FileMode]::CreateNew,[IO.FileAccess]::Write)
try{$writer=[IO.BinaryWriter]::new($stream);$width=640;$height=480;$pixels=$width*$height*3;$writer.Write([byte[]](0x42,0x4d));$writer.Write([int](54+$pixels));$writer.Write([int]0);$writer.Write([int]54);$writer.Write([int]40);$writer.Write([int]$width);$writer.Write([int]$height);$writer.Write([short]1);$writer.Write([short]24);$writer.Write([int]0);$writer.Write([int]$pixels);$writer.Write([int]2835);$writer.Write([int]2835);$writer.Write([int]0);$writer.Write([int]0);$writer.Write([byte[]]::new($pixels));$writer.Dispose()}finally{$stream.Dispose()}
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$alias=Join-Path $env:TEMP ('map-calib-state-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/sdk" -Target 'D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK'|Out-Null
New-Item -ItemType Junction "$alias/module" -Target 'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu'|Out-Null
$cmd=@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/map-calibration/$OutputName/probe.obj" /Fe"$alias/work/analysis/map-calibration/$OutputName/probe.exe" "$alias/work/tests/MapCalibration.NativeProbe/ModuleStateLifecycleProbe.cpp"
"@
[IO.File]::WriteAllText("$out/build.cmd",$cmd,[Text.Encoding]::ASCII)
& cmd /c "$out/build.cmd" 2>&1|Tee-Object "$out/build.log"
if($LASTEXITCODE -ne 0){throw 'Native probe build failed'}
$env:PATH="$alias/sdk/Runtime/x64;"+$env:PATH
Push-Location $out
try {
  & "$out/probe.exe" "$alias/sdk/Runtime/x64" "$alias/module/IMVSMapCalibModu.dll" "$alias/work/analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp" "$alias/work/analysis/full-audit/n-image-calibration/solver-fixture/frame-4.bmp" "$badImage" "$alias/work/analysis/map-calibration/$OutputName" 2>"$out/stderr.log" | Set-Content "$out/results.jsonl" -Encoding utf8
  if($LASTEXITCODE -ne 0){throw 'Native probe failed'}
} finally { Pop-Location }
$records=Get-Content "$out/results.jsonl" | ForEach-Object {$_ | ConvertFrom-Json}
$steps=@{};foreach($record in $records){if($null -ne $record.step){$steps[[int]$record.step]=$record}}
if($steps.Count -ne 6){throw "Expected 6 lifecycle records, got $($steps.Count)"}
if($steps[1].pending[0] -ne 1 -or $steps[1].pending[1] -ne 0){throw 'Image half-frame was not pending after source-only input'}
if(('0x{0:X8}' -f [uint32]$steps[2].code) -ne '0x80100006' -or $steps[2].pending[0] -ne 1 -or $steps[2].pending[1] -ne 0){throw 'File-mode failure did not preserve the image half-frame flags'}
if(('0x{0:X8}' -f [uint32]$steps[3].code) -ne '0xE0000314' -or $steps[3].writes.Count -ne 0 -or $steps[3].pending[0] -ne 1){throw 'Empty Image-mode call did not preserve the pending cache without output writes'}
if($steps[4].code -ne 0 -or $steps[4].pending[0] -ne 0 -or $steps[4].pending[1] -ne 0 -or $steps[4].sdkGetResult -ne $true){throw 'Completing the cached pair did not run successfully'}
if($steps[5].code -ne 0x10103002 -or $steps[5].sdkGetResult -ne $false){throw 'Blank-image solver failure did not clear SDK GetResult as expected'}
$failureStatus=@{};foreach($write in $steps[5].writes){if($write.name -eq 'ModuStatus' -or $write.name -eq 'CalibStatus'){$failureStatus[$write.name]=[int]$write.values[0]}}
if($failureStatus['ModuStatus'] -ne 0 -or $failureStatus['CalibStatus'] -ne 0){throw 'Blank-image algorithm failure did not write zero status outputs'}
$steps.Values | Sort-Object step | ForEach-Object {[pscustomobject]@{step=$_.step;name=$_.name;code=('0x{0:X8}' -f [uint32]$_.code);pending=($_.pending -join ',');sdkGetResult=$_.sdkGetResult}} | ConvertTo-Json -Compress
