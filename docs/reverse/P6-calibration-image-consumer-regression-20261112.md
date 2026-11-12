# P6 标定图像工具消费回归

日期：2026-11-12

- `NImageSolver.Checks`：18 项通过，数值求解输出和边界记录保持。
- `NImageCode.Checks`：163 项通过，范围限定为 codeword 层，不宣称图像检测等价。
- `DistortionCalibration.ImageSourceChecks`：11 项通过，覆盖 IMAGE 初始化、类型化 EnumEntrys、下拉选择和订阅不执行。

原厂 native 求解、图像检测、设备输入和 `.sol` 字段仍单独标记“运行未确认”。
