# 图像源到图像二值化的普通工具链

本批把已经接入主窗口目录的图像二值化工具接到图像源操作链之后，范围是已有源码能够确认的硬阈值执行、可选掩膜输出、颜色输入转换门控、图像结果显示以及方案保存重开。

`ImageBinaryPlugin` 的源码证据确认了这些公共契约。`Image` 是必需的 Mono8 输入。`InputMask`、区域和屏蔽区域是可选输入。硬阈值使用 `LowThreshold` 和 `HighThreshold`。`OutputMaskImage` 决定是否发布 `OutMask`。运行结果始终发布二值 `Image` 和 `ProcessedPixels`。当前 Runtime 对 `BinaryType` 只实现 1，其他原生模式会返回明确错误。均值、高斯、Sauvola 和 OTSU 的 XML 参数与原生 SDK 证据已经登记，算法实现仍需各自的 native 证据。

主窗口已经注册 `vision.image-binary`，节点配置使用 `ImageBinaryWindow`，结果沿通用 `ToolDisplayFrame` 和图像预览显示。图像源到二值化的连线、单次执行和保存重开都由 `GraphExecutor`、`GraphEditorDocument` 与现有插件注册表处理。

无界面复核使用以下两个已有生产测试项目，构建和运行均成功。

- `FrameworkPluginChain.Checks` 通过 4 个图像源到二值化图案例，覆盖 Mono8、RGB 输入拒绝、RGB 转 Mono8 和禁用转换后的上游跳过。
- `FrameworkImageTypeGroup.Checks` 通过 7 个真实图执行案例，覆盖直接图、颜色输入、重开、可选 `OutMask` 关闭和开启，以及 Group 消费。

汇总位于 [image-binary-host 回归](../../analysis/full-audit/image-binary-host/run-fc9f273c00c748fcb9d67163c0bf5aa8/summary.json)。这些案例没有启动窗口、加载原厂 SDK 或执行设备。

本批没有将四种未实现 BinaryType 接口标为完成，也没有将单一硬阈值路径称作原厂算法等价。可见主界面变化集中在图像源列表操作和二值化结果预览。原厂二值化窗口的像素布局、native 模式算法、完整 ROI 模板与设备路径仍按证据继续推进。
