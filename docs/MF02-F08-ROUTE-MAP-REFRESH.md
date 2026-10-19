# MF-02：F-08 障碍地图、fallback 与节点移动后的连线刷新

## 结论

本次沿着原生 `DiagramPage.GetPath → GetPathMap/GetAllRect → AStar → fallback → PathFinder` 核对合法端口和 5000×5000 画布。源码中的 obstacle rasterization、网格映射、fallback 次序与本地 `VmOrthogonalRouter.Route` 一致，没有找到源代码支持的合法输入路由差异，因此没有改路由算法。继而核对了端点移动、非端点障碍移动对路径缓存的影响；本地 WPF 操作结果与原生 `Connection.OnPositionChanged` 的依赖属性触发条件相符。

## 原生源码链

- `D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs`
  - `DiagramPage.DiagramPageWidth` 默认 5000（第 47 行）；构造函数把 Width、Height 设为该值（315–316），且反编译目录中没有该静态值的其他赋值。
  - `DiagramPage.GetPath`（438–480）：取两端 `AnchorPoint`、5 单位 margin rect 与 offset point；调用 `GetAllRect()`、`GetPathMap(list2)`；以 offset point 除以 5 加 1 建立 AStar 起终网格坐标。AStar 点数少于 3 时再以 endpoint rect pair 建图和搜索；仍少于 3 时退回 offset 点之间的一段直角或直线。最后依次 `OptimizeLinePoints`、加回两端 anchor、`GetTurnPoints`、`RechargeLinePoints`。
  - `GetPathMap(List<Rect>)`（482–486）以当前 DiagramPage 的 `GetAllRect()` 作为障碍输入。
  - `GetPathMap(List<Rect>, List<Rect>)`（488–516）仅在实例字段 `map == null` 时创建 `(int)ActualWidth/5+32` × `(int)ActualHeight/5+32` 的 `APoint[,]`；每次标障碍前更新 `APoint.CurTimeEnableFlag`，按每个 rect 的 5 单位格坐标将内部格设成 Wall。
  - `GetAllRect()` / `GetAllRect(List<Rect>, ItemsPage)`（524–551）枚举当前 hostPanal 中所有 `DiagramItem` 的 `GetRectWithMargin(5)`；遇到子 `ItemsPage` 递归调用。节点的 `GetRectWithMargin` 在 `DiagramItem.xaml.cs:318–323` 使用 `ActualWidth/ActualHeight`。
- `D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base.Astar/AStar.cs`, `AStar.FindPath`：路径搜索使用传入矩阵和 `APoint.IsWall`，短路/搜索预算退出返回少于 3 个点的起止对；`APoint.cs:49–67` 通过 `CurTimeEnableFlag` 懒清理每个格点的旧状态。
- `D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml.cs`
  - `OnItemChanged`（230–250）给 SourceItem/TargetItem 的 Position 和 ActualWidth 建单向绑定。
  - `OnPositionChanged`（252–264）仅当本连线的 StartPosition、EndPosition、SourceIndex、TargetIndex 改变时清空 PathPoints；任何已监听属性改变都会调用 SetPath。
  - `SetPath`（266–272）只有 PathPoints 为空且端口有效、两端宽度已知时才调用 `Page.GetPath`。
  - `PathPointsChanged` / `OnPathPointsChanged`（274–288）只在存在已保存 PathPoints 时钝化并绘制该手动路径。

## 本地对照

