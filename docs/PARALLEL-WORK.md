# Two-line reconstruction workflow

## Workspaces
- Framework/integration: original checkout, branch main.
- Tool plugins: worktrees/tool-plugins, branch tool-plugins.
- main: tool-independent framework development. The user explicitly chose main for the original checkout. The old framework branch is historical; do not continue development there. The user controls when independent work is integrated; main performs final integration only on that instruction.

Use two long-lived Codex conversations, each opened in its own directory. Conversations do not automatically synchronize; source commits and these documents are the handoff record. Creating these branches does not create or message a conversation.

## Ownership
Framework/main: shell, workflow tabs/canvas/navigation, Group ownership/loops/accumulation, solution/procedure scheduling and persistence, variables/subscriptions at the workflow boundary, tool-independent data contracts. It provides stable integration seams and consumes plugin capabilities, without developing a second plugin configuration system.
Plugins/tool-plugins: catalog and dependency inventory, per-tool source audit and algorithms **plus the plugin-facing framework**: common configuration model, template catalog, ordinary/dedicated editor host, parameter and ROI interaction, tool execution/debug sessions, display adapters, tool-specific persistence, and dedicated tests. Each tool task must name the common plugin-framework capability it requires and verify the tool through that capability in the real host.

The current plugin architecture is not yet a stable complete native-equivalent interface. Inventory existing contracts before extending them. Shared interfaces must cover identity/version/category/assets, typed scalar/array I/O, parameters, execution/errors/cancel/timeout, configuration lifecycle, preview/ROI services and serialization.

## Integration
1. Record original evidence paths and exact types/methods/resources.
2. List confirmed, reconstructed and unknown behaviors separately.
3. Implement real data execution as well as UI; keep capture code in tests.
4. Commit one reviewable feature and record verification/results/limitations in docs/HANDOFF.md.
5. After the user chooses a handoff, main reviews selected committed changes, resolves shared contracts, tests end-to-end and publishes. Neither project treats the other's dirty working tree as an integration artifact.

Do not simultaneously update the same shared file from both branches. Put interface needs in CONTRACT-REQUESTS.md with caller, payload, lifecycle and evidence. The single feature owner may edit shared file paths in its own worktree; the other project only records the request until the handoff. Do not merge/push integration changes from the plugin line without the user's chosen handoff.

The authoritative role, file ownership and baseline state are in [PLUGIN-FRAMEWORK-BOUNDARY.md](PLUGIN-FRAMEWORK-BOUNDARY.md) and [TWO-PROJECT-BASELINE.md](TWO-PROJECT-BASELINE.md). Historical sections below record implementation history, not a standing order to direct the other project.

## Current known gaps

Current framework status is maintained in docs/FRAMEWORK-REVERSE-CHECKLIST.md and docs/framework-acceptance.html (2026-10-17). Owned Group navigation, range loops, primitive break/accumulation, input editing, categories/multi-target synchronization, dedicated two-pane configuration, graphical ROI and real image-source/binary-tool integration have implemented subsets. They are not full VM parity. Open scope includes native signed-step/overflow differential behavior, general plugin index editors/native compound field trees, per-round native lifetime, upstream preparation/callback/device semantics, complete original control templates and actual VM differential acceptance. Cross-top-level calls remain compatibility behavior. Full .sol/API/ABI and all plugin coverage remain open. The detailed table separates independent implementation, original evidence and unverified native behavior.

## Local evidence and reports
Original evidence remains in the original checkout's analysis directory. It is excluded from remote history. Build-required UI assets and sample image are tracked. Store new reports/build outputs inside the active worktree, not the other conversation's directory. Reports that require original binary evidence are not portable solely from Git.

## Group configuration/ROI update

Source-guided two-pane runtime configuration and graphical ROI infrastructure now run in main; scoped image-source full editor and actual Group execution are integrated. See GROUP-NATIVE-EDITORS.md and FCR-007. A real hard-threshold binary tool is now integrated as an ROI consumer (FCR-008/009). This does not complete native complex UI, all algorithm modes, Fixture native transformation or full ROI behavior parity. No plugin worktree changes were made.

## Workflow context closure

2026-10-17: see WORKFLOW-TAB-TOOLBOX-GROUP-CLOSURE.md for source-confirmed Tab/configuration dispatch, live-source clipboard, toolbox popup/depth and actual Group geometry. Latest acceptance contains364 WPF event operations and actual image runtime evidence; native desktop differential parity remains open.

Group loop update: GROUP-LOOP-BOUNDARIES.md records source-guided LoopStep and actual image/break/cancellation results. The original XML has no step widget. Latest378 event operations include clearing stale successful-restart status; full native parity remains open.

Group index/compound update: GROUP-INDEX-COMPOUND.md records parent scalar integer indexes for break operands, and Circle/Point/Fixture array transport/accumulation. Region/Components mapping, native field trees and full template parity remain open. Latest392 window events and88 runtime checks include real radial-circle results.

2026-10-17 compound leaf batch: see COMPOUND-LEAF-MAPPING.md and compound-leaf-acceptance.html. Source-confirmed field names are projected through independent managed contracts; root rename/removal propagates leaf identities. Source-guided subscription-side Popup and minimap Shift routing are integrated. Polygon/mixed ROI, full double-Popup index behavior and native ABI remain open. User explicitly authorized parallel agents for this batch; file ownership was isolated and builds/UI regression remained serial on main. See FRAMEWORK-PARALLEL-TASKS.md for future split.
