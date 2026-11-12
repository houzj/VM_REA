# CodeBuddy 接手计划：VisionMaster 高保真逆向（2026-11-12）

## 0. 文档定位

本文是 **CodeBuddy 侧的唯一接手基线**，用于把 Codex 会话在同一工作树留下的工作接入 CodeBuddy 继续执行。

- 用户本轮指令：**「制定计划和任务清单，把工作从 codex 接入到 codebuddy 继续」**。
- 本轮只产出计划与任务清单，**未执行任何逆向、构建、回归或提交动作**。
- 本文不改写、不删除、不勾选 Codex 留下的 `tasks/plan.md`、`tasks/todo.md`、`docs/CODEBUDDY-HANDOFF-20261112.md`；它们作为历史快照保留，其状态字段不再作为执行依据。
- 状态与任务以 `docs/CODEBUDDY-TAKEOVER-TASKS-20261112.md` 为准；计划设计依据为 `docs/VM-COMPLETE-REVERSE-IMPLEMENTATION-PLAN-20261112.md`；门禁依据为 `docs/VM-REVERSE-GATE-MATRIX-20261112.json`。

## 1. 接手对象与来源识别

同一工作树 `d:/MyWork/Rea逆向` 上存在两个来源的产出，必须分开记账：

| 来源 | 标识 | 内容 | Git 状态 |
|---|---|---|---|
| CodeBuddy 上一轮 | 提交 `31acdb4c` | 工作区执行链路：模块级账本、节点数据投影、结果树、历史结果、帮助页、原厂菜单数据驱动、节点 110×30 与状态角标、A05/W01 契约与回归 | 已提交并推送，`origin/main` 同值 |
| Codex 本批 | `docs/CODEBUDDY-HANDOFF-20261112.md`（14:33:57 写入）及同批文件 | G1–G9 门禁体系、保真度账本、执行目标、实施总方案、44 份 `docs/reverse/*20261112` 报告、17 个已跟踪文件修改、新增源码/测试 | **全部未提交**；CodeBuddy 未复跑、未验收 |

Codex 批次的已跟踪修改（`git diff --stat`）：17 文件、`+187 / -48`，覆盖 `ModuleRunLedger.cs`、`NodeResultProjection.cs`、`WorkflowCanvas.cs`、`SubprogramCallWindow.xaml.cs`、`ToolNavigation.cs`、六个标定配置提供者、`ToolConfigurationCalibrationPathPolicy.cs` 及四套测试。这批改动建立在 `31acdb4c` 之上，即 CodeBuddy 的已提交链路被 Codex 继续修改，接手时**不得回滚**。

新增条目按目录：`docs/reverse` 44、`src/VisionEditor` 4、`tests/VisionEditor.StartupShellChecks` 1、`tasks/` 2、`docs/VM-*` 4。`src/VisionEditor/tool-lifecycle-*/` 两个哈希目录（共 3 文件）来源未标注，T0 需判定是临时产物还是交付物。

## 2. 基线冻结（T0 输入）

| 项 | 当前值 | 冻结动作 |
|---|---|---|
| HEAD | `31acdb4c` | 记录并写入接手清单 |
| 远程 | `origin/main = 31acdb4c` | 推送前重新 `git fetch` 检查分叉 |
| 工作树 | 17 `M` + 55 个未跟踪条目（接手时点 `git status --porcelain -uall`，不含本文与任务清单） | 逐文件登记责任来源，不做静默整合 |
| 原厂静态证据 | `analysis/decompiled`、`analysis/full-audit` | 二者被 `.gitignore` 的 `analysis/*` 排除，**不随提交进入远程**；接手结论不可仅靠 Git 复原 |
| 门禁产物 | `analysis/full-audit/fidelity/20261112/*.json` 共 44 份 | 同上；T0 计算哈希并登记，CodeBuddy 复跑前不引用其数值为结论 |
| UI 资产 | `analysis/ui-assets/**`、`analysis/images/circle-find-sample.bmp` | 已在 `31acdb4c` 跟踪 |

