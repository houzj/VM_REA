## 2026-10-20 LOCATION-FIXTURE-COPY-015 — 运行态参数复制的可选插件合同

原证据：原 `VmModuleSet` 调 `ModuleManager.CopyModuleParam`，经 SDK/manager/proxy 到 `Frame VM_M_CopyModuleNodeParam 0x13a0f0→0x040d50`；后者从源模块 vtable+0x28 调 `SaveModuleData`，复制所得缓冲，再向目标 vtable+0x30 调 `LoadModuleData`。位置修正模块自身 `Save 0xab60→GetAll` 原八项和 `Load 0xaa70→SetAll`，因此正常运行后只克隆文档快照会遗漏 live 参数。静态证据为 B 线 `analysis/fixture/source-20261020` 匹配原件体；原 DLL 整体运行未执行，最终差分及真实Host仍待验证。

生产者：本工作树 `VisionPluginContracts.IToolScopedNodeCopyProvider` 是旧 `IToolNodeCopyProvider` 的可选扩展；`MainWindow.PrepareToolPasteSubscriptions` 只在来源过程当前仍有同 ID 和 ModuleId 的节点时传 `executionScope.ForProcedure(source.Id)`，否则传 null。消费者：位置修正专属 Fixture 插件的复制准备方法，将当前八个 live 参数按原 GetParam 六位格式投影到副本；尺度和其它缓存不通过此合同复制。载荷是原/新 `NodeDocument`、ID映射、复制后的执行后继与可空源 `ModuleExecutionScope`。生命周期是一次 Paste 准备的同步读取，不把源状态转移给目标 scope；实际提交仍由 GraphEditorDocument 事务完成。

兼容性：旧复制接口和 BranchModule/BranchString 等消费者调用不变，失效来源只走 fragment 快照的独立安全选择；组拥有者早退规则保持原有宿主边界。跨项目采用需用户决定，不复制第二套插件配置/复制框架，不自动合流。当前共享窄路由已落，Fixture 专属实现及主窗口复制回归待完成，不据静态证据预先计整工具。

## 2026-10-20 CAL-RADIO3-009 — 原三模式控件公共模板实现

原证据：Apps.XmlParser.ParamTab.RadioSelecterEx3.DisplayType=RadioSelecter3；已恢复的Apps.XmlUI/styles/paramusercontroldatatemplate.xaml:1373模板固定绑定EnumEntrysA[0..4]。ConfigurationTemplateCatalog原先已映射该key，StandardControls却无注册，Coordinate真实Host报UnsupportedTemplate。

生产者：本工作树ToolConfigurationStandardControls新增RadioSelecter3注册，复用Radios私有helper的maximum=5；旧RadioSelecter仍默认2、接口不变。消费者：Coordinate InputWay原声明0POINT/1XY/2Matrix三合法选项。运行/参数载荷仍通过既有State.CommitUser→descriptor adapter→宿主Apply，不新增持久字段或模式值。模板注册生命周期与配置session相同，关闭由现宿主释放，无新跨项目后台线程。

独立呈现边界：只创建原资源明确声明的合法项；原固定五槽中两个缺项的WPF UnsetValue/空Content和无Value点击副作用不仿造，也不声称逐像素一致。三个合法模式需实际radio点击验证后才能关闭Coordinate实施自审。当前专属Candidate/Host复验中，原Unsupported日志保留。Map未用Radio3，其已通过450源码快照发生在该无关新key添加之前，不冒充修改后仍同hash。另一项目采用由用户选择，不自动合流。
## 2026-10-20 CAL-IMAGE-ENUM-008 — 初始化候选树真实数据流

生产者：具体工具dispatcher按资源GetFrontParamItemsOperation查询已存在ConfigurationSubscriptions服务，返回新增可选ConfigurationOperationResult.SubscriptionTree；消费者：ToolConfigurationState.Operate保存ConfigurationNodeState.SubscriptionTree的readonly roots快照，公共ImageEnumeration初建时装入typed ComboBoxItem，之后已有下拉刷新行为保留。Board/NImage/Distortion/Map/Coordinate均返回树，不将caption转identity、不丢EnumEntrysA数据。原具体Operate明确查询后SetPropertyValueByNameNotrigger(TargetName+A,list)，不是仅Handled无值。

载荷继续沿用ConfigurationSubscriptionTreeResult/typed binding/metadata；校验已有树身份与深度规则，算法参数、ROI及文件状态不变。生命周期为单次配置会话临时候选，不持久化进方案；新API为init可选属性，旧dispatcher与调用兼容。根线独占共享实施，另一项目不用复制第二份配置框架。实际NI生产初始化+真实控件pre-dropdown候选/刷新/选择10通过，build0warn/error；Dist/Map/Coordinate及共享State回归继续串行，未把中间9仅查询结果冒称最终列表赋值完成。最终自审核和跨项目采用分别记录。
## 2026-10-20 COORDINATE-ROI-007 — 专属工具接公共ROI与公开POINT输入

生产者：本工作树ToolConfigurationRoiControls.RegisterRefreshable / IToolConfigurationRoiRegistration.Refresh、ConfigurationOperationKind.RefreshRoiVisibility；消费者：CoordinateModule专属provider。载荷是仅Coordinate、Maximum1、无FullImage/Exclusions、InputImage端口的ROI lease；原RoiDisplayOperation触发typed刷新，关闭配置Dispose释放注册与viewport。兼容性为新增接口与枚举末尾值，旧Register签名保留IDisposable，不改普通Box门。原ModuleDisplayDecorator→IsCanDrawROI→CheckStatus链决定外显，不按模式名猜隐藏。当前原源核完，真实Coordinate Host尚待执行。

生产者GraphEditorDocument/GraphExecutor公开的POINT数组端口，消费者CoordinateModulePlugin.Read专属兼容投影。原native仍读X/Y两float叶；产品公开组合口合法Canvas连线需独立适配。优先scope、有结果deferred（含错误）、显式叶（含空），仅全缺时投影materialized父POINT；不把原031b错误当可忽略，未改共享Graph运输。此是明确的独立合同兼容，不称新增native组合读取。当前插件SHA94678605FB6F1B96CE05D8AC5DA166334C2C7E1FE88F8DB2A18653527CEDA6C2，Candidate/Host验证待执行；不会因已落代码计工具完成。

VisionData.ImagePreview新增原01000000 mono预览alias，Coordinate输入prelude只接受原01000000/01080001/02180014/02180021及精确载荷长。opaque未知format拒绝与独立long乘法防溢出域分列。资源、实际MainWindow/catalog注册只在本worktree实施，另一项目未采用；最终集成由用户安排。
## 2026-10-20 CAL-PATH-UI-006 — 原控件文件门与后台默认写入补证

生产者：本工作树 VisionPluginContracts 的 ToolConfigurationCalibrationPathPolicy、ToolConfigurationState.ParameterWriteValidator/ParameterWriteCompleted、CalibrationPathControls.selectionCommitted；消费者：Board/Distortion/NPoint/NImage/Map及普通Calibration provider。载荷为具体资源node/tag、请求值、操作结果KeepAttemptedValueOnError及owner/通知服务。仅OpenFileForCalibDialog运行资源后缀/目录校验；base OpenFileDialog和Loader不同类不混用。picker提交后history无条件执行，typed编辑不写history；SaveFileDialog以资源第一段EndsWith在导出之前拒绝。生命周期仅当前配置会话；无新持久字段，失败UI尝试值与已接受后端值分开。新增末尾可选回调兼容旧调用；不为主项目建立第二份配置框架。

验据：实际五provider85项、N点配置55通过，root-review-manifest SHA8A64C67B8D7F8511B8E9DF61CD7BC9566332AB1CF4AA8B748F0DC08965919602；独立审计manifest SHA871B0E0B05E915D013DDCC72D57AF349EF2C74449144417042D207E1F9879A78。后台默认名单WriteParameter(name,text)与可选writeBackendDefault已由Translation Reset42/State1692验证，替代下方“待回归”的当前状态，历史文字保留。远程/root明确Unsupported，未调用外部服务。另一项目未采用或合流，人工验收独立。

Coordinate的RegisterRefreshable及RefreshRoiVisibility当前已落本工作树，实际Candidate/Host尚待执行，不把编译成功写成宿主完成。Map/Coordinate MainWindow与catalog注册同属本工作树可审实现；跨项目采用仍由用户选择。
## 插件公共后台默认名单写入（当前实施、待最终回归）

生产者：VisionPluginContracts.ToolConfigurationParameterAdapter.WriteParameter(name,text)；消费者：本工作树CalibrationToolConfigurationProvider→ToolConfigurationAlgorithmReset.Execute可选writeBackendDefault。原Translation Algorithm.xml八项默认中WeightFun/WeightFactor/DistThreshold/SampleRatio没有Tab控件，原SetDefaultParamList仍按资源顺序写后台；旧helper依state.Find而跳过，属真实实施缺口。

payload为descriptor已声明参数名及原DefaultValue字符串；复用同一类型/范围校验、Host Apply、权威读回。无UI项不构造伪节点、不进入Nodes/ISub/renderer；未知descriptor仍明确错误。生命周期为手动Reset命令，逐项顺序、失败继续、不回滚较早成功、不自动Run；只有完整成功时回读UI。新参数末尾可选、旧调用兼容；仅普通calibration provider传入，Rotate专属UI writer仍保留原empty FilePath E0000001拒绝。当前Translation tests真实6ISub/隐藏四项/中途拒绝与ConfigurationState回归待执行，未宣称通过；根线拥有共享代码。另一worktree未改，通知不代表采用。

# Shared contract requests

## CAL-CONFIG-RESET-005 — 公共原名单顺序重置

