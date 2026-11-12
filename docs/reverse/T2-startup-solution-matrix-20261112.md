# T2 启动 / 方案 / 工具载体对照矩阵（2026-11-12）

执行：CodeBuddy ｜ 任务：T2.1（FR-02）、T2.2/T2.3（FR-03）｜ 输入：`docs/reverse/T1-evidence-index-20261112.md`、原厂反编译源码、当前 `31acdb4c` + Codex 未提交改动。

> 本阶段为**对照矩阵与差异决定**，未修改任何产品代码。按 `noEvidenceAction`，凡缺 E1 调用链或 E3 受控运行证据的差异一律不重写，只登记。

## 1. FR-02 逐功能对照矩阵

| 区域 | 原厂入口（证据） | 当前入口 | 已确认状态 | 具体差异 / 未知 | 证据等级 | 处理决定 |
|---|---|---|---|---|---|---|
| 启动入口与单实例 | `VisionMaster.Exe.Audit/VisionMaster/Program.cs`；`VM.Utility.SingleInstance` 引用见 `MainWindowData.cs:30` | `src/VisionEditor/Program.cs`、`StartupLaunch.cs` | `source-shaped` | 原厂初始化服务、认证、同机时序未运行确认 | E1 | 保留 |
| Welcome 页 | 原厂 `WelcomeDialog`（资源 `analysis/ui-assets/original-guide/WelcomePage.GIF`） | `StartupWelcomeWindow.xaml` | `source-shaped` | 用户桌面实际比例与启动时序未同机对照 | E0+E1 | 保留 |
| Guide 显示条件 | `Program.cs:206-217`：`!flag && IsGuideWindowShow() && !IsAutoLoad() && !IsSdkMode() && !IsTakeOver && loginUser != 3` | `StartupGuideWindow.xaml*` + 启动判定 | `source-shaped` | 当前侧未逐条件复现 `flag`/`loginUser`/SDK 模式分支 | E1 | 运行未确认 |
| Guide 卡片写配置 | `GuideWindow.xaml.cs:124-131` → `SaveConfig("ToolBoxType", ...)` | `ToolboxPriorityStrategy.cs` + `ToolNavigation.cs` | `source-shaped` | 真实 `ToolGroup.xml` 四类排序值缺失 | E1+E4 | 隔离（不写死数值） |
| Guide 卡片命令 | `GuideWindow.xaml.cs:133-147`：`Open_Demo`/`OpenFile_OnClick` → `OpenSolutionCommand?.Execute(...)` → `Close()` | 启动方案选择与 Guide 命令 | `divergent` | 当前侧三个分支与 `OpenSolutionCommand` 参数的逐项对应未取证 | E1 | 运行未确认 |
| 首次工具箱重排 | `Program.cs:218` → `ToolBoxData.ChangePriorityByInfo()`（`ToolBoxData.cs:231-261`） | `StartupSettings.ToolBoxType` + `CatalogGroup.PriorityStrategyInfo` | `source-shaped` | 原厂 `LoadData()` 与当前目录装载顺序未逐成员对齐 | E1+E4 | 保留 |
| 启动期自动加载方案 | `Program.cs:196` `ProjectManager.LoadSolution(text, isGuid:false, ..., isFromAppStart:true)` | `MainWindow.StartupSolution.cs:30-35` `CommandLinePath` → `TryLoadStartup` | `independent-contract` | 原厂 `.sol` 容器与 `isGuid/isRemoteMode/isFromAppArgs` 分支未接入 | E1 | 隔离 |
| 方案选择 / 最近列表 | `ProjectManager.cs:77,900-927` `RecentSolution` / `UpdateRecentlySolutions`（>5 移除、倒序 Insert(0)、`RefreshRecentSolution`） | `MainWindow.ShellRecentSolutions.cs` `RecentSolutionStore`（Take(6)、去重、按 LastTime 倒序） | `source-shaped` | 见第 5 节：T0 首个失败未定位 | E1+E4 | 运行未确认 |
| 自动保存 | `SolutionItemsData.cs:19-95` `DispatcherTimer`、`_AutoSaveTime=3.0`、`SaveSolTimerEnable/SaveAllSolTimerEnable` | 当前保存链路（`MainWindow.ShellSaveSolution.cs`） | `independent-contract` | 原厂定时保存的触发、脏标记与失败回滚未取证 | E1 | 隔离 |
| 方案持久化 | `ProjectManager.cs:166,197,177,436,628` `VmSolution.Export(SolutionPath, ...)` | `SolutionStore.Save` → `.vmx.json`；`StartupSolutionStore.DefaultPath` | `independent-contract` | 原厂 `.sol` 字段不透明；当前显式提示「原厂方案格式暂未接入」 | E1 | 隔离（不伪装兼容） |
| 主窗口数据载体 | `MainWindowData.cs:41-85`（`solutionData`、`menuSource`、`selectedModule`、`configZoomIndex` 等） | `MainWindow.*.cs` 25 个分部类 | `divergent` | 逐成员对应矩阵见 `T1-evidence-index` §4，多数仅字段层登记 | E1 | 运行未确认 |
| 菜单 / 工具栏 | 原厂 `HostShell/ExeEntry/menuitems.xml`；`MainWindowData.cs:45` `MenuItems="MenuItems.xml"` | `Resources/shell-menuitems.xml` + `ShellMenuCatalog` + `ShellCommandRouter` | `source-shaped` | 25 个命令未接入并显式禁用；快捷键 `not-applicable` | E0+E1+E4 | 保留 |
| 关闭与释放 | 原厂 `Program.cs` 启动/关闭链、`GuideWindow` Dispatcher 处理 | `StartupShellChecks` 关闭链回归 | `unknown` | 原厂服务/设备释放顺序未运行确认 | E4 | 运行未确认 |

