# Integration handoff

## 2026-10-19 — 第十一批四线实施

[第十一批](reverse/BATCH11-20261019.md)已落地近期偏好写失败不误报方案保存、Group索引源删除保留数组源/静态索引、启动cfg到main Begin显示策略、原容器名称/键原始槽保全。实际自审核A16/B23/C24+12/D19项通过，产品构建成功并保留既有CS8603警告。C为main隐藏view适配，不冒充原SDK清叶；C03.2只到partial。**实施30/96=31.25%，剩余66项，本批整项+0**，已有单元不重复加分、人工验收后置。原SDK/格式/多消费者缺实施仍开放；无原厂执行/推送，tool-plugins未动，审计保全。

## 2026-10-19 — 第十批四线产品实施

[第十批报告](reverse/BATCH10-20261019.md)：实际落地Tab显示Group记忆/overview定位、Group候选搜索与重复事件幂等、配置历史root抑制/异常复位及真实Group消费、独立存储保存/重开重复属性校验。最终自审核A12/B12/C29/D9与SaveHistory23项均通过，产品构建成功，保留既有CS8603警告。**实施30/96=31.25%，本批新增C01.2一项，剩余66项**；A/B/D修复不重复加分。C为main快照适配，原SDK root/part仍开放；B资格元数据、FCR024等未闭合。人工验收后置；无原厂执行/设备/推送，tool-plugins未动，完整审计保留。

## 2026-10-19 — 第九批及用户最新实施口径

用户明确总进度只按实施完成并自审核计数，人工验收在所有实施完成后安排，不纳入进度门槛。当前**29/96=30.2%，剩余67项**；旧28/96三层交集保留为历史证据，不再当总进度，此变更不是新增实施。[第九批](reverse/BATCH9-20261019.md)取得SDK到PlatformManager两个函数的静态绑定及下一消费者，审查历史输入失败归因和跨方案结果/计时缓存；无产品更改/原目标执行/推送，tool-plugins未动。失败与原件哈希保全，后续优先落实已有源码依据的实施、自审核收口。

## 2026-10-19 — 第八批静态消费者与隔离硬门

[第八批报告](reverse/BATCH8-20261019.md)：A定位原配置错误双产生点，内层异常缺失仍blocked；B原生二进制参数经+23a8函数指针/0bd2消息分派；C证明空流程构造/空Init仍触发方案与SDK静态初始化，真实Begin运行门关闭；D取得保存callee及下一层消费者，+2398实际绑定/文件序列化未知，返回0不当落盘完成。4个有限静态切片退出，不算4个产品功能完成。原件/字节核对、失败和证据哈希保留，无原目标运行/产品修改/历史回归重跑/推送，tool-plugins未动。总28/96=29.2%（+0），源码51/96=53.1%、实现及验证29/96=30.2%，68项仍缺至少一层。

## 2026-10-19 — 第七批A03/B05/C02/D01

见[第七批报告](reverse/BATCH7-20261019.md)。原Value消费者5例35断言通过，gvar托管→原生参数边界确认；两个保存导出5段236行原指令获独立字节复核。日志初始化配置错误仍阻塞，最后子进程finally日志缺失；同权限发现本探针GUID注册表键残留，根线仅清理精确键并复核不存在，失败与清理证据保留。D01.3源码仅推进partial，无产品改动/历史回归重跑/原机差分/推送；tool-plugins未动。总完成28/96=29.2%（+0），源码53.1%、实现/验证30.2%，68项仍缺至少一层。

## 2026-10-19 — 按计划第二批A03/B01/C02/D02

见 [第二批记录](reverse/BATCH2-20261019.md)。A03原件22方法CIL推进显示/运行对象和缩放存储链，实际WPF复现并修复main方案替换同ID视口泄漏；专项26项、既有画布73项通过（离屏真实WPF，非原机桌面差分）。B01原选择器合成缓存20谓词、C02原配置/等待helper20断言通过；D02独立存储23项通过并获独立复核，无存储产品补丁。完整包继续开放：原生缩放持久化、真实首次缓存/SDK、实际启动配置与回调、插件准备器资源组合未完成。tool-plugins未动，D01原生引擎阻塞保留，raw原件/失败/哈希和台账更新。

## 2026-10-19 — 按总计划启动A01/B01/C01/D01第一批

详见 [第一批执行记录](reverse/BATCH1-20261019.md)。原ToolBoxData.UpdateLoad隔离17断言/10观察、原parser合成owner13检查、原UnRedoService合成动作13断言均通过，原件及实际依赖哈希保全；失败日志未删除。D01确认保存回调及五参数/PF结构体ABI边界，保存导出坐标已固定，但当前REA native引擎未配置，writer函数体/单操作原机保存仍开放。更新任务台账与FCR-019/022的证据，不关闭完整包、不改产品或tool-plugins。新探针均在tests，本轮无产品构建或真实VM桌面差分，不将43断言当框架等价验收。

## 2026-10-19 — 建立驱动分组逆向的源码级总计划

新增 [FRAMEWORK-REVERSE-MASTER-PLAN.md](FRAMEWORK-REVERSE-MASTER-PLAN.md) 与 [framework-reverse-plan.json](framework-reverse-plan.json)：4条长期线、32个一级工作包覆盖现有23领域，明确依赖、原始入口、取证操作、完成标准、共享文件和运行资源锁。源码/REA优先，反编译歧义核对CIL/汇编，截图和产品意图不能补规格；未知和独立实现分列。下一批明确为A01/B01/C01/D01，但本次只制定计划，未启动任务或修改tool-plugins。台账planned表示新任务待执行，不重置历史实现和验收；验收JSON/近期报告的历史差异由D08对账。

## 2026-10-19 — 第二轮四方向独立取证

- 工具加载：闭合悬停/展开/右键共享缓存、扫描失败残留和Group回调到模型/装饰器的时序，见 [LOADER-ROUND2-20261019.md](LOADER-ROUND2-20261019.md)。主宿主动态发现仍未接入。
- 撤销：闭合首次旧值/延迟新值捕获、同模块part去重、不同对象恢复载荷、密码取消移栈以及Group写入到Register，见 [UNDO-ROUND2-20261019.md](UNDO-ROUND2-20261019.md)。未执行厂商服务，未将简单布尔抑制作为原生复刻。
- 持久化：8个随包sol、122条MoudleFrame记录的外层候选结构，两份独立读取器的名称/偏移/长度/载荷哈希一致；检查器保留名称槽原hex。真实样本前后哈希不变；6个合成夹具验证正常记录、四类坏输入和剩余字节报告，PowerShell AST检查通过。见 [SOL-ROUND2-20261019.md](SOL-ROUND2-20261019.md)。mdata内部与原机保存往返仍未知。
- 运行Tab：补入VM.Xml/BaseNode的原始程序集身份与反射集合装配，追踪集合钩子和缓存边界，见 [VARIABLE-TAB-ROUND2-20261019.md](VARIABLE-TAB-ROUND2-20261019.md)。未取得原机真实Params.Data/Tabs差分。

本轮产品代码未改，未访问或修改tool-plugins，没有新建工作树、执行厂商代码或干预其他任务进程。证据分别保留于 `analysis/full-audit/{loader,undo,sol,variable-tab}-round2-20261019/`，汇总清单在 `analysis/full-audit/parallel-round2-20261019/`。未声称完整原生等价；没有因仅文档和测试辅助改动重跑整套WPF回归。

## 2026-10-19 — 四方向只读并行复核与最小修复

四个独立方向完成工具发现、VM.UnRedo CIL、真实sol容器、变量目标Tab审查，完整结论与边界见 [PARALLEL-RECONSTRUCTION-20261019.md](PARALLEL-RECONSTRUCTION-20261019.md)。修正目录策略最后匹配覆盖；11项检查通过。新增测试专用只读样本检查器，递归发现8个样本、98个UI条目完整消费、原件哈希不变。更新FCR-019/022，保留本地CIL和JSON证据。没有执行厂商服务、没有实现原生sol导入器或撤销栈，没有修改tool-plugins；本地通过不代表VM等价。

## 2026-10-18 — 第二批 MF-01 / MF-02 / MF-04 / MF-05 合并验收

