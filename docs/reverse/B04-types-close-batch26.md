# B04.3 原生类型/复合叶/累积门槛复核（第二十六轮）

- 基线：`ea64cb0e573a9e658e656c3eebc951e9daf8a514`（与本轮主线基线一致）
- 范围：B04.3；只审查 main 的原生类型、pointset/POLYGON/Region/Components 叶与跨轮累积，未访问 `tool-plugins`。
- 结论：**实施增量 +0，B04.3 仍未闭合**。本轮没有可由 main 单独安全实施并完成自审核的源码单元；不把独立托管容器或拒绝路径冒充原生 ABI，也不重复计入已完成 B04.1/B04.2。

## 已核对的源码证据

1. `src/VisionFramework/Contracts.cs` 的 `DataKind` 暴露 `Components`、`Region`、`Fixture`、`Point` 等托管种类；`PortValue` 对这些值执行托管对象校验。该文件没有原生 `pointset`/`POLYGON` 载荷结构、指针所有权、float 打包或 SDK 句柄。
2. `src/VisionFramework/PortArrays.cs` 的 `PortTypes.Valid` 仅允许 `Integer/Number/Text/Circle/Point/Fixture` 作为数组；`Element`、`JoinArray` 对未知复合种类明确抛出 `Unsupported array type`。这证明本地拒绝边界，但不是原生产者到 Group 的 ABI 实现。
3. `src/VisionFramework/CompoundOutputFields.cs` 明确注释为 managed projection；当前只投影已证的 Components 数值列及 Rectangle/Annulus Region 数值列。Polygon、pointset、bytearray、混合叶模式没有被静默映射。
4. `src/VisionFramework/PolygonRuntimeValues.cs` 是独立 managed `PolygonResult/PolygonBatch`：保存点数、扁平 X/Y、坐标空间与 roundCounts，做长度/有限值/上限校验并支持 JSON 往返。它没有声称 `IVmROIPolygon` 或 native `IMVS_MODULE_*` ABI。
5. `src/VisionFramework/GroupBreak.cs` 的 `Accumulatable` 只包含托管已建模的 primitive/Circle/Point/Fixture；`Components/Region/Polygon` 不会被误纳入 Group 累积。

## 原始反编译证据与缺口

已有 `analysis/decompiled` 反编译源显示原 VM/SDK 具有独立的 `DynamicInput.SetInputPointset`、`DynamicInput.SetInputPolygon`、`DynamicOutput.GetOutputPointset/GetOutputPolygonArray`、`IMVS_SetInputPointset`、`Polygon`/`ByteArrayData` 等入口。`DynamicInput.SetInputPolygon` 依据 `ROIPOLYGON`，并以每个多边形点数及扁平 X/Y 组成 SDK 数据；`DynamicOutput` 通过 `IVmROIPolygon.Num/Points` 重建多边形。与此同时，`DynamicObjectConfigControl.IOPickData` 将 `pointset`、`byte` 与 image/int/float/string/double 分列，`ProcessGroupOutputV44` 的反编译记录 pointset/bytearray 分派和清理分支。

这些证据只建立了原程序集中的入口/载荷候选，尚未取得同一真实生产者的完整 producer→subscription→Group→storage→result 调用链、`ProcessGroupOutputV44` 叶序列化 ABI、float/packing/所有权及失败/中断清理语义。没有这些证据，向 `DataKind`、`PortArrays` 或 `GroupOutputAccumulator` 添加 native pointset/POLYGON/Region/bytearray 运行路径会是猜测，违反源码级重建要求。

## 自审核判定

- 原生 producer：未闭合（只有反编译入口，缺真实工具生产者和 payload contract）。
- 订阅/Group：未闭合（当前 main 只消费已建模托管类型；未知类型显式拒绝）。
- 存储/结果：未闭合（没有 native ABI/所有权/序列化证据）。
- 当前独立实现：`PolygonBatch` 的 JSON/跨轮托管容器可单独使用，但属于独立合同，不计 B04.3 原生实现。
- 因此本轮不改产品源码、不新增伪造测试，不增加实施进度。

解除条件：取得 FCR-023 所要求的真实原生生产者/载荷合同（字段、基数、float/packing、所有权、错误/中断清理），并能在 main 责任边界内逐入口落地后，再拆分可实施单元。

## 审计索引

- `analysis/full-audit/batch26-20261020/B/b04.3-gate.json`
- 本报告 SHA-256 由批次整合时写入；本目录只含本线证据。
