# Shared contract requests

Each request identifies source evidence, caller, typed payload, lifecycle, compatibility and acceptance. Framework line owns shared implementation. Plugin line is already working independently; this file does not represent unreceived requests or automatically synchronize its worktree.

## FCR-001 — typed primitive arrays (framework-originated)

- Owner: framework; status: runtime foundation implemented, verification/integration tracked in HANDOFF.
- Consumers: Group accumulated outputs and future algorithm plugins. This is a framework need; no plugin-line request is claimed.
- Evidence: VM.Core.DynamicOutput.GetOutputInt/Float/String, IMVSGroupCs.GroupResult, VmIODecorate.Get/SetGroupIOReportMode. See FRAMEWORK-CONTRACTS.md.
- Payload: primitive DataKind plus Scalar/Array cardinality; immutable long/double/string list values; empty differs from missing; no scalar coercion.
- Compatibility: existing scalar defaults/constructors retained. New array JSON requires the new reader; native binary ABI/precision is not implemented.
- Accepted scope: registry/type validation, execution, subscriptions, owned Group I/O, persistence and readable output display. Group looping, accumulation semantics and full array selector UI remain open.

## FCR-002 — WPF debug session lifecycle (framework-originated, runtime foundation implemented)

- Owner: main/framework; scope: ordinary WPF tool nodes. Details: DEBUG-SESSION-CONTRACT.md.
- Confirmed source chain: DynamicParamsTabControl → BottomCommand → VmModule.Run → ModuleManager.SelfRun → IMVS_SelfExecuteOneModule(module ID). Continuous loop waits for callback; close paths stop the continuous flag and dispose parameter trees.
- Implemented: IToolDebugSession with single/continuous/stop/dispose; optional ToolEditorContext.Session; existing Run delegate routes through the session. Per-node execution uses explicit cached inputs and actual invocation argument snapshots. WPF editor and main window await active cleanup before closing.
- Compatibility: old constructors/factories and scalar/array contracts retained; plugin implementations and plugin worktree untouched. Plugin line may consume Session after integrating the main commit; buttons are not fabricated by the host.
- Open native boundaries: upstream preparation/first self-run, native in-flight cancellation, device resource ownership, full debug page UI, and Group configuration execution. The independent stop/failed-round policies are documented rather than claimed as native equivalence.
## FCR-003 — private Group LoopIndex (framework-originated, partial)

- Added WorkflowSourceKind.GroupLoopIndex with identity LoopIndex, no port, scalar Integer; valid only inside an owned Group body. It is not a public Group output.
- ExecutionSessionResult contains optional GroupLoopIndex and CompletedGroupIterations diagnostics. NodeDebugExecutor accepts an optional invocation index; the host supplies its cached current invocation snapshot.
- Existing positional arguments and plugin contracts remain unchanged. Native integer type is int32 while PortValue.Integer uses long; the loop range is bounded to int32. This does not establish native ABI parity.
- Loop break, accumulated reporting and native callback streaming are not supplied by this contract.

## FCR-004 — Group break and report snapshots (main/framework, selected types implemented)

- NodeDocument.GroupBreak adds scoped operands (Inside/Self/Source/Constant/fixed Index). Null omitted. JSON literals cloned; node references remapped on copy.
- ExecutionSessionResult adds WorkflowDisplays, DisplayErrors, GroupBroke, GroupBreakIndex, GroupDiagnostics; existing constructor parameters retained. Owned Group primitive accumulated output changes its public signature to Array. No silent scalar coercion.
- WorkflowInterfaceItem.LinkedOutputId is optional/omitted when null, linking image display to its output. Display flag materializes this actual configuration. Other compound automatic displays explicitly unsupported.
- Plugin interfaces unchanged; no plugin implementation/worktree modified. New metadata requires the current strict JSON reader; not native ABI compatibility. Details and evidence: GROUP-BREAK-ACCUMULATION.md.

## FCR-005 — owned Group input transactions and isolated execution

- ConfigureOwnedGroupInputs commits input signature, cardinality and parent bindings atomically; stable-ID renames/deletions propagate incoming connections and internal source references. No plugin contract change.
- GroupDebugExecutor accepts explicit parent cached records, caller input snapshot and optional private loop index. It executes an isolated Group through the same SolutionExecutor loop/break/accumulation path. Missing parent data fails without running upstream. Result's parent WorkflowInputs are original parent inputs; synthetic Group arguments belong only to nested call snapshots.
- Group window has optional registry/parameter/input-edit callbacks; old constructors and cross-top-level-flow compatibility retained. New callbacks distinguish parameter vs input by explicit bool, including same-name targets.
- Evidence and upstream-preparation/native lifecycle limitations: GROUP-INPUT-RUNTIME-SELFRUN.md. No native ABI guarantee or automatic integration into plugin worktree.

## FCR-006 — runtime categories, relations and declared complex editors (framework-originated)

- WorkflowConfiguration.RunCategories is a separate ordered category list; WorkflowRunParameter.CategoryId and Relations use stable IDs and typed input/parameter targets. Null fields omitted for old JSON. Snapshot/copy/delete remap all related targets. Native default maximum32 and configurable app setting are represented by the independent reader documented in FCR-020; ConfigurationHelper lookup semantics remain unverified.
- SetGroupRunParameterValue/Source and SetGroupRunComplexValue apply to all related targets in one validated edit. Incompatible types/editors, ranges and subscriptions fail atomically. Existing plugin implementations unchanged.
- ParameterDescriptor.Editor optionally declares File/Folder/Enumeration/Range/Roi and exact field names/choices. Framework does not infer editors from names or parameter types. Plugin line must supply evidence-based metadata after integration; this file does not claim it has done so. Defaults preserve existing constructors.
- Historical baseline before FCR-007: complex editors were independent functional adapters. FCR-007 replaces the ROI table with source-guided graphical editing and adds the dual-pane configurator; native SliderArrange, all ROI types, feature/model-training windows and real ROI-tool integration remain incomplete. Dedicated factory contract must be agreed with plugin line before integrating such editors; no plugin worktree changes made.

## FCR-007 — native Group parameter editors / real ROI-tool integration

Framework delivery: GROUP-NATIVE-EDITORS.md. Main includes a source-guided graphical ROI editor for its four current RoiDocument shapes and a scoped IWpfToolEditorFactory host. Image-source full editor is wired and exercised inside an owned Group. No plugin implementation or other worktree was modified.