## 3. 必须校正的三处口径偏差

1. **证据路径错误**（交接文档第 22–23 行）：写的是 `D:/MyWork/REA逆向测试/analysis/decompiled` 与 `.../full-audit`，实测两个目录均不存在。真实原厂证据在本工作树 `analysis/decompiled` 与 `analysis/full-audit`；已确认 `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs` 可读。T1 在接手基线中统一为工作树内相对路径，并修正引用处，不改变任何行为。
2. **进度口径并存四套，禁止混用**：
   - 交接文档写「框架 41/96（42.71%），工具 16/162（9.88%）」——历史快照；
   - `docs/framework-progress-audit.json` 当前为 **42/96 = 43.75%**（`implementation-self-review-v1`，96 单元）；
   - `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` 当前为 **10/10 = 100%**（用户指定的授权第一阶段标定工具口径）；
   - 128 个 stage gates 属内部维度（25.78%），不作对外完成率。
   权威口径：对外总完成度按 AGENTS.md 最新覆盖规则用 **10/10 = 100%**，框架实施口径单列 **42/96 = 43.75%**，人工验收不计入。G1–G9 门禁完成数**不得**并入上述任一完成率。
3. **交接文档「下一步」第 1 项已部分完成**：`docs/reverse/C01-toolbox-type-priority-chain-20261112.md` 已按原厂 `GuideWindow.xaml.cs:128`、`ToolBoxData.cs:231-263`、`ToolBoxItemData.cs:222`、`app.config:56` 完成解析与消费，并明确「仓库缺 `Modules/*/ToolGroup.xml` 真实 `PriorityStrategyInfo` 数值，保持运行未确认」。因此 T1 的批量索引重定位到：Guide 卡片配置写入与首次工具箱加载入口、方案实体成员链、`MainWindow` 与 `MainWindowData`/HostShell 对应矩阵；不重做工具箱解析。

## 4. Codex 交付的登记方式与验收状态

下列状态**均为 Codex 自述 + 门禁 JSON 存在**，CodeBuddy 侧状态一律为「未复跑」，不得写成「已验证」或「VM 等价」：

| 目标 | Codex 自述状态 | 门禁产物 | CodeBuddy 验收 |
|---|---|---|---|
| G1 保真度账本 | `completed-for-planning`，15 区域、0 `parity-closed` | `g1-completion.json` | 未复跑 |
| G2 启动/方案实体 | `source-shaped` | `g2-completion.json` | 未复跑 |
| G3 执行/停止/结果/释放 | `source-shaped-with-runtime-open` | `g3-completion.json` | 未复跑 |
| G4 配置/ROI/Mask/显示 | `source-shaped-with-runtime-open` | `g4-completion.json` | 未复跑 |
| G5 ImageSource→真实工具切片 | `source-shaped-with-native-runtime-open` | `g5-completion.json` | 未复跑 |
| G6 保存/重开/错误恢复 | `source-shaped-with-native-runtime-open` | `g6-completion.json` | 未复跑 |
| G7 Group/子流程 | `source-shaped-with-native-runtime-open`；另有停止恢复修复 | `g7-*.json` | 未复跑 |
| G8 原厂宿主首处分歧 | `runtime-open`（**未闭合**） | `g8-completion.json` | 未复跑 |
| G9 工具批次 | 多批 `batch-self-reviewed-native-runtime-open`；CameraMapping、GrayMatch Oracle 等保持证据开放 | `g9-*.json` | 未复跑 |
| C01/C02/P6 回归修复 | 17 文件修改及多份回归报告 | `c01-*.json`、`c02-*.json`、`p6-*.json` | 未复跑 |

`tasks/todo.md` 中 FR-02～FR-06 与 CHECK-FR 未勾选、`tasks/plan.md` 全部任务卡未勾选，却在同一批叙述了大量 G2–G9 结果——这是**状态与叙述脱节**，不是「全部未做」也不是「全部完成」。T0 必须逐项重新定状态。