生产者：VisionPluginContracts.ToolConfigurationAlgorithmReset.Execute；消费者：标定板与畸变配置provider。载荷为ModuleDescriptor、ToolConfigurationState、参数dispatcher、可空订阅服务、原Algorithm.xml文本、可选effective参数同步委托。公共helper按实际ISubInfo控件资源顺序读persisted @Name+tail模式、尝试解绑、同步权威参数并持久化False；再仅按有效ParamItem顺序写默认值，继续失败并报告最后失败，不回滚已成功项。宿主仍通过既有context.Apply提供写入权威，不需要第二套atomic协议。

生命周期：单次确认后的配置命令，无新持久状态，不清执行缓存、ROI、显示或非名单值。兼容性：新增API；两个工具移除对可选ResetConfigurationParameters整字典回调的依赖，其他工具保持现有行为。源码证据及当前身份见CALIBRATION-BOARD-ACTIVE-UI-RESET-AUDIT、DISTORTION-CALIBRATION-RESET-SOURCE-LIST。专项测试已新增，尚未验证时不记完成；通知不构成主工程采用或合流。

## CAL-NUMERIC/DISPLAY — 当前修正待回归

完整VM_M GetFloat/GetInt公共读接口失败时Count保持0，不能套用leaf内部正越界返回物理Count的行为；WorkflowNumericInputRead已据Frame ca500/ca720及11e580/11f5f0修正。Camera Index新增两项真实聚合错误门验证，尚待串行回归。ShowText位置订阅接受原GetOutputTreeNode未排除的float/int数组，消费index0；空/失败位置明确诊断并置0，不按位置数组长度扩展默认Content文本shape。共享显示实现已改，DisplaySubscription与DisplayArray专项待验证。这些为既有合同修复，不扩大一般参数或其他类型的数组接入。

## CAL-SOURCE-RECOGNITION-004 — R04真实源码识别生产接入

已由根线实施并完成有限生产验证。生产者为本工作树VisionPlugins.Calibration的FindSource/SourceRecognition/Opening/TagSampler/RecognitionBridge与原630120谓词CPU selector；消费者为MapCalibrationImageSolver、CalibrationBoardSolver（含畸变raw/corrected复用）和NImageRuntime。没有在主框架复制工具识别核或修改另一工作树。

载荷：NImageSourceExecution(NativeCpuClass)与NImageSourceRecognitionResult(Board,UnsupportedReason,Refined)；FindSource只生产原grid W/H、原序ID/XY、dense context，不再要求旧geometry先成功。SourceExecution可选实参追加到Map.Solve/NImageRuntime.Add及BoardCalibrationOptions；旧源码调用继续可编译，公开方法/record构造二进制签名变化需重编，不承诺旧ABI兼容。原Find/独立geometry API仍可单独使用，新生产路径不在源识别失败后回退它。

生命周期：一次Map双侧或NImage实际批次复用同一CPU record；Board每次可显式提供或按原CPUID/WinAPI谓词检测。记录不写方案JSON、不建立全局native句柄。现Windows x86正常本机检测11与原630120相同；backend仅原class11已完成当前源码/有限oracle验证，其余class明确Unsupported，不将Avx2.IsSupported改名当原class。回调、设备、平台Init不引入。

ROI载荷保留先crop→原grid→mask稳定压紧→float原点→重算剩余min且保留W/H→原图解码。原proposal顺序/首成功/方向与anchor替换旧扫描所有quad再按全opening过滤，原R03纠错继续复用。radius2未知前缀或未支持后端明确返回原因，禁止硬编码原pool字节。

证据：analysis/map-calibration/r04-production-validation/final-manifest.json SHA BB41538225FC917ECFA7CE2325B5B529885E675B603F3FCD8651CE89A50BF27E，14生产源/三DLL/入口/真实日志齐全。Opening40000 normalized字节、40000score、12payload、外H/逆H9位均相同；原先12字节FAIL保留，经80ac20 asm按行加法分组修正。纯ProjectReference public生产2046项、实际Map ROI7361项通过；新畸变Plugin6/RotatedROI151/实际Host21也通过。仅Refined=false原支路被本批运行命中，Refined=true实现有源映射但无本批成功运行，不外推全部bit parity。原core SHA85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549。人工验收独立；通知不代表采用/合流。

## CAL-SRC-001：标定板候选角度的共享源码接口（2026-10-19，插件线）

- 生产者：插件线VisionPlugins.Calibration的SourceCandidateDetector/SourceGridPyramid/SourceLatticeAdapter；消费者：本工作树CalibrationBoardSolver的棋盘及编码板全图/crop/mask。根线为共享文件唯一修改者；没有请求主项目重做算法或配置框架。
- 载荷：新增float minimumAngle/maximumAngle重载，原有方法签名与30/135默认行为保留。标定板按原厂公共options+48/+4c传30/140；第二轮65/110不变。无工作流I/O、方案格式或持久化字段变化。
- 生命周期：角度仅随本次识别同步传递，无静态状态、句柄或跨会话缓存；取消仍沿既有token传递。
- 证据及接受条件：plugins/CALIBRATION-BOARD-A2-CANDIDATE-ANGLE-SOURCE.md；原厂18次运行记录和具体投影/消费者源码。全候选回归结果独立登记，不把有限fixture视为全域等价。接收方同步时只审查相关共享三文件及Board四处调用，保护其他在制改动；通知不代表采用。

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

## PLUGIN-DISPLAY-001 — Display.xml 图像对象选择呈现

- 生产者：工具插件执行快照与 `ToolDisplayImageLayers`；消费者：公共 `ToolDisplayImagePresenter`。
- 原厂依据：畸变Display XML声明 `InImage`（映射 `InImage`，默认显示）及 `Display Post-Correction Image`（映射 `PostCorrectionImageData`，默认隐藏）；模块 `ProcessOutput` 将校正图发布为 `PostCorrectionImage`。`ModuleResultView`先选中一张底图；独立 `DisplayImageListDecorator.SelectImageCommandExecute` 与 `AddCheckImage` 支持最多3张额外检查叠加图（总计最多4张），有底图且其 `IsBackgroundImage` 为真时叠加可见，否则折叠。故不是始终单图，也不是默认将所有image对象同时叠加。
- 载荷：追加可选 `ToolDisplayFrame.ImageLayers`、`SelectedImageName`、`BackgroundImageName`、`CheckedImageNames`。Presenter显示选中底图和至多3张明确勾选的overlay；底图为BackgroundImage时overlay才显示。旧节点继续使用原 `Image`/`ImageVisible`。
- 生命周期：图像从该节点最近一次已捕获的输入/输出解析；刷新配置不重跑算法。畸变显示桥接只对 `PostCorrectionImageData` 作别名，不修改 `PostCorrectionImage` 运行端口。原厂对象IsDisplay写入VmModule.UserData；另将Selected/Background/Checked清单序列化到`CheckedImages`。产品层目前没有提供原厂等价的节点会话保存/恢复交互，frame载荷是该交互的显式边界。
- 兼容：新增字段默认空，保持旧 `Image`/`ImageVisible` 行为。layer.Visible只保留原厂IsDisplay标志；源结果视图对Image对象按类型取图，且checked-overlay经独立命令路径处理，不将IsDisplay简单等同于是否可选图/叠加。
- 验收：畸变专属检查验证映射和校正输出像素；WPF Presenter已验证单图选择、以及由frame指定的背景图+三张overlay合成。`RenderControlBase`/`ModuleResultView`为叠加层分配ZIndex 100/101/102和Alpha 191/127/63。真实DisplayImageList checkbox/background交互、缩放、用户数据恢复尚未完成；目前检查验证渲染合同，不宣称完整原厂交互复现。

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

## FCR-030：main终止scope释放失败诊断

- Producer：main既有VisionFramework.ModuleExecutionScope；consumer：主窗关闭/方案替换和拥有scope的诊断工具。root唯一修改本工作区该类，不修改插件配置、工具代码或tool-plugins。
- Payload：新增只读CleanupFailures快照及ModuleScopeCleanupFailure(Phase,ResourceType,Exception Error)，Phase为cancel/resource/token。诊断非方案数据；原异常只保内存，工具选择文本输出。
- Lifecycle：取消回调异常记录后仍等待最后lease；逐项尝试owned资源释放，最后尝试取消源Dispose。失败对象/异常保留至scope回收，无自动重试，不宣称失败资源已成功释放。borrowed view Dispose仍不关root。
- Compatibility：保留既有签名，新增可选诊断。终止Dispose从单项异常传播改为记录，原先依靠catch获取释放故障的消费者应读取CleanupFailures；RegisterService替换/RemoveNode/RetainProcedureNodes即时Dispose仍可能抛，不扩大承诺。局限为main独立加固，非原VM异常策略。
- Evidence/acceptance：修前8项实证显示首项失败跳过后续资源/token及取消回调传播；修后8项确认逐项尝试、诊断、一次释放与lease门保持。实际主窗恢复89项、配置close16项及Group最终57项通过，详见reverse/C-host-recovery-batch20.md和reverse/BATCH20-20261020.md。长稳/设备/native未知另列，插件合流由用户选择。


### N点公共配置架构迁移：本线在制接口（0f3a1d4之后）

