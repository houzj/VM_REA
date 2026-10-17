# 流程等待超时：原始证据与当前实现

更新：2026-10-17。本项仍未达到原生运行语义等价。

## 原始证据

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmProcedureDecorator.cs`：`SetTimeOutCommandExe`从流程菜单打开`TimeOutSet((DecorateSource as IVmModule).ID)`。
- `analysis/decompiled/Apps.XmlUI/Apps.XmlUI.ParamTab/TimeOutSet.xaml`：370×200窗口；“流程等待超时(ms)”数值控件，范围0—100000，步进10；底部确认/取消。
- 同目录`TimeOutSet.xaml.cs`：加载时调用`ProcessManager.GetProcedureWaitTime(ProcessID)`；确认时调用`SetProcedureWaitTime(ProcessID,(uint)timeSpan.StartValue)`。XAML初值80会被成功读取的服务端值覆盖。读取失败会显示错误，因此不能据此断言新流程默认80。
- `analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ProcessManager.cs`和`ServerSDKAPI.cs`：托管层仅将读写转发给原生SDK。当前证据未揭示原生调度器如何超时、如何中断设备模块或如何生成状态码。

## 当前独立实现

- `ProcedureDocument.WaitTimeMs`保存0—100000毫秒；旧独立方案缺字段时为0，保持既有行为。编辑器菜单打开WPF数值窗口，确认提交、取消不改动；方案保存、重新打开及撤销沿用文档事务。
- `SolutionExecutor`为每次流程调用创建独立等待计时；非零值到期后取消协作式模块，等待模块释放资源，再返回`TimedOut`。这是一种明确标注的**独立托管行为**，不是已反编译出的原生实现。用户停止仍通过外部取消令牌处理。0表示本实现不施加流程级时限。
- 测试验证：慢模块因20毫秒流程时限返回`TimedOut`并完成`DisposeAsync`；0值正常运行；实际WPF菜单输入35毫秒并保存重开。既有Group运行用例重置为0后复验，防止测试设置污染后续操作。

## 未关闭

仍需分析原生`IMVS_SetProcedureWaitTime`的后端实现，或在真实VM上按可复现实验记录零值、到期、运行中设备/Group、状态码和重入行为。当前WPF窗口使用共享数值控件复刻几何，原生`SliderArrange`的全部焦点/键鼠交互及像素差分尚未验收；不得标记完整原生等价。
