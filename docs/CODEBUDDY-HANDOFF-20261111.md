# VisionMaster 逆向交接说明

## 1. 当前目标和工作约束

工作树：`D:\MyWork\Rea逆向`

逆向标准以原厂反编译源码、原始资源、PE/汇编和已确认运行证据为依据。证据不足时必须区分源码确认、当前独立实现和运行未确认，不能按产品意图补写原厂行为。主框架线负责启动壳、方案生命周期、流程画布、Group、执行调度和公共合同。工具插件线负责工具特有实现。不要修改工具插件工作树。人工启动和桌面验收由用户完成。

当前固定进度口径为框架 **41/96（42.71%）**、工具 **16/162（9.88%）**。历史文档中的 `42/96（43.75%）` 保留为历史对账，不作为当前完成度。

## 2. C02 共享执行生命周期

主要修复文件：

- `src/VisionFramework/GraphExecutor.cs`
- `src/VisionFramework/SolutionExecutor.cs`
- `src/VisionFramework/ModuleExecutionScope.cs`
- `src/VisionFramework/ModuleExecutionScope.Parameters.cs`
- `src/VisionFramework/ToolDebugSession.cs`
- `src/VisionPluginContracts/ToolConfigurationExecution.cs`

已完成并自审的独立实现行为包括单次执行和同步等待、取消与停止、资源清理、回调队列异常隔离、参数快照、执行范围、结果缓存、调试会话、服务替换、方案切换、Group 边界和子流程错误传播。

最新回归摘要：

`analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/summary.json`

摘要 SHA-256：

`55AA3FAF46BAC752F6083BEBDA65245758767D73C1CE9C84AD11AEC44E922DCF`

结果为 11 个套件、462 项检查全部通过。

身份审计：

`analysis/full-audit/batch1-20261107/C02/lifecycle-regression-495f7b05aab7469e9bd08ee3633d1496/source-identity-audit.json`

结果为 13 对程序集/PDB 和 87 份源码文件通过。

报告：`docs/reverse/C02-shared-execution-lifecycle-20261110.md`

## 3. C03 Blob 同输入差分

固定样图：`D:\MyWork\测试图片\标定\1.jpg`

样图 SHA-256：

`3A77112B982DF494A74CD756CD412D045210BADE88604BE52AB875C972CA303E`

直接 SDK 矩阵：

`analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-4dd306f7d33a447dba8e0793ad073807/summary.json`

已完成 26 次 SDK 运行，复现默认区域 `47 / 973202` 和显式全图矩形 `45 / 971486`，覆盖输入缓冲、BinaryImage、BlobImage、输出开关和重复性。

新增输出比较：

`analysis/full-audit/batch1-20261107/C03/sdk-region-matrix-4dd306f7d33a447dba8e0793ad073807/sdk-buffer-comparison.json`

SHA-256：

`739A8CFAAC7A60EA1E565ED34AA16C88FDFB63FDAF188AD4DB2F17BEBC484707`

报告：`docs/reverse/C03-same-input-differential-20261110.md`

原厂宿主差分仍未闭合。历史方案 `test1108.sol` 不在当前机器，原厂宿主依赖和归档依赖存在哈希差异，原厂 Run 前像素、活动 ROI/Mask、完整结果向量和首个运行时分歧均保持“运行未确认”。不要用 SDK 结果推断原厂宿主输入。

## 4. 启动链和方案中心

主要文件：

- `src/VisionEditor/Program.cs`
- `src/VisionEditor/StartupLaunch.cs`
- `src/VisionEditor/StartupWelcomeWindow.xaml`
- `src/VisionEditor/StartupGuideWindow.xaml`
- `src/VisionEditor/StartupGuideWindow.xaml.cs`
- `src/VisionEditor/MainWindow.StartupSolution.cs`

