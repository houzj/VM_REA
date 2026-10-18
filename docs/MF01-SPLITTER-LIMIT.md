# MF-01 主工作区分隔条边界

原始证据：`analysis/decompiled/VMControls.WPF/VMControls.WPF/MainViewControl.xaml` 中工作区左右列为 `*`、`Auto`、`1.0000001* MinWidth=400`，中间 `GridSplitter` 宽 5；`MainViewControl.xaml.cs:435-447` 在拖动开始时依布局方向设置一侧 `MaxWidth = workarea.ActualWidth - 65`。当前 WPF 固定左画布/右结果布局，对应原代码 `IsRightLayout == false` 分支，因此只给右结果列设置该上限。窗口最小宽 800、工具箱展开后工作区仍大于 465；实现另以右列原有 `MinWidth=400` 兜底，避免窄窗出现负或矛盾上限。

真实 WPF 专项以窗口 900px 宽对分隔条发送 `DragStarted/DragDelta` 路由事件，验证右列受限、左画布约保留 60px、拖回后左画布恢复；再以窗口最小宽 800px 且展开工具箱重启拖动，验证上限重算。`analysis/full-audit/shell-splitter-limit.png` 是测试项目导出的边界截图并已目检。

未验证：真实 VM 在不同 DPI、多屏与 `IsRightLayout == true` 下的拖动视觉差分；本项目目前只有左画布/右结果方向，未实现反向布局。
