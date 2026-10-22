# AR-001-F：停止与同 ID 方案替换后的迟到结果准入

本轮只核主框架当前实现，不把审核对话的复核请求当作已确认缺陷。原版有限正常停止链的原汇编 `analysis/framework-independent-review/round7-20261022/native/1800dd330.asm.txt`（SHA-256 `46F97CA2BAE439225BABDD07DB39F6446BD626FD054F804D1D697B70E8793939`）显示 `+0x88` 事件先于后续状态/订阅通知，最后才置 `+0x98` 事件；审核线 `round17-20261022/REVIEW.md` 给出跨模块路径。此处只能确定停止等待与全部通知完成不是同一个静态边界，不能推出原版数据/图像结果的迟到发布政策。

当前 main 的完整宿主路径：`MainWindow.StartRunCore` 持有 `activeRun` 取消源，`RunCore` 捕获 `ExecutionOwnerStamp`、`executionScope` 和文档快照；`GraphExecutor` 在模块晚返后先检查外部 cancellation token，置该节点 `Cancelled` 且不提交晚返输出。`RunCore` 的 `await Task.Run` 后检查 owner；通过后 `WorkflowCallbacks.PublishModuleResult` 先同步执行 `StoreWorkflowCallResults`、`RecordWorkflowRun` 与历史，再分派有 owner 检查的 UI 回调。`CreateExecutionProgress` 另检查 owner 和流程运行序号，阻止旧轮动态 descriptor 回填。方案替换的 `ReplaceExecutionScope` 先 `executionOwnerEpoch.Invalidate`，再取消旧运行、清缓存、释放旧 scope；`FinishWorkflowRun` 只处理同一 `activeRun` 实例。窗口关闭先取消并等待正在运行的流程结束，`OnClosed` 最终关闭 epoch。

新增 `tests/WorkflowLateOwnerChecks22` 在真实离屏 WPF `MainWindow` 注册受控不响应取消的合成模块，执行两种场景：① 停止后才释放模块，宿主保留 `Cancelled` session/节点，但无晚返 `Value` 输出；② 保存后运行、在模块待定时加载相同方案（方案/流程/节点 ID 全相同），释放旧模块后，新方案 `workflowSessions` 仍为空。两项均通过。输出 `analysis/full-audit/workflow-late-owner-20261022/summary.json`，SHA-256 `56529D13BE829A14558683D91D01CC91130CD2BC8D55037FE1EE707022360B1B`；其中记录四份当前源码 SHA、分别匹配 `VisionEditor`/`VisionFramework` 的 PDB 文档校验和、两个 PDB 哈希及实际编译宿主程序集 SHA `6AE95B98CF42A1C0A064E4B92F12AB794443BF7C982B102071D5A19D9B1E620E`。运行命令：`dotnet run --project tests/WorkflowLateOwnerChecks22/WorkflowLateOwnerChecks22.csproj -c Release --no-restore -- analysis/full-audit/workflow-late-owner-20261022`。首次新探针构建因缺 `System.IO` using 失败；首轮 PDB 检查误把框架源码查在宿主 PDB，修正后重跑通过。既有宿主 `CS8603` 警告未改。

结论限于这两条实际 main 宿主路径：停止仍可记录一条 **Cancelled** session/历史，不表示算法成功；相同 ID 方案替换后的旧结果不能写入新 session 缓存。原版停止后的图像/数据输出、插件私有副作用、配置单跑、全部队列模式以及交互桌面与设备尚未由此探针证明。该复核没有证据要求改动产品实现；AR-001-F 可按上述范围答复审核方，范围外问题继续开放。本轮实施单元增量为 0，不把补探针折算为功能完成。

## 2026-10-22 关闭待定运行补证

第三条测试在同一真实 `MainWindow.StartRunCore` 路径启动不响应取消的模块。模块待定时调用 `window.Close()`，`DrainWorkflowBeforeClose` 先取消本次关闭并置 `closeDrainPending`，窗口仍可见、运行仍待定；释放模块后，`RunCore` 将节点记为 `Cancelled` 且无成功输出，`FinishWorkflowRun` 清除活动运行，关闭续体再次调用 `Close`，`OnClosed` 置 `windowClosed`。测试再泵送 Dispatcher，检查关闭后缓存数量不变。实际关闭后缓存可留一条 **Cancelled** session；它在窗口真正关闭前已提交，并不是关闭后的新写入。

复验：`dotnet run --project tests/WorkflowLateOwnerChecks22/WorkflowLateOwnerChecks22.csproj -c Release --no-restore -- analysis/full-audit/workflow-late-owner-close-20261022`，三项 PASS；该输出 `summary.json` SHA-256 `68D2D388610195EC0B914A93100DCF1805BFDC3FB2F1CE0837227BF370996485`，记 `closedSessionCount=1`、状态 `Cancelled`、宿主程序集 SHA `A1DC8E0F1A0C6D0F2237626CB75E6E4814D430538188D8D4CE97056B5964B905`，四份 PDB 文档 SHA 均匹配当前源码。初版关闭断言错误要求缓存为空，实际失败后改为核“无成功输出、真正关闭、关闭后无新增写入”，保留失败事实。首次构建曾遇共享 `obj` 写入拒绝；隔离构建尝试另遇源文件通配导致重复程序集特性，移除自有隔离产物后重建通过。未干预其他任务进程或输出。

此补证只证明**当前独立实现**的关闭排水与成功输出隔离，不证明原版 VM 在同一时序下的回调/历史政策；未测插件私有 side effect、异步队列全部模式或用户交互桌面。产品代码未修改，实施进度不增加。
