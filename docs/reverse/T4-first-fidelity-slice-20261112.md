# T4 首条垂直切片与 CHECK-FR 判定（2026-11-12）

执行：CodeBuddy ｜ 任务：T4.1（FR-06）、T4.2（CHECK-FR）｜ 门禁：`docs/VM-REVERSE-GATE-MATRIX-20261112.json`。

> **判定结论：CHECK-FR 未通过，继续冻结工具批次扩展。** 本阶段复跑的回归只证明当前实现的合同（E4），FR0–FR4 中仍有 `divergent` / `independent-contract` / `unknown` 项，不得解除 `fidelityCorrectionBeforeExpansion` 冻结。

## 1. 切片定义与输入身份（E0）

覆盖 VS1（新建方案 → ImageSource → 加载本地图像 → 结果）、VS3（配置 → 参数提交 → 执行）、VS5（失败 → 停止 → 释放 → 再运行）与 VS2（保存重开）的**当前实现侧**链路；不覆盖 VS6（原厂 `.sol` / 场景直启）。

| 输入 | 长度 | SHA-256 |
|---|---|---|
| `analysis/images/circle-find-sample.bmp` | 3,981,366 | `690B17333CE8A9F6FE8961C90D7F21FBD6472DBF0F7CC3443FC89DA43BB33ABA` |
| `examples/architecture-demo.vmx.json` | 3,464 | `D718BC61A0E56985B8B09C07F73A153AD12E3BB3AD72C5C5E825852C98C579FF` |
| `examples/image-source-plugin.vmx.json` | 1,530 | `64BCE23497BE6CC8EC8E4772F1A002CFBAB25298ED1BCA5AE601CB38D192388C` |

构建身份：T0 Release 构建 0 警告 / 0 错误（`artifacts/codebuddy-verify/t0-build.log`）。原厂运行身份：本机 `C:/Program Files/VisionMaster4.4.0` 的 `VisionMaster.exe`（`CF5730DF…D291`）与 `iMVS-6000PlatformSDK.dll`（`F5129789…2A4E`），但**本次未启动原厂宿主**，无 E3 同输入运行。

## 2. 实际复跑回归（E4）

### 2.1 T0 已冻结的 8 条命令

见 `docs/reverse/T0-replay-command-inventory-20261112.md`。结果：Release 构建 0/0；`ToolCatalogLifecycleChecks15` 19/19；`NodeProjectionChecks` 54；`VisionEditor.WorkspaceProjectionChecks` 79；`VisionEditor.ShellMenuChecks` 65；`VisionEditor.StartupShellChecks` 通过；`VisionEditor.ShellHistoryChecks` 通过；**`VisionEditor.ShellChecks` 失败**（`Program.cs:275`，退出码 `-532462766`）。

### 2.2 T4 新增复跑（按 C02 已冻结的套件清单）

`pwsh` 在本机不可用，`tests/NativeEvidence/C02/Run-LifecycleRegression.ps1` 首行 `#Requires -Version 7.0` 无法执行；因此按该脚本第 13–25 行列出的套件名，用 `dotnet run --project tests/<name>/<name>.csproj -c Release` 等价复跑（未使用 `--artifacts-path` 与独立 cwd，产物路径不同，见第 6 节限制）。

| 套件 | 退出码 | 结果 | 日志 |
|---|---|---|---|
| `VisionFramework.ExecutionChecks` | 0 | `91 execution checks passed.` | `artifacts/codebuddy-verify/t4-VisionFramework.ExecutionChecks.log` |
| `VisionFramework.DebugChecks` | 0 | `PASS 35 debug runtime checks; isolated target result15; stop/close await cleanup.` | `t4-VisionFramework.DebugChecks.log` |
| `VisionFramework.PersistenceChecks` | 0 | `30 persistence checks passed.` | `t4-VisionFramework.PersistenceChecks.log` |
| `VisionConfiguration.ExecutionChecks` | 0 | `39 configuration execution checks passed; source scheduling gates, no devices.` | `t4-VisionConfiguration.ExecutionChecks.log` |
| `VisionWorkflow.CallChecks` | 0 | `PASS 93 canvas subprogram checks; real counts365 -> 1; nested calls, failure, cancellation and timeout.` | `t4-VisionWorkflow.CallChecks.log` |

新增合计 **288 项**通过。另有 6 个 C02 套件（Group 跳出/累积、Group 多层生命周期、回调队列、执行所有权、执行策略、真实模块图集成）因需 `audit` 目录参数或示例方案路径，本阶段**未复跑**，不推断其结果。

## 3. 切片环节覆盖矩阵

