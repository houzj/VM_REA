# MF-01 主窗口启动诊断（2026-10-18）

## 源码与实现边界

- 原始反编译 `VisionMaster.Exe.Audit/VisionMaster/App.cs` 的 `ShowMainWindow` 创建主窗口、设置 `Application.Current.MainWindow`、绑定数据并调用 `Show()`，之后恢复窗口状态。此处确认的是启动/显示顺序，不能据此推断全部原生初始化耗时。
- 当前独立实现 `src/VisionEditor/Program.cs` 在 STA 线程以 `Application.Run(new MainWindow())` 启动；`MainWindow.xaml` 默认可见、居中，构造函数同步构建工具/图/预览并载入示例。WPF 专项检查直接构造并 `Show()`，没有覆盖进程入口到窗口出现的时间。

## 实际诊断

本线 Debug 构建成功。使用 `tests/VisionEditor.StartupChecks/startup-probe.ps1` 启动**仅由该脚本拥有**的 EXE，每 250 ms 查询主窗口句柄，45 秒内须出现 `VisionMaster`，之后关闭自启进程。三次独立运行得到 6609、5244、4709 ms。另一次在启动 8 秒时仍读到零句柄，延长到 15 秒时读到有效句柄。这表明固定短等待可能误判尚在初始化的程序为“无主窗口”；不能证明别的发布目录中的旧进程也属于同一原因。

已存在的 `dist/VisionEditor-WpfCompoundLeaf` 和 `dist/VisionEditor-WpfRoiDisplayFixture` 两个进程均读到零主窗口句柄，但其启动时间为前一天、输出目录不同，本线没有终止或操作它们。仍需在其所属任务中核对是否窗口已关闭而进程残留、初始化卡住或发布差异。

本次没有修改产品启动行为。`startup-probe.ps1` 是测试辅助，不进入产品程序集；默认执行可见窗口启动，始终只清理它亲自创建的 PID。

运行方式：

```powershell
pwsh -NoProfile -File tests/VisionEditor.StartupChecks/startup-probe.ps1 -Executable src/VisionEditor/bin/Debug/net10.0-windows/VisionEditor.exe
```
