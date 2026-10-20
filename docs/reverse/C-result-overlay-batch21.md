# 第二十一批 C：完整流程发布与调试覆盖层

固定范围是实际 MainWindow 中先调试普通节点得到 1，再完整 StartRun 得到 2 时，完整流程结果、当前可见结果与下一次调试输入保持一致。没有将此局部宿主合同扩充为原生 SDK 清结果等价；C02.3（清结果配置、回调队列及消费者状态）仍为 partial。

## 依据与数据流

- 既有 `docs/DEBUG-SESSION-CONTRACT.md` v0.3 明确 main 完整流程运行清除调试结果覆盖层，这是已声明的独立宿主策略。
- `ToolEditorHost.CreateScopedToolEditorContext → ToolDebugSession → StoreToolDebugResult` 向 `toolDebugRecords` 写入单模块结果；`ToolDebugInputs` 先读 `workflowSessions` 再覆盖调试记录。
- `WorkflowExecution.StartRunCore → RunCore → SolutionExecutor.RunAsync → StoreWorkflowCallResults → RestoreWorkflowResults → RestoreToolDebugResults`：修前完整流程只替换 workflowSessions，旧调试覆盖层仍可重新遮盖新结果。Group 的 session.Calls 递归使用同一发布入口。
- 原 `BottomCommand.SelfExcuteModule → VmModule.Run → ModuleManager.SelfRun` 的清显示缓存与 native 调度边界已记录在上述合同。原清值叶、trueBegin、回调计数与原生消费者并不因这次 main 修复获得等价证明，细见 `C02-value-consumer-batch7.md`、`C02-begin-isolation-batch8.md`、`C-execution-policy-batch11.md`。

## 可复验探针与修改门槛

`tests/ResultOverlayChecks21` 使用真实 MainWindow、真实 Context.Run 与 StartRunCore，自有无设备模块分别输出 1、2；读取实际 session、results、ToolDebugInputs、toolDebugRecords。camera discovery switch 在构造窗口前关闭自动发现；统一失败文件与 25 秒等待期限。没有运行 SDK，没有动 tool-plugins，也没有声称用户桌面验收。

修前模式 `observe-unfixed` 必须实际观察 session=2、visible=1、nextDebugInput=1、debugCount=1。固定后模式要求 session/visible/nextDebugInput 全为 2、debugCount=0。观察先写 observation.json 再断言，失败也保留。

修前 source snapshot/hashes 位于 `analysis/full-audit/batch21-20261020/C/WorkflowExecution.before.cs` 与 `source-before-hashes.json`。修前真实执行 exit 0，5/5 反例观察成立：`result-overlay-22bb1ffd1e5944238c66f8ba2af58425/summary.json` 确认 session=2、visible=1、nextDebugInput=1、debugCount=1。已独立读取核实。

实际最小修复只在 `StoreWorkflowCallResults` 完整流程发布入口调用既有 `ClearToolDebugResults(procedure)`，沿原 session.Calls 递归消费，不增加插件配置接口、原生回调或缓存架构。公共合同未改变；这是 main 既有合同实现修正，无新增跨项目 producer/consumer 载荷。

## 完成边界

根线修后真实执行 exit 0，5/5 通过：`analysis/full-audit/batch21-20261020/C/result-overlay-52614c68e19242abb5230746ae0b3a34/summary.json`。本线独立读取确认 session=2、visible=2、nextDebugInput=2、debugCount=0。与修前五项反例是两种不同断言，不能合称十项修复通过。Group递归代码保留；本探针只覆盖普通节点，不宣称独立Group矩阵已跑。`ClearToolDebugResults`按当前solution+指定procedure过滤，发布递归按实际Calls逐一触达，未运行的流程覆盖层不因此全局删除。

**实施总进度建议 +0，C02.3 implementation 继续 partial。** 这不是人工验收未做造成的扣留：固定criterion还包含回调队列及消费者状态。原源码已经明确缓存开关、10000容量、overflow丢弃、同步/队列事件顺序与关闭不drain；main当前任务完成发布路径没有实施该回调队列合同，属于尚未实施，不能全挪为validation未知。原Begin仅清_value而merge/RelateIO可恢复Value，main隐藏结果视图亦仍是独立策略。若改变范围为main独立调度合同，应由总计划明确修改criterion及保留原剩余项，不能由本次五项测试默默改分母。

FCR判断：没有公共API、载荷或插件生命周期合同变更，不新增跨项目FCR；本地缺陷为完整流程发布未兑现既有v0.3调试覆盖层失效合同。tool-plugins保持未触。


