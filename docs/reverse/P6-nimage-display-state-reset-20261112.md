# P6 NImage 显示、状态与 Reset 回归

日期：2026-11-12

- `CalibrationNImageDisplay.Checks`：通过，覆盖八帧有效图、空白第九帧失败、当前错误显示、批次重启和源图层保持。
- `CalibrationNImageStateChain.Checks`：26 项通过，覆盖九帧拥有状态、失败边界、恢复批次、九浮点 XML、Group→Coordinate、保存重开。
- `NImageCalibration.ResetChecks`：56 项通过，覆盖源图像列表 Reset。

原厂 native NImage 求解和人工桌面像素差分仍为“运行未确认”。
