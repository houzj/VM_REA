# T3 执行生命周期与配置 / ROI / 显示数据流重审（2026-11-12）

执行：CodeBuddy ｜ 任务：T3.1（FR-04）、T3.2（FR-05）｜ 输入：`docs/reverse/C02-shared-execution-lifecycle-20261110.md`、`docs/reverse/C03-same-input-differential-20261110.md`、`docs/reverse/T1-evidence-index-20261112.md`、原厂安装原件与 REA。

> 本阶段为**逐成员重审与差异登记**，未修改任何产品代码。原厂静态证据复用 C02/C03 已归档结论并注明来源；当前侧证据为 E4（独立实现）。

## 1. FR-04 Run → Callback → Result → Stop/Cancel → Dispose 状态机

### 1.1 原厂侧（E1 源码链 + E2 PE/ABI，来源 C02 §「原厂源码核对」）

| 环节 | 原厂成员 / 位置 | 已确认行为 |
|---|---|---|
| 单次提交 | `analysis/decompiled/VM.Core/VM.Core/VmProcedure.cs:1094` `VmProcedure.Run` | 调 `ProcessManager.SyncExecuteOnce`；SDK 抛 `VmException` 时清 `IsRunning` 并上抛 |
| 等待与槽位 | `.../VM.PlatformSDKCS/ProcessManager.cs:277` | 按流程 ID 减 10000 定位 1000 槽，清运行标志，提交 `IMVS_ExecuteOnce_V30`；非零码立即抛错；成功且需等待才进 `PrecisionWait` |
| 回调 | `ProcessManager.cs:630` `GetWorkStatusCallBackInfo` | 工作状态为 0 且流程 ID 合法时写正常/完成标志再分发事件；调度器跳过超时模块时绕过该模块的清零路径（同轮实际值未采集） |
| 结果缓存与发布 | `VM.Core/VM.Core/VmSolution.cs:4067` | 先发全局事件再解析目标流程；结束分支写计数、耗时及待通知缓存；开始分支仅在清结果开关开启且计数 >1 时清叶输出；工作状态先入队再通知同步结果；`SetValue` 模块结果分支（约 4312）顺序相反 |
| 连续/停止提交 | `ProcessManager.cs:389-420` | 停止请求末参为 3；非零码保留为 `VmException` |
| 原生停止 | `IMVS_StopExecute_V30` VA `0x180647920` → `CServerManager::SM_StopExecute_V30` `0x1806702F0` → `PF_M_MsgDeal` `0x408` 分支 → `ControlStopExecuteByProcId` | 流程记录缺失返回 `0xE0000316`；`record+0x218` 内嵌接口 vtable `+0x10` 调用；处理器按实际返回码生成 `OK` / `Err*%d` |
| 原生完成等待 | `VM_M_StopRunProcedure` VA `0x18013C250`；`0xBB8` = 3000 ms | 事件 `+0x90/+0x80`、清连续标志、等 `+0x88`；超时返回 `0xE0000300`；仅 `WAIT_TIMEOUT` 单独分支，返回 0 不能反推已发信号 |
| 托管停止失败 | `VmProcedure.ContinuousRunEnable` setter | 更新属性前调 `RefreshUIStopExecute`；后台 `StopExecute` 抛错时 `isStopEnd=true` 被跳过；`SyncWaitor.Wait(ref bool)` 无期限，错误不回传等待线程 |
| 回调收尾 | 归档 worker `0x1800DD330`；`CVmObjectBase` 平台消息 772 | 先 signal `+0x88` → 发状态 → 可选回调 → signal `+0x98`；托管侧仅动作值 1/2/3 且存在流程时更新并入通知字典 |

### 1.2 当前侧（E4）

