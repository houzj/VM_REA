# 2026-10-20 框架宿主—节点联合只读审查（A 线）

审查基线：`main` `120611182c8908334298ba47eb9244bb1f9b6307`，工作区已有未提交文件；本线未改产品代码、未碰 `tool-plugins`。以下按“原版源码/资源已证实”“当前实现已证实”“推断/未知”分开。用户反馈用于确定审查目标，不作为原版规格。

## 1. 主框架到图像二值化的实际链

1. 工具箱：`src/VisionEditor/tool-catalog.json:723-728` 把 `IMVSBinaryModu` 映射到 `vision.image-binary`；`src/VisionEditor/MainWindow.xaml.cs:48` 注册 `ImageBinaryPlugin`；`src/VisionEditor/ToolNavigation.cs:29-67,88-102,157-175` 读取分类、核对 `ModuleId` 是否已有 descriptor、创建可拖动 tile；`MainWindow.xaml.cs:76-77` 和 `GraphEditorDocument.cs:135-146` 增加节点及默认参数。上述为当前实现，不代表原版工具箱创建顺序相同。
2. 打开：`WorkflowCanvas.cs:239-246` 双击普通节点进入 `OpenNode`；`MainWindow.xaml.cs:112` → `ToolConfigurationHost.cs:16-46`。图像二值化只实现 `IWpfToolEditorFactory`（`ImageBinaryPlugin.cs:8,19`），走该文件中的过渡窗口工厂分支，而非 `IToolConfigurationProvider` 模型窗口。`VisionPluginContracts/ToolConfigurationWindowHost.cs:18-36` 在 UI Dispatcher 同步构造、`Show`、切换窗口。原版托管 `IMVSBinaryModuTool.Run` 仅进入 `serverSDKManager.SyncSelfRun(ID)`，配置模板来自安装目录 `IMVSBinaryModuAlgorithmTab.xml` 的 `GroupLinkItem`；当前窗口是独立重建，不能视为原版控件。
3. 绑定：`ToolExecutionScope.cs:52-113` 提供插件配置订阅、运行状态和选择服务；`WorkflowConfigurationSubscriptionService` 最终写入 `GraphEditorDocument.SetProcedureInputSubscription`，该方法在 `GraphEditorDocument.cs:159-165` 清除同端口旧 connection 并写新 subscription，执行 `GraphValidator` 后进入 `Edit`。旧画布有线连接由 `ToolEditorHost.ToolInputLabel` 优先显示；插件独立窗口用 `ImageBinaryWindow.xaml.cs:28,31` 的 `PreviewInput` / `InputLabel` / `EditInput`。`NodeDebugExecutor.ResolveInput`（`src/VisionFramework/NodeDebugExecutor.cs:11-19`）要求一个真实绑定及匹配类型，不自动执行上游。原版 `IMVSBinaryModu.xml:31-39` 的 `InputImage`、`InputMask` 是 IMAGE Combination；GroupLink 的候选初始化/RelateIO 全链仍须与插件线联合核对，当前托管端口名 `Image` 是独立映射。
4. 单节点运行：配置服务 `ToolExecutionScope.RunConfiguredTool`（该文件 `RunConfiguredTool`）用既有 `workflowSessions`/`configurationResults` 的上游缓存解析输入，再在 `Task.Run` 内执行孤立 `GraphExecutor`，结果回 UI 线程写 `configurationResults`、`results`，调用 `RefreshRight` 与 `RenderGraph`。旧调试会话 `ToolEditorHost.CreateScopedToolEditorContext` 另有 `NodeDebugExecutor` 路径，但当前从主窗双击二值化实际走配置服务。没有上游运行记录时自运行返回输入缺失；不把它解释为原版 SelfRun 的上游准备语义。
5. 整流程运行：`WorkflowTabs.cs:123-131` 单次/连续入口 → `WorkflowExecution.StartRunCore` / `StartContinuous` → `RunCore` → `SolutionExecutor.RunAsync`；完成后 `FrameworkCallbackQueue` 同步提交 `StoreWorkflowCallResults`/历史，再更新视图；`ExecutionOwnerEpoch` 和取消/关闭检查限制过期结果发布。`WorkflowExecution.cs:73-88` 连续运行检查 `StopOnNg`、取消和间隔。`ToolEditorHost.cs:70-79`、`ToolConfigurationWindowHost.cs:35` 管理窗口关闭；这是当前宿主链，原版逐节点状态、回调次序与释放时机不能由这些托管代码推定。