- NPointPlugin改为IToolConfigurationProvider，生产入口使用原厂NPointAlgorithmTab/Group和公共ModelWindow；旧NPointWindow仅保留历史兼容代码，不再是工厂入口。
- 类型化对象桥接承接Show Text五条ColorConfig/FloatConfig/StringConfig路径，使用DisplaySettings持久化，不写算法CurValue；默认/更多颜色面板复用VisionUi原厂资源。
- 新保存/确认服务分别承接SaveFileDialog和Reset确认；ConfigurationFileRequest附加CheckFileExists/CheckPathExists，默认行为不变，标定路径明确允许不存在文件。SaveFileDialog的Command显示别名有单独操作路由。
- ToolConfigurationState增加CategoryHiddenOperation、EditButton_Calib整数继承及RefreshParameter权威读回（同步失败回滚基线）；输入选择扩展POINT组合类型，但标量/数组基数严格校验。
- 显示位置订阅采用Domain.Display和DisplayInputBindings typed DTO，单独枚举祖先/self数值输出，绑定不生成算法自环；ToolDisplays从所属流程真实结果缓存投影坐标，缺失回落0并诊断。原厂动态IsResultShow IO ABI未声称相同。
- ResetConfigurationParameters由宿主接入GraphEditorDocument.ResetProcedureNodeParameters，将参数默认值与参数订阅清理合并为一次可撤销事务。此为插件框架所需局部Core能力，本线独占新partial文件；不修改主项目目录。
- 用户纠正后四线为N点/N图像/标定板/相机映射四个独立工具，各线提出必要框架需求，主控单一协调共享文件。
- PointPair 复合字段按原厂 CLR 声明登记独立子状态（保留无 Name、原始 XML 与元素身份），GetNode(element) 取值；ControlContext.CreateChild(element) 继承 Owner 和刷新生命周期。禁止以同名局部标签做全局查找。新增状态测试覆盖两组 POINT destination 和数值隔离。
- PointPair 原厂 AddButton 仅移动内存 Collection/Items；公共 State.GetChildren/MoveBefore 维护运行时同父元素顺序，View保留控件实例重排，不写入自创持久化参数。EnumerationG 使用原厂 GetFrontParamItemsOperation/IMAGE + SetCombinationSourceOperation/InputImage 查询与 typed 下拉绑定。标定路径模板与三态资源已提升到公共 Contracts，NPoint包装仅转发。
- 相机映射后续缺口（未在本次验收节点实现）：原厂组合 POINT 与逐 X/Y 输入的别名/分量绑定，需要共享服务显式描述关联、组合来源投影、单轴覆盖及一次事务切换。当前独立 POINT 端口类型/基数校验不能代替该语义；运行时逐轴覆盖仅是独立兼容行为，原厂字段/切换证据由相机映射清单补齐后实施。

### 四工具后续公共接口（本线在制，尚未通知为已同步）

- POINT 别名：N 点、N 图像、相机映射声明 IToolPointInputAliasProvider；公共服务将组合绑定变为真实 X/Y 两条叶订阅。混合来源以只读 typed tuple 回读；数组使用显式 NumberArray 传输。GraphEditorDocument.SetProcedureInputSubscriptions 统一验证并提供一次 Undo，宿主在关闭、换方案、锁定或执行中拒绝写入。此原子性为托管安全设计，原厂逐次失败副作用未声称一致。
- CompoundOutputFields 的 POINT.X/Y 同时供普通图执行与配置自运行使用；不得在 UI 枚举可选来源后缺失实际执行投影。共享文件由本线本轮协调所有，不改另一工作树。索引 ABI 仍明确 Unsupported，不能由官方托管 GetPubInfo 的 0/1 字符串结果推断 native SubscribeResult 的边界。
- 图像对象：ToolDisplayObjectStore 的 DisplayObjectSettings 与算法参数分开；公共 ImageViewConfig 适配 source getter/setter，严格写后回读并保留当前弹层。ToolDisplayGraphic 承载真实 point/path/coordinate/rect，ToolDisplayPresenter 统一绘制；XML 重复对象保持源定义，不虚构唯一身份。消费者为四个标定工具。
- RGB：VisionData 的 ImageColorConversion.ToMono8 支持 RGB24 与行跨度，为 N 图像标定及图像源 Mono 输出提供公共核。文件解码器的灰度算法保持单独契约，见 N 图像 RGB 原厂像素对照报告。
- 仍需实现：标定板原厂普通配置窗口的外置 ROISelecter 会话服务（调用方、输入图尺寸、RoiType 参数、Single Box、FullScreen、RoiTypeSeq、一次配置事务、提交后执行、关闭/换节点取消绘制）。禁止为板工具单独新建另一套图像配置窗口。完整原厂高亮/DPI 交互尚未闭合。
- 证据、独立设计与未验证边界见 plugins/CALIBRATION-SHARED-DISPLAY-POINT.md；本轮手动验收及提交前仍属在制，不表示主框架已采用。

## 2026-10-18：标定板/相机映射公共能力（插件线在制）

- 单一交叉文件所有者：插件线根任务；改动仅在 tool-plugins 工作树。消费者为标定板、相机映射，后续畸变和映射标定按能力迁移。未提交、未获本轮人工验收，也未表示主框架已采用。
- 外置 ROI：生产者 ModuleDescriptor.RoiEditor 声明工具能力；ToolEditorContext.ConfigurationRois 提供带 NodeId、原厂 SourceName、输入端口与能力的作用域会话。宿主从实际订阅和运行结果读取图像，复用 GroupRoiEditor 几何引擎，把原厂选择/参数区域放普通配置窗，把图像表面放外置视图。几何与参数一次 Core 事务写入，再自运行；Undo/Redo 权威回读。锁定、运行、换流程/方案、关闭或已失效回调不得写入。旧参数级 ROI 描述保持兼容。当前只实现 Single Box + FullScreen，托管 RoiDocument 不等于原厂 IUserBytesData 字节 ABI；高亮、DPI、全部形状不宣称等价。原厂源码位置见 plugins/CALIBRATION-BOARD-FRAMEWORK.md。实际宿主 23 项检查通过，截图在 analysis/ordinary-roi/board-common-window.png。
- 有效参数：生产者 GraphExecutor 在绑定解析/验证完成、进入模块前记录 ModuleExecutionScope 的流程+模块+节点隔离快照；配置自运行显式传原文档 seed。消费者 ToolEditorContext.ReadEffectiveParameters 可读运行值，并与之后文档显式变更合并；解除参数订阅用最后有效值和绑定清理一次提交，Reset 覆盖有效值。节点删除、流程清理、作用域释放清理快照。此为托管模块入口状态，不是 native SDK SetParam 已成功确认；算法失败前后的 native 部分 setter 副作用另列未验证。相机映射实际宿主 19 项通过，包括失败绑定不污染、Undo/Redo、自运行与跨流程隔离。
- IntegerBettween：依据 IntegerBettween/ValueBettween.GetSubChildren 和 ParamUserControlDataTemplate.xaml 两个真实 Integer 子字段，各自持有状态、操作、订阅身份；按源同行显示与交叉端点限制。两个浮动滑条仍复用现有数值控件，原厂同弹层双滑轨外观/事件时序须继续补齐，不将“无未知模板”当外观一致证明。
- 畸变显示缺口：CalibPointStatus 数组逐点 OK/NG 颜色、Show Evaluation Error Text 的坐标/误差数组逐点文本及独立 FontSize feature、Display Post-Correction Image 独立图像可见性。当前单文本/输入图适配不能替代它们，暂不声称支持。
- 映射标定缺口：原厂 RoiType_0/RoiType_1 分别绑定 InputImage1/InputImage2。当前单会话和 Node.Rois 扁平列表不能同时保留两个 ROI，需要带源身份的分组持久化与两个图像上下文；禁止用第二次注册覆盖第一次冒充完成。

### 畸变固定点集数值核需求（2026-10-18，在制）

生产者：公共标定核；消费者：畸变标定。需在现有 RefineNormalized 内核外提供窄的固定全量点集入口（点对、自由度、权重/WeightFactor、明确成功或失败），跳过 Solve 的 RANSAC/四点枚举，不复制数值核。证据及7ed0c0四个调用阶段见 plugins/DISTORTION-CALIBRATION-FIXED-FIT-REQUEST.md；原厂 WeightFun1/4 为不同分支，options20不能直接当迭代次数。现有 Solve 行为及 N 图像168点矩阵回归须保持。本项尚未实现，不纳入相机映射验收。

### DC-WORKSPACE-001：畸变固定拟合的排序工作区前缀上下文（2026-10-18，在制）

- 生产者：插件侧标定运行会话/原厂兼容 scratch 容器（布局与初始化仍待恢复）；消费者：共享 7ff3c0→7fd4e0→7fec40 数值核及畸变标定。跨项目仅记录合同，不修改主框架工作树。公共核生产源码暂不改动。
- 已实现入口澄清：FitNativePoints 直接对应7ff3c0，含Weight1常权IRLS；FitNativeLeastSquaresPoints 对应畸变7ed0c0的8345e0独立OLS。Map即使Weight1也用前者。畸变options+0xc已从7fd4e0源码确证是20次上限，Map为50，delta均1e-4f；上节“options20不能直接当迭代次数/尚未实现”是旧阶段记录，以此处为准。旧Solve路径保持兼容。
- 所需载荷：完整float点对、原生options（自由度、权重、迭代、delta、factor）、前置H，以及排序缓冲前缀的原始float bits/边界、scratch容器来源和会话身份。当前最小受控fixture使用2n+32个前缀float；这只是覆盖已捕获12次回放的边界，不是已恢复的通用容器尺寸。禁止按病例硬编码sigma或统一填sentinel=1。
- 生命周期：原厂E2E SDK复用workspace与独立清零workspace在相同输入/前置H/options下返回不同结果。需继续恢复分配布局、前次写入来源、每次拟合/参数重置/模块关闭时保留或清理规则，之后才能决定scope状态和持久化策略；不把工作区脏值直接写进方案参数，不能把未知生命周期伪装为无状态数学核。
- 源证据：7fec40插入排序没有显式index>=0保护，会读写排序缓冲前缀。seq56 E2E前1float为0x3f800000(1f)，sigma=1.48257971；相同归一化点/系数、清零前缀时原核sigma=.00767620793。seq67分别3.61378813与2.20921592e-7。两套7fd4e0入口1296字节完全相同，初始设计/RHS1386字段同位；首次差异在权重生成后的正规矩阵。已排除旧trace序列关联错误。
- 验收证据：tests/DistortionCalibration.NativeProbe/{RunTraceMedianContext,RunMedianContextReplay}.ps1；docs/plugins/distortion-calibration-workspace-evidence.json保存6组完整输入/前缀与12个原厂输出。直接原核6组、清零/捕获上下文各一次均复现；受限托管排序回放只在捕获数组内访问，越过边界显式失败。原厂SDK完整容器布局与生命周期尚缺，不能据此声称全工具等价。
- 兼容要求：保持旧Solve404组/11066字段回归及Map Weight1 12组108字段、独立OLS252组2232字段结果；任何共享内核修正须先协调并单独验收。当前Workspace证据是上下文合同需求，不是强制所有调用者使用原厂越界行为的全局开关。

