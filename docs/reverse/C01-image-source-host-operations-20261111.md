# 普通图像源主界面操作链交付

本批从已确认的 `ImageListPanelBase` 源码链收口普通图像源：加入本地图像、目录导入、拖入、单项删除、清空、回到首图、选中模式和实际图像源执行。没有启动桌面，也没有执行原厂 SDK 或设备。

原源码确认：原厂接受 `.bmp/.jpeg/.jpg/.png/.tif/.tiff`；目录遍历按路径长度再名称排序，递归当前目录下一层；拖入只接受存在且扩展名合格的文件；删除通过 `DeleteImage` 写用户数据，清空通过 `ClearImage`；`FirstImage_Click` 按全部/选中列表和 `AutoPlay` 选择首项或末项，并置回首图标志；真实运行使用 `CurrentImagePath`，回调后刷新当前项。

当前主线将这些规则落在 `src/VisionEditor.Core/ImageSourceListOperations.cs`，主窗口接入 `MainWindow.ImageListOperations.cs` 和 `MainWindow.xaml`。目录导入、拖入和删除均经过同一参数快照更新，清理运行结果并保留文档 Undo/Redo；图像缩略图走插件的 `IToolImagePreviewProvider`，不再直接绕过插件解码器读取文件。

新增 `tests/ImageSource.HostOperationsChecks`，以实际 `ImageSourcePlugin`、`GraphExecutor`、BMP/JPEG/PNG/TIFF 解码器和 `GraphEditorDocument` 做无界面检查。当前专项 **38 项通过**，覆盖六种扩展名、下一层目录、深层排除、路径排序、重复路径、拖入拒绝、首图/AutoPlay/运行选中、实际宽高输出、删除/清空、空列表失败、Undo/Redo 和保存重开。输出位于 `analysis/full-audit/image-source-host/run-37da5edb78a14ec68355e2fe3cc93193/checks/summary.json`。

这批是普通图像源的可操作链，已编译到用户指定的 `src/VisionEditor/bin/Release/net10.0-windows`。同轮 C02 生命周期回归更新为 **11 套、462 项**，共享配置回归为 **11 套、3679 项**；这两项分别验证公共调度和配置合同，不计成工具数量。它不等于 162 个工具完成，也不覆盖相机/SDK 图像提供器、远程路径、原厂后台缩略图线程、逐像素桌面差分。相机、SDK、物理设备入口仍按证据单列。
