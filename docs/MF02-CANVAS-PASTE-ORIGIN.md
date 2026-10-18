# MF-02 画布粘贴坐标闭合

原始证据：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ItemsPage.cs` 的 `KeyVdown` 通过 `(0,0)` 发出键盘粘贴；`VMControls.WPF.DataForUI/VmModuleBaseDecorator.cs` 的 `PasteItemsCommandExecute` 也将 `(0,0)` 当作哨兵，取剪贴板位置 `+100`，对目标点按 5 像素取整并将负值落在 5 像素。当前画布右键原点此前直接传入 `(0,0)`，导致副本落在画布左上角。

本次在 `WorkflowGestures.PasteSelection` 统一处理原点与键盘粘贴哨兵、普通菜单点取整和负坐标下限；数据复制与撤销仍使用已有 `GraphEditorDocument` 事务。这是针对本项目画布的独立实现，不是把 VM 的 SDK 复制进来。多节点时原生剪贴板首项顺序和本项目片段最小坐标未证实相同，仍待验。

运行 `dotnet run --project tests/VisionEditor.CanvasPasteChecks/VisionEditor.CanvasPasteChecks.csproj`：真实 WPF 菜单点击验证原点副本为 `(200,200)`、普通点 `(263,242)` 对齐到 `(265,240)`、负点落到 `(5,5)`；检查一次文档事务、撤销及保存重开。9 项通过。测试截图 `analysis/full-audit/canvas-paste-origin-grid.png` 已目检，最终 `(5,5)` 节点贴近左上角并与面包屑区域视觉重叠；这是极端坐标下仍可见的界面缺陷，未宣称完整 VM 视觉一致。
