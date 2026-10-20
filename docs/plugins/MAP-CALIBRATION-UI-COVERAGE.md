# 映射标定普通配置与结果显示 source→product 覆盖矩阵

审计范围：普通配置页、算法配置组与结果显示资源及对应托管路径。非设备测试；没有修改共享框架或其他工作树。

## 原厂身份与路径

- 模块定义：`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu/IMVSMapCalibModu.xml`，SHA-256 `228bf45a24691c0cd13c2201e516941f20e6b1e5352478ccb60ed01c774b4f5f`。
- 算法配置：同目录 `IMVSMapCalibModuAlgorithmTab.xml`，SHA-256 `cc91e61790265b3dad51ec37bb87cf65edd6b1fad718630253b390ddc0a46bf2`；与 `src/VisionPlugins.MapCalibration/Resources/MapCalibrationAlgorithmTab.xml` SHA-256 完全相同。
- 结果显示：同目录 `IMVSMapCalibModuDisplay.xml`，SHA-256 `d630451cd94d6d6eb4b1a69f5b9204707b18b083799b6218e4493a84700ca0a4`；与 `src/VisionPlugins.MapCalibration/Resources/MapCalibrationDisplay.xml` SHA-256 完全相同。
- 托管公开类型：`analysis/full-audit/calibration-dependencies/decompiled/IMVSMapCalibModuCs/IMVSMapCalibModuCs/MapCalibParam.cs`、`MapCalibResult.cs`、`IMVSMapCalibModuTool.cs`。`IMVSMapCalibModuTool.ModuResult` 通过 `QueryModuleOneResult` 与 `SetModuleResult` 装配 `MapCalibResult`；工具 `Run()` 则调用 SDK 的 `SyncSelfRun`。

## 覆盖矩阵

