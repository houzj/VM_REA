# C01 启动链与主窗口壳对照矩阵

日期：2026-11-11
左列依据：`docs/reverse/C01-startup-chain-behavior-contract-20261111.md`（证据等级同该文件定义）
右列依据：`d:/MyWork/Rea逆向/src/VisionEditor/` 当前工作树（含 105 项未提交改动）

## 0. 判定口径

| 判定 | 含义 |
|---|---|
| 已对齐 | 行为、顺序与副作用均与契约一致 |
| 偏差 | 存在对应物，但顺序、谓词或副作用不一致 |
| 缺失 | 无对应物 |
| 独立实现 | 原厂无等价证据，属本项目自定语义，不算偏差也不算缺失 |
| 待补骨架 | 本轮按用户要求只保留接口/UI 占位 |

## 1. 「过欢迎页即退出」根因（已定位）

### 1.1 结论

**`StartupWelcomeWindow` 是本进程第一个被构造的 `Window`，因而被 WPF 自动赋值为 `Application.MainWindow`；在 `ShutdownMode.OnMainWindowClose` 下，`splash.Close()` 直接触发应用关停。** 之后执行的方案中心与主窗口构造只是关停过程中的残留执行，`app.Run()` 正常返回，所以没有任何异常日志。

### 1.2 机制链（逐行）

| 步 | 位置 | 代码 | 语义 |
|---|---|---|---|
| 1 | `src/VisionEditor/Program.cs:16` | `new System.Windows.Application { ShutdownMode = ShutdownMode.OnMainWindowClose }` | 关停条件绑定到 `Application.MainWindow` |
| 2 | `src/VisionEditor/Program.cs:28-29` | `var splash = new StartupWelcomeWindow(); splash.Show();` | splash 是第一个构造的 `Window` → WPF 自动 `Application.MainWindow = splash` |
| 3 | `src/VisionEditor/Program.cs:33-34` | `timer.Stop(); splash.Close();` | 关闭 `MainWindow` → 进入 `Application` 关停序列 |
| 4 | `src/VisionEditor/Program.cs:25` | `app.MainWindow = editor;` | 赋值时机晚于关停启动，无法挽回 |
| 5 | `src/VisionEditor/Program.cs:54` | `app.Run()` 正常返回 | 无异常 → 不进 `catch` → 不写 `startup-error.log` |

### 1.3 与观察证据的一致性

| 观察 | 解释 |
|---|---|
| 看到欢迎页 | splash 正常显示 900 ms |
| 之后出现方案中心并可用 | `ShowDialog` 的嵌套消息循环在关停过程中仍可运行 |
| `%LOCALAPPDATA%\VisionEditor\guide-hidden.txt` 被写为 `0`（13:56:32） | `Program.cs:39` 在 `ShowDialog` 返回后写入 |
| `recent-solutions.json` 更新（13:56:33，143 字节） | `MainWindow` 构造函数内 `LoadStartupSolution` → `TryLoadStartup` → `RecentSolutionStore.Update` |
| 进程静默退出、无 `startup-error.log` | 关停由 `ShutdownMode` 触发，不是异常 |
| 无残留进程、互斥体已释放 | `using` 正常释放 |

### 1.4 为什么现有测试没抓到

`tests/VisionEditor.StartupShellChecks/Program.cs:15` 显式使用 `ShutdownMode.OnExplicitShutdown` 规避生产关停模式，且从未构造第一个 `Window` 与 `Close()` 的组合。全 `tests/` 目录不存在任何使用 `OnMainWindowClose` 的用例。因此"欢迎页关闭是否终止应用"在自动化里被系统性屏蔽。

### 1.5 与原厂结构的对照

原厂把 `WelcomeDialog` 放在**独立 STA 线程 + 独立 Dispatcher**，`Application.MainWindow` 直到 `App.ShowMainWindow()`（`App.cs:358`）才被赋值；关闭走 `Environment.Exit(0)`（`App.cs:395`），全程不使用 `ShutdownMode`。本次偏差不是"某个属性选错"，而是**生命周期模型与契约不同构**。

