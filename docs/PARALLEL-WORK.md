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

## 2026-10-16 user-assigned integration exception
The user explicitly assigns framework adjustments required by plugin reconstruction to the plugin line. Implement and validate ImageSource-required shared changes in this worktree, record contracts and migrations in CONTRACT-REQUESTS.md, and keep the other worktree untouched. This supersedes the request-only ownership restriction for this work; it does not authorize merging or pushing the other branch.

The user extends that assignment to the next tool, ImageAcquisitionModule. Its necessary shared camera/light contracts, registration and multi-output previews are implemented here. The user subsequently excludes physical-device testing from this line: finish code from original source/resources and controlled regression evidence, without requesting devices or treating equipment availability as a delivery blocker. This does not establish native equivalence for unobserved behavior.

The user extends the same scope to ImageBufferModule and authorizes committing and pushing completed tool milestones from this branch, with Chinese commit descriptions. This does not authorize merging or changing the other worktree.
