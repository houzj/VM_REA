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
