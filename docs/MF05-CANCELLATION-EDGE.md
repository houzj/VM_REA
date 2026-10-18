# MF-05 执行取消边界（2026-10-18）

原始托管代码 `analysis/decompiled/VM.Core/VM.Core/VmProcedure.cs` 在禁用流程时调用 `ProcessManager.StopExecute(ID)`；这仅确认停止命令会下发 SDK，不能从托管层推出原生空流程的返回状态或设备回收语义。

当前独立执行器的漏洞是：取消令牌在开始前已经取消，而流程没有节点、或节点全部禁用时，没有任何节点产生 `Cancelled` 记录，汇总状态误报 `Success`。现在最终汇总同时检查流程取消令牌和执行作用域停止令牌，返回 `Cancelled`；不会因此启动模块。已在执行专项验证空流程、禁用节点及 `SolutionExecutor` 的完整方案调用，合计 71 项通过。

这是独立托管执行状态的一致性修复。VM 原生 `StopExecute` 对空流程、已完成模块、设备回调和竞争时序的状态码仍需原生后端或实机差分；本项不宣称原生调度等价。
