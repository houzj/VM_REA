# T1 口径校正与原厂证据索引（2026-11-12）

执行：CodeBuddy ｜ 依据：`docs/CODEBUDDY-TAKEOVER-TASKS-20261112.md` T1.1–T1.4 ｜ 证据等级：E0/E1/E2（静态），无 E3 运行时新增。

> 本阶段只做只读取证与口径统一，**未修改任何产品行为**。所有路径相对 `d:/MyWork/Rea逆向`。

## 1. T1.1 口径对照表

### 1.1 原厂证据根

| 来源 | 路径 | 实测 | 结论 |
|---|---|---|---|
| 交接文档第 22–23 行 | `D:/MyWork/REA逆向测试/analysis/decompiled`、`.../full-audit` | **不存在** | 过时/错误引用，后续报告一律不采用 |
| 本工作树反编译源码 | `analysis/decompiled` | 存在，27 个顶层目录 | **唯一静态源码证据根** |
| 本工作树审计产物 | `analysis/full-audit` | 存在 | 门禁/探针/原厂 DLL 副本所在 |
| 上述两者 | 被 `.gitignore` 的 `analysis/*` 排除 | 远程不可复原 | 所有引用必须注明「本地证据、远程不可移植」 |
| 构建必需资源例外 | `analysis/ui-assets/**`、`analysis/images/circle-find-sample.bmp` | 已跟踪 | 与临时产物区分 |

### 1.2 四套进度口径（禁止混用）

| 数值 | 来源 | 定义 | 权威级别 |
|---|---|---|---|
| **10/10 = 100%** | `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` | 授权第一阶段 10 个标定工具「实施完成 + 自审通过 / 10」 | **用户指定对外权威口径**；人工验收不计入 |
| **42/96 = 43.75%** | `docs/framework-progress-audit.json` | `implementation-self-review-v1`，96 个框架单元中 `implementation=done` | 框架实施口径，单列展示 |
| 41/96（42.71%）、16/162（9.88%） | `docs/CODEBUDDY-HANDOFF-20261112.md` | 历史快照值 | 历史值，不参与报表 |
| 128 gates = 25.78% | `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` | A1-A4/B1-B4/C1-C4/D1-D4 × S0-S7 内部维度 | 内部维度，不作对外完成率 |

G1–G9 门禁完成数、测试断言数（19/54/79/65 等）**均不并入**上述任一分母。

### 1.3 状态词汇

- 结论分类：`source-confirmed` / `native-confirmed` / `runtime-confirmed` / `independent-implementation` / `unverified`。
- 保真度：`parity-closed` / `source-shaped` / `independent-contract` / `visual-only` / `divergent` / `unknown`。
- 门禁 `noEvidenceAction`：无证据不改行为，只登记缺失证据与复跑条件。

### 1.4 本阶段实际文档订正

- `docs/CODEBUDDY-TAKEOVER-PLAN-20261112.md` §6 T0 行「46 份门禁 JSON」→「44 份」（T0 实测 44；§2 表格原已为 44）。
- 未改 `AGENTS.md`、`docs/CODEBUDDY-HANDOFF-20261112.md`、`tasks/plan.md`、`tasks/todo.md`；历史叙述保持原样。

## 2. T1.2 Guide 卡片配置写入与首次工具箱加载入口

### 2.1 已定位的原厂调用边（E1，source-confirmed）

| 顺序 | 原厂位置 | 成员 / 行为 |
|---|---|---|
| 1 | `analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/GuideButton.cs:51` | `public ToolBoxType ToolBoxType { get; set; }` |
| 2 | `analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/ToolBoxType.cs:3-9` | 枚举 `General, Position, Defect, Recognition` |
| 3 | `analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/GuideWindow.xaml.cs:124-131` | `GuideButton_OnClick` → `ConfigurationHelper.SaveConfig("ToolBoxType", guideButton.ToolBoxType.ToString())` → `Close()` |
| 4 | 同上 `:133-141` | `Open_Demo` → `GuideHelper.OpenSolution(opendemo:true, out filename)` → `OpenSolutionCommand?.Execute(filename)` → `Close()` |
| 5 | 同上 `:143-147` | `OpenFile_OnClick` → `OpenSolutionCommand?.Execute(null)` → `Close()` |
| 6 | `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs:206-217` | Guide 显示条件：`!flag && GuideHelper.IsGuideWindowShow() && !SystemConfigManager.IsAutoLoad() && !IsSdkMode() && !IsTakeOver && (int)loginUser != 3`；`WecPage(out win)` 后 `Dispatcher.InvokeShutdown()` 条件分支 |
| 7 | `Program.cs:218` | **Guide 关闭后** `ToolBoxData.ChangePriorityByInfo()` |
| 8 | `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs:199-202` | `LoadData(); ChangePriorityByInfo();` |
| 9 | `ToolBoxData.cs:231-261` | `ChangePriorityByInfo()`：`GetConfig("ToolBoxType")` → 空则返回 → 按 `PriorityStrategyInfo.Split(';')` / `Split(':')` 取 `type:value` → 匹配则 `groupData.Priority = float.Parse(value)` |
| 10 | `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxItemData.cs:41,222` | 字段 `priorityStrategyInfo`、属性 `PriorityStrategyInfo` |

