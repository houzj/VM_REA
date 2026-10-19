# Group步长、中断及运行边界闭环

## 2026-10-18：隐藏负步长与禁用循环

原始 `analysis/full-audit/group-native-audit/group-set-param-native.c:59–66` 显示 setter 仅拒绝零，负数仍写入 int32 步长字段；`group-run.c:314–319,534–539,1007–1008` 显示正向范围取该步长，初始及每轮终止都只判断 `index < end`，末尾是 int32 加法。静态推导：升序配负步长不会向终点收敛；降序范围无论负步长如何都不会进入该循环；int32 溢出可能环绕。后两种原生动态状态码与实际终止仍未测，不从伪代码推断完整结果。

发现独立实现关闭循环后虽把范围归一为 `[0,1)`，仍沿用保存的负 `LoopStep`，会在一次执行后继续反向迭代。现同时归一步长为 1，使禁用循环的独立单次语义一致；启用时继续接受原生 setter 允许的非零负值，保留可取消与独立 10000 次/越界防护。没有把负步长拒绝写成原生规则。

专项复验：真实 WPF Group 窗口切换“循环使能”并点击“单次运行”，保存的 `LoopStep=int.MinValue` 在禁用时仅运行索引 0；启用后运行索引 0 和 `int.MinValue`，下一索引越界时独立宿主报告失败；撤销、重做、保存重开保持相同行为（`VisionEditor.GroupNegativeStepChecks` 6 项）。`VisionFramework.GroupLoopChecks` 20 项包括负步长升序主动取消、负步长降序零执行与 int32 上界的独立 long 推进；既有 30ms 计时取消断言在高并发下曾偶发失败，原构建不变重跑通过。上述是本项目 WPF/运行数据验收，不是 VM 动态差分。

基线：58d9ca7。范围是现有Group循环的索引推进、配置数据、实际图像结果和停止重启；不宣称全Group完成。

## 原始依据与实现决策

|依据|确认行为|本次处理|
|---|---|---|
|ModuleFrame CGroupNode::DoWork，0x1800a0930，group-run.c:314–319/534–539/1007–1008|起点小于终点时使用字段0x234作为步长，执行条件仍为index < end，末尾按步长累加|修复固定index++；不自行实现倒序范围|
|group-set-param-native.c:59–66，0x1800a3200|LoopStep为int32，0返回0xe0000001，非零写入0x234|补齐可保存的引擎参数；整组配置验证及执行读取均拒绝零|
|group-run.c:173–174/235–254|未启用循环时范围默认为0..1，启用才从订阅取范围|保留现有禁用单次行为，验证带步长配置时仍正确|
|group-run.c:927–931|下一轮输出准备有nextIndex < end判断，但间隔等待独立执行，包括末轮|保留末轮等待；没有将输出准备条件误套到等待|
|原安装IMVSGroupAlgorithmTab.xml，EnableLoop/LoopStartCount/LoopEndCount/LoopTimeGap/EnableBreak及DataType触发器|原循环界面没有LoopStep控件；范围从来源选择器绑定，关闭循环隐藏依赖控件|不臆造步长控件；通过保存的引擎配置复验实际原有窗口、来源选择、中断类型/运算符/常量及运行|

原始证据文件、SHA256见group-loop-boundary-sources.json。反编译C是分析器伪代码，不是厂商原始C++。静态控制流已确认，不等于运行过原生所有边界。

## 2026-10-19：有符号步长、溢出与取消/异常清理审计

### 逐项源码对照

