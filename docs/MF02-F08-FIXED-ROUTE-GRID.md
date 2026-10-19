# MF-02 / F-08：固定画布路由网格

## 取证与调用链

- 原始文件：`D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs`。
- `DiagramPage.DiagramPageWidth` 在第 47 行为 `5000.0`；构造函数第 315–316 行把 `Width`、`Height` 都设为该值。
- 原始交互链：`Connection.OnPositionChanged`（`Connection.xaml.cs:252–263`）在本连接的 `StartPosition`、`EndPosition`、`SourceIndex`、`TargetIndex` 改变时清空 `PathPoints`，随后 `SetPath`（`:266–270`）调用 `Page.GetPath`。`GetPath`（`DiagramPage.xaml.cs:438–480`）取端口锚点/偏移、全节点矩形，调用 `GetPathMap`，用返回的矩阵执行 `AStar.FindPath`；短路径时仅用端点矩形重试，再经 `PathFinder.OptimizeLinePoints`、`GetTurnPoints`、`RechargeLinePoints` 返回折线路径。
- 网格分配链：`GetPathMap(List<Rect>, List<Rect>)`（`DiagramPage.xaml.cs:488–516`）在 `map == null` 时只分配一次，大小为 `(int)ActualWidth / 5 + 32` × `(int)ActualHeight / 5 + 32`；每次标障碍前更新 `APoint.CurTimeEnableFlag`（同文件 `:501`），由 `VMControls.WPF.Base.Astar.APoint.CheckTime`（`VMControls.WPF.Base.Astar/APoint.cs:60–67`）懒重置节点状态。因此实际原生网格是 1032 × 1032（1,065,024 格）。
- 手动路径链独立于自动路由：连接 `PathPoints` 变化经 `OnPathPointsChanged` → `SetPath` 绘制；中点拖动写入该路径。此修复只改变自动路由输入的画布矩阵尺寸，不改手动点、连线数据结构或保存/撤销语义。

## 原实现差异与修复

- 本地调用入口：`src/VisionEditor/WorkflowCanvas.cs` 的 `RenderGraph`。此前把 `max(Node.Right)+100`、`max(Node.Bottom)+100` 作为路由矩阵宽高，和原生固定 `DiagramPage` 尺寸不同。
- 已改为固定 `DiagramPageSize`（5000 × 5000）。`src/VisionEditor.Core/VmRouting/VmOrthogonalRouter.cs` 在同一尺寸下复用 `APoint[,]`，保留原生“每画布缓存矩阵、通过时间标识刷新每轮搜索状态”的生命周期，避免每条线重复分配约一百万个格点对象。锁覆盖矩阵重置和 AStar 搜索；尺寸变化时重建缓存。
- 证据状态：上述原生成员/值/调用关系为反编译源码确认。固定尺寸实现是本地对照源码迁移，不表示完整 VM 等价；不规则、重叠、逆向和极端布局的路径差分仍在 F-08 未关闭范围。

## 验证状态

