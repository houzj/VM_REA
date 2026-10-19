# C04.2 完整关闭可行性审查（第十二批）

基线 f53aa7b。只读检查现有原始反编译与 CIL；没有构造原 UnRedoSchedule、运行 SDK、改动安装目录或 tool-plugins。本文不增加实施完成计数。

## 原资源合同

- `analysis/full-audit/mf04-unredo/VM.UnRedo.il`：PoolStack.TryPush（318–380）在头部插入，超容量从尾部移出并通过 out 返回；UnRedoSchedule.BuildRoot（2763–2807）随即 Dispose 被淘汰根。
- Schedule 构造（1785–1848）先 ClearTemporaryFiles，随后创建两栈，读取 UnRedoStackCount，uint.TryParse 失败采用 15，两栈同容量。ClearTemporaryFiles（2542–2564）递归删除 AppDomain.BaseDirectory/TempFiles；不能在安装目录或其他任务路径构造服务。
- RedoStackClear/DisposeFileForBuilder（1876–1987）与 BuildPart（2809–2856）：新 part 如实现 IDisposeTempFile，先将新对象交给旧动作，再释放旧根、清 redo。一般路径直接 Dispose 旧根。
- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/SolutionUnRedoOperation.cs`：DeleteFile 名称具有误导性；实际将新动作 Data.Path 记录为 TempPath。Dispose 只删除存在、包含 TempFiles 且不等于 TempPath 的 Data.Path，保留新动作仍引用的同路径。
- `analysis/decompiled/VM.Core/VM.Core/ModuleUnRedoOperation.cs`、ProcessUnRedoOperation.cs：Dispose 清三个回调；存在且包含 TempFiles 的路径会删除；随后 Data=null。Module 数据亦有 pData/nDataLen 内存分支；Process 数据还包含 Point/Calib，不可统一替换成文件路径。
- RootUnRedoOperation.Dispose（3544–3569）依次释放动作后 Actions=null，没有 finally 保证某个动作释放异常后仍继续释放其余动作。
- 动态修改容量（约2308–2388）设置两栈长度；超过新容量时先清 redo，再从 undo 尾部逐个移出并 Dispose。
- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs` GetNewFilePath（382–413）使用 0..UnRedoStackCount-1 的 .sol 文件槽。既有方案路径可直接作为历史 Data.Path，不能假定每个历史都是不可变自有副本。

## 当前实现与完整关闭判断

`src/VisionEditor.Core/GraphEditorDocument.cs` 当前 Snapshot 存文档、流程选择和基目录，使用内存栈；Capture/Restore 实施独立 JSON 文档复制与基目录重定位，没有 SDK .sol/.prc/.gro 历史载荷。GraphEditorDocument.History.cs 为配置 root 的布尔禁止记录适配。

受控唯一目录、引用租约、容量淘汰、New/Load/Dispose 清理可以完整实现**独立文档快照资源管理**。但它不能恢复当前既定单元“原TempFiles载荷所有权、淘汰和释放”中的原 SDK 文件载荷，亦不能仅因方法映射、人工验收后置而改判该单元完成。本批不引入无宿主用途的新磁盘序列化，不修改已完成 C01.2，不调整分母、不将剩余 native 范围移出单元。

完整实施需补：原文件与内存分支的宿主载荷适配、方案同路径共享所有权及所有根类型释放接入。原文件格式及导入导出仍依赖 D01；如由根线明确决定当前主机使用独立文档格式履行生命周期合同，应在总计划留下显式范围决定，不能把该决定描述为原 SDK 格式已实现。
