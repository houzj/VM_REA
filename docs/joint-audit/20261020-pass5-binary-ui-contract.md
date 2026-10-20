# J03/J10：图像二值化输入源与配置窗口接缝，源码级对账

范围：原版 VisionMaster 4.4.0 的 `IMVSBinaryModu` 参数页，从配置窗口打开、`InputImage` 候选初始化/选中/改订阅，到底部执行和确定；对照当前 `VisionEditor` / `ImageBinaryWindow`。本轮只作代码审核，未操作用户桌面、运行原厂 SDK 或修改产品代码。截图仅提示要查的现象，不作实现规格。

## 原版已证调用链

1. `VMControls.WPF.DataForUI.VmModuleDecorator.ConfigParamsExecute` 检查 `CanOpenConfigurationInterface`，关闭其他配置窗后进入 `OpenParamsTabControl`；后者登记 `ModifyConfig`，建立 `ParamsTabControlBase`，绑定 `VMModule.Params` 和 ROI UI，设置 Owner 后 `Show`。`Closed` 回调清静态窗口引用并调用 `IUnRedoService.RemoveEmptyOperation`。证据：`analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmModuleDecorator.cs:225-280`。
2. 原版 `IMVSBinaryModuAlgorithmTab.xml:6-8,674-675` 链接共享 `Group.xml` 的 `ImageSourceGroup` 与 `BottomCommandGroup`。`Group.xml:5-38` 声明 `EnumerationG ImageSource`：初始化先运行 `GetFrontParamItemsOperation(IMAGE)` 写 `EnumEntrys`，再运行 `GetSelectedCombinationOperation(InputImage)` 写 `CurValue`；`CurValue` 触发 `SetCombinationSourceOperation(InputImage)`。
3. `Apps.XmlParser.ParamTab.GetFrontParamItemsOperation.Operate` 从页根拿到 `VmModule`，调用 `GetFrontIOWithLeafItemsByName("IMAGE")`，无触发地写 `EnumEntrysA`。`VmModule.GetFrontIOWithLeafItemsByName` 经 `GetFrontParamItems` → `GetFrontOutputItems` → 递归 `PreModules` 收集前置模块，另加入全局变量模块 ID 13000、局部变量模块和非 `VmSolution` Parent；前置模块取 Outputs，Parent 取 Inputs，再按 IMAGE、`CanSubscribeA && Visible` 过滤，返回 `ModulePathName`。证据：`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/GetFrontParamItemsOperation.cs:24-56`、`analysis/decompiled/VM.Core/VM.Core/VmModule.cs:3953-3996,4049-4078`。
4. `GetSelectedCombinationOperation.Operate` 查询 `GetInputIOWithLeafByName("InputImage")`，读 `SubscriptionInfo`，必要时回退 `GetSubscriptionInfo()`，无触发地写 `CurValueA`。用户选择时 `SetCombinationSourceOperation.Operate` 解析路径和可选 `[index]`，通过 `GetIoByPathFromAround` 找到源；成功即设置目标 `VmIO.RelateIO`、`RelateIndex`，空选择清 `RelateIO`，无效路径提示订阅失败并返回。证据：同目录 `GetSelectedCombinationOperation.cs:23-83`、`SetCombinationSourceOperation.cs:25-96`。这一链表明订阅变化发生于选择触发，**不依赖底部“确定”按钮再提交订阅**。
5. `Group.xml:2938-2980` 的底部连续执行、停止、单次执行分别反射调用 `BottomCommand.ContinuousSelfExcuteModule`、`StopSelfExcuteModule`、`SelfExcuteModule`；“确定”只调用 `CloseParamsTabControl`。`BottomCommand.CloseParamsTabControl` 执行 `SystemCommands.CloseWindowCommand`；`SelfExcuteModule` 在忙/未就绪/方案加载时拦截，清 Display value store 后 `owner.Run()`。证据：`BottomCommand.cs:41-44,92-133`。参数控件的逐项写入路径尚未在本轮完整追踪，故**不能**据“确定只关闭”断言全部普通参数已自动持久化，也不能断言原版存在独立取消/回滚语义。

## 当前实现与已确认偏差

本次对账按架构链路复用：`VmModuleDecorator → ParamsTabControlBase → GroupLinkItem/Apps.XmlParser → VmModule/VmIO` 是原版**公共配置/输入/执行/关闭链**，当前对应 `OpenToolConfiguration → ToolConfigurationWindowHost/ToolEditorContext → WorkflowSources/GraphEditorDocument`，应在跨工具公共审核中只计一次。`IMVSBinaryModuAlgorithmTab.xml` 引用 `ImageSourceGroup`、其 `InputImage` 字段名，以及当前 `ImageBinaryWindow.InputRow`、二值化参数 `Apply`/ROI/显示更新，属于 **Binary 专属接入与事件分支**；后续其他工具只核各自资源链接、端口映射和特有控件事件，避免把同一公共候选/窗口机制反复审核成多个工具成果。

