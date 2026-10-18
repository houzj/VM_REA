# MF-02 总览卡片展开的视口交接验收

## 原始证据与边界

- 原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF/ProcedureConfigControl.xaml` 第 523、538、552 行：流程 Tab 内容中的 `DiagramArea` 与全部流程的 `DiagramAreaControl` 是两个不同的 WPF 控件实例。其各自的 `ZoomArea` 提供独立滚动和缩放状态。这确认了总览与流程画布不是同一视口。
- 本项目用一个 `GraphLayer` 和 `canvasViews` 字典实现视口交换，是独立实现，并非反编译的原控件树。过去总览卡片展开会在保存总览视口前置 `workflowOverview=false`，随后 `SelectWorkflow` 首次渲染把总览的比例/偏移当成目标流程视口；最后才把 `canvasProcedure` 置空，已经太迟。
- 修复为先保存总览视口，进入流程前清空当前画布身份，令首次渲染直接加载目标流程保存的视口。解决方案文件仍只存流程数据，不保存 UI 视口；原生跨关闭重开时是否保持缩放/滚动没有足够证据，不宣称等价。

## 实际 WPF 操作验收

运行 `dotnet run --project tests/VisionEditor.OverviewViewportChecks/VisionEditor.OverviewViewportChecks.csproj`。专项创建两个流程，分别设为 140% 和 170%；真实 WPF 按钮进入总览并设为 60%；触发第二流程卡片的展开按钮，验证恢复 170%；重新进入总览验证仍为 60%；再切回第一流程验证 140%。保存并重新加载，确认两流程数据仍完整。测试结果 7 项通过。

测试专用截图为 `analysis/full-audit/canvas-overview-viewport-return.png`（不纳入产品程序集）；目检显示第一流程 Tab、140% 状态栏与小地图位置相符。该验收不证明全部 VM 样式一致，亦未测多屏 DPI 和窗口尺寸变化。
