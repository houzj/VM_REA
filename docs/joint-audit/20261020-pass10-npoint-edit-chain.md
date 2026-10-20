# 第十轮：N 点标定单格编辑到运行文件的联合审核

仅审“标定”十工具内的 N 点表格编辑。原件和产品源码只读；受控探针只写 `tests/CalibrationNPointEditChain.Checks/` 和忽略目录 `analysis/joint-audit/pass10-cal-npoint-edit/`。截图反映慢或卡的现象，但不替代调用链或计时证据。

## 原版窗口与 setter 链

原安装 `IMVSNPointCalibModu.dll` SHA-256 `F8E07023A816C55FF07906E8889F45B93658F59CE88F040AD2204A70922F4839`。托管反编译位于只读 `worktrees/tool-plugins/analysis/full-audit/n-point-calibration/managed/control/IMVSNPointCalibModuControl/`：`NPointList.xaml.cs` SHA `B972D12AF77B27DC3A19E00629C77F3BED58D3FD7A266DD0AA28050A281B5A46`，`PointList.cs` SHA `A396060C73DBF56B1A7FB7849A4BCD17FE5CBC3D04AD8356F68D4B3B1C9B3813`，`NPointList.xaml` SHA `92945CE0D5ED92F98F12187A0EB92F1673516760B894F0CD52F89FC2C7341D79`。`EditButton_Calib` 的 XML 内嵌 `IMVSNPointCalibModuControl.NPointList`；原 `UserControl_Loaded` 调 `Init` 并把窗口高改为 399。

原 `NPointList.Init` 先读 `CalibPointTotalNum+RotPointTotalNum`，逐行对 `ImagePointX/Y`、`WorldPointX/Y`、`DeltaAngle` 各读取并 `float.Parse(...).ToString("f3")`，构造 `PointList` 时依序赋五个属性。五 setter 仅在文本变化时向 `((IUserStringData)OwnerParam)[属性名+ID]` 写**该单字段**；失败时不更新显示 backing field，成功后才发 `PropertyChanged`。默认 backing 为 `"0"`，因此默认数值格式化 `"0.000"` 仍是一次属性变化；默认 9+3 行可有 60 次**原版单叶 setter 写入**。不能把“打开时 60 次 setter”本身称为当前实现特有 bug。

原原生 `CIMVSNPointCalibModu::SetParam` RVA `0x3e560` 的 C 体 `worktrees/tool-plugins/analysis/full-audit/n-point-calibration/native/18003e560.c` SHA `D28D9DD6D9618EEE3F877223FA8148F9BB3A3CBB5721061919BBFB55DBDDA95B`，按五种参数名前缀和尾部索引进入对应处理；Ghidra 对若干 `sscanf_s` 目的地址未可靠还原，此处只把它作为单叶 native 调用分支证据，不声称精确内部矩阵偏移。

## 当前产品的额外放大路径

当前 `NPointConfigurationPointEditor` ctor 也按行五字段调用 `commit(i,a,formatted)`，单格 setter 调一次 `commit`，这一层与原托管 setter 粒度一致。差异在 `NPointConfiguration.CreateConfiguration` 所供回调：**每一次单叶提交**都执行 `PointValues()`，从 `Snapshot(Current())` 复制全部 32×5=160 点叶为新的 JSON 参数字典，再覆盖一个字段；随后 `Apply(next)` 把整个字典交 `ToolEditorContext.Apply` 并调用 `UpdateConfiguration`，后者 `Synchronize` 又扫描全部 160 点叶与完整参数字典。

