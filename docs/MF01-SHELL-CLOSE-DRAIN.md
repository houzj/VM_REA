# MF-01：主窗口关闭与运行资源排空

## 原始调用链证据

证据来自当前原始反编译目录，下面描述的是可见托管层行为，不推断 SDK 内部实现：

- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/ApplicationOperation.cs`：`ApplicationOperation.Dispose()` 先调用 `Program.MainData.DoClose()`、释放看门狗并屏蔽模块回调；除 `IsRunBackground == "1"` 外，将 `MaskMessage` 设为“正在关闭方案”并打开进度遮罩；之后调用 `RunSyncWithEvent(() => VmSolution.Instance.Dispose(isRunBackground))`。
- 同文件 `RunSyncWithEvent(Action)` 在线程池执行 SDK dispose，并在调用线程运行 `System.Windows.Forms.Application.DoEvents()`、轮询完成标志，直到销毁操作结束。这里确认的是应用层等待边界，SDK Dispose 的内部资源/回调语义不在托管源码中。
- `analysis/decompiled/VisionMasterMain/VisionMasterMain/MainWindow.xaml.cs`：`IsShowProgressMask` 依赖属性回调到 `AddMask/RemoveMask`，并将 `ProgressMask` 添加到 `PART_View`；`MainWindow.xaml` 将 `TitleBar.IsButtonClickEnabled` 绑定到 `IsShowProgressMask` 的取反，TitleBar 再把它应用于菜单和工具栏。
- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs`：`VmSolution_OnSolutionUpgradeBeginCallBack` 从回调参数设置遮罩消息并显示遮罩，`VmSolution_OnSolutionUpgradeEndCallBack` 隐藏遮罩；初始化时分别订阅 Procedure/Group upgrade begin/end 回调。该升级链与关闭链分开核对。

## 本线 WPF 修复

现有 `MainWindow.OnClosed` 位于另一个 partial 文件 `src/VisionEditor/WorkflowExecution.cs`，执行顺序是关闭工具配置、立即释放 `executionScope`、最后取消 `activeRun`。该文件不是本线所有者，本次没有改动它。

在本线所有的 `src/VisionEditor/MainWindow.xaml.cs` 构造阶段挂接 `Closing` 事件：如果当前存在框架自有流程执行令牌，先取消这次关闭，禁用内容交互并显示“正在停止流程并等待资源释放…”，请求取消流程，再等待同一个执行令牌从 `FinishWorkflowRun` 生命周期中释放。令牌释放后重新发起关闭；这样既有 `OnClosed` 的 scope dispose 只会在执行器完成清理后运行，随后原有 `ToolEditorHost.OnClosing` 仍可处理工具编辑器/Group 窗口的异步释放。

关闭期间内容禁用和状态提示是本项目对 SDK dispose 等待边界的 WPF 独立适配。当前产品没有原 VM 的 `VmSolution.Dispose` / `ProgressMask` / 后台运行模式服务；本改动只等待框架自有 `activeRun`，不声称移植了 SDK 销毁或完整原生遮罩行为。没有实现/伪造 `FrontendUI.Design` 运行模式后端。

## 实际操作验证

`tests/VisionEditor.ShellChecks` 通过真实 WPF `Window.Close()` 触发关闭链，给 `activeRun` 设置一个测试执行令牌，确认首次关闭被取消、令牌收到取消请求、窗口继续显示且内容禁用；由 Dispatcher 定时器模拟拥有者完成释放，确认第二次关闭后窗口才消失。测试在等待态捕获 `analysis/full-audit/shell-shutdown-wait.png`，已目检确认窗口保留并显示状态提示；截图捕获逻辑只在 tests。

命令：`dotnet run --project tests/VisionEditor.ShellChecks/VisionEditor.ShellChecks.csproj -v:q -m:1`。结果：68 项 shell WPF 操作检查通过，包含新增关闭排空 2 项；构建有一条既有 `MainWindow.xaml.cs` 可能返回 null 警告，无编译错误。逐项输出见 `analysis/full-audit/shell-close-drain-final.log`。该测试驱动 WPF 事件和生命周期令牌，未运行真实 VM，也未代替真实模块 Dispose/真实设备释放测试。

## 未确认边界

- VM `IsRunBackground` 的来源/持久化以及 SDK Dispose 真实耗时、回调次序与设备释放状态。
- 执行器取消是否与 VM `VmSolution.Dispose` 在每种工具/回调下等价；当前只证明 WPF 主窗在自己的执行令牌结束前不会触发关闭后的 scope dispose。
- 原 VM 遮罩关闭过程的桌面视觉/鼠标可用性差分，以及真实运行模式窗口 `FrontendWindowData.InitRunWindowFromDesignData` 后端。
