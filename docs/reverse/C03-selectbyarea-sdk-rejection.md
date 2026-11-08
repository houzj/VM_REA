# C03 SelectByArea SDK 边界

## 证据范围

本记录使用原厂 SDK C++ 探针在同一输入 `D:\\MyWork\\图片库\\标定\\1.jpg` 上分别设置 `SelectByArea=True` 与 `SelectByArea=False`。探针的基础阈值、连通性、面积范围和输入图像保持一致。

证据文件为 `analysis/full-audit/batch1-20261107/C03/selectbyarea-sweep/selectbyarea-summary.json`。两次参数设置均返回 `paramException`，随后 SDK 仍报告相同结果，Blob 数量 47、总面积 973202、首个 Blob 面积 718420。

## 结论

- 当前公开 SDK 参数入口不接受 wrapper 的 `SelectByArea` 字段，参数异常发生在 SDK 设置阶段。
- 在该异常路径下，SDK 结果没有因字段值变化而变化。
- 原厂 wrapper 源码把 `SelectByArea` 写入模块参数存储，`LimitJudge` 只生成状态位，`ProcessOutput` 再从 SDK 结果对象发布数量、总面积和向量。因此不能把本次 SDK 参数异常解释成原厂 VM 已确认的筛选语义。
- 本地 `BuiltinModules.cs` 目前对 `SelectByArea=true` 直接按面积过滤。该行为继续标记为独立实现，直到恢复 wrapper 到 SDK 的完整参数桥接和原厂运行时结果向量。

## 未决项

尚未获得原厂 VM 对完整 `ModuleFrame` 参数的 native payload，以及 45 个结果元素的完整向量。该缺口仍阻止对 `SelectByArea` 的最终语义归因。
