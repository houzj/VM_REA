# C01 启动链统一重写与生命周期回归

日期：2026-11-11
上游：`docs/reverse/C01-startup-chain-behavior-contract-20261111.md`（契约）、`docs/reverse/C01-shell-host-reconciliation-matrix-20261111.md`（对照矩阵与根因）

## 1. 本轮改动清单

| 文件 | 性质 | 职责 | 契约条目 |
|---|---|---|---|
| `src/VisionEditor/Program.cs` | 重写 | 工作目录、`IsMultiApp` 分流、单实例、构造 App | S00 |
| `src/VisionEditor/App.cs` | 新增 | `Application` 子类：欢迎页线程、初始化线程、双线屏障、方案中心门控、主窗口展示顺序、异常分工 | S01-S09, S12 |
| `src/VisionEditor/StartupLaunch.cs` | 重写 | 启动阶段状态、命令行分流、后台初始化阶段、失败日志 | S00, S03, S06, S12 |
| `src/VisionEditor/SingleInstance.cs` | 新增 | 单实例守卫（修正非持有者释放问题）+ 命名管道参数转发 | S00, S11 |
| `src/VisionEditor/StartupSettings.cs` | 新增 | 读取/回写 `VisionEditor.exe.config` appSettings，缺键回退原厂基线 | 契约第 1 节 |
| `src/VisionEditor/AuthGate.cs` | 新增 | 授权接口占位 `IAuthGate` + `NullAuthGate` 恒放行 | S03, S09（骨架） |
| `src/VisionEditor/MainWindowData.cs` | 新增 | 宿主数据：进度遮罩、遮罩消息、缩放档位、运行模式、窗口状态持久化与恢复 | S04, S08, W04 |
| `src/VisionEditor/ShellLifecycle.cs` | 新增 | 关闭判定、释放顺序、退出出口、关停模式不变量 | S10, S11 |
| `src/VisionEditor/MainWindow.ShellHost.cs` | 新增 | `IsShowProgressMask` / `MaskMessage` 依赖属性、进度遮罩层、授权提醒骨架 | W02, W05 |
| `src/VisionEditor/MainWindow.xaml` | 修改 | 根容器 `ShellRoot` 包裹 `WindowContent`，承载遮罩层 | W05 |
| `src/VisionEditor/MainWindow.xaml.cs` | 修改 | 标题取自 `ApplicationName` | W01 |
| `src/VisionEditor/MainWindow.ShellOpenSolution.cs` | 修改 | 新增 `OpenForwardedSolution`（二次实例转发装载） | S11 |
| `src/VisionEditor/StartupGuideWindow.xaml(.cs)` | 修改 | 卡片语义改为写 `ToolBoxType` + 关闭；最近方案改依赖属性绑定；`不再显示` 写回 `ShowEnable`；关闭按钮 `DialogResult=true` | G05-G10 |
| `src/VisionEditor/VisionEditor.csproj` | 修改 | 对启动壳检查项目开放 internal | — |
| `tests/VisionEditor.StartupShellChecks/Program.cs` | 重写 | 新增配置基线、参数分流、关停模式、宿主数据绑定、遮罩、关闭链断言 | 第 4 节 |
| `docs/reverse/C01-startup-chain-behavior-contract-20261111.md` | 新增 | 行为契约 | — |
| `docs/reverse/C01-shell-host-reconciliation-matrix-20261111.md` | 新增 | 对照矩阵与根因 | — |

未改动：工具插件工作树、`analysis/full-audit`、`VisionFramework` / `VisionPluginContracts` 合同、其他测试工程。

## 2. 根因与修复

**根因**：`StartupWelcomeWindow` 是第一个被构造的 `Window`，被 WPF 自动赋值为 `Application.MainWindow`；`ShutdownMode.OnMainWindowClose` 下 `splash.Close()` 直接触发应用关停，后续方案中心与主窗口只是关停过程中的残留执行，`app.Run()` 正常返回，故无异常日志。

**修复**（结构性，不是属性微调）：

1. 欢迎页改为独立 STA 线程 + 独立 `Dispatcher`，主线程只在其 `Dispatcher.InvokeShutdown()` 时结束它；`Application.MainWindow` 在 `ShowShellWindow` 之前不参与任何判定。
2. 关停模式固定为 `ShutdownMode.OnExplicitShutdown`（`ShellLifecycle.ShutdownModeChoice`，可被测试断言），关停语义不再依赖窗口计数。
3. 关闭路径改为显式 `IApplicationOperation.Dispose()` + `Environment.Exit(0)`，与原厂 `App.MainWindow_Closing` 一致。
4. 欢迎页存活时长由初始化屏障决定，不再使用 900 ms 定时器。

## 3. 构建与验证

