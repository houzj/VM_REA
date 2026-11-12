# P6 Coordinate/Translation/NPoint 宿主回归

日期：2026-11-12

- `TranslationCalibration.HostChecks`：19 项通过。
- `NPointCalibration.HostChecks`：32 项通过，覆盖手动点表、旋转轴、显示、保存重载、失败清理和恢复。
- `CoordinateModule.HostChecks`：仅通过注册项；随后因原始图像 fixture 缺失停止，未修改产品代码。

Coordinate 的 fixture 缺口保持证据开放；原厂 native 求解和设备输入继续标记“运行未确认”。
