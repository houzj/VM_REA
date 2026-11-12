# CodeBuddy 接手任务清单：VisionMaster 高保真逆向（2026-11-12）

计划：`docs/CODEBUDDY-TAKEOVER-PLAN-20261112.md` ｜ 门禁：`docs/VM-REVERSE-GATE-MATRIX-20261112.json` ｜ 设计依据：`docs/VM-COMPLETE-REVERSE-IMPLEMENTATION-PLAN-20261112.md`

## 0. 本轮状态声明

- 用户本轮只要求「制定计划和任务清单，把工作从 codex 接入到 codebuddy 继续」。
- **PLAN-00 已完成：接手计划与任务清单。**
- **T0（T0.1–T0.4）已完成**：只做只读验收、Release 构建与回归复跑、44 份门禁哈希登记、临时目录判定，并落地四份 `docs/reverse/T0-*` 产物；未改产品行为、未提交、未推送。首个失败（`VisionEditor.ShellChecks`）如实登记，未掩盖。
- **T1（T1.1–T1.4）已完成**：口径对照表、三类原厂证据索引与 REA E0/E2 身份记录落地于 `docs/reverse/T1-evidence-index-20261112.md`；无行为修改。**T2（T2.1–T2.3）已完成**：FR-02/FR-03 对照矩阵、差异决定（重写 0 项）与首个失败归属落地于 `docs/reverse/T2-startup-solution-matrix-20261112.md`；无行为修改、未改测试夹具。**T3（T3.1–T3.2）已完成**：FR-04/FR-05 逐成员重审、native ABI 边界与本机原厂安装 E0 复核落地于 `docs/reverse/T3-execution-config-dataflow-20261112.md`；同时订正了 T1 §5.2 的安装路径误判。无行为修改。**T4（T4.1–T4.2）已完成**：首条垂直切片回归（新增 5 套 288 项）与 CHECK-FR 判定落地于 `docs/reverse/T4-first-fidelity-slice-20261112.md`，保真度账本已追加判定章节；**判定未通过，继续冻结工具批次扩展**。无行为修改。**T5（T5.1–T5.5）已完成**：T5.1 取得 30 份 `ToolGroup.xml`（4 份含真实优先级数值）与 3 份 `CircleFind.sol` 身份；T5.2 首处分歧保持 `unknown`；T5.3 因 CHECK-FR 未通过保持 `blocked`；T5.4 合流回归 3 通过 / 1 仍失败（无回退）；T5.5 最终报告已交付，提交与人工验收留用户决定。全部计划任务执行完毕。
- Codex 留下的 `tasks/plan.md`（全部未勾选）与 `tasks/todo.md`（FR-01 勾选、FR-02～FR-06 未勾选，但正文叙述大量 G2–G9 结果）状态与叙述脱节，**冻结为历史快照，不再勾选、不作为执行依据**。
- Codex 交付的 G1–G9 门禁结果在 CodeBuddy 侧一律标记为「未复跑」，不得当作已验证结论。

图例：`completed` 本轮已交付 ｜ `pending` 未开始 ｜ `blocked` 需先取得缺失原件或用户决定 ｜ `unknown` 证据不足且不推断。
保真度词：`parity-closed` / `source-shaped` / `independent-contract` / `visual-only` / `divergent` / `unknown`。

## 1. UI 任务列表与任务卡映射

| UI 项 | 状态 | 对应任务卡 |
|---|---|---|
| 制定接手计划与任务清单 | `completed` | PLAN-00 |
| T0 交付验收 | `completed` | T0.1–T0.4 |
| T1 口径校正与批量证据索引 | `completed` | T1.1–T1.4 |
| T2 启动/方案/工具载体对照矩阵 | `completed` | T2.1–T2.3 |
| T3 执行链与配置数据流重审 | `completed` | T3.1–T3.2 |
| T4 首条高保真垂直切片与 CHECK-FR | `completed`（判定未通过） | T4.1–T4.2 |
| T5 缺口取证、原厂差分、G9 续作与发布 | `completed`（G8 unknown / G9 blocked） | T5.1–T5.5 |

