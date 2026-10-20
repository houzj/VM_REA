# 第三轮 J12：真实图像流程的受控 WPF 响应性检查

2026-10-20。审核基线：`main` `d7a8741482df16b02ce2d24030852703daf6d715`，加主线尚未提交的 `DynamicOutputPresentation.cs` / `WorkflowExecution.cs` owner/序号保护（本线未写这两个文件；本次实际编译的 `WorkflowExecution.cs` SHA-256 `62DFE19E6836DE7C116F3C89A6C17521D64B252D14DD137116987AADC9A666D4`）。只在独占 `tests/WorkflowImageResponsiveness.Checks` 与 ignored `analysis/joint-audit/ui-image-run/` 写测试/审计；未使用用户桌面窗口、进程、设备或 `tool-plugins`。

## 源码边与原版限度

当前 `MainWindow.StartRunCore → RunCore`（`src/VisionEditor/WorkflowExecution.cs:19–59`）先在 Dispatcher 捕获 `Progress<ExecutionEvent>`，然后 `Task.Run` 把 `SolutionExecutor.RunAsync` 移到后台；完成、`IsCurrentExecutionOwner`、结果/历史发布、`FinishWorkflowRun` 回 Dispatcher。`MainWindow.DrainWorkflowBeforeClose`（`src/VisionEditor/MainWindow.xaml.cs:88–106`）在运行中第一次关闭时取消流程并等待 `activeRun` 清空，再发第二次关闭；`OnClosed`（`WorkflowExecution.cs:91–99`）失效 owner、取消并释放 scope。`ModuleExecutionScope.Dispose` 在租约退出后释放资源；其 `Stopping` 令牌须在关闭前捕获，不能在已释放的 CTS 上再读取属性。

原版 UI `VMControls.WPF.DataForUI.VmProcedureDecorator.RunOnceCommandExe` 调 `VmProcedure.Run(null,bIsWait:false)`；原 `VM.PlatformSDKCS.ProcessManager.SyncExecuteOnce`（`analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ProcessManager.cs:277–299`，SHA-256 `06FEE7C7B5387FEA086ABE5073BE5DD2606AD121D9BC64B0AB9335BACE100414`）先调 `ServerSDKAPI.IMVS_ExecuteOnce_V30`，只在 `bIsWait=true` 时调用 `SyncWaitor.PrecisionWait`。这证实原 UI 命令**不在托管层等待流程完成**；原生 P/Invoke 本身返回前的耗时和实际线程/回调仍未知。当前 `Task.Run` 是本地宿主响应性实现，不冒称原版线程拓扑逐指令一致。

## 可复验测试与结果

`tests/WorkflowImageResponsiveness.Checks/Program.cs` SHA-256 `8F4D0FB7B47EF7E1884099943F32E1A32A58D184E23179BF46DC0F74F2BC3F04`。使用独立 `MainWindow`、`ShowInTaskbar=false`、离屏位置，禁自动相机发现；真实 `ImageSourcePlugin` SDK 提交 4096×4096 GrayImage，再连接真实 `ImageBinaryPlugin`，通过实际主窗 `StartRunCore` 执行。`DispatcherTimer` 10 ms 采样心跳；同宿主分别做正常运行、运行中取消、运行中关闭及闭窗后一次 Dispatcher 脉冲。没有调用原厂或设备。取消必须断言 `SessionStatus.Cancelled`；关闭必须断言 `windowClosed=true` 且关闭前的 `ExecutionOwnerStamp` 已失效。

复验命令：`dotnet run --project tests/WorkflowImageResponsiveness.Checks/WorkflowImageResponsiveness.Checks.csproj -c Release --no-build`。日志 `analysis/joint-audit/ui-image-run/4096-image-ui-check.json` SHA-256 `65997757C9A05D6091B482973DA878462DC09D62124D5B8F5C58E5E96805531C`，退出码 0。该次：

| 场景 | 操作级结果 |
|---|---|
| 正常 | UI 入口返回 **40.55 ms**，当时 Task 未完成；全次 **337.65 ms**，未完成期间 8 次 Dispatcher timer tick，真实 Binary 节点 Success。另一次有效样本入口 161.01 ms、全次 659.32 ms、14 tick。心跳能运行，不要求 10 ms 准点。 |
| 取消 | 等待约 5 次 UI tick 后取消仍在运行的流程，约 **262.02 ms** 完成，session 被断言为 `Cancelled`。此测试不能精确证明取消指令落在二值化像素循环内；可能在图执行的其他阶段。 |
| 关闭 | 等待约 2 次 UI tick 后对运行中窗请求关闭；约 **222.42 ms** 后真实窗口关闭、scope 令牌已取消、旧 owner stamp 被断言失效；闭窗后的另一次 Dispatcher 脉冲未观察到迟到 session 覆写。第一次 `Close()` 因 `DrainWorkflowBeforeClose` 延迟关闭，探针按两阶段生命周期判断。 |

探针开发中三次失败及修正完整保留在 `analysis/joint-audit/ui-image-run/prior-failures.md`。它们分别是测试用错静态 ID、释放后读取 CTS、误以为第一次 Close 即已关闭；最终通过不能抹去这些历史。

## 结论与仍需证据

此检查实际证实 d7a8741 工作线程修复在大帧真实 ImageSource→Binary 的**正常、取消、关闭**三条当前宿主链可运行，UI 能在流程未完成时处理 Dispatcher 工作。它也证实该条受控链未在本次离屏宿主重现永久无响应。用户先前空闲后 Windows 弹出的 Hang 仍无 UI 线程栈/等待链，不能宣布根因已修好；测试窗没有用户可见桌面验收资格。

J12 仍缺：用户同构方案/图像及 EXE 身份、空闲触发时序、真实卡住时 UI 线程栈；相机/文件夹事件、Group、其他已接入插件在工作线程上的 UI 亲和副作用未由本夹具覆盖。下一探针应在自有受控实例固定这些输入并在超阈值心跳间隔捕获该进程线程栈；原厂 `IMVS_ExecuteOnce_V30` 的本地线程/回调时序需另取原生指令或受控差分。不要用本地测试通过给原厂行为或全部插件打等价标签。
