# 工作流 Tab 选中后的横向滚动

原始证据：`analysis/decompiled/VMControls.WPF/VMControls.WPF/ProcedureConfigControl.xaml.cs` 的 `m_ProcessTabControl_SelectionChanged` 与 `Sv_ScrollChanged`；同目录 `ProcedureConfigControl.xaml` 将 `scroll` 指定为 TabControl 模板中的 ScrollViewer，并连接 `ScrollChanged` 事件。

源码确认：选择变化时原控件先聚焦选中的 Tab，再清除键盘焦点。后续滚动事件只检查已进入视口的 Tab 左边界；其左边或右边距离视口边缘小于 `MinExWidth = 30` 时，横向滚动固定的 30 单位。完全处于视口外的 Tab 不走这段边界补滚，其首次进入视口依赖 WPF 聚焦行为。

本项目：`src/VisionEditor/WorkflowTabs.cs` 已让用户切换的 Tab 聚焦并清除键盘焦点，程序化新增/选择的新 Tab 则在布局刷新后定位并聚焦一次；边界补滚改成原始源码的可视区门限与固定 30 单位。当前外层 ScrollViewer 不会仅凭焦点把完全离屏的 Tab 带入视口，因此程序化路径使用 `BringIntoView`，选中首 Tab 时将外层横向偏移归零。这两处均为宿主适配，不是反编译原样代码，也未声称完全等同原生的 `DelayAction`/`ScrollChanged` 时序。

待验证：多个超出视口的流程标签连续切换、滚动后再次选择、窗口宽度改变后的焦点滚入位置；真实 VM 与重建程序需同屏幕尺寸和 DPI 差分。原始源码确认的是事件规则，运行时像素结果尚未得到 VM 差分确认。
