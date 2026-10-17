#Requires -Version 7.0
param([ValidateSet('N点标定','N图像标定','平移旋转标定','旋转标定','标定加载')][string]$Tool='N点标定',[string]$SampleCalibFile='')
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$module=@{'N点标定'='vision.n-point-calibration';'N图像标定'='vision.n-image-calibration';'平移旋转标定'='vision.translation-calibration';'旋转标定'='vision.rotate-calibration';'标定加载'='vision.read-calib-file'}[$Tool]
$run=Join-Path $workspace ('analysis/manual-acceptance/'+[guid]::NewGuid().ToString('N'))
$artifacts=Join-Path $run 'build'
New-Item -ItemType Directory -Path $run | Out-Null
& dotnet build (Join-Path $PSScriptRoot 'Calibration.ManualAcceptance/Calibration.ManualAcceptance.csproj') "-p:ArtifactsPath=$artifacts" *> (Join-Path $run 'build.log')
if($LASTEXITCODE -ne 0){throw "验收程序构建失败，日志：$run/build.log"}
$exe=Join-Path $artifacts 'bin/Calibration.ManualAcceptance/debug/Calibration.ManualAcceptance.exe'
# A visible window is intentional: the user explicitly requests interactive acceptance.
$arguments=@($module)
if($SampleCalibFile){if($Tool -ne '标定加载'){throw 'SampleCalibFile只用于标定加载的公共配置框架验收'};$sample=[IO.Path]::GetFullPath($SampleCalibFile);if(-not (Test-Path -LiteralPath $sample -PathType Leaf)){throw "验收样本不存在：$sample"};$arguments+=('"'+$sample+'"')}
$process=Start-Process -FilePath $exe -ArgumentList $arguments -WorkingDirectory $workspace -PassThru -RedirectStandardOutput (Join-Path $run 'stdout.log') -RedirectStandardError (Join-Path $run 'stderr.log')
@{tool=$Tool;module=$module;processId=$process.Id;executable=$exe;scope='tool-plugins';banner='插件线验收';userAccepted=$false} | ConvertTo-Json | Set-Content (Join-Path $run 'launch.json')
Write-Output "已启动 $Tool 手动验收，顶部橙色条标记：插件线验收。"
Write-Output $run
