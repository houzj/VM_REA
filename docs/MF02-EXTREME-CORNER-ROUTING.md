# MF-02 画布右下角路径消失边界

## 原始依据与差异

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/DiagramPage.xaml.cs`：`DiagramPageWidth = 5000`，`GetPathMap` 建立 `(ActualWidth/5+32) × (ActualHeight/5+32)` 的寻路图；`GetPath` 先对全部节点避障，再在短路径时以端点矩形重试，最终执行 `PathFinder` 优化。原生方法没有 250000 单元的预算分支。
- 当前移植的 `VmOrthogonalRouter` 保留上述 AStar/PathFinder 管线，但默认 `maxCells=250000` 是独立保护。右下角两端节点使宽、高都超过约 2400 时会返回 `BudgetExceeded`；WPF `RenderGraph` 因路径不足两点而跳过连线。这是现有 UI 路径消失的一个可复现原因，不代表所有消失场景都由此引起。

## 本次独立适配与实际验收

- `WorkflowCanvas.cs` 与 `WorkflowGestures.cs` 的显示及交互预览布线路径，都将最大范围限定在原生 5000 画布，并允许最多 1100000 个单元，覆盖原生完整画布的 `(5000/5+32)^2=1065024` 单元。近距离画布仍使用已有按节点边界缩小的地图尺寸；这一内存策略是独立适配，不冒称原生缓存实现。
- 专项 WPF 主窗口在右下角添加源、目标与障碍节点，在 4300–4600 的坐标范围连接后，验证实际路径、三角箭头和点击选中。移动目标到 4700 后，路径重新计算，文档一次修订；撤销恢复原位置和路径；保存重开后连线重新绘制。截图 `analysis/full-audit/canvas-corner-route-selected.png` 由测试程序集生成，已目检路线绕开中间障碍节点。

## 仍未关闭

- 重叠、逆向端口、全障碍封闭及地图边缘走线尚未完成原生同输入输出差分。原生 `GetPath` 在第一次 AStar 短路径后会只保留端点矩形重试，可能穿过第三方节点；不能把“所有障碍场景均不穿越”当作原生规则。
- 本次验证真实 WPF 控件的路线显示与选中，但未注入系统级桌面鼠标；与真实 VM 的同位置截图、耗时、缓存时序未验。全画布寻路比近距离场景成本更高，仍需长稳与多连线负载测量。
