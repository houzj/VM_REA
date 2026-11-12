# C01 启动壳层整批实施

本批只落地原厂证据已经闭合的启动壳层，不宣称完整原厂启动链。

## 已实现

- `WelcomeDialog` 的 640×360 无边框窗口和安装包 `WelcomePage.GIF` 资源。
- `GuideWindow` 的 800×546 方案中心布局、四类方案入口、最近方案列表、文件打开入口和“不再显示”开关。
- 原厂 GuideWindow 资源从已安装 `VisionMasterMain.dll` 的 `g.resources` 提取，保留原始文件名和字节内容。
- 对照原厂 XAML 补齐了 GuideWindow 的对角纹理背景、卡片字体层级、最近方案标题和无滚动条布局、橙色分隔标记、打开方案图标及底部三组 `ic_1`/`ic_2`/`ic_3` 导航图标。新增资源均从同一 `g.resources` 原始条目提取。
- 关闭主窗口后将应用生命周期设为 `OnMainWindowClose`，释放单实例互斥体，避免窗口关闭后后台进程阻塞下一次启动。
- `Program.Main` 的工作目录切换和单实例互斥体。
- `BeforeStartPart` 对应的后台 STA 初始化线程、完成屏障和开发模式认证跳过。
- 启动参数中 `.vmx.json` 的独立方案路径分流。
- 原厂 `.sol` 参数的显式未接入提示，不把未知容器当作 JSON 读取。
- 未接入认证和加密狗时继续进入方案中心和编辑器的开发模式。
- 通用方案入口创建当前独立空方案，最近方案入口进入当前独立 JSON 方案，示例卡片映射到仓库已有示例文件。

## 证据边界

`WelcomeDialog.xaml/.cs`、`GuideWindow.xaml/.cs`、`AppAfterStart`、`Program.Main` 和已安装资源已经提供上述窗口尺寸、资源名、按钮处理器、最近方案绑定和命令行入口证据。示例卡片到仓库 JSON 文件的映射、单实例互斥体名称和开发模式认证跳过属于独立实现选择，不能写成原厂字段或行为。

## 暂缓项目

- 原厂 `MainWindowData.PreLoad` 与 `ServerManager.InitServer` 的等价主数据容器。
- SDK 自动加载状态 `IMVS_AUTO_LOAD_SOLUTION_INFO`。
- 原厂 `.sol` 容器解码、密码、流程/场景载荷和失败回滚。
- `GuideHelper.OpenSolution` 的原厂示例路径及 `WecPage` 的完整返回对象。

这些项目保持“运行未确认”或“原件未取全”，因此本批不能宣称完整启动链等价。Release 构建、`VisionEditor.StartupShellChecks`、框架方案链和 Shell 方案路径回归通过。启动壳层可由用户直接启动当前构建进行人工验收，方案容器和场景载荷仍单列为未确认行为。

## 本轮构建与测试验证（2026-11-11）

### 阻塞原因（源码确认）

`Program.cs` 启动异常诊断补丁中的 `MessageBox.Show` 未做命名空间限定，而 `VisionEditor.csproj` 同时启用了 `UseWindowsForms` 与 `UseWPF`，编译报 `CS0104`（`System.Windows.Forms.MessageBox` 与 `System.Windows.MessageBox` 二义性，`Program.cs(59,13)`）。该补丁加入后 `VisionEditor.csproj` 实际无法编译，标准 Release 产物 `VisionEditor.exe` 停留在 `13:31:28`，早于 `Program.cs` 的 `13:52:43`，因此可确认诊断补丁此前没有进入任何 Release 产物。

### 修复

唯一代码改动是把该调用限定为 `System.Windows.MessageBox.Show`，其余启动链、UI 和方案代码未改。

### 验证结果

- 隔离构建 `artifacts/codebuddy-verify/VisionEditor/VisionEditor.exe`：0 警告、0 错误。
- 测试构建 `artifacts/codebuddy-verify/StartupShellChecks/`（含 `MainWindow` 构造）：0 警告、0 错误；两处构建均使用独立 `OutDir`、`UseSharedCompilation=false`、`BuildInParallel=false`。
- 运行 `VisionEditor.StartupShellChecks.exe` 退出码 0，输出 `VisionEditor.StartupShellChecks: WelcomeDialog and GuideWindow contracts passed.`。该断言位于 `MainWindow` 构造与 `ui-assets/original-guide/WelcomePage.GIF`、`guide_general.png` 存在性校验之后，因此通过同时覆盖 `MainWindow` 构造。
- 标准 Release 已重建：`src/VisionEditor/bin/Release/net10.0-windows/VisionEditor.exe` 时间戳 `13:55:33`，`ui-assets/original-guide/` 含 20 个文件。
- `%LOCALAPPDATA%\VisionEditor\startup-error.log` 当前不存在，无残留 `VisionEditor.exe` 进程。

### 仍未确认

欢迎页、方案中心和关闭行为的桌面人工验收仍由用户完成；本轮只提供编译、单元级窗口契约和产物层证据，不构成原厂启动链等价结论。

## 统一重写（2026-11-11，取代上文的批次式实现）

上文"已实现"一节描述的 `Program.Main` + `DispatcherTimer` + `ShutdownMode.OnMainWindowClose` 结构，正是"过欢迎页即退出"的根因：欢迎页是第一个被构造的 `Window`，被 WPF 自动赋值为 `Application.MainWindow`，`splash.Close()` 于是直接触发应用关停。根因链与逐行证据见 `docs/reverse/C01-shell-host-reconciliation-matrix-20261111.md` 第 1 节。

按用户"先完整逆向分析、再统一对照实现"的要求，启动壳与主窗口壳已整体重写：

- 新增 `App.cs`：`OnStartup` 内复现原厂时序——欢迎页独立 STA 线程/独立 `Dispatcher` ∥ 后台初始化线程 → 主线程预加载 → 双线屏障 → 关闭欢迎页 → 方案中心门控 → 主窗口。
- 关停模式固定为 `OnExplicitShutdown`；关闭走 `IApplicationOperation.Dispose()` + `Environment.Exit(0)`，与原厂 `App.MainWindow_Closing` 一致。
- 新增 `MainWindowData` 宿主数据、`IsShowProgressMask` / `MaskMessage` 依赖属性与遮罩层、窗口状态持久化与恢复、标题取自 `ApplicationName`。
- 方案中心：卡片语义改为写 `ToolBoxType` 并关闭；最近方案改为依赖属性绑定；"不再显示"与欢迎页共用 `ShowEnable` 单开关；关闭按钮 `DialogResult=true`。
- 单实例修正非持有者释放问题，并补命名管道参数转发。
- 授权只保留 `IAuthGate` 恒放行占位与提醒调用点。

行为契约、对照矩阵与回归结果分别在 `docs/reverse/C01-startup-chain-behavior-contract-20261111.md`、`docs/reverse/C01-shell-host-reconciliation-matrix-20261111.md`、`docs/reverse/C01-startup-lifecycle-regression-20261111.md`。第 5 节"最近启动故障"中原先的 `OnMainWindowClose` 结论已被本重写取代。
