# J04 候选范围小探针：独立 C 出现在通用来源表，却不能订阅到 B

范围仅为一个 A→B、C 独立的输入候选/执行前驱差异；没有修改产品或 `tool-plugins`。运行命令：

```powershell
dotnet run --project tests/WorkflowCandidateScope.Checks/WorkflowCandidateScope.Checks.csproj -c Release
```

探针使用实际 `GraphEditorDocument` 建立 A=ImageSource、B=ImageBinary、C=ImageSource，`Connect(A.Image,B.Image)` 生成 A→B 执行边，C 无执行边。反射调用现有 `MainWindow.WorkflowSources("main", GrayImage)`，再调用真实 `WorkflowConfigurationSubscriptionService.Enumerate(Combination/Image/IMAGE)`；最后提出 `C.Image` 对 `B.Image` 的订阅，既直接检查 `GraphValidator`，又调用实际 `GraphEditorDocument.SetProcedureInputSubscription`。结果四项全部通过：

| 检查 | 观察 |
|---|---|
| `WorkflowSources` | 返回 A 与独立 C 的可流入图像输出；这个通用方法按当前流程全部节点枚举，没有执行祖先过滤，也包含 B 自己的输出。`EditToolInput` 随后排除自己，但仍不排除 C。 |
| 类型化订阅桥 `Enumerate` | 只列执行祖先 A，排除 C。 |
| `GraphValidator` | 将 B 输入改提为 C 输出时报 `DataExecutionOrder / Subscribed source must precede consumer`。 |
| `GraphEditorDocument` | 同一改动抛出该错误，A→B 旧连线仍保留，未写入错误订阅。 |

当前源码调用点：`src/VisionEditor/WorkflowConfigurationHost.cs:15` 的 `WorkflowSources`；`src/VisionEditor/ToolEditorHost.cs:46` 的普通工具输入选择；`src/VisionPluginContracts/WorkflowConfigurationSubscriptionService.cs:33,107` 的候选及执行祖先；`src/VisionFramework/GraphValidator.cs:30-31` 的执行先后判定；`src/VisionEditor.Core/GraphEditorDocument.cs:159` 的编辑前拦截。因此 **同一 B 输入存在两条当前候选路径，范围并不一致**；通用工具输入选择器可能展示最终不允许写入的 C，当前确认会拒绝而非静默生成坏图。探针未打开实际下拉窗口，也未证明所有插件使用哪一种编辑器入口。

原版证据边界：安装 `VM.Core.dll` 的 `VmModule.GetFrontIOWithLeafItemsByName` 本体 CIL 为 `analysis/joint-audit/VM.Core.GetFrontIOWithLeafItemsByName.il`（SHA-256 `A442D5B02176056DC4BC2BA972713B87CF12916BF54F58A1F9ECF8916C79F71C`），对照 `analysis/decompiled/VM.Core/VM.Core/VmModule.cs:3961-3994,4049`（SHA-256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`）。`GetFrontParamItems` 从 `PreModules` 递归取执行前驱，并可加父模块；随后 `GetFrontIOWithLeafItemsByName` 加变量模块、按类型及 `CanSubscribeA && Visible` 过滤。这支持“仅拓扑前驱的普通模块候选”规则，但 **未复验原版在此具体 A/B/C 图中的 `PreModules` 维护、编辑器列表或显示文案**；本探针不代替原机差分。

审计文件：`tests/WorkflowCandidateScope.Checks/Program.cs` SHA-256 `FD1C83D506694A6F848E9EABA9F3A22BFF9B3563328CA802ED41B3E1AC3DDB44`；`analysis/joint-audit/candidate-scope/checks.log` SHA-256 `1BA8DBF1AD50740919F206463619BD2059ADCD212E74CD41B56B7230E771FB88`；`analysis/joint-audit/candidate-scope/candidate-scope.json` SHA-256 `9BCB6DDFF6743D05B7D26963A435D736BEC6662F431808BBA48736242AAB1827`。JSON 包含本次随机节点 ID、两组实际候选及验证器错误；重新运行会生成新 ID 与新哈希。仅代码级可复验，人工宿主验收另计。
