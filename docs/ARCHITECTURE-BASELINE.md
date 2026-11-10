# 架构基线与源码量清单（逆向优先级依据）

> 目的：把“原厂架构 / 当前架构 / 源码规模 / 覆盖缺口”冻结成可复算的基线，使逆向优先级由数据决定，避免按印象挑点导致偏离与遗漏。
>
> 机器账本：`docs/architecture-baseline.json`；原始计数：`analysis/full-audit/source-inventory/source-inventory.json`；映射判断：`tests/ArchitectureRecovery/architecture-baseline.map.json`。
>
> 复算：
> ```
> dotnet run --project tests/ArchitectureRecovery/SourceInventory -c Release -- "d:/MyWork/REA逆向测试"
> powershell -ExecutionPolicy Bypass -File tests/ArchitectureRecovery/Build-ArchitectureBaseline.ps1 -Root "d:/MyWork/REA逆向测试"
> ```
> 已复算两次：除 `generatedUtc` 外逐字段一致。

## 1. 口径（先说清，数字才可信）

| 项 | 定义 |
|---|---|
| 原厂源码 | `analysis/decompiled/**`（安装版反编译，只读） |
| 当前源码 | `src/**` 中含 `.csproj` 的工程 |
| 排除 | `bin`、`obj`、`.a01group`、`.pending`、`TestResults` |
| 生成文件 | `*.g.cs` / `*.g.i.cs` / `*AssemblyInfo.cs` / `*.AssemblyAttributes.cs` 单独计数，不参与类型/成员统计 |
| 类型/成员 | Roslyn **仅语法**解析（不编译、不执行、不做语义绑定） |
| 依赖边 | 原厂：文件内 `using` 指令映射到所属反编译树（**文件数**，不是运行调用）；当前：`.csproj` 的 `ProjectReference` |
| 重复反编译 | `VisionMaster.exe`、`Apps.XmlParser`、`VMControls.WPF` 各有两次反编译 pass；**只取较大的一次**，不累加（因两次是重叠部分，不是并集） |
| 未计入 | BAML、native 二进制、`analysis/full-audit` 生成证据、docs |

## 2. 总量（两栏）

| 侧 | 单元 | .cs | .xaml | 行 | 类型 | 方法 |
|---|---|---:|---:|---:|---:|---:|
| 原厂（28 树原始，含重复 pass） | — | 2,705 | — | 470,162 | — | — |
| **原厂（25 唯一程序集，去重后）** | 28 树 | 2,614 | 422 | **435,666** | **3,503** | 10,195 |
| **当前** | 29 工程 | 430 | 41 | **33,324** | **955** | 2,576 |

比例：行数约 **13.1 : 1**，类型约 **3.7 : 1**。结论：当前实现对原厂是“行为子集实现”，不是等价重写；**不能按当前代码量估算完成度**。

## 3. 原厂唯一程序集基线（25 个）

| 程序集 | 层 | 行 | 类型 | 方法 | fanIn | 重复 pass |
|---|---|---:|---:|---:|---:|---|
| iMVS-6000PlatformSDKCS | L3 运行时/SDK | 92,225 | 1,389 | 842 | 0* | 否 |
| VMControls.WPF | L4 画布控件 | 57,343 | 220 | 1,314 | 13 | 是（另有 .Release） |
| Apps.XmlParser | L5 配置 | 50,181 | 385 | 1,559 | 185 | 是（另有 3 文件 pass） |
| VM.PcUI | L6 显示 | 44,652 | 169 | 1,288 | 30 | 否 |
| VM.Controls | L4 画布控件 | 35,902 | 301 | 1,393 | **230** | 否 |
| VM.PlatformSDKCS | L3 运行时/SDK | 33,100 | 337 | 986 | **176** | 否 |
| VM.Core | L2 模型 | 25,317 | 138 | 734 | **208** | 否 |
| Apps.XmlUI | L5 配置 | 23,220 | 132 | 407 | 25 | 否 |
| VMBaseControls | L4 画布控件 | 22,680 | 134 | 643 | 5 | 否 |
| DynamicObjectConfigControl | L5 配置 | 14,316 | 87 | 330 | 0* | 否 |
| Apps.Style | L6 显示 | 10,610 | 0（纯 XAML） | 0 | 0 | 否 |
| VisionMaster.exe | L1 外壳 | 8,714 | 44 | 218 | 7 | 是（另有 .Exe.Audit） |
| CompatibleTool | L7 迁移 | 4,418 | 38 | 129 | 0 | 否 |
| Apps.PcUI.ImageSource | L6 显示 | 2,423 | 22 | 65 | 23 | 否 |
| IMVSGroup | L8 工具 | 2,278 | 5 | 54 | 5 | 否 |
| VM.ExternalCall | L7 外部调用 | 2,083 | 51 | 87 | 30 | 否 |
| Apps.ColorFun | L6 显示 | 1,378 | 3 | 3 | 15 | 否 |
| VMControls.Data | L2 模型 | 1,174 | 7 | 40 | 8 | 否 |
| vServerApp | L1 服务 | 982 | 14 | 44 | 0* | 否 |
| ImageSourceModuleControl | L7 工具宿主 | 939 | 7 | 19 | 0 | 否 |
| IfBranchModule | L8 工具 | 444 | 4 | 17 | 0 | 否 |
| IfModule | L8 工具 | 416 | 5 | 5 | 0 | 否 |
| BranchStringCpmL | L8 工具 | 412 | 4 | 9 | 0 | 否 |
| BranchModule_STD | L8 工具 | 270 | 4 | 5 | 0 | 否 |
| ExtensionParamUserControl | L5 配置 | 189 | 3 | 4 | 0 | 否 |

