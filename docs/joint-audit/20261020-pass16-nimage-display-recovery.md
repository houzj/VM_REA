# 第十六轮：N 图像标定 J08/J10 显示失败与恢复

本轮只补 N 图像标定同一主窗节点的真实图显示状态，不重复第十一轮批次算法、文件/Group 检验。不改产品、共享文件或 `tool-plugins`。

## 原版源码与资源边

原件 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModuDisplay.xml` SHA-256 `89A9D3665D9A90CFB751008DA7559F1BD179DBA84A15F134D5117795A62014E8`，同当前 `src/VisionPlugins.Calibration/Resources/NImageDisplay.xml` 字节一致。`Display` 中 `InImage` 对象绑定输入图像及宽高/像素格式；`Image Point` 以 `OutImagePointX/Y` 与 `CornerPairsNum` 绘点；Show Text/Data Record/Result List 读 `ModuStatus,CalibStatus,CurNum,TotalNum` 和误差、旋转中心等输出。这是字段/对象**声明**证据，不等于原厂失败后真实画面。

原 `CIMVSNImageCalibModu::Process` RVA `0x20b90`，反编译 `worktrees/tool-plugins/analysis/full-audit/n-image-calibration/native/180020b90.c` SHA-256 `EED1AE6F4FD0B0A6AC39ECA2F0F9EAC5EDE090B3BA234A2E10798EFFFC37F2C1`：新批次达到配置数后清旧帧，逐帧加入内部队列，到总数才 `ProcessAlg`，随后 `ProcessOutput`。原 `ProcessOutput` RVA `0x21850`，`.../180021850.c` SHA-256 `21A81AFD487ED2C915D19F18842747C0A9297896D699767B8B5EBDDA72A0DAE4`：无有效解时仍写当前/总数、状态与占位角点端口；有解时发布误差、中心、点等。原 `RenderControlBase.UpdateVMResultShow` 和 `ModuleDisplayDecorator.GetDisplayList` 的图片选择/绘制调用边见 [第十四轮](20261020-pass14-b-four-display.md)。原机同一输入序列失败/恢复后 `GetDisplayList` 的各对象值和可见状态仍未捕获。

## 当前主框架→插件→主窗

`src/VisionPlugins.Calibration/NImagePlugin.cs` SHA-256 `EAF3DD0D77D244FBAEA86DE086A7DB52A358FA070BC8A5F38B8588A19FDC1033`：`ExecuteAsync` 在每轮保存 `Inputs/Outputs/Diagnostic`；失败清本轮输出再写 `ModuStatus=0` 与计时，而保存本轮输入图；`LastDisplay` 从同一节点状态构建文本/记录/列表、图形及原资源声明的图像层。`src/VisionEditor/ToolDisplays.cs` SHA-256 `F3AE89186549591A8A025FC8D36D66BC2B5EA959EC3ED293D3E1030BCDD0E656`：当前选中节点已有 `NodeRunRecord` 时，即使失败仍拉 `LastDisplay`；选节点前先清三个 presenter，避免上节点内容残留。此主框架路径是独立兼容适配器，不能称原 WPF 主窗实现等价。

## 离屏真实主窗探针

`tests/CalibrationNImageDisplay.Checks/` 使用 `MainWindow`、`ImageSourcePlugin.SubmitImage`、工作流数值订阅 `MovePhysicalPointX/Y/RotateAngle` 和实际 `RunProcedure`。源图 `tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png` SHA-256 `CEA3978AF8112865B13E21D76646887DFDEC8D7F1A078BE4F3CB8A39A541912D`；九帧平移方式沿第十一轮已证夹具。步骤和本轮新增观察：

| 状态 | 当前运行结果 | 当前选中 N 图像时显示 |
|---|---|---|
| 前 8 张有效 | `Success`、`CurNum=8`，待批次完成 | `InImage` 图像层 1、点图形 0、记录 173 字符、列表 8 列。 |
| 第 9 张同尺寸空白图 | `Failed`、`0x10103000` | frame 仍存在，保**本轮空白输入**图层 1、点图形 0、诊断记录 51 字符、列表 8 列；没有把上一轮点/指标当本轮解。 |
| 下一张有效图 | 新批次 `Success`、`CurNum=1` | 重新显示新图像层；它是**下一轮恢复**，并非失败帧自行成功。 |
| 再补 8 张有效图 | `Success`、`CalibStatus=1` | 图像层 1、校准点图形 1、记录 195 字符、列表 8 列。 |
| 选回图像源 | — | N 图像 frame、记录、列表 presenter 全空。 |

10/10 断言通过。复验命令：

`dotnet run --project tests/CalibrationNImageDisplay.Checks/CalibrationNImageDisplay.Checks.csproj -c Release --artifacts-path analysis/joint-audit/pass16-cal-nimage-display/build -- analysis/joint-audit/pass16-cal-nimage-display/run`

完整运行日志 `analysis/joint-audit/pass16-cal-nimage-display/first-run.log` SHA-256 `C357901E0C99FC91AA385AB0FBF6E562D0F13E0FF3D8BD36F46BBE01C229B309`；摘要 `run/summary.json` SHA-256 `09A3B773C13F3D975AEC7605DFA0BBAF785A4AB461C10BDD82CC29C3C01E43C0`。本轮首跑直接通过。测试为离屏主窗，未证明用户交互桌面的可见验收。

## 有界结论

当前这条 J08/J10 状态链已实测：同节点本轮图像、失败诊断、恢复后的点图形及切选清理均能逐次更新。原厂同图失败时 `GetDisplayList` 图层/点/文本对象保留与否、SelectedImage/CheckedImages 选择状态及原厂像素显示未有运行 trace；原 `ProcessOutput` 源码只能确认端口写入，不能直接规定 WPF 画面。其他板型、输入格式、取消、保存重开与用户交互桌面验收不在本轮结论中。
