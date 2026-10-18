# 连线命中宽度验收（MF-02）

源码确认：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml` 将 `Connection` 定义为单个 `Shape`，直接处理鼠标、焦点、拖入和 Delete；`Connection.xaml.cs` 的 `SetDefaultStroke` 将该 Shape 线宽设为 2。未发现另一个 12 像素透明命中层。当前 WPF 为适应可见路径与焦点分层，保留透明覆盖 Path，但把其线宽由 12 收敛到 2，避免离中心线较远处误选。这是按源码重建的命中几何，不声称已完成真实 VM 的 DPI 级像素差分。

实际 WPF `VisionEditor.WireFocusChecks --hit-only`：`GraphLayer.InputHitTest` 在线段中心命中连接 Path，法向偏离 7 像素不命中；点击后取得焦点并显示中点柄，菜单删除一次事务，撤销和保存重开恢复连线。`VisionEditor.WireMultiHoverChecks`：多节点拖入 10 像素、拖离恢复 2、再次拖入 10；落下不拆线，撤销/重做/保存重开正常。截图 `analysis/full-audit/wire-midpoint-focus.png` 与 `wire-multinode-hover.png` 已目检。完整 `WireFocusChecks` 的系统指针视口外释放步骤另有已记录的测试焦点竞态，本项用隔离入口验收，未改该路径。
