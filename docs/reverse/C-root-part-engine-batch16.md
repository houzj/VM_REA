# C01.1 root/part 主机动作引擎实施（第十六批）

本批将普通历史从交换文档快照栈改为真实root/part动作合同，不用容量小修代替该单元。子线实现新引擎及测试，根线持有GraphEditorDocument和既有配置scope的串行接入；实际构建、宿主回归及计分由根线确认。没有构造原SDK或TempFiles服务。

## 原始源码与CIL

原身份沿用 `docs/reverse/C01-undo-batch1.md`：VM.UnRedo.dll SHA256 `AA719D093C948E8E6573D20C0198B60EF493F2BED8C029CAAB95ED5621A7AAA2`，MVID `05297188-764c-48da-a387-1ca4029dd9b8`，已有REA身份记录和13项原合成服务证据。精确CIL为 `analysis/full-audit/mf04-unredo/VM.UnRedo.il`；B线本批恢复的完整静态类位于 `analysis/full-audit/batch16-20261019/B/unredo-source`，作为交叉复核，不执行类型。

- Register（2008–2112）先 `_unRedoing<=0`、`_nLaziLoading<=0`，part要求现root、非ForbidBuild，predicate在准入后决定是否加入。设置ForbidRecord是当前root布尔值；SetIsLaziLoading是有符号增减计数，不能混为嵌套bool。
- BuildRoot新建root并按容量淘汰；BuildPart在已准入后清redo并加part。不能在被拒绝的part之前读取昂贵旧参数或清redo。
- UpdateActionData（2865–2906、3085–3110）只选未完成part，**先标complete再Update**。Undo先完成新值、再迁移root到redo、然后执行；Redo不重新Update。
- 普通Undo先priority1逆序，再其余逆序；普通Redo先priority1正序，再其余正序。CopyModule Undo全体逆序，Redo先非priority2正序，再priority2正序。判定来自RootUnRedoOperation及各生成lambda，不用常规栈习惯猜排序。
- Undo/Redo catch不回滚已迁移栈，finally减计数。Update失败在迁栈前且complete标记已置；后次Undo不会重新捕获。这是原已证失败边界，不伪装安全事务。密码/RecoverStack另属C04.3。

## 真实实现

`src/VisionEditor.Core/DocumentHistoryEngine.cs`：

- `BeginRoot`与`RegisterPart`均实施replay/lazy准入；root、part各有可选predicate；part旧值只在准入后capture。
- root有实际part集合、operation和ForbidBuild；part有旧值、新值、complete及capture/restore委托。首次Undo完成new，按原排序执行实际restore，不是接口空壳。
- 两栈迁移、默认15容量、动态缩小清redo并淘汰旧undo；内存载荷不创建原文件格式，TempFiles生命周期不在此冒称完成。
- 异常被捕获并保留LastError（主机诊断增强），计数finally恢复，无自动重试/回滚。
- 明确公开 `AdaptOldValue`/仅complete可用的`AdaptNewValue`主机seam，用于根线仅合并CanvasZoomIndex的独立视图适配。引擎本身从不自动调用它们，不据此改变原普通动作old/new合同。

根线接入应使每个普通Edit创建root+真实document part，配置scope同root按document key predicate合并，首次Undo读取最终新文档；capture/restore保留深复制、流程选择和跨目录Rebase。New/Load清引擎，Undo/Redo不再走旧Stack。配置布尔嵌套仍沿已done C01.2合同，不能因换引擎破坏29项既有检查。

## 自审核与剩余边界

`tests/DocumentHistoryEngineChecks16`覆盖旧新捕获次数、predicate拒绝不capture、root/part准入、boolean禁止记录、lazy嵌套/原有符号下溢、普通及CopyModule排序、回放期间拒绝登记、capture失败前标志、动作失败后栈迁移、redo分支、容量与显式视图适配。

子线不构建；根线还须运行实际GraphEditorDocument编辑/Undo/Redo/配置scope、旧新视图与SaveAs历史回归，才可将C01.1判断为主机普通动作实施完整。测试数量不替代宿主接入。

C01.3真实Group SDK参数采集、C04原native文件载荷与密码恢复继续独立保留未完成；该边界没有被移动到新的分母或从原计划删除。原异常策略与本地主机诊断增强均如实标识。tool-plugins未动。

根线实际结果：engine46+真实文档host12通过；配置scope29、文档视图29、保存失败22、SaveHistory23、编辑核心115、Group33、实际WPF缩放29回归全exit0。真实主文档已接入，C01.1按固定普通动作实施门槛记done；C01.3/C04原载荷仍开放。更新SaveFailure14观察器只为跟随真实root/part旧新payload，不削弱事务比较。
