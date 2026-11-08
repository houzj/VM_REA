# C03 本地与原厂 SelectByArea 差异

本地 `src/VisionModules/BuiltinModules.cs` 在 `SelectByArea=true` 时直接按 `MinArea/MaxArea` 过滤组件，再发布 `BlobNum/TotalArea`。原厂 wrapper 的源码证据显示 `SelectByArea` 只转发到 SDK 参数存储，LimitJudge 只写状态位，不减少 Blob 数量或总面积。

差异记录在 `analysis/full-audit/batch1-20261107/C03/local-vs-original-selectbyarea-boundary.json`。本轮不改本地行为，因为 SDK 内部 `SelectByArea` 的精确语义仍未从一级证据恢复，贸然修改会破坏现有 C01 回归。
