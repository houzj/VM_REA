# C01 NImage 真实宿主回归

日期：2026-11-12

本轮验证不依赖缺失 solver-fixture 的 NImage 公共与宿主链：

- `NImageCalibration.ImageSourceChecks`：10 项通过。
- `NImageCalibration.InputReadChecks`：118 项通过。
- `NImageCalibration.HostChecks`：49 项通过。

覆盖 IMAGE 初始化与选择、物理点/旋转参数绑定、ImageSource 九帧输入、完整标定结果、轨迹显示、三页配置模板、配置会话关闭、SDK 状态导出、方案重载和非持久图像批次释放。

原厂 native 标定求解、设备输入和 `.sol` 字段保持“运行未确认”。
