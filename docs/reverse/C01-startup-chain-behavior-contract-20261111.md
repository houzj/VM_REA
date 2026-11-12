# C01 启动链与主窗口壳行为契约

日期：2026-11-11
状态：契约冻结候选（供统一重写使用）
上游文档：`docs/reverse/C01-startup-chain-design-review-20261111.md`（早期差异矩阵，本文件取代其"当前实现"列）、`docs/reverse/C03-startup-initial-ui-evidence.md`（原厂行号锚点）

## 0. 证据边界

### 0.1 证据来源

| 代号 | 来源 | 绝对路径 |
|---|---|---|
| SRC-EXE | 原厂宿主入口反编译源码 | `D:\MyWork\SunEyeVision_V2-param-window-datatree\docs\07-工具逆向\decompiled\HostShell\ExeEntry\VisionMaster\{Program.cs, App.cs, MainWindowData.cs}` |
| SRC-MAIN | 原厂主程序集反编译源码 | `...\decompiled\HostShell\Main\VisionMasterMain\{MainWindow.xaml, MainWindow.xaml.cs, WelcomeDialog.xaml, WelcomeDialog.xaml.cs}` |
| SRC-GUIDE | 原厂方案中心反编译源码 | `...\decompiled\HostShell\Main\VisionMaster.AppMainWindow\{GuideWindow.xaml, GuideWindow.xaml.cs, GuideButton.cs, GuideHelper.cs, ToolBoxType.cs}` |
| CFG | 原厂 `app.config` 实际取值 | `...\decompiled\HostShell\ExeEntry\app.config` |
| ASSET | 已提取原始资源 | `analysis/ui-assets/original-guide/` |
| AUDIT | 既有全量审计证据 | `analysis/full-audit/`（历史批次，勿删） |

### 0.2 证据等级定义

- **源码确认**：原厂反编译源码中存在可复核成员体、签名或调用边。
- **配置确认**：原厂 `app.config` 中存在该键及取值。
- **独立实现**：本项目自行设计，原厂无对应证据。
- **运行未确认**：源码/配置可见但未在原厂运行环境观察；或原厂成员体缺失。

### 0.3 本轮静态核验限制（必须保留）