- MF-01：菜单与工具栏撤销/重做统一遵守原始 `TitleMenueItemData` 的参数窗口开启禁用条件。实际 WPF 专项验证四入口保护，关闭窗口后对真实节点名撤销/重做；原生警告弹窗视觉未复刻。见 `MF01-SHELL-HISTORY.md`。
- MF-02：依据 `SearchModuleCommandExecute`、`ItemsPage` 和 `ZoomArea` 定位链修正工作流列表的节点跳转；160% 缩放时实际 WPF 滚动到 (3556.8,2888.8)，视图切换不修改文档，撤销及保存重开正常。列表弹层/Group 嵌套路径的原生差分仍开放。见 `MF02-WORKFLOW-LIST-SEARCH.md`。
- MF-04：删除或改型局部变量时，同一次文档事务清理 Group 中断操作数和动态索引的失效引用。实际 WPF 删除/改型、Group 运行、撤销重做、保存重开专项 19 项通过。原生变量订阅解绑有证据，具体 GroupBreak 清空规则属独立模型一致性实现。见 `GROUP-VARIABLE-BREAK-CLEANUP.md`。
- MF-05：预取消的空流程或仅禁用节点流程不再误报成功；执行作用域停止也参与最终状态汇总。完整方案调用与执行专项 71 项通过；原生 `StopExecute` 的返回状态/设备回调仍未证实。见 `MF05-CANCELLATION-EDGE.md`。
- 合并后的主窗口历史专项、画布专项 42 项、Group 清理专项 19 项、完整实际 WPF 回归 456 项通过。以上仅关闭所列边界，框架总清单的原生差异仍开放。插件分支继续由用户决定何时合流，本次没有合入。

## 2026-10-18 — 主框架 MF-01 / MF-02 / MF-04 独立闭合点

- MF-01：依据原始 `App.ShowMainWindow` 启动链核对当前 STA 入口。测试脚本只启动并清理自己创建的进程；三次工作树启动窗口耗时 4.7—6.6 秒，main 的 Release 构建实测 8.953 秒出现 `VisionMaster` 窗口。旧发布进程无窗口句柄的原因仍未确认；产品入口未改。见 `MF01-STARTUP-DIAGNOSTIC.md`。
- MF-02：依据 `ZoomArea.xaml.cs` 的逐次鼠标位移和非零滚轮档位，修复画布贴边后反向拖动仍粘边、Ctrl 零增量滚轮误缩放。合并后实际 WPF 画布专项 36 项通过；原生边距公式、系统鼠标注入和 DPI 差分仍开放。见 `MF02-CANVAS-PAN-ZOOM-EDGE.md`。
- MF-04：Group 局部变量结果绑定只提供当前独立执行图可用的标量来源，提交时复校执行顺序。main 上真实选择器、Group 运行值 2→7、保存重开及循环拒绝共 11 项通过；原生候选过滤规则与复合变量类型仍未确认。见 `GROUP-LOCAL-VARIABLE-BINDING.md`。
- 合并后 `VisionEditor.WpfChecks` 456 项、独立存储 30 项通过。三项均为局部功能闭合，不表示整个框架或 VM 原生行为等价。插件项目提交仍由用户选择合流时机；本轮未合入或修改插件工作树。

## 2026-10-17 职责与清单重新调整：插件相关公共框架在本项目开发

用户明确将插件/节点必需框架列为本项目正式工作，新增 `docs/PLUGIN-FRAMEWORK-CHECKLIST.md` PF-001～014，并调整插件总清单与 PARALLEL-WORK.md。公共定义/状态/模板/宿主、订阅/执行/文件通知/ROI 桥接与算法插件同步逆向，先公共链路再闭合标定工具。N点/N图像最高优先级、坐标系最后、无设备测试、验收后中文提交推送保持不变。

本轮代码在制，未提交/推送/合并；已向原 checkout 的主框架会话发同步消息，说明共享文件与接口负责范围，请其确认插件发现注册、复合 I/O、数组索引和 ROI 的已有实现，避免覆盖和重复实现。消息已发送不等于对方已确认或代码已同步。公共合同当前状态与迁移见 CONTRACT-REQUESTS.md；旧 N点/平移旋转历史算法提交保留，配置 UI 闭合状态重新打开。

当前普通配置模型提供器以 ReadCalibFile 为真实宿主样本；N点复杂模板演示为测试覆盖工具，不是完整N点验收。公共链路专项回归与实际WPF回归继续执行，完成可验收节点后启动明确标记程序供用户人工验收，不以测试数量宣布全部节点等价。

## 2026-10-16 / tool-plugins: JPEG source correlation and independent backend

- Supersedes the previous JPEG-unavailable implementation. Original MVDMediaProcess identity/version plus focused decompilation establish libjpeg-turbo 2.1.3, default RGB decoding flags/pitch and warning acceptance. Correlated source hashes and 27 original SDK fixture observations are tracked in docs/plugins/image-source-jpeg-evidence.json. The original conversion wrapper is traced further, but its kernel remains unrecovered.
- Independently compiled official upstream source supplies the plugin-local Windows x64 turbojpeg.dll, with source archive hash, rebuild script and redistributed licenses. No vendor binary or new NuGet package is used. Preserve runtimes/win-x64/native and ThirdParty/libjpeg-turbo during integration/deployment. Static CRT, scalar SIMD-off build; performance and other architectures are not equivalent.
- Pixel verification: all 26 valid RGB fixtures and all 13 successful native MONO8 fixtures match byte-for-byte. Includes progressive/subsampling/quality/grayscale/restart/EXIF/no-EOI cases; invalid header rejected. Historical WIC 24/323 mono pixel discrepancy is eliminated. Thirteen small-color fixtures still diverge in error behavior: native RGB succeeds but MONO conversion fails; reconstruction emits gray. Preserve this explicit gap rather than inventing a minimum geometry rule.
- Validation: 362 checks passed, build zero warnings/errors, including 12 concurrent JPEG reads and existing lossless/stitch/UI/state checks. Original outputs and independent mismatch report distinguish matched pixels from native failure boundaries. This is not complete ImageSource parity.
- No framework/shared contract sources or other worktree modified. CONTRACT-REQUESTS.md clarifies plugin-native content deployment and retained RGB/provider/lifecycle service gaps. Next independent work: recover small-color conversion failure/kernel, broaden CMYK/ICC/malformed coverage, folder sort semantics and original popup/desktop evidence.

Initial baseline: existing independent reconstruction, including owned Group navigation and Tab return fix. Not complete VM parity.

For each handoff record: branch/commit, source evidence, changed tool or framework behavior, real UI/run verification, remaining gaps, and shared contract requirements.

## 2026-10-16 — F-01 typed primitive arrays / F-02 source audit

- Scope: Scalar/Array metadata with scalar defaults; immutable integer/number/text array values; registration, connection, subscription, workflow input/output, Group signature, runtime output and persistence validation. Main result formatter displays array elements.
- Evidence: VM.Core.DynamicOutput.GetOutputInt/Float/String; IMVSGroupCs.GroupResult; VmIODecorate report modes; DynamicParamsTabControl self-execution command setters. Details and boundaries in FRAMEWORK-CONTRACTS.md; hashes and local HTML in analysis/full-audit/array-contract-source-files.json and framework-contracts.html.
- Verification: 28 array-contract checks (including owned Group array → downstream sum 12 and save/reopen); 115 editor core, 34 Group calls, 31 variable/data, 38 execution, 22 module checks; 244 actual WPF checks. Release lifecycle: core/plugin/WPF/editor all exited with code0. These are independent runtime checks, not VM loop/ABI equivalence.
- Release: dist/VisionEditor-WpfArrayContract. Screenshot code remains in tests. No plugin worktree or tool implementation was changed.
- Compatibility: plugin interfaces untouched; scalar callers unchanged. New Cardinality JSON fields require the new strict reader; do not cherry-pick regenerated sample files alone into an older engine.
- Open: native numeric ABI/precision, remaining types, full array selection UI, loop/break/LoopIndex, actual accumulation behavior, debug-session lifetime. F-01 and F-02 remain open overall. Existing host editor Run still executes owning workflow; self-command native scheduling needs further tracing before changing it.

## 2026-10-16 — main branch policy

