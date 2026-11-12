# P6 Map/ReadCalibFile 宿主回归

日期：2026-11-12

- `ReadCalibFile.InputChecks`：84 项通过。
- `MapCalibration.HostChecks`：目录/注册身份检查通过 2 项后，因缺失 `analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp` 停止。

Map 宿主没有进入图像执行，不对算法或显示作结论；该 fixture 缺口与前序 Map/Distortion 资产审计一致。原厂 `.sol`、native 加载器和人工桌面差分保持“运行未确认”。