- 本机 **不存在** 原厂 `VisionMaster.exe`、`VisionMasterMain.dll`、`VM.Core.dll` 等任何原厂托管二进制（已对 `D:\` 全盘按文件名检索，`installer`/`Release`/`libs` 均为空或仅打包脚本）。
- 因此 REA 的 `inspect_managed_artifact` / `inspect_managed_members` / `find_xrefs_to_name` **本轮不可用**。全部结论只能基于反编译源码 + `app.config` + 已提取资源。
- `VM.Utility.SingleInstance.SingleInstanceManager`、`SingleInstanceManagerSetup`、`ApplicationDispatcherInvoker` 的成员体 **不在** 已反编译目录中（仅被 `Program.cs` 引用）。其行为只能按调用点与类型名描述，标为 **运行未确认**。
- 由此，本契约中凡标"源码确认"者均指"反编译源码确认"，不等同于运行时确认。

## 1. 配置基线（配置确认）

`app.config` 实测取值，构成启动分支的默认前提：

| 键 | 取值 | 影响 |
|---|---|---|
| `IsMultiApp` | `0` | 走单实例分支；`1` 时跳过单实例 |
| `ApplicationName` | `VisionMaster` | 单实例名 + 主窗口标题回退 |
| `ShowEnable` | `True` | **同时**控制欢迎页与方案中心是否出现 |
| `FrontendFirst` | `False` | 走 `ShowMainWindow` 而非 `ShowFrontendUI` |
| `IsRunBackground` | `0` | 关闭前需过 `SolutionSaveCheck`，可取消关闭 |
| `CloseSoftwarePasswordEnabled` | `0` | 关闭密码校验关闭 |
| `IsNotifyRunBackground` | `0` | 关闭提示分支 |
| `RunningMode` | `0` | `IsNormalMode = (RunningMode != 1)` |
| `IsMaximizedForVM` | `False` | 启动后最大化 |
| `IsFullScreenForVM` | `False` | 启动后覆盖整屏矩形 |
| `ZoomIndex` | `1.0` | `ConfigZoomIndex` 初值 |
| `UnRedoServiceImplement` | `myLibs\VM.UnRedo.dll` | 撤销服务程序集注入路径 |
| `IsStartTimerGC` | `False` | 5 秒 `GC.Collect` 定时器 |
| `ToolBoxType` | `General` | 方案中心卡片点击后写回 |
| `IsIgnoreGlobalModule` | `1` | `IsIgnoreGlobalModule = true` |
| `ProcessorAffinity` | `0` | 为 `0` 时不设置进程亲和性 |
| `Demo` | `\Samples\` | 示例方案目录 |
| `IsAutoLoadPath` | `False` | 自动加载路径开关 |

## 2. 启动链逐调用点契约

### S00 进程入口（源码确认）

`Program.Main(string[] args)`，`[STAThread]`，`Program.cs:61-74`。

| 顺序 | 操作 | 证据 |
|---|---|---|
| 1 | `Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory)` | `Program.cs:63` |
| 2 | `GetConfig("IsMultiApp") == "1"` → 直接 `StartApp(args)` 并返回 | `Program.cs:64-69` |
| 3 | 否则 `using (SingleInstanceManager.Initialize(GetSingleInstanceManagerSetup())) StartApp(args)` | `Program.cs:70-73` |

`GetSingleInstanceManagerSetup()`（`Program.cs:87-97`）：

- `new SingleInstanceManagerSetup(GetConfig("ApplicationName"))`
- `ArgumentsHandler = args => ((App)Application.Current).ProcessCommandLineArgs(args)`
- `ArgumentsHandlerInvoker = new ApplicationDispatcherInvoker()`

`StartApp`（`Program.cs:76-85`）：`args.Length != 0` → `RefreshRegistry()`（写 `HKCU`/`ClassesRoot` 下 `.<Solution扩展名>` 默认值 `vm310`）；`new App(args).Run()`。

> 约束：工作目录切换必须先于任何相对路径资源加载（`WelcomePage.GIF`、`Resources\guide_logo.png` 均按 CWD 读取）。

### S01 `App` 构造（源码确认）

`App.cs:46-59`。

| 顺序 | 操作 |
|---|---|
| 1 | 读取当前 `CultureInfo`；非 `zh*` → `DefaultThreadCurrentCulture = en-US` |
| 2 | `DispatcherUnhandledException += App_DispatcherUnhandledException` |
| 3 | `AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException` |
| 4 | `TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException` |
| 5 | `EnableTimerGC()`：`IsStartTimerGC` 为真才建 5 秒 `DispatcherTimer` 调 `GC.Collect` |

### S02 欢迎页（源码确认 + 关键生命周期约束）

`App.OnStartup`，`App.cs:100-119`。

- 谓词：`Convert.ToBoolean(GetConfig("ShowEnable"))`。
- 结构：

```text
new Thread(IsBackground = true, ApartmentState = STA) {
    WelcomeDialog w = new WelcomeDialog();
    w.Show();                 // 在子线程自己的 Dispatcher 上
    mWaitEvent.WaitOne();     // 阻塞该线程，直到初始化汇合
    w.Dispatcher.InvokeShutdown();
    w = null;
}.Start();
```

- **不设置 `Application.Current.MainWindow`**；欢迎页不参与应用关停判定。
- `WelcomeDialog` 自身无计时、无关闭逻辑（`WelcomeDialog.xaml.cs:10-15`）：`Window` + `WindowsFormsHost` + `PictureBox`，`Image.FromFile("WelcomePage.GIF")`（CWD 相对路径）。
- Window 属性（`WelcomeDialog.xaml:1`）：`Height=360 Width=640 ResizeMode=NoResize WindowStartupLocation=CenterScreen ShowInTaskbar=True WindowStyle=None Background=#00FFFFFF`。
- 关闭时机由 `mWaitEvent.Set()`（S06 之前）驱动，欢迎页存活时长 == 初始化耗时，**不是固定延时**。