At the user's request, the original checkout now develops framework and integration directly on main. The former framework commits were fast-forwarded without rewriting history. tool-plugins remains isolated and was not modified. Follow main for current shared contracts; framework is a historical branch.

## 2026-10-16 — F-02 ordinary WPF tool debug sessions

- Source chain recovered through DynamicParamsTabControl → BottomCommand → VmModule.Run → ModuleManager.SelfRun → IMVS_SelfExecuteOneModule(moduleId), plus callback-gated continuous execution and parameter-window shutdown disposal paths. Details: DEBUG-SESSION-CONTRACT.md.
- Implemented: NodeDebugExecutor; ToolDebugSession single/continuous/stop/dispose; optional ToolEditorContext.Session; WPF Run adapter uses isolated module execution with explicit cached inputs. ExecutionSessionResult preserves actual invocation input snapshots for internal Group-style tool inputs.
- Host behavior: clears target/downstream stale results, updates actual image preview, prevents concurrent workflow run while module debugging, waits for module disposal before editor or main window shutdown. Screenshot helpers and slow cleanup-gate modules remain in tests.
- Verification: 21 debug runtime checks; 253 actual WPF checks including a real image-source window with incomplete downstream workflow and controlled active-runtime close; 28 array, 34 Group-call, 31 data, 115 core checks. Final core/plugin/WPF/editor-close lifecycle checks all exit0.
- Release: dist/VisionEditor-WpfDebugSession. Startup entry updated only after final checks.
- Compatibility: old ToolEditorContext constructor and Run delegate retained; no plugin implementation or plugin worktree changed. Context/host UI APIs must be invoked on the WPF dispatcher.
- Remaining boundaries: native upstream preparation/first-run behavior, native in-flight cancellation and device cleanup are not reconstructed; explicit cached-input/cooperative-cancel/failure-stop policies are independent. Group configuration run remains separate; Group loop/break/accumulation and full plugin debug UI remain open. This supersedes the earlier ordinary-WPF-host whole-workflow Run gap, not every legacy WinForms or Group path.

## 2026-10-16 — F-12 owned Group loop range and private LoopIndex

- Evidence: isolated Ghidra/REA analysis of ModuleFrame.dll, CGroupNode::DoWork 0x1800a0930, range subscription resolver 0x1800a53a0, IsBreakLoop 0x1800a55b0; original Group XML and managed callback/private-source tree. Static evidence, not VM runtime equivalence. Details: GROUP-LOOP-RECONSTRUCTION.md; local raw evidence/HTML: analysis/full-audit/group-native-audit/.
- Implemented: owned Group [start,end) execution, default one-shot index0, parent variable/parameter subscriptions, private internal LoopIndex, nested index isolation, cancellable interval, last-result diagnostics, save/reopen/undo. Legacy cross-top-level calls remain one-shot.
- UI: actual owned Group loop tab switch and start/end subscription selectors are connected to runtime; interval validation 0..10000. Existing shared template is still a partial UI reconstruction, not original template parity.
- Verification: 17 Group runtime checks; existing 34 calls,31 data,28 arrays,21 debug,115 core checks. WPF actual run uses real BMP input, chosen end variable3 and three body invocations. Initial actual-run check failed because its older canvas fixture contained an unconfigured image source; configured the real image input and reran. Final WPF count and lifecycle recorded below after final checks.
- Independent policies: stop on child failure, fresh per-iteration contexts,10000-iteration guard,last-result publication, cooperative cancellation. Break,LoopStep,accumulation,full native callback/status semantics and original control templates remain open. F-12 stays partial; F-13/F-14 are not marked implemented.
- Plugin implementation/worktree unchanged. Snapshot fields and optional NodeDebugExecutor argument are additive framework interfaces; no plugin source change required.
- Final verification: 259 actual WPF checks, including real Run button completing three Group body executions and opening internal editor retaining index2. Final core/plugin/WPF/editor-close lifecycle checks all exit0 (group-loop-range-final-lifecycle). Release: dist/VisionEditor-WpfGroupLoopRange; Start-Flow-Editor.cmd updated after final checks.

## 2026-10-16 — F-13/F-14 Group break, primitive accumulation and display linkage

- Source: original Group XML; VmIODecorate/DisplayObjectDecorate/VmIO/VmSolution; isolated ModuleFrame native DoWork, IsBreakLoop, first-round guard, ProcessGroupOutputV44, report setter, save/load/prepare/release. Raw int32 assembly overrides misleading floating pseudocode. Detailed addresses and policies: GROUP-BREAK-ACCUMULATION.md. Full remaining Group scope: GROUP-FULL-RECONSTRUCTION-CHECKLIST.md.
- Implemented UI/runtime: pre-body int/float/string break, six comparisons, fixed array index, parent/internal/private/self-output source; primitive scalar/array accumulation, latest image, downstream signatures, interface rename/delete/copy propagation; real display editor and image output linked displays; native switch images/icons; responsive cancellation/close cleanup. Own-output buffer reads selected element without copying full accumulated history each round.
- Verification: 54 targeted break/accumulation,17 loop,34 call,31 data,28 array,21 debug,115 core;282 actual WPF operations. Rendered/inspected own WPF break/output/array windows. Test-generated circle overlays are display verification only, not VM algorithm parity. Capture helpers remain in tests.
- Release: dist/VisionEditor-WpfGroupBreakAccumulation; Start-Flow-Editor.cmd updated after lifecycle checks. Core/plugin/WPF/editor-close all exit0; observed respectively0.75s/2.62s/25.80s/4.83s,60s watchdog. NU1900 feed audit warnings do not fail builds.
- Native parity remains partial: complete input editing, runtime proxy editing, Group isolated SelfRun/continuous, all native controls/states, LoopStep/dynamic indices/compound types, persistent per-round instances/local state, callback/error/device semantics, .sol/ABI and real VM differential acceptance. Current Group Run still runs owning parent workflow. Independent guards/failure policies are explicitly documented.
- Contract: optional GroupBreak operands and result diagnostics/displays; optional LinkedOutputId for image-output displays; null fields omitted. Existing plugin interfaces retained. No plugin implementation/worktree changes. New metadata needs current strict reader.

## 2026-10-16 — Group input transactions, editable proxies and isolated execution

- Source evidence: GroupInputParam/GroupParam, DynamicIOConfigControl/VmIODecorate, GroupRunParam/RunParamConfigDecorate/DynamicNameValueItemOfGroup, DynamicRunParamConfigControl/Panal, DynamicParamsTabControl self/continuous/stop commands. Original-file hashes in group-input-runtime-source-manifest.json. Details: GROUP-INPUT-RUNTIME-SELFRUN.md.
- Implemented: owned Group input add/batch/rename/delete/bind with stable-ID propagation, cardinality checks and atomic transaction/undo; basic internal parameter/input proxies with actual editing, validation, subscriptions/unsubscription, label/delete; isolated single/continuous/stop execution reusing same loops/break/accumulation. Parent unconfigured siblings are not executed. Old cross-top-level calls retain compatibility path.
- Actual operations: input3 plus parameter5 exports8; parameter subscribes input3 exports6;解除 restores8. Input proxy disconnect reports failure and clears preview, rebind recovers. Batch selector adds two typed inputs plus parent subscriptions; one undo reverses whole edit. Continuous stop and restart, active close/disposal tested. Captured/inspected own WPF controls; screenshots only in tests, not VM screenshots.
- Found/fixed through actual operations: repeated self-run leaking synthetic Group arguments into parent input snapshot; old compatible call accidentally blocked by Group concurrency protection; missing internal input errors disappearing when no child session was produced. Repeated, failure, recovery and old call paths revalidated.
- Verification:18 new Group editing/self-run checks;54 break/accumulation,17 range,34 call,31 data,28 array,21 debug,115 core;300 actual WPF operations. Final lifecycle Core/Plugin/WPF/EditorClose all exit0 under60s (0.81/2.43/52.30/8.76s); group-input-selfrun-lifecycle. Publish succeeded, with cached NU1900 feed-audit warnings.
- Release: dist/VisionEditor-WpfGroupInputSelfRun; startup entry updated after lifecycle checks. main only; no plugin implementation/worktree changes or other-task processes terminated.
- Remaining native differences: complete run-parameter categories/order/multi-target relation/sync and complex editors; all compound input types; native SelfRun initial preparation/cache invalidation/callback ABI; per-round persistent instances/device semantics, LoopStep/dynamic index/native result/status/.sol parity. Explicit cached-input/failure-stop policies are independent, not a claim of full native equivalence.