---

## 2. T0 交付验收与基线冻结

### T0.1 ｜ 工作树责任清单与身份冻结 ｜ `completed`
- 单一问题：`31acdb4c` 之后 Codex 究竟改了哪些文件，各自归属与证据是什么。
- 原厂身份/输入：`git rev-parse HEAD`、`git status --porcelain -uall`、`git diff --stat`、`git ls-files`；17 `M` + 55 未跟踪条目。
- 允许修改：仅新增 `docs/reverse/T0-*` 报告；不改任何产品代码。
- 依赖/合同：无；跨项目边界按 `docs/PARALLEL-WORK.md` 顶部规则。
- 验证命令：`git status --porcelain -uall`；`git diff --name-only`；`git ls-remote origin refs/heads/main`。
- 产物：`docs/reverse/T0-codex-takeover-acceptance-20261112.md` 责任清单。
- 退出条件：每个 `M`/`??` 条目有来源、责任、是否已提交、证据路径四列；无遗漏。
- 未知与解除条件：无原件身份（E0）的条目标 `unknown`，待 T0.3 哈希补齐。

### T0.2 ｜ Release 构建与核心回归复跑 ｜ `completed`
- 单一问题：Codex 交付在 CodeBuddy 侧能否按记录命令重现 0 警告/0 错误与测试结果。
- 原厂身份/输入：`src/VisionEditor/VisionEditor.csproj`、`tests/ToolCatalogLifecycleChecks15/`。
- 允许修改：独立 `OutDir` 下构建产物；不改源码。
- 依赖/合同：`UseSharedCompilation=false`、`BuildInParallel=false`；不终止其他任务进程。
- 验证命令：`dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore`；`dotnet run --project tests/ToolCatalogLifecycleChecks15/ToolCatalogLifecycleChecks15.csproj -c Release -- src/VisionEditor`。
- 产物：构建与测试原始日志、退出码、产物时间戳。
- 退出条件：两条命令实际执行并留日志；Codex 记录的 19/19 只在复跑一致时才可引用。
- 未知与解除条件：其余测试套件的 csproj 与参数未冻结，T0.3 补清单后再复跑，不猜命令。

### T0.3 ｜ 44 份门禁产物哈希与状态重定级 ｜ `completed`
- 单一问题：G1–G9、C01/C02/P6 的 `*-completion.json` 各自声明什么状态，是否被 `analysis/*` 忽略规则排除在 Git 之外。
- 原厂身份/输入：`analysis/full-audit/fidelity/20261112/*.json`（44 份，含 `matrix.json`）。
- 允许修改：只新增哈希清单报告。
- 依赖/合同：T0.1；产物不可从远程复原，报告中必须显式注明。
- 验证命令：`Get-ChildItem ... -Filter *.json | Get-FileHash -Algorithm SHA256`；`git check-ignore -v`。
- 产物：SHA-256 清单 + 每个 `goal/status/nextGoal` 的登记表。
- 退出条件：44 份逐一登记 `status`；CodeBuddy 验收列统一写「未复跑」，直到 T0.2/T0.4 完成。
- 未知与解除条件：门禁 JSON 是 Codex 自述，不证明原厂等价；E5 人工验收仍缺。

### T0.4 ｜ 临时目录判定与资产清单 ｜ `completed`
- 单一问题：`src/VisionEditor/tool-lifecycle-*/` 两个哈希目录（3 文件）是运行时临时产物还是交付物。
- 原厂身份/输入：目录内文件、生成时间、引用点。
- 允许修改：只出判定报告；确认是临时产物时再由用户决定是否清理，不自行删除。
- 依赖/合同：T0.1。
- 验证命令：`Get-ChildItem -Recurse`；全仓搜索目录名引用。
- 产物：判定结论 + 被跟踪 UI 资产清单（`analysis/ui-assets/**`、样图）。
- 退出条件：每个目录有「保留/忽略/待定」决定及依据。
- 未知与解除条件：找不到生成者时标 `unknown`，不猜测用途。

