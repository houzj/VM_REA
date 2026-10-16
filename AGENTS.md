# Parallel reconstruction work

Read docs/PARALLEL-WORK.md before making changes. Use decompiled source, resources and confirmed runtime evidence as implementation evidence; user feedback identifies differences and is not a substitute specification. Distinguish source-confirmed behavior, independent implementation, and unverified native behavior. Never claim complete parity from passing tests alone.

Framework work owns src/VisionFramework, src/VisionEditor.Core, the WPF shell/canvas/Group in src/VisionEditor, and shared plugin contracts. Plugin work owns tool implementations and dedicated configuration views. Request shared contract changes in docs/CONTRACT-REQUESTS.md rather than changing framework files concurrently.

Keep screenshot/export helpers in tests. Do not introduce screenshot code into product assemblies. Do not spawn subagents unless the user explicitly requests delegation. Do not modify the other worktree or terminate processes owned by another task.

Evidence is local at D:/MyWork/REA逆向测试/analysis/decompiled and D:/MyWork/REA逆向测试/analysis/full-audit. Read original evidence there; write generated reports, screenshots and build outputs to the current worktree. Raw decompiled assemblies/projects and full audit outputs are intentionally excluded from Git. Required UI assets and the image sample are tracked for builds; their presence does not establish runtime parity.
