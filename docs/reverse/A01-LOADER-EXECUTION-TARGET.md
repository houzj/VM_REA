# A01 工具箱装载与执行入口证据索引

本页按现有证据重建任务卡引用。原厂装载、拖入画布和后续执行分别取证。

- 原厂 `VMControls.Data.ToolBoxData.UpdateLoad` 有隔离托管探针记录。已观察到先写 IsInit、失败保留前缀、同身份不自动重试。完整 LoadData/Reload 与宿主运行不在该探针范围内。[装载证据](A01-loader-batch1.md)
- 工具箱拖动到画布的静态调用边和原 CIL 见 [调用边](../architecture-recovery/toolbox-drag.edges.json) 与 [原 CIL](../architecture-recovery/toolbox-drag-cil-evidence.json)。从空载状态进入可执行节点仍须沿实际选中流程和命令目标核验。
- 自研宿主的显式引用、身份冲突与失败隔离由独立的托管注册合同覆盖。[失败矩阵](A-loader-failure-matrix-batch19.md)
