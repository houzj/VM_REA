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
