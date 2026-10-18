# Group 常量输入标签与执行

原始证据：`analysis/decompiled/VM.Core/VM.Core/VmConstantIO.cs` 的常量值由 `Name` 解析；`Apps.XmlParser.ParamTab/GetSelectedFilterOperation.cs` 和 `DynamicObjectConfigControl/Apps.XmlParser.ParamTab/GetFeatureRelatedIOOperation.cs` 对关联到 `VmConstantIO` 的输入直接显示 `relateIO.Name`，普通订阅才显示来源路径。原始证据支持 int/float/string 常量的原文标签；本项目的数字数组常量仍是独立合同，不宣称原生等价。

已修复：Group 双栏输入页原先把 `IntegerConstant` 当作 `NodeOutput`，按常量文本寻找节点 ID，打开真实窗口即抛 `Sequence contains no matching element`。现按来源种类解析标签：整数/数值显示存储文本，字符串显示解码后的值，数字数组显示独立合同的存储文本，节点输出仍显示节点及端口。未引入额外输入算法；运行继续使用现有 `WorkflowRunContext` 的类型解析。

`VisionEditor.GroupFailureStatusChecks` 的真实主窗口与 Group 双栏验证：整数常量 3 的来源按钮显示 `3`，实际 Group 调用输入为整数 3；保存并重读 `.vmx.json` 后再次执行仍为 3。原有输入失效、修复重试及超时恢复回归均通过，合计 10 项。尚未在真实 VM 上做同状态截图差分。
