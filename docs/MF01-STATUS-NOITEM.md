# MF-01 状态栏空组件状态验收

原始证据：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/StatusBar.xaml` 的 `NOItem` 触发器选择 `bottombar_config.png` 和“等待添加组件”；`VmModuleSetDecorator.WorkState` 在 `IsReady == null` 时返回 `NOItem`。本项目没有 SDK 的 `IsReady`，只将**当前无流程或流程无节点**映射到该状态。有节点时使用现有 `GraphValidator` 选择绿色或异常图标；这是独立适配，未确认与原机 `IsReady` 等价。

实际 WPF 验收：经“文件→新建方案”，空画布底栏显示配置图标与等待文字；添加流程及图像源节点后，退出空状态，底栏显示节点操作反馈。专项检查断言真实窗口资源与文字，截图为 `analysis/full-audit/shell-status-no-item.png`、`shell-status-with-node.png`；截图辅助代码仅在测试项目。

开放差异：原生 `WaitConfig/Ready` 判定依赖 SDK，尚未进行同状态原机差分。截图还暴露新建方案后旧历史结果与流程耗时残留；数据写入在 `WorkflowExecution.cs`，`ToolExecutionScope.ReplaceExecutionScope` 只清理会话与结果，未清 `history`/耗时文本。这项跨流程运行生命周期，不并入本次 MF-01 状态图标提交，交主框架运行线核对原生清理时机。
