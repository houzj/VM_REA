# Parallel reconstruction work

Read docs/PARALLEL-WORK.md before making changes. Use decompiled source, resources and confirmed runtime evidence as implementation evidence; user feedback identifies differences and is not a substitute specification. Distinguish source-confirmed behavior, independent implementation, and unverified native behavior. Never claim complete parity from passing tests alone.

The human user controls the two projects and chooses integration points. Main/framework owns the tool-independent WPF shell, workflow tabs/canvas, Group, procedure/solution lifecycle, persistence, scheduling, framework-level data contracts, and final integration when requested. The tool-plugins worktree owns tool implementations **and the plugin-facing framework needed to run them**: common tool configuration models/templates, dedicated and common tool editors, parameter/ROI/display interaction, plugin execution sessions, and per-tool evidence/tests. Do not implement a second copy of the plugin configuration framework in main or leave tool-facing framework gaps to each tool.

Shared files are assigned per feature in docs/PLUGIN-FRAMEWORK-BOUNDARY.md. A project may edit its own worktree, but cross-project contract changes must be recorded in docs/CONTRACT-REQUESTS.md with producer, consumer, payload, lifecycle, compatibility, and acceptance evidence. Do not change the other worktree or silently merge uncommitted changes. Main does not automatically direct the plugin project; the user decides task priority and handoff timing.


Keep screenshot/export helpers in tests. Do not introduce screenshot code into product assemblies. Do not spawn subagents unless the user explicitly requests delegation. Do not modify the other worktree or terminate processes owned by another task.

For manual acceptance, “start the app” means show the current build on the user's interactive desktop (`WinSta0\Default`), not merely create a process or window on `CodexSandboxDesktop-*`. Verify a visible `VisionMaster` window by enumerating the input desktop; `Get-Process.MainWindowHandle` from the sandbox can be zero for a valid user-desktop window. A temporary Task Scheduler task with interactive-token logon can start the current build there; remove only that temporary task afterward and leave the app open. Do not touch windows or processes owned by other tasks.

Evidence is local at D:/MyWork/REA逆向测试/analysis/decompiled and D:/MyWork/REA逆向测试/analysis/full-audit. Read original evidence there; write generated reports, screenshots and build outputs to the current worktree. Raw decompiled assemblies/projects and full audit outputs are intentionally excluded from Git. Required UI assets and the image sample are tracked for builds; their presence does not establish runtime parity.
