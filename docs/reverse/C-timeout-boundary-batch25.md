# C02.2 第二十五轮超时边界复核

## 结论

C02.2 本轮没有新增产品改动，仍不能从 `partial` 提升为 `done`。现有 main 已有可执行的托管参数快照、节点超时、流程等待预算、取消传播及释放等待；但原始源码明确把 `SyncWaitor` 的实现和 `IMVS_SetProcedureWaitTime` 的 native 计时归因留在 SDK 边界，不能把托管策略宣称为原行为闭合。

## 源码依据

- `docs/MF05-PROCEDURE-RUN-CALLCHAIN.md`：`VmProcedure.Run(string,bool,bool)` 经过 `ProcessManager.SyncExecuteOnce`；`bIsWait` 进入 `SyncWaitor.PrecisionWait`。`GetProcedureWaitTime/SetProcedureWaitTime` 只转发 `IMVS_Get/SetProcedureWaitTime`，反编译源码没有 native 计时、取消和状态归因。
- 同文明确记载 `SyncWaitor` 具体忙等、消息泵、超时和线程重入实现未在当前 decompiled tree；`IMVS_SetProcedureWaitTime` 是否限制流程、回调、设备任务或重入仍未知。
- `docs/MF05-CONTINUOUS-RUN-CALLCHAIN.md`：main 的 `StartContinuous/ContinuousRun` 是协作式托管循环，与 native `ContinousRunStart/RefreshUIStopExecute` 的线程、停止延迟和回调顺序不等价。

## 当前 main 行为（可实施但独立）

- `src/VisionFramework/GraphExecutor.cs`：复制节点参数和 ROI 快照；`NodeDocument.TimeoutMs` 建立 linked token，模块结束后先等待 `Dispose/DisposeAsync`，再分类 `TimedOut`。
- `src/VisionFramework/SolutionExecutor.cs`：`ProcedureDocument.WaitTimeMs` 建立流程级 linked token；取消产生 `TimedOut` 与 `ProcedureWaitTimeout` 节点错误，并等待子模块清理；0 表示不设流程等待预算。
- 参数/输入快照及取消路径已有 `tests/VisionFramework.DebugChecks`、`tests/VisionFramework.ExecutionChecks`、`tests/VisionWorkflow.CallChecks` 覆盖。

## 自审核

- `tests/VisionFramework.ExecutionChecks`：**71 checks passed**，覆盖单节点 timeout、取消、模块释放、顽固模块清理、参数边界、执行顺序及预取消。
- `tests/VisionWorkflow.CallChecks`：**93 checks passed**，覆盖跨流程参数快照、子流程 timeout、父流程取消和 child disposal。
- 本轮未修改 `src/`；没有可依据的最小修复可安全补入。C02.2 保持 `implementation=partial`、`validation=partial`，不计入实施进度。

## 未闭合范围

需取得 `SyncWaitor` 实现或受控 native/SDK 运行证据，确认等待预算的计时对象、回调线程、设备取消、重入和错误码归因后，才能决定是否新增兼容层。不得把现有 `WaitTimeMs` 视为原生等价，也不修改 `tool-plugins`。
