# G3 执行、停止、结果和释放门禁

日期：2026-11-12

## 证据链

原厂证据已确认 `VmProcedure.Run → ProcessManager.SyncExecuteOnce → IMVS_ExecuteOnce_V30`，停止路径经 `IMVS_StopExecute_V30 → SM_StopExecute_V30 → ControlStopExecuteByProcId`，回调路径由工作状态消息更新流程状态后再发布结果。原生停止等待固定 3000 ms，托管 `WaitTimeMs` 不能据此解释为同一语义。

原件与静态证据见：

- `docs/reverse/C02-shared-execution-lifecycle-20261110.md`
- `analysis/full-audit/batch1-20261107/C02/stop-wait-argument-platform.asm.txt`
- `analysis/full-audit/batch1-20261107/C02/stop-wait-events-moduleframe.asm.txt`

## 当前实现闭环

```text
MainWindow.StartRunCore
  → SolutionExecutor.RunAsync
  → GraphExecutor.RunAsync
  → module execution / callback / result snapshot
  → WorkflowCallbacks.PublishModuleResult
  → UI result cache and history
  → cancellation or window close
  → scope lease drain
  → module/session/resource disposal
```

`GraphExecutor` 统一检查调用者取消与方案作用域关闭；`SolutionExecutor` 持有调用期间的作用域租约；迟到输出、动态端口回写和参数准备取消均有回归覆盖。窗口关闭先排空活动运行，再由 `OnClosed` 释放执行作用域、文档和回调队列。

## 门禁结果

| 范围 | 结果 | 证据 |
|---|---|---|
| 正常执行、拓扑、结果类型和失败传播 | 通过 | `VisionFramework.ExecutionChecks` 91 项 |
| 取消、超时、停止后清理 | 通过 | 同上及 C02 11 套回归 |
| 回调取消和迟到输出抑制 | 通过 | C02 最新 462 项回归 |
| 作用域状态、服务和资源单次释放 | 通过 | C02 最新回归与 `ModuleExecutionScope` 检查 |
| 原厂 SDK/宿主同输入运行时顺序 | 运行未确认 | 原厂环境和宿主方案仍缺 |
| 原生 3000 ms 停止等待与托管实现的数值 parity | 运行未确认 | 仅有静态 PE/汇编证据 |

## 复跑

```powershell
dotnet D:\MyWork\Rea逆向\artifacts\execution-checks-g3\VisionFramework.ExecutionChecks.dll
pwsh -File tests/NativeEvidence/C02/Run-LifecycleRegression.ps1
```

前一条本轮构建运行结果为 `91 execution checks passed`，构建 0 警告、0 错误。后一条历史最新身份固定结果为 11 套、462 项；当前源码身份见对应 `source-identity-audit.json`。

## 结论

G3 在托管执行合同上达到 `source-shaped`，原厂宿主回调时序、原生等待和同输入结果仍保持“运行未确认”。测试通过只关闭自研生命周期门禁，不提升原厂 parity 状态。
