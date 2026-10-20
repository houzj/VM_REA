# C 线第三十轮：native 结果回调与取消/释放解除条件

## 结论

本轮取得了更完整的原始托管回调消费者源码，但没有取得 native 结果载荷的生产者、设备取消实现或原生历史载荷格式。因此 C 线没有安全可计分的新增实施，**实施增量 +0**。

## 新增源码证据

- `analysis/decompiled/VM.Core/VM.Core/TempVmSolution.cs:19-50` 的静态初始化一次性订阅 `OnModuleResultCallBack`、`OnWorkStatusCallBack`、连续/停止流程状态、导入导出、Group 导入导出、全局变量、通信、相机和方案结束等事件。该文件只证明订阅拓扑，不包含 native producer 或 payload 所有权。
- `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs:4067-4171` 的 `MProcessManager_OnWorkStatusCallBack` 先广播 `OnWorkStatusEvent`，按 `nWorkStatus` 更新 `ExecuteCount/ProcessTime/canModuleSelfExecute`，在有执行次数时入双缓冲队列，然后调用 `proc.OnModuleResultCallBack`；非连续运行还更新 `_waitNotifyPages`。异常仅记录日志。
- `VmSolution.cs:4191-4335` 的 `MModuleManager_OnModuleResultCallBack` 先按模块 ID 查找对象；Group 循环结果使用 `StartIndex/EndIndex/LoopIndex` 和状态码 5/2 判定 loop end，再把结果字段按参数名 hash 写入 `VmIO`。普通结果在 `SetValue` 中先调用同步 `module.OnModuleResultCallBack`，之后才入队异步通知；循环未结束时抑制 `RaiseValueChanged`，结束时恢复默认值并触发值变化。真实 `pInfo` 所有权、释放、跨线程 producer 仍在 SDK 边界。
- `VmSolution.cs:4345-4415` 的 `CreateCallBackQueue/DequeueForCallBack/EnqueueForCallBack` 明确是低优先级后台线程、双数组交换、容量溢出记录、缓存关闭时同步直 dispatch；队列项分 `VmIO` 值变化、流程工作状态和模块结果三类。`ShutDownCallBackQueue` 仅停止线程指针，不等待或排空。
- `analysis/decompiled/VM.Core/VM.Core/VmModule.cs:3724-3735` 与 `VmProcedure.cs:739-741` 只把结果包装为 `ValueEventArgs` 后交给异步事件；未给出 native 结果内存释放或设备取消合同。

## 与当前 main 的对照

`src/VisionFramework/FrameworkCallbackQueue.cs` 已按上述可证顺序分别提供 `PublishWorkStatus`（先入队后同步）和 `PublishModuleResult`（先同步后入队），并保留双缓冲、容量、溢出、后台线程和关闭语义。`tests/CallbackQueueChecks22` 已覆盖顺序、交换、关闭、溢出和 subscriber fault；重复实现会重计已完成 C02.3，不能作为本轮新增。

## 未闭合范围

仍缺：`IMVS_MODULE_WORK_STAUS`/`IMVS_MODULE_RESULT_INFO_LIST_EX_Data` 的 native producer、`pInfo`/动态内存释放责任、`StopRun` 对设备和回调线程的取消顺序、`SyncWaitor` 计时/重入，以及原生历史载荷的序列化格式。现有托管回调队列和取消令牌不能冒充这些行为。

## 自审核

本轮只读原始反编译文件和当前 main；未修改 `src/`、未修改 `tool-plugins`、未新增测试。报告保留来源、调用顺序和精确未知范围，故 C02.2/C03.2/C04 维持 partial/open，实施总进度不变。
