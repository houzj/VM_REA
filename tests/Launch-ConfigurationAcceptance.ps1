#Requires -Version 7.0
$ErrorActionPreference='Stop'
$workspace=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$run=Join-Path $workspace ('analysis/manual-acceptance/framework-'+[guid]::NewGuid().ToString('N'))
$artifacts=Join-Path $run 'build'
New-Item -ItemType Directory -Path $run | Out-Null
& dotnet build (Join-Path $PSScriptRoot 'Calibration.ManualAcceptance/Calibration.ManualAcceptance.csproj') "-p:ArtifactsPath=$artifacts" --nologo -v:q *> (Join-Path $run 'build.log')
if($LASTEXITCODE -ne 0){throw "配置验收构建失败：$run/build.log"}
$exe=Join-Path $artifacts 'bin/Calibration.ManualAcceptance/debug/Calibration.ManualAcceptance.exe'
# Visible window is intentional: the user explicitly requested manual acceptance.
$process=Start-Process -FilePath $exe -ArgumentList '--framework' -WorkingDirectory $workspace -PassThru -RedirectStandardOutput (Join-Path $run 'stdout.log') -RedirectStandardError (Join-Path $run 'stderr.log')
@{scope='tool-plugins';kind='actual MainWindow, source + read common configuration';processId=$process.Id;executable=$exe;manualAccepted=$false} | ConvertTo-Json | Set-Content (Join-Path $run 'launch.json')
Write-Output $run
