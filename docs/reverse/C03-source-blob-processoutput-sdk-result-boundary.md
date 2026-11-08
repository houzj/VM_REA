# C03 ProcessOutput 与 SDK 结果边界

`CIMVSBlobFindModu::ProcessOutput` 的静态证据写入 `analysis/full-audit/batch1-20261107/C03/source-blob-processoutput-sdk-result-boundary.json`。

源码显示 ProcessOutput 从缓存 SDK 工具取得结果对象，调用结果 vtable `+0x08` 并把返回值发布为 `BlobNum`，调用结果 vtable `+0x28` 并把返回值发布为 `TotalArea`，随后按开关发布 BlobImage、BinaryImage、BlobStatus 和几何向量。

因此 VM 可见的 BlobNum/TotalArea 是 SDK 结果对象的直接投影。LimitJudge 位于此前的独立阶段，不能解释这两个数值。当前 45/971486 差异已经锁定在 ProcessAlg 输入、ROI/Mask、图像转换或 SDK 结果生成之前。