---

## 3. T1 口径校正与批量证据索引

### T1.1 ｜ 证据路径与进度口径统一 ｜ `completed`
- 单一问题：接手后所有报告引用哪套证据根、哪套完成度分母。
- 原厂身份/输入：交接文档第 22–23 行；`docs/framework-progress-audit.json`；`docs/SOURCE-LEVEL-REVERSE-PROGRESS.json`。
- 允许修改：仅 `docs/CODEBUDDY-TAKEOVER-PLAN-20261112.md` 与新增口径说明；**不改行为、不改历史叙述、不改 AGENTS.md**。
- 依赖/合同：T0.1。
- 验证命令：`Test-Path` 校验证据根；读取两份进度 JSON 的 `summary` / `completionPercent`。
- 产物：口径对照表（41/96、42/96、10/10、16/162、128 gates 各自定义与权威级别）。
- 退出条件：后续报告只引用 `analysis/decompiled` 与 `analysis/full-audit` 工作树相对路径；对外完成度只报 10/10 = 100%，框架 42/96 = 43.75% 单列。
- 未知与解除条件：16/162 目录工具口径无当前权威台账，标为历史值，不参与报表。

### T1.2 ｜ Guide 卡片配置写入与首次工具箱加载入口索引 ｜ `completed`
- 单一问题：Guide 卡片点击后写入了什么配置，主窗口何时首次装载工具箱。
- 原厂身份/输入：原厂 `GuideWindow.xaml.cs:128`、`app.config:56`、`ToolBoxData.cs`、`ToolBoxItemData.cs`；当前 `StartupGuideWindow.xaml*`、`ToolNavigation.cs`、`ToolboxPriorityStrategy.cs`。
- 允许修改：只新增索引报告。
- 依赖/合同：与 `docs/reverse/C01-toolbox-type-priority-chain-20261112.md` 衔接，**不重做解析算法**；真实 `ToolGroup.xml` 优先级数值缺失保持 `unknown`。
- 验证命令：源文件逐行定位 + 复跑 `ToolCatalogLifecycleChecks15`。
- 产物：索引条目含证据路径、成员/RVA、观察、未知、复跑方法。
- 退出条件：入口到首次装载的调用边闭合或明确断边位置。
- 未知与解除条件：四类真实排序值缺原件，取得后只补元数据并复跑，不写死。

### T1.3 ｜ 方案实体成员链索引 ｜ `completed`
- 单一问题：创建/打开/Procedure/Node/资源目录/保存/关闭各自读写哪些原厂成员。
- 原厂身份/输入：`analysis/decompiled` 中方案与持久化相关类型；当前 `MainWindow.StartupSolution.cs`、`SolutionStore`。
- 允许修改：只新增索引报告。
- 依赖/合同：原厂 `.sol` 未知字段保持不透明，不把 JSON 伪装成 `.sol` 兼容。
- 验证命令：源码定位 + 现有方案生命周期回归（命令待 T0.3 冻结）。
- 产物：成员链矩阵（原厂成员 / 当前实现 / 字段读写 / 状态 / 未知）。
- 退出条件：七个环节逐一有证据或明确 `unknown`。
- 未知与解除条件：`test1108.sol`、`CircleFind.sol` 缺失，需匹配原件与运行环境。