DC-WORKSPACE-001内部布局补证：7ff3c0以A(x)=Align128(x)排列workspace。normalizedSource偏移2*A(8n)、normalizedTarget偏移3*A(8n)、designA偏移4*A(8n)、rhs再加A(64n)、Ti再加A(8n)、Tw再加128、pointErrors再加128、weights再加A(4n)、medianSort再加A(16n)。7fd4e0只写2n个float权重，medianSort[-1]因此位于该区尾部未写padding。n=77时weights=8832，medianSort=10112，实际权重写入止9448，余664字节未写；sentinel偏移10108。内部布局已从原厂源码恢复，仍缺外层SDK分配/清理/复用与前次写入来源，不对未定义旧值规定统一默认。受限托管context replay正式12/12原厂sigma逐float位相同。

### DC-TEXT-001：具名文字显示 feature 的公共桥接（2026-10-18，在制）

- 生产者：插件公共显示对象/配置桥接；消费者：畸变 `ErrorTextFontSize` 及其它明确接入的具名 text 对象。原Tab:874–886通过GetDisplayObjectFeatureOperation("Show Evaluation Error Text;FontSize")绑定Feature.ValueA，不能写算法CurValue。
- 源码路径：Apps.XmlParser.ParamTab/GetDisplayObjectFeatureOperation从module.[Display]中按唯一对象Name选取Feature；BridgeItemBase.BindingPathA=BridgeObjA.PropertyNameA，GetParamUnit/SetParamUnit为no-op；FloatConfig_sub使用SliderArrangeFloat并绑定Min/Max/Inc；畸变为6..72、步长1。Feature.ValueA通知对象刷新并保存，FeatureBase.SaveValue写用户数据key `[ParamRoot]_对象名.Feature名.Value`（UTF8补零）。Object.SetShapeProperty(text)消费FontSize及逐索引Position/Content/Status，非算法参数。
- 最小载荷：source对象名、feature名、字符串Value；字体来源Display.xml默认10。托管实现可复用已有ConfigurationUserData字符串字典，以原始key保存，避免破坏DisplayObjectSettings已有样式JSON或旧Show Text的DisplaySettings模型；named text桥接以组合service显式接入，旧Show Text仍走原服务。
- 生命周期/兼容：模块会话内编辑后立即显示、重开与方案持久化保留；算法参数重置按现Provider保留用户显示数据；未知/重复对象名或未知feature必须拒绝、拒绝后读回原值。保留现有色彩/线宽/可见性数据和其它UserData键；不要让旧style写入覆盖feature值。保持旧Show Text路由和所有既有节点默认Build行为不变。
- 显示消费：新增可选具名文字投影入口，按CalibPointX/Y、CalibPointError、CalibPointStatus生成每点文字/颜色并读取FontSize，跳过旧Show Text避免重复；不把该数组文字错投成单条均值文字。公开传输可追加非破坏字段，但不强制所有现有节点自动启用。
- 验收：真实WPF FloatConfig从10编辑并持久化、6/72边界与拒绝回滚；重开/Graph+方案保存加载；原Show Text现有ObjectChecks保持；新投影和Presenter读取同一feature、独立字体与逐点状态；明确不将不等长数组/原生字体异常行为外推。

DC-TEXT-001实现状态：公共 ToolDisplayFeatureStore/ToolDisplayNamedTextConfigurationObjectService 与显式 BuildNamedTexts 已接入畸变配置和专属投影；复用原 ConfigurationNumericSpinner，不建第二套模板。NamedTextChecks33项、旧ObjectChecks48项通过，包含实际SolutionStore保存重开及Presenter。现有DisplaySettings/DisplayObjectSettings格式和默认Build保持；原生异常字号/不等长数组及畸变完整runtime仍单列未验。

## CAL-RCF-001 — 标定加载刷新后的动态输出描述

- 生产者：`ReadCalibFileModu` 插件及插件侧 `ModuleRegistry`/`GraphExecutor`；消费者：主框架图连线视图与运行宿主。
- 原厂依据：`ReadCalibFileModu.dll` 的 `Process` 在非零 RefreshSignal 后重读当前文件并按新字段设置输出。受控原厂实例复现同 schema 值更新和 `Dynamic`→`CalibMatrix` 九项输出 schema 改变；不代表完整平台/桌面运行。
- 载荷：仅显式声明 `ModuleDescriptor.SupportsDynamicOutputs=true` 的模块可从 `ModuleResult.UpdatedDescriptor` 提交本轮描述。ID/version、输入、参数、显示能力须与注册描述一致，只能更新 Outputs；输出名唯一、类型/基数合法、Required 由 GraphExecutor 对结果强制验证。`ExecutionEvent.UpdatedDescriptor` 与 `ExecutionSessionResult.NodeDescriptors` 将有效描述送给宿主。
- 生命周期：先按现描述解析输入，执行后先验证并记录实际描述，再在每条下游边取值前检查源端口仍存在、输出类型/基数与目标输入兼容。旧端口删除/改型时，消费者在运行前以 `DynamicConnectionInvalid` 失败。宿主接收更新后刷新端口视图；不自动改写流程、不自动重连。工作流变量、接口、显示与参数订阅继续要求稳定 schema。
- 兼容：固定模块的验证分支、构造和原进度事件保持不变；没有动态能力声明的模块不得提交更新描述。默认 `SupportsDynamicOutputs=false`。插件工作树已完成增量实现和真实 GraphExecutor 试验；待主线选定同步点后集成并检查编辑器如何消费描述事件。
- 验收：覆盖同 schema Refresh、端口改名后的旧连线阻断、新 schema 连线成功、类型变化阻断、无效描述拒绝，以及静态模块既有检查。固定节点行为逐项回归；新事件提供新描述，旧宿主忽略该可选载荷仍能继续消费状态事件。动态输出暂不支持工作流静态订阅，不能推断其兼容。

## CAL-RCF-002 — 标定加载远程文件选择模式

- 生产者：主框架工具编辑器上下文；消费者：标定加载 `OpenFileDialog4ReadFile` 适配器与 `IToolConfigurationFileService` provider。
- 原厂依据：原厂 `OpenFileDialog4ReadFile` 检查 `RemoteOperate.IsRemoteMode`。本地分支使用 WPF 文件对话框并从本地历史目录初始化；远端分支使用 FTP `DirectoryBrowser.OpenFileDialog`，XML 过滤器，选择首个文件后更新 FilePath，再记录所选目录历史。反编译确认远端目录枚举最终调用 SDK FTP API，并依赖原生平台句柄。
- 载荷：`ToolEditorContext.IsRemoteMode` 可选 bool，缺省 false；ReadCalibFile 将该值复用到已有 `ConfigurationFileRequest.Remote`，不读取未定义节点参数、不新增平行选择接口。`IToolConfigurationFileService.Select` 收到 module key、当前路径、原厂 XML filter、标题和 Remote 标记，返回 typed 状态及路径；随后原有 SetCurValue 提交和 Remember 历史次序不变。
- 生命周期：宿主明确提供远端服务时由其处理 FTP 路径和选择结果；未配置远端 backend 时默认 WPF service 必须返回 `Unsupported`，不能回落到本地 picker。取消不改值；选择后仍由节点配置事务加载并提交文件/schema，失败时诊断，原厂会记录所选目录历史。
- 兼容：`IsRemoteMode` 默认 false；本地请求逐项保持原参数与行为。插件侧已用 mock 确认 Remote=true、当前路径、XML filter、选择值提交和运行；没有真实 FTP backend、SDK 句柄或远程平台操作，本请求不宣称原厂远端运行等价。
- 验收：主线将 remote mode 从实际宿主状态传入上下文，并注入 FTP 文件服务；检查本地/远程分发、XML 筛选、当前目录、选取首文件、取消、错误诊断与历史提交。真实远端 native 运行需要有效设备/服务器和平台句柄，未具备时保持明确未验证。

## 标定板生产宿主消费记录（2026-10-19，待人工验收）

- 生产者：插件线 `VisionPlugins.CalibrationBoard` 及既有公共配置/外置ROI/显示服务；消费者：本工作树 `VisionEditor.MainWindow`、工具箱与真实执行作用域。未修改主项目工作树。
- 载荷：保持现有 `vision.calibration-board` 1.0.0 描述、`InputImage`、单Rectangle/FullImage能力、`RoiType`命名组、参数/显示设置和 `ModuleExecutionScope.ForProcedure`，无公共签名或方案格式变化。新增生产引用/注册和源工具箱ModuleId；交接时需接收方逐文件审核，不能整文件覆盖MainWindow/项目引用/目录。
- 生命周期：复用普通Owner/modeless配置窗，ROI提交原子写回并自执行，关窗释放外部视图；按方案/流程/节点作用域取执行快照。失败Detect ROI与成功结果分开门控；两个同名Data Record按资源声明顺序格式化，换行是托管呈现选择，不声称原厂精确排版。
- 兼容：不修改其他工具工厂/算法，不新增扫描器，不改动另一工作树；生成验收棋盘与截图代码只在tests。同步前必须保留接收方现有注册/工具箱改动。
- 接受证据：49项真实宿主检查（生产注册、三页签、显示眼睛/透明度、ROI/撤销/保存重开/失败恢复及owned Group）；日志 `analysis/board-host-acceptance/host-localized.stdout.log`。最终全量与启动见 `HANDOFF.md`，人工验收尚待用户反馈，原厂桌面及ROI原生ABI未验证。

