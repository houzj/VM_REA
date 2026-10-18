# MF-02 多选垂直居中相对坐标取整

原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/RectDragPanal.xaml.cs` 的 `menuAlgClick` 在 `VerticalCenterAlg` 分支先计算 `(ActualHeight - item.ActualHeight) / 2 / 5`，按 5 像素取整后才加 `Canvas.Top(this)`。本地此前对 `area.Top + area.Height/2 - nodeHeight/2` 的绝对坐标整体取整；当选区顶部为 93 等非5网格位置时产生 3 像素偏差。

现在 `AlignSelection("centerY")` 先按每个节点的实际高度取整相对偏移，再加选区绝对顶部。独立 WPF 检查 `tests/VisionEditor.CanvasAlignChecks` 在实际窗口框选两个30px图像源和一个40px Group，操作多选菜单的“对齐 → 垂直居中”。选区顶部93、普通节点结果178、Group结果173；菜单只产生一个文档事务，撤销恢复原始非网格位置，重做和保存重开均保留结果。截图 `analysis/full-audit/canvas-align-native-relative-y.png` 已目检多选框与三节点相对排列。截图代码仅在测试程序。

其他对齐与分布选项及真实 VM 同屏像素差分未由本项覆盖；本轮仅关闭相对 Y 取整的源码确认差异。
