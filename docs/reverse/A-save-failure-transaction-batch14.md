# 第十四批 A：保存失败的提交边界

本次只核验 main 独立格式的 `GraphEditorDocument.Save` / `SolutionStore.Save`，不推断原厂重试策略，不重跑413秒长稳，不修改产品，不访问 tool-plugins。所有拒绝夹具仅使用本批自己创建的目标文件与 `FileShare.None` 锁；每类操作一次。

## 源码顺序与预期

`GraphEditorDocument.Save` 依次：解析新路径→对文档副本调用PrepareSave→Rebase产生候选→SolutionStore.Save→提交document/BaseDirectory/FilePath/IsDirty/Revision→Changed通知。前四步未改当前文档身份或历史栈。`SolutionStore.Save` 校验并序列化候选，创建同目录GUID临时文件，写入并Flush(true)，随后 `File.Move(temp,full,overwrite:true)`；finally仅清理自己的临时文件。

因此应区分异常阶段：

| 失败阶段 | 本地合同 |
|---|---|
| PrepareSave抛错或校验/Rebase失败 | 编辑器身份/文档/历史尚未提交；外部delegate已产生的副作用不自动回滚 |
| 临时文件写入或File.Move拒绝 | 编辑器尚未提交；独占目的文件拒绝应保持原目的文件字节，并清理本次temp |
| Changed订阅者抛错 | 文件与编辑器身份已经提交；Save仍向调用者抛异常，**不能说所有Save异常都代表未提交** |

没有依据把413秒长稳中的UnauthorizedAccessException归因为本夹具的锁；本夹具只制造确定的拒绝输入，核验同一提交前路径。原长稳失败仍保留，不由专项通过覆盖。

## 可复验自审核

`tests/SaveFailureTransactionChecks14` 创建GUID独占目录，顺序检查：

1. 已保存clean文档对当前路径保存，被明确锁拒绝后仍clean；文档、完整undo/redo栈、选择、目录/路径、Revision和Changed事件数均不变。
2. 已有undo和redo的dirty文档保存到另一个被锁路径；PrepareSave返回不同候选，核验候选不泄漏、无内部重试、源与目的hash不变、temp清理，以及随后Undo/Redo可继续使用。
3. PrepareSave自己抛错时，整个编辑器状态不变且不写目的文件。
4. 无文件锁、Changed订阅者故意抛错：核验确已生成可读文件且编辑器路径/base/dirty/revision已提交，明确postcommit异常边界。

根线统一构建运行；实际成功/失败、异常完整类型/HResult/堆栈和逐项标签写入 `analysis/full-audit/batch14-20261019/A/transaction-<GUID>/summary.json`（夹具失败则写failed.json）。本线未执行构建或测试，不提前宣称通过。此专项自身不增加实施完成单元。

根线实际结果：SaveFailureTransactionChecks14 exit0，22/22通过；完整阶段异常/HResult与目标/事务hash保留A/GUID/summary和D/save-transaction-checks.log。该锁拒绝测试不等于原长稳瞬时根因复现。