## 2026-10-16 — Group categories/relations, declared complex adapters, SelfRun preparation

- Source: ParamConfigDataBase/GroupRunParamConfigData category/ordering/confirmation paths; RunParamConfigDecorate and DynamicNameValueItemOfGroup relation/sync paths; ParamTypeList/ParamGroup/ROISelecter/OpenFileDialog. Current VisionMaster.exe.config MaxRunParamsNum=32; SDK relation list128. Hashes: group-advanced-source-manifest.json. Full implementation and limits: GROUP-ADVANCED-RUNTIME.md.
- Native static analysis in an isolated owned REA/Ghidra session: SDK IMVS_SelfExecuteOneModule0x180645a50 → SM_SelfExecuteOneModule0x180664060 (assembly command1013) → SendCommand; ModuleFrame RunNodeSelf0x180043f60 → procedure0x1800ddd80 → JustRunModuleSelf0x180100e00; topology update0x18010ae80, parameter bindings0x1800b98c0, Group input0x1800a4a00/0x1800a4a90, ProcessIntIO0x180010450/GetInt0x18011e580. PrepareGroupData0x18005ce70 loads solution data and was not mistaken for SelfRun inputs. Local SDK/engine evidence bundles under group-advanced-audit. Native target closed; owned analysis helper stopped; no original runtime or other task process modified.
- Framework/UI: separate ordered run categories, rename/membership, member/group movement, confirmed delete; relation picker initial selections/clear, typed target validation, atomic multi-target value/subscription writes; deep-copy/delete/interface-rename propagation. Explicit optional ParameterDescriptor.Editor for File/Folder/Enumeration/Range/Roi, no name guessing or plugin implementation changes. Range sync copies all fields; ROI geometry enters actual ModuleExecutionContext. ROI invalid grid values reject commit; polygon table edits blocked rather than pretending graphical support.
- First SelfRun Prepare resolves typed caller inputs/cache/variables and parameter defaults/subscriptions before entering body. Missing sources diagnosed without executing upstream; failed preparation clears prior successful child result. Real WPF recovery after explicit source preparation verified. Fail-fast and cache snapshot rules remain independent policies; native full failure/status/cache ABI is not claimed equivalent.
- Verification:33 Group editing/self-run checks;54 break,17 loop,34 call,31 data,28 arrays,21 debug,115 core,23 persistence.309 actual WPF checks include category operations, related range fields, enum/path/ROI real module outputs12/16, first missing-cache failure/recovery, ROI invalid cell rejection and correction. Own rendered windows inspected; not VM screenshots or original algorithm parity. Source-independent test modules only test host data flow.
- Final release: dist/VisionEditor-WpfGroupAdvancedRuntime; startup updated after publish and exit checks. Final Core/Plugin/WPF/EditorClose all exit0 under90s (1.49/2.11/38.61/2.44s); group-advanced-roi-final-lifecycle. Cached NU1900 feed-audit warnings did not fail builds. Screenshot code stays in tests.
- Remains partial: original two-column category/config tree/GridView and SliderArrange/graphical ROI/training editors, actual tools' editor metadata/factories (FCR-006), complete public dynamic-name/API mapping, subscribed-value synchronization semantics and native first-cache/callback/error behavior. The functional adapters do not satisfy full native UI fidelity; these gaps are explicitly open. No plugin worktree changes or unrequested cross-chat messages.

## 2026-10-16 — Group native configurator / graphical ROI / scoped real-tool host

- Branch main. Details and remaining scope: GROUP-NATIVE-EDITORS.md; 24 original managed/XAML source file hashes and 21 exact copied UI asset hashes in group-native-editors-source-manifest.json.
- Added independent source-guided two-pane runtime configuration with native dimensions/columns/type filters, category selection and group0 naming, partial batch insertion up to32, relations/sync and original icons, source-confirmed Yes/No clearing. Management removed from runtime parameter page. Legacy ungrouped/input-proxy data remain compatibility behavior.
- Replaced the ROI table with an image/parameter layout, native resource/color/icon states, four current geometry kinds, source-guided circle/polygon semantics and independent geometric manipulation/rollback. Target-bound image preview uses read-only NodeDebugExecutor.ResolveInput. Fixture without transform is rejected; no arbitrary node-image fallback.
- Real image-source dedicated WPF editor is connected inside owned Group through the existing factory and scoped host/session. Actual edit InitialSN17 -> actual Group run Width1296/SNCode17 verified. Full tool editor bridge is not the native per-parameter ROI editor; no real ROI algorithm plugin is integrated. See FCR-007; no tool implementations, other worktree, or plugin contracts changed.
- Verification:330 actual WPF,28 debug/preview,115 core,18 plugin checks; manually inspected generated client-area images. Release dist/VisionEditor-WpfGroupNativeEditorsFinal. Core/Plugin/WPF/EditorClose all exited0, times0.586/2.044/39.002/5.186s. Only own test processes managed. NU1900 cached audit-feed warnings, no code errors.
- Remaining: real ROI-tool implementation/metadata/editor integration; native additional shapes, normalized/Fixture coordinates and complete handles/capture/clipboard/clipping; SliderArrange/window theme/outer chrome and original relation-selector layout; native ABI and true-VM differential acceptance. Do not mark Group complete from these checks.

## 2026-10-16 — Source ROI templates and real binary-tool integration

- Main integration; no plugin worktree or existing ImageSource implementation changes. Shared contracts recorded as FCR-008. Detailed scope/evidence/limits: ROI-REAL-TOOL.md, roi-source-inventory.json, roi-style-assets.json, roi-runtime-evidence.json. Local HTML: analysis/full-audit/roi-real-tool-audit.html.
- VisionUi contains source-guided shape templates, selector resources/order, pointer state machines, numeric commit, caliper handles, polygon points, normalized/pixel conversion and dedicated Group frame. Full binary debug host uses actual input subscription picker, atomic configuration writes, dedicated ROI editor and single/continuous/stop session lifecycle. Screenshot helpers remain tests-only.
- CPreproMaskTool managed source recovered and safely reimplemented with float/scanline/endpoint/tangent-row semantics. All 120 original SDK sector/ring differential cases match pixel-for-pixel, resolving prior 11-pixel discrepancy. Hard threshold is independent implementation; native C++ engine and other numerical modes are not recovered.
- Validation: 185 ROI/tool checks; 6 actual main-shell factory/input/ROI/runtime/close checks; 5 normalized-coordinate checks; 330 existing WPF regressions. Actual main-shell 400x400 ROI consumes160000 pixels. Real screenshots inspected; they are reconstructed-program captures, not original VM UI acceptance.
- Remaining fidelity gates explicitly open: full display-object styling popup, all combo/slider/scrollbar templates, some auxiliary/multi-select ROI behaviors, Fixture and inherited blind-region paths, unsupported binary modes, full original VM runtime comparison. Source properties alone and passing checks do not establish complete parity.
- Final self-contained release: dist/VisionEditor-WpfRoiSource; launcher updated after Core/Plugin/WPF/EditorClose all exit0 (0.980/2.513/37.175/5.391s). Retained own process handles only; report roi-native-final-lifecycle. No native SDK dependencies in product.

## 2026-10-16 — ROI显示、区域继承与位置修正实际闭环