### T1.4 ｜ MainWindow 与 MainWindowData / HostShell 对应矩阵 ｜ `completed`
- 单一问题：当前每个主窗口入口对应原厂哪个成员、资源键与命令。
- 原厂身份/输入：原厂 `MainWindowData`、`HostShell/ExeEntry/menuitems.xml`；当前 `MainWindow.*.cs`、`ShellMenuCatalog.cs`、`ShellCommandRouter.cs`。
- 允许修改：只新增索引报告。
- 依赖/合同：`menuitems.xml` 已在 `31acdb4c` 入库且 SHA-256 与原件一致；25 个禁用命令的原因不得被抹掉。
- 验证命令：复跑 `VisionEditor.ShellMenuChecks`（命令待 T0.3 冻结）。
- 产物：入口对应矩阵。
- 退出条件：每个入口有原厂对应项或标 `independent-contract`。
- 未知与解除条件：快捷键证据经全树检索无结果，已记 `not-applicable`，不推断。

---

## 4. T2 启动、方案与工具载体对照矩阵

### T2.1 ｜ FR-02 当前构建与原厂启动/方案/主窗口对照矩阵 ｜ `completed`
- 单一问题：窗口、资源、状态转移和交互差异的具体证据是什么。
- 原厂身份/输入：原厂 XAML/BAML、资源键、启动状态机；当前 `31acdb4c` + Codex 未提交改动。
- 允许修改：只新增矩阵报告；**不因截图改尺寸/颜色/点击语义**。
- 依赖/合同：T1.1–T1.4；截图与用户反馈只登记差异，不定义语义。
- 验证命令：资源/绑定/事件逐项比对；差异项标注证据等级 E0–E5。
- 产物：`docs/reverse/T2-startup-solution-matrix-20261112.md`。
- 退出条件：每个差异有「保留 / 重写 / 隔离 / 运行未确认」决定。
- 未知与解除条件：Guide 新修正版未经人工复核，保持 `divergent`。

### T2.2 ｜ FR-03 启动链与方案实体重审 ｜ `completed`
- 单一问题：Welcome→Guide→空载主窗口→关闭释放，以及新建/打开是否共享同一方案实体。
- 原厂身份/输入：原厂 Program/App/启动服务/方案成员；当前 `Program.cs`、`StartupLaunch.cs`、`MainWindow.StartupSolution.cs`。
- 允许修改：经 T2.1 判定为「重写」且具备 E1 证据的框架文件；单一提交所有者。
- 依赖/合同：T2.1；关闭后进程与互斥体释放须验证。
- 验证命令：`VisionEditor.StartupShellChecks` 启动身份/保存路径/身份保持/重开/clean 状态。
- 产物：状态机 + 差异修复与回归记录。
- 退出条件：VS0、VS2 可复跑；创建/保存/重开/执行使用同一方案实体。
- 未知与解除条件：原厂初始化服务、认证、同机时序保持 `unknown`。

### T2.3 ｜ FR-03 工具载体、配置入口与布局状态重审 ｜ `completed`
- 单一问题：原厂动态工具载体、配置入口和布局状态逐成员对应到当前何处。
- 原厂身份/输入：原厂工具载体/布局资源；当前 `MainWindow.xaml*`、`WorkflowCanvas.cs`、`ToolNavigation.cs`。
- 允许修改：仅在 T2.1 判定允许且证据齐全时改框架文件。
- 依赖/合同：T1.4；公共配置合同归 `main`，跨项目需求写 `docs/CONTRACT-REQUESTS.md`。
- 验证命令：工具生命周期与宿主归属回归（命令待 T0.3 冻结）。
- 产物：逐成员对应表 + 差异决定。
- 退出条件：`divergent` 项收敛为 `source-shaped` 或明确列为 `unknown`。
- 未知与解除条件：原厂动态装载与布局状态仍缺逐成员证据，不按截图补实现。

---

## 5. T3 执行链与配置数据流重审

