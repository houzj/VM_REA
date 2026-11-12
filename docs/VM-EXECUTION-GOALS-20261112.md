# VisionMaster 可执行目标驱动计划

本文件把高保真总方案转换为可以逐个关闭的交付目标。目标关闭必须满足退出条件和证据要求。测试数量、代码行数、截图数量和工具清点不能单独关闭目标。

## 使用规则

- 同时只推进一个共享框架目标和一个不冲突的证据目标。
- 目标必须有唯一结果文件、唯一责任目录和可复跑命令。
- 目标失败时先记录首个失败和下一步解除条件，不通过增加测试数量掩盖失败。
- 目标关闭后冻结交付摘要，后续差异只能新增目标或显式回开。
- `source-confirmed`、`independent-contract` 和 `parity-closed` 分开统计。

## 目标总表

| ID | 目标结果 | 依赖 | 当前状态 |
|---|---|---|---|
| G0 | 工作树、原件和运行环境身份冻结 | 无 | ready |
| G1 | 已有实现保真度账本和差异矩阵 | G0 | ready |
| G2 | 启动链和解决方案实体达到 source-shaped | G1 | ready |
| G3 | 公共执行/停止/结果/释放合同达到 source-shaped | G1 | ready |
| G4 | 配置、ROI、Mask、显示和参数提交达到 source-shaped | G1 | ready |
| G5 | ImageSource → Blob/真实工具高保真垂直切片 | G2,G3,G4 | ready |
| G6 | 保存、重开、错误恢复和再次运行闭环 | G2,G3,G5 | ready |
| G7 | Group、子流程和复杂数据闭环 | G3,G5,G6 | ready |
| G8 | 匹配原厂环境下首处分歧报告 | G0,G5 | waiting-original-baseline |
| G9 | 选定工具批次逐工具发布 | G5,G6,G7,G8 | gated |
| G10 | 全量审计、人工验收和交接 | G9 | gated |

## G0：冻结身份和环境

**目标**：任何后续结论都能回到唯一原件、输入和构建身份。

**执行内容**：

- 固定 main 和 tool-plugins 的 HEAD、工作树差异和共享文件所有者。
- 清点 VisionMaster、宿主 DLL、SDK、PDB、资源、方案、样图和依赖。
- 记录版本、长度、SHA-256、MVID、PE 时间戳、导出和实际装载路径。
- 固定样图、像素格式、尺寸、参数、ROI/Mask、输出开关和错误码。
- 记录原厂机器、桌面、权限、设备和 REA 可用性。

**交付物**：

`analysis/full-audit/identity/<run-id>/manifest.json`

**退出条件**：所有后续任务引用唯一身份。缺失项有具体解除条件。

**失败处理**：身份不完整时只允许继续静态源码索引，不允许数值差分或算法修改。

## G1：建立保真度账本

**目标**：把已经实现的部分按原厂证据重新分类，找出最大的真实差异。

**执行内容**：

- 盘点启动、方案、主窗口、工具箱、配置、ROI、执行、Group、结果和释放。
- 每项登记原厂入口、当前入口、输入、输出、状态、资源和持久化。
- 标记 `parity-closed`、`source-shaped`、`independent-contract`、`visual-only`、`divergent`、`unknown`。
- 用原厂 XAML、资源、CIL、PE、汇编和已有运行记录逐项复核。
- 禁止因测试通过把 independent 项提升为 parity 项。

**交付物**：

- `docs/reverse/VM-fidelity-ledger-20261112.md`
- `analysis/full-audit/fidelity/20261112/matrix.json`

**退出条件**：所有当前已实现模块都有状态、证据、差异和处理决定。最大差异前五项有明确执行目标。

## G2：启动链和解决方案实体

**目标**：从启动入口到真实方案载体建立连续状态机。

**执行内容**：

- 对齐 Program/Main、单实例、工作目录、初始化、欢迎页、Guide、最近方案和主窗口。
- 对齐新建方案、打开方案、方案身份、Procedure、Node、资源目录和关闭释放。
- 记录原厂 `.sol` 的可确认字段，未知字段保持不透明。
- 让独立 JSON 方案通过同一方案接口运行，不能另造旁路入口。
- 增加启动异常落盘、残留进程检测和关闭释放验证。

**交付物**：

- `docs/reverse/G2-startup-solution-state-machine.md`
- `tests/StartupSolutionLifecycle/`
- 启动身份和状态转移 JSON

**退出条件**：VS0、VS2 通过，窗口关闭后进程和互斥体释放，创建/保存/重开/执行使用同一方案实体。

**原厂边界**：缺少匹配 `.sol` 时，独立 JSON 闭环可以关闭，原厂容器兼容仍保持未确认。

## G3：执行、停止、结果和释放合同

**目标**：把当前执行实现与原厂源码链对齐到可审计的状态机。