- main框架线集成，未修改插件工作树、现有ImageSource实现或其他对话的进程。中文范围与限制：ROI-DISPLAY-FIXTURE.md；证据哈希：roi-control-template-sources.json；本地HTML：analysis/full-audit/roi-display-fixture.html。
- 显示页独立DisplayStyles接通眼睛、OK/NG颜色、透明度、粗细、保存和撤销。按ImageViewConfig实际引用的VM.PcUI版本迁移调色板、高级取色器、RGB显示和持续更新；没有使用同名Apps.XmlUI版本的自动关闭行为。原组合框、滚动条、滑块、单选、布尔开关、关联/编辑入口资源已迁移。真实ColorStyle1配置和148项画刷已核对。
- 绘制/继承区分矩形、圆环的区域/参数方式；独立屏蔽继承；真实参数关联含局部变量。ApplyConfigurationChange将模式参数、ROI与失效订阅合并为一次原子编辑，一次撤销恢复状态和订阅。FCR-009记录可选回调和追加Region/Fixture/Point独立协议。
- Fixture信息、点和坐标输入进入真实算法，平移/非等比缩放改变OutMask；显示实际解析区域，不再残留手工ROI。多图像输入预览按明确Image端口解析，开始绘制回到输入图像，防止只读预览阻断绘制。
- 最终专项212项通过，另输出归一化5项检查；包括原SDK120组扇形/圆环零像素差分、Group实工具、连续停止、真实窗口取色/亮度/透明度、保存、变量关联和原子撤销。20×10继承参数处理200像素，Fixture平移50改变真实掩膜，切回手工恢复160000像素。实际截图逐张检查，捕获代码仅在tests。
- 原生缺口仍明确开放：Fixture底层仿射及舍入/边界未恢复，当前为独立最近邻逆映射；InputBlindPolygonString编码未恢复，使用托管InputBlindRegion；Fixture变换坐标直接编辑、全部原生来源树和复合滑块模式尚未完成。结果预览只读，正缩放是独立约束。通过上述验证不等于原VM同状态像素级验收。
- 最终发布：dist/VisionEditor-WpfRoiDisplayFixture，启动入口已更新。最新115核心、18插件、330 WPF及发布窗口关闭检查均退出0，耗时0.587/1.851/41.407/2.329秒；报告roi-native-display-fixture-lifecycle/exit-results.json。仅管理自身启动的进程，无原SDK产品依赖。
- 提交前显示边界复核：透明度/线宽越界文字已修复为限制到有效范围；最终真实宿主24项操作检查通过（roi-display-fixture-final-host.log）。完整专项212项及其原SDK样例结果保留，未以单独边界检查替代完整运行证据。

## 2026-10-17 — 框架验收清单重整与实际界面复验

- main框架线；依据当前实现重整23个框架域，记录已完成子集、未完成、原生差异、源码入口及操作证据。结构化源：framework-acceptance.json；便携HTML：framework-acceptance.html；精选截图与操作记录：acceptance-evidence。原始反编译和完整audit仍排除Git。
- 实际截图发现并修复：Group运行参数双栏内容根容器透明、前景未明确导致黑底/文字不可读；按DynamicRunParamConfigPanal.xaml:149/237/282明确#f2f3f5背景和#4c4c4c前景。主窗口状态提示增加右边距，避免紧贴流程耗时。修复后逐张检查对应截图，不把目检当作原生差分。
- 验收日志升级为330条逐项WPF窗口事件/内部处理操作记录，带结果与时间。真实ROI宿主24项操作重新执行，包括订阅、弹层、实际200像素继承、Fixture平移、模式切换撤销及关闭。28数组契约和23持久化检查通过。Core115、插件18、WPF330和主窗关闭在监护运行中全部exit0，记录见便携HTML。
- 一次未加监护的检查停滞在配置阶段，结束本轮确切PID后增加逐项日志，以90秒监护重跑完整通过；没有把中断运行记为通过。桌面Computer Use重试截图超时、点击缺少geometry、后续观察缺前台PID，桌面鼠标拖放仍未验收。未修改安全设置或其他任务进程。
- 进一步确认尚未接入：全局变量、相机/通信管理、全局触发、脚本、综合配置、流程超时、原生导出与.gvar导入导出；运行模式只是静态文字，菜单仍含简化独立命令。没有按图标可见判定完成。
- tests/Export-FrameworkAcceptance.py从显式清单及实际操作记录生成可筛选HTML与清单，保留证据哈希。报告工具和截图均留在tests/文档，未引入产品截图代码。原生同状态视觉操作差分、多屏DPI、设备长稳、完整.sol/ABI仍开放；23域均不标为全域完成。

- 最终发布：dist/VisionEditor-WpfFrameworkAcceptance；发布程序正常启动关闭exit0（5469ms），启动脚本已切到新版本。ProcessExit新增EditorOnly用于发布产物单独验证，不以重复整套检查替代当前产物关闭。桌面输入阻塞保留，未标成鼠标验收成功。

## 2026-10-17 — 工作流Tab、工具箱与Group上下文闭环

- 逐功能原始依据及SHA256见WORKFLOW-TAB-TOOLBOX-GROUP-CLOSURE.md、workflow-context-sources.json。Group Tab配置打开所属Group；内部画布单次/连续/停止运行根流程且不跳出；专用图像源编辑器、循环父变量和运行输出已接通。
- 按原始实时源查找修正复制：粘贴读取当前参数，删除/改名源不复活快照，同父级保留有效上游及变量订阅，内部图身份独立；Group创建/撤销原子提交，节点与内部名称同步。工具箱取消自定120ms定时器，修复旧分类离开事件、深度插入边界、40px拖影与混合高度对齐。
- 目检运行截图发现图像预览与列表计数不一致，按原ImageListPanelBase.CurImagePath链修复，复验(1/1)，无额外运行或方案修改。截图助手仍只在tests。
- 最新监护记录workflow-context-image-index-lifecycle：Core115、插件18、真实WPF窗口事件364项及主窗关闭全部exit0；Group调用52项通过。实际BMP三轮累积宽[1296,1296,1296]、序号[17,18,19]，复制品独立[42,43,44]。事件/宿主方法驱动加截图目检，不宣称桌面鼠标或真实VM差分验收。既有ROI24项本批未重跑。中间失败和修正保留在批次报告。
- 最终发布dist/VisionEditor-WpfWorkflowContext，独立启动关闭5464ms、exit0，启动脚本更新。导出器核对最新监护成功及操作数，避免失败后发布旧成功记录。
- 原生等待超时仅确认Get/SetProcedureWaitTime及SDK说明；REA能读取二进制，但当前分析器连接未配置。没有猜成流程取消计时器。LoopStep/复合数据/设备回调/.sol往返/全模板/原生视觉差分仍开放，23领域不标完整。未改插件工作树或其他任务进程。

## 2026-10-17 — Group循环、中断与结果状态闭环

- 依据ModuleFrame group-run.c/group-set-param-native.c和安装版IMVSGroupAlgorithmTab.xml，恢复LoopStep非零写入和索引推进，保留index<end条件与末轮等待，不臆造原XML不存在的步长控件。明确缺省1、long推进、int32溢出和10000轮上限的独立边界。单参数和整组零值拒绝均在提交前完成。见GROUP-LOOP-BOUNDARIES.md及源文件SHA256清单。
- 实际BMP阈值/连通域按20/70/120/170四轮执行，与四次独立单轮输出一致；>=120中断保留前两轮和阈值70图像。保存重开、复制、空/相等/禁用范围、负步长中断和溢出保护已验证，调用专项67项。空循环无必需图像时宿主明确失败，不复用旧图像；原生零轮图像行为未差分。
- 实际Group窗口使用真实图像源，配置终值来源、中断选择器/运算符/常量，step2执行0/2/4，>=4只完成两轮；长间隔停止后可禁用循环重跑。首轮377项通过后截图发现旧中断红字残留，修复RefreshResults默认清空并添加断言。最终378项窗口事件通过（59989ms）、Core115/插件18/主窗关闭exit0。一次并发发布时90秒超时轮次保留为失败，未把旧成功记录冒充最终。
- 最终截图目检确认width[1296]、SN[22]且底部旧红字已清除。所有截图助手位于tests；没有改插件工作树或终止其他任务进程。真实VM差分、动态数组索引、复合累积、原生回调和全模板仍开放。
- 新发布目录VisionEditor-WpfGroupLoop；最终发布启动关闭2426ms、exit0，记录由验收表关联。证据目录禁用Git换行转换，保持导出哈希对应提交字节。

## 2026-10-17 — Group动态索引与当前复合类型闭环

