# MF-02 连线中点柄焦点生命周期

原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml.cs` 中，`Connection.OnPreviewMouseDown` 先 `Focus()` 再执行 `OnIsSelectedChanged()`；后者仅在 `IsFocused` 时根据清理后的路径生成 `MidPoints`，失焦时清空。`connectionUserControl_LostFocus` 再次调用该方法。这确认中点柄是连线焦点的临时交互层，而非只由持久化选择标记决定。

本地 `DrawWireHandles` 改为检查当前连线命中层的键盘焦点，鼠标拖动进行中保留预览柄；命中层取得或失去焦点时立即重算。原有画布失焦清理仍保留，未改变连线文档模型。

独立 `tests/VisionEditor.WireFocusChecks` 在实际 WPF 窗口中创建两节点和执行连线，点击选线见到中点柄，转焦工具栏后立即清空，再次点击连线重现。随后从实际中点柄按下、移动并以预览鼠标释放提交路径；一次撤销恢复自动路径，重做及保存重开恢复编辑后的坐标。截图 `analysis/full-audit/wire-midpoint-focus.png` 目检了节点、连线、箭头和三个中点柄。截图代码仅在测试程序。

原始所有复杂路径的中点数量、极端交叉布局焦点竞争，以及真实 VM 同屏像素差分尚未完成；本项只关闭已确认的中点柄焦点生命周期及此场景的 WPF 编辑事务。
