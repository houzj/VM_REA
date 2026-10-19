# D02 batch2 独立复核

2026-10-19。复核对象为当前 main 的独立 `.vmx.json` 实现、`tests/VisionEditor.SaveHistoryChecks` 及根任务已运行的23项日志；本复核不重复运行构建、不调用原生SDK，不涉及 tool-plugins。它不提供原厂 `.sol` 或 VM 历史机制等价性证据。

## 结论

本切片结论成立，未发现需要产品修复的错误或阻塞性测试误报。检查使用真实 `GraphEditorDocument`、`SolutionStore`、`ResourceCatalog`，没有复制被测实现。已有日志及 `result.json` 的23项标签相符。报告 `D02-storage-batch2.md` 对独立实现、原生未知和整包未关闭的区分适当。

源码关键链为：

- `src/VisionEditor.Core/GraphEditorDocument.cs:37` 的 Save 先准备副本、重定位、写盘，成功后才提交当前路径/基目录/文档/修订。锁定另存失败发生于提交编辑器字段之前。
- 同文件249–250的 Capture/Restore 将快照基目录与文档一起保存；Restore 先从快照基目录重定位到当前基目录，再移动历史栈。因此 Undo/Redo 恢复内容但保持当前另存身份，有明确本地源码依据。
- 同文件254起的 Rebase 只处理资源表 Path 和内置 `vision.image-file` 的字符串 Path。测试分别读取实际磁盘及嵌入资源，检查内置路径解析的绝对资产身份。
- `src/VisionFramework/SolutionStore.cs:17` 的临时文件写入/Flush/Move/finally 清理链与目标字节保全检查一致。此次锁测证明同目录实际占用失败的结果，不证明断电或任意文件系统原子性。
- `src/VisionFramework/GeometryResources.cs:24` 的 OpenVerified 在返回流之前完整计算 SHA256 并复位位置。最终重开检查虽仅读一个字节，仍已对磁盘资源做完整摘要验证，不是只验证首字节。

## 精确覆盖与限制

本探针由一次 RenameProcedure 产生一个编辑历史项，在两次跨目录另存前后执行多次 Undo/Redo。可以表述为“跨多次另存的历史资源身份验证”，不能扩张为“任意深度、多种编辑历史全部闭合”。最终重开检查覆盖磁盘资源和名称；嵌入资源在编辑器各阶段完整读取，但没有单独声明重开后的嵌入资源检查。

失败另存断言检查文档序列化内容、当前路径/基目录、Revision、脏状态、CanRedo、目标字节和临时文件。随后真实 Redo 继续验证历史可用，不只检查一个布尔值。未直接比较整个 undo 栈或选择状态；源码也仅在写盘成功后提交字段，现有结论无需因此阻塞。

探针未设置 PrepareSave。实际宿主 `MainWindow.xaml.cs` 将其连接至 `plugins.PrepareSave`，因此本切片不证明插件准备器生成的新资源、参数重写或外部文件副作用与历史快照之间的关系。已有单纯准备器抛错检查不能替代这种组合场景。此处是有实际调用入口的后续缺口，不能从本地普通路径测试推断已通过。

## 后续必要操作

若下一切片继续D02，优先沿实际已交付准备器源码追踪“准备前文档→生成资源/重写参数→成功另存→Undo/Redo→重开”，明确哪些资源应保留、哪些由重新准备产生，再选择最小真实宿主检查。先核对现有 `ImageSource.PluginChecks` 的嵌入保存覆盖，避免重复基础测试；不得访问或复制另一个工作树的未提交实现。

未知字段/版本、32MiB及深度边界、真实中断等仍按主报告开放。Changed 在成功提交之后抛错属于提交后通知，不能归为写盘失败回滚。无需为本切片添加泛化测试或更改产品。

## 审计记录

已读取日志 `analysis/full-audit/batch2-20261019/D02/save-history.log` 及该日志指向的 `save-history-f1af2a7dae404d29b57c5c035af90187/result.json`。本次只读复核所用核心源码、测试、日志及全部该运行产物的SHA256保存于 `analysis/full-audit/batch2-20261019/D02/review/review-input-sha256.csv`。没有重新执行23项操作，运行结果归属根任务。
