# Shared contract requests

Each request identifies source evidence, caller, typed payload, lifecycle, compatibility and acceptance. Framework line owns shared implementation. Plugin line is already working independently; this file does not represent unreceived requests or automatically synchronize its worktree.

## FCR-001 — typed primitive arrays (framework-originated)

- Owner: framework; status: runtime foundation implemented, verification/integration tracked in HANDOFF.
- Consumers: Group accumulated outputs and future algorithm plugins. This is a framework need; no plugin-line request is claimed.
- Evidence: VM.Core.DynamicOutput.GetOutputInt/Float/String, IMVSGroupCs.GroupResult, VmIODecorate.Get/SetGroupIOReportMode. See FRAMEWORK-CONTRACTS.md.
- Payload: primitive DataKind plus Scalar/Array cardinality; immutable long/double/string list values; empty differs from missing; no scalar coercion.
- Compatibility: existing scalar defaults/constructors retained. New array JSON requires the new reader; native binary ABI/precision is not implemented.
- Accepted scope: registry/type validation, execution, subscriptions, owned Group I/O, persistence and readable output display. Group looping, accumulation semantics and full array selector UI remain open.

## FCR-002 — WPF debug session lifecycle (framework-originated, under source analysis)

- Owner: framework; status: source entrypoints identified, API not frozen/implemented.
- Evidence: DynamicParamsTabControl command setters SelfExcuteModule / ContinuousSelfExcuteModule / StopSelfExcuteModule and disposal path.
- Current gap: editor factory exists, but old contract includes Form; host Run executes whole owning flow and there is no unified per-editor cancellation/continuous/stop-and-await service.
- Required next analysis: trace actual native execution semantics, dependencies, config commit timing, image/event ownership and closing behavior. Names alone are not proof of scheduling semantics.
- Compatibility policy: keep current plugin factories usable until a reviewed session interface and adapter are actually wired/tested; do not ask plugin line to guess an unstable interface.
