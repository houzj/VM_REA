# D02 第六批：自有保存子进程中断

基线 `48a076000a784dd3400494f34c21567c12e61d6c`。有界子项 D02.process-interruption：**done（独立实现操作）**，不等于完整 D02 或原厂格式闭合。没有产品修改，不访问 tool-plugins。

## 源码与探针

`src/VisionFramework/SolutionStore.cs::Save` 在结构/大小验证后创建同目录 GUID 临时文件，Write→Flush(true)→File.Move(overwrite:true)，finally 删除本次临时文件。正常异常可走 finally，强制终止进程没有这一保证。本轮不调用 REA，因为分析的是完整当前源码；不能外推原 `.sol` 或磁盘断电语义。

`tests/VisionFramework.SaveInterruptionProbe` 引用真实 Framework，没有改存储实现或插入产品钩子。父进程先写有效旧方案，再以 Process.Start 创建自己的子进程，记录 PID/启动时间、独占工作目录和 token；子进程校验 token 后用真实 SolutionStore 保存16MiB名称的有效方案。父进程仅轮询本次独占目录的临时文件，通过刚创建的 Process 对象 Handle 暂停该子进程，确认临时文件仍存在后终止该对象。无进程枚举、无 PID 推断、无他任务进程操作。最多3次尝试，每子进程最多4次保存；若始终未观测到中断窗口，则 exit2、不计通过。

暂停/终止是本任务控制的子进程故障注入，**不是 SDK、原厂进程、窗口或设备操作**。临时文件保留，不递归清目录。后续恢复测试仅显式再保存同一目标，不自动扫描删除孤立临时文件。

## 实际运行

命令：`dotnet run --project tests/VisionFramework.SaveInterruptionProbe/VisionFramework.SaveInterruptionProbe.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false -- analysis/full-audit/batch6-20261019/D02`。

最终 exit0、passed=true，首轮即观察到窗口，4项检查通过。日志 `analysis/full-audit/batch6-20261019/D02/interruption.log`，记录及文件在 `interruption-87f94fd90c0947d194ab46f77d1bbb04/`。

- 自有子进程 PID55548，Suspend 返回0，终止后 exit=-1。
- 中断后目标仍为完整原方案，630字节，摘要 `AB593B2ED3EFA9994CA6A1181ECF334863995A56E72B09EFBDF5A28C45360664`。通用断言允许完整旧文档或完整新文档；此次实际保留旧文档。
- 留下一个16,777,838字节的 GUID 临时文件，摘要 `1808FCCB3969B516ABF24A38A34FE5602D19D0581521463965DC17478A2CD098`；中断前它仍存在，符合尚未完成替换的观测。
- 随后显式保存 recovered 方案成功并能重开；旧孤立临时文件仍在。未把该文件当成恢复权威，未静默删除。

这是一次具体进程中断观测，不保证任意时机/任意文件系统/跨卷/磁盘断电的原子性。未验证反复重启的孤立文件回收、自动恢复策略或任意 PrepareSave 外部副作用。FCR-024 的实际插件资产 Undo 失败仍开放，本次不覆盖。

本子项退出后不为增加次数重复运行。后续仅在出现不同中断阶段或明确产品恢复要求时定义新门槛；不得据4项检查关闭D02或推算框架完成率。

独立复核见 [D02-review-batch6.md](D02-review-batch6.md)。首断言是Load及指定ID/Name判定，不是任意完整图逐字段等价；当前留存目标为恢复后的recovered文件，中断时旧目标的证据为日志摘要及复核重构的字节匹配。现有证据不能区分具体发生在Write/Flush/Move之间的哪一点。