## 2. FR-03 启动链与方案实体重审（T2.2）

### 2.1 原厂状态机（E1）

```
Program.AppAfterStart
 ├─ 自动加载分支：SystemConfigManager.IsAutoLoad() → ProjectManager.LoadSolution(text, ..., isFromAppStart:true)
 ├─ Guide 分支：条件见 Program.cs:206 → WecPage(out win) → Dispatcher.InvokeShutdown() (条件)
 ├─ ToolBoxData.ChangePriorityByInfo()                       [Program.cs:218]
 ├─ SoftWareData.SetAutoLaunchAccess()                       [Program.cs:219]
 ├─ MainData.IsNormalMode ← RunningMode                     [Program.cs:220]
 ├─ VmSolution.IsIgnoreGlobalModule ← IsIgnoreGlobalModule  [Program.cs:221-222]
 ├─ SolutionFileHelper.GetInstance()                        [Program.cs:223]
 ├─ IsClearLastExecuteResult / IsDirectDownSampling         [Program.cs:224-225]
 └─ IsCallBackCacheEnable → ExpandCallBackQueueLength / ShutDownCallBackQueue [Program.cs:226-234]
```

### 2.2 当前状态机（E4，`MainWindow.StartupSolution.cs:21-71`）

```
LoadStartupSolution
 ├─ StartNewSolution → doc.NewSolution(true)
 ├─ CommandLinePath 存在且 TryLoadStartup → ReportStartupSolution
 ├─ RecentSolutionStore.Read().FirstOrDefault(存在且非 demo.vmx.json)
 │    └─ default.vmx.json → PrepareDefaultStartupSolution → EnsureDefaultExecutionGraph
 ├─ DefaultPath → PrepareDefaultStartupSolution(CopyStartupAssets / BlankStartupSolution) → TryLoadStartup
 └─ 兜底：doc.NewSolution(true)，状态文本提示「原厂方案格式暂未接入」或「创建内存启动方案」
```

### 2.3 差异决定

| 差异 | 证据 | 决定 |
|---|---|---|
| 原厂 8 个启动期状态写入（RunningMode、回调队列、直降采样等）在当前侧无逐项对应 | 原厂 `Program.cs:219-234` E1，当前侧无 E1 证据 | **运行未确认**，不补实现 |
| 原厂 `ProjectManager` 单例 + `VmSolution.Instance` 与当前 `doc`/`SolutionStore` 是否共享同一方案实体 | 原厂 E1；当前侧仅 `LoadStartupSolution` 与 `doc` 可见，未取证全部调用方 | **运行未确认**，T3.1 补执行侧后再判 |
| `.sol` 容器与资源目录 | 缺 `test1108.sol` / `CircleFind.sol` | **隔离 + unknown**，JSON 不伪装兼容 |
| 自动保存定时器 | 原厂 `SolutionItemsData` E1；当前侧语义未取证 | **隔离** |

**重写项：0。** 没有一项差异同时具备「原厂 E1 调用链 + 当前侧数据流证据 + 明确预期输出」，按 `noEvidenceAction` 不得改行为。

## 3. FR-03 工具载体、配置入口与布局状态（T2.3）

| 项 | 原厂证据 | 当前实现 | 状态 | 决定 |
|---|---|---|---|---|
| 动态工具载体 | `MainWindowData.cs:81` `IVmModule selectedModule`、`VMControls.WPF/ToolControl.xaml.cs`、`ToolBoxData.UpdateLoad` 线索 | `WorkflowCanvas.cs` 节点卡（110×30）、`ToolNavigation.cs` 工具箱 | `divergent` | 运行未确认 |
| 配置入口 | `MainWindowData.cs:59` `_IsOpenParams`；`Apps.XmlParser` 参数页（T1 已定位目录） | `NodeParameterWindow`、`ToolConfigurationHost` | `divergent` | 运行未确认 |
| 公共配置/ROI/显示合同 | 原厂模板、绑定、事件、参数键未逐项取证（T3.2） | `src/VisionPluginContracts/ToolConfiguration*.cs`、`ToolDisplays.cs` | `independent-contract` | 隔离 |
| 布局状态持久化 | `MainWindowData.cs:73` `mainWindowState`、`:79` `configZoomIndex` | `MainWindow.xaml` + 状态栏缩放回归 | `independent-contract` | 保留（当前回归已覆盖缩放行为） |
| 菜单/工具栏数据驱动 | `menuitems.xml`（SHA-256 与原件一致） | `ShellMenuCatalog` 50/42/15/4 自检 + `ShellCommandRouter` | `source-shaped` | 保留 |