**执行内容**：

- 逐成员核对 Run、SyncExecute、Callback、Result、Error、Stop、Cancel、Dispose。
- 记录回调进入、结果缓存、清结果、通知顺序和资源完成边界。
- 覆盖重入、停止失败、在途算法、异常订阅者、子流程和 Group。
- 让至少一个真实工具消费同一公共执行合同。
- 保留独立实现与原厂运行状态的差异。

**交付物**：

- `docs/reverse/G3-execution-lifecycle-contract.md`
- `tests/NativeEvidence/G3/`
- 生命周期状态转移和失败矩阵

**退出条件**：C02 回归通过，关键路径调用链有证据，VS5 的失败/停止/释放可重复。

## G4：配置、ROI、Mask、显示和参数提交

**目标**：消除“界面看起来能改但数据没有按原厂链路提交”的差异。

**执行内容**：

- 逐参数恢复读取、编辑、验证、序列化、提交和失败恢复。
- 恢复 ROI/Mask 集合、全屏、图像尺寸、二进制载荷、显示开关和结果发布。
- 对照原厂模板、绑定、命令、事件和参数键。
- 记录主框架公共合同和工具专属转换的归属。

**交付物**：

- `docs/reverse/G4-configuration-roi-display-contract.md`
- 参数、ROI、Mask 和显示缓冲测试
- 一个真实工具配置消费记录

**退出条件**：VS3 通过，参数和 ROI 数据在提交前后可追踪，非法值、空区域和图外区域有明确结果。

## G5：第一条高保真真实工具切片

**目标**：用一条最完整证据链验证框架和工具能共同复现原厂操作。

**默认选择**：ImageSource → Blob，若原厂 Blob 宿主环境仍不可用，则选择证据最完整的真实工具，同时保留 Blob 差分目标。

**执行内容**：

- 从工具入口追到公共配置、执行、结果和持久化。
- 对照工具参数、ROI/Mask、像素缓冲、结果字段和错误码。
- 在主窗口完成工具箱、配置、执行、停止、结果和保存重开。
- 建立独立实现和原厂运行的并排矩阵。

**交付物**：

- `docs/reverse/G5-first-high-fidelity-tool-slice.md`
- `tests/HighFidelityVerticalSlice/`
- 工具身份、输入、输出和差分报告

**退出条件**：VS1、VS3、VS5、VS2 通过。工具状态至少达到 `source-shaped`，有原厂证据时再向 `parity-closed` 推进。

## G6：保存、重开、错误恢复和再次运行

**目标**：确认方案实体和运行状态不会因为保存、失败或关闭而分叉。

**执行内容**：

- 保存前后比较方案身份、节点、参数、资源、ROI/Mask、结果和最近列表。
- 重开后重新绑定执行上下文和图像资源。
- 覆盖缺文件、坏版本、错误参数、运行中关闭和再次运行。
- 检查文件、线程、句柄、订阅、缓存和 native 资源释放。

**交付物**：

- `docs/reverse/G6-persistence-recovery-release.md`
- 保存/重开/异常/再次运行回归包

**退出条件**：连续两次保存重开结果一致，失败后可以重新运行，关闭无残留进程和锁。

## G7：Group、子流程和复杂数据

**目标**：让复杂流程载体使用同一方案和执行合同，不再作为独立演示路径。

**执行内容**：

- 对齐 Group、子流程、Loop、Branch、Break、累加、数组和复合叶。
- 对齐 Group 内参数、ROI、订阅、结果投影、错误和停止。
- 覆盖嵌套、重入、空集合、部分失败和取消。

**交付物**：

- `docs/reverse/G7-group-complex-data-contract.md`
- Group 垂直切片和生命周期回归

**退出条件**：VS4、VS5、VS2 通过，复杂数据未知项明确列出，不能用清零或强制类型转换掩盖。

## G8：原厂首处分歧

**目标**：在匹配原厂身份下，定位宿主与直接 SDK 的第一处分歧。

**执行内容**：

- 宿主和直接 SDK 各独立运行两次以上。
- 采集 Run 前像素、ROI/Mask 集合、参数缓冲、完整结果、图像输出、错误码和释放。
- 使用 PE/汇编/运行探针确认第一处分歧的准确调用边。
- 判断差异属于公共框架、工具专属、环境或输入版本。

**交付物**：

- `docs/reverse/G8-first-divergence-report.md`
- 宿主/SDK 原始探针产物和哈希

**退出条件**：报告能指向一个可重复的第一处分歧，或记录具体阻塞点和解除条件。没有首处分歧证据时禁止改算法。

## G9：工具批次发布

**目标**：在公共合同稳定后，按工具批次扩展覆盖。

**每个工具退出条件**：

