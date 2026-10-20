# J12 第五轮：工具配置窗口 Show 与主框架接缝分段取证

## 结论及适用范围

本轮在独占离屏 WPF 进程中运行真实 `ImageSource → ImageBinary` 并确认二值化节点成功，等待空闲后测主框架 `OpenToolConfiguration → ToolConfigurationWindowHost.Open → ImageBinaryWindow`。当前宿主调用 `OpenNode` 同步占用 UI 线程 **8060 ms**（64² 受控帧，本次样本）；`ImageBinaryWindow.Loaded` 在调用开始后约 **7971 ms** 才出现，`ToolConfigurationWindowHost.Changed`（已执行主线订阅的 `RefreshAll`）距 Loaded 约 **89 ms**。因此本次大段停顿发生在 Changed/后续 RefreshAll **之前**，而非普通流程运行或结果提交。此为当前实现的临时 UI 停顿证据；用户所述无操作后的长期卡死仍没有同一调用栈或复现条件，**不能认定为同一根因**。

简化工厂上下文下，窗口构造 144 ms、`Window.Show()` 1956 ms；同一自有 MainWindow 中直接显示 `GroupRoiEditor`，64² 帧 Show 2944 ms、无输入图 Show 1351 ms、空 `Window` Show 118 ms。另一次 2048² ROI 对照中 `SetImage` 4.8 ms、Show 1265 ms。跨独立进程存在明显冷启动/环境波动，不能用这些差值归因某个控件、图像复制、资源或 WPF 内部步骤；能下的最窄结论是**主框架同步打开窗口的接缝覆盖一个可观的 Show/首轮布局时段**，ROI 窗口树即使无输入图也有 >1 秒样本。插件内布局/ROI 优化归插件实现所有者，本轮只作跨框架接缝定位，不改任何产品代码。

## 静态调用与事件边界

- `src/VisionEditor/MainWindow.xaml.cs`：`OpenNode → OpenToolConfiguration`。
- `src/VisionEditor/ToolConfigurationHost.cs`：主框架在 `OpenToolConfiguration` 构造 `ToolEditorContext`，接 `ConnectToolEditorServices`、`ConnectConfigurationRoi`，交给 `ConfigurationWindows().Open`；窗口工厂依据插件接口创建专用编辑器。主线的 `Changed` 处理器更新 `parameterEditorOpen` 并 `RefreshAll`。
- `src/VisionPluginContracts/ToolConfigurationWindowHost.cs`：`Open` 同步 `create()` → `Owner`/`Current` 赋值 → `window.Show()` → `Changed`。`Close` 同步 `Current.Close()`，Closed 再触发 Changed。这个接缝没有调度让步，故 Show 或构造长时工作会阻塞同一 UI 线程。
- `src/VisionPlugins.ImageBinary/ImageBinaryPlugin.cs` 与 `ImageBinaryWindow.xaml.cs`：工厂创建 `ImageBinaryWindow`；构造执行资源合并、`GroupRoiEditor` 创建及 `SetImage`、参数页构造。`src/VisionUi/GroupRoiEditor.cs` 的 `SetImage` 在内存复制像素并创建 `BitmapSource`，但本轮对照并未证明它是 Show 停顿的主因。
- WPF `Loaded`、`LayoutUpdated`、`SourceInitialized`、`ContentRendered` 只记录了事件到达的单调时间。各事件可能在 `Window.Show` 同步返回前触发，也可能经 Dispatcher 排队；本轮不把事件先后顺序解释为 WPF 原生内部函数耗时或 GPU 等待。

## 独占探针与实测

测试 `tests/WorkflowShowPhase.Checks/`，副作用预检见忽略目录 `analysis/joint-audit/ui-show/preflight.md`。每个 mode 是独立进程，自己的不可见主窗和子窗；禁自动相机发现，无设备/安装原件/用户窗口访问。20 ms Dispatcher 心跳及 >1 秒 watchdog 只记录本进程活动段，30 秒总时限只退出本测试 PID。所有运行正常退出码 0、`mainClosed=true`；长 Show 时心跳确实未执行，不能因最终退出 0 认为交互流畅。

