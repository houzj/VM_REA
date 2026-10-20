# 第二十三批 C03.3 复核：历史、底部耗时与结果缓存

基线 `403d8e4af09dca4ebc9bc4f971b1b3cb41cb4c40`；本轮仅复核源码与现有 main 实施，未修改产品代码，未访问 `tool-plugins`、原厂设备或原生运行时。

## 新的可复验证据

- `MainWindow.FrameworkResultHistory.RecordFrameworkRunHistory` 将真实 `ExecutionSessionResult.Nodes.ElapsedMilliseconds` 写入主框架的 500 条有界历史；`ResetFrameworkRunHistory` 在 `SolutionReplaced` 清除该主框架历史并将流程/模块/算法显示复位。
- `RefreshFrameworkResultTiming` 只从当前 procedure 的 `workflowSessions`、结果集合和选中节点计算显示值；`WorkflowExecution.StoreWorkflowCallResults` 先清当前调试结果再写入当前 session，`ClearWorkflowCallResults` 递归清理子流程。
- `FrameworkExecutionPolicy` 在方案替换时清除执行计数和隐藏结果标记；`ToolEditorHost.EnsureToolDebugSolution` 以 `SolutionId` 隔离调试记录。上述均是 main 自有 managed 生命周期，已有 `ExecutionViewChecks11`、`FrameworkHistoryHostChecks15`、`ResultOverlayChecks21` 覆盖。

## 尚未闭合的门槛

原始 `VmSolution` 的 `VmInterfaceManager.SetModuleResultBuffer/GetModuleResultBuffer` native 缓存所有权与清除时机仍无源码级 consumer 证据；原静态 `_waitDoneProcedureCache` 的跨实例通知清空规则也未证实。C03.3 的“全局结果缓存”部分因此不能用 main 自有字典清理冒充原行为。C02.3 的原生回调队列尚未接入 main 真实消费者，亦不能作为依赖已满足的关闭条件。

本轮没有出现新的解除条件，故 C03.3 仍为 `partial`，实施分子 **+0**。继续实施需要取得 `VmInterfaceManager` 缓存清理的原始调用者/资源证据，或由根线明确登记独立兼容实现及其边界；在此之前不新增产品代码。

## 审计证据

`analysis/full-audit/batch23-20261020/C/source-hashes.json` 保存本轮读取的 main 源文件 SHA-256；报告与前批 C03 证据共同保留。人工桌面验收、native/SDK 差分不计实施进度。
