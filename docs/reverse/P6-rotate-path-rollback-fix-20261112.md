# P6 RotateCalibration 路径回滚修复

日期：2026-11-12

## 根因

Rotate 配置路径的源行为是：非法候选路径在输入框中保持可见，但参数模型仍保持上一次已接受值。公共 `ToolConfigurationCalibrationPathPolicy` 返回 `KeepAttemptedValueOnError=true`，导致失败候选同时污染模型的 `CurValueA`。

## 修复

校验拒绝现在只返回错误，不保留失败参数值；`ToolConfigurationCalibrationPathControls` 继续在控件文本层保存候选值。这样恢复、重复失焦提交和保存均使用已接受模型值。

## 验证

- `RotateCalibration.Checks`：720 数值检查 + 1190 总检查通过。
- 独立 WPF 行为：1259 项通过。
- `RotateCalibration.LifecycleChecks`：6 项通过。

TypedInput 依赖的历史 oracle 文件缺失，保持运行未确认。
