# Two-line reconstruction workflow

## Workspaces
- Framework/integration: original checkout, branch framework.
- Tool plugins: worktrees/tool-plugins, branch tool-plugins.
- main: integrated baseline only. Integration is performed by the framework conversation after reviewing plugin commits.

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
Group owned internal canvas and tab Up navigation exist. Native loop scheduling, break/LoopIndex and accumulated array outputs remain incomplete. Old cross-top-level-flow calls remain compatibility behavior, not native owned Group. Plugin/debug UI and native algorithm coverage are not complete.

## Local evidence and reports
Original evidence remains in the original checkout's analysis directory. It is excluded from remote history. Build-required UI assets and sample image are tracked. Store new reports/build outputs inside the active worktree, not the other conversation's directory. Reports that require original binary evidence are not portable solely from Git.
