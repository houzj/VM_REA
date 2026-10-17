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

