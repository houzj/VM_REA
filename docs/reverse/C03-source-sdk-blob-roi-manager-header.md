# C03 原厂 Blob ROI 管理接口

安装版 `IVmBlobFind.h` 的证据写入 `analysis/full-audit/batch1-20261107/C03/source-sdk-blob-roi-manager-header.json`。

公开 SDK 的 `BlobFindRoiManager` 明确提供四个执行期输入接口。`SetRoiRectangle(RectBox*, unsigned int)`、`SetRoiAnnulus(Annulus*, unsigned int)`、`SetBlindPolygonString(StringData*, unsigned int)` 和 `SetMaskImage(ImageBaseData)`。同一头文件还确认 Blob 参数通过 `SetInputImage(ImageBaseData*)` 设置输入图像，并通过 `GetModuRoiManager()` 取得 ROI 管理器。

这些方法只对当前执行有效。头文件没有定义盲区字符串编码、坐标变换、native 字段偏移或 `GenerateMaskImage` 内部实现，因此只用于约束公开参数边界。
