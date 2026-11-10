# 渐进落地第三批：B1 输出名契约闭合

日期：2026-11-10 · 证据卡：`analysis/full-audit/batch1-20261107/C03/c03-incremental-b1-contracts.json`（`C03-INCREMENTAL-B1-CONTRACTS-001`）

## 边界

本批只做本地静态源码级取证：**未跑方案测试、未改 `src/` 产品代码、未提交、未推断原厂 ABI**。闭合的是第二批 `C03-INCREMENTAL-B1-STATIC-001` 遗留的两个输出名契约缺口。

## G1：9 个 `Components.*` 复合输出名

`src/VisionFramework/CompoundOutputFields.cs` 定义了全部生成规则：

- `ComponentNumbers` 7 项：`Area`、`CentroidPoint.CentroidX/Y`、`BlobRect.RectPoint.BlobRectX/Y`、`BlobRect.BlobRectWidth/Height`
- `Leaves(DataKind.Components)` 另加 2 个标量叶：`BlobNum`、`TotalArea`
- `Describe()` 用 `port.Name + "." + leaf.Path` 生成复合名；`Expand()` / `Project()` 用 `pair.Key + "." + leaf.Key` 展开输出值
- `Project()` 的 `Func<Component,double>[] fields` 依序填面积、中心 X/Y、MinX/MinY、宽/高，与 `ComponentNumbers` 顺序一致

**逐项核对**：C01 报告 13 个 Blob 输出名中，9 个 `Components.*` 与上述 9 个 leaf 拼接结果**集合完全相等**（双向无差集）；其余 4 个为顶层 `BlobNum`、`Components`、`Count`、`TotalArea`。因此「复合名未作为整串字面量出现」的原因是动态投影拼接，不是缺失。

## G2：Group 小写 `width`

`image-source-group-vertical-slice.vmx.json` 的 `Procedures[Group].Configuration.Outputs` 显式声明：

| Name | Kind | Port | Cardinality |
|---|---|---|---|
| `image` | Image | `Image` | Scalar |
| `width` | Integer | `Width` | Scalar |

小写输出名是**方案配置值**，映射到大写端口名；源码中找不到 `"width"` 整串属正常。`GroupDisplayControls.cs` 的 `DisplayKindName(DataKind.GrayImage) => "image"` 是 UI 类型显示名，与方案输出名一致，但不是端口定义。

## 必须保留的推断边界

`CompoundOutputFields.cs` 文件头注释明确：`this is a managed projection, not native ABI`，且 `native field names/ABI are not inferred from these X/Y suffixes`。

所以本批闭合的是**自研 managed 投影规则与本地方案契约**，不是原厂 native 字段 ABI、不是 `Process` 函数体、不是 parity。原厂字段名、首调用目标与运行时输出仍需 REA 补证。

## 状态影响

- 第二批的 `blobOutputs` 口径从「4/13 命中、9 个待取证」更新为「13/13  accounted：4 个顶层字面量 + 9 个投影 leaf」；`groupOutputs` 从「1/2」更新为「2/2 accounted」。
- **C03 根因未闭**：差分 `45/971486` 对 `47/973202` 仍开放，5 项 unknown、区域对象 / 45 元素向量 / 图像原始缓冲仍未捕获；`B0 open / B1–B3 blocked` **不因输出名闭合而解除**。
- 证据引用 8 条，全部带 SHA-256 与字节数；`commands=[]`、`productChanges=[]`。

整体完成度：权威口径仍为 `docs/SOURCE-LEVEL-REVERSE-PROGRESS.json` 的 10/10 = 100%（仅标定授权范围，人工验收单列）；本批不计入该分母。
