# MF-01：运行模式 / 运行界面入口源级审计

## 源码证据与调用链

- `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml:84,103-104` 将原生“设置 > 运行策略”映射至 `TitleMenueItemData.ProcRunSet`。`TitleMenueItemData.cs:763-766` 调用 `SoftwareSetDlgShow("itemprocrunset")`；`SoftwareSetDlgShow`（`:676-715`）创建 `SoftwareSetDlg`，将 `Program.MainData.ProcRunPolicyData` 绑定至 `procedureRunPage`，先执行 `LoadRunPolicyConfig()`，再显示对话框。`Sdg_ApplyEventHander`（`:708` 起）在 Apply 时调用 `SetRunPolicyConfig()`；取消路径没有已确认的保存调用。
- `analysis/decompiled/VisionMasterMain/VisionMasterMain/ProcedureRunPage.xaml:115` 的 `ComboBoxOperationMode` 以 `SelectedIndex="{Binding RunningModeIndex}"` 绑定，并将 `OperationModeOptions` 作为选项。`VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/ProcedureRunData.cs:18,45-57` 存储索引并在变动时触发属性通知；`:34-35,243-246` 的两项为 `Normal Mode` / `诊断模式`，中文资源分别为普通模式和诊断模式。
- `ProcedureRunData.cs:229-245` 的 `ReloadProcRunStragety()` 读取 `ConfigurationHelper.GetConfig("RunningMode")`，非 1 映射普通模式索引 0，1 映射索引 1，并更新 `MainData.IsNormalMode`。`app.config:90` 默认 `RunningMode=0`。Apply 保存时，`:316-327` 调用 `ConfigurationHelper.SaveConfig("RunningMode", index==1 ? "1" : "0")` 并再次同步 `IsNormalMode`。
- `VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs:1125,1138,1151` 证明该状态副作用仅限已见的资源告警分支：诊断模式会使内存/网络告警条件生效，并允许 CPU 告警路径；普通模式在这些条件上跳过告警。
- **另一个不同命令**：`menuitems.xml:405-415` 的 `Lang_FrontEdit` / `EditFrontendUI` 是工具栏“运行界面”命令。`TitleMenueItemData.cs:1545-1555` 创建外部 `FrontendUI.Design.EditFrontendWindow`、设置主窗 Owner 并 `ShowDialog()`。该外部设计器实现不在可读反编译源码中。没有证据证明它等同 `RunningMode` 选择。

## 当前代码差异与本轮实现

- 本仓库 `src/VisionEditor/MainWindow.xaml:46` 的右上角 `TextBlock`“▣ 运行模式”没有 Click、绑定或状态来源。它与原生“设置 > 运行策略”选择控件、工具栏 `EditFrontendUI` 三者的映射关系均未在源码里出现。故本轮不把静态文字擅自连到外部 FrontendUI 设计器。
- `MainWindow.xaml.cs` 的“设置”菜单现增加源可确认的“运行策略”命令。它打开轻量 WPF 选择窗，复现 `ProcedureRunPage` 已证实的两个 `ComboBox` 选项以及 Apply 时 `0/1` 写入和 `IsNormalMode` 同步。其他“策略模式”、每流程线程/进程配置仍未实现。
- `ShellRunningModePreference` 将索引存入独立 JSON `RunningMode` 字段。原 `ConfigurationHelper` 实现/写入目标未在提供的反编译源码中，因此该存储是明确的独立适配，并非已证实 VM `app.config` 写回行为。native tips 只在 XAML 引用 `Lang_NormalModeTip` / `Lang_DiagnosticModeTip`，资源值缺失，界面不仿造提示文案。
- 点击保存后，Shell 内 `shellIsNormalMode` 按 `index != 1` 更新；原生告警后端没有在此工作树复刻，因此该状态对框架外部没有额外运行副作用。

## WPF 操作验收状态

测试 `tests/VisionEditor.ShellChecks/Program.cs` 设置隔离偏好路径，确认缺省索引 0；通过真实 WPF `MenuItem` 进入“设置 > 运行策略”，在弹窗中操作 `ComboBox` 选择诊断模式，捕获 `analysis/full-audit/shell-running-mode-diagnostic.png`，点击实际 WPF Apply 按钮，核对窗口状态 `RunningModeIndex=1`、`IsNormalMode=false` 与持久化 JSON `RunningMode:1`，并在同一轮中再次读文件/调用 preference reader 得到 1。目检使用本 worktree 绝对路径 `D:\MyWork\REA逆向测试\worktrees\framework-shell\analysis\full-audit\shell-running-mode-diagnostic.png`：截图显示“运行模式”标签、诊断模式下拉框和右下取消/确定按钮，控件没有互相遮挡或错位，内容区没有裁切。图片仅是 WPF 操作证据，不证明与 VM 的窗口布局/皮肤完全一致。

构建/运行命令为 `dotnet run --project tests/VisionEditor.ShellChecks/VisionEditor.ShellChecks.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false`，使用隔离 `Directory.Build.props`（精确内容见 `MF01-TOOLBAR-UNDO-REDO.md`）。进程退出码 1；此前既有 39 项 PASS，本项新增 3 项 PASS，故 Recent Popup 检查前累计 42 项 PASS；随后在既有 Recent Popup 离屏宿主输入限制处失败。唯一编译告警是 `MainWindow.xaml.cs(123,224)` nullable warning。未重复构建或尝试合成输入。

**状态：两模式状态交互已操作验证；对原生运行策略只完成两模式子集，不声称完整页面复刻。** Recent Popup 离屏输入宿主问题与此项无关，仍保持原失败和截图断言，不注入合成鼠标以假装验收。`
