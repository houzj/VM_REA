# Group 局部变量类型切换验收

## 原生证据与范围

- `analysis/decompiled/VM.Core/VM.Core/VmModuleSet.cs` 的 `AddOrGetLocalVariableModule` 按流程或 Group 容器创建局部变量模块，分别使用 `ProcessLocalVariableModule`、`GroupLocalVariableModule`；两种作用域应独立。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.GlobalUI/GlobalVariableDialog.xaml.cs` 的 `ValueChanged` 在 `Type` 改变时调用 `VariableManager.ModifyType` 并重置绑定显示。
- `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.Variable/VariableManager.cs` 的 `ModifyType` 在普通类型变化时调用 `UnBindGlobarVarItem`；复合类型则删除并重建对应 IO。这里仅实现当前独立程序支持的 `int`、`float`、`string`、`byte` 标量变量，不声称复合变量完整对等。

## 实现与实际验收

`WorkflowLocalVariablesWindow.Commit` 在变量类型改变时，同时清除该变量作为来源的旧订阅；保留同类型名称、备注、数值编辑的订阅。原有绑定模块结果来源也按既有窗口行为清除。Group 内变量修改不会影响父流程的同名局部变量。

运行 `dotnet run --project tests/VisionEditor.GroupVariableTypeChecks/VisionEditor.GroupVariableTypeChecks.csproj -c Release`：专项实际打开 WPF 局部变量窗口，操作类型下拉框；原先 Group 节点收到整数 7，类型变更后旧订阅消失，必需输入因无来源而报告 `MissingInput`。改绑兼容整数常量后节点输出 0；保存并重开解决方案后结果仍为 0。父流程同名变量仍为 99。9 项检查通过。

## 仍待核验

原生 VM 的复合变量类型切换、原生窗口的错误提示和视觉细节尚未实机逐项比对；本轮行为是有源码依据的独立实现，并非宣称完整原生一致。