- 已新增 `tests/VmRouting.Checks/Program.cs` 的 5000 × 5000 原始 WPF AStar/PathFinder oracle 对照。
- 已新增 `tests/VisionEditor.CanvasChecks/Program.cs` 实际 WPF 画布连接场景：建立源、目标及障碍节点，计算固定尺寸原始算法期望路径，检查命中路径几何与期望一致，并导出 `analysis/full-audit/canvas-fixed-native-route-grid.png`。
- 尚未通过构建或目检。第一次执行的精确命令：`dotnet run --project tests/VmRouting.Checks/VmRouting.Checks.csproj -c Debug`；退出码 1，未进入测试运行，MSBuild `MSB3491` 在 `src/VisionData/obj/Debug/net10.0/VisionData.AssemblyInfoInputs.cache` 写入阶段拒绝访问临时文件 `b1uwcn4b.o2z~`。只读检查确认目标缓存不是只读，文件和目录 ACL 包含 CodexSandboxUsers 的 Modify；无法读取活动 dotnet 的命令行/归属（`Get-CimInstance Win32_Process` 返回拒绝访问），`Get-Process` 看到多个 dotnet 进程（启动于 09:01、09:02、09:36、09:58、09:59）。
- 按协调者指示，在忽略目录 `analysis/full-audit/build-mf02/IsolatedBuild.props` 创建临时 props，将每个项目的 `BaseIntermediateOutputPath`、`MSBuildProjectExtensionsPath`、`BaseOutputPath` 分别导向 `$(MSBuildProjectDirectory)\..\..\analysis\full-audit\build-mf02\{obj,bin}\$(MSBuildProjectName)`。`dotnet msbuild -getProperty` 只读核验了测试项目和 VisionData 项目的路径按项目名分别展开且互不冲突；PowerShell 写入探针也能写入/删除于相关目录。
- 属性一致性：隔离构建命令与最终只读求值命令使用完全相同的 `DirectoryBuildPropsPath` 绝对路径，即 `D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\IsolatedBuild.props`；求值的 `BaseIntermediateOutputPath`、`MSBuildProjectExtensionsPath` 和 `BaseOutputPath` 分别为 `D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\{obj,bin}\VisionEditor.CanvasChecks\` 及 `...\VisionData\`，项目名不同，未共用程序集信息缓存路径。
- 隔离后唯一一次 `dotnet run` 命令：`dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj -c Debug -p:DirectoryBuildPropsPath='D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\IsolatedBuild.props'`；退出码 1，MSBuild 已进入编译目标但未执行测试。完整关键错误（均来自该命令）：

  ```text
  Microsoft.Common.CurrentVersion.targets(1208,5): warning MSB3191: 无法创建目录“D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\bin\VisionEditor.CanvasChecks\Debug\net10.0-windows\”。Access to the path 'D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\bin\VisionEditor.CanvasChecks\Debug\net10.0-windows\' is denied. [D:\MyWork\REA逆向测试\worktrees\framework-canvas\tests\VisionEditor.CanvasChecks\VisionEditor.CanvasChecks.csproj]
  Microsoft.Common.CurrentVersion.targets(1208,5): warning MSB3191: 无法创建目录“D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows\”。Access to the path 'D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows\' is denied. [D:\MyWork\REA逆向测试\worktrees\framework-canvas\tests\VisionEditor.CanvasChecks\VisionEditor.CanvasChecks.csproj]
  Microsoft.Common.CurrentVersion.targets(1208,5): warning MSB3191: 无法创建目录“D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows\refint\”。Access to the path 'D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows\refint\' is denied. [D:\MyWork\REA逆向测试\worktrees\framework-canvas\tests\VisionEditor.CanvasChecks\VisionEditor.CanvasChecks.csproj]
  Microsoft.Common.CurrentVersion.targets(1208,5): warning MSB3191: 无法创建目录“D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows\ref”。Access to the path 'D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows\ref' is denied. [D:\MyWork\REA逆向测试\worktrees\framework-canvas\tests\VisionEditor.CanvasChecks\VisionEditor.CanvasChecks.csproj]
  Microsoft.NET.GenerateGlobalUsings.targets(45,5): error MSB4018: “WriteLinesToFile”任务意外失败。 [D:\MyWork\REA逆向测试\worktrees\framework-canvas\tests\VisionEditor.CanvasChecks\VisionEditor.CanvasChecks.csproj]
  System.UnauthorizedAccessException: Access to the path 'D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows' is denied.
  Microsoft.Common.CurrentVersion.targets(6059,5): error MSB3191: 无法创建目录“D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows\”。Access to the path 'D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\obj\VisionEditor.CanvasChecks\Debug\net10.0-windows\' is denied. [D:\MyWork\REA逆向测试\worktrees\framework-canvas\tests\VisionEditor.CanvasChecks\VisionEditor.CanvasChecks.csproj]
  生成失败。请修复生成错误并重新运行。
  ```

  遵照“失败即停”未重试。CanvasChecks 未执行，也未生成/目检新截图。
- 后续只读静态核验：参照 Shell 本线 `MF01-TOOLBAR-UNDO-REDO.md`，MF02 临时 props 已追加 `**/obj/**;**/bin/**;analysis/full-audit/build-mf02/**` 默认项排除，并用 `MSBuildThisFileDirectory` 推导 worktree 根。新的 `dotnet msbuild -getProperty` 结果仍为 CanvasChecks 和 VisionData 各自独立的 `analysis/full-audit/build-mf02/obj|bin/<ProjectName>`。此方案尚未构建验证；遵照调度等待 Group 释放唯一构建 slot 后，才运行一次 CanvasChecks。
- 目前证据无法区分 dotnet 子进程权限限制和未知并行进程/安全软件对目录创建的拦截；需协调者确认活动进程归属或安排允许 dotnet 创建当前 worktree 内构建目录的执行环境。未知归属的进程未被停止。获得可运行的单次窗口后，仍需运行 `CanvasChecks`，核对通过数，目检 `canvas-fixed-native-route-grid.png`，并补写精确输出、内存/耗时。

## 未知/剩余边界

### 本次隔离运行（2026-10-19）

- 协调者确认本线可以使用隔离构建 slot，避免访问另一线 PID 6060。实际执行一次：`dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj -c Debug -p:DirectoryBuildPropsPath='D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\IsolatedBuild.props' -p:BuildInParallel=false -p:UseSharedCompilation=false`；进程完整退出，退出码 1。隔离编译确实启动并完成（生成物位于 `analysis/full-audit/build-mf02/bin|obj/<ProjectName>`）；没有第二次构建/测试。该会话约运行两分钟，之后输出了 WPF 检查结果。
- 运行顺序输出 49 条 `PASS`。其中与本项直接相关的两条通过为：`actual canvas route retains one native 1032-by-1032 cached APoint grid`，及 `real WPF edge display uses the native fixed 5000x5000 DiagramPage routing grid`。另有 47 条既有画布交互检查通过；不能据此声称整套检查通过。
- 唯一失败在新增 obstacle fixture 的前提断言 `moved obstacle intersects prior route and a fresh native routing call would choose a different geometry`（`tests/VisionEditor.CanvasChecks/Program.cs:281`）。失败点是检查“新障碍下的假设性新路由”是否与原缓存路径不同；后面的固定连接路径、命中选择、端点改变重路由、撤销/保存检查没有执行。现有输出未打印两条实际几何，因此不能将失败归因于产品实现或 VM 差异；可能是 fixture 的路径/障碍构造前提不成立。遵照协调指示，本轮仅静态查阅几何生成代码，未改算法、未改夹具、未重跑。
- 已有截图：`analysis/full-audit/canvas-fixed-native-route-grid.png`（本次运行约 10:35 生成，86,287 字节）。已目检：主画布视口为空白，小地图右下只有很小的标记，没有可辨认的节点和连线。尽管反射几何断言通过，这张截图不能作为“可见 WPF 连线外观”的目检证据；本次 obstacle 场景截图位于失败断言之后，未生成。可见性仍未验收。
- 当前状态：固定路由网格实现已成功编译；精确矩阵尺寸与 WPF 路由几何两项自动断言通过；完整 CanvasChecks 未通过，截图未能证明可见路线，obstacle 移动闭环仍未验收。不得将“49 PASS”表述成测试全通过。

### obstacle fixture 诊断（第二次、单次隔离运行）

- 仅在测试夹具新增诊断输出：输出旧路径、新路由状态/路径、障碍矩形范围和与最长旧线段的相交判断。实际命令仍通过 `DirectoryBuildPropsPath='D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\IsolatedBuild.props'`、`BuildInParallel=false`、`UseSharedCompilation=false` 隔离，完整命令与上节相同。该次运行退出码 1；遵照要求未重跑。
- 诊断原文：`DIAG obstacle=300,225..410,255; longest=(157,240)->(553,240), intersects=True; oldRoute=[(155,130) -> (155,238) -> (157,240) -> (553,240) -> (555,242) -> (555,350)]; newStatus=BudgetExceeded; newRoute=[]`。因此本次失败不是证据表明算法路线相同，也不能证明 VM 产品行为错误；是测试调用遗漏了矩阵预算参数，落回 `VmOrthogonalRouter.Route` 的通用默认 `maxCells=250000`，小于固定 5000 页面实际需用的 1,065,024 格。
- 原生依据：反编译 `DiagramPage.GetPathMap` 的固定 `5000 / 5 + 32 = 1032` 尺寸。仅修正测试夹具，显式给假设重路由调用传入 `1_100_000` 上限，令其覆盖原生矩阵格数；产品路由算法未改。此测试修正尚未经过第二次运行，重路由路径差异以及后续保留路径检查仍待下一次单独复验。
- 失败前生成并已目检截图 `analysis/full-audit/canvas-obstacle-diagnostic-window.png`：截图能看到画布内三个节点和橙色折线路径，以及右下小地图，证明本次捕获的是有内容的 WPF 窗口；它是本地当前画布的视觉产物，不证明与 VM 截图的完整样式等价。
- 该次输出在前一轮 49 条检查后新增了诊断行，随后仍由预算断言前提导致退出码 1。矩阵网格及 route geometry 两项此前通过情况不变；新增 budget 修正未验证。完整 CanvasChecks 仍未闭合。

### 最终隔离复验（2026-10-19）

- 唯一复验命令：`dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj -c Debug -p:DirectoryBuildPropsPath='D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\IsolatedBuild.props' -p:BuildInParallel=false -p:UseSharedCompilation=false`；完整退出码 0。
- 共 65 条 `PASS`。包含固定 1032×1032 缓存、5000×5000 WPF 几何、障碍与旧路径相交时的新路径差异、无关障碍移动保留当前 Connection 路径、连接选择、改变端点后重路由、两次撤销及保存/重开；另外已包含中点拖动、平移缩放、小地图导航、工作流搜索、Copy 插入连线和多节点复制等本线既有检查。此结果仅表示本地 CanvasChecks 当前场景通过，不声称所有 VM 交互已等价。
- obstacle DIAG 确认旧路径 `(155,130) -> (155,238) -> (157,240) -> (553,240) -> (555,242) -> (555,350)`；障碍矩形为 `(300,225)..(410,255)`，相交最长水平段。预算修正后假设重路由状态为 `Success`，新路径 `(155,130) -> (155,343) -> (157,345) -> (553,345) -> (555,347) -> (555,350)`。新旧路径不同；实际 `Connection` 在障碍节点单独移动后仍保留旧几何，移动端点后才重新计算，检查、撤销、保存重开全通过。这与反编译 `Connection.OnPositionChanged` 只监听该连接端点/端口改变后清空 `PathPoints` 的调用链一致。
- 最新 `analysis/full-audit/canvas-obstacle-diagnostic-window.png` 已目检：画布有三个节点、橙色折线路径、小地图，截图捕获的是有内容的 WPF 窗口。`canvas-fixed-native-route-grid.png` 仍为空白，是较早场景捕获，不能作路线外观证据；obstacle 诊断图可作为本次实际界面操作的局部目检证据。
- 预算上限只修正测试调用，使用 1,100,000 覆盖原生 1,065,024 格，不改生产算法。`git diff --check` 无空白错误；有 PowerShell/Git 对测试文件 LF→CRLF 的提示。

## F-08 路径简化、端口方向与重叠节点复核

- 原生证据：`D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs` 中 `DiagramPage.GetPath`（438–479）依次计算两端锚点和 5px 外扩点、取 `GetAllRect()`、用全部矩形建立 AStar 网格、执行端点矩形 fallback、对点列调用 `PathFinder.OptimizeLinePoints`、补回锚点、再运行 `GetTurnPoints` 与 `RechargeLinePoints`。`GetAllRect`（524–551）递归枚举每个 `DiagramItem` 的 `GetRectWithMargin(5)`，故重叠节点也进入障碍集合。
- 端口映射由 `DiagramItem.xaml.cs` 的 `GetOrientation`（331–341）、`AnchorPoint`（325–329）、`GetOffsetPoint`（353–363）、`GetConnectorPosition`（584–592）确认：合法 index 0/1/2/3 对应左/上/右/下；锚点根据 `ActualWidth/ActualHeight`。`GetRectWithMargin`（318–323）也使用控件实际尺寸。
- 静态对比：本地 `src/VisionEditor.Core/VmRouting/PathFinder.cs` 与反编译 `PathFinder.cs` 通过 `git diff --no-index` 比较，`RechargeLinePoints`、`Passivation`、`GetTurnPoints` 和带矩形/端口方向的 `OptimizeLinePoints` 算法语句一致；差异集中在命名空间、WPF几何类型替代和 `GetConnectionLine` 的 `DiagramItem`→`RoutePort` 参数适配。`WorkflowCanvas.Port` 对合法 0–3 index 的边/锚点映射一致。没有找到可由这批源码支持的路径简化算法缺陷，因此本轮没有改产品算法。
- 可证实的边界：反编译 `GetOrientation` 对超范围 index 默认 Right，而 `GetOffsetPoint` 默认左偏移；本地 factory 的默认分支为 Down。但本地 `GraphEditorDocument.ConnectExecution`（`GraphEditorDocument.cs:237`）和 `SolutionStore.ValidateStructure`（`SolutionStore.cs` execution links 验证段）拒绝 index 不在 0–3 的文档，真实 `DiagramItem` 只实例化四个 Connector。该差异只存在于无效输入状态，本轮不扩张存储契约去模拟不可加载输入；若未来要处理旧版损坏数据，应先获得可确认的 VM 载入行为。
- 本轮只补了实际 WPF 重叠节点端到端覆盖：`tests/VisionEditor.CanvasChecks/Program.cs` 构造源 `(180,150)`、目标 `(220,160)` 与障碍 `(210,145)`，端点分别选 bottom/top；反射读取实际 GraphLayer 的连线路径，与本地 Router 输出几何比对。该坐标与 `tests/VmRouting.Checks/Program.cs` 的 untouched AStar oracle 重叠端点场景相同，oracle已有全部端口方向/重叠端点差分。截图 `analysis/full-audit/canvas-overlapping-endpoint-route.png` 已目检，显示三节点重叠；折线被上层节点卡片遮住，截图不能单独证明线条外观，几何比较通过。
- 使用 MF01 隔离模式单次运行 CanvasChecks，完整命令：`dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj -c Debug -p:DirectoryBuildPropsPath='D:\MyWork\REA逆向测试\worktrees\framework-canvas\analysis\full-audit\build-mf02\IsolatedBuild.props' -p:BuildInParallel=false -p:UseSharedCompilation=false`。退出码 0，共 66 条 PASS；其中重叠场景通过 `actual WPF canvas renders the native-oracle-tested overlapping endpoint route`。该项是集成验证补充，不是已发现/修复的路由算法差异。
- 未知：反编译当前仓库边界内的 `DiagramItem.xaml` 是壳层，完整实际尺寸/模板由外部 GlobalResources 与 tool/module template 影响。已确认本地 Group 110×40 注释指向外部 `diagramitemtemplate.xaml`，但本轮没有独立捕获其全量绑定关系；具体插件节点 ActualWidth/ActualHeight 是否都等于本地节点视觉尺寸，仍需按节点模板分别验证。重叠布局的 VM运行态视觉层级/遮挡细节也未由截图推定。

- 反编译 `GetAllRect` 会递归访问其子 `ItemsPage`；Group 内部控件相对外层页面的坐标变换是否在原生视觉树中以同坐标参与避障，需从该版本实际控件模板和运行态进一步确认。本次不改变障碍收集范围。
- 原生 `ActualWidth/Height` 是构造时固定值，但控件后续尺寸属性变更时缓存 `map` 不重建；当前本地固定尺寸也不模拟运行中改变页面尺寸。
- 本次未证明节点重叠、完全逆向、封闭障碍、DPI 或超界布局路径与 VM 完全一致；这些仍按清单 F-08 留待差分。
