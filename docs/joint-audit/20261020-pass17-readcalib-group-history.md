# 第十七轮：标定加载动态叶变更后的 Group 订阅与历史

范围只含标定加载 ReadCalibFile 的动态输出、owned Group 内坐标系 `CalibMatrix` 订阅、编辑历史与方案保存重开；不重复第十二轮 Group 出口旧映射，也不修改产品或 `tool-plugins`。本轮结果是**当前实现的真实图行为**，原版文件刷新后是否迁移既有订阅仍未证。

## 原件路径及证据边界

- 原生 ReadCalibFile `LoadFile @0x18000ab50` 从 XML 的 `CalibOutputParam/P@ParamName,DataType` 建输出集合，`SetParam @0x18000d120` 的路径分支加载文件，`Process @0x18000c520` 在 `RefreshSignal` 非零时重读并按集合发布；原件/反编译身份见 [第十二轮](20261020-pass12-readcalib-schema.md)。该链只说明运行时发布的端口名会变化，**不**说明 Group 或订阅 UI 自动改线。
- `analysis/decompiled/VM.Core/VM.Core/DynamicVmIO.cs` 的 `Name` setter 先保存 `RelateIO/RelateIndex`、清关系、对**同一对象**改名，并在 `OnNameChanged` 恢复关系及触发 `Root.InvokeOnModuleIONameChanged`；`TypeName` setter 清叶关系。`IMVSGroup.InvokeOnModuleIONameChanged` 去 `%` 后发事件，`SetIoParams → GetGroupIoList → SetGroupIO` 是 Group IO 映射路径。`VmModule.IOData_Undo/Redo` 显式 `RemoveDynamicOut → SetModuOutputIOInfo → InitDynamicIO → Done_IOData`；`Done_IOData` 在适用时 `SetIoParams`、`RecoverSubscribe`、刷新配置树。这里是原版 **UI 改名/动态 IO 历史回放**，没有证据证明 ReadCalibFile 的文件刷新重用原 `DynamicVmIO` 对象，也没有找到本次源集合内 Group `OnModuleIONameChanged` 的实际订阅者，故不得外推为原版自动迁移文件叶。
- 原托管源 SHA-256：`DynamicVmIO.cs` `23C0A2B880706338298B826D810FE9CD118908E46A60CC1387B7ECA5032CC9A8`，`VmModule.cs` `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`，`IMVSGroup.cs` `DC772AABA433316B7D0F4DC3B0B3C80C3C63769334D7D013896AFE7B54C367D3`。

## 当前代码逐边核查

- `ReadCalibFilePlugin.Runtime` 每次 `RefreshSignal=1` 解析文件并返回 `UpdatedDescriptor`；`GraphExecutor` 仅在本轮运行会话的 `descriptors[id]` 写该 descriptor。静态 `ModuleRegistry.GetDescriptor(node)` 从节点持久化 `OutputSchema` 读候选，因此运行时新叶不会自动修改节点参数或 `WorkflowSubscription.Source.Port`。源 SHA：`ReadCalibFilePlugin.cs` `B6DF11D12ACE8CA5F3537764F97AEDDC52D03873E5B0D387F01610FD4A12DC02`，`GraphExecutor.cs` `6FED55FE29E547248CAB05A36072E690B76EB9668609F0318B795C24D7628F9C`。
- `WorkflowSubscriptions.Validate/SourceType` 以静态 descriptor 校验订阅；`GraphValidator` 把订阅 producer→consumer 加入执行先后约束。对坐标系这种启用 `SupportsDeferredNumericRead` 的输入，`GraphExecutor` 解析旧叶抛 `InvalidDataException` 时，若 producer **本轮成功**，走 `CanDeferNumericRead` 并把读取留给 `WorkflowNumericInputRead`/工具执行。此路径没有像直接 `ConnectionDocument` 一样按本轮更新后的动态 descriptor 预先阻止缺叶订阅；这解释了下述 warm/cold 分叉。源 SHA：`WorkflowSubscriptions.cs` `3EEF9D13467BC687FCFBAA1029F5337AC46EC2C59D6657864306B179BA428C01`，`WorkflowRunContext.cs` `D00F29701F640028040E0F2C3D1AC11246FFB500945246C3424E435741241247`。
- `GraphEditorDocument.SetParameters` 改静态 `OutputSchema`，`SetWorkflowConfiguration` 校验并写订阅；`ExecuteConfigurationEdit` 将连续配置写入一个文档历史根，`Undo/Redo` 回放快照。`Save/Load` 保存和恢复最终文档。探针用这些公开 API 代表**受控程序化编辑**，未自动操作原版或当前 WPF 订阅选择器。源 SHA：`GraphEditorDocument.cs` `4A44A59C96449A286EAC7162E42A0B42DD012669F49E5AA810E1CE131799B558`，`GraphEditorDocument.History.cs` `D4A3F13130E6C9334547ECAF567C5CBF34E122E976166990FA02BB796522411F`。

## 当前真实 owned Group 探针

