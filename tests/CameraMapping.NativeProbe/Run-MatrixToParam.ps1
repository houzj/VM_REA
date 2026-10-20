$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$sdk='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK'
$native='D:/MyWork/REA逆向测试/.tools/native-analysis'
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$vs=& $vswhere -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvars=Join-Path $vs 'VC/Auxiliary/Build/vcvars64.bat'
$out=Join-Path $root 'analysis/camera-mapping/native/matrix-to-param'
New-Item -ItemType Directory -Force $out|Out-Null
$alias=Join-Path $env:TEMP ('matrix-to-param-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory $alias|Out-Null
New-Item -ItemType Junction "$alias/work" -Target $root|Out-Null
New-Item -ItemType Junction "$alias/sdk" -Target $sdk|Out-Null
$build=@"
@echo off
call "$vcvars" >nul
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/camera-mapping/native/matrix-to-param/probe.obj" /Fe"$alias/work/analysis/camera-mapping/native/matrix-to-param/probe.exe" "$alias/work/tests/CameraMapping.NativeProbe/MatrixToParamProbe.cpp"
"@
[IO.File]::WriteAllText((Join-Path $out 'build.cmd'),$build,[Text.Encoding]::ASCII)
& cmd /c (Join-Path $out 'build.cmd') 2>&1|Tee-Object (Join-Path $out 'build.log')
if($LASTEXITCODE -ne 0){throw 'MatrixToParam probe build failed'}
$env:PATH="$sdk/Runtime/x64;"+$env:PATH
$dll=Join-Path $sdk 'Runtime/x64/MVDMatrixToParamCpp.dll'
$cases=[ordered]@{
 identity=@(1,0,0,0,1,0,0,0,1)
 translation=@(1,0,12,0,1,-7,0,0,1)
 rotation90=@(0,-1,12,1,0,-7,0,0,1)
 anisotropicScale=@(2,0,12,0,3,-7,0,0,1)
 affineSkew=@(1,0.25,12,0.3,1,-7,0,0,1)
 projective=@(1.2,0.15,12,-0.2,0.9,-7,0.0008,-0.0011,1)
 singularNonzero=@(1,0,0,0,1,0,0,0,0)
 allZero=@(0,0,0,0,0,0,0,0,0)
}
$lines=[Collections.Generic.List[string]]::new()
foreach($entry in $cases.GetEnumerator()){
 $argsList=@($dll)+@($entry.Value|ForEach-Object{([double]$_).ToString('R',[Globalization.CultureInfo]::InvariantCulture)})
 $outputLines=& (Join-Path $out 'probe.exe') @argsList
 if($LASTEXITCODE -ne 0){throw "Probe failed for $($entry.Key): exit=$LASTEXITCODE"}
 foreach($line in $outputLines){$lines.Add('{"case":"'+$entry.Key+'","matrix":'+(ConvertTo-Json -InputObject @($entry.Value) -Compress)+',"result":'+$line+'}')}
}
$lines|Set-Content (Join-Path $out 'results.jsonl') -Encoding utf8
Get-FileHash $dll,$PSScriptRoot/MatrixToParamProbe.cpp,$PSScriptRoot/Run-MatrixToParam.ps1 | ForEach-Object{"$($_.Hash)  $($_.Path)"}|Set-Content (Join-Path $out 'sha256.txt') -Encoding ascii
Get-Content (Join-Path $out 'results.jsonl')
