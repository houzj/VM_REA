# 第十五轮：N 点标定 J08 结果显示成功→失败→切选

仅核 N 点标定的当前主窗显示转换。第十四轮已对 N 点及相机映射四项静态源做过矩阵，本轮新增实际成功→退化点失败→切节点，不重复映射标定测试。不改产品、共享文件或 `tool-plugins`；原版同条件失败显示仍未知。

## 原版入口与资源

- 原件 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSNPointCalibModu/IMVSNPointCalibModuDisplay.xml` SHA-256 `88DE08E7D5881777E9C2096B7BA9D7DAB826F62F547DA129352991866F2CB0D9`。`Show Text`/`Data Record` 映射 `ModuStatus,CalibStatus,CalibErrStatus,CalibCurIndex,CalibTotalNum`；`Image Point` 映射 `OutImagePointX/Y` 与物理点提示；`RotAxisPoint` 映射旋转中心；Result List 声明三列。
- 原 `CIMVSNPointCalibModu::Process` RVA `0x3a8c0`，反编译 `worktrees/tool-plugins/analysis/full-audit/translation-calibration/native/18003a8c0.c` SHA-256 `BD39FDC8FDAA99709F88FF7084E2E3DDF6190F288090CC2B1E14F6832B14F9F3`。普通路径先 `GetProcessInput`，非零时绕过后续；`ProcessAlg` 非零直接返回而不进 `ProcessOutput`。未满点数的特定分支则直接进 `ProcessOutput`，不能概括为“失败均无显示输出”。
- 原 `ProcessOutput` RVA `0x3c3f0`，`worktrees/tool-plugins/analysis/full-audit/translation-calibration/native/18003c3f0.c` SHA-256 `FCE52A352B3FE6013C736157208D175234483C790C69D807FCC742713BB20D47`：进入该路径时向 ModuleFrame 写 `ModuStatus`、`CalibStatus`、当前点前缀和进度等，成功态另写解算结果。原 `Apps.XmlParser.ParamRoot.GetDisplayList` 与 `VMControls.WPF.DataForUI.ModuleDisplayDecorator` 的原显示框架入口详见 [第十二轮](20261020-pass12-board-distortion-display.md)。这些静态源码**没有**给出退化点 `ProcessAlg` 失败后原 WPF 画面保留/清除的同图运行结果。

## 当前链及实测

- 当前 `src/VisionPlugins.Calibration/Resources/NPointDisplay.xml` 与上述原 XML 字节相同。`src/VisionPlugins.Calibration/NPointPlugin.cs` SHA-256 `DB9482BB034D9F98F436A932C51804843BB20A18A67AA1EEA28D874DBF1574C0`：`Runtime.ExecuteAsync` 每轮结束把本轮 `output`、输入和错误放入 `State.DisplayOutputs/DisplayInputs/DisplayDiagnostic`。当前独立实现即使求解失败也建立空/部分输出快照，不应反推原厂行为。
- `src/VisionPlugins.Calibration/NPointConfiguration.cs` SHA-256 `FEA7744DC344CA708995394AE40C059322B092100D5C672159015FF2C9EF9B55`：`LastDisplay` 由上次运行快照经 `ToolDisplayService.Build` 与 `ToolDisplayGraphics.Build` 形成 frame。`src/VisionPluginContracts/ToolDisplayContracts.cs` SHA-256 `8E09C8B9D5C67A949BFAECA44613D25730295A5DEE4B20604C9DA40C03E661B4`：`Build` 有 diagnostic 时以错误码/信息构成 `RecordText`。`src/VisionEditor/ToolDisplays.cs` SHA-256 `F3AE89186549591A8A025FC8D36D66BC2B5EA959EC3ED293D3E1030BCDD0E656`：`RefreshToolDisplay` 切选前清三 presenter，结果存在时失败也询问 `LastDisplay`。
- `tests/CalibrationNPointDisplay.Checks/` 在离屏 `MainWindow` 创建合法手动 4 点（图像正方形 0/10，物理正方形 0/20；`CalibPointGet=1,CalibPointTotalNum=4,RotPointTotalNum=0`），真实 `RunProcedure` 成功；随后仅把四个图像点改成 `(0,0)`，参数仍合法，解算失败 `0x10103007`；切到另一个图像源节点。**6/6 通过**：成功 frame 2 图形、记录 60 字符、列表 3 列；失败 frame 存在但 0 图形、记录含错误码（25 字符）、列表仍 3 列；切选后三 presenter 空。此时的清图形来自当前本轮空输出，不证明原版做了相同清理。

复验命令：

`dotnet run --project tests/CalibrationNPointDisplay.Checks/CalibrationNPointDisplay.Checks.csproj -c Release --artifacts-path analysis/joint-audit/pass15-cal-npoint-display/build -- analysis/joint-audit/pass15-cal-npoint-display/run`

保留首编译失败（测试误读不存在的 `ToolDisplayFrame.Diagnostic` 属性）：`analysis/joint-audit/pass15-cal-npoint-display/first-run.log` SHA-256 `6C3B5290148B58BCAAA55BECC93F5CFAF550E9E54CEEF1D994559DBEB002EC69`。修正测试后通过日志 `second-run.log` SHA-256 `E56872F40074D734E7C390C9D809DDB742D2C8C8A136A7DCF8B690087327F917`；`run/summary.json` SHA-256 `E525D941FDBA763B54ACEA94669728EBAC8C87AA3BD0C2642A1440970E39B09E`。离屏实例不计用户交互桌面人工验收。

## 边界

本轮闭合了当前 N 点 J08 的这一条具体图：成功→退化点失败→切图像源。原厂相同点表/运行次序下 `ProcessAlg` 错误码、`ModuleDisplayDecorator.GetDisplayList` 值、选中图像与记录窗口的失败后状态无运行快照，不能宣称原厂清空图形或保留记录。其他失败类型（GetProcessInput、导出、未完成点数）、重开与用户桌面可见状态未覆盖。相机映射仍仅有第十四轮静态 J08 证据；没有把本轮 N 点探针算给它。
