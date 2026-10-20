param([string]$SdkRoot='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')
$ErrorActionPreference='Stop'
$root=(Resolve-Path "$PSScriptRoot/../..").Path
$out=Join-Path $root 'analysis/camera-mapping/b2-matrix-source-20261019'
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
cl /nologo /EHsc /std:c++17 /I"$alias/sdk/Includes/Algorithms" /I"$alias/sdk/Includes/Common/VisionDesigner" /Fo"$alias/work/analysis/camera-mapping/b2-matrix-source-20261019/probe.obj" /Fe"$alias/work/analysis/camera-mapping/b2-matrix-source-20261019/probe.exe" "$alias/work/tests/CameraMapping.NativeProbe/B2MatrixTraceProbe.cpp"
"@
[IO.File]::WriteAllText((Join-Path $out 'build.cmd'),$build,[Text.Encoding]::ASCII)
& cmd /c (Join-Path $out 'build.cmd') 2>&1|Set-Content (Join-Path $out 'build.log')
if($LASTEXITCODE -ne 0){throw 'Native build failed'}
$env:PATH="$SdkRoot/Runtime/x64;"+$env:PATH
$dll=Join-Path $SdkRoot 'Runtime/x64/MVDMatrixToParamCpp.dll'
$cases=[ordered]@{}
$records=@(Get-Content "$root/analysis/camera-mapping/reachable-matrix-20261019/results.jsonl"|ForEach-Object{$_|ConvertFrom-Json})
foreach($name in @('seed-33-extent-1000-grid-4','seed-15-extent-10000-grid-4','seed-16-extent-10-grid-4','seed-15-extent-1000-grid-4','seed-33-extent-1000-grid-2')){$record=$records|Where-Object{$_.case -eq $name};$cases[$name]=$record.matrix}
$lines=[Collections.Generic.List[string]]::new()
foreach($entry in $cases.GetEnumerator()){
 $argsList=@($dll,$entry.Key)+@($entry.Value|ForEach-Object{([double]$_).ToString('R',[Globalization.CultureInfo]::InvariantCulture)})
 $native=& (Join-Path $out 'probe.exe') @argsList
 if($LASTEXITCODE -ne 0){throw "SDK probe failed: $($entry.Key)"}
 foreach($line in $native){$lines.Add($line)}
}
$lines|Set-Content (Join-Path $out 'results.jsonl') -Encoding utf8
Get-FileHash $dll,$PSScriptRoot/B2MatrixTraceProbe.cpp,$PSCommandPath,"$root/src/VisionPlugins.CameraMapping/CameraMappingPlugin.cs","$root/src/VisionPlugins.CameraMapping/CameraMappingLinearAlgebra.cs"|ForEach-Object{"$($_.Hash)  $($_.Path)"}|Set-Content (Join-Path $out 'sha256.txt') -Encoding utf8
"Native cases: $($cases.Count)"