### S03 后台初始化线程（源码确认）

`App.cs:121-143`：`Thread(IsBackground = true, Priority = Highest, ApartmentState = STA)`，`try { Program.BeforeStartPart(); } catch { _subThreadException = ex; } finally { mWaitEventBefore.Set(); }`。

`Program.BeforeStartPart()`（`Program.cs:111-134`）顺序：

| # | 调用 | 说明 | 失败出口 |
|---|---|---|---|
| 1 | `LogHelper.Info("---appBeforeStart--Beigin")` | 日志 | — |
| 2 | `SoftEncrypt()` | `CodeManager.IsUseSoftEncrypt()` → `SystemConfigManager.IsSoftEncrypt` + `StrEncryptType`（软件期限/设备型号/加密狗） | catch → `HikMessageBox` + `Environment.Exit(0)`（`Program.cs:389-415`） |
| 3 | `InitColor()` | `ColorThemesResourceDictionary.GetInstance()` | catch 吞掉（`Program.cs:284-293`） |
| 4 | `EncryptAPI.TrialVersionInit()` | 试用版初始化 | — |
| 5 | `CPUConfig()` | `ProcessorAffinity != "0"` → 按二进制串设置进程亲和性 | catch → `HikMessageBox`（`Program.cs:373-387`） |
| 6 | `CultureInfo.DefaultThreadCurrentCulture = en-US` | 强制 | — |
| 7 | `AccessManagerHelper.LoadAccessInfo()` | 权限配置加载，false → 记日志 + `UILogManager.logCreate` + `throw new Exception("权限配置异常")` | 抛出 → 进 `_subThreadException` |
| 8 | `VmSolution.GetSolutionTakeState()` | false → `VmSolution.StartNewInstance()` | — |
| 9 | `Ioc.Set<IApplicationOperation>(new ApplicationOperation())` | 主窗操作注册 | — |

### S04 主线程预加载（源码确认）

`App.cs:144-156`：

1. `Program.BeforeStartMian()` → `InitMainWindow()`（`Program.cs:159-166`）：
   - `MainData = new MainWindowData()`
   - `MainData.PreLoad()`（`MainWindowData.cs:984-1020`）：`new TitleMenueItemData()` → `loadXml("Resources//menuitems.xml")` → 语言（`MakeServerReportDongleInfo` 成功用 `pstDongleInfo.nSoftLanguage`，否则 `VmSolution.SoftLanguage`）→ 挂 `LangPropertyChanged` → `ToolBoxData.LoadData()`。**失败 → 弹框 + `Environment.Exit(0)`**。
   - `ServerManager.Instance.InitServer(TitleMenueItemData.GetToolFunctionSources)`
   - `NameValueItem.logManager = LogUIBridge.GetInstance()`
2. `new PreLoadPage()`
3. catch → `DoneException(ex)`

### S05 屏障汇合（源码确认）

`App.cs:157-163`：

```text
mWaitEventBefore.WaitOne();          // 等 S03 完成
if (_subThreadException != null) DoneException(_subThreadException);
mWaitEvent.Set();                    // 放行 S02 子线程 → InvokeShutdown 关闭欢迎页
```

> 约束：欢迎页关闭必须发生在**两条初始化线都结束之后**。任一线失败 → 走 `DoneException` → `Environment.Exit(0)`。

### S06 `AppAfterStart`（源码确认）

`App.cs:165` → `Program.AppAfterStart()`（`Program.cs:179-236`），顺序固定：

