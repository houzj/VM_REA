# 框架已实现部分源码级自检

更新：2026-10-18。本文先审计主框架已经实现且能在本工作树独立复验的部分，不把历史通过数当作完整原生等价。原始实现证据位于 `analysis/decompiled`；截图和操作记录写入 `analysis/full-audit`，不进入产品程序集。

## 已直接复核的链路

|区域|VM源码/资源证据|本框架实际实现|操作与数据复核|状态|
|---|---|---|---|---|
|流程Tab与空流程|`VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml(.cs)`、`VMControls.WPF/VMControls.WPF.Base/ItemsPage.cs`|`MainWindow` 的Tab/菜单命令映射、视口状态、增删改名复制和空态|本轮全量WPF序列456项完成；Tab、菜单、保存编辑由逐操作用例覆盖，截图与事件验证不等于VM桌面输入差分|部分行为通过，原生鼠标差分仍开放|
|工具箱与拖放|`VMControls.WPF/VMControls.WPF/ToolBox.xaml(.cs)`、`VMControls.Data/VMControls.Data/ToolBoxData.cs`|主框架分类、折叠/弹层、失效拖动和GraphCanvas投放|完整WPF序列中前段操作记录覆盖展开、工具列表、拖影、取消、提交、Group创建及Undo|事件路径已复核；动画、真实桌面鼠标差分仍开放|
|画布节点/连线|`VMControls.WPF/VMControls.WPF.Base/ItemsPage.cs`、`DiagramPage.xaml.cs`、`DiagramGroupListUI.xaml.cs`|节点拖动/框选/复制、吸附、线段中点/插入、Group几何|本轮全量序列456项完成，画布拖动/连线/选择相关事件用例通过；未做真实VM桌面对照|源码链已有映射，未据测试数宣称完整原生路由|
|Group与循环|`VM.Core/VM.Core/IMVSGroup.cs`、`IMVSGroup.WorkflowAudit/IMVSGroupCs/GroupParam.cs`、`DynamicObjectConfigControl/Apps.XmlParser.ParamTab/GroupRunParam.cs`|Group所有权、双栏窗口、循环/中断/代理输入输出|隔离窗口18项通过；全量WPF操作序列456项通过，含运行、循环中断、累积输出、SelfRun准备、保存重开与关闭清理|该子路径本轮通过；线程/设备回调和VM差分未关闭|
|Group运行参数原生编辑器|`DynamicNameValueItemOfGroup.cs`、`DynamicExtern.cs`、`paramusercontroldatatemplate.xaml`|当前由主框架通用编辑区展示descriptor字段；没有按 `IGroupRunParam` 调用原生 `IDecorate` 工厂，也未承接 `ROISelecter_GroupControl`|静态Group表格及ROI数据编辑可操作，但截图与原生 Group 专属控件模板明显不同|未实现的插件-facing能力；FCR-018，不能归为框架自检通过|
|参数显隐/目标树|安装目录 `Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModuleAlgorithmTab.xml`；`Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/NameValueItem.cs`|`WorkflowLocalVariablesWindow.TargetOptions` 从静态 `ModuleDescriptor` 参数建候选|实际弹窗截图 `wpf-workflow-variable-targets.png` 显示本地 `StitchEnable=False` 时仍列出起始高度/拼接高度|确认缺少原生Tab/订阅元数据；不能仅凭CustomVisible断言目标应隐藏，见FCR-019及下述修订|

## 目标树元数据与待验证差异

ImageSource 随包 ImageSourceModuleAlgorithmTab.xml 中 `StitchStartHeight` 和 `StitchHeight` 标记 `CustomVisible=False`，`StitchEnable=True` 触发器再通过 `VisibleOperation` 显示它们。原始对象模型也把 `NameValueItem.CustomVisibleA` 与 `SubscribeVisibleA` 分开：后者另受 `CanSubscribe` 和所属 `Category.SubscribeVisibleA` 控制。因此，主框架不能把“隐藏”和“不可订阅”混为一种布尔值，也不能按 `Stitch*` 参数名硬编码特例。当前 `ParameterDescriptor` 没有表达参数可见条件/订阅显示状态，主线的类型筛选只能构建静态列表。

