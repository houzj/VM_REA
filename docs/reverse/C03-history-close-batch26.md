# 第二十六轮 C线：C03.2 结果清理与历史替换门槛复核

基线 `ea64cb0e573a9e658e656c3eebc951e9daf8a514`。本轮只审查 C03.2 的结果清理配置、历史/计时与方案替换边界，没有修改产品源码，没有访问 `tool-plugins`。

## 已确认的 main 实施

`FrameworkExecutionPolicy.BeginWorkflowResults` 已读取 `IsClearLastExecuteResult`，按 `count > 1 && workStatus != 0` 的来源条件维护每流程隐藏标记；`WorkflowExecution.StoreWorkflowCallResults` 在新结果发布时清调试覆盖并递归保存 Group 调用；`FrameworkRunHistory`/`ResetFrameworkRunHistory` 在方案替换时清自身500条摘要、序号和耗时显示；`ExecutionOwnerEpoch` 与 `ReplaceExecutionScope` 拒绝旧方案/关闭后的晚到回调。这些是 main 的 managed 兼容实现，已有历史/结果视图证据，不新增可计实施单元。

## 仍不能闭合的实施门槛

原始源码只明确了 `VmSolution` 的结果缓存由 `VmInterfaceManager.SetModuleResultBuffer/GetModuleResultBuffer` 代理以及 `ModuleResultView` 按模块 ID 清理；native 结果叶字段、真实消费者、清理时机和多消费者顺序仍无源码级证据。main 当前 `ShouldClearAtBegin(count, 1)` 的 `1` 是独立适配，不能声称为原 SDK 回调 status。原静态 `_waitDoneProcedureCache` 的跨实例失效规则、失败载入是否替换实例也没有完整调用链。

因此本轮没有证据充分的最小产品改动，C03.2 继续 `partial/blocked`，实施增量 **+0**。不以新增 helper、测试或重复历史报告计分。

## 审计证据

独占审计：`analysis/full-audit/batch26-20261020/C/c03-2-history-gate.json`，含本轮读取源码及上一轮报告 SHA、精确解除条件。人工桌面验收和 native 差分不计实施进度。
