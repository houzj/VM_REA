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

已取得原厂 SDK 的本地 SN、读前切换、选中列表顺序、缓存和 SDK 路径提交运行证据；随仓库 MONO8 样本的像素哈希与重建一致。独立实现已补首 N 路径缓存、可取消间隔和模式参数行，60 项检查通过。完整模式 UI、通用解码、相机/RGB 服务、触发、拼接及原生保存格式仍未验收；末张停止和目录监视不再使用错误替代行为。详见 [图像源记录](IMAGE-SOURCE.md) 和 [公共接口需求](../CONTRACT-REQUESTS.md)。
