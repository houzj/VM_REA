## 2026-10-22 最新职责与联动覆盖规则

用户将**所有通用框架层**（含插件面向的公共配置模型、模板/编辑器调度、参数/ROI/显示交互合同及会话接口）的逆向和实施归 `main`。`tool-plugins` 只负责具体工具的源码恢复、专属 UI/后台/算法和工具适配，但必须基于 [全局架构模型](framework-architecture.html) 追工具到框架的完整调用边。缺公共能力按 [联动协议](FRAMEWORK-PLUGIN-REVERSE-HANDOFF.md) 提交证据和合同需求；框架线优先处理工具阻断项，交付完整提交 SHA 和验证后，由用户决定同步，插件线确认统一基线再继续。下面旧的“插件线拥有公共插件框架”内容保留为历史，不再用于新任务分配；不得据此自动改动另一个工作树。

## 2026-10-20 main 合流说明

用户已选择把 `tool-plugins` 的**已提交**代码 `032ed4b02e89ccab6c7110e8393cdb0475f64580` 合入 main。冲突裁决、检查与尚未统一的工作树 HEAD 见 [集成记录](PLUGIN-BASELINE-INTEGRATION-20261020.md)。下面工具线历史进度不代表 main 框架 96 单元的完成率；插件在制文件仍归插件工作树所有。

## 最终交接：10/10=100%，续作已停，手验已就绪

automation-2 已通过官方automation_update返回PAUSED，原prompt/周期/目标聊天保留；未停用框架聊天自己的automation。标定板最新手验进程56356，标题“插件线验收 · 公共配置模型 — vision.calibration-board”，主窗口handle非0、Responding=true、session1；本次stdout已出现MANUAL_ACCEPTANCE_READY且stderr空。证据analysis/manual-acceptance/97588c753f784939aa35687bbc3a61e2/ready-verification.json；旧用户验收窗口未终止。手验样本是本地合成棋盘+真实ImageSource→Board，不是相机实拍/设备验收；manualAccepted=false。

总工具实施与根线自审为10/10=100%，剩余范围仅各退出矩阵明确的原生/ABI/数值/呈现安全界限及另列人工验收/跨项目采用，不能据此改称原厂全域位等价。根线总清单analysis/calibration-tools/final-implementation-root-review.json。任务范围已完成，不再自动新开工具；后续仅接用户指定范围或具体真实缺陷。未提交、推送、自动合流或范围外设备操作。
## 2026-10-20 当前授权10工具实施完成：10/10=100%

完整源码逆向/实施并根线自审通过：标定板、相机映射、畸变标定、映射标定、标定加载、N点标定、N图像标定、平移旋转标定、旋转标定、坐标系。人工验收、提交与另一项目采用分别记录，不参与分母/分子。128阶段账本仅保历史内部断点，不冒称128全部证门已逐项重填；本次不是162全目录完成。

最后Coordinate Candidate74/真实MainWindow Host28通过，root独立核450当前源码、206PDB文档、19artifact/log身份0差异；final-validation.json SHA6D514F811F72872E24CB31A2E31B3CC066183EC5C1E0FF25C6E7262E618AF744。包含源IMAGE初始化完整typed数据流、Radio3三合法模式真实点击、ROI编辑与失败恢复、真实图背景/六轴/样式、整方案保存重开及关闭释放。旧71/模板首失败等日志保留。Map final30/26、NI134/16/257/229/49+10、Dist专项11与State1692及各历史数学/ROI/Reset证据仍按实际编译快照分开。

明确有限范围：安全拒绝未知native heap/首轮未写矩阵与危险越界/畸形ABI，不硬编码/清零冒原等价；动态scope batch与普通immutable Graph运输分列；已记录数值floatbit/CPU/Region适配及原SDK/桌面/设备未执行边界保留；Radio3原5槽但只有3声明值，仅实现合法3项，不模拟两缺项UnsetValue/点击副作用，不称像素级模板等价。无已确认尚未实施的当前正常工具必需活动成员。未知不是伪验证通过。

此授权范围已满足结束条件，停用automation-2后不再空跑或扩到未选插件。标定板最新手验宿主正独立构建启动，必须以本次stdout MANUAL_ACCEPTANCE_READY及无新stderr异常判ready，不能以launch.json PID代替。旧用户验收/其它任务进程不干预；没有提交、推送、自动合流或设备操作。
## 2026-10-20 最新完整实施9/10=90%

Map整工具根线源码→消费者自审通过：Path30、最终真实Host26（非doc.Connect图像捷径，初始typed A/B候选/控件选择、168 pairs、显示、缓存保留、Save/readback/history、方案/RemoveNode）；final-manifest SHACC86E1612D925BAC0E8B1D9F169D2885714FD5AAE74E51E96BF2AF23DD8425AC。root在下一共享改动前独立核450源/资源和49source/产物/log 0差异。原Save/busy失败保留，不称原生全域等价。

剩Coordinate：新Candidate74已过，Host后续明确只缺共享RadioSelecter3模板注册。已核原XAML固定5索引槽、Ex3.DisplayType RadioSelecter3；root应用StandardControls48A41候选支持最多5个已声明choice，旧RadioSelecter默认2不变，Coordinate本资源实际3合法value。两个缺声明的空绑定槽不虚造可提交项，WPF UnsetValue/空槽像素外观副作用明列边界。新Candidate/Host正复验，C独占managed、源全冻结。Map不使用Radio3，后加模板不冒充Map旧450源码仍零差异。人工验收独立。

## 2026-10-20 最新完整实施8/10=80%