**新增于本次的环节**：Codex 报告 `docs/reverse/C01-toolbox-type-priority-chain-20261112.md` 只记录了写配置与解析算法；本次补齐了 **Guide 关闭 → `Program.cs:218` → 首次工具箱优先级重排** 的启动链环节，以及 Guide 的显示条件与三个卡片命令分支。未改 `src/VisionEditor/ToolboxPriorityStrategy.cs` 的解析算法。

### 2.2 当前实现对应

`src/VisionEditor/StartupGuideWindow.xaml*`、`src/VisionEditor/ToolboxPriorityStrategy.cs`、`src/VisionEditor/ToolNavigation.cs`（`CatalogGroup.PriorityStrategyInfo`、`StartupSettings.ToolBoxType`）、`src/VisionEditor/ShellMenuCatalog.cs` 为当前侧；T0 复跑 `ToolCatalogLifecycleChecks15` 19/19 含 `ToolBoxType=General/Position applies source-shaped group priorities` 两项（E4，独立实现证据，非原厂等价）。

### 2.3 未知与复跑

- 真实 `Modules/*/ToolGroup.xml` 中四类 `PriorityStrategyInfo` 数值缺失 → `unknown`，**不写死排序值**；取得原件后只补元数据并复跑，不改解析算法。
- 原厂 `GuideWindow` 卡片资源与当前 `StartupGuideWindow` 的逐控件对应尚未取证（T2.1）。
- 复跑：见 `docs/reverse/T0-replay-command-inventory-20261112.md` 命令 2。

## 3. T1.3 方案实体成员链索引

### 3.1 已定位的原厂成员（E1，source-confirmed）

| 环节 | 原厂位置 | 成员 / 行为 |
|---|---|---|
| 启动期自动加载 | `Program.cs:196` | `ProjectManager.GetInstance().LoadSolution(text, isGuid:false, null, null, isRemoteMode:false, isFromAppStart:true)` |
| 加载入口 | `.../VisionMaster.Project/ProjectManager.cs:506` | `internal bool LoadSolution(string filename=null, bool isGuid=false, string solPassWord=null, string initialDirectory=null, bool isRemoteMode=false, bool isFromAppStart=false, bool isFromAppArgs=false, bool isShowSaveNotice=true)` |
| 页面事件订阅 | `ProjectManager.cs:163` | `ItemsPage.OnLoadSolution += ProjectManager_OnLoadSolution`；`:498-502` 回调再调 `LoadSolution(path)` |
| 保存 | `ProjectManager.cs:166`、`:197` | `SaveSolution(Task afterSaveTask=null, bool bDisplayProgress=true, bool isRemoteSave=false)`；`SaveSolution(string strPath, string strPassWord=null, Task afterSaveTask=null)` |
| 持久化写出 | `ProjectManager.cs:177`、`:181`、`:436`、`:628` | `VmSolution.Export(VmSolution.Instance.SolutionPath, ...)` |
| 路径状态 | `ProjectManager.cs:209,473,742,754` | `Program.MainData.OnPropertyChanged("SolutionPath")` |
| 最近方案 | `ProjectManager.cs:54,77` | `OpenRecentSolutionInfo`、`public static SolutionItemInfo RecentSolution { get; private set; }` |
| 最近列表更新 | `ProjectManager.cs:900-927` | `UpdateRecentlySolutions(solutionpath)`：已存在则刷新 `LastTime`；否则新项 `Insert(0, ...)`，且 `_recentlySolutions.Count > 5` 时移除末尾；随后 `SaveRecentlySolutions()` 与 `Program.MainData.RefreshRecentSolution()` |
| 列表成员 | `ProjectManager.cs:861` | `recentlySolutions.Any(p => p.SolutionPath == path)`；`SolutionItemInfo` 含 `SolutionPath` / `LastTime` |
| 自动保存 | `.../VisionMaster.AppMainWindow/SolutionItemsData.cs:19-95` | `SolutionItemsData : BaseNode, INotifyPropertyChanged`；`ObservableCollection<SolItem> SolItems`、`DispatcherTimer saveTime`、`_AutoSaveTime = 3.0`、`SaveSolTimerEnable` / `SaveAllSolTimerEnable`、`AddTimer/DisposeTimer` |
| 其他入口 | `Program.cs:223` | `SolutionFileHelper.GetInstance()` |
| 待展开 | `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs`、`.../VM.PlatformSDKCS/SolutionManager.cs` | 本次仅定位路径，未展开成员 |