| mode/帧 | 直接构造/资源 | 同步 Show 或宿主 OpenNode | Loaded 或首个布局标记 | 关闭子窗 |
|---|---:|---:|---|---:|
| `host 64` | 完整主框架上下文，未单独计 | `OpenNode` 8060.29 ms | `binary-Loaded` 在 OpenNode 起后 7970.95 ms；Changed 在 Loaded 后 89.33 ms | 124.40 ms |
| `factory 64` | 资源缓存读取 5.04 ms；二值化窗口 ctor 144.45 ms | `Window.Show` 1956.38 ms | 首个 LayoutUpdated 起后 1365.75 ms；Loaded 起后 1912.23 ms；ContentRendered 在 Show 返回后 | 101.55 ms |
| `roi 64` | ROI ctor 36.51 ms；`SetImage` 0.96 ms | `Window.Show` 2944.46 ms | 首个 LayoutUpdated 起后 2926.37 ms | 130.86 ms |
| `roi-noimage 64` | ROI ctor 47.80 ms；未 SetImage | `Window.Show` 1350.70 ms | 首个 LayoutUpdated 起后 1337.37 ms | 151.25 ms |
| `empty 64` | 空 Grid | `Window.Show` 118.18 ms | 首个 LayoutUpdated 起后 95.57 ms | 133.72 ms |
| `roi 2048` | ROI ctor 45.34 ms；`SetImage` 4.78 ms | `Window.Show` 1264.67 ms | 首个 LayoutUpdated 起后 1251.47 ms | 166.03 ms |

复验命令（仓库根目录）：`dotnet run --project tests/WorkflowShowPhase.Checks/WorkflowShowPhase.Checks.csproj -c Release -- host 64`；将末尾改为 `factory 64`、`roi 64`、`roi-noimage 64`、`empty 64` 或 `roi 2048`。报告中的事件和分段时间取各自 `analysis/joint-audit/ui-show/<mode>-<size>/phases.json`；这些模式的简单 ROI/空窗口只是**隔离对照**，不是原厂配置窗口或完整主框架宿主。

## 审计哈希、排除项与下一证据

- 探针源码 `tests/WorkflowShowPhase.Checks/Program.cs` SHA256 `345A76843401DE4DC28FAA8133EEB4891E6D2433C706B4489F5B0023EFBEBFAD`；csproj SHA256 `FE121D76E786E9D902068A040DFF73D8091E0E65AB1A53F81A54A5935C1B8DA4`。
- `host-64/phases.json` SHA256 `9EBE59810815A5FC74330416C935CA8C687B4B6B8F36ED80866B45A3833E1DA5`；`factory-64/phases.json` `2719D07DB07C5CF43AED23E6F337BD6FD2A35F75F0DDCE1A364572CFA01D038A`；`roi-64/phases.json` `7FA21ECA194741526D5697153CFDA03FFC55A4D3884C5A19179A4B8A5CC355CF`；`roi-noimage-64/phases.json` `F88F55FCEF5E11870F31E6C6F27F5AA5149C39F35686F32A84E6EE785027B08E`；`empty-64/phases.json` `F2BDDA73AF67EB5AC393BD08043D2B3D00BDE6138D39733B25347846729CB2F3`；`roi-2048/phases.json` `A0F6A47FEACFF32ABC2901E226EACF14C12BF8D998BC36C7849CA4737A87C3E8`。
- 首次编译测试因缺 `using VisionEditor.Core` 失败，随后只修测试引用，所有上述 mode 成功。产品 `MainWindow.xaml.cs` 的既有 CS8603 警告仍在，不由此归因 UI 停顿。
- 现有自有 minidump 未解析出符号化 UI 栈；本轮无 ETW 调用树，也无原厂同操作计时。**未证**是 WPF 布局循环、ROI 渲染、资源加载、消息泵、图像复制或插件运行时锁。若继续，应在自有宿主按 `create()` / `Show` 内部 WPF 加载事件与调用栈采样拆分，并对照原厂源中配置窗真正打开路径；用户长期卡死需其发生时的可归属 UI 线程栈/等待链，不由这组临时停顿代替。
