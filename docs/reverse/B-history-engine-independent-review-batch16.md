# 第十六批 B：C01.1 原服务与真实实施独立复审

只读复审，未改C产品/测试、未构建或运行、未访问tool-plugins。原 `Applications/myLibs/VM.UnRedo.dll` SHA256 `AA719D093C948E8E6573D20C0198B60EF493F2BED8C029CAAB95ED5621A7AAA2`，原身份Evidence复用第一批 `ev_dfa8463577a4a813c34d018a98de91a35493282640a53ca9f68b3e15b3c0b8aa`。本片用现有ILSpy静态读取RootUnRedoOperation/UnRedoSchedule及接口ExecutePriority，存本线unredo-source；不是执行原Schedule（其构造器会清TempFiles）。并交叉核对 `analysis/full-audit/mf04-unredo/VM.UnRedo.il`。

## 原源码/CIL精确证据表

| 行为 | 原位置 | 语义 | 新Engine核对 |
|---|---|---|---|
| 准入 | UnRedoSchedule.Register，原IL2008附近 | replay与lazy计数<=0；predicate要求已有根；part要求已有根且非Forbid；predicate先于Build | root/part均有replay+signed lazy guard，predicate在capture前 |
| forbid | SetForbidRecord，IL1849 | 只改当前undo顶根布尔；空栈无影响，不是scope计数 | 相同 |
| root | BuildRoot，IL2765 | 构造新根并复制operation/forbid，根入栈不清redo | BeginRoot不受旧根forbid，不清redo |
| part | BuildPart，IL2807 | 准入后才清redo，并追加部件；原DisplayNotify/Parent是原UI合同 | RegisterPart准入后capture/清redo/追加；UI对象改为托管payload独立适配 |
| 完成新值 | UpdateActionData，IL2865/3085 | 未完成部件先IsDataComplete=true，再Update；首次Undo在pop前完成 | CompleteNewValue先标完成再capture；后续Undo不recapture |
| 普通Undo | RootUnRedoOperation.Undo，IL3370附近 | Heigher组反序，其余组反序 | Priority1反序后其他反序 |
| 普通Redo | RootUnRedoOperation.Redo，IL3250附近 | Heigher组正序，其余正序 | Priority1正序后其他正序 |
| CopyModule Undo | 同上CopyModule分支 | 所有Actions整体反序，无优先组重排 | 相同 |
| CopyModule Redo | 同上 | 非Lower正序，再Lower正序 | 非Priority2再2 |
| 枚举原值 | VMControls.Interface.ExecutePriority | Default=0，Heigher=1，Lower=2 | 整数映射已核 |
| 采集异常 | Schedule.Undo，IL2113 | Update在pop前，异常吞，根不迁移但完成标记已置；后次不重新Update | 捕获LastError、return false，不造rollback/安全重试 |
| 应用异常 | Schedule.Undo/Redo | 先pop/push和BuildParams/BuildInStack，后动作；异常吞，无自动恢复栈 | 先迁移后调用restore，异常保留迁移；finally退出replay |

IsRunning/LockAreaStatus在原Schedule中只是属性存储/通知，CanUndo/CanRedo只查栈数；不得额外给Register加入运行/锁禁止规则。原lazy计数允许负数，准入只拒>0；新实现保留此点。容量默认15，缩容清redo后淘汰旧undo；原Dispose/TempFiles载荷生命周期不在本泛型值payload内。

## 真实宿主适配已接入，非孤立fixture

本次阅读版本 `GraphEditorDocument` 的undo/redo Stack已替换为DocumentHistoryEngine<Snapshot>；CanUndo/CanRedo与Undo/Redo实际走engine。RecordEditHistory建立root并登记唯一document part，旧值Capture来自隔离Document复制；首次Undo完成新值，RestoreSnapshot重定位基目录并恢复方案/选择，增加Revision/Changed。配置会话仍一个root，Forbid作用只在当前root部件准入，不另造第二历史栈。

main所有普通动作使用整方案Snapshot作为payload，而非原SDK分类型参数；属于独立适配，原真实Group参数采集/SDK Undo属于明确另列C01.3。以泛型服务实现重建已证root/part/old-new/priority/准入，不表示原业务载荷完全移植。

Undo前仅对已完成部件NewValue做当前zoom合并，Redo前只对OldValue做合并；MergeCurrentView仅按存活ProcedureId替换CanvasZoomIndex，不改结构字段。此为维持已交付A03.3的main视图适配，不能称原PartentZoomIndex绑定/SDK字节恢复实测。根线应保留viewport、配置历史、跨目录SaveHistory回归结果，防止替换引擎伤及已有语义。

## 完成门槛判断

已读新 DocumentHistoryEngineChecks16：覆盖无根拒绝、bool forbid、predicate避免旧值重复采集、首次/重复Undo新值、普通与Copy顺序、replay拒录、Update/restore异常栈状态、根/part对redo不同影响、容量、zoom适配、lazy嵌套/负数与root predicate。源码审查时发现缺lazy准入已即时反馈C线，随后版本已补；没有私自修改C文件。

C01.1固定“root/part、旧新值与普通动作撤销”可在根线实际运行上述新检查并完成真实GraphEditorDocument既有配置/保存历史/视口回归、自审核无遗漏后关闭实施；当前只读复审不替代实际通过记录。原SDK payload、TempFiles资源文件、password/RecoverCallBack原业务分支继续独立开放，不把C01.1的done扩成C01整包或原服务完全等价。人工验收后置不阻该完整普通动作实施，缺真实业务载荷亦不能被偷偷宣称已做。

哈希索引在 `analysis/full-audit/batch16-20261019/B/history-review-hashes.json`；原IL与静态类体保留，所有失败/未验证边界仍保留。
