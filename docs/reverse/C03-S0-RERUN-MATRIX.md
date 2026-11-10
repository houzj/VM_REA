# S0 复算：C01 复跑与 12 张标定样本矩阵

日期：2026-11-10 · 证据卡：`analysis/full-audit/batch1-20261107/C03/c03-s0-rerun-matrix-20261110.json`（`C03-S0-RERUN-MATRIX-20261110`）

## 复跑与保护

命令 `dotnet run --project tests/VMVerticalSlice.C01/VMVerticalSlice.C01.csproj --no-restore`，dotnet 10.0.401，HEAD `7d65a1e1`，**exitCode=0、18 项 PASS**。
每次复跑用子进程级 `VM_C01_IMAGE` 指定输入；历史 4 个证据文件先备份再恢复，基线 SHA-256 保持 `ce543016…`。

## 同输入复跑

用历史同一张 `D:\MyWork\图片库\标定\1.jpg` 复跑，得到 **47 / 973202**，报告 SHA-256 与历史报告**逐字节相同**（`ce54301605446527…`）。13 个 Blob 输出名、Group `image/width` 重开输出名、空失败输出名全部一致。

## 12 张标定样本矩阵

| 样本 | BlobCount | TotalArea | 重开计数 | 输出名 | 会话 |
|---|---|---|---|---|---|
| 1.jpg | 47 | 973202 | 47 | 13 | 0 |
| 2.jpg | 50 | 977000 | 50 | 13 | 0 |
| 3.jpg | 45 | 986444 | 45 | 13 | 0 |
| 4.jpg | 51 | 964047 | 51 | 13 | 0 |
| 5.jpg | 61 | 962495 | 61 | 13 | 0 |
| 6.jpg | 72 | 954959 | 72 | 13 | 0 |
| 7.jpg | 70 | 994718 | 70 | 13 | 0 |
| 8.jpg | 62 | 997593 | 62 | 13 | 0 |
| 9.jpg | 46 | 987481 | 46 | 13 | 0 |
| 10.jpg | 82 | 944158 | 82 | 13 | 0 |
| 11.jpg | 52 | 961870 | 52 | 13 | 0 |
| 12.jpg | 70 | 983259 | 70 | 13 | 0 |

12 张图全部 `sessionStatus=0`、`reloadedCount == count`、13 个输出名。样本 1 与历史一致；不同输入产生不同数值属正常。

**样本 3 的计数 45 与原厂 45 相同，但面积 986444 ≠ 971486，属数值巧合，不构成差分解释。**

## 方案文件字节级比对

- `image-source-vertical-slice.vmx.json`：字节一致
- `image-source-blob-vertical-slice.vmx.json`：字节一致
- `image-source-group-vertical-slice.vmx.json`：不一致；规范化后差异仅 4 处随机 `ProcedureId`/`NodeId` 引用

Group 方案含运行时生成的 Id，跨运行字节全等不成立；本次以「重开结果一致 + 差异定位」满足 S0.2，不声称方案字节全等。

## S0 复判

- **S0.1** `satisfied-on-current-build`：exitCode=0、18 PASS、12 样本会话与输出名齐备、同输入报告逐字节可复现。
- **S0.2** `satisfied-on-current-build`：12 样本重开计数一致、Group 重开输出名一致、方案差异定位为随机 Id。
- **S0.3** `satisfied`：差分并列开放，不以测试通过冒充等价。
- **整体**：`S0-closed-on-local-slice; C03-root-cause-still-open`。

S0 是纵向切片闭环，**不等于 C03 原厂运行时根因闭合**。`B0` 仍 `open`、`B1–B3` 仍 `blocked`；差分 `45/971486` 对 `47/973202` 仍开放，仍缺区域对象、45 元素向量与图像原始缓冲。

## 回归与边界

**11 个回归 exitCode=0**：C01（18）、BranchModule（1）、Canvas（13）、FrameworkImageBufferChain（8）、DocumentHistoryHostChecks16（13）、CalibrationBoard.EnclosingChecks（90）、CalibrationBoard.ResetChecks（19）、ContourMatch.ResultChecks（24）、ExecutionViewChecks11、CoordinateModule.CandidateChecks（74）、Fixture.PluginChecks（43）。预期失败路径中的 `Failed/…` 是断言用例，不计为 FAIL。

环境修复（均为非产品改动）：4 个工程单独 `dotnet restore` 补齐 `project.assets.json`；创建 `analysis/calibration-board` 报告输出目录；`ExecutionViewChecks11` 重新 `dotnet build` 后程序集加载恢复。

**2 个回归因原厂外部数据前置不可执行，不记为通过、不伪造**：`DistortionCalibration.PluginChecks` 需设备采集的 `fixture.bmp`；`GrayMatch.ScoreArithmeticChecks` 需 native scalar arithmetic oracle JSON。两者均在本机缺失，登记为外部证据依赖。

REA native provider 仍不可用；本批未配置引擎、未新取 native 函数体、未改 `src/` 产品代码、未提交。

证据引用 24 条，全部带 SHA-256 与字节数，见证据卡 `evidence[]`。