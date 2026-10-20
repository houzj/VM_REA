# 第十二轮 J08：标定板与畸变标定的结果显示/失败切换

仅当前“标定”分类两工具及其必要主框架显示桥。原安装版与反编译体只读；当前用离屏真实 `MainWindow` 执行成功→失败→切换节点。**当前结果显示状态是实测；原厂同输入桌面显示没有实跑，不据此判视觉/清理等价。** 不改产品、`tool-plugins` 或共享文件。

## 原版资源和托管入口

| 原件/成员 | SHA256 | 源码支持的结论与限度 |
| --- | --- | --- |
| `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu/IMVSCalibBoardCalibModuDisplay.xml` | `01B6AA734E09BB8284F09CD13CDCABD02115E26B01C0BA218A01DBC960F041D6` | `InImage` 图、`Show Text`、标定点/原点/坐标、检测 ROI、两个同名 `Data Record` 和 `Result List` 各自按 Mapping 取值；同名记录不可当唯一键覆盖。原板 `ProcessOutput @0x180020630` 的状态先行、成功后点/指标见第八轮报告。 |
| `.../IMVSImageCalibModu/IMVSImageCalibModuDisplay.xml` | `B63F5BD17DAEBFE62D004BC963D58FB74CC86CC8CF113644271F86F27CDFF7E7` | `InImage` 默认显示，`Display Post-Correction Image` 映射 `PostCorrectionImageData` 且默认 `IsDisplay=false`；标定点、误差文字、ROI、记录/结果列表有独立特征映射。`PostCorrectionImageData` 是 `PostCorrectionImage` IMAGE 组合成员，不是另一个独立输出端口。原 `ProcessOutput @0x180020b70` 的非原子字段发布见第九轮报告。 |
| `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser/ParamRoot.cs`，`InitNode/InitVmIOs/GetDisplayList` | `41D349894D2CFCB017F2840EBEB79D922A71B09A3F77F905F68DFD619435A24A` | 用模块目录+`ModuleTypeName` 载入 `*Display.xml`，初始化显示对象 IO；显示来源是每模块的资源和 IO，而不是纯绘图常量。 |
| `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/ModuleDisplayDecorator.cs`，`ModuleParams`、`RefreshImageList`、`RefreshShapeList`、`Dispose` | `CEDA24B7FB93BCD9F02B77B23D42A839B1F737BF70CF3ABC6B7786D11765740E` | 从 `ModuleParams.GetDisplayList()` 建图像/图形列表，维护记录计时和弱事件订阅，Dispose 脱钩。该托管入口只证明原资源如何进入显示模型；本轮没有原厂成功→失败→切节点的运行值/可见性 trace。 |

安装资源与当前派生资源字节 SHA 相同：`src/VisionPlugins.CalibrationBoard/Resources/Display.xml` 同 `01B6...041D6`；`src/VisionPlugins.DistortionCalibration/Resources/DistortionDisplay.xml` 同 `B63F...7E7`。这证明资源保真，**不证明执行结果状态相同**。

## 当前源码链与离屏主窗实测

当前 `CalibrationBoardPlugin.LastDisplay` 从同节点/同 scope `Snapshot.Inputs/Outputs` 构造 frame，并按声明顺序分别形成两个 `Data Record`，图形经 `ToolDisplayGraphics.Build`。当前 `DistortionCalibrationPlugin.LastDisplay` 将公开 `PostCorrectionImage` 映射为资源名 `PostCorrectionImageData`，`ToolDisplayImageLayers.Build` 生成原图与修正图层；`Fail` 只覆盖 `ModuStatus/ModuRunTime`，保留旧 `State.Outputs` 其他字段。Graph 层失败轮 `NodeRunRecord.Outputs` 可为空，但宿主 `RefreshToolDisplay` 在本节点 `results.ContainsKey` 时从插件 owned display state 拉取 frame；切换节点时它先执行 `Show(null)` 清旧图、记录和列表。`ToolDisplayImagePresenter.Show` 选择一个 base 图和可选 overlay，图像对象的资源 `IsDisplay` 与插件 layer 选择状态仍须区分。

当前源码身份：`src/VisionPlugins.CalibrationBoard/CalibrationBoardDisplay.cs` SHA `51055B1F5FF390805A342479C905D574C86B2817254FCCF18A11879C4C7ACDB6`；`src/VisionPlugins.DistortionCalibration/DistortionCalibrationPlugin.cs` SHA `494A29CC3143890EF12E2570905BF13E90CDC3C28F2A7EB0BE393D6FDE7DA51E`；`src/VisionEditor/ToolDisplays.cs` SHA `F3AE89186549591A8A025FC8D36D66BC2B5EA959EC3ED293D3E1030BCDD0E656`；`src/VisionPluginContracts/ToolDisplayPresenter.cs` SHA `0148527F597E5DB2239B4EFAB3987CA992B2216317CD166A95F9F51F7238C6E2`。

