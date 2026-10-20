# 第十六轮：相机映射文本结果在主窗的成功、失败与恢复

本项只补标定十工具中相机映射 J08 的**当前**实际 WPF 主窗刷新边；原资源/算法入口与当前执行结果分别标记，不把资源声明或离屏主窗当原厂失败画面证据。`tool-plugins` 在制文件只读。

原安装 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu/IMVSCameraMapModuDisplay.xml` SHA-256 `35338987B8ED83B93CEFBDA952C218E19B9CC0497ED920AD9D170F36C11C9E97`：`Show Text/Data Record` 取 `CalibStatus,ScaleX,ScaleY`，`Result List` 九列取状态、比例、平移、旋转、缩放、错切与纵横比，**没有 image/path 对象**。原 native `CIMVSCameraMapModu::Process` RVA `0xb830` 的反编译 `worktrees/tool-plugins/analysis/camera-mapping/ghidra/module/18000b830.c` SHA-256 `405011B35674B5C3E28B254721BE1A36E99D7AFEF137FC67278EB7F051C6E40B`：`GetProcessInput/ProcessAlg` 非零直接返回，不进入 `ProcessOutput`；正常 `ProcessOutput` RVA `0xc450` SHA-256 `01338247C382DD899591B2E5CE4672D65C9B7AFFF7C7F3C313AD29C2F49A8EFE` 写状态/比例/平移等。早退后原版 UI 是否继续显示上次列表值，需要原 `GetDisplayList` 的当次对象状态或隔离原机同条件结果，不能由此推出清空。

当前 `src/VisionPlugins.CameraMapping/CameraMappingPlugin.cs` SHA-256 `1429F7892BB792E38F40294C5BC31CBD93FCBDEAAE64BBB80F3B94E03DA186E2` 的 Runtime 每轮先把 `state.Inputs/Outputs` 置 null，成功后填本轮结果；`LastDisplay` 只有 Outputs 非 null 才构帧。`src/VisionEditor/ToolDisplays.cs` SHA-256 `F3AE89186549591A8A025FC8D36D66BC2B5EA959EC3ED293D3E1030BCDD0E656` 的 `RefreshToolDisplay` 先清三个 presenter，再根据选中节点和本轮 `results` 取插件快照。

新增 `tests/CalibrationCameraMappingDisplay.Checks/` 离屏 `MainWindow` 探针在真实 `GraphEditorDocument` 给两组四对数值数组订阅，无图像源或伪造图层。正常求解后主窗记录含 `CalibStatus`、Result List 九列且帧无图/图形；把 `TargetPointY1` 长度改成 1 导致当前输入失败，本轮三 presenter 为空；修复长度后同节点恢复成功；切到另一图像源节点仍无旧相机映射记录。命令：`dotnet run --project tests/CalibrationCameraMappingDisplay.Checks/CalibrationCameraMappingDisplay.Checks.csproj -c Release --artifacts-path analysis/joint-audit/pass16-cal-camera-display/build -- analysis/joint-audit/pass16-cal-camera-display/run`，exit 0、**7/7 PASS**；`run/summary.json` SHA-256 `B2053E98F8D1863681FBC0A4D8E92BE56D2FE80D3BBD0A92F7E2CB7BE94AD1EA`。构建有既有 `MainWindow.xaml.cs` CS8603 警告，无错误。测试源码 SHA-256 `A5F7E1BB29B92402EAF075F044348DE23333C327828EBF0BCF5A6E44D268BD86`。

这关闭当前相机映射的具体文本结果→输入数组失配→修正→切选交接，不关闭原版失败后可见记录、其他 16 点/索引/算法数值或人工桌面验收。当前独立兼容行为与原生源码声明分列；本项无产品改动，主框架实施仍 **41/96 = 42.71%**。
