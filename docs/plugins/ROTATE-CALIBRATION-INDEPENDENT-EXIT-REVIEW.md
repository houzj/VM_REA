# Rotate旋转标定独立整工具退出复核

2026-10-20。根线统一freeze后已实际完成Path专项：build/run均exit0、0警告0错误，PASS25。本轮仅改测试/docs/analysis，未改产品，managed已明确释放给Coordinate线。完整实施结论交root审核，不自行计数。

| 活动链 | 源→当前实施→验证/退出 |
|---|---|
| 资源/模式/订阅 | 原InputWay只决定POINT/XY显示，算法不按意图读取该UI值；FilePath为OpenFileForCalibDialog，FilterName .xml|*.xml;*.iwcal，无SaveFileDialog，不把其它工具保存门扩到Rotate |
| FilePath前端 | 原ValueChanged suffix→raw父目录→SetParam；统一Attach按确切controlTag启用，失败保attempt不碰matrix/path/sample；picker强制重试且无条件history。实际Rotate PathChecks25通过，独立于五工具85 |
| FilePath后端 | 原SetParam38980在后缀门后Load返回错误仍存path；当前WriteFilePathParameter先Session.Load，再Initialized/AppliedPath，再adapter；Apply触发Synchronize因AppliedPath相同不重复Load。同路径显式写仍先Load，失败Loaded=false/保bytes与samples，前端拒绝则不Load。独立Host拒绝不声明native回滚。PathChecks25已核真实production状态及唯一通知 |
| typed采样 | 原Process376b0 Refresh→matrix valid→RotNum→X→Y→Angle，失败/Count0不消费poison；两Int/三Float opt-in及scope优先。Typed179已通过，原14oracle和Graph数组域保留 |
| Reset | 唯一Algorithm默认FilePath空、两个ISubInfo；原Frame传strlen0→38980入口E1，不碰旧缓存/采样/路径；专属CheckResetParameter仅default阶段拒绝，Reset16已过。直接Reset不经过文件UI门 |
| 求解/文件/非有限 | 原392e0/390e0/38f60求解、36byte与XML读取，保持既有1259；本轮不重跑数学。超长XML>9为明确安全边界，非有限结果保原errorCode且显式表示边界，不造0 |
| 保存/缓存 | 原SaveModuleData/Load的9float+valid映射CachedMatrix/Loaded；方案恢复不持久化采样counter。PrepareSave/Restore及删除文件恢复原fixture保留，不称原平台方案编码相同 |
| 显示/释放 | 原4显示对象/状态字段、LastDisplay快照；Lifecycle6核配置关闭保inflight samples、scope隔离/RemoveNode/Reset；共同execution.Dispose链不变 |

当前没有新发现的具体产品实施差异；新增FilePath writer/shared Attach实际25项已通过，当前选定本地工具范围源码逆向、实施及自动自审可交root最终审核。PathChecks含backend失败/成功各唯一通知两条（不改原算法断言），Program SHA118A196200965F158BAC56BAC0ED36360BB163DB6DBBC7EF2B4D965EBB4F1A8D。原Typed179/Reset16/Lifecycle6/旧1259真实日志与本次路径验证分别归档，不声称全部native等价。

统一freeze并获managed槽后最短命令（PowerShell；日志独立保留，逐个检查exit）：

```powershell
dotnet build tests/RotateCalibration.PathChecks/RotateCalibration.PathChecks.csproj --artifacts-path .a/rotpath -v:minimal *> analysis/rotate-calibration/independent-review-20261020/path-build.log
dotnet .a/rotpath/bin/RotateCalibration.PathChecks/debug/RotateCalibration.PathChecks.dll *> analysis/rotate-calibration/independent-review-20261020/path-run.log
```

若Path未改其它生产，既有Typed179/Reset16/Lifecycle6/1259作为此前真实快照保留；root决定共享变动必要复验，不自动堆数学探针。原Init设备、远程/root provider、任意native指针和原方案存储ABI明确边界，人工验收独立不作实施计数门。

本轮11项当前production/shared/resource/test与既有manifest身份：analysis/rotate-calibration/independent-review-20261020/manifest.json，SHA23AC9EF213B11AC505CB06D0219C75FD4950CA0DC545B00A6BE42F63E3864F47。历史typed manifest SHA4C32C893E64F1E999ABC800E08F3E224E7D34963D2D554E995F3091AED74C8B6不冒充本次路径新DLL验证。

## 实际运行最终身份

path-build.log及path-run.log位于analysis/rotate-calibration/independent-review-20261020。实际Calibration/Contracts Portable PDB共131源的compiled-source-manifest.json SHA D155B6D44E0886B926F183DB52F1902F3EFFC1E307F6B9E129559F9EB51D2A15；runtime-log-manifest.json（3DLL、2log、测试）SHA78BA6FDD67107FD252F4C5B5B513A486816116F87747D001C1DA0540CFE0F024。原静态11项manifest仍保留；本轮未调数学、容差或新增设备测试。
