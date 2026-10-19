# A03：方案替换后隔离画布会话视口

2026-10-19；对应 [A03原源码链](A03-tabs-batch2.md)。本修复解决main已复现的跨方案状态泄漏，是独立重构的会话身份清理；没有实现或声明原厂ProceZoomIndex持久化等价。

## 触发与原因

方案A的流程ID为main，画布调整到180%并偏移；新建方案仍使用main，或加载另一份包含main的方案。原main的SolutionReplaced只清automaticRoutes，canvasProcedure仍为main，RenderGraph的流程切换分支不执行，旧缩放/偏移继续使用。即使清了流程身份，概览模式在该分支之前提前返回，仍可能沿用旧即时视口，并在后续切换写回__solution缓存。

原程序的显示对象、所属运行对象和每模型UserData.ProceZoomIndex链见A03源码报告。该证据说明不能把流程ID跨模型复用等同同一显示对象；它不证明原厂新建/加载一定重置为100%，原厂会读取持久化值。main当前只实现会话缓存，没有该原生键，沿用既有默认100%/零偏移初始化新会话。

## 最小修复

仅修改 `src/VisionEditor/WorkflowCanvas.cs::InitializeCanvasInput` 的既有SolutionReplaced订阅：保留路由清理，增加canvasViews清空、canvasProcedure=null，并将scale/pan初始化。普通Changed及同方案流程切换不清缓存；概览模式选择保持，视口重新初始化。未修改锁定的MainWindow.xaml.cs或插件框架文件。

## 实际验证

新增 `tests/VisionEditor.CanvasReplacementChecks`，使用真实MainWindow、文档NewSolution/Load和流程/概览处理器，在离屏WPF窗口运行。测试中仅设置旧会话初态；替换、渲染、滚动和切换调用实际产品逻辑。

- 修复前退出1：首项“new solution reusing main id: scale and offsets reset”失败，日志 `analysis/full-audit/batch2-20261019/A03/canvas-before.log` 保留。
- 修复后初版12项通过；补物理UI状态及空方案/相同方案身份重载后，最终26项通过，exit0，日志canvas-final.log。
- 检查包含同ID新建、跨方案加载、同方案/流程身份重载、概览替换后往返、空方案后首次添加流程。逐状态核对字段、缓存、真实LayoutTransform、ScrollViewer偏移和100%文字；同方案两流程的140%/170%会话缩放仍分别保留。
- 现有 `VisionEditor.CanvasChecks` 从本批独占工作目录执行，exit0，73条PASS；15份测试截图存于该目录的analysis/full-audit下，没有覆盖历史同名截图。日志 `A03/canvas-regression/canvas-regression.log`。
- 构建保留原有MainWindow.xaml.cs:123 CS8603警告，本次未引入该警告。相关dotnet任务串行执行。

命令入口：`dotnet run --project tests/VisionEditor.CanvasReplacementChecks/VisionEditor.CanvasReplacementChecks.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false -- analysis/full-audit/batch2-20261019/A03`。现有CanvasChecks无参数，运行时将工作目录设为本批canvas-regression目录以隔离截图。

这些为重构产品真实WPF事件/渲染状态检查，不是用户桌面输入、原厂导航运行或像素差分。A03完整包仍开放：原厂装饰器初始化、非活动Tab返回Group历史、原生缩放保存重开及对应历史事务继续取证。没有自动把此修复扩展成每Tab历史栈，也没有改变方案存储格式。
