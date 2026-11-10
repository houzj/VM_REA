# 启动链路与整屏空载初始态 · 可派工逆向任务清单

builtFrom `7d65a1e1/dfd5fd922c28` · 台账 `tests/ArchitectureRecovery/startup-initial-ui.map.json` · 证据卡 `analysis/full-audit/batch1-20261107/C03/c03-startup-initial-ui-20261110.json`

## §0 状态面与边界

本清单的状态面是 **空载初始态（程序刚启动、未加载任何方案、流程画布空白、图像显示区棋盘格空、历史结果表为空）**，与第 18 页 `16-ui-control-landing.html` 的「运行态」17 控件清单**并列、不混为一套口径**，不产生第二套完成度。

截图范围：顶部菜单与工具栏、左侧竖排工具箱、流程1 Tab 与空白流程画布、右侧流程图像/模块结果双 Tab 与棋盘格空显示区、右上缩放适配图标与左右翻页箭头、下方历史结果/帮助 Tab 与执行序号/时间/模块数据空表、右下角带橙色视口框的导航缩略图浮动面板、底部状态栏。

source-confirmed 仅指原厂反编译源码、XAML/CIL 台账与窗口布局边；runtime-confirmed 只引用既有运行报告，不新增运行证据；independent-implementation 指自研 managed 壳；unknown 一律不臆造。截图只作控件范围识别与差异信号，不作实现规格。本轮不做原厂同状态差分与自研操作级复跑，未改产品代码。

| 项 | 值 |
|---|---|
| 启动步骤 | 11 |
| 初始态控件 | 10 |
| 分组 | 4（启动链路 / 壳与全局控件 / 左侧工具箱与流程画布 / 右侧显示与结果区） |
| 六段链节点 | 126，全部 `unknown` |
| 三级验收 | 3 级，`0` 项完成（零预填） |
| E0–E7 门覆盖 | 8 / 8，当前 `closedEntryChains=0` |
| A 线工作包覆盖 | 8 / 8 |
| W/K/C/U 布局边 | 关联 22 / 30，未关联 8 条如实登记 |
| 去重证据路径 | 51 |
| 产品改动 / 命令 | 0 / 0（本轮不改产品代码、不跑方案测试） |
| 勾选框 | 全部 `[ ]`，零预填 |

硬规则：不自动提交/推送、不自动跑方案测试、不改 `src/`、缺失标 `unknown`、截图只作差异信号不作实现规格。

## §1 启动链路时间线（原厂方法名与行号）

来源 `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs` 与 `App.cs`、`MainWindowData.cs`、`VisionMasterMain/MainWindow.xaml.cs`、`vServerApp.Audit/ServerBackgroundApp/Program.cs`。

