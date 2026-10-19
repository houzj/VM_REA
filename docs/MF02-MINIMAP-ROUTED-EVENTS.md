# MF-02 小地图鼠标路由事件消费边界

## 源码追踪

- **原始类型**：`VMControls.WPF.Base.ZoomArea`，`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml.cs`。
- **资源绑定**：`ZoomArea.xaml` 的 `GridEagleEye` 绑定 `MouseDown="GridEagleEye_MouseDown"`、`MouseMove="GridEagleEye_MouseMove"`、`MouseUp="GridEagleEye_MouseUp"`；`RectSelectRegion` 另绑定 `MouseDown="SelectRegionRect_MouseDown"`。这说明原生是普通可冒泡鼠标事件，不是 Preview/Tunnel 事件。
- `SelectRegionRect_MouseDown` 调用 `RecordCurPos(e.GetPosition(GridEagleEye), 当前视口左上角)`；事件继续冒泡到 `GridEagleEye_MouseDown`。后者置 `isMouseDown=true`，并且只有点击当前矩形之外时才把视口移到点击点居中、记录请求位置。方法没有设置 `e.Handled`。
- `GridEagleEye_MouseMove` 只有 `LeftButton == Pressed`、指针确实移动且 `isMouseDown` 时才捕获 Grid 并调用 `MoveSelectRegionByPosChange`。它没有设置 `e.Handled`。
- `GridEagleEye_MouseUp` 复位 `isMouseDown` 并释放 Grid 捕获；没有按键过滤，也没有设置 `e.Handled`。
- **结论**：按键策略是 MouseDown/Up 接受任意按钮，只有左键移动会拖动；三个普通鼠标事件都不被小地图控件消费。这里由源代码与 XAML 直接确认。

## 本地实现与修复

本地 `src/VisionEditor/MainWindow.xaml` 把同类普通事件绑定到 `MiniMapDown/Move/Up`。`WorkflowCanvas.cs` 原先在三个处理器内都将 `e.Handled=true`，使路由行为偏离原生。现已移除三处标记：`MiniMapDown` 仍记录视口/指针起点，`MiniMapMove` 保持“有活动手势且左键仍按下才捕获和移动”，`MiniMapUp` 仍结束手势、释放捕获并刷新视口。此修复只改事件消费状态，不改视口公式、缩放或文档事务。

## WPF 操作验证

- `dotnet run --project tests/MinimapCaptureChecks/MinimapCaptureChecks.csproj -c Release`：真实 WPF `Canvas.RaiseEvent` 路由 `MouseDown`、`MouseMove`、`MouseUp`，检查按下启动手势但不抢先捕获，三个事件均保持未处理，释放结束手势。通过。
- `dotnet run --project tests/VisionEditor.MiniMapButtonsChecks/VisionEditor.MiniMapButtonsChecks.csproj -c Release`：右键按下进入/释放退出、左键入口、点击矩形外改变 ScrollViewer 两轴偏移、拖动继续导航、不修改工作流撤销版本、保存重开保持流程结构；7 项通过。
- 对测试生成的 `analysis/full-audit/canvas-minimap-all-buttons.png` 做了目检：WPF 主窗口结构正常，小地图和视口框可见，框位置与操作后的空画布视口状态吻合。截图辅助代码只在测试程序集。

## 实现边界与未验证项

- **源码确认**：事件种类/冒泡绑定、任意按键开始与结束手势、仅左键移动时导航、延迟捕获、ReleaseMouseCapture、原生处理器不置 Handled。
- **独立实现**：当前 WPF 小地图控件树、固定 187×150 映射、由 `ScrollViewer` 表示视口和测试操作夹具。
- **未知**：真实 VM 的父级是否另有订阅并观察这些未处理事件、真实硬件跨窗口捕获顺序、中键设备行为、DPI/多屏以及原生与重建 UI 的视觉差分。测试的路由检查来自真实 WPF 控件事件，未使用物理鼠标注入，也不证明 VM 全域等价。