### 3.2 与 T0 首个失败的关系（只登记，不改行为）

`tests/VisionEditor.ShellChecks/Program.cs:275` 期望最近方案菜单 2 项、顺序 `recentSecond, recentFirst`；T0 现场为 `Items.Count:3`。原厂侧现已确认 `UpdateRecentlySolutions` 的上限（>5 移除）与倒序插入语义，但**当前实现为何出现 3 项、以及菜单是否应过滤/去重，尚无当前侧数据流证据**。按 `noEvidenceAction`，T1 不修改产品代码或测试夹具；该差异带上述 E1 证据移交 T2.1/T2.2 与 T5.4 处理。

### 3.3 未知与解除条件

- 原厂 `.sol` 容器字段、资源目录结构、版本迁移、场景直启：`unknown`；解除条件为取得 `test1108.sol`、`CircleFind.sol` 与匹配原厂环境。
- `VmSolution.Export` 的原生写出格式、Procedure/Node 与资源引用的成员链：本次未展开，T2.2 补齐。
- 当前 `MainWindow.StartupSolution.cs` / `SolutionStore` 与上述原厂成员的逐字段映射尚未完成，T2.2 补齐。

## 4. T1.4 MainWindow 与 MainWindowData / HostShell 对应矩阵

### 4.1 原厂字段（E1，source-confirmed）

`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs:41-85`：

| 原厂字段 | 类型 | 当前侧候选（未验证对应） |
|---|---|---|
| `appData` | `AppData` | 启动上下文 `StartupLaunch.cs` |
| `MenuItems = "MenuItems.xml"` | `string` | `Resources/shell-menuitems.xml` + `ShellMenuCatalog.cs` |
| `menuSource` | `TitleMenueItemData` | `ShellMenuCatalog` 节点树 |
| `menuToolSource` | `object` | `ShellCommandRouter` / 工具栏生成 |
| `isWorkAreaLocking` | `bool` | 主窗口锁定命令 |
| `solutionData` | `SolutionItemsData` | `MainWindow.StartupSolution.cs` |
| `softwareData` | `SoftWareData` | 启动配置 |
| `openSolPathCommand` / `openRunSetCommand` | `DelegateCommand` | 菜单命令路由 |
| `mainWindowState` | `WindowState` | 标题栏状态 |
| `configZoomIndex` | `double` | 状态栏缩放 |
| `selectedModule` | `IVmModule` | 画布选中节点 / 配置宿主 |
| `procRunPolicyData` | `ProcedureRunData` | 执行策略 |
| `_IsOpenParams` / `isShowProgressMask` / `maskMessage` | 配置窗与遮罩状态 | 配置入口（T2.3） |

`Program.cs:220-226` 另确认主窗口创建前后的状态写入：`MainData.IsNormalMode`（依据 `RunningMode`）、`VmSolution.IsIgnoreGlobalModule`、`IsClearLastExecuteResult`、`ImageViewBox.IsDirectDownSampling`、`VmSolution.IsCallBackCacheEnable` 及回调队列扩/关。

### 4.2 已入库的原厂资源身份

`src/VisionEditor/Resources/shell-menuitems.xml` 在 `31acdb4c` 入库，SHA-256 与原厂 `HostShell/ExeEntry/menuitems.xml` 一致；解析自检 50 节点 / 42 命令 / 15 工具栏项 / 4 分组 / 5 一级菜单，17 命令接线、25 命令显式禁用。快捷键经全树检索无证据，记 `not-applicable`。

### 4.3 未知与解除条件

- HostShell 的动态工具载体、`Dock`/布局状态持久化、配置入口逐成员对应：未取证，`divergent`（见保真度账本）。T2.3 处理。
- `MainWindowData` 完整命令/事件订阅表未展开；本次仅登记字段层。
- 原厂桌面 DPI/状态与当前构建的同机对照：仍缺 E3/E5。

## 5. REA E0 / E2 身份记录

### 5.1 本次新增（REA `inspect_pe_imports`）