## CAL-SRC-002/003：映射源码识别阶段的内部载荷与首成功边界（2026-10-19，待补证/实现）

- 证据：MAP-CALIBRATION-D2-SOURCE-EXIT-AUDIT和D2-PRODUCT-REVIEW；原core SHA85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549。D2 S1/S2已批准不代表D1 decoder或整卡S4完成。
- 生产者/所有者：插件根线共享NImageSourceLatticeAdapter、GridPyramid、NImageBoardRecognition。消费者：本工作树MapCalibrationImageSolver与其他RecognizeLattice调用者；主框架无需重写算法或新增宿主Contracts。
- CAL-SRC-002载荷：一次识别显式区分选中的首成功identity/tag与独立兼容opening掩码。当前产品扫描全部quad并tags.Any过滤点，原813ca0首成功即停。不得仅break造成历史过滤点静默改变；实现前复用normal/reverse proposal序列及既有168点对照，明确全扫描兼容输入与选锚定阶段。
- CAL-SRC-003载荷：保留source grid Width/Height、minColumn/minRow、有效ID/XY原顺序，以及双int INT_MIN缺槽上下文；同ID后写覆盖。原float缺槽未知，不以零数组冒称原厂初值。外部稀疏list是否走source-grid适配应明确，不能在各层重复去重/Nearest重猜anchor。
- 生命周期：只随本次识别同步传递，取消沿现有token；无持久化、设备、跨会话状态或工作流I/O变更。当前独立geometry fallback保留，原厂proposal/decoder未闭合部分不按意图替换。
- 兼容/安全：继续过滤原生负flags写，禁止引入负数组访问；实际padding值/尾槽/多候选影响属于S3，独立安全选择持续明示。通知不代表另一项目采用/合流。
- 接受条件：先复用GridSelection/Padding/Assignment/Expansion/Pruning及DetectorBoundary normal-reverse、image-execution-source-grid证据做同输入阶段差分；源码/运行/实现/宿主分别记录。当前仅接收内部请求，未修改公共产品，未计S4或重验通过。

## CAL-DISPLAY-POINT-STATUS — 逐点状态颜色（已实施、自审通过）

- 生产者：ToolDisplayGraphics.Build；消费者：ToolDisplayImagePresenter，以及畸变等显式point对象调用者。
- 载荷：ToolDisplayPoint追加可选Color，保留原X/Y/Tips/Index构造；point每个标记保存源index颜色，未提供Color时Presenter回原ToolDisplayGraphic.Color。非point与path连线消费保持旧路径。
- 生命周期：仅本轮显示投影，无持久化/schema/工作流I/O变更。原Object.SetBaseProperty按index取Status；原非空数组超界回首项，单项广播，空转0→NG。当前实施明确覆盖整数状态点集合，不宣称所有shape逐索引特征闭合。
- 兼容/验收证据：NamedTextChecks38项退出0，包含混合色、index、实际WPF Path色、广播和空状态；旧字体/方案持久化仍通过。summary在analysis/point-status-checks/summary.json，报告CALIBRATION-POINT-STATUS-IMPLEMENTATION.md。根线实施自审通过，用户人工验收单列不计进度；通知不代表另一工作树采用。

## CAL-NUMERIC-READ-001 — 可选分阶段数值输入读取（已实施、自审通过）

- 生产者：现有ModuleExecutionScope注入的IToolNumericInputReader provider；消费者：相机映射84b0/8580对应的两遍输入收集。所属插件侧Contracts，不修改另一工作树或主框架调度/PortValue。
- 窄载荷：Read(ModuleExecutionContext context,string inputName,int index)返回ToolNumericInputReadResult(uint ErrorCode,int Count,double Value)。index0计数探测与随后逐index取值是独立调用，provider可返回原读取错误与已知count，调用者不得用抛异常代替原错误路径。
- 生命周期：读取同步且只在当前Execute内使用；provider由现有scope服务提供，使用context身份/输入，不持久化、不缓存跨节点来源，不新增设备或全局native句柄。未注册服务时保持当前materialized PortValue/组合兼容适配，该默认路径不能表达时变原生叶错误，必须明示独立边界。
- 原厂证据：CAMERA-MAPPING-B3-B4-CLOSURE-MATRIX及同名json。84b0探测四count再折flag；8580逐值保留原错误；a710按TX TY OX OY每成功字段立即写缓存，失败不回滚。数值全部成功后的既有生产路径与24例216字段基线应保持。
- 自动验证要求：count不等拒绝、count等但某flag非1组不纳入、动态读取失败/中途字段错误保留已写cache、后续小轮成功和显式Save容量行为。新托管分配的零初始化仅作安全独立边界，不宣称复刻原malloc未初始化内存。已完成根线代码/测试自审：InputChecks36、生产PluginChecks146均通过；新Runtime24例216字段固定0.003差分mismatch0，三项目构建0警告0错误。产物analysis/camera-mapping/b3-input-lifecycle-source-20261019/cm0203-*；仅该合同与CM02/03实施通过，整工具仍待其它实际缺口，人工验收不作进度门。

CAL-SRC-003/R02运输实施状态：NImageSourceGridContext及NImageLattice.SourceGrid已落生产adapter，保留格宽高/min offset、原顺序及dense双INT_MIN缺槽，XY缺槽nullable未知、同ID按源序后写覆盖；非法外部上下文独立安全拒绝。独立构建/运行exit0/sourceStable=true，两实图上下文逐项对应及8例当前识别168点保持，记录analysis/nimage-source-context/2fdc0978528e439db9480cd929d4399f。报告NIMAGE-SOURCE-GRID-CONTEXT-IMPLEMENTATION.md。只完成运输，R01 mask/R03纠错/R04原始anchor和方向消费仍开放，不冒充完整D1或完整工具。

R02后续投影接入：生产者CalibrationBoardSolver crop/mask投影点，消费者同次识别NImageLattice.SourceGrid及后续R04。新增RebuildForProjectedPoints保留原grid宽高，在mask压紧/float原点投影之后重算剩余ID minima，重建dense身份/XY；旧公开构造与null上下文兼容，载荷不序列化、不跨运行缓存。根线于本工作树实施，未改主项目。依据同hash80e690实际顺序，八组168点边界及Board encoded-axis/mask完整输出aggregate通过，详情NIMAGE-ROI-SOURCE-CONTEXT-PROJECTION.md。R03源码纠错另已实施通过；R04自动识别仍开放，通知不代表采用/合流。
# CAL-IMAGE-PREVIEW-001 — generic IMAGE cached preview

2026-10-19，插件根线实施，待最终宿主回归记录。生产者：图像源已拥有的GrayImage/RgbImage/IImageData PortValue；消费者：畸变等声明DataKind.Image的工具ROI预览及单模块调试。NodeDebugExecutor以前只用PortTypes.Matches精确kind，正常工作流能运行的GrayImage→Image被预览拒绝，ROI snapshot为null。现仅补充目标DataKind.Image且源为已声明图像种类、cardinality一致时接受；没有改序列化payload、数值转换策略或跨节点所有权。

生命周期：读取本轮已有上游缓存，不执行生产者，不复制/持有额外native句柄；失败缓存、多个来源、类型/cardinality冲突仍拒绝。兼容：旧精确类型路径保留。证据：实际MainWindow畸变HostChecks前两次运行成功但ROI为null，第三次诊断NodeDebugExecutor明确type/cardinality mismatch，见analysis/distortion-host-validation/fef068c1d91a415ca873f4991b7f8578/diagnostic.log。自动宿主验证正在进行；人工验收另列，不代表跨项目已采用。

# CAL-NUMERIC-INDEX-002 — scoped numeric selector transport

2026-10-19，根线审查并应用相机线21文件候选，端到端All/索引206及36/47/147、固定0.003的24例216字段通过；后续严格父节点生命周期修补正在追加验证。生产者：配置订阅桥、POINT逐叶别名与GraphExecutor；消费者：CameraMappingInputReader及畸变中心两数值叶。WorkflowValueReference新增可选ArrayIndex（互斥原串LiteralText或typed IndexSource），旧JSON/null仍All；PortDescriptor.SupportsDeferredNumericRead默认false，相机32坐标口及畸变中心两口显式启用。非数值、参数、Global/Local/父Group索引仍明确不支持，不隐含已实现。

运行载荷为ModuleNumericInputReadResult(ErrorCode,Count,Value)及ModuleExecutionContext.ReadNumericInput每次调用委托。Framework不反向依赖PluginContracts；既有scope IToolNumericInputReader优先。绑定原数组保留，不在Graph前提前slice；动态int首值读取失败回退0，成功非All Count1、负索引Count0、正越界保留原Count。GraphExecutor.RunAsync末尾增加可选逐节点callback供实际配置自运行和NodeDebugExecutor隔离执行运输；生命周期仅本次await执行，不序列化、不存全局。公开方法新增参数需接收方重新编译，不能声称旧二进制调用签名不变。

原串数字解析按原四regex和atoi有界域；正常UI GetBindIndex先Contains('.')查IO路径，未找到Warning后仍应用来源+All。候选保留这一部分应用，而非typed decimal成功或回滚。Warning枚举追加末尾，旧值不移动。动态索引身份随复制/删除/Group重映射递归处理。producer/consumer、旧格式兼容、执行与释放范围见CAMERA-MAPPING-ARRAY-INDEX-INTEGRATION.md及LITERAL-GATE报告。补丁SHA69C7B5EEBF656F3AC5476B7D27DC03BF68E510EE6923B8203E864E3A5D4F9252及后续All/debug修补已应用并完成实际Graph/Host/Debug验证；父节点末尾修补另列，不以旧206覆盖新源码。通知/合同不代表主项目已采用或合流。

