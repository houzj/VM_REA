$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/map-calibration/r04-opening-oracle'
New-Item -ItemType Directory -Force $out|Out-Null
$core='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll'
if((Get-FileHash -LiteralPath $core).Hash -ne '85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549'){throw 'Core identity mismatch'}
$alias=Join-Path $env:TEMP ('map-codeword-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Junction -Path $alias -Target $root|Out-Null
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$rel='analysis/map-calibration/r04-opening-oracle'
$source=Join-Path $alias 'tests/MapCalibration.NativeProbe/OpeningSamplingProbe.cpp'
$exe=Join-Path $alias "$rel/probe.exe"
@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /Fo"$alias/$rel/probe.obj" /Fe"$exe" "$source"
"@ | Set-Content "$out/build.cmd" -Encoding ascii
& cmd.exe /c "$out/build.cmd" *> "$out/build.log"
if($LASTEXITCODE -ne 0){throw 'Native build failed'}
$oldPath=$env:PATH
try{
 $env:PATH="$(Split-Path $core);$oldPath"
 & $exe $core (Join-Path $root "analysis/map-calibration/image-input/native-source.gray") $out 2> "$out/stderr.log" | Set-Content "$out/probe.jsonl" -Encoding utf8
 $exitCode=$LASTEXITCODE
 @{exitCode=$exitCode;platformInit=$false;deviceUse=$false;case='existing source SDK image, exact sequence1 selected opening; 80a7f0 and four fixed100x100 prelocator windows';exit='TerminateProcess self only after fflush; existing oracle lifecycle'} | ConvertTo-Json | Set-Content "$out/process.json"
 if($exitCode -ne 0){throw "Probe exit $exitCode"}
}finally{$env:PATH=$oldPath}
@($core,$source,$exe) | ForEach-Object {@{path=[IO.Path]::GetFullPath($_);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()}} | ConvertTo-Json | Set-Content "$out/provenance.json"
Get-Content "$out/probe.jsonl"

