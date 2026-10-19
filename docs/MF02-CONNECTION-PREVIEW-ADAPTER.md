# MF-02: 连接拖动预览适配核对

## 范围和结论

本轮核对原生 `DiagramPage.GetPath` / `PathFinder.GetConnectionLine` 参数进入拖线预览，再到鼠标释放时文档写回的边界。选择合法的普通节点 `source port 3 (Bottom) -> target port 1 (Top)`；不涉及非法端口、不改变路由算法。

## 原生源码证据

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connector.xaml.cs`, `Connector.AttachedDiagramItem_PreviewMouseMove`: 在 `ItemsPage` 坐标系取得指针位置，调用 `PathFinder.GetConnectionLine(DragStartItem, point, Index)`，将返回点转换到 `ZoomArea`，再更新临时 adorner；随后做 `HitTesting` 和 `SetNearestConnector`。
- 同文件 `Connector.ToolItemGrid_PreviewMouseUp`: 按此前 move 时保存的目标项及端口完成连接，不是 mouse-up 才重算拖动源路径。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramItem.xaml.cs`, `DiagramItem.AnchorPoint`, `GetOrientation`, `GetOffsetPoint`, `GetRectWithMargin`: 合法索引 `0/1/2/3` 对应 Left/Top/Right/Bottom；预览用 ActualWidth/ActualHeight、5 单位边距。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/PathFinder.cs`, `PathFinder.GetConnectionLine(DiagramItem, Point, int)`: 从端口外偏移点开始，对起始项外扩 5 的矩形执行可见性/最近邻/反向角点选择，然后 `OptimizeLinePoints2`、插入 anchor/sink、`OptimizeLinePoints`。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs`, `DiagramPage.GetPath` 与 `GetAllRect`: 已连线的完整路由由 AStar/障碍集合计算；它与拖动中 `GetConnectionLine` 预览算法是两条不同的链。本轮只核对预览链及其释放写回边界。

## 本地实现对照

- `src/VisionEditor/WorkflowCanvas.cs`, `CreateExecutionPort`, `AdvanceConnectionPreview`, `CanvasPointerMove`: 端口按下进入连接手势；首次仍按下的 move 捕获画布并进入预览。
- `src/VisionEditor/WorkflowGestures.cs`, `BeginConnection`, `UpdateConnectionPreview`, `FindConnectionTarget`, `FinishConnection`: 指针在 GraphLayer 逻辑画布坐标内路由预览、更新最近目标端口，释放时调用 `GraphEditorDocument.ConnectExecution` 写入文档。
- `src/VisionEditor.Core/VmRouting/PathFinder.cs`, `PathFinder.GetConnectionLine(RoutePort, Point)`: 本地保留原生决策顺序与优化步骤，适配 `RoutePort` / `RouteRect`；合法端口的方向、offset 和边距映射一致。节点尺寸输入仍由本地固定画布契约提供（普通节点 110×30）；与原生 `DiagramItem.ActualWidth/ActualHeight` 在插件节点尺寸方面的差异不由此轮推断或修改。
- `src/VisionEditor/WorkflowGestures.cs`, `FinishConnection`: 发现明确的事件生命周期差异。原生 `Connector.ToolItemGrid_PreviewMouseUp` 每次释放都会移除 adorner、释放 capture 并在 `finally` 清空 adorner/path/context 引用；本地成功提交会触发 `RenderGraph` 清空 `GraphLayer.Children`，但旧 `connectionPreview` 引用未清空。下一次 `AdvanceConnectionPreview` 看到非 null 就跳过首次 `CaptureMouse`，故再次拖线时离开 GraphLayer 后不再有原生 capture 行为。
- 已在本地 FinishConnection 中将此临时 Path 从 GraphLayer 移除并置 null；没有更改 PathFinder 参数适配或合法端口契约。

## 操作级验收准备与状态

新增 `tests/VisionEditor.CanvasChecks/Program.cs` 场景：创建两个普通节点，开始 port 3 拖线；经本地 `AdvanceConnectionPreview` 进入 WPF 预览后检查预览几何等于 `VmOrthogonalRouter.Preview` 输出、目标 port 1 被选中、文档修订号与 execution links 尚未改变；捕获 `analysis/full-audit/canvas-connector-drag-preview.png`；随后释放，检查一笔文档写回包含正确端点/索引，临时 Path 引用被清除，再 Undo 检查移除；再次开始拖线，检查创建了新的 Path 并挂回 GraphLayer，以覆盖二次拖动清理边界。

隔离构建/运行命令：

```powershell
dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj -c Debug -p:DirectoryBuildPropsPath='D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\IsolatedBuild.props' -p:BuildInParallel=false -p:UseSharedCompilation=false
```

第一次运行在离屏 OS mouse-capture 断言处退出码 1；此前预览几何、target port、未提前写入、释放单笔写入、清理 preview 字段、Undo 均通过。CanvasChecks 使用 `ShowActivated=false` 且窗口位于 `-20000,-20000`，因此删去了对 `IsMouseCaptured` 的环境不适配断言，保留代码路径的临时 Path 生命周期验收。

第二次隔离运行 EXIT_CODE=0，CanvasChecks 共 74 条 PASS。连接预览新增场景的通过顺序为：预览路由几何、目标上端口、transient 无写入、release 单笔写入、清理预览引用、Undo、第二次预览创建并重新挂入 GraphLayer。既有 CanvasChecks 的其余工作流/连线/滚轮/小地图/保存行为也全部通过。仅有 `MainWindow.xaml.cs(83,224)` 既存 nullable warning。

截图 `analysis/full-audit/canvas-connector-drag-preview.png` 由 RenderTargetBitmap 生成并已目检：两节点和橙色虚线路由清晰可见，未遮挡关键预览；右侧图像区域显示“暂无图像”，属于空图像源测试夹具。此截图是离屏 WPF 画布呈现，不证明用户桌面上的实际 OS 鼠标捕获。隔离 props 为本 worktree 既有 MF-02 构建配置，保留用于串行复验；本轮未启动/停止其他 dotnet 进程，也未清理共享的本线构建缓存。

## 未验证边界

- 测试覆盖源代码的 capture 调用分支条件（首个预览为空时才执行 CaptureMouse）和二次预览对象重建；尚未验证 WinSta0\\Default 实际鼠标离开画布后的 capture 行为，需要当前主程序实际桌面操作确认。
- 当前测试使用确定的两个普通节点尺寸与图层逻辑坐标；插件节点的运行时 ActualWidth/ActualHeight 与多 DPI/Zoom 指针变换仍需独立证据。
- `GetConnectionLine` 本身的合法端口输入与步骤没有发现参数映射差异；已证实的修复在鼠标 capture/临时 adorner 生命周期，不涉及寻路点计算。
