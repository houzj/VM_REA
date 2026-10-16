# Group loop range reconstruction

Date: 2026-10-16. Scope: owned Group range execution and private LoopIndex. This is a partial implementation, not full Group parity.

## Original evidence

- `Applications/Module(sp)/x64/Logic/IMVSGroup/IMVSGroupAlgorithm.xml`: EnableLoop=false, StartIndex=0, EndIndex=1, LoopTimeGap=0.
- `IMVSGroupAlgorithmTab.xml`: start/end use ButtonSelecter subscriptions; interval 0..10000 ms; enable controls visibility. Break controls exist but are not implemented in this change.
- `IMVSGroup.xml`: LoopIndex is an int private output, hidden from the result tree; comment restricts it to internal modules.
- `analysis/decompiled/Apps.XmlParser/Apps.XmlParser.ParamTab.VmModuleExtern.decompiled.cs`, GetOutputTreeNode: adds Group LoopIndex only when the consumer's parent is that Group and requested type includes int.
- `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs`, MModuleManager_OnModuleResultCallBack: final-iteration condition `LoopIndex + 1 == EndIndex` (also recognizes native status 5/2). This alone does not establish the complete native scheduler.
- Native `Applications/ModuleProxy/x64/ModuleFrame.dll`, SHA256 `45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b`. Ghidra 12.1.4 static pseudocode:
  - `0x1800a0930`, CGroupNode::DoWork: resets default range to 0..1; enabled loops resolve range; checks `index < end`; updates index, checks break before body dispatch, prepares output and waits interval before incrementing. Default unit step is implemented; other LoopStep behavior is not.
  - `0x1800a53a0`, GetLoopRangeFromSubRecord: resolves StartIndex/EndIndex from native subscription records.
  - `0x1800a55b0`, IsBreakLoop: enable gate, first-iteration dependency guard, typed comparisons. Not yet implemented; pseudocode types/precision require instruction corroboration.
  - `0x1800a6730`: first-iteration break-source scope analysis. Not fully mapped.
  - `0x1800a8040`: identified as ReportPointSetResultWhenLoop, **not** the general accumulated-output algorithm. Do not use it as proof of primitive accumulation.

Local retained evidence and reproducible addresses: `analysis/full-audit/group-native-audit/`. Native analysis was isolated from the plugin conversation. Initial provider configuration/sandbox failures are retained separately; the configured elevated analysis succeeded. No original binary was executed by this analysis.

## Implemented behavior

- Group range executes actual internal modules, start inclusive/end exclusive, default one-shot index zero with loop disabled. Equal/reversed ranges execute no body; no descending loop is invented.
- Parent typed parameter subscriptions are resolved before invocation. Native start/end inputs are represented as framework parameters with subscription metadata; this is an independent host representation, not the native ABI.
- GroupLoopIndex is a separate source kind, validated only in an owned Group body, scalar integer, with no public port. Nested Groups have independent invocation indices. Debug execution can use an explicit cached index snapshot; absence does not fabricate a value.
- Interval uses cancellable Task.Delay, including after the final successful iteration, following the observed placement. Parent cooperative cancel/timeout remains effective.
- WPF owned Group loop tab has enable, start/end subscription selectors and interval validation. Legacy cross-top-level calls retain one-shot behavior and cannot enable this tab. Existing Group entry/back navigation remains.
- Last iteration's ordinary result is returned; no accumulation is claimed. Runtime diagnostics record completed iteration count and last invocation index.

## Independent policies and open boundaries

- Stop on failed child, 10000-iteration resource guard, managed task scheduling and final-result publication are independent policies. Native error/timeout/callback behavior is not reproduced in full.
- No break condition runtime, source-scope guard, native LoopStep, accumulated outputs/displays, intermediate callback stream, native image merge exceptions or device cleanup equivalence yet.
- Per-iteration local contexts are fresh in this runtime. Native local-variable mutation/lifetime across iterations still requires tracing.
- The WPF page uses current shared templates, a CheckBox and interval TextBox. It is functionally checked, **not** a pixel-identical reproduction of the original Boolean/Integer templates. No real VM side-by-side or differential run was performed.
- Empty ranges with required public outputs fail ordinary missing-output validation instead of inventing values; native empty-range output/status remains unverified.
- Existing Accumulate configuration remains an open item; users must not interpret its persistence as working accumulation.

## Verification

17 runtime checks: actual index sequence 2,3,4; disabled/signed/empty/reversed ranges; private scope rejection; missing invocation context; parent variable range; nested isolation; cancellation/timeout; debug index snapshot; persistence and undo.

Actual WPF checks exercise the owned Group window, switch and end-selector choice; test-only RenderTargetBitmap output is `analysis/full-audit/wpf-group-loop-range.png`. This image is our program's render, not a screenshot of VM. Final counts and release lifecycle results are recorded in HANDOFF.md.
