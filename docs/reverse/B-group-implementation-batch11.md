# 第十一批 B：Group 索引解除与嵌套复制自审核

基线 `b894e8c`。仅修改 main 自有 `src/VisionEditor.Core/GroupInterfacePropagation.cs`；未涉及插件配置、Contracts、执行器或 tool-plugins。总进度单元不因断言数增加。

## 原源码与真实缺口

本批复用本地原始反编译材料，未新增 REA 调用或原厂运行：

- `analysis/decompiled/VM.Core/VM.Core/VmIO.cs`：`ClearRelation` 查询 `QueryModuleIOBeSubscribed`，用发布字段、索引字段和模块 ID 区分引用种类。`case 1` 表示仅索引引用，只有 `item3.RelateIndex = null`；`case 0/2` 才同时清值引用。`OnRemoveCollection` 进入 `Dispose`；不将本批静态阅读冒称原生删除全链运行。
- `analysis/decompiled/VM.Core/VM.Core/DynamicVmIO.cs`：`OnNameChanged` 对 `nextSubIO` 的 `case 1` 重新设置 `RelateIndex=this`；值源与索引源身份独立。
- `analysis/decompiled/VM.Core/VM.Core/VmModuleSet.cs`：`ProcedurePaste` 实时查 `VmSolution.Instance[sub]`；Group 经 `Export`、`LoadGroup(...isFromCopy:true)` 和参数复制；复制映射建立后依次 `ModulePaste`、`ModulePasteFinish`。main 的独立 owned 图映射不能称为原 gro/native GUID 实现。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmGroupDecorator.cs`：`MaxLevelOfGroup=3` 与 `GetLevel` 的父 Group 遍历。

main 原 `PropagateGroupInterface.Apply.Bindings.Operand` 在输出被移除时，无论是值源消失还是仅索引源消失都返回 null，丢掉仍有效的数组操作数。现仅值源删除时清操作数；仅索引源删除时保留 Source、Inside、Self、Constant 和 Index，只清 IndexSource。输出 rename 继续按稳定 interface ID 更新 index Port。

没有给删除索引强加 Index=0：原 `ClearRelation` 没有更改 Index；main `GroupOperand.Index` 虽默认 0，但已有静态值不能由默认值取代；这也与 main `ClearInvalidGroupBreakVariables` 的既有策略一致。`IndexSource` 按 main `GroupBreak.Validate` 固定引用父域标量整数，没有单独的 IndexInside/IndexKind 字段。

## 自审核与有限测试

`tests/GroupImplementationChecks11` 针对实际 GraphEditorDocument 产品 API；不复制生产传播函数。共 23 个检查，覆盖：

- 输入 rename/remove 与父订阅同步；复制时使用已更新的源；两层 owned node/body/Parent 独立重映射。
- 删除复制 Group 级联清后代与父订阅、保留源图；Undo/Redo 恢复及删除。
- 拒绝错误输入类型、拒绝第四层 Group 时文档和 Revision 不变；类型拒绝保留历史可用性。
- 实际 GroupBreak 自数组源的父索引输出 rename/remove；删除仅清索引，保留显式 Index=2；Undo 恢复改名后索引关系。

根线串行运行退出码 0，23/23 项全部通过；日志 `analysis/full-audit/batch11-20261019/B/check.log`。未由本子线重复构建或运行。最初补丁草案曾拟在解除索引时归零 Index；复核原 ClearRelation 和现 main 变量删除路径后，在任何测试运行前撤回归零，改为保留已有静态索引。该静态更正不是已执行失败，未虚构红测。

## 进度与未覆盖

B02.1、B02.2 原已实施完成，本次回归及补丁不重复计分。B02.3 原 gro/GUID 分配、原生复制持久化仍未实施，不据此关闭。B03 的原生全生命周期、原机交互桌面差分及跨容器 SDK 行为不在本批覆盖。没有修改原件、设备、注册表或其他任务进程。

材料路径与 SHA256 清单：`analysis/full-audit/batch11-20261019/B/evidence-hashes.json`。最初相对源码路径和 PowerShell glob 查找失败未产生目标运行或产品输出；改为准确文件路径后读取成功。
