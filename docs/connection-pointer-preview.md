# 连线拖动预览终点与最终端口

- 源码确认：原始 `Connector.xaml.cs` 的 `AttachedDiagramItem_PreviewMouseMove` 总是调用 `PathFinder.GetConnectionLine(DragStartItem, 当前指针, Index)` 绘制虚线；`HitTesting` 与 `SetNearestConnector` 只负责目标节点和最近端口的悬停高亮。`ToolItemGrid_PreviewMouseUp` 才以最近端口索引调用 `Connect`。
- 独立实现：连线预览始终把实际指针作为终点，不在进入目标节点时提前切换成提交后的完整自动路由；鼠标松开仍按最近端口创建连线。
- WPF 验收：`dotnet run --project tests/VisionEditor.ConnectionPreviewChecks/VisionEditor.ConnectionPreviewChecks.csproj -c Release`。真实 WPF 起点端口按下、目标节点内预览终点与指针一致、端口高亮、预览文档不变、松开端口索引正确、一次事务、撤销重做、保存重开均通过。截图 `analysis/full-audit/connection-pointer-preview.png` 已目检虚线跟到节点内部指针。
- 未验原生：真实 VM 桌面同状态鼠标移动/像素差分、复杂节点模板中的端口几何。
