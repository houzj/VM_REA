# 连线中点拖动在可见视口外松开

- 源码确认：原始 `Connection.xaml.cs` 的 `connectionUserControl_MouseMove` 根据 `Page.Width/Height` 裁剪路径拖动，`connectionUserControl_MouseLeftButtonUp` 无条件结束拖动并释放鼠标捕获；没有按当前 `ScrollViewer` 可见视口取消的判断。
- 独立实现：WPF 画布鼠标松开时，对已经捕获的连线中点拖动提交最后预览路径。Esc、失去捕获以及编辑边界失效仍按已有逻辑取消。
- 验收：`dotnet run --project tests/VisionEditor.WireFocusChecks/VisionEditor.WireFocusChecks.csproj -c Release`。真实 WPF 中点柄按下、预览移动，将系统指针移到可见视口外后松开，确认单次路径事务、撤销重做及保存重开。截图 `analysis/full-audit/wire-midpoint-outside-release.png` 已目检，连线路径和箭头持续可见。
- 未验原生：真实 VM 桌面鼠标越过窗口边界后的操作差分，以及失捕事件与鼠标松开同时到达时的系统事件顺序。
