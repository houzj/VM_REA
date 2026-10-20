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

- WorkflowConfiguration.RunCategories is a separate ordered category list; WorkflowRunParameter.CategoryId and Relations use stable IDs and typed input/parameter targets. Null fields omitted for old JSON. Snapshot/copy/delete remap all related targets. Native default maximum32 recovered from ParamConfigDataBase (native config can override; independent default32 currently enforced).
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
