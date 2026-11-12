# G7 Group 停止恢复修复

日期：2026-11-12

## 可复现偏差

`VisionEditor.GroupStopRecoveryChecks` 原先在 Group 窗口点击“停止运行”时失败：窗口把停止请求直接转发为当前 SelfRun 的取消，导致在途运行不能完成当前结果。

## 源码/宿主合同

现有宿主检查明确区分两个生命周期：

- Group 窗口的 StopRun 只清除后续连续运行请求，当前 SelfRun 继续到结果完成。
- 关闭窗口时才取消在途任务，并等待清理完成。

## 实施

`src/VisionEditor/SubprogramCallWindow.xaml.cs` 的 `StopClick` 现在只清除 `repeatRequested`；`StopAndCloseAsync` 与 `OnClosing` 保留取消和等待路径。

## 验证

- `VisionEditor.GroupStopRecoveryChecks`：10 项通过。
- `GroupRoundLifecycleChecks20`：58 项通过，覆盖多层 Group、正/负步长、异常、取消、异步释放和 owner 隔离。

原厂 native SelfRun ABI 和人工桌面差分仍标记“运行未确认”。
