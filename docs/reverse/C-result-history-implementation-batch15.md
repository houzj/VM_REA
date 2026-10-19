# C03.3 主框架历史与耗时消费者实施（第十五批C线）

## 范围与整项判定

本批实际实施主框架自身摘要的有界保留和当前所选结果的耗时消费，不再只补静态报告。但 **C03.3 保持 partial**：原生全局结果缓存所有权、跨方案清除时机、原模块 dataRecords 内容与完整宿主差分没有恢复。本实现不是SDK缓存或原ModuleDisplayDecorator完整替代。

## 原始依据

沿用第九批身份认证、精确CIL与源码：`docs/reverse/C03-history-lifecycle-batch9.md`。

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/ModuleDisplayDecorator.cs`：MaxRecordCount=500；RefreshRecordTimerTick（约830–851）把新批插入索引0，随后裁掉500以后条目。原始项由模块dataRecords及nExecuteCount生成，不能将当前流程摘要称为原模块日志。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/StatusBar.xaml`：状态栏分别读取所选模块的ModuleTime/AlgorithmTime，以及所选流程的ProcessTime。
- `analysis/decompiled/VM.Core/VM.Core/VmModule.cs` SetModuleState：分别从info.fAlgorithmTime和info.fModuleTime填充两个不同字段。因此当前NodeRunRecord只有ElapsedMilliseconds时不能把其同时赋给算法耗时。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF/ModuleResultView.xaml.cs`：手动清理ModuleDisplayInfoList并以ModuleSource.ID调用ClearModuleHistoryResult；这不是无条件清所有全局缓存。第九批已记录方案实例替换不证明统一历史/耗时清零。

## 实施与接入合同

`src/VisionEditor.Core/FrameworkRunHistory.cs` 提供当前主框架独立流程摘要：保留500条，最新头插，执行序号独立递增，不再从当前集合长度推导（淘汰后不会重复序号）；保存流程ID、实际耗时和完整毫秒时间。Clear仅清自身集合并重置自身序号，这是明确的**独立方案替换策略**，不是宣称原厂新方案必清零。

同文件 FrameworkResultTiming 从当前ExecutionSessionResult.Nodes求主机原有流程耗时和选中可见NodeRunRecord读取模块耗时。无当前源或选中项时显示“—”；算法耗时始终“—”，不把模块总耗时冒充算法专用计时。它不执行native查询或创建另一套插件会话。

`src/VisionEditor/MainWindow.FrameworkResultHistory.cs` 为根线串行接入提供：

1. `InitializeFrameworkResultHistory()`：构造后把HistoryGrid切到新集合、订阅doc.SolutionReplaced并立即初始化清理。
2. `RecordFrameworkRunHistory(procedureId,session)`：替换RunCore的无限history.Add。
3. `ResetFrameworkRunHistory()`：自身事件订阅在方案替换时清摘要和序号，设置当前主机的初始计时文本；仅本地策略，无需根线另改共享替换方法。
4. `RefreshFrameworkResultTiming()`：在RefreshRight已获得实际selected/results后刷新所选源，遵守当前clearedWorkflowResultViews，不读取旧方案缓冲。

根线持有WorkflowExecution、MainWindow构造/RefreshRight及方案替换共享文件，本子线未修改。必须在实际接入后验证绑定，不以helper单测替代宿主消费者。

## 验证与审计

`tests/FrameworkHistoryChecks15` 为无SDK自审核：750次追加后只留最新500、稳定顺序及序号、不同流程/失败、非法数据不耗号、清理后从1计数；所选节点切换、真实session总耗时、无源/清空/未知选择、算法不可用、非法耗时和跨文化格式。输出自有GUID目录summary。

`tests/FrameworkHistoryHostChecks15` 实例化实际MainWindow/DataGrid，检查750条消费后500条头插及序号；注入明确的托管结果fixture后，通过真实RefreshRight消费selected耗时和算法不可用；调用真实doc.NewSolution/Load检查事件清摘要及重置计数。它是实际WPF消费者自审核，不是原SDK执行或交互桌面人工验收。

子线未构建、未运行测试；根线串行执行并追加真实日志路径及结果后，才可报告通过。源码及本批材料哈希在 `analysis/full-audit/batch15-20261019/C/implementation-hashes.json`。无原厂/设备操作，无长稳重跑，不碰tool-plugins。

根线实际接入与结果：构造Initialize、RunCore Record及RefreshRight timing hook已接；Core22项与实际WPF Host12项全部exit0，绑定/500淘汰/选择耗时及NewSolution/Load清理均实际执行。首轮Core调用漏目录参数的拒绝日志保留，正确参数后通过。C03.3 native缓存仍partial。
