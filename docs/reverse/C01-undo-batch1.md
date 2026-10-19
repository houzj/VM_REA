# C01 第一批：原厂撤销服务的隔离动态证据

2026-10-19。工作包仍为 **running**：本批执行的是未修改的原 `VM.UnRedo.UnRedoService/UnRedoSchedule` 与测试自有合成 root/part，不是真实 Group、SDK 参数、原机窗口或产品重构验收。未修改产品、共享合同或 tool-plugins。

## 原件与执行边界

REA `inspect_managed_artifact` Evidence `ev_dfa8463577a4a813c34d018a98de91a35493282640a53ca9f68b3e15b3c0b8aa` 确认安装目录 `Applications/myLibs/VM.UnRedo.dll`，SHA256 `AA719D093C948E8E6573D20C0198B60EF493F2BED8C029CAAB95ED5621A7AAA2`，35840 bytes，MVID `05297188-764c-48da-a387-1ca4029dd9b8`，AnyCPU CIL，.NET Framework 4.6.1。完整 REA 返回保留于 `analysis/full-audit/batch1-20261019/C01/rea-managed.json`。

先核对原 CIL `analysis/full-audit/mf04-unredo/VM.UnRedo.il:1785–1848,2542–2564`：Schedule 构造清除 `AppDomain.BaseDirectory/TempFiles`，再建两栈和读取 ConfigurationHelper。故探针 exe 和依赖复制在独占 `C01/run-<guid>/`，marker 校验后只在那里构造服务；TempFiles 只有本探针 sentinel。没有构造安装目录服务、没有设备或 SDK 初始化、没有附加/干预原机或其他任务进程。

可重跑入口 `tests/NativeEvidence/C01/Run.ps1`；合成动作 `Probe.cs`。直接使用系统 Framework64 csc，不触发共享 dotnet 构建。实际 CLR `4.0.30319.42000`；主服务从探针副本加载，强名依赖 VMControls.Interface 与 VM.Utility 实际由 GAC 解析，完整路径和哈希保留在 `loaded-hashes.csv`，没有把复制目录假称全部依赖的实际加载位置。原件、探针源码、可执行文件副本执行前后哈希一致。

## 实际序列与结果

正式脚本运行目录：`analysis/full-audit/batch1-20261019/C01/run-da13448c6b894eb19f1e6a81e1e1e318/`，`run.log`、`compile.log`、`hashes-before.csv`、`loaded-hashes.csv`。退出 0，13 个断言通过。

| 原服务操作 | 动态观察 | 原始成员/CIL 对照 |
|---|---|---|
| 构造前建立自有 TempFiles sentinel，构造服务 | 自有目录被删 | Schedule::.ctor、ClearTemporaryFiles，1785、2542 |
| 空栈 SetForbidRecord(true)，再 Register root | 新 root ForbidBuild=false | SetForbidRecord，1849 |
| 同根 true、true、false | 标志为 false，非嵌套计数 | 同上 |
| 根标 true 后 Register part | Actions 仍为 0 | Register，2008–2112 |
| 旧根 true 时 Register 新根，随后 false | 新根建立；false 仅改新根，旧根仍 true | Register、BuildRoot，2765 |
| Register 合成未完成 part | Actions=1，Updates=0 | BuildPart，2807–2864 |
| 首次 Undo | Update 内已见 IsDataComplete=true，之后才调用 part.Undo；root 入 redo | UpdateActionData/匿名委托 2865–2906、3085–3110；Undo 2113 |
| Redo | Redo 调用一次，Update 总数仍 1 | Redo，2216 |
| 新根 part.Update 主动抛自有异常 | 调用方未获异常；IsDataComplete=true、Updates=1、Undos=0；undo 栈数不变，IsInUnRedo=false | Undo 的 Update 在 Pop 之前，catch 吞异常，finally 清计数 |
| 对同根第二次 Undo | Updates 仍 1，Undos 变 1 | 完成标记导致跳过重新采集；并非安全重试 |

最后两项把前轮静态推论提升为此受控原服务运行观察：失败捕获会留下已完成标志，后次仍可能执行动作。这不等于真实 ParamUnRedoOperation 出现相同业务异常，也不证明原 Group 录制命中。

## 保留的失败与方法修正

首轮 `probe-run1.log` 保留，退出 1：合成 Part 仅实现接口，未继承 DisplayNotify，原 BuildPart 发生 NullReferenceException。CIL 2810–2864 在两个 `isinst DisplayNotify` 后直接调用 setter，接口实现本身不足。修正探针继承原 DisplayNotify 后，`probe-run2.log` 和正式脚本皆通过；未修补厂商程序集。首轮日志还保留自有 myLibs 不存在的日志监视器错误，正式脚本预建空 myLibs，不读取或替换安装配置。

## 未关闭项与下一步

尚未验证真实 Group Owner/Ioc/动态参数写入到 Register 的实际命中、同模块 predicate 去重、Param_Update 的 SDK 数据内容、CopyModule 优先级、RecoverStack 密码取消、TempFiles 栈淘汰的真实载荷、窗口及跨线程行为。合成 action 的 Update/Undo/Redo 是测试计数器，不是原 Group 或参数实现。

下一批在专属原机副本被动记录配置打开→root Register→Group 内 UserData/SDK setter→part Register→正常关闭→Undo/Redo 的对象身份与值；先闭合真实命中再决定 main 的全方案快照模型适配。本批不据 13 个断言关闭 C01，不声称运行等价，不把 root bool 改成全局 scope。
