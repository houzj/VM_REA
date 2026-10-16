# 工具插件清单

入口：[逐插件逆向任务表](CHECKLIST.md) · [机器可读证据清单](inventory.json) · [声明计数 CSV](catalog.csv) · [P0 图像源记录](IMAGE-SOURCE.md)。

证据：D:/MyWork/REA逆向测试/analysis/full-audit/plugin-coverage.csv、plugin-coverage.json；原始安装目录 Applications/Module(sp)/x64。清点边界是该目录的 162 个插件文件夹（含 Global），不是仅工具箱中可见项。逐插件声明与依赖数量见 catalog.csv。

| 类别 | 插件数 | 原生 DLL | 托管 DLL |
|---|---:|---:|---:|
|Calculation|11|11|12|
|Calibration|10|11|14|
|Collection|5|5|9|
|ColorProcessing|6|8|9|
|Communication|5|5|10|
|DefectDetection|14|14|19|
|Global|17|15|20|
|ImageGeneration|4|4|4|
|ImageProcessing|21|21|26|
|Location|28|29|38|
|Logic|20|29|230|
|Measurement|11|11|12|
|Recognition|4|4|6|
|SplitCombination|6|6|6|

插件目录 DLL 总数 588（173 原生、415 托管）；公共控件 8 个托管 DLL；myLibs 68 个 DLL（4 原生、64 托管）。这是既有静态审计的计数，未重新执行所有插件。

## 实现状态

当前注册图像源、多图采集和缓存图像三个独立工具插件；三者暂共用 VisionPlugins.ImageSource 程序集以复用已验证的解码器和 UI 资源。工具箱映射不代表原生等价。catalog.csv 是初次声明清点状态，后续进度以 CHECKLIST.md 与逐工具证据记录为准。

图像源当前验证与边界见 [IMAGE-SOURCE.md](IMAGE-SOURCE.md)；多图采集的源码、资源、原厂运行、用户桌面截图及实现边界见 [IMAGE-ACQUISITION.md](IMAGE-ACQUISITION.md)。缓存图像的原生FIFO、完整SDK参数区、原始图像载荷与55组像素序列见 [IMAGE-BUFFER.md](IMAGE-BUFFER.md)。公共服务和宿主调整持续记录在 [CONTRACT-REQUESTS.md](../CONTRACT-REQUESTS.md)。