| 原厂路径 | 原厂可确认行为 | 当前 Map 产品对应 | 证据/状态 |
|---|---|---|---|
| Basic / `InputType` | 值1输入图像、值2输入文件；原XML trigger 控制两组图像源、ROI、示教控件与双文件/刷新控件的显隐 | `MapCalibrationMetadata.Definition()` 解析原XML；`MapCalibrationPlugin.CreateConfiguration()` 使用公共普通配置状态与 Map 文件/ROI/source controls | `tests/MapCalibration.ConfigurationChecks` 对原parser default/file/image/teach显隐逐控件比较；源、目标各自ROI输入映射检查通过。dynamic image-source候选弹层尚无原厂桌面差分 |
| `ImageSource_0/1` | `EnumerationG` 对前级 `IMAGE` 项执行 `GetFrontParamItemsOperation`、`GetSelectedCombinationOperation`；选择变更触发 `SetCombinationSourceOperation`，分别面向 `InputImage1/2` | 两个原始声明由 ConfigurationDefinition 解析；图像执行端口分别为 `InputImage1/2` | 原XML与资源身份同上；现有窗口检查未逐项对照原厂动态候选/展开弹层，需桌面证据 |
| `RoiType_0/1` | 独立 `ROISelecter`，ROI 类型 Box，分别关联图像1/2；`MapCalibParam` 构造器 InputROI1/2 是原生 ROIBOX 输入 | 公共 `RoiGroups` 会话分别映射 `RoiType_0→InputImage1`、`RoiType_1→InputImage2`；执行接口传两组ROI | 配置检查覆盖两来源；双ROI真实SDK mask/插件运行证据见 `MAP-CALIBRATION.md` ROI 段。本矩阵不重做数值回归 |
| 示教 / 物理参数 | `CalibPointInput` 的点/坐标选择；`PhysicalPoint`、X/Y 与 `WorldRotateAngle` 为输入选择器；`TeachEnable` 控制 Trigger/TeachFlagInput 可见 | Descriptor 暴露 `Physical Point`、X/Y、angle、Trigger、TeachFlagInput；`PointInputAliases` 将物理点分量合并到共享点入口；窗口使用公共订阅控件 | source class 与 AlgorithmTab 字段相符；配置 parser 可见性已比较，真实 trigger、缓存结果和完整写文件由 PluginChecks/SessionChecks 测试。弹层像素尚无原厂桌面证据 |
| Align File | `SaveCalibPath` 命令执行 `SaveCalibPath`；`CalibPathName` 是校准文件对话框；`RefreshFileEnable` 控制刷新 | `ToolConfigurationCalibrationPathControls`、公共 `SaveFileDialog→Command`、单选文件控件与 `MapCalibrationImageFiles`/`MapCalibrationFiles` | Map ConfigurationChecks 检查 Command/路径叶尺寸、Filter、单选、取消和成功历史；原模块 writer 与完整XML对照详见 `MAP-CALIBRATION.md` |
| Run Params | `CalibBoardType` 允许7/6/9/8；`HomoFreedom` 为1 Perspective/2 Affine/3 Similarity；`WeightFun` 为1/2/3；`WeightFactor` 值1..200。配置组 `Input Source 1/2` 暴露 FilterStatus；contrast及subpixel参数标记弃用/隐藏 | Descriptor 参数与枚举范围相符；普通窗口解析相同算法Tab及配置组 | 字段定义映射直接检查；parser合成运行只覆盖常见显隐相位，不含每个 weight、Filter 和重置场景的原厂桌面截图 |
| ResultShow | `CurCornersPoint/ObjCornersPoint/ProCornersPoint` 分别以 `BridgeObj.GetDisplayObjectOperation` 绑定三个点显示对象；`TextDisplayGroup` 链接文本设置 | `MapCalibrationDisplay.xml` 原样消费三组XY/count；`DisplayDefinition` 与 `ToolDisplayObjectStore` 绑定 InputImage2、文字和对象设置 | 显示XML字节/hash与原厂相同；真实运行输出点数组、计数、比例、误差有既有数值回放。原厂桌面中的三种图元开关/工具提示没有逐像素对照 |
| Display 的 `Coordinate System` | 原厂显示资源引用 `CalibOriPointX/Y` 与两向量分量 | 同一 XML 原样保留该对象，但 `IMVSMapCalibModu.xml` 的输出定义与公开 `MapCalibResult` 均没有这些名字 | 源资源存在引用，公开输出源不存在。不能从资源本身推断坐标图元可见、有效或该对象是遗留项；需要原厂结果显示窗口/运行值确认 |
| ModuRunTime 输出 | 模块 `IMVSMapCalibModu.xml` 将 `ModuRunTime` 声明为 `float`, `IsForce=true`, `IsArray=false`；SDK `CModuleResultBase.ModuRunTime` 注释为 “Module Run Time”。`ModuleResultManager.SetModuleResult` 无条件把 `stModuleResult.fModuleTime` 复制到该基类属性；通用 `ProcedureResult.ModuRunTime` 查询结果存在时也直接返回 `fModuleTime`，查询无结果则返回 `0`。UI 资源将 `ModuRunTime` 显示为“耗时（ms）”/“Time（ms）”，因此单位有直接 UI 证据 | 先前 Map Descriptor 把端口标为 optional，文件与图像 `ModuleResult.Outputs` 均未给值。现改为 required，并在 `Runtime.ExecuteAsync` 统一加入单次执行毫秒值 | SDK/壳层证实该值来自原生结果结构 `fModuleTime`，并以毫秒呈现；尚未恢复产生 `fModuleTime` 的原生计时起止函数。SDK `VM_M_SetFloat` 只是通用 float 输出接口，头文件未给出此专用输出的写入者或计时来源。`SetModuleResult` 对带错误码的结果仍会复制 `fModuleTime`，但没有 Map 的失败/未运行逐态 trace。当前 Stopwatch 是独立实现，无法声称区间相同 |

## 本轮修正与验证

原厂 module XML 已强制 `ModuRunTime` 输出，但 Map 插件先前成功的文件模式、单图等待、双图结果与Teach缓存结果都没有该输出，GraphExecutor也不会把 `NodeRunRecord.ElapsedMilliseconds` 注入插件端口。现在 Map descriptor 将其标为必需，所有正常返回（成功或失败）由 `Runtime.ExecuteAsync` 外围统一附加执行耗时，确保各种提前返回都不会漏掉。SDK `ModuleResultManager.SetModuleResult` 直接复制原生结果结构 `fModuleTime`，壳层资源确认显示单位为 ms；但 `VM_M_SetFloat` 文档只描述一般 float 输出，不能据此定位 `fModuleTime` 的写入者。当前实现取值采用毫秒 `double`，区间为插件入口到模式处理完成；原厂区间和未执行语义尚无直接证据，Stopwatch 仅是独立替代实现。