| 步骤 | 阶段 | 关键原厂成员 | 关联 A 线包 | E 门 | 取证第一动作 |
|---|---|---|---|---|---|
| ST-01 · 进程入口与工作目录 | process-entry | VisionMaster.Program.Main(string[] args) @61；Directory.SetCurrentDirectory @63；ConfigurationHelper.GetConfig("IsMultiApp") @64 | A02、A08 | E0、E3 | 核对 Main 入口、工作目录设置与多实例配置读取的实际分支条件 |
| ST-02 · 单实例管理器初始化 | single-instance | VisionMaster.Program.GetSingleInstanceManagerSetup() @87；SingleInstanceManager.Initialize(...) @70；((App)Application.Current).ProcessCommandLineArgs(args) @93；ApplicationDispatcherInvoker @… | A02、A08 | E0、E3、E4 | 核对单实例 setup 字段、命名管道/窗口消息转发与二次启动参数路由 |
| ST-03 · 应用启动与预加载并行 | app-startup | VisionMaster.Program.StartApp(string[]) @76；RefreshRegistry() @80；new App(Arguments) @83；app.Run() @84；VisionMaster.App.OnStartup(StartupEventArgs) @92；WelcomeDialog @108；Program.BeforeStar… | A01、A02、A05 | E2、E3、E5 | 核对 OnStartup 中子线程预加载与两个 WaitEvent 的同步顺序 |
| ST-04 · 启动前准备（加密/CPU/区域/权限/方案） | before-start | VisionMaster.Program.BeforeStartPart() @111；SoftEncrypt() @114；InitColor() @115；EncryptAPI.TrialVersionInit @116；CPUConfig() @118；CultureInfo.DefaultThreadCurrentCulture @120；AccessManagerH… | A02、A05、A07 | E1、E3、E5 | 逐项核对启动前准备的顺序依赖与失败分支（授权、加密、方案接管） |
| ST-05 · 主窗口数据构造与预加载 | main-window-data | VisionMaster.Program.BeforeStartMian() @136（原厂拼写）；VisionMaster.Program.InitMainWindow() @159；MainData = new MainWindowData() @161；MainData.PreLoad() @162；ServerManager.Instance.InitServer(T… | A01、A02、A05 | E1、E3、E5 | 核对 MainWindowData 构造、PreLoad 与工具源服务注册的依赖顺序 |
| ST-06 · 启动后加载与引导分支 | after-start | VisionMaster.Program.AppAfterStart() @179；ProjectManager.GetInstance().LoadSolution(...) @196；Auto_load_case @200；GuideHelper.IsGuideWindowShow() @206；SystemConfigManager.IsAutoLoad() @206；… | A01、A05、A07 | E1、E4、E5 | 核对空载初始态下自动加载/引导页/工具优先级三条分支的取值 |
| ST-07 · 主窗口显示与状态恢复 | show-window | VisionMaster.App.ShowMainWindow() @354；new MainWindow() @358；DataContext = Program.MainData @359；SetBinding(IsShowProgressMaskProperty / MaskMessageProperty) @360-361；MainWindow_Closing @36… | A02、A03、A08 | E0、E2、E4、E7 | 核对主窗口构造、DataContext 绑定、进度遮罩与窗口状态恢复的六段链 |
| ST-08 · 缩放/语言/配色与标题绑定 | post-show | VisionMaster.Program.ConfigZoomIndex() @238；MainData.ConfigZoomIndex = double.Parse(ConfigurationHelper.GetConfig("ZoomIndex")) @240；VisionMaster.Program.InitLang() @272；VisionMaster.Progra… | A02、A05 | E1、E2、E6 | 区分「有定义但无调用点」的方法与真实启动链路节点，避免把定义当执行 |
| ST-09 · 双入口与伴随进程 | entry-variants | VisionMaster.App.Main() @560-564（第二个 [STAThread] 与 app.Run() @563）；VisionMaster.App.ProcessCommandLineArgs @529；ServerBackgroundApp.Program.Main() @12；Application.Run(new VisionMasterServer… | A07、A08 | E0、E3、E7 | 核对 Program.Main 与 App.Main 两个入口的编译期选择与 vServerApp 唤醒条件 |
| ST-10 · 首屏就绪（空载初始态） | first-frame-ready | VisionMaster.App.ShowFrontendUI() @231；VisionEditor.Program.Main() @4；new System.Windows.Application { ShutdownMode = OnMainWindowClose }；app.Run(new MainWindow()) | A02、A06、A08 | E5、E6、E7 | 以截图空载态为范围信号，逐控件核对首屏可见性默认值与空数据绑定 |
| ST-11 · 定义存在但未进入可见链路的方法 | unlinked-definitions | VisionMaster.Program.AppBeforStart() @142（原厂拼写 Befor）；VisionMaster.Program.ReInitData() @168；VisionMaster.Program.InitLang() @272；VisionMaster.Program.WecPage() @243 / WecPage(out GuideWind… | A02、A05、A08 | E3、E6、E7 | 逐项搜索全仓调用点，区分定义/反射调用/事件订阅，未找到即保留 unknown |

> 原厂 `BeforeStartMian` / `AppBeforStart` 拼写按原文保留，不擅自纠正。`Program.cs:84` 与 `App.cs:563` 都有 `app.Run()`，属双入口，需分别标注。

## §2 整屏空载初始态控件分区

| 控件 | 分组 | 截图状态 | 布局边 | A 线包 | E 门 |
|---|---|---|---|---|---|
| UI-S01 · 顶部菜单栏与全局工具栏 | shell | 顶部搜索/用户图标、流程1 标题、下拉箭头与一排灰化工具图标可见，未选中任何工具 | W01、W02、W03、K01、K06、C01、C03 | A05、A08 | E0、E1、E2、E4 |
| UI-S02 · 标题栏与系统窗口状态 | shell | 无边框深色标题栏，右上最小化/最大化/关闭按钮与窗口图标 | W01、W04、W05、W06、W07、W08、W09、C01、C02、U01 | A02、A08 | E0、E2、E6、E7 |
| UI-S03 · 左侧竖排工具箱分类图标 | left | 约 12 个竖排分类图标可见，无展开的工具项面板，未加载任何工具 | K06、C03 | A01、A07 | E0、E1、E3、E4 |
| UI-S04 · 流程1 Tab 与空白流程画布 | left | 单个「流程1」Tab，深灰网格空白画布，无节点、无连线 | K05、K06、C03 | A03、A04、A08 | E2、E3、E5、E6 |
| UI-S05 · 右下角导航缩略图浮动面板与橙色视口框 | left | 右下角灰色浮动面板，左上角橙色空心视口框，右下角调整柄；对应空白画布的当前视口 | K06、C03、U02 | A03、A04、A08 | E2、E4、E5、E7 |
| UI-S06 · 右侧流程图像 / 模块结果双 Tab 与棋盘格空显示区 | right | 两 Tab 可见，棋盘格透明背景空显示区，右上 4 个缩放/适配图标与左右翻页箭头可见但无图像 | C03 | A06、A04 | E1、E2、E4、E5 |
| UI-S07 · 下方历史结果 / 帮助 Tab 与三列空数据表 | right | 历史结果/帮助 两 Tab，执行序号 / 时间 / 模块数据 三列表头，表体为空 | C05、U02 | A08、A06 | E1、E3、E5、E6 |
| UI-S08 · 底部状态栏（等待添加模块 / 耗时 / 缩放 / 版本） | shell | 左下「等待添加模块」、流程 0.00ms、100% 缩放、v4.4.0 Build20250506、右下红色圆形状态图标 | K01、C03、U01 | A02、A08、A06 | E1、E3、E5、E7 |
| UI-S09 · 左右分栏与工作区布局 | shell | 左侧流程画布与右侧显示/结果区约各占一半，垂直分隔条在中间 | W08、W09、K05、C02、U01 | A02、A06 | E2、E6、E7 |
| UI-S10 · 工作区锁定与缺插件/空方案提示 | shell | 空载初始态未显示锁定提示，工具箱可见但未加载工具 | K01、K02、K03、K04、K05、K06、K07、U02 | A01、A05、A08 | E0、E3、E5、E7 |

## §3 逐项六段链状态（全部 unknown）

六段链 `CL1 入口 / CL2 绑定·命令 / CL3 状态读写 / CL4 事件·回调 / CL5 持久化或执行效果 / CL6 异常·清理`。

| 对象 | CL1 | CL2 | CL3 | CL4 | CL5 | CL6 |
|---|---|---|---|---|---|---|
| ST-01 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-02 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-03 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-04 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-05 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-06 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-07 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-08 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-09 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-10 | unknown | unknown | unknown | unknown | unknown | unknown |
| ST-11 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S01 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S02 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S03 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S04 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S05 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S06 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S07 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S08 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S09 | unknown | unknown | unknown | unknown | unknown | unknown |
| UI-S10 | unknown | unknown | unknown | unknown | unknown | unknown |

## §4 三级验收

| 级别 | 名称 | 判定规则 | 状态 |
|---|---|---|---|
| L1 | 源码级闭环 | 六段链 CL1–CL6、资源/CIL、绑定、状态读写、事件/回调逐项有可复核证据 | `not_started` |
| L2 | 原厂同状态差分 | 与原厂空载初始态同状态差分通过；本轮未选，保持未执行 | `not_started` |
| L3 | 自研操作级复跑 | 当前构建产物操作级复跑；本轮未选，保持未执行 | `not_started` |

> 本轮用户选定的深度为「源码级证据闭环 + 可派工台账，不改 `src/`」，因此 L2 原厂同状态差分与 L3 自研操作级复跑**均保持未执行**，不得写成已验证。

## §5 E0–E7 门与 W/K/C/U 布局边映射

- 布局边来源 `docs/architecture-recovery/window-layout.edges.json` hierarchy：W01–W16（标题栏/系统窗口/模态关闭）、K01–K07（工作区锁定与工具栏）、C01–C05（自研独立实现）、U01–U02（未闭合证据），共 30 条。
- 本状态面已关联：C01、C02、C03、C05、K01、K02、K03、K04、K05、K06、K07、U01、U02、W01、W02、W03、W04、W05、W06、W07、W08、W09。
- **未关联 8 条**：C04、W10、W11、W12、W13、W14、W15、W16。原因是 W10–W16 属「共用窗口的模态与关闭」、C04 属锁偏好 JSON，与空载首屏可见状态无直接对应；不强行挂接、不凑数。
- 该 hierarchy **没有**画布 / 显示区 / 结果区 / 状态栏 / 导航缩略图子系统；这些控件改用 `canvas-viewport.edges.json`、`statusbar-entries.edges.json`、`global-toolbar.edges.json`、`toolbox-drag.edges.json`、`history-results-entry-audit.json` 与 `documents/MF02-MINIMAP-*.html` 取证。
- E0–E7 八门状态仍为 `E0 open / E1 partial / E2–E7 open`，`closedEntryChains=0`；本清单不改动这些门状态。

## §6 unknown 清单与解除条件

- **ST-01 · 进程入口与工作目录**：IsMultiApp 配置默认值与部署差异未取证
- **ST-02 · 单实例管理器初始化**：单实例跨进程通信实体与失败回退未取证；docs/reverse/ 的 A 线证据中单实例 0 命中
- **ST-03 · 应用启动与预加载并行**：WelcomeDialog 的显示条件与本地化资源未取证；预加载页与首屏切换时序仅静态可见
- **ST-04 · 启动前准备（加密/CPU/区域/权限/方案）**：授权/加密的 native 依赖与失败降级未取证
- **ST-05 · 主窗口数据构造与预加载**：ServerManager.InitServer 的进程外服务启动条件未取证
- **ST-06 · 启动后加载与引导分支**：空载初始态具体走哪条分支未运行确证；引导页 WecPage 的显示条件与资源未取证
- **ST-07 · 主窗口显示与状态恢复**：RecoverMainWindowState 的持久化键与多屏/DPI 恢复矩阵未闭（U01）
- **ST-08 · 缩放/语言/配色与标题绑定**：InitLang @272 / AwakeServerAsProcess @528 / AppBeforStart @142 在可见 Main→首屏链路中无调用点；InitMq / StartServerApp 同样未进入该链路；ZoomIndex 默认值与本地化资源未取证
- **ST-09 · 双入口与伴随进程**：实际部署使用哪个入口未取证；AwakeServerAsProcess @528 的调用点未取证
- **ST-10 · 首屏就绪（空载初始态）**：原厂首屏就绪的运行时标志未捕获；本轮未做原厂/自研同状态差分与自研复跑
- **ST-11 · 定义存在但未进入可见链路的方法**：上述方法在 Main→首屏可见链路中无直接调用点；反射或后期绑定的间接调用未取证
- **UI-S01 · 顶部菜单栏与全局工具栏**：灰化规则的完整命令 CanExecute 矩阵未闭合（closedEntryChains=0）；E4 声明到 handler 的双向对账未闭
- **UI-S02 · 标题栏与系统窗口状态**：原件 CIL、多屏、DPI、桌面矩阵仍未闭（U01）；原厂/自研标题栏行为差异未做同状态差分
- **UI-S03 · 左侧竖排工具箱分类图标**：图标资源与分类优先级的原厂映射未完全取证；E1 资源声明全集仅 partial
- **UI-S04 · 流程1 Tab 与空白流程画布**：原厂空画布的网格间距/坐标系常量未取证；A03 仍 blocked
- **UI-S05 · 右下角导航缩略图浮动面板与橙色视口框**：原厂 minimap 的橙色视口框颜色/边框/命中测试常量未取证；WorkflowOverview.cs / WorkflowOverviewDrag.cs / WorkflowCanvas.cs 内无同名类型，均为 MainWindow partial，台账不得把文件名当类型名；U02 锁状态传播未闭
- **UI-S06 · 右侧流程图像 / 模块结果双 Tab 与棋盘格空显示区**：原厂棋盘格笔刷与缩放图标资源未取证；A06 仍 evidence
- **UI-S07 · 下方历史结果 / 帮助 Tab 与三列空数据表**：A 线证据中「历史结果」0 命中，需引用 C 线与 architecture-recovery 台账；原厂表头本地化与列宽未取证
- **UI-S08 · 底部状态栏（等待添加模块 / 耗时 / 缩放 / 版本）**：原厂版本号注入点与红色状态图标语义未取证；状态栏多屏/DPI 矩阵未闭（U01）
- **UI-S09 · 左右分栏与工作区布局**：原厂分栏初始比例常量未取证
- **UI-S10 · 工作区锁定与缺插件/空方案提示**：锁状态传播到所有命令/手势/参数/持久化的全量消费者未闭（U02）；缺插件提示在空载态的触发条件未运行确证

解除条件需逐项写明证据类型与可复核路径；`source-confirmed` 不能替代 `runtime-confirmed`，静态分析不能冒充运行观察。

## §7 可派工任务卡（21 项，零预填）

- [ ] **ST-01 · 进程入口与工作目录**〔startup / not_started〕
  - 第一动作：核对 Main 入口、工作目录设置与多实例配置读取的实际分支条件
  - 准确成员：VisionMaster.Program.Main(string[] args) @61；Directory.SetCurrentDirectory @63；ConfigurationHelper.GetConfig("IsMultiApp") @64
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：1 项
- [ ] **ST-02 · 单实例管理器初始化**〔startup / not_started〕
  - 第一动作：核对单实例 setup 字段、命名管道/窗口消息转发与二次启动参数路由
  - 准确成员：VisionMaster.Program.GetSingleInstanceManagerSetup() @87；SingleInstanceManager.Initialize(...) @70；((App)Application.Current).ProcessCommandLineArgs(args) @93；ApplicationDispatcherInvoker @95
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs`、`docs/architecture-recovery/startup-order-cil-evidence.json`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **ST-03 · 应用启动与预加载并行**〔startup / not_started〕
  - 第一动作：核对 OnStartup 中子线程预加载与两个 WaitEvent 的同步顺序
  - 准确成员：VisionMaster.Program.StartApp(string[]) @76；RefreshRegistry() @80；new App(Arguments) @83；app.Run() @84；VisionMaster.App.OnStartup(StartupEventArgs) @92；WelcomeDialog @108；Program.BeforeStartPart() @126（子线程）；new PreLoadPage() @149；mWaitEven…
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs`、`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/App.cs`、`docs/architecture-recovery/menu-startup-cil-evidence.json`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **ST-04 · 启动前准备（加密/CPU/区域/权限/方案）**〔startup / not_started〕
  - 第一动作：逐项核对启动前准备的顺序依赖与失败分支（授权、加密、方案接管）
  - 准确成员：VisionMaster.Program.BeforeStartPart() @111；SoftEncrypt() @114；InitColor() @115；EncryptAPI.TrialVersionInit @116；CPUConfig() @118；CultureInfo.DefaultThreadCurrentCulture @120；AccessManagerHelper.LoadAccessInfo @121；VmSolution.GetSolutionTa…
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：1 项
- [ ] **ST-05 · 主窗口数据构造与预加载**〔startup / not_started〕
  - 第一动作：核对 MainWindowData 构造、PreLoad 与工具源服务注册的依赖顺序
  - 准确成员：VisionMaster.Program.BeforeStartMian() @136（原厂拼写）；VisionMaster.Program.InitMainWindow() @159；MainData = new MainWindowData() @161；MainData.PreLoad() @162；ServerManager.Instance.InitServer(TitleMenueItemData.GetToolFunctionSources) @164；Vis…
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs`、`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：1 项
- [ ] **ST-06 · 启动后加载与引导分支**〔startup / not_started〕
  - 第一动作：核对空载初始态下自动加载/引导页/工具优先级三条分支的取值
  - 准确成员：VisionMaster.Program.AppAfterStart() @179；ProjectManager.GetInstance().LoadSolution(...) @196；Auto_load_case @200；GuideHelper.IsGuideWindowShow() @206；SystemConfigManager.IsAutoLoad() @206；WecPage(out GuideWindow win) @210；ToolBoxData.Chan…
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs`、`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/App.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **ST-07 · 主窗口显示与状态恢复**〔startup / not_started〕
  - 第一动作：核对主窗口构造、DataContext 绑定、进度遮罩与窗口状态恢复的六段链
  - 准确成员：VisionMaster.App.ShowMainWindow() @354；new MainWindow() @358；DataContext = Program.MainData @359；SetBinding(IsShowProgressMaskProperty / MaskMessageProperty) @360-361；MainWindow_Closing @362；Show() @363；Program.ConfigZoomIndex() @364；Progr…
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/App.cs`、`analysis/decompiled/VisionMasterMain/VisionMasterMain/MainWindow.xaml.cs`、`docs/architecture-recovery/window-layout.edges.json`、`docs/architecture-recovery/base-window-cil-evidence.json`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：1 项
- [ ] **ST-08 · 缩放/语言/配色与标题绑定**〔startup / not_started〕
  - 第一动作：区分「有定义但无调用点」的方法与真实启动链路节点，避免把定义当执行
  - 准确成员：VisionMaster.Program.ConfigZoomIndex() @238；MainData.ConfigZoomIndex = double.Parse(ConfigurationHelper.GetConfig("ZoomIndex")) @240；VisionMaster.Program.InitLang() @272；VisionMaster.Program.InitColor() @284；VisionMaster.Program.UpdateTitl…
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs`、`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/App.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：3 项
- [ ] **ST-09 · 双入口与伴随进程**〔startup / not_started〕
  - 第一动作：核对 Program.Main 与 App.Main 两个入口的编译期选择与 vServerApp 唤醒条件
  - 准确成员：VisionMaster.App.Main() @560-564（第二个 [STAThread] 与 app.Run() @563）；VisionMaster.App.ProcessCommandLineArgs @529；ServerBackgroundApp.Program.Main() @12；Application.Run(new VisionMasterServerForm()) @22；RunningInstance() @31
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/App.cs`、`analysis/decompiled/vServerApp.Audit/ServerBackgroundApp/Program.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **ST-10 · 首屏就绪（空载初始态）**〔startup / not_started〕
  - 第一动作：以截图空载态为范围信号，逐控件核对首屏可见性默认值与空数据绑定
  - 准确成员：VisionMaster.App.ShowFrontendUI() @231；VisionEditor.Program.Main() @4；new System.Windows.Application { ShutdownMode = OnMainWindowClose }；app.Run(new MainWindow())
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/App.cs`、`src/VisionEditor/MainWindow.xaml`、`src/VisionEditor/Program.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **ST-11 · 定义存在但未进入可见链路的方法**〔startup / not_started〕
  - 第一动作：逐项搜索全仓调用点，区分定义/反射调用/事件订阅，未找到即保留 unknown
  - 准确成员：VisionMaster.Program.AppBeforStart() @142（原厂拼写 Befor）；VisionMaster.Program.ReInitData() @168；VisionMaster.Program.InitLang() @272；VisionMaster.Program.WecPage() @243 / WecPage(out GuideWindow) @257；VisionMaster.Program.IsCancelClose() @295…
  - 证据路径：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **UI-S01 · 顶部菜单栏与全局工具栏**〔shell / not_started〕
  - 第一动作：核对 53 个菜单命名声明 / 15 个工具栏声明在空载态的可用性与灰化规则
  - 准确成员：VisionEditor.MainWindow.ToolBar；ToolBarScroll；MenuHost；ShellHeader；ToolBarPreviewMouseWheel()（MainWindow.ShellToolbarScroll.cs）；VisionMaster.MainWindowData.MenuSource @103 / MenuToolSource @105
  - 证据路径：`docs/architecture-recovery/global-toolbar.edges.json`、`docs/architecture-recovery/global-toolbar-entry-inventory.json`、`docs/architecture-recovery/entry-inventory.json`、`docs/reverse/A05-full-menu-map-batch17.md`、`docs/reverse/A05-input-registration-batch31.md`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **UI-S02 · 标题栏与系统窗口状态**〔shell / not_started〕
  - 第一动作：核对 W05–W09 的窗口消息/DPI/多屏换算与自研 WorkAreaHook 的对应边界
  - 准确成员：VisionMasterMain.MainWindow : BaseWindow, IComponentConnector；BaseWindow.OnApplyTemplate → Header 事件 / DragMove；VisionEditor.MainWindow.OnSourceInitialized() / WorkAreaHook()；ShellWindowStateChanged() / UpdateMaximizeButton()
  - 证据路径：`docs/architecture-recovery/window-layout.edges.json`、`docs/architecture-recovery/base-window-cil-evidence.json`、`src/VisionEditor/WindowWorkArea.cs`、`src/VisionEditor/MainWindow.ShellWindowState.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **UI-S03 · 左侧竖排工具箱分类图标**〔left / not_started〕
  - 第一动作：核对分类图标的装载来源、缺插件体验与空载态下的启用状态
  - 准确成员：VisionEditor.MainWindow.ToolRail / Rail / ToggleToolboxButton / ToolboxFoldImage；ToggleToolbox() / ToggleToolboxClick()（MainWindow.xaml.cs:54-55）；ToolBoxData.ChangePriorityByInfo()（Program.cs:218）
  - 证据路径：`docs/architecture-recovery/toolbox-drag.edges.json`、`docs/architecture-recovery/toolbox-drag-cil-evidence.json`、`docs/reverse/A01-loader-batch1.md`、`docs/reverse/A01-LOADER-EXECUTION-TARGET.md`、`docs/reverse/A-loader-failure-matrix-batch19.md`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **UI-S04 · 流程1 Tab 与空白流程画布**〔left / not_started〕
  - 第一动作：核对空方案的节点集合、网格绘制与 Tab 绑定默认值
  - 准确成员：VisionEditor.MainWindow.ProcedureTabs / ProcedureTabScroll / EmptyWorkflowPanel / CanvasHost / CanvasViewport / GraphSurface；WorkflowSelectionChanged() / SelectWorkflow()（WorkflowTabs.cs）；RenderGraph() / ApplyCanvasTransform() / ViewRect()…
  - 证据路径：`docs/architecture-recovery/canvas-viewport.edges.json`、`docs/reverse/A03-tabs-batch2.md`、`docs/reverse/A03-binding-batch4.md`、`docs/reverse/A-canvas-implementation-batch10.md`、`src/VisionEditor/MainWindow.xaml`、`src/VisionEditor/WorkflowTabs.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **UI-S05 · 右下角导航缩略图浮动面板与橙色视口框**〔left / not_started〕
  - 第一动作：核对橙色视口框的几何来源、拖动/滚轮/路由事件与画布视口的双向同步
  - 准确成员：VisionEditor.MainWindow.MiniMapPanel / MiniMapCanvas / MiniMapPreview / MiniMapViewport / MiniMapToggle（XAML 命名控件）；UpdateMiniMap() / NavigateMiniMap() / MiniMapDown() / BeginMiniMapDrag() / MoveMiniMapDrag() / MiniMapMove() / MiniMapUp() /…
  - 证据路径：`docs/architecture-recovery/canvas-viewport.edges.json`、`docs/architecture-recovery/documents/MF02-OVERVIEW-VIEWPORT-HANDOFF.html`、`docs/architecture-recovery/documents/MF02-MINIMAP-EDGE-DRAG.html`、`docs/architecture-recovery/documents/MF02-MINIMAP-ROUTED-EVENTS.html`、`src/VisionEditor/MainWindow.xaml`、`src/VisionEditor/WorkflowCanvas.cs`、`src/VisionEditor/WorkflowOverviewDrag.cs`、`src/VisionEditor/GraphCanvas.cs`、`src/VisionEditor.Core/GraphEditorDocument.Viewport.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：3 项
- [ ] **UI-S06 · 右侧流程图像 / 模块结果双 Tab 与棋盘格空显示区**〔right / not_started〕
  - 第一动作：核对无图像时棋盘格绘制、缩放图标可用性与模块结果空绑定
  - 准确成员：VisionEditor.MainWindow.DisplayTabs / ImagePreviewPane / Preview / EmptyImage / ModuleResultsGrid / ImageSelector；InitializeToolDisplays() / RefreshToolDisplay()（ToolDisplays.cs）；RuntimeDescriptor() / ModuleResultRows()（DynamicOutputPresen…
  - 证据路径：`src/VisionEditor/MainWindow.xaml`、`src/VisionEditor/ToolDisplays.cs`、`src/VisionEditor/DynamicOutputPresentation.cs`、`docs/reverse/A06-gate-batch27.md`、`docs/reverse/A06-frontend-gate-batch24.md`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **UI-S07 · 下方历史结果 / 帮助 Tab 与三列空数据表**〔right / not_started〕
  - 第一动作：核对空数据表列定义、绑定与运行序号/计时列的初始占位值
  - 准确成员：VisionEditor.MainWindow.HistoryGrid / BottomTabs；InitializeFrameworkResultHistory() / RecordFrameworkRunHistory() / RefreshFrameworkResultTiming()；FrameworkRunHistory.Append() / Clear() / Entries / MaximumRecords；FrameworkResultTiming.From…
  - 证据路径：`docs/architecture-recovery/history-results-entry-audit.json`、`docs/architecture-recovery/history-results-entry-audit.cil.json`、`docs/architecture-recovery/history-replay.edges.json`、`docs/reverse/C-result-history-implementation-batch15.md`、`src/VisionEditor/MainWindow.xaml`、`src/VisionEditor/MainWindow.FrameworkResultHistory.cs`、`src/VisionEditor.Core/FrameworkRunHistory.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **UI-S08 · 底部状态栏（等待添加模块 / 耗时 / 缩放 / 版本）**〔shell / not_started〕
  - 第一动作：核对空载态状态文本、三段耗时占位与 100% 缩放的默认值/持久化键
  - 准确成员：VisionEditor.MainWindow.WindowStatusBar / StatusIcon / StatusText / ShowTimeButton / ProcessTimeText / ModuleTimeText / AlgorithmTimeText / ZoomSlider / ZoomIcon / ResetCanvasZoomButton / ZoomValue；RefreshShellConfigurationStatus() / Reset…
  - 证据路径：`docs/architecture-recovery/statusbar-entries.edges.json`、`docs/architecture-recovery/statusbar-entries-cil-evidence.json`、`docs/architecture-recovery/statusbar-entries-review.json`、`src/VisionEditor/MainWindow.xaml`、`src/VisionEditor/MainWindow.ShellStatus.cs`、`src/VisionEditor/MainWindow.FrameworkResultHistory.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项
- [ ] **UI-S09 · 左右分栏与工作区布局**〔shell / not_started〕
  - 第一动作：核对分栏初始比例、拖动持久化与最小化宽度约束
  - 准确成员：VisionEditor.MainWindow.WorkareaGrid / LeftWorkareaColumn / RightWorkareaColumn / WorkareaSplitter；WorkareaSplitterDragStarted()（MainWindow.ShellSplitter.cs）
  - 证据路径：`docs/architecture-recovery/window-layout.edges.json`、`src/VisionEditor/MainWindow.xaml`、`src/VisionEditor/MainWindow.ShellSplitter.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：1 项
- [ ] **UI-S10 · 工作区锁定与缺插件/空方案提示**〔shell / not_started〕
  - 第一动作：核对模块数阈值触发的自动锁/延迟提示/解锁（K07）在空方案下是否触发
  - 准确成员：VisionMaster.MainWindowData.IsWorkAreaLocking @127 / IsNotifyLock @162 / LockProcess() @925 / VmSolution_LoadEnd() @736；K04 setter → IUnRedoService → LockProcess → PropertyChanged
  - 证据路径：`docs/architecture-recovery/window-layout.edges.json`、`docs/reverse/A-loader-failure-matrix-batch19.md`、`docs/reverse/A07-managed-loader-partial-batch16.md`、`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs`
  - 退出判据：逐项登记证据类型，未取证处保持 unknown；不以截图作实现规格、不臆造 native 函数体或资源
  - unknown：2 项

## §8 与既有产物的关系

- 第 18 页 `16-ui-control-landing.html`：17 控件 / 4 类，针对**运行态**（有流程、有图像、有表格数据），全部 `blocked`。本清单是**空载初始态**，两者并列。
- 第 19 页 `17-incremental-landing.html`：4 批次 13 步，执行顺序仍是 B0 S0/C03 补证 → B1 最小纵切 → B2 控件横扩 → B3 工具分批；本清单不改变 `B0 open / B1–B3 blocked`。
- `docs/COMPLETE-REVERSE-TASK-LIST.md`：544 项完整清单，附录 G 索引本清单；任务总数不变。
- `docs/reverse/C03-startup-initial-ui-evidence.md`：本轮人读证据结论（观察 / 推断 / unknown 分级）。

权威完成度口径仍为 `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` 的 10/10 = 100%（仅标定授权范围，人工验收单列）；本清单不计入该分母。