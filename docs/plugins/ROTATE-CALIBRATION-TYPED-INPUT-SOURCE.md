# Rotate typed读取门源码与原定点oracle（2026-10-19）

旧Reset/Lifecycle/1259证据保留，未证明Typed source完整；Rotate仍未计完整。

原Process376b0读取RefreshSignal GetInt index0，失败将local值改0；非0刷新FilePath，然后检查matrix valid。未valid只清采样返E0000008，**不读取RotNum或任何Float**。valid后RotNum GetInt index0，原asm37906..3793a要求return0且Count>0且3..16才更新Total；37940..37961仍按localRotNum范围决定E1。Current+1/Remaining先更新，负数清采样返E1。X/Y/A分别index0，asm37a0e..37a1d及后两段均要求return0且Count>0才push；任失败写ModuStatus0、清三个vector/Current，返回原读码。因此Count0+return0是模块成功返回但ModuStatus0，不能用“读不到值”统一替换E031c/默认0成功。

原体旧反编译有(false)/(true)伪条件，Count门以module-disasm原asm为准；源module hashb08486...。新原厂owned实例12例在typed-input-oracle/run-e6f5e0048aae454887d29431a6b6b7b9：原probe创建/析构/导出，只有ModuleFrame五IO imports改受控host；无Init/设备。process.json exit0、provenance有原DLL/helper/exe身份。输入是两项数组，index0只取首项。

| 原例 | 返回/状态/顺序 |
|---|---|
| 0 | 正常第1样本Current1/Total3/Remaining2，八输出 |
| 1/2/3 | X/Y/A分别E0000123，后口不读，Current0/Total3/Remaining2，仅ModuStatus0 |
| 4/5/6 | X/Y/A分别成功Count0，返回0但Current0/Total3/Remaining2，仅ModuStatus0，后口不读 |
| 7/8 | RotNum错误或成功Count0，返回E1，三个Float不读 |
| 9 | Refresh错误忽略置0，其余正常第1样本 |
| 10 | RotNum第0项2（第1项3不可替代），返回E1，不读Float |
| 11 | matrix未加载，仅读Refresh，返回E8 |

完整候选proposals/typed-input/rotate-typed-input.patch SHA f8b6c0fa0cf31eff5dc3c29fb8f1d08327b0a03daafbd9925403e67a2602c74d，apply-check通过，未落未建。两Int/三Float显式SupportsDeferred opt-in；生产Runtime按分支调用typed reader，Session.Run typed回调重载保所有错误/Count/顺序；旧materialized Run重载仅为归档synthetic fixture协议兼容，不作为typed完成证明。scope服务优先→execution deferred→有限materialized，未改共享。

RotateCalibration.TypedInputChecks仅ProjectReference生产，原12例返回/计数/输出presence/读序重放与真实Graph Float/Int All/index1/正负越界已备未运行；不得据候选恢复计分。

## 目的槽poison纠正及14例补证

根线指出旧f8b6候选countRead.Value未按实际getter写槽条件隔离poison。原RotNum局部和Refresh局部均初始化0；新的9118adc4a4e276099b7df9d03e961437eb10e94cd4791adeb865ed8ec52061ad候选仅在Error0且Count>0消费Value；这是运输“写目的槽”适配，不把Refresh caller说成Count检查。

第二自有oracle run-b0c8ff040efd42218b3c94850eb1eea8共14例exit0：RotNum sourcePoison3在error/Count0两例，真实out slot before/after均0，原仍E1。新增Refresh sourcePoison1两例（成功Count0/错误Count0），out slot仍0；加载成功后删除磁盘原文件，原Process仍valid/current1，直接证明未触发refresh。C#测试注入ReadResult.Value=3/1/999 poison覆盖相同门。旧12例原trace保留，不覆写。
