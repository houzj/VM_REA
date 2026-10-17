param(
    [string]$SdkRoot = 'D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK',
    [string]$PythonPath = "$env:LOCALAPPDATA/Programs/Python/Python314/python.exe"
)
$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$outputRoot = Join-Path $projectRoot 'analysis/full-audit/n-point-solver'
New-Item -ItemType Directory -Force -Path $outputRoot | Out-Null
$runtimeRoot = Join-Path $SdkRoot 'Runtime/x64'
if ((Get-FileHash (Join-Path $runtimeRoot 'MVDNPointCalibCpp.dll')).Hash -ne '74B65DAACB1FC0B48ABE39EC0603DC4FC6E70C0A910680787AB318FDC7D0FA17') { throw 'Original SDK wrapper identity differs.' }
if ((Get-FileHash (Join-Path $runtimeRoot 'MVD_Algorithm.dll')).Hash -ne '85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549') { throw 'Original SDK core identity differs.' }
$vswherePath = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio/Installer/vswhere.exe'
$installationPath = & $vswherePath -latest -products '*' -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
$vcvarsPath = Join-Path $installationPath 'VC/Auxiliary/Build/vcvars64.bat'
if (!(Test-Path -LiteralPath $vcvarsPath)) { throw 'MSVC x64 build tools are required for the owned SDK probe.' }
# MSVC command-line input aliases avoid the original SDK toolchain's Unicode-path limitation.
$aliasRoot = Join-Path $env:TEMP ('npoint-solver-probe-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $aliasRoot | Out-Null
New-Item -ItemType Junction -Path (Join-Path $aliasRoot 'work') -Target $projectRoot | Out-Null
New-Item -ItemType Junction -Path (Join-Path $aliasRoot 'sdk') -Target (Resolve-Path $SdkRoot).Path | Out-Null
$aliasOutput = Join-Path $aliasRoot 'work/analysis/full-audit/n-point-solver'
$commandFile = Join-Path $outputRoot 'build.cmd'
$commandText = @"
@echo off
call "$vcvarsPath" >nul
cl /nologo /EHsc /std:c++17 /I"$aliasRoot/sdk/Includes/Algorithms" /I"$aliasRoot/sdk/Includes/Common/VisionDesigner" /Fo"$aliasOutput/solver.obj" /Fe"$aliasOutput/solver.exe" "$aliasRoot/work/tests/NativeEvidence/OriginalNPointSolverProbe.cpp"
"@
[IO.File]::WriteAllText($commandFile, $commandText, [Text.Encoding]::ASCII)
& cmd /c $commandFile 2>&1 | Tee-Object -FilePath (Join-Path $outputRoot 'build.log')
if ($LASTEXITCODE -ne 0) { throw 'Original SDK probe build failed.' }
Push-Location $projectRoot
try { & $PythonPath 'tests/Export-NPointSolverEvidence.py'; if ($LASTEXITCODE -ne 0) { throw 'Original SDK replay failed.' } }
finally { Pop-Location }
