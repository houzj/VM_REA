# C03 原厂 SDK ROI/Mask 预处理顺序

`analysis/full-audit/image-binary-sdk-tool.cs` 的托管源码证据写入 `analysis/full-audit/batch1-20261107/C03/source-sdk-roi-mask-preprocess.json`。

源码确认通用图像二值工具在脏状态下先设置 PreproMask 输入图像，清空 `RegionListEx`，把主 ROI 以 `MvdRegionTypeRoi` 加入，再把每个 Mask 以 `MvdRegionTypeMask` 加入，执行 PreproMask 后清除脏标志。Mask 支持多边形、矩形、圆弧扇区和圆形。结果阶段从 `RegionImageAssistant.ROIList` 读取 ROI 数量，并通过 `CMvdInnerTool.ConvertMVDShpae2AlgROI_X64/X86` 转换为算法 ROI。

这条证据用于解释 C03 的 ROI/Mask 数据流边界。它来自公开 SDK 图像工具源码，不能直接证明 `CIMVSBlobFindModu` 的内部坐标变换或具体过滤公式。
