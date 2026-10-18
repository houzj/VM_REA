# MF-01：主工具栏横向溢出

## 原始证据

- `analysis/decompiled/VisionMasterMain/VisionMasterMain.AppMainWindow/TitleBar.xaml:350`：`m_HorScroll` 包裹原生标题工具栏，水平滚动为 `Auto`，垂直滚动禁用，高度 42，接 `PreviewMouseWheel`。
- 同目录 `TitleBar.xaml.cs:31` 的 `ScrollViewer_PreviewMouseWheel`：滚动距离为 `Abs(Delta / 2)`；向下增加水平偏移、向上减少；只在偏移实际变化时将事件标记为已处理。

## 本线实现及操作验收

- 独立 WPF 壳的主工具栏改为横向 `ScrollViewer`；原有命令按钮不换序。滚轮按原始公式限幅，工具栏滚动区域不会触发标题栏拖动。
- `VisionEditor.ShellChecks` 实际显示主窗口并缩到 800 像素：确认存在横向溢出；滚轮向下 120 后偏移 60，向上 120 后回到 0；区域鼠标按下没有触发标题栏手势。该专项的其余主窗口操作检查也在同一构建的复跑中全部通过。
- 首轮专项末尾遇到已有的耗时子窗关闭/焦点重入异常；同一构建复跑全部通过。本提交没有改动耗时子窗生命周期。

## 尚未原生验证

原生自定义 `titleBarShrinkScrollViewer` 模板、最窄窗口时的滚动条外观，以及“运行模式”按钮与工具栏同处滚动容器的细节，仍需与真实 VM 窗口并排目检；本项只声明已还原源码可证的滚轮行为和按钮可达性。