|边界|VM原始证据|当前main实现|审计判断|
|---|---|---|---|
|符号接受|`analysis/full-audit/group-native-audit/group-set-param-native.c::FUN_1800a3200` 的 `LoopStep` 分支调用 `atoi`，仅当结果为0返回 `0xe0000001`，其余 int32 写入 `this+0x234`|`GroupLoop.Parameters` 范围 int32；`GroupLoop.Read` 拒绝0，但保留非零负值|一致于可见的非零判定。超范围字符串 `atoi` 的实际结果不能由反编译 C 推断|
|方向与边界|`group-run.c::CGroupNode::DoWork`：先以起止比较选择 `local_2e4 = LoopStep` 或 `-LoopStep`（314–319）；初始 `iVar15=start`，只有 `iVar15 < end` 才入循环（534–539），每轮末尾做 int32 `iVar15 = iVar15 + local_2e4` 后仍以同一 `< end` 判断（1007–1008）|`GroupLoop.Increment` 复用起止比较并把有符号步长提升到 `long`；`SolutionExecutor` 以 `[Start,End)` 条件迭代|方向选择与严格上界在可见控制流一致。负步长和 start>=end 造成不收敛/空范围的结论仅限该可见比较路径；native 运行态结果/状态码没有原机差分|
|迭代状态/收尾|`local_2ec` 记录轮数；正常尾部汇总状态，释放 `local_2d0` 链表元素/容器，再解锁 `_Mtx_unlock` 并返回（group-run.c:1144–1185）|Group 每次调用新建 `GroupOutputAccumulator`；每轮成功才加入；失败结果不加入并终止；循环间隔取消由 `Task.Delay(token)` 抛取消|可证托管每次执行隔离累计器；可见 native 正常收尾清理容器和锁。不能据此证明 C++ 异常/硬件中断所有路径均走同一清理，也不能称托管取消回调等价|
|32位溢出|伪代码末尾保留 `iVar15 + local_2e4` 的 int32 计算；反编译的 C++ 有符号溢出语义不可从该行确定，原机环绕/终止行为无运行证据|托管以 `long` 推进；进入轮体前遇到 int32 外索引就抛独立防护异常；超过10000轮也失败|这是刻意的防无限循环/避免托管溢出，不是native溢出复刻。上溢越过 end 会自然退出；下溢仍小于 end 时会触发保护错误|

### 取消与异常数据路径

本地 `SolutionExecutor.Execute.Call` 每个 Group 调用各自创建 `GroupOutputAccumulator`。内部图每轮由 `Execute(body.Id, ..., childToken, ...)` 执行；`GraphExecutor.RunAsync` 在节点调用结束后先检查取消令牌，之后才处理节点状态。Group 只对 `SessionStatus.Success` 调 `accumulator.Add`，失败状态立即停止迭代；父流程的 Group 节点在取消令牌已请求时记为 `Cancelled`，普通模块异常记为 `Failed`。间隔等待发生在成功轮后且受同一个 token 控制。该设计没有跨 invocation 的累加器字段，已完成部分留在局部 child result/诊断调用记录；根流程不把失败 Group 作为成功输出消费。内部节点超时的 native映射仍不清楚。

为复核上述闭环，在 `tests/VisionFramework.GroupLoopChecks/Program.cs` 增加确定性场景：节点执行中同步触发父取消、第二轮抛出普通异常、失败后立即重新执行同一 Group。预期观测是取消/失败的根 `WorkflowOutputs` 没有成功 Group 输出，执行索引只包含已进入的轮次，下一 invocation 从空状态运行并得到新结果。首轮 `dotnet run --project tests/VisionFramework.GroupLoopChecks/VisionFramework.GroupLoopChecks.csproj --configuration MF04` 曾在测试工程创建 `bin/MF04/net10.0`、`obj/MF04/net10.0/refint/ref` 时收到 `MSB3191/MSB4018 UnauthorizedAccessException: Access denied`，未进入编译/断言。

### 2026-10-19 隔离复验

静态追查未发现窗口 Dispatcher、同步等待或无界信号等待。测试使用顶层 `await`；两处间隔仅为 100ms，并受 30ms timeout/cancellation token 取消。负步长场景在测试模块执行索引 `-3` 时同步调用 `CancellationTokenSource.Cancel()`；token 沿 Group child token、子流程链接 token 传入 `GraphExecutor`，取消后节点状态为 `Cancelled`，Group runner 对非 Success 状态立即停止。因此按本地代码路径预计有限结束。此前长时间未返回的具体原因因原始控制台会话 ID 未留存，无法事后证实。