- 原件和资源身份固定
- 源码入口到输出路径闭合
- 配置和参数持久化有证据
- 正常、失败、停止、保存重开通过
- 主窗口真实消费通过
- 原厂差分或明确运行未确认边界
- 工具专属修改未复制公共框架

**批次顺序**：ImageSource/Binary/Blob/Fixture/Branch → 标定/坐标/定位/测量 → 识别/缺陷 → 其余目录工具。

## G10：发布和交接

**目标**：形成 CodeBuddy、用户和后续会话都能复跑的交付包。

**交付物**：

- 最终身份清单
- 源码和资源证据索引
- 构建产物和 SHA-256
- 全量回归摘要
- 每个工具退出矩阵
- 已知差异和未知项
- 回滚点和恢复命令
- 人工验收步骤
- 双项目同步和提交 SHA

**最终退出条件**：选定范围所有关键路径达到相应保真度状态，未确认行为和解除条件公开，构建、测试、主机操作和交接证据齐全。

## 首个执行目标

下一项不是继续扩展工具，而是执行 **G1：建立保真度账本**。它必须先回答：当前已经实现的每个区域到底是原厂确认、源码形状、独立合同、视觉近似、已知差异还是未知。G1 关闭后，按最大差异选择 G2、G3、G4 的实施顺序。

## 接手执行后目标状态（2026-11-12，CodeBuddy 追加）

本节由 `docs/reverse/T5-final-delivery-20261112.md` 驱动，只追加状态，不改写上方目标定义与「首个执行目标」历史段落。CodeBuddy 侧状态与 Codex 自述状态分开记录；门禁结果一律不等同于 VM 等价。

| 目标 | Codex 自述 | CodeBuddy 接手后状态 | 依据 |
|---|---|---|---|
| G0 身份冻结 | ready / 部分 | **部分完成**：本机原厂 EXE/SDK/`VMControls.Data.dll`/`ToolGroup.xml`/`CircleFind.sol` 已固定 SHA-256；`test1108.sol`、设备与认证状态仍缺 | `T5-original-asset-gap-20261112.md` |
| G1 保真度账本 | `completed-for-planning` | **已复核并追加 T4 判定**：15 区域、0 `parity-closed`；五个最大差异各有处理决定，状态不提升 | `VM-fidelity-ledger-20261112.md` |
| G2 启动/方案实体 | `source-shaped` | 保持自述状态，**未逐门禁复跑**；T2 判定「重写 0 项」，`.sol` 字段仍 `unknown` | `T2-startup-solution-matrix-20261112.md` |
| G3 执行/停止/结果/释放 | `source-shaped-with-runtime-open` | 保持自述状态；T3 逐成员重审，原厂同轮时序与 native 完成边界仍 `unknown` | `T3-execution-config-dataflow-20261112.md` |
| G4 配置/ROI/Mask/显示 | `source-shaped-with-runtime-open` | 保持自述状态；公共合同为 `independent-contract`，二进制载荷 `unknown` | 同上 §2 |
| G5 首条垂直切片 | `source-shaped-with-native-runtime-open` | **当前实现侧已复跑**：T4 新增 5 套 288 项 + T0 8 条命令；E3 原厂宿主运行仍缺 | `T4-first-fidelity-slice-20261112.md` |
| G6 保存/重开/恢复 | `source-shaped-with-native-runtime-open` | 保持自述状态；`StartupShellChecks`、`PersistenceChecks` 30 项通过（E4） | 同上 §2–3 |
| G7 Group/复杂数据 | `source-shaped-with-native-runtime-open` | 保持自述状态；`VisionWorkflow.CallChecks` 93 项通过（E4） | 同上 |
| G8 原厂首处分歧 | `runtime-open` | **`unknown` / `blocked`**：`test1108.sol` 与匹配宿主环境缺失，首处运行时分歧未定位 | `T5-first-divergence-20261112.md` |
| G9 工具批次发布 | 多批 `batch-self-reviewed-native-runtime-open` | **`blocked`**：CHECK-FR 未通过，`fidelityCorrectionBeforeExpansion` 继续冻结扩展 | `T4-first-fidelity-slice-20261112.md` §5 |
| G10 发布和交接 | gated | **本轮交付已完成报告部分**：12 份 T0–T5 报告、命令与结果、未知项与解除条件；人工验收、提交、推送与双项目同步**由用户决定**，未执行 | `T5-final-delivery-20261112.md` |

CHECK-FR 判定：**未通过**。FR0–FR4 中仅 FR3 部分达 `source-shaped`；E3 原厂运行与 E5 主机验收仍缺，故不解除工具批次冻结。

口径：授权第一阶段 **10/10 = 100%**（`docs/SOURCE-LEVEL-REVERSE-PROGRESS.json`）；框架实施 **42/96 = 43.75%**（`docs/framework-progress-audit.json`）。本轮报告数、回归断言数与门禁数均不并入完成度分母。

