# C03 原厂 SDK 阈值/极性有限差分

使用安装版原厂 C++ SDK、确认的 `1.jpg` 输入、8 连通、面积范围 10 到 9,999,999，对 `ThresholdType=0..5` 和 `Polarity=0/1` 做了 12 组有限差分。结果写入 `analysis/full-audit/batch1-20261107/C03/param-sweep/threshold-polarity-sweep.json`。

12 组中没有得到原厂 UI 已确认的 `BlobCount=45, TotalArea=971486`。`ThresholdType=0/2, Polarity=0` 均为 47/973202，`ThresholdType=3` 为 48/770199，其余组合也没有同时匹配目标计数和面积。临时二值图和 Blob 图在记录结果后已删除，目录只保留 1,148 字节的 JSON 差分结果。

这是独立 SDK 参数边界证据。它排除了这组公开阈值类型和极性组合直接解释当前 VM 结果的可能性，但不能单独定位 VM 的 ROI、图像转换或隐藏参数差异。