- 自动路径由 `src/VisionEditor/WorkflowCanvas.cs`, `MainWindow.RenderGraph` 传入节点矩形、端口和 5000×5000 route size；地图尺寸因而为 1032×1032（1,065,024 格）。`VmOrthogonalRouter.Route` 在 `src/VisionEditor.Core/VmRouting/VmOrthogonalRouter.cs` 将 owner 与 obstacles 扩张 5 单位，使用 `(int)coordinate/5+1`、固定 margin 32 和 `APoint.CurTimeEnableFlag`；先对全 rect 执行 AStar，短路径再仅标 endpoint pair 并重搜，仍不足 3 点时走直角 fallback，然后依次用本地反编译移植的 `PathFinder` 简化/钝化。
- 原始 `AStar` 输入文件与本地移植差异只包含头部来源说明、namespace 和访问级别；原始 `AStar.FindPath` 算法语句保持一致。`APoint` 同理。`PathFinder.GetPath` 的地图后处理来源单独记录在 `MF02-F08-FIXED-ROUTE-GRID.md`。
- 已知实现适配：VM 在每个 DiagramPage 实例持有 `map`；本地路由器以静态缓存加锁复用同尺寸矩阵。这保持当前单 UI 线程的逐次结果，并避免每条连线分配一百万个点；多个画布并行寻路时，本地会串行化，而原生实例矩阵允许彼此独立。该并发差异尚无运行证据，当前标为实现差异/未知，而非宣称完全相同。
- VM 的 `GetAllRect` 对出现的嵌套 ItemsPage 递归取矩形；本地当前 procedure 的 `RenderGraph` 传入该 procedure 的 node rects。当前证据没有确认主流程页会实际嵌入共享坐标空间的子 ItemsPage，因此“嵌套 ItemsPage 是否成为外层路由障碍”仍待真实 VM 数据/控件树证据；本次合法普通节点场景不覆盖该边界。
- 端口有效性契约一致：本地连接文档校验限制索引 0–3；原生 `DiagramItem.GetOrientation` 的越界默认与 Offset fallback 不一致，但非法索引不属于本次可加载场景，未扩展输入契约。

## 路由数据差分与操作证据

- `tests/VmRouting.Checks/VmRouting.Checks.csproj` 编译原始 `VMControls.WPF.Base.Astar/*.cs`，并包含 `OriginalPathFinder.cs`；`tests/VmRouting.Checks/Program.cs:54` 使用 source `(200,100,110×30), port 3`，target `(500,350,110×30), port 1`，obstacle `(320,180,110×30)`，画布 5000×5000，比较本地 `RoutePoint[]` 与原始 AStar/PathFinder Oracle 的点列，使用 `SequenceEqual`（逐点、逐序比较）。重叠端口、所有16种端口方向及随机障碍场景亦有对应 exact sequence 对照（Program.cs:35–49）。此处是源移植 oracle 差分，不是对 VM 运行时 API 的黑盒调用。
- 本次完整隔离 WPF 操作命令（2026-10-19）：

  ```powershell
  dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj -c Debug -p:DirectoryBuildPropsPath='D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\IsolatedBuild.props' -p:BuildInParallel=false -p:UseSharedCompilation=false
  ```

  退出码 0，共 74 项 PASS。实际 Canvas 连接几何固定网格用例与障碍刷新用例均运行通过。
- 刷新操作夹具（`tests/VisionEditor.CanvasChecks/Program.cs` obstacle-move 区段）：源 `(100,100)`、目标 `(500,350)` 通过 port 3→1 连线，先在 `(900,900)` 放一个不相干节点，再将障碍移至当前折线路径上。诊断输出确认旧线段与 obstacle `(300,225)..(410,255)` 相交；旧路径为 `(155,130) → (155,238) → (157,240) → (553,240) → (555,242) → (555,350)`。若重新寻路，Oracle-compatible Router 给出 `(155,130) → (155,343) → (157,345) → (553,345) → (555,347) → (555,350)`。实际移动障碍时 WPF 路径保留旧几何；然后移动目标至 Y=400，实际路径改变；两次 Undo 分别恢复端点、障碍，最后保存/重载后连线存在。对应通过项为 `moved obstacle intersects prior route and a fresh native routing call would choose a different geometry`、`unrelated obstacle move preserves existing Connection path until its own endpoints change`、`moving this connection's endpoint invalidates and recomputes its path` 及其 Undo/save 检查。
- 目检图 `analysis/full-audit/canvas-obstacle-diagnostic-window.png`：真实 WPF 渲染捕获了节点、橙色线路及小地图；图证仅说明本地操作状态和可视几何，不证明 VM 的颜色/控件样式一致。

## 分类、未知与范围

- **源码确认**：原生尺寸、端口 offset、障碍建图、AStar/fallback 次序、手动路径刷新触发属性和 `SetPath` 条件。
- **移植并由 oracle/操作验证**：普通节点有效端口的固定 5000 画布路径点列、障碍节点移动保留路径、端点移动触发路径重建、撤销和保存重载。
- **尚未验证**：VM 实际控件树是否向外层路由传入嵌套 ItemsPage；多 DiagramPage 并发寻路是否是外部可见行为差异；不同插件节点实际尺寸何时变化及其对 manual PathPoints 的效果；本地结果与真实 VM 运行窗口逐点对照。本次不以 fixture 通过宣称完整 F-08 等价。
