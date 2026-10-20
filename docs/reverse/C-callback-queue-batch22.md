# 第二十二批 C：回调队列实施边界

固定目标为 C02.3 的回调缓存投递合同与 main 实际消费者接入。先保修前观察，再修改。tool-plugins、原 SDK、设备和原程序集运行均不在本批执行范围。

## 精确原证

复用 `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs` 与第四批相应 CIL/EH 证据，源哈希保存在 `analysis/full-audit/batch22-20261020/C/source-before-hashes.json`。

- `.cctor`：alive=true，receive/send 两数组各10000，cache默认false，启动 BelowNormal 后台线程。
- `EnqueueForCallBack`：cache=false同栈分发；cache=true锁内append到receive，receive满仅记录overflow并丢弃当前项。当前send批次分发期间仍可另外接收10000，不是总待处理量最多10000。
- `CreateCallBackQueue`：每轮swap双buffer并清receive计数，按send顺序分发并在成功后清槽；批次期间不检查alive。订阅者异常没有catch，后续清槽/分发不能保证；不将未运行的进程后果写作已知。
- `MProcessManager_OnWorkStatusCallBack`：count非零先Begin/End处理，再Enqueue，再同步ModuleResult。count零跳过Begin/End与Enqueue，仍同步ModuleResult。
- `SetValue` 普通模块结果：先同步ModuleResult，再Enqueue，顺序不能与WorkStatus合并。
- `ExpandCallBackQueueLength`：仅更大值时重置收发数组及两计数；源码注释要求未加载方案，但方法未强制检查。
- `ShutDownCallBackQueue`：只置alive=false、线程字段null，不join、不drain、不取消SDK。已经swap的batch可继续完成；原cache=false在关闭后仍可同栈调用，cache=true仍可入receive但不会再启动worker。

## 修前可复验观察

`tests/CallbackQueueBeforeChecks22` 仅执行自有模块的真实 MainWindow StartRun，输出2，并检查尚无frameworkCallbacks集成入口。它是缺失入口的观察，不冒充旧产品已有队列的容量/溢出失败。camera discovery在构造MainWindow前禁用，25秒期限，统一failed.json。待根线串行执行后补实际产物；没有由子线自行构建。

## 仍需明确的合同

main现有RunCore是合作取消、epoch隔离、最终session发布；原VM队列不检查generation。任何main队列闭窗/换方案隔离必须标明独立安全适配，不能照搬原无隔离导致旧回调写新图。订阅异常的托管留证/停止worker属于安全适配，不声称已证原进程行为。原清_value、merge/RelateIO恢复与图像SDK生命周期不因queue测试通过自动完成。

## 本线已落地的独占实现（待串行运行）

`src/VisionFramework/FrameworkCallbackQueue.cs` 实现泛型双buffer队列，默认10000/不缓存；Enqueue返回Dispatched/Queued/Overflow；PublishWorkStatus先入队后同步（count零仅同步），PublishModuleResult先同步后入队。关闭不等待线程、不清receive、不阻止原style后续投递，已swap的批次继续；Expand仅接受更大值并重置pending。原泛型接收对象由调用方持有，队列不Dispose载荷，不能把丢弃回调当作释放图像/SDK资源。

两个明确的独立实现差异：worker线程以Thread.Sleep(1)让步代替原SpinWait.SpinUntil(false,1)；worker订阅异常写入DispatchErrors并停worker，防止未处理后台异常，无自动重试、不伪称原厂进程后果。直调分支订阅异常仍同步传播。Expand的注释继续限定未活动分发时调用，不宣称并发扩容与源数组竞态等价。

`tests/CallbackQueueChecks22` 使用自有线程/TCS式事件控制，确定性覆盖默认直调顺序/count0、活动send期间另收10000与overflow丢弃/FIFO、关闭后当前batch完成而receive保留、不join语义、关闭后直调/缓存仍接收、扩容与异常诊断。没有原生回调线程或设备。

目前以上独占文件已冻结等待根线构建。只有根线真实MainWindow回调发布/消费者接入并自审核，才构成main实际使用的实现；孤立helper及单测不作为C02.3已完成依据。

## 实际纯helper自审核

按根线授权执行纯测试。首次共享obj构建CS2012 AccessDenied；转自有GUID --artifacts-path仍在VisionData.dll写入时同错，两次失败均保留。单次require_escalated在同一自有输出构建成功，0警告/0错误，执行exit0，**15/15通过**。真实summary：`analysis/full-audit/batch22-20261020/C/callback-queue-74767acd9f9946e580aaaca760e8f8bd/summary.json`。本线独立读取核验；`actual-helper-hashes.json`包含产品helper、探针及summary哈希。构建日志位于`queue-isolated-15b3c3b1da464b64bb57c103b504b925/build-elevated.log`，前次隔离失败日志build.log同目录保留。

建议最小实际host接缝：完整流程owner验证后调用PublishModuleResult；同步commit执行StoreWorkflowCallResults/运行计数/历史，异步观察者刷新当前视图与配置结果。默认cache=false保持原main当前顺序；cache=true时worker通过Dispatcher.BeginInvoke交回UI，真正执行动作时再验证epoch、solution、scope、windowClosed，防止已入队旧方案刷新。Close只关闭队列不Join/drain；不要让UI Close等待worker线程，避免跨Dispatcher死锁。源WorkStatus还有独立的Begin→enqueue async→syncModuleResult消费者，而main没有该后一同步入口，不能仅造一个空委托就声称已实施。

**当前只有helper合同落地并经自审核，尚未接实际main消费者，因此C02.3 implementation保持partial、完成分子+0。** 共享文件由根线唯一写，子线未改MainWindow/WorkflowExecution。原清叶/原生回调/SDK图像所有权继续独立开放。
