# P6 Map/Distortion 证据资产审计

日期：2026-11-12

本轮检查发现以下测试不能在当前主工作树复跑，原因均为历史工具证据资产缺失，不是产品行为结论：

- `MapCalibration.ImageFileChecks` 缺少 `analysis/map-calibration/image-fit-weight1-trace/image-results.jsonl`。
- `MapCalibration.ImageFitChecks` 缺少 `analysis/map-calibration/image-fit-trace/image-results.jsonl`。
- `DistortionCalibration.HostChecks` 缺少 original-board fixture。
- `DistortionCalibration.PluginChecks` 缺少 `analysis/distortion-calibration/trace-coded-boards/fixture.bmp`。

主窗口注册断言仍能通过到 Distortion 第 1 项；之后因 fixture 前置失败停止。没有生成替代图像或改写算法。待原始资产交接后按原命令复跑。
