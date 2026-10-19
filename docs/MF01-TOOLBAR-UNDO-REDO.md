# MF-01：工具栏撤销/重做状态图标审计

## 原始源码调用链

- `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml:256-289` 将隐藏工具栏项 `Lang_Undo`/`Lang_Redo` 分别绑定 `TitleMenueItemData.Undo()`/`Redo()`，分配启用与禁用资源 `Resources\ToolBar\ic_topbar_{undo,redo}_{normal,disable}.png`。Undo 另绑定 `DownPopTemplate`、`GetUnRedoStack` 和 `UndoList` 子项。
- `TitleMenueItemData.cs:322-340` 的 `ItemClickCommandExecute()` 只在 `IsEnabled` 且方法名存在时反射调用方法。
- `TitleMenueItemData.cs:1056-1100` 的 Undo/Redo 分别做重入计数保护；打开模块配置时显示“打开模块配置时，不能执行撤消重做”警告并返回；否则调用 `IUnRedoService.Undo/Redo(Application.Current.Dispatcher)`，然后写撤销/重做信息日志。Undo 捕获服务异常，Redo 通过 `finally` 恢复计数。
- `TitleMenueItemData.cs:293-296, 1130-1162` 的历史按钮调用 `GetScheduleInstance()`，`UndoList()` 创建由 `GetUnRedoView()` 返回的视图并放进窗口。`app.config:63` 指定 `myLibs\VM.UnRedo.dll`，`VisionMaster.AppMainWindow/App.cs:210-214` 在启动时加载此程序集并注入 `IUnRedoService`。当前反编译目录未包含该实现，因此不能确认其具体可撤销状态推送、历史列表跳转行为或视觉细节。

## 本线差异与修改

修改前主框架 `src/VisionEditor/MainWindow.xaml.cs::Tool()` 将撤销/重做创建为仅含 normal 图的普通按钮；`WorkflowTabs.cs::RefreshAll()` 会按 `!isLocked && activeRun is null && doc.CanUndo/CanRedo` 切换 `IsEnabled`，但按钮图像不随禁用状态切换。`analysis/ui-assets` 原有 normal 图片与反编译 `VMControls.WPF/resources/ic_topbar_*_normal.png` 同长（636/651 bytes）；对应 disable 资源原始文件长 626/641 bytes，本线将两个源资源作为构建资产复制到 `analysis/ui-assets/toolbar_ic_topbar_{undo,redo}_disable.png`。

现在 `MainWindow.xaml.cs::SetToolbarStateIcon()` 监听对应 Button 的 `IsEnabledChanged`，把 normal/disable 源资源路径与真实按钮可用状态绑定。它不替代或仿造 `IUnRedoService`，只还原 XML 明确声明的工具栏图像状态。

## 操作验收计划与边界

`tests/VisionEditor.ShellChecks/Program.cs` 已加入操作级断言：初始空历史显示两个 disable 源图；修改当前流程名后撤销按钮启用且显示 normal 图；触发实际 WPF Button Click 执行 Undo 后恢复流程名、undo 禁用、redo 启用并互换图标；Redo 再实际恢复名称；最后再 Undo 将测试文档恢复至最初名称。该用例不打开最近菜单，不依赖桌面输入。

## 本轮操作验证结果（2026-10-19）

运行命令：`dotnet run --project tests/VisionEditor.ShellChecks/VisionEditor.ShellChecks.csproj -p:BuildInParallel=false -p:UseSharedCompilation=false`。临时根级 `Directory.Build.props` 内容如下；路径以 props 文件目录（worktree 根）为基准，每个项目按 `MSBuildProjectName` 隔离；显式排除旧/新 `obj/bin` 与隔离根目录，避免自定义 obj 文件重新进入默认 SDK 编译 Glob。

```xml
<Project>
  <PropertyGroup>
    <BaseIntermediateOutputPath>$(MSBuildThisFileDirectory)analysis\full-audit\build-mf01\obj\$(MSBuildProjectName)\</BaseIntermediateOutputPath>
    <BaseOutputPath>$(MSBuildThisFileDirectory)analysis\full-audit\build-mf01\bin\$(MSBuildProjectName)\</BaseOutputPath>
    <DefaultItemExcludes>$(DefaultItemExcludes);**/obj/**;**/bin/**;analysis/full-audit/build-mf01/**</DefaultItemExcludes>
  </PropertyGroup>
</Project>
```

在 `dotnet msbuild ... -getProperty:BaseIntermediateOutputPath,BaseOutputPath,MSBuildProjectDirectory,MSBuildProjectName` 中确认过求值结果指向本 worktree 的项目名子目录。验证结束后已删除临时 props 与 build-mf01 缓存。

测试进程退出码 1；前序 39 项 PASS，之后仍在 Recent 菜单 Popup 检查处失败（`file=False, recent=False`，焦点仍为 `TabItem`，Recent 子项尚未 Loaded/Visible，合成 MouseDown 未处理）。这项与本按钮图标无关，是既有离屏菜单测试宿主限制；完整检查未通过，不能报告整套 ShellChecks 通过。Undo/Redo 图标相关的 6 项状态/真实 WPF Click 操作断言全部在该失败之前通过，包括禁用初态、编辑切换、Undo、Redo、恢复原流程名。

新截图 `analysis/full-audit/shell-toolbar-history-disabled.png`（6755 bytes）和 `analysis/full-audit/shell-toolbar-history-undo-enabled.png`（6833 bytes）由本轮执行生成并逐张目检，均包含真实 WPF 工具栏。目检确认截图成功、工具栏布局可读；图像尺寸较小，禁用与启用小图标的像素差在此缩略视图不够显著，因此本轮将操作状态/源资源路径绑定确认为已验证，精细视觉像素与 VM 实机的对照仍标为未确认。

本轮构建无权限拒绝；生成告警仅为 `MainWindow.xaml.cs(121,224)` 的 nullable warning。`git diff --check` 通过。**状态：本项操作路径通过，精细视觉及原生差分未闭合；MF-01 整体仍未闭合。** 尚须审计 Undo 下拉历史窗及外部 `VM.UnRedo.dll` 行为、保存/打开/其他工具栏按钮调用链；不得推断外部服务历史跳转语义。Recent Popup 的确切宿主限制见 `MF01-HELP-DOC-SOURCE-AUDIT.md`。