| 环节 | 当前位置 | 行为 |
|---|---|---|
| 启动前置 | `src/VisionEditor/WorkflowExecution.cs:10-17` `CanStartWorkflow` | 拒绝「模块调试忙 / Group 执行中 / 无节点 / `GraphValidator.Validate` 失败」，失败时清无效结果 |
| 会话建立 | `:20-25` `StartRunCore` | `activeRun = new CancellationTokenSource()`、`continuousRun=false`、`runningProcedureId`；`finally` 调 `FinishWorkflowRun(owner)` |
| 工作区与执行 | `:26-38` `RunCore` | `SolutionWorkspace(doc.Document, doc.BaseDirectory)` → `BeginWorkflowResults` → `SolutionExecutor(modules).RunAsync(workspace, procedureId, progress, cancellationToken, scope, control)` |
| 结果发布 | `:43-57` | 注释明确对应 `VmSolution` 的「先同步提交状态、再向双缓冲回调队列发布同一结果」seam；`WorkflowCallbacks.PublishModuleResult(同步展示闭包, 异步缓存闭包)`；缓存闭包写 `StoreWorkflowCallResults`、`RecordWorkflowRun`、`RecordFrameworkRunHistory` |
| 成功判定 | `:58` | `session.Status==Success` 且所有节点为 `Success/Skipped` |
| 取消 | `:60` `catch(OperationCanceledException)` | 仅在当前执行所有者仍有效时置状态文本「运行已停止」 |
| 托管作用域 | C02 §「自研代码交付」 | `GraphExecutor` 统一判断调用者取消与方案关闭；`SolutionExecutor` 持调用期作用域租约；关闭优先于流程预算超时；抑制迟到输出；参数准备回调取消后不进入 `ExecuteAsync` 且实例照常释放；`ToolDebugSession.StopAsync` 改为异步派发取消回调 |
| 停止修复（Codex 未提交） | `src/VisionEditor/SubprogramCallWindow.xaml.cs` | Stop 不再误取消当前 SelfRun；关闭时仍取消并排空执行 |

### 1.3 状态转移与失败矩阵（差异登记）

| 场景 | 原厂证据 | 当前侧证据 | 判定 |
|---|---|---|---|
| 正常运行完成 | `VmSolution.cs:4067` 结束分支 | `RunCore` 发布两阶段结果 | `source-shaped`（形状）、运行差分未闭合 |
| 重入 / 连续运行 | `ProcessManager.cs:389` 连续提交 | `StartRunCore` 以 `activeRun != null` 拒绝；`ModuleRunControl.Mode=Continuous` | `independent-contract` |
| 停止失败 | 托管 `SyncWaitor.Wait` 无期限、错误不回传（E1） | 协作取消 + `finally` 释放（E4） | **`divergent`**：当前侧无对应的无限等待与错误吞没语义；不按原厂缺陷复刻 |
| 在途算法 / 资源完成边界 | 原生 3000 ms 事件序列（E2） | 作用域租约 + 等待活动执行（E4） | `unknown`：原厂同轮时序未采集 |
| 异常订阅者 | `CVmObjectBase` 通知字典（E1） | 回调取消/抛错回归（E4，C02 432 项） | `independent-contract` |
| 子流程 / Group | `Vmsolution` 流程解析（E1） | `SubprogramCallWindow`、Group 生命周期 58 项（E4） | `independent-contract` |
| 清结果开关 | 开始分支仅在开关开启且计数 >1 时清叶输出（E1） | `ClearInvalidWorkflowResults` / `BeginWorkflowResults`（E4） | `independent-contract`，未逐条件对齐 |

**重写项：0。** 唯一具备明确原厂 E1/E2 的「托管停止失败」是原厂缺陷语义，用户与门禁均未授权把缺陷复刻进自研实现；其余差异缺 E3 同轮运行证据。

## 2. FR-05 配置 / ROI / Mask / 显示 / 参数提交数据流

### 2.1 当前侧公共合同清单（E4）

`src/VisionPluginContracts/ToolConfiguration*.cs` 共 **27** 个文件，按职责分组：

| 组 | 文件 | 语义 |
|---|---|---|
| 模型与状态 | `ToolConfigurationDefinition.cs`、`State.cs`、`Provider.cs`、`View.cs`、`ModelWindow.cs` | 参数定义、会话状态、提供者、视图与窗口 |
| 数值/文本/路径 | `NumericRanges.cs`、`NamedTextObjects.cs`、`OpenFilePathControls.cs`、`SaveFileService.cs`、`CalibrationPathControls.cs`、`CalibrationPathPolicy.cs` | 数值上下界、命名文本、文件路径与标定路径候选值保留 |
| 控件与绑定 | `StandardControls.cs`、`ObjectControls.cs`、`ObjectBindings.cs`、`ImageObjectControls.cs`、`SubscriptionControls.cs`、`DisplaySubscriptions.cs` | 模板/控件、绑定与显示订阅 |
| ROI | `ToolConfigurationRoiControls.cs`、`ToolConfigurationRoiService.cs` | ROI 编辑会话 |
| 提交与执行 | `ToolConfigurationExecution.cs`、`Operations.cs`、`InteractionServices.cs`、`ConfirmationService.cs`、`AlgorithmReset.cs` | 读取/编辑/验证/提交/失败恢复 |
| 订阅与宿主 | `Subscription.cs`、`SubscriptionBatch.cs`、`WindowHost.cs`、`FolderServices.cs` | 订阅批处理与窗口宿主 |

