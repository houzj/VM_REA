# 工具输入候选的执行前驱范围（2026-10-22）

## 原版证据与边界

- `analysis/decompiled/VM.Core/VM.Core/VmModule.cs`（SHA-256 `53E2F0F9032C4631B59CF0EB2A00E06D50469D94706E7E370E9CC7E1059C72CE`）`GetFrontParamItems` → `GetFrontOutputItems` → `DeGetFrontOutputItems`：从 `PreModules` 递归收集，已访问模块不重复加入；有非 `VmSolution` 父模块时另加父模块。`NextModules_CollectionChanged` 的添加分支调用 `ConnectOneModule` 并更新目标的 `PreModules`，删除分支反向移除；装载连接信息时也填充 `PreModules`。这是执行连接图，不是全流程节点枚举。
- `analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/GetFrontFilterTreeOperation.cs`（SHA-256 `859B7C4EC74F7843623BC4923E797C898052E8E2A315B061FB4251C74A0446E7`）`Operate` 从 `AlgorithmTabRoot.Owner` 或 `DynamicIOContainer.DataSource` 调用 `GetFrontFilterTree`，写入 `TargetName + "A"`。
- `analysis/decompiled/Apps.XmlParser/Apps.XmlParser.ParamTab.VmModuleExtern.decompiled.cs`（SHA-256 `4D4B726DF4EAC492C955502CB90118E793679EE74718B900E62625CFE2DE2C9C`）`GetFrontFilterTree(VmModule, ...)` 从 `GetFrontParamItems()` 起步，再按类型筛输出叶；可选自模块、内部模块、变量和空项由调用参数控制。`GetOutputTreeNode` 另检 `CanSubscribeA`、`Visible`、时间戳名和私有性，并以 `ID + " " + Name` 命名模块根。当前端口合同不足以恢复全部原生叶资格/树结构，也不能由 GUID 推定原数字 ID。

## 当前实现和本轮最小修正

`MainWindow.WorkflowSources` 列举整个流程的输出，供流程级配置、Group 等入口使用；它不能直接当作某个工具输入的前驱候选。原 `ToolEditorHost.EditToolInput` 恰好这样使用，故 A→B 连线、C 独立时会向 B 展示 C，选择 C 又被 `GraphValidator` 的 `DataExecutionOrder` 拒绝。

本轮新增 `WorkflowFrontSources(procedureId, targetNodeId, kind)`：仅在有显式 `ExecutionLinks` 时沿目标节点的反向连线遍历，去重并排除自身及已禁用节点，然后复用当前类型输出列表。`EditToolInput` 改用此范围；流程级 `WorkflowSources` 保持全流程。旧文档无显式执行连线时拒绝构造候选，避免把存储顺序或数据连接冒充原版 `PreModules`。这是主框架独立执行图表示上的实现，不宣称原版 Group/父模块、变量、叶过滤或文案已等价。

`tests/WorkflowCandidateScope.Checks` 在真实 `MainWindow`/`GraphEditorDocument` 构造 A 图像源→B 二值化、C 独立图像源，证明全流程候选仍含 A/C，工具输入候选仅含 A；现有 typed 订阅服务也只含 A，非法 C→B 持久化仍被拒绝。`dotnet run --project tests/WorkflowCandidateScope.Checks/WorkflowCandidateScope.Checks.csproj -c Release --no-restore` 通过（仅既有 `CS8603`），记录在 `analysis/joint-audit/candidate-scope/candidate-scope.json`（SHA-256 `6A431E59ED988162889ECFAC8D2A27F49430346F86E6533A55F78116748A8B70`）。首次普通沙箱构建无法写 `obj`；独立 `obj` 参数触发 SDK 自动生成程序集特性重复；随后在授权构建环境用标准输出目录通过。这两个失败保留，不作为产品回归。

## 尚未收口

`ToolExecutionScope.PickSubscription` 仍使用全流程 `WorkflowSources`；它属于插件面对的公共配置入口，本轮未修改该文件或 `tool-plugins`。该入口的前驱候选范围、父 Group/变量资格、原生输出叶 `Visible/IsPrivate/CanSubscribeA`、数字 ID 和已选值刷新，须由插件面对框架所有者按原 `GetFrontFilterTree` 调用参数逐项核证。当前测试证明了主框架独立工具输入入口的候选筛选和提交拒绝，**不能**证明所有插件配置输入框或原版 UI 等价。本轮不增加 96 项实施完成计数。
