# MF-01 文件菜单“退出”验收

原始菜单资源 `analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml` 在文件菜单末尾定义带分隔标记的 `Lang_Exit`，绑定 `TitleMenueItemData.Exit`。该方法在 WPF 主窗口存在时调用 `Application.Current.MainWindow.Close()`，由窗口既有关闭链处理退出。

独立 WPF 实现在文件菜单最近记录之后添加分隔线和“退出”，点击时调用当前 `MainWindow.Close()`，复用标题栏关闭后的执行取消、工具配置窗口收尾和执行作用域释放。专属 ShellChecks 实际打开菜单、检查分隔结构并触发 `MenuItem.Click`，验证 `Closed` 事件和窗口不可见。

本项仅闭合“退出”菜单的入口与关闭生命周期；原版菜单完整顺序、禁用规则以及密码/后台运行等 SDK 退出策略未作等价声明。
