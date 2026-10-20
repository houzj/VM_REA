param([string]$SdkRoot='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/reachable-matrix-20261019'
New-Item -ItemType Directory -Force $out|Out-Null
$prior=Get-Content "$root/docs/plugins/CAMERA-MAPPING-MATRIX-DIFFERENTIAL.json" -Raw|ConvertFrom-Json
$cases=[Collections.Generic.List[object]]::new();$text=[Collections.Generic.List[string]]::new();$ci=[Globalization.CultureInfo]::InvariantCulture
foreach($index in @(15,16,33,61)){
 $seed=$prior.native|Where-Object{$_.case -eq "seeded-projective-$index"};$h=[double[]]$seed.matrix
 foreach($extent in @(10,1000,10000)){foreach($side in @(2,4)){
  $name="seed-$index-extent-$extent-grid-$side";$points=[Collections.Generic.List[object]]::new();$text.Add("$name $($side*$side)")
  for($row=0;$row -lt $side;$row++){for($col=0;$col -lt $side;$col++){
   $x=$extent*$col/($side-1);$y=$extent*$row/($side-1);$z=$h[6]*$x+$h[7]*$y+$h[8];$u=($h[0]*$x+$h[1]*$y+$h[2])/$z;$v=($h[3]*$x+$h[4]*$y+$h[5])/$z
   $p=@([float]$x,[float]$y,[float]$u,[float]$v);$points.Add($p);$text.Add(($p|ForEach-Object{$_.ToString('R',$ci)}) -join ' ')
  }}
  $cases.Add([ordered]@{case=$name;seedMatrix=$h;points=$points.ToArray()})
 }}
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
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/camera-mapping/reachable-matrix-20261019/probe.obj" /Fe"$alias/work/analysis/camera-mapping/reachable-matrix-20261019/probe.exe" "$alias/work/tests/CameraMapping.NativeProbe/ReachableMatrixModuleProbe.cpp"
"@
[IO.File]::WriteAllText((Join-Path $out 'build.cmd'),$build,[Text.Encoding]::ASCII)
& cmd /c (Join-Path $out 'build.cmd') 2>&1|Set-Content (Join-Path $out 'build.log')
if($LASTEXITCODE -ne 0){throw 'Native build failed'}
$env:PATH="$SdkRoot/Runtime/x64;"+$env:PATH
$module='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu/IMVSCameraMapModu.dll'
& (Join-Path $out 'probe.exe') $module (Join-Path $out 'cases.txt') |Set-Content (Join-Path $out 'module-results.jsonl') -Encoding utf8
if($LASTEXITCODE -ne 0){throw 'Native module probe failed'}
$all=@(Get-Content (Join-Path $out 'module-results.jsonl')|ForEach-Object{$_|ConvertFrom-Json})
$all|Where-Object{$_.case}|ForEach-Object{$_|ConvertTo-Json -Depth 8 -Compress}|Set-Content (Join-Path $out 'results.jsonl') -Encoding utf8
Get-FileHash $module,"$SdkRoot/Runtime/x64/MVDCameraMapCpp.dll","$SdkRoot/Runtime/x64/MVDMatrixToParamCpp.dll",$PSScriptRoot/ReachableMatrixModuleProbe.cpp,$PSCommandPath|ForEach-Object{"$($_.Hash)  $($_.Path)"}|Set-Content (Join-Path $out 'sha256.txt') -Encoding utf8
"Native cases: $($cases.Count)"