# 第二十九轮 D02 独立存储事务候选筛选

基线：`f18afa7`（第二十八轮问题空间审查）。本轮只审查 main 自有 `SolutionStore`/`GraphEditorDocument` 的可独立闭合事务候选，不访问或修改 `tool-plugins`，不把独立 `.vmx.json` 规则扩展为原生 `.sol` 语义。

## 入口与源码证据

- `src/VisionFramework/SolutionStore.cs::Save`：`ValidateStructure` → UTF-8 JSON 序列化/32 MiB 限制 → 重复属性检查 → 同目录 GUID 临时文件 `CreateNew` → `Write` → `Flush(true)` → `File.Move(temp, full, overwrite:true)`；`finally` 仅删除本次临时文件。
- `src/VisionFramework/SolutionStore.cs::Load`：长度限制、`JsonDocument.Parse`、递归重复属性拒绝、`JsonUnmappedMemberHandling.Disallow` 反序列化及结构校验。
- `src/VisionEditor.Core/GraphEditorDocument.cs::Save`：先调用 `PrepareSave`，再 `Rebase`，成功写盘后才提交 `document/BaseDirectory/FilePath/IsDirty/Revision`；失败准备器的外部副作用不在该事务内。
- `src/VisionFramework/GeometryResources.cs::ResourceCatalog.OpenVerified`：资源打开后完整 SHA-256 校验，成功才返回流；未发现新的缺失分支。

## 候选结论

本轮 **没有可安全实施并闭合的 D02 单元**。原因是现有源码已经闭合 main 自有 JSON 保存的有限事务链，新增重试、孤儿临时文件回收、跨卷替换或断电恢复均缺少原程序的调用约定/资源语义；历史 D02.2 已证明对旧历史调用当前 `PrepareSave` 会改变 scope/插件资源语义，不能用补丁假关。`SolutionStore.Save` 的 `File.Move` 失败、进程中断和外部 `PrepareSave` 副作用仍是已记录边界，不构成新实现依据。

因此：实施增量 **+0**；不修改 `src/`，不新增产品测试，不重跑历史失败探针。

## 自审核与剩余阻塞

已核对 D02 既有报告 `D02-review-batch2.md`、`D02-storage-boundary-batch4.md`、`D02-interruption-batch6.md`、`A-save-failure-transaction-batch14.md` 与 `D02-asset-history-batch3.md`。已有结果覆盖成功保存、拒绝事务、受控子进程中断和跨目录资产历史反例；未提供原生 `.sol` writer/reader、插件资产 scope 合同或恢复策略。解除条件：取得同版本原始 writer/reader 或用户明确选择独立兼容格式的差异范围，并形成新的操作级合同。

`tool-plugins`：未访问、未读取、未修改。
