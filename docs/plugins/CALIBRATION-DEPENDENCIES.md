# 标定工具并行依赖取证

2026-10-17。本专项仅补齐其余标定工具的静态源码、资源和依赖清点，不实现产品、不执行设备或原厂桌面运行。N点、N图像分别由其他工作项推进；坐标系仅登记文件和导入依赖，按用户要求最后开发。

最新任务设计按 [插件职责与配套表](../PLUGIN-DEVELOPMENT-BOUNDARY.md) 执行：工具、公共节点框架和主项目接口必须同时安排。本文件的静态取证范围不替代配置/调试/ROI/显示任务；N点/N图像仍最高优先级，坐标系最后。

## 证据与复查

原厂安装目录：`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration`。机器可读清单 [calibration-dependencies.json](calibration-dependencies.json) 记录 8 项、98 个原厂文件的绝对路径、长度、SHA-256，以及 153 个 AlgorithmTab 直接控件。声明保留嵌套组合数据，避免把姿态/点数组当成单标量。计数是 XML 直接 Items 子节点数，不能当成原厂实际运行端口数。

复现生成器：`tests/Export-CalibrationDependencies.py`，只读目标文件、解析 XML 和 PE 导入表，不装载或执行目标。辅助反编译由本机 ilspycmd 生成于本工作树 `analysis/full-audit/calibration-dependencies/decompiled/<模块>Cs`，两个实际存在的专属 Control 程序集也单独反编译；生成器记录逐个 C# 文件哈希、公开属性和方法名。原始生成目录不纳入 Git。

REA 对 `ReadCalibFileModuControl.dll` 的静态托管检查证据：`ev_ca66bc7e6c4cc7a9571f53344daf7263f2ee1dc9fa37fd17dc18320bb536c94d`，SHA-256 `9c5c2c529b5242f9735a51def23c30fe90b2e3cbfe16c1ff31f95d0f1c129300`。确认 .NET Framework 4.6.1 / AnyCPU / CIL，元数据完整；只确认托管身份、引用和资源，不代表 CLR 加载或执行成功。原始返回保存在本工作树分析目录 `rea-read-calib-control.json`。

## 模块对照

下表输入/输出是主声明 XML 中 Category 的直接 Items 数量，括号内为 UI 控件数。

| 工具 | 原厂模块 | 输入/输出（控件） | 已确认原生算法依赖 | 文件选择资源 |
|---|---|---:|---|---|
| 标定板标定 | IMVSCalibBoardCalibModu | 1/16（26） | MVDCalibBoardCalibCpp | `.iwcal` |
| 相机映射 | IMVSCameraMapModu | 16/10（16） | MVDCameraMapCpp、MVDMatrixToParamCpp | `.iwcal` |
| 畸变标定 | IMVSImageCalibModu | 2/10（30） | MVDImageCalibCpp | `.iccal` |
| 映射标定 | IMVSMapCalibModu | 9/17（36） | MVDMapCalibCpp | 输入 `.xml;.iwcal`，保存/加载 `.xml` |
| 标定加载 | ReadCalibFileModu | 10/12（13） | ModuleFrame；未直接导入专用 MVD 算法 DLL | `.xml` |
| 旋转标定 | RotateCalibModu | 5/7（9） | MVDNPointCalibCpp | 资源原文 `.xml\|*.xml;*.iwcal` |
| 平移旋转标定 | TranslationCalibModu | 6/28（23） | MVDNPointCalibCpp | `.xml`，点列表编辑器另有 `.txt` |

每个结论均可回查 `<模块>/<模块>.xml` 的 `Category[@Name='Input'/'Output']/Items`、`<模块>AlgorithmTab.xml` 的 `FileOption/FilterName`，以及生成器解析的原生 `<模块>.dll` 导入目录。文件过滤器只说明 UI 接受的扩展名，不证明格式相同或可互换。

坐标系 `CoordinateModule.dll` 导入 `MVDMatrixToParamCpp.dll`，其余基础依赖见 JSON；未深入配置语义，不据此改变最后开发的优先级。

## 可共享的代码边界