| # | 行为 | 谓词/细节 |
|---|---|---|
| 1 | 命令行方案 | `Arguments[0]` 取出后立即 `Arguments = null`；`File.Exists(text) && Path.GetExtension(text) == "." + AppFileExtension.Solution` → `flag = true` |
| 2 | 自动加载门 | `!SystemConfigManager.IsAutoLoad() && !MainData.SoftWareData.IsAutoLoadLast` → `ProjectManager.GetInstance().LoadSolution(text, isGuid:false, null, null, isRemoteMode:false, isFromAppStart:true)`；否则 `HikMessageBox.Show(Lang("Auto_load_case"), Warning, NoButton, 500ms)` |
| 3 | 登录用户 | `CodeManager.GetInstance().GetLoginUser()` |
| 4 | **方案中心门控** | `!flag && GuideHelper.IsGuideWindowShow() && !IsAutoLoad() && !IsSdkMode() && !VmSolution.Instance.IsTakeOver && loginUser != 3` |
| 5 | 方案中心显示 | `WecPage(out win)`：`new GuideWindow()` → `DataContext = ProjectManager.GetInstance()` → `SetBinding(RecentlySolutionsProperty, RecentlySolutions, OneWay)` → `SetBinding(OpenSolutionCommandProperty, OpenSolutionCommand)` → `ShowDialog()`；返回后若 `win.Dispatcher != Application.Current.Dispatcher` → `win.Dispatcher.InvokeShutdown()` |
| 6 | 工具箱/自动启动 | `ToolBoxData.ChangePriorityByInfo()`；`SoftWareData.SetAutoLaunchAccess()` |
| 7 | 运行模式 | `MainData.IsNormalMode = int.Parse(GetConfig("RunningMode")) != 1` |
| 8 | 全局模块 | `IsIgnoreGlobalModule = config == null \|\| config == "1"` |
| 9 | 方案文件帮助 | `SolutionFileHelper.GetInstance()` |
| 10 | 结果/采样/回调 | `IsClearLastExecuteResult`、`ImageViewBox.IsDirectDownSampling`、`VmSolution.IsCallBackCacheEnable` → `ExpandCallBackQueueLength(CallBackQueueLength)` 或 `ShutDownCallBackQueue()` |

其中：

- `GuideHelper.IsGuideWindowShow()` = `bool.Parse(GetConfig("ShowEnable"))`（`GuideHelper.cs:34-38`）。
- `Program.IsSdkMode()` = `Arguments[0].Contains("tcp://")`（`Program.cs:636-643`）。

### S07 主窗口 vs 前端窗口（源码确认）

`App.cs:167-185`：`loginUser != 3` → `FrontendFirst ? ShowFrontendUI() : ShowMainWindow()`；`loginUser == 3` → `ShowFrontendUI()`。

### S08 `ShowMainWindow`（源码确认）

`App.cs:354-366`，顺序不可交换：

```text
Application.Current.MainWindow = new MainWindow();
Application.Current.MainWindow.DataContext = Program.MainData;
SetBinding(MainWindow.IsShowProgressMaskProperty, "IsShowProgressMask");
SetBinding(MainWindow.MaskMessageProperty, "MaskMessage");
Application.Current.MainWindow.Closing += MainWindow_Closing;
Application.Current.MainWindow.Show();
Program.ConfigZoomIndex();                                   // MainData.ConfigZoomIndex = double.Parse(GetConfig("ZoomIndex"))
Program.MainData.RecoverMainWindowState(Application.Current.MainWindow);
```

`RecoverMainWindowState`（`MainWindowData.cs:958-964`）**只恢复 `WindowState`**，不恢复 `Left/Top/Width/Height`。

### S09 主窗口显示后（源码确认）

`App.cs:187-228`：

