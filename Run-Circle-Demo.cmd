@echo off
cd /d "%~dp0"
dist\VisionRunner-Portable\VisionRunner.exe analysis\images\circle-find-sample.bmp analysis\demo-result 717 507 375 12
if errorlevel 1 (
  echo Detection failed. Review the console message.
  pause
  exit /b 1
)
echo Results saved to analysis\demo-result.json and analysis\demo-result.png
pause