| 项目 | 命令/结果 |
|---|---|
| 隔离构建 | `dotnet build src/VisionEditor/VisionEditor.csproj -c Release -p:OutDir=artifacts/codebuddy-verify/VisionEditor/ -p:UseSharedCompilation=false -p:BuildInParallel=false` → 0 警告 0 错误 |
| 标准 Release | `src/VisionEditor/bin/Release/net10.0-windows/VisionEditor.exe` 已重建 |
| 启动壳检查 | `tests/VisionEditor.StartupShellChecks` → 退出码 0，`WelcomeDialog, GuideWindow, host data and close-chain contracts passed.` |
| 进程级冒烟（带命令行方案） | 主窗口出现（标题 `VisionMaster`，句柄非 0），6 秒后仍存活，无 `startup-error.log` |
| 进程级冒烟（无参数，走方案中心） | 欢迎页 → 方案中心（800×546）→ 关闭方案中心 → 主窗口 1366×768 → 6 秒后仍存活，无 `startup-error.log` |

进程级冒烟用的是标准 Release 产物，方案中心通过窗口消息关闭，未注入任何测试开关。

### 3.1 扩展后的启动壳断言清单

1. 配置基线：`ShowEnable`/`ApplicationName`/`IsMultiApp`/`ZoomIndex` 缺键回退；`SetShowEnable` 能回写配置文件。
2. 命令行分流：JSON 进独立槽位，`.sol` 进显式不支持槽位，空参数两槽位均清空。
3. 关停模式不变量为 `OnExplicitShutdown`。
4. 欢迎页 640×360 且使用 `StretchImage` 的 WinForms 图片框（原厂结构）。
5. 方案中心 800×546、最近列表存在、四张卡片正常/悬停双图、四个原厂图标资源。
6. 方案中心最近列表来自 `RecentlySolutions` 依赖属性；`不再显示` 与 `ShowEnable` 同源。
7. 主窗口标题取自 `ApplicationName`、最小尺寸 800×600。
8. `MainWindowData.PreLoad` 从 `ZoomIndex` / `RunningMode` 派生 `ConfigZoomIndex` / `IsNormalMode`。
9. `IsShowProgressMask` / `MaskMessage` 绑定随数据源变化，遮罩层在 `ShellRoot` 上增删。
10. 关闭链：非主窗口关闭不退出、不释放；保存检查失败取消关闭且不释放；主窗口关闭按 `释放 → 落盘窗口状态 → 退出码 0` 顺序执行。

## 4. 既有套件中的两处既有缺陷（本轮未修复，单列）

运行 `tests/VisionEditor.ShellChecks` 时发现两处失败。两者都位于本轮未修改的代码路径，判定为**既有缺陷**，不在启动链重写范围内，特此单列，不计入本轮完成度：

| 编号 | 断言 | 现象 | 根因 | 涉及文件（本轮未改） |
|---|---|---|---|---|
| F1 | `Save menu persists and orders recent solution paths` | 最近打开菜单项数为 3 而非 2 | 主窗口构造期装载启动方案并调用 `RecentSolutionStore.Update`，使最近列表多出一条更早的条目 | `src/VisionEditor/MainWindow.StartupSolution.cs`（前序新增文件） |
| F2 | `File New displays the source NOItem icon and waiting text on an empty canvas` | 图标与文案正确，但 `document.HasCurrentProcedure` 为 `true` | `MainWindow.New()` 调用 `doc.NewSolution(true)` 会创建 `main` 流程，与断言"无当前流程"冲突 | `src/VisionEditor/MainWindow.xaml.cs:175`、`src/VisionEditor.Core/GraphEditorDocument.cs:36,54` |

对 `tests/VisionEditor.ShellChecks/Program.cs` 的任何临时修改都已回退，该文件与仓库版本一致（`git status` 干净）。F1 / F2 的处置需要用户决定：或修正产品行为，或更新断言前提，不在本轮擅自改动。

## 5. 仍未闭合

| 编号 | 内容 | 状态 |
|---|---|---|
| U01 | `SingleInstanceManager` 原厂成员体 | 原厂二进制缺失，本实现为独立实现 |
| U03/U04 | `ProjectManager.LoadSolution`、`.sol` 扩展名常量与容器 | 保持显式不支持提示 |
| U05/U06 | 权限文件格式、`EncryptAPI` 全族 | 只保留 `IAuthGate` 占位与提醒调用点 |
| U07 | `SolutionSaveCheck` 判定细节 | 只保留 `ShellLifecycle.SolutionSaveCheck` 插槽，未接入 |
| U08 | 菜单数据源 `menuitems.xml` | 菜单仍为既有独立实现 |
| U09 | `VmMainViewConfigControl` 内部结构 | 未接入，缩放为独立实现 |
| — | `MainWindowData` 只覆盖启动壳用到的字段 | 菜单/方案树字段未落地 |
| — | 授权提醒 UI | 仅 `ShowAuthorizationReminder` 骨架，未实现原厂弹窗 |

## 6. 人工验收步骤

1. 直接启动 `D:\MyWork\Rea逆向\src\VisionEditor\bin\Release\net10.0-windows\VisionEditor.exe`（不带参数）。
2. 期望：欢迎页 → 方案中心（800×546）→ 选择任一入口 → 主窗口；全程不退出。
3. 关闭主窗口后，任务管理器中不应残留 `VisionEditor.exe`。
4. 再次启动应立即成功（互斥体已释放）；若在已有实例运行中再次启动，应激活已有窗口而不是静默退出。
5. 若出现异常，`%LOCALAPPDATA%\VisionEditor\startup-error.log` 记录启动阶段与堆栈。
