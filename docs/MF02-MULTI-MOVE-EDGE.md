# MF-02 多节点拖放的逐节点边界

原始证据：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ItemsPage.cs` 的 `GetDropItems` 对所选 `DiagramItem` 逐项计算落点：每个 X 限于 `[0, 5000-ActualWidth]`，每个 Y 限于 `[0, 5000-ActualHeight]`，并按 5 像素取整。原始 `DiagramPage.xaml.cs` 的 `_dragOver` 先计算拖动偏移，`_dragDrop` 再交给 `GetDropItems`。因此一个节点先碰到边界时，其余选中节点仍可移动。此前本项目对整组应用共同的最大偏移，整组会提前停止。

本次在 `WorkflowCanvas.PreviewNodeMove` 统一逐节点计算预览与提交位置；单节点维持既有路径。组的临时边框按全部预览节点的新外接矩形绘制。极端右边界下，原来位于 X=4550 和 X=4800 的两个宽 110 节点向右拖 200 后分别落在 X=4750 和 X=4890，这是原始逐项限幅逻辑的结果，节点间距可能缩小甚至重叠。

运行 `dotnet run --project tests/VisionEditor.MultiMoveEdgeChecks/VisionEditor.MultiMoveEdgeChecks.csproj`。专项通过 WPF 组边框鼠标事件启动手势，再调用画布拖动预览/提交处理器；核验两个临时节点位置与最终位置一致、一次文档事务、撤销、重做及保存重开。8 项通过。`analysis/full-audit/canvas-multimove-right-edge.png` 已目检：右侧边界处两个预览节点发生重叠，小地图可见视口移向右侧。测试未向系统注入连续鼠标移动；原生拖动临时层在边界处的每一帧视觉效果仍未验证。
