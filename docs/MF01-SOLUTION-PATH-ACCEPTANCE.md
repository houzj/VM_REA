# MF-01 标题栏方案路径命令验收

原始证据：`VisionMasterMain.AppMainWindow/TitleBar.xaml` 的方案名按钮绑定 `OpenSolPathCommand`，提示绑定 `SolutionPath`；`VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs` 的 `OpenSolutionPathCmdExcute` 在非远程模式、有方案名时检查目录和方案文件，再执行 `Explorer /select,<方案路径>`。原生远程模式及用户名/权限状态未在当前独立宿主中恢复。

本线 WPF 标题方案名已成为可点击按钮。保存过的本地方案在目录和文件均存在时由系统文件管理器定位；未保存方案不执行；丢失目录/文件时拒绝外部启动并在状态栏给出对应错误。外部进程启动器以委托隔离，测试可核对精确文件路径而不在自动验收时打开 Explorer；产品默认使用真正的 `explorer.exe /select`。

`tests/VisionEditor.ShellSolutionPathChecks` 实际显示主窗口、点击标题栏：新方案不调用启动器，保存后点击传递真实方案文件，删除测试文件后点击不再调用并报告“方案不存在!”。两组专项检查通过；原有 `VisionEditor.ShellChecks` 22 项回归通过。原生路径缺失时的 HikMessageBox 样式、远程模式分支与 Explorer 窗口像素差分未验。

后续标题状态闭合：原始 `MainWindowData.SolutionName` 从方案文件路径取得，加载方案后通知 `SolutionName` / `SolutionPath` 变化。当前 WPF 标题栏在方案新建、保存与切换时同步实际文件名；独立格式 `.vmx.json` 按完整后缀剥离。专项已操作新建后标题为空、保存到 `mf01-solution-path.vmx.json` 后标题为 `mf01-solution-path`，再点击定位文件。原生 `.sol` 的完整打开/保存格式属于方案生命周期清单，不因标题状态测试而宣称兼容。
