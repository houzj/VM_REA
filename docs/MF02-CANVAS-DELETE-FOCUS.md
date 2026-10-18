# MF-02 多选删除焦点边界

## 源码确认

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramItem.xaml.cs` 的 `ListViewItem_KeyDown` 仅在节点键盘事件中执行 `DeleteCommand`，并受工作区锁定、参数窗口和管理员条件约束。
- 同目录 `RectDragPanal.xaml.cs` 的 `RectDragPanalBase_KeyDown` 在多选面板的键盘事件中触发删除、复制、禁用和启用。其 Ctrl+F 是多选禁用，`ItemsPage.KeyVdown` 的 Ctrl+F 搜索属于另一焦点上下文；不能将两者混为一个全局快捷键。
- 原始 `ZoomArea.xaml.cs` 在 `GridContent.MouseLeftButtonDown` 中记录 Ctrl 平移起点，但 `DiagramItem.DiagramItem_PreviewMouseMove` 对 Ctrl 拖动又明确进入 OLE Copy。两者竞争的真实 VM 最终手势效果仍需运行差分，故本轮未改 Ctrl 拖节点逻辑。

## 修改与 WPF 验收

`WorkflowCanvas.CanvasKeyDown` 现在只在 `GraphLayer.IsKeyboardFocusWithin` 时处理 Delete，移除以画布鼠标悬停作为删除许可的条件。独立 `tests/VisionEditor.CanvasDeleteFocusChecks` 在实际 WPF 窗口使用框选入口选中两个图像源节点，向窗口发送真实鼠标位置消息以确认画布悬停，同时让工作流新增按钮持有键盘焦点；Delete 不改节点或文档修订。输入框持焦点时也不被画布命令截获。画布取焦点后 Delete 一次删除两节点，一次撤销恢复、重做删除，撤销后保存重开仍有两个节点。

截图 `analysis/full-audit/canvas-delete-focus.png` 为测试程序输出，目检两节点和多选虚线框均可见。截图导出未进入产品程序集。真实 VM 的同屏鼠标差分、权限角色及参数窗口特殊焦点仍未验收，不据本测试宣称完整等价。
