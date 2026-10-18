# MF-02 连线路由重算生命周期

## 原始依据

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connection.xaml.cs`：`OnPositionChanged` 仅在当前连接的 `StartPosition`、`EndPosition`、`SourceIndex` 或 `TargetIndex` 改变时清空 `PathPoints`，随后 `SetPath` 才调用 `Page.GetPath`。这四项分别由连接的源、目标绑定；第三方节点位置变化不在该连接的通知链上。
- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs`：`GetPath` 在触发时读取当时的全部节点矩形，先按全障碍寻路；短路径才用端点矩形重试。因此“初次/端点改变时避障”与“第三方障碍移动后不自动重算”可以同时成立。
- `tests/VmRouting.Checks/Program.cs` 新增逆向、部分重叠、完全重叠端点三组原始 AStar/PathFinder 对照，67 项通过。这些用例未发现算法移植差异；不能据此声称所有组合都等价。

## 本次实现和 WPF 验证

- 画布现在缓存自动路由的显示路径，以流程、连线身份及源/目标端口几何识别。无关节点移动只重建可视控件，不重算该连接；任一端点位置或端口改变会重新执行原路由。手动路径不使用此缓存；连线删除和方案替换会清理缓存。
- 专项真实 WPF 窗口把第三方节点移动到现有线段上。先用原算法单独重算，确认若重新寻路会改变形状；再检查实际画布仍保持旧路径、箭头及点击命中。移动目标节点后路径改变；连续撤销恢复目标与障碍位置，保存重开后连接重新绘制。
- 目检截图 `analysis/full-audit/canvas-obstacle-move-retains-route.png`：移动后的障碍节点覆盖旧路径。该画面并非主张跨节点走线更理想，而是本次逆向确认的更新时序。截图由专项测试程序生成。

## 仍未验证

- 原生 Group 嵌套画布的视觉生命周期、运行时添加/删除不同类别模块时的 WPF 属性通知，以及真实 VM 同步骤截图差分未完成。
- 初次布局的所有重叠、逆向、封闭障碍组合及原生路由性能仍未穷举。本次只关闭“无关障碍移动触发错误重算”的生命周期差异。
