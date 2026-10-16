# Integration handoff

Initial baseline: existing independent reconstruction, including owned Group navigation and Tab return fix. Not complete VM parity.

For each handoff record: branch/commit, source evidence, changed tool or framework behavior, real UI/run verification, remaining gaps, and shared contract requirements.

## 2026-10-16 — F-01 typed primitive arrays / F-02 source audit

- Scope: Scalar/Array metadata with scalar defaults; immutable integer/number/text array values; registration, connection, subscription, workflow input/output, Group signature, runtime output and persistence validation. Main result formatter displays array elements.
- Evidence: VM.Core.DynamicOutput.GetOutputInt/Float/String; IMVSGroupCs.GroupResult; VmIODecorate report modes; DynamicParamsTabControl self-execution command setters. Details and boundaries in FRAMEWORK-CONTRACTS.md; hashes and local HTML in analysis/full-audit/array-contract-source-files.json and framework-contracts.html.
- Verification: 28 array-contract checks (including owned Group array → downstream sum 12 and save/reopen); 115 editor core, 34 Group calls, 31 variable/data, 38 execution, 22 module checks; 244 actual WPF checks. Release lifecycle: core/plugin/WPF/editor all exited with code0. These are independent runtime checks, not VM loop/ABI equivalence.
- Release: dist/VisionEditor-WpfArrayContract. Screenshot code remains in tests. No plugin worktree or tool implementation was changed.
- Compatibility: plugin interfaces untouched; scalar callers unchanged. New Cardinality JSON fields require the new strict reader; do not cherry-pick regenerated sample files alone into an older engine.
- Open: native numeric ABI/precision, remaining types, full array selection UI, loop/break/LoopIndex, actual accumulation behavior, debug-session lifetime. F-01 and F-02 remain open overall. Existing host editor Run still executes owning workflow; self-command native scheduling needs further tracing before changing it.

## 2026-10-16 — main branch policy

At the user's request, the original checkout now develops framework and integration directly on main. The former framework commits were fast-forwarded without rewriting history. tool-plugins remains isolated and was not modified. Follow main for current shared contracts; framework is a historical branch.

## 2026-10-16 — F-02 ordinary WPF tool debug sessions

- Source chain recovered through DynamicParamsTabControl → BottomCommand → VmModule.Run → ModuleManager.SelfRun → IMVS_SelfExecuteOneModule(moduleId), plus callback-gated continuous execution and parameter-window shutdown disposal paths. Details: DEBUG-SESSION-CONTRACT.md.
- Implemented: NodeDebugExecutor; ToolDebugSession single/continuous/stop/dispose; optional ToolEditorContext.Session; WPF Run adapter uses isolated module execution with explicit cached inputs. ExecutionSessionResult preserves actual invocation input snapshots for internal Group-style tool inputs.
- Host behavior: clears target/downstream stale results, updates actual image preview, prevents concurrent workflow run while module debugging, waits for module disposal before editor or main window shutdown. Screenshot helpers and slow cleanup-gate modules remain in tests.
- Verification: 21 debug runtime checks; 253 actual WPF checks including a real image-source window with incomplete downstream workflow and controlled active-runtime close; 28 array, 34 Group-call, 31 data, 115 core checks. Final core/plugin/WPF/editor-close lifecycle checks all exit0.
- Release: dist/VisionEditor-WpfDebugSession. Startup entry updated only after final checks.
- Compatibility: old ToolEditorContext constructor and Run delegate retained; no plugin implementation or plugin worktree changed. Context/host UI APIs must be invoked on the WPF dispatcher.
- Remaining boundaries: native upstream preparation/first-run behavior, native in-flight cancellation and device cleanup are not reconstructed; explicit cached-input/cooperative-cancel/failure-stop policies are independent. Group configuration run remains separate; Group loop/break/accumulation and full plugin debug UI remain open. This supersedes the earlier ordinary-WPF-host whole-workflow Run gap, not every legacy WinForms or Group path.
