param([string]$SdkRoot='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/b1-solve-source-20261019'
New-Item -ItemType Directory -Force $out|Out-Null
$prior=Get-Content "$root/docs/plugins/CAMERA-MAPPING-MATRIX-DIFFERENTIAL.json" -Raw|ConvertFrom-Json
$cases=[Collections.Generic.List[object]]::new();$text=[Collections.Generic.List[string]]::new();$ci=[Globalization.CultureInfo]::InvariantCulture
foreach($selected in @('seed-15-extent-10000-grid-2','seed-15-extent-10-grid-4')){
 $existing=Get-Content "$root/analysis/camera-mapping/reachable-matrix-20261019/cases.json" -Raw|ConvertFrom-Json
 $entry=$existing|Where-Object{$_.case -eq $selected};$cases.Add($entry);$text.Add("$selected $($entry.points.Count)")
 foreach($point in $entry.points){$text.Add(($point|ForEach-Object{([float]$_).ToString('R',$ci)}) -join ' ')}
}
$text|Set-Content (Join-Path $out 'cases.txt') -Encoding ascii
$cases.ToArray()|ConvertTo-Json -Depth 7|Set-Content (Join-Path $out 'cases.json') -Encoding utf8
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$alias=Join-Path $env:TEMP ('reachable-matrix-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/sdk" -Target $SdkRoot|Out-Null
$build=@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/camera-mapping/b1-solve-source-20261019/probe.obj" /Fe"$alias/work/analysis/camera-mapping/b1-solve-source-20261019/probe.exe" "$alias/work/tests/CameraMapping.NativeProbe/B1SolveTraceProbe.cpp"
"@
[IO.File]::WriteAllText((Join-Path $out 'build.cmd'),$build,[Text.Encoding]::ASCII)
& cmd /c (Join-Path $out 'build.cmd') 2>&1|Set-Content (Join-Path $out 'build.log')
if($LASTEXITCODE -ne 0){throw 'Native build failed'}
$env:PATH="$SdkRoot/Runtime/x64;"+$env:PATH
$module='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu/IMVSCameraMapModu.dll'
& (Join-Path $out 'probe.exe') $module (Join-Path $out 'cases.txt') |Set-Content (Join-Path $out 'module-results.jsonl') -Encoding utf8
if($LASTEXITCODE -ne 0){throw 'Native module probe failed'}
$all=@(Get-Content (Join-Path $out 'module-results.jsonl')|ForEach-Object{$_|ConvertFrom-Json})
$all|Where-Object{$_.phase -eq 'b1-nonexistent'}|ForEach-Object{$_|ConvertTo-Json -Depth 8 -Compress}|Set-Content (Join-Path $out 'results.jsonl') -Encoding utf8
Get-FileHash $module,"$SdkRoot/Runtime/x64/MVDCameraMapCpp.dll","$SdkRoot/Runtime/x64/MVDMatrixToParamCpp.dll",$PSScriptRoot/B1SolveTraceProbe.cpp,$PSCommandPath|ForEach-Object{"$($_.Hash)  $($_.Path)"}|Set-Content (Join-Path $out 'sha256.txt') -Encoding utf8
"Native cases: $($cases.Count)"