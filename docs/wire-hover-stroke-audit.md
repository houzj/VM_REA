# 连线悬停线宽验收（MF-02）

原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml` 的 `Shape` 没有 `MouseEnter`、`MouseLeave` 线宽事件。`Connection.xaml.cs` 的 `Shape_DragEnter` / `Shape_DragLeave`（约 838–865 行）只在节点拖入连接线的插入预览期间将 `StrokeThickness` 从 2 改成 10，拖离恢复 2。这是源码确认的事件边界；原生桌面上的像素级视觉效果尚未独立测量。

当前 WPF 实现移除了普通鼠标悬停时擅自将可见连线加粗到 3 像素的处理。实际 WPF 窗口验收：`VisionEditor.WireFocusChecks --hover-only` 中悬停、离开、点击选中均为 2 像素；`VisionEditor.WireMultiHoverChecks` 中节点拖入为 10 像素、拖离恢复 2、再次拖入为 10，落下后仍只移动多选节点，撤销、重做、保存重开不改变原连线。目检 `analysis/full-audit/wire-midpoint-focus.png` 和 `wire-multinode-hover.png`：前者是细线并显示中点柄，后者是加粗插入预览。

完整 `WireFocusChecks` 曾全部通过；追加离开断言后的另一次运行在与本变更无关的“系统指针移到视口外再释放”检查偶发失败。该检查依赖 `SetCursorPos` / `Mouse.Synchronize` 和窗口焦点，失败发生在前述悬停断言、路径拖动及保存重开全部通过之后。隔离的真实 WPF 悬停复验通过。透明命中区域与原生的精确命中像素仍待真实 VM 差分。
