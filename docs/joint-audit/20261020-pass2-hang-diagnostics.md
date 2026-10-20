# 第二轮：VisionEditor 无响应链的独立诊断（J12）

基线：main `120611182c8908334298ba47eb9244bb1f9b6307` 加本地未提交工作区，2026-10-20。只读产品与原厂反编译；只在 `analysis/joint-audit/hang-diagnostics/` 建立并运行独立路由探针。未操作用户窗口、进程、相机、安装原件或 `tool-plugins`。用户反馈为“先感觉程序卡住，下一次操作才弹未响应”，不能把该次操作当起因。

## 已确定的当前执行路径

| 入口/触发 | 当前具体调用链和线程意义 | 能证明/不能证明 |
|---|---|---|
| 文档编辑 | `src/VisionEditor.Core/GraphEditorDocument.cs:306` 的 `Edit` 同步触发 `Changed`；`src/VisionEditor/MainWindow.xaml.cs:48` 的订阅依次执行 `ValidateToolConfigurationOwner → RetainExecutionNodes → RefreshAll → RefreshConfigurationRoi → RefreshShellConfigurationStatus`。`WorkflowTabs.cs:75–160` 的 `RefreshAll` 同步做每顶层流程 `GraphValidator.Validate`，再 `RenderGraph` 和 `RefreshRight`。 | 当前 UI 线程上存在全量重绘路径；不能确定这次空闲 Hang 进入了该路径。 |
| 画布刷新 | `WorkflowCanvas.cs:81–175` 每次清空并重建图元；自动路线只在无保存 `PathPoints` 且端点/端口缓存未命中时调用 `VmOrthogonalRouter.Route`，页面固定 5000×5000、预算 1,100,000。 | 普通 `RefreshAll` 并不必然重新 AStar；不能把“每次都路由 100 万格”当事实。缓存命中后仍重新建立图元。 |
| 图像结果 | `MainWindow.xaml.cs:114,153,172` 的 `RefreshRight → RefreshSelectedImageSourcePreview → ToBitmap`，有结果时调用 `ImagePreview.Packed` 并复制 `Pixels.ToArray()`，在 UI 线程创建 `BitmapSource`；本地图像无缓存结果时还可在 UI 路径读取预览文件。 | 可证明同步复制/读取候选；本次缺图像尺寸、运行状态和时间片，无法归因。 |
| 配置自运行 | `ToolExecutionScope.cs:154–205` 在 UI 线程捕获上游结果/订阅并构造隔离流程，`Task.Run` 执行 `GraphExecutor`，恢复后在 UI 线程 `RefreshRight/RenderGraph`。`ToolEditorHost.cs:35–43` 的旧调试窗另走 `NodeDebugExecutor`，也用 `Task.Run`。 | 算法主体通常不在 UI 线程；不能据此排除前/后同步阶段、原生设备等待或 Dispatcher 排队。 |
| 全流程与回调 | `WorkflowExecution.cs:19–89` 的 UI 命令路径**直接** `await new SolutionExecutor(modules).RunAsync(...)`，未切到 `Task.Run`；`SolutionExecutor.cs:14–36` 与 `GraphExecutor.cs:7–78` 均在首次未完成 `await` 前同步执行验证、快照及节点。`ImageBinaryPlugin.cs:24–38` 的整幅硬阈值 CPU 循环返回已完成的非 `async ValueTask`，因此该图的算法可在调用它的 UI 线程上跑完。完成后 `FrameworkCallbackQueue.PublishModuleResult` 先同步存储结果，再派发显示；`MainWindow.FrameworkExecutionOwnership.cs:10–19` 对非 UI 回调使用 `Dispatcher.BeginInvoke`。 | 已确认普通流程与配置自运行的线程策略不同。这里虽没有显式 `.Result` / `.Wait()`，却存在首个异步悬挂点之前阻塞 UI 的确实路径。仍不能静态证明本次用户 Hang 正好经过它。 |
| 空闲异步来源 | `VisionPlugins.ImageSource/LocalFolderSubscription.cs:16–23` 对已创建的文件夹监听器在文件创建/删除时调用 `scope.RequestRun`；`ToolExecutionScope.cs:29–30` 将请求 `Dispatcher.BeginInvoke` 到流程运行。`ToolExecutionScope.cs:208–213` 的相机发现同步发生于宿主初始化 `AttachExecutionScope` 或用户点“相机列表”，无证据表明它在空闲时周期触发。 | 文件夹事件可在无点击时发起运行，但需确认用户方案启用了该模式及事件；相机枚举是有界入口候选而非自动空闲定因。 |

## 原版对照与受控探针