1. `ProjectManager.OpenRecentSolutionInfo != null` → `OpenRecentSolution(info, isGuid:true)`；失败 → `HikMessageBox(Lang_SolOpenErr / Lang_SolOpenErrtoRM, YesOrNo)`，选是 → `RemoveRecentSolution(path)`；随后清空 `OpenRecentSolutionInfo`。
2. 授权提醒线程（`Program.IsNeedRemind()`，软加密剩余天数 ≤3 且值变化）→ 提示 → 是 → `Program.StartAuthActivation()`。**本轮按授权骨架保留，不实现校验。**
3. `IUnRedoService` 注入：`Assembly.LoadFrom(BaseDirectory + GetConfig("UnRedoServiceImplement"))` → `GetTypes().FirstOrDefault(c => c.GetInterfaces().Contains(typeof(IUnRedoService)))` → `Ioc.Set`。
4. `Program.UpdateTitleBinding()` → `SoftWareData.StartSetParamModify()` / `StartSetParamAboutRender()` / `EnableWatchDog()`。
5. `IsMaximizedForVM` → `WindowState = Maximized`；`IsFullScreenForVM` → `Left=0 Top=0 Width=PrimaryScreenWidth Height=PrimaryScreenHeight`。

### S10 关闭链（源码确认）

`App.MainWindow_Closing`（`App.cs:385-397`）：

```text
LogHelper.Info("Main Window close.");
e.Cancel = Program.IsCancelClose();
if (!e.Cancel) {
    UILogManager.logCreate("信息", "Lang_CloseApp", true, true);
    Ioc.Get<IApplicationOperation>()?.Dispose();
    Environment.Exit(0);          // 进程级退出，非 Application.Shutdown
}
```

`Program.IsCancelClose()`（`Program.cs:295-341`）按序判定：

| # | 谓词 | 结果 |
|---|---|---|
| 1 | `IsSdkMode()` | `MainWindow.Visibility = Collapsed`；返回 `true`（取消关闭） |
| 2 | `GetLoginUser() == 3` | 返回 `false`（放行） |
| 3 | `GetConfig("CloseSoftwarePasswordEnabled") == "1"` | `AccesssCodeCheck` 对话框，非 Yes → `true` |
| 4 | `GetConfig("IsRunBackground") == "0" && ProjectManager.SolutionSaveCheck()` | `true`（提醒保存） |
| 5 | `IsThirdSoftwareRun()` | `IsRunBackground=="0"` → 提示 + `true`；否则提示 `Lang_DevelopmentReloadSolution` 后继续 |
| 6 | 其他 | `false` |

### S11 二次实例（源码确认）

`App.ProcessCommandLineArgs(args)`（`App.cs:529-555`）：`MainWindow?.Activate()`；`args[1]` 为存在的 `.<Solution扩展名>` 且 `!IsAutoLoad && !IsAutoLoadLast` → 最小化则还原 → `LoadSolution(..., isFromAppArgs:true)`；异常仅记日志。

### S12 异常出口（源码确认）

| 入口 | 行为 | 证据 |
|---|---|---|
| `DoneException(ex)` | `VmException` → 记录；`IsNeedStartAuth(code)`（`-536869115` / `-536869113`）→ 授权提示 → 是 → `StartAuthActivation()` + `Sleep(5000)` + `ReInitData()`；`-536869093` → 远程加密狗帮助；否则 `ErrorCodeInfo.GetErrorInfo`。非 `VmException` 且含 `VM.Core.TempVmSolution` → 拉起 `Tools\EnvironmentDetectionTool\EnvironmentDetectionTool.exe`。最终 `DeInitExit()` → `VmSolution.Dispose(false)` + `Environment.Exit(0)` | `App.cs:249-315, 335-352` |
| `App_DispatcherUnhandledException` | 记录 + 按类型弹框 + `RecordErrorInfo` 写 `log\error.txt`；**`e.Handled = true`（不退出）** | `App.cs:437-480` |
| `CurrentDomain_UnhandledException` | `MiniDumpUtil.TryWriteMiniDump` + 记录 + 弹框 + `DeInitExit()` | `App.cs:414-432` |
| `TaskScheduler_UnobservedTaskException` | 记录 + 弹框 + `SetObserved()` | `App.cs:482-502` |
| `EnableTimerGC` | `IsStartTimerGC` 为真时每 5 秒 `GC.Collect` | `App.cs:61-79` |