## 2. 按严重度排序的问题和证据界限

### P0：图像二值化 UI 可选的四个原版模式没有执行实现（确认的功能断链）

安装资源 `D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/ImageProcessing/IMVSBinaryModu/IMVSBinaryModuAlgorithmTab.xml:70-399` 的 `BinaryType` 枚举/条件支持硬阈值、均值、高斯、Sauvola、自动阈值（值 1/2/3/8/4）。`docs/plugins/IMAGE-BINARY-SOURCE-AUDIT.md:21-31` 记录原托管 `SyncSelfRun` → 原生模块 `ProcessAlg` → `IImageBinaryTool` / 通用算法运行库边界；此处引用的是已有审计，所记原生 Ghidra 导出路径本次检查未在此工作区找到，不把缺失的原生函数体当作已重新核对。当前 `ImageBinaryWindow.xaml.cs:32-52` 提供五项选择；`ImageBinaryPlugin.cs:25` 对 `BinaryType != 1` 立即返回 `BinaryModeUnavailable`。因此“可配置”与“可执行”跨层不一致，且这是已确认的当前实现差异，不是截图推测。`docs/plugins/IMAGE-BINARY-SOURCE-AUDIT.md` 的单夹具均值/高斯/OTSU 候选、Sauvola 未匹配不能替代源码级算法重建。建议先恢复通用算法库对应成员/指令和参数送入分支，再做有边界的逐像素探针；若暂不能恢复，应在宿主能力声明中清楚区分可编辑与不可运行，而非声称五模式完成。改动归插件线/双方确认的合流任务。

### P0：用户报告的“空闲后未响应”尚无根因；UI 线程有需优先测量的同步热点（卡死事实与候选分开）

用户说明：先感觉程序卡住，尝试继续操作时 Windows 才弹“未响应”；没有明确双击 N 点、编辑或发起运行的触发操作。主线另存 Windows Application Hang 1002、2026-10-20 18:16:59、PID `0xc0c8` 于 `analysis/joint-audit/20261020-hang-event.json`；事件只能证实无响应，不能证明线程阻塞位置。本线没有抓现有进程堆栈，也没有操作该窗口。

已确认的当前同步路径：`MainWindow.xaml.cs:48` 把 `doc.Changed` 连到 `RetainExecutionNodes`、`RefreshAll`、配置 ROI/状态刷新；`WorkflowTabs.cs:75-160` 每次 `RefreshAll` 对流程做 `GraphValidator.Validate`，随后 `RenderGraph`、`RefreshRight`；`WorkflowCanvas.cs:81-175` 的 `RenderGraph` 清空并重建图元，对端点失效的自动连线调用 `VmOrthogonalRouter.Route`，路由图幅 5000×5000，搜索上限 1,100,000；`MainWindow.xaml.cs:114,153,172` 的右侧刷新重建来源列表、位图并复制像素。`ToolExecutionScope.cs:26-31` 将 scope 刷新/执行请求排入 Dispatcher；`WorkflowExecution` 连续运行也可通过 Dispatcher 回到结果刷新。配置窗口创建及二值化 `ImageBinaryWindow.xaml.cs:25-26` 的 `roi.SetImage(InputImage())` 在 UI Dispatcher，`VisionUi/GroupRoiEditor.cs:44` 创建 Gray8 `BitmapSource` 并复制像素。**这些只是可测量的阻塞候选**；无法从静态代码认定本次空闲 Hang 由路由、图像复制、设备回调、某插件或 N 点标定导致。

可复验探针：在受控副本的同一方案/同样图像大小下，给 UI Dispatcher 加心跳和耗时事件（只在测试宿主），记录 `doc.Changed`、`RefreshAll`、`GraphValidator`、`RenderGraph/Route`、`RefreshRight/ToBitmap`、配置窗口构造及 scope 回调的起止时间、线程 ID、队列长度；同时按无操作空闲、后台源刷新、单次运行、连续运行、打开/关闭配置四种场景分组。若再次无响应，先取该受控实例 UI 线程堆栈/等待链，再归因；不得根据 Windows 弹窗的点击时刻反推触发源，也不应终止用户已有进程。

