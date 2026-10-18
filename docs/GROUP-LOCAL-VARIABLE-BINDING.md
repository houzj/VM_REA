# Group 局部变量绑定模块结果的执行边界

## 证据和适用范围

- 原生 `VM.Core/VmModuleSet.cs` 的 `AddOrGetLocalVariableModule` 为 Group 和流程分别创建局部变量模块；`Apps.XmlParser.Variable/VariableManager.cs` 的 `GetGlobalBindingRelation`、`UnBindGlobarVarItem` 显示变量与模块订阅由变量模块维护。这确认了独立作用域及订阅管线，未证明原生选择器对每个候选的确切过滤规则。
- 本项目 `VisionFramework/WorkflowSubscriptions.cs` 以局部变量的生产节点确定依赖边，`GraphValidator.cs` 对显式执行链检查 `DataExecutionOrder`，并检测 `Cycle`。因此 UI 应在提交前排除本框架不可运行的候选；这是独立实现的运行安全边界，不宣称已复原 VM 的全部候选策略。

## 实现与实际验收

`WorkflowLocalVariablesWindow` 的“绑定模块结果”仅显示标量且不会造成 `WorkflowConfiguration`、`DataExecutionOrder` 或 `Cycle` 错误的来源，提交时再次校验。解除绑定仍可选。

运行 `dotnet run --project tests/VisionEditor.GroupVariableBindingChecks/VisionEditor.GroupVariableBindingChecks.csproj -c Release`：实际 WPF 选择器显示前置节点的 `source` 端口，隐藏消费该变量的节点自身的 `result` 端口；选择前者后 Group 输出从初始变量值 2 变为来源值 7，保存重开仍输出 7。直接构造循环来源被验证器以 `DataExecutionOrder` 拒绝。专项 11 项通过。

## 开放差异

VM 原生对变量结果绑定的前后顺序、循环错误提示，以及原生局部变量控件的完整视觉与交互尚未实机对照；本项只是依据原始订阅机制和当前图执行契约实现的闭合边界。复合变量类型仍待独立数据契约设计。
