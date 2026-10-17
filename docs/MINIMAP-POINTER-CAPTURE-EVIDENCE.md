# 小地图指针捕获边界

证据：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml.cs` 的 `GridEagleEye_MouseDown` 只设置 `isMouseDown`、计算点击后的选择区域并记录起点。`GridEagleEye_MouseMove` 在左键按下、已进入拖动且位置发生变化时才调用 `CaptureMouse`，随后移动选择区域；`GridEagleEye_MouseUp` 释放捕获。原始 XAML `ZoomArea.xaml` 将这些事件绑定到 `GridEagleEye`。

本线修复 `src/VisionEditor/WorkflowCanvas.cs` 的 WPF 小地图：按下时不抢占鼠标捕获；只有指针位移并保持左键按下时才捕获和导航。`tests/MinimapCaptureChecks` 在实际 WPF 窗口中调用按下事件，验证点击已处理但鼠标未被提前捕获。

**证据范围**：捕获时机属于反编译源码确认行为；鼠标桌面硬件拖动、不同 DPI 和滚动坐标等同原生尚未差分验证。本次未触及画布路由算法，也不据此声明 F-09 全面闭合。
