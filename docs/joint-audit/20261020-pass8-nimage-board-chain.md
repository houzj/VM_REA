# 第八轮：N 图像与标定板的图像输入及结果交接

仅核“标定”分类两项生产者及其必要图像源、标定加载、坐标系交接。第七轮共同配置窗和第六轮 N点结果链不重计。原厂原件/反编译只读；探针在 `analysis/joint-audit/pass8-cal-image/`，没有修改产品或 `tool-plugins`。

## 原版入口与载荷身份

| 专有边 | 本轮重核的原件、成员与实际含义 | 对当前框架的限制 |
|---|---|---|
| N图像的 IMAGE、运动点入口 | 安装 `IMVSNImageCalibModu.xml` SHA-256 `DAC2AA0C023F712DD0E693CE285FA66DE1E77DC4DD121060C573706A430E88FF`：必需 `InputImage` IMAGE 四叶 `InImage/Width/Height/PixelFormat`，可选 `PhyPoint`、`Image Point`、`Physical Point` POINT 组合和 `RotateAngle` float。安装 `IMVSNImageCalibModuAlgorithmTab.xml` SHA `A83474FEC309E81EAED70366AD813A2D5A06A02A8DCC9F12D398079E16E5C3C3` 的 `ImageSource` 经 `GetFrontParamItemsOperation(IMAGE)` 初始化、`GetSelectedCombinationOperation(InputImage)` 读订阅、`SetCombinationSourceOperation(InputImage)` 写订阅。 | IMAGE 下拉候选、真实数据连接和算法认可图像是三道不同检查。当前 `NImagePlugin.Descriptor.InputImage` 是 `DataKind.Image`，运动点分别有别名叶；九帧真实主窗专项已跑。 |
| N图像算法结果→XML | 原 DLL `IMVSNImageCalibModu.dll` SHA `68C04FC9782E873A80776D6EA4BC1E0515B87D54E9D13B41DBD73799747525EC`。`CIMVSNImageCalibModu::WriteXmlData` RVA `0x230e0` 的原生反编译在 `worktrees/tool-plugins/analysis/n-image-calibration/file-export-source/output/1800230e0.c` SHA `F7437CF38A9B6C8BBF1B3D0B8095EDB06A9D3C1A315F4FBB2B5D2CFBE0FC6F67`：读取 SDK 结果，缺结果有 `0x80100011`，输出 `CalibOutputParam/CalibFloatListParam`、`ParamName=CalibMatrix`、`DataType=FloatList`。原模块 XML 的公开输出列 `OutImagePointX/Y` 数组，没有公开 `CalibData` 对象端口。 | 当前 `NImagePlugin` 的 `CalibData:Calibration` 是独立便利对象；跨原版标定加载/坐标系的可复核矩阵运输是文件中 `float[9]`，不能把便利对象假称原厂端口。`NImageCalibration.HostChecks` 已通过九帧真实 ImageSource 图，导出的是主窗 scope 内实际 SDK 批结果。 |
| 标定板 IMAGE/ROI→输出 | 安装 `IMVSCalibBoardCalibModuAlgorithmTab.xml` SHA `76C9CD661A359216041B902CCE8FAE115566726707E749CC42C0BB9722BCBF91` 声明 `ImageSource` 的 `InputImage` 候选/写订阅及 `ROISelecter` 单个 Box/FullScreen；原 XML SHA `19227E4FCDF800BA38447445BF3561BE07F40A9AFA6610E198F536600DFC19FF` 声明 IMAGE 四叶、`Calibration Point` POINT 数组的 `CalibPointX/Y` float 数组、ROI 与标量，未声明 `CalibData` 对象。原 DLL SHA `BD62EE940A2E299C0C47E41909E64CA78C09D7717E7CEFC629DBFB9E21105C1D`；`ProcessAlg` VA `0x180020240` 反编译 SHA `54F2D3D7E37CE1DFBA973629F8B135D450E2DF7B78AC5263FB71889B5E8A073A`，`ProcessOutput` VA `0x180020630` 反编译 SHA `E13AF4828717AFE54ED4A9B54947CB21300427482FA81432D709779F044F66C0`。后者先发布 ModuStatus/ROI 检测框，算法成功后发布 CalibPointX/Y 等结果；原受控 SDK 导出给 `.iwcal` 的 36 字节 little-endian float32 矩阵，先前模块/SDK探针详见 `docs/plugins/CALIBRATION-BOARD.md`。 | 当前板插件输入要求 `GrayImage`；图像源的普通 `Image` 可携带 Gray8 实值。当前 `CalibData:Calibration` 也是独立便利对象；板 `.iwcal` 是二进制 SDK 文件，不可冒充标定加载的 XML。 |

