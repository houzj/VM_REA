# CodeBuddy 接手执行最终交付（2026-11-12）

范围：`docs/CODEBUDDY-TAKEOVER-PLAN-20261112.md` T0–T5 ｜ 计划：`codebuddy-takeover-execution` ｜ 全部 7 项计划任务已执行完毕。

## 1. 变更文件与责任边界

### 1.1 新增证据与报告（12 份）

| 阶段 | 文件 |
|---|---|
| T0 | `docs/reverse/T0-codex-takeover-acceptance-20261112.md`、`T0-gate-artifact-hashes-20261112.json`、`T0-replay-command-inventory-20261112.md`、`T0-tool-lifecycle-verdict-20261112.md` |
| T1 | `docs/reverse/T1-evidence-index-20261112.md` |
| T2 | `docs/reverse/T2-startup-solution-matrix-20261112.md` |
| T3 | `docs/reverse/T3-execution-config-dataflow-20261112.md` |
| T4 | `docs/reverse/T4-first-fidelity-slice-20261112.md` |
| T5 | `docs/reverse/T5-original-asset-gap-20261112.md`、`T5-first-divergence-20261112.md`、`T5-workspace-merge-acceptance-20261112.md`、`T5-final-delivery-20261112.md` |

日志与产物：`artifacts/codebuddy-verify/*.log`（构建、回归、首个失败堆栈；目录已被 `.gitignore` 忽略）。

### 1.2 修改的既有文件（5 份）

| 文件 | 改动 | 责任 |
|---|---|---|
| `docs/CODEBUDDY-TAKEOVER-PLAN-20261112.md` | §6 表格「46 份门禁 JSON」订正为 44 份 | CodeBuddy 接手基线 |
| `docs/CODEBUDDY-TAKEOVER-TASKS-20261112.md` | T0.1–T5.5 任务卡与 UI 映射表按实际执行逐项更新状态 | CodeBuddy 接手基线 |
| `docs/reverse/T1-evidence-index-20261112.md` | §5.2 订正「原厂安装不可访问」误判，补入本机 E0 复核 | CodeBuddy 本报告 |
| `docs/reverse/VM-fidelity-ledger-20261112.md` | 追加「T4 CHECK-FR 判定」章节；上方历史账本原文保留 | Codex 原文件 + CodeBuddy 追加 |
| `docs/VM-EXECUTION-GOALS-20261112.md` | 追加「接手执行后目标状态」章节；原文保留 | Codex 原文件 + CodeBuddy 追加 |

### 1.3 明确未改动

- **产品代码零修改（本轮）**：CodeBuddy 未改动 `src/**` 任何文件；`git status` 中 `src` 下 12 项 `M` 均为接手前 Codex 留下的未提交修改，保持原样。T2/T3 判定「重写 0 项」，按 `noEvidenceAction` 无证据不改行为。
- **测试夹具零修改**：`VisionEditor.ShellChecks` 未放宽断言、未跳过检查。
- 未改 `AGENTS.md`、`.gitignore`、`docs/PARALLEL-WORK.md`、`docs/PLUGIN-FRAMEWORK-BOUNDARY.md`、`docs/CONTRACT-REQUESTS.md`、`worktrees/tool-plugins/`。
- Codex 历史快照 `docs/CODEBUDDY-HANDOFF-20261112.md`、`tasks/plan.md`、`tasks/todo.md` 未改、未勾选、未删除。
- **未提交、未推送**；HEAD 仍为 `31acdb4c`，`origin/main` 同值。提交时点由用户决定。
- 未删除 `src/VisionEditor/tool-lifecycle-*/` 三个测试临时目录（是否清理由用户决定）。

## 2. 原厂证据路径与证据等级