ignored 目录 `analysis/joint-audit/pass17-readcalib-group-history/` 中 `probe.csproj`/`Program.cs` 建真实 Group body：ReadCalibFile `OldMatrix:FloatList` 九项→`Coordinate.CalibMatrix` **WorkflowSubscription**，ImageSource→Coordinate `InputImage`，Coordinate `CorCalibMatrix`→Group `Matrix` 出口；Read→Coordinate 有显式执行边。图像由本探针提交到 body scope；文件在本探针独占目录。首两轮失败暴露必须给订阅 producer 显式执行先后；第三、四轮错误断言把 deferred numeric 的 warm 行为误写成立即订阅失败，均保留。最终 `dotnet run --project analysis/joint-audit/pass17-readcalib-group-history/probe.csproj -c Release` exit 0、13 项断言通过。

| 状态 | 持久化选择 | 运行时发布 | Coordinate / Group 当前结果 |
|---|---|---|---|
| 原文件 | `OldMatrix` | `OldMatrix` | 两者 Success；Group `Matrix=[1,0,0,0,1,0,0,0,1]`。 |
| 文件改名，同 warm scope | `OldMatrix` | `NewMatrix` | 两者 Success；坐标系 `ModuStatus=0`，Group 仍输出同九项。**此时输出不能证明新文件的 NewMatrix 已到消费者。** |
| 同文件改名，cold scope | `OldMatrix` | `NewMatrix` | Coordinate `Failed/NativeUninitializedMatrix`，Group `Failed/SubprogramFailed`，无 `Matrix`。 |
| 显式更新静态 `OutputSchema` 和订阅 | `NewMatrix` | `NewMatrix` | 两者 Success；坐标系 `ModuStatus=1`。 |
| 一次 Undo | `OldMatrix` | `NewMatrix` | schema 和订阅一起回旧；warm 再次 Success、`ModuStatus=0`，输出同九项；不代表重新绑定成功。 |
| Redo；保存重开（fresh scope） | `NewMatrix` | `NewMatrix` | 两者 Success，`ModuStatus=1`，Group 九项出口有效。 |

因此最短可关闭的**当前文档历史/持久化边**是：显式 schema+订阅组合编辑 → 单次 Undo/Redo 同步还原 → 保存重开保留新选择并在 fresh scope 成功消费新叶。相反，“文件重命名自动迁移旧订阅”没有发生；warm 旧矩阵可通过 Group，不能只看 `NodeRunStatus.Success` 判断图像/标定新数据链已接通。

## 最小修复建议与剩余证据

若主框架要求动态源名变化时禁止旧订阅作为当前值进入工具，建议在 `GraphExecutor` 的订阅解析前，针对 `WorkflowSourceKind.NodeOutput` 且本轮 producer `SupportsDynamicOutputs` 的来源，用 **本轮** `descriptors[producer]` 校验端口存在及 kind/cardinality 与目标相容，缺失/改型时给消费者明确的动态订阅失败，阻断 deferred fallback；这与同函数现有直接连接的 `DynamicConnectionInvalid` 判定位置对称。它是**当前实现的安全一致性建议**，并非经原件证实的原版行为；主框架写入者应先确认产品希望的 warm 保旧政策，尤其 Coordinate 原生缓存可保旧矩阵。不能仅因当前 warm 成功就擅自清缓存或宣称用户 UI 缺陷。

原版 ReadCalibFile 文件刷新→托管 `VmModule` IO 更新通知、Group 事件实际处理者与旧订阅迁移结果仍缺精确 caller/受控原版运行。当前 WPF 选择器是否把 `OutputSchema` 与 `WorkflowSubscription` 当同一配置历史根也未作交互桌面操作；本轮是同 API 受控构造。`ModuStatus=0/1` 只记实测值，不从本轮反推原生状态语义。冷失败是当前安全拒绝未知初始矩阵，不声称原版也如此。

探针身份 SHA-256：`Program.cs` `CE6D5041FBC0E9944D58DD5F1BE9D16295CB990032EDB1B75C2B86D57A2726C9`，`probe.csproj` `B7B7F8434BA6080B3ED7199DDEEBE42886BBBB5039E762A1F3139F6CDEAA16CC`，最终 `final-run.log` `AD00EBB72C4C6297383DBF91869AAE7E832B71C84FF152A7F2AB2648FABFA71C`。失败轨迹 `first-run.log` `3987BE23C11E78A47AB85DCCEB82CA9AD85A66A69CDA464B873838F37857AD0F`、`second-run.log` `521E553032DBDD6284443880BAC6608EC6EE81D19F1186B5BC75582DE9A5728B`、`third-run.log` `7D43CFC6A7F74CEB13DC3D7A7A167E0109C9D16AADE04DFA6D4BE94BBA54D07E`、`fourth-run.log` `E7D4DAED734B5C9B33386BBA826C397255B28CBEEA3D0E77154A5A14FB26D255`。生成 `dynamic.xml` 与 `group-history.vmx.json` 含实例路径/ID，不承诺跨运行字节哈希稳定。
