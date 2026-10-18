@echo off
cd /d "%~dp0"
if not exist "%~dp0src\VisionEditor\bin\Release\net10.0-windows\VisionEditor.exe" (
  dotnet build "%~dp0src\VisionEditor\VisionEditor.csproj" -c Release --nologo
  if errorlevel 1 exit /b %errorlevel%
)
start "" "%~dp0src\VisionEditor\bin\Release\net10.0-windows\VisionEditor.exe"