Required from plugin line before claiming real ROI-tool/native Group editor completion:
- One real source-audited ROI algorithm tool; descriptor must declare parameter identity, exact ROI kinds/maximum, pixel vs normalized/Fixture semantics, image-input selector, fields changed together, and algorithm consumption of ROI. No inference from parameter names.
- Agree optional dedicated per-parameter WPF editor factory with immutable context, current invocation image/input snapshot, Apply/Cancel, related-target validation, async execution/session cleanup, capabilities and errors. Full tool editor factory is a separate existing capability, not a replacement for the native per-parameter ROI editor.
- Readonly input resolver is now NodeDebugExecutor.ResolveInput (connection/subscription/workflow input; no producer execution); no existing plugin contract was changed.
- Native shapes beyond Rectangle/Circle/Annulus/Polygon, normalized coordinates and transforms need a reviewed shared schema extension; don't silently map native sector/caliper values to annuli.
- Main has not integrated tool-plugins commits after the current baseline. Review selected commits separately and run end-to-end acceptance before using their claimed capabilities.
## FCR-008 — tool-declared ROI capabilities and dedicated parameter editor

Implemented additively in main for the source-audited ImageBinary integration. This does not assert adoption by the plugin line; no other worktree or ImageSource implementation was changed.

- `ParameterEditorDefinition.Roi`: exact allowlisted kinds, count, polygon point bound, full-image/exclusion capability. Group validates all relation targets before a single edit; related ROI declarations must match.
- `IWpfToolParameterEditorFactory`: dedicated per-parameter WPF window. Immutable node/parameter/image snapshot plus the existing atomic multi-target Apply callback. Full-tool editor remains separate.
- `ToolEditorContext.PreviewImage`: exact invocation input lookup; `ApplyConfiguration`: single validated parameter-and-ROI edit. Existing session cancellation/disposal is retained.
- New `VisionUi` assembly owns shared source-guided ROI presentation; product assemblies contain no screenshot helpers.
- New `VisionPlugins.ImageBinary` is a main integration addition explicitly required by the current request for a real ROI consumer, not a merge of plugin-line work. It implements only independent MONO8 hard threshold. Its full tool window and remaining ROI editors are not certified native UI replicas. Do not extend other plugin implementations concurrently; coordinate future adoption through this request.

Evidence: `docs/roi-source-inventory.json`, original IMVSBinaryModuAlgorithmTab.xml / Algorithm.xml, native SDK probe in tests. Runtime observations and unresolved raster differences are in `docs/ROI-REAL-TOOL.md`.

### FCR-008 integration follow-up

Shared VisionUi now owns source-guided ROI templates, native button resources and coordinate conversions; ImageBinary uses the dedicated parameter factory and scoped debug host. ToolEditorContext adds optional input label/edit callbacks and atomic configuration writes. Normalized storage retains coordinate space through pixel editing; Fixture requires an explicit transform. Original ImageSource and the plugin worktree remain unchanged. This is main integration only, not an assertion of plugin-line adoption or ABI equivalence.

## FCR-009 — ROI显示样式、继承区域与Fixture数据（框架线集成）

- NodeDocument.DisplayStyles为独立显示配置；Core负责校验、快照、撤销和持久化。算法参数不承担颜色等显示状态。
- ToolEditorContext增加可选ApplyDisplayStyles、PreviewInput(port)、ClearInput(port)，旧工厂构造参数保持不变。按明确端口读取实际缓存数据，并由作用域Guard防止写入过期节点。
- DataKind尾部追加Region/Fixture/Point，PortValue工厂验证并快照数据。它们是独立托管协议，不是原SDK二进制ABI。数组累积仍只支持原有数值/字符串，不能把新复合类型计为累积已完成。
- 继承区和Fixture接入main中的真实ImageBinary，不改插件工作树。原InputBlindPolygonString编码和Fixture原生舍入未恢复，详见ROI-DISPLAY-FIXTURE.md；本记录不表示插件线已采用这些接口。

- FCR-009补充：可选ApplyConfigurationChange(values,rois,clearInputs)将参数、ROI和失效订阅合并为一次原子编辑/撤销。关联选择器补充同类型局部变量输入；原ClearInput回调保留兼容。

## FCR-010 — Group引擎步长（框架线，兼容新增）

- GroupLoop增加init属性Step，保留原四个位置构造参数；workflow.subprogram描述符增加可选LoopStep整数，默认1兼容原方案。原生SetParam非零验证和DoWork步长使用已确认，默认值跨版本未确认。
- 参数和保存格式使用现有Parameters字典，不修改插件实现及专用窗口。原Group XML没有LoopStep控件，禁止据此新增仿原生步长UI。零值拒绝；int32溢出/10000轮限制为独立保护。
- 依据及复验见GROUP-LOOP-BOUNDARIES.md。此项不要求插件工作树同步改动，也不宣称原生ABI或全部负步长行为等价。

## FCR-011 — Group动态索引及复合数组（框架线兼容新增）

- GroupOperand新增可选IndexSource，来源只允许父级前置标量整数；常量Index兼容。运行准备错误不持久化。身份重映射、依赖、清理和Group接口传播一起处理。
- PortTypes增加Circle/Point/Fixture数组，PortValue新增对应快照工厂、元素枚举和嵌套拼接；原有数值数组/标量行为保持。消费者应按Cardinality验证，不把数组当标量读取；没有修改插件工作树。
- 这是独立托管协议；原生组合IO按叶字段mode0追加的依据和pointset替换限制见GROUP-INDEX-COMPOUND.md。全复合类型/ABI/叶字段树未完成，不能据此宣称插件线已采用或全部原生等价。

## FCR-012：框架复合叶端口（2026-10-17）
调用方：Group/订阅选择器及执行器。框架统一提供 CompoundOutputFields.Describe/Project/Expand；节点 GetDescriptor 暴露可选叶端口，执行器发布相同完整路径，插件不需要重复投影。根输出保留。Components和Region不新建根数组ABI；已有数值叶数组可参与Group累积。字段和限制见 COMPOUND-LEAF-MAPPING.md；缺少的原生指标不伪造。插件线如需扩充字段，先提出数据和原生XML依据，不在另一工作树并发修改契约。

## FCR-013：跨流程节点实例身份（2026-10-17，双线协调）

