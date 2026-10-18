# MF-02 画布滚轮事件路由验收

## 原始证据与边界

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml.cs` 中 `OnPreviewMouseWheel` 依次调用 `OnHoriScroll` 和 `OnChangeZoom`。
- `OnHoriScroll` 仅在 Shift 分支设 `e.Handled = true`；`OnChangeZoom` 在 Ctrl 分支推进缩放索引，但不消费事件。两种修饰键并存时两条分支均执行。
- 本项只确认控件事件处理与路由生命周期；真实 VM 外层控件后续如何处理未消费的滚轮事件，尚未逐窗口验证。

## 本地实现和操作验收

`WorkflowCanvas.CanvasWheel` 保留既有缩放锚点和横滚行为，移除 Ctrl 分支的 `Handled` 赋值。独立 WPF 检查在 `tests/VisionEditor.CanvasWheelRoutingChecks`，创建真实窗口，通过 WPF 鼠标预览路由和线程键盘状态依次操作 Ctrl、Shift、Ctrl+Shift、零增量滚轮。它检查 ScrollViewer 收到的事件及其 `Handled` 状态、缩放刻度、横向偏移、文档修订，以及保存重开后的流程与视口。

本轮截图：`analysis/full-audit/canvas-wheel-native-routing.png`（仅测试输出，未加入产品程序集）。目检窗口结构、画布、鹰眼和 100% 缩放显示正常。Ctrl 缩放采用鼠标锚点，水平偏移会随缩放变化，这不等于再次触发横向滚动。

## 尚未验证原生

原生窗口中 ScrollViewer 之后的父级接收方、不同 DPI/鼠标驱动的 Delta 序列，以及真实 VM 文件重开时的视口持久化行为仍待运行取证。此处不宣称完整原生等价。