NI与Dist图像来源初始化数据流有界重开已全部完成并根线自审：原查询→EnumEntrysA赋值，现typed Result→State readonly根快照→控件初建填候选；NI10、Dist11、必要公共State1692全通过0warning/error。原完整数值/ROI/Reset/Host基线不覆写，中间9项查询不冒充最终列表赋值。

剩Map/Coordinate：D当前新版真实Host测试（实际IMAGE列表创建前已有A/B、选择而非测试doc.Connect捷径），结束后唤醒C跑Candidate/Host；全产品冻结。人工验收独立，不提交推送/合流、不终止任何验收进程。

## 2026-10-20 最新源码自审更正：6/10=60%

Rotate Path25已生产通过0警告0错误，根线核实际PDB131源及6日志/程序集hash0差异，完整工具已计。Map最终Path30/Host21通过且scope缓存缺口已修，但SAVE-DIAG追出源EnumerationG.Initers GetFrontParamItemsOperation仍Unsupported；原Operate明确初始化IMAGE来源列表，不能以dropdown另查service代替。NI/Dist同缺口确认，临时撤完整计数，所有数值/ROI/Reset/Host成功证据保留；Coordinate核同一ImageSourceGroup。

当前四独立工具 rootNI/B Dist/D Map/C Coordinate；C当前managed批源码冻结，其余只准备专属源补丁与真实初始化/IMAGE来源测试。C结束后统一短源码窗，再串行验证。不放宽完成标准、不重复算法大探针；人工验收始终独立。

## 2026-10-20 当前续作索引

当前完整实施并根线自审7/10=70%，以SOURCE-LEVEL-REVERSE-PROGRESS.json及HANDOFF顶部为准；人工验收不计进度。活动独立工具是root协调、D映射、B旋转、C坐标系；root N图像已闭合并同步。产品源码冻结后托管构建严格D→B→C串行，不把四工具并行理解成并行build。下面历史5/10和原四线排班不覆盖当前。

# 双项目逆向开发与交接

用户最新进度规则：**总进度只统计实施完成并经自审核的单元/96；人工验收留到全部实施完成后，不纳入总进度。** 源码与验证状态继续保留证据，历史三层交集不再作为总进度。每批优先落实已具源码依据的实施，不以不断拆分静态切片代替交付。此要求覆盖下面旧进度条款。

主框架后续分组逆向由 [源码级总计划](FRAMEWORK-REVERSE-MASTER-PLAN.md) 与 [任务台账](framework-reverse-plan.json) 驱动：4条长期线、32个一级包覆盖F-01～F-23，按源码证据、依赖和完成门槛派发；动态发现的入口拆子任务，不凭产品意图补实现。历史章节不覆盖总计划中当前执行规则或双方所有权。

用户于第五批新增进度要求：每批执行前固定有限子项和退出门槛，结束时按 [进度指标](FRAMEWORK-REVERSE-PROGRESS.md) 报告整包关闭数/32、本批子项关闭/阻塞、实际验证与下一批解除条件；方法数和测试数不得充当总完成率。已关闭静态子项不重复派发；阻塞项只有出现新的解除条件才重启，不用“继续补证”滚动代替收口。 全面对账后，总进度按 [96项三层对账](FRAMEWORK-PROGRESS-RECONCILIATION.md) 与 framework-progress-audit.json 计算源码/实现/验证及三层交集，整包/32仅为最终收口；不得继续只报0/32。
限额中断恢复后的最新验证见HANDOFF顶部：标定板30/140参数传递修复5228稳定/49生产宿主通过，相机映射源码数值顺序修复24例216字段固定0.003通过。旧段落“本轮无算法补丁/仍18或9例失败”保留历史，不覆盖最新结果。四线继续A3/B3/C3/D1叶体；未提交推送或自动合流，手动验收独立记录。

定时驱动已于2026-10-19启用：当前聊天heartbeat“插件源码级逆向四线续作”（automation-2），每30分钟续作；首轮A1/B1/C1/D1已启动/派发，断点见[SOURCE-LEVEL-REVERSE-STATUS.md](SOURCE-LEVEL-REVERSE-STATUS.md)。触发时先复用现有工作，避免重复派工。

当前分组驱动计划为[SOURCE-LEVEL-REVERSE-PLAN.md](SOURCE-LEVEL-REVERSE-PLAN.md)（2026-10-19）。用户要求优先经REA获取源码，避免按意图猜实现：四组先做源码获取/调用链还原，分别从A1/B1/C1/D1推进，再按证据进入补证、实现和真实宿主验收。该计划细化执行，不改变两项目职责与用户的集成控制权。

2026-10-19 第二轮四线已继续完成独立证据切片，新增结果见HANDOFF最顶部第二轮表。相机映射真实生产Runtime存在固定0.003阈值差异，诊断失败属于待修复事实；其余探针通过不代表宿主或全域等价。本轮不修改产品算法，不提交推送；逐线向用户指定“框架层逆向”通知，不自动合流。

## 2026-10-19 本轮并行与接收方

用户本轮明确授权4个智能体独立4线继续逆向：根线标定板/共享协调，三个智能体各自拥有相机映射、畸变、映射专属目录；不因此扩大到另一worktree，不并行改公共核。每线完成可交接切片后通知用户指定的 **“框架层逆向”（REA主目录）**。本轮四条证据通知已发送，尚待接收方采用确认；当前无新提交/算法补丁，通知不能作为复制未提交文件或自动合流的授权。详细成果和边界见HANDOFF最新表；后文历史管理/同步叙述不覆盖本轮用户指定接收方。

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
