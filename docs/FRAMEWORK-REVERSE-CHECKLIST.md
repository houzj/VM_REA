# 框架线逆向与实现清单

日期：2026-10-16。负责人：framework 对话。覆盖宿主、流程编辑和运行框架；工具专用算法/参数页由插件线负责。本表依据当前源码、已有反编译文件及最近验证记录重整，不等于已经重新逐函数审完全部组件。

## 状态与证据登记规则

证据：E0 尚未定位；E1 找到源码/资源入口，调用链未全部审完；E2 相关调用链及字段/条件已确认；E3 增加真实 VM 行为对照。
实现：I0 未实现；I1 局部/适配；I2 有可运行闭环；不得从 I2 推导原生一致。
验收：V0 未验收；V1 有自动回归/独立程序实际操作记录；V2 完成该项已声明范围的 VM 差分验收。当前没有条目在本清单被标为 V2。

本次仅对已明确核实的 Group Tab 返回给 E2；其他多数条目仍为 E1，要求继续追踪调用链。历史截图、测试计数或原资源文件存在不自动升级证据级别。

## 总表

| ID/优先级 | 范围与当前状态（证据/实现/验收） | 实现与证据入口 | 关闭条目前必须验证 |
|---|---|---|---|
| F-01 / P0 | 数据/模块公共契约 E1/I1/V1 | VisionFramework/Contracts.cs；VM.Core、PlatformSDK 恢复代码 | 原生类型映射、标量与数组、空值/缺失/长度、不可变快照、参数范围和版本兼容；先满足 Group/插件数据需求 |
| F-02 / P0 | 插件宿主契约 E1/I1/V1 | VisionPluginContracts/Contracts.cs、WpfEditor.cs；DynamicObjectConfigControl | 现接口仍含 Form CreateEditor，另有 WPF 工厂；厘清有效路径，约定配置提交、运行停止、会话、图像/ROI服务、释放；与插件线联调 |
| F-03 / P2 | WPF 主窗口及布局 E1/I2/V1 | MainWindow.xaml、WindowWorkArea.cs；VMControls.WPF 主窗/模板 | 正常/最大化/还原、多屏/DPI、任务栏、最小尺寸、分隔拖动、状态栏，逐状态截图比对 |
| F-04 / P2 | 菜单与所有工具栏按钮 E1/I1/V1 | MainWindow.xaml.cs、ShellWidgets.cs；原始命令/资源 | 为每个按钮登记显示、可用条件、命令、弹窗、提交/取消及快捷键；不能以“点击有反应”验收 |
| F-05 / P1 | 流程 Tab 全部操作 E1/I2/V1 | WorkflowTabs.cs；ProcedureConfigControl.xaml/事件 | 新增、重名、改名、复制、删除、选择落点、溢出列表、运行状态、锁定、空方案；标题与命令绑定当前模块集 |
| F-06 / P1 | 工具箱/导航宿主 E1/I1/V1 | ToolNavigation.cs、tool-catalog.json；原始 ToolBox | 分类悬停切换/延迟/收起展开、弹层边界、点击/拖放、不可用工具、注册后刷新；工具定义由插件线交付 |
| F-07 / P1 | 节点选择/移动/复制 E1/I2/V1 | WorkflowCanvas.cs、WorkflowGestures.cs、WorkflowInputBoundaries.cs；DiagramItem/Area | 临时节点与提交时机、吸附对齐、框选/多选/组合、剪贴板/撤销、焦点/捕获丢失、锁定/运行中边界 |
| F-08 / P1 | 连线、避障、编辑 E1/I1/V1 | VmRouting、WorkflowWireEditing.cs、RecoveredConnectionDrag.cs；恢复路由代码 | 端点选择、箭头方向、障碍/重叠/逆向布局、路径不消失、命中/删除、中点拖动/插入节点、缩放和手工路径持久化；不得称已完全使用 VM 算法 |
| F-09 / P1 | 视口/小地图/缩放 E1/I2/V1 | WorkflowCanvas.cs/Presentation；DiagramPage、ZoomArea | 5000范围、独立流程视口、鹰眼矩形公式、点击/拖动、展开收起、滚动同步、DPI、无节点及超界 |
| F-10 / P0 | Group 所有权和内部画布 E1/I2/V1 | OwnedGroups.cs、GraphEditorDocument.Groups.cs；VmGroupDecorator、VmProcedureDecorator | 三层嵌套、父流程归属、内部图编辑、接口同步、复制/删除/撤销、保存重开、跨方案剪贴板；旧跨顶层调用单独标识 |
| F-11 / P0 | Group Tab Up 返回 E2/I2/V1 | ProcedureConfigControl.xaml:btnUp/UpEnable；VmProcedureDecorator.UpCommandExecute；WorkflowTabs.cs | 当前已有按钮逐层返回/根层隐藏/再进入测试；还须 VM 多层名称、状态资源、焦点/当前选择/视口以及运行中行为对照 |
| F-12 / P0 | Group 循环/LoopIndex E1/I1/V1 | GROUP-LOOP-RECONSTRUCTION.md；ModuleFrame CGroupNode::DoWork 0x1800a0930；IMVSGroup资源 | 恢复开始/结束订阅、端点包含性、零/反向范围、索引可见时机、间隔、取消/超时、每轮上下文；托管 SDK 包装不足以证明原生调度语义 |
| F-13 / P0 | Group 中断条件 E1/I0/V0 | 原始 EnableBreak、DataType、CmpType、CmpSrc/Dst 字段 | int/float/string比较、空值、错误、执行前/后判断、中断轮次与结果；需与原生执行行为核对 |
| F-14 / P0 | Group 累积输出/显示 E1/I0/V0 | VmIODecorate、DisplayObjectDecorate、Get/SetGroupIOReportMode | 已知 nMode=0累积、1普通；仍需数组/顺序/本轮与上次隔离、清空、展平规则、部分失败/中断、下游读取、图像例外及显示契约 |
| F-15 / P1 | Group 配置窗口 E1/I1/V1 | SubprogramCallWindow；DynamicParamsTabControl、DynamicIOConfigControl | 输入/输出增删改名、订阅、运行参数代理、显示、循环控件可见性、提交/取消、单次运行/停止；隐藏旧目标选择器不等于原生配置完成 |
| F-16 / P1 | 调度和运行控制 E1/I1/V1 | GraphExecutor、SolutionExecutor、WorkflowExecution；VM.Core/原生调度边界 | 单次/连续/单步语义、分支、使能、超时取消、停止NG、顺序、重入并发与跨 Group 传播；原生引擎未完整恢复 |
| F-17 / P1 | 局部变量/输入输出/订阅 E1/I2/V1 | WorkflowRunContext、Subscriptions、Configuration；DynamicObjectConfigControl | 变量初始化/更新顺序、类型、源失效、跨轮隔离、代理参数、父子传参、缺失输出、编辑事务；必须检查下游真实结果 |
| F-18 / P1 | 方案与资源存储 E1/I2/V1 | SolutionStore、GeometryResources、GraphEditorDocument | .vmx.json所有权/接口/路径往返、移动目录、缺文件、异常写入、版本升级、引用修复；独立格式不冒充.sol |
| F-19 / P2 | 原生方案兼容 E1/I1/V0 | SolutionInspector；原方案分析证据 | 全参数/ROI/Group/资源/版本映射、无法识别字段保留、原生往返；当前只有部分只读恢复 |
| F-20 / P1 | 图像/结果/ROI宿主 E1/I1/V1 | MainWindow、WorkflowSourcePicker、ToolEditorContext；原始图像/动态配置控件 | 坐标/缩放/ROI交互、输出选择、历史及结果清理、空图/多图/运行切换、插件调试窗口与主显示同步 |
| F-21 / P1 | 生命周期与进程释放 E1/I1/V1 | WorkflowExecution、OnClosed、Check-ProcessExit.ps1 | 关闭/重复打开/运行中退出、事件订阅/定时器/相机/图像资源、异常与取消释放；本次退出检查不等于长期无泄露保证 |
| F-22 / P0 | 首个真实插件集成 E1/I1/V1 | VisionPlugins.ImageSource + 公共契约；插件线证据交付 | 工具导航→添加→配置→真实输入→执行→输出订阅→保存重开→关闭释放；文件及相机范围分别验收 |
| F-23 / P2 | 全库集成/部署/SDK E1/I0/V0 | 插件清单交付、公开 SDK/模块清单、发布与回归 | 全工具覆盖率与未实现项、干净机器独立运行、设备、SDK/API/ABI、长稳/性能；不得借单工具成功认定全库完成 |

以上入口简称均在 src 对应工程或 analysis/decompiled 中；补证据时登记精确文件、类/方法、资源键和版本/哈希。原生伪代码与托管源码分开标注，不能把函数签名当作完整算法。

## 2026-10-16 实施记录

F-01新增基础类型数组运行契约，已接通连线/订阅/Group/保存重开；源码确认的数据形态与独立托管接口见 [FRAMEWORK-CONTRACTS.md](FRAMEWORK-CONTRACTS.md)。此子项有独立运行验证，F-01整体仍未关闭：完整类型、原生精度/ABI及数组配置UI未完成。F-02随后完成单模块命令链与关闭路径审计，并接通WPF普通工具的调试会话及关闭等待，接口和未恢复边界见 [DEBUG-SESSION-CONTRACT.md](DEBUG-SESSION-CONTRACT.md)。新增Context.Session可空属性兼容旧构造函数，插件实现文件未修改；Group/设备及完整参数UI仍开放。

## 当前优先队列与依赖

| 顺序 | 条目 | 启动/完成条件 |
|---|---|---|
| 1 | F-01/F-02/F-22 | 记录现有插件接口与图像源实际需求；插件线接口请求到达后确认；先定义数组、会话及 WPF 生命周期，不并行改公共接口 |
| 2 | F-12/F-13 | 追踪循环相关字段到原生执行/运行证据，建立边界用例；来源未确认时不自定语义并标为原生还原 |
| 3 | F-14/F-15 | 在数据契约与循环上下文上实现累积输出；同时恢复配置交互而非只保存字段 |
| 4 | F-10/F-11 | 补跨方案复制、多级导航状态、事务与序列化边界，将近期缺陷纳入回归 |
| 5 | F-05～F-09 | 对照 Tab/ToolBox/Diagram/ZoomArea 逐事件审计正常及异常路径 |
| 6 | F-16～F-21、F-03/F-04 | 执行器与数据、存储、显示和窗口各命令逐项收口；持续集成插件 |

## 每个条目的操作级子清单模板

每个控件/事件一行，详细结果放该条目对应报告，不在此用“样式完成”代替全部操作：

| 控件/事件 | 原始类型/方法/资源 | 条件及状态变化 | 当前实现文件 | 正常/异常用例 | VM 对照证据 | 当前结论 |
|---|---|---|---|---|---|---|
| 示例：Group Tab Up | btnUp / UpCommandExecute | 内部模块集可向父级返回 | WorkflowTabs/GroupCanvasNavigation | 多层/根层/再进入/运行状态 | 尚未完成完整差分 | E2/I2/V1，未全量验收 |

只有能提供上述条目的证据及适用验收记录才能关闭。无法恢复的原生部分列出明确范围、继续分析路径及替代实现，不根据用户反馈猜测成品行为。

## 双线协作约束

- 不改插件 worktree，不重写插件线在途清单，不自动拉取/合并其未交付代码。
- 框架线维护本文件；插件线维护其详细插件台账。交付时由 HANDOFF 登记工具和提交编号；CONTRACT-REQUESTS 登记接口需求。
- 同一个工具的算法/UI验收由插件线记录，宿主导航/运行/订阅/保存/释放由框架线记录；最终集成仍需再次验证。
- 最近本地证据：analysis/full-audit/group-tab-return.html、group-tab-return-lifecycle/exit-results.json、group-owned-canvas.html、subprogram-runtime/results.json。测试数只说明已覆盖的回归，不升级全部条目状态。
