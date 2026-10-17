# MF-01 标题栏命令与输入闭合记录

## 原始证据

- `analysis/decompiled/VM.Controls/VM.Controls/BaseWindow.cs` 的 `OnApplyTemplate` 将双击与拖动分别挂到 `Header` / `titlebar`，而不是整个包含工具按钮的主窗口顶层。`DragMouseEvent` 要求左键按住、窗口为普通状态且 `MoveAble`。
- 同文件 `Header_DoubleClick` 在允许最大化时切换普通与最大化；`_maxBtn_Click`、`_minBtn_Click` 分别切换窗口状态和最小化。原始模板的所有命中边界仍需用真实 VM 桌面输入差分确认。

## 本线独立实现

- WPF 主窗口的 `ShellHeader` 仍保留空白标题区拖动、双击最大化/还原；拖动增加普通窗口和左键按住条件。菜单、工具栏及其它交互子控件的事件不再被统一的标题区事件处理器当成拖动/双击手势消费。
- 顶栏最大化按钮保留正常状态切换；使用已有 `WindowChrome` 工作区约束，没有把原厂 `BaseWindow` 作为运行依赖。
- 专项测试 `tests/VisionEditor.ShellChecks` 创建真实 WPF 窗口并触发按钮、菜单/工具栏输入及空白双击，使用 Win32 窗口/工作区矩形检查最大化后的上下边界。取证 JSON 仅写入测试工作树 `analysis/full-audit/shell-window-work-area.json`。

## 验收与开放范围

`dotnet run --project tests/VisionEditor.ShellChecks/VisionEditor.ShellChecks.csproj -v:q -m:1`：8 项检查通过。构建时现有 `MainWindow.xaml.cs` nullable 警告不属于本次标题栏逻辑。尚未验证真实 VM 桌面逐像素差分、多屏/DPI、真实鼠标拖拽移动和辅助功能输入；本批不声称完整原生等价，也未实现全局运行模式。
