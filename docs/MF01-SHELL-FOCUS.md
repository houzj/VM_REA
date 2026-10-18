# MF-01：主菜单和标题工具栏的流程焦点

## 原始源码确认

- `analysis/decompiled/VisionMasterMain/VisionMasterMain.AppMainWindow/TitleBar.xaml.cs:51`：菜单展开调用 `MainWindow.SeMenuFocus(sender as UIElement)`。
- 同目录 `TitleToolBar.xaml.cs:29`：标题工具栏按钮点击也调用该方法。
- `analysis/decompiled/VisionMasterMain/VisionMasterMain/MainWindow.xaml.cs:95`：从控件找到主窗口与 `ProcedureConfigControl.curTabControlBase`，再通过 `FocusManager.SetFocusedElement` 登记事件控件。原始行为是焦点范围登记，并没有在这里执行新的业务命令。

## 独立 WPF 实现及验收

主菜单展开及标题工具栏按钮点击后，将对应控件登记到当前壳的 `ProcedureTabs` 焦点范围；原有命令继续执行。`VisionEditor.ShellChecks` 显示真实 WPF 主窗口，展开“文件”菜单并读取焦点元素，再点击“停止”工具栏按钮并读取焦点元素，两次均与事件控件相同；整套专项在本次构建中通过。

## 原生尚未验证

本项目 `ProcedureTabs` 是原版 `ProcedureConfigControl.curTabControlBase` 的独立映射；真实 VM 中菜单打开后焦点和键盘导航的可见表现尚未差分录制。本项没有宣称完成原生键盘导航一致性。