### P1：二值化配置写入与结果刷新存在重复全量工作风险（当前链确认，性能影响未测）

`ImageBinaryWindow.xaml.cs:53-57` 的 `CommitLive` 每次有效字段变化/ROI 提交都调用 `Apply`，重读当前全部可见字段、提交全部参数和 ROI；`ToolConfigurationHost` 的 Apply → `GraphEditorDocument.SetProcedureToolConfiguration` → `Edit` → `Changed` → 主窗 `RefreshAll`。`SetProcedureToolConfiguration` 没有可见的等值短路，故单字段变动也可能触发全图重建和右侧位图刷新。当前实现链是确定的；是否达到本次卡死或比原版慢，尚无耗时证据。探针：受控实例 1/16/64 节点、1/8/32 条边、1K/4K 图像，对单次配置提交统计 `Edit`、`Changed`、`RenderGraph`、像素复制次数和耗时。先拿原版配置 setter/事件链核对写入粒度，再讨论增量刷新，不能凭一般 UI 优化习惯直接改。

### P1：自运行与全流程运行的输入准备和结果来源不同（需明示边界）

`RunConfiguredTool` 消费既有成功缓存；`NodeDebugExecutor.ResolveInput` 明确“不执行生产者”；整流程 `SolutionExecutor` 则沿图运行。若图像源→二值化等节点已建立连接但从未整流程运行，打开配置页可能有来源标签而没有可预览图像，单节点运行会输入缺失。该区别由当前代码确认；原版 SelfRun 是否先准备上游，已有 `NodeDebugExecutor` 注释明确未重建，原版链待 REA 恢复。探针需区分“仅连线未运行”“上游有一次成功输出”“上游参数变更后缓存失效”“整流程执行”四状态，逐端口记录绑定、输入快照、运行错误与结果来源。

### P2：结果语义/端口映射需要逐工具同一张矩阵核对

原版 `IMVSBinaryModu.xml:103-116` 声明 `OutputImage` IMAGE Combination 和可选 `OutMask`；当前 `ImageBinaryPlugin.Descriptor` 提供 `Image`、可选 `OutMask`、独立 `ProcessedPixels`。当前主窗 `RefreshRight` 和下游订阅按托管 descriptor/端口处理。`OutputImage`→`Image` 是映射而不是原厂相同 ID，`ProcessedPixels` 不能冒称原版结果。与图像源、轮廓匹配、标定工具联审时，必须记录原端口名/类型/基数/是否可自动订阅、当前端口名、错误与无输出状态，并在保存重开、Group、复制粘贴及上下游更换后复核。当前无证据断言这些路径全部错误；这是防止跨项目链路偏差的明确审核门槛。

## 3. 原始证据与下一步交接

- 安装资源：上述 `IMVSBinaryModuAlgorithmTab.xml`、`IMVSBinaryModu.xml`；SDK 头 `D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Includes/Algorithms/ImageBinaryCpp.h:66-150` 给出 `SetInputImage`、`SetROI`、`Run` 等接口。接口声明不提供像素算法体或完整运行时序。
- 既有审计：`docs/plugins/IMAGE-BINARY-SOURCE-AUDIT.md` 和 `docs/ROI-REAL-TOOL.md`；其中记载的原生导出路径本次缺席，应从原始二进制重新导出/核 SHA 后继续，不可把旧报告当新证据。
- 联合审查建议优先顺序：① 无响应受控堆栈/分段耗时定因；② 二值化五模式的原始通用算法源/汇编与实际运行参数；③ 统一“上游连接→候选/预览→自运行/全流程→结果/失败→持久化”端口矩阵；④ 再落实最小修复并复核其他已接入插件。插件线拥有二值化算法/配置 UI，main 拥有调度、状态、边界合同；跨项目变更按 `docs/PLUGIN-FRAMEWORK-BOUNDARY.md` 与 `docs/CONTRACT-REQUESTS.md` 记录，不能直接碰在制 `tool-plugins`。

本线只完成只读审查和交接，不把该联合审查计为主框架新增实施完成单元；既有实施分母/进度由根线权威台账决定。
