# 原生数组索引编辑器专项核对（2026-10-17）

本报告只核对原生 `Apps.XmlParser`、`Apps.XmlUI` 反编译证据与框架当前 Group 中断条件索引界面，不推断未观察的 VM 运行行为，也不把 Group 独立实现称为原生界面复刻。

## 原生证据与状态机

| 入口或状态 | 原生源码 | 可确认行为 |
| --- | --- | --- |
| 索引源类型 | `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/ButtonSelecter.cs:36-41,288-318` | `IndexOpenSetter` 使用 `GetFrontFilterTreeOperation`，参数固定为 `int`；普通 `DropOpenSetter` 使用传入的输出类型。`GetFrontFilterTreeOperation.cs:22-55` 从当前模块/动态容器取前置输出树，包含空值节点；当前模块自身和内部节点均排除。 |
| 主来源打开 | `ButtonSelecter.cs:321-344` 与 `analysis/decompiled/Apps.XmlUI/styles/paramusercontroldatatemplate.xaml:307-332` | 链按钮打开时调用 `DropOpenSetter` 刷新树；设置 `IsSubscribeIndex=false`。独立订阅窗已开时选中当前参数并关闭本地来源 Popup。 |
| 主来源选择 | `ButtonSelecter.cs:347-399` | 叶 `VmIO` 双击时保留旧的 `[...]` 尾部，仅替换模块路径；无括号时写 `ModulePathName + "[]"`。空 Holder 清空来源，选择后关闭 `IsDropDownOpen`。 |
| 光标进入索引 | `ButtonSelecter.cs:517-585`；`analysis/decompiled/Apps.XmlUI/Apps.XmlUI.ParamTab/DeleteTextBox.xaml.cs:66-91` | 文本框焦点/选区改变触发；只在可编辑、文本长度大于 1、光标前最近未闭合 `[` 且光标后最近未嵌套 `]` 时刷新固定 `int` 来源树，记录光标位置并打开 `IsTextDropDownOpen`。失焦关闭正文 Popup；光标不在括号内也关闭。`DeleteTextBox` 仅在选区为空或全选时派发状态。 |
| 正文 Popup | `paramusercontroldatatemplate.xaml:255-305` | `ButtonSelecter_sub` 两列 `* / 32`。第 0 列文本上的 Popup 内容为 `SubTreeView`，宽高均 180；`IsTextPopupVisible` 控制显示；叶双击调用 `IndexClickCommand`。 |
| 索引选择/清空 | `ButtonSelecter.cs:455-515` | `IndexClick` 先删掉光标所在方括号内的旧内容，再插入选中的 `VmIO.ModulePathName`；空 Holder 仅清空括号内容。清空操作寻找光标前的 `[` 和后面的 `]`，异常被原码吞掉。索引选完把 `IsSubscribeIndex` 复位为 false。源码未在此处显式关 `IsTextDropDownOpen`。 |
| 独立订阅窗 | `ButtonSelecter.cs:578-584,598-629`；`analysis/decompiled/Apps.XmlUI/Apps.XmlUI/ParamSubscribeWindow.xaml:1-46` | 独立窗 240×480。窗口绑定 `SelectedSubscribeParam` 与 `IsSubscribeIndex`；后者切换普通树/索引树，两棵树分别派发 `ClickCommand`/`IndexClickCommand`。正文光标在括号内且独立窗打开时设置 `IsTextPopupVisible=false` 并 `IsSubscribeIndex=true`，使索引选取转移到独立窗。 |
| 来源树叶提交 | `analysis/decompiled/Apps.XmlUI/Apps.XmlUI.ParamTab/SubTreeView.xaml:85,135-143`；`SubTreeView.xaml.cs:116-126,211-267` | 仅无子项的 `TreeViewItem` 鼠标双击触发选择；`VmIO.ModulePathName` 去掉 `[]` 用于回选。展开中间节点不是提交。 |
| 文本提交到运行数据 | `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/SetFilterSourceOperation.cs:83-111,183-205` | 解析完整来源文字后，基础来源给 `VmIO.RelateIO`，索引经 `GetBindIndex` 给独立 `VmIO.RelateIndex`；索引有 `.` 时按路径取，数字时建 `VmConstantIO`。索引解析失败警告。该层证实索引不是简单 UI 后缀。 |

