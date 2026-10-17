@echo off
cd /d "%~dp0"
pwsh.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0tests\Launch-CalibrationAcceptance.ps1"
