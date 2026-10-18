# MF-02 鹰眼鼠标按钮事件验收

原始反编译 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml` 第 26、40 行分别给鹰眼容器和视口矩形绑定 `MouseDown`，容器同时绑定 `MouseUp`；`ZoomArea.xaml.cs` 的 `GridEagleEye_MouseDown` 不筛选按键，而 `GridEagleEye_MouseMove` 只在左键按下时拖动。由此可确认：视口矩形外的右键或中键点按也会触发跳转，但只有左键可拖动。

本项目原本只绑定 `MouseLeftButtonDown`/`MouseLeftButtonUp`。现在改为 `MouseDown`/`MouseUp`，仍由现有导航和拖动逻辑处理。`MainWindow.xaml` 中鹰眼与画布是 `CanvasHost` 下的兄弟元素，因此此前提出的父画布预览事件会抢鹰眼事件的推断不成立，也没有为此修改代码。

专项 `dotnet run --project tests/VisionEditor.MiniMapButtonsChecks/VisionEditor.MiniMapButtonsChecks.csproj` 向实际 WPF 鹰眼控件派发右键/左键路由事件，验证进入和退出手势、点击矩形外后的滚动偏移、拖动后视口、文档修订号不变和保存重开。7 项通过。测试截图 `analysis/full-audit/canvas-minimap-all-buttons.png` 已目检，橙色视口框位于小地图右下部，与滚动后的空画布位置一致。真实硬件中键、不同 DPI 与 VM 原生运行时的像素级视觉差异尚未验证。