## 5. 明确保持 unknown / 运行未确认的边界

按门禁 `noEvidenceAction`：无证据不改行为，只登记缺失证据与解除条件。

- 原厂 `.sol` 容器字段、SDK 自动加载、场景直启；`test1108.sol`、`CircleFind.sol` 缺失。
- 匹配原厂宿主 Blob 的 Run 前缓冲、活动 ROI/Mask 与首处分歧（历史 `45/971486`、`47/973202` 仅为静态线索）。
- 真实设备、相机、认证服务、原厂同机桌面/DPI 运行状态。
- native 求解器、动态 IO、ABI 寄存器/栈/虚表深层证据。
- 缺失原件：`solver-fixture/frame-0.bmp`、`original-board fixture`、`distortion fixture.bmp`、`reader-8.xml`、Map 图像结果 JSONL、CameraMapping 原厂 XML、solver trace、`ToolGroup.xml` 真实优先级数值。
- `WpfChecks` 在 104 项后因测试流程缺 ImageSource 节点停止。
- Guide/主窗口视觉与状态差异为 `divergent`，新修正版尚未人工复核。
- `ToolGroup.xml` 四类排序值不得写死；取得原件后只补元数据并复跑，不改解析算法。

## 6. 接手后阶段路线

| 阶段 | 目标 | 主要输入 | 唯一交付 | 退出条件 |
|---|---|---|---|---|
| T0 | 交付验收与基线冻结 | 未提交 diff、44 份门禁 JSON、原厂证据根 | `docs/reverse/T0-codex-takeover-acceptance-20261112.md` + 哈希清单 | Release 构建与指定回归在 CodeBuddy 侧重跑成功；逐文件责任与状态登记完成；无行为修改 |
| T1 | 口径校正与批量证据索引 | 交接文档、`tasks/*`、原厂反编译树 | `docs/reverse/T1-evidence-index-20261112.md` | 证据路径、进度口径、状态词汇统一；三类索引各含证据路径、成员/RVA、观察、未知、复跑方法 |
| T2 | FR-02/FR-03 对照矩阵与启动链重审 | 原厂 XAML/CIL、当前启动与方案代码 | `docs/reverse/T2-startup-solution-matrix-20261112.md` | 启动→Guide→空载主窗口→关闭、新建/打开方案共享同一实体；`.sol` 未知字段保持不透明 |
| T3 | FR-04/FR-05 执行链与配置数据流重审 | 原厂 Run/Callback/Result/Stop/Dispose、配置与 ROI 成员 | `docs/reverse/T3-execution-config-dataflow-20261112.md` | 逐成员列出字段读写、分支谓词、失败传播、释放与持久化边界；缺口标注 unverified |
| T4 | FR-06 首条高保真垂直切片 + CHECK-FR | T2/T3 结论、ImageSource→证据最完整真实工具 | `docs/reverse/T4-first-fidelity-slice-20261112.md` + 回归产物 | 配置→ROI/显示→执行→结果→停止→保存重开→释放可复跑；门禁判定通过才允许扩展工具批次 |
| T5 | 缺口取证、原厂差分、G9 续作与发布 | 缺失原件清单、匹配原厂环境 | 分批退出矩阵 + 最终验收报告 | 每项有 E0–E5 状态、正/负/边界/重复/保存重开命令；人工验收与提交时点由用户决定 |

阶段纪律：**先 T0/T1 再动任何实现**；T4 未通过 CHECK-FR 前不新增工具 UI、不扩大工具目录；匹配原厂环境缺失时只把依赖它的项标记 `unknown`，不停止不依赖原厂的静态取证。

## 7. 任务卡规范

沿用总方案 §7 固定格式，每张卡必须含：ID / 阶段 / 单元 / 唯一负责人、单一源码问题、原厂目标与身份（E0，缺则写「待 T0 冻结」）、输入与参数、入口成员与调用边、字段读写与生命周期、当前分类、允许修改的文件与目录、依赖合同与跨项目请求、实现 seam、正负/边界/重复/保存重开测试、验证命令与产物路径、退出条件、剩余未知与解除条件。

