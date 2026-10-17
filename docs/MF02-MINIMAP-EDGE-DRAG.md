# MF-02 小地图边界拖动的起点记录

原始 `analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml.cs` 的 `GridEagleEye_MouseDown`（约 563–575 行）在点击视窗矩形外时，先按点击位置计算新的矩形左上角并调用 `MoveSelectRegion`，随后以**计算出的矩形位置**调用 `RecordCurPos`。`GridEagleEye_MouseMove → MoveSelectRegionByPosChange`（约 583–600 行）在后续拖动中继续使用这个记录值。即使 ScrollViewer 将实际视口裁剪在画布边界，拖动起点也不是裁剪后的显示位置。

重建版以前在导航后读取 `MiniMapViewport` 的实际坐标作拖动起点。点击小地图右下角后，初始请求会越过可滚动边界；随后轻微反向拖动会立即移动视口，与原始代码按请求位置产生的边界滞后不同。本批将小地图按下和移动计算拆成可测试的处理函数，保持既有 WPF `MouseLeftButtonDown`、`MouseMove`、`MouseLeftButtonUp` 事件入口，记录点击计算出的请求位置。该记录方式为**源码确认**；由本项目 ScrollViewer、187×150 小地图和视口矩形组成的界面仍属于**独立宿主实现**。

专属 `VisionEditor.CanvasChecks` 创建实际 WPF 主窗口，发送小地图左键按下/释放 routed event，检查拖动状态；点击右下角验证记录位置与裁剪后的显示位置分离，轻微反向移动时视口仍在边界，超过差值后开始移动。检查导航不写文档或撤销记录，保存重开保留流程节点与连线。测试专用截图 `analysis/full-audit/canvas-minimap-edge-drag.png` 只在本工作树生成，目检可见右下角视口框。真实 VM 桌面鼠标、DPI、极端缩放与多屏差分仍未完成，不据此宣称整套小地图完全等价。

完整 `VisionEditor.WpfChecks` 首次运行在与本改动无关的 500ms 连续运行定时断言处失败（第 207 项）；不改该测试或运行代码，直接对同一构建重跑后 456 项通过。此类墙钟断言仍有调度敏感性，不能用一次通过掩盖首次失败。
