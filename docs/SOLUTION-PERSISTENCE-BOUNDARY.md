# 原生方案保存边界与独立格式验收

2026-10-17。范围：主框架 F-18/F-19。本文区分反编译可确认的调用链与本项目独立格式的行为；它不宣布 `.sol` / `.solx` 已兼容。

## 原生调用链证据

| 原始文件与位置 | 可确认行为 | 边界 |
| --- | --- | --- |
| `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs:160-193,307-319` | 已有路径时调用 `VmSolution.Export` 或包装的保存命令；另存对话框显示 `.solx` 过滤项，保存前处理标定用户数据 | UI 与调用可见，文件容器字节布局不可见 |
| `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs:1535-1604` | `CheckSol` 调用 SDK `CheckSolution` 判密码；`InternalExport` 管理保存中状态、密码、路径，最终调用 `mSolutionManager.SyncSaveSolution` | 托管层没有序列化容器、校验块或压缩算法实现 |
| `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs:2924-2990` | `Import` 调用 `SyncLoadSolution` 并通过加载结束回调更新路径、密码；特殊错误码有条件接受 | 无法仅由此推出 `.sol/.solx` 完整导入语义 |
| `analysis/decompiled/iMVS-6000PlatformSDKCS.PublicAudit/iMVS_6000PlatformSDKCS/ImvsSdkPFDefine.cs:1382-1403` | 保存输入 ABI 有路径、密码、附加数据指针和长度、保留字段 | 结构体是 SDK 入参，并非磁盘格式声明 |
| `analysis/decompiled/iMVS-6000PlatformSDKCS.PublicAudit/iMVS_6000PlatformSDKCS/ImvsNativeMethods.cs:69,78` | 保存与加载的托管出口均为原生 P/Invoke | 磁盘格式及真正读写逻辑位于尚未完整分析的原生引擎 |

因此不能从已反编译的托管代码声称可以完整读写原生方案。要关闭 F-19，需要同版本真实样例、原生保存/加载实现的静态与运行取证、未知字段保留策略、密码与资源矩阵、往返差分。未获得这些证据之前不得把独立 JSON 改名为 `.sol` 或 `.solx`。

## 当前独立格式

`src/VisionFramework/SolutionStore.cs` 使用版本化 `.vmx.json`，校验后写同目录临时文件再替换；`src/VisionEditor.Core/GraphEditorDocument.cs:37-44,251-254` 在另存时重定位通用资源和内置文件图像节点的相对路径，仅成功写入后更新编辑器基目录与文件路径。这些是本项目的独立实现，不是原生实现证据。

`tests/VisionProcedure.Checks/Program.cs` 新增实际保存、另存、重开、按新目录读取同一资源，以及准备阶段失败时编辑器状态和目标文件不变的检查。测试不覆盖掉电、跨卷重命名、全部插件私有路径和原生文件兼容。插件私有持久化归插件线，不在主线重复实现。

## MF-05 独立嵌入资源校验增量

`SolutionStore.ValidateStructure` 现在在保存前和加载后检查 `EmbeddedBase64` 编码，并在有 SHA256 声明时校验解码字节与摘要。损坏数据在进入执行器前失败；已有方案不能因失败保存而被替换。这是 `.vmx.json` 的独立格式一致性保护，不是从原生 `.sol/.solx` 推断出的规则。`tests/VisionFramework.PersistenceChecks` 以实际二进制资源验证保存、离线重开、摘要不符与非法 Base64 拒绝，27 项通过；真实图像源含嵌入保存的 773 项插件检查亦通过。超大资源、掉电和原生容器格式仍未闭合。

后续增加同进程独占文件锁的异常写入检查：覆盖目标被占用时保存失败，原文件字节保持不变，生成的临时文件被清理；解除占用后再次保存成功。该 Windows 文件锁场景的实际持久化检查现为 30 项通过。此项只验证本项目独立格式的写入边界，未验证断电、跨卷或原生 SDK 保存事务。

编辑器层也用实际保存路径复验同一失败场景：失败后 `FilePath`、资源基目录、修订号与脏状态不变，已保存字节不变；解除占用再保存后，编辑内容真正写入且脏状态清除。`VisionProcedure.Checks` 现为 26 项通过。该操作验证补上“底层文件未损坏但编辑器误标记为已保存”的风险边界；并未改变原生 `.sol` 格式判断。