`ToolConfigurationRoiService.cs:12-33` 定义 `ConfigurationRoiRequest`、`ConfigurationRoiSnapshot`、`IToolConfigurationRoiSession`（`GroupRoiEditor`、`Refresh()`、`Changed`、`IDisposable`）与 `Open(request)` 按 `SourceName` 管理会话；`:24` 源码注释明确 **「Managed geometry is not native IUserBytesData ABI」**，即当前几何为独立托管实现，不伪装原生 ABI 等价。

结果显示侧：`src/VisionEditor.Core/NodeResultProjection.cs`、`ModuleRunLedger.cs`、`src/VisionEditor/MainWindow.WorkspaceProjection.cs`、`MainWindow.ModuleTree.cs`、`ToolDisplays.cs`（`31acdb4c` 工作区链路）。

### 2.2 原厂侧已确认证据（E2 native ABI，来源 C03 §33-65）

| 观察点 | 原厂行为 |
|---|---|
| `CIMVSBlobFindModu::ProcessAlg` VA `0x18001FEC0` | 用输入 `+0x10` 设置 SDK 图像并重置区域状态；模式 1 直传 `+0x118`；其他模式下「首集合恰一项且第二集合为空」直传单范围，否则走 vtable `+0x48` |
| Mask 生成助手 | vtable `+0x48` → RVA `0x9C80`；`this+0x20` 为空时经 IAT `0x18002D4D0` 延迟创建 `MVDPreproMaskCpp.dll!CreatePreproMaskToolInstance`；汇编只证明分支存在 |
| 掩膜追加条件 | 第二集合为空且输入 `+0x110` 非零 |
| `GenerateMaskImage` 五参输入 | `ProcessAlg` 在 `0x180020007` 设 R8=输入 `+0x20`、`0x180020043-052` 写第五参数低字节 0；入口 `0x180009D3F` 读该字节；为 0 时跳至 `0x18000A2FF` 绕过已有区域与宽高比较 |
| 区域构建 | `+0x50` `ClearRegions`；两循环按 begin/end 算 8 字节条目；`+0x28` `AddRegion` 先加输入 `+0x20` 集合传 `true`，再加 `+0x38` 集合传 `false`；`+0x10` `SetInputImage` |
| 运行与回写 | `+0x20` `Run` → `+0x18` `GetOutputImage` → 写 `+0x118`；图像接口 `+0xB8` 对应 `IMvdImage::GetData`，键 `SharedMemName`，返回写 `+0x120`；成功置 `+0x110=1` |
| 结果读取 | vtable `+0x78` 运行、`+0x30` 取结果；空结果抛 `0xe0000315`；`ProcessOutput` 再取结果投影数量与总面积，图像输出各有独立标志 |

### 2.3 数据流矩阵与差异

| 环节 | 原厂证据 | 当前实现 | 状态 | 决定 |
|---|---|---|---|---|
| 参数读取 / 编辑 / 验证 | 原厂 `Apps.XmlParser` 参数模板、绑定与事件未逐项取证 | 27 个 `ToolConfiguration*` 文件 + 数值范围/路径策略 | `independent-contract` | 隔离，T5.3 按需扩展 |
| 参数提交与失败恢复 | 原厂 `IUserBytesData` 二进制载荷未取证 | `ToolConfigurationExecution.cs`、`CalibrationPathPolicy.cs`（候选值保留，P6 回归 85 项） | `independent-contract` | 保留 |
| ROI 集合 / 全屏 / 尺寸变化 | 原厂 `+0x20`/`+0x38` 双集合、`ClearRegions`/`AddRegion`（E2） | `GroupRoiEditor` + `RoiDocument`（托管几何，自述非原生 ABI） | `divergent` | 隔离；不伪造原生集合 |
| Mask 生成与像素缓冲 | 原厂 `GenerateMaskImage` 调用链（E2），活动宿主是否经过未证 | 当前侧无对应原生缓冲链 | `unknown` | 运行未确认 |
| 显示开关 / 结果发布 | `VmSolution.cs:4067` 双缓冲回调（E1） | `WorkflowCallbacks.PublishModuleResult` 两阶段（E4，代码注释明确对应 seam） | `source-shaped` | 保留 |
| 二进制载荷 / 失败恢复 | 原厂 `IUserBytesData` 字段未取证 | 未见等价字段 | `unknown` | 不补实现 |
| 同输入差分 | 直接 SDK 两次复跑 47/973202 一致；原厂宿主 45/971486（历史） | 当前实现无匹配宿主复跑 | `unknown` | T5.2 处理 |

**重写项：0。** 原厂 ROI/Mask 的 E2 证据只覆盖归档 Blob 模块的静态分支；当前侧公共配置合同属独立实现，强行按单个 native 模块改写会破坏 27 文件既有契约且缺 E3 宿主运行证据。

