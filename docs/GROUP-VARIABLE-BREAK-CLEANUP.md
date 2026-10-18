# Group 中断条件的局部变量引用清理

## 原始证据与实现界限

- `analysis/decompiled/VM.Core/VM.Core/VmModuleSet.cs` 的 `AddOrGetLocalVariableModule` 区分流程和 Group 的局部变量模块。
- `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.Variable/VariableManager.cs` 的 `DeleteVar` → `DeleteIO` 先解除关联订阅，再调用 `VmIO.RemoveSelf`；普通类型的 `ModifyType` 也解除原变量绑定。原始源码证明旧 IO 关系须移除，但没有证明本项目 `GroupBreak` 对话框清空时的全部视觉及提示细节。
- 本项目为 Group 中断操作数和动态数组索引保存了独立的 `WorkflowValueReference`。删除变量或改变类型时，清理这些引用是独立模型中的一致性规则，不把它称为 VM 原生内部对象布局。

## 修复

`GraphEditorDocument.SetWorkflowConfiguration` 识别删除或变型的局部变量，在 `PropagateGroupInterface` 校验之前、同一次 `Edit` 事务中清理相应 GroupBreak 引用。外部操作数与数组 `IndexSource` 按父流程作用域处理；内部操作数按 Group 拥有的内部流程作用域处理。失效操作数变为未绑定，失效动态索引清空来源并保留已配置的固定索引；一次撤销/重做同时恢复或移除变量及其引用。

## 实际验收

运行 `dotnet run --project tests/VisionEditor.GroupVariableBreakCleanupChecks/VisionEditor.GroupVariableBreakCleanupChecks.csproj -c Release`，19 项通过：实际 WPF 窗口删除父流程索引变量、修改父流程中断变量类型、删除 Group 内部变量；每次验证引用清理和单步撤销/重做。Group 起初正常执行；索引回退后继续执行；操作数解绑时运行给出 `Break operand is unbound` 诊断而不读取旧变量；重新绑定常量后恢复执行。两次保存重开均保持可运行的引用结构。

## 开放差异

原生中断配置器清空后的提示、焦点与视觉细节尚未实机差分。原生局部变量复合类型重建和完整订阅 ABI 不在此项范围。
