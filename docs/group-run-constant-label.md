# Group 运行参数的常量订阅回显

## 原始证据与范围

`analysis/decompiled/DynamicObjectConfigControl/Apps.XmlParser.ParamTab/GetFeatureRelatedIOOperation.cs` 第 52–55 行对 `VmConstantIO` 关联项直接回显 `relateIO.Name`；`analysis/decompiled/VM.Core/VM.Core/VmConstantIO.cs` 将整数、浮点及字符串常量解析为该名称。之前 Group 双栏运行参数行只从节点来源候选列表取名称，常量不在该列表中，已保存的有效常量订阅因此显示 `------`。

本次仅修正现有订阅的回显。整数与数值常量显示原值，文本常量显示解码后的值；节点等其他来源仍走原候选名称。没有据此推断或扩张原生来源选择器。

## 实际验收

专项 `VisionEditor.GroupRunConstantLabelChecks` 以真实 Group、内部 `GainModule`、运行参数代理和整数常量订阅运行，输出为 7；打开真实 WPF `SubprogramCallWindow` 的运行参数页，订阅行显示 7；保存 `.vmx.json` 后重开，再次运行输出 7，窗口仍显示 7。四项检查均通过。

这证明本实现的显示与数据执行闭合；未验证 VM 原生窗口的完整布局或所有常量来源选择交互，不能由专项检查推定完整原生等价。
