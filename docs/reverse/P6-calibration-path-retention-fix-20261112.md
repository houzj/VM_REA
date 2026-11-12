# P6 标定路径拒绝值保留修复

日期：2026-11-12

MapCalibration.PathChecks 首次暴露公共策略偏差：Map 的原厂路径控件在后缀/目录拒绝后保留候选文本，但当前共享策略回滚了节点值。修复为让 `ToolConfigurationCalibrationPathPolicy.Attach` 接受显式 `keepAttemptedValueOnValidationError`，Map、CalibrationBoard、Distortion、NImage、NPoint、Translation 使用保留合同；需要原生回滚的提供者仍可关闭该选项，Rotate 的专用写入路径保持原行为。

验证：`Calibration.PathPolicyChecks` 85 项通过；`MapCalibration.PathChecks` 通过路径节点、通知、后端隔离和 picker 历史前 16 项，随后因缺少 `analysis/map-calibration/native/reader-8.xml` 停止。产品策略修复已构建通过，缺失原件保持开放。
