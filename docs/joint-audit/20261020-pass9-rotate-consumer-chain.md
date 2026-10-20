# 第九轮：旋转标定的文件消费者与连续采样链

范围仅为“标定”十工具中的旋转标定消费者、相机映射文件生产者及既有 N 点／映射／平移旋转 XML 产物的格式交接。原件只读，未改产品、共享框架或 `tool-plugins`。本报告把原函数结论、当前实现实测和未证原版整图分开。

## 原件入口、状态和输出

安装件 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/RotateCalibModu/RotateCalibModu.dll` SHA-256 `B08486B8481ED6D7774634B30B5CAE167F85B911EBF394122663BAB400A9BB41`。原函数反编译位于只读取证目录 `worktrees/tool-plugins/analysis/full-audit/rotate-calibration/native/`：

| 成员/RVA | C 体 SHA-256 | 可核实的分支及状态 |
|---|---|---|
| `CAlgorithmModule::SetParam` `0x38980` | `487625BEFE572F7CBBCCD887CAE0FC677F0CC4F2A57419958F3C95A06828892C` | `FilePath` 非空后按路径末两字节精确选 `ml` 或 `al`；前者传 `LoadFile(...,true)`，后者传 `LoadCalibFile`；调用后即写路径，即使装载报错；其他后缀返回 `E0000204`。原指令 `38980..38b1a` 已核 ABI 和分支。 |
| `LoadCalibFile` `0x3a340` | `A6042F2AF785EBE281C2AD8E4F36CF7E3E4C89470F2E28D96E69D3FC4F1DEB86` | 先清有效位 `+0x15e44`；以 `rb` 打开，严格 `ftell=0x24` 并读 36 字节到 `+0x15e20` 的九 float 缓存；成功才置有效位；打开失败 `E0000201`，长度／读取失败 `E0000204`。 |
| `LoadFile` `0x3a670` | `1FCE51DAAF54E214DA6DF07C1DCE3E5178B82AEA5320F18FB7513777330D4AC6` | 同样先清有效位；定位 `CalibOutputParam`，其子项 `DataType="FloatList"` 写九 float 缓存并置有效位；缺节报 `E0000017`。短列表保留旧尾项，超九项原生越界；独立实现有显式九项边界。 |
| `Process` `0x376b0` | `4B92DBF021269B9C1942B419DF5260CE0757DA3F059C9FE33D976884C13ACE01` | `RefreshSignal` 第 0 项非零时对缓存 `FilePath` 再调用 `SetParam`；有效位缺失先 `DeInitVecData` 返回 `E0000008` 且未写状态输出；其后 `RotNum` 限 3..16，依次取 `ImagePointX/Y`、`WorldRotateAngle` 第 0 项；中间样本写 `ModuStatus=1`、`remainCount` 和零中心，满额调用正反 `CalibRot`、清样本并写六个中心／误差输出。Ghidra 对 Getter 条件显示恒真／恒假，实际门已在 `docs/plugins/ROTATE-CALIBRATION-TYPED-INPUT-SOURCE.md` 按 `37906..37a1d` 原指令核对，不据该伪常量推断。 |

原端口为旋转样本 `ImagePointX/Y`、`WorldRotateAngle`、`RotNum`、`RefreshSignal`，没有 `CalibMatrix` 输入。文件路径是矩阵消费者的正式交接面。当前 `RotateCalibrationSession.Load/Run`、`RotateCalibrationFile.Read` 与 `RotateCalibrationPlugin.Runtime` 使用对应矩阵缓存、有效位和顺序采样；独立方案 JSON 缓存不冒充原生方案 ABI。

## 当前真实图的可复验结果

`tests/CalibrationRotateConsumer.Checks/Program.cs` 创建真实 `GraphEditorDocument`：相机映射两组四点通过 `WorkflowSubscription.NumberArrayConstant` 进入原端口，执行边连接相机→采样源→旋转；相机 `CalibStatus` 接旋转 `RefreshSignal`，采样源连续给出 `RotNum=3`、X/Y 环形三点与 `WorldRotateAngle=120`。同一 `ModuleExecutionScope` 连跑三次，每轮相机重写自身目录内的 36 字节 `camera.iwcal`，旋转在相机之后刷新并消费该文件。三轮两节点全部 Success/原生码 0，`remainCount=2,1,0`，前两轮中心 0，第三轮图像中心 `(50,50)`。文件 SHA-256 `E2196EC26DEC21D729B86D14FA52A70AF217D9B4C87D7B40681AF01B66BFC06D`。这是**当前实现真实图交接**，并非原版 VisionEditor 同构整图实跑，也不替代原版同输入数值差分。

独立恢复图仍走 `SolutionExecutor/GraphEditorDocument`：先删除自身 `camera.iwcal`，保留相同 `FilePath` 且 `RefreshSignal=1`；旋转返回 Failed/`E0000008`，输出集合为空。写回刚生成的 36 字节文件，在**同一 execution scope** 再跑则 Success/0、`remainCount=2`，证明当前刷新重新装载和失败清采样。直接读取不存在的 `.iwcal` 得 `E0000201` 且 `loaded=false`；这是 `LoadCalibFile` 底层结果，`Process` 因无有效矩阵另返回 `E0000008`，不得混为同一码。

对既有真实生产物只读校验：`analysis/joint-audit/pass6-cal-result/npoint-generated.xml` SHA `C04A0EB0B5D3C6DD45F62ABEF63184E286D0234D5FC1911012123E36E2175601`、`analysis/joint-audit/pass8-cal-map/mapped.xml` SHA `BDA8736F696E5B8721F811B31BE8486D3C09B436BD32CFE30A1587221275E967`、`analysis/joint-audit/pass8-cal-translation/npoint-generated.xml` SHA `9E63AE925797C5F5778E75FA8AF7373B5AB88A7A5A3CD9BD1A284F4111D5CA0C`，三者经当前 `RotateCalibrationFile.Read` 均 `0/loaded=true`。这些是**预存产物格式兼容探针**；本轮未把三个生产者分别放进同一运行图，也未把 XML 测试说成原厂完整运行认可。相机 `.iwcal` 则在本轮图内现生成、现消费，关闭一条完整可执行文件链。

命令 `dotnet run --project tests/CalibrationRotateConsumer.Checks/CalibrationRotateConsumer.Checks.csproj -c Release` 最终退出 0，stdout `analysis/joint-audit/pass9-cal-rotate/checks.log`。首个构建尝试遇其他并行线共享 `VisionPlugins.ImageSource/obj` 的 `CS2012` 写锁，未改进程或文件；错峰重试成功。验证日志保留失败尝试应由协调线另计。当前生产代码未修改，本切片没有足以提出主框架独占修复的差异。

## 未覆盖与接口判断

1. 原版平台内 CameraMapping→Rotate 的同构完整图、回调调度和同数据集数值差分未运行；只证两个原件各自文件读写合同与当前整图。相机生产者本轮复用第八轮的原函数证据，不重复宣称其 SDK 所有算例合格。
2. N 点、映射、平移旋转三份 XML 仅现有产物的消费者直接解析，不等于本轮原件生产者重新执行；短 XML 缓存尾保留与坏 XML 的全部状态组合可依据旧 Rotate 原定点取证，未在这张图中扩大测试。
3. 旋转输出没有原版直接矩阵端口。旋转由文件取得变换，再输出中心／误差给后继；不能把 CameraMapping 的 `CalibStatus` 当矩阵、也不能把旋转虚构为标定加载的矩阵生产者。后继若需要中心坐标，应按实际六个输出端口逐个核订阅。