`MainWindow.OpenToolConfiguration` 创建 `ToolEditorContext`，经 `ConnectToolEditorServices` / `ConnectConfigurationRoi` 交给插件编辑窗，再由 `ToolConfigurationWindowHost.Open` 设置 Owner、`Show` 并管理关闭；源码：`src/VisionEditor/ToolConfigurationHost.cs:16-43`、`src/VisionPluginContracts/ToolConfigurationWindowHost.cs:18-39`。`ImageBinaryWindow.InputRow` 通过 `context.InputLabel` 显示当前源，点击 `context.EditInput`；`EditToolInput` 用 `WorkflowSources(flow,input.Kind)` 枚举候选，弹 `WorkflowSourcePicker`，选中后立即 `SetProcedureInputSubscription`，再使调试结果失效。源码：`src/VisionPlugins.ImageBinary/ImageBinaryWindow.xaml.cs:31`、`src/VisionEditor/ToolEditorHost.cs:44-46`。

**新增已证偏差：输入源候选范围。** `WorkflowSources` 对流程中**所有节点**的可订阅兼容输出展开；`EditToolInput` 只排除当前节点并筛基数，没有按目标节点的 `PreModules`/执行前驱过滤。原版 `GetFrontIOWithLeafItemsByName` 对一般模块只取前置模块。故当前选择器可能展示非前置节点的 IMAGE 输出；随后 `GraphEditorDocument.SetProcedureInputSubscription` 的 `GraphValidator` 才可能以 `DataExecutionOrder` / `Cycle` 拒绝提交。这是“候选列表先显示、提交时才失败”的源码级偏差，不以截图推断。`src/VisionEditor/WorkflowConfigurationHost.cs:15`、`src/VisionEditor/ToolEditorHost.cs:46`、`src/VisionEditor.Core/GraphEditorDocument.cs:159-163`。同时原版明确加入全局/局部/Parent 候选；当前是当前流程输入/变量与节点输出的独立模型，不能简单声称名称或层级完全等价。

**命令序列差异：** 当前 `ImageBinaryWindow` 确定处理为 `Session.StopAsync → Apply → Close`，单次运行是 `Apply → context.Run`；原版底部确定的资源绑定是 `CloseParamsTabControl`，单次执行是 `SelfExcuteModule → owner.Run`。当前输入订阅本身也在选择后立即写入。参数最终状态是否有用户可见差异，要继续追原版 `NameValueItem` 参数 Setter/底层 SDK 写入和窗口关闭时保存路径，不能凭按钮名称判定。

## 解阻与复验

- 最小代码探针：建立三个节点 A、B、C，仅执行前驱 A→B，令 C 也有 IMAGE 输出。打开 B 的二值化输入选择器；检查当前候选出现 C，而原版 `GetFrontIOWithLeafItemsByName` 对同构 `PreModules={A}` 不应列 C。可在隔离测试宿主构造该图并对 `WorkflowSources` 列表断言；不需要操作用户窗口。提交 C 时另断言 `GraphValidator` 返回实际结果，避免把“可能拒绝”写成必然拒绝。
- 若决定修复，先由主框架所有者定义前置候选的图语义与 Group/流程输入/变量例外，再改共享 `WorkflowSources` 或工具选择入口；插件 `ImageBinaryWindow.InputRow` 不应自行另造一套过滤。当前记录为审核发现，**没有实施修复**。
- 未解：原版普通参数项 Setter→SDK 参数写入及关闭时 undo/save；`GetIoByPathFromAround` 对跨 Group 路径的全部分支；原版 native `owner.Run()` 实际线程/耗时；当前与原版候选名称的精确映射。它们不影响上述普通前驱候选偏差的结论，但阻止宣称完整 UI/执行等价。

原始证据 SHA256 和复验命令见 `analysis/joint-audit/binary-ui-contract/evidence-index.md`。REA 直接检查 `IMVSBinaryModu.dll` 显示其为 native PE；检查 `IMVSBinaryModuCs.dll` 显示托管 4.6.1 且无内嵌资源。`Apps.XmlParser.dll` 的直接成员枚举遇 64 MiB 表示预算上限，因此成员体引用既有本地反编译，并保留这一取证限制。
