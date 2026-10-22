# MF-05 流程执行调用链逆向

更新：2026-10-19。此记录区分 VM 的托管可见调用链与 native SDK 内部未知行为。它不把独立 WPF 执行器描述成 VM 算法复刻。

> 2026-10-22 补证：下文“SyncWaitor 实现未取得”是旧断点；第二批已取得原 `VM.Util.dll` CIL 并运行受控探针，见 [C02 第二批](reverse/C02-scheduling-batch2.md)。`IMVS_Set/GetProcedureWaitTime` 的 x64 导出与下层命令传输也已静态恢复，见 [C02 原生等待传输](reverse/C02-native-wait-transport-20261022.md)。服务端消费字段 8 后的实际计时、执行取消和回调归因仍未知。保留以下旧段落作为历史，不再用其描述当前缺证据范围。

## 普通流程单次运行

|阶段|原始证据|确认行为|
|---|---|---|
|公开入口|`analysis/decompiled/VM.Core/VM.Core/VmProcedure.cs::Run()`|调用 `ProcessManager.SyncExecuteOnce(ID)`；异常时置 `IsRunning=false` 并重新抛出 `VmException`。|
|带选项入口|`VmProcedure.Run(string,bool,bool)`|非 SDK 模式先置 `IsRunning=true`，再将触发字符串、是否等待和是否响应 UI 传给 `SyncExecuteOnce`；异常回滚 `IsRunning`。|
|托管执行入口|`analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ProcessManager.cs::SyncExecuteOnce`|流程 ID 先换算为 `nProcessID-10000` 并检查0..999；先重置该流程完成标志，再调用 `ServerSDKAPI.IMVS_ExecuteOnce_V30`；native 返回非零抛出；`bIsWait=true` 时调用 `SyncWaitor.PrecisionWait`。|
|完成回调|`ProcessManager.GetWorkStatusCallBackInfo`|仅在 `nWorkStatus==0` 且流程索引有效时写 `mProcedureNormalFlag` 与 `mProcessRunFlag=true`；随后派发 `OnWorkStatusCallBack`。|
|等待时间配置|`ProcessManager.GetProcedureWaitTime` / `SetProcedureWaitTime`|托管方法转发至 `IMVS_GetProcedureWaitTime` / `IMVS_SetProcedureWaitTime`，检查 native 返回码；反编译源码没有给出 native 计时、取消或状态归因逻辑。|

## 其他运行入口的区别

- `VmProcedure.Run(string, ref uint, bool, bool)` 走 `SyncExecuteOnceEX`，转发至 `IMVS_PF_ExecuteOnceEX_V30` 并可按 `bIsWait` 等待。
- `VmProcedure.RunP2P(uint nTimeout, bool)` 走 `SyncExecuteOnceP2P`：在1024槽位中循环分配命令索引，将其字符串作为 command 传给 `IMVS_ExecuteOnce_V30(..., bIsP2P:true)`；完成回调按命令字符串解析索引并置对应标志。其等待结束后才递减 `NumOfRunP2PFlag`。`SyncWaitor.PrecisionWait` 的实现不在已反编译清单内，因此超时/异常路径中该计数是否总能归还尚不能确认。
- `VmProcedure.SyncRun()` 使用 `SyncExecuteOnceGetEx` 及 `SyncWaitor.SyncWait`，不是带超时的普通入口。
- `SolutionManager.SyncSaveSolution` / `SyncLoadSolution` 也以结果回调旗标同步等待；加载 SDK 模式下额外检查 `nSolutionLoadErrorCode`。这与普通流程运行标志分开。

## 原生方案保存完成链

`VM.Core/VmSolution.cs::Export` 进入 `InternalExport`，用 `_isSaving` 防止并发保存；选定或复用路径后调用 `SolutionManager.SyncSaveSolution`，随后更新 `SolutionPassword` 与 `SolutionPath`。托管接口转到 `ServerSDKAPI.IMVS_SaveSolution`。`SolutionManager` 初始化时订阅 `CVmObjectBase.OnModuleSolutionSaveEndCallBack`；底层回调依次经过 `CVmObjectBase` 分发、`SolutionManager.GetSolutionSaveEndCallBackInfo` 先同步派发 `OnSolutionSaveEndCallBack`（其中 `VmSolution.MSolutionManager_OnSolutionSaveEndCallBack` 处理完成事件并清除 `_isSaving`），然后才记录 `nStatus` 并置 `mWaitSaveSolutionFlag=true`。同步保存在 `SyncWaitor.Wait` 后仅 SDK 模式检查 `nSolutionSaveErrorCode`。因此普通模式是否在非零结束状态抛错、等待器超时/线程语义都不能从这段源码推断为成功/失败对称行为。

保存界面/工程提供的 `.vmx.json` 是独立托管序列化格式，不是上述原生 `.sol` 路径；本项目 `MainWindow.Save` 调用 `GraphEditorDocument.Save`，写入文件后更新最近文件和另存目录偏好。当前没有证据证明两种文件在格式、原生算法资源或兼容性上等价。

## 主框架当前边界

`src/VisionFramework/SolutionExecutor.cs::RunAsync` 以 `GraphExecutor` 执行托管节点，使用协作式 `CancellationToken`；`ProcedureDocument.WaitTimeMs` 到期时归类为 `TimedOut` 并等待模块释放。这是本项目自己的异步托管实现。它没有调用 `IMVS_ExecuteOnce_V30`，也没有证据表明 VM 普通流程使用同样的协作取消语义。

本次运行数据验证：`tests/VisionFramework.ExecutionChecks` 的71项执行检查通过；`tests/VisionWorkflow.CallChecks` 的93项子流程检查通过，包含子流程超时/父流程取消和资源释放。它们证明当前托管行为，不证明 native 等价。此前主窗口 WPF 全量操作序列456项通过，另见 [框架源码自检](FRAMEWORK-SOURCE-SELF-AUDIT.md)。

## 未知与下一项源级问题

1. `SyncWaitor` 具体忙等/消息泵/超时/线程重入实现不在当前 decompiled tree；需定位其真实程序集或记录实机。
2. `IMVS_ExecuteOnce_V30` 如何构建流程图执行序、节点失败后的中止/继续条件、设备取消及状态码由 native SDK 决定，当前静态托管层没有实现体。
3. `IMVS_SetProcedureWaitTime` 是否限制整个流程、等待回调、设备任务及重入需原版实验；现有 `WaitTimeMs` 只是一种明确记录的托管实现。
4. P2P 槽位环回时的并发保护、wait 抛出时的计数清理需要核实 `SyncWaitor` 和完整回调时序；当前不据方法结构单独断定泄漏。
5. 保存结束的 UI 进度回调只在 `isDisplayProgress` 为真时派发；结束回调清除 `_isSaving`。`InternalExport` 的空路径取消分支和 `catch` 也显式清除 `_isSaving`，后者另清空 `SolutionPath` 并重新抛出。若 native 调用持续等待、既未回调也未抛出，托管标志如何恢复仍未知，不能概括成自动复位。同步派发事件在完成标志赋值之前，订阅者若抛出会影响后续赋值；本轮仅记录调用顺序，不推断 native 异常恢复。

## 可复验路径

```powershell
dotnet run --project tests/VisionFramework.ExecutionChecks/VisionFramework.ExecutionChecks.csproj --no-restore
dotnet run --project tests/VisionWorkflow.CallChecks/VisionWorkflow.CallChecks.csproj --no-restore
```