| 环节 | 证据 | 等级 | 状态 |
|---|---|---|---|
| 新建 / 打开方案 | `StartupShellChecks`（保存路径、身份保持、重开、clean） | E4 | 当前侧覆盖 |
| ImageSource 供图 | `image-source-plugin.vmx.json` 身份固定；`ToolCatalogLifecycleChecks15` 仅覆盖目录元数据 | E0+E4 | 部分覆盖 |
| 配置读取 / 参数提交 | `VisionConfiguration.ExecutionChecks` 39 项；`NodeParameterWindow` 11 项（Codex 自述） | E4 | 当前侧覆盖，原厂模板未取证 |
| ROI / 显示 | `VisionEditor.WorkspaceProjectionChecks` 79 项；`ToolConfigurationRoiService` 自述非原生 ABI | E4 | 当前侧覆盖，原厂 ROI/Mask `unknown` |
| 执行 / 回调 / 结果 | `VisionFramework.ExecutionChecks` 91 项；`WorkflowExecution.cs:43-57` 两阶段发布 | E4 | 当前侧覆盖，原厂同轮时序 `unknown` |
| 停止 / 取消 / 释放 | `VisionFramework.DebugChecks` 35 项（stop/close await cleanup）；`VisionWorkflow.CallChecks` 93 项（failure, cancellation, timeout） | E4 | 当前侧覆盖 |
| 保存 / 重开 / 错误恢复 | `VisionFramework.PersistenceChecks` 30 项；`StartupShellChecks` | E4 | 当前侧覆盖 |
| 原厂同输入差分 | 无本次运行；历史 45/971486（宿主）与 47/973202（直接 SDK） | — | **未覆盖，`unknown`** |
| 主窗口人工验收 | 未执行（需用户明确要求且 `WinSta0\Default`） | E5 | **未覆盖** |

## 4. E0–E5 标注

| 等级 | 本次状态 |
|---|---|
| E0 身份 | 已固定三个输入哈希、构建日志、本机原厂 EXE/SDK 哈希、REA 三条 Evidence；**原厂 `.sol` 与匹配宿主身份仍缺** |
| E1 源码链 | T1/T2/T3 已登记启动、方案、执行、配置调用边 |
| E2 PE/ABI | C02/C03 归档证据 + REA `inspect_pe_imports` / `inspect_managed_artifact` / `inspect_managed_members` |
| E3 受控运行 | **本次无原厂宿主运行**；仅有当前实现回归 |
| E4 独立实现回归 | T0 + T4 共 13 条命令，含 1 项未解决失败 |
| E5 主机验收 | **未执行** |

关键路径最低门槛（E0+E1+E3+E4+E5）：**未达到**（缺 E3 原厂运行与 E5 人工验收）。

## 5. CHECK-FR 判定（T4.2）

| 门禁项 | 当前状态 | 依据 | 是否达 `source-shaped` |
|---|---|---|---|
| FR0 当前 vs 原厂启动矩阵 | `divergent` | T2 §1：Guide 新修正版未人工复核，主窗口载体逐成员未对齐 | **否** |
| FR1 方案实体与宿主容器 | `independent-contract` | T2 §2：`.sol` 不透明，当前 JSON 独立合同 | **否** |
| FR2 工具载体与配置合同 | `divergent` | T2 §3、T3 §2：动态载体/配置入口/二进制载荷未取证 | **否** |
| FR3 执行/停止/结果/释放 | `source-shaped-with-runtime-open` | T3 §1：形状对齐，原厂同轮时序与 native 完成边界未闭合 | **部分** |
| FR4 保存/重开/错误恢复 | `independent-contract`（部分 source-shaped） | T2 §1/T3 §1：当前闭环 E4 通过，原厂持久化字段未取证 | **否** |

判定：**不通过**。五个最大差异均已给出处理决定（`docs/reverse/T2-startup-solution-matrix-20261112.md` §6 与 `T3-execution-config-dataflow-20261112.md` §4），但「有处理决定」不等于「达到 source-shaped」。

按门禁 `fidelityCorrectionBeforeExpansion` 与总方案 §1.1：

- 继续冻结新增工具 UI 与工具目录扩展；
- `independent-contract` 不写成 `parity-closed`；
- 不因 288 项回归通过或 13 条命令成功提升完成度或宣称 VM 等价；
- T5.3「G9 工具批次续作」保持 **blocked（依赖 CHECK-FR）**。

## 6. 限制与未确认项

1. `pwsh` 缺失导致 C02 整批 11 套脚本未运行；本阶段只等价复跑 5 套，且**未使用脚本的 `--artifacts-path` 独立输出与套件独立 cwd**，产物哈希不可与历史 462 项批次直接比对。
2. `VisionEditor.ShellChecks` 首个失败仍未解决；T2 §5 已给出定位路径但未改动任何代码/夹具。
3. 6 个 C02 套件未复跑，不推断通过。
4. `test1108.sol` 在 `d:/MyWork/Rea逆向`、`d:/MyWork/测试图片`、`C:/Program Files/VisionMaster4.4.0` 三个根下递归搜索均无结果；匹配原厂宿主环境仍缺。
5. 本机 `VMControls.Data.dll`（`8A9F45E1…`）与历史 `rea-identity.json`（`a0321eb4…`）哈希不同，反编译源码对应构建 `unknown`。
6. 原厂安装原件与 C02/C03 归档 DLL 存在哈希差异记录，E2 结论不外推到当前安装。
7. 人工验收与提交推送时点由用户决定，本阶段未执行。

## 7. 保真度账本更新

已在 `docs/reverse/VM-fidelity-ledger-20261112.md` 追加「T4 CHECK-FR 判定」章节：五个最大差异逐项给出处理决定，账本状态**不提升**为 `parity-closed`。