### 1.6 重写后必须新增的回归

1. 构造 `StartupWelcomeWindow` → `Show()` → `Close()`，断言 `Application.Current.MainWindow` 未被赋值为 splash，且 `Application` 仍在运行。
2. 双线汇合前 splash 不得关闭；汇合后必须关闭。
3. `IsCancelClose` 为真时 `Closing` 被取消，为假时走 `Environment.Exit(0)` 等价出口（可用可注入的退出委托观测，避免测试真的杀进程）。
4. `ShowEnable = false` 时欢迎页与方案中心**同时**不出现。
5. 二次实例转发：`args[1]` 为方案文件时激活主窗口并按 `isFromAppArgs` 语义加载。

## 2. 启动链对照矩阵

| 契约 | 原厂证据 | 当前实现 | 判定 | 处置 |
|---|---|---|---|---|
| S00 工作目录切换 | `Program.cs:63` | `StartupLaunch.Prepare` 内 `Directory.SetCurrentDirectory`（`StartupLaunch.cs:34`） | 已对齐 | 保留 |
| S00 `IsMultiApp` 分流 | `Program.cs:64-69` | 无 | 缺失 | 重写期补：读配置，`1` 时跳过单实例 |
| S00 单实例 | `Program.cs:70-73`（成员体未反编译，U01） | `StartupInstance.TryAcquire`（`StartupLaunch.cs:19-23`，Mutex 名 `VisionEditor.C01.SingleInstance`） | 偏差 | 重写为独立组件；名字与转发语义按契约组件化，标注"独立实现（原厂成员体未知）" |
| S00 二次实例转发 | `Program.cs:91-96` → `ProcessCommandLineArgs`（`App.cs:529`） | 无：第二实例 `return` 后 `using` 释放非自身持有的 Mutex（`StartupLaunch.cs:24`），异常被吞 | 缺失 | 重写：`ArgumentsHandler` 等价管道 + 主窗口激活 + `isFromAppArgs` 加载 |
| S00 注册表扩展名刷新 | `Program.cs:78-81, 99-109` | 无 | 缺失 | 重写期按需补；若补需显式声明写注册表副作用 |
| S01 `App` 构造钩子 | `App.cs:46-59` | 仅 `DispatcherUnhandledException`（`Program.cs:17-21`） | 偏差 | 引入 `App : Application`，三个异常入口分离 |
| S01 `IsStartTimerGC` | `App.cs:61-79` | 无 | 缺失 | 低优先，按配置实现 |
| S02 欢迎页独立线程/Dispatcher | `App.cs:100-119` | splash 建在 UI 线程（`Program.cs:28-29`） | **偏差（根因）** | 重写：独立线程 + 独立 Dispatcher + 门控 |
| S02 欢迎页尺寸/样式 | `WelcomeDialog.xaml:1` 640×360 `WindowStyle=None` `CenterScreen` | `StartupWelcomeWindow.xaml` 640×360 契约，测试断言 | 已对齐 | 保留，补 `ShowInTaskbar=True` 与透明背景核对 |
| S02 欢迎页 GIF 加载 | `WelcomeDialog.xaml.cs:14` `Image.FromFile("WelcomePage.GIF")`（CWD 相对） | 读 `ui-assets/original-guide/WelcomePage.GIF`（`StartupWelcomeWindow.xaml.cs`） | 偏差（资源根不同） | 重写：按 CWD/BaseDirectory 解析，路径来源显式化 |
| S02 关闭时机 | `App.cs:110-112`（`mWaitEvent`） | 固定 900 ms `DispatcherTimer`（`Program.cs:30-34`） | 偏差 | 重写：改为双线汇合门控 |
| S03 后台初始化线程 | `App.cs:121-143`（STA、Highest、异常收集） | `StartupLaunch.CompleteInitialization`（`StartupLaunch.cs:50-70`）在 UI 线程 `Wait()` 等 STA worker，worker 内为空实现 | 偏差 | 重写：真实阶段化初始化 + 异常合并 |
| S03 `SoftEncrypt` | `Program.cs:389-415` | 无 | 待补骨架 | 只保留 `IsSoftEncrypt`/`StrEncryptType` 占位与失败提示结构 |
| S03 `CPUConfig` | `Program.cs:373-387` | 无 | 缺失 | 低优先，按配置实现 |
| S03 权限加载 | `Program.cs:121-126` | 无（开发模式跳过） | 待补骨架 | 授权骨架内保留失败出口 |
| S03 方案占用状态 | `Program.cs:128-132` | 无 | 缺失 | 重写期为独立实现（无 SDK），标注独立 |
| S04 主数据预载 | `Program.cs:159-166` + `MainWindowData.cs:984-1020` | 无 `MainWindowData`；`MainWindow` 构造函数直接装配（`MainWindow.xaml.cs:48`） | 缺失 | 重写：新增宿主数据容器，承载进度遮罩/缩放/窗口状态 |
| S04 菜单数据 | `MainWindowData.PreLoad` → `Resources//menuitems.xml` | 菜单硬编码在 `MainWindow.xaml.cs:51`（`BuildChrome`） | 独立实现 | 本轮不做菜单数据源替换（U08 未闭合），矩阵保留为独立实现 |
| S05 屏障汇合 | `App.cs:157-163` | 无（无欢迎页/初始化耦合） | 缺失 | 重写：与 S02/S03 同批实现 |
| S06 命令行 `.sol` 分流 | `Program.cs:187-203` | `StartupSolutionStore.CommandLinePath` + `LoadStartupSolution`（`MainWindow.StartupSolution.cs:30-35`），任意扩展名走 `doc.Load` | 偏差 | 重写：扩展名与自动加载门按契约；`.sol` 保持显式不支持提示（U04） |
| S06 方案中心门控谓词 | `Program.cs:206` 六项与 | `Program.cs:35` 三项与 | 偏差 | 重写为单点谓词函数，逐项可测 |
| S06 方案中心数据上下文与绑定 | `Program.cs:247-268`（`DataContext=ProjectManager`，绑定 `RecentlySolutions`/`OpenSolutionCommand`） | 无 `DataContext`；`RecentList.ItemsSource` 直接赋值（`StartupGuideWindow.xaml.cs:31`） | 独立实现 | 重写为绑定式，标注独立（无 `ProjectManager`） |
| S06 运行模式/回调缓存等 | `Program.cs:218-235` | 部分：`ShellRunningModePreference`（`MainWindow.xaml.cs:48`） | 偏差 | 重写期归入宿主数据容器 |
| S07 前端窗口分流 | `App.cs:167-185` | 无 `loginUser` 概念 | 缺失 | 授权骨架内保留 `loginUser` 占位，恒为"非 3" |
| S08 `DataContext` 与两个绑定 | `App.cs:359-361` | 无 `DataContext`、无 `IsShowProgressMask`/`MaskMessage`（全 `src/` 零命中） | 缺失 | 重写：主窗口 DPs + 遮罩层 + 绑定 |
| S08 `Show()` 顺序 | `App.cs:363-365` | `new MainWindow()` → `Show()`（`Program.cs:24-26`），无 `ConfigZoomIndex`/状态恢复 | 偏差 | 重写按契约顺序 |
| S08 状态恢复 | `MainWindowData.cs:958-964`（仅 `WindowState`） | 无；仅 `ShellWindowStateChanged` 同步图标（`MainWindow.ShellWindowState.cs:7-16`） | 缺失 | 重写：宿主持久化 `WindowState` |
| S09 最近方案自动打开 | `App.cs:187-194` | 无独立"打开后失败提示"分支；最近方案在构造期加载（`MainWindow.StartupSolution.cs:36-50`） | 独立实现 | 保留为独立实现并标注 |
| S09 授权提醒 | `App.cs:195-209` | 无 | 待补骨架 | 保留调用点与提示结构，不实现校验 |
| S09 撤销服务注入 | `App.cs:210-214` | 无 | 缺失 | 本轮不接入（`VM.UnRedo.dll` 不存在）；标注缺失原因 |
| S09 最大化/全屏 | `App.cs:216-228` | 无 | 缺失 | 重写：按配置实现 |
| S10 关闭判定 | `Program.cs:295-341` | 仅"运行中排空"（`MainWindow.xaml.cs:88-107`） | 偏差 | 重写：`IsCancelClose` 等价判定 + 排空并存 |
| S10 退出路径 | `App.cs:395` `Environment.Exit(0)` | 依赖 `ShutdownMode.OnMainWindowClose` | **偏差（根因）** | 重写：显式退出出口 |
| S12 未处理异常 | `App.cs:437-502` | `Handled = false`（`Program.cs:20`） | 偏差 | 重写：`Handled = true`，退出交给 `AppDomain` 分支 |

