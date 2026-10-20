# 第二十五轮 B 线：B02.3 / B05.2 实施门槛复核

基线：`ea64cb0`（第二十四轮 C02.3 接入后）。本轮只审查两个未闭合实施单元：B02.3 原 `.gro` 导入的 ID/GUID/复制持久化，以及 B05.2 全局变量模块边界。未访问或修改 `tool-plugins`。

## B02.3 结论：仍阻塞，实施 +0

已有源码证据仍限定在原托管入口：

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmModuleBaseDecorator.cs` 的 Copy/Paste 只保存 `FullName`，粘贴时重新解析当前对象。
- `analysis/decompiled/VM.Core/VM.Core/VmModuleSet.cs` 的 `ProcedurePaste` 通过 `Export`、`LoadGroup(..., isFromCopy:true)`、两阶段 `ModulePaste`/`ModulePasteFinish` 建立对象映射。
- `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs` 的 `internalImportGroup` 与 `MModuleManager_OnImportGroupEndCallBack` 依赖 native 回调中的新 ID、进程 ID、名称，再经 `AddModuleByInfo` 建模；回调顺序和失败 flag 行为仅有静态证据。
- `analysis/decompiled/VM.Core/VM.Core/VmModule.cs` 的 `Init` 在 UserData 有 `GUID` 时读取，否则生成并写回新 GUID；该分支不能证明复制时 native 必然保留或替换 GUID。

当前 main 仍只有独立托管 owned 图/JSON 模型，没有 `.gro` reader、native `ImportGroup` 载荷消费者、`AddModuleByInfo` 等价回调入口或原始导入持久化格式。已有 B02.1/B02.2 的 managed 克隆与索引传播不能替代该链。因此不新增兼容实现，不计 B02.3。

## B05.2 结论：仍阻塞，实施 +0

原 `GlobalVariableModuleTool`/`VariableManager` 证据显示：全局模块通过 ID 13000 动态装载；`InitGlobalIo` 从 `GetAllVar`/`GlobalValueList` 重建动态输出；添加、改名、改型、删除和绑定均经原参数命令；支持域包含数组、图像和复合 ROI；运行值上报有引用计数、事件和 UI 防反馈逻辑；方案重开经 native 全局模块表恢复。

当前 main 的 `WorkflowConfiguration`/`WorkflowSubscriptions`/`WorkflowRunContext` 只实现流程局部变量，类型受限于 `int`/`float`/`string`/`byte`，并无方案级全局模块身份、动态输出、双向订阅引用计数、全类型值写回或 native gvar 持久化合同。将局部变量扩展成三标量全局服务会把未知行为伪装成已知，因此不改 main、不关闭 B05.2。

## 新证据和退出条件

本轮重新核对 main 现有变量链和历史原始证据，未取得新的 `.gro` 编码/版本、native 导入回调载荷、GUID 分配差分、GlobalVariable 全类型合同或 gvar 持久化源码。下一次只有在取得这些 producer 证据或用户明确选择独立兼容合同后才重启；否则不重复扫描。

实施进度：本线 `+0`。本报告不把静态方法数、既有测试或外部 tool-plugins 交接计作完成。

