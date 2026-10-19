# MF-05：连续运行与停止调用链

日期：2026-10-19。此记录只陈述托管反编译可见链路；底层 process manager 与 SDK 的循环调度仍属 native 未知。

## 原生调用链

- `VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs::SigRun` 先调用 `CheckRun()`；无启用流程或启用流程状态非 Ready 时返回/显示告警，否则调用 `VmSolution.Instance.Run()` 并记一次运行日志。
- 同文件 `MutiRun` 使用同一 `CheckRun`，再经 `MutiOrStopRun(true)` 设置 `((IVMRun)VmSolution.Instance).ContinuousRunEnable`；`StopRun` 经相同 setter 设置 `false`。连续启动/停止的 UI 入口映射位于 `TitleMenueItemData.cs` 约 1805–1813 行。
- `VM.Core/VM.Core/VmSolution.cs::ContinuousRunEnable` 的 getter 是全局脚本连续运行、任一流程连续运行和 self-execute 标志的并集。setter 在 `false` 时停止 global script、清除 self-execute，并逐一令流程 `ContinuousRunEnable=false`；setter 为 `true` 时走全局脚本连续运行入口。
- `VmProcedure.cs::ContinuousRunEnable` 对 `true` 调用 `mProcessManager.ContinousRunStart(ID)`；对 `false` 调用 `RefreshUIStopExecute(ID)`，再更新属性与父级通知。`IsForbidden=true` 另调用 `StopExecute(ID)`。native SDK 调度循环及这些 API 内部实现不可见，不能等同为托管 while-loop。
- process callback `VmSolution.MProcessManager_OnModuleStopRunProcessStatusCallBack` 在 `nStopAction` 为 1/2/3 时将流程连续状态更新为 false，并排入 wait-notify 状态；全局 stop-status 表也在该回调中被更新。`MProcessManager_OnProcessStartContinuousStatusCallBack` 与 `MSolutionManager_OnModuleContinousRunStatusCallBack` 更新对应流程/全局显示状态。
- `VmProcedure.IsProcessRunStop` 将 `GetProcRunParam` 返回值等于 `1` 映射为 true；setter 通过 `ProcessManager.SetProcRunParam(ID, value ? 1u : 0u)` 转到 `ServerSDKAPI.IMVS_SetProcRunParam`。因此托管层确认配置值 `1` 是流程 NG 停止开关；停止回调中的 `2/3` 语义没有对应托管说明，不作命名推断。
- `VmSolution.ContinuousRunStatus` 仅声明 `EndOnce=0`、`StartOnce=1`；`MProcessManager_OnWorkStatusCallBack` 将 `workStatusInfo.nWorkStatus` 强转后派发 `OnContinuousRunStatusCallBack`。SDK 对其它状态值的定义和顺序未在托管源中给出。
- `MProcessManager_OnWorkStatusCallBack` 在 `nWorkStatus==0` 时设置执行计数和流程耗时、开放 SelfExecute 并发出 end 回调；其他状态发 begin 回调。若 `isClearLastExecuteResult` 且执行计数大于 1，begin 分支先对该流程下全部模块的叶输出调用 `SetNoneValueWithoutRaiseEvent()` 并置 `IsEmpty=true`。之后每轮都会调用 `OnModuleResultCallBack`；连续运行中则跳过单次运行所用的 `_waitNotifyPages` 更新。
- `Program.cs`（`VisionMaster.Exe.Audit/VisionMaster/Program.cs:224`）从 `SystemConfigManager.IsClearLastExecuteResult` 设置 `IVmSolution.IsClearLastExecuteResult`。`VmSolution` 初始化（`VM.Core/VM.Core/VmSolution.cs:1244`）先把核心字段设为 `false`；启动配置可能覆盖它。该值因此会影响连续运行每轮开始前的输出可见状态，而不是仅影响面板历史行。
- 当 `isClearLastExecuteResult && nExecuteCount > 1` 时，`VmSolution.MProcessManager_OnWorkStatusCallBack`（`VmSolution.cs:4115-4126`）在 work-begin 状态清理 root 流程全部模块 `Outputs.leafIOsWithoutTimeStamp` 的叶值，并将模块 `IsEmpty=true`。`VmModule.RefreshResult`（`VM.Core/VM.Core/VmModule.cs:2602-2615`）在该开关为 false 且查询结果非空时选取查询列表首项 `nExecuteCount`；`ModuleResultView.UpdateModuleViewResultShow`（`VMControls.WPF/VMControls.WPF/ModuleResultView.xaml.cs:1353`）按开关选择当前 `ExecuteCount` 或 `uint.MaxValue` 读取显示对象。
- `VM.Core/VM.Core/VmIO.cs::SetNoneValueWithoutRaiseEvent` 只在 `_value != null` 时把 `_value` 置空，不发 ValueChanged；调用方随后显式把模块设为 `IsEmpty=true`。`VmIO.GetValue(excuteCount,index,...)` 对非 `uint.MaxValue` 的过期执行序号（不匹配自身、关联 IO 或常量）会把 `Value=null` 后返回 null。`VmIO.Value` getter 在非回调阶段发现 root 执行序号落后时会先触发 `Root.RefreshResult()`。
- `Apps.XmlParser.CapabilityAudit/Apps.XmlParser/ObjectBase.cs::GetImageValue/GetValueByType` 按请求的执行序号读取图像及字段；没有值时部分基本字段返回类型默认值。右侧视图传 `uint.MaxValue` 表示请求该对象当前可读值。SDK 查询返回内容及它如何保留历史执行值仍属于 native 边界。

## 与当前 main 的界线

- 当前本地 `StartContinuous/ContinuousRun` 是协作式托管循环：逐次调用 `RunCore`，按流程间隔等待；失败且 StopOnNg 开启时停止；停止按钮取消 `CancellationTokenSource`。`RunCore` 在每轮开始都会调用 `ClearWorkflowCallResults` 并清除根流程 `results`，没有对照 VM 的 `IsClearLastExecuteResult` 设置；所以本地是无条件清除缓存，VM 则有配置分支。这是已确认的托管行为差异，暂未擅自引入对应设置项，因为设置 UI / 配置缺省写入链尚未找到。
- 这与 native `ContinousRunStart/RefreshUIStopExecute` 的真实设备调度属于不同执行器。源代码只确认其入口、状态回调、输出清空和通知策略，不能据此声称 native 与 managed 的时间、线程、停止延迟或回调顺序一致。
- 本轮没有改运行算法：已有主线运行检查覆盖连续运行/停止和 Group 状态。本记录补充 native 状态/数据链；VM 输出清除配置目前没有作为本地行为复刻，必须先有产品配置合同和 VM 对照证据再决定。

## 验证与未闭合项

- 静态源核对覆盖 `SigRun/MutiRun/StopRun`、`VmSolution.ContinuousRunEnable`、`VmProcedure.ContinuousRunEnable`、process status callbacks 与 `isClearLastExecuteResult` 的值流。
- 该源码链不是 VM 原生引擎运行验收；SDK 内部调度、nStopAction 状态码完整定义及异常/设备取消时序仍未知。
- 下一步：把 native begin/end 状态回调与当前 `RunCore/ContinuousRun/FinishWorkflowRun` 用操作日志做数据级差分；没有真实 VM 观测时继续标为未验证，不推断等价。
