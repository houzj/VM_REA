# C02 共享执行生命周期整块交付

当前各项交付条件、已完成证据和未通过的原厂门槛见 [C02 / C03 退出审计](C02-C03-delivery-audit-20261111.md)。

## 原厂源码核对

本轮复用了已归档的源码与原件。下列是源码和静态 PE 观察。

| 环节 | 已证实的行为 |
|---|---|
| 单次提交 | `analysis/decompiled/VM.Core/VM.Core/VmProcedure.cs:1094` 的 `VmProcedure.Run` 调 `ProcessManager.SyncExecuteOnce`，SDK 抛 `VmException` 时清 `IsRunning` 并向上传递。 |
| 等待 | `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ProcessManager.cs:277` 按流程 ID 减 10000 定位 1000 槽，清运行标志，再提交 `IMVS_ExecuteOnce_V30`。非零码立即抛错，成功且要求等待时才进 `PrecisionWait`。 |
| 回调 | 同文件 `GetWorkStatusCallBackInfo`（630）在工作状态为 0、流程 ID 合法时写正常标志和完成标志，再分发事件。已追到原生正常标志的每轮初始化、模块执行返回聚合及长格式通知字段。调度器跳过超时模块时绕过该模块执行返回的清零路径，同轮实际值仍待宿主采集。 |
| 缓存与发布 | `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs:4067` 的工作状态路径先发全局事件，再解析目标流程。非零计数的结束分支写计数、耗时及待通知缓存。开始分支只在清结果开关开启且计数大于 1 时清叶输出。工作状态先入队再通知同步结果，`SetValue` 模块结果分支（4312 附近）顺序相反。 |
| 停止 | `ProcessManager.cs:389–420` 分别提交连续运行与停止。停止请求携带末参 3，非零码保留为 `VmException`。归档原生停止链的实际等待由 ModuleFrame 的固定 3000 ms 分支决定。托管等待、原生事件、状态回调和最终资源收尾分列如下。 |
| 执行期等待阈值 | 归档版 `0x41f` 经代理命令 `0xBDD` 将 `waitTime` 写入 ModuleFrame 流程记录 `+0x170`。调度器对类型 3/5 在进入模块前比较本轮耗时，超出阈值时跳过该次调用。命中模块结果记录时写入 `0xE0000009`，可沿已核实的结果字段桥进入模块回调。流程停止动作另由收尾状态构造，待核对同轮的流程正常标志和在途模块行为。此前的 `0x7FE` 已确定为流程导出进度。 |

详细原件标识、静态地址和复验入口见 [原生等待传输](C02-native-wait-transport-20261022.md) 与 [回调证据](C02-callbacks-batch4.md)。

### 停止请求、返回码与完成边界

