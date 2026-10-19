# MF-02 连线上的 Ctrl 拖放插入

## 反编译源码调用链

- **拖动入口与载荷**：`D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramItem.xaml.cs`，`DiagramItem_PreviewMouseMove`（约 623–650 行）。当左键拖动成立时，构造 `DataObject` 并将 `new List<DiagramItem> { this }` 放入载荷。代码创建拖动数据时保留的是原 `DiagramItem` 实例。
- **拖放效果**：同一成员后续根据 Ctrl 修饰状态选择 `DragDropEffects.Copy`（约 659–674 行）。Copy 是 OLE 拖放效果；此处没有构造一个新的 `DiagramItem` 或替换载荷里的项。
- **连线投放入口**：`D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml` 将 `DragDrop.Drop` 绑定到 `Shape_Drop`。对应 `Connection.xaml.cs` 的 `Shape_Drop`（约 815–831 行）读取 `List<DiagramItem>`，数量为 1 时通过 `OnInsertItem` 传出 `list[0].DataContext`。此处没有按 `DragEventArgs.Effects` 分支 Copy 与 Move。
- **连线装饰器**：`D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmConnectionDecorator.cs`，`InsertItemCommandExecute`（约 204–218 行）从装饰对象取得已有 `IVmModule`，然后调用 `InsertModule`。`InsertModule`（约 220–245 行）拒绝流程、当前端点模块；否则读取现连接的 `SourceIndex`/`TargetIndex`，先连接目标与拖入模块，再连接拖入模块与源，第二条失败时删除第一条，成功后删除旧连接。
- **行为结论**：连接投放使用数据载荷中的原模块并改接既有图；Ctrl Copy 效果本身不会让目标连线插入克隆节点。拖到自己的端点连接时，`InsertModule` 直接返回，不重接。

## 本地修复

`src/VisionEditor/WorkflowCanvas.cs` 的 `CommitNodeMove` 现在在单节点 Ctrl-copy 手势且命中一条可插入的连线时，保留原节点坐标，以原节点 ID 调用 `doc.InsertExecutionNode`，不再执行 `CopyNodes`/`PasteNodes`。若命中的是包含该节点自身的端点连线，则不改文档并重绘。普通拖动插入、多节点 Ctrl-copy、未命中连线的 Ctrl-copy 路径保持各自既有处理。

## WPF 操作与数据验证

- `tests/VisionEditor.CanvasChecks/Program.cs` 增加了真实 WPF 窗口场景：建立源节点 `(100,100)`、目标节点 `(500,100)` 和被 Ctrl 拖动的原节点 `(800,700)`，源边端口索引为 `2`、目标索引为 `0`；调用画布手势入口和预览处理器，把节点预览放到连线附近，然后提交手势。
- 检查连线命中识别为原连接；提交后节点仍为 3 个，原节点坐标未移动，拓扑为源→原节点→目标，两条边端口索引均保持 `2/0`，文档只新增一个撤销事务。Undo 恢复原边与原坐标；Redo 后保存并重开仍保持分割拓扑。
- 命令：`dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj -c Release`。结果：全套 CanvasChecks 均通过，包括小地图路由、wire focus/Delete、画布边界 Pan/Zoom、超大画布连线和本次 Ctrl-copy 插入/撤销/重做/保存重开检查。
- 截图 `analysis/full-audit/canvas-wire-copy-insertion-preview.png` 已目检：原节点以橙色预览显示在被高亮的水平连线上，节点数量与端点连接可辨；截图记录提交前预览态。截图辅助逻辑只存在测试项目。

## 证据边界

- **源码确认**：拖动数据包含原 `DiagramItem`、Ctrl 对应 OLE Copy 效果、连接的 Drop handler 只看单项载荷并传递原 `DataContext`、`InsertModule` 的端点拒绝/端口索引复用/连接替换与失败回滚逻辑。
- **本地独立实现**：WPF 测试通过调用画布的 `BeginNodeGesture`、`PreviewNodeMove`、`CommitNodeMove` 完成操作，业务文档用 `ExecutionLinkDocument` 表达两段连线；没有调用原 VM 的 `DragDrop.DoDragDrop` 或 `IVmModule`。
- **未确认**：真实 VM 在物理鼠标 Ctrl-OLE 拖放中的跨窗口/拖放源捕获细节未运行复验；反编译显示连接端点数据模型与本地 `ExecutionLinkDocument` 并不相同，除已检查的端口索引、端点拒绝及拓扑替换外，不据此声称所有连接元数据完全一致。截图来自本地 WPF 窗口，不作为原生外观证据。
