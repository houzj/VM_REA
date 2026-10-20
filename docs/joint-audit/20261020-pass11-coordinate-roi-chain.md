# 第十一轮：坐标系 Draw/ROI 与图像、显示交接

范围仅为标定十工具中的 `CoordinateModule` Draw 模式。此轮核 `InputImage`、`RoiType`、参数、坐标系结果与显示的端到端链；既有 MATRIX/POINT 成功链和其他框架功能不重复计数。

## 原件依据

- 原安装件 `Applications/Module(sp)/x64/Calibration/CoordinateModule/CoordinateModule.dll`，SHA-256 `8547DF5CEF905FB824A068359DD49B7B5FEFA5100A061D6555A993D882666AF6`。原 `CoordinateModuleAlgorithmTab.xml` 中 `CordinateCreate` 的触发器控制 `RoiType` 与 `InputWay` 可见性；`ROISelecter Name="RoiType"` 的选择类为 `Coordinate`。这是原 UI 资源事实，不表示原生 ROI 使用了当前 `Normalized` 存储格式。
- 原 `CAlgorithmModule::Process` `0x18003f4a0`：每次将 `this+0x15e10` 状态置零；`CordinateCreate==0` 且 `this+0x38==12` 时复制 ROI 的 `this+0xec/f0/f4` 到 pose `this+0x15e48/4c/50`，再调 `CorToCalibmatrix` `0x180040320`。无 Coordinate ROI 时走诊断尾；尾部 runtime-info 成功覆盖内层诊断，暖状态可保留旧 pose/矩阵。
- `0x180040320` 把 Draw 角度按度转换并写 `[cos,sin,-cos*x-sin*y; -sin,cos,sin*x-cos*y;0,0,1]`。`0x180040550` 继续发布状态、矩阵、原点和四个轴值，`AxisYY=50*m00`。外层 `0x18003f280` 先做图像前置，内层返回不阻断 `ProcessOutput`；图像前置拒绝时不会调用内层/输出。这些结论依赖原函数取体，见 `analysis/coordinate-module/native-source/output/18003f4a0.c`、`180040320.c`、`180040550.c`、`18003f280.c` 和 `180017a40.c`。
- 原 `CoordinateModuleDisplay.xml` SHA-256 `DB489D4738F31A133E8C198EA9B6C7756DE5F68C47DE1C04B68CB2F39C860B83`：`Coordinate System` 图形的 CenterX/Y 和两个轴向量分别取六个结果叶；`Data Record` 取 `ModuStatus`。没有由该资源定义的任意状态文字叠加。

审计取体 SHA-256：`CoordinateModuleAlgorithmTab.xml` 为 `C1760FFEAF852E4B663F93965FA1A167B75F134D5A1DFEE960092B98A23E1F5C`；`18003f4a0.c` 为 `3420E2291FD782FFDAF6BA801F834BE8385239BEDCFF36C4EA4CC1A16C850A68`，相应 asm 为 `9C19934A1DBBED8274AF671B4DBD11E7BCF339617086256A31B7E9EA8D392332`；`180040320.c` 为 `D9083CC6574822FA7DFD329DA612DA85CCDDF8F785E3E5CD408346D8551E229C`，相应 asm 为 `0A3F1987275EC200AF8973DA63638E0CD072AA97D53E2AE91C49D4D0E81E5BE3`；`180040550.c` 为 `7B732F0AA0A6E46581C0A75B8B3E53F4E81963AE5DD862E87CE03EF93FA1E16B`，相应 asm 为 `7AE961AE4DA5F4540793207E529451C8094283B49132348FE223B559BD3ED650`。这些原件只读，未复制/修改工具线在制文件。

## 当前代码链与边界

`GraphEditorDocument.SetProcedureToolConfigurationRoiGroup` 将 `RoiType` 的 Coordinate ROI 与参数写入当前节点、Undo 历史和方案；`GraphExecutor` 快照保留 ROI groups，`CoordinateModulePlugin.Runtime` 从真实上游 `InputImage` 取图、检查尺寸/格式/载荷，再选 `RoiType`。当前 `RoiGeometry.ToPixels` 把 `Normalized` 的中心乘以本轮图像宽高，然后将像素 pose 交给 `CoordinateSession.Process`；输出保存于同一执行 scope，`LastDisplay` 使用该轮输入图和六叶构造坐标图形。实现路径见 `src/VisionEditor.Core/GraphEditorDocument.RoiGroups.cs`、`src/VisionFramework/GraphExecutor.cs`、`src/VisionFramework/RoiGeometry.cs`、`src/VisionPlugins.CoordinateModule/CoordinateModulePlugin.cs` 与 `CoordinateSession.cs`。

`Normalized`→像素及随图像尺寸重新缩放是当前宿主的明确适配，并非原 DLL 的 ROI 持久化合同。原 Draw 模式缺 ROI 后可留暖状态是有原函数依据的；首次缺 ROI 时原 ctor 未初始化矩阵，不能用托管零矩阵声称原值。当前托管对首次未写全矩阵发 `NativeUninitializedMatrix`，这是安全边界。

## 受控真实图复验

探针 `tests/CalibrationCoordinateRoiChain.Checks/` 使用实际 `GraphEditorDocument`、`ImageSourcePlugin`、`CoordinateModulePlugin` 和 `SolutionExecutor`，构造 ImageSource→Coordinate `InputImage` 连接与 `RoiType` 的归一化 Coordinate ROI。独占构建槽 `dotnet run -c Release` 首轮即 exit 0，日志 `analysis/joint-audit/pass11-cal-coordinate-roi/checks-first.log`：100×80 图给原点 `(25,40)`，90° 矩阵 `[0,1,-40;-1,0,25;0,0,1]`，显示帧宽 100 且有 `Coordinate System`；上游图换为 200×80 后原点 `(50,40)`、显示帧宽 200 且图形中心 X=50。移除 `RoiType` ROI 后本轮仍 `Success`、`ModuStatus=1` 且原点/矩阵保留；Undo 恢复归一化 ROI 后用 100×80 图再次得到 X=25；保存并重开方案，在新执行 scope 中用 100×80 图仍得 X=25。产物 `analysis/joint-audit/pass11-cal-coordinate-roi/coordinate-normalized-roi.vmx.json` 可供复验。这里的 `Success` 是原内层诊断被计时尾覆盖对应的托管状态，不意味着缺 ROI 获得了新坐标。

现有 `tests/CoordinateModule.HostChecks` 已覆盖可视化 ROI 像素拖动、显示样式持久化和窗口 lease 释放；这些是已有验证，不计作此轮新增覆盖。此轮也未做原机窗口逐像素对照或真实相机 SDK 容错差分。

## 待核边界

- 原厂 `RoiType` 从原 UI 编辑器到模块 `this+0xec/f0/f4` 的精确序列化/坐标空间仍未由本轮源码链恢复；不得把当前 `Normalized` 适配称作原厂。
- 原图像前置的非安全/越界载荷行为和原模块第一次缺 ROI 的未初始化堆内容不进行等价模拟。
- 原逐 API `ProcessOutput` 的部分发布失败与当前托管字典原子结果不是同一个错误粒度；本轮仅覆盖暖状态正常发布。
