# 第十四轮：旋转标定图像与结果显示的失败刷新

范围限标定十工具中 `RotateCalibModu` 的输入图像、结果显示和采样失败后的旧结果释放。此前已证的相机标定文件→Rotate 算法链不重跑；本轮用受控 36 字节矩阵文件加真实 ImageSource 图验证显示交接。`Translation`、`ReadCalibFile` 和真实 MainWindow 的原机画面差分均不在此轮实测结论内。

## 原件成员和数据流

安装原件 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/RotateCalibModu/RotateCalibModu.dll` SHA-256 `B08486B8481ED6D7774634B30B5CAE167F85B911EBF394122663BAB400A9BB41`。原 `RotateCalibModu.xml` SHA-256 `EBFEE94C61EF36EAF7CCACBE75C2EF4AD14C9F5FEF8CC50D1B00388BF8D4909D` 声明 `InputImage` 是 IMAGE 组合，`InImage/InImageWidth/InImageHeight/InImagePixelFormat` 四叶均 `IsForce=true`；旋转输出含 `ModuStatus`、`remainCount`、两组中心叶和两个误差叶。原 `RotateCalibModuDisplay.xml` SHA-256 `0D197A40F4F998DCA798F987FA1E09BBFC4B296EC83334EA19B7A03BA7E1A652` 的 `InputImage` 显示对象从输入叶 `InImage` 取图，Show Text 与 Data Record 映射八个状态/结果值，Result List 声明七列且不列 `remainCount`。

原 `CAlgorithmModule::Process` RVA `0x376b0` 源体 `worktrees/tool-plugins/analysis/full-audit/rotate-calibration/native/1800376b0.c` SHA-256 `4B92DBF021269B9C1942B419DF5260CE0757DA3F059C9FE33D976884C13ACE01`：先读 `RefreshSignal`、检查缓存有效位，再读 `RotNum`，仅 3..16 进入采样；无效值调用 `DeInitVecData` 并返回 `E0000001`，未走正常结果叶写入段。合法采样时先发布 `ModuStatus=1` 与 `remainCount`，未满额的中心/误差叶写零，满额写求解值。反编译器对 `GetFloat` 判定曾显示伪常量，相关门以既有原汇编核验 `docs/plugins/ROTATE-CALIBRATION-TYPED-INPUT-SOURCE.md` 为准；本轮不据伪常量推出新的成功/失败条件。

## 当前主框架与插件交接

`src/VisionPlugins.Calibration/RotateCalibrationPlugin.cs` 的 `Runtime` 把本轮结果与输入复制到 scope 内专属 State；`LastDisplay` 再从该 State 构造图像、文本、记录、七列结果。当前 `Descriptor` 用聚合 `InputImage` 的可选端口，`DisplayDefinition.ImageInput` 也取它；它对应宿主适配，并非原 XML 四叶强制输入的逐口实现。原 native `Process 0x376b0` 本身没有据此证明无图时算法一定拒绝，因此精确差异是**声明/连线与显示来源合同**，不能外推算法结果。当前 `ToolDisplayService` 把已核原 Show Text/Data Record 映射作中文本地化文本；文字语言不声称原 XML 逐字一致。

主框架 `src/VisionEditor/WorkflowExecution.cs` 的 `RestoreWorkflowResults` 在每轮清 `results` 后装入该轮节点记录；`src/VisionEditor/ToolDisplays.cs` 的 `RefreshToolDisplay` 先对图、Record、List 都 `Show(null)`，再以选中节点 ID 对应本轮记录拉 `LastDisplay`，请求代号与 execution owner/procedure 检查防过期异步结果回灌。`src/VisionEditor/WorkflowGestures.cs` 节点切选会调用 `RefreshRight` 继而走该刷新方法。这是当前主窗源码链；本轮未对用户可见 MainWindow 执行切节点验收。

## 实际图探针

`tests/CalibrationRotateDisplayChain.Checks/` 建立 ImageSource→Rotate 的实际 GraphEditorDocument，向 ImageSource 注入 80×60 灰图，Rotate 使用自有 `.iwcal` 矩阵文件与 workflow 常量采样。`dotnet run --project tests/CalibrationRotateDisplayChain.Checks/CalibrationRotateDisplayChain.Checks.csproj -c Release` 首跑 exit 0、9 项 PASS，日志 `analysis/joint-audit/pass14-cal-c-display/first-run.log` SHA-256 `CBFB3A9A381B65F7D054384C29841D78CB99FF2B8086A9BBFFC678846D9AF4BE`。

首次合法 `RotNum=3`，真实图源与 Rotate 都成功；`remainCount=2`、显示帧背景宽高 80×60、绿色状态、七列结果与本次 Record 对应。相同 scope 将 `RotNum=2` 后，Rotate 返回 `E0000001`，本轮节点无上次 `remainCount/RotCenterImageX`，显示 Record 转为错误码与“参数错误”，七列值清空并转红色，图像仍来自本次输入。恢复 `RotNum=3`，同 scope 获得新采样 `remainCount=2` 与正常 Record。另一个未执行的 Rotate 节点 ID 的 `LastDisplay` 为 null，不能以选择它取得前节点快照。这证实当前插件按节点/轮刷新，不代替主窗实际切选测试。

探针源码 `tests/CalibrationRotateDisplayChain.Checks/Program.cs` SHA-256 `3271D2E69A754609387A7490A4DC75C302DEF1B1790EB04B5D71CD7A257A1938`；项目文件 SHA-256 `847319680488AEC3D3EE07AA0045D93B2F598C1F180CC3EC9BBE986B3EC7F175`；受控文件 `analysis/joint-audit/pass14-cal-c-display/rotate-matrix.iwcal` SHA-256 `3A77059085107DB51B92A054A8F50347DE7F02562CB97956438B8FBABDE57411`。原件与 tool-plugins 在制文件只读，本轮未改产品/shared、未提交。

## 结论与明确缺口

可关闭 Rotate 的**当前真实图输入→结果显示、无效采样失败后不展示旧数值→同 scope 恢复**代码级交接；实际主窗清空/切选是源码确认，尚未用户可见桌面验收。原 XML 四叶强制 IMAGE 与当前聚合可选端口存在声明差异，应交插件所有者按原配置绑定/连线框架补证或修正；本轮没有在主项目补第二套插件框架。原厂实际窗口显示时序、原状态错误 DTO 文本、原无图运行的主框架判定，及 Translation/ReadCalib 的各自显示失败路径仍需分别核证，不以本轮成功替代。
