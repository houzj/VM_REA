# Group 独立运行失败状态清理

原始证据：`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/BottomCommand.cs` 的单次执行捕获 `VmException` 并显示错误；连续执行发生 `VmException` 时清除连续标志并显示错误。`VM.Core/VmSolution.cs` 接收自运行结果时更新模块输出与运行状态。原生主窗口状态栏的精确文案和全部错误码映射尚未确认。

独立实现修复：`RunGroupDebug` 原先只在得到 `ExecutionSessionResult` 后更新主窗口状态。若首次输入准备直接抛出异常，Group 编辑器会显示错误，但主窗口还写着上一次的“Success”。现在捕获该异常时更新状态栏为本次失败信息，再原样抛给编辑器；`finally` 仍负责清理运行令牌。此处状态栏文案是本项目反馈策略，不宣称原生文字等价。

`VisionEditor.GroupFailureStatusChecks` 以真实主窗口和 Group 双栏窗口操作：必需输入有效时运行成功；解除后第一次准备失败，主窗口与编辑器都显示缺失输入而不残留成功；恢复输入后重试成功；内部慢工具 1 ms 超时后主窗口显示失败、编辑器显示超时，改为 1000 ms 后再次实际运行成功并清除错误。7 项通过。受控慢工具用于状态验证，不代表 VM 原生算法或超时码。
