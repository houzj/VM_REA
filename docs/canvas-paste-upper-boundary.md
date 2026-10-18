# 粘贴首节点与后续节点的上边界

- 源码确认：`analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/VmModuleBaseDecorator.cs` 的 `PasteItemsCommandExecute` 对首个模块的非 `(0,0)` 目标点只按 5 格取整、负值改为 5；后续模块才用 `DiagramPageWidth - 150` 限幅。`(0,0)` 键盘哨兵仍先将复制起点加 100 并限幅。`Position` setter 直接保存给模块，无第二层限幅。
- 独立实现：画布菜单点的首个复制节点保留靠右/下边界的实际位置，后续节点分别限到 4850；只改粘贴事务，未改移动或拖放坐标策略。
- WPF 验收：`dotnet run --project tests/VisionEditor.CanvasPasteChecks/VisionEditor.CanvasPasteChecks.csproj -c Release`。真实窗口菜单粘贴在 `(4901,4923)`，首节点成为 `(4900,4925)`、后续节点 `(4850,4850)`；两节点为一次事务，撤销/重做和保存重开均通过。截图 `analysis/full-audit/canvas-paste-upper-boundary.png` 已目检：首节点在画布边缘可发生裁切，这与原始定位公式相符。
- 未验原生：真实 VM 桌面同状态菜单输入与像素差分、原始复制字典枚举在复杂选择中的首节点顺序。
