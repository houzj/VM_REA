# 第六轮 J01：原版“标定”十工具身份逐项核对

范围只含当前目标的“标定”分类十项；不重复审核公共 `ToolBoxData` 加载调用链（见 `20261020-pass6-calibration-catalog-host.md`），也不把身份对上当作算法、端口、配置或执行链审核完成。原始证据直接读自安装版 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/<目录>/ToolItemInfo.xml`、同名主 XML 与二进制；未改安装件。取证脚本与含十行各原件 SHA-256、帮助 GUID、优先级、SDK/Control DLL 的完整机器清单保存在忽略目录 `analysis/joint-audit/pass6-calibration-identity/{collect.ps1,identity.json}`。这两份是本次新增产物，不是先前摘要推测。

| 当前目录显示 | 原安装目录 / 主 XML `ModuleName` | 原 `ToolItemInfo.name` | 安装版代码载体 | 当前独立插件 ID |
|---|---|---|---|---|
| N点标定 | `IMVSNPointCalibModu` | `IMVSNPointCalibModu` | `IMVSNPointCalibModu.dll`、`IMVSNPointCalibModuCs.dll`、`IMVSNPointCalibModuControl.dll` | `vision.n-point-calibration` |
| N图像标定 | `IMVSNImageCalibModu` | `IMVSNImageCalibModu` | 同名前缀 `.dll`、`Cs.dll`、`Control.dll` | `vision.n-image-calibration` |
| 标定板标定 | `IMVSCalibBoardCalibModu` | **`CalibBoardCalib`** | `IMVSCalibBoardCalibModu.dll`、`IMVSCalibBoardCalibModuCs.dll` | `vision.calibration-board` |
| 相机映射 | `IMVSCameraMapModu` | `IMVSCameraMapModu` | 同名前缀 `.dll`、`Cs.dll` | `vision.camera-mapping` |
| 映射标定 | `IMVSMapCalibModu` | `IMVSMapCalibModu` | 同名前缀 `.dll`、`Cs.dll` | `vision.map-calibration` |
| 畸变标定 | `IMVSImageCalibModu` | `IMVSImageCalibModu` | 同名前缀 `.dll`、`Cs.dll` | `vision.distortion-calibration` |
| 平移旋转标定 | `TranslationCalibModu` | `TranslationCalibModu` | 同名前缀 `.dll`、`Cs.dll`、`Control.dll` | `vision.translation-calibration` |
| 旋转标定 | `RotateCalibModu` | `RotateCalibModu` | 同名前缀 `.dll`、`Cs.dll` | `vision.rotate-calibration` |
| 坐标系 | `CoordinateModule` | `CoordinateModule` | `CoordinateModule.dll`、`CoordinateModuleCs.dll`、`CoordinateModuleCpp.dll` | `vision.coordinate-module` |
| 标定加载 | `ReadCalibFileModu` | `ReadCalibFileModu` | 同名前缀 `.dll`、`Cs.dll`、`Control.dll` | `vision.read-calib-file` |

原件身份：每个安装目录均实际存在 `ToolItemInfo.xml`、`<目录名>.xml`、同名原生 DLL 和 `Cs.dll`；四项另有同名 `Control.dll`，坐标系另有 `CoordinateModuleCpp.dll`。十份主 XML 的 `Base/Items/String[@Name='ModuleName']/CurValue` 全等于各自目录名；原 `ToolItemInfo.name` 九项等于目录名，唯标定板不同。十份原 `ToolItemInfo` 均有 `helpUrl` GUID，逐项值及原件 SHA 在 `identity.json`。原 priority 为 0–8，其中坐标系和标定加载均为 8；不自行改成唯一优先级。没有从目录名或工具箱显示名倒推原版固定数值实例 ID。

当前映射：`src/VisionEditor/tool-catalog.json` 工作区快照的十行 `Id` 均等于上述原安装目录；其 `ModuleId` 分别对到右列独立插件 ID。对应 ID 定义分别位于 `src/VisionPlugins.Calibration/NPointPlugin.cs`、`NImageConfiguration.cs`、`ReadCalibFilePlugin.cs`、`RotateCalibrationPlugin.cs`、`TranslationPlugin.cs`，`src/VisionPlugins.CalibrationBoard/CalibrationBoardPlugin.cs`、`src/VisionPlugins.CameraMapping/CameraMappingPlugin.cs`、`src/VisionPlugins.MapCalibration/MapCalibrationPlugin.cs`、`src/VisionPlugins.DistortionCalibration/DistortionCalibrationMetadata.cs`、`src/VisionPlugins.CoordinateModule/CoordinateModulePlugin.cs`。这些 `vision.*` 是**当前重建版身份**，不是原厂 XML 中的 ID 或同名 ABI。`HEAD` 的目录仅八项有非空 `ModuleId`：N图像标定、相机映射的映射来自当前工作区**未提交**改动，不列为已交付基线。第六轮目录宿主探针另证明当前工作区十个插件可实例化，但不证明原版运行同一性。

证据边界：本报告完成十项**静态入口身份**核对，尚未逐项核原厂 SDK 动态创建后实际数值实例 ID、工具箱点击到 SDK `strModuleName` 的每条运行态路径，亦未核十项 UI/算法与结果链。标定板 `ToolItemInfo.name` 差异尤其说明不能把三个原始标识强行视作一个字段；后续若工具箱选择失配，须以原 `ToolBoxItemData` 构造参数和 SDK 创建请求核实。以上缺口属于当前目标后续 J01/J02 与各工具独有链，不因本报告关闭。

复验：`pwsh -NoProfile -File analysis/joint-audit/pass6-calibration-identity/collect.ps1`，逐项比对产出 JSON 的 `ToolName`、`XmlModuleName`、`CatalogId`、`PluginId`、哈希和 DLL 存在性；脚本仅读安装件与当前目录，将 JSON 写到本线忽略证据目录。本次 `identity.json` SHA-256 `57AADBBE06D6B47153DC88CD7F234E44F32871D917454A731D1AE449C384E561`；`collect.ps1` SHA-256 `F9C0A74C6E0C1AC22E15CD50DF037D969511627AFEB5ABBF8CBB959A71AFF7DE`。
