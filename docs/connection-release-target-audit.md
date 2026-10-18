# 连线释放目标生命周期（MF-02）

源码确认：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connector.xaml.cs` 中 `AttachedDiagramItem_PreviewMouseMove` 通过 `HitTesting` 更新 `oldItem`，并高亮最近端口；`ToolItemGrid_PreviewMouseUp`（约 90–106 行）仅在 `oldItem != null` 时连接，释放位置只供 `GetNearestConnectorIndex` 计算目标端口。释放事件不会重新寻找目标节点。

当前 WPF `FinishConnection` 消费预览期间记录的 `connectionTarget` 节点；释放坐标仅重新计算该节点的最近端口。专项 `VisionEditor.ConnectionPreviewChecks` 在实际 WPF 窗口用起点端口鼠标事件启动，再检查：未经目标预览直接释放不建连；再次启动并预览目标后才建立一条连接；预览终点仍在指针处，释放位置决定最近端口；单次事务、撤销、重做及保存重开均通过。`analysis/full-audit/connection-pointer-preview.png` 已目检，虚线终点未提前吸附，目标端口高亮。

本专项的释放动作调用产品 `FinishConnection`，起点按下使用 WPF 路由事件；尚未用真实 VM 桌面鼠标对极快移动/遗漏 MouseMove 的时序做差分。独立 WPF 实现仍保留 `CanvasPointerUp` 的视口边界处理，视口外捕获释放与原生完整事件序列未在此项验收。