- 调用方：框架调试缓存、Group独立运行、插件执行状态与图像列表。当前独立方案格式仅要求节点ID在所属流程内唯一；两个流程可以含相同NodeId。原生VM是否允许相同内部ID尚无运行取证，不能把这一托管约束称为原生规则。
- 框架线用`WorkflowNodeIdentity(SolutionId, ProcedureId, NodeId)`隔离调试结果、失效传播和Group预备缓存。`ModuleId`表示工具类型，不充当节点实例ID。
- 插件线应将方案生命周期、流程和节点身份传给实际执行状态及事件。来自节点的运行/刷新事件必须带流程身份；旧NodeId-only事件仅保留给无流程上下文的兼容调用，不用于多流程生产派发。
- 验收：在同一方案的两个流程放入相同NodeId、不同图像列表和运行状态，分别调试、清除和保存；一方的结果、参数编辑及事件不得改变另一方。方案关闭后旧事件和状态不得进入新方案。框架线和插件线各自检查，集成后再做端到端验收。

## FCR-014：流程等待时间字段（2026-10-17，框架线登记）

- 调用方：工作流菜单、独立方案存储、`SolutionExecutor`。`ProcedureDocument.WaitTimeMs`为0—100000毫秒整数，旧方案缺字段时按0读取。该字段不修改插件节点描述、模块参数或图像源状态。
- 原生依据：`VmProcedureDecorator.SetTimeOutCommandExe`打开`TimeOutSet`，XAML定义0—100000毫秒，确认调用`ProcessManager.SetProcedureWaitTime`。SDK后端超时语义尚未恢复；当前协作取消是独立托管行为。
- 双线协调：插件线正在自己的工作树修改`Contracts.cs`；两线提交后由main审查合并，不在插件工作树复制本字段或直接覆盖其在制改动。回归同时验证旧方案、新字段往返、慢模块释放及插件线的流程身份状态。

## FCR-015：主窗口全局配置入口与插件模块能力（MF-01 审计）

- 生产方：插件项目的全局变量、通信管理、全局触发模块及其配置会话；消费方：主框架 Shell 工具栏/菜单。框架不复制这三套模块的配置模型或窗口。
- 原始证据：`TitleMenueItemData.GlobalVariable` 动态装载 `GlobalVariableModuleCs.dll`，取 `VmModuleExtern.GetGlobalVarModule()`，将模块作为 `GlobalVariableDialog.DecorateSource` 并 `ShowDialog`；`CommSet` 装载 `CommManagerModuleCs.dll`、模块 ID 11000，创建/重用 `GlobalCommunication` 会话并读取数据；`LoadGlobalTrigger` 装载 `GlobalTriggerModuleCs.dll`、模块 ID 14200，以 `GlobalTriggerConfig` 绑定 `GloablTriggleConfigUI`。三者先检查 SDK/用户权限和模块配置可用性，不能按静态空窗恢复。
- 所需能力：插件侧声明可用全局配置模块种类、当前方案内的实例/权限状态、可创建配置会话的工厂，以及会话的关闭/失效通知。Shell 仅调用工厂并维护窗口所有权；配置数据和执行时生命周期由插件模块负责。
- 建议负载：`SolutionId`、全局模块种类、模块实例身份、权限/配置可用状态、配置会话工厂与失效 token。宿主在方案更换时释放旧会话；插件仍需在自己的工作树定稿具体接口。
- 兼容性：能力缺席时入口保持不可用，不伪造变量表、通信端口或触发器状态。原生 SDK 用户权限、模块动态装载与方案持久化的精确语义待插件线证据和实机验证。
- 双线验收：用真实插件模块打开相应 WPF 配置窗，保存值后运行图并重开方案确认持久化；切换方案/关闭窗口不保留旧模块会话；权限不足时命令不能改数据。Shell 与插件线集成后再宣布入口闭合。
## FCR-017：原生 Group PointF 三分量输出合同（请求，未实现）

- 生产者：插件线的 PointF 输出工具及工具描述符；框架线的 Group 输出配置器和累积器消费该声明。当前 `DataKind.Point` / `VisionCore.Point2` 仅含 X、Y，不能暗中把原生 R 丢弃或填零。
- 消费者：Group 输出页的 PointF 三子行、公共输出端口、跨流程订阅、累积数组、调试结果和持久化层。需要明确第三分量是角度还是其他量及其单位、下游端口身份。
- Payload：拟定独立强类型 PointF(X,Y,R) 或显式三叶字段协议，包含完整类型/基数、字段名、数值范围及端口映射。具体 API 和 JSON 形状须两线审查后决定；不得未经同意替换现有 `Point2`。
- Lifecycle：工具产生完整三分量快照 → Group 配置选择点输出及三个子字段 → 每轮读取/累积 → 调试显示、跨流程消费 → 保存重开/复制重映射/清理。失效或缺字段时应显式报错，不默默退化为二分量。
- 兼容：旧 `Point2` 方案仍按 X/Y 独立类型读取，新增合同应为可选/版本化，不让插件线在制合同产生二次实现；原生 `.sol`/ABI 往返另行取证。
- 验收证据：`analysis/decompiled/Apps.XmlUI/styles/paramusercontroldatatemplate.xaml:1078–1177` 的 `PointFParam` 三子行和独立“增加点输出”；`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/PointF.cs` 的 `SubChildren` 将其命名为 `.X/.Y/.R`，R 初始范围 -360..360。需选真实 PointF 工具逐项 WPF 操作，验证三叶数据下游、累积、撤销、保存重开，并与原生 VM 同状态对照；当前只完成独立 Point2 入口及两分量数据运行，不能作为本合同验收。

## FCR-018：Group 运行参数选择后的工具装饰器（请求，未实现）

