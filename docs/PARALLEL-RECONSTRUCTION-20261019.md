# 2026-10-19 四方向源码复核

基线 `2153160686746ae463b54c65fb03774c9ded6f06`。四个独立子任务只读原始证据，由 main 汇总及修改；受并发槽位限制，前三项结束释放槽位后启动第四项。未新建工作树，未修改 tool-plugins。以下路径以仓库根目录为基准。证据类型为反编译文本、CIL 和随包资源静态读取；没有新增原机操作验收。

## 1. 工具发现、加载与优先级

原始入口 `analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs`：`GetItems`（196–229）首次加载后调用 `ChangePriorityByInfo` 并缓存集合；`LoadData`（266–325）读取 ModulesDir/ToolGroup.xml，普通分组先构造 defaultProxyModule；Global 加载全部，UserTools/GroupTools 初始在首个有效元数据处 break。`UpdateLoad`（77–120）在扫描前设置 IsInit，替换占位并通知，异常后可重试性未证实。`Reload` 清除三个缓存。

`ChangePriorityByInfo`（231–264）遍历全部分号策略，对匹配类别反复赋值，故最后一项覆盖前项。main 的 `ToolDirectoryCatalog.StrategyPriority` 原先首次匹配即返回，本轮修正为最后匹配；不匹配保留基础优先级，非法或非有限数值继续使用本地容错策略。原生 float.Parse 的错误行为未复刻。相同优先级原生比较器返回0，本地目录次排序仍是独立策略；UserTools/GroupTools 的本地全量扫描也仍有差异。

`ToolBox.xaml.cs::OnMouseEnter` 触发延迟加载；`DecoratorBase.DropItemsCommandExecute`（174起）对 GroupTools 使用 `VmSolution.LoadGroup`，普通工具进入 `ToolBoxItemData.AddNewItem`（279–314），按自定义类型或 Cs/Control 程序集实例化。`GetLibs` 使用 CR 分割 Librarys.Config，不等同通用 XML/LF 配置。`VmModuleSet.LoadModules/CreatModuleByInfo/LoadModuleDLL/CreateModuleToolRe` 另有原生模块树和路径缓存链；Cs 缺失可回退基类 VmModule。不能将工具箱入口、加载存量模块入口和插件版本策略合并成一种合同。

main 当前导航仍由静态 tool-catalog.json 和显式注册驱动，ToolDirectoryCatalog 仅被专项检查消费，本轮不声明主宿主已动态发现插件，不跨项目接入插件框架。

验证：`dotnet run --project tests/ToolDirectoryCatalog.Checks/ToolDirectoryCatalog.Checks.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false -- 'D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64'` 通过11项检查；安装元数据16组、Collection 5工具、初始诊断0。构建存在原有 MainWindow.xaml.cs:123 CS8603 警告。

## 2. VM.UnRedo 服务与 Group 配置

已找到 `D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VM.UnRedo.dll`，35840字节，SHA256 `AA719D093C948E8E6573D20C0198B60EF493F2BED8C029CAAB95ED5621A7AAA2`；Development/V4.x/ComControls/Assembly 下同名文件一致。不是 MVDSDK.VM.UnRedo.dll。原 CIL 位于 `analysis/full-audit/mf04-unredo/VM.UnRedo.il`，本轮保留副本，SHA256 `981835AD9230585A0DF83B7FE38C7013BEE514105CC11C47B99482DCB965A3B2`。

- `UnRedoService.SetForbidRecord`（IL4212–4224）委托到 Schedule（1849–1875）：undo栈空则返回，否则设置栈顶 IRootParam.ForbidBuild。不是全局、嵌套计数或窗口作用域开关。
- `Register`（2008–2112）对 undo/redo中、lazy层、无根的part等分别提前返回；ForbidBuild 检查只限制 part 注册，不能推导所有 root 注册被禁止。`BuildRoot`（2765–2806）建根动作，`BuildPart`（2807–2864）挂到根 Actions 并清理 redo。
- `IsNeedExport`（2565–2625）也检查栈顶 ForbidBuild，影响恢复临时数据导出。Schedule 构造函数（2542–2564）会删除应用 TempFiles，故本轮没有实例化厂商服务。
- `Undo`（2113–2215）和 `Redo`（2216–2315）转移栈、更新动作数据并调用恢复；异常被捕获，finally 恢复进行中计数。不能据此保证 Group 调用方 ForbidBuild 在异常后复原。
- `GroupRunParam.ConfigCommandExecute`（反编译 GroupRunParam.cs:251–304）先整理数据，再 true→Invoke→false，无 finally；`VmModuleSet.CreateLinkModule` 同形。连续 true/true/false 只修改当时栈顶根，不是嵌套作用域。
- `SetCopyStatus` 为实例字段，null读取、非null解析赋值；VmModuleSet 设置true，ProcedureConfigControl 替换集合后消费并设置false。CopyModule 根动作有自己的动作次序，不能等同普通快照撤销。

main 的 Group 保存仍逐次调用 GraphEditorDocument.Edit，形成完整方案快照。没有原生 root/part、临时恢复数据等价结构，本轮未以一个布尔量或跳过 undo.Push 冒充复刻。FCR-022 的 scope/finally/窗口隔离只能作为独立安全设计请求；本段替代历史“DLL尚未取得”的事实状态。原机异常/并发窗口/实际 Group part 注册仍待验证。

## 3. 真实 .sol 容器和持久化链