## 3. 方案中心契约（源码确认，`SRC-GUIDE`）

### G01 窗口

`GuideWindow : BaseWindow`，`Title={DynamicResource GuideWindow}`，`WindowStyle=SingleBorderWindow`，`Height=546 Width=800`，`CloseButtonVisibility=Collapsed`，`BorderThickness=0`，`CornerRadius=4`，`ShowInTaskbar=True`（`GuideWindow.xaml:2`）。

### G02 标题栏

- 高 130，`logo_VM.png` 100×48（`Margin=20,9`），版本文字 `Lang_version`（`Margin=138,37`）。
- `ToggleButton buttonShow`，内容 `不再显示`：`Checked → DoNotShow_OnChecked`，`Unchecked → DoNotShow_OnUnchecked`。
- 关闭按钮 `WinCloseBtnStyle` 32×32 → `Close_OnClick`。

### G03 主体

- 底层对角点阵 `DrawingBrush`（`Viewport=0,0,6,6`，`Col_GuideWindow_Background_Line`）。
- 左侧 `Border Width=412`，底对齐，内含 4 个 `GuideButton`（188×130，间距 22）。
- 右侧 `ListView Width=225`，`Margin=0,0,58,56`，`Height` 绑定左侧 `Border.ActualHeight`。
- 底栏高 78，含 `Menu areaGuideLine`（本地/在线帮助等）。

### G04 四张卡片

| 卡片 | `ToolBoxType` | `BackImage` / `BackImageClick` | 标题 | `HelpContent` |
|---|---|---|---|---|
| 通用方案 | `General` | `guide_general.png` / `guide_general_click.png` | 通用方案 | 建立普通空白方案 |
| 定位测量 | `Position` | `guide_position.png` / `guide_position_click.png` | 定位测量 | 通过定位、测量工具进行设计 |
| 缺陷检测 | `Defect` | `guide_defect.png` / `guide_defect_click.png` | Defect Detection | 借助检测工具查找工件缺陷 |
| 识别 | `Recognition` | `guide_recognition.png` / `guide_recognition_click.png` | 用于识别 | 通过识别工具进行方案设计 |

卡片模板：`Image` + `ContentPresenter(Margin=16,50,0,0)` + 底部 `help` TextBlock(FontSize 11)；hover 切换 `BackImageClick` 并把前景改白（`GuideWindow.xaml:25-50`）。

### G05 卡片点击

`GuideButton_OnClick`（`GuideWindow.xaml.cs:124-131`）：`sender is GuideButton` → `ConfigurationHelper.SaveConfig("ToolBoxType", guideButton.ToolBoxType.ToString())` → `Close()`。**不设置 `DialogResult`，不直接打开方案**。

> 即：卡片语义是"选择工具箱类型并关闭方案中心"，不是"打开某个示例文件"。

### G06 最近方案列表

`ListView ItemsSource = {Binding RecentlySolutions, RelativeSource FindAncestor GuideWindow}`；两列：`FileName`（`LongTextConvert`，`ToolTip=SolutionPath`）与 `LastTimeStr`。`ListViewItem` 通过 `Attached:AttachedRoutedEvent` 在 `MouseDoubleClick` 执行 `SelectSolutionCommand`，`CommandParameter = {x:Reference guide}`（`GuideWindow.xaml:81-83, 164-191`）。

### G07 打开方案图标

`GridViewColumnHeader` 内 `IconMenuItem`（`guide_open_normal.png` / `guide_open_click .png`）→ `OpenFile_OnClick`：`OpenSolutionCommand?.Execute(null)` + `Close()`。

### G08 示例打开