### T3.1 ｜ FR-04 Run→Callback→Result→Stop/Cancel→Dispose 状态机 ｜ `completed`
- 单一问题：回调进入、结果缓存、清结果、通知顺序、资源完成边界的原厂顺序。
- 原厂身份/输入：原厂执行生命周期成员；当前 `GraphExecutor`、`WorkflowExecution`、`ToolExecutionScope`、`SubprogramCallWindow.xaml.cs`。
- 允许修改：只在 E1/E3 证据支持时改执行合同；Codex 的 Stop 修复不回滚。
- 依赖/合同：T2.2；重入、停止失败、在途算法、异常订阅者、子流程和 Group 全覆盖。
- 验证命令：C02 回归套件（命令待 T0.3 冻结）。
- 产物：`docs/reverse/T3-execution-config-dataflow-20261112.md` 执行部分 + 状态转移表。
- 退出条件：VS5 失败/停止/释放可重复；关键路径调用链有证据。
- 未知与解除条件：原厂实例同轮回调与 native 资源边界保持 `unknown`。

### T3.2 ｜ FR-05 配置/ROI/Mask/显示/参数提交数据流 ｜ `completed`
- 单一问题：逐参数的读取、编辑、验证、序列化、提交、失败恢复与显示发布链。
- 原厂身份/输入：原厂模板、绑定、命令、事件、参数键；当前 `ToolConfiguration*`、`NodeResultProjection.cs`、`ToolDisplays.cs`。
- 允许修改：只在证据支持时改框架侧公共合同；工具专属转换遵守双项目边界。
- 依赖/合同：T3.1；ROI 集合、Mask 生成、像素缓冲、显示开关均需逐项对应。
- 验证命令：配置/订阅/结果显示回归（命令待 T0.3 冻结）。
- 产物：T3 报告配置部分 + 参数数据流矩阵。
- 退出条件：每项公共交互合同有来源、实现、测试与至少一个真实工具消费。
- 未知与解除条件：原厂二进制载荷与像素缓冲未闭合；缺失 fixture 不伪造。

---

## 6. T4 首条高保真垂直切片与门禁

### T4.1 ｜ FR-06 ImageSource → 证据最完整真实工具垂直切片 ｜ `completed`
- 单一问题：配置→ROI/显示→执行→结果→停止→保存重开→释放能否在原厂证据链上闭合。
- 原厂身份/输入：ImageSource 与选定工具的原厂成员、资源、输入输出；固定样图与参数（T0.3 冻结）。
- 允许修改：切片范围内框架与工具适配文件，单一提交所有者。
- 依赖/合同：T2、T3；至少复用同一公共执行与配置合同，不建第二套。
- 验证命令：正例、负例、边界、重复、保存重开、停止、释放全套（命令随切片卡冻结）。
- 产物：`docs/reverse/T4-first-fidelity-slice-20261112.md` + 回归产物身份。
- 退出条件：闭环可复跑，且每项标注 E0–E5 与保真度状态。
- 未知与解除条件：匹配原厂宿主环境缺失时，不把独立实现写成 `parity-closed`。

### T4.2 ｜ CHECK-FR 保真度门禁判定 ｜ `completed`（未通过，冻结扩展）
- 单一问题：FR0–FR4 是否达到 `source-shaped`，能否解除「冻结扩展工具 UI」。
- 原厂身份/输入：T2/T3/T4 矩阵与门禁 `fidelityCorrectionBeforeExpansion`。
- 允许修改：只更新保真度账本与目标状态。
- 依赖/合同：T4.1；`independent-contract` 不得写成 `parity-closed`。
- 验证命令：门禁复核清单逐项打勾 + 复跑命令回执。
- 产物：更新后的 `docs/reverse/VM-fidelity-ledger-20261112.md` 与判定记录。
- 退出条件：五个最大差异各有处理决定；未达标的区域继续冻结。
- 未知与解除条件：未通过前不进入 T5.3 工具批次扩展。

---

## 7. T5 缺口取证、原厂差分、续作与发布

