# FCR-038：公共插件输入选择器的执行前驱

## 原始链与证据范围

原 `VM.Core.VmModule.GetFrontParamItems` 经 `PreModules` 递归收集前置模块，增删连接分别更新反向关系；原 `Apps.XmlParser.ParamTab.GetFrontFilterTreeOperation.Operate` 从 Owner/DataSource 进入 `GetFrontFilterTree`，由前置模块起步后再按类型、`CanSubscribeA`、`Visible`、私有性等构造叶树。精确源码路径及 SHA 见 [前一轮原始证据报告](B-tool-input-predecessor-scope-20261022.md)。这只确定**节点候选的前驱来源**；原版各控件 `IncludeSelf/Inside/GlobalVar/Null` 实参、父 Group、叶命名与数字 ID 仍须逐入口核查，不能用当前平面列表冒充完整原树。

## 当前 main 的数据流修复

`MainWindow.ConnectToolEditorServices` 生成插件用的 `ToolEditorContext.PickSubscription`。此前该回调给 `WorkflowSourcePicker` 的节点部分调用全流程 `WorkflowSources`，A→B 执行链之外的独立 C 也进入候选，实际提交却被 `GraphValidator.DataExecutionOrder` 拒绝。按新的公共框架所有权，main 在 `ToolExecutionScope.cs` 提取 `ToolSubscriptionOptions`：变量、流程输入、空项保持原顺序与表示；节点只从 `WorkflowFrontSources(procedure,node,null)` 取得，继承目标端口的 `DataKinds.CanFlow` 资格。没有显式执行链的原始内存模型不猜节点来源，但仍可保留非节点候选。`WorkflowSources` 继续供流程级配置与变量入口使用。

本地旧方案并非简单的“没有显式链”：`GraphEditorDocument.Load` 调 `ProcedureGraph.Separate`，后者在保存文件 `ExecutionLinks=null` 时用数据连接生成显式执行链。旧文件 A→B 经 Save/Load 后插件节点候选仍有 A；这是**当前独立格式迁移**，不是原版 `PreModules` 的文件格式结论。前一轮报告对此已更正。

## 可复验自审

`dotnet run --project tests/WorkflowCandidateScope.Checks/WorkflowCandidateScope.Checks.csproj -c Release --no-restore` 在真实离屏 `MainWindow` 通过：全流程候选仍有 A/C；工具输入与插件公共选择器只取 A；禁用 A 排除，断开执行连线排除，重连恢复；下游 D 同时取得直接 B 与间接 A，始终排除独立 C；typed 订阅服务同范围；C→B 被验证器和编辑器拒绝，原 A→B 数据线不变；旧文件的 `ExecutionLinks=null` 经真实保存/加载后从数据线恢复 A。测试直接调用产品的 `ToolSubscriptionOptions`，实际插件专属弹窗的布局、选择提交及保存重开还未覆盖，不能据此关闭完整 UI 链。

输出 `analysis/joint-audit/candidate-scope/candidate-scope.json` SHA-256 `B4E5B568BDD66585519512DB390FB11F04D4C0E962ED23AE8B33CCF58C1DCEED`；旧文件样本 `legacy-no-links.vmx.json` SHA-256 `B1EBE0A3211DC53176A82D632469017345A1413D08E734DCD09F0A84F1AA4F4A`。产品 `ToolExecutionScope.cs` SHA-256 `0E35A5B6A0C34690CABAFC0911A521C6E53F227A4F6A8B98352A991F7BD4A2E7`；验证时宿主 DLL SHA-256 `61370F6AE1FB04BB8E228F0A7A77AE7196C4B4D4EB29DC6406DE43901156E3B0`。第一次扩展测试误以为断开数据线也会移除执行线，运行失败；`GraphEditorDocument.Disconnect` 与 `DisconnectExecution` 是独立操作，改用真实执行断线后通过。第二次旧文件断言误要求 Load 后仍为 null，实际 `ProcedureGraph.Separate` 恢复连接；修正后通过。两次失败不覆盖为通过。

当前无工具算法改动，也未修改 `tool-plugins`。这个修复只闭合当前独立实现的公共选择器前驱范围，不闭合原生完整候选树、各插件真实窗口、Group 父模块/变量资格或原厂同状态差分。对应 96 单元没有整单元满足全部范围，本轮实施增量为 0。
