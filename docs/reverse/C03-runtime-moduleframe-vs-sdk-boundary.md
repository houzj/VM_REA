# C03 VM ModuleFrame 与直接 SDK 参数边界

原厂运行时导出包含 108 个 ModuleFrame 参数。直接 C++ SDK 探针只覆盖 `ThresholdType`、阈值、极性、连通性、面积和查找数量等算法核心参数。运行时还确认了 `SelectByArea=True`、LimitJudge 状态、`ROIAreaType`、`OutputMaskImage`、`FixtureInfo` 和输出图像开关等 wrapper 状态。

边界数据写入 `analysis/full-audit/batch1-20261107/C03/runtime-moduleframe-vs-sdk-parameter-boundary.json`。因此此前的 SDK 阈值/面积差分只能排除算法核心参数的有限组合，不能把 VM wrapper 的筛选、ROI、输出状态当作已经覆盖。