本轮追查源文件（哈希均为 SHA256）：`VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleResultManager.cs` `FD3960EE308FB739729AF43589035D628544BB5B26941BE5EB6F3250A41739E7`（第 15–18 行设置错误码、执行次数及 `fModuleTime`；后续按结果属性名反射映射输出）；`CModuleResultBase.cs` `3295ED4E4CC9DB4D7EDA35281C9195E1E1E842CCAEFA2BF7186A1B5D15469092`（第 14–27 行属性注释、默认 0）；`ProcedureResult.cs` `DCBCFC9E2BB9D1110C35D8527B1C6CE620AAE4A979624F857FC5A4AC600866AA`（第 37–49 行查询 `fModuleTime`，无结果返回 0）；SDK `VmModuleBase.h` `4AC3FC2368E710DC9040E46F5B308A1DBADACAE1AC67745F26153AE25145C9E7`（第 342–351 行 `VM_M_SetFloat` 是具名 float 结果写接口，没有描述计时）；`zh-cn.xaml` `C3AD9209618D7B36FA3F59C6AF2103FB2DEFB7066A03804E8DE80E992471610E`（第 1737 行将该结果资源化为“耗时（ms）”）。Map XML SHA256 `228BF45A24691C0D13C2201E516941F20E6B1E5352478CCB60ED01C774B4F5F`，第 87 行为必需 float 声明。

定向验证：

- `dotnet run --project tests/MapCalibration.PluginChecks/MapCalibration.PluginChecks.csproj -c Release`：PASS 54。保留原厂运行步骤错误码与原始输出断言，另检查 descriptor 要求、8个文件运行阶段均含有限非负运行值、单图等待和完整双图求解也含运行值。
- `dotnet run --project tests/MapCalibration.ConfigurationChecks/MapCalibration.ConfigurationChecks.csproj -c Release`：PASS 184。保留原parser显隐比较、两组ROI来源及文件叶检查；额外覆盖 WeightFun 三值下当前普通配置叶状态。原parser已采集相位仅含默认 WeightFun=3，不把新加的三值检查误作三份原厂runtime结果。
- `dotnet build src/VisionPlugins.MapCalibration/VisionPlugins.MapCalibration.csproj -c Release --no-restore`：成功，0 warning / 0 error。

## 仍需的最小原厂证据

1. 原厂普通配置窗口分别打开 Image、File、Teach enabled 状态；一组包含两路 ImageSource 候选/当前绑定和两个 ROI 面板，另一组展示 WeightFun 与 WeightFactor 实际显隐、Reset 后状态。这样区分 XML 原始声明与桌面模板运行结果。
2. 原厂结果窗口在一组成功双图运行后截取文字、三组点显示的开关、ROI/Coordinate System 对象；同时保存输出列表/结果面板中对应字段值。重点查 `CalibOriPoint*` 是否存在动态输出、坐标对象是否实际显示。
3. 要闭合 `ModuRunTime` 数值语义，最小新增原厂 trace 为同一 Map 模块进程的一次成功运行、一次带错误码的已运行结果和一次未运行/无结果查询：记录模块 `Process`/调度入口与返回的单调时钟时间戳、返回码、`IMVS_MODULE_RESULT_INFO_LIST_EX.fModuleTime`、`nErrorCode`，并记录 `QueryModuleOneResult` 是否有值。若要精确定位计时边界，还需原生 `fModuleTime` 生成函数或可将模块处理开始/结束函数与该值相关联的调用栈。现有 `VM_M_SetFloat` 头文件声明和 UI 单位资源不能提供这些时点。原厂 Map trace 当前未记录该字段；本轮未调整 Stopwatch 行为。

这些人工证据只用于消除可见性、绑定候选和显示对象的未知，不替代 XML 控制流证据，也不把等待截图当作行为规范。