| 阶段 | 已核对的调用与行为 |
|---|---|
| SDK 提交 | 归档 `IMVS_StopExecute_V30`（VA `0x180647920`）校验句柄后，在 `0x180647A10` 调用 `CServerManager::SM_StopExecute_V30`（`0x1806702F0`）。后者拒绝非正流程 ID，按 `%d-%u-%u` 写入 `0x408`、流程 ID、`WaitTime`，提交给 `0x1806C00D0`，保留传输的非零返回。 |
| 服务端消费 | `PF_M_MsgDeal` 的 `0x408` 分支在 `0x180604508` 调 `CiMVSWorkJob::ControlStopExecuteByProcId`（`0x180619670`）。`0x1806198D1` 和 `0x1806198EA` 分别转换流程 ID、等待参数。前者保存在 EBX，后者的 EAX 在下一条指令被覆盖。流程记录不存在返回 `0xE0000316`。命中记录后经 `record+0x218` 内嵌接口的 vtable `+0x10` 调用。该表在已核实的构造类型上指向 `0x1806A4120`，尾调用 `0x1805E3640` 时重新装载管理器及流程 ID，未使用刚解析的等待参数。处理器按实际返回码生成 `OK` 或 `Err*%d`。 |
| ModuleFrame 停止 | 归档 `0xBC9` 代理链连到 `VM_M_StopRunProcedure`（VA `0x18013C250`）。普通流程记录分支调用 `0x1800DD030(record, 0)`。活动分支设置 `+0x90/+0x80` 的事件，清连续标志，等待 `+0x88`。常量 `0xBB8` 即 3000 ms，由 helper 本身选取。等待超时调用后续辅助维护并返回 `0xE0000300`。此错误也可由前置状态拒绝产生。只有 `WAIT_TIMEOUT` 在该比较点单独分支，返回 0 不能反推等待结果一定是已发信号。 |
| 托管停止失败 | `VmProcedure.ContinuousRunEnable=false` 在更新属性前调用 `RefreshUIStopExecute`。后台 `StopExecute` 抛出时，赋值 `isStopEnd=true` 被跳过。等待器 `SyncWaitor.Wait(ref bool)` 无期限，该路径没有把后台任务错误传回等待线程。`VmProcedure.IsForbidden=true` 使用同步 `StopExecute`，先更新屏蔽状态再停，失败进入记录日志的 catch，跳过后面的属性通知。 |
| 回调与收尾 | 归档 worker `0x1800DD330` 先 signal `+0x88`，再发状态、调用可选回调并 signal `+0x98`。托管 `CVmObjectBase` 将平台消息 772 解包，经 `ProcessManager` 转发到 `VmSolution.MProcessManager_OnModuleStopRunProcessStatusCallBack`，先发事件并更新内部状态，随后仅当动作值为 1/2/3 且存在对应流程时更新该流程并入通知字典。停止返回、状态通知和资源收尾各有独立观察点。 |

已确认的 3000 ms 是**原生停止等待**。它不提供 `SetProcedureWaitTime` 配置触发执行中断的证据，也不能作为自研 `WaitTimeMs` 的等价依据。托管失败分支的描述来自实际控制流，现场是否进入该路径、其他停止入口和回调的实际时序继续保留为原厂运行取证项。

原件位于 `analysis/full-audit/b08-native-20261030`。归档 SDK、PlatformManager、ModuleFrame 的 SHA-256 分别为 `B62D7E15588CCDA6671228AEBC11FEC3E27ACA878F0E03067E44B17D5A54A9A1`、`06D86F51BEBFEBA357D1B79297E20A3B0A70580A40AF7B77C8ABDD6C38CA0624`、`45C57BC46F72FD1F9E0B3A6F9183FF974D14590CFD05ECF4E9FA97A4FF95AE9B`。本机 PlatformManager 与 ModuleFrame 的 SHA 均与归档不同。复验用 `dumpbin /exports` 定位 SDK 和 ModuleFrame 导出，`/disasm` 核对表中调用点及 EAX/EBX 值流，按 PE 节表读取 SDK 格式串 RVA `0xAC5830` 和 PlatformManager 日志标签 RVA `0x7DEE88`。托管入口可按 `VmProcedure.cs:176,235`、`ProcessManager.cs:400,765`、`CVmObjectBase.cs:717`、`VmSolution.cs:3866` 复核。[平台参数指令](../../analysis/full-audit/batch1-20261107/C02/stop-wait-argument-platform.asm.txt) · [ModuleFrame 等待及事件指令](../../analysis/full-audit/batch1-20261107/C02/stop-wait-events-moduleframe.asm.txt) · [流程记录与代理桥 GEX31](../architecture-recovery/global-variable-execution.json) · [原生停止与卸载归档](../../analysis/vm-framework-architecture-20261030/parallel-stop-unload-dispose-lifecycle.md)

