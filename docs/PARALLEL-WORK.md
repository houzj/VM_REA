# 双项目逆向开发与交接

用户最新进度规则：**总进度只统计实施完成并经自审核的单元/96；人工验收留到全部实施完成后，不纳入总进度。** 源码与验证状态继续保留证据，历史三层交集不再作为总进度。每批优先落实已具源码依据的实施，不以不断拆分静态切片代替交付。此要求覆盖下面旧进度条款。

主框架后续分组逆向由 [源码级总计划](FRAMEWORK-REVERSE-MASTER-PLAN.md) 与 [任务台账](framework-reverse-plan.json) 驱动：4条长期线、32个一级包覆盖F-01～F-23，按源码证据、依赖和完成门槛派发；动态发现的入口拆子任务，不凭产品意图补实现。历史章节不覆盖总计划中当前执行规则或双方所有权。

用户于第五批新增进度要求：每批执行前固定有限子项和退出门槛，结束时按 [进度指标](FRAMEWORK-REVERSE-PROGRESS.md) 报告整包关闭数/32、本批子项关闭/阻塞、实际验证与下一批解除条件；方法数和测试数不得充当总完成率。已关闭静态子项不重复派发；阻塞项只有出现新的解除条件才重启，不用“继续补证”滚动代替收口。 全面对账后，总进度按 [96项三层对账](FRAMEWORK-PROGRESS-RECONCILIATION.md) 与 framework-progress-audit.json 计算源码/实现/验证及三层交集，整包/32仅为最终收口；不得继续只报0/32。

## Workspaces
- Framework/integration: original checkout, branch main.
- Tool plugins: worktrees/tool-plugins, branch tool-plugins.
- main: tool-independent framework development. The user explicitly chose main for the original checkout. The old framework branch is historical; do not continue development there. The user controls when independent work is integrated; main performs final integration only on that instruction.


用户自行管控两个独立项目。插件项目负责工具及其所需公共节点框架，主项目负责与具体工具无关的主框架。下面历史记录用于追溯，不覆盖本节；旧的“插件只能提框架请求”“管理线自动调度”不再作为当前职责规则。

## Ownership
Framework/main: shell, workflow tabs/canvas/navigation, Group ownership/loops/accumulation, solution/procedure scheduling and persistence, variables/subscriptions at the workflow boundary, tool-independent data contracts. It provides stable integration seams and consumes plugin capabilities, without developing a second plugin configuration system.
Plugins/tool-plugins: catalog and dependency inventory, per-tool source audit and algorithms **plus the plugin-facing framework**: common configuration model, template catalog, ordinary/dedicated editor host, parameter and ROI interaction, tool execution/debug sessions, display adapters, tool-specific persistence, and dedicated tests. Each tool task must name the common plugin-framework capability it requires and verify the tool through that capability in the real host.


细分所有权与任务结构见 [PLUGIN-DEVELOPMENT-BOUNDARY.md](PLUGIN-DEVELOPMENT-BOUNDARY.md)。不能按程序集目录整体划归一线；同一个交叉文件只指定一位提交所有者。当前已分歧文件先审查存量候选，不得整文件覆盖或在未协调时追加另一套实现。

## Integration
两项目在各自闭合功能并中文提交、推送后，由提交方告知另一方完整 SHA、共享接口或文件变化、验证结果和未闭合差异。接收方自行判断同步时机，先保护本地在制工作，完成同步后回报实际采用的基线 SHA；通知本身不代表已经同步。用户负责协调两项目，不设置自动调度。

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

