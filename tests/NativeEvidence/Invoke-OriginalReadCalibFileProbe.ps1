#Requires -Version 7.0
param([string]$Install='D:/Program Files/VisionMaster4.4.0')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$run=[guid]::NewGuid().ToString('N')
$relative="analysis/full-audit/read-calib-file/runtime/run-$run"
$output=Join-Path $workspace $relative
New-Item -ItemType Directory -Path $output | Out-Null
$alias=Join-Path $env:TEMP "read-calib-probe-$run"
New-Item -ItemType Junction -Path $alias -Target $workspace | Out-Null
$binary=Join-Path $Install 'Applications/Module(sp)/x64/Calibration/ReadCalibFileModu/ReadCalibFileModu.dll'
if((Get-FileHash -LiteralPath $binary).Hash -ne '86B8B112ADED58DE27A41A5EDE6EC6B41117D7DB8D0C5934647FA9514E6FA403'){throw 'Original module identity changed'}
$exe=Join-Path $alias "$relative/OriginalReadCalibFileProbe.exe"
$source=Join-Path $alias 'tests/NativeEvidence/OriginalReadCalibFileProbe.cpp'
$build=Join-Path $output build.cmd
@"
@echo off
call "C:/Program Files/Microsoft Visual Studio/18/Community/VC/Auxiliary/Build/vcvars64.bat" >nul
cl /nologo /EHsc /std:c++17 /Fo"$alias/$relative/probe.obj" /Fe"$exe" "$source"
"@ | Set-Content -LiteralPath $build -Encoding ascii
& cmd.exe /c $build *> (Join-Path $output build.log)
if($LASTEXITCODE -ne 0){throw "Build failed: $output"}
$translation=Get-Content (Join-Path $workspace 'docs/plugins/translation-calibration-evidence.json') -Raw | ConvertFrom-Json
$rich=$translation.files | Where-Object name -EQ 'camera1-separate-teach' | Select-Object -First 1
$rich.xml | Set-Content (Join-Path $output rich.xml) -Encoding utf8
'<CalibInfo><CalibInputParam/><CalibOutputParam><ParamItem ParamName="CalibMatrix" DataType="FloatList"><ParamValue>1</ParamValue><ParamValue>0</ParamValue><ParamValue>0</ParamValue><ParamValue>0</ParamValue><ParamValue>1</ParamValue><ParamValue>0</ParamValue><ParamValue>0</ParamValue><ParamValue>0</ParamValue><ParamValue>1</ParamValue></ParamItem></CalibOutputParam></CalibInfo>' | Set-Content (Join-Path $output minimum.xml)
'<CalibInfo><CalibInputParam/><CalibOutputParam/></CalibInfo>' | Set-Content (Join-Path $output empty-sections.xml)
'<Other><CalibInputParam/><CalibOutputParam/></Other>' | Set-Content (Join-Path $output wrong-root.xml)
'<CalibInfo><broken>' | Set-Content (Join-Path $output malformed.xml)
'<Any><CalibInputParam><P ParamName="Sample" DataType="int"><V>12tail</V></P><P ParamName="BadFloat" DataType="float"><V>oops</V></P></CalibInputParam><CalibOutputParam><P ParamName="AnInt" DataType="int"><V>17tail</V></P><P ParamName="AFloat" DataType="float"><V>2.5tail</V></P><P ParamName="AText" DataType="string"><V>Hello</V></P><P ParamName="Floats" DataType="FloatList"><V>1</V><V>bad</V><V>3</V></P><P ParamName="Pose" DataType="CalibPointF"><CustomX>4</CustomX><CustomY>5</CustomY><CustomR>6</CustomR></P><P ParamName="Ignored" DataType="Other"><V>9</V></P></CalibOutputParam></Any>' | Set-Content (Join-Path $output custom-types.xml)
'<Any><CalibInputParam/><CalibOutputParam><P ParamName="Pose" DataType="CalibPointF"><X>1</X><Y>2</Y><R>3</R><Extra>4</Extra></P></CalibOutputParam></Any>' | Set-Content (Join-Path $output pose-four.xml)
'<Any><CalibInputParam/><CalibOutputParam><P ParamName="Pose" DataType="CalibPointF"><X>1</X><Y>2</Y></P></CalibOutputParam></Any>' | Set-Content (Join-Path $output pose-short.xml)
'<Any><CalibInputParam/><CalibOutputParam><P ParamName="Pose" DataType="CalibPointF"><X>-999</X><Y>-99999</Y><R>-990</R></P><P ParamName="Pose2" DataType="CalibPointF"><X2>1</X2><Y2>2</Y2><R2>-990.001</R2></P></CalibOutputParam></Any>' | Set-Content (Join-Path $output sentinel-r.xml)
@($binary,$source,$exe) | ForEach-Object { @{path=[IO.Path]::GetFullPath($_);sha256=(Get-FileHash -LiteralPath $_).Hash.ToLowerInvariant()} } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output provenance.json)
$taskPath=$env:PATH
try {
 $env:PATH="$(Join-Path $Install 'Applications/PublicFile/x64');$(Join-Path $Install 'Applications/ModuleProxy/x64');$taskPath"
 $arguments=@($binary,(Join-Path $alias $relative)) | ForEach-Object {'"'+$_+'"'}
 $process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $output -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $output probe.jsonl) -RedirectStandardError (Join-Path $output stderr.log)
 if(!$process.WaitForExit(60000)){$process.Kill();throw "Owned probe timed out: $output"}
 $process.Refresh()
 @{exitCode=$process.ExitCode;scope='Source-confirmed own native ctor instance; synthetic ModuleFrame host; no platform Init or device'} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $output process.json)
 if($process.ExitCode -ne 0){throw "Owned probe exit $($process.ExitCode): $output"}
} finally {$env:PATH=$taskPath}
Get-Content (Join-Path $output probe.jsonl) | ForEach-Object {$_ | ConvertFrom-Json} | Where-Object { $_.phase -in @('run','load') } | Select-Object phase,label,code
Write-Output $output