本次平台和 ModuleFrame 指令摘录 SHA-256 分别为 `F8D8F7B797097F3A02CA2EE751CBE2DE020A88C2F7854992EABB5D7BB6221D71` 和 `744578B0ED32CEE74FC566EAE668C178F5D4C9E5FC1655506FAC265C047B06C5`。给 `ProcedureWaitTransportProbe.cjs` 与 `PlatformManagerDispatchProbe.cjs` 显式传入上述归档 SDK 与 PlatformManager 后，两个既有配置传输探针复验通过。探针默认使用历史 D 盘安装路径。

## 自研代码交付

自研执行器原先只在节点层检查调用者取消。方案作用域关闭时，整个会话可返回 Cancelled，但节点可能被记为 ModuleException，后续独立节点还可能创建实例。方案预算与关闭同时发生时，节点取消还可能被重标为超时。

`GraphExecutor` 现在统一判断调用者取消与方案关闭。在调度下一个节点、创建模块实例和异步清理结束后的结果发布点检查停止状态。已开始的模块仍走清理。正在关闭的节点不发布其迟到输出。`SolutionExecutor` 持有整个调用的作用域租约，保持停止令牌在调用返回前有效，且作用域关闭优先于独立的流程预算超时。该改动属于自研托管生命周期，原生计时和设备回调继续单列。

进一步回归复现了两个同步回调窗口。动态端口通知中的观察者关闭方案时，旧路径仍提交节点结果及更新后的端口描述。现在通知返回后重新检查停止与超时，取消节点不提交这两项状态。参数准备回调取消时，旧路径仍进入 `ExecuteAsync`。现在在准备后检查联合令牌，已创建的实例仍走原有释放路径。子流程和普通模块实例也共用入口前的联合令牌检查。

工具调试入口由 `MainWindow.CreateScopedToolEditorContext` 创建 `ToolDebugSession`，运行目标节点并按会话状态发布结果。旧 `StopAsync` 在持锁时同步执行取消回调，回调跨线程读取会话状态的用例复现了超时。回调抛错也会跳过等待活动执行的路径。显式停止现在异步派发取消回调，由停止与关闭共享等待任务，等活动执行和回调都完成后释放令牌源，再报告回调异常。这属于自研工具调试合同。

方案作用域的 `ModuleExecutionScope.State` 还存在一个重入关闭问题。状态工厂创建资源时可以触发所属方案关闭，旧路径在关闭后仍登记并返回刚创建的状态。没有外层执行租约时，关闭已完成资源清理，这份状态会遗漏释放。现在初始化持有自己的租约，登记新资源后检查所属方案是否已关闭。关闭时调用者收到 `ObjectDisposedException`，新资源随方案统一释放。外层执行租约仍持有时，释放继续等到执行结束。

方案资源预检原先使用执行快照，根流程与子流程却分别从调用方的资源列表重建目录。回归在根节点的 `Running` 回调中替换资源声明，旧实现随后会让子流程读到另一份内容。现在整个调用树复用预检使用的 `ResourceCatalog`，下一次运行再按新声明预检。外部文件仍在每次打开时按声明的 SHA-256 校验。本项属于自研方案执行合同。

`GraphExecutor` 以前在参数准备回调返回后立即写入有效参数，再检查取消。因此模块虽未进入执行，配置界面仍会读到这次未执行的参数。现在先准备执行上下文和两份参数快照，再在缓存提交时检查取消并一起更新种子值和执行值。用例先成功执行值 3，再准备值 7 并在回调中取消。修复后保留值 3，未进入执行的实例照常释放。

## 整批验证

服务替换的旧路径在持锁清理旧服务后才登记新服务。清理重入关闭会遗漏新服务；清理抛异常会使旧注册丢失所有权；同一对象按两个类型注册时，替换其中一个也会提前释放仍被另一个注册引用的对象。现在在锁内先发布新注册及所有权，仅当旧服务没有其他服务注册引用时移出所有权，再在锁外清理旧服务。清理异常仍传给调用者，新服务已由作用域管理。本项属于自研服务注册合同；没有增加原厂等价声明。