- 生产者：插件项目的工具配置框架及各工具 `IGroupRunParam` 对应的专属/通用装饰器工厂；框架 Group 类别树产生当前选中参数及其节点身份。
- 消费者：Group 双栏配置窗口右侧的参数配置区；运行参数映射、同步与执行仍由现有框架数据链负责。
- Payload：至少包含所属流程/Group/节点实例身份、参数名、对应描述符、只读当前参数和订阅快照、可空专属装饰器能力，以及原子 Apply/Cancel 回调。具体接口需双线审查，不能把原生 `IDecorate` 假定为现有 WPF 接口。
- Lifecycle：选中运行参数 → 框架定位第一个关联节点 → 插件工厂提供 `GetSupportDisplayForGroupData` 等价视图或通用视图 → 右侧更新 → 编辑经既有配置事务提交 → 取消/切换/关闭释放订阅和会话。选中类别时恢复 Group 本体装饰器；方案重开时不保存窗口焦点状态。
- 兼容：工厂应可选；未接入的工具继续使用现有运行参数代理编辑，不复制插件配置模板到主线。旧方案的参数和订阅格式不因窗口接线改变。
- ROISelecter 源码证据补充：`Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/ROISelecter.cs` 的 `DisplayForGroupType=RoiConfigControl` 与 `IGroupRunParamWithMultiParams`；`DynamicObjectConfigControl.XmlData.Extern/DynamicExtern.cs::GetSupportDisplayForGroupData` 产生 `ROISelecterForGroupDecorate`；`DynamicObjectConfigControl/datatemplate/paramusercontroldatatemplate.xaml` 将 `ROISelecter_GroupControl` 映射到 `ModuleROIConfigControl`；装饰器把原始 `ROIList` 直连并将 ROI 操作转给 `IROISelecter`；`ModuleROIConfigControl.xaml` 绑定模块图像、原始 ROI 列表及 `RenderControlBase` 命令。单参数模板 `RoiConfigControl` 与 Group 装饰模板 `ROISelecter_GroupControl` 是两条路由，不能合并猜测。源码详情和未确认点见 `GROUP-NATIVE-EDITORS.md` 的“源码级调用与数据链复核”。
- ROI复制链的源码结论：`VMBaseControls/ImageViewBox.xaml.cs::CanCopyRoi` 调用 `RoiCopyCommand.CanExecute` 作模式门控，`CopyRoi` 自行克隆并写入私有粘贴对象；代理的 `RoiCopyCommandExecute` 空方法体不表示复制功能不存在，也不能在代理层再实现一次复制。
- 待插件线提供可消费工厂时，验收需用真实 ROISelecter 工具验证：类别/普通/专属参数切换，精确关联模块和 IMAGE 来源，ROI 选择/添加/更新/删除/粘贴及复制能力门控，多字段参数和序号写回，SelfRun 前后数据、取消/撤销、关闭释放。完整调用链和本轮验证结果见 `GROUP-NATIVE-EDITORS.md`。
- 当前主线只有显式“工具配置”按钮，不宣称右侧动态装饰器等价。未完成的插件侧能力应由插件项目在其边界内实现；主线只消费已审查并集成的合同，不复制插件模板。

## FCR-019：参数目标的原生可见/可订阅能力（请求，未实现）

计划B01第二批：原RunParamsSelectData缓存命中/分型/确认成功路径在合成缓存及经CIL核验的ID-only模块壳下通过20谓词。切换类型复用节点、重设ParentTree，不重算首次占用/资格；成功先Clear旧选择列表再替换，旧别名变空。调用方弹窗前已重建旧绑定快照，取消不能表述为完全无副作用。首次真实模块填充、拒绝消息窗和SDK绑定未运行，详见 [选择器第二批](reverse/B01-selection-batch2.md)。合同继续消费真实元数据快照，不把合成缓存当实际模块树。

计划B01首批运行增量：未修改原厂parser在合成owner/最小XML下通过13项检查，确认LinkItem在原集合位置生成独立克隆，CustomVisible=false/SubscribeVisible=false项仍可进入GetRunParamsTree根候选，空DisplayName被排除且不按名称去重。安装ImageSource副本仅注册common节点、业务回调关闭，23/25项和空RunTree不能外推真实VM。原件hash不变，详见 [B01运行证据](reverse/B01-tab-batch1.md)。这没有关闭RunParamsSelectData后续过滤、真实Params.Data或插件合同。

第二轮装配证据：已找到VM.Xml的BaseNode反编译及原始程序集身份；XML名称经public字段反射、TypeTable和集合钩子装配，不可直接把原始Items当最终候选。GroupLinkItem/LinkItem可替换展开集合；合同应消费装配后的稳定候选快照，仍不自动将Tab_Basic Params改成Tab_Run Params。详见 [运行Tab第二轮](VARIABLE-TAB-ROUND2-20261019.md)。

2026-10-19四方向复核补充（以下新结论优先于本节旧生命周期假设）：原始选择窗首次全部类型创建模块缓存，切换类型只投影缓存；插件会话元数据刷新和新开窗口快照须分开，活窗口即时刷新尚未验证。Payload还需原始DisplayType、ComType、动态/Between展开身份，ISubInfo和SubscribeVisible分别表示。原树消失的旧选择无独立补回路径，“保留并诊断”只能是独立兼容策略。参数目标与ModuleInputSelectData的输入目标是两种来源，不能合并为CanBind。详见 [四方向复核](PARALLEL-RECONSTRUCTION-20261019.md)。

2026-10-19修订：撤回仅凭CustomVisible推断局部变量目标隐藏的验收要求，保留本节标题及引用锚点。

- 生产方：插件项目的公共参数描述符与各工具动态参数配置；消费者：主框架工作流局部变量“目标输出”树及 Group 运行参数代理树。
- 证据：随包 `Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModuleAlgorithmTab.xml` 将StitchStartHeight/StitchHeight标为CustomVisible=False，StitchEnable=True触发VisibleOperation；该XML的CurValue/DefaultValue为True，只有Tab_Basic Params。不能由本地False状态推定原机默认值或有效订阅候选。
- 目标链：GlobalVariableItem.ModifyRunParamsExecute经RunParamsSelectData.GetSubRunParamsTree调用AlgorithmTabRoot.GetRunParamsTree，后者从Tab_Run Params.Collection枚举，AddSubChildToTree检查DisplayNameA；RunParamsBaseNode.IsSupportSub读取ISubInfo/SubscribeVisibleA，没有直接按CustomVisibleA过滤。Group的RunParamPickDataExtern则明确按CustomVisibleA等条件过滤。旧合同按关闭拼接即必须从局部变量目标树隐藏的验收推断撤回；运行时Tab装配和实际目标集合仍未知。
- 负载：分别表达Tab归属、CustomVisible、SubscribeVisible/CanSubscribe、ISubInfo支持及触发关系；局部变量和Group候选分别按源谓词计算，不合并为一种隐藏布尔值。优先表达现有节点参数依赖，不传递UI控件或未审计委托。
- 生命周期：节点参数修改后重新求值目标集合；已保存但暂不可见的订阅仍保留/执行策略需按源码另行确认，目标不可用时应显示状态而不是静默删除；切换节点、关闭配置器不得留过期参数会话。
- 兼容：字段可选；旧插件未声明时保持现有静态参数行为并标为未验证。该合同由插件线在自己的边界内实现并提交；主线只消费经审查的字段，不修改插件工作树。
- 验收：分别采集StitchEnable两态下运行时Tab树、局部变量目标、Group候选、可订阅属性、存量绑定及执行/保存重开；依据真实树与方法谓词确定集合，不预设隐藏结果。无元数据插件维持兼容；完整修订依据见MAIN-PENDING-REVIEW.md。

