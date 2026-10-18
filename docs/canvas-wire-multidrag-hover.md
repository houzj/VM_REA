# 连线拖入多节点预览边界

- 原始依据：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml.cs` 的 `Shape_DragEnter` 对任意 `List<DiagramItem>` 调用 `SetBlodStroke`（10）；`Shape_Drop` 只有 `Count == 1` 才触发 `OnInsertItem`。`VmConnectionDecorator.InsertModule` 保留源/目标端口索引；当前插入事务已经与此一致。
- 独立实现：`WorkflowCanvas.PreviewNodeMove` 对多选拖动及复制拖动也显示连线加粗预览。`CommitNodeMove` 仍只允许单节点、非复制拖动拆线插入；多节点落线只作为一个移动事务。
- 验收：独立真实 WPF 窗口检查框选两节点、拖过连线、10px 加粗和截图目检、落下后原连线保留、撤销/重做、保存重开。运行 `dotnet run --project tests/VisionEditor.WireMultiHoverChecks/VisionEditor.WireMultiHoverChecks.csproj -c Release`，截图输出 `analysis/full-audit/wire-multinode-hover.png`。
- 未验原生：没有在真实 VM 桌面窗口做同状态鼠标拖放与像素差分；当前预览命中仍走重建画布的几何检测。