- 依据ButtonSelecter/GetBindIndex/GetFrontFilterTreeOperation/VmIO.RelateIndex，新增中断操作数的父级前置标量整数IndexSource，常量Index继续兼容。类型/顺序验证、当前运行准备、越界诊断、复制身份重映射、索引生产者单独删除清理、撤销与保存重开一并实现。不是全插件端口索引恢复；完整原生内嵌方括号/树模板仍有差异。
- 依据VmIODecorate遍历GetAllLeafIO的报告模式及V44数值分支，补当前Circle/Point/Fixture强类型数组和嵌套累积/接口传播。图片继续替换。Region/Components与pointset映射及混合叶报告模式未完成，没有擅自追加所有类型。见GROUP-INDEX-COMPOUND.md和源文件SHA256。
- 实际BMP径向找圆内3外2轮输出6圆，半径叶数组逐项一致；关闭内累积后实际变2圆。点/Fixture经测试字段投影从真实圆构造并跨两层调用，不冒充原生算法工具。数据专项88项、数组31项通过。
- 实际窗口使用真实图像源SN[0,1]，变量索引1在LoopIndex2前中断，并实际返回2圆。来源选择、数字解除/撤销、保存重开和累积关闭后标量运行通过。389项通过后目检发现结果裁切/空白常量框，修复后392项WPF事件通过（57328ms）；最终截图两圆可读、叠加2个圆记录。监护Core115/插件18/主窗关闭exit0。新构建Core和发布程序另附记录。
- 初次Polarity输入错误、构建命名冲突及过时“圆数组必须拒绝”断言均修正，未计为最终成功。原始完整audit仍忽略，精选证据保持字节哈希。截图助手只在tests，未修改插件工作树及其他任务进程。
- 全原生复合字段树、未建模类型、缓存时序和实际VM差分仍开放。新发布目录VisionEditor-WpfGroupIndexCompound，最终启动关闭6477ms、exit0，证据由验收表关联。

## 2026-10-17 复合叶字段与并行逆向闭合子集

主线整合并行审查结果：Components部分Blob字段及像素矩形/整环Region叶投影；Group派生叶改名删除传播；Region/Components根来源链按钮订阅Popup；小地图Shift/Ctrl路由。实际BMP threshold128产生784个连通域，两轮面积1568项、总和1710536；保存重开及父输出验证。41数组、93调用、412实际WPF事件通过，退出监控均exit0。源码依据、界面和未闭合项见 compound-leaf-acceptance.html 及四份并行审计报告。未改插件工作树；不代表全类型/原生ABI或完整控件模板等价。用户新增要求：必要闭合节点构建并启动给用户手动验收；发布启动检查通过后保留人工验收实例。

## 2026-10-17 Group索引正文弹层与复合类型证据

先按原生 `ButtonSelecter`/XAML核对索引状态机，再在Group数组中断操作数接入方括号光标触发、180×180整数树和双击叶提交，复用已有结构化 `IndexSource` 及实际循环执行。真实图像源两轮SN、两圆结果、索引解除/撤销/重开及窗口关闭以416项WPF操作复验；42项数组契约和21项Polygon独立容器检查通过。弹层图片已目检；原生独立订阅窗、统一来源文本、原VM同状态差分仍待完成。Polygon仅为保留`Num/Points`对象边界的托管模型，没有端口/工具集成。Fixture十叶和零尺度差异只完成源审计，未在本批虚构映射。详细范围见FRAMEWORK-INDEX-POLYGON-AUDIT.md、NATIVE-ARRAY-INDEX-EDITOR-AUDIT.md、FIXTURE-NATIVE-FIELD-AUDIT.md。未修改插件工作树或其他任务进程。

## 2026-10-17 两线节点框架边界重整

用户要求两项目并行逆向，并明确插件开发所需的节点公共配置框架也在插件工作树内推进。main新增 PLUGIN-FRAMEWORK-BOUNDARY.md 和 PLUGIN-FRAMEWORK-INTEGRATION-CHECKLIST.md：当前main承担集成管理与任务登记，插件线负责公共配置模型/模板/宿主/配置执行及具体工具，main负责画布/Group/流程/工具发现与最终集成。只读查看插件工作树1d66877及其未提交在制文件，未修改该工作树，也未将其实现计入main完成项。MainWindow、Contracts、GraphExecutor、GraphValidator、WorkflowSubscriptions及配置宿主入口列为共享文件交界；按单一提交所有者和交付提交逐项合流，保留原生证据、托管验证和原厂差分的独立状态。此批仅更新任务清单与边界，无产品代码改动或测试通过声明。

## 2026-10-18 主框架 MF-05：CompatibleTool 版本迁移静态逆向

- 沿 `ProjectManager` → `CompatibleTool.exe` → `ConvertHelper` → `DynamicIOUtils` → 原生 SDK SaveAs/Export 入口，补齐方案、流程、Group 和设计文件的可见迁移步骤与证据哈希；逐阶段转换不等于原生容器实现。
- 发现迁移结果标志在 V4.0/V4.1/V4.2 路径会被后续 V4.3→V4.4 方法覆盖；流程和 Group 对应末级方法在托管反编译中返回 `true`。这形成需原版受控样本核验的失败传播风险，不能直接当作 VM 实际行为。
- 未运行原版 CompatibleTool；没有原生 `.sol` 成对迁移夹具、SDK/native writer 或回滚差分。本轮仅闭合静态证据链，F-19 仍未通过，独立 `.vmx.json` 仍不宣称兼容 `.sol`。详见 [COMPATIBLETOOL-VERSION-MIGRATION.md](COMPATIBLETOOL-VERSION-MIGRATION.md)。

## 2026-10-17 主框架继续：流程等待超时

按`TimeOutSet.xaml/.cs`和`VmProcedureDecorator.SetTimeOutCommandExe`恢复流程菜单入口、370×200配置窗口、0—100000毫秒读写与独立方案持久化。托管执行使用独立的协作取消/清理实现，原生SDK后端的超时语义仍未知，不能称为原生调度等价。证据、已验证范围及缺口见PROCEDURE-WAIT-TIME.md。此批只修改main所有的流程/文档/执行器文件，不改插件工作树。

## 2026-10-17 主框架四线并行：Tab、小地图、Group比较与方案边界

- 共同起点 `0f3a1d45fa34f7343ad81d705fe962f5a5bc53c7`。按文件所有权并行取证与编辑，构建/实际 WPF 操作串行验证；未修改插件工作树。四线没有把通过检查等同于 VM 全域等价。
- 工作流 Tab：依据原 `ProcedureConfigControl` 选中与滚动事件，恢复选中聚焦、近边界固定30单位补滚。程序化离屏 Tab 的 `BringIntoView` 与首 Tab 归零是本宿主适配，详见 `WORKFLOW-TAB-SCROLL-EVIDENCE.md`。
- 小地图：依据原 `ZoomArea.GridEagleEye_MouseDown/MouseMove`，按下仅记录起点，首次实际移动才捕获鼠标。独立 WPF 宿主检查通过；真实硬件拖动、DPI 与原 VM 同状态差分未完成，见 `MINIMAP-POINTER-CAPTURE-EVIDENCE.md`。
- Group 中断：依据原 `CGroupNode::IsBreakLoop`，整数操作数先转 float32；高位相邻整数可能比较相等。直接与实际 Group 运行检查 57 项通过。LoopStep 的 int32 环绕仍与本项目有界 long 推进不同，见 `GROUP-BREAK-INTEGER-FLOAT32.md`。
- 方案存储：反编译确认原生保存/加载在托管层转交 SDK；不能由托管源码推出 `.sol/.solx` 格式。本项目 `.vmx.json` 的相对资源另存、重开和准备失败回滚新增实际检查；23 项流程管理检查通过，见 `SOLUTION-PERSISTENCE-BOUNDARY.md`。
- 完整 WPF 操作链 456 项、框架执行 68 项通过；解决方案含新小地图与 Group 专项项目并以0警告0错误构建。截图 `analysis/full-audit/wpf-workflow-parity.png` 已目检，界面仍存在与原生 VM 的开放差异。完整桌面鼠标、原生方案往返、极值/多屏差分未验收。

## 2026-10-17 独立清单 MF-05：嵌入资源完整性

四条主框架工作线现按不同 MF 清单条目和独立工作树推进，边界见 `FRAMEWORK-FOUR-LANE-OWNERSHIP.md`。MF-05 在独立 `.vmx.json` 保存和加载结构校验中检查嵌入 Base64 字节与声明 SHA256；损坏编码或摘要在进入执行器之前拒绝，失败保存不替换原文件。实际二进制资源的保存/离线读取、错误拒绝共 27 项持久化检查通过，真实图像源 773 项检查通过；原生 `.sol/.solx` 容器与密码格式仍未逆向闭合，见 `SOLUTION-PERSISTENCE-BOUNDARY.md`。此批不修改插件专属持久化或其它 MF 工作树。

