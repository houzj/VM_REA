# 连线首帧与鼠标捕获（MF-02）

原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/Connector.xaml.cs` 的 `OnPreviewMouseLeftButtonDown`（约 81–88 行）仅记录源节点并订阅首个移动/释放；`ToolItemGrid_PreviewMouseMove`（约 141–156 行）先移除首帧处理器，若左键已松开则调用无释放事件的清理方法，只有按下时才 `CaptureMouse()` 并创建虚线 Adorner。之前产品在按下时立即捕获，且首个移动没有按键条件。

当前 WPF 把捕获延后到首个仍按下的移动；若左键未保持则取消手势、移除预览且不创建连线。捕获时忽略 WPF 更换捕获所有者造成的重入 MouseMove，对应原代码先解除首帧事件订阅的顺序。

专项 `VisionEditor.ConnectionPreviewChecks` 17 项通过：按下仅设 pending、不捕获；无左键的实际 WPF 路由 MouseMove 取消且不画虚线；显式 Pressed 首帧捕获并显示虚线；主动失捕清理后可重启；正常目标连接、单次文档事务、撤销/重做与保存重开通过。截图 `analysis/full-audit/connection-pointer-preview.png` 已目检。测试用真实 WPF 窗口和路由 MouseMove 检查 Released 分支；Pressed 分支通过产品方法传入按键态以规避合成鼠标按下与系统设备状态不一致。持续按下的实体鼠标和真实 VM 快速拖动时序仍待桌面差分。