## FCR-020：Group运行类别与代理数量配置源（框架线实现，独立XML读取）

- 生产者：框架应用发布配置；消费者：`VisionFramework.WorkflowRunParameterLimit`、Group类别添加/代理选择器、`WorkflowRunParameters.Validate`。
- Payload：应用级 `MaxRunParamsNum` 整数。当前重建读取应用目录下 `<FriendlyName>.config` 的 `appSettings/add[@key='MaxRunParamsNum']`；键缺失、配置不可读或不可解析时默认32。
- Lifecycle：应用进程首次访问时惰性读取一次；类别和运行参数添加回调用该上限拒绝超限，配置校验阻止持久化/载入超过上限的方案。修改配置需重启应用。
- 兼容：旧配置缺少该键时沿用32；方案格式不变。`ConfigurationHelper.GetConfig` 的真实路径/优先级/重载语义尚未证实，因此 XML 读取仅是有应用配置证据支撑的独立重建，不能标成对其实现源码的逐指令复刻。
- 验收：原始 `ParamConfigDataBase.cs:85–102,489–496` 显示默认32并以 `ConfigurationHelper.GetConfig("MaxRunParamsNum")` 覆盖；安装配置 `VisionMaster.exe.config` 当前值32。专项 WPF 测试以应用配置覆写为2，实际点击添加组和代理后分别验证第三项被拒绝、保存重开和 Group 执行数据仍正确；同时验证非法配置回退32。

## FCR-021：Group运行参数候选资格元数据（插件框架请求，未实现）

- 生产者：插件侧工具描述符/参数模型与配置能力工厂。消费者：main 的 `GroupRunConfigurationWindow` 候选树；Group 自有代理、类别、关系持久化和执行消费继续由 main 维护。
- Payload：节点与端口/参数身份、输入或参数种类、原生参数类型身份、`IGroupRunParam` 能力、`CustomVisibleA`、`GroupRunParamEnable`、`AccessMode`、`DisplayForGroupType`、本地化显示名，以及是否存在原生编辑器/Group 展示工厂。应允许能力缺失并明确标为未知，不可默认所有 descriptor 参数都合格。
- Lifecycle：插件描述符物化 → 依照 VM 等价谓词和类别过滤/搜索生成候选 → 用户勾选并创建 Group 代理 → 保存关系 → 真实 Group 执行并把值传给对应工具。能力/可见性变化时，候选刷新且既存代理保持显式可诊断，不静默删除。
- 兼容：元数据应可选/版本化；缺字段节点不得被主线伪装成已证实可运行参数。筛选需保留原生类型名和搜索范围，不能用托管 `DataKind` 推断插件原生参数类别。该请求不允许 main 复制插件配置模型或控件模板。
- 原始证据：`analysis/decompiled/DynamicObjectConfigControl/DynamicObjectConfigControl.UI.Data/RunParamPickDataExtern.cs::GetInsideRunParamTree` 与 `ExpendHolder` 的候选谓词要求 `IGroupRunParam`、`CustomVisibleA`、`GroupRunParamEnable`、`filterTypes.Contains(p.GetType().Name)`、`MultiLanguageDisplayNameA.Contains(searchWord)`，并只接受 `AccessMode.ToLower()=="rw"` 或非空 `DisplayForGroupType`；`RunParamPickData.cs::Init` 及 `GroupRunParamConfigData::AsyncLoadParams` 构建该列表。`DynamicRunParamConfigControl.Common/ParamTypeList.cs` 提供原生八类过滤。当前 `GroupRunConfigurationWindow.BuildTree` 遍历每个 descriptor 的全部 `Parameters` 和 `Inputs`，没有以上元数据，差异已由源码确认；具体真实插件候选结果尚未与 VM 同状态动态对照。
- 验收：用至少一个原始源码已审计的真实插件，逐项验证隐藏、禁用、只读且无展示能力、按原生类型排除、搜索不匹配的参数不出现在列表；合格参数可被选择、写入关联、运行得到实际值；配置保存重开后保持；再与 VM 同状态候选树对照。没有真实插件证据时不得以测试模块关闭该项。

## FCR-022：Group 配置对话框期间撤销记录抑制（main文档seam已实施，SDK回放开放）

2026-10-19 第十批实施更新（优先于下面历史实现状态）：main现已交付`GraphEditorDocument.ExecuteConfigurationEdit`及真实`SubprogramCallHost`消费。modal窗口多次即时写回合并一条惰性文档历史，modeless每动作单独root；布尔嵌套非计数器，finally复位为独立加固。29项main实际检查和产品构建通过，按用户实施自审核口径计C01.2完成。此为main快照适配，不是原SDK root/part载荷等价；C01.3、跨项目插件seam采用及原厂差分仍开放，不自动合流。详见[第十批实施](reverse/C-history-suppression-batch10.md)。

计划C01首批运行增量：未修改原厂UnRedoService在自有BaseDirectory/TempFiles与合成动作下通过13项断言，实测空栈抑制无后效、当前root布尔标志、旧根禁止不阻新根；Update前先置完成，Update异常被吞后根留undo，第二次Undo跳过重采仍执行。详见 [C01运行证据](reverse/C01-undo-batch1.md)。该运行不是实际Group/SDK参数，仍不能把main整方案快照替换为简单抑制开关；真实Group注册链保持开放。

第二轮数据流证据：ModifyConfig根内按模块去重，首次合法注册时求旧参数，Undo前才采新参数；普通模块内存载荷、Group/流程临时文件、方案路径引用须分别建模。Group配置通过UserData到part Register的静态可达性已闭合，抑制记录不阻止SDK写入。RecoverCallBack定位于密码取消，不能泛化为失败事务回滚。详见 [撤销第二轮](UNDO-ROUND2-20261019.md)；本轮未据此改动main快照模型。

2026-10-19证据状态更正（优先于下文历史记录）：已取得安装目录myLibs/VM.UnRedo.dll并核对CIL。SetForbidRecord对空undo栈无效，否则设置栈顶IRootParam.ForbidBuild；Register仅在part分支检查此标志，IsNeedExport也受其影响。它不是全局或计数开关，scope/finally/窗口隔离是独立安全设计，不能以跳过本地快照压栈表示原生等价。服务构造会清理TempFiles，本轮未实例化。历史“DLL缺失”已被本轮静态证据取代，Group运行态/异常差分仍开放。详见 [四方向复核](PARALLEL-RECONSTRUCTION-20261019.md)。

