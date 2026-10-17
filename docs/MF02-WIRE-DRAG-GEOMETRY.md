# MF-02 连线中点拖动的三层几何同步

工作流画布已有中点手柄和路径提交，但拖动预览只改可见折线 `Path.Data`，透明命中路径与箭头仍使用拖动前的几何。这样鼠标看到的线与下一次可点击区域不一致，箭头在端段转向时也可能指向旧方向。

## 原始依据与实现边界

- **源码确认**：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml.cs` 的 `connectionUserControl_MouseMove` 在路径变化后设置 `PathPoints`（约 398–543 行）；`PathPointsChanged → OnPathPointsChanged → SetPath`（约 274–286 行）每次同时更新折线与箭头（约 754–775 行）。原生 `Connection` 以一份 `Shape` 几何绘制并命中，不存在重建程序单独创建的透明 hit Path。
- **独立宿主适配**：重建画布保留可见 Path、Polygon 箭头和透明命中 Path 三个 WPF 元素。现在 `PreviewWireDrag` 将同一条钝化后的路径同步到三者；取消时重绘文档原路线，提交时由 `SetExecutionPath` 记一笔撤销事务。命中 Path 是项目自己的宿主设计，三层同步为恢复原生单几何效果的适配。
- **未验证原生**：复杂避障、反向端口、极端重叠节点和原 VM 桌面鼠标精确差分未因本批关闭。

## 实际检查

`dotnet run --project tests/VisionEditor.CanvasChecks/VisionEditor.CanvasChecks.csproj` 创建真实 WPF 主窗口、两个工具节点和连线，拖动中点时检查可见几何与命中几何同源、箭头按当前展示路线重算；检查预览不写模型、取消恢复、提交只产生一次撤销记录、撤销恢复。测试专用截图保存在本工作树忽略的 `analysis/full-audit/canvas-wire-midpoint-preview.png`；目检确认折线、箭头和中点手柄位置一致。完整 `VisionEditor.WpfChecks` 另作回归，不能代替原 VM 差分。
