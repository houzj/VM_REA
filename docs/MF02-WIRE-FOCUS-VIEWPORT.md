# MF-02 连线焦点与视口边界

原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml.cs` 的 `OnPreviewMouseDown`（约 342–350 行）将焦点给连线并显示中点；`Connection_RequestBringIntoView`（约 333–340 行）显式处理自动入视请求，以免获得焦点时滚动条移动；`connectionUserControl_LostFocus`（约 383–385 行）失焦后清理中点。连接点删除后的底层图模型事务属于本项目独立实现，不将其视为原厂源码。

重建版先前点击透明命中 Path 后将焦点给整个 5000×5000 的 GraphLayer，并立即重建全部画布。现在命中 Path 自身可获得焦点，按原生意图阻止该连线的 `RequestBringIntoView`，在原元素上刷新选择/手柄而无需点击时销毁重建。失焦沿现有画布焦点清理路径去掉手柄，按 Delete 则通过 `GraphEditorDocument.DisconnectExecution` 一次事务删除，Undo 可恢复。

`VisionEditor.CanvasChecks` 在真实 WPF 窗口中把画布水平/垂直滚动到非零位置，向连线发左键 routed event，检查焦点、中点和滚动偏移；转移焦点后检查手柄消失，重新点击再发键盘 Delete routed event，检查只删连线、一笔撤销、Undo 恢复及保存重开。测试专用截图 `analysis/full-audit/canvas-wire-focus-no-scroll.png` 位于本工作树忽略目录；目检可见远处两个节点与连线保持在当前视口。真实 VM 桌面鼠标焦点链、多屏 DPI 和全部快捷键状态仍需差分，不宣称完整原生等价。