- 生产者：Group 配置器向框架 undo 服务请求一个可作用域释放的“禁止记录”事务；消费者：Group 配置编辑入口和通用 `IUnRedoService` 等价层（该接口由框架 seam 所有者实现，不能由本 Group 线复制另一套 undo 管理器）。
- Payload：进入对话框前捕获 undo 状态并打开期间抑制逐字段历史记录；原生没有取消/回滚路径，窗口“确定”和普通标题栏关闭都保留已即时写入的配置。异常时 scope 必须 `finally` 解除 forbid 状态并幂等释放；这是重建安全性要求，原生方法未显示此保证。
- Lifecycle：`ConfigCommandExecute` 准备配置数据 → `SetForbidRecord(true)` → `DialogHandler.Invoke` → `SetForbidRecord(false)` → 重载 `Collection`/`GroupCollection`、`GetParamUnit` 并恢复展开状态。`DynamicRunParamConfigWin.OnClosing` 对普通关闭强制设 `DialogResult=true`，加载进度期间 `_isCloseDialog=false` 才暂时阻止关闭。当前 WPF 每次 `GroupRunConfigurationWindow.Change` 调用保存委托，关闭不回滚；`SetWorkflowConfiguration` 对每个非空差异调用一次 `Edit`，逐次把整个方案快照压入 undo 栈，因此本地历史边界是每个写回事件，而非整个配置窗口。此静态差异需由统一 seam 收敛。
- 兼容：既有普通画布操作撤销行为不变；scope 仅覆盖此次 Group 配置窗口，不能跨并发编辑器或残留到异常后续操作。方案数据格式不变。
- 原始证据：`analysis/decompiled/DynamicObjectConfigControl/Apps.XmlParser.ParamTab/GroupRunParam.cs::ConfigCommandExecute` 在 `method.Invoke` 前后调用 `IUnRedoService.SetForbidRecord(true/false)`，忽略返回值，之后重新取集合、调用 `GetParamUnit` 并复原类别展开标志；`analysis/decompiled/VM.Core/VM.Core/VmModuleSet.cs::CreateLinkModule` 在 `CreatModuleByInfo` 前后也直接调用 `SetForbidRecord(true/false)`，同样没有 `try/finally`。`analysis/decompiled/VisionMaster.Exe.Audit/app.config` 把 `UnRedoServiceImplement` 配置为 `myLibs\VM.UnRedo.dll`；`VisionMaster/App.cs` 从应用基目录拼接该配置值，`Assembly.LoadFrom`，选择实现 `IUnRedoService` 的类型并注入 IOC。但该 DLL、接口声明及实现源码均不在当前 `analysis/decompiled` 树中，所以无法证明该服务内部是布尔开关、计数嵌套、线程/窗口作用域、栈清理或异常恢复；两个调用点只能证明显式 true/false 成对的源码形态，不能证明服务效果。
- 本地对照：`src/VisionEditor/SubprogramCallHost.cs::CreateBoundGroupWindow` 的 Group 配置保存回调逐次调用 `doc.SetWorkflowConfiguration(Body().Id, configuration)`；`GroupRunConfigurationWindow.Change` 每次控件事件立即调用保存委托。`src/VisionEditor.Core/GraphEditorDocument.cs::SetWorkflowConfiguration` 做相等检查后进入 `Edit`；`Edit` 每次先 `undo.Push(Capture())`、清空 redo、替换完整文档并递增 Revision；`Undo/Restore` 恢复整个方案快照。故每个不同的配置写回单独生成撤销项。
- 相关窗口证据：`DynamicRunParamConfigControl.xaml.cs::EditRunParam` 返回 `DynamicRunParamConfigWin.ShowDialog()`；`DynamicRunParamConfigWin.xaml` 设置 `CancelVisible="Collapsed"`；`DynamicRunParamConfigWin.OnClosing` 在非加载态强制设 `DialogResult=true`，加载中则 `e.Cancel=true`；共享 `ConfirmBaseWindow.CancelButton_Click` 设 false，但相应按钮不可见。`ParamConfigDataBase.AddGroupCommandExecute/DeleteAllParams` 直接更新 `Owner.UserData`/父集合，`RunParamConfigDecorate.Name` 调SDK修改参数名，说明配置不是确认时才整体提交。`ConfigCommandExecute` 没有检查 `method.Invoke` 返回值，也未用 `try/finally`，因此异常恢复保障未知。
- 验收：隔离输出 `dotnet run --project tests/VisionEditor.GroupCategoryExpansionChecks/VisionEditor.GroupCategoryExpansionChecks.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false` 退出码0，`PASS 30 Group category expansion checks`。真实 WPF 控件连续执行分类名编辑、候选复选框添加代理两次写回，再调用文档 Undo 两次/Redo 两次；每一步参数和分类状态均按单次写回边界变化，六项新增断言通过。截图 `analysis/full-audit/group-run-config-f15-confirm-close.png` 已目检；此为离屏真实 WPF 客户区，不是用户桌面或 VM 对照。此前还实际验证“确定”和关闭窗口保留即时变更、重开及 Group 输出有效。VM 服务行为仍需取得 `myLibs/VM.UnRedo.dll` 与接口/运行时证据后做同状态核验；取消/回滚不适用原生窗口，异常恢复继续标未知。


## FCR-023：Group原生IO类型与本地托管类型的能力边界（请求，未实现原生合同）

- Producer：框架数据合同与插件原始结果生产者；consumer：Group输入/输出类型菜单、来源选择器、序列化及执行器。
- 原始证据：GroupParam.InputOutputTypes经LeafIOTypeEnum/CombinationIOTypeEnum生成候选，GroupParam.AddCommandCmdExecute分别调用AddLeafIO/AddCombinationIO并统计SDK叶IO数量。原始pointset与POINT组合类型分立；DynamicOutput.ConvertTypeName对POLYGON/ROIPOLYGON另作转换。现有DataKind.Point数组及DataKind.Region没有这些原生ABI和生产链证明。
- Payload：需提供经源码确认的原生类型名、托管形状、基数、叶字段、支持能力和序列化/执行规则；未知类型必须不可创建，不能按名称猜测映射。尚未新增通用插件合同字段。当前pointset/POLYGON菜单项禁用；原有Point数组、Region和Components仍按独立托管合同使用。
- Lifecycle：插件注册能力→Group枚举→创建/绑定→保存重开→真实执行→输出/清理。候选列表1024项计数是本地接口数策略，不宣称等同原生SDK复合叶IO计数。
- 兼容：现有vmx.json格式、DataKind及基数不变；仅改变UI类型标签和可创建能力，不迁移或删除既有独立托管端口。WorkflowSourceOption.NodeName是可空UI元数据，由主框架提供，订阅树消费；未填时回退到Label，不写入方案。
- 验收：须用实际pointset/POLYGON原生生产者验证结构、字段、基数、数组/索引/复制、Group输入输出、存储及实际执行，取得原机/SDK差分后再开放。当前20项本地审查只确认未知类型拒绝创建、真实BMP标量导出、删除/撤销和保存重开，不关闭原生合同。