| 项 | 值 |
|---|---|
| Evidence ID | `ev_1332cc2722c530deb0bf2279233f39399a7377ed145553c264bcd08eb872e7bc` |
| 目标 | `analysis/full-audit/b08-binding-read-20261030/ModuleFrame.dll`（本地原厂组件副本） |
| SHA-256 | `45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b` |
| 格式 | PE32+ / amd64 / 2,931,200 bytes |
| 导入 | 6 DLL、316 项、无 delay imports；`imphash(pefile)=c68ed65a9f5776e0939848d792b41264`，`coverage.status=complete`，`issues=[]` |
| 限制（原文） | 静态导入表不证明导入被绑定、转发或运行时可达；IAT 加载态与导出转发未观察 |

这是 **E0 身份 + E2 静态导入证据**，不构成 E3 运行证据，也不证明该副本与 VisionMaster 安装原件逐字节一致（原件当前不可访问）。

### 5.2 原厂安装原件复核（T3 阶段订正）

> **订正说明**：本节初稿误判「原厂安装不可访问」，原因是只检查了 `D:\Program Files\...`。T3 阶段按 `docs/reverse/C03-same-input-differential-20261110.md:9` 复查 `C:/Program Files/VisionMaster4.4.0`，确认安装根与原件存在，故此处按实测结果更正；初稿结论不再有效。

本机实测（`Get-FileHash -Algorithm SHA256`）：

| 原件 | 长度 | SHA-256 | 与 C03 归档记录 |
|---|---|---|---|
| `Applications/VisionMaster.exe` | 1,440,384 | `CF5730DF126C9C244E13E454CB664909F7E6A2BDF78B5314D03F6DC64E76D291` | **一致** |
| `Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll` | 13,422,592 | `F5129789D7CF29DCB5F1B772D58159478D1110F1380905BCD27C38ED5F072A4E` | **一致** |
| `Applications/myLibs/VMControls.Data.dll` | 30,208 | `8A9F45E1213D178E777F390B5978ECFE747A89916561D317CECF2436F38CCA50` | 历史 `rea-identity.json` 记 `a0321eb4...`（D: 路径），**不一致** |

REA `inspect_managed_artifact`（evidence `ev_8f927177917f330224e9f61831479402636bf865b0f4e73e132d2501cb682307`）给出本机 `VMControls.Data.dll` 的 E0 身份：MVID `37aef1a6-0fe9-4ded-a35a-8b1d8e2ecdae`、assembly `1.0.0.0`、`.NETFramework v4.6.1`、PE32 x86、IL-only + strong-name-signed、16 TypeDef / 165 MethodDef / 42 Property。

REA `inspect_managed_members`（evidence `ev_a45d15b7ca017552f931248505e3e0a9371da78fc125c4bdd050e9559fc73df8`）对 `VMControls.Data.ToolBoxData` 与 `VMControls.Data.ToolBoxItemData` 的 `unmatched_type_names=[]`，选中 2 类型 / 60 方法 / 336 调用边 / 146 字段访问 / 41 user strings，`coverage.state=complete`。**类型层交叉确认**了 §2 引用的反编译源码类型，但未逐方法体比对。

- 历史 `analysis/full-audit/batch1-20261019/A01/rea-identity.json` 记录的是 `D:\Program Files\...\VMControls.Data.dll`（SHA-256 `a0321eb4...`），与本机 C: 安装件哈希不同；两者长度同为 30,208 bytes。
- **结论**：本机安装件身份已可 E0 复核；但 `analysis/decompiled/VMControls.Data/**` 反编译源码究竟对应哪个构建、以及两版 `ToolBoxData` 方法体是否逐行一致，仍为 `unknown`。不得把类型层确认扩大为方法体等价。
- 解除条件：用 REA 对两版 DLL 做 `compare_managed_members` 或取得历史 D: 原件，逐方法比对 CIL 摘要与 `#US` 字面量。

## 6. 退出条件核对

| T1 退出条件 | 结果 |
|---|---|
| 证据路径、进度口径、状态词汇统一 | 已落地于第 1 节；计划文档残留计数已订正 |
| 三类索引含证据路径、成员/位置、观察、未知、复跑方法 | 第 2/3/4 节均已给出；RVA 仅 REA 目标有，托管源码以文件:行号定位 |
| 不重做工具箱解析算法 | 未改 `ToolboxPriorityStrategy.cs` |
| 无行为修改 | 已确认；只新增本报告与台账状态 |

## 7. 移交 T2 的开放项

1. `VisionEditor.ShellChecks` 最近方案菜单 3 项差异（附原厂 `UpdateRecentlySolutions` E1 证据）。
2. Guide / 主窗口逐控件与资源对照矩阵（FR-02）。
3. 启动→方案实体共享、`.sol` 容器不透明字段（FR-03）。
4. 工具载体、配置入口、布局状态（FR-03）。
