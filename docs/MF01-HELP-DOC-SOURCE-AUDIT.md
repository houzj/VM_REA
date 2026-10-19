# MF-01：本地帮助菜单源码调用链核对

## 用户可见差异

当前 `src/VisionEditor/MainWindow.xaml.cs::BuildChrome()` 在“帮助”下显示“操作说明”，点击后只弹出静态说明文本。原始菜单资源 `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml` 的菜单项名为 `Lang_HelpDoc`，事件目标为 `HelpDoc`；本项目当前入口、标签和结果均与原始调用链不同。

## 源码证据与调用链

1. `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml`：菜单 XML 将“帮助文档”关联到 `TitleMenueItemData.HelpDoc`；“帮助文档（在线）”关联到 `OnlineHelpDoc`，版本信息另关联 `VerInfo`。
2. `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs:1021`：`HelpDoc()` 单行调用 `GuideHelper.GetHelp(true)`。
3. `analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/GuideHelper.cs:114-151`：`GetHelp(true)` 读取 `AppDataFactory.Instance().IsNeutral`，将其传给 `GetHelpPathFile`；再对配置值调用 `AppLocalizationService.GetLang`，以 `AppDomain.CurrentDomain.BaseDirectory` 为基目录组合路径，设置 `ProcessStartInfo.WindowStyle=Maximized`、`WorkingDirectory=text` 并调用 `Process.Start`。语言资源查找异常会显示 `Lang_LangLack` 并记录“语言资源缺失”；启动帮助路径异常会显示 `Lang_HelpFileLack` 并记录“帮助文档缺失”。注意源码在语言查找异常后继续执行启动尝试。
4. `GuideHelper.cs:194-197`：`GetHelpPathFile(bool ifMeu=false)` 从配置读取 `HelpDocNeu`（true）或 `HelpDocStd`（false）。`VisionMaster.Exe.Audit/app.config:57-58` 对应值分别为 `help_doc_neu` 和 `help_doc_std`。
5. `VisionMaster.Exe.Audit/resources/{zh-cn,zh-tw,en-us}.xaml` 含 `Lang_HelpDoc` 和 `Lang_HelpFileLack` 文案；这一小份反编译资源没有 `help_doc_std`、`help_doc_neu` 的路径映射。随后对本机随附程序 `Applications/Lang` 的只读核对找到了中英/繁中映射（见下节）；`AppLocalizationService.GetLang` 的实现仍不在当前反编译证据目录。
6. `VisionMaster.Exe.Audit/VisionMaster/MainWindowData.cs::CheckIsNeutral()` 根据 `appData.IsNeutral` 切换 `MenuItems.xml`/`NeutralMenuItems.xml`（远程模式另有 Linux 资源）；本地帮助目标依赖真实授权/产品状态。当前独立 WPF Shell 没有 `AppDataFactory.IsNeutral` 或等价授权状态。

## 可确认、独立实现与未知

- **可确认的 VM 行为：** 帮助菜单事件调用、产品类型分支、配置键选择、语言资源转换调用、启动参数、两类异常的文案/日志副作用。在线帮助入口则由 `OnlineHelpDoc()` 直接读取 `OnlineHelpDoc` 并 `Process.Start(config)`；`app.config:54` 记录其 URL。
- **独立实现现状：** 静态“操作说明”消息框是本项目自有说明，不能视作 VM 功能；此前 MF-01 关闭流程已在 `docs/MF01-SHELL-CLOSE-DRAIN.md` 单独记录。
- **尚未确认：** `AppLocalizationService.GetLang` 的内部资源加载/缺键行为、`IsNeutral` 的具体来源，以及在缺失映射后 `Path.Combine(baseDir, "")` 和 `Process.Start` 的机器级结果。不能凭截图或常见帮助系统行为补足。

## 当前决定与待验证

本记录没有将本地 HelpDoc 改成默认中文标准版路径；独立 WPF Shell 仍缺少 `IsNeutral` 产品状态契约，不能冒充本机 VM 授权状态。MF-01 第二项现先补齐独立、有完整可见配置值的“帮助文档（在线）”调用路由，保持本地静态“操作说明”并在差异中继续标明它不是原生 HelpDoc。在线入口默认值与反编译 `app.config` 一致，通过可注入启动器送到系统浏览器；WPF 操作测试将替换该启动器记录 URL，避免自动测试开启浏览器。

## 安装包只读核对（2026-10-19）

在 `D:\Program Files\VisionMaster4.4.0\Applications` 只读搜索语言资源和手册文件，得到：

