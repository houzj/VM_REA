# MF-02 框选预览样式与操作验收

原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ItemsPage.cs` 的 `ToolItemGrid_PreviewMouseMove` 创建未填充的 `Path`，边框 `LightSlateGray`、粗细 1、虚线 `[2,1]`。`DiagramPage.xaml.cs` 的 `GetItemsByRect` 只纳入完全包含的节点。当前项目此前使用橙色实线及绿色半透明填充，造成明显的拖选视觉差异。

本次把预览边框改为灰蓝虚线并去掉可见填充。当前用 `Rectangle` 绘制，按缩放比例调整粗细，属于 WPF 独立实现；不宣称原生 `AdornerPanelAttachToWorkArea` 控件树已完整移植。

专项 `dotnet run --project tests/VisionEditor.MarqueeChecks/VisionEditor.MarqueeChecks.csproj`：向真实 WPF 画布派发鼠标按下事件，检查预览边框；在测试中设定框选几何以截图；取消手势后调用框选终点处理，核验完整包含的两个节点、未包含第三个节点；再执行左对齐、撤销和保存重开。9 项通过。`analysis/full-audit/canvas-marquee-native-dash.png` 已目检，虚线框覆盖两个节点且无绿色填充。测试没有向操作系统注入连续鼠标移动，极端 DPI 下的虚线节奏未验。