## 2026-10-17 四份独立清单首轮集成验收

- MF-01 主界面：标题栏空白区手势不再吞掉菜单、工具栏输入；状态栏流程耗时点击打开实际运行记录明细，算法独立耗时没有契约时显示“—”。详见 `MF01-SHELL-HEADER-GESTURES.md`、`MF01-STATUS-TIMING.md`；主工作区专项 14 项通过。
- MF-02 工作流画布：普通节点与 Group 混合多选按实际高度对齐、分布；连线中点拖动时可见路径、箭头和命中区域同步。详见 `MF02-MIXED-HEIGHT-ALIGNMENT.md`、`MF02-WIRE-DRAG-GEOMETRY.md`；主工作区专项 13 项通过。
- MF-04 Group：输入和运行代理来源弹层过滤数组基数；参数关联按原始代码中的工具身份与参数名筛选，实际两个模块同步后运行、保存重开。详见 `GROUP-SOURCE-CARDINALITY.md`、`GROUP-RELATION-IDENTITY.md`；主工作区专项 15 项通过。
- MF-05 方案存储：本轮独立格式嵌入资源完整性修复见上一节。四线分别属于不同清单，主线顺序合入；合入后的完整 WPF 操作回归 456 项通过，已推送 `main` 到 `79cdef3`。以上专项验收不关闭原生桌面鼠标/DPI、复杂避障、嵌套 Group 代理或 `.sol/.solx` 往返差分。

## 2026-10-17 四份清单后续独立闭合

- MF-01：原 `StatusBar.ZoomCallBackClick` 的百分比按钮复位接入实际画布缩放路径，150% 点击回 100% 且文档修订不变。合入时发现耗时窗口流程切换关闭重入，主线修为先摘监听与引用再关窗；Shell 专项 17 项通过。见 `MF01-STATUS-ZOOM-RESET.md`、`MF01-STATUS-TIMING.md`。
- MF-02：按原 `ZoomArea` 的未裁剪请求位置记录小地图边界点击，修复靠右下边缘反向拖动时机；真实 WPF 鼠标事件、边界、保存重开专项 20 项通过。原生极端缩放/DPI 仍待差分。见 `MF02-MINIMAP-EDGE-DRAG.md`。
- MF-04：按原 `GroupParam` 的 32 字符截取与输入输出合并查重规则生成多选输入名；真实窗口、父流程两订阅及运行、保存重开专项 20 项通过。复合叶 IO/SDK 调用仍未闭合。见 `GROUP-INPUT-MULTISELECT.md`。
- MF-05：独立 `.vmx.json` 被占用时原文件保留、临时文件清理及解除占用后的重试；编辑器路径/基目录/修订/未保存状态在失败时不误提交。持久化 30 项、流程管理 26 项通过。见 `SOLUTION-PERSISTENCE-BOUNDARY.md`。

四线改动顺序合入后，主工作区完整 WPF 操作回归 456 项通过。MF-02 分支曾在第 207 项时间阈值偶发失败，同一构建复跑通过；该项时间敏感性不据此宣称已消除。插件项目 `tool-plugins` 独立在制，本批未合流或编辑其工作树。

## 2026-10-17 四份清单再一轮闭合

- MF-01：按原 `TitleMenueItemData.AreaLocking/AreaUnLocking` 与 `LockDlg` 接通工作区锁定确认，“否”保留编辑，“是”锁定，“不再显示”写独立用户偏好，解锁直接恢复。主工作区 Shell 专项 22 项通过，见 `MF01-WORKAREA-LOCK-PROMPT.md`。原厂提示文案/皮肤及配置确切位置仍未核实。
- MF-02：按原 `Connection` 焦点与 `RequestBringIntoView` 路径，让连线自身获得焦点、选中显示中点且视口不自动跳动；失焦清中点，Delete/Undo/保存重开经过实际 WPF 验证。主工作区画布专项 28 项通过，见 `MF02-WIRE-FOCUS-VIEWPORT.md`。
- MF-04：按原 `VariableManager.ModifyType` 的普通类型分支，在 Group 局部变量改型时解除旧订阅。改型后实际运行报缺失输入、重新绑定后运行成功、保存重开结果保持且父流程变量隔离；主工作区专项 9 项通过。复合类型 IO 重建仍开放，见 `GROUP-LOCAL-VARIABLE-TYPE.md`。
- MF-05：损坏独立方案加载失败不替换当前编辑内容、选中流程、未保存状态或 Undo 历史，流程管理检查 28 项通过。另确认 `analysis/rejected-input-check.sol` 是外部实体拒绝样本，不是原厂有效 `.sol` 样本；见 `SOLUTION-PERSISTENCE-BOUNDARY.md`。

四线顺序合入后，主工作区完整 WPF 操作回归 456 项通过。真实 VM 桌面鼠标/DPI、原生复合类型重建与 `.sol/.solx` 往返仍未关闭；插件工作树未在本批合流。

## 2026-10-18 主框架 MF-05：方案替换后的结果视图边界

- 新增 `MF05-RESULT-LIFECYCLE.md`：追踪原生 CreateNewSolution、方案 LoadBegin/LoadEnd 与 ModuleResultView 的模块源重绑定/数据缓存清理。原生显式清除命令是当前模块级 SDK 调用；方案级运行日志与底部耗时的全局清理没有托管证据，也没有真实 VM 操作差分。
- 当前独立实现的 `HistoryGrid` 是方案运行摘要，粒度不同于原生模块历史。本轮没有为追求表面一致而擅自清空或重命名；需在真实 VM 对“运行后新建方案/加载另一方案”与“切换模块/模块历史清除”做对照，再确定 F-16 的 UI 行为。


## 2026-10-19 三个 framework 工作树收尾

按用户指令审查 framework-shell/canvas/group 的未提交增量；4755个源码/资源/审计文件已在 main 的 analysis/full-audit/framework-retirement-20261019 保全并逐文件SHA-256比对。合入Shell运行策略/帮助/关闭等待、画布路由与复制插线、Group数量配置及专项验证，保留main原有在制改动；tool-plugins不修改。精确基线、源码证据、测试和清理结果见 FRAMEWORK-WORKTREE-RETIREMENT.md。Shell默认键鼠弹层验收仍失败，显式控件事件模式不冒充桌面验收；原生撤销服务、候选资格、ABI及设备释放差分仍开放。


## 2026-10-19 main剩余在制审查

配置修复提交2d6ddd8：审查Group IO/订阅/显示、局部变量及既有WPF夹具；修复输出多选空候选、显示删除漏接事件和搜索提示占用数据；排除pointset→Point数组、POLYGON→Region、无共享状态的独立订阅窗及禁用运行区候选。原始22份在制文件及674份既有审计证据保全在analysis/full-audit/main-pending-review-20261019，含补丁、SHA-256、失败记录和新验证日志。tool-plugins不修改。

新增20项实际WPF审查、6项订阅弹层、完整456项WPF、71项执行及93项调用检查均exit0；截图和方案保存在tests产生的审计目录。构建0错误，既有nullable/装箱警告保留。MF-05保存回调顺序及取消/异常复位说明已按源码修订；FCR-019撤回仅凭CustomVisible判断局部变量目标隐藏的旧推断，改按Tab_Run Params/ISubInfo/SubscribeVisible调用链待验收。安装ImageSource XML默认StitchEnable=True，非本地False默认；不得混称。完整采纳/排除及源路径见MAIN-PENDING-REVIEW.md，未知native/ABI/设备/真实桌面差分继续开放。

## 2026-10-19 源码级总计划第三批

基线6a2cf54，四方向分批使用三个并发子槽，第四位独立复核D02；本批全部子任务已完成，无产品修改、自动推送或tool-plugins操作。A03新增37方法CIL；B01原首次填充8组合成输入44检查通过；C02原配置写入17断言通过，纠正SaveConfig重载存储/缓存边界。D02实际插件PrepareSave/另存/Undo诊断exit1：旧相对路径在新基目录失效，Redo与离线嵌入重开正常；保留passed=false并登记FCR-024，未以有副作用PrepareSave批量改旧历史。报告与下一步见reverse/BATCH3-20261019.md及framework-reverse-plan.json；32包仍未完成，原生/桌面差分继续开放。

