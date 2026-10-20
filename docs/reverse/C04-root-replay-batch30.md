# C04.1 各根类型回放实施门槛复核（第三十轮 C 线）

## 结论

**本轮不关闭 C04.1，不改产品源码，实施增量 0。** 已交付的 `DocumentHistoryEngine<T>` 有 root/part 顺序、首次 Undo 补新值、栈迁移和失败边界；实际 `GraphEditorDocument` 只登记一个 `document` 快照 part。它能回放 main 自有方案文档，不能代表原程序的各根类型业务载荷。继续把每种根包装成同一个 JSON 快照，会掩盖不同的 SDK Import/Export、模型和密码取消语义。

## 根类型逐项核对

| 原根/part | 原反编译数据与消费者 | main 当前消费者 | 缺少的可执行合同 |
|---|---|---|---|
| 普通模块/Group | `VM.Core.ModuleUnRedoOperation` 持 `VmModuleDataInfo`；`VmModule.Import_Module/Update_Module` 分普通模块内存数据与 Group `.gro` 路径，导入进入 SDK。见 `docs/UNDO-ROUND2-20261019.md` 第 2 节。 | `GraphEditorDocument` 整文档快照可恢复自有节点及 Group 文档字段。 | 原模块导出字节的结构、SDK 导入结果及 Group `.gro` writer/reader；没有可消费的原始载荷。 |
| 流程 | `VmSolution.Delete_Process/Import_Process`（反编译 1638–1659）按 `UnRedo_ProcessData.Path` 选择导出临时流程、`ImportProFile(...,1u)` 或创建新流程；`Export_Procedure`（1661 起）等待原生导出结束。 | 自有 `ProcedureDocument` 由整个方案快照回放。 | 临时流程文件格式、导入 callback、实际 SDK 流程身份/释放顺序。 |
| 方案 | `ProjectManager.Redo_Solution/Undo_Solution`（1032–1082）检查密码，随后 `VmSolution.Import(path,passWord)` 或创建新方案；取消时仅调用 `RecoverCallBack`。`SolutionUnRedoOperation` 比较 `TempPath/Data.Path` 决定删除。 | main `NewSolution/Load` 清本地历史，自有编辑用文档快照。 | 原 `.sol` 导入/导出、密码取消后显示/数据状态、原临时方案路径所有权。 |
| 模型 | `VmModule.Model_Undo/Redo/Update`（5563–5578）调用具体模块的 `ImportModel/GetModel`。基类 `GetModel` 返回 null、`ImportModel` 空实现（3874–3891），需具体模块实现。 | 自有节点参数包含在文档快照。 | 各模块模型 producer/consumer 属具体工具或 SDK，main 无可回放载荷。 |
| 参数、UserData、动态 IO、订阅 | `ParamUnRedoOperation`、`UserDataUnRedoOperation` 等各持专用 data；参数经 `SetAllParams`，UserData 的 `[ParamRoot]` 首尾在操作的 Undo/Redo 动态判定；详见 `docs/UNDO-ROUND2-20261019.md`。 | 自有配置字段含在单个 `document` part。 | 原 SDK 参数树、UI 数据对象、订阅重载副作用与各根类型间的回放顺序在 main 无逐 part 消费者。 |

## 已实施的栈语义与差异

`analysis/full-audit/mf04-unredo/VM.UnRedo.il` 2008–2112、2137 起、2865–2906、3085–3110：准入后登记、首次 Undo 先标完成再 Update、随后移根并按 priority 执行；异常不自动回滚。`src/VisionEditor.Core/DocumentHistoryEngine.cs` 实际实现此通用调度与显式载荷释放。`src/VisionEditor.Core/GraphEditorDocument.History.cs:62–67` 仅创建 `Edit` 根并登记 key=`document`；`GraphEditorDocument.cs:274–305` 的 Undo/Redo 和 JSON 快照消费者是 main 独立适配。C04.2 的文件 lease 生命周期已经在第十八轮实施，不在此重复计分。

## 自审核与解除条件

核对了原反编译操作类型、数据字段、主要调用者及 main 实际登记/恢复路径；未运行安装原件、SDK、设备或密码输入。无产品改动，因此没有新构建结果可作为实施进度。本轮各根类型并未闭合，C04.1 保持 `implementation=partial`、总计数不变。

要关闭 C04.1，须取得并核对每一仍属 main 框架的根类型的完整 producer→payload→Import/restore→异常/释放合同，包括受控原始 `.sol/.gro` writer/reader 与密码取消的真实数据状态；具体工具模型由 `tool-plugins` 所有，需用户决定版本化合同交接后 main 才消费。`tool-plugins` 未访问、未修改。精确源文件及 SHA256 见本地 `analysis/full-audit/batch30-20261020/C-root/source-manifest.json`。
