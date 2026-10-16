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
