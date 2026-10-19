# C02 第二批：有效配置来源与等待叶函数

日期：2026-10-19；main 基线 `6e5b05c`。本切片为源码和原托管叶方法验证；C02 完整调度包保持开放。无产品修改、SDK 初始化、设备调用、共享构建或其它工作树访问。

## 新闭合的配置链

此前 MF05 文档把 `ConfigurationHelper.GetConfigValue` 实现列为缺失，本轮已定位原安装 `Applications/myLibs/VM.Utility.dll`。

`Program.cs:224 → SystemConfigManager.IsClearLastExecuteResult → ConfigurationHelper.GetConfigValue("IsClearLastExecuteResult")`：

1. `GetConfig("ApplicationName")` 使用 `ConfigurationManager.AppSettings`；安装 exe.config 的 ApplicationName 是 `VisionMaster`。
2. 拼接 `.cfg`，对相对路径调用 `XDocument.Load`，因此解析相对当前工作目录，不是明确拼接 exe 所在目录。成功加载后 `_isLazyLoadCfg=true`，缓存 `_cfgDocument`。
3. 在根的直接子节点按名称 `SingleOrDefault`，再在其直接子节点选唯一 `CurValue`，返回 `Value`。缺节点、重复匹配、解析等 try 内异常均返回 null。不是读取 appSettings 同名键。
4. SystemConfigManager 的精确 `== "1"` 将 null、`"0"` 等判为 false；该属性另有 nullable 缓存。setter 只更新属性缓存；不是配置保存入口。`ConfigurationHelper.SaveConfig` 存在并写 cfg，但本切片未执行或证明设置页调用它。

安装 `D:/Program Files/VisionMaster4.4.0/Applications/VisionMaster.cfg:174–177` 实际记录 `CurValue=0`，SHA256 `30dccf82c93a1075d19fe0b699c3000a10e31e2b66935579720e094d34bc607e`。在此工作目录、该文件不变、正常进入上述初始化链的条件下，源码计算出的启动值为 false。**没有采集真实 VisionMaster 进程的 CWD 或属性值，不能称已观察其实际窗口有效配置。** 更不能把此安装文件值当成所有发行版本的默认。

## 新闭合的 Wait 链

`Hik.Util.SyncWaitor` 实际在 `Applications/myLibs/VM.Util.dll`，而非找不到的独立 Hik.Util.dll。原 CIL 已可复验：

| 入口 | 原方法行为 |
|---|---|
| `Wait(ref bool)` | 每轮检查标志；PeekMessage/TranslateMessage/DispatchMessage；无消息 Sleep(1)，没有时间上限。 |
| `PrecisionWait(ref bool,bool)` | UI=true 同上；false 包围 TimeBeginPeriod(1)/TimeEndPeriod(1)并 Sleep(1)，仍没有时间上限，后者没有 finally。 |
| `Wait(bool[],uint,uint)` / `PrecisionWait(bool[],uint,uint,bool)` | Stopwatch.ElapsedMilliseconds **大于** nTimeout 或标志为 true 就返回；不抛超时、不把 flag 设为 true、没有返回状态；Precision 的非 UI 分支配对计时精度调用。 |
| `SyncWait(ref bool)` | 使用阻塞 `GetMessage`，不是 PeekMessage；即使其它线程置 flag，也不能由此证明会唤醒阻塞的 GetMessage。 |

因此 `ProcessManager.SyncExecuteOnce/EX` 的普通等待没有托管超时参数；流程 `SetProcedureWaitTime` 是另一个 native 配置接口，不等于此等待函数内置计时取消。P2P 是带 nTimeout 的数组重载，时间到后可以在 flag 仍 false 时返回，随后正常递减计数。异常路径（如 SDK 提前抛出）仍无 finally，不能据超时返回证明所有路径都释放计数。

`RefreshUIStopExecute` 的 task 仅在 `StopExecute` 成功后置 isStopEnd；它调用无上限 `Wait(ref bool)`。源码不能保证 SDK 抛出后主等待一定结束。`GetWorkStatusCallBackInfo` 仅 workStatus=0 更新正常标志/完成旗，再派发事件；native 是否发出回调、时序及设备行为未验证。

## 原类隔离验证与副作用审查

`tests/NativeEvidence/C02/Run.ps1` 用 Framework csc 编译专属 Probe，不跑共享 dotnet。先核对 ConfigurationHelper 静态构造仅设置 lazy=false/document=null；只调用 GetConfigValue，未调用任何 Save 方法。脚本把原 cfg 复制进独占 run 目录，原件前后 SHA 不变；更改缓存验证只写专属 `Probe.cfg`。

