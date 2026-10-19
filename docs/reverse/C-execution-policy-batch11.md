# C 第十一批：执行结果保留配置实施

基线 `b894e8c`；2026-10-19。本批实施已有原源码确认的配置读取与 Begin 判定。没有初始化原 VM/SDK、访问设备、写安装配置或修改 tool-plugins。

## 原始调用链与配置合同

复用 C02 第二、三批同版本证据，不重做原程序集探针：

- `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster/Program.cs:224` 启动时将 `SystemConfigManager.IsClearLastExecuteResult` 复制到方案属性。
- `SystemConfigManager.cs:123–139` 首次 getter 调用 `ConfigurationHelper.GetConfigValue("IsClearLastExecuteResult") == "1"`，缓存 nullable bool；setter 仅改内存。
- `analysis/full-audit/batch2-20261019/C02/VM.Utility.il:8148–8248` 原 helper 的路径为 appSettings 的 ApplicationName + `.cfg`，相对首次实际加载时的当前工作目录。加载后缓存 XDocument；根名不受约束，从根直接子节点用唯一 XName 匹配字段，再从字段直接子节点唯一匹配无命名空间 CurValue。返回 Value，不 trim，不接受 bool 文本。
- 同文件 `:8537–8549`、`:8576–8587` 两个谓词均使用完整 XName 比较，不能用 LocalName 忽略命名空间。
- Load 失败时 helper 的 lazy 标志未置 true，下次直接调用 helper 可以重试；但 SystemConfigManager 首次结果已缓存 false，其后该属性不会重试。加载成功而缺节点/重复节点同样使该属性缓存 false。SaveConfig 不使既有缓存失效，原链没有热重载。
- `analysis/decompiled/VM.Core/VM.Core/VmSolution.cs:4115–4134` 位于非零执行计数回调的非零 workStatus Begin 分支：开关 true 且 nExecuteCount > 1 才清该 Root 下各模块 Outputs.leafIOsWithoutTimeStamp 的值并置 IsEmpty，然后触发 Begin。不是删除完整 session，也不是清 SDK 所有结果。

原件 VM.Utility.dll SHA256 `65ebb5edabf77465fa01f8c6d70efdcbd4b6a87f510911249cd93d73649d1cdf`；MVID `32e8ac89-6335-43c6-aa3e-61d8c0a670e5`；复用 REA `ev_17a12d045406658be0d3ec4eaacc51b23a6108dc714f50ac1f3ff3abd51366d1`。GetConfigValue token `06000145`、RVA `0x59b0`、raw CIL SHA256 `e4f35712ad48741f440de25e54a0e208376df47dc376999de060c2819dda6e1f`。CIL 文件 SHA256 `ae2c2abe8251c6261dddaa8c77e3bfb5825d95fe8c1fc4472bf69c30b8b26677`。

## 实施与自审核边界

新增 `src/VisionEditor/FrameworkExecutionPreferences.cs`，只读显式自有配置路径。首次 bool getter 折叠上述文档和属性双层缓存为该单属性启动快照；缺失/无效/异常为 false，setter 仅内存。`FromWorkingDirectory` 接收应用名与目录，固定该次调用的路径；不会发现或读取安装原件，重新创建实例才能重新读取。

原 helper 在首次加载时使用动态 CWD，本实现由主程序在启动时固定自有 VisionEditor.cfg 路径。这是 main 的显式适配，不宣称动态 CWD 行为等价。该类没有保存或文件监听接口，也不增加配置页。

`ShouldClearAtBegin(count,status)` 保留原非零状态及 count>1 条件。根线作为共享执行文件唯一写入者接入真实 RunCore，并以 main 自有执行计数适配 native callback 计数；缓存清理继续使用 main 既有范围。原 VM 清叶值及多消费者行为没有被本次辅助类证明等价。C02.3 保持 partial，C03.2 至多从 not_started 进入 partial，不增加整体完成单元。

新增 `tests/ExecutionPolicyChecks11` 用链接方式编译实际产品 helper，仅写显式审计目录：字段/值缺失与重复、命名空间、直接子层级、非法格式、精确文本、首次失败缓存、成功缓存、新实例读取、内存 setter、不写配置、Begin 条件。根线串行执行后 **24/24 通过、退出0**；实际日志 `analysis/full-audit/batch11-20261019/C/check.log`，自有副本目录 `policy-checks/run-f6d0ce2399ed4d89931a1514593972ec`。子线未自行构建或执行原程序。

根线新增 `FrameworkExecutionPolicy.cs` 并在 `WorkflowCanvas.InitializeCanvasInput` 初始化：应用启动即读取配置快照；真实 `WorkflowExecution.RunCore` 调用 Begin 策略，各流程使用 main 自有计数，SolutionReplaced 清计数。策略为开关为真且第二轮开始时隐藏旧视图，保持完成 session 不变；Restore 防止旧值重现，Store 新结果解除隐藏标记。该接入经本子线只读自审核，与 helper 共同形成真实产品路径，非闲置配置工具。main 的 uint 计数饱和处理也是独立适配，不外推原生溢出行为。

`tests/ExecutionViewChecks11` 由根线验证真实宿主的上述方法，最终 **12/12 通过、退出0**，日志 `analysis/full-audit/batch11-20261019/C/view-check.log`。覆盖首轮保留、第二轮隐藏、session 身份保留、Restore 防旧值重现、新结果恢复、关闭开关保留、独立流程计数、删除流程释放计数/隐藏标记、替换方案清空两者。产品 WPF 构建成功，既有 `MainWindow.xaml.cs` CS8603 警告保留。子线只读复核 `doc.Changed`：只移除已不在方案流程表中的计数/标记，先快照键后删除，不改变现存流程的运行状态。该生命周期补齐未触及插件会话。本子线不将“调用真实宿主私有方法”称为原厂运行、原生差分或完整图执行验证。Group 配置及其它结果消费者仍单列开放。

## 剩余范围

native callback/计数真实时序、叶结果非事件清理、模块 IsEmpty、RefreshResult 与 ModuleResultView 的计数消费者、原 SDK 错误/取消未到达 Begin 的结果行为仍开放。本批不以 helper 通过证明调度全链完成，也不将人工验收作为实施进度门槛。