VM源码 `DynamicNameValueItemOfGroup.OpenDialogCommandExecute` 先调用 `IGroupRunParam.SupportDisplayForGroupData()`，再回退到 `GetSupportDisplayForGroupData()`；`DynamicExtern.cs` 为 ROISelecter 创建 `ROISelecterForGroupDecorate`，XAML 的 `ROISelecter_GroupControl` 再映射到 `ModuleROIConfigControl`。本框架当前未复刻这条原生装饰器/模板链，属于插件-facing契约 FCR-018，和可见性差异FCR-019均仍未闭合。



已将该跨线契约记为 [FCR-019](CONTRACT-REQUESTS.md#fcr-019-参数目标的原生可见可订阅能力请求未实现)。在插件线提供经源码审计的元数据前，这一控件仍未闭合；其余静态可用整数目标已由实际流程变量订阅和运行数据路径覆盖。测试目标改用本地图像模式不影响执行的 `SetCameraParamWaitTime`，避免把 42ms 订阅值误当作 `Interval` 而改变测试运行时序。

## 本轮复验记录

- WPF工程构建：成功，0错误；仍有两条既有测试空值装箱警告（`MiniMapWheelParityChecks.cs`）。
- Group动态索引/复合累积独立窗口序列：18项操作均通过，含真实图像工具运行；结果落在 `analysis/full-audit/wpf-group-dynamic-index-compound.png` 与保存的 `.vmx.json`。
- 主WPF全量序列：旧阻塞已从目标节点查找推进到第二目标执行；按UI真实路径修正了验收脚本：订阅叶节点使用原生树弹层双击、Group输入订阅使用`NativeCompoundSourceSelector`，而非误找普通按钮/完整选择器窗口。全量序列自然结束，末尾记录456项通过；结果日志为 `analysis/full-audit/framework-source-self-audit-run.log`。人工检查截图确认本地静态候选状态；FCR-019具体原生集合须按下述调用链修订验证，Group实际截图确认结果数据和叠加圆形；这些WPF截图不是原VM像素对照。

## 后续自检顺序

1. 已完成：全量456项WPF事件序列自然结束，修正两处验收脚本错误，并人工检查目标树与Group运行截图。
2. 当前主线已实现子集完成本轮源码/事件自检；后续按领域继续对原始方法、资源和状态生命周期逐项核对，未知VM行为继续标明，不据测试数宣称完整复刻。
3. 插件描述符的动态可见和可订阅元数据由插件项目按FCR-019实现并交接；主框架随后消费元数据并验证两态切换、既有订阅、保存重开和运行数据。
4. FCR-018（Group专属运行参数编辑器）与FCR-019（目标参数动态可见及可订阅元数据）按双项目职责保留开放，用户决定交接时点；主线可继续审查其余独立领域，不将这些合同当作所有框架工作的前置阻塞。不能据456项事件通过宣称完整。


2026-10-19 main剩余在制审查：未支持的pointset/POLYGON映射已排除；新增20项审查场景及456项完整WPF回归通过。历史记录中的候选代码和视觉推定不作为最终合同；采纳/排除决定与精确源码证据见 MAIN-PENDING-REVIEW.md。

## 2026-10-19 目标资格结论修订

旧记录不能把CustomVisible=False直接等同为局部变量订阅树隐藏。`GlobalVariableItem.ModifyRunParamsExecute → RunParamsSelectWindow/RunParamsSelectData.GetSubRunParamsTree → AlgorithmTabRoot.GetRunParamsTree`只从`Tab_Run Params.Collection`构造目标；`AddSubChildToTree`检查DisplayNameA，未直接按CustomVisibleA过滤。`RunParamsBaseNode.IsSupportSub`另读取ISubInfo及SubscribeVisibleA。Group运行参数候选的`RunParamPickDataExtern`则明确检查CustomVisibleA，这是另一条链。当前安装ImageSourceModuleAlgorithmTab.xml只有Tab_Basic Params，运行时如何生成/替换Tab_Run Params及有效目标集合仍未知。旧文所称隐藏拼接参数错误出现应降为待验证，不作为实现合同。

安装XML的StitchEnable.CurValue和DefaultValue均为True；本地截图中的False不能称为原生默认值。XML的SHA-256为`001A57675192867594D3BBC7EFA94462C19F056583659D759CEB5648E55091E0`。最新结论以此修订和MAIN-PENDING-REVIEW.md为准。