所有权按 `docs/PARALLEL-WORK.md` 顶部 2026-10-22 规则：公共配置/交互/会话框架归 `main`，工具专属转换归 `tool-plugins`；本阶段未产生跨项目接口需求，未追加 FCR。

## 4. 已复跑的独立回归（E4，不作原厂等价）

`docs/reverse/T0-replay-command-inventory-20261112.md` 中：`VisionEditor.StartupShellChecks`（启动/Guide/宿主数据/关闭链）、`VisionEditor.ShellMenuChecks`（65 项）、`VisionEditor.WorkspaceProjectionChecks`（79 项）、`NodeProjectionChecks`（54 项）均通过；`VisionEditor.ShellChecks` 在第 275 行失败。通过只证明当前合同。

## 5. T0 首个失败的 T2 归属

- 现象：`Save menu persists and orders recent solution paths` 断言期望 2 项，现场 `Items.Count:3`，退出码 `-532462766`。
- 测试隔离已确认：`tests/VisionEditor.ShellChecks/Program.cs:25-28` 把 `RecentSolutionStore.FilePath` 重定向到临时文件并删除旧文件，**不是外部持久化污染**。
- 原厂语义（E1）：`ProjectManager.cs:900-927` 上限 `>5` 移除 + 倒序 `Insert(0)` + `SaveRecentlySolutions()` + `RefreshRecentSolution()`。
- 当前实现（E4）：`MainWindow.ShellRecentSolutions.cs:50-57` `Read().Where(去重).Prepend(new).Take(6).Persist()`；`:90-102` 菜单按 `Read()` 结果全量生成，无条目数限制。
- 写入点分散：`RecentSolutionStore.Update` 在 `MainWindow.StartupSolution.cs:122,141`、`ShellSaveSolution.cs:78`、`ShellSampleSelection.cs:75`、`ShellRecentSolutions.cs:118`、`ShellOpenSolution.cs:73,87` 共 8 处调用。
- **结论：差异位于当前侧最近列表写入时机或测试构造路径，首个多出的条目尚未定位**；原厂与当前侧的「上限 6 / 倒序」语义本身一致。按 `noEvidenceAction` 不修改产品代码、不修改测试夹具、不放宽断言。
- 解除条件：在测试流程中逐次记录 `RecentSolutionStore.Update(path)` 的调用点与 path，定位第 3 项的来源；再对照原厂 `UpdateRecentlySolutions` 的调用时机（`LoadSolution` 成功 / `SaveSolution` 成功 / `OpenRecentSolution`）决定是产品修复还是夹具修正，并复跑全套件。此项移交 T3.1/T5.4。

## 6. 差异处理决定汇总

| 决定 | 数量 | 项目 |
|---|---|---|
| 保留 | 5 | 启动/单实例、Welcome、首次工具箱重排、菜单数据驱动、布局缩放 |
| 隔离 | 5 | 卡片排序值、自动加载方案、自动保存、方案持久化、公共配置合同 |
| 运行未确认 | 6 | Guide 显示条件、Guide 卡片命令、最近列表、主窗口载体、配置入口、关闭释放 |
| **重写** | **0** | 无一项满足 E1+E3 双重证据 |

## 7. 退出条件核对

| T2 退出条件 | 结果 |
|---|---|
| 每个差异有「保留/重写/隔离/运行未确认」决定 | 已完成（第 6 节） |
| 启动→Guide→主窗口→关闭、新建/打开共享同一方案实体 | **未完全达成**：当前侧 `doc` 是否唯一实体未取证，移交 T3.1 |
| `.sol` 未知字段保持不透明 | 已确认（当前显式提示未接入） |
| 未因截图改尺寸/颜色/点击语义 | 已确认，本阶段无产品代码修改 |

FR0–FR4 是否达到 `source-shaped` 由 T4.2 CHECK-FR 判定；本矩阵不提前宣称通过。

## 8. 未确认项与解除条件

- 首个失败的第 3 项来源：需逐次记录 `Update` 调用点（见第 5 节）。
- 原厂 `.sol`、SDK 自动加载、场景直启：需原件与匹配环境。
- HostShell 动态工具载体 / 布局状态：需逐成员 E1 + 同机 E3 证据。
- 原厂初始化服务、认证、设备状态：需匹配原厂运行环境。
- Guide 新修正版与原厂桌面视觉/状态：`divergent`，人工复核未执行（需用户明确要求且在 `WinSta0\Default`）。