- `Lang\zh-cn.xaml`、`Lang\zh-tw.xaml`：`help_doc_std` 与 `help_doc_neu` 均映射到 `.` 前缀路径 `Help\CH\index.html`；实际 `Help\CH\index.html` 存在，长度 255540 字节。
- `Lang\en-us.xaml`：`help_doc_std` 映射到 `.` 前缀路径 `Help\EN\index.html`；`help_doc_neu` 映射到 `Help\EN\VisionMasterHelp.chm`。
- 当前安装目录下 `Help\EN\index.html` 与 `Help\EN\VisionMasterHelp.chm` 均不存在；不代表所有机器/发行包均缺失，只记录本机所查版本。
- 同目录 `LangCFG\VisionMaster.cfg`、`VisionMasterEN.cfg` 将对应配置键保留为资源 key 名称，符合源码“配置读 key，再经 GetLang 取路径”的两阶段调用链。

这些文件证实本机中文帮助路径资源和目标文件存在，不能代替 WPF Shell 的语言与授权状态模型，也不能据此修改/启动真实 VM。

## 在线帮助入口独立实现与验收边界

- 新增 `src/VisionEditor/MainWindow.ShellHelp.cs::OpenOnlineHelpDocument()`，系统启动器使用 `Process.Start(new ProcessStartInfo(url) { UseShellExecute = true })`；URL fallback 常量与原反编译 `OnlineHelpDoc` 配置一致。`BuildChrome` 加入“帮助文档（在线）”命令，保留原独立“操作说明”。
- `tests/VisionEditor.ShellChecks/Program.cs` 通过真实 WPF MenuItem `Click` 事件，把 `onlineHelpLauncher` 替换成测试记录器，验证准确的源配置 URL 到达启动器，且工作流 `Revision` 未变化。该测试不打开浏览器。
- **尚未验证：** Windows 默认浏览器对该 URL 的实际打开、网络连接/服务端响应、VM 实机上的外部进程副作用；测试通过不代表上述桌面和网络行为。

## 本轮 WPF 检查与菜单截图限制（2026-10-19）

命令：`dotnet run --project tests/VisionEditor.ShellChecks/VisionEditor.ShellChecks.csproj --no-launch-profile -p:BuildInParallel=false -p:UseSharedCompilation=false`。本轮编译成功，33 项 shell WPF 检查通过，随后在 Recent 菜单可见截图验收断言之前异常退出。在线帮助菜单路由通过：源 URL 到达注入启动器，工作流 revision 未变。完整日志为 `analysis/full-audit/shell-help-route-diagnostic.log`。因先前那次诊断测试未退出，清理的是本线自己的测试会话后才完成本轮串行运行；没有触碰其他任务进程。

为修正旧夹具，本轮将测试 `MainWindow` 从屏幕外改为屏幕内 `(100,100)`、`ShowActivated=true`，显示后显式调用 `Activate()`。同时对 File 和 Recent 菜单使用 `Keyboard.Focus`、WPF `KeyDown` (Down/Right)，再以 PreviewMouseDown/Up 路由事件作为同一 WPF 输入层的辅助尝试；全程没有直接设置 `IsSubmenuOpen`。诊断显示 `window.IsActive=true`、File root 的 `Focusable/IsLoaded/IsVisible` 均为 true，但菜单及其子菜单的 `Keyboard.Focus(...)` 都返回流程 `TabItem`；键盘事件后 `fileRoot.IsSubmenuOpen=false`，路由鼠标事件 `Handled=false`，Recent 子项 `IsLoaded=false/IsVisible=false`（因父 Popup 尚未打开）。因此这不是原先“窗口在屏幕外”的单一问题；当前测试宿主的 routed-event 合成无法建立菜单所需的实际设备焦点/鼠标命中状态，具体控制树行为仍需真实桌面鼠标输入才能区分。没有移除菜单层级、Popup `IsOpen`、`ActualWidth`/`ActualHeight` 或位图断言。

该运行没有写出新的 `analysis/full-audit/shell-recent-menu.png`；同目录历史图不是本轮验收证据。当前工具环境未提供可用的原生鼠标桌面自动化能力，因此需要在屏幕内实际运行的 WPF 窗口上用真实鼠标点开“文件 → 最近打开”，再截取 Popup 并点击一个条目，确认加载数据和最近列表更新。该操作级差异未验收；在线帮助启动器的真实浏览器/网络副作用也未验收。失败后已结束本线自己的测试会话并释放构建槽，未提交或推送。

关闭排空原有独立验证记录见 `docs/MF01-SHELL-CLOSE-DRAIN.md` 和 `analysis/full-audit/shell-close-drain-final.log`；本轮在 Recent 菜单失败前没有重跑关闭排空检查，不能把早前 68 项结果并入本轮 33 项通过数。
