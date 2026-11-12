# C01 真实图像工具消费回归

日期：2026-11-12

## 通过项

`CalibrationImageProducer.Checks` 通过，覆盖：

- ImageSource Gray8 图像进入 NImage/CalibrationBoard 检查图并得到 99 个点；
- 标定板发布独立点列表和标定对象；
- `iwcal` 36 字节 float32 输出及 XML loader 对错误二进制的拒绝；
- NImage XML 九浮点矩阵重新发布为数值数组并传入 Coordinate；
- 三条类型化数据链和 NImage XML 路径保存重开后保持。

## 证据缺口

`CalibrationMapImageChain.Checks` 与 `MapCalibration.ImageExecutionChecks` 因依赖工具插件工作树中的 `analysis/full-audit/n-image-calibration/solver-fixture/frame-0.bmp` 缺失而未执行。该路径属于工具线证据资产，当前不复制或伪造；待工具线提供固定资产后复跑。

原厂 native 算法和设备输入仍标记“运行未确认”。
