# MF-01 状态栏 100% 缩放复位

## 源码确认

- `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/StatusBar.xaml`：缩放滑块旁的百分比是可点击 `Button`，绑定 `ZoomCallBackClick`；放大镜图标位于该按钮外侧。
- 同目录 `StatusBar.xaml.cs`：`_ZoomTicks` 从 `0.1` 到 `2.0`，间距 `0.1`；`ZoomCallBackClick` 查找 `1.0` 档并设置 `ZoomIndex`。因此按钮复位到 100%，不执行“适应画布”。

## 独立实现

- 保持现有放大镜图标与缩放滑块，将百分比改为 WPF 按钮。点击后将现有 `ZoomSlider.Value` 设为 `1`，由画布原有 `ValueChanged → SetCanvasZoom → ApplyCanvasTransform` 路径更新实际 `GraphSurface.LayoutTransform`、滚动视口、小地图和百分比显示。
- 视口缩放不是方案内容编辑；按钮不修改 `GraphEditorDocument.Revision`，不会新增撤销记录。

## 操作验收与边界

`tests/VisionEditor.ShellChecks` 在真实 WPF 窗口里把滑块改到 150%，确认画布变换和百分比，再点击百分比确认二者恢复 100% 且文档版本不变。连同已有标题区、耗时窗口检查，专项共 17 项通过。测试渲染 `analysis/full-audit/shell-zoom-reset.png` 已目检：按钮呈现为状态栏 100% 文本，放大镜在其左侧。原生缩放档位显示与按钮位置有源码证据；实际 VM 桌面鼠标、全 DPI 与动画/颜色逐像素差分未验，不能据此宣称状态栏完整等价。
