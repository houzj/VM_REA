@echo off
setlocal
cd /d "%~dp0"
for /f %%G in ('powershell.exe -NoLogo -NoProfile -Command "[guid]::NewGuid().ToString()"') do set "RUN_ID=%%G"
if not defined RUN_ID (
    echo 无法创建本次启动目录。
    pause
    exit /b 1
)
set "ARTIFACTS=%~dp0analysis\manual-acceptance\flow-%RUN_ID%\build"
set "EXE=%ARTIFACTS%\bin\VisionEditor\release\VisionEditor.exe"
echo 正在构建当前工作树的 VisionEditor...
dotnet build "%~dp0src\VisionEditor\VisionEditor.csproj" -c Release --artifacts-path "%ARTIFACTS%" --nologo
if errorlevel 1 (
    echo 构建失败，请检查上方错误。
    pause
    exit /b 1
)
if not exist "%EXE%" (
    echo 找不到当前工作树的程序："%EXE%"
    pause
    exit /b 1
)
start "" "%EXE%"