`*` fanIn=0 **不代表无耦合**：`using` 代理对“命名空间与程序集名不同”（DynamicObjectConfigControl）和“跨进程/服务调用”（iMVS-6000、vServerApp）天然低估。涉及这两类时必须人工判定，不得只看数字。

## 4. 分层汇总与当前对应

| 层 | 原厂行 | 原厂类型 | fanIn | 当前对应工程 | 当前行 | 状态 |
|---|---:|---:|---:|---|---:|---|
| L1 外壳/服务 | 9,696 | 58 | 7 | VisionEditor（壳部分） | 6,653 | partial（双进程→单进程） |
| L2 文档模型 | 26,491 | 145 | 216 | VisionFramework + VisionEditor.Core | 3,701 | partial（身份/动态 IO 不同） |
| L3 运行时/SDK | **125,325** | **1,726** | 176 | VisionFramework（执行器） | 1,525 | **absent（机制缺失）** |
| L4 画布控件 | **115,925** | 655 | 248 | VisionEditor + VisionUi + VisionEditor.Core | 12,144 | partial（控件树不同、行为对齐） |
| L5 配置框架 | **87,906** | 607 | 210 | VisionPluginContracts | 4,847 | partial（覆盖不足） |
| L6 显示绘制 | 59,063 | 194 | 68 | VisionUi + VisionPluginContracts | 8,162 | partial（子集） |
| L7 迁移/外部调用 | 7,440 | 96 | 30 | VisionPlugins.ImageSource + SolutionInspector | 3,483 | partial（迁移器缺失） |
| L8 工具插件样本 | 3,820 | 22 | 5 | VisionPlugins.* | 5,595 | partial（3/11 批次） |

（L8 当前行数大于样本源码，因为已包含完整算法实现，不是“超额完成”。）

## 5. 架构一致性判定（结论）

| 维度 | 判定 | 依据 |
|---|---|---|
| 分层职责 | **一致** | 外壳 / 文档模型 / 执行 / 配置 / 画布 / 显示 / 持久化 / 插件 八层一一对应 |
| 数据与契约 | **部分一致** | `Solution/Procedure/Node` 同构，但身份（数值 ModuleId/GUID vs 字符串 Id）、动态 IO（SDK 动态端口 vs 静态 PortDescriptor）不同 |
| 运行时机制 | **不一致** | 原厂 `IMVS_* + ModuleFrame + 事件回调 + 服务进程`；当前 `IModule.ExecuteAsync + GraphExecutor 顺序 await` |
| 容器与持久化 | **不一致** | 原厂 `.sol/.solx` 原生容器 + 加密；当前 `.vmx.json` + `.gro` 只读 |
| 控件实现 | **不一致（有意保留）** | 原厂 `ItemsPage(ListView)+DiagramItem+Adorner`；当前自绘 `Canvas`，几何/交互已按源码对齐 |

## 6. 逆向优先级板（由第 3、4 节数据推出）

| 级别 | 项 | 层/规模 | 为什么排这里 | 阻塞单元 | 第一动作 |
|---|---|---|---|---|---|
| **P0-1** | 执行后端抽象（提交-回调 vs await） | L3 / 125,325 行、1,726 类型 | 原厂执行语义的最上游；所有工具的状态、错误码、自运行、连续执行都经过它 | C02.2、C05.3、C06、C08.3 | 落 `IExecutionBackend`（in-process / sdk-rpc）双实现，事件→结果映射，不改工具 |
| **P0-2** | 文档/模块身份与动态 IO | L2 / 26,491 行 | 身份是连接、订阅、结果缓存、保存的键；当前字符串 Id 与静态端口是系统性偏差源 | B01.2、B02.3、B04.3、C01.3、C03.2 | NodeDocument 增原生身份字段 + 动态 IO 映射，保持 JSON 兼容 |
| **P0-3** | 方案容器与原生帧 | L2+L7 | 容器不兼容使导入/导出/加密/版本迁移全部无法闭环 | D01、D03、D04、D05 | 只读 `NativeSolutionAdapter`（.sol 外层 + ModuleFrame 帧边界） |
| **P1-4** | 配置框架覆盖 | L5 / 87,906 行、607 类型 | 结构已按原链重建，但按行数覆盖约 5.5%（4,847/87,906）；影响所有工具配置窗 | A06、B01.1、B06.1、B06.2 | 按原 `NameValueItem/ParamGroup/模板键` 逐族补目录并出缺口清单 |
| **P1-5** | 显示对象与绘制 | L6 / 59,063 行 | 决定所有工具的可观察输出；当前只覆盖已用子集 | B04.2、B06.3 | 以 `ToolDisplay*` 契约为边界补 `VM.PcUI` 类型族 |
| **P2-6** | 画布控件树与装饰器 | L4 / 115,925 行 | fanIn 最高（VM.Controls 230），但行为已按源码对齐；整树替换代价高收益低 | A03/A04 的控件级等价 | **不重写**，继续补缺失命中/装饰器/边界 |
| **P2-7** | 外壳入口与加密/授权 | L1 / 9,696 行 | 入口完整性是防遗漏的必要核查项；214 条声明、222/222 方法已定位但效果链未闭 | A05.1、A05.3、A08.2、A08.3 | 按 `entry-inventory` 逐载体闭合“入口→命令→状态→持久化/执行” |
| **P3-8** | 平台服务与设备 | L3 / 92,225 行、1,389 类型 | **单一最大程序集**，当前 C06 三单元全 not_started，是最大未取证面 | C06.1-3 | 只读盘点服务发现/连接/回调，不做设备操作 |
| **P3-9** | 工具插件 | L8 / 3,820 行样本 | 依赖前 6 项机制；按整工具计分 | 11 批次余 8、162 目录其余 | 保持批次节奏，不回填已完成工具 |