`Open_Demo`：`GuideHelper.OpenSolution(opendemo:true, out filename)`（`GuideHelper.cs:68-112`）——`OpenFileDialog` 过滤 `*.<Solution扩展名>`，初始目录取 INI `OpenSample` 或 `BaseDirectory + GetConfig("Demo")`；目录不存在 → `HikMessageBox(Not_Installed_Samples)`；文件存在 → `OpenSolutionCommand?.Execute(filename)` + `Close()`。

> 注意：`GuideHelper.OpenSolution` 的 `Filter` 由 `string.Format($"{0}|*.{AppFileExtension.Solution}", ...)` 构成，属反编译产物，格式串本身不可靠；只取"文件对话框 + 示例目录"语义。

### G09 "不再显示"

- `DoNotShow_OnChecked` → `GuideHelper.SetGuideWindowOff()` = `SaveConfig("ShowEnable", "false")`。
- `DoNotShow_OnUnchecked` → `SetGuideWindowOn()` = `SaveConfig("ShowEnable", "true")`。
- `GuideWindow_Loaded` → `buttonShow.IsChecked = !IsGuideWindowShow()`。

> 关键结论：**同一个 `ShowEnable` 同时决定欢迎页与方案中心**。勾选"不再显示"会同时关闭两者。

### G10 关闭按钮

`Close_OnClick` → `DialogResult = true` 后 `Close()`（`GuideWindow.xaml.cs:108-112`）。其余路径（卡片、打开、示例）均只 `Close()`，`DialogResult` 为 `null`/`false`。

### G11 资源依赖（ASSET）

`logo_VM.png`、`guide_check-box_off.png` / `guide_check-box_on.png`、`guide_general(. _click).png`、`guide_position(_click).png`、`guide_defect(_click).png`、`guide_recognition(_click).png`、`guide_logo.png`、`guide_open_normal.png`、`guide_open_click .png`（原厂文件名含空格）、`ic_1/ic_2/ic_3`。

## 4. 主窗口壳契约（源码确认，`SRC-MAIN`）

### W01 类型与窗口属性

`VisionMasterMain.MainWindow : BaseWindow`（`MainWindow.xaml:1`）：

`Title="VisionMaster"`，`Height=768 Width=1366`，`MinWidth=800 MinHeight=600`，`ResizeMode=CanResize`，`FontSize=13`，`FontFamily="Microsoft YaHei"`，`ShowInTaskbar=True`，`SizeAble=True`，`CloseButtonVisibility/MinButtonVisibility/MaxButtonVisibility=Collapsed`，`BorderThickness=0`；挂 `Closed=Window_Closed`、`PreviewKeyDown=BaseWindow_PreviewKeyDown`、`Closing=BaseWindow_Closing`（三个处理体均为空，`MainWindow.xaml.cs:112-120`）。

构造函数（`MainWindow.xaml.cs:80-87`）：`InitializeComponent()` → `CurrentUIHelper.Instance.Dispatcher.Invoke(()=>{})` → `Title = GetConfig("ApplicationName") == "" ? "VisionMaster" : GetConfig("ApplicationName")`。

### W02 标题栏进度遮罩联动

`localApp:TitleBar m_TitleMy` 的 `IsButtonClickEnabled` = `{Binding IsShowProgressMask, ElementName=MainView, Converter=negateConverter}`。

### W03 主视图

`Grid PART_View`（`Style=WindowMainPanelStyle`，`Maximized` 时 `Margin=0,0,0,7`）→ `Border` → `VMWpf:VmMainViewConfigControl Name=MainView`：

- `IsParamsEditState = {Binding IsAdmin, Source={x:Static user:CodeManager.Instance}}`
- `SelectedModule = {Binding SelectedModule, Mode=TwoWay}`
- `IsLockWorkArea = {Binding IsWorkAreaLocking, OneWay}`
- `IsOpenParams = {Binding IsOpenParams, Mode=TwoWay}`
- `ZoomIndex = {Binding ConfigZoomIndex, Mode=TwoWay}`
- `SectionChanged` → `ParamsEventCommand Command={Binding SelectionChangedCommand}`

