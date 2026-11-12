# G9 DistortionCalibration 工具批次

日期：2026-11-12

本轮串行复核通过：

- Configuration 16；
- Encoded68 24；
- File 89（含原始 SDK 二进制/文件生命周期）；
- FixedKernel：Board7 固定拟合边界 34/34，7ff3c0 36 次调用与 324 个浮点字段 bit-exact；
- Mean：164/164 均值、11852 个点字段与 1284 个 MatrixToParam 字段 exact；
- PointAlias 14；Radial 48；RadialFile 12 组原始点集；Recognition 17（12/12 原始输入识别）；Workspace 原始中位数回放通过。

CenterInput 和 RotatedRoi 因输入 fixture 缺失保持开放。以上结果覆盖源码/托管和已有原始样本证据；native 完整执行、原厂方案字段和桌面像素差分仍为“运行未确认”。
