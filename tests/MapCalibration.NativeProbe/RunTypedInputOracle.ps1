param([string]$OutputName="typed-input-oracle")
$Images=$false;$Module=$true;$Runtime=$false
$ErrorActionPreference='Stop'
if((Get-FileHash 'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu/IMVSMapCalibModu.dll').Hash -ne '658D63E2C4E2378E7EB1776D27B966990C0D0E01E29CA57ED82FE9ACC740FDE8'){throw 'Original module identity changed'}
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root ("analysis/map-calibration/"+$OutputName)
New-Item -ItemType Directory -Force $out|Out-Null
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$alias=Join-Path $env:TEMP ('map-calib-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/sdk" -Target 'D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK'|Out-Null
$probe="MapTypedInputOracle"
$cmd=@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/map-calibration/$OutputName/probe.obj" /Fe"$alias/work/analysis/map-calibration/$OutputName/probe.exe" "$alias/work/tests/MapCalibration.NativeProbe/$probe.cpp"
"@
[IO.File]::WriteAllText("$out/build.cmd",$cmd,[Text.Encoding]::ASCII)
& cmd /c "$out/build.cmd" 2>&1|Tee-Object "$out/build.log"
if($LASTEXITCODE -ne 0){throw 'Native build failed'}
$env:PATH="$alias/sdk/Runtime/x64;"+$env:PATH
Push-Location $out
try {
if($Module -or $Runtime){New-Item -ItemType Junction "$alias/module" -Target "D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu"|Out-Null;& "$out/probe.exe" "$alias/sdk/Runtime/x64" "$alias/module/IMVSMapCalibModu.dll" "$alias/work/analysis/map-calibration/native" 2>"$out/module-stderr.log"|Set-Content (Join-Path $out $(if($Runtime){"module-runtime-results.jsonl"}else{"module-file-results.jsonl"})) -Encoding utf8}elseif($Images){New-Item -ItemType Junction "$alias/module" -Target "D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu"|Out-Null;& "$out/probe.exe" "$alias/sdk/Runtime/x64" "$alias/module/IMVSMapCalibModu.dll" "$alias/work/analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp" "$alias/work/analysis/full-audit/n-image-calibration/solver-fixture/frame-4.bmp" "$alias/work/analysis/map-calibration/$OutputName" 2>"$out/image-stderr.log"|Set-Content "$out/image-results.jsonl" -Encoding utf8}else{& "$out/probe.exe" "$alias/sdk/Runtime/x64" "$alias/work/analysis/map-calibration/native" 2>"$out/stderr.log"|Set-Content "$out/results.jsonl" -Encoding utf8}
if($LASTEXITCODE -ne 0){throw 'SDK probe failed'}
} finally { Pop-Location }
Get-Content "$out/module-file-results.jsonl" | ForEach-Object { $record=$_ | ConvertFrom-Json; if($null -ne $record.case){ [pscustomobject]@{case=$record.case;code=$record.code;teach=$record.teach} } } | ConvertTo-Json -Compress