## 3. REA E0 / E2 复核（本机原厂安装）

### 3.1 身份核对

`C:/Program Files/VisionMaster4.4.0` 实测存在；`VisionMaster.exe`（`CF5730DF…D291`）与 `iMVS-6000PlatformSDK.dll`（`F5129789…2A4E`）与 C03 归档记录**逐字节一致**；`VMControls.Data.dll` 本机 SHA-256 `8A9F45E1…CA50`，与历史 `rea-identity.json` 的 `a0321eb4…9f8`（D: 路径）**不同**。

### 3.2 REA 调用结果

| Evidence | 工具 | 关键结果 |
|---|---|---|
| `ev_1332cc2722c530deb0bf2279233f39399a7377ed145553c264bcd08eb872e7bc` | `inspect_pe_imports`（T1，本地 `ModuleFrame.dll` 副本） | SHA `45c57bc4…ae9b`、PE32+ amd64、6 DLL / 316 imports、imphash `c68ed65a9f5776e0939848d792b41264`；该 SHA 与 C02 §32 归档 ModuleFrame 哈希一致，交叉确认副本身份 |
| `ev_8f927177917f330224e9f61831479402636bf865b0f4e73e132d2501cb682307` | `inspect_managed_artifact`（本机 `VMControls.Data.dll`） | MVID `37aef1a6-0fe9-4ded-a35a-8b1d8e2ecdae`、v1.0.0.0 / .NETFramework 4.6.1、16 TypeDef / 165 MethodDef / 42 Property、IL-only + strong-name-signed |
| `ev_a45d15b7ca017552f931248505e3e0a9371da78fc125c4bdd050e9559fc73df8` | `inspect_managed_members`（同上，选 `ToolBoxData`/`ToolBoxItemData`） | `unmatched_type_names=[]`、2 类型 / 60 方法 / 336 调用边 / 146 字段访问 / 41 user strings、`coverage.state=complete` |

限制（REA 原文）：静态检查不加载 CLR、不解析依赖、不反编译 C#、不执行目标代码；元数据 token 为构建局部坐标，仅在给定 SHA-256 + MVID 下有意义；PE 导入表不证明运行时绑定、转发或可达。

### 3.3 结论边界

- 类型层交叉确认了 T1 §2 引用的 `ToolBoxData` / `ToolBoxItemData` 存在，**不等于方法体逐行等价**。
- 本机安装件与历史 D: 记录的 `VMControls.Data.dll` 哈希不同；`analysis/decompiled/VMControls.Data/**` 对应哪个构建仍 `unknown`。
- 归档 `ModuleFrame.dll` 本地副本可与 C02 归档哈希对齐，但 C02/C03 均记录**安装原件与归档存在哈希差异**；因此 E2 结论限定于归档件，不外推到当前安装。

## 4. 差异决定汇总

| 决定 | 数量 | 说明 |
|---|---|---|
| 保留 | 3 | 显示双缓冲发布 seam、配置提交/路径策略、布局缩放 |
| 隔离 | 4 | 重入/连续运行、参数模型与模板、ROI 集合、Group/子流程合同 |
| 运行未确认 | 4 | 原生停止同轮时序、异常订阅者、Mask 生成宿主路径、二进制载荷 |
| **重写** | **0** | 无一项同时具备 E1/E2 + E3 证据 |

## 5. 退出条件核对

| T3 退出条件 | 结果 |
|---|---|
| VS5 失败/停止/释放可重复 | 当前侧 C02 11 套 / 462 项（Codex 自述，未在本阶段复跑）；原厂侧未闭合 |
| 关键路径调用链有证据 | 原厂 E1/E2 已登记；当前侧 E4 已登记 |
| 每项公共交互合同有来源、实现、测试与真实工具消费 | **未完全达成**：原厂模板/绑定/事件来源未逐项取证，真实工具消费证据依赖 T4 |
| 无行为修改 | 已确认 |

## 6. 未确认项与解除条件

- 原厂同轮回调、在途算法与资源完成边界：需匹配宿主 E3 采集。
- `VMControls.Data.dll` 两构建与反编译源码的版本对应：需 `compare_managed_members` 或取得 D: 原件。
- 原厂参数模板、绑定、事件、`IUserBytesData` 二进制载荷：需原件与逐成员取证。
- 活动 ROI/Mask 集合与 Run 前像素：需 `test1108.sol` 与匹配宿主（T5.2）。
- `VisionEditor.ShellChecks` 首个失败：需逐次记录 `RecentSolutionStore.Update` 调用点（T2 §5，移交 T5.4）。
- 原厂托管停止失败分支（无限等待/错误吞没）：**原厂缺陷语义，不在自研复刻**，仅保留差异记录。