原厂 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs:438–516` 的 `GetPath → GetPathMap → AStar.FindPath → PathFinder` 在实例 `map == null` 时生成 `(ActualWidth/5+32) × (ActualHeight/5+32)` 的 `APoint[,]`。本地 `src/VisionEditor.Core/VmRouting/VmOrthogonalRouter.cs:8–47` 在静态锁内复用同尺寸 map。这说明固定大矩阵有原厂源码依据，不能仅因它大就改算法或称当前实现偏离原厂。原文件 SHA-256 `35FDA4306245515F4DFB6DBF20E7B8479BBAE56830B2F76549B63E9D456699D3`，原 `AStar.cs` SHA-256 `8F21DFC6DE5D801B9EB781F1976817607CCDEECDAA42BC437052A9AC184B8304`；当前 Router 源 SHA-256 `4E403FC5E2FAA5B99A241774FF60306D38E33F7EB388D0EEA4570DE777D6BB3D`。

原版 UI 命令 `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmProcedureDecorator.cs:461–474` 的 `RunOnceCommandExe` 调用 `VmProcedure.Run(null, bIsWait: false)`；`analysis/decompiled/VM.Core/VM.Core/VmProcedure.cs:1130–1150` 把 `bIsWait` 和 `bIsRespondToUI` 传给 `mProcessManager.SyncExecuteOnce`。这只证实 **UI 命令参数为不等待**，并不揭示原生 SDK 在返回前运行多少 CPU 工作、实际执行线程或回调时机。原两文件 SHA-256 分别是 `A229FE33277EB857A0AFA3F6A9FDB56807D2178AD5E6958B28BC63175CE19B5A`、`FDDE074D17C50C37F8D0BFE097CF20F904CDB4B59D4AC39612FC023C3F0B69DA`。

独立探针 `analysis/joint-audit/hang-diagnostics/RouterProbe.csproj` 只调用当前 Router，一条固定 5000×5000、两节点、无其他障碍的路径。命令：`dotnet run --project analysis/joint-audit/hang-diagnostics/RouterProbe.csproj -c Release --no-build`。保留输出 `analysis/joint-audit/hang-diagnostics/router-probe.json`，SHA-256 `DFF7E95C38DFBB1ECDCA752B11CCE368AFF21F484932A7407D5CFA724A3DC584`。该次首次路由 **148.79 ms、分配 68.21 MB**；同进程后两次 **0.75/0.17 ms、各约 29 KB**。另一冷启动样本约 234 ms/68.20 MB。数值仅适用于此机器/Release/夹具；图元重建、真实方案障碍、BitmapSource、Dispatcher 未纳入。

路由探针揭示可测的首次 UI 停顿成本，但不足以解释用户经历的空闲后持续无响应：map 是复用的；本次事件没有端点变化或调用栈证据。Windows Application Hang 1002、2026-10-20 18:16:59 +08，PID `0xc0c8`，记录于 `analysis/joint-audit/20261020-hang-event.json`；该报告只有进程/事件信息，没有 UI 线程栈或等待链，原进程已退出。它证明发生过无响应，不证明挂在路由、N 点标定、二值化、相机 SDK、文件监听或用户最后一次点击。

更直接的当前线程探针 `analysis/joint-audit/hang-diagnostics/ExecutionThreadProbe/` 以真实 `ImageSourcePlugin` SDK 已提交的 1024×1024 GrayImage → 真实 `ImageBinaryPlugin` 建图，在**调用 `GraphExecutor.RunAsync` 的同一线程**立即检查返回 Task。命令 `dotnet run --project analysis/joint-audit/hang-diagnostics/ExecutionThreadProbe/ExecutionThreadProbe.csproj -c Release --no-build`；保留 `execution-thread-probe.json` SHA-256 `45A581C49D4F6856FA15C36743AB2114B157703BECBC5BF83C8CA8B79EC86F4C`。这次 `initialThread=returnThread=2`、调用返回耗时 **123.79 ms**、`completedAtCallReturn=true`、两节点均 Success；另一次冷跑为 180.05 ms。由于命令未用 `Task.Run` 且整个 Task 在返回时已完成，这段真实算法在调用线程完成。控制台线程不是 WPF Dispatcher；把它映射到产品的依据是上述 `WorkflowExecution.RunCore` **从 UI 命令直接调用** 同一执行器，而不是冒称已经抓到用户实例 UI 栈。图像源 SDK 已提交帧与硬阈值路径的 await 均未异步悬挂；相机/非零间隔/其他插件可能悬挂，不能外推全部流程。

## J12 未关闭条件与下一探针

J12 状态：`confirmed-current`（存在普通流程实际同步 CPU 路径及可复验成本）＋ `blocked-exact-evidence`（缺当次或可重现受控实例的 UI 线程栈/等待链和分段时间线）。本线不擅自改动调度：在没有对原版 `SyncExecuteOnce(..., bIsWait:false, ...)` 的返回/回调生命周期与当前方案持有/取消语义做差分前，把整个执行器简单塞入 `Task.Run` 可能引入新的竞态或改变回调顺序。它是可优先修的结构性风险，不是这次用户事件的已证根因。

下一步由 main 单写入者在**自有受控副本/实例**加入测试宿主诊断：固定方案和 EXE SHA；UI Dispatcher 心跳记录间隔与线程 ID，给 `doc.Changed`、`GraphValidator`、`RenderGraph`（区分路由缓存命中/失效）、`RefreshRight/ToBitmap`、配置构造/提交、`RunConfiguredTool` 前后、`scope.RunRequested`/结果回调打起止时间；原样保存超阈值时间线。按空闲、文件夹事件、单次/连续运行、配置编辑分组。若重现，**仅**给受控进程取 UI 线程栈及等待链；对比在暂停点是否为本地计算、I/O、Native SDK、锁或 Dispatcher 等待。不能以“没有重现”宣称问题已修，也不能拿测试宿主结果冒充用户那次原进程定因。