### T5.1 ｜ 缺失原件与 fixture 取证 ｜ `completed`
- 单一问题：第 8 节登记的每个缺失原件能否取得真实副本与哈希。
- 原厂身份/输入：缺失清单（见第 8 节）。
- 允许修改：只把真实原件加入证据目录并登记身份；**不生成替代资产**。
- 依赖/合同：需用户提供原件或可用原厂环境。
- 验证命令：取得后逐件 `Get-FileHash -Algorithm SHA256` 与路径登记。
- 产物：原件身份清单与解除记录。
- 退出条件：每项有「已取得 / 仍缺 + 解除条件」。
- 未知与解除条件：未取得前相关项保持 `unknown`，不以独立实现替代。

### T5.2 ｜ G8 原厂宿主与直接 SDK 首处分歧 ｜ `completed`（结果仍 `blocked` / `unknown`）
- 单一问题：Run 前像素、活动 ROI/Mask、结果向量与错误码的第一处可重复差异。
- 原厂身份/输入：匹配原厂宿主、SDK、`test1108.sol`、固定样图与参数哈希。
- 允许修改：只在定位并证实差异后做最小修改。
- 依赖/合同：T5.1；宿主与 SDK 各独立运行至少两次。
- 验证命令：同输入差分脚本（随原件冻结）。
- 产物：首处分歧报告与归属（公共框架 / 工具专属）。
- 退出条件：指向可重复差异、准确调用边与证据。
- 未知与解除条件：`test1108.sol` 与匹配宿主缺失，保持 `unknown`，不阻塞不依赖它的静态取证。

### T5.3 ｜ G9 工具批次续作 ｜ `blocked`（CHECK-FR 未通过）
- 单一问题：下一批有源码证据、能真实消费公共合同的工具族是哪些。
- 原厂身份/输入：`docs/ALL-TOOLS-IMPLEMENTATION-STATUS.json`、各工具报告、T4.2 门禁结论。
- 允许修改：按批次的框架/工具适配文件，单一提交所有者。
- 依赖/合同：**T4.2 通过才允许扩展**；设备/动态 IO 维持「运行未确认」。
- 验证命令：每批插件自审 + 生产宿主门禁 + 保存重开。
- 产物：批次退出矩阵。
- 退出条件：每批至少一个真实主窗口路径，覆盖配置、运行、停止、结果、保存重开。
- 未知与解除条件：162 个目录清点不计源码闭合，不宣称全量完成。

### T5.4 ｜ 工作区链路与保真度线合流验收 ｜ `completed`
- 单一问题：`31acdb4c` 的工作区链路（画布编辑→执行→右侧图像/模块结果→底部当前/历史/帮助）与 Codex 保真度回归是否在同一基线上互不回退。
- 原厂身份/输入：`ModuleRunLedger.cs`、`NodeResultProjection.cs`、`MainWindow.WorkspaceProjection.cs`、`MainWindow.ModuleTree.cs`、`ToolDisplays.cs`、`WorkflowCanvas.cs`；Codex 对其中前两项与画布的后续修改。
- 允许修改：仅修复合流回退，须有调用链证据与回归。
- 依赖/合同：T0.2、T3.1、T3.2。
- 验证命令：`NodeProjectionChecks`、`VisionEditor.WorkspaceProjectionChecks`、`VisionEditor.ShellMenuChecks`、`VisionEditor.ShellChecks`。
- 产物：合流差异清单与回归结果。
- 退出条件：节点级结果、历史行 `RowIndex/AddTime/Content`、树形模块结果、帮助页、菜单数据驱动全部仍可复跑。
- 未知与解除条件：节点边框 `Path + NormalModulePathStyle`、状态角标真实资源、参数窗口字段级对齐仍为独立近似，不因合流改称原厂等价。

