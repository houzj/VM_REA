# 已实现节点源码级自检台账

依据 [源码级逆向与复刻标准](../SOURCE-LEVEL-REVERSE-ENGINEERING-STANDARD.md)。此表只盘点当前工作树中已存在的实现或产品注册，不代表原厂等价或验收完成。原厂证据、独立实现和未知行为必须分开填写。新增工具逆向在本台账中的已实现项审计完成前暂停。

| 工具/节点 | 当前形态 | 产品注册 | 原厂证据及实现映射 | 自检状态 | 手动验收 |
|---|---|---:|---|---|---|
| 图像源 | 插件 | 是 | `docs/plugins/IMAGE-SOURCE.md`；实现位于 `src/VisionPlugins.ImageSource` | 待按新标准逐项重审 | 待核 |
| 多图采集 | 插件 | 是 | `docs/plugins/IMAGE-ACQUISITION.md`；实现位于 `src/VisionPlugins.ImageSource`；`tests/ImageAcquisition.PluginChecks` | 待审 | 待核 |
| 缓存图像 | 插件 | 是 | `docs/plugins/IMAGE-BUFFER.md`；实现位于 `src/VisionPlugins.ImageSource`；`tests/ImageBuffer.PluginChecks` | 待审 | 待核 |
| 光源 | 插件 | 是 | `docs/plugins/LIGHT.md`；实现位于 `src/VisionPlugins.ImageSource`；`tests/Light.PluginChecks` | 待审 | 待核 |
| N点标定 | 插件 | 是 | `docs/plugins/N-POINT-CALIBRATION.md`；实现位于 `src/VisionPlugins.Calibration`；专属及配置检查 | 待审；公共模型需源码复核 | 待核 |
| N图像标定 | 插件 | 是 | `docs/plugins/N-IMAGE-CALIBRATION.md`；实现位于 `src/VisionPlugins.Calibration`；`tests/NImageCalibration.PluginChecks` | 待审；算法差异与配置差异分别核 | 待核 |
| 相机映射 | 插件 | 是 | `docs/plugins/CAMERA-MAPPING.md`；`src/VisionPlugins.CameraMapping`；`tests/CameraMapping.*Checks` | 待审 | 待核 |
| 平移旋转标定 | 插件 | 是 | `src/VisionPlugins.Calibration`；`docs/plugins/TRANSLATION-ROTATE-CONFIGURATION-RECONSTRUCTION.md` | 配置窗口用户初步验收通过；全量源码/行为差异仍待审 | 配置 UI 初步通过 |
| 标定文件读取 | 插件 | 是 | `docs/plugins/READ-CALIB-FILE.md`；`src/VisionPlugins.Calibration`；`tests/ReadCalibFile.*Checks` | 待审 | 待核 |
| 旋转标定 | 插件 | 是 | `src/VisionPlugins.Calibration`；`docs/plugins/TRANSLATION-ROTATE-CONFIGURATION-RECONSTRUCTION.md` | 配置窗口用户初步验收通过；全量源码/行为差异仍待审 | 配置 UI 初步通过 |
| 图像二值化 | 插件 | 是 | `docs/plugins/IMAGE-BINARY-SOURCE-AUDIT.md`；实现 `src/VisionPlugins.ImageBinary`；221 项测试 `analysis/source-audit-regression/20261018/supplemental/RoiTool.Checks.final.stdout.log`；SDK 像素比较 `analysis/source-audit-regression/20261018/native-binary-modes/compare.log` | 源资源与托管调用链已核；UI 五模式、触发字段、原厂范围及整数输入已修；算法仍部分：均值/高斯/OTSU 单夹具候选匹配，Sauvola 未匹配；221 项通过 | 待桌面对照及模式算法闭合 |
| 标定板标定 | 插件项目 | 是（2026-10-19生产注册） | `CALIBRATION-BOARD-FRAMEWORK.md`；实际普通宿主/外置ROI/显示/保存重开/Group 49项通过 | 源XML/普通宿主/ROI及显示映射已追踪；原厂桌面与剩余原生边界开放 | 待本轮人工验收 |
| 畸变标定 | 插件项目 | 否（未见 MainWindow 注册） | `src/VisionPlugins.DistortionCalibration`；`tests/DistortionCalibration.*Checks` | 待审；先核注册/入口边界 | 不适用 |
| 映射标定 | 插件项目 | 否（未见 MainWindow 注册） | `src/VisionPlugins.MapCalibration`；`tests/MapCalibration.*Checks` | 待审；先核注册/入口边界 | 不适用 |
| 图像文件内置模块 | 内置模块 | 是 | `src/VisionModules/BuiltinModules.cs`；模块检查见解决方案测试项目 | 待审 | 待核 |
| 阈值内置模块 | 内置模块 | 是 | `src/VisionModules/BuiltinModules.cs`；ROI、输入图像和显示桥接需复核 | 待审 | 待核 |
| 连通域内置模块 | 内置模块 | 是 | `src/VisionModules/BuiltinModules.cs`；模块检查见解决方案测试项目 | 待审 | 待核 |
| 圆检测内置模块 | 内置模块 | 是 | `src/VisionModules/BuiltinModules.cs`；模块检查见解决方案测试项目 | 待审 | 待核 |
| 数值比较内置模块 | 内置模块 | 是 | `src/VisionModules/BuiltinModules.cs`；模块检查见解决方案测试项目 | 待审 | 待核 |

