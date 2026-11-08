# C03 LimitJudge 输出边界

`CIMVSBlobFindModu::LimitJudge` 的静态证据写入 `analysis/full-audit/batch1-20261107/C03/source-blob-limitjudge-output-boundary.json`。

源码确认 LimitJudge 遍历原始 Blob 数量，计算面积、质心、中心、角度、包围盒、周长、轴、圆形度和矩形度判定位，把每个判定位写入模块状态数组，并把结果汇总到模块状态。它不删除 Blob，也不修改 BlobNum、TotalArea 或几何向量。`Process` 顺序是 `ProcessAlg → LimitJudge → ProcessOutput`。

因此 VM 的 45/971486 差异仍位于 ProcessAlg 输入、ROI/Mask、图像转换或 SDK 结果路径，不能归因于 LimitJudge 后处理。
