# T0 Codex 交付验收与基线冻结（2026-11-12）

执行：CodeBuddy ｜ 依据：`docs/CODEBUDDY-TAKEOVER-TASKS-20261112.md` T0.1–T0.4 ｜ 门禁：`docs/VM-REVERSE-GATE-MATRIX-20261112.json`

## 1. 基线身份

| 项 | 实测值 |
|---|---|
| HEAD | `31acdb4c5b1f4418b514fa729370a78ec77e0477` |
| `origin/main` | 同值（无分叉） |
| 已跟踪修改 | 17 文件，`+187 / -48` |
| 未跟踪条目 | 接手时点 55，T0 执行后新增 2 份接手文档 + 1 个测试临时目录 |
| 工作树状态 | 全部未提交；本阶段未提交、未推送 |
| 行为修改 | **无**（T0 只做只读验收、哈希登记与报告落地） |

## 2. 逐文件责任清单（T0.1）

### 2.1 已跟踪修改 17 项（来源：Codex，均未提交）

| 文件 | 责任 | 证据关联（批次级） |
|---|---|---|
| `src/VisionEditor.Core/ModuleRunLedger.cs` | Codex | C02 结果投影/执行回归 |
| `src/VisionEditor.Core/NodeResultProjection.cs` | Codex | C02 结果投影/执行回归 |
| `src/VisionEditor/WorkflowCanvas.cs` | Codex | C01 启动崩溃回归（MouseLeave 安全查找） |
| `src/VisionEditor/SubprogramCallWindow.xaml.cs` | Codex | G7 Group 停止恢复 |
| `src/VisionEditor/ToolNavigation.cs` | Codex | C01 工具箱类型优先级链 |
| `src/VisionPluginContracts/ToolConfigurationCalibrationPathPolicy.cs` | Codex | P6 标定路径候选值保留 |
| `src/VisionPlugins.Calibration/CalibrationToolConfigurationProvider.cs` | Codex | P6 同上 |
| `src/VisionPlugins.Calibration/NImageConfiguration.cs` | Codex | P6 同上 |
| `src/VisionPlugins.Calibration/NPointConfiguration.cs` | Codex | P6 同上 |
| `src/VisionPlugins.CalibrationBoard/CalibrationBoardConfiguration.cs` | Codex | P6 同上 |
| `src/VisionPlugins.DistortionCalibration/DistortionCalibrationConfiguration.cs` | Codex | P6 同上 |
| `src/VisionPlugins.MapCalibration/MapCalibrationConfiguration.cs` | Codex | P6 同上 |
| `tests/ToolCatalogLifecycleChecks15/Program.cs` | Codex | General/Position 合成元数据验证 |
| `tests/VisionEditor.ShellHistoryChecks/Program.cs` | Codex | C02 工作区参数回归（工具栏-only Undo/Redo） |
| `tests/VisionEditor.StartupShellChecks/Program.cs` | Codex | G2 启动/方案实体门禁、C01 启动崩溃 |
| `docs/plugins/n-image-solver-verification.json` | Codex | P6 NImage 插件自审 |
| `examples/workflow-data-bindings.vmx.json` | Codex | C01 图像源流程回归 |

> 关联列来自 `tasks/todo.md` 的批次叙述，为**批次级关联，未逐行核对 diff 与报告的成员对应关系**；不作为「已验证」结论。逐成员归属留 T2/T3 重审。

### 2.2 未跟踪条目（接手时点 55）

| 分组 | 数量 | 内容 | 处置 |
|---|---|---|---|
| `docs/reverse/*20261112.md` | 44 | C01/C02/P6/G2–G9 批次报告、保真度账本 | 历史快照，保留 |
| `docs/VM-*` + 门禁 JSON | 4 | 总方案、执行目标、门禁矩阵 | 保留；门禁按 T1.1 校正口径 |
| `docs/CODEBUDDY-HANDOFF-20261112.md` | 1 | Codex 交接文档 | **冻结**，不勾选、不改写 |
| `tasks/plan.md`、`tasks/todo.md` | 2 | Codex 计划与任务清单 | **冻结**，状态与叙述脱节，不作执行依据 |
| `src/VisionEditor/ToolboxPriorityStrategy.cs` | 1 | 工具箱优先级解析 | 保留，T1.2 衔接不重做 |
| `tests/VisionEditor.StartupShellChecks/StartupCrashRegression.cs` | 1 | 启动崩溃回归 | 保留 |
| `src/VisionEditor/tool-lifecycle-*/` | 2 目录 / 3 文件 | 测试运行时临时产物 | 见 `T0-tool-lifecycle-verdict-20261112.md`，不自行删除 |