## CAL-POINT-PARENT-003 — 组合节点索引摘要与复制接缝

生产者：PointAliasConfigurationSubscriptions与宿主非持久PointAliasPresentationState；消费者：相机/畸变POINT配置，以及本工作树GraphEditorDocument.PasteNodes。原VmIO.RelateIndex取子节点首个非null索引后fallback父本地缓存，父缓存不由叶清All自动清除；源生命周期不可用持久化隐藏参数替代。

PointAlias构造追加可选presentation状态，旧调用仍可编译。实际MainWindow按流程/模块/节点保存，配置重开复用，ReplaceExecutionScope或节点移除清理；该状态不写JSON、不进运行叶输入，文档Undo不伪造原父字段快照。宿主图复制需要显式消费当前源父摘要，故Core新增暂态GraphPasteContext/GraphPastePreparation及PreparePasteSubscriptions hook，默认null保持旧行为；在来源刷新后、单次Edit前准备订阅，提交后才记录新节点父缓存。新父缓存不随clipboard快照持久化。原PasteIO可重建完整同父X/Y时会按父摘要重建两叶，动态索引的mapped-publisher分支按实际原GetIO规则处理；不套用caption解析。

所有改动只在插件工作树，由根线应用；接收方需审查Core暂态hook与其现有复制逻辑，不能整文件覆盖。parent-lifecycle.patch SHA F74DF44CEB56B8FB45EE50847A7E530A3CC66195651C7A6173EA35DD609F2A14已应用，新增真实MainWindow重开/复制/SaveLoad/移除Undo等Index245检查通过，源序交换两叶和tuple文本的末尾修补另行验证。此合同不代表另一工作树已采用、已合流或人工验收完成。





## 2026-10-20 LOGIC-BRANCH-EXEC-010 — 分支工具选择下游执行

原证据：安装 `BranchModule_STD.dll` SHA256 `47E5AE68E1744C0A899D70A7F98DB1723C1C4B921616B462D3B12D94A1F9AC73` 的 `Process 0x1800078f0` 和安装 `IfBranchModule.dll` SHA256 `4C01DB8B1BE38BEC2A7EBF3B0659EB0D95D532DBB6E9AE0E3BFCFDA147D844A8` 的 `Process 0x18000b0e0` 均实际读取当前 NextModuleList、选子集并 SetNextModuleListEx；后者还能根据每项 `_State=1/2` 区间判断选择。证据为 `docs/plugins/BRANCH-MODULE-SOURCE-RECOVERY.md`、`analysis/branch-module/source-20261020/native/1800078f0.{c,asm.txt}` 及 `analysis/next-batch/condition-native/IfBranchModule/output/18000b0e0.{c,-asm.tsv}`。原分支表 ID 与本工作树稳定 node ID 的映射尚未直接确认，不能用名称或 GUID 的 atoi 猜测。

生产者：本工作树共享 `ModuleExecutionContext` 提供当前真实后继的稳定 NodeId、有序连接身份及明确的整数 ID 适配；插件 `ModuleResult` 可选输出本轮选定的后继 NodeId 集合。消费者：本工作树 `GraphExecutor` 仅让选择集中的真实后继执行；`null` 保留旧工具行为，空集合抑制全部后继，非空子集过滤未选连接。输出范围必须验证属于当前真实连接，不允许插件凭空跳到别的节点。原 `GetNext/SetNext` 返回码、同一 ID 重复项、失败后清表与跨轮存储寿命需要分别按源码/运行证据确定，不能由这一最小合同自动宣称原厂等价。

载荷属于当前节点单次运行结果，配置中的分支行与节点身份须随连接变更、复制/粘贴、方案读回映射；不在宿主建立第二套工具配置壳。兼容性为末尾可选字段，现有非分支工具不提供时执行顺序不变。验收包括值/位/条件的选定与未选下游、空集、重复连接、非法 NodeId 拒绝、两轮不同选择及配置保存重开。

首个共享执行合同已落在 `VisionFramework.Contracts`/`GraphExecutor`：`ExecutionSuccessor(NodeId, NumericId?)`、上下文只读 `ExecutionSuccessors`、结果可选 `SelectedExecutionSuccessors`。当前 NumericId 为 `null`，绝不由 GUID/名称伪造原数值 ID；真实绑定/复制/方案读回、重复连接的原厂处理与专属工具尚未闭合。`BranchSelection.ContractChecks` 的 5 项真实图执行专项和 `VisionFramework.IntegrationChecks` 的 10 项既有回归均 exit0，独立 artifacts 分目录。通过这些测试只证明共享最小调度合同的当前实现，不证明两个原厂分支插件等价或整工具完成；另一项目采用由用户选择，不自动合流。

2026-10-20 补充：配置 `SelfRun` 的宿主原本构造零边的隔离图，分支工具读到的后继会错误为空。`ToolExecutionScope.RunConfiguredTool` 现从原流程执行边读取该节点真实后继，并经 `GraphExecutor.RunAsync` 的可选只读 `executionSuccessorRead` 传入执行上下文；隔离图本身仍无边，故仅当前节点运行，不会触发实际下游调度。只允许插件返回该可见集合中的 NodeId，越界按 `InvalidExecutionSuccessor` 失败。`BranchSelection.ContractChecks` 现 9 PASS，`VisionFramework.IntegrationChecks` 10 PASS，`VisionEditor` Release 构建 0 error（既有 MainWindow CS8603 警告 2 条），输出分别在 `analysis/next-batch/branch-selection-selfrun-artifacts`、`framework-selfrun-artifacts`、`selfrun-host-artifacts`。这是共享宿主合同验证；BranchModule 真实配置窗/保存复制及原厂运行差分仍是专属工具退出门。

2026-10-20 第二次补充：工具侧可选 `IToolExecutionTopologyProvider` 在真实增删执行边的事务内重写节点参数；Load/Undo/Redo 不合成原厂连接事件。可选 `IToolNodeCopyProvider` 在全部新 NodeId 和复制边形成后重映射参数，源来自 clipboard fragment，复制单节点时后继空集可清旧目标。宿主只调实现该接口的插件；Branch 专属实现按原 `ModuleOperate`/`Paste` 顺序处理行表，32 后继门在加入第 33 条时原子拒绝。`PortDescriptor.CanSubscribe` 默认 true，Branch 原 XML 的 `outputint`、`ModuStatus`、`BranchMatch` 显式 false；配置树枚举、绑定验证、显示候选与工作流来源候选消费此元数据。原 XML SHA256 `DB4179DED7075902BFEDFB1E2BC385801C0D7202F986E784EF438629A5FAB068`。真实 MainWindow Host15 PASS，Branch 生产 PluginChecks35 PASS，共享 WorkflowSubscription60 PASS；宿主 Release 0 error、现存 CS8603 两条。上述检查覆盖当前接线行为，不证明原厂全域运行/模板像素等价，跨项目采用仍由用户决定。

## 2026-10-20 LOGIC-IF-FAILED-OUTPUT-011 — 条件检测失败结果仍可呈现

原证据：`IfModule.dll` SHA256 `1594AD9652D50448A80133F51D1CCA21B944EA5CB0CD9EE4BE6864F4D2571A6B` 的 `Process 0x18000b0c0` 在非空条件且全部输入读取失败时返回 `E0000314`，但此前仍调用 `SetInt(ModuStatus/NIfResult)`、`SetString(StrResult)` 和动态输出辅助 `0x18000a170`。空条件 `E0000309`、非法区间 `E000030E` 则早退。源和 asm 位于 `analysis/next-batch/condition-native/IfModule/output/`，详见 `analysis/next-batch/A-control-script-identity-round1.md`。

生产者：插件侧 `ModuleResult.PreserveFailedOutputs` 默认为 false，仅持有明确原始失败发布证据的工具选择 true。消费者：本工作树 `GraphExecutor` 在选择后仍校验实际输出与可选动态 descriptor，并记录失败节点的输出、错误码与 native code；失败节点仍阻断下游，未选择的旧模块行为保持清空。`tests/BranchSelection.ContractChecks` 12 PASS 验证保留、阻断和默认回归；`tests/IfModule.TextChecks` 12 PASS 验证当前正常条件计算叶。结果字符串、动态端口和原厂真实宿主运行仍是 IfModule 专属未完门，当前共享合同不代表 IfModule 整工具完成或另一项目已采用。


## 2026-10-20 LOGIC-STRING-RESTORE-012 — 仅真实文件加载时恢复工具参数

原证据：`BranchStringCpmL` 托管配置链的 `Save→GetParam` 与 `Load→SetParam`；源位置和原件哈希见 `docs/plugins/BRANCH-STRING-RESTORE-CONTRACT.md`。原 `id#text#flag$` 串在保存和重新加载之间存在可观察的空字段分词差异，因此需要真实文件加载边界；不能在 Save、Undo、Redo 或一般配置编辑时自行归一化。

生产者：本工作树 `GraphEditorDocument.Load` 在读取、分离流程与提交文档之间调用可选 `PrepareLoad`；`ToolPluginRegistry.PrepareLoad` 仅对实现 `IToolNodeRestoreProvider` 的工具逐节点调用 `PrepareRestoredNode`。消费者：BranchString 专属参数恢复逻辑。提供者不得更改节点身份，返回结果须通过方案结构校验；抛错时加载不提交文档、路径、修订号或变更事件。无提供者的旧工具原样加载。

