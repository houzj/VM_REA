# A08.2 默认输入失败前置审查（第九批）

基线 `f11bc13d413ea82fb074c0c00b3b2f2dffad01ad`。本片仅审查保留日志、原资源和现有夹具；A08 父依赖未闭合，不启动总验收。使用 reverse-engineer-anything 技能规定的同版本已有反编译证据复用路径。本片没有运行目标程序、WPF、桌面探针或输入注入，没有操作任何窗口/进程，没有修改产品或 tool-plugins，也没有重新取得 REA 二进制身份。

## 实际失败操作与证据边界

原日志 `analysis/full-audit/framework-retirement-20261019/validation-shell.log`：**42 条 PASS 后**在“WPF menu input opens File and Recent menus”失败。窗口 IsActive 检查通过，记录 bounds=100,100,1366x768；实际 file=False、recent=False，三次 Keyboard.Focus 返回 TabItem，Recent 尚未 Loaded/Visible，合成 mouseDownHandled=False。旧文档另一轮的33/39不是这份日志计数，不能相加。本轮没有重跑这些42条。

当前 `tests/VisionEditor.ShellChecks/Program.cs:223–251` 的输入方式是构造 `KeyEventArgs` 后 `RaiseEvent(KeyDown)`；失败后用 `RaiseEvent(PreviewMouseLeftButtonDown/Up)`。它不是 OS 设备输入注入，鼠标设备命中状态和真实键盘焦点不能由这些调用成功推定。`window.Show/Activate/Focus` 与 `IsActive` 本身也没有确认输入桌面为 WinSta0\\Default。保留日志没有桌面名、前台 HWND、输入桌面枚举或原生命中记录。

因此，此次确实是默认夹具失败；**不能已证实地归因为产品菜单错误，也不能已证实地归因为 CodexSandboxDesktop**。历史报告“测试宿主限制”是候选解释，所需区分证据仍缺。Recent 未 Loaded 是父菜单没打开时的观测，不能单独视为 Recent 产品错误。

默认路径在251行即抛出，后续 Popup 布局/位图断言和最近项重开未执行；旧图片不得充当本次成功。`validation-shell-control-events-complete.log` 的76条 PASS 属于另一次绕过弹层输入/渲染断言的控件事件路径。它确实验证了较后面的命令路径，但不能覆盖默认失败。

## 原资源到命令链（静态复用）

1. 原 `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml:26–31` 的 Lang_OpenLastSol 使用 `dynamicGetItemsMethodName=GetRecentlySolutions`。
2. 原 `VisionMaster.AppMainWindow/TitleMenueItemData.cs:136` 的 Items getter 反射该方法，异常返回 null；493行 GetRecentlySolutions 将 ProjectManager.RecentlySolutions 投影成条目，FileName为名称，SolutionPath为 ToolTip，methodName=OpenRecentlySolution。
3. 原 `Apps.Style/menustyle/menustyle.xaml:25,92` 的顶部/子级 Popup 都绑定模板父项 IsSubmenuOpen，方向分别 Bottom/Right；尺寸资源200×34。122行 MenuItem.Command 绑定 ItemClickCommand；角色选择模板。该资源没有证明本轮实际输入焦点或 Popup 已打开。
4. 原 TitleMenueItemData.ItemClickCommandExecute 先检查 IsEnabled 与 MethodName，经 ExcuteMethodByName 反射调用，异常被吞；OpenRecentlySolution 从 ToolTip 取路径。OpenRecentSolution 检查 IsCreating/IsSaving/IsLoading，随后 StopRun、日志、ProjectManager.LoadSolution。停止/加载的深层生命周期不属于此片，不宣称闭合。

原始路径、逐行锚点及 SHA256 见本片 JSON。它们是已有反编译文本身份，不能冒充本轮重取程序集 SHA/MVID。

## 当前实现及能否修复

当前 `MainWindow.ShellFileMenu.BuildFileMenu` 给文件根项和子项指定 ShellFileMenuRoot/ShellFileMenuItem；`Styles/ShellFileMenu.xaml:12,36` 同样绑定 IsSubmenuOpen 并以 Bottom/Right 弹出。`MainWindow.ShellRecentSolutions` 在 SubmenuOpened 时刷新列表，条目 Click 调用 OpenRecentSolution；独立 JSON RecentSolutionStore 与原项目服务不是同一实现。模板存在不代表模板全部一致，也不证明真实输入正常。

本片没有定位到足以安全实施的产品修复。直接设置 IsSubmenuOpen、删掉焦点断言或把 control-events-only 设为默认，只会绕过当前失败，不能算修复。可以先改善**测试证据**：由串行验收所有者在受控用户交互桌面核实自己的窗口身份/前台/可见性，记录焦点、命中与 Popup 独立呈现源，再执行真实鼠标/键盘输入。此建议尚未实现，且不能操作其他任务窗口。

有限解除门槛：先证明自有当前构建窗口处于 WinSta0\\Default、可见并获得输入；用真实输入打开 文件→最近打开，记录两个 IsSubmenuOpen、Popup 尺寸和本轮截图；点击已保存项目验证文档路径及排序。若设备输入仍失败，结合命中元素/焦点/角色模板定位产品事件差异；若设备输入成功而 RaiseEvent 失败，则修复夹具并保留合成事件测试用途。之后仍需独立原机同状态与100/150/200%DPI矩阵，单个菜单成功不能关闭 A08.2。

## 可复验输出与进度

运行只读核对器：`python tests/NativeEvidence/A08InputFailure/Inspect.py --output analysis/full-audit/batch9-20261019/A08/static-reconciliation.json`。退出0，核对失败字段、合成事件入口、两个模板的绑定以及42/76独立计数；输出逐输入 SHA256 与行号。这是静态对账，不是新增运行验收。

A08.2 源码 partial / 实现 partial / 验证 failed 保持；本片新增已达成验收单元 **0/96**。总进度由主线统一核对，本片不改分母、不关闭整包。