## 可复验当前生产图

先以 `dotnet run --project tests/NImageCalibration.HostChecks/NImageCalibration.HostChecks.csproj -c Release -- analysis/joint-audit/pass8-cal-image/nimage-host` 在真实 `MainWindow` 中由图像源九帧→N 图像运行→导出实际 XML。此专项本次 49 项通过，日志 SHA `E581375E9DAAA185567E7CBFE8F5246C47BED079A3B890181B86CE3D8238892B`，XML SHA `35AD1EF1CB6078D00401D842948F60057260459EF6A5DF097CEEFED53D1EADB4`。

再运行 `dotnet run --project tests/CalibrationImageProducer.Checks/CalibrationImageProducer.Checks.csproj -c Release`，测试源码 `tests/CalibrationImageProducer.Checks/Program.cs`，最终 stdout SHA `A207E61CFDE120B09690792F0A15FC1DAFD423B8E9D947536A3234651AC7FE92`。独立图文档是 `ImageSource.Gray8 IMAGE → 标定板`，同一图另由 `ReadCalibFile(N图像刚导出的XML).CalibMatrix → 坐标系`，并把源图接给坐标系；给读文件器加板的显式执行前驱。四节点在同一次 `SolutionExecutor` 运行中均成功：板检测 99 点，发布 `CalibPointX:NumberArray` 99项与独立 `CalibData:Calibration`；板写出 36 字节 `.iwcal` SHA `7FF08C971F1F0CB69ECC4816421D0A3B53895B89169BF26AFBEA553493997BF8`。标定加载把 N图像 XML 的九项矩阵发布为 `NumberArray`，坐标系发布九项 `CorCalibMatrix`；方案保存重开保留三个 typed 数据边和 XML 路径，生成方案 SHA `11563279881714AAB7FD2989761E2D4DCBA01EEC4DE5C47584F9B0018C829F12`。当把板二进制 `.iwcal` 交给当前 `ReadCalibFileData.Read`，精确返回 `0xe0000201`；因此当前完整闭环**分别**是 N图像 XML→标定加载→坐标系与标定板 Gray8→点/矩阵/二进制文件，不存在两格式间的隐式转换。

首次探针错误地把 Gray8 输入连到图像源可选 `ImageMono8`；该端口只在 RGB 加 `OutMono8` 时发布，于是标定板 `UpstreamUnavailable`、后继 `ExecutionPredecessorUnavailable`。失败输出已单独保存 `analysis/joint-audit/pass8-cal-image/producer-first-failure.log` SHA `358A598D37CFC1CA3398AABD756C36CE2BA13608BD37D4D0D8BD799409CBAA57`；修正连普通 `Image` 后通过。它是探针连线错误，不是产品缺陷，避免后续再次用下拉端口名推断实际发布。

## 未证与下一步

本轮复用了旧 N图像九帧源码/数值专项，但没有重新取其 `ProcessAlg/ProcessOutput` 完整体，也没有在原厂 VisionEditor 桌面运行同构混合图；原厂板 `.iwcal`→其他原版消费者的可达范围尚未确定。Board 仅测 checker/全图/Gray8、N图像仅测已有九帧基准；两者其他板型、ROI/掩膜、彩色输入、失败后旧结果/文件保留、跨 Group 与撤销状态不能由本图宣称完成。当前 XML 加载器对 `.iwcal` 的拒绝与其 XML 解析入口一致；尚无证据要求 main 添转换或改两工具产品。继续审核两类独有失败与 Group/后继组合时应复用本报告的载荷边，不重复共同窗口证据。