已按原厂 XAML 和资源恢复欢迎页 640×360、GuideWindow 800×546、原厂欢迎 GIF、Guide Logo、四张方案卡片、hover 资源、对角纹理背景、卡片字体层级、最近方案标题、打开方案图标、底部 `ic_1`/`ic_2`/`ic_3` 图标和“不再显示”开关。

原厂 GuideWindow 证据：

`D:\MyWork\SunEyeVision_V2-param-window-datatree\docs\07-工具逆向\decompiled\HostShell\Main\VisionMaster.AppMainWindow\GuideWindow.xaml`

提取资源目录：

`analysis/ui-assets/original-guide/`

启动门禁和报告：

- `docs/reverse/C01-startup-chain-design-review-20261111.md`
- `docs/reverse/C01-startup-implementation-gate.json`
- `docs/reverse/C01-startup-shell-implementation-20261111.md`

独立 JSON 方案已支持创建、保存、重开和执行。原厂 `.sol` 容器、SDK 自动加载和方案直启场景仍未实现，保持运行未确认。

## 5. 最近启动故障和当前修复

用户关闭窗口后曾留下无窗口 `VisionEditor` 进程，持有单实例互斥体和程序集文件，导致下一次启动立即返回。`Program.cs` 已将应用关闭策略从 `OnExplicitShutdown` 改为 `OnMainWindowClose`，确保主窗口关闭后进程退出并释放互斥体。

最近又加入启动异常诊断：

- `Application.DispatcherUnhandledException`
- `%LOCALAPPDATA%\\VisionEditor\\startup-error.log`
- 启动阶段、时间和完整异常堆栈记录
- 启动失败时显示日志路径

诊断补丁位于 `src/VisionEditor/Program.cs` 和 `src/VisionEditor/StartupLaunch.cs`。该补丁加入后尚未确认已写入标准 Release 产物，接手后应重新构建。

## 6. 当前构建和测试状态

隔离构建曾生成：

`artifacts/guide-shell-verify/VisionEditor.exe`

启动壳测试曾通过：

`VisionEditor.StartupShellChecks: WelcomeDialog and GuideWindow contracts passed.`

最近把 `MainWindow` 构造加入 `tests/VisionEditor.StartupShellChecks/Program.cs`，但加入后的完整测试尚未完成最终验证。测试构建曾受到 `obj` 下 DLL 被 dotnet 编译进程占用影响，建议使用独立 `OutDir`、`UseSharedCompilation=false` 和 `BuildInParallel=false`。

标准启动路径：

`D:\MyWork\Rea逆向\src\VisionEditor\bin\Release\net10.0-windows\VisionEditor.exe`

## 7. CodeBuddy 接手后的执行顺序

1. 先用独立输出目录重新构建 `VisionEditor.csproj`，确认诊断补丁和 UI 修复进入产物。
2. 构建并运行扩展后的 `VisionEditor.StartupShellChecks`，重点验证 `MainWindow` 构造、默认方案恢复和关闭行为。
3. 用户手动启动标准 Release。若欢迎页后退出，读取 `%LOCALAPPDATA%\\VisionEditor\\startup-error.log`。
4. 若无法再次启动，检查并清理残留的 `VisionEditor.exe` 进程，然后重新构建标准 Release。
5. 修复后同步 `docs/reverse/C01-startup-shell-implementation-20261111.md`、`docs/VM-LOCAL-RESTORATION-STATUS.json` 和 `docs/reverse/C02-C03-final-delivery-manifest-20261111.json`。
6. 不要在缺少 `test1108.sol`、匹配原厂宿主和 Run 前缓冲证据时修改 Blob 私有算法或宣称原厂等价。

## 8. 当前工作树注意事项

工作树存在前序交付的大量未提交修改和新增文件。不要重置整个工作树，不要删除 `analysis/full-audit` 历史证据，也不要修改另一个工具插件工作树。计划页面和完整任务清单最近已重新生成，验证器结果为 `inputsStable: true`、`deterministic: true`。