`DeleteTextBox` 的 `Delete` 清空整个文本（`DeleteTextBox.xaml.cs:50-61`），且数字模式拦非数字输入、关闭粘贴命令（`96-114`）。索引所用 `ButtonSelecter_sub` 模板并未启用 `IsNumberOnly`，因此其正文容许 `模块.端口` 路径。`ClearBetweenBracket` 的去括号代码存在边界可疑处（`IndexOf` 返回 -1 时的算术），不宜复制潜在缺陷；应通过实际 VM 差分确认边界结果。

## 与现有 Group 交互的差异

`src/VisionEditor/GroupConfigurationControls.cs:32-46` 使用独立的来源按钮、数字 TextBox、宽 26 的 `…` 索引按钮和模态 `WorkflowSourcePicker`。可选择前置的标量整数，也可键入非负整数，`GroupOperand.IndexSource` 保存结构化引用；`src/VisionFramework/GroupBreak.cs:5-34` 将运行时整数解析为数组索引。这是已接通的独立实现，但不是原生的 `模块.端口[索引]` 单文本框、32 宽订阅侧按钮、180×180 正文 Popup / 240×480 独立窗双模式状态机。

当前界面 `selected?.Array==true` 才出现索引行；原生来源一选中数组即可形成 `[]`，用户将光标放入括号才弹索引树。当前常量要求非负 int，而原生 `GetBindIndex` 接受 `MathHelper.IsNumber`，具体负数、小数如何与数组运行逻辑配合还未验证。框架约束应保留，除非原生运行差分证实其他值有效。

## 可实施接口与缺口

1. **无需插件契约即可闭合的 Group 操作**：给 Group 数组操作数提供 `来源路径[整数索引]` 编辑显示；用现有 `WorkflowSourceOption` 筛前置 `DataKind.Integer`、标量来源；正文索引树只选择整数叶和“清空索引订阅”；源切换保留既有索引；光标触发正文 180×180 树；独立窗 240×480 在主来源/索引树之间切换。提交时仍写现有 `GroupOperand.Source`、`Index` / `IndexSource`，由既有 `GroupBreak` 验证和执行。
2. 建议隔离 UI 状态模型：`CurrentText`、`Caret`、`HasFocus`、`IsInlineIndexOpen`、`IsSourceOpen`、`IsDetachedOpen`、`DetachedMode=Source|Index`、`SelectedReference`。单独纯函数检测光标是否在同一对 `[]` 内及安全替换内容。索引源列表刷新在进入括号和开启 Popup 时发生，防止历史节点残留。
3. **数据缺口**：原生完整 `GetFrontFilterTree` 的 Holder 分组、全局变量、层级、翻译名、远端路径与 VM 内部元素生命周期依赖原生模块树；框架只可用现有节点/流程变量引用填充同形树。Group 的前置标量整数筛选符合当前运行约束，但不能称为全插件通用索引树。
4. 原生数组索引 UI 没有在本次运行的 VM 里做同状态截图和点击差分；源码确认的是模板/命令静态结构，视觉像素、Popup 定位、负数/小数实际运行仍待验证。专用 `ButtonSelecterConfig_sub` 在 `paramusercontroldatatemplate.xaml:2625-2690` 也有同类双 Popup，但宽度/偏移有所变化，应按具体宿主模板选择，不能全局共用一个尺寸。

建议闭合验收：添加两路数组来源和前置整数来源，选第一路后填常量索引，切换第二路确认索引保留；把光标移入/移出括号验证 180×180 索引树开关；选择整数叶、清空索引来源并改回常量；在独立窗打开时验证主树/索引树切换；保存重开执行 Group 两轮并确认订阅源更新后的数组元素及越界报告。该测试只证明托管实现行为，VM 原生运行差分仍单独记录。
