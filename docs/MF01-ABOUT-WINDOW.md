# MF-01 关于窗口源控件树复验

原始 `analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs` 的 `VerInfo` 打开模态 `AboutMessageBox`。`analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/AboutMessageBox.xaml` 明确窗口 400×440、灰底、350 宽白色卡片、内部滚动区、140×60 标志及双列版本行（左列 160）；代码后置按已获取的真实版本信息填充列表。原版还依赖 `VersionInfoTool.exe`、VM/算法 SDK 版本键、版权和中性版开关。

WPF 壳的“帮助→关于”继续模态打开，但版本区域改用源控件树的 `ScrollViewer`、`ListBox`、`DataTemplate` 双列行；只展示当前程序、框架与编辑器核心三个真实程序集版本，未伪造 VM 或算法 SDK 的版本。标志沿用当前已追踪的 `logo_vm.png`，原版 `about_logo.png` 未移植。

专属 ShellAboutChecks 实际点击菜单、检查模态拥有关系、400×440、可滚动版本列表与三条程序集版本，并生成 `analysis/full-audit/mf01-about-window.png` 目检；窗口关闭后回到主界面。原版 BaseWindow 外框、Logo、完整版权/版本字段和动态主题的像素等价仍未验证。
