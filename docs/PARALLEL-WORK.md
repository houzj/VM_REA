# Two-line reconstruction workflow

## Workspaces
- Framework/integration: original checkout, branch main.
- Tool plugins: worktrees/tool-plugins, branch tool-plugins.
- main: active framework development and integration. The user explicitly chose main for the original checkout. The old framework branch is historical; do not continue development there. Integration is performed by the framework conversation after reviewing plugin commits.

Use two long-lived Codex conversations, each opened in its own directory. Conversations do not automatically synchronize; source commits and these documents are the handoff record. Creating these branches does not create or message a conversation.

## Ownership
Framework: shell, workflow tabs/canvas/navigation, Group ownership/loops/accumulation, scheduling, persistence, variables/subscriptions, shared contracts, plugin discovery and lifecycle.
Plugins: catalog and dependency inventory, per-tool source audit, algorithm reimplementation, parameter/configuration views, dedicated tool tests. Begin with image source as the integration sample.

The current plugin architecture is not yet a stable complete native-equivalent interface. Inventory existing contracts before extending them. Shared interfaces must cover identity/version/category/assets, typed scalar/array I/O, parameters, execution/errors/cancel/timeout, configuration lifecycle, preview/ROI services and serialization.

## Integration
1. Record original evidence paths and exact types/methods/resources.
2. List confirmed, reconstructed and unknown behaviors separately.
3. Implement real data execution as well as UI; keep capture code in tests.
4. Commit one reviewable feature and record verification/results/limitations in docs/HANDOFF.md.
5. Framework line integrates selected commits, resolves shared contracts, tests end-to-end and publishes.

Do not simultaneously update shared files from both branches. Put interface needs in CONTRACT-REQUESTS.md with caller, payload, lifecycle and evidence. Do not merge/push integration changes from the plugin line without the agreed handoff.

## Current known gaps

Current framework status is maintained in docs/FRAMEWORK-REVERSE-CHECKLIST.md and docs/framework-acceptance.html (2026-10-17). Owned Group navigation, range loops, primitive break/accumulation, input editing, categories/multi-target synchronization, dedicated two-pane configuration, graphical ROI and real image-source/binary-tool integration have implemented subsets. They are not full VM parity. Open scope includes native signed-step/overflow differential behavior, dynamic indexes/composite data, per-round native lifetime, upstream preparation/callback/device semantics, complete original control templates and actual VM differential acceptance. Cross-top-level calls remain compatibility behavior. Full .sol/API/ABI and all plugin coverage remain open. The detailed table separates independent implementation, original evidence and unverified native behavior.

## Local evidence and reports
Original evidence remains in the original checkout's analysis directory. It is excluded from remote history. Build-required UI assets and sample image are tracked. Store new reports/build outputs inside the active worktree, not the other conversation's directory. Reports that require original binary evidence are not portable solely from Git.

## Group configuration/ROI update

Source-guided two-pane runtime configuration and graphical ROI infrastructure now run in main; scoped image-source full editor and actual Group execution are integrated. See GROUP-NATIVE-EDITORS.md and FCR-007. A real hard-threshold binary tool is now integrated as an ROI consumer (FCR-008/009). This does not complete native complex UI, all algorithm modes, Fixture native transformation or full ROI behavior parity. No plugin worktree changes were made.

## Workflow context closure

2026-10-17: see WORKFLOW-TAB-TOOLBOX-GROUP-CLOSURE.md for source-confirmed Tab/configuration dispatch, live-source clipboard, toolbox popup/depth and actual Group geometry. Latest acceptance contains364 WPF event operations and actual image runtime evidence; native desktop differential parity remains open.

Group loop update: GROUP-LOOP-BOUNDARIES.md records source-guided LoopStep and actual image/break/cancellation results. The original XML has no step widget. Latest378 event operations include clearing stale successful-restart status; full native parity remains open.
