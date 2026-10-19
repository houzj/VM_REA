# C04.2 历史载荷所有权、淘汰与释放（第十八批C线）

## 固定单元边界

根线明确本单元实施 opaque payload 所有权合同；原SDK导出/导入与内部格式仍归C04.1/D01开放。文件内容不需要解析为原.sol/.gro才能管理生命周期，但只有新类或合成文件测试不构成完成：根线须将实际GraphEditorDocument历史part接入文件载荷，并在engine容量淘汰、清redo、清历史、替换和关闭时真实释放。

本批子线独占新HistoryPayloadStore和测试，不修改现engine或共享MainWindow，不运行SDK、设备或原UnRedoSchedule构造。

## 原始依据与安全适配

原证据沿用 `docs/UNDO-ROUND2-20261019.md`、`docs/reverse/C-temp-history-closure-batch12.md` 与 `analysis/full-audit/mf04-unredo/VM.UnRedo.il`：

- BuildRoot经PoolStack.TryPush得到旧尾root并Dispose；缩容先清redo，再逐尾释放undo。
- BuildPart先清redo并释放旧root；原SolutionUnRedoOperation.DeleteFile实际设置TempPath，随后Dispose时同路径不删，保留新part仍引用的文件。
- Module/Process Dispose清回调，文件路径包含TempFiles且存在时删除，再Data=null。Module另有内存pData/nDataLen分支。
- Root Dispose逐part；某项释放异常会传播，中途没有finally保证其余项继续。不能把释放异常包装成通用回滚。
- 原Schedule构造递归清AppBase/TempFiles；本项目明确不采用该危险全目录行为，亦不构造原服务。

实现采用唯一GUID目录与显式租约，不照搬字符串Contains("TempFiles")判删除权。共享租约泛化了原方案同路径保留的效果；借用外部现有方案路径永不取得删除权。两者均明确是main安全适配，不声称原厂采用引用计数。

## 实际实现及根线接入接口

`src/VisionEditor.Core/HistoryPayloadStore.cs`：

- `new HistoryPayloadStore(parent)`只创建自有`history-<GUID>/.owner`，不清理既有目录。
- `CreateMemory(bytes)`复制内存，`CreateFile(bytes)`写新生成`.payload`文件，内容是opaque bytes，绝非原SDK格式声明。
- 流式`CreateFile(Action<Stream>)`允许生产方直接序列化，也用于真实部分写故障fixture。Write/Flush失败只尝试清理本次CreateNew成功的新文件一次；原写错误、清理错误及路径留在WriteFailures，清理失败保留文件，不进入盲重试。
- `AcquireFile(path)`如已追踪则共享其所有权；其他存在路径只借用、不删除。
- lease支持`Retain`、复制读取`ReadBytes`和幂等Dispose；最后引用才删除自有文件。删除失败传播并保留该lease及文件，供调用方审查/显式后续释放，不盲重试。
- Store.Dispose关闭新准入，现lease仍可读取；待所有lease释放后仅移除空自有目录。未知审计文件会阻止目录清理，绝不递归删除。

根线唯一接入：engine新release callback用于old/new实际载荷淘汰；文档Snapshot默认JSON≥64KiB时lazy创建自有store并持实际payload lease，小文档保持内存，传入自有parent的测试构造参数强制文件。64KiB是独立main存储策略，非原厂阈值。Capture写当前独立文档字节，Restore真实读取该lease；独立zoom适配创建新载荷、释放被替换租约，不就地改共享字节。New/Load清历史租约并复用本doc自有store，doc/主窗close关闭store；失败保留原异常与仍有效的payload。isolateddraft使用using释放自身历史。

## 自审核

`tests/HistoryPayloadChecks18`：内存输入/输出复制、共享同路径多lease与幂等旧释放、借用文件不删、store close延迟释放、关闭后拒绝新准入、未知审计文件保护、受控只读共享句柄阻止删除时保留原载荷和引用，锁释放后由测试调用方显式释放。所有文件都在新GUID审计目录，锁只属于本测试。

本子线未构建。根线须补实际文件历史路径的编辑/Undo/Redo、淘汰、清redo、缩容、方案替换与关闭回归并留before/after文件清单及hash；未通过前不把本报告当完成证据。原SDK载荷编码、密码恢复、各根业务导入导出仍未完成，不因本单元所有权适配而关闭。tool-plugins未动。

## 实际引擎复核与追加测试

只读复核根线新接入：part以hasNewValue区分完成标记与成功捕获，失败capture的default不会被当作有效新载荷释放；adapt先置新payload，旧释放失败保存在retainedValues并传播，root释放失败保存在独立集合，显式ReleaseRetainedPayloads复收，失败日志不删除。这是比原“异常中断其余释放”更强的独立清理/保全策略，不能说原厂具有同一异常恢复。

`tests/HistoryPayloadEngineChecks18`直接运行实际engine release callback：容量、Undo/Redo迁栈不提前删、分支清redo释放旧新两侧、缩容、共享path跨root淘汰、old/new adapt释放替换文件、受控锁删失败保留root及文件、锁解除后无自动重试、测试显式复收后错误审计仍保留。`HistoryPayloadChecks18`另新增真实写入2字节后抛IOException的partial写入fixture；最终summary保留具体WriteFailures。

子线没有修改引擎或GraphEditorDocument，也没有构建。根线自有 `HistoryPayloadHostChecks18`负责实际文档文件路径验证，三层测试通过与结果由根线串行整合。

## 最终独立自审核结论

已只读核对根线实际产物及当前产品接入：

| 实际专项 | 结果 | summary |
|---|---:|---|
| store内存/文件/共享/失败写与释放 | 18/18 | `analysis/full-audit/batch18-20261019/C/payload-checks-ca3141eabca745a294666080db96fae0/summary.json` |
| 实际engine迁栈/淘汰/替换/失败保全 | 17/17 | `analysis/full-audit/batch18-20261019/C/payload-engine-47befd87080e41e8a679b2505f426867/summary.json` |
| 实际文档默认溢出/强制file/历史/生命周期 | 19/19 | `analysis/full-audit/batch18-20261019/D/history-host-7b66a56ce7f143f18c9bd257eb1af2f2/summary.json` |

三份summary的passed值均与实际checks数组长度一致。最终Host19替代同批较早Host18为最终验证；两次产物都保留，不累加成37项覆盖。Host19 SHA256为 `4E437BCA19CB3832279A0B6D282B27753E1F1B5EC2BBEFED7051B276C5A44017`。

新增marker失败边界已过：自有.owner被改变时保留目录，GraphEditorDocument.Dispose把清理异常加入HistoryReleaseErrors，避免阻断后续主窗scope关闭；HistoryPayloadWriteFailures公开原部分写失败诊断。它不是强行清目录或吞掉审计证据。

独立审查判断：**按本轮明确固定的opaque所有权/淘汰/释放合同，C04.2实施可判完成**；不是仅有接口或模拟文件，真实主框架大文档默认进入文件历史，实际Undo/Redo从文件恢复，真实分支、容量、Load/New/Dispose消费释放。根线还将运行既有engine、文档、配置与保存失败定向回归，最终台账和提交由根线在这些结果齐备后更新，本报告不预填其未执行结果。

原SDK文件编码、原导入导出、原业务根载荷恢复和密码回滚继续C04.1/D01/C04.3开放；借用路径与SDK数据未被改写。局部专项通过不证明原生完整等价，人工验收不参与本实施计分。最终源码与专项summary哈希见同目录 `final-review-hashes.json`。
