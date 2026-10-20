# C07.3 实际主窗故障恢复矩阵（第二十批C线）

固定基线3996ad1；2026-10-20。子线只新增自有测试及报告，共享执行/释放产品文件由根线唯一修改。没有SDK、设备、插件在制目录、原TempFiles服务或长稳重跑。

## 冻结门槛与计分边界

本批必须经过真实MainWindow.StartRunCore/RunCore/ContinuousRun，不能把C08/Core通过替代宿主恢复：

1. throw、显式Failed、错误输出、异步Dispose故障、实际owner取消、现有合作式节点超时六类单次运行，检查真实结果状态/错误码、owner和continuous状态复位；每类随后正常重启成功。
2. 六类continuous StopOnNg停止及随后重启；非owner取消的五类另测StopOnNg=false故障后下一轮成功，并保留实际历史错误与成功两条。
3. 真实自有主窗Close分别等待受控执行与异步清理门，随后模块及自有scope服务仅释放一次。
4. 原scope释放故障须先用自有对象复现，再验证逐项尝试、保留诊断、不自动重试、不破坏pending lease延迟释放。

台账C07.3目前明确尚缺长稳并发故障矩阵。本批是**有限托管host故障/恢复自审核**，不运行30分钟、原SDK、设备或资源增长负载；即使本批有限矩阵全部通过，也保持C07.3 partial，不改原门槛争取分数。C07.1已知SaveAs失败与C07.2持续并发仍独立保留。

## 原证据与当前支持合同

原依据复用 `docs/MF05-CONTINUOUS-RUN-CALLCHAIN.md`、`docs/MF05-PROCEDURE-RUN-CALLCHAIN.md`、`docs/DEBUG-SESSION-CONTRACT.md`：原托管入口检查busy/ready/loading，原单次调用SelfRun，连续等待回调，停止清连续标志并等待任务/解绑。原SDK能否强停正执行算法、设备内部释放时序及原全局故障恢复仍未知，不能推导当前合作取消与native等价。

当前GraphExecutor在finally实际await模块IAsyncDisposable；异常映射ModuleException，显式失败保留ErrorCode，错误输出映射InvalidOutput，owner取消与节点timeout分别记录Cancelled/TimedOut。主窗RunCore发布session/摘要，StartRunCore finally归还activeRun；ContinuousRun按StopOnNg及owner取消处理；关闭路径等待当前owner，再释放scope。测试针对这些明确支持域，而非猜测原生错误码语义。

## 新增可复验测试

`tests/HostRecoveryChecks20`注册自有managed受控模块并实际调用MainWindow，含六类single+restart、六类StopOnNg+restart、五类continueOnNg、Close双门及scope服务。状态/错误码读取真实workflowSessions，历史读取真实FrameworkRunHistory，activeRun/runningProcedureId/continuousRun从宿主实例读。每个等待有25秒截止，异常写自有failed.json并退出1；WPF构造前禁用自动相机发现，不调用原厂或设备。自有offscreen窗口仅测托管事件，不称交互桌面验收。

`tests/ScopeCleanupFailureChecks20`为纯Framework对象，不启动WPF或设备，支持`observe-unfixed`与修后默认模式：

- 原Release的states.Concat(ownedServices)中唯一state先抛Dispose，观察后续service未释放、取消源Dispose被跳过。
- 自有CancellationToken.Register回调抛错，观察原scope.Dispose异常传播。
- 每阶段在断言前先保存实际resource-observation.json或cancel-observation.json，失败不会只留下“断言失败”而丢原异常。
- 修后通过反射读取新CleanupFailures诊断，不编译依赖修前不存在的类型；要求其余资源/取消源均尝试释放，无自动重试，pending lease仍延迟。

## 释放修复建议与原生边界

当前源显示ModuleExecutionScope.Release逐个Dispose没有catch；stopping.Cancel的回调异常也会在finally后传播，可打断MainWindow.OnClosed的后续清理。这是可控制的本地框架故障路径，不宣称原VM具有同一缺陷。

建议根线最小local hardening：取消回调、逐个owned资源、最后取消源Dispose均各自尝试并记录Phase/ResourceType/Error；保留失败对象/异常供审计，不自动重复Dispose；finally路径不向主窗关闭抛未处理异常，且不能提前释放仍由pending lease借用的服务。共享ModuleExecutionScope.cs及FCR由根线串行处理。

子线未构建。修前及修后实际结果、源哈希与根线集成后由真实产物追加，不预填通过。

## 修前实际证据与修复只读审查

根线修前实际退出0，子线已核对 `analysis/full-audit/batch20-20261020/C/scope-cleanup-517f28d18e104ab1be5569e1f468070b/summary.json`：8项counterexample检查，badAttempts=1/laterAttempts=0/tokenReleased=false；resourceError保留原Release/Dispose完整栈。自有取消回调callbacks=1并传播AggregateException，虽随后cancelService=1/cancelTokenReleased=true，仍可打断主窗外层关闭。它是修前失败机制的成功复验，不算修复通过。