## FCR-024：插件资产在另存与历史恢复间的纯重定位（请求，未实现）

- Producer：插件资产持久化/公共插件框架能力；consumer：main GraphEditorDocument SaveAs、Snapshot/Restore。用户决定跨项目交接，本请求不自动调度插件项目。
- Payload：旧/新基目录、方案/流程/节点身份、不可变资源引用及经插件解释的私有资产路径；需要不执行工具、不采当前运行状态、不读取设备、不产生保存副作用的重定位或稳定身份能力。具体接口签名尚未决定，不在 Core 硬编码 ImagePaths。
- Lifecycle：捕获准备前历史 → 当前文档 PrepareSave/跨目录另存 → Undo/Redo 恢复旧快照到当前基目录 → 资产解析。重定位不能把当前 scope 状态写入旧快照，不能以反复调用带 I/O 的 PrepareSave 代替纯合同。
- Compatibility：能力应可选并明确诊断旧插件不支持的情形；不静默丢弃历史/资源，不猜改私有 JSON 或原生方案。未改共享 Contracts，也未改 tool-plugins。
- Evidence：main 6a2cf54 的实际 ImageSourcePlugin/ToolPluginRegistry/GraphEditorDocument 链见 [D02 诊断](reverse/D02-asset-history-batch3.md) 与[独立审查](reverse/D02-asset-history-review-batch3.md)。真实失败 exit1/passed=false：原外部图像仍在时，跨目录 SaveAs 后 Undo 恢复旧相对路径且资源0，新基目录解析失败；Redo、离线嵌入重开通过。这是独立实现集成缺口，尚非原生 VM 行为规格。
- Acceptance：保留当前失败样本；合同落地后对准备前/后、多层历史、跨目录另存、Undo/Redo、源文件存在及删除后重开分别核对身份和编码字节，并用实际模块执行核对图像结果。须验证校准等带运行状态准备器不会污染旧快照；原生方案/VM 差分独立验收，不能据本地测试关闭。

### FCR-019 第四批装配证据补记（不改变共享接口）

[B01-lazy-root-batch4](reverse/B01-lazy-root-batch4.md)补齐ModuleParam.Data缓存发布点、Owner/共享Params、InitLoad与XmlLoaded/Setter通知链。真实Owner为ISubjectModel时，InitLoad在AlgorithmTabRoot的loadXml try之外，proxy会加载/激活工具程序集；loadXml会改XML文件属性。16项原方法检查仅使用正常ModuleParam、Parent=null与任务自有XML，不是实际VM候选装配/绑定。实际定义Hik.Xml.BaseNode的程序集已通过REA身份确认是VM.Xml.dll，不能按反编译目录假定归属。仍不据此新增main插件配置模型；真实SDK订阅写入、全局变量确认/取消和生命周期仍待原源码/运行补证。既有FCR-024失败及交接边界不变。

## FCR-025：main容器缩放持久化字段（本地实施，未跨项目合流）

- Producer：main画布缩放/适应入口；consumer：GraphEditorDocument、SolutionStore及画布恢复。唯一提交所有者main根线，仅在本工作区修改Contracts中的ProcedureDocument；不涉及插件配置模型，不操作tool-plugins。
- Payload：可选JSON属性CanvasZoomIndex，托管int默认9；本地UI与加载校验限定0..19，映射(index+1)/10。原setter不校验任意int，本地范围检查是独立格式边界。
- Lifecycle：缩放写当前容器→Dirty/Changed，不新建结构历史root、不清redo、不增结构Revision；现有整文档snapshot保存并回放，包括非活动容器；Save/Load保留。overview/pan保持会话态。
- Compatibility：既有独立JSON缺属性默认9；原native UserData/SDK载荷不是此JSON字段。旧版main加载器采用UnmappedMemberHandling.Disallow，不能承诺旧可执行程序打开新增字段文件；本次新版本可读取既有无字段文件，未改独立格式版本号。插件合流由用户决定，本条不是插件线派工。
- Evidence/acceptance：VmModuleSetDecorator.ProceZoomIndex/UserData、ZoomArea._ZoomTicks及原UnRedo part链，详见reverse/A03-closure-source-review-batch12.md；本批真实Core/WPF自审核覆盖根/Group、保存重开、复制及非活动Undo/Redo。原绑定alias反馈与原生格式验收继续未知。

## FCR-026：main托管发现加载接缝（部分实施，入口协调锁保留）

- Producer：main通用发现/加载器与用户明确选定的既有IVisionToolPlugin实现；consumer：既有ToolPluginRegistry.Register及ModuleRegistry。唯一实现所有者main A线，本批不修改公开插件Contracts或注册/配置锁文件。
- Payload：ManagedToolPluginReference的绝对AssemblyPath、TypeFullName、SHA256、ModuleId、Version；LoadedToolPlugin返回当前真实IVisionToolPlugin及程序集身份。它是方法参数，不新增参数配置模型/外部manifest或人工审批流程。
- Lifecycle：选定引用→静态PE/type/合同引用/hash核对→Default ALC同名/实际MVID/hash复核→无参构造与Manifest/Descriptor身份→私有registry校验；由未来单一注册者明确写真实registry。无自动目录扫描、依赖resolver hook或失败自动重试。类型兼容不等于执行隔离；本批只运行自有fixture，原SDK/DLL未执行。
- Compatibility：复用现IVisionToolPlugin/IToolNodeDescriptorProvider，不改现公开合同。main启动/目录到注册入口仍按PLUGIN-FRAMEWORK-BOUNDARY协调锁保留，用户选择合流后逐接口审，不能把此helper计A07.1完整关闭。
- Evidence/acceptance：原ToolBoxItemData.AddNewItem程序集/类型构造链见reverse/A07-managed-loader-partial-batch16.md；自有fixture验证真实registry、节点执行、方案往返和错误边界；完整宿主发现入口尚未接入。