| 等级 | 证据 | 路径 / 标识 |
|---|---|---|
| E0 身份 | 原厂安装原件 | `C:/Program Files/VisionMaster4.4.0/Applications/VisionMaster.exe`（`CF5730DF…D291`）、`iMVS-6000PlatformSDK.dll`（`F5129789…2A4E`）、`VMControls.Data.dll`（`8A9F45E1…CA50`，MVID `37aef1a6-0fe9-4ded-a35a-8b1d8e2ecdae`） |
| E0 身份 | 切片输入 | `circle-find-sample.bmp`（`690B1733…BA`）、`architecture-demo.vmx.json`（`D718BC61…FF`）、`image-source-plugin.vmx.json`（`64BCE234…8C`） |
| E0 身份 | 原厂资源 | 安装根 `ToolGroup.xml` 30 份（x64 15 份，4 份含真实 `priorityStrategyInfo`）；`CircleFind.sol` 3 份同哈希 `B91A7736…D0B` |
| E1 源码链 | 反编译源码 | `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs:196,206-235`、`.../VisionMaster.Project/ProjectManager.cs:166,197,506,900-927`、`.../VisionMaster.AppMainWindow/SolutionItemsData.cs:19-95`、`MainWindowData.cs:41-85`、`VMControls.Data/ToolBoxData.cs:199-261`、`GuideWindow.xaml.cs:124-147` |
| E2 PE/ABI | 归档 + REA | Evidence `ev_1332cc27…`（`inspect_pe_imports`，ModuleFrame SHA `45c57bc4…`）、`ev_8f927177…`（`inspect_managed_artifact`）、`ev_a45d15b7…`（`inspect_managed_members`，2 类型 / 60 方法 / 336 调用边）；C02/C03 归档 VA 与寄存器/栈证据 |
| E3 受控运行 | **本次无原厂宿主运行** | 历史直接 SDK 47/973202（C03）与宿主 45/971486 仅作历史线索 |
| E4 独立回归 | 本轮实跑 | 见第 3 节 |
| E5 主机验收 | **未执行** | 需用户明确要求且在 `WinSta0\Default` |

所有 `analysis/*` 证据被 `.gitignore` 排除，**远程不可移植**；报告以路径 + SHA-256 记录。

## 3. 实际执行的命令与结果

| 批次 | 命令 / 套件 | 结果 |
|---|---|---|
| T0 构建 | `dotnet build src/VisionEditor/VisionEditor.csproj -c Release --no-restore -p:UseSharedCompilation=false -p:BuildInParallel=false` | 0 警告 / 0 错误 |
| T0 回归（8 条） | `ToolCatalogLifecycleChecks15` 19/19、`NodeProjectionChecks` 54、`WorkspaceProjectionChecks` 79、`ShellMenuChecks` 65、`StartupShellChecks`、`ShellHistoryChecks` 通过；**`ShellChecks` 失败**（`-532462766`，`Program.cs:275`） | 7 通过 / 1 失败，失败已留日志与堆栈 |
| T4 切片（5 套） | `VisionFramework.ExecutionChecks` 91、`DebugChecks` 35、`PersistenceChecks` 30、`VisionConfiguration.ExecutionChecks` 39、`VisionWorkflow.CallChecks` 93 | 288 项通过，退出码全 0 |
| T5 合流（4 套） | `NodeProjectionChecks` 54、`WorkspaceProjectionChecks` 79、`ShellMenuChecks` 65、**`ShellChecks` 同一失败** | 3 通过 / 1 仍失败，确认无回退 |
| T5 构建复核 | 同 T0 构建命令 | 0 警告 / 0 错误 |

命令与参数全文见 `docs/reverse/T0-replay-command-inventory-20261112.md`、`T4-first-fidelity-slice-20261112.md` §2、`T5-workspace-merge-acceptance-20261112.md` §1。

限制：`pwsh` 本机不可用，C02 整批 11 套脚本（`#Requires -Version 7.0`）未运行；本轮按脚本清单等价复跑 5 套，未使用其 `--artifacts-path` 独立输出与套件独立 cwd，产物哈希不可与历史 462 项批次直接比对。另有 6 个 C02 套件未复跑，不推断通过。