排序原则：**机制在上、覆盖在下；共享在上、单工具在下**。同一层内先做“影响所有工具”的共享契约，再补单工具条目。

## 7. 遗漏风险登记（当前台账没有覆盖到的面）

| 项 | 规模 | 现状 | 风险 |
|---|---|---|---|
| `iMVS-6000PlatformSDKCS` | 92,225 行 / 1,389 类型 | 无任何框架单元对应（C06 全 not_started） | 相机/通信/设备链路整体空白 |
| `vServerApp` | 982 行 / 14 类型 | 未单列单元 | 执行语义差异的根因之一（服务进程 + 事件码） |
| `CompatibleTool` | 4,418 行 / 38 类型 | 无迁移器 | 旧版本方案无法迁移（D04.2/3） |
| `VM.ExternalCall` | 2,083 行 / 51 类型 | 无对应实现、无单元 | 外部调用/集成扩展点缺失 |
| `ExtensionParamUserControl` | 189 行 / 3 类型 | 未纳入 L5 覆盖清单 | 扩展参数控件族可能被漏 |
| `Apps.Style` | 10,610 行、0 类型（纯 XAML） | 只复用已用控件样式 | 样式/模板资产未按 key 全量对账 |
| `VMControls.WPF.Release`（第二 pass） | 8,300 行 / 52 类型 | 未与主 pass 差分 | 可能存在 Release 特有逻辑 |
| 162 目录其余工具 | 未建账 | 仅 11 个工具纳入批次 | 按工具数量计的完成度不代表目录覆盖 |

## 8. 维护规则（防止基线漂移）

1. 新增反编译树 / 新增当前工程 → 必须重跑两段命令，禁止手改数字。
2. 判断性内容（归层、canonical pass、优先级）只改 `architecture-baseline.map.json`，并在本文件对应表说明理由。
3. 分母或口径变化（排除规则、生成文件判定、重复 pass 归并）必须提升 `schemaVersion` 并记录日期与原因。
4. 优先级板每次调整需写明“数据依据”（层规模 / fanIn / 已闭单元），禁止无依据插队。
5. 人工验收、提交与另一项目采用仍单列，不进入本基线的计数。

## 9. 与既有进度台账的关系

- 框架实施进度（42/96 = 43.75%）仍以 `docs/framework-progress-audit.json` 为准；本文件**不改变**该分子分母，只提供“为什么要按这个顺序做”的依据。
  - 口径待统一：`framework-progress-audit.json` 的 2026-10-22 对账记为 `implementation done=42/96=43.75%`，并注明“此前汇总字段误记 41/96”；`AGENTS.md` 与部分历史交接仍写 `41/96=42.71%`。两者差异只在汇总字段，单元状态未变。需用户确认后统一为一个数字，本文档暂用台账重算值。
- 插件整工具进度以 `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json`（标定 10/10）与 `docs/NEXT-BATCH-PLUGIN-PROGRESS.json`（3/11）为准。
- 行为闭环进度以 `docs/VM-LOCAL-RESTORATION-STATUS.json`（C03 进行中）为准。
- 完整逆向计划站点入口：[索引页](reverse-plan/index.html)（11 页：口径与源码量、架构对照、S0–S9 路线图、纵向切片、P0–P3 任务、162 工具台账、门禁与治理）。生成 `node tests/ArchitectureRecovery/build-reverse-plan.cjs`，独立校验 `node tests/ArchitectureRecovery/verify-reverse-plan.cjs`；页面数字与本文件同源，重跑后页脚哈希同步更新。
- 完整逆向总计划：[权威总纲](reverse-plan/00-master-plan.html)（16 页站点的唯一执行口径），配套 WBS 工作包、框架单元任务卡、目录工具任务卡与 E0–E7 入口闭合链共 5 页执行视图；4 份既有分散计划保留为历史并登记处置状态。
