# 第十二轮：标定加载动态叶改名/改型与 Group 已选连接

范围限定标定加载的 `OutputSchema` 动态叶、owned Group 输出引用和坐标系矩阵消费者，验证 J04/J09 的具名变化路径。与第七轮“文件打不开保旧/缺输出节清集合”不同，本轮改变**成功解析文件内的输出名称与类型**。原件、`tool-plugins` 只读；没有产品修复。

## 原件调用链与不能跨越的未知

- 安装件 `ReadCalibFileModu.dll` SHA-256 `86B8B112ADED58DE27A41A5EDE6EC6B41117D7DB8D0C5934647FA9514E6FA403`。原生 `CMyAlgorithmManager::LoadFile @0x18000ab50` 的成功解析分支按 `CalibOutputParam/P@ParamName,DataType` 建立输出集合；`SetParam @0x18000d120` 对路径调用 LoadFile；`Process @0x18000c520` 在 `RefreshSignal` 非零时重读并按当前集合逐键发布。三只读 C 体 SHA-256 分别为 `EC0CFDED878E3B054A65F8F83550364D74503B59D792069D0566C42D91231626`、`35DC0C1699E08DFCE863173B529FBE8BA4E0579BE5D6CE1D3167AEE92D2F57F5`、`E3C6315832CE99019996B39A5BDC7B4CB10867E8C35C7C1F132092353750DC2A`。正常解析新文件后值的**发布名称/类型**会变；内核自身并不定义 WPF Group 已选订阅的迁移语义。
- 原托管 `VM.Core.DynamicVmIO.Name` setter 对同一个现存动态叶改名时先收集 `RelateIO/RelateIndex` 与下游引用、改名称后恢复关系，调用 `Root.InvokeOnModuleIONameChanged` 和方案 `OnModuleActionChanged("ReName")`；`TypeName` setter 对叶子先 `ClearRelation` 再更新动态 IO，`UpdateDynamciIOInfo` 把输出集合送 `SetModuOutputIOInfo`。源 `analysis/decompiled/VM.Core/VM.Core/DynamicVmIO.cs` SHA-256 `23C0A2B880706338298B826D810FE9CD118908E46A60CC1387B7ECA5032CC9A8`。`IMVSGroup.InvokeOnModuleIONameChanged` 去 `%` 后发 `OnModuleIONameChanged`，`SetIoParams/GetGroupIoList` 将自己的动态端口映射给 SDK；源 `analysis/decompiled/VM.Core/VM.Core/IMVSGroup.cs` SHA-256 `DC772AABA433316B7D0F4DC3B0B3C80C3C63769334D7D013896AFE7B54C367D3`。
- **关键不等价风险**：上述 `DynamicVmIO.Name/TypeName` 是对同一 `VmIO` 对象的 UI 属性编辑路径。安装版文件内容刷新是否重用该对象、销毁重建还是只变 SDK 内部输出集合，本轮所读 `LoadFile/SetParam/Process` 与 `VmModule.InitDynamicIO` 尚无连接该具体事件的完整调用边。不能把 UI 改名时的关系恢复直接宣称为“刷新 XML 会自动迁移旧 Group 订阅”；需要原 `GetModuOutputIOInfo` 更新通知及该 ReadCalibFile 对象的托管 reload caller，或原平台同构受控运行，才能作原厂 Group 结论。
- `VmModule.InitOutputDynamicIO/InitDynamicIO/AnalysisIOInfo` 从 SDK `GetModuOutputIOInfo` 构造端口；`AnalysisIOInfo` 仅 `%...%` 名选择 `DynamicVmIO`，一般名创建 `VmIO`。`VmModule.IOData_Undo/Redo` 则显式 `RemoveDynamicOut→SetModuOutputIOInfo→InitDynamicIO→Done_IOData` 并恢复订阅/更新配置显示。这是**历史回放**调用链，并非已证明的本工具文件刷新调用链。源 `analysis/decompiled/VM.Core/VM.Core/VmModule.cs` SHA-256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`。原主 XML 固定声明 `CalibMatrix:float[]` 等端口；任意文件 `ParamName` 在原 UI 中的包装/转义规则需另取实际 SDK IO info。

## 当前源码实际交接

`ReadCalibFilePlugin.Runtime` 在 `RefreshSignal=1` 时重新解析同一路径，按当前集合生成 `ModuleResult.UpdatedDescriptor`；`GraphExecutor` 在节点本轮运行后替换 session 内 descriptor 并发 `ExecutionEvent.UpdatedDescriptor`。`GraphEditorDocument` 的 `OutputSchema` 仍是持久化时可连接的端口声明，不会因一次运行自动改掉已有 `WorkflowOutputDocument.SourcePort` 或 `ConnectionDocument.FromPort`。owned Group 的 `WorkflowRunContext.Complete` 根据本轮输出名/类型取 `OldMatrix`，缺名或类型不同均产生 output error；父流程 Group 失败，坐标系按前驱不可用跳过。保存时 `ReadCalibFilePlugin.PrepareSave` 可缓存 XML 文本，但旧 Group 输出引用不会据文件内容静默改名。实现链见 `src/VisionPlugins.Calibration/ReadCalibFilePlugin.cs`、`src/VisionFramework/GraphExecutor.cs`、`WorkflowRunContext.cs`、`src/VisionEditor.Core/GraphEditorDocument.cs`。

## 可复验受控图

探针 `tests/CalibrationReadCalibSchemaChain.Checks/` 只在本轮独占目录写 `dynamic.xml`，实际图为标定加载 `OldMatrix:FloatList`→owned Group `Matrix:NumberArray`→坐标系 `CalibMatrix:NumberArray`，另有实际 ImageSource→坐标系 `InputImage`。独占构建槽 `dotnet run --project tests/CalibrationReadCalibSchemaChain.Checks/CalibrationReadCalibSchemaChain.Checks.csproj -c Release` 首轮 exit 0，日志 `analysis/joint-audit/pass12-cal-c-schema/first-run.log`：

| 状态 | loader 本轮 descriptor | Group | Coordinate | 文档/缓存含义 |
|---|---|---|---|---|
| 原 `OldMatrix:FloatList` 九项 | `OldMatrix:NumberArray` | Success | Success | 实际九项进入 Group/下游。 |
| 文件改名 `NewMatrix:FloatList`，同 scope 刷新 | `NewMatrix:NumberArray`，无旧输出 | Failed `SubprogramFailed`，子流程 `Matrix: Current-run source unavailable: …OldMatrix` | Skipped `ExecutionPredecessorUnavailable` | 图文档旧 Group 输出源及父连接仍在，不自动按新标签重绑。 |
| 改名状态保存/重开，新 scope 复跑 | `NewMatrix:NumberArray` | 同上 | 同上 | 保存重开仍保持旧选择，非静默迁移。 |
| 同名改型 `OldMatrix:int`，原 scope 刷新 | `OldMatrix:Integer` | Failed `SubprogramFailed`，子流程 `Matrix: type mismatch` | Skipped | 旧值不会透过 NumberArray Group 出口。 |
| 原九项 FloatList 恢复 | `OldMatrix:NumberArray` | Success | Success | 原 scope 与重开 scope 分别恢复，不需改画布连接。 |

此处的 Group 失效是 `WorkflowRunContext.Complete` 的出口源缺席/类型错误，再由 `WorkflowCalls` 映射为 `SubprogramFailed`；不同于已有**直接**动态数据线的 `DynamicConnectionInvalid`。两种错误对象不可混称。测试源 `Program.cs` SHA-256 `F0C94F9ACAB6CE362BC456A56A3F8D6E9CD1693DE6332901978A77398A6B5345`，工程 SHA-256 `78FE6A036804F54576739F502849B9C78D2B19B734166B8DD1D06CE59F842BB0`，首轮日志 SHA-256 `7C1B9A2D4927452DD5FD9EF288D53FA6F2321E711A1BAB4678BA03A115337191`。保存于改名状态的方案 SHA-256 `0CF5915FC59EAFEDDDBDBAAB75EB31243BC5CD8C999854E39FB95134A893DA1E`；最后恢复后的 `dynamic.xml` SHA-256 `AFDE6DF9B4D5F37998C19C9992A81B5841DF2D84AD1B5D1780BBAC4CBEFC8CCD`。后两者是本次实例身份，重复运行时方案的动态缓存/时间字段或其他序列化状态可能改变，不能当原件 oracle。

既有 `tests/ReadCalibFile.PluginChecks/Program.cs` 的 `DynamicExecution` 使用 test-only `NumberSink` 验证了直接旧连接在重命名/改型时 `DynamicConnectionInvalid`，本轮不是重跑该直接边，而是验证 owned Group 的 `WorkflowOutputDocument` 与父流程选中边、重开和恢复。原平台对应状态未实跑，结果应标为“当前实现”而非原厂对照。

## 本轮仍需的精确证据

1. 找原 SDK 输出 IO 信息在文件刷新后的通知入口：从 ReadCalibFile `SetParam(FilePath)`/`Process RefreshSignal` 到 `GetModuOutputIOInfo` 与 `VM.Core.VmModule` 重建/对象复用的精确 caller。若不存在动态通知，需原版同构方案受控运行判定旧 Group 订阅是否自动迁移。
2. 若 Group 原 `OnModuleIONameChanged` 收到事件，追实际处理者和 `VmIO.RelateIO`/`SetGroupIO` 是否同步修改已选端口；同名改型另核 `TypeName.ClearRelation` 与后继通知。不能从事件声明推出最终状态。
3. 当前 JSON `OutputSchema`、缓存 XML 和 Group 出口三者可在文件变更后短时不一致；本轮探针只以运行时 descriptor 和后继实际结果作判断，不把静态校验通过误作实际连接有效。