## 2026-10-19 源码级总计划第四批

基线8b7b2de，A03/B01/C02三子任务并行，C完成后D02独立复核，root串行验证整合。A03新增20方法CIL及5资源值保全（双名setter与原名恢复通知差别为静态结论，实际WPF未知）；B01原惰性根受控16项通过、30相关方法CIL，确认XML改属性与真实proxy装配副作用；C02原33方法CIL/EH，明确清叶字段、同步/队列顺序与关闭不等待边界。D02真实main32MiB输入/输出及拒绝事务22项通过并独立复核；不是原生格式验收，不覆盖FCR-024资产历史失败。无产品修改、自动推送或tool-plugins访问。报告reverse/BATCH4-20261019.md，台账保留四包开放与后续精确问题；原件/失败/输出及哈希在analysis/full-audit/batch4-20261019。

## 2026-10-19 第五批：退出门槛与进度指标

基线e6bf62a，四线有限子项及独立复核收口。A03隔离blocked：日志独立HKLM选安装配置并watch；B01绑定写入、C01注册、D01保存回调静态done，运行未验证。整包0/32（本批+0），固定子项3静态完成/1阻塞，新明确隔离阻塞1/解除0，原方法运行0、产品修复0。原件/失败/CIL及哈希保存在analysis/full-audit/batch5-20261019；进度规则已写FRAMEWORK-REVERSE-PROGRESS.md与PARALLEL-WORK，阻塞或关闭切片不得无新条件重复派发。FCR-024失败仍开放，tool-plugins未访问、未自动采用来信、未推送。下一批按未满足退出条件选题，详见reverse/BATCH5-20261019.md和台账。

## 2026-10-19 第六批：隔离新前置与受控操作

基线48a0760，固定A03/B02/C02/D02四子项并第四位独立复核。自有子进程RegOverridePredefKey映射成功、宿主观察值不变且自有键清理；原logger装载/watch/output仍未跑。B02提前源码预审静态4/4，不绕过B01/A01依赖；C02原清值叶方法合成对象4案例32断言通过；D02自有保存子进程中断4断言及只读独立复核通过。总任务0/32=0%，本批整包+0，子项3done/1blocked；解除映射前置1，完整运行隔离未解除，产品修复0，原机差分0。审计含所有脚本/API失败，目录analysis/full-audit/batch6-20261019，报告reverse/BATCH6-20261019.md。tool-plugins未访问、不采用来信、不推送；后续只按新条件推进剩余门槛。

## 2026-10-19 全量成果对账与真实进度

按用户指令暂停扩展逆向，仅对账冻结70b3b42的总计划/23域/历史实现/六批源码报告/后续失败。建立plan-units-v1：32包各3具名验收单元，共96；源码51/96=53.1%，实现29/96=30.2%，验证29/96=30.2%，三层同达成28/96=29.2%，68项至少一层未达成。0/32是最终整包关闭率，撤回其作为整体逆向完成率的旧表达。没有部分折半、没有将本次统计工具计新增逆向成果；历史验证非本次重跑，FCR-024资产Undo与Shell默认输入失败继续保留。

权威数据docs/framework-progress-audit.json，逐包逐项证据/边界docs/FRAMEWORK-PROGRESS-RECONCILIATION.md，可复算tests/ProgressAudit/Reconcile.py，来源哈希与剩余CSV在analysis/full-audit/progress-reconciliation-20261019。没有运行新产品/原机验证、没有改产品、没有访问tool-plugins或自动采用来信、没有推送。总计划/PARALLEL/进度规则/任务JSON同步新口径，父包依赖及未关闭状态不变。

## 2026-10-19 第十二批：完整关闭A03.3实施

基线f53aa7b828807e3dfd20875b24b84b03cc8bc41b。四方向三子槽+root，选未完成A03.3并固定原criterion；每容器缩放字段/持久化、画布与Fit消费、非活动页及同页snapshot恢复、owned复制删除身份已实施，Core29及实际WPF29自审核通过。首轮WPF测试缺System.IO编译失败保留日志，修复测试引用后exit0；主项目构建0错误、既有CS8603告警保留。原通知alias、原SDK格式及原机验收未知单列，不作为实施分母。总实施31/96=32.29%，本批+1，剩余65。B06资格合同与C04原生载荷仍有真实缺口，不用报告或另造独立载荷计完成。完整证据reverse/BATCH12-20261019.md、analysis/full-audit/batch12-20261019，提交SHA由HEAD-after.txt保全。tool-plugins不读不改、不自动推送。

## 2026-10-19 第十三批：迁移实施与长稳在途

基线b11594396061226f669f2bfe49d8378f0af27b21。四方向独立审查/实施：恢复A06真实关闭缓冲依赖，REA取得B05真实全局模块；不以假窗口/三标量替代。D新增纯托管运行界面XML迁移CLI，18项自审及真实CLI成功/拒覆盖通过，native分支仍未实施，D04.1 partial。C新增受控30分钟资源基线工具，UTC12:02:03自有PID54372已启动，固定GUID见framework-reverse-plan.json ownedRuns；最早北京时间20:32足时，summary/退出齐备前C07.1 partial不计完成。总实施31/96=32.29%，本轮暂+0；不是以人工验收挡分。所有失败/源代码/实际输出/哈希留batch13审计；tool-plugins不动，不自动推送。下一心跳先收取已有owned run，不重复启动。

## 2026-10-19 第十四批：长稳失败与保存事务

原PID54372长稳实际413.078秒失败于第二SaveAs的File.Move UnauthorizedAccessException，未足1800秒；7项原审计hash匹配，外部子进程退出码旧launcher未采，不虚报。四方向封存/复核/事务测试：新增保存持锁与PrepareSave/Changed阶段22项实际通过，现场瞬时根因仍未知，不归因权限/扫描器、不改产品盲重试。增强probe未来phase/HResult/目标前后hash，并加只等自有子进程的exit.json launcher；构建0警告0错误，launcher仅语法检查未执行，不重跑长稳。C07.1验证记failed、实施partial，FAIL-STABILITY-SAVEAS登记；总实施31/96=32.29%，本轮+0，剩余65。详见reverse/BATCH14-20261019.md及batch14审计，tool-plugins不动，不自动推送。

## 2026-10-19 第十五批：A01.2实施完成

基线f7709efea904063991ca87a8c71b68a3d4ae2614，四方向三子槽+root。A01.2实际palette首次/hover/Expanded/画布菜单同状态、失败sticky前缀、成功排序和显式Refresh新身份完整实施，17项实际MainWindow控件自审通过，计完成+1；JSON载体/原锚点差异明示，不关闭A01.3/A07。另落实500条有界流程摘要/稳定序号/真实selected模块耗时/算法不可用，以及原方案显式声明图和ProceZoomIndex只读解释；C03.3/D03.1原native未知仍partial。105项限定自审通过，Core首轮漏目录参数拒绝保留、正确参数后通过；既有CS8603保留。总实施32/96=33.33%，剩余64。证据reverse/BATCH15-20261019.md与batch15审计；长稳失败未重跑，tool-plugins未访问、不采用来信、不自动推送。

## 2026-10-19 第十六批：C01.1实施完成

基线258490780a0a31fe3b1a0a7df99c16e16d319358。真实DocumentHistoryEngine root/part旧新值、首次Undo补new、predicate/forbid/replay/lazy准入、原priority/CopyModule排序与异常迁栈实施，并将GraphEditorDocument旧Snapshot栈真正替换；配置布尔scope保留，缩放回放间仅合并视图为独立适配。355项限定自审核全部exit0，既有CS8603保留，C01.1计done。原SDK/TempFiles/Recover仍各项未完成。A当前合同托管loader有实际fixture注册/执行/存读但startup锁未接，A07.1仍partial并登记FCR-026；B候选/gvar缺真实能力不猜造。总实施33/96=34.38%，本批+1，剩余63。原长稳失败未重跑，tool-plugins未读未改，不采用来信、不自动推送。报告reverse/BATCH16-20261019.md和batch16审计保全完整源/失败/未知/hash。
