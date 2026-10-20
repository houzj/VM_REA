param([string]$SdkRoot='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/matrix-differential-20261019'
New-Item -ItemType Directory -Force $out|Out-Null
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$alias=Join-Path $env:TEMP ('matrix-diff-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/sdk" -Target $SdkRoot|Out-Null
$build=@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/camera-mapping/matrix-differential-20261019/probe.obj" /Fe"$alias/work/analysis/camera-mapping/matrix-differential-20261019/probe.exe" "$alias/work/tests/CameraMapping.NativeProbe/MatrixProjectionPrecisionProbe.cpp"
"@
[IO.File]::WriteAllText((Join-Path $out 'build.cmd'),$build,[Text.Encoding]::ASCII)
& cmd /c (Join-Path $out 'build.cmd') 2>&1|Set-Content (Join-Path $out 'build.log')
if($LASTEXITCODE -ne 0){throw 'Native build failed'}
$env:PATH="$SdkRoot/Runtime/x64;"+$env:PATH
$dll=Join-Path $SdkRoot 'Runtime/x64/MVDMatrixToParamCpp.dll'
$cases=[ordered]@{}
foreach($degrees in @(0,30,45,89.999,90,90.001,135,180,225,270,315,-30,-90)){
 $a=$degrees*[Math]::PI/180;$c=[Math]::Cos($a);$s=[Math]::Sin($a)
 $cases["rotation-$degrees"]=@($c,-$s,12,$s,$c,-7,0,0,1)
}
$cases['exact-rotation90']=@(0,-1,12,1,0,-7,0,0,1)
foreach($sx in @(-2,0.001,2)){foreach($sy in @(-3,0.001,3)){$cases["scale-$sx-$sy"]=@($sx,0,12,0,$sy,-7,0,0,1)}}
foreach($factor in @(-10,-1,0.001,1,1000)){
 $cases["homogeneous-$factor"]=@(1.2,0.15,12,-0.2,0.9,-7,0.0008,-0.0011,1)|ForEach-Object{$_*$factor}
}
$cases['tiny-invertible']=@(0.000001,0,0,0,0.000001,0,0,0,1)
$cases['singular-nonzero']=@(1,0,0,0,1,0,0,0,0)
$cases['zero']=@(0,0,0,0,0,0,0,0,0)
$random=[Random]::new(20261019)
for($i=0;$i -lt 64;$i++){
 $a=0.1+10*$random.NextDouble();$b=$random.NextDouble()-0.5;$c=200000*($random.NextDouble()-0.5)
 $d=$random.NextDouble()-0.5;$e=0.1+10*$random.NextDouble();$f=200000*($random.NextDouble()-0.5)
 $g=0.00001*($random.NextDouble()-0.5);$h=0.00001*($random.NextDouble()-0.5)
 $cases["seeded-projective-$i"]=@($a,$b,$c,$d,$e,$f,$g,$h,1)
}
$lines=[Collections.Generic.List[string]]::new()
foreach($entry in $cases.GetEnumerator()){
 $argsList=@($dll)+@($entry.Value|ForEach-Object{([double]$_).ToString('R',[Globalization.CultureInfo]::InvariantCulture)})
 $native=& (Join-Path $out 'probe.exe') @argsList
 if($LASTEXITCODE -ne 0){throw "SDK probe failed: $($entry.Key)"}
 foreach($line in $native){$lines.Add('{"case":"'+$entry.Key+'","matrix":'+(ConvertTo-Json -InputObject @($entry.Value) -Compress)+',"result":'+$line+'}')}
}
$lines|Set-Content (Join-Path $out 'results.jsonl') -Encoding utf8
Get-FileHash $dll,$PSScriptRoot/MatrixProjectionPrecisionProbe.cpp,$PSCommandPath,"$root/src/VisionPlugins.CameraMapping/CameraMappingPlugin.cs","$root/src/VisionPlugins.CameraMapping/CameraMappingLinearAlgebra.cs"|ForEach-Object{"$($_.Hash)  $($_.Path)"}|Set-Content (Join-Path $out 'sha256.txt') -Encoding utf8
"Native cases: $($cases.Count)"