Release 无界面用例覆盖执行与取消 75 项、模块调试与释放 30 项、持久化 30 项、真实模块图 10 项、Group 跳出与累积 57 项、Group 多层生命周期 58 项、回调队列 15 项、执行所有权 10 项、子流程调用 93 项、清结果配置策略 24 项、配置调度 14 项。新增用例覆盖 Running 观察者关闭作用域、预算先到后关闭、清理等待、禁止后继工厂、抑制迟到输出和嵌套 Group 停止。

在工作区根目录运行整批 Release 回归。

```powershell
pwsh -File tests/NativeEvidence/C02/Run-LifecycleRegression.ps1
```

入口为每轮创建独立产物目录，并为每套测试设置独立工作目录。子流程用例会生成示例方案，现写入该轮的输出目录。真实模块图用例读取仓库中的固定方案，子流程用例使用复制到其工作目录的 BMP。汇总记录实际通过数、执行程序集哈希、构建和运行退出码及各套日志。单套失败时继续收集其余结果，最终整批返回失败。原厂宿主与自研桌面由用户另行手验。[整批复跑入口](../../tests/NativeEvidence/C02/Run-LifecycleRegression.ps1)

补查正常标志后，独立 Release 目录重新构建 `VisionFramework.ExecutionChecks`，0 警告、0 错误，75 项通过。本轮新增的是原厂静态证据，自研继续使用协作取消合同。

补上同步回调窗口后，Release 整批复跑 11 套共 **420 项**。执行套件从 75 项增加到 79 项，覆盖正常动态输出、观察者关闭后抑制输出和返回描述、参数准备阶段取消和实例释放。新增动态结果用例在修复前复现了失败。 [整批结果](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-4ab61195-042e-4ff9-88d8-991fedb49d72/summary.json)

工具调试停止修复后，Release 整批复跑 11 套共 **425 项**。调试套件增加至 35 项，覆盖回调跨线程读取状态、回调抛错时并发停止和关闭仍等待清理、错误传播及单次释放。 [整批结果](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-debug-stop-20261110/summary.json)

从 `docs` 目录调用整批入口，重新构建并运行 **11 套、425 项**，全部通过。仓库中的子流程示例在运行前后 SHA-256 一致。汇总 SHA-256 为 `670E28A0FEF1B3C5D4C8411513E65729C0D0940E4F6226D6E03A2ACD713D4930`。[隔离复跑结果](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-126bf8d881c34dd4bac276381655390a/summary.json)

状态工厂重入关闭用例在旧实现复现失败。修复后检查了有无外层执行租约两种情况，分别验证关闭后拒绝返回状态和资源单次释放。Release 整批复跑 **11 套、429 项**，汇总 SHA-256 为 `5B0D216B788517F53DD1C54ABA8C5BC20CDBF0D1CCD34275B234CCD4FA2F439D`。[本轮整批结果](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-4449f593236e4cccba3b2b36ae76d909/summary.json)

资源快照用例在旧实现复现根流程与子流程读取不同声明。修复后，当前调用树读到预检过的内容，下一次调用树统一读到更新内容。Release 整批复跑 **11 套、431 项**，汇总 SHA-256 为 `38DF65D50F91D2790931FAE47CA040030F07929D8FA13512D4C99B3E08F8EE52`。[本轮整批结果](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-d9275364e0584b97aae31e95cf339e61/summary.json)

参数准备取消用例在旧实现复现了有效参数缓存被覆盖。修复后整批 Release 回归 **11 套、432 项**全部通过，汇总 SHA-256 为 `2EA8AD4343691643927A104CC801034C47CCCECDA64EB651B90539F4A9F863E1`。[本轮整批结果](../../analysis/full-audit/batch1-20261107/C02/lifecycle-regression-baea087e89a14d11a39698db768959a2/summary.json)

## 桌面 Release 交付