生命周期：只在明确的磁盘文件 Load 调用一次；Save、Undo/Redo、配置页重开与节点复制不触发。该合同不建立原厂 numeric ModuleId 与当前 NodeId 的伪映射，也不代替专属工具对原 0x400 字节保存容量、空字段及运行长度门的源码取证。共享合同已通过 `BranchSelection.ContractChecks` 15 项（含成功一次与失败回滚），专属实现、真实宿主复验及整工具自审仍待完成。主框架若选择消费，需要逐文件审查并重新编译；通知不代表跨项目合流或人工验收。


## 2026-10-20 LOGIC-BRANCH-DEBUG-013 — 单节点调试复用真实后继与作用域

生产者：`NodeDebugExecutor.RunAsync` 的原流程 `ProcedureGraph.ExecutionEdges` 和可选 `ModuleExecutionScope`；消费者：BranchString/BranchModule 的当前运行节点，以及 `ToolEditorHost` 调试后 `LastDisplay`。原分支 Process 读当前后继表，即使独立 SelfRun 的隔离执行图不包含下游节点，分支仍需看到原后继身份。`GraphExecutor` 的 `executionSuccessorRead` 仅供本节点读取与验证选择集，不调度隔离图外的节点；同一真实 procedure scope 传给运行与显示，宿主在异步调用期间持有 owner lease。

兼容：旧 `NodeDebugExecutor.RunAsync` 调用不传 scope 时保持 null；后继透传仅来自原流程真实执行边，未提供 native numeric ID，不推断 GUID 到数字的映射。测试增加 `BranchSelection.ContractChecks` 的独立 debug 用例；分支专属插件与真实宿主复验由 B 线串行构建后记录。接口仍为本工作树共享实现，主项目是否采用由用户决定，不自动合流。

## 2026-10-20 LOCATION-FIXTURE-TRANSPORT-014 — 零尺度位置修正值与安全仿射消费

原证据：`IMVSFixtureModu.dll` 原 Process 的 X/Y Scale 比较汇编 `COMISS scale,10000; JA` 与 `COMISS 0,scale; JBE` 接受 0、-0、10000，负数和大于10000 返回 `E000032E`；`FixtureInit` 与保存/重开链可能产生参考尺度 0。原件哈希、寄存器/字段链见 `docs/plugins/FIXTURE-SOURCE-RECOVERY.md` 与 `analysis/fixture/proposals/fixture-transport-contract.md`。NaN 的原运行行为仍未执行证实。

生产者：本工作树 `VisionPlugins.Fixture` 输出 `FixtureValue(Reference,Run)`；消费者：本工作树 `PortValue.Fixture`、FixtureArray/Element/JoinArray、工具显示及继承 ROI/ImageBinary。已增加 `FixturePose/FixtureValue.ValidateTransport`，只要求所有字段有限且两个尺度不为负，`PortValue.Fixture` 以它验证运输。既有 `Validate` 与 `Transform/WarpMask` 保持严格正尺度；0 可在图与方案间传递，但实际需要求逆的仿射消费者明确拒绝，不产生 NaN 或隐式填 1。这里的有限性要求是独立安全选择，不冒称原生谓词拒 NaN。

兼容：原所有正尺度值和既有仿射结果保持；插件专属 Runtime 自行执行原 0..10000 范围门，公共运输不强加 10000 上界。应验收 0/负/非有限运输、FixtureArray/Element/JoinArray 保值、仿射拒 0、正尺度 ROI 回归及真实 Fixture 工具宿主保存重开。当前共享源码已落，B 线专属与宿主构建仍在进行；主框架项目是否采用由用户决定，不自动合流。
## FCR-031 — IMAGE 来源候选的前驱、容器与实际图像格式合同（2026-10-20，待用户选择合流）

原厂依据：安装版 `VM.Core.dll` SHA-256 `936763F6C8A1BF9FCF59F22ACEBCA8FEB09B5F3D5701AE77C82BDC81E626190D`；`VmModule.GetFrontIOWithLeafItemsByName(string)` 的原始 CIL 位于 `analysis/joint-audit/VM.Core.GetFrontIOWithLeafItemsByName.il`，并与 `analysis/decompiled/VM.Core/VM.Core/VmModule.cs:3702-3721,3953-3997,4049-4075` 对照。完整 `VmModule`/`VmIO` CIL 与哈希见 `docs/joint-audit/20261020-pass2-original-image-chain.md`。前驱首次访问经 `GetMultiModuleConnect` 从 SDK 懒加载，同容器连线增删由 `NextModules.CollectionChanged` 经 `ConnectOneModule/DisConnectOneModule` 同步目标 `PreModules`，撤销/重做也操作源集合。候选再加入非 Solution 父模块、ID 13000 全局变量及当前/父容器局部变量；父模块查 Inputs，其余查 Outputs；递归查非叶且 TypeName 相同的组合项，最后筛 `CanSubscribeA && Visible` 并返回 `ModulePathName`。原 `Group.xml` 的 `ImageSourceGroup` 与掩膜输入均用 `IMAGE` 候选查询。`Visible` 有属性通知，`CanSubscribeA` setter 无属性通知；候选 UI 何时重查、跨层动态模块及最终排序仍未证。候选方法不检查图像格式叶，故 RGB/MONO8 的实际接收或转换必须单独取原生运行证据。

生产者：主框架的流程拓扑、Group/父容器、全局/局部变量、各节点实际输出种类与身份。消费者：插件侧 `WorkflowConfigurationSubscriptionService`/配置模型和图像源、二值化、轮廓匹配等 IMAGE 输入编辑器。载荷需要稳定的源节点/端口/容器身份、原生组合类型、`CanSubscribe`/`Visible` 状态、实际像素格式能力及失效通知；展示文字不得反向解析成身份。**两条当前入口不可混同**：插件侧 `WorkflowConfigurationSubscriptionService` 桥只枚举 root 流程显式 `ExecutionLinks` 的启用前驱；主框架 `EditToolInput → WorkflowSources` 却能枚举流程内全部兼容输出、仅排当前节点。真实 A→B、C 独立的 `tests/WorkflowCandidateScope.Checks` 四项探针确认通用来源表列 A+C、类型化桥只列 A，提交 C→B 被 `GraphValidator` 的 `DataExecutionOrder` 拒绝且旧连线保留，见 `docs/joint-audit/20261020-pass5-candidate-scope-probe.md`。后二者的源代码对账见 `docs/joint-audit/20261020-pass5-binary-ui-contract.md`；它们均不覆盖原版全局/局部/父容器及 `CanSubscribeA/Visible` 元数据。这是已确认的**入口不一致和覆盖差异**，不意味着可直接开放所有范围。

生命周期：候选初始化、窗口保持打开时的连接/断开与可见性变更、前驱启停或动态输出变化、Undo/Redo、保存重开、切换 Group/方案与关闭释放；初始化集合与实时刷新分开核验，旧异步结果不可写入新方案。兼容性：保留现有根流程 typed 引用和已存绑定；原 `ImageData`/`ImageDataMono8` 是两个输出组合，本地 `Image`/`ImageMono8` 是映射，不将候选存在等同于像素能力，也不默默自动转灰度。受控同版 SDK 探针在 20×16 样本中已证实际 Mono8 五模式均成功、RGB24 五模式均返回 `MVALG_STS_ERR_IMG_FORMAT=0x10000007`，见 `docs/joint-audit/20261020-pass5-binary-core.md` 与原始 CSV SHA-256 `55CBAD3C3DEAA4C6A59AB4BE263F9B1AC02CB1FE51448FBD4FDDB6431876A79D`；这不覆盖全部像素格式及原桌面状态。接受证据：同一图像源→二值化→实际下游链的 Gray/RGB/`ImageMono8` 四状态，分别记录候选、选值/持久化、`PixelFormat`、执行与失败释放；再核 Group/变量及保存重开矩阵。当前四状态图执行和编辑器保存重开检查见 `tests/FrameworkPluginChain.Checks/Program.cs`；原生**五模式像素公式**、实时 UI 刷新与跨容器候选仍缺，故请求状态为待证据/待用户选择合流，未修改 `tool-plugins`。

## FCR-032 — 可选工具输出跨 Group 的缺席状态合同（2026-10-20，待原版执行证据）

生产者：工具插件运行结果及端口元数据，首个实证为 `vision.image-binary` 的可选 `OutMask`；消费者：主框架 `WorkflowRunContext.Complete`、owned Group 返回值、父流程真实下游。载荷须区分端口已声明、此次已发布及缺席时的原因，不能把上轮旧值当本轮输出。生命周期覆盖开关切换、失败/NoMatch、Group 内运行、父流程消费、保存重开与取消释放。

当前七态探针 `tests/FrameworkImageTypeGroup.Checks` 已证：Binary 的 `OutputMaskImage=false` 时仍成功发布普通 `Image`，但 Group 同时声明 `MaskResult ← OutMask` 会因该可选值缺席整体 `SubprogramFailed`，普通 `Result` 的父流程消费者也被跳过；开启后两路成功。日志与 SHA 见 `docs/joint-audit/20261020-pass5-group-image-type.md`。原厂 `IMVSBinaryModu.xml` 把 OutMask 叶设 `IsForce=false`；同版 `VmProcedure.SetIoParams → GetGroupIoList → AddFilterToList → ProcessManager.SetGroupIO` 的 CIL 及 SDK 结构却只向 Group IO 写类型号/名称，不传 `IsForce`，故**尚不能**凭 XML 直接裁定原版 Group 缺席值是整体失败还是局部可用。原版 Group/SDK 在开关关、开、失败后的实际输出和状态是解除条件。

兼容性：当前 Group 声明输出视为必需，保存格式无可选性字段；不得无证据将所有 Group 输出改为可选或吞掉真实失败。验收需同一 ImageSource→Binary→Group→父流程 Binary 链测普通图、可选 OutMask、失败与重开，并分别核原版与当前状态；需要扩展共享输出合同或插件端口元数据时由唯一所有者实施，用户决定跨项目合流。本条仅登记合同风险，不采用插件工作树在制代码。

