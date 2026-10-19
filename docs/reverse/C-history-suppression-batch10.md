# C 第十批：配置历史根的布尔抑制与异常复位实施

2026-10-19，基线 `877671f08eb2e6bf4eefff0bea461473624895cf`。范围为 C01.2 的 main 文档级 seam；C01.1 的原操作模型、C01.3 真实 SDK 参数采集与回放均不因此完成。未读取或修改 tool-plugins。

## 原始依据

- 原 `VM.UnRedo.dll` SHA256 `AA719D093C948E8E6573D20C0198B60EF493F2BED8C029CAAB95ED5621A7AAA2`，MVID `05297188-764c-48da-a387-1ca4029dd9b8`。既有 REA 身份及受控原服务运行见 `docs/reverse/C01-undo-batch1.md`，本批不重跑原服务或 SDK。
- `analysis/full-audit/mf04-unredo/VM.UnRedo.il:1849` 的 `UnRedoSchedule.SetForbidRecord`，RVA `0x2D1C`，空 undo 栈直接返回；非空只修改栈顶 `IRootParam.ForbidBuild`。`Register` 的 `IL_0064–008A` 只在 part 分支检查此标志，且在 predicate 求旧值之前拒绝注册。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmModuleDecorator.cs:242`，`OpenParamsTabControl` 先 Register `RootOperation{RootOperType="ModifyConfig"}`，再设置窗口 `ParamsConfig=VMModule.Params`，最后 Show。
- `analysis/decompiled/DynamicObjectConfigControl/Apps.XmlParser.ParamTab/GroupRunParam.cs:287`，`ConfigCommandExecute` 为 true → `method.Invoke(groupRunParamConfigData)` → false → 重载 Collection/GroupCollection/GetParamUnit。原方法的 EH 已于第五批核实，这三个调用没有异常 finally 覆盖。
- 第一批原服务已经实际观察 true、true、false 后为 false，以及空栈 true 对后建 root 不生效。因此本次没有引入嵌套计数器，也没有把原标志说成全局开关。以上文件本批 SHA256 在 `analysis/full-audit/batch10-20261019/C/source-hashes.json`。

## 本地实施与消费

`GraphEditorDocument.History.cs` 的 `ExecuteConfigurationEdit(Action<GraphEditorDocument>)` 是 main 全文档快照历史的适配：第一个通过校验的实际 Edit 才建立 root；后续写回仍修改真实文档、递增 Revision、发 Changed，但 root 的 ForbidBuild 为 true 时不再追加快照。空动作、配置等值及首次写回前异常不会生成 root。每次实际写回仍清除 redo 分支。

原布尔语义体现在 mutable Snapshot.ForbidBuild 与 `SetHistoryRecordingForbidden(bool)`：空栈不产生延迟标志；nested 使用同一 root，内层 finally 的 false 立即允许外层后续 Edit 再建立独立快照。内层为空但处在首写之前时，也按同一布尔语义取消剩余抑制，不偷换成“等所有 scope 离开后才恢复”。

finally 清标志与换方案后隔离是明确的独立安全加固。异常已经写入的配置保留，不伪造 Cancel/事务回滚。该 finally 不作为原 VM 异常复位行为的证明。

真实消费点包括 `GraphEditorDocument.Edit` 的 RecordEditHistory，以及根线接入的 `SubprogramCallHost` Group 配置入口。modal 外层 scope 对多次即时写回形成一条配置 root；modeless 回调每次动作单独 root，不宣称整个 modeless 窗口具有统一历史。这是本地 snapshot adapter，不把它冒称原 Register/SDK 参数旧值、新值、去重、临时文件载荷及 CopyModule 优先级的等价实现。原 Group 内层禁止 part 注册与本地“整个 modal 历史归一”的边界宽度也不等同。

## 自审核与限制

根线最终完整结果为 `analysis/full-audit/batch10-20261019/C/check.json`，29/29通过、exit0；真实调用点已在产品WPF构建中编译通过。第一次结果在工具输出，随后直接运行同一DLL只为保存JSON，不累加为额外覆盖。子线已只读复核modal和全部modeless写回，无双重nested scope；C01.2按实施自审核计done。

`tests/HistoryImplementationChecks/HistoryImplementationChecks.csproj` 覆盖真实 `SetWorkflowConfiguration`、`EditProcedure`、Undo/Redo、多次写回、空动作/等值、已有历史、异常前后、Changed 事件异常、nested bool、换方案、modal 绑定目标与 modeless 分动作模式。根线统一串行执行，实际退出 0，**29 项全部通过**；首次 JSON 输出保留于本对话工具记录，根线另将保存可复核文件。C 线未重复构建。

接入复核：`CreateSubprogramCallWindow` 唯一调用方是 `OpenSubprogramCall`，明确传 `groupEditsShareRoot:true` 并在 `ShowDialog` 外使用一个 ExecuteConfigurationEdit，回调没有再包同一 scope，避免每次 nested false 提前打开录制。`GroupCanvasNavigation` 采用默认 false 创建 modeless，OwnerEdit 与直接配置/节点参数/订阅/GroupRun 回调均经过 ConfigurationEdit；守卫先于作用域，无效入口及早退不遗留标志。ShowDialog 异常同时穿过文档 finally 和 parameterEditorOpen finally；窗口取消/关闭不撤销已完成的即时写回，空配置窗口无 root。该静态接入复核通过，未发现当前入口的双重嵌套或标志泄漏。

人工交互验收独立列待办，不计入实施完成分母。C01.2 的 main 文档 seam 已实施、接入、自审核通过，可据此计实施完成；报告数量和新增 helper 本身不计分。原 SDK 参数载荷/跨线程/真实 Group 差分继续属于 C01.3，未通过本批本地检查推定完成。
