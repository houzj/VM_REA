# VisionMaster 当前实现保真度账本

建立日期：2026-11-12

本账本区分当前实现是否接近原厂证据。测试通过只证明当前实现合同，不自动提升原厂保真度。

## 状态定义

- `parity-closed`：原厂源码/资源、匹配运行和当前实现均对齐。
- `source-shaped`：调用链和数据形状按原厂恢复，运行差分未闭合。
- `independent-contract`：当前独立合同可运行，原厂语义未证明。
- `visual-only`：外观近似，交互或数据流未闭合。
- `divergent`：已观察到与原厂的具体差异。
- `unknown`：证据不足，禁止推断。

## 当前账本

| 区域 | 当前状态 | 已确认内容 | 具体差异/未知 | 证据 | 下一目标 |
|---|---|---|---|---|---|
| 启动入口与单实例 | `source-shaped` | Program、工作目录、单实例、初始化阶段和关闭策略已有实现 | 原厂初始化服务和认证真实状态未运行确认；曾出现窗口关闭后残留进程 | `src/VisionEditor/Program.cs`、`src/VisionEditor/StartupLaunch.cs`、`docs/reverse/C01-startup-chain-design-review-20261111.md` | G2 |
| WelcomeDialog | `source-shaped` | 640×360、原厂 GIF、WindowsFormsHost 证据已恢复 | 用户桌面实际比例和启动时序还未与原厂同机对照 | `src/VisionEditor/StartupWelcomeWindow.xaml`、`analysis/ui-assets/original-guide/WelcomePage.GIF` | FR0 |
| GuideWindow | `divergent` | 原厂 XAML、卡片资源、标题栏、对角背景和底部图标已恢复 | 用户已观察到旧构建与原厂差异大；新修正版尚未完成用户复核 | `src/VisionEditor/StartupGuideWindow.xaml`、原厂 `GuideWindow.xaml`、`docs/reverse/C01-startup-shell-implementation-20261111.md` | FR0/FR1 |
| 方案选择与最近列表 | `independent-contract` | 独立 JSON 创建、最近列表、打开和保存路径已实现 | 原厂方案实体、最近项字段、`.sol` 容器和直启场景未确认 | `src/VisionEditor/MainWindow.StartupSolution.cs`、`docs/reverse/C01-startup-solution-chain-20261111.md` | G2 |
| 主窗口工具载体 | `divergent` | 当前工具箱、画布和主窗口可构建 | 原厂工具载体、动态装载、配置入口和布局状态未逐成员对齐 | `src/VisionEditor/MainWindow.xaml`、`MainWindow.xaml.cs`、原厂 HostShell 证据 | FR2 |
| 公共执行生命周期 | `source-shaped` + `independent-contract` | Run、Stop、Callback、Result、Cancel、Dispose 的源码链和当前修复已记录 | 原厂实例同轮回调、在途算法和资源完成边界未运行确认 | `docs/reverse/C02-shared-execution-lifecycle-20261110.md`、C02 最新回归摘要 | G3 |
| C02 当前实现回归 | `independent-contract` | 11 套、462 项无界面检查通过，身份审计通过 | 不能证明 VM 原厂调度等价 | `analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/summary.json` | G3 |
| 配置与参数提交 | `independent-contract` | 托管参数模型、保存、执行和部分编辑器已有实现 | 原厂模板、绑定、事件、二进制载荷和失败恢复未完整对齐 | `src/VisionPluginContracts/ToolConfiguration*.cs`、相关配置报告 | G4 |
| ROI/Mask/显示 | `divergent` | 已有 ROI、全屏、显示开关和部分参数投影 | 原厂 ROI 集合、Mask 生成、像素缓冲和显示发布链未完整确认 | `docs/reverse/C03-same-input-differential-20261110.md`、原厂 Blob 汇编证据 | G4 |
| 直接 SDK Blob | `runtime-confirmed` | 固定样图下 26 次矩阵和缓冲差分可重复 | 仅证明直接 SDK；不能代替原厂宿主 | `analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-4dd306f7d33a447dba8e0793ad073807/summary.json` | G5/G8 |
| 原厂宿主 Blob | `unknown` | 已有历史 `45/971486` 与 `47/973202` 记录和静态调用边 | `test1108.sol` 缺失，宿主依赖不匹配，首处分歧未取得 | `analysis/full-audit/batch1-20261107/C03/host-identity-a28da3dbf1394c1d92d8145253651002/summary.json` | G8 |
| Group/子流程 | `independent-contract` | 当前 Group、循环、分支、复合叶和执行回归已有实现 | 原厂复杂数据、模板、回调和生命周期未完整差分 | Group 相关报告、C02 回归套件 | G7 |
| 保存/重开/资源恢复 | `independent-contract` | 独立 JSON 方案保存、重开和默认迁移已实现 | 原厂 `.sol`、资源容器、版本迁移和错误恢复未确认 | `src/VisionEditor/MainWindow.StartupSolution.cs`、方案测试 | G6 |
| 工具批次 | `independent-contract` 或 `unknown` | 已有工具实现和部分源码/测试 | 多数工具尚未完成原厂同输入、主窗口和差分闭环 | `docs/ALL-TOOLS-IMPLEMENTATION-STATUS.json`、各工具报告 | G9 |

