# G8 原厂宿主与直接 SDK 首处分歧审计

日期：2026-11-12

## 当前可复核基线

直接 SDK 在同一 `1.jpg`、同一参数和独立进程下重复得到：

- 默认模式：47 个 Blob，总面积 973202。
- 全屏矩形模式：45 个 Blob，总面积 971486。
- 两次运行的结果向量和 Binary/Blob 缓冲哈希一致。

原厂历史宿主记录为 45 / 971486 与 47 / 973202，但缺少 `test1108.sol`、宿主 Run 前输入缓冲、活动 ROI/Mask 集合和完整结果向量，因此只能确认输出存在两种历史状态，不能把其中任一状态归因到某个实现分支。

## 静态调用链

```text
CIMVSBlobFindModu::ProcessAlg
  → input image + ROI/Mask state
  → GenerateMaskImage candidate (vtable +0x48, conditional)
  → SDK SetInputImage / SetROI / Run
  → GetResult / BinaryImage / BlobImage
  → ProcessOutput
```

已确认的首处分歧候选按证据强度排序：

1. 宿主上游 `VM_M_GetImage` 解码后的实际像素、行距和格式与直接 SDK 文件解码不同。
2. `RoiType` 两字节载荷在宿主代理中生成显式全屏矩形，直接 SDK 默认运行未调用同一宿主 ROI 编排。
3. 多集合或外部屏蔽区触发 `GenerateMaskImage`，其第五参数和 `PreproMask` 区域集合改变 SDK Run 前输入。
4. `OutputMaskImage`/共享缓冲发布只影响输出投影，不能先验解释 Blob 数差异。

## 当前判定

| 项目 | 判定 |
|---|---|
| 直接 SDK 身份和重复性 | runtime-confirmed |
| Blob 模块 PE/ABI 静态链 | source-confirmed |
| 历史宿主输出差异存在 | historical-observed |
| 首个可重复运行时差异 | 运行未确认 |
| 当前托管 Blob 实现可替代原厂宿主 | 禁止宣称 |

## 解除条件

需要在匹配原厂安装和方案环境中取得：

- `test1108.sol` 或用户明确的新建等价方案身份；
- 宿主加载的 EXE、PlatformManager、ModuleFrame、ModuleProxy 哈希；
- `VM_M_GetImage` 返回的宽高、格式、行距和像素 SHA-256；
- `ProcessAlg` 前两组 ROI/Mask 条目及顺序；
- SDK Run 前后输入/输出缓冲和完整结果向量。

这些条件满足后，才能把候选 1–3 中的第一项定为首处分歧并实施最小修复。当前不修改 Blob 私有算法，也不把直接 SDK 数值写成原厂宿主行为。

## 原件与复跑入口

- `docs/reverse/C03-same-input-differential-20261110.md`
- `docs/reverse/C03-source-blob-generatemask-region-bridge.md`
- `analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-4dd306f7d33a447dba8e0793ad073807/summary.json`
- `tests/NativeEvidence/C03/Get-HostIdentityPreflight.ps1`