## 3. 方案中心对照矩阵

| 契约 | 原厂证据 | 当前实现 | 判定 | 处置 |
|---|---|---|---|---|
| G01 尺寸 800×546 | `GuideWindow.xaml:2` | `StartupGuideWindow.xaml` + 测试断言 | 已对齐 | 保留 |
| G01 `CloseButtonVisibility=Collapsed` | 同上 | 自绘关闭按钮 | 已对齐 | 保留 |
| G02 标题栏 130 + logo + 版本 | `GuideWindow.xaml:113-131` | 已恢复 logo 与层级 | 已对齐 | 保留 |
| G03 对角点阵背景 + 左 412 + 右 225 + 底 78 | `GuideWindow.xaml:132-192` | 已按原厂恢复 | 已对齐 | 保留 |
| G04 四卡尺寸/图片/hover | `GuideWindow.xaml:150-161` | 4 卡 + `Tag`/`CommandParameter` 双图（`StartupGuideWindow.xaml.cs:38-50`） | 已对齐 | 保留 |
| G05 卡片语义 | `GuideWindow.xaml.cs:124-131` 写 `ToolBoxType` + `Close()` | "通用"建新方案；其余三卡打开仓库示例 JSON（`StartupGuideWindow.xaml.cs:54-58`） | **偏差（语义替换）** | 重写：改为写 `ToolBoxType` + 关闭；示例入口只保留在 `Open_Demo` |
| G06 最近方案绑定与双击 | `GuideWindow.xaml:81-83, 164-191` | `ItemsSource` 直赋 + `RecentDoubleClick`（`:59`） | 偏差 | 重写为绑定式，行为保留 |
| G07 打开图标语义 | `OpenFile_OnClick` → `OpenSolutionCommand.Execute(null)` | `OpenFileClick` 直接弹 `OpenFileDialog` 并返回路径（`:60`） | 偏差 | 重写：命令化，参数语义对齐 |
| G08 示例目录 | `GuideHelper.OpenSolution` + `Demo`/`OpenSample` | 映射到仓库 `examples/*.vmx.json`（`:61`） | 独立实现 | 保留并标注 |
| G09 `ShowEnable` 单一开关 | `GuideHelper.cs:34-38, 58-66` | 独立 `guide-hidden.txt`（`StartupLaunch.cs:84-99`），且**不控制欢迎页** | **偏差** | 重写：统一到同一开关 |
| G10 关闭 `DialogResult=true` | `GuideWindow.xaml.cs:108-112` | `CloseClick` 设 `DialogResult=false`（`:66`） | 偏差 | 重写：对齐 true |
| G11 资源完备 | `Resources/*.png` | `analysis/ui-assets/original-guide/` 已含 20 个文件 | 已对齐 | 保留；核对含空格文件名 `guide_open_click .png` |