探针源 `tests/CalibrationBoardDistortionDisplay.Checks/Program.cs` SHA `010AFBE85D9192A7D812F6019DDA14CEAA2C080F1F6BEDAAF3C5BA85DE07114A`，工程 SHA `82B59BF4717EB98B8B6282294DE8AD81EE35A30F9FBDB02138F49E66BF8542ED`。调用真实 `MainWindow.RunProcedure`、实际 ImageSource→工具图和主窗 `RefreshRight/RefreshToolDisplay`；不直接伪造 display frame 或注入 results。运行：

```powershell
dotnet run --project tests/CalibrationBoardDistortionDisplay.Checks/CalibrationBoardDistortionDisplay.Checks.csproj -c Release -- analysis/joint-audit/pass12-cal-b-display/probe
```

最终退出码 0，**11/11**，`analysis/joint-audit/pass12-cal-b-display/fifth-run.log` SHA `A0D4F6F06BEF739231D60526FA97677966CED2A7DD0DB70B5585F53BB0C60057`，结构化输出 `probe/summary.json` SHA `A2C356A7D7C7A5E03523AFF579DA38C8EBEB4E25C54D84402C0F1CA2C2E94386`。

| 转换 | 当前 `NodeRunRecord` | 真实主窗所选 `ToolDisplayImagePresenter.Frame`、记录/列表 |
| --- | --- | --- |
| 标定板 480×400 Gray8 棋盘 → | Success | 有图、4 个 graphic、记录 208 字符、列表 10 列。棋盘为测试自造 `probe/board-valid.bmp` SHA `2D3260C121DC46450B77456A81618AA3D8F297F5BCE70398FDACBA998186CFD3`。 |
| 同节点空白图 → | NoMatch | 仍显示**本轮输入图**、0 个 graphic、失败记录 79 字符、列表列定义仍 10；没有旧标定点 graphic。空白夹具 SHA `EED3E49469D3E90FA5A2962614FDF8531D5CE747623AD401FBC6519AF44A6621`。 |
| 切选图像源 | 图层不重跑 | 板工具 frame=null、记录空、列表 0 列。 |
| 畸变 1000×1000 Mono8 SDK 板图 → | Success | `InImage` 与 `Display Post-Correction Image` 两层均有 image，170 graphics、记录 40 字、列表 1 列。只读 fixture `worktrees/tool-plugins/analysis/distortion-calibration/trace-coded-boards/fixture.bmp` SHA `86E175CD993F9737380690A60F7BADA49935196691CA4509C93FF0066FADCC40`。两层**可用于选择**，未证明默认同时可见；原修正层 `IsDisplay=false`。 |
| 同图但自动导出路径改 `.bad` → | Failed | owned LastDisplay 仍有前轮原图/修正图两层及 170 graphics；记录改为 60 字失败状态。该保留来自 `Fail` 仅覆盖状态/时间、宿主按节点存在拉取 owned frame，不能把图层失败 `NodeRunRecord.Outputs` 误称保留。 |
| 切选图像源 | 图层不重跑 | 畸变工具 frame=null、记录空、列表 0 列；未把旧工具结果泄漏到所选源节点。 |

失败审计未抹去：`first-run.log` 缺测试工程 `System.IO`，`second-run.log`/`third-run.log` 在测试未调用 ImageSource `SubmitImage` 时源节点 `0xe000000f`、板跳过，`fourth-run.log` 把本地插件错写为静态 `ImageSourcePlugin.Id`；均为探针自身错误。第五次使用与第十轮板链同一棋盘构造、真实 `SubmitImage`，才计算 11 项通过。原始日志均在本轮忽略目录；没有干预其它任务进程。

## J08 裁决及下一有限证据

当前构建的上述两条具体显示/失败/切换边可关闭。畸变失败时旧图与点 graphic 保留是**观察到的当前行为**；原 `ProcessOutput` 非原子发布路径说明不能假定失败必清空，但未直接证明原版在同一文件扩展名错误后仍显示这些旧图或点。若要判断偏差，下一步只需对原厂受控实例的同一“先成功→自动导出失败→切节点”序列记录 `GetDisplayList` 图层对象值/可见状态与结果回调，或通过原托管 `ModuleDisplayDecorator` 结果刷新调用链加受控原模块输出复验；缺这一步不得自动清旧图。标定板也需原同图 NoMatch 的记录/图层差分，以及两项当前 UI 的默认选层/人工可见呈现。原配置窗口、全部图层勾选与缩放/像素级外观不由 11 项断言覆盖。J08 两工具**整门仍开放**，不重复正常夹具而扩称完整审核。
