# 第十九批 B：C08.2 嵌套错误、取消和历史释放组合交付

固定单元为“嵌套错误、取消和历史释放组合”；不是重复关闭C05.2/C03.1，也不靠单层成功用例冒充完整组合。新增独占可复验工具 `tests/NestedCompositionChecks19`，实际引用main Core，使用GraphEditorDocument、SolutionExecutor、GraphExecutor、ModuleExecutionScope与第18批HistoryPayloadStore。没有SDK/设备/原厂窗口、没有tool-plugins、没有第二插件会话实现。本线不构建，根线串行执行后才填写实际结果。

## 原源码及独立实现的明确边界

原 `analysis/decompiled/VM.Core/VM.Core/VmProcedure.cs::Run` → ProcessManager.SyncExecuteOnce → IMVS_ExecuteOnce_V30，原VmException分支置IsRunning=false后重抛；详细CIL/回调位置见 `docs/MF05-PROCEDURE-RUN-CALLCHAIN.md`。VmModule.Run的参数准备、回调等待与End资源链在C02/C05证据内。原Group托管类没有实现完整native调度循环；失败传播、取消/设备停止与原生回滚承诺不能从托管转发方法猜补。

原UnRedo root/part完成新值、动作异常后不自动恢复栈、释放资源文件的证据来自第16/18批；产品的整方案snapshot/owned payload与SDK导出格式分开。工具验证的是当前独立managed已交付语义，不把协作CancellationToken当原native取消实现，也不声称执行副作用会回滚。

三层上限是当前main `OwnedGroups.Validate` 与GraphEditorDocument.CanAddOwnedGroup的明确支持域。本工具不由截图推导上限、不宣称已恢复原native最大深度。C08父包依赖有未闭范围，允许提前完成已有main支持域的有限组合工具，不借本子项关闭父包或原机综合C08.3。

## 固定完整矩阵

| 组合 | 实际对象/操作 | 同时核验 |
|---|---|---|
| 深度1/2/3 | 实际文档API创建OwnedGroup，最深含prefix→target→suffix执行链 | 每深度成功、throw跨全部Group失败、后继skip、下一次恢复；模块/state/history最终释放 |
| 最深正常 | root→G1→G2→G3→三个真实managed模块 | Calls嵌套、真实运行结果、异步Dispose全部完成 |
| 最深throw | target抛异常 | root/三Group失败、suffix未执行、prefix效果保留、不假设回滚 |
| 最深失败返回 | target返回ModuleStatus.Failed | 同层失败传播与skip、释放、下一次成功 |
| 最深清理异常 | target.DisposeAsync抛异常 | 已创建实例仍经过dispose、失败传播、下一次不保留旧失败 |
| 最深中途取消 | target已进入等待后发Cancel，任务忽略token直至自有release | 未提前伪称完成、root Cancelled、prefix保留、suffix未执行、dispose完成、随后恢复 |
| 运行/编辑/历史/持久化 | 完整运行后改嵌套名→Undo/Redo→Save→同IDLoad→再跑 | 原身份/新旧值、文件载荷实际释放、无旧history与失败污染 |
| 删除/恢复 | 停止后删除outer，RetainProcedureNodes，再Undo→再跑 | 全部三层后代移除、state各一次释放、历史恢复图后新state可执行 |
| 关闭/替换组合 | nested pending中scope.Dispose+doc.NewSolution，再新编辑并doc.Dispose，最后release旧任务 | scope lease尚在时state未释放、文档旧history清空、新history独立释放、旧任务Cancelled且末lease释放一次 |

删除限定停止后，符合main运行时禁编辑支持域；不强造“运行中任意删模块”的新产品规格。最后一项是实际Core对象关闭/替换，不冒充MainWindow、厂商插件窗口或原native线程关闭；现有主窗口关闭矩阵仍由C05已交工具保留，不重复计分。

## 工具实现与审计约束

每次新建独占GUID输出目录，强制文档file-backed history；不枚举或终止他进程。模块是任务自有真实IModule/IAsyncDisposable，失败/返回码/取消释放均经过产品GraphExecutor，未复制执行器。受控等待15秒限时；前缀/后继标识分开记录，已在源码自审修正不能把target返回当suffix执行。全局catch保留failed.json并exit1，避免断言异常变成WER挂进程；没有自动重跑或删除失败。

新旧值、状态、文件数和state释放都核在同一条组合上，不仅引用各单项曾经通过。输出summary含checks、3故障逐层观察、Created/Disposed与StatesCreated/StatesDisposed计数。运行结果未知前不写pass；native ABI/原资源导出格式留validation部分未知，不当人工验证阻塞main工具实施。

本线初始审查未发现需要先改产品的必要缺口，若实跑发现问题须保留failed.json/根线日志并由root唯一修改共享产品后复验。初稿源码哈希：`analysis/full-audit/batch19-20261019/B/initial-hashes.json`。

## 首轮实际结果及有限修正

已读取根线首轮实际摘要 `analysis/full-audit/batch19-20261019/B/nested-composition-809469d356bf42129bfffaef8fb7e78c/summary.json`：46项通过，包含全部3故障、深度1/2/3、中途cancel、文档历史/持久化/删除恢复/并发scope关闭释放组合；根线报告exit0。未出现产品修复需求，原始结果保留。

首轮新fixture有两处CS8604来自payload目录的冗余null检查流分析。根线确认无该项目构建期间，本线仅改为 `HistoryPayloadDirectory ?? throw` 并去除冗余isnotnull条件，仍强制确有真实file-backed目录；逻辑和矩阵不变，立即冻结并通知根线最终构建/复跑。首轮46不与后次累加，最终计数仍是同一46项，不预报尚未取得的新产物。

## 最终独立核验与具名单元关闭

已只读核对最终代码与 `analysis/full-audit/batch19-20261019/B/nested-composition-7d7912ad417c4cc2aa5c239cb503e3bf/summary.json`：46个具名且互不重复检查全部通过；三类故障观察分别保留ModuleException/OwnedFailure/ModuleException，叶后继均ExecutionPredecessorUnavailable；实际模块Created=Disposed=31，source state Created=Disposed=6。最终目录保留实际方案文件与summary，没有遗留history payload目录；这和运行内逐步文件/释放断言一致，不能只用目录干净替代断言。根线报告最终exit0、无新warning；首轮两warning与旧摘要保留，不累加两次运行数。

代码核验确认不是镜像执行器：实际生产SolutionExecutor递归、GraphExecutor执行/Dispose、文档API编辑/历史、ModuleExecutionScope state/lease、HistoryPayloadStore文件均被真实调用。两层简单用例不足的问题已扩至main支持深度1/2/3，最深运行三类错误/取消/恢复/历史/替换与关闭资源组合。已执行前缀效果明确保留，没有制造回滚成功断言；pending阶段真实TCS等待，不以睡眠假装并发。

**C08.2 作为总计划具名的当前main组合验证工具，implementation可记done。** 本工具完成当前已交付Core能力的嵌套错误、取消和文件历史释放组合交付及自审核，未发现该有限支持域的重大实施缺口；不需要为涨分虚构产品修复，也不重复计C05.2/C03.1/C04既有实现。

validation仍应partial：这是Core对象关闭/替换，非MainWindow交互关闭矩阵，更非原厂native事件、ABI/设备线程或完整UI同状态差分。C08.3原机综合未关闭，C08父包依赖也不因本工具自动完成。人工验收后置与上述边界分开，未知native不被当作main工具实施阻塞。最终源码/两轮真实摘要哈希索引 `analysis/full-audit/batch19-20261019/B/final-closure-hashes.json`。