历史阶段的服务替换修复曾以 11 套、437 项回归验证关闭后的服务泄漏、清理重入和新服务所有权。该阶段摘要保留作修复过程证据，当前工作树结果以文末 462 项复跑为准。

用桌面项目入口 `src/VisionEditor/VisionEditor.csproj` 构建 Release，核验输出中的托管依赖、11 个插件程序集和内置 BMP。桌面产物与 425 项回归产物的便携 PDB 都记录了与当前文件一致的 `GraphExecutor.cs`、`SolutionExecutor.cs` 和 `ToolDebugSession.cs` SHA-256。构建 0 错误，`MainWindow.xaml.cs:122` 的 CS8603 在 WPF 临时项目和正式项目各出现一次。桌面手验待用户启动。[Release 入口](../../analysis/manual-acceptance/flow-c02-e044bf98a44b4256ae3e0264857ac527/build/bin/VisionEditor/release/VisionEditor.exe) · [构建清单](../../analysis/manual-acceptance/flow-c02-e044bf98a44b4256ae3e0264857ac527/summary.json)

状态工厂修复后的桌面 Release 已另行构建，便携 PDB 中 `ModuleExecutionScope.cs` 的哈希与 429 项回归和工作树一致。[状态工厂修复版](../../analysis/manual-acceptance/flow-c02-b823606734924333b115858b43f9e044/build/bin/VisionEditor/release/VisionEditor.exe)

资源快照修复后的桌面 Release 已构建。便携 PDB 中 `SolutionExecutor.cs` 的哈希与 431 项回归和工作树一致。[资源快照修复版](../../analysis/manual-acceptance/flow-c02-98da2e5ea62a420c8c3b1c1571723ca8/build/bin/VisionEditor/release/VisionEditor.exe)

参数缓存修复后的桌面 Release 已构建，便携 PDB 中 `GraphExecutor.cs` 和 `ModuleExecutionScope.Parameters.cs` 的哈希与 432 项回归和工作树一致。已检查托管依赖及随包 BMP，构建 0 错误，同一处 CS8603 仍报告两次。[参数缓存修复版](../../analysis/manual-acceptance/flow-c02-709803a1e1914c8daff078860e4a302c/build/bin/VisionEditor/release/VisionEditor.exe) · [构建日志](../../analysis/manual-acceptance/flow-c02-709803a1e1914c8daff078860e4a302c/build.log)

服务替换修复后的桌面 Release 已构建，0 错误。随后修复了主界面新建方案的空流程问题，当前 Release 构建为 0 警告、0 错误。未启动界面。[当前 Release 入口](../../src/VisionEditor/bin/Release/net10.0-windows/VisionEditor.exe)

框架实施与自审经台账复核为 41/96（42.71%）。C02.2 的直接与间接派发均已追到同一原生阈值比较。`StopWhenNG` 只在成功读取指定模块的 `ModuStatus` 且值不为 1 时触发。流程终态与在途模块的同轮时序仍待宿主验证。标定工具实施与自审为 10/10（100%）。[原生派发证据](C02-native-wait-transport-20261022.md)

## 当前工作树复跑（2026-11-11）

当前 `main` 工作树再次执行 `tests/NativeEvidence/C02/Run-LifecycleRegression.ps1`，Release 构建和 11 个套件全部通过，共 462 项。当前交付摘要为 `analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/summary.json`，SHA-256 为 `55AA3FAF46BAC752F6083BEBDA65245758767D73C1CE9C84AD11AEC44E922DCF`。本轮包含调度器 39 项检查，覆盖重入关闭、观察者异常隔离、停止排空和取消回调异常传播。
随后执行 `tests/NativeEvidence/C02/Audit-RegressionIdentity.ps1`，确认 13 对程序集/PDB 和 87 份源码文件与当前工作树一致。身份审计结果为 `analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/source-identity-audit.json`。