## 最大差异排序

1. 启动后 Guide/主窗口与原厂的视觉和状态差异。
2. 解决方案实体、`.sol` 容器和场景直启缺失。
3. 工具载体、配置入口和公共配置合同与原厂的差异。
4. 原厂宿主与直接 SDK 的 ROI/Mask/像素首处分歧未定位。
5. Group、复杂数据和保存重开尚未完成原厂差分。

## 账本退出条件

- 每行都有唯一原件、代码、测试和报告路径。
- 每个差异有处理决定：保留、重写、隔离或运行未确认。
- `independent-contract` 不得写入原厂等价结论。
- 最大差异前五项各自进入 G2-G8 目标。
- 新目标关闭前重新计算本账本，不用旧测试结果覆盖新差异。

## T4 CHECK-FR 判定（2026-11-12，CodeBuddy 追加）

本节由 `docs/reverse/T4-first-fidelity-slice-20261112.md` 驱动，只追加状态与决定，不改写上方历史账本。判定：**未通过**，继续冻结 `fidelityCorrectionBeforeExpansion` 所列的工具批次扩展。

| 最大差异 | 判定前状态 | 处理决定 | 判定后状态 | 依据 |
|---|---|---|---|---|
| 1. 启动后 Guide/主窗口视觉与状态差异 | `divergent` | 运行未确认（不因截图改语义，等待人工复核与原厂同机证据） | **不提升** | `T2-startup-solution-matrix-20261112.md` §1 |
| 2. 方案实体、`.sol` 容器和场景直启 | `independent-contract` | 隔离（JSON 不伪装 `.sol` 兼容） | **不提升** | T2 §2、§6 |
| 3. 工具载体、配置入口和公共配置合同 | `divergent` | 运行未确认 + 隔离 | **不提升** | T2 §3、T3 §2 |
| 4. 原厂宿主与直接 SDK 的 ROI/Mask/像素首处分歧 | `unknown` | 运行未确认（缺 `test1108.sol` 与匹配宿主） | **不提升** | T3 §2、T4 §6 |
| 5. Group、复杂数据和保存重开的原厂差分 | `independent-contract` | 隔离（当前闭环 E4 通过，原厂语义未证） | **不提升** | T3 §1、T4 §3 |

FR0–FR4 达 `source-shaped` 情况：仅 FR3 为部分（`source-shaped-with-runtime-open`），其余为否；五个最大差异均已登记处理决定，但「有决定」不等于「达标」。

- 本轮新增复跑 5 套共 288 项、T0 复跑 8 条命令；**断言数与命令数不并入任何完成度分母**。
- `independent-contract` / `source-shaped` 一律未改写为 `parity-closed`；E3 原厂运行与 E5 人工验收仍缺。
- `VisionEditor.ShellChecks` 首个失败（`Program.cs:275`）保持开放，未修改产品或夹具。
- 复跑命令见 `docs/reverse/T0-replay-command-inventory-20261112.md` 与 `T4-first-fidelity-slice-20261112.md` §2。
- 解除条件：FR0/FR1/FR2/FR4 取得原厂 E1 逐成员证据 + FR3 取得匹配宿主 E3 同轮运行证据，且人工验收由用户明确要求后另行记录。