### FCR-022 第十六批main普通root/part实施

main GraphEditorDocument已改为真实DocumentHistoryEngine的root/part旧新载荷、首次Undo补new、predicate/forbid/replay/lazy准入和排序，不再走交换Snapshot栈。既有ExecuteConfigurationEdit布尔范围API不变；默认保留15根，异常保留LastHistoryError而不造事务回滚。main payload仍是自身不可变文档与BaseDirectory，CanvasZoomIndex在回放间的合并为明确独立视图适配。公开SDK Group/模块采集、TempFiles二进制载荷和RecoverStack另项仍缺，不能由普通root/part自审核推成native payload已实现。插件项目未被改动或调度。

## FCR-027：main执行与配置回调归属代际

- Producer：main既有workflow/Group/debug/config宿主；consumer：同一main的结果、运行历史、配置回调与scope资源。root唯一改已有宿主，不修改公开插件Contracts/Session/注册框架，不操作tool-plugins。
- Payload：内部ExecutionOwnerStamp含host epoch、generation、solutionId、scope引用；不进入方案或插件参数，不创建第二配置框架。
- Lifecycle：Replace先Invalidate并清旧owner缓存/计数；await完成与modal返回核对；OnClosed Close永久拒绝旧发布。workflow/debug保留等待，配置采用失效+真实lease退出后释放；不承诺设备线程已停。旧configuration finally不得修改新计数。
- Compatibility：外部ToolEditorContext与插件API不变；旧失效读回调空、写/服务回调拒绝。GraphEditorDocument配置scope内部代际不持久化。测试专用AppContext switch仅禁自动相机发现，默认产品行为不变。
- Evidence/acceptance：原ModuleResultView过时请求/Decorator.DeInit/InnerDispose链、真实MainWindow同ID修前污染与修后隔离、配置close及真实lease释放、B独立复审，见reverse/BATCH17-20261019.md。原SDK和插件私有窗口另项未知；未来插件合流由用户选择，不表示向插件线派工或采用其改动。

### FCR-026 第十八批当前分类消费者落地

- Producer：main现有tool-catalog通用发现；consumer：既有ToolPluginRegistry/ModuleRegistry及palette/节点创建。root唯一改ToolNavigation，新session为A所有者；不改MainWindow启动、注册工厂或公开插件配置合同，不操作另一工作树。
- Payload：现有CatalogTool可选Implementation复用ManagedToolPluginReference绝对AssemblyPath/type/hash/id/version，非参数模板或新manifest。既有无字段目录照常读取。
- Lifecycle：首次分类才load→预查当前双registry同ID→既有Register；成功引用保留到宿主结束，不刷新卸载；失败sticky并记录阶段原异常，显式Refresh只清失败。现UI线程同步消费者，不承诺跨线程装载或旧public Register事务语义。
- Compatibility/evidence：当前公开合同适配不是原Cs/Librarys.Config/IVmModule兼容，后者D06和后续交付仍未知。实际host15项通过及源链见reverse/A-runtime-editor-batch18.md。A07.1当前消费者闭合，A07.2完整冲突矩阵仍部分；插件合流仍由用户选择。

## FCR-028：main结果pull消费者与历史opaque载荷生命周期

- Producer/consumer：main结果显示宿主→既有三个presenter；main GraphEditorDocument→DocumentHistoryEngine和HistoryPayloadStore。root唯一现ToolDisplays/ToolExecutionScope/WorkflowExecution及Core接入，B/C各自独占测试和新store；不复制插件配置/会话/算法框架。
- Payload：内部显示request代数+owner/procedure票据，不进入公开插件API；历史文件保存main自身文档JSON opaque bytes与独立lease，非原SDK二进制格式。GraphEditorDocument可选parent仅选择自有存储母目录，无字段持久化变更。
- Lifecycle：provider调用前清旧frame、同步重入过时返回丢弃，Replace/Close物理解除main的两个scope事件。历史迁栈不删、淘汰/清redo/换方案/Dispose释放末lease；借用路径不删、失败保载荷/错误，显式复收；仅删除自有GUID新文件与空目录，不递归清TempFiles。默认64KiB阈值为独立策略。
- Compatibility/acceptance：既有IVisionToolPlugin/IToolDisplayProvider及配置合同不变，已有小文档仍内存；原native回调/全显示工厂/SDK导出密码格式另项未知。结果20、store18、engine17、真实文档19与既有定向回归通过，见reverse/BATCH18-20261019.md。提交仅在main，不代表采用或通知插件交付。

## FCR-029：main现有插件注册的身份与失败发布边界

- Producer：main当前目录引用消费者；consumer：既有ToolPluginRegistry/ModuleRegistry与palette/方案校验。root唯一改本工作区Contracts.cs中的Register方法和ToolNavigation调用；不改插件参数模型、配置模板、执行会话或其他工作树。
- Payload：保留Register(plugin,modules)原二参ABI；新增Register(plugin,modules,expectedId,expectedVersion)供显式引用消费者使用，不新增插件类型接口或配置字段。
- Lifecycle：所有外部metadata读取前置同步重入guard并finally复位；一次capture Manifest/Descriptor→对照expected→既有ModuleRegistry冻结/发布→插件表发布。metadata/重复/Freeze失败不留自身半注册；成功后不再调用metadata getter。当前主机UI线程合同，不声称并发安全、OOM事务或不可信代码隔离。
- Compatibility/evidence：现有二参调用保留且实际旧loader17/catalog15回归通过；新47项host矩阵与5项真实metadata回调通过，源顺序风险未冒称baseline复现。详见reverse/BATCH19-20261019.md。插件项目合流由用户选择，通知不代表采用。

### FCR-024 第十九批新反例：历史文档与未来scope不一致

纯托管公共API已实际复验11项：旧点11、当前scope99，重新准备旧Node和真实Undo后Save均得到99。无scope重载在当前NPoint保11不等于通用IToolAssetPersistence纯函数保证。不得对历史反复调用PrepareSave或清空scope丢状态，亦不加origin公共字段绕过。解除条件扩为版本绑定资产身份/无状态历史序列化/纯重定位以及scope随历史一致恢复；旧图片路径失败与本轮校准值污染都须复验，详见reverse/C-asset-history-batch19.md。未修改工具代码或另一工作树，未向插件线派工，本项仍未实现。
