# MF-02 混合高度节点多选布局闭合

本批只处理工作流画布的多选对齐与分布几何。普通节点高度为 30，Group 画布节点高度为 40；旧实现的“底部对齐”“垂直居中”“垂直分布”统一按 30 计算，使普通节点与 Group 的可见边缘或中心相差 10/5 像素。

## 依据与边界

- **源码确认**：原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramItem.xaml.cs` 的 `DrawAlignLine`（约 855–869 行）分别用节点的 `ActualWidth/ActualHeight` 判定左右、底部及中心对齐线。`ItemsPage.cs` 的 `GetBoxRect`（约 604–611 行）按被选节点各自的 `ActualWidth/ActualHeight` 汇合边界，`DiagramPage.xaml.cs` 的 `GetItemsByRect`（约 381–389 行）要求完整包含节点实际边界。
- **独立实现**：本项目的右键对齐/分布命令位于 `WorkflowGestures.cs`；这部分并非从原始 VM 的对齐菜单处理方法逐语句移植。本批将已有菜单的纵向计算改为按 `NodeVisualHeight(node)`，保留原有 5 单位吸附、选择集合和单次 `MoveNodes` 事务。
- **未验证原生**：混合 Group/普通节点时 VM 原生右键菜单的分布间距、边界外扩及真实桌面鼠标操作尚未完成差分。此批不能据此宣称所有画布布局行为与 VM 一致。

## 操作验收

独立入口：`dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj`。该检查实际创建 WPF 主窗口，通过“对齐/分布”的 `MenuItem.Click` 事件操作普通节点与 Group，并核对底边、中心、间距、一次撤销事务、撤销恢复和保存重开。测试截图由测试代码保存到本工作树忽略的 `analysis/full-audit/canvas-mixed-height-bottom-align.png`；目检可见两节点底边齐平。原有整套 `VisionEditor.WpfChecks` 另作回归，不将通过数视为原生等价证明。
