# 第三十轮 A08.2 默认 Shell 输入复核

范围：原始菜单、节点/连线输入源码与 main 的文件菜单默认路由。实施增量 **+0**；A08.2 仍为 partial/failed。没有运行原机、注入用户桌面输入、修改产品或 `tool-plugins`。

## 已确认的原始链

- `analysis/decompiled/Apps.Style/menustyle/menustyle.xaml:25-45,92-113,122-145`：原菜单根据 `MenuItem.Role` 分别选顶层 Header、子级 Header、叶项模板。顶层/子级 `Popup.IsOpen` 绑定模板父项 `IsSubmenuOpen`；`NormalMenuItemStyle` 的命令绑定 `ItemClickCommand`。这是静态资源证据，不证明当前真实键鼠输入打开了弹层。
- `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml:26-31` 的最近方案项调用 `GetRecentlySolutions`；`VisionMaster.AppMainWindow/TitleMenueItemData.cs` 将最近路径装入 `ToolTip`，选择后反射 `OpenRecentlySolution`。完整的停止/加载链见第九批报告。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramItem.xaml.cs:471-486` 的 `OnPreviewMouseLeftButtonDown` 仅在参数窗已开且非双击、非按钮时标记 Handled；`Connection.xaml.cs:342-351` 在预览鼠标事件上 `Focus()` 并标记 Handled。这些是画布局部输入，不支持将菜单失败归于节点或连线拦截。

## 当前实现及失败的准确边界

- `src/VisionEditor/MainWindow.xaml:35-38` 的 `MenuHost` 位于顶栏；`MainWindow.xaml.cs:56-58` 创建 `MenuItem`、订阅顶层 `SubmenuOpened` 并设置焦点记录。`HeaderMouseDown` 遍历命中链时遇到 `MenuItem` 或 `MenuHost` 就返回，不拖动窗口、不主动吞菜单输入。
- `src/VisionEditor/Styles/ShellFileMenu.xaml:4-45` 的文件根项和子项均有透明命中区域、`PART_Popup`、`IsSubmenuOpen` 绑定以及 ItemsHost。最近方案通过 `MainWindow.ShellRecentSolutions.cs:82-101` 的 `SubmenuOpened` 刷新后点击调用加载。源码静态检查没有证明模板已在真实设备输入下工作。
- 原失败日志 `analysis/full-audit/framework-retirement-20261019/validation-shell.log:43`：在 42 条通过后 `file=False, recent=False`，`Keyboard.Focus(menu/fileRoot/recentMenu)` 均返回 `TabItem`，最近子项尚未加载。`tests/VisionEditor.ShellChecks/Program.cs:225-251` 是 `Keyboard.Focus` 配合 `RaiseEvent(KeyDown/PreviewMouseLeftButtonDown/Up)`；它没有 OS 输入设备命中与前台焦点证据。`--control-events-only` 的 76 项通过排除了弹层输入断言，不能覆盖这次失败。

结论：这条失败只证明现有默认测试夹具未打开菜单；**产品菜单输入缺陷及其修复点未被证实**。给 `IsSubmenuOpen` 强制赋值、删断言、或把合成事件通过计为真实键鼠实现，都不满足 A08.2。人工桌面验收后置的计分规则并不能把尚未定位的产品改动计为实施完成。

解除条件：使用自有当前构建并确认其窗口位于 `WinSta0\\Default`、前台、可见，记录真实鼠标/键盘对文件菜单及最近子项的命中、焦点、`IsSubmenuOpen`、Popup 尺寸和命令消费。若真实输入失败，再沿具体命中元素和菜单角色定位最小产品修复；若真实输入成功，则修复测试夹具的证据表达。A08.2 的多屏/DPI及原机同状态矩阵仍单独开放。

哈希与本轮静态输入清单见 `analysis/full-audit/batch30-20261020/A-shell/evidence.json`。本轮未新增可自审核的实现单元。
