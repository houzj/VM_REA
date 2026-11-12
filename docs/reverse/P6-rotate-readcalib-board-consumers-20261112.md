# P6 Rotate/ReadCalib/Board 真实消费者回归

日期：2026-11-12

- `CalibrationRotateConsumer.Checks`：通过，覆盖 XML 来源、相机连续二进制样本、缺失文件失败和 RefreshSignal 恢复。
- `CalibrationRotateDisplayChain.Checks`：通过，覆盖成功/错误/恢复显示和节点作用域隔离。
- `CalibrationReadCalibDisplayChain.Checks`：通过，覆盖有效文件、损坏文件保留、缺输出清理、动态叶变化、非有限值安全错误和恢复。
- `CalibrationBoardRotateChain.Checks`：20 项通过，覆盖三帧标定板、36 字节 iwcal、Rotate 连续样本、保存重开、空图失败和恢复。

这些链路验证当前真实工具之间的文件/矩阵/显示消费合同；原厂 native 算法和 `.sol` 字段仍保持“运行未确认”。