## 4. 未确认行为、阻塞与解除条件

| 项 | 状态 | 解除条件 |
|---|---|---|
| G8 原厂宿主首处分歧 | `unknown` / `blocked` | `test1108.sol` + 匹配宿主环境 + 两次独立运行 + Run 前缓冲 |
| G9 工具批次续作 | `blocked`（CHECK-FR 未通过） | FR0/FR1/FR2/FR4 达 `source-shaped` 且 FR3 取得 E3 同轮证据 |
| CHECK-FR 门禁 | **未通过**，冻结扩展工具 UI | 见 `T4-first-fidelity-slice-20261112.md` §5 |
| `VisionEditor.ShellChecks` 首个失败 | 开放（T0/T5 复现一致） | 逐次记录 `RecentSolutionStore.Update` 调用点定位第 3 项，再对照原厂 `UpdateRecentlySolutions` 时机决定产品修复或夹具修正 |
| 原厂 `.sol` 字段 / 资源目录 / 场景直启 | `unknown` | 对 `CircleFind.sol` 建立只读探针并取得用户许可 |
| `VMControls.Data.dll` 两构建与反编译源码对应 | `unknown` | `compare_managed_members` 或取得历史 D: 原件 |
| 原厂参数模板、绑定、`IUserBytesData` 二进制载荷 | `unknown` | 逐成员 E1 取证 |
| 活动 ROI/Mask、Run 前像素、native 同轮时序 | `unknown` | 匹配宿主 E3 采集 |
| 原厂设备/认证/同机桌面状态、人工验收 | `unknown` / 未执行 | 用户安排环境与明确验收要求 |
| `solver-fixture/frame-0.bmp`、`fixture.bmp`、`reader-8.xml`、Map JSONL、CameraMapping XML、solver trace、native oracle | 仍未取得 | 真实原件；**未生成替代资产** |
| `tool-lifecycle-*/` 三个临时目录 | 判定为测试运行时产物 | 用户决定是否加入忽略规则或由测试清理 |

## 5. 保真度账本、目标状态与完成度

### 5.1 账本与目标

- `docs/reverse/VM-fidelity-ledger-20261112.md` 已追加 T4 判定：五个最大差异各有处理决定，**全部不提升**为 `parity-closed`。
- `docs/VM-EXECUTION-GOALS-20261112.md` 已追加接手后状态：G1 账本已复核；G2–G7 沿用 Codex 自述 `source-shaped-with-runtime-open`（CodeBuddy 未逐门禁复跑）；G8 `unknown`；G9 `blocked`；G10 未执行（人工验收与提交由用户决定）。
- FR0–FR4：仅 FR3 部分达 `source-shaped`，其余未达；CHECK-FR 未通过。

### 5.2 完成度（本轮未改变）

| 口径 | 数值 | 依据 |
|---|---|---|
| **授权第一阶段（用户指定权威口径）** | **10/10 = 100%** | `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json`；人工验收不计入 |
| 框架实施口径 | **42/96 = 43.75%** | `docs/framework-progress-audit.json`（`implementation-self-review-v1`） |
| 内部 stage gates | 25.78%（128 gates） | 内部维度，不作对外完成率 |
| 历史 41/96、16/162 | 历史快照 | 不参与报表 |

本轮的 12 份报告、13 份门禁哈希、288 项回归、13 条复跑命令**均不并入**任何完成度分母；测试通过不改写为 VM 等价。

### 5.3 交付结论

Codex 未提交交付已在 CodeBuddy 侧完成验收、口径校正、四类证据索引、FR-02/FR-03/FR-04/FR-05 重审、首条垂直切片回归、门禁判定、原件重新取证与工作区链路合流复核。**核心结论是 CHECK-FR 未通过、原厂宿主首处分歧仍未知、工具批次继续冻结**；未改产品行为、未提交、未推送、未宣称原厂全域等价。

提交、推送、真实元数据接入、人工验收与两项目同步时点，均由用户决定。
