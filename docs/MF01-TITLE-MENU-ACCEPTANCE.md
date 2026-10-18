# MF-01 标题菜单容器验收

原始证据：`analysis/decompiled/VisionMasterMain/VisionMasterMain.AppMainWindow/TitleBar.xaml` 使用 WPF `Menu` 承载 `MenuSource.Items`，各子项由 `MenueDataTemplate` 绑定 `ItemClickCommand`；它不是按钮配各自 `ContextMenu` 的结构。

本线将五个顶层菜单改成 WPF `MenuItem`，保留已有命令委托，并由 WPF 菜单控件处理子菜单展开、切换和键盘输入。此前 `Button.ContextMenu` 的结构已移除。标题栏拖动命中边界仍将菜单项排除在拖动区域之外。

实际 WPF 操作验收：`VisionEditor.ShellHistoryChecks` 从新菜单容器执行撤销/重做，验证配置窗口打开时不改文档、关闭后真实节点名称可撤销/重做；`VisionEditor.ShellAboutChecks` 从帮助菜单打开模态关于窗口并返回；`VisionEditor.ShellChecks` 22 项包含标题栏菜单不触发拖动以及最大化、工具栏、状态栏回归，均通过。`analysis/full-audit/shell-zoom-reset.png` 的本线离屏图已目检菜单横向位置。原生菜单模板、动画、键盘焦点和全部菜单条目与真实 VM 的逐像素差分仍未完成；当前命令列表仍是部分实现。