## 4. 主窗口壳对照矩阵

| 契约 | 原厂证据 | 当前实现 | 判定 | 处置 |
|---|---|---|---|---|
| W01 标题/尺寸/最小尺寸/字体 | `MainWindow.xaml:1` | `MainWindow.xaml:5-6` 完全一致（`Title=VisionMaster` 1366×768 800×600 YaHei 13） | 已对齐 | 保留 |
| W01 标题取 `ApplicationName` | `MainWindow.xaml.cs:85` | 硬编码 `Title="VisionMaster"`（XAML） | 偏差 | 重写：构造期按配置赋值 |
| W01 三个空处理体 | `MainWindow.xaml.cs:112-120` | 无对应 | 独立实现 | 无需补 |
| W02 标题栏按遮罩禁用 | `MainWindow.xaml` `IsButtonClickEnabled` | 无 | 缺失 | 重写：随 W04 一起 |
| W03 `ZoomIndex` 双向绑定 | `MainWindow.xaml` `ZoomIndex={Binding ConfigZoomIndex, TwoWay}` | 画布缩放为独立字段 `scale`（`MainWindow.xaml.cs:50`） | 独立实现 | 保留独立，补宿主数据承载（不宣称等价） |
| W04 宿主数据容器 | `MainWindowData.cs:252-352` | 无 | 缺失 | 重写：新增 `MainWindowData` 等价物 |
| W05 进度遮罩 DPs + `ProgressMask` | `MainWindow.xaml.cs:48-78` | 无 | 缺失 | 重写：Deps + 遮罩层 + `PART_View` 等价容器 |
| W06 焦点转发 | `MainWindow.xaml.cs:89-100` | `SetShellMenuFocus`（`MainWindow.xaml.cs:58`） | 已对齐 | 保留 |
| 关闭排空 | 原厂无等价（`Environment.Exit` 直退） | `DrainWorkflowBeforeClose`（`MainWindow.xaml.cs:88-107`） | 独立实现 | 保留，但必须与 `IsCancelClose` 合流，且不得成为永不关闭的僵进程 |

