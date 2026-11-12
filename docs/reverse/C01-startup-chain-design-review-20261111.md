# C01 启动链路全链路设计审查

日期：2026-11-11

本文件先于下一轮启动代码实施。它把原厂反编译、原厂 XAML/CIL 台账、用户提供的启动截图和当前独立实现分开记录。截图用于确认用户观察到的界面差异，不能单独定义行为。未有原厂调用证据的行为标为“运行未确认”。

## 1. 原厂证据基线

### 1.1 进程入口和初始化线程

原厂 `VisionMaster.exe` 的入口链在 `docs/architecture-recovery/execution-architecture.json`、`entry-routing-security.edges.json` 和 `entry-chain-focus.json` 中确认。

```text
Program.Main
  → Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory)
  → ConfigurationHelper.GetConfig("IsMultiApp")
  → SingleInstanceManager.Initialize(...)（IsMultiApp != "1"）
  → StartApp(args)
  → new App(args).Run()
  → App.OnStartup
  → 高优先级 STA 线程 BeforeStartPart
  → mWaitEventBefore.WaitOne()
  → DoneException / 失败恢复
  → Program.AppAfterStart
  → 登录用户和启动模式判断
  → App.ShowMainWindow
```

`BeforeStartPart` 的源码摘录确认了 SoftEncrypt、InitColor、TrialVersionInit、CPUConfig、文化设置、权限加载、`VmSolution.GetSolutionTakeState` 和 `VmSolution.StartNewInstance`。等待事件在 `finally` 中设置，主启动线程不会把创建线程当作初始化完成。线程是否在当前原厂安装环境实际执行到每个分支，运行未确认。

认证和加密狗不属于当前启动交付的阻塞条件。原厂授权调用保留为独立可插拔阶段，当前自研启动实现采用开发模式跳过认证并继续进入方案选择和编辑器。这是当前项目的独立实现策略，不代表原厂失败路径，也不把授权状态写成已完成行为。

### 1.2 主窗口数据容器

`Program.InitMainWindow` 的反编译摘录确认顺序如下。

```text
new MainWindowData()
  → MainData.PreLoad()
  → ServerManager.Instance.InitServer(TitleMenueItemData.GetToolFunctionSources)
  → NameValueItem.logManager = LogUIBridge.GetInstance()
```

`App.ShowMainWindow` 的反编译摘录确认顺序如下。

```text
Application.Current.MainWindow = new MainWindow()
  → DataContext = Program.MainData
  → 绑定 IsShowProgressMask / MaskMessage
  → 绑定 Closing
  → Show()
  → Program.ConfigZoomIndex()
  → Program.MainData.RecoverMainWindowState(window)
```

当前没有证据表明 `MainWindow` 构造函数可以替代 `MainWindowData.PreLoad`、服务器初始化或状态恢复。这些属于待实现差异。

### 1.3 方案和场景选择

`Program.AppAfterStart` 的已确认摘录是当前最重要的方案入口证据。

```text
Arguments[0]
  → 清空 Arguments
  → File.Exists(path) && Path.GetExtension(path) == "." + AppFileExtension.Solution
  → flag = true
  → !SystemConfigManager.IsAutoLoad() && !MainData.SoftWareData.IsAutoLoadLast
  → ProjectManager.GetInstance().LoadSolution(
       path, isGuid:false, null, null,
       isRemoteMode:false, isFromAppStart:true)
```

如果自动加载状态阻止命令行方案，原厂显示 `Auto_load_case` 提示。`SystemConfigManager.IsAutoLoad` 只把 SDK 的 `nIfAutoLoad == 1` 解释为启用自动加载。`IsAutoLoadLast` 是另一个状态，赋值时还会从 SDK 读取方案并更新最近方案。密码、路径和执行状态由 `IMVS_AUTO_LOAD_SOLUTION_INFO` 通过 `GetAutoLoadSolution` / `SetAutoLoadSolution` 传递。

命令行方案的原厂扩展名是 `AppFileExtension.Solution`，当前证据没有把它证明为 JSON 或 `.vmx.json`。当前实现把 `.vmx.json` 当作自有格式，不能宣称等价。

`ProjectManager.LoadSolution` 后续的场景、流程、结果缓存、错误回滚和主窗载荷回写，当前证据没有闭合，均标为运行未确认。原厂方法体和资源缺失时不能以当前 JSON 载入逻辑补齐。

### 1.4 引导页和启动 Splash

`VisionMasterMain.WelcomeDialog` 的反编译 XAML/C# 已确认。

```text
WindowStyle=None
Width=640, Height=360
WindowStartupLocation=CenterScreen
WindowsFormsHost → PictureBox pictureBoxLoading
pictureBoxLoading.Image = Image.FromFile("WelcomePage.GIF")
```

用户提供的第一张截图与该 `WelcomeDialog` 证据相符，显示细节来自 `WelcomePage.GIF`，不是当前工程的推断绘制。

第二张截图对应原厂引导/方案选择阶段的外观。`AppAfterStart` 已确认存在 `GuideHelper.IsGuideWindowShow()`、`WecPage(out win)` 和窗口调度分支，但当前本地反编译资料没有闭合 `GuideWindow` 的完整类型、资源和点击后的方案载荷。该界面中的卡片、最近方案列表、版本文字和“不再显示”行为标为运行未确认。当前不能按截图直接实现这些语义。

