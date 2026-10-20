# J12 第四轮：真实图像流程完成后的空闲 UI 分段取证

## 范围、基线和结论

- 取证基线：`main` `31f0dee5f345e6c73cf2e2d3e14d91987b049547`，工作区另有主线在制文件；本轮只写 `tests/WorkflowIdleUi.Checks/`、本文和忽略目录 `analysis/joint-audit/idle-ui/`，未触碰主线源码、插件工作树、用户窗口或用户进程。
- **实测发现**：自有离屏 WPF `MainWindow` 在真实 `ImageSource → ImageBinary` 流程成功后，空闲 1.5 秒期间持续收到 Dispatcher 心跳；刷新、节点选择、流程切换、文档保存和关闭在本探针中均未出现长停顿。随后打开当前二值化配置窗口，完整宿主调用同步占用 UI 线程 **1.54–5.95 秒**（不同受控运行），明确可造成临时“未响应”。即使 64×64 小图，禁用 dump 的打开仍为 **3.94 秒**。关闭约 **0.30–0.47 秒**。这仅证明当前重建版的离屏配置打开热点；用户那次看似空闲后的永久卡死**未复现、根因未定位**。
- 分段工厂探针使用简化 `ToolEditorContext`：64×64 时 `ImageBinaryWindow` 构造 **292 ms**，随后 `Window.Show()` **2599 ms**，关闭 **430 ms**；长段主要在 `Show` 触发的 WPF 加载/布局/呈现调用中，但简化上下文与完整 `OpenToolConfiguration` 不同，**不能进一步归咎某个绑定、资源或 ROI 子步骤**。未做任何插件窗口性能修复。

## 源码调用链与边界

`src/VisionEditor/WorkflowExecution.cs` 的 `StartRunCore → RunCore` 在 `Task.Run` 内启动 `SolutionExecutor.RunAsync`，完成后 UI 延续段将 `ExecutionSessionResult` 写入 `workflowSessions`、刷新结果。`src/VisionEditor/WorkflowTabs.cs` 的 `RefreshAll`、`SelectWorkflowNode`、`SelectWorkflow` 是本探针分段入口。双击节点走 `src/VisionEditor/MainWindow.xaml.cs` 的 `OpenNode → OpenToolConfiguration`；`src/VisionEditor/ToolConfigurationHost.cs` 在 UI 线程创建上下文，接 `ConnectToolEditorServices` 与 `ConnectConfigurationRoi`，再由 `IWpfToolEditorFactory.CreateWpfEditor` 构造窗口。`src/VisionPluginContracts/ToolConfigurationWindowHost.cs` 的 `Open` 同步执行 `create()`、设置 `Owner`、`window.Show()`、`Changed → RefreshAll`；`Close` 同步调用 `Current.Close()`，Closed 事件再次 `Changed → RefreshAll`。`src/VisionPlugins.ImageBinary/ImageBinaryPlugin.cs` 工厂创建 `ImageBinaryWindow`；其构造函数在 `src/VisionPlugins.ImageBinary/ImageBinaryWindow.xaml.cs` 初始化资源、ROI、图像、多个参数页和控件。上述是当前实现的路径，不代表原厂的性能或线程行为。

原厂入口边仍按前轮已取证路径 `VmProcedureDecorator.RunOnceCommandExe → VmProcedure.Run(null,bIsWait:false) → ProcessManager.SyncExecuteOnce → IMVS_ExecuteOnce_V30` 保留；`bIsWait:false` 只排除托管等待，原生调用持续时间未测。本轮无原厂 UI 堆栈，不拿重建版计时推断原厂挂起原因。

## 可复验探针与观测

`tests/WorkflowIdleUi.Checks/Program.cs` 的自有窗口位于 `(-20000,-20000)`，禁自动相机发现；内存构造 GrayImage，通过真实 ImageSource 提交并运行二值化，确认节点 `Success`。`DispatcherTimer` 每 20 ms 记录心跳；每个同步 UI 操作前后分段计时，段间泵 Dispatcher。后台 watchdog 只监视自有测试 PID，心跳缺失 >1 秒留事件，可选 `--dump` 生成 `MiniDumpWithThreadInfo`，自有进程总时限 20 秒。保存使用 `GraphEditorDocument.Save` 到审计目录，避免主窗口最近文件副作用。详见 `analysis/joint-audit/idle-ui/preflight.md`。