## FCR-033 — N 点标定点表初始化与图文档历史写入边界（2026-10-20，待插件线裁定）

生产者：标定工具专属 `NPointConfigurationPointEditor` 的逐字段初始化和编辑回调。消费者：共享配置宿主 `ToolConfigurationHost.Apply` 及主框架 `GraphEditorDocument.SetProcedureNodeParameters` 的参数、撤销历史和后续运行。载荷为 32×5 点字段及当前点数/行 ID；字段成功/失败与文档版本必须保持一致，不可把初始化读回误当用户编辑。

原版 `IMVSNPointCalibModuControl.NPointList.Init` 每行取五个原参数并给 `PointList` 五属性，setter 写对应 `IUserStringData` 字段；当前也逐字段写，但每格均复制整份 `PointValues`、同步 160 键并调用图文档编辑。`tests/NPointEditCost.Checks` 的真实配置＋图文档探针证开 4/9/32 点表分别调用 20/45/160 次 host Apply，单格修改各一次；原始成员、当前路径、耗时边界及哈希见 `docs/joint-audit/20261020-pass6-npoint-ui.md`。这是已证调用放大，不是原版与当前的同机时延差分，也未归因用户长期卡死。

生命周期与兼容：首次打开、再次打开、字段失效/写失败、导入/清空/删除、修改后撤销重做与保存重开均需保留原字段写入副作用及当前图历史语义。所需合同是区分**初值投影**与**用户修改**，以便插件配置层决定何时触发共享文档 Apply；主框架不能无据吞掉编辑事件或修改所有工具的历史策略。接受证据应包含原 setter 成功/失败指令、4/9/32 点真实主窗口 Apply/历史/重开矩阵及逐字段值，性能优化前后均回归。插件配置与点表为 `tool-plugins` 所有，本工作树只登记请求，不改其在制文件；跨项目采用由用户决定。

第十轮追加：`tests/CalibrationNPointEditChain.Checks` 以真实点编辑器和与产品相同的回调算法写真实图文档，实测四行初始化 20 次 `Apply/Changed/revision`、单格 +1，Undo/Redo、保存重开、N 点求解→XML→标定加载通过。主窗口源码的 `doc.Changed→RefreshAll→GraphValidator/RenderGraph/RefreshRight` 将逐次文档变更放大为逐次图刷新；这是调用链证据，不是已经实测的用户桌面耗时或长期未响应根因。原版 `NPointList.Init/PointList` setter 逐叶写，native `SetParam @0x3e560` 分派五字段；不能简单删去初始化 setter。详细证据和动态 `CreateCalibTime` XML 哈希界限见 `docs/joint-audit/20261020-pass10-npoint-edit-chain.md`。若后续采用窄批次/刷新合并合同，仍需保留每叶成功/失败与顺序，其他工具退回原 Apply；本条继续保持待用户选择，未实施跨项目代码。

第十一轮补充真实 `MainWindow` 按钮路径：`tests/CalibrationNPointMainWindowCost.Checks` 在实际配置窗口内打开 4/12/32 行点表，测得图文档 `Revision/Changed` 分别 +20/+60/+160，改单格各 +1；最终 exit 0，离屏模态调用样本为 851.7/1948.3/3463.2 ms。当前同步调用放大已不依赖同构回调推断，证据见 `docs/joint-audit/20261020-pass11-npoint-mainwindow-cost.md`，日志 SHA-256 `85D8940022B6495D9A5899C067F272A6B1291F471B2DA51D40E8D60FA9D3AAF6`。这不是原厂同机差分或用户长期卡死根因，插件/主框架合同仍待跨项目裁定。

## FCR-034 — 畸变标定失败轮的显示快照归属（2026-10-20，待原版同条件证据）

生产者：`DistortionCalibrationPlugin` 的同节点执行 scope、`LastDisplay` 图像层/图形与失败状态。消费者：main 的 `NodeRunRecord`、`RefreshToolDisplay` 和图像/记录/结果列表 presenter。载荷为原图、`PostCorrectionImage` 对应显示层 `PostCorrectionImageData`、170 个图形及 `ModuStatus/ModuRunTime`；框架公开的失败轮输出可以为空，插件 owned display 仍可能持有上一成功轮的图。

第十二轮真实离屏 MainWindow 图先成功、再改自动导出扩展名为 `.bad` 导致该节点 `Failed`：当前显示仍有两图层和 170 图形，失败记录文本已更新；切换到图像源节点后 presenter 图/记录/列表清空。标定板的空白 `NoMatch` 则保持本轮输入图、清掉旧检测点图形。原 `IMVSImageCalibModuDisplay.xml` 将校正图映射为 `PostCorrectionImageData`，原 `ProcessOutput` 可非原子发布；但未证同一导出失败后原厂是否保留旧图。证据、原件哈希和运行日志见 `docs/joint-audit/20261020-pass12-board-distortion-display.md`；根线独立复跑日志 SHA-256 `0DA4B51325A5A0AFFBEFA6FB8629F5D858F18FAECCA2E4C29A5CBA4E56AF16B2`。

生命周期和兼容裁决：不得只因 `NodeRunRecord.Failed` 就在 main 清所有插件显示，也不得把 retained 图形无证据称为原厂行为。需要原版同输入“成功→自动导出失败→切节点”的 `ProcessOutput/GetDisplayList` 结果与可见选层，再由插件显示快照所有者决定保留、标注旧轮或清除的具体条件；main 只按约定显示该快照和当前运行状态。接受证据应含同节点连续两轮、切选、切方案、关闭、重开和失败后再成功的实际 WPF 显示/释放回归。此条仅登记跨项目合同风险，不修改 `tool-plugins` 在制文件或擅自修复。

## FCR-035 — 旋转标定 IMAGE 强制叶与聚合端口的声明合同（2026-10-20，待插件所有者补证）

原版生产者：安装 `RotateCalibModu.xml` 的 `InputImage` 组合及 `InImage/InImageWidth/InImageHeight/InImagePixelFormat` 四个 `IsForce=true` 叶；`RotateCalibModuDisplay.xml` 从 `InImage` 展示。当前生产者：ImageSource/图连接；消费者：`vision.rotate-calibration` Descriptor/Runtime/`LastDisplay`、main 连线验证和图像 presenter。当前 Descriptor 只暴露可选聚合 `InputImage`，显示从 scope 中的聚合图快照取得；这不是原四叶端口的逐口重建。

载荷与生命周期：应逐项核组合 IMAGE 与四叶的源身份、宽高、格式、断开/重连、无图运行、失败后本轮图/旧数值清理、Group/保存重开及取消释放。第十四轮源码及真实图探针见 `docs/joint-audit/20261020-pass14-c-display-chain.md`；根线 9/9 证当前真图首轮、非法 `RotNum=2` 失败、同 scope 恢复与未运行节点快照隔离。原 `Process 0x376b0` 未见读取图，故 `IsForce=true` 只能证**声明/绑定合同差异**，不能据此断言原算法无图拒绝或当前运算数值错误。原版无图条件的上游 validator/SDK 路径仍需原指令或受控原机证据。

责任：插件项目持有工具 Descriptor、配置/显示适配及复合输入契约，main 仅按已确认的端口类型/必需性执行连线与调度；如需跨项目改公共复合端口合同，先由用户选择合流点。兼容需保留现有方案中聚合 `InputImage` 连接或明确迁移路径，验收以真实 ImageSource→Rotate 的连线/执行/显示/Group/重开正负例及原同条件差分为准。此项仅登记，未修改 `tool-plugins` 在制文件或建立第二套插件配置框架。

## FCR-036 — 标定加载有限值失败后的记录快照归属（2026-10-20，待插件所有者核证）

生产者：`ReadCalibFilePlugin.Runtime.ExecuteAsync` 的文件解析、有限值安全拒绝和 `State.LastOutputs`；消费者：`ReadCalibFilePlugin.LastDisplay` 的 Data Record 以及 main `RefreshToolDisplay` 在失败 `NodeRunRecord` 下仍询问的插件显示帧。载荷需携带本轮状态、动态输出 schema/值、错误码与快照所属运行轮次；不能让上一轮的 `ModuStatus=1` 被误读成本轮成功。原 `ReadCalibFileModuDisplay.xml` 仅声明 `Data Record` 从 `ModuStatus` 取值，原 native NaN 和失败后 WPF 记录值尚未取证。

第十六轮真实 owned Group 探针从合法九项矩阵，经畸形刷新保留、合法空输出状态0、动态叶改名和恢复后，读取含 `NaN` 的 XML：当前 loader 报 `INVALID_CALIBRATION_NUMBER`、Group `SubprogramFailed`，但 `LastDisplay.RecordText` 仍为上轮 `Module Status:1`。原 Runtime 的有限值错误早退没有更新 `LastOutputs`，main 失败记录仍允许拉 LastDisplay；这是**当前缓存/显示归属差异**，不等于原版应清空或显示某个特定文本。原源码、当前方法、完整 12 项探针及日志见 `docs/joint-audit/20261020-pass16-readcalib-display-chain.md`。

生命周期与兼容：同节点合法→畸形→无输出→改名→恢复→非有限拒绝→再次恢复，另核切节点、切方案、Group、重开和取消时的快照 owner。插件侧决定失败轮显示是否用本轮错误 DTO、无帧或保留并标注旧帧，main 只呈现有明确轮次/状态的快照；如公共显示合同需扩展 owner/epoch，单一共享写入者先提接口并由用户决定合流。不得无原版证据把所有失败帧一律清空；此项只登记，不修改 `tool-plugins` 在制文件。