根线最小修复已只读审查：ModuleScopeCleanupFailure包含Phase、ResourceType、原Exception；CleanupFailures在root锁下返回副本。Cancel异常记录后仍进入原finally；Release对每个owned资源分别catch，再总尝试取消源Dispose；failedResources强引用保留失败对象，不自动重试。closing/cancellationComplete/leases/releaseStarted门保持，borrowed scope的Dispose仍不关闭root，最后lease退出触发一次释放。未见本固定终止路径的新所有权缺口，实际修后和host矩阵仍待产物。

局限必须保留：这是**终止scope/主窗关闭路径的本地异常隔离**，不是原VM释放策略。RegisterService替换、RemoveNode和RetainProcedureNodes的即时Dispose仍可能抛异常，不在本补丁中声称安全；诊断/失败对象只随scope存活保留，不表示失败资源已释放或有自动恢复。OOM等无法记录诊断的不可恢复故障、设备和并发长稳没有被本测试覆盖。

## FCR-030：终止scope释放失败诊断（根线登记）

- Producer：VisionFramework.ModuleExecutionScope；consumer：main主窗关闭/方案替换、拥有该scope的会话与诊断工具。插件在制项目不自动同步或采用。
- Payload：新增只读CleanupFailures，条目为ModuleScopeCleanupFailure(Phase,ResourceType,Exception Error)；Phase目前为cancel/resource/token，保留原异常对象。需要序列化时由诊断工具选择Error.ToString等安全文本，不把Exception原样当持久化方案字段。
- Lifecycle：Dispose请求取消→捕获回调故障→等待最后lease→逐owned资源尝试→尝试取消源Dispose→保留失败诊断/对象。重复Dispose不重试原失败项，不提前释放在用服务。
- Compatibility：不变Enter/Dispose/ForProcedure等原签名，新增可选诊断读取；终止Dispose不再把单项释放异常抛给关闭链，消费者如原先依赖catch获取故障应改读CleanupFailures。并非所有scope即时操作都改成不抛。
- Acceptance：保留本次修前8项实际反例；修后须验证后续资源与token总尝试、cancel故障不外抛、诊断存在、失败对象不自动二次释放、pending lease仍推迟释放；实际MainWindow关闭和故障后重启另由HostRecoveryChecks20验证。原生VM差分及长稳并发仍未关闭。

## 最终实际自审核

子线只读核对根线实际退出0产物，passed与checks数组数目均一致：

| 实际专项 | 结果 | summary |
|---|---:|---|
| 原scope释放缺陷特征化 | 8/8反例复验 | `analysis/full-audit/batch20-20261020/C/scope-cleanup-517f28d18e104ab1be5569e1f468070b/summary.json` |
| scope终止异常隔离修后 | 8/8 | `analysis/full-audit/batch20-20261020/C/scope-cleanup-fbc972f86a5747e498b5dabde7efea55/summary.json` |
| 真实MainWindow故障/恢复/关闭 | 89/89 | `analysis/full-audit/batch20-20261020/C/host-recovery-8789d05f0c3b4e1fbf9391085bb0b10e/summary.json` |

修后scope具体结果：首失败资源尝试1次、后续service释放1次；resourceError=null、tokenReleased=true、resourceDiagnostics=1。取消回调1次、cancelError=null、cancelService=1、cancelTokenReleased=true、cancelDiagnostics=1。原失败确实被隔离而非仅忽略断言，失败诊断仍在，重复Dispose和pending lease检查通过。

实际host observations含29个结果状态截面：六类single故障及六次随后重启、六类continuous StopOnNg及六次随后重启、五类continueOnNg的第二轮成功；后者Calls/DisposeStarted/DisposeCompleted均为2。错误记录分别为ModuleException、FixtureFailure、InvalidOutput、Cancelled、Timeout，恢复记录为Success；最后三个断言另验证实际Close等待执行/异步清理以及模块和scope服务只释放一次。没有用旧Core检查替代这些宿主路径。

最终判断：本轮实际交付并自审核通过有限host恢复矩阵及本地终止释放加固；**C07.3保持partial**，不把89项断言或修前/修后重复次数转换为96单元完成率。未运行C07.1足时长稳、未完成C07.2持续并发/增长与长稳故障矩阵，原SDK/设备及交互桌面差分仍未知。根线报告的原CS8603警告仍保留，不描述为零警告构建。

最终核对及源码/报告/summary哈希分别见同审计目录 `final-summary-verification.json`、`final-review-hashes.json`。无子线构建或共享产品修改，tool-plugins未动。
