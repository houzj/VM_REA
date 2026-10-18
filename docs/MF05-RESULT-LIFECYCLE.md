# MF-05：方案替换后的结果视图生命周期

日期：2026-10-18。本文记录新建/加载方案与模块结果视图之间能由托管反编译源码确认的边界；不把 native SDK 内部状态推断成已知行为。

## 原生证据

- 新建方案入口为 `VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs::CreateNewSolution`，完成条件检查和用户保存决策后执行通信关闭、`FrontendWindowData.CreateNewDesign()`、`ResetResource()`、`ProjectManager.CreateNewSolution()`，再启动全局脚本。
- `VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs::CreateNewSolution` 注册 `SolutionCreate` 撤销操作、清空 VMControls 的剪贴板信息，并调用 `IVmSolution.CreatNewInstance()`。该托管方法体没有显式清空主窗口的执行耗时或结果历史控件。
- 方案加载开始回调 `VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs::VmSolution_LoadBegin` 关闭通信并调用 `TitleMenueItemData.ResetResource()`；加载结束处理最近方案和方案路径/名称通知。当前托管方法体没有显式清空主窗口的执行耗时或结果历史控件。
- 模块结果面板 `VMControls.WPF/VMControls.WPF/ModuleResultView.xaml.cs::ModuleSourceChanged` 将模块源切换委托给 `DoSourceChanged`。后者先释放旧的 `ModuleDisplayDecorator`，调用 `ClearDataStore()`，再为新模块创建并初始化新的 decorator；模块源为空时置空当前 decorator 并调用 `UpdateModuleView()`。
- 模块历史的显式清除入口是 `ModuleResultView::Menu_Clear_Click`：清空当前 decorator 的 `ModuleDisplayInfoList`，再调用 SDK `ClearModuleHistoryResult(ModuleSource.ID)`。SDK 函数体不在当前托管反编译证据中。

## 当前实现差异与验收状态

- 主线 `VisionEditor` 的 `ReplaceExecutionScope` 会取消当前运行，清理结果/会话字典并替换执行作用域；但当前 `history` 集合和状态栏流程/模块耗时没有在该方法中归零。`UpdateShellSelection` 在没有选中节点结果时也不会写入默认时间。
- 当前 `HistoryGrid` 是方案级的独立运行记录（序号、时间、流程名和状态），而原生截图/`ModuleResultView` 的历史结果附着于当前模块 decorator。二者粒度不同，不能把方案级运行日志冒称为原生模块历史。
- 原生方案新建/加载入口没有显式清理耗时/历史控件的托管证据；模块源变化会重建模块 decorator，但当前未通过真实 VM 操作确认方案切换是否总会导致面板解绑、其历史呈现及底部计时行为。
- 因此本轮只确认了作用域字典会被清理和模块结果视图的源码生命周期；方案级 UI 记录在方案替换时是否清空、底部时间如何复位，仍是未验证差异。本文件不据此授权或声称 UI 行为与 VM 等价。

## 需要的验收

在真实 VM 中先运行一个有结果的方案，再分别执行“新建方案”和“打开另一方案”，观察右侧模块历史、图像/模块结果绑定、底部耗时、工作流状态；另做模块切换和手动清除作为对照。记录每个动作前后截图与数据行数。之后再决定主线应保留跨方案运行日志，还是将它限制在当前方案会话中。
