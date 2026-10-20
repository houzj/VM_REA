# 第十轮：标定板 `.iwcal` → 旋转标定源码与实际图链

2026-10-20。仅当前授权的标定十工具联合代码审核。本轮没有改产品、共享框架或 `tool-plugins`；原件和另一工作树均只读。特别先核文件字段再接图，不从“两个文件恰好都是 36 字节”直接推兼容。

## 原厂 producer/consumer 合同

| 边界 | 原始成员/资源与结论 |
|---|---|
| 标定板产生 | `MVDCalibBoardCalibCpp.dll` 的 Export 函数 `FUN_180003720` 取得当前 result 的矩阵对象，调用其 vtable+0x30 填充 `local_4a0`，随后 `fwrite(local_4a0,0x24,1,file)`；扩展名检查为 `iwcal`。原模块 `CIMVSCalibBoardCalibModu::ProcessAlg @0x180020240` 在求解结果成功且路径不存在或刷新使能时调用该导出。原厂受控十组 Export 均为 9×float32，见既有 `docs/plugins/CALIBRATION-BOARD.md`。 |
| Rotate 配置/装载 | 原 `RotateCalibModuAlgorithmTab.xml` 使用 `OpenFileForCalibDialog Name="FilePath"`，单选过滤 `.xml|*.xml;*.iwcal`；原 `CAlgorithmModule::SetParam @0x180038980` 按尾部小写 `ml`/`al` 分流。二进制 `LoadCalibFile @0x18003a340` 先 `fseek/ftell` 要求 `0x24`，后 `fread(this+0x15e20,1,0x24,file)` 原样填入连续 9 个 float，成功才写有效标志 `this+0x15e44=1`。无 XML 解码或另一个矩阵头。 |
| Rotate 数值消费 | `Process @0x1800376b0` 首读 `RefreshSignal`，非零经 `SetParam("FilePath",...)` 重新装载；若有效标志为 0 则先清采样返回 `0xe0000008`，不会采集本次点。完成总样本数时将 `(float*)(this+0x15e20)` 传入 `CalibRot @0x1800392e0`。`CalibRot` 明确取矩阵索引 `[0,1,3,4]` 作为图像点投影的 2×2 线性部分，然后从旋转点求轴心；并没有把 36 字节仅作 opaque 标识。原 `Process` 反编译中的恒定 `true/false` 是反编译歧义，实际 Count/读数门以既有 `ROTATE-CALIBRATION-TYPED-INPUT-SOURCE.md` 的指令核对为准。 |

因此 **Board 导出的九个 float 可被 Rotate 的 `.iwcal` 分支原样读入，Rotate 用其前两行的线性部分**。这是原模块/SDK 的结构与索引兼容，且本轮当前图实际重放核实了矩阵逐位交接；没有原厂桌面中“Board 节点直接接 Rotate 节点”的自动连线证据。二者之间实际载体是文件路径和刷新信号，不应发明 `CalibData` 直接输入端口。

原件 SHA256：`D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu/IMVSCalibBoardCalibModu.dll` `BD62EE940A2E299C0C47E41909E64CA78C09D7717E7CEFC629DBFB9E21105C1D`；其 `AlgorithmTab.xml` `76C9CD661A359216041B902CCE8FAE115566726707E749CC42C0BB9722BCBF91`。`D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDCalibBoardCalibCpp.dll` `0A65D7A9E84A3EC6F662B4F05DAA5A88351690AE4FBECD1748427C13C56FA308`。`RotateCalibModu.dll` `B08486B8481ED6D7774634B30B5CAE167F85B911EBF394122663BAB400A9BB41`，原 `RotateCalibModuAlgorithmTab.xml` `A6D2E3C283597AC044EC49D572A50C743F5221E0B858488F2A14D559E432E73E`。

只读反编译源路径/SHA256：`worktrees/tool-plugins/analysis/calibration-board/sdk-lifecycle-a0bfc843caa84d41913e6a5c95cebc89/output/180003720.c` `92FDB7E1F05D1BE1969CF5FA1E49C98F83C4EC4DD657D68A39310A1FE1E8424D`；`worktrees/tool-plugins/analysis/calibration-board/native-decompile/output/180020240.c` `54F2D3D7E37CE1DFBA973629F8B135D450E2DF7B78AC5263FB71889B5E8A073A`；`worktrees/tool-plugins/analysis/full-audit/rotate-calibration/native/180038980.c` `487625BEFE572F7CBBCCD887CAE0FC677F0CC4F2A57419958F3C95A06828892C`；`18003a340.c` `A6042F2AF785EBE281C2AD8E4F36CF7E3E4C89470F2E28D96E69D3FC4F1DEB86`；`1800376b0.c` `4B92DBF021269B9C1942B419DF5260CE0757DA3F059C9FE33D976884C13ACE01`；`1800392e0.c` `BDD0E9CBF8B1B4CA246A40E25B46B67DBCABD7B9E01780BEF275CFEF064175F4`。

## 当前图链与实跑

当前 `CalibrationBoardPlugin` 接 `ImageSourcePlugin.Image` 的 Gray8，识别/拟合后独立发布 `CalibData.ImageToWorld`，并在 `CalibPathName`/`RefreshFileEnable` 条件下以 `CalibrationBoardFile` 写 `.iwcal`。`RotateCalibrationFile.Read` 按 36 字节逐项读浮点，`RotateCalibrationSession.Run` 首先消费 `RefreshSignal`，然后读取 `RotNum` 与图像 X/Y/角度；成功帧在同一 `ModuleExecutionScope` 累积，失败清样本。为了避免虚构一个原厂自动连接，本测试以 test-only `SampleSource` 给旋转点、数量和刷新信号，Graph 执行依赖按 `ImageSource→Board→SampleSource→Rotate` 排列；唯一跨工具数据介质为 Board 文件。

实际命令：`dotnet run --project tests/CalibrationBoardRotateChain.Checks/CalibrationBoardRotateChain.Checks.csproj -c Release -- analysis/joint-audit/pass10-cal-board-rotate/probe`，退出 0，**20/20** 断言。480×400 合成棋盘 Gray8 每帧使真实图像源→Board 识别 99 点并写 36 字节 `.iwcal`；读取出的九项逐 float32 位匹配 Board `CalibData.ImageToWorld` 和 Rotate 文件 reader。连续三次图执行的 Rotate `remainCount` 为 2→1→0。保存重开图时 IMAGE 绑定与五条样本订阅仍在。随后仅删除自有 probe 目录中的文件并提交空白图，Board `NoMatch/0x10103000` 不重写文件，Rotate 强制刷新缺文件后 `Failed/0xe0000008`；再次提交正常图，Board 重建文件，Rotate 在同一 scope 恢复且剩余 2，证明失败清样本与重新首帧。日志 `analysis/joint-audit/pass10-cal-board-rotate/run.log`、结果 `probe/summary.json`、`probe/board.iwcal`、`probe/board-rotate.vmx.json` 保存在本轮独占目录。

本轮只证明当前受控棋盘、默认 Board 参数和 test-only 旋转点的文件交接与状态门。未证原厂桌面同图操作、任意 Board 参数/透视矩阵下旋转数值同位、物理单位解释、相机真实采样、原 SDK 写关闭失败后文件状态，以及其他 XML 写者到 Rotate 的全部兼容范围。测试里的旋转点是明确独立输入，不代表 Board 自动产生旋转采样。图保存是当前 JSON 格式，不宣称原厂方案 ABI 字节兼容。
