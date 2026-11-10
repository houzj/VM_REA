# 162 个目录工具实现规格（索引）

机器台账：`tests/ArchitectureRecovery/tool-implementation-specs.map.json` · builtFrom `ef7b1cc9/cb822df8db70`

**全部为待逆向实现目标**：161 个未建立、1 个 partial；不按自研工程映射、不声称等价。

## 证据覆盖

| 项 | 数量 |
|---|---|
| 工具 | 162 |
| 分类 | 14 |
| 有 XML 证据 | 162 |
| 有 DLL 证据 | 162 |
| 有资源证据 | 153 |
| 有算法参数 | 133 |
| 有输入端口 | 133 |
| 有输出名 | 153 |
| 证据文件总数 | 2015 |
| 证据分级 high / mid / low | 160 / 2 / 0 |

## 14 分类

| 分类 | 工具数 | 声明数 | high 证据 |
|---|---|---|---|
| Calculation | 11 | 11 | 11 |
| Calibration | 10 | 10 | 10 |
| Collection | 5 | 5 | 5 |
| ColorProcessing | 6 | 6 | 6 |
| Communication | 5 | 5 | 5 |
| DefectDetection | 14 | 14 | 14 |
| Global | 17 | 17 | 15 |
| ImageGeneration | 4 | 4 | 4 |
| ImageProcessing | 21 | 21 | 21 |
| Location | 28 | 28 | 28 |
| Logic | 20 | 20 | 20 |
| Measurement | 11 | 11 | 11 |
| Recognition | 4 | 4 | 4 |
| SplitCombination | 6 | 6 | 6 |

## 每工具六项规格

台账 `tools[]` 每项含：身份与状态、资源哈希（XML/DLL/资源，SHA-256 前 8 位）、参数 IO 与输出名、native 边界（kind / 声明库 / `not-recovered`）、实现状态与验收项、unknown 列表，并挂 `13-tool-cards.html#toolcard-N` 任务卡锚点。

## 边界

- 静态证据充分度 `high` 只表示声明性材料齐备，**不等于**函数体已取得或可实现。
- `Process native 函数体、ABI 与首调用目标` 全部标 `not-recovered`；REA native provider 当前不可用。
- 不以合理设计填补缺失函数体/资源/ABI，不把 XML 声明写成已实现，不把测试通过写成 parity。
- C03 差分 `45/971486` 对 `47/973202` 仍开放；`IMVSBlobFindModu` 相关算法不在本批实现范围。