# 工作流并行逆向证据审查

日期：2026-10-17。范围：原生托管源码与当前主线静态对照。未启动原生 VM，未构建、未进行桌面鼠标操作；本文不是原生一致性验收结论。当前验收表 F-05～F-09 均为部分完成，不能因窗口事件测试通过而关闭。

## 已发现并实施的小地图边界

原始证据：`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ZoomArea.xaml.cs:469-501,605-608`。

- `GridEagleEye_PreviewMouseWheel` 先调用 `OnHoriScroll(scrollViewer,e)`，再调用 `OnChangeZoom`。
- Shift + 负 delta 调用 `LineRight()`；正 delta 调用 `LineLeft()`，置 Handled。Ctrl 单独判断，因此 Ctrl+Shift 同时执行横滚和缩放。
- `OnChangeZoom` 对 delta=0 不推进缩放刻度；没有修饰键时该处理函数不置 Handled。
- 当前 `WorkflowCanvas.cs:345` 原实现只处理 Ctrl，并无条件吞掉事件。已补 Shift 分支、零 delta 与原生顺序。没有更改主窗口布局或共享契约。
- 独立检查 `tests/VisionEditor.WpfChecks/MiniMapWheelParityChecks.cs` 提供 `Run(MainWindow,Action<bool,string>)`；在测试线程临时保存/设置/恢复键盘状态，调用实际窗口处理函数，检查实际 ScrollViewer 偏移和缩放。没有注入系统按键，没有截图代码进入产品。需主线串行接入后运行；当前未执行，不能标记通过。
- 原生 `_ZoomTicks` 为 0.1～2.0（同文件23-27）；当前主画布公式已包含该范围。极端坐标/DPI、完整原生桌面差分仍开放。

## 可并行的完整功能包

| 包 | 精确证据与现有入口 | 缺口及交付边界 | 文件所有权与依赖 |
|---|---|---|---|
| W1 小地图输入边界 | ZoomArea.cs469-501、553-608；当前 WorkflowCanvas.cs286-351 | 本批已补 Shift/零delta；后续核对捕获丢失、空图、极端偏移，必须比较运行后的视口位置 | 可独立拥有 WorkflowCanvas.cs；主窗口绑定修改由集成线完成 |
| W2 工具箱左右布局与系统弹层策略 | ToolBox.xaml.cs138-150 根据 IsRightLayout 选择 Left/Right；ToolBox.xaml126 绑定 placementMode，使用系统 MenuPopupAnimationKey；386-416 折叠图标依赖布局 | 当前 ToolNavigation.cs144 固定 Right，未设置系统 PopupAnimation；底部方向状态需随布局统一恢复。不能只反转一个箭头 | ToolNavigation.cs、Styles/ToolboxStyles.xaml；需先由集成线提供持久布局状态/主窗口布局入口，不与主窗同时编辑 |
| W3 工具目录热刷新/延迟展开 | ToolBoxData.cs67-115 Reload清缓存和通知；UpdateLoad按目录读取 ToolItemInfo.xml并挂接父节点；ToolBox.xaml.cs185-198 悬停触发 UpdateLoad | 当前 ToolNavigation.cs30缓存静态 tool-catalog.json，无 Reload/目录二级延迟加载。需保持禁用未实现插件、身份稳定和拖拽失效规则；不可将目录存在当工具可运行 | 新独立 Catalog 服务及其测试先行；集成点 ToolNavigation.cs 与 W2互斥；插件注册接口先登记契约请求 |
| W4 原生流程等待配置 | ProcedureConfigControl.xaml271 SetTimeOutCommand；VmProcedureDecorator.cs455-459 打开 TimeOutSet；Apps.XmlUI/Apps.XmlUI.ParamTab/TimeOutSet.xaml.cs31-60 Get/SetProcedureWaitTime | 当前 WorkflowTabs.cs未实现该命令。源码证明这是 ProcedureWaitTime，不能臆造为整个算法执行墙钟超时；需继续跟踪SDK/原生消费点再实施运行行为 | 独立窗口可先还原，运行字段/执行器为集成线所有；不可只做配置保存并宣称完成 |
| W5 流程/算法代码导出 | ProcedureConfigControl.xaml229-266；VmProcedureDecorator.cs395-439 调用 IVmProcedure.ExportFile，远程扩展.prcx；VmModuleSetDecorator.cs193 ExportAlgorithmCommandExecute | 当前 WorkflowTabs.cs菜单明确禁用。托管入口可见不代表原生序列化或代码生成逻辑完整；必须进一步逆向具体实现与格式，不能把自有JSON改扩展名冒充原生 | 单独分析/格式服务可并行；.sol/.prcx共享存储由集成线统一，未证实格式前不启用按钮 |
| W6 非活动Tab/Group历史状态矩阵 | ProcedureConfigControl.xaml295-355 控件命令与上层返回；.xaml.cs1357 SelectionChanged、1417 ScrollChanged；当前 WorkflowTabs.cs88-120缓存header状态，GroupCanvasNavigation.cs | 现有增删复制返回已实现子集。完整核对状态来源、切换后运行/停止/禁用、Group返回视口，需要逐状态原生运行证据。静态代码无法宣布所有差异已关闭 | WorkflowTabs.cs、GroupCanvasNavigation.cs同一负责人；只读审计可与其他包并行，产品更改不得与主线Group配置并写 |

## 现有实现应保留的边界

`WorkflowCanvas.cs322-335` 使用 ScrollViewer Actual/Extent 映射鹰眼，已对应原生 ZoomArea.cs328-372；VisualBrush 来源 GraphLayer，对应原生426。不能仅因小地图看起来空就再改为“所有节点包围盒适配”——那会偏离已确认的固定画布映射。

`ToolNavigation.cs103-123` 只在按下后首次移动启动 DragDrop；CatalogTile 点击不会直接建节点。此处已有源码指向，不应为方便测试改成点击插入。

原生工具箱不是“所有文件都是完整工具”：全局模式名单 ToolBoxData.cs37 为 ProcedureModule、GlobalCameraModule、DataQueueModule；当前仅流程可用，另外两个禁用状态必须如实保留。

## 集成和验收

1. 并行线只拥有分配文件，新功能包先写原生行为表与缺口；共享契约由主线合并。
2. 同一时刻只运行一套 WPF/UI/发布检查；不在并行线启动或终止另一任务进程。
3. 主线接入小地图检查后，实际核对偏移、事件Handled、Ctrl+Shift组合、零delta；结果写回统一HTML验收表。
4. 各包只有在界面、数据和运行消费链一起闭合后提交。原生格式/引擎等待语义没有证据时保持未验证，不以独立策略替代原生结论。

## 二次只读复核：路由与测试边界

原生 `ZoomArea.xaml:14,26` 中 `scrollViewer` 与 `GridEagleEye` 为兄弟。当前 `MainWindow.xaml:59-61` 同样 `CanvasViewport` 与 `MiniMapPanel` 为兄弟，`CanvasWheel` 仅订阅 `CanvasViewport.PreviewMouseWheel`。因此小地图未处理事件不会因向上路由再次进入该画布处理器；不要为了假设的“双缩放”无条件吞事件。

原生 Ctrl-only 的 `OnChangeZoom` 不设置 Handled，而当前补丁沿用了旧实现的 Ctrl置Handled策略。这一点已报告集成线，建议再按原生移除Ctrl分支该赋值，保留Shift分支Handled；最终以集成线代码和实际检查结果为准，本文不宣称已完成这项修正。

`MiniMapWheelParityChecks` 初稿通过反射调用真实窗口处理函数并观察真实滚动器；不等同完整隧道路由。建议集成线改用 `MiniMapCanvas.RaiseEvent` 验证XAML事件绑定，并增加Ctrl-only Handled=false检查。键盘修饰键通过测试线程Get/SetKeyboardState临时设置，不向系统注入键盘输入；首项检查专门验证WPF Keyboard.Modifiers是否读取成功，静态分析不能替代本机运行结果。
