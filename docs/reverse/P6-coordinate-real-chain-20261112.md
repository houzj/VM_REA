# P6 Coordinate 真实链回归

日期：2026-11-12

- `CalibrationCoordinateRoiChain.Checks`：通过，覆盖 100×80/200×80 图像、归一化 ROI、Draw 矩阵、显示、热状态失败保留、Undo 和重开。
- `CalibrationCoordinatePointChain.Checks`：矩阵和点路径各执行两次通过，非法直接 CalibData→CalibMatrix 连接被拒绝且不污染图。
- `CalibrationCoordinateTerminal.Checks`：通过，覆盖终端 Draw、Group 输出、标量叶、显示、冷/热缺 ROI、Undo 和保存重开。

这些检查证明 Coordinate 的真实图像/ROI/Group 消费链可运行；native solver/设备和原厂 `.sol` 仍保持“运行未确认”。
