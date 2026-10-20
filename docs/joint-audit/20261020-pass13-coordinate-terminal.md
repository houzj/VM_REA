# 第十三轮：坐标系终端结果到 Group 与显示

范围限标定十工具中的 `CoordinateModule` 输出端。此轮复用已证 ImageSource→Draw/ROI，不重复核 MATRIX/POINT 求值或全部 UI；追原声明/发布/显示资源到当前节点结果、拥有 Group 的输出与同次执行显示。没有由这些证据证明的下游插件消费者，不把 Group 出口推作任意工具输入。

## 原件到端口

- 安装原件 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/CoordinateModule/CoordinateModule.xml` SHA-256 `116D4BCB2CDFEF0E2E380C527B6666F1F11C7D1B4F6D5640B1C281E2745E9218` 声明 `CorCalibMatrix` float 数组、`ModuStatus` int、原点组合 `CoordinateOriPointX/Y` 与两个轴组合 `CoordinateAxisXX/XY`、`CoordinateAxisYX/YY` 的四个 float 叶。矩阵、原点、轴是不同结果口。
- 原 DLL SHA-256 `8547DF5CEF905FB824A068359DD49B7B5FEFA5100A061D6555A993D882666AF6`。`CAlgorithmModule::Process` RVA `0x3f280` 在图像前置成功后调用内部 Process，再调用 `ProcessOutput`，并以 `ProcessOutput` 返回码作外层返回。原 `ProcessOutput` RVA `0x40550` 按 `ModuStatus`→9 项 `CorCalibMatrix`→原点 X/Y→轴 XX/XY/YX/YY 顺序逐项调用输出 API；每项失败会中断余项。轴取矩阵值为 `50*m00, 50*m01, 50*m10, 50*m00`，最后 YY 确实再次读 `m00`，不是推断的 `m11`。源体 `worktrees/tool-plugins/analysis/coordinate-module/native-source/output/18003f280.c` SHA-256 `3D1CC8C5A860C185B1FAEBAA9435FF46FA8FE40192F1C4502AF7B60F1B38168B`、`180040550.c` SHA-256 `7B732F0AA0A6E46581C0A75B8B3E53F4E81963AE5DD862E87CE03EF93FA1E16B`；后者汇编 SHA-256 `7AE961AE4DA5F4540793207E529451C8094283B49132348FE223B559BD3ED650`。
- 原 `CoordinateModuleDisplay.xml` SHA-256 `DB489D4738F31A133E8C198EA9B6C7756DE5F68C47DE1C04B68CB2F39C860B83` 将 `Coordinate System` 的 CenterX/Y 与两向量六叶映射到上述输出，Status 映射 `ModuStatus`，`Data Record` 的 Content 映射 `ModuStatus`。这是显示资源映射，不等于原厂主窗像素/时序已验。

## 当前实现与实际流程

`src/VisionPlugins.CoordinateModule/CoordinateModulePlugin.cs` 的 `Runtime.ExecuteAsync` 从 `InputImage` 与 ROI 取本轮数据，`CoordinateSession.Process` 产出矩阵、原点、轴，正常时输出字典包含这些叶；`LastDisplay` 在相同 `ModuleExecutionScope.ForProcedure(body)` 读输入和结果快照，按 Display.xml 六叶构造图形。`src/VisionPluginContracts/ToolDisplayGraphics.cs` 将轴映射为 `XVector/YVector`。`GraphEditorDocument` 的 owned Group 将所选子节点端口导出到主流程，`SolutionExecutor` 负责调用和状态传播。当前宿主 `Normalized`/像素适配、显示帧、Group 类型包装是独立实现边界，不能称原 DLL 对 Group 的原生发布 API。

受控实际图探针 `tests/CalibrationCoordinateTerminal.Checks/` 建立 owned Group 内 ImageSource→Coordinate，Draw ROI 中心 `(20,30)`、角度 90°、100×80 真图，并将矩阵 `NumberArray`、OriginX `Number`、AxisYY `Number` 三口导出。`dotnet run --project tests/CalibrationCoordinateTerminal.Checks/CalibrationCoordinateTerminal.Checks.csproj -c Release` 首轮 exit 0，10 条断言通过。日志 `analysis/joint-audit/pass13-cal-coordinate-terminal/first-run.log` SHA-256 `3CBB70466813941C606689C542DCB3DB94B9748848DF0B04EDA07689DD9441A8`。结果矩阵 `[0,1,-30;-1,0,20;0,0,1]`（零位采用容差），Group 导出 `Matrix/OriginX/AxisYY`；显示帧取同轮宽 100 图、中心 `(20,30)`、向量 Y 分量 `50` 和 X 分量 `-50`。

同 scope 移除 ROI 后，内部计时尾仍 `Success/ModuStatus=1`，旧矩阵继续导出和绘制；这表示原已证的暖状态保留，不代表本轮拿到新 ROI。新冷 scope 首次无 ROI 时，当前适配拒绝未知原生未初始化矩阵，内部 `Failed/NativeUninitializedMatrix`，Group `Failed/SubprogramFailed`、无出口，显示帧无坐标图形。Undo 恢复 ROI 后成功，保存方案再重开并用新 scope 运行仍得到同三出口。方案探针文件 `analysis/joint-audit/pass13-cal-coordinate-terminal/coordinate-terminal.vmx.json` SHA-256 `D38D1EF30CC8DF068476F1E5098F4A3D299DE31C96F7AF177756554E0C134B59`；测试源 SHA-256 `92FDC229033B0B0430B100975C00F285820BA02AC6B2118AE63A775BC942D74A`，项目文件 SHA-256 `41F2C8576DFEB6C5A17FEC60026798E3ADBDE64FE2B3DC7C46BDCEA613D7B5C1`。

## 关闭范围和缺口

本轮可关闭 **当前坐标系正常/暖保留/冷安全拒绝结果→Group 终端导出及同 scope 坐标图形** 的代码级交接。原厂真实 `ProcessOutput` 各 API 单项失败后的部分发布、原生首轮堆矩阵、实际 MainWindow 切节点/切方案后的旧显示清理、原厂与当前主窗像素差分均未做等价声明。`Coordinate` 后再接具体其他插件的消费者链也未由本轮端口证据证明，未来需选定确切消费端口后另核。原件和 tool-plugins 在制文件均只读；无产品/shared 修改、无提交。
