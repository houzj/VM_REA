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

只有 VisionPlugins.ImageSource 已作为独立插件注册。工具箱 ModuleId 映射和 VisionModules 内置算法不代表原插件等价实现。catalog.csv 中的 native_analysis/runtime_comparison 是原清点时状态，后续图像源证据见 IMAGE-SOURCE.md，不能把旧列解释为当前全部证据状态。

## 首个交付：图像源

源码已确认：枚举值、参数范围、模式显隐、执行全部/执行选中、原生文件转换及目录监视入口。独立实现：本地 BMP 灰度数据读取、顺序/循环/停止、SN、输出、持久化、WPF 参数编辑。原生未核对：解码逐像素等价、缓存/拼接、相机/SDK、触发、原生保存格式和全部模式界面。详见 IMAGE-SOURCE.md 和 ../CONTRACT-REQUESTS.md。
