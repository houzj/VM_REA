# 第二十四轮 C02.3：main 回调消费者接入

基线：`6fc5e70942e5e021f5de117b65c9163a5ca2ad50`。本轮将第二十二轮已通过的独立双缓冲队列接入 main 的真实流程结果发布路径。

## 实施

- `MainWindow.FrameworkExecutionOwnership.cs` 增加延迟创建的 `FrameworkCallbackQueue<Action>`，由 WPF Dispatcher 安全分发；默认 `CacheEnabled=false` 保持现有同步行为。
- `WorkflowExecution.RunCore` 在 owner 校验后使用 `PublishModuleResult`：同步提交 `StoreWorkflowCallResults`、耗时和历史；再发布带 owner/关闭检查的结果刷新回调。
- `OnClosed` 关闭队列；旧方案或关闭窗口的回调在 Dispatch 前被丢弃，不写入新方案。

## 自审核

`dotnet build src/VisionEditor/VisionEditor.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false --no-restore`：0 warning / 0 error。既有 `tests/ExecutionViewChecks11`：12/12 PASS；第二十二轮独立队列：15/15 PASS。实现计入 C02.3，原生 WorkStatus 多消费者、SDK callback 线程和设备路径仍列为未知，不宣称原厂等价。

