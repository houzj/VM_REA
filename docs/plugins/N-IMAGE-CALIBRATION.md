# N图像标定：并行逆向记录

2026-10-17 独立工具线进展见 [本轮配置与数值接线](N-IMAGE-CONFIGURATION-PROGRESS.md)。新的 `NImageConfigurationProvider` 消费原厂 XML 和公共节点宿主；旧 `NImageWindow` 仅保留兼容回归，不是生产接入建议。生产工厂 `new NImagePlugin()` 已连接真实识别、数值阶段、输出、文件与会话，宿主注册由根线统一完成；桌面验收与已知数值差异仍分别列出。

本工具已具备生产配置模型、独立海康编码板识别、平移/旋转求解、37个原厂输出、矩阵修正及文件导出。首轮研究记录保留下文；最新边界和未验证项以本轮进展文档为准。没有设备测试。

## 证据与复现

原厂目录为 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSNImageCalibModu`，共 12 个文件。原生模块 SHA256 为 `68c04fc9782e873a80776d6ea4bc1e0515b87d54e9d13b41dbd73799747525ec`。声明中输入 11 个底层字段（含图像的 4 个字段），输出 37 个字段；AlgorithmTab 有 38 个具名控件。完整结构与哈希见 [证据索引](n-image-calibration-evidence.json)，供后续 UI 实现使用的派生结构为 `Resources/NImageDefinition.json`。

REA 托管静态证据 ID：`ev_47d8fe724a3d57ef879aa778e0fb1e24e74df258e6f6f9266459af03c8e39ca6`。控件 DLL 是 .NET Framework 4.6.1、AnyCPU，其自定义 `HidePhysicInfoOperation.GetVisibility` 决定自动物理坐标区是否显示。静态分析不代表执行或界面采集。

原生源码通过独立 Ghidra 项目分析，输出位于本工作树 `analysis/full-audit/n-image-calibration/native`。关键 RVA：

| 方法 | RVA | 已确认内容 |
|---|---|---|
| 构造 | 0x1f670 | 9 次平移、3 次旋转；原点 0；步长 1；X 优先；换向次数 3 |
| Init | 0x209b0 | 正常创建 NImageCalibV2 与 MatrixFix 算法对象 |
| Process | 0x20b90 | 图像批次、三项订阅的整体回退、蛇形物理轨迹 |
| ProcessAlg | 0x21450 | 一批图像传入官方算法；可选矩阵修正；可选更新文件 |
| ProcessOutput | 0x21850 | 计数、角点、误差、方向输出；未完成批次的零值输出 |
| SetParam | 0x229d0 | Clear、参数缓存、文件路径、算法参数别名 |
| WriteXmlData | 0x230e0 | 模块自己的 CalibInfo XML 文件格式 |
| 输出辅助 | 0x1f090/0x1f0a0 | 直接转发 BatchSetFloat/Int，第二形参未使用 |
| 帧节点构造 | 0x1f5a0 | 40 字节帧载荷及双向链表布局 |

原厂 SDK 为 `MVDAlgorithmSDK/Runtime/x64/MVDNImageCalibV2Cpp.dll`。测试通过官方 `CreateNImageCalibV2ToolInstance`、`AddCalibImageData`、`Run` 等接口，在自有进程中执行；产品不引用或依赖该 DLL。

已执行两组各 36 次算法调用，另记录每组创建和默认配置：

- 6 个板类型 × 6 个图像数量（0、1、2、3、9、12）的空白图边界。
- 相同组合使用安装帮助中 HIK I 型板的派生图像。来源 `GUID-3D2B2BA5-C470-43B0-8EFD-D2D7D34DB3F0-high.png`，裁剪 `(740,0,294,295)`、最近邻放大 3 倍、置于 1000×1000 白底，再按已知位移生成 9 张图像。它们是帮助图派生的合成测试数据，不能称为相机实拍。

派生图输入板类型 7、9 且 9 张平移图时，实际 Run 返回 0，各产生 168 个跨 9 图像的角点对。`GetStatus` 为 `0x1010304e`（机构最大平移误差率告警），与调用错误必须区分。完整角点坐标、编号、矩阵和误差均已保存；不是仅检查“调用成功”。普通棋盘类型 1、圆板类型 2 返回 `0x10000019`，不能用一般棋盘算法替换本工具的编码板识别。

同一 I 型图像选择普通 I 型（7）和扩展 I 型（9）均能提取角点，但解译出的编号/原点差异明显。普通 I 型矩阵平移约为 `(-4.6832,-5.2160)`，误选扩展 I 型则约为 `(1101.8185,4475.9399)`。因此四种板类型的编码解释必须分别实现和验证。

另执行一次 **synthetic-owned-instance** 原厂模块方法回放：正常构造原厂对象，绑定正常创建的官方算法对象到源码确认的成员，并使用原厂帧节点构造函数装入自有图像，调用原厂 `ProcessAlg` 和 `SetParam(SaveCalibPath)`。前者返回 0、CalibStatus 为 1，后者返回 0并实际生成模块 XML。未调用成功的平台 Init，不代表完整平台调度；没有修改原厂代码、磁盘 DLL、许可检查或其他进程。

模块 XML 根节点为 `CalibInfo`，包含 `CalibInputParam` 和 `CalibOutputParam`，其 `ImagePointLst/WorldPointLst` 的点包含 X/Y/R，矩阵为 9 个 `ParamValue`。它与官方算法 SDK 的配置/导出文件不是同一种结构。曾观察到原厂保存返回 0 但 Unicode 路径没有文件；改用当前工作树 ASCII 别名后才实际生成，测试脚本因此同时检查输出文件存在与哈希。

测试入口均在 `tests/NativeEvidence`：`Export-NImageFixture.ps1`、`Invoke-OriginalNImageAlgorithmProbe.ps1`、`Invoke-OriginalNImageModuleHarness.ps1`。后者在使用私有辅助 RVA 前核对原厂 DLL 精确哈希；所有输出写当前工作树。

## 已实现的独立部分

`NImageSequence` 保存图像副本，按原厂批次累计；已有批次数达到平移+旋转总数后，下一张图像自动开始新批次。Clear 立即清空。

自动轨迹按源码实现，默认前 9 点为：`(-1,-1)、(0,-1)、(1,-1)、(1,0)、(0,0)、(-1,0)、(-1,1)、(0,1)、(1,1)`，再应用原点与各轴步长。Y 优先交换走向。旋转图的物理 X/Y 为 0，角度为基准角加旋转序号×角度步长。只有移动 X、移动 Y、旋转角三个订阅全部存在时才整体使用输入；只绑定其中一部分时全部回退自动计算。

`NImageUiRules` 复现 HidePhysicInfoOperation：旋转角已绑定时隐藏自动坐标区；按点模式再检查点绑定；按坐标模式检查 X/Y 任一是否绑定。原厂 `SetMutualExclusion` 将另一次数最小值设为 1（本次数为 0 时）或 0。旋转次数为 0 隐藏角度字段；矩阵修正的按点/按坐标选择器按 XML 分支显示。

数值守卫中的有限数检查、全零批次拒绝和图像所有权为独立实现的安全边界，不宣称原厂所有非法参数路径相同。

`NImageWindow` 已实现基本参数、运行参数、结果页，以及输入订阅、矩阵修正、板类型、滤波、清图、文件与执行调用边界。5 张独立离屏截图由测试生成。另通过原厂 `AlgorithmTabRoot` 与自定义 `HidePhysicInfoOperation` 取得 14 条真实 parser 记录，覆盖按点/坐标、矩阵修正开关与输入方式、旋转次数、四种板类型。模型与 owner 是显式合成对象，不代表完整平台模块；未尝试把已知失败的共享 BAML 壳当成功 UI。详见 [UI 证据](n-image-calibration-ui-evidence.json)。

`NImageFile` 根据真正原厂模块输出实现 CalibInfo XML 序列化，全部原生元素/属性和数值完成回归，保留 X/Y/R、-999 哨兵与告警码。提供的类型化读取函数用于校验/交换；原厂 N图像模块的文件路径字段不执行加载。

`NImageBoardDetector`、`NImageFiducials`、`NImageBoardRecognition` 已独立串通像素鞍点、局部网格、标签开口、四圆定位、透视采样、RS 码字和世界编号。帮助图派生九帧中，四标签全部自动解码，168×9 个世界编号与原厂逐一对应，最大像素差 0.679788；未将原厂角点坐标写入产品。`NImageCodeDecoder`/`NImageCodeReedSolomon` 的原厂内核对照由主线补充，标签圆心现在由产品算法自动定位，不再由测试提供。

从原厂工具 `DemoGenCalBoard_en.exe` 的 Python 3.6 字节码恢复 `gen_calibration` 12 参数 ABI，正常调用原厂 `DemoMvbCalib.dll` 得到 I/II/扩展 I/扩展 II 的 16×16 图样。原厂 SDK 成功分支分别给出 197/225/197/225 角点；完整编号和九帧像素留在 [生成板证据](n-image-calibration-generated-evidence.json)。II/扩展 II 在 1000×1000 时原厂返回 0x10103000，2000×2000 时成功，不能将独立解码器在较小图上的成功冒称原厂等价。专属测试同时检查四板型标签与编号，四个直角朝向保持编号和坐标；额外角度归一化属于独立实现：7° 合成旋转与原厂 197 点对照最大差 0.086127 像素。新增局部邻接网格生长可处理已测一般透视，合成投影恢复 197 点/四标签，原厂同图也成功；更广透视、裁切和失真条件仍需继续验证。

`NImageBoardIdentity` 已从 80e690/80f9a0/80bcd0/80cfc0 和 813020 恢复普通/扩展板字节格式、原点、网格物理间距和八方向坐标解释，只有通过真正码字解码的字节才可送入。普通/扩展类型不能互换。

专属检查持续覆盖上述序列、规则、真实原厂 parser 显隐、独立编辑器、原厂 XML 同构，以及 168×9 像素近邻与编码坐标打包边界。当前数量与构建日志位于本工作树 `analysis/full-audit/n-image-calibration/checks.log`、`build.log`；通过这些检查仍不构成完整工具验收。

```powershell
dotnet run --project tests/NImageCalibration.PluginChecks --artifacts-path analysis/build/n-image-calibration
```

## 仍需完成

- 四板型正向、直角朝向、7° 旋转与一个一般透视分支已实现；继续补更广角度/透视、尺寸失败条件、裁切和失真图样的亚像素一致性。不得仅凭固定样本通过就宣称原厂检测完全等价。
- 在独立图像检测基础上实现平移/旋转、相机移动、矩阵修正、各误差输出，补充成功与失败分支像素/数值对照。
- 专属编辑器与原厂 parser 动态记录已补齐；完整桌面 UI 尚未采集，安装帮助截图不作为运行截图。
- 文件结构已完成；后续接入真实解算结果并完成运行生命周期、宿主注册、工具自运行和集成回归。

共享需求由主代理统一记录到 `CONTRACT-REQUESTS.md`：浮点标量输入、浮点数组输出、9 项标定矩阵和会话状态。`NImageRuntime` 已将拥有的图像批次、清图版本、算法调用点校验、真实识别和取消连接起来；仍等待统一数值结果和模块端口集成，未注册成完整工具。产品实现不依赖原厂 SDK；本次并行准备不能标记为单工具完整复现。

## 2026-10-18：共享首payload停止的完整回归

原厂813ca0的首成功payload决定identity及world原点；共享识别不再用后续identity一致性拒绝输入，公开Tags仅选中1个。既有候选角点排除仍以独立decoded-opening几何mask维持，尚非原厂873de0格网标记完整恢复，不能据此声称原厂会解码全部码区。

旧专属Tags.Count=4断言按新源码证据改为1，同时保持168点ID和全部原厂点数/像素门槛。该变更首先揭示生成board7多出1点（198 vs197）；共享owner修复identity选择与几何排除耦合后，完整NImageCalibration.PluginChecks正式8751项通过：9帧168点、7/9生成197点、6/8生成225点、透视及7度旋转。实际检查进程211.761秒，现登记solution/全量回归并给该项600秒预算。运行错误统一捕获并返回失败，避免未处理异常留桌面对话框。此项是有界原厂fixture及独立实现回归，不扩大到任意图像/native完整等价。

## 2026-10-19：NImage Display.xml 图像对象可见性接线

### 源码/资源确认

原厂 `D:\Program Files\VisionMaster4.4.0\Applications\Module(sp)\x64\Calibration\IMVSNImageCalibModu\IMVSNImageCalibModuDisplay.xml` SHA256 为 `89a9d3665d9a90cfb751008da7559f1bd179dba84a15f134d5117795a62014e8`；工作树派生资源 `src/VisionPlugins.Calibration/Resources/NImageDisplay.xml` 哈希一致。`Display` 节点的 `InImage` 对象声明 `Type="image"`、`IsDisplay="true"`，其 `Image` feature 映射 `InImage`。这确认默认对象可见和输入绑定；不是从用户截图或设计意图推定。

### 差异与修正

此前 `NImagePlugin.LastDisplay` 只投影 `ToolDisplayFrame.Image` 和图形。现在 LastDisplay 通过 `ToolDisplayImageLayers.Build` 将原厂 Display.xml 中图像对象的名字、映射图像、项目侧对象可见偏好和透明度构造成 `ImageLayers`，并设 `ImageVisible=false`，让呈现走源图像对象选择路径。项目侧 `DisplayObjectSettings.Visible` 与原厂 `IsDisplay` 的等价关系尚未证明。原厂 `RenderControlBase.UpdateVMResultShow` 在没有 SelectedImage 时选显示列表中第一个 image/pixelinfoimage，选中图像时直接取该对象值；两个分支均不检查 `IsDisplay`。因此 `IsDisplay=false` 不能单独推导出原厂不会把它显示为所选底图；隐藏图像叠加层也受另一个图像选择状态控制。

### 验证与边界

原厂来源：`RenderControlBase.xaml.cs` SHA256 `e13f97e8e82616056bf1ca02efb61f15527fa7ce8ca0724ef501c9a8f5efac67`，方法 `UpdateVMResultShow`；`DisplayImageListDecorator.cs` SHA256 `b1a5e6f41b69f8e755a5353fa79cccc789236fd1db7191a0cc22a7c9d6077a9`，方法 `RefreshImageList`。前者取 SelectedImage 的图像值，缺少选择时按 GetDisplayList 顺序选首个 image/pixelinfoimage；后者先按 Type=image/pixelinfoimage 构造来源，再按映射 IO 过滤加入图像列表。这两个分支均不读取 IsDisplay。按新增来源证据将对象 `Visible=false` 的测试预期改为“源选择路径仍可呈现已选底图”后，`dotnet run --project tests/NImageCalibration.PluginChecks -c Release --artifacts-path analysis/build/nimage-display-object-source-recheck -- --module-only` 重新通过68项。该测试验证的是当前插件 presenter 与对象偏好输入，不是原厂桌面差分运行。

原厂多图选择语义已部分确认：结果呈现优先使用 SelectedImage；没有选择时按 `GetDisplayList()` 顺序找首个 image/pixelinfoimage；图像列表仅按类型与IO映射组织，未用 IsDisplay 过滤。CheckedImages、BackgroundImage 等状态另行参与叠加显示。当前 NImage Display.xml 只有一个图像对象，缺少原厂宿主中的用户选择/勾选状态运行快照，因此不得把项目侧 `DisplayObjectSettings.Visible` 当作 IsDisplay=false 的原厂等价运行验证；原厂桌面画面仍未手动验收。

## 2026-10-19：标定点对顺序与 ProcessOutput

原厂模块 `ProcessOutput` RVA `0x21850` 从 SDK `GetCornerPairs(0)` 取第一个角点对，再按该对象 `GetCornersNum()` 索引顺序逐一写入 `OutImagePointX/Y`；它没有在模块层重排。SDK 返回的 `GetCornerPairs(i)` 序列及 world ID 已由四种官方生成板运行 traces 保存于 `n-image-calibration-generated-evidence.json`。本次在 `NImageGeneratedBoardChecks` 增加序列级比较：四板类型不仅逐 ID 对坐标，还检查产品 `NImageBoardRecognition` 的 world ID 顺序与 SDK `CornerPairs` 的顺序一致。`dotnet run --project tests/NImageCalibration.PluginChecks -c Release --artifacts-path analysis/build/nimage-candidate-order-check -- --generated-only` 通过2049项；四板ID顺序均一致，像素近邻最大误差均低于0.09 px。该比较为有边界运行证据，不能证明任意朝向、透视或失真输入的候选排序一致。

`NImageTrackedCorners.Build` 的输出端口投影仍有已记录的来源边界：按第一帧识别点的顺序枚举、跨批次按 world ID 取公共交集；NImage 模块的 OutImagePointX/Y 则只输出第一个角点对在每张图像中的像素坐标。新增的 module-only 生产检查把真实产品处理的九帧 OutImagePointX/Y 与同一帮助图派生输入的原厂SDK首 `CornerPairs` 九点轨迹逐帧比较，要求误差<1 px。这样确认了该样本的首项/顺序；没有把结论外推到其他候选同分、标签朝向和图像退化条件。
`NImageModuleChecks` 在第68项图像显示检查基础上又加入九个首 `CornerPair` 像素输出比较；`dotnet run --project tests/NImageCalibration.PluginChecks -c Release --artifacts-path analysis/build/nimage-first-track-source-recheck -- --module-only` 最新通过77项。产品真实执行的 OutImagePointX/Y 九帧顺序与原厂 `fixtureRuntime` 对应首角点轨迹逐项误差均<1 px。此比较只覆盖该帮助图派生批次。

## 2026-10-19：矩阵修正缺少输入时的采集生命周期

反编译 `analysis/full-audit/n-image-calibration/native/180020b90.c`（原厂模块 `Process`，RVA `0x20b90`，SHA-256 `EED1AE6F4FD0B0A6AC39ECA2F0F9EAC5EDE090B3BA234A2E10798EFFFC37F2C1`）可见，原厂先调用帧节点构造 `0x1f5a0`、插入帧链并递增 `m_frameCount`，然后才检查 `HomoFixEnable` 下的 `PicPointX`、`PicPointY`、`PhysicalPointX`、`PhysicalPointY`。四个 `VM_M_GetFloat` 失败分支都会汇入同一 `LAB_180021152` 跳过 `ProcessAlg`，随后仍调用 `ProcessOutput`。`ProcessOutput` RVA `0x21850` 的源码 SHA-256 为 `21A81AFD487ED2C915D19F18842747C0A9297896D699767B8B5EBDDA72A0DAE4`：无成功算法结果时仍写入当前 `CurNum`、配置 `TotalNum`、`CalibStatus=false` 和零值角点占位；Process 在调用它之前将模块运行状态位置为 1。成功自动保存位于 `ProcessAlg` RVA `0x21450`，源码 SHA-256 `BA435B09E9D566A362F4DC93E780E566A25F8E01247DB91465A53BA48866B3BB`，因此这个缺少点输入路径不会执行自动保存。具体 `VM_M_GetFloat` 返回的原始错误整数尚未通过原厂模块运行追踪确认；插件层仍返回自身的缺输入错误码 `0xe000031c`。

此前产品在采集图像前校验矩阵修正点，缺少任一绑定时帧计数保持不变。现已让这一分支按源码先记帧，再跳过识别、求解和文件写入；错误结果保留 `CurNum`/`TotalNum`、零角点占位、`CalibStatus=0` 和来源确认的 `ModuStatus=1`。其他输入错误路径未改变。受控专属回归分别让四个字段各自缺失，并用空白图确保若错误进入识别便会返回不同检测错误；每个变体均验证前三帧绑定完整、第四帧报错但计数到 4、没有 XML 输出，下一次补齐点后从满批清理并以 `CurNum=1` 开始新批。取消保留已有帧、新 Procedure scope 隔离和已有完整批次重置检查也通过。完整 `NImageCalibration.PluginChecks` 为 8,755 项；独立 `--module-only` 为 119 项通过。输出位于 `analysis/build/nimage-fix-input-lifecycle/` 与 `analysis/build/nimage-fix-input-module-only/`。两次构建前后 `NPointSolver.cs` SHA-256 均为 `F39DE6E04388B6BC9AA1B0C4B1B97B65E5C9B174380C7F8BCAA767A1C2A624CC`，源码检查 `sourceChangedDuringBuild=false`。这验证的是插件执行模型和相应源码路径，不是原厂 UI/VM 的逐项运行等价或桌面验收。

## 2026-10-19：首角点坐标残差复核

对上一节九帧逐帧差值作了审计。受控样本 `analysis/full-audit/n-image-calibration/board-0.bmp` 与产品测试输入 `tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png` 均为 1000×1000，使用 `System.Drawing.Bitmap.GetPixel` 穷举确认像素完全相同；原始 help 图裁剪/缩放及平移生成脚本为 `tests/NativeEvidence/Export-NImageFixture.ps1`，证据清单 `analysis/full-audit/n-image-calibration/fixture.json` 记录来源图、裁剪和各帧 SHA256。原厂 SDK 受控运行记录在 `docs/plugins/n-image-calibration-evidence.json` 的 `fixtureRuntime`（board 7、count 9、code 0）；原厂 SDK 探针 `tests/NativeEvidence/OriginalNImageOutputProbe.cpp` 遍历 `GetCornerPairs(0)->GetCorners(i)`，该源码文件 SHA256 `0D07104BB6EE108B53B04699DBCE6051120F55600893418A8038C8A058401A90`。

产品 `NImagePlugin.ExecuteAsync` 使用独立角点检测器的首轨迹输出；原厂模块 `CIMVSNImageCalibModu::ProcessOutput`（反编译 `analysis/full-audit/n-image-calibration/native/180021850.c`，SHA256 `21A81AFD487ED2C915D19F18842747C0A9297896D699767B8B5EBDDA72A0DAE4`）直接取结果中的第一个 CornerPairs，再按 `GetCornersNum`/`GetCorners(i)` 顺序将 x/y 传给 `VM_M_SetFloat`。所见模块层是 float 数据转交，没有额外像素坐标变换或整数舍入。因而目前能确认的差值发生在“产品角点检测输出”和“原厂 SDK CornerPairs 输出”之间；没有直接证据可进一步归因到两边的具体亚像素定位算法步骤，原厂算法内部定位策略仍未知。

实测首轨迹帧0产品点为 `(105.50076945759416,111.24954933734436)`，原厂 SDK 点为 `(105.408211,110.790932)`，差 `( +0.09255846,+0.45861734 )`；其余 8 帧平移后保持同样的 x 差，y 差最大变化约 `7e-6`，九帧最大坐标分量差小于 `0.459 px`。这不是像素输入不同或 `ProcessOutput` 中的整数舍入证据。测试已从宽松的 `<1 px` 收紧为每分量 `<0.5 px`，并检查重复平移下差值保持稳定（`1e-4`）；这是当前已确认输入范围的残差约束，不宣称零差或任意输入完全一致。未捕获原厂模块 ProcessOutput 的逐帧运行时端口 trace，因此端口投影值仍由静态源码和原厂 SDK 同输入结果交叉确认。

## 2026-10-19：原厂角点细化路径追踪边界

### 核心调用与输出边界补证

对安装版 x64 原厂库继续静态追踪后，可以把“SDK 内部不透明”的范围收窄到具体核心求解路径。`MVDNImageCalibV2Cpp.dll`（SHA256 `B67B7779C1F7E0BBFAA0F1F59C29DAAF266C3F4ED045F7C3E3416E9548281ADA`）的 `Run` 包装函数 `analysis/full-audit/n-image-calibration/sdk-native/180003d60.c` 在 RVA `0x3d60` 调用 `MVD_Algorithm_Process(handle, 0x72, input, output)`；其核心库 `MVD_Algorithm.dll` SHA256 为 `85D17ECC87D52E9A62D8BB86C8D5208AC999BCD31EC6D0806453F8165733F549`。核心分发在 `0x18001c4e0` 的算法类型 `0x72` 分支进入 `0x180021590`。该函数通过 `0x1807c6ba0(handle, 0x22, imageBatch, result)` 进入图像处理分发；分发对操作 `0x22` 选择 `0x1807fc310`，再进入输入检查函数 `0x1807cef50`，校验通过后调用 `0x1807cf3f0`。本链的 Ghidra 反编译导出由 `tests/NativeEvidence/NImageCore.java` 在当前工作树的 `analysis/n-image-calibration/residual-chain-decomp/` 生成；调用分发可见于导出的 `180021590.c`、`1807c6ba0.c`、`1807fc310.c`、`1807cef50.c` 和 `1807cf3f0.c`。

这条路径证实同输入会交给原厂核心算法类型 `0x72` 和其帧批处理/几何拟合过程；`0x1807cef50` 本身是参数、图像类型/尺寸、帧节点与有效区间的守卫，未在此守卫中重写检测点；`0x1807cf3f0` 接收帧数据和参数，包含后续拟合/质量量计算。C++ 包装 `0x180003d60` 将核心返回缓冲中的 float 字段构造成 CornerPairs 对象；模块 `ProcessOutput`（`0x180021850`）随后按 CornerPairs getter 顺序把点坐标交给模块输出。所审查的这两层没有发现整数取整或像素坐标系偏移，因此现有证据继续将约 `(+0.0926,+0.4586) px` 的差异定位在产品角点检测输出与原厂核心产生的 CornerPairs 之间；还不能据静态包装代码确认 `0x1807cf3f0` 内哪些内部步骤对应板标签解码或局部亚像素定位，也未证明其入口/整条输出路径实现了 CornerPairs 的创建。

源码/调用证据不等于算法同源实现：产品当前独立检测器没有逐指令移植这段原厂核心算法，因此未修改产品坐标来消除该样本残差。当前没有可证实的生产修复点；残差的精确成因仍未知。静态证据边界为 `MVD_Algorithm.dll` 的 `0x1807cf3f0` 几何拟合体及其下游核心实现，运行证据仍只有原厂 SDK 导出的 CornerPairs 数值，不包括该函数内部中间角点/细化响应 trace。

## 2026-10-19：平移/旋转参数到解算结果的端到端链

已把模块调用与现有生产数值链逐项对上。`ProcessAlg`（RVA `0x21450`）先从 `GetBasicParam` 清空算法基础结果，再按缓存帧链表顺序调用 SDK tool 的 `AddCalibImageData(image, ROI, regionMask, imageType, movePoint, angle)`，最后调用 `Run` 并取 `GetResult`；若启用矩阵修正，再把 SDK 矩阵和修正点传给独立矩阵修正对象。插件 `NImageSequence.Motion` 为每帧确定 translation/rotation kind、物理 X/Y 和角度，`NImageRuntime.Add` 将图像副本和 pose 按同序保留；`NImageBatchSolver.Solve` 再按 kind 切分并把各 pose 与该帧检测角点一起交给平移/旋转解算器。这与原厂先逐帧 Add、再一次 Run 的批次顺序吻合。局部变量/调用参数已经证明 SDK 得到每帧 movePoint/angle，但其 SDK ABI 参数的精确结构仍以公开接口头文件与受控输入 trace 为准；没有在原厂模块内部观察该 SDK 对象的逐字段内存快照。

`ProcessOutput`（RVA `0x21850`）在结果有效时按 getter 顺序投影：状态和方向、旋转中心、首个 `CornerPairs` 的点轨迹、平移平均/最大误差及索引、旋转平均/最大误差及索引、世界误差、帧间平移/旋转运动误差、方向向量。产品对应路径为 `NImageBatchSolver`、`NImageOutputMetrics.Measure` 和 `NImagePlugin.ExecuteAsync`；既有 `NImageOutputMetricsChecks` 对原厂 SDK 保存的组合 trace 逐字段比较上述数值、整数、状态，`NImageBatchSolveChecks` 比较 9+2 输入的旋转中心与合成矩阵，矩阵修正另由 `NImageMatrixFix` 专项原厂五组输入验证。原厂 9+0 和 9+3 模块 XML 的矩阵/轨迹也在相应检查中逐字段对照。检查已覆盖 0/多个旋转输入等边界；这不是这次重新执行后的新测试结果。

本次源码审计未发现 Rotation/Translation pose 被错序、遗漏或被产品插件另行改写的证据，也未发现一个有证据支撑的生产补丁点。原厂 `ProcessOutput` 暴露的矩阵/标量通过 SDK result getter 返回，模块 source 可确认字段名称、getter 顺序和写入路径；核心算法内部如何从 movePoint/angle 拟合全部系数仍委托给算法类型 `0x72`，其过程在 `0x1807cf3f0` 及下游，不能从 module wrapper 推断。NPointSolver 正在被另一条线按原厂 float 运算顺序修复；本轮因此不运行依赖该共享源码的数值回归，待其源码稳定后再由主线协调调用者回归。

继续追查局部角点/亚像素细化调用：原厂模块 `ProcessAlg` 反编译 `analysis/full-audit/n-image-calibration/native/180021450.c`，SHA256 `BA435B09E9D566A362F4DC93E780E566A25F8E01247DB91465A53BA48866B3BB`，创建/取得 `INImageCalibV2Tool`，清空 `BasicParam`，循环调用 `AddCalibImageData(image, ROI, regionMask, imageType, movePoint, angle)`，随后运行 SDK 并取 `Result`。该函数源码没有向 SDK 调用 `SetRunParam`，也没有局部角点定位、窗口选择或坐标精化代码；内部检测路径委托给 `MVDNImageCalibV2Cpp.dll`。

原厂 `IMVSNImageCalibModuAlgorithmTab.xml`（SHA256 `A83474FEC309E81EAED70366AD813A2D5A06A02A8DCC9F12D398079E16E5C3C3`）包含 `SubpixelWinAuto=1`、`SubPixelWindowSize=7` 两项，但两项均标记 `IsAbandon="True"`、`AccessMode=O`；资源未证明它们是当前活动的可调选项。模块的 Get/SetParam 反编译 `native/180020260.c`、`native/1800229d0.c` 中，XML 名 `SubpixelWinAuto` / `SubPixelWindowSize` 的 setter 分支直接返回0；旧名 `SubpixelWin` / `SubpixelWinSizeUserSetValue` 则被改名后传给 SDK `SetRunParam` / `GetRunParam`。SDK 公开头文件 `NImageCalibV2Cpp.h`（SHA256 `3849D63914EAE01FC23EADDDCA87FD534C11FC92059C09B7812DD5E7336D9873`）只暴露 `INImageCalibV2Tool::SetRunParam(key,value)` 通用接口，未在该接口文档中声明亚像素定位参数或细化算法。

因此不能根据这些废弃参数为产品的 `(+0.0926,+0.4586) px` 残差补偿；也不能确定 SDK 内部到底使用哪种局部细化方法、窗口估计方式、阈值或舍入顺序。现有 SDK 运行证据只观测其输出角点，不暴露中间响应或定位参数。未做猜测性算法改动，边界保留为未知。

### 同一模块实例的参数因果探针

为检查参数是否能改变原厂 SDK 结果，新增 `tests/NativeEvidence/OriginalNImageAbandonParamProbe.cpp` 与 `Invoke-OriginalNImageAbandonParamProbe.ps1`。探针校验原厂模块 DLL SHA256 `68C04FC9782E873A80776D6EA4BC1E0515B87D54E9D13B41DBD73799747525EC`，以 synthetic owned instance 构造一个模块及一个内部 SDK tool，使用相同9张 board-0…8 图像在同一实例中连续运行6组参数状态；无平台初始化、无设备。原始 trace 位于 `analysis/n-image-calibration/abandon-param-probe-361dfdc57f4d4dee84328662339bdd61/runtime.jsonl`，SHA256 `7D787F9DF0F1AD68D86DC2F00E7081D64C5104FCAAEC58500B58294B1DBC08C2`。

模块默认状态输出168个 CornerPairs，SDK 状态269496398。尝试写 canonical `SubpixelWinAuto` / `SubPixelWindowSize` 值（含 XML 的 1/7、窗口边界3/150、自动/手动切换）时，模块 `SetParam` 均返回0但 canonical `GetParam` 值保留在 synthetic 构造器状态1/1；setter 源码对应直接返回0的分支。通过旧别名写1/7、2/3、1/150、2/150时，模块 `SetParam` 代码同为0，别名 `GetParam` 从 SDK 读回空值；源码证实这些旧名被转交给 SDK `SetRunParam`，但该 void API 没有接受状态，不能从空读回解释为 SDK 内部处理方式。所有变体均为168个 CornerPairs、相同 SDK 状态和相同 homography；对全部3024个 x/y 角点分量逐项比较，最大绝对差为0。

该探针确认在此原厂模块版本、固定9帧样本与这些写入路径下，弃用 XML 参数没有改变 `ProcessAlg` 输出；它不能证明所有 SDK 输入或所有 `SetRunParam` 键值均不影响检测。原厂模块 `ProcessAlg` 不主动调用 `SetRunParam`，直接 `AddCalibImageData` 后 `Run`；XML canonical setter直接返回，legacy alias才转发。由于资源声明的窗口默认7在 ctor-only owned 模块实例中也无法通过 canonical setter设置，探针没有声称复现已初始化桌面宿主的全部 XML 参数生命周期。具体 SDK 亚像素算法仍未知，不据此对产品检测器做补偿。