## 本轮状态

- 统一标准已建立；`tests/Invoke-ConfigurationRegression.ps1 -RunDirectory analysis/source-audit-regression/20261018` 完成，63/63 检查退出码为 0，构建 0 错误、5 条 nullable 警告。五项遗漏的已注册工具专属检查另行补跑，结果 5/5；图像二值化 ROI/宿主检查在本轮更新为 221 项通过。最终专属检查日志见 `analysis/source-audit-regression/20261018/supplemental/RoiTool.Checks.final.stdout.log`。这些是独立实现与已采集夹具回放，不代表原厂桌面等价。
- 产品注册以 `src/VisionEditor/MainWindow.xaml.cs` 和 `BuiltinModules.RegisterAll` 为准；“项目中存在实现”不等于当前宿主可用。
- 本表的“待审”需要替换成证据路径、具体源符号和验证记录后才能关闭。报告不得只填测试通过数量。
- 平移旋转标定与旋转标定配置窗用户反馈目前“看着没有太大问题”；本项配置 UI 记录为初步通过，不能代表原厂源码/运行行为审计已完成。模式、编辑器事件及持久化逐项追踪仍需登记。
- 暂无设备测试计划；设备相关原生行为如无既有原厂采集，按“未确认”记录。
- 图像二值化原厂配置资源与 SDK 参数封装已完成首轮逐项核对，原厂 UI 五种模式现均可选且运行字段按 XML 触发；独立运行逻辑仍仅实现硬阈值，受控 SDK 候选及未知项见 [IMAGE-BINARY-SOURCE-AUDIT.md](IMAGE-BINARY-SOURCE-AUDIT.md)。

## 当前宿主 UI 装配路径（代码事实）

`src/VisionEditor/MainWindow.xaml.cs` 注册 12 个插件（2026-10-19加入标定板）。`src/VisionEditor/ToolConfigurationHost.cs` 的分派条件是：只要插件实现 `IToolConfigurationProvider`，宿主就直接创建 `ToolConfigurationModelWindow`；只有没有 provider 时才调用 `IWpfToolEditorFactory.CreateWpfEditor`。因此当前已注册公共模型路径包括图像源、N点、N图像、相机映射、平移旋转、标定加载、旋转标定和标定板；即便这些类型同时实现了窗口工厂，工厂也不会被这个宿主分派。当前走专用工厂窗口的是多图采集、缓存图像、光源和图像二值化。此处是本项目的代码事实，不能据此推断原厂架构也采用相同类层次。

畸变、映射标定项目也有配置模型提供器/工厂代码，但当前 `MainWindow` 未注册它们；全量回归覆盖其项目检查不等于产品节点已可由用户添加。标定板已从此边界移出，其真实宿主候选等待本轮人工验收。后续自检将逐项把原厂 XML/XAML/模板的实例化路径与本项目选择公共模型或专用窗口的理由对应起来，避免因“统一模板”而抹掉原厂专用行为，也避免因“专用窗口”绕开原厂共享模板。
