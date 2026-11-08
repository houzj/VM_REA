# C03 SelectByArea 与 LimitJudge 顺序

源码证据写入 `analysis/full-audit/batch1-20261107/C03/source-blob-selectbyarea-limitjudge-order.json`。

Blob wrapper 的 `SetParam` 将 `SelectByArea` 以及同组的排序选择字段写入底层 SDK 参数存储。该分支没有写入 `BlobAreaLimitEnable`、`BlobNumLimitEnable` 等模块 LimitJudge 状态。`Process` 的固定顺序是 `ProcessAlg → LimitJudge → ProcessOutput`。

因此运行时导出的 `SelectByArea=True` 不能仅凭 wrapper 源码被解释为后处理 Blob 数量过滤器。它首先属于底层 SDK 选择/排序参数，具体算法语义仍需 SDK 本体证据。