主窗口 `ToolConfigurationHost.OpenToolConfiguration` 的 `Apply` 调 `GraphEditorDocument.SetProcedureNodeParameters` 并记执行 scope 参数；该图方法验证/复制整份参数，`Edit` 执行完整方案复制/结构验证、`RecordEditHistory`、递增 revision、同步触发 `Changed`。此配置宿主没有用 `ExecuteConfigurationEdit` 包住 N 点窗口。`MainWindow.xaml.cs` 的 `doc.Changed` 回调每次调用 `RefreshAll`；`WorkflowTabs.RefreshAll` 进一步 `GraphValidator.Validate`、`RenderGraph`、`RefreshRight`，而 `WorkflowCanvas.RenderGraph` 清空并重建图层。因此默认 12 行窗口初始化的 60 次单叶 setter，在当前宿主成为 **60 次整节点/方案编辑、历史记录和主窗口刷新**；单格实际编辑也走一次该整链。此为源码可证的性能放大点，不是已经量到的原厂/当前时延比，也不能单凭它断定任何一次系统“未响应”的唯一原因。

安全的修复方向仅限主框架对参数批量落盘/Changed 合并或配置操作历史合并，同时保持原单叶 setter 成功/失败、格式化、即时回写及结果可见时序；插件项目拥有点表和配置回调。具体跨项目合同应先列 FCR，不能在 main 另造点表配置框架，也不能把五个原 setter 错改为一次 native 批量写入。用户桌面卡顿需要另做交互桌面同机时序，当前受控 API 计数是机制证据。

## 受控同 API 探针与边界

`tests/CalibrationNPointEditChain.Checks/Program.cs` 在 STA 构建当前真实 `NPointConfigurationPointEditor`，以产品回调同构算法写入真实 `GraphEditorDocument.SetProcedureNodeParameters`，对单格编辑执行 Undo/Redo，再保存/重开图并通过 `SolutionExecutor` 运行 N 点→文件→标定加载链。没有自动点击用户交互桌面，也没有创建/关闭他任务窗口；因此不声称已做 MainWindow 实体交互验收。探针的 GraphEditorDocument Changed 次数是确定可复验的内核计数；MainWindow 每次 Changed 的处理路径来自上述产品源码。

命令 `dotnet run --project tests/CalibrationNPointEditChain.Checks/CalibrationNPointEditChain.Checks.csproj -c Release` 最终退出 0，原始 stdout `analysis/joint-audit/pass10-cal-npoint-edit/checks.log` SHA-256 `3502D651D5A6C2E3047F71F7EFF6DE9519DFCF828DE038D38DC8C033BF86586A`；协调线独立复跑也退出 0，日志 `analysis/joint-audit/pass10-cal-npoint-edit/root-run.log` SHA-256 `6342C9071DCD54BECE0E0F1DBA63B2FC2F6B4572610088F237AB608893FCCC18`。四行点表在**仅创建当前真实控件**时发生 `Apply=20`、`Changed=20`、revision 增 20；随后一次 `TrySetCell(0,WorldX,"2.500")` 再增一次，Graph Undo 恢复原 0、Redo 恢复 2.5，保存重开后值仍 2.5。重新执行图中 N 点和标定加载都 Success/原生码 0，已编辑点参与求解，N 点写出 XML，加载器解析九项矩阵。首跑 XML SHA-256 `D4009EAC924B794E256FC0080790C1DBBC1A2B60373092D8ECA9ED7F93BF10FE`，协调线复跑当前同路径 XML SHA-256 `BCC30BB4FB6F5032154DA6F1D276F46EBAB4109D770926ED2641BF3E480DBB76`；XML 内 `CreateCalibTime` 按实际墙钟写入，复跑当前文件该字段为 `2026-10-20 21:09:08`，所以不能把两次字节哈希当作稳定产物或语义差异。方案文件当前 SHA-256 `39C165DE32CBEE211298EEE645516DAD6678042B1602E4F784E2198991B8100D`。默认十二行的 60 次为同一构造循环的源码计数，本轮只实际运行四行计数，避免把推导伪装为实测。

第一次探针编译因为测试文件漏 `System.IO` 且误调用 `GraphEditorDocument` 内部 `Node` 方法失败；修正测试后最终通过。该失败属于探针自身，不能算产品缺陷。此轮不修改产品或工具工作树，不提前声明已修复卡顿。