四个独立进程场景：安装 cfg 副本返回0、显式1返回1、缺字段返回null、重复字段返回null。每例另验证原文档缓存不随副本磁盘改写自动刷新、已完成 flag 立即返回、5ms数组等待返回时 flag 仍false。**共20项断言通过，退出0**；实测等待约9–20ms，不把计时量当作精度保证或完整调度验收。probe只将 `=="1"` 作为调用方表达式计算，未运行 Program/SystemConfigManager/VmSolution。

强名称解析实际选择 GAC 的 VM.Utility/VM.Util，loaded-hashes.csv 保留全部实际程序集路径与哈希；已核对两者分别与分析安装原件SHA完全相同。本轮没有构造 VmSolution/ProcessManager、未发SDK请求。UI等待仅泵该probe线程队列，没有接管其它任务窗口。

首轮原类20项亦通过，但脚本生成哈希清单时把正在写出的清单自身纳入枚举而失败；该 run 原样保留，修复为先完成枚举后导出，再跑新run通过，不隐藏失败。

成功证据目录：`analysis/full-audit/batch2-20261019/C02/run-f2f8736123fe475481581ca57ca57da3/`。
首轮目录：`analysis/full-audit/batch2-20261019/C02/run-f8ca8a7de3e4424a95723350b33f6827/`。

## REA 身份与 CIL 锚点

完整 inline 记录保留 `analysis/full-audit/batch2-20261019/C02/{utility,util,exeJ,sdk}-rea.json`；完整 VM.Utility IL 在同目录。Wait IL复用 `analysis/full-audit/batch1-20261019/A01/VM.Util.il:9040–9513`。哈希为 raw CIL 字节，不包含方法头、异常表或局部变量签名。

| 原件 | SHA256 / MVID / REA evidence |
|---|---|
| VM.Utility.dll | `65ebb5edabf77465fa01f8c6d70efdcbd4b6a87f510911249cd93d73649d1cdf` / `32e8ac89-6335-43c6-aa3e-61d8c0a670e5` / `ev_17a12d045406658be0d3ec4eaacc51b23a6108dc714f50ac1f3ff3abd51366d1` |
| VM.Util.dll | `26b90443046fb2cb7f4cda3d4ef364a93c9a9ba65b492ba90da98abe01b2c3a0` / `96462886-0dfe-4999-8e3b-5d9df578645f` / `ev_9927f0ecf14305fc97b57ae6410f9549a32d6a56cc62e9548ecc5c66a92f6252` |

| 方法 token / RVA | raw CIL SHA256 |
|---|---|
| ConfigurationHelper.GetConfig `0600012c` / `5230` | `ce78077cd91a260684644a6f1ad1b86166464d6dfffd7a5d88cb805c1369b012` |
| GetConfigValue `06000145` / `59b0` | `e4f35712ad48741f440de25e54a0e208376df47dc376999de060c2819dda6e1f` |
| SyncWaitor.PrecisionWait(refbool,bool) `060000a9` / `68f4` | `dbbf9b45414767f5154ed620059d12a55d454996f346c748af2eb7980248096f` |
| PrecisionWait(array,index,timeout,bool) `060000ab` / `6a10` | `a38a6f4c1ee7d964e5d4247eb8463726790a95d6b9eb0524375e93630bfb4ca2` |
| SyncWait(refbool) `060000ad` / `6b6c` | `08d98f7ddb6f5863b2e389ee1c7d24cd083789526c4fdcec82eaf8c7c5e8b1cb` |

REA VisionMaster.exe evidence `ev_fbe6e3bef56ab98d6f6a7756adbe474d2b6583f820583efa489711898053e0bb`；SDK成员检查 evidence `ev_de13b657e3113b360efba931e280afdfd22fb5cd1901b38dcf2f58204aa718b7` 明确为metadata超过67108864字节预算、coverage unavailable，不能冒充 SDK CIL完整核对。ProcessManager 使用既有反编译源码；本轮不重复诊断未配置native引擎。

## main差异与下一步

`src/VisionEditor/WorkflowExecution.cs:31` 每轮无条件移除会话及根results；原方案只在开关true且执行计数大于1的begin分支清叶值。`SolutionExecutor.cs:22,49` 本地CancelAfter及TimedOut状态为独立协作取消，与本轮确认的原托管等待不同。当前不得把UI缓存策略、SDK结果缓存和算法执行输出混成一个开关直接修补；需明确main结果保留合同再由root修改唯一所有者文件。

建议最小后续子项：以自有实例记录真实CWD和启动读取值；取得单次/连续/停止/超时的SDK回调序列；原生等待配置效果与main协作取消分别验收；另追配置编辑/SaveConfig实际入口。设备、native调度、回调异常恢复和真实窗口结果留存仍开放。没有本轮产品补丁，不宣称native parity。
