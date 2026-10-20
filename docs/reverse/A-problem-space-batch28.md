# 第二十八轮 A 线问题空间闭合审查

基线：`fbcf222f08505555f3a06db9fa116cc21e4fd90e`（第二十六轮提交）。问题是：在不依赖真实桌面验收的前提下，A02/A05/A06/A08 中是否存在由原源码确认、可在 main 独立形成完整状态闭环的壳/交互入口。

## 筛选结果

### A02 主窗口状态与 DPI

`src/VisionEditor/MainWindow.ShellWindowState.cs` 已实现标题栏最大化/还原状态、Tooltip 和资源图标同步；`src/VisionEditor/WindowWorkArea.cs` 已实现 `WM_GETMINMAXINFO` 按监视器工作区写入物理像素边界。现有 `tests/VisionEditor.ShellChecks/Program.cs` 覆盖状态切换、工作区边界、双击和工具栏输入隔离。这些属于历史已交付能力，不能重复计分。原多屏/DPI 同状态矩阵仍是人工/原机验收范围，不能作为新增实施单元。

### A05 菜单、工具栏及全局入口

`MainWindow.xaml.cs` 对文件、历史、帮助、运行和工作区锁等入口已有真实主框架消费；剩余全局变量、通信、全局触发、脚本、综合配置及服务入口明确保留“尚未接入”状态。原 `TitleMenueItemData` 的这些入口依赖外部服务/原生模块会话和错误/关闭合同，main 没有可证 producer。补空 handler 或窗口不能形成状态、持久化、执行闭环，因此 A05.1/A05.3 本轮不可关闭。

### A06 运行界面编辑与显示布局

原入口链已确认 `EditFrontendUI → EditFrontendWindow(DisplayMode=1) → FrontendEdit.Close → DoSave`，并依赖 `DocumentManager` 的 temp/solution/designer 三缓冲、MD5/NeedSave、保存/取消和关闭解绑。当前 main 只有离线 `FrontendDesignMigration` XML 转换，没有可编辑设计面、运行值消费者或三缓冲合同；A06.1/A06.2 继续阻塞，不能以资源记录读取或空窗口替代。

### A08 壳与交互全状态验收

A08 依赖 A02～A07 完整闭合，且 A08.2 的默认键鼠/多屏 DPI 属于后置人工桌面验收。本轮不运行桌面输入，也不把合成 WPF 事件当作原机证据，故无独立可实施单元。

## 结论

本轮没有找到可在不引入未证服务或设计器合同的情况下独立实施并自审核关闭的 A 单元，实施增量 **+0**。解除条件：A02 需新的原绑定/几何状态消费证据；A05 需具体服务 producer→session→execute/callback→error→close 合同；A06 需可编辑设计面和三缓冲快照消费；A08 需父项闭合后再做人工桌面矩阵。`tool-plugins` 未访问、未修改。

