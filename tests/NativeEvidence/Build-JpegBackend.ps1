#Requires -Version 7.0
param([string]$SourceZip = 'analysis/downloads/jpeg/libjpeg-turbo-2.1.3.zip')
$ErrorActionPreference = 'Stop'
$workspace = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$archive = [IO.Path]::GetFullPath((Join-Path $workspace $SourceZip))
if ((Get-FileHash -LiteralPath $archive).Hash -ne 'CC868C30E5ED5AE08A103ED726E7CD235D8D7208124CEA1A4740BD8B94482EB9') { throw 'Official source archive identity differs' }
$source = Join-Path (Split-Path $archive) 'libjpeg-turbo-2.1.3'
if (!(Test-Path -LiteralPath $source)) { Expand-Archive -LiteralPath $archive -DestinationPath (Split-Path $archive) }
$runId=[Guid]::NewGuid().ToString('N')
$aliasRoot=Join-Path $env:TEMP ('vision-jpeg-build-'+$runId)
New-Item -ItemType Directory -Path $aliasRoot | Out-Null
$alias=Join-Path $aliasRoot 'workspace'
New-Item -ItemType Junction -Path $alias -Target $workspace | Out-Null
$source=Join-Path $alias 'analysis/downloads/jpeg/libjpeg-turbo-2.1.3'
$visualStudio = & 'C:/Program Files (x86)/Microsoft Visual Studio/Installer/vswhere.exe' -latest -property installationPath
if (!$visualStudio) { throw 'Visual Studio C++ tools required' }
$cmake = Join-Path $visualStudio 'Common7/IDE/CommonExtensions/Microsoft/CMake/CMake/bin/cmake.exe'
$ninja = Join-Path $visualStudio 'Common7/IDE/CommonExtensions/Microsoft/CMake/Ninja/ninja.exe'
$vcvars = Join-Path $visualStudio 'VC/Auxiliary/Build/vcvars64.bat'
$build = Join-Path $alias ('analysis/build/jpeg-backend-'+$runId)
New-Item -ItemType Directory -Force -Path $build | Out-Null
$batch = Join-Path $build 'build.cmd'
$commands=@"
@echo off
call "$vcvars" >nul
if errorlevel 1 exit /b 1
"$cmake" -S "$source" -B "$build" -G Ninja -DCMAKE_MAKE_PROGRAM="$ninja" -DCMAKE_BUILD_TYPE=Release -DCMAKE_POLICY_VERSION_MINIMUM=3.5 -DWITH_SIMD=OFF -DENABLE_STATIC=OFF -DWITH_TURBOJPEG=ON -DWITH_CRT_DLL=OFF -DCMAKE_MSVC_RUNTIME_LIBRARY=MultiThreaded
if errorlevel 1 exit /b 1
"$cmake" --build "$build" --target turbojpeg --parallel 4
"@
[IO.File]::WriteAllText($batch,($commands.Replace("`r",'').Replace("`n","`r`n")),[Text.Encoding]::ASCII)
& $batch
if ($LASTEXITCODE -ne 0) { throw 'Independent JPEG backend build failed' }
$destination = Join-Path $workspace 'src/VisionPlugins.ImageSource/runtimes/win-x64/native'
New-Item -ItemType Directory -Force -Path $destination | Out-Null
Copy-Item -LiteralPath (Join-Path $build 'turbojpeg.dll') -Destination $destination
Get-FileHash -LiteralPath (Join-Path $destination 'turbojpeg.dll')