从仓库根目录执行 `dotnet run --project tests/WorkflowIdleUi.Checks/WorkflowIdleUi.Checks.csproj -c Release -- no-dump-2048 2048`；小图将末尾两个参数改为 `no-dump-64 64`；简化拆段运行 `-- split-64 64 --factory-split`。测试预期在心跳超 1 秒时退出码 1 并保留分段日志；不要把它当编译或方案执行失败。

| 运行 | 图像 | 取证方式 | 打开/构造 | Show | 关闭 | 其余可比操作 |
|---|---:|---|---:|---:|---:|---|
| 首轮连续调用 | 2048² | 2 秒阈值，dump | 1538.58 ms | 包含在打开 | 297.76 ms | RefreshAll 12.9–35.3 ms、选择 29–47 ms、切换 13–18 ms、保存 30.9 ms；因连续不让步，在关闭段才累计超过 2 秒触发 dump |
| 分段让步 | 2048² | 1 秒阈值，dump | 6087.55 ms | 包含在打开 | 424.53 ms | watchdog 在打开段约 1.07 秒心跳缺失时留 dump；dump 可能增加总时间 |
| 分段让步 | 2048² | **不写 dump** | 5946.11 ms | 包含在打开 | 474.92 ms | 其余刷新/选择/切换约 19–58 ms，保存 38.5 ms |
| 分段让步 | 64² | **不写 dump** | 3943.74 ms | 包含在打开 | 424.71 ms | 其余刷新/选择/切换约 12–33 ms，保存 21.2 ms |
| 简化上下文拆段 | 64² | **不写 dump** | 构造 292.08 ms | 2599.02 ms | 429.75 ms | 不等于完整宿主上下文，只用于定位大段所处同步边界 |

首轮 `maxPulseGapMs=116.6` 的记录不完整：那版只在下一次 Timer.Tick 更新，关闭前停止计时器使长同步段未计入；后续版本记录每段 `endGapMs`，watchdog 事件证明 >1 秒缺失。探针现将超阈值作为**失败**退出码 1，避免把配置打开卡顿误报 PASS。首次编译曾因测试缺 `using System.IO` 失败，已仅在测试源中修正；失败历史不覆盖。上述五次均最终正常关闭自有窗口，未触发 20 秒总超时；这也不能证明长期使用不会卡死。

## 审计定位与未解点

- 探针：`tests/WorkflowIdleUi.Checks/Program.cs` SHA256 `9F6A76699464A23D7CF778E0C49C40F3A727E4D744CA15F9EDC1C97B5B340165`。
- 首轮自有 minidump：`analysis/joint-audit/idle-ui/idle-ui-threaddump.dmp` SHA256 `9DC5D06279DDE909231CB2E2EE18C8C766D82B621112F92A85888CB8A5E37414`；分段让步 dump：`analysis/joint-audit/idle-ui/yielded/idle-ui-threaddump.dmp` SHA256 `6778FFC7B1217C28E10570C2D0E6C0C6A40D8A809156B6EF592EE52A2CCB09E0`。这台机尚未找到可用 `cdb`、`windbg`、`dotnet-dump`，**尚未解析 UI 栈**，不能从 dump 猜测锁或等待链。
- 禁 dump 的 2048²、64²、简化拆段完整控制台输出分别为 `analysis/joint-audit/idle-ui/no-dump-2048-console.txt` SHA256 `85AFA083A735ABC66A5A1BA48A3C7F2DA7BA340D32ADD53B9B22E0D4C7CA1F3B`、`no-dump-64-console.txt` SHA256 `69A04FA79A093EFB12D0D6A9ED583B04BEF6D48FE678B6C5A0D8E16EC3C1BF97`、`split-64-console.txt` SHA256 `506B457E17CFB7D7CFAA7AD19E3DB51E7E7F5DCB71EBD2F320171B66D672EC3B`。
- 下一最小探针：在**自有进程**加载可用符号调试器或 WPF ETW/采样器，解析上述 dump 的 UI 线程栈；或在独占窗口宿主中按 `create()`、`Show` 内部 Loaded/布局/ROI `SetImage`/`Changed` 回调分别取时间。须先确认窗口构造中注册的回调副作用，并保留无 dump 对照。用户交互桌面的长期空闲卡死需独立可复现时间窗及 UI 栈/等待链；目前没有证据把该事件归因于 ImageBinary、图像源、GraphExecutor、锁或原生 SDK。