在 Canvas 释放唯一构建槽后，以 MF01 的临时根 `Directory.Build.props` 将每个项目 `obj/bin` 分流到 `analysis/full-audit/build-mf04/{obj,bin}/$(MSBuildProjectName)`，并排除旧/新 `obj/bin` 与隔离目录；`dotnet msbuild ... -getProperty` 确认绝对路径位于 framework-group worktree。单次运行 `dotnet run --project tests/VisionFramework.GroupLoopChecks/VisionFramework.GroupLoopChecks.csproj --configuration MF04 -p:BuildInParallel=false -p:UseSharedCompilation=false` 使用受监控的 session `7345`，控制台完整输出 `PASS 23 Group loop checks`，退出码 `0`。包含负步长同步取消、父流程 timeout/cancel、迭代中取消不发布局部成功输出、异常后失败及同一 Group 立即成功重跑并输出新值。该次结果确认本地 runner 用例通过；不证明与 VM native 异常/中断返回码等价。

前一轮长时间进程已由协调者按用户授权结束；本轮仅通过 session `7345` 完成运行。根 props 和 `analysis/full-audit/build-mf04` 是本线本轮临时项，已在验证它们解析到当前 Group worktree 后清理。此前未能返回的原因仍未知，结论依据本轮完整控制台输出而非猜测。

### 当前边界

静态源对照暂未发现需要改动 `GroupLoop` / `SolutionExecutor` 的确定性错误：可见符号、步进和上界形式一致，`long` 与10000轮限制是明示的独立防护。仍未证实 VM 对 int32 溢出、无中断负步长、内部节点异常、线程取消/硬件 abort 的返回状态和结果释放语义；不得把这些行为猜成托管实现合同。此审计不改算法或native状态映射。构建及测试待 slot 后执行；证据不包含VM原机差分。

## 真实数据和界面复验

- 使用实际BMP、阈值及连通域工具，LoopIndex订阅到实际阈值参数。步长50、范围20..200运行20/70/120/170四轮，累积结果逐项等于四次独立单轮结果，最终图像像素等于阈值170的实际图像处理输出。
- 索引>=120中断只执行20/70，保留两轮累积及最后阈值70图像。保存重开及复制后的Group保留步长、索引引用、中断与实际输出。
- start>=end无执行，不复用上次图像。若声明必需图像输出，零轮没有图像，当前宿主明确报缺失；这属于独立类型契约，原生零轮图像发布语义仍需差分。
- 关闭循环按默认索引0执行一次；零步长配置拒绝且修订不变。
- 非零负步长不等于倒序范围。按回收控制流的less-than条件执行，空内部图配索引<=-2中断时从0/-1执行两轮，在-2中断。未将该例推广为原生负步长完整等价。
- 实际WPF窗口用真实图像源，保存step2，经原有来源选择器绑定end5，检查0/2/4三轮；再经窗口中断选择器、运算符、常量设置在4前停止。间隔等待中点停止，检查取消状态、旧中断提示清除及禁用循环后的重新运行。

## 仍开放

- 原生整数溢出、负步长无中断的结束行为及状态码未差分；托管实现使用long推进，超出int32或超过10000轮明确失败，防止环绕/无界执行。这是独立保护策略。
- LoopStep历史配置缺失时默认1是兼容原有单步方案的托管默认；厂商所有版本/配置默认和原生ABI仍未确认。
- 动态数组比较索引、所有复合累积、原生实例回调、上游准备、完整控件模板和真实VM桌面差分仍开放。
- 截图只由tests生成。窗口复验使用真实WPF控件事件及部分宿主方法，不冒充桌面鼠标输入或原VM截图。

## 截图发现并修复

首轮377项窗口事件通过后，逐张检查wpf-group-stepped-break.png和wpf-group-loop-disabled.png，发现成功重跑已返回单个新结果SN22，但底部还留有旧的“索引4，已执行2次”中断红字。当前Error文本是独立宿主汇总控件；本次修复RefreshResults在没有当前错误/中断/诊断时明确清空，避免旧状态伪装成当前状态。新增成功重跑后文字为空的检查，并重新生成和目检最终截图，不将修复前截图作为最终证据。

首次增加提示清除断言的复验在并发发布期间于第326项后超过90秒监护时限，该轮已标失败并由监护结束其自有进程。随后停止并发构建、串行重新构建并复验；最终状态以验收表关联的最新完成轮次为准，不使用旧成功JSON。证据目录通过.gitattributes禁用换行转换，确保提交、拉取后文件字节与记录SHA256一致。
