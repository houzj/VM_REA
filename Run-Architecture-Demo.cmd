@echo off
cd /d "%~dp0"
dist\VisionGraphRunner\VisionGraphRunner.exe validate examples\architecture-demo.vmx.json
if errorlevel 1 goto failed
dist\VisionGraphRunner\VisionGraphRunner.exe run examples\architecture-demo.vmx.json > analysis\architecture-demo-result.json
if errorlevel 1 goto failed
type analysis\architecture-demo-result.json
echo.
echo Module graph run completed. Inspect each node output for measurement pass.
pause
exit /b 0
:failed
echo Module graph validation or execution failed.
pause
exit /b 1