1. **标定结果不能仅表示为单个矩阵。** 标定加载 XML 明确包含 `CalibMatrix`（float 数组）、`RotCenterImage`/`RotCenterWorld` 等 POSTURE 组合；其 SDK `ReadCalibFileResult.cs` 公布 `List<float> CalibMatrix`。平移旋转声明还包括图像/物理点数组、旋转中心、方向、图像和世界误差、比例和平移旋转参数。矩阵维度、行列顺序、坐标约定、单位尚未在本专项恢复，不能从 List<float> 推断为固定 3×3。
2. **加载器支持运行时结果结构。** `ReadCalibFileModuControl/Apps.XmlParser.ParamTab/ReadFileConfigData.cs` 的 `ReadData(bool needAddoutput=false)` 从 `((IUserBytesData)vmModule.Params)[$"Get{NameA}"]` 取得 UTF-8 文本；本工具 XML 的控件名为 `CalibInputParam`。`AnalyzeString` 先以 `;` 分类型段，再以 `#` 分类型和正文、`$` 分条目、`,` 分名称和值，姿态内部以 `~` 分 X/Y/R。类型分支为 `int`、`float`、`string`、`CalibPointList`、`CalibPointF`、`FloatList`。这是原生返回给 UI 的参数描述协议，**不是磁盘 XML 的序列化格式**。
3. `ReadData(true)` 倒序删除模块现有 `DynamicVmIO` 和 `Combination` 后，通过 `IOutput.AddOutput` 重建输出；`ReadData()` 默认不重建。不能仅依据读取显示调用，就声称执行图的输出结构已更新。`OpenFileDialog4ReadFile.ValueChangedCmdExecute` 先调用基类，再调用其私有 `ReadData()`，由后者调用该显示读取方法。
4. **平移旋转专属点表可复用交互基础，但字段不能照抄 N点。** `TranslationCalibModuControl/TranslationCalibModuControl/NPointList.xaml.cs` 的 `Init` 读取 `CalibPointTotalNum + RotPointTotalNum` 行，每行 `ImagePointX/Y`、`WorldPointX/Y`、`DeltaAngle`；展示采用当前文化 `float.Parse(...).ToString("f3")`。`GetPointListText` 验证五个 float，按五列、空格及 CRLF 导出；`GetPointListByText` 按 CRLF/空格拆分，仅处理现有数量范围。导入异常、缺列处理及浮点文化需要后续受控运行对照，不能用一个宽松 CSV 导入器代替。
5. **专属 UI 与公共编辑器分开。** 本范围实际存在 `ReadCalibFileModuControl.dll` 和 `TranslationCalibModuControl.dll`；其余五项安装目录没有同名前缀 Control.dll，主要配置在 AlgorithmTab XML。它们仍使用 `OpenFileForCalibDialog`、`ImageViewConfig`、点组合选择器、ROI 及条件显示等公共控件。不能把无专属 Control.dll 理解成没有 UI，也不能只复制面板截图而省略图像/ROI 交互。

## 建议接口准备及后续顺序

主线负责在 CONTRACT-REQUESTS.md 统一登记和落地，避免多代理并行修改公共合同：

- 标定结果载体需显式携带模型种类、矩阵系数及其约定、点/姿态数组、误差和状态；在原生验证前允许未知维度，不预先固化单位或矩阵顺序。
- 标定文件服务要区分 `.xml`、`.iwcal`、`.iccal` 及点表 `.txt`；读写原子性、刷新、方案内缓存矩阵与文件外部变更要由运行证据确定。ReadCalibFile 的 `SaveCalibDataEnable` 是明确独立参数。
- 点表配置需要行增删/采样、不同字段集、导入导出、订阅写入与撤销生命周期；UI 点和姿态组合与独立 X/Y 分量订阅均在 XML 中存在。
- 动态加载结果需要配置变更后端口重建、订阅失效检查和方案持久化，而不能只提供任意字符串字典。

保持用户最高优先级 **N点标定 → N图像标定**。建议之后先推进共享 `MVDNPointCalibCpp` 的平移旋转和旋转，再做标定加载的读写互通，接着标定板、相机映射、映射、畸变；坐标系最后。这是基于依赖的建议，不是已确认的执行先决关系。标定加载的文件结构调查可提前作为 N点保存加载的验证辅助，但不能以本静态清单宣称格式兼容。

## 未验证范围

本专项没有运行原厂算法、采集原厂 UI、核对像素或标定数值、读写真实标定文件、验证设备。原生求解、退化条件、误差定义、矩阵系数顺序、刷新与缓存策略、跨工具文件互通仍待各工具的受控运行证据。此报告仅闭合并行依赖清点，不构成七项工具实现完成或原生等价声明。