### W04 宿主数据容器（`MainWindowData`）

- `IsShowProgressMask` / `MaskMessage` / `ConfigZoomIndex` / `IsNormalMode` 均为 `INotifyPropertyChanged` 属性（`MainWindowData.cs:252-352`）。
- `IsShowProgressMask = true` 时同时设置 `MaskMessage`（`MainWindowData.cs:1226-1237`），完成后经 `Dispatcher.BeginInvoke` 置回 `false`。
- `PreLoad` / `RecoverMainWindowState` 见 S04 / S08。

### W05 进度遮罩

`IsShowProgressMaskProperty` 变化 → `AddMask()` / `RemoveMask()`（`MainWindow.xaml.cs:48-78`）：

- `new ProgressMask { IsShowProgressBar = false }`
- 绑定 `Message ← MaskMessage`、`Value ← Progress`（源为窗口自身）
- `Grid.ColumnSpan = 2`，加入 `PART_View.Children`

### W06 焦点转发

`SeMenuFocus(UIElement element)`（`MainWindow.xaml.cs:89-100`）：向上找 `MainWindow`，向下找 `ProcedureConfigControl`，`FocusManager.SetFocusedElement(procedureConfigControl.curTabControlBase, element)`。

## 5. 未确认项（不得据产品意图补写）

| 编号 | 内容 | 原因 |
|---|---|---|
| U01 | `SingleInstanceManager` / `SingleInstanceManagerSetup` / `ApplicationDispatcherInvoker` 的成员体与二次实例转发时序 | 成员体未反编译，二进制缺失 |
| U02 | 欢迎页在原厂环境的实际可见时长 | 取决于初始化耗时，未在该环境观测 |
| U03 | `ProjectManager.LoadSolution` 内部（场景/流程/结果缓存/回滚） | 成员体未取全 |
| U04 | `AppFileExtension.Solution` 的实际扩展名字符串 | 常量值未取到 |
| U05 | `AccessManagerHelper.LoadAccessInfo` 的权限文件格式 | 授权线，本轮不逆向 |
| U06 | `EncryptAPI` 全族（试用、加密狗、软加密） | 按用户要求只留骨架 |
| U07 | `SoftWareData.SolutionSaveCheck` 的判定细节 | 属方案保存线，未取全 |
| U08 | `TitleMenueItemData.loadXml` 对 `menuitems.xml` 的菜单树展开规则 | 菜单入口线，另有 A05 批次文档 |
| U09 | `VmMainViewConfigControl` 内部标签页/画布/Group 结构 | 属控件库，未反编译 |

## 6. 对统一重写的强制约束

1. **欢迎页必须脱离应用生命周期**：独立线程 + 独立 `Dispatcher`，`Application.MainWindow` 在 `ShowMainWindow` 之前保持 `null`；禁止用 `ShutdownMode` 承担关停语义。
2. **关闭路径必须走 `Environment.Exit(0)`**（`IApplicationOperation.Dispose()` 之后），不得依赖 `OnMainWindowClose`。
3. **欢迎页关闭时机 = 双线汇合点**，不得用固定延时替代。
4. **`ShowEnable` 单一开关**同时决定欢迎页与方案中心；"不再显示"写回该开关。
5. **卡片语义** = 写 `ToolBoxType` + 关闭，不打开文件。
6. **主窗口必须先有 `DataContext` 与两个绑定的挂接，再 `Show()`**，`Show()` 后才做 `ConfigZoomIndex` 与状态恢复。
7. **`DispatcherUnhandledException` 必须 `Handled = true`**（原厂语义：记录而不退出），与 `AppDomain.UnhandledException` 的退出路径分离。
8. **授权**：保留 `StartAuthActivation` 调用点、提醒分支与失败提示结构；校验实现替换为恒放行的接口占位，并在文档中标注为骨架。