## 2. 当前独立实现的实际链路

```text
Program.Main(args)
  → StartupLaunch.Prepare（工作目录、单实例、开发模式认证、参数分流）
  → WelcomeDialog（安装包 WelcomePage.GIF）
  → GuideWindow（四类入口、最近方案、文件打开）
  → MainWindow 构造函数注册独立 managed modules
  → GraphEditorDocument / execution scope 初始化
  → LoadStartupSolution()
  → 独立 .vmx.json / 最近方案 / 默认方案 / 新建方案
  → GraphEditorDocument.Load / NewSolution
  → RefreshAll()
  → MainWindow.Show
```

当前实现可以保存和重开独立 `.vmx.json`，也可以在 `GraphExecutor` / `SolutionExecutor` 中运行当前图。现已补齐有原厂窗口与资源证据的 WelcomeDialog、GuideWindow 和独立方案入口。它仍没有原厂 `App`、`MainWindowData`、`BeforeStartPart` 的真实服务初始化、等待事件、SDK 自动加载状态、`ProjectManager.LoadSolution` 和原厂 `.sol` 容器。最近方案恢复、默认方案和示例卡片映射是独立实现，不能当成原厂最近方案或场景载荷行为。

## 3. 差异矩阵

| 阶段 | 原厂证据 | 当前实现 | 结论 |
|---|---|---|---|
| 进程工作目录 | `Program.Main` 静态确认 | 未设置 | 有证据缺陷 |
| 单实例 | `IsMultiApp` / `SingleInstanceManager` 静态确认 | 未实现 | 有证据缺陷 |
| 后台授权与权限初始化 | `BeforeStartPart` 静态确认 | 认证适配器暂不接入，开发模式继续启动 | 原厂失败路径运行未确认，自研策略明确为非阻塞 |
| 初始化完成等待 | `mWaitEventBefore` 静态确认 | 未实现 | 有证据缺陷 |
| 主数据预载 | `InitMainWindow` 静态确认 | 构造函数直接装配编辑器 | 有证据缺陷 |
| SDK 自动加载 | `GetAutoLoadSolution` / `SetAutoLoadSolution` 静态确认 | 无 SDK 状态 | 有证据缺陷 |
| 命令行方案 | `.Solution` + `LoadSolution(...isFromAppStart:true)` 静态确认 | 任意路径直接 `doc.Load` | 不等价 |
| 引导页门 | `GuideHelper.IsGuideWindowShow` + `WecPage` 静态确认 | 无 | 有证据缺陷 |
| Splash | `WelcomeDialog` + `WelcomePage.GIF` 静态确认 | 无 | 有证据缺陷 |
| 方案容器 | `ProjectManager.LoadSolution` 入口静态确认，内部闭环未取全 | `SolutionStore` JSON | 运行未确认 |
| 流程/场景执行载荷 | 原厂调用关系未闭合 | 独立 `GraphExecutor` | 独立实现，不能宣称等价 |
| 主窗状态恢复 | `ConfigZoomIndex` + `RecoverMainWindowState` 静态确认 | 未实现原厂载荷 | 有证据缺陷 |

## 4. 集中实施设计

下一轮只在证据卡和差异矩阵审查完成后实施，顺序固定为以下五个可复核阶段。

1. **入口适配层**。补齐工作目录、命令行原始参数、单实例和启动阶段状态对象。认证/加密狗作为可插拔服务保留接口，未接入时使用开发模式继续启动，不把它作为当前交付门禁。
2. **初始化屏障**。实现可测试的启动阶段状态机，对应原厂 `BeforeStartPart`、等待事件和 `AppAfterStart`。每个阶段记录开始、完成、异常和是否允许进入主窗。
3. **方案选择解析器**。按原厂已确认优先关系处理命令行 `.Solution`、SDK 自动加载、引导页门和主窗显示。当前 JSON 方案作为独立兼容入口，不能伪装成原厂 `.sol`。
4. **方案/场景载荷适配器**。先定义 `LoadSolution` 的输入、密码、远程标志、启动来源、流程/场景列表、结果缓存和失败回滚合同。缺少原厂字段时保留未知状态，不用截图内容或常规产品设计填充。
5. **主窗呈现与状态恢复**。只有方案载荷成功或明确进入空载态后才创建/显示编辑器，绑定启动遮罩和状态恢复，确保从方案中心、命令行和空载入口都能回到同一个方案实体。

统一实施后再执行一次 Release 构建、无界面全量回归、方案往返、启动入口矩阵和与原厂证据的人工审查。启动 Splash、方案中心和场景卡片只有在取得对应原厂资源/调用证据后才落地为产品行为。

## 5. 当前明确不能宣称的内容

- 当前 `VisionEditor.exe` 不是原厂启动链的等价实现。
- 当前 `RecentSolutionStore` 不是原厂 `ProjectManager.UpdateRecentlySolutions` 的已证实替代。
- 当前 `.vmx.json` 不是原厂 `.sol` 的已证实兼容格式。
- 第二张截图中的每个卡片和点击后的具体场景载荷，运行未确认。
- 只通过当前测试不能证明原厂启动、方案恢复、场景执行或 UI 视觉 parity。
