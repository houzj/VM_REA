# G9 CalibrationBoard 工具批次

日期：2026-11-12

已复跑并记录：

- `CalibrationBoard.EnclosingChecks`：90 个原生包围圆 fixture 通过；
- `CalibrationBoard.BoundaryFitChecks`：独立边界拟合进程退出码 0，保留完整 JSON 追踪输出；
- `CalibrationBoard.ResetChecks`：19 项通过；
- `CalibrationBoardRotateChain.Checks`：20 项通过，覆盖三帧标定板、36 字节 iwcal、Rotate 连续样本、保存重开、空图失败和恢复；
- `CalibrationBoardDistortionDisplay.Checks`：板成功/失败显示清理 5 项通过，随后因缺少 distortion SDK fixture 停止；
- `CalibrationBoard.Checks`：因缺失 `analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp` 保持开放。

本批次的源码/内置样本合同得到部分复核；缺失的原件和 distortion fixture 未复制或伪造，原厂 native 与桌面像素一致性仍为“运行未确认”。
