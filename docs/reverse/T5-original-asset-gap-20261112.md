# T5 缺失原件取证与身份清单（2026-11-12）

执行：CodeBuddy ｜ 任务：T5.1 ｜ 取证根：`C:/Program Files/VisionMaster4.4.0`（本机实测存在）。

> 本阶段重新清点了交接文档与 `docs/CODEBUDDY-TAKEOVER-TASKS-20261112.md` §8 登记为「缺失」的原件。**部分原件实际存在，证据缺口据此解除；但未生成任何替代资产，也未把取证结果写入产品配置。**

## 1. 原厂安装身份（E0）

| 原件 | 长度 | SHA-256 | 与 C03 归档 |
|---|---|---|---|
| `Applications/VisionMaster.exe` | 1,440,384 | `CF5730DF126C9C244E13E454CB664909F7E6A2BDF78B5314D03F6DC64E76D291` | 一致 |
| `Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll` | 13,422,592 | `F5129789D7CF29DCB5F1B772D58159478D1110F1380905BCD27C38ED5F072A4E` | 一致 |
| `Applications/myLibs/VMControls.Data.dll` | 30,208 | `8A9F45E1213D178E777F390B5978ECFE747A89916561D317CECF2436F38CCA50` | 与历史 `rea-identity.json` 的 `a0321eb4…` 不同 |

REA 身份复核见 `docs/reverse/T3-execution-config-dataflow-20261112.md` §3。

## 2. 已取得的原件

### 2.1 `ToolGroup.xml`（解除 T1.2 的证据缺口）

- 安装根全树递归：`ToolGroup.xml` **30 份**；其中 `Applications/Module(sp)/x64/` 下 **15 份**。
- 含 `<priorityStrategyInfo>` 的有 **4 份**（均在 x64 类别目录）：

| 类别 | 真实值 | 文件 SHA-256 |
|---|---|---|
| `Calculation` | `Position:3.5;Recognition:4.5` | `40C489D36F3CCE60000D086B5AAC42592CF9161049C73FA5D30DD2FFDD687D39` |
| `DefectDetection` | `Defect:1.5` | `52AFE490BAD58D0F02C9D843D4C2C9D4F777F01FF09D94A79A827C5A0736E2F2` |
| `ImageProcessing` | `Defect:1.75` | `F4D40005C927EB219515496515EE0A56676F0B33D4BB5E93586BEC458FA7E1F0` |
| `Recognition` | `Recognition:1.5` | `F1099305928E209C10BD7DA9DD66F576D32B51B50402CE14871E1BEB6DBAA529` |

样例结构（Calculation）：

```xml
<?xml version="1.0" encoding="UTF-8"?>
<ToolBoxItemData>
    <name>Calculation</name>
    <priority>8</priority>
    <priorityStrategyInfo>Position:3.5;Recognition:4.5</priorityStrategyInfo>
</ToolBoxItemData>
```

**结论**：`type:value;...` 格式与 `ToolBoxData.ChangePriorityByInfo()`（`ToolBoxData.cs:231-261`）完全吻合，T1.2 登记的「真实数值缺失」证据缺口**已解除**。原厂四类枚举中 `General` 在这 4 份文件里没有对应策略项，这与原厂「无匹配则不改优先级」的分支一致，**不得据此推断 General 应取某个默认值**。

**处置决定**：按交接文档「取得原件后只补元数据并复跑，不改解析算法」，本阶段只**登记证据**（路径、哈希、数值），**未**修改 `src/VisionEditor/ToolboxPriorityStrategy.cs`、未复制 XML 进产品资源、未写死任何排序值。把真实元数据接入产品属于会改变工具箱排序行为的变更，需在 CHECK-FR 通过后或用户明确授权时作为独立任务执行，并复跑 `ToolCatalogLifecycleChecks15`。当前 CHECK-FR 未通过（`T4-first-fidelity-slice-20261112.md` §5），工具批次扩展仍冻结。

### 2.2 原厂 `.sol` 样本

安装根下发现 **11 份 `.sol`**，其中 `CircleFind.sol` **3 份**（C#/QT/VC 示例各一），三者长度与哈希完全相同：

| 长度 | SHA-256 |
|---|---|
| 4,082,307 | `B91A7736F467757E15A55AF7836E22364AE153478E17C69AAEB69E0C30674D0B` |

**结论**：T2 §2 登记的「原厂 `.sol` 样本缺失」证据缺口**已解除到身份层**；但本阶段未解析容器字段、未建立只读探针，`.sol` 的字段、资源目录与版本迁移仍为 `unknown`。JSON 方案继续不伪装 `.sol` 兼容。解除条件：在取得用户许可后建立只读探针，逐字段记录并保留不透明字段。

### 2.3 未取得（保持 `unknown` / `blocked`）

| 原件 / 资产 | 本次搜索范围 | 结果 | 解除条件 |
|---|---|---|---|
| `test1108.sol` | 安装根全树 + `d:/MyWork/Rea逆向` + `d:/MyWork/测试图片` | **0 命中** | 用户提供历史方案或匹配场景 |
| `frame-0.bmp`（solver-fixture） | 安装根全树 | **0 命中** | 真实 fixture 原件 |
| `fixture.bmp`（distortion） | 安装根全树 | **0 命中** | 真实 fixture 原件 |
| `reader-8.xml` | 安装根全树 | **0 命中** | 真实资源 |
| Map 图像结果 JSONL、CameraMapping 原厂 XML、solver trace | 未按文件名穷尽检索 | 未取得 | 定向取证 |
| GrayMatch / native solver exact oracle | 归档报告记录缺失 | 未取得 | 原生运行证据 |
| 真实设备、相机、认证服务状态 | 需匹配原厂运行环境 | 未取得 | 用户安排环境 |

## 3. 与既有结论的冲突处理

- `docs/CODEBUDDY-HANDOFF-20261112.md` §60 与 `docs/reverse/C01-toolbox-type-priority-chain-20261112.md` 记载「当前仓库没有真实 `PriorityStrategyInfo` 数值」—— 对**仓库**成立，对**本机原厂安装**不成立。本节不改变其历史叙述，只登记新证据。
- `docs/CODEBUDDY-TAKEOVER-TASKS-20261112.md` §8 缺口表在本阶段后需按第 2 节更新状态；未取得的条目保持原状。
- 本机安装件与 C02/C03 归档 DLL 存在哈希差异记录，取证结论不外推到历史宿主版本。

## 4. 声明

- 未生成、未伪造任何替代资产；未复制原件进产品资源目录；未删除任何历史证据。
- `analysis/*` 被 `.gitignore` 排除，本报告记录的是**安装根路径 + SHA-256**，远程不可直接复原这些原件。
- 原厂安装目录为只读取证对象，未启动 VisionMaster、未修改其文件、未查询设备或认证凭据。
