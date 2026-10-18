# MF-01 关于窗口验收

原始依据：`analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs` 的 `VerInfo()` 创建 `AboutMessageBox` 并模态显示；`analysis/decompiled/VisionMasterMain/VisionMaster.AppMainWindow/AboutMessageBox.xaml` 指定 400×440、居中、白色版本面板；其 `.xaml.cs` 从应用数据和原生组件查询版本，只加入有值的版本行。

本线独立实现：帮助菜单增加“关于”，打开归属主窗口的 WPF 模态窗；使用当前 `VisionEditor` 程序集版本和现有标识图。没有原生 SDK 查询器时不虚构 VM、算法、驱动版本，也不执行原始 `VersionInfoTool.exe`。因此命令链和实际版本显示闭合，原生组件版本、版权条件和完整 BaseWindow 模板仍未闭合。

专项 `tests/VisionEditor.ShellAboutChecks` 实际显示主窗口，点击帮助菜单的“关于”，在模态窗打开期间检查 Owner、可见性、400×440 尺寸、真实程序集版本，保存测试截图到本线 `analysis/full-audit/mf01-about-window.png`，关闭后返回主窗口。检查通过，截图已目检。该截图由离屏 WPF 渲染测试生成，外围非客户区不能用来判断原生窗口边框差分。
