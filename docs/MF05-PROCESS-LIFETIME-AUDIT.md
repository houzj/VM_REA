# 主框架进程生命周期静态审计

日期：2026-10-18。此审计只回答主框架产品源码是否主动创建外部进程，不代替运行时句柄/线程/原生 SDK 生命周期检查。

## 检查结果

对 `src/` 搜索 `System.Diagnostics`、`ProcessStartInfo`、`new Process`、`Process.Start`、`Process.Get` 和 `StartInfo`：没有发现产品代码显式创建、枚举、等待或终止子进程。命中项中的 `System.Diagnostics` 均用于 `Stopwatch`，例如 `VisionEditor/ToolExecutionScope.cs`、`VisionFramework/GraphExecutor.cs` 与图像源/标定插件的计时。当前可以确认的是主框架托管源码未展示子进程启动器；这降低了“主框架忘记释放自己创建的子进程”这一类风险，但不能据此判定整个程序无进程泄漏。

测试脚本会启动其自己创建的 `dotnet`/验收子进程并在超时后终止该子进程，示例为 `tests/Check-ProcessExit.ps1` 与 `tests/Invoke-ConfigurationRegression.ps1`。这些是测试工具，不是产品程序集中的后台进程功能。

## 未覆盖的边界

- VM 原生 SDK、设备驱动、插件依赖或外部 VM 可能自行创建进程；托管静态搜索无法观察它们。
- 本轮未对桌面进程树做受控前后快照，也未验证窗口关闭后原生线程、句柄、设备会话是否归零。
- 任务管理器中多个 `VisionEditor.exe` / `Dashboard.Client.exe` 实例不能仅凭名称归因于本项目；需要 PID、启动路径、父 PID、创建时间及受控关闭前后差分。

## 结论

目前没有源码证据表明主框架产品会自行启动子进程并遗留它们。原生 SDK/设备层和真实桌面进程树仍未验收；因此结论是“未发现主框架显式子进程泄漏路径”，不是“项目不存在进程泄漏风险”。