## 5. 重写顺序（依据本矩阵）

1. **生命周期骨架**：`App : Application` + 独立欢迎页线程 + 双线屏障 + 显式退出出口（解决 S01/S02/S03/S05/S10/S12）。
2. **门控与开关**：`ShowEnable` 单一开关（S02/S06/G09）、方案中心门控谓词（S06）。
3. **宿主数据**：`MainWindowData` 等价物 + 主窗口 DPs + 遮罩 + 状态恢复 + 标题取配置（S04/S08/S09/W01/W02/W04/W05）。
4. **方案中心语义**：卡片写 `ToolBoxType`、命令化打开、`DialogResult`（G05/G06/G07/G10）。
5. **单实例与转发**：二次实例管道（S00/S11）。
6. **授权骨架**：`IAuthGate` 恒放行 + 保留调用点与提示（S03/S07/S09）。

## 6. 本轮明确不做

- 菜单数据源从 `menuitems.xml` 重建（U08 未闭合，现为独立实现）。
- `ProjectManager.LoadSolution`、`.sol` 容器、场景载荷（U03/U04）。
- 原厂控件库 `VmMainViewConfigControl` 内部结构（U09）。
- 加密狗/授权校验实现（U06）。
- 工具插件工作树的任何改动。

## 7. 风险与守则

| 风险 | 处置 |
|---|---|
| 重写 `Program.cs`/新增 `App` 时与 WPF 自动 `MainWindow` 赋值冲突 | 欢迎页在独立 Dispatcher，`Application.Current.MainWindow` 保持 `null` 直到 `ShowMainWindow` |
| `Environment.Exit` 使测试无法观测 | 引入可注入退出委托，生产实现为 `Environment.Exit(0)` |
| 测试用 `OnExplicitShutdown` 掩盖问题 | 新增用例必须显式覆盖 `OnMainWindowClose` 与"关闭首个窗口"组合 |
| `StartupShellChecks` 反射调用 `StartupLaunch.CompleteInitialization` | 重写时同步维护探针，或改为调用新入口 |
| `obj` 输出被占用 | 继续使用独立 `OutDir` + `UseSharedCompilation=false` + `BuildInParallel=false` |
| 工作树 105 项未提交改动 | 不 reset、不整树清理；只改 `src/VisionEditor`、`tests/VisionEditor.StartupShellChecks` 与 `docs/reverse` |