新增测试辅助脚本 `tests/Inspect-NativeSolutionSample.ps1`：只读 ZIP、逐文件/条目哈希、禁用 DTD/外部实体读取 VmServer.xml、保留 UI 字段原始十六进制。限制条目64MiB、UI/XML16MiB；不提取文件、不执行XAML、不调用SDK、不转换成本地 vmx.json。UI 候选结构是样本观察：66553322、BE uint32 未解释值、BE数量，然后长度/键/长度/值；它不是完整原生格式合同。未知结构记录 Unsupported，剩余字节显式记录。

根任务递归枚举发现 **8** 个随包 C# .sol（子任务初查7个，另含 GetResultControl/SampleSolution 样本）。全部完成只读检查，原件前后SHA256一致；98个 UiParamData 条目均按候选结构消费至末尾、无 Unsupported。完整路径、样本/条目哈希、XML和字段在本地 JSON，不能据此推断所有版本/密码/solx 均为此结构。

CircleFind 有8条目，包括 UiParamData、VmServer.xml、GlobalScript_0、拼写为 MoudleFrame 的二进制项和图像附件。XML声明 MainInterface@FileName，但ZIP无同名条目，不能设为必需文件。订阅 Relation 原始8段语义未闭合。UI GUID 与 XML GUID 不同，不能合并身份。DESIGNERBUFFER 为含末尾NUL的XAML字节，本轮未执行。MoudleFrame/mdata边界、模型附件和脚本布局仍未知。

保存链：`ProjectManager.Save`（166–211）保存校准后进入 `VmSolution.InternalExport`（1551–1597），经 `SolutionManager`（305–323）→`ServerSDKAPI`（286–288）→NativeMethods 的 iMVS-6000PlatformSDK.dll；完成事件先派发后更新等待状态（1075–1080）。导入链 `VmSolution`（2924–2974）注册回调并在finally解除，SolutionManager（272–290、1095–1106）调用SDK/处理完成。托管调用边界已确认，native内部序列化、部分失败恢复、实际打开/执行/往返未验证。UserData→ModuleManager 的 UI 参数 get/set（765–849）为空值删除且涉及SDK动态内存；不得把样本解析器当产品导入器。

## 4. 局部变量目标与运行参数 Tab

`analysis/decompiled/VMControls.WPF/VMControls.WPF.GlobalUI/GlobalVariableItem.cs:728–754` 明确创建 `RunParamsSelectData`，不是 ModuleInputSelectData；确认成功才更新 SelectedRunParams。`Apps.XmlUI/Apps.XmlUI.ParamTab/RunParamsSelectWindow` 的 SearchComboBox 是类型下拉，没有文本搜索框；未传 searchModuleFilter。

`Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/VmModuleExtern.GetParamRoot` 懒建 AlgorithmTabRoot。其构造读取 XmlPath，loadXml 后调用空的 DynamicSetProperty；Tab.InitCollection 将 Category.Items 扁平化。LinkItem 可替换既有条目，LinkRealItem 设置目标属性，均不是自动新增运行Tab。Hik.Xml装配和代理副作用未完全覆盖。

`AlgorithmTabRoot.GetRunParamsTree`（975–1040）只取 `Tab_Run Params.Collection`，展开 IDynamicChild 和 IntegerBettween/FloatBettween，检查 DisplayNameA 非空；该链没有 CustomVisibleA 或 SubscribeVisibleA 过滤。`RunParamsBaseNode`（131–177）分别计算 ISubInfo+SubscribeVisibleA 以及 ComType。

`RunParamsSelectData`（223–285）首次全部类型创建模块缓存：排除其他变量占用，按 ModuleID+Name 恢复仍存在的旧选；新候选保留 !IsSupportSub 或 ComType匹配项。分型按原 DisplayType 小写集合匹配，确认从所有缓存汇集已选并复查重复和订阅类型。变量绑定上限 int.MaxValue，其他用途32。原始树消失项不会独立补回；类型切换保留缓存选择不等于所有消失绑定都被保留。

安装 ImageSourceModuleAlgorithmTab.xml 仅 Tab_Basic Params。若原样装配且无外部修改，GetRunParamsTree为空；这仍是条件结论。原机 Params.XmlPath/Data/Tabs、动态扩展及真实拼接两态目标树未采集，不能由 CustomVisible=False 推断隐藏，也不能断言拼接开关一定改变树。

main 按 DataKind 从 descriptor.Parameters 和未连接 Inputs 生成候选，是独立兼容实现，缺少原 Tab/DisplayType/ISubInfo/ComType 信息。没有根据未知元数据贸然删除候选。FCR-019 应分别表达来源、稳定身份、运行Tab、动态/Between展开、DisplayType、ISubInfo、SubscribeVisible和ComType；窗口缓存与插件会话刷新分开，原机活窗口动态刷新另验。

## 证据保全及后续验收

本地目录 `analysis/full-audit/parallel-reconstruction-20261019/` 保留基线、初始状态、目标Tab源码哈希、IL副本、全部样本及字段审计JSON、元数据检查日志和最终清单。raw证据继续不入Git；本报告、脚本和最小修复入Git。脚本运行需要 PowerShell 7/.NET 支持 Convert.ToHexString 和 SHA256.HashData。

后续分别验证：工具箱首次/悬停/刷新/Group导入与原机路径；空栈/旧根/新根/异常下 ForbidBuild 及 part数据恢复；真实 .sol 原机打开、保存重开和执行；实际运行Tab、各类型切换、他变量占用、订阅不匹配、原树删除与取消窗口。通过本地检查不表示原生等价。
