# G9 Calibration 公共合同回归

日期：2026-11-12

本轮复跑了 CalibrationConfigBridge、CalibrationLifecycleOwnership、CalibrationFailureChain、CalibrationHistoryChain 和 CalibrationResultChain：

- 配置桥覆盖 NImage、CameraMapping、NPoint、CalibrationBoard、Map、Distortion、Translation、Rotate 的真实普通配置窗进入、提交、Undo/Redo；
- 生命周期 6 项通过，覆盖真实图节点删除、Undo 恢复、方案替换和单次释放；
- 失败链 14 个失败/恢复状态通过，包含缺失输出、坏刷新、冷启动矩阵未初始化和恢复执行；
- 历史链覆盖初始、两次 Undo 与重开；结果链覆盖真实执行与重开执行。

这些结果验证公共配置、历史、失败和释放合同的当前实现；原厂 native 调用、原厂方案字段和桌面像素差分仍为“运行未确认”。
