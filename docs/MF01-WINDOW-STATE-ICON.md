# MF-01 最大化与还原状态图标

原始依据：`analysis/decompiled/Apps.Style/buttonstyle/normalbuttonstyle.xaml` 的 `BtnMaximize` 模板随祖先 `Window.WindowState` 切换 `windowshift_ic_maximize.png`、`windowshift_ic_normal.png`，并切换最大化/还原命令与“向下还原”提示。`VisionMasterMain.AppMainWindow/TitleBar.xaml` 将此模板用于主窗口按钮。两张位图从原始资源读出并作为构建所需 UI 资源置于本线 `analysis/ui-assets`。

当前独立 WPF 主窗口仍沿用已有工作区最大化/还原处理，但按钮内容和提示现随窗口状态改变；标题栏双击同样触发状态刷新。`VisionEditor.ShellChecks` 实际显示窗口，点击最大化和还原，核对窗口状态、两张原始资源与提示，并继续验证工作区边界和其余 Shell 操作。同一构建复跑全部检查通过；一次先前运行在全部断言通过后遇到离屏 WPF 焦点清理异常，复跑未复现，本次不把它算作原生差异或产品根因。离屏截图 `analysis/full-audit/shell-zoom-reset.png` 已目检当前图标位置。

多屏/DPI、不同 Windows 主题下的系统按钮区域及完整原生像素差分仍未完成。
