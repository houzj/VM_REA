# 第二包合流与主框架响应性修复

基线 main `120611182c8908334298ba47eb9244bb1f9b6307`，2026-10-20。三个独立审核报告分别为 [原 IMAGE 链](20261020-pass2-original-image-chain.md)、[无响应诊断](20261020-pass2-hang-diagnostics.md)、[19 项映射矩阵](20261020-pass2-mapped-tool-matrix.md)。彼此只写独占证据，主线核对后单独修改框架拥有的 `WorkflowExecution.cs` 与 FCR-031；`worktrees/tool-plugins` 未读写或合流。无响应子线报告描述的是**修复前**调用路径，本报告下面记录修复及后测。

## 已改变的判断

- 原 `VmModule.PreModules` 不只是视图中的静态连线：首次访问由 `GetMultiModuleConnect` 从 SDK 获取同容器连接，后续 `NextModules.CollectionChanged` 增删同步目标前驱。原 `Group.xml` 的 `InputImage` 和 `InputMask` 都查询 `IMAGE` 组合；`GetFrontIOWithLeafItemsByName` 只按组合类型、可订阅、可见过滤，未按像素格式叶过滤。详情与完整 CIL SHA 见原 IMAGE 报告。FCR-031 已按该证据修订。**候选可见不等于 RGB 可以被二值化执行。** 原 Binary 原生输入格式分支仍缺。
- 当前目录快照是 143 项，19 项有映射，其中 18 个插件加内建 Group；124 项未映射。没有将它与历史 162 项混算。缓存图像原 XML 声明 15 个图像槽，当前 ImageSource→Buffer→Binary 真实图执行 8 态通过，日志 `analysis/joint-audit/mapped-tool-matrix/framework-image-buffer-chain.log` SHA-256 `3C91F51DFB127AE8A4EE4C60DAA3F2752CDB89A354F399322A4D727FCABCA2E9`。RGB 所在槽接 Gray 消费者会 `InputTypeMismatch`；空槽是 `UpstreamUnavailable`；数量 16 是 `InvalidBufferCount`，下游 `ExecutionPredecessorUnavailable`。旧原生缓存导出当前缺席，8 态只是**本地实现**检查。
- 无响应路径中，普通流程 `WorkflowExecution.RunCore → SolutionExecutor.RunAsync → GraphExecutor.RunAsync` 在首次未完成的 await 之前同步工作；二值化 `ExecuteAsync` 返回立即完成的 `ValueTask`，像素循环不会异步让出。受控图像源→二值化线程探针在调用线程完成，耗时 123.79 ms，JSON `analysis/joint-audit/hang-diagnostics/execution-thread-probe.json` SHA-256 `45A581C49D4F6856FA15C36743AB2114B157703BECBC5BF83C8CA8B79EC86F4C`。当前配置自运行已有 `Task.Run`，两条宿主路径不同。原 UI `VmProcedureDecorator.RunOnceCommandExe` 在 `analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmProcedureDecorator.cs:461-470` 调 `VmProcedure.Run(null,bIsWait:false)`，源文件 SHA-256 `A229FE33277EB857A0AFA3F6A9FDB56807D2178AD5E6958B28BC63175CE19B5A`；原 SDK 返回前的实际工作线程与回调仍未证实。用户那次空闲卡死没有 UI 线程栈，不能据此归因。

## 主框架最小修复与自审核

`src/VisionEditor/WorkflowExecution.cs` 现在在 UI 上先捕获 `Progress<ExecutionEvent>` 的 Dispatcher 上下文，再把 `SolutionExecutor.RunAsync` 交给 `Task.Run`。完成后的 owner 校验、状态/结果提交、关闭与取消仍在现有 await 后的 UI 路径。此改动针对**已证实的当前同步阻塞机制**，是本项目独立的宿主响应性修复；原版 `bIsWait:false` 是方向依据，不能宣称 native 调度逐指令相同，也不能宣称用户那次卡死已修好。

新增 `tests/WorkflowDispatcherChecks` 在自有受控 WPF 宿主中注册同步阻塞模块：模块未释放时 UI Dispatcher 可以处理脉冲，模块线程 5 与 UI 线程 2 不同，释放后流程与 owner 正常完成。日志 `analysis/joint-audit/20261020-workflow-dispatcher-check.log` SHA-256 `609E208A159715FBC52E64A7012E5B724B749D23B7E93A09CBB7BBF146390C87`；检查源 SHA-256 `BE8262E30A0820EB9930F4142DCA1F77CB2E23584AA503FCB119B18AA0132B55`。该探针不操作用户桌面、不涉及原生 SDK。

验证：`VisionEditor` Release 构建通过（最终增量日志 `analysis/joint-audit/20261020-editor-build.log` SHA-256 `88EDA00B7B4B4482BAA0BFEE9622482519A2C32827B2C0FFF6A49CB7C7FF2D8F`）；`ExecutionOwnerLifecycleChecks17` **16/16**，`CallbackQueueBeforeChecks22` 通过；图像源→二值化经 `GraphEditorDocument` 保存重开再执行 **4/4**，缓存链 **8/8**，UI Dispatcher 新检查 **2/2**。保存重开的 4 态日志 SHA-256 `27062F9DA81A9DD7F65A6548E087C48A34574573AB8BC063D49509BFA79AA99D`。这些是当前实现自审，不是原机桌面差分。

## 剩余精确界限

J04 的原厂候选集合和连线维护静态链已证；配置窗口保持打开时的刷新、变量/Group 跨层行为与 `.sol` 重开链未证。J05 的原版 Binary 对 RGB24、MONO8 和掩膜的格式门仍缺原生 `ProcessAlg`/算法库函数体与汇编；REA Ghidra provider 本轮 `not_configured`，历史导出路径缺席。J12 仍缺同构方案的 UI 心跳/分段时间线，以及停顿时的受控进程 UI 线程栈/等待链；首次路由约 149 ms/68 MB 是候选成本，不能解释持续空闲卡死。普通流程改为工作线程后，各具体工具若有 UI 线程亲和的隐藏副作用，仍需逐插件 J10 审核；本轮现有关闭/回调检查通过不消除该边界。

本包实施修复属于已存在框架执行单元的响应性修订，**不新增一个已完整关闭的 96 分母单元**。权威总体实施进度仍为 **41/96 = 42.71%**；人工验收不参与，十个标定工具的 10/10 是另一范围。