**缺原件身份、入口、退出条件或复跑命令的任务卡不得进入实施。** 细粒度任务卡见 `docs/CODEBUDDY-TAKEOVER-TASKS-20261112.md`。

## 8. 证据与门禁

- 等级 E0 身份 → E1 源码链 → E2 PE/ABI → E3 受控运行 → E4 独立回归 → E5 主机验收；关键路径最低要求 E0+E1+E3+E4+E5。
- 单元状态 S0–S7；**S6 通过不自动关闭 S7**。
- 垂直切片 VS0–VS8；纠偏切片 FR0–FR4 未达 `source-shaped` 前不扩展更多工具 UI。
- 只有 `parity-closed` 可称原厂等价；测试数量、截图、用户反馈只能登记差异，不能授权语义。
- 门禁规则以 `docs/VM-REVERSE-GATE-MATRIX-20261112.json` 的 `implementationRule` 与 `noEvidenceAction` 为准。

## 9. 双项目边界与提交纪律

- 按 `docs/PARALLEL-WORK.md` 顶部最新覆盖规则：所有通用框架层（含插件面向的公共配置/交互/会话合同）归 `main`；`tool-plugins` 只负责工具专属源码与实现。
- 不修改 `tool-plugins` 工作树，不复制其未提交代码，不静默整合；跨项目接口写入 `docs/CONTRACT-REQUESTS.md`（现有最新为 FCR-038）。
- 同一交叉文件只设一位提交所有者；共享文件变更需登记生产者、消费者、载荷、生命周期、兼容性与验收证据。
- **提交与推送时点由用户决定**。接手阶段不自动提交 Codex 的未提交改动；如用户要求提交，先重跑 T0 验收并列出全部 17 `M` + 新增条目。
- 不删除历史证据、失败记录与未确认项；不用 `reset` 清理工作树。

## 10. 每次交付的报告格式

按交接文档 §125 与总方案 §10，每次交付同时给出：

1. 变更文件清单与责任边界（含是否提交、HEAD）；
2. 每项变更对应的原厂证据路径与证据等级（E0–E5）；
3. 实际执行的构建、测试命令与结果；
4. 未确认行为、阻塞条件与最小解除条件；
5. 更新后的保真度账本、目标状态与总体完成度百分比（注明分母口径）。

## 11. 风险登记

| 风险 | 处理 |
|---|---|
| Codex 未提交交付与 `31acdb4c` 交织 | T0 逐文件登记、先验收后动；不回滚、不静默提交 |
| 门禁 JSON 位于被忽略的 `analysis/` | 本地哈希登记；远程不可移植，报告必须注明 |
| 两套历史任务清单状态失真 | 冻结为历史快照，改用接手任务清单；不再勾选旧文件 |
| 交接文档证据路径与进度值过时 | T1 统一口径，保留原文不改历史叙述 |
| 测试通过被误读为 VM 等价 | 维持 `independent-contract` / `source-shaped`，差分单独记录 |
| 缺失原件被独立实现补齐 | 保持 `unknown`，列解除条件；不伪造资源 |
| 构建产物被占用 | 独立 `OutDir`、`UseSharedCompilation=false`、`BuildInParallel=false`；不终止其他任务进程 |
| 人工验收被自动化替代 | 需用户明确要求时才在 `WinSta0\Default` 启动，单独记录 |

## 12. 当前完成度（本轮口径）

- 授权第一阶段（用户指定权威口径）：**10/10 = 100%**，`docs/SOURCE-LEVEL-REVERSE-PROGRESS.json`。
- 框架实施口径：**42/96 = 43.75%**，`docs/framework-progress-audit.json`（`implementation-self-review-v1`）。
- 本轮只完成「计划与任务清单」，**不改变上述任何数值**，也不把 G1–G9 门禁数计入完成率。
