# Group 独立运行停止与失败恢复

原始托管证据：`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/BottomCommand.cs` 的 `StopSelfExcuteModule` 只把 `IVmSolution.IsContinuousSelfExecute` 设为 `false`；`DynamicParamsTabControl.xaml.cs` 将“停止运行”绑定到此方法。`VM.Core/VmSolution.cs` 在自运行结果回调时清除模块及流程运行标志。由此可确认按钮停止连续后续运行，不通过该命令取消正在执行的单轮；原生底层设备取消及所有异步回调时序仍未验证。

重建窗口的“停止运行”现只撤销连续运行请求。窗口关闭保留独立取消与等待清理策略；这属于本项目资源生命周期防护，不宣称与原生关闭行为完全一致。

专项运行：`dotnet run --project tests/VisionEditor.GroupStopRecoveryChecks/VisionEditor.GroupStopRecoveryChecks.csproj`。真实 WPF Group 窗口中，受控慢工具进入连续运行后点击停止，当前轮完成一次且产生输出，后续轮不再启动；接着单次运行取得新的实际 Group 输出。注入一次失败后窗口退出忙状态、显示错误，下一次单次运行成功并清除错误。单次运行中点击停止不取消当前轮；运行中关闭窗口取消并等待清理。共 9 项通过。受控工具仅用于验证框架行为，不代表原生插件算法。

后续结果显示复核：原始 `BottomCommand.SelfExcuteModule` 与 `ContinuousSelfExcute` 均在调用 `VmModule.Run()` 前逐个执行 `[Display]` 项的 `ClearValueStore()`。重建窗口现在启动 Group 运行后立即刷新结果区，避免慢工具执行期间继续显示上一轮结果；完成后再次刷新真实输出。专项在已有成功结果后挂起下一轮，验证 WPF 结果表在运行中清空、完成后恢复新结果；失败后重试与停止语义仍通过，共 10 项。原生每个显示项的缓存清理和回调时间点仍未做 VM 运行差分。
