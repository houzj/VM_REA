# 第十一轮：N 图像标定失败批次、恢复与 owned Group 消费

范围仅为标定分类的 N 图像标定状态链及其标定加载/坐标系消费。第八轮已覆盖正常九帧链；本轮增量是**第九帧失败后的拥有者状态、下一帧重置、恢复批次，以及保存重开后的 owned Group 传输**。本报告的成功均指当前重建实现的自动化探针，未把它当作 VM 原桌面运行结果。

## 原始证据与行为门槛

| 证据 | SHA256 | 能确定的事实 |
| --- | --- | --- |
| `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModu.dll` | `68C04FC9782E873A80776D6EA4BC1E0515B87D54E9D13B41DBD73799747525EC` | N 图像标定原生模块身份。 |
| 同目录 `IMVSNImageCalibModu.xml` | `DAC2AA0C023F712DD0E693CE285FA66DE1E77DC4DD121060C573706A430E88FF` | 原模块接口资源。 |
| 同目录 `IMVSNImageCalibModuAlgorithmTab.xml` | `A83474FEC309E81EAED70366AD813A2D5A06A02A8DCC9F12D398079E16E5C3C3` | 原算法页资源。 |
| `worktrees/tool-plugins/analysis/full-audit/n-image-calibration/native/180020b90.c`，`CIMVSNImageCalibModu::Process` | `EED1AE6F4FD0B0A6AC39ECA2F0F9EAC5EDE090B3BA234A2E10798EFFFC37F2C1` | 进入新帧前若现有数达到平移+旋转总数，销毁/清空旧图帧；读取三个运动输入，追加帧并增加计数；达到总数才调用 `ProcessAlg`，随后调用 `ProcessOutput`。见约 101–112、124–184、185–263、276 行。 |
| `worktrees/tool-plugins/analysis/full-audit/n-image-calibration/native/180021450.c`，`ProcessAlg` | `BA435B09E9D566A362F4DC93E780E566A25F8E01247DB91465A53BA48866B3BB` | 按持有的列表顺序向 SDK BasicParam 增加图像/运动数据，然后调用 SDK Run；结果存在且开启刷新文件时调用 `WriteXmlData`。见约 85–160 行。反编译只显示虚表调用，不能由此确定 SDK 内部算法。 |
| `worktrees/tool-plugins/analysis/full-audit/n-image-calibration/native/180021850.c`，`ProcessOutput` | `21A81AFD487ED2C915D19F18842747C0A9297896D699767B8B5EBDDA72A0DAE4` | 输出阶段读取 SDK 结果及字段；原错误路径/全部输出值仍受原生 SDK 返回约束。 |
| `worktrees/tool-plugins/analysis/n-image-calibration/file-export-source/output/1800230e0.c`，`WriteXmlData` | `F7437CF38A9B6C8BBF1B3D0B8095EDB06A9D3C1A315F4FBB2B5D2CFBE0FC6F67` | 文件含 `CalibMatrix`；SDK 结果缺失分支可返回 `0x80100011`，见约 130–195、829 行。 |

原 SDK 结果缓存生命周期还有独立原生探针，见 `docs/plugins/N-IMAGE-RETAINED-SDK-RESULT-SOURCE-AUDIT.md`。原模块仅清帧列表时，旧 SDK 结果可能仍保留；真正进入 SDK Run 才清结果标记。因此不能把“帧重置”泛化为“所有结果均被清除”。本轮第九帧是 **SDK Run/识别失败** 路径，和只清帧、不运行 SDK 的路径不同。

## 当前实现与实际探针

探针源：`tests/CalibrationNImageStateChain.Checks/Program.cs`；原图样本 `tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png`，SHA256 `CEA3978AF8112865B13E21D76646887DFDEC8D7F1A078BE4F3CB8A39A541912D`。样本派生 Gray8 平移帧；第九帧为恒值 Gray8 空白帧。真实 `GraphEditorDocument` 接线 `ImageSource.Image → NImage.InputImage`，各帧通过 `ModuleExecutionScope` 送入 `SolutionExecutor`，三个运动参数经订阅输入。不是直接调用 `NImageSequence.Add` 来冒充图运行。

运行命令（工作目录为仓库根）：

```powershell
dotnet run --project tests/CalibrationNImageStateChain.Checks/CalibrationNImageStateChain.Checks.csproj -c Release -- analysis/joint-audit/pass11-cal-nimage-state/probe
```

结果：退出码 0，**26/26**；完整日志 `analysis/joint-audit/pass11-cal-nimage-state/run.log`，实际输出 `probe/recovered.xml`、`probe/nimage-group.vmx.json`、`probe/summary.json`。

1. 前八帧的公开 `NodeRunRecord.CurNum` 和 NImage owned `Inspect.Current` 均从 1 增到 8。
2. 第九空白帧达到总数后识别失败：`NodeRunRecord.Status=Failed`，当前实现错误码 `0x10103000`，公开 `CurNum` **缺席**；owned `Inspect.Current=9`，无自动 XML。公开输出缺席是框架对失败节点输出的处理，不能说原厂 `ProcessOutput` 没有执行。
3. 同一 owned 状态下显式导出抛出 `NImageExportException`，当前错误码 `0x80100011`；这与原 `WriteXmlData` 的 SDK 结果缺失分支一致，但尚未对原厂输入同一张空白图做运行差分。
4. 下一张有效帧启动新批次，公开/owned 当前数均为 1；再送八张有效帧后产生可读 XML，`CalibMatrix:NumberArray` 长度 9。此处验证的是新批次状态与消费形状，**不是**原厂数值矩阵的浮点位等价。
5. 新建真实图：`ImageSource` 执行至 owned Group；Group 内 `ReadCalibFile` 读取恢复 XML 并由 Group 输出 `CalibMatrix:NumberArray`；外层 `Coordinate` 同时接矩阵和实时 `Image`。Group 与 Coordinate 均成功，Coordinate 的 `CorCalibMatrix` 为九元素。保存方案、重开，确认 Group body/矩阵边仍在，重新送图后 Coordinate 再次成功。保存重开并未保存 NImage 尚未完成的采集会话；这里只保存了完成后的 XML 消费图。

当前实现对应 `src/VisionPlugins.Calibration/NImageSequence.cs` 的满数后清帧再追加、`src/VisionPlugins.Calibration/NImagePlugin.cs` 的运行/导出状态、`src/VisionFramework/GraphExecutor.cs` 的失败节点输出发布和 `src/VisionFramework/SolutionExecutor.cs` 的图执行。Group、读取器和坐标系的源码/规格链另见第八轮报告；本轮检验其在失败恢复后的实际组合，并不重新声称原版 Group 行为已验证。

## 留存的失败与未知

- 首次编译的项目引用缺失、第二次命名空间错误分别保存在 `analysis/joint-audit/pass11-cal-nimage-state/compile-failure.log` 和 `compile-namespace-failure.log`；修正仅在本探针项目内，最终运行成功。
- 未运行原厂桌面/原 SDK 对同一张空白图和恢复序列的差分。`0x10103000` 是当前实现本次探针返回，不能冒称原 SDK 同图错误码。
- 原 `ProcessAlg` 的 SDK 虚表调用顺序可见，但 `AddCalibImageData`、Run 和 GetResult 的具体识别/数值分支及 UI 错误呈现，须另以 SDK 原函数和运行证据约束。
- 原只 Clear 而未 Run 时的保留结果与本次识别失败需分开；本次探针不证明所有 Clear/失败分支都无可导出旧结果。
- 未测试批次采集状态跨方案关闭重开持久化；本轮重开验证的是完成文件经 owned Group 的消费链。