### T5.5 ｜ 全量回归、人工验收与提交决策 ｜ `completed`（提交/人工验收留用户决定）
- 单一问题：选定范围是否满足总方案 §12 退出声明，以及是否提交/推送。
- 原厂身份/输入：全部任务卡退出矩阵、身份审计、构建产物哈希、回滚点。
- 允许修改：最终报告；提交与推送**只在用户明确要求时执行**。
- 依赖/合同：T5.3、T5.4；人工验收需用户明确要求才在 `WinSta0\Default` 启动。
- 验证命令：全量无界面回归 + 身份审计 + 关键切片主窗口验收。
- 产物：最终交付报告（含 5 项固定内容）、逐工具退出矩阵、已知限制。
- 退出条件：S0–S7 在选定范围内达到退出条件，未知项均有边界。
- 未知与解除条件：人工验收与原厂差分状态独立记录，不参与完成度分母。

---

## 8. 缺失原件登记（解除条件）

| 原件 / 资产 | 受影响任务 | 当前状态 | 解除条件 |
|---|---|---|---|
| `test1108.sol`、匹配原厂宿主环境 | T5.2、G8 | `unknown` | 用户提供原件与可运行宿主 |
| `CircleFind.sol` | T1.3、T3.2 | `unknown` | 原厂方案样本 |
| `Modules/*/ToolGroup.xml` 真实 `PriorityStrategyInfo` | T1.2 | `unknown` | 取得原件后只补元数据并复跑 |
| `solver-fixture/frame-0.bmp` | T5.1、G9 Board/Map | `blocked` | 真实 fixture |
| `original-board fixture`、`distortion fixture.bmp` | T5.1 | `blocked` | 真实 fixture |
| `reader-8.xml`、Map 图像结果 JSONL | T5.1 | `blocked` | 真实资源 |
| CameraMapping 原厂 XML、solver trace | T5.1 | `blocked` | 原厂资源与 trace |
| GrayMatch / native solver exact oracle | T5.3 | `unknown` | 原生运行证据 |
| 真实设备、相机、认证服务状态 | T2.2、T5.2 | `unknown` | 匹配原厂运行环境 |
| WpfChecks 第 104 项后流程 | T0.2 | `unknown` | 补齐 ImageSource 节点后复跑 |

## 9. 待冻结的复跑命令

以下两条来自 Codex 交接文档，**CodeBuddy 未复跑**，T0.2 先验证：

```powershell
dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore
dotnet run --project tests/ToolCatalogLifecycleChecks15/ToolCatalogLifecycleChecks15.csproj -c Release -- src/VisionEditor
```

其余套件（`NodeProjectionChecks`、`VisionEditor.WorkspaceProjectionChecks`、`VisionEditor.ShellMenuChecks`、`VisionEditor.StartupShellChecks`、C02/C03/P6 各回归）由 T0.3 逐一冻结 `csproj` 路径、参数、预期产物后再填入；**未冻结前不臆造命令**。

## 10. 状态更新规则

1. 任务卡只有在「原厂证据 + 实现设计 + 实现 + 自审 + 规定测试 + 报告」六项齐备后才改 `completed`；测试数量不单独闭合任务。
2. 每次推进只更新被实际执行的那一项，不批量勾选，不把历史项挂到新计划下。
3. 遇到首个失败先记录失败点、证据与解除条件，不通过增加测试数量掩盖。
4. 保真度状态与任务状态分开记录；`completed` 不等于 `parity-closed`。
5. 每轮报告同时给出：变更与责任边界、证据路径与等级、命令与结果、未确认项与解除条件、更新后的账本与完成度百分比。
6. 检查点：C1 = T0+T1 完成；C2 = T2+T3 完成；C3 = T4 门禁通过。未过检查点不进入下一阶段。

## 11. 当前完成度（不改变）

- 授权第一阶段权威口径：**10/10 = 100%**（`docs/SOURCE-LEVEL-REVERSE-PROGRESS.json`）。
- 框架实施口径：**42/96 = 43.75%**（`docs/framework-progress-audit.json`）。
- 本轮只完成 PLAN-00，未改变任一完成度；G1–G9 门禁不计入分母。