CodeBuddy 本轮新增：`docs/CODEBUDDY-TAKEOVER-PLAN-20261112.md`、`docs/CODEBUDDY-TAKEOVER-TASKS-20261112.md`（上一轮），以及本次 T0 四份产物。

## 3. 复跑验收结果（T0.2）

命令、参数、退出码与日志见 `docs/reverse/T0-replay-command-inventory-20261112.md`。

| 检查 | 结果 |
|---|---|
| `dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore -p:UseSharedCompilation=false -p:BuildInParallel=false` | **0 警告 / 0 错误**，退出码 0 |
| `ToolCatalogLifecycleChecks15` | **19/19 通过**，退出码 0 |
| `NodeProjectionChecks` | 54 项通过，退出码 0 |
| `VisionEditor.WorkspaceProjectionChecks` | 79 项通过，退出码 0 |
| `VisionEditor.ShellMenuChecks` | 65 项通过，退出码 0 |
| `VisionEditor.StartupShellChecks` | 启动/Guide/宿主数据/关闭链通过，退出码 0 |
| `VisionEditor.ShellHistoryChecks` | 通过，退出码 0 |
| `VisionEditor.ShellChecks` | **失败**，退出码 `-532462766`（0xE0434352 未处理异常） |

### 3.1 首个失败（按纪律不掩盖）

- 位置：`tests/VisionEditor.ShellChecks/Program.cs:275`，检查函数在 `Program.cs:48`。
- 断言：`Save menu persists and orders recent solution paths`（期望最近方案菜单 2 项且顺序为 `recentSecond, recentFirst`）。
- 现场：同一日志前序 PASS 行显示 `标头:最近打开方案 Items.Count:3`。
- 日志：`artifacts/codebuddy-verify/t0-VisionEditor.ShellChecks.log`（首个异常位于第 45 行）。
- 处置：T0 **不修改**产品代码或测试夹具。按 `noEvidenceAction` 登记为待查差异，归入 T2（方案/最近列表）与 T5.4（合流验收）；解除条件为取得原厂最近列表条目数、排序与持久化语义的 E1 证据，再决定修产品或修夹具。
- 未做：未通过增加测试数量、跳过断言或改夹具掩盖失败；未判定该失败由哪一次提交引入（需 `git bisect` 或逐提交复跑，留待有证据时执行）。

## 4. 门禁产物登记（T0.3）

- 产物：`docs/reverse/T0-gate-artifact-hashes-20261112.json`，44 份门禁 JSON 的 SHA-256、`goal`、`declaredStatus`、`nextGoal`。
- `codebuddyVerification` 全部为「未复跑」：本阶段复跑的是构建与回归套件，**没有**逐一复跑 44 份门禁 JSON 对应的原生探针与差分流程；不把回归套件通过等同于门禁闭合。
- 声明状态样例：`partial-source-shaped-evidence-open`、`source-shaped-runtime-open`、`batch-self-reviewed-native-runtime-open`、`completed-for-planning`。
- 这些文件位于被 `.gitignore` 的 `analysis/*` 下，**远程不可复原**；后续报告必须显式注明。

## 5. 临时目录判定（T0.4）

见 `docs/reverse/T0-tool-lifecycle-verdict-20261112.md`：三个 `tool-lifecycle-<hash>/` 目录均为 `ToolCatalogLifecycleChecks15` 运行时生成，非交付物；不自行删除，是否清理或加忽略规则由用户决定。

## 6. 退出条件核对

| T0 退出条件 | 结果 |
|---|---|
| Release 构建与指定回归在 CodeBuddy 侧重跑 | 已完成（含 1 项失败如实登记） |
| 逐文件责任与状态登记完成 | 已完成（17 `M` + 55 `??` 分组） |
| 44 份门禁哈希与声明状态登记 | 已完成 |
| 临时目录判定 | 已完成 |
| 无行为修改 | 已确认（仅新增 `docs/reverse/T0-*`、任务台账状态与计划文档计数订正） |

## 7. 未确认项与解除条件

- `VisionEditor.ShellChecks` 首个失败的根因与引入提交：需原厂最近列表证据 + 逐提交复跑。
- 44 份门禁 JSON 的实际复跑：需 T1.3/T3/T5 冻结各自探针命令后执行。
- 17 个 `M` 的逐成员原厂证据：T2/T3 重审，当前仅批次级关联。
- 原厂 `.sol`、native ABI、匹配宿主环境：保持 `unknown`，不影响 T0 结论。
