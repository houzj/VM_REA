@echo off
chcp 65001 >nul
cd /d "%~dp0"
pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0tests\Launch-CalibrationAcceptance.ps1" -Tool "标定板标定" -WaitForExit
