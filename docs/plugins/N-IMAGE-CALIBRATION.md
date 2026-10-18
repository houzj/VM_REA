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
