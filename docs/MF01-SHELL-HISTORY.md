# MF-01 菜单与工具栏历史命令边界

原始反编译 `VisionMaster.AppMainWindow/TitleMenueItemData.cs` 的 `Undo`、`Redo` 在 `Program.MainData.IsOpenParams` 为真时拒绝撤销重做，并显示“打开模块配置时，不能执行撤消重做”；正常情况调用 `IUnRedoService`。这是命令行为证据，不代表当前实现已复刻原生警告弹窗模板或其服务实现。

当前 WPF 菜单和工具栏统一经过 `UndoShell`、`RedoShell`。参数配置窗口打开时不改图文档历史，并在状态栏提示；窗口关闭后恢复真实节点名称的撤销/重做。锁定、运行中、已关闭窗口也阻止这些入口，这是当前独立安全边界。

独立 WPF 专项 `tests/VisionEditor.ShellHistoryChecks` 实际显示主窗口、创建可撤销的节点改名，然后操作菜单及工具栏四个入口：参数窗口标志为真时版本不变；标志恢复后工具栏撤销复原名称、菜单重做再次应用名称。两组检查通过。原生模态提示的视觉样式、原生 Undo 服务内部栈与窗口焦点行为尚未做 VM 实机差分。
