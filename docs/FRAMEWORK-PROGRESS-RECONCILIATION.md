# 框架逆向成果全面对账

2026-10-19；冻结基线 `70b3b4297a798cc0f0f7c149aeaf49be1031be72`。仅 main；tool-plugins 未访问、未计入。

**当前计划验收单元三层均达成：28/96 = 29.2%。**

这是本次逐项证据对账的结果，不是重跑测试后的原机等价结论。以前0/32只统计新计划的整包最终关闭，未回填早已合入main的子集成果；因此它不能代表当前整体逆向进度。此次修正统计，不将历史成果说成本轮新增。

| 层次 | 严格达成 | 进度 | 尚未达成 |
|---|---:|---:|---:|
| 源码 | 51/96 | **53.1%** | 45 |
| 实现 | 29/96 | **30.2%** | 67 |
| 验证 | 29/96 | **30.2%** | 67 |

## 分母与计数规则

总计划32包，每包3个具名验收单元，共96项；每项分别审核源码、实现、验证，因此每层分母均为96。下表列明全部单元，不按文件/方法/测试数量计权，不给部分完成折半。

综合值只计算三层同时达成的同一单元，不平均三层百分比。本次统计工具和进度文档不新增逆向完成项；D08仍为部分。

实现包括当前main的明确独立重构及计划要求的可执行逆向辅助工具，不能直接称VM等价。验证为有证据的已列操作范围；有已知反例则记失败。原方法合成输入、离屏WPF、原机差分分别保留边界。

历史验证未在本次重新执行；最近产品源码改动为6a2cf54的画布方案替换，后续批次主要是报告与探针。旧的framework-acceptance.json基线较旧，以专题报告和后续失败更正，不覆盖旧记录。

该分母是总计划的交付单元，不是原程序所有入口/函数的穷举，也不是工时预测。未来发现新范围须升级分母版本并保留前后差异，不能临时拆小已完成项抬高进度。

完整包仍为0/32关闭：这是最终收口门槛，不再作为唯一整体逆向进度。真实桌面、native ABI、设备和原生容器最终等价尚未通过。

## 32包汇总

| 包 | 范围 | 源码/3 | 实现/3 | 验证/3 | 三层同达成/3 |
|---|---|---:|---:|---:|---:|
| A01 | 工具加载与Group导入时序 | 3 | 1 | 1 | 1 |
| A02 | 主窗口状态与DPI | 2 | 2 | 2 | 2 |
| A03 | 流程Tab与Group导航 | 3 | 2 | 2 | 2 |
| A04 | 节点手势、路由和视口 | 3 | 3 | 3 | 3 |
| A05 | 菜单、工具栏及全局入口清点 | 1 | 1 | 1 | 1 |
| A06 | 运行界面编辑与显示布局 | 1 | 0 | 0 | 0 |
| A07 | 通用发现接入与缺插件体验 | 2 | 1 | 1 | 1 |
| A08 | 壳与交互全状态验收 | 1 | 1 | 1 | 1 |
| B01 | 实际运行Tab与变量目标 | 3 | 0 | 0 | 0 |
| B02 | Group身份、所有权和复制 | 2 | 2 | 2 | 2 |
| B03 | Group循环、中断与每轮生命周期 | 2 | 2 | 2 | 2 |
| B04 | 原生类型、复合叶及累积 | 2 | 2 | 2 | 2 |
| B05 | 流程IO、局部/全局变量与gvar | 1 | 1 | 1 | 1 |
| B06 | Group配置、ROI和结果消费边界 | 3 | 1 | 1 | 1 |
| B07 | 插件交接合同与真实消费 | 2 | 1 | 1 | 1 |
| B08 | 模型与Group组合验收 | 1 | 1 | 1 | 1 |
| C01 | 撤销root/part实际捕获 | 3 | 0 | 0 | 0 |
| C02 | 单次/连续/停止调度与参数准备 | 3 | 1 | 1 | 1 |
| C03 | 结果历史与方案替换生命周期 | 2 | 0 | 0 | 0 |
| C04 | 撤销恢复与临时文件所有权 | 2 | 0 | 0 | 0 |
| C05 | 配置/调试会话与框架释放 | 1 | 1 | 1 | 1 |
| C06 | 外部服务调度与故障隔离 | 0 | 0 | 0 | 0 |
| C07 | 长稳、并发与异常恢复 | 0 | 0 | 0 | 0 |
| C08 | 执行与历史组合验收 | 1 | 1 | 1 | 1 |
| D01 | sol单操作保存与native写入点 | 2 | 1 | 1 | 1 |
| D02 | 独立存储与资源事务补齐 | 3 | 2 | 2 | 2 |
| D03 | 原生容器与对象数据映射 | 0 | 0 | 0 | 0 |
| D04 | 版本、密码和迁移 | 1 | 0 | 0 | 0 |
| D05 | 原生兼容最小实现与往返 | 0 | 0 | 0 | 0 |
| D06 | SDK/API/ABI与部署依赖 | 0 | 1 | 1 | 0 |
| D07 | 真实工具集成及干净环境验收 | 1 | 1 | 1 | 1 |
| D08 | 全域缺口复查与交付 | 0 | 0 | 0 | 0 |

## 已知失败与主要剩余工作

- **D02.2：FCR-024资产Undo失败**。跨目录另存后Undo恢复插件相对路径却保留新基目录。需要纯资产重定位合同及用户选择的跨项目交接，不能硬编码插件私有字段。
- **A08.2：Shell默认键鼠/弹层验收失败**。控件事件模式通过不覆盖它；真实输入桌面/DPI矩阵仍缺。
- **A03/B01/C01/C02/C03/C04**：受控原实例、原日志装载/watch/output、真实参数捕获/回调/结果消费和释放。局部注册表映射已证明，但完整隔离未证明。
- **D01/D03/D04/D05**：native provider、writer/reader、mdata语义、旧版本/密码样本和原厂保存重开往返。只读外层解析不能当导入器。
- **A06/C06/B05**：运行界面编辑、全局变量/通信/触发/脚本服务、gvar等产品能力未交付。
- **B04/B06/B07/D07**：全部原生类型/ABI、完整候选/ROI/专用模板、新插件公共框架交接。FCR-015/018/019/021/022/024仍有未闭合范围；不自动采用插件线来信。
- **C07/D06/D07**：长稳/并发资源基线、干净机器和全依赖部署矩阵未完成。

## 全部96项逐项证据

达成/部分/阻塞/未取得交付/未验证/失败分别保留。每包的证据是已有报告及其原成员/指令/运行记录，代码位置是当前main的审查对象；路径存在本身不构成通过。

### A01 工具加载与Group导入时序

前置：无；完整包未因本表计分关闭。

证据：[LOADER-ROUND2-20261019.md](LOADER-ROUND2-20261019.md)、[TOOL-DISCOVERY-METADATA.md](TOOL-DISCOVERY-METADATA.md)、[A01-loader-batch1.md](reverse/A01-loader-batch1.md)

当前实现/工具位置：`src/VisionEditor/ToolDirectoryCatalog.cs`、`src/VisionEditor/ToolNavigation.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| A01.1 | 目录XML、分类优先级与惰性元数据读取 | 达成 | 达成 | 达成 | 只计元数据读取，不计工具DLL动态注册。 |
| A01.2 | 原加载器首次/悬停/刷新及失败分支 | 达成 | 部分 | 部分 | 原UpdateLoad合成输入已验；真实WPF与原Group模型落地未验。 |
| A01.3 | 原Group导入回调到模型落地 | 达成 | 未取得交付 | 阻塞 | CIL链可定位；原gro导入及实际回调未接入main。 |

### A02 主窗口状态与DPI

前置：A01；完整包未因本表计分关闭。

证据：[MF01-WORKAREA-LOCK-PROMPT.md](MF01-WORKAREA-LOCK-PROMPT.md)、[MF01-TITLE-MENU-ACCEPTANCE.md](MF01-TITLE-MENU-ACCEPTANCE.md)、[FRAMEWORK-WORKTREE-RETIREMENT.md](FRAMEWORK-WORKTREE-RETIREMENT.md)

当前实现/工具位置：`src/VisionEditor/WindowWorkArea.cs`、`src/VisionEditor/MainWindow.ShellLock.cs`、`src/VisionEditor/MainWindow.ShellWindowState.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| A02.1 | 标题拖动、最大化与恢复的命令状态 | 达成 | 达成 | 达成 | 计当前WPF命令/控件事件，不计跨DPI原机差分。 |
| A02.2 | 工作区锁定、确认及解锁 | 达成 | 达成 | 达成 | 原始提示资源/配置位置仍有独立适配；已列明差异。 |
| A02.3 | 真实桌面100/150/200%DPI与窗口边界矩阵 | 部分 | 部分 | 阻塞 | 仅当前子集布局，完整输入桌面矩阵缺证据。 |

### A03 流程Tab与Group导航

前置：无；完整包未因本表计分关闭。

证据：[WORKFLOW-TAB-TOOLBOX-GROUP-CLOSURE.md](WORKFLOW-TAB-TOOLBOX-GROUP-CLOSURE.md)、[A03-tabs-batch2.md](reverse/A03-tabs-batch2.md)、[A03-canvas-replacement-fix.md](reverse/A03-canvas-replacement-fix.md)、[A03-binding-batch4.md](reverse/A03-binding-batch4.md)、[A03-process-isolation-batch6.md](reverse/A03-process-isolation-batch6.md)、[A03-owned-logger-batch7.md](reverse/A03-owned-logger-batch7.md)、[A03-config-error-batch8.md](reverse/A03-config-error-batch8.md)

当前实现/工具位置：`src/VisionEditor/WorkflowTabs.cs`、`src/VisionEditor/GroupCanvasNavigation.cs`、`src/VisionEditor/WorkflowCanvas.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| A03.1 | 流程增删复制切换和Tab归属 | 达成 | 达成 | 达成 | 当前独立文档行为已实现并操作验证。 |
| A03.2 | 嵌套Group进入/返回和显示模型归属 | 达成 | 达成 | 达成 | 当前Group导航已验；不计所有原厂视觉状态。 |
| A03.3 | 每显示容器缩放持久化及非活动Tab历史 | 达成 | 部分 | 阻塞 | 原CIL/资源已追；main仍为会话视口，原绑定反馈受隔离条件限制。 第七批：原日志配置读取阶段失败；最终子进程退出码未知，finally日志缺失。受控核验发现自有键残留，根线仅清理该精确GUID键并确认不存在。装饰器/SDK运行仍未获证。 第八批：静态定位配置错误产生/吞没链；旧运行未保留内层异常，原因不可判定，不再运行。 |

### A04 节点手势、路由和视口

前置：A03；完整包未因本表计分关闭。

证据：[MF02-CONNECTION-ROUTE-LIFECYCLE.md](MF02-CONNECTION-ROUTE-LIFECYCLE.md)、[FRAMEWORK-WORKTREE-RETIREMENT.md](FRAMEWORK-WORKTREE-RETIREMENT.md)

当前实现/工具位置：`src/VisionEditor/WorkflowGestures.cs`、`src/VisionEditor/WorkflowWireEditing.cs`、`src/VisionEditor/WorkflowOverview.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| A04.1 | 节点拖放、多选、实时源复制和插入手势 | 达成 | 达成 | 达成 | 对应已列控件事件矩阵，真实键鼠全状态另属A08。 |
| A04.2 | 连线中点、插线、固定网格寻路及缓存 | 达成 | 达成 | 达成 | 原算法及实际WPF已验；不声称所有几何组合无差异。 |
| A04.3 | 视口缩放、小地图和修饰键事件路径 | 达成 | 达成 | 达成 | 已有事件/数值验收；跨DPI极端坐标完整矩阵另列。 |

### A05 菜单、工具栏及全局入口清点

前置：A02；完整包未因本表计分关闭。

证据：[MF01-GLOBAL-ENTRY-AUDIT.md](MF01-GLOBAL-ENTRY-AUDIT.md)、[MF01-TITLE-MENU-ACCEPTANCE.md](MF01-TITLE-MENU-ACCEPTANCE.md)、[MF01-SOLUTION-PATH-ACCEPTANCE.md](MF01-SOLUTION-PATH-ACCEPTANCE.md)、[FRAMEWORK-WORKTREE-RETIREMENT.md](FRAMEWORK-WORKTREE-RETIREMENT.md)

当前实现/工具位置：`src/VisionEditor/MainWindow.xaml.cs`、`src/VisionEditor/MainWindow.ShellHelp.cs`、`src/VisionEditor/MainWindow.ShellRunMode.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| A05.1 | 完整菜单/工具栏/快捷键入口清点 | 部分 | 部分 | 未验证 | 已有多个入口专题；尚非全部入口逐项覆盖。 |
| A05.2 | 常规文件、帮助、路径与运行策略命令数据链 | 达成 | 达成 | 达成 | 当前已交付命令及控件事件；原生后端/完整键鼠不纳入本单元达成。 |
| A05.3 | 全局变量、通信、触发、脚本等专用服务 | 部分 | 未取得交付 | 阻塞 | 原入口部分已定位，main不具备完整模块/会话/后端。 |

### A06 运行界面编辑与显示布局

前置：A05、B06；完整包未因本表计分关闭。

证据：[MF01-FRONTEND-UI-SOURCE-AUDIT.md](MF01-FRONTEND-UI-SOURCE-AUDIT.md)

当前实现/工具位置：未取得对应产品实现证据

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| A06.1 | 运行界面编辑入口、窗口状态与关闭流程 | 达成 | 未取得交付 | 未取得交付 | 托管入口和关闭分支已取证，未实现运行界面编辑器。 |
| A06.2 | Frontend完整布局、资源、BAML绑定还原 | 部分 | 未取得交付 | 未取得交付 | 已有资源保全；完整安全解码及布局合同未闭合。 |
| A06.3 | 运行界面结果绑定、持久化与重开 | 部分 | 未取得交付 | 阻塞 | 部分托管/SDK边界已知，实际设计文件及消费未验。 |

### A07 通用发现接入与缺插件体验

前置：A01、B07；完整包未因本表计分关闭。

证据：[TOOL-DISCOVERY-METADATA.md](TOOL-DISCOVERY-METADATA.md)、[ROI-REAL-TOOL.md](ROI-REAL-TOOL.md)

当前实现/工具位置：`src/VisionEditor/ToolDirectoryCatalog.cs`、`src/VisionEditor/MainWindow.xaml.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| A07.1 | 发现目录到真实ModuleRegistry的动态映射 | 达成 | 部分 | 部分 | ToolDirectoryCatalog仅元数据，尚未接主工具箱动态注册。 |
| A07.2 | 缺插件、版本冲突及刷新失败处理 | 部分 | 部分 | 部分 | 已有拒绝与缺目录子集，未有完整宿主刷新矩阵。 |
| A07.3 | 既有真实插件注册到根流程/Group消费 | 达成 | 达成 | 达成 | 仅main历史集成的图像源/二值等工具，不含tool-plugins在制。 |

### A08 壳与交互全状态验收

前置：A02、A03、A04、A05、A06、A07；完整包未因本表计分关闭。

证据：[FRAMEWORK-REVERSE-CHECKLIST.md](FRAMEWORK-REVERSE-CHECKLIST.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)、[FRAMEWORK-WORKTREE-RETIREMENT.md](FRAMEWORK-WORKTREE-RETIREMENT.md)

当前实现/工具位置：`tests/VisionEditor.WpfChecks/Program.cs`、`tests/VisionEditor.ShellChecks/Program.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| A08.1 | 已实现壳/画布/导航的控件事件回归交付 | 达成 | 达成 | 达成 | 本单元是验收套件交付，非全原厂功能闭合。 |
| A08.2 | 真实键鼠、弹层、DPI及原机同状态矩阵 | 部分 | 部分 | 失败 | Shell默认键鼠/弹层失败保留，control-events-only不得覆盖。 |
| A08.3 | 发布版全部壳状态的综合差分 | 部分 | 部分 | 未验证 | 有当前子集发布/退出记录，无完整原机综合矩阵。 |

### B01 实际运行Tab与变量目标

前置：无；完整包未因本表计分关闭。

证据：[VARIABLE-TAB-ROUND2-20261019.md](VARIABLE-TAB-ROUND2-20261019.md)、[B01-first-fill-batch3.md](reverse/B01-first-fill-batch3.md)、[B01-lazy-root-batch4.md](reverse/B01-lazy-root-batch4.md)、[B01-binding-batch5.md](reverse/B01-binding-batch5.md)

当前实现/工具位置：`src/VisionEditor/WorkflowVariableTargetsWindow.xaml.cs`、`src/VisionEditor/WorkflowSourcePicker.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| B01.1 | 真实运行Tab根、XmlLoaded和惰性装配 | 达成 | 部分 | 部分 | 16项原lazy分支仅Parent=null，真实proxy/SDK根未验。 |
| B01.2 | 候选类型、占用、首次/缓存及确认取消 | 达成 | 部分 | 部分 | 原选择器合成输入通过；原窗口与main元数据消费仍不全。 |
| B01.3 | 全局变量目标绑定写入及保存重开 | 达成 | 未取得交付 | 阻塞 | 原绑定链已取证；真实SDK转绑/失败/原方案重开未验。 |

### B02 Group身份、所有权和复制

前置：B01、A01；完整包未因本表计分关闭。

证据：[GROUP-RELATION-IDENTITY.md](GROUP-RELATION-IDENTITY.md)、[B02-identity-preflight-batch6.md](reverse/B02-identity-preflight-batch6.md)、[WORKFLOW-TAB-TOOLBOX-GROUP-CLOSURE.md](WORKFLOW-TAB-TOOLBOX-GROUP-CLOSURE.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)

当前实现/工具位置：`src/VisionFramework/OwnedGroups.cs`、`src/VisionEditor.Core/GraphEditorDocument.Groups.cs`、`src/VisionEditor.Core/GraphEditorDocument.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| B02.1 | main Group内部图创建、Parent及所属关系 | 达成 | 达成 | 达成 | 独立OwnedGroups合同已验，不把它当原生GUID分配。 |
| B02.2 | main实时源复制、内部图和代理引用重映射 | 达成 | 达成 | 达成 | 已有真实图像Group复制/撤销/重开；跨原生容器另列。 |
| B02.3 | 原gro导入ID/GUID及原生复制持久化 | 部分 | 未取得交付 | 阻塞 | 托管预审已完，native GUID分配与真实六场景未验。 |

### B03 Group循环、中断与每轮生命周期

前置：B02、C02；完整包未因本表计分关闭。

证据：[GROUP-LOOP-BOUNDARIES.md](GROUP-LOOP-BOUNDARIES.md)、[GROUP-ADVANCED-RUNTIME.md](GROUP-ADVANCED-RUNTIME.md)、[FRAMEWORK-WORKTREE-RETIREMENT.md](FRAMEWORK-WORKTREE-RETIREMENT.md)

当前实现/工具位置：`src/VisionFramework/GroupLoop.cs`、`src/VisionFramework/GroupBreak.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| B03.1 | LoopStep、范围与零值拒绝的当前重建 | 达成 | 达成 | 达成 | 有native算术证据；long/上限为明确独立安全策略。 |
| B03.2 | 中断比较、动态索引及执行前准备 | 达成 | 达成 | 达成 | 当前支持的类型/索引合同已验，非全原生参数编辑器。 |
| B03.3 | 每轮真实实例、回调与异常取消生命周期 | 部分 | 部分 | 阻塞 | 本地有隔离子集，原生每轮状态与设备路径未验。 |

### B04 原生类型、复合叶及累积

前置：B03；完整包未因本表计分关闭。

证据：[FRAMEWORK-CONTRACTS.md](FRAMEWORK-CONTRACTS.md)、[COMPOUND-LEAF-MAPPING.md](COMPOUND-LEAF-MAPPING.md)、[GROUP-INDEX-COMPOUND.md](GROUP-INDEX-COMPOUND.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)

当前实现/工具位置：`src/VisionFramework/Contracts.cs`、`src/VisionFramework/GroupOutputAccumulator.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| B04.1 | 基础标量/数组、基数与类型拒绝 | 达成 | 达成 | 达成 | int/float/string等当前合同与拒绝矩阵有证据。 |
| B04.2 | 有证复合叶投影与Point/Circle/Fixture传输 | 达成 | 达成 | 达成 | 只计已映射字段/独立托管形状；不计全ABI。 |
| B04.3 | 全部原生类型、pointset/POLYGON/Region ABI | 部分 | 部分 | 阻塞 | 未知类型禁用或独立标识；原生产者/ABI未闭合。 |

### B05 流程IO、局部/全局变量与gvar

前置：B01、B04；完整包未因本表计分关闭。

证据：[GROUP-LOCAL-VARIABLE-BINDING.md](GROUP-LOCAL-VARIABLE-BINDING.md)、[GROUP-LOCAL-VARIABLE-TYPE.md](GROUP-LOCAL-VARIABLE-TYPE.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)、[B05-gvar-preflight-batch7.md](reverse/B05-gvar-preflight-batch7.md)、[B05-native-dispatch-batch8.md](reverse/B05-native-dispatch-batch8.md)

当前实现/工具位置：`src/VisionFramework/WorkflowConfiguration.cs`、`src/VisionFramework/WorkflowSubscriptions.cs`、`src/VisionEditor/WorkflowLocalVariablesWindow.xaml.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| B05.1 | 流程IO和局部变量的作用域、订阅与改型 | 达成 | 达成 | 达成 | 当前已实现类型/操作，完整gvar/全局服务另列。 |
| B05.2 | 原全局变量模块的创建、服务与执行消费 | 部分 | 未取得交付 | 阻塞 | 有原全局绑定链，但完整模块/会话未交付main。 |
| B05.3 | 原gvar导入导出和往返 | 部分 | 未取得交付 | 阻塞 | 存在托管入口线索，无原格式往返和产品实现证据。 第七批：路径UTF8+NUL经GlobalValueImport/Export进入IMVS_SetBinaryData；原生格式消费者、覆盖/密码/事务仍未知，未运行SDK。 第八批：原生SetBinaryData入口经66f090到对象+23a8函数指针/0bd2消息；实际绑定、gvar格式仍未知。 |

### B06 Group配置、ROI和结果消费边界

前置：B01、B04、C01；完整包未因本表计分关闭。

证据：[GROUP-NATIVE-EDITORS.md](GROUP-NATIVE-EDITORS.md)、[GROUP-ADVANCED-RUNTIME.md](GROUP-ADVANCED-RUNTIME.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)

当前实现/工具位置：`src/VisionEditor/GroupRunConfigurationWindow.cs`、`src/VisionEditor/GroupRunCategories.cs`、`src/VisionEditor/GroupDisplayControls.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| B06.1 | Group候选资格、类别和代理多目标同步 | 达成 | 部分 | 部分 | 类别/同步已做；FCR-021资格元数据未全消费。 |
| B06.2 | 原ROI/专用参数模板及写回消费 | 达成 | 部分 | 部分 | 独立ROI子集可运行，FCR-018完整模板/实际原插件仍缺。 |
| B06.3 | 当前Group显示关联、样式及结果数据消费 | 达成 | 达成 | 达成 | 计有证的已实现关联/结果，不计原生全部显示工具。 |

### B07 插件交接合同与真实消费

前置：B04、B05、B06；完整包未因本表计分关闭。

证据：[PLUGIN-FRAMEWORK-BOUNDARY.md](PLUGIN-FRAMEWORK-BOUNDARY.md)、[CONTRACT-REQUESTS.md](CONTRACT-REQUESTS.md)、[ROI-REAL-TOOL.md](ROI-REAL-TOOL.md)

当前实现/工具位置：`docs/CONTRACT-REQUESTS.md`、`src/VisionPluginContracts/Contracts.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| B07.1 | 既有插件合同、能力及边界登记 | 达成 | 部分 | 部分 | 已有合同与FCR登记，但FCR未交付实现；登记不等于接口能力和操作已验收。 |
| B07.2 | 新插件公共框架的提交级交接与接口整合 | 部分 | 阻塞 | 阻塞 | 用户尚未选择交接，禁止采用另一工作树在制文件。 |
| B07.3 | main既有真实插件合同的根流程/Group消费 | 达成 | 达成 | 达成 | 只计已合入main的历史工具合同，不计新标定来信。 |

### B08 模型与Group组合验收

前置：B02、B03、B04、B05、B06、B07；完整包未因本表计分关闭。

证据：[GROUP-FULL-RECONSTRUCTION-CHECKLIST.md](GROUP-FULL-RECONSTRUCTION-CHECKLIST.md)、[GROUP-NATIVE-EDITORS.md](GROUP-NATIVE-EDITORS.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)

当前实现/工具位置：`tests/VisionEditor.WpfChecks/Program.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| B08.1 | 已支持Group嵌套/局部变量/图像组合用例 | 达成 | 达成 | 达成 | 当前重构组合已有真实输出、WPF操作和重开记录。 |
| B08.2 | 全类型/完整ROI/代理的组合验收 | 部分 | 部分 | 部分 | 测试子集不覆盖完整原生类型与编辑器。 |
| B08.3 | 原机混合故障/历史/持久化同状态差分 | 部分 | 未取得交付 | 阻塞 | 真实VM同状态组合未完成。 |

### C01 撤销root/part实际捕获

前置：无；完整包未因本表计分关闭。

证据：[UNDO-ROUND2-20261019.md](UNDO-ROUND2-20261019.md)、[C01-undo-batch1.md](reverse/C01-undo-batch1.md)、[C01-registration-batch5.md](reverse/C01-registration-batch5.md)

当前实现/工具位置：`src/VisionEditor.Core/GraphEditorDocument.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| C01.1 | root/part、旧新值与普通动作撤销 | 达成 | 部分 | 部分 | 原合成服务13项已验；main为独立文档快照。 |
| C01.2 | Group配置期禁止记录及异常复位合同 | 达成 | 未取得交付 | 阻塞 | 原root布尔语义已知；FCR-022统一seam未实现。 |
| C01.3 | 真实Group参数采集与SDK Undo/Redo | 达成 | 未取得交付 | 阻塞 | 静态注册链闭合，实际原Group/专属TempFiles未观测。 |

### C02 单次/连续/停止调度与参数准备

前置：无；完整包未因本表计分关闭。

证据：[MF05-CONTINUOUS-RUN-CALLCHAIN.md](MF05-CONTINUOUS-RUN-CALLCHAIN.md)、[MF05-PROCEDURE-RUN-CALLCHAIN.md](MF05-PROCEDURE-RUN-CALLCHAIN.md)、[C02-config-write-batch3.md](reverse/C02-config-write-batch3.md)、[C02-callbacks-batch4.md](reverse/C02-callbacks-batch4.md)、[C02-clear-leaf-batch6.md](reverse/C02-clear-leaf-batch6.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)、[C02-value-consumer-batch7.md](reverse/C02-value-consumer-batch7.md)、[C02-begin-isolation-batch8.md](reverse/C02-begin-isolation-batch8.md)

当前实现/工具位置：`src/VisionEditor/WorkflowExecution.cs`、`src/VisionFramework/GraphExecutor.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| C02.1 | 当前单次/连续/停止及失败状态执行 | 达成 | 达成 | 达成 | 独立执行器实际验证，非native调度器复刻。 |
| C02.2 | 原参数准备、等待与超时边界消费 | 达成 | 部分 | 部分 | 原叶等待可运行，但当前取消/超时策略与SDK不同。 |
| C02.3 | 清结果配置、回调队列及消费者状态 | 达成 | 部分 | 部分 | 原清值叶32项通过；真实Begin到消费者仍未闭合。 第七批：原Value getter受控5例35断言通过：清_value后可从merge列表或已注册RelateIO恢复；合成图不替代真实Begin/SDK/桌面。 第八批：真实Begin前置静态初始化/实例/后台线程/SDK副作用已审查，运行硬前提尚未满足，未重复合成getter。 |

### C03 结果历史与方案替换生命周期

前置：C02、D02；完整包未因本表计分关闭。

证据：[MF05-RESULT-LIFECYCLE.md](MF05-RESULT-LIFECYCLE.md)、[C02-config-write-batch3.md](reverse/C02-config-write-batch3.md)、[C02-callbacks-batch4.md](reverse/C02-callbacks-batch4.md)

当前实现/工具位置：`src/VisionEditor/MainWindow.xaml.cs`、`src/VisionEditor/ToolExecutionScope.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| C03.1 | 原结果视图切源、缓存与旧事件解绑 | 达成 | 部分 | 部分 | 主线有替换清理子集，原回调跨方案处置未全验。 |
| C03.2 | 清结果有效配置到实际SDK消费 | 达成 | 未取得交付 | 阻塞 | 原配置重载已明确；main未恢复同一条件控制。 |
| C03.3 | 历史、底部耗时与全局结果缓存生命周期 | 部分 | 部分 | 部分 | 当前独立摘要粒度不同，原全局UI策略证据不足。 |

### C04 撤销恢复与临时文件所有权

前置：C01、D01；完整包未因本表计分关闭。

证据：[UNDO-ROUND2-20261019.md](UNDO-ROUND2-20261019.md)、[C01-registration-batch5.md](reverse/C01-registration-batch5.md)、[C01-undo-batch1.md](reverse/C01-undo-batch1.md)

当前实现/工具位置：`src/VisionEditor.Core/GraphEditorDocument.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| C04.1 | 各根类型数据回放与栈迁移 | 达成 | 部分 | 部分 | main快照回放有验证，未适配原根类型全部载荷。 |
| C04.2 | 原TempFiles载荷所有权、淘汰和释放 | 达成 | 未取得交付 | 阻塞 | 原内存/文件分支已知，实际文件生命周期未验。 |
| C04.3 | 密码取消、恢复栈和异常边界 | 部分 | 未取得交付 | 阻塞 | 不能把RecoverCallBack泛化成事务回滚，缺真实案例。 |

### C05 配置/调试会话与框架释放

前置：C02、B07；完整包未因本表计分关闭。

证据：[DEBUG-SESSION-CONTRACT.md](DEBUG-SESSION-CONTRACT.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)、[FRAMEWORK-WORKTREE-RETIREMENT.md](FRAMEWORK-WORKTREE-RETIREMENT.md)

当前实现/工具位置：`src/VisionEditor/ToolEditorHost.cs`、`src/VisionFramework/ToolDebugSession.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| C05.1 | 普通节点调试单次/连续/停止/关闭等待 | 达成 | 达成 | 达成 | 历史集成会话子集已验证，不含所有真实设备。 |
| C05.2 | 换方案/配置并发关闭与回调解除 | 部分 | 部分 | 部分 | 有当前关闭等待子集，真实插件会话换源矩阵不足。 |
| C05.3 | 原SDK/设备取消、回调和资源释放 | 部分 | 未取得交付 | 阻塞 | 依赖真实可归属实例和已交付会话。 |

### C06 外部服务调度与故障隔离

前置：A05、C02、C03；完整包未因本表计分关闭。

证据：[MF01-GLOBAL-ENTRY-AUDIT.md](MF01-GLOBAL-ENTRY-AUDIT.md)、[CONTRACT-REQUESTS.md](CONTRACT-REQUESTS.md)

当前实现/工具位置：未取得对应产品实现证据

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| C06.1 | 外部触发、脚本、通信服务入口与消费 | 部分 | 未取得交付 | 阻塞 | 已识别入口部分；完整服务列表/后端未实现。 |
| C06.2 | 外部服务调度顺序、故障隔离与恢复 | 部分 | 未取得交付 | 阻塞 | 没有完整真实服务执行合同/操作矩阵。 |
| C06.3 | 真实设备连接、停止与关闭释放 | 未取得交付 | 未取得交付 | 阻塞 | 未获设备/会话证据，不默认操作连接设备。 |

### C07 长稳、并发与异常恢复

前置：C03、C04、C05、C06；完整包未因本表计分关闭。

证据：[MF05-PROCESS-LIFETIME-AUDIT.md](MF05-PROCESS-LIFETIME-AUDIT.md)、[GROUP-LOOP-BOUNDARIES.md](GROUP-LOOP-BOUNDARIES.md)、[FRAMEWORK-WORKTREE-RETIREMENT.md](FRAMEWORK-WORKTREE-RETIREMENT.md)

当前实现/工具位置：`tests/Check-ProcessExit.ps1`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| C07.1 | 长稳负载/时长及内存线程句柄基线 | 未取得交付 | 未取得交付 | 未取得交付 | 没有符合总计划定义的测量基线。 |
| C07.2 | 持续并发运行与资源增长验收 | 部分 | 未取得交付 | 未取得交付 | 已知队列/线程源码不等于长稳运行证据。 |
| C07.3 | 运行异常、失败后重启和关闭恢复 | 部分 | 部分 | 部分 | 已有局部恢复检查，未完成长稳并发故障矩阵。 |

### C08 执行与历史组合验收

前置：C02、C03、C04、C05、C06、C07、B08；完整包未因本表计分关闭。

证据：[MF05-PROCEDURE-RUN-CALLCHAIN.md](MF05-PROCEDURE-RUN-CALLCHAIN.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)、[GROUP-LOOP-BOUNDARIES.md](GROUP-LOOP-BOUNDARIES.md)

当前实现/工具位置：`tests/VisionEditor.WpfChecks/Program.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| C08.1 | 当前运行-编辑-撤销-保存重开组合 | 达成 | 达成 | 达成 | 只计已有独立WPF/执行组合记录。 |
| C08.2 | 嵌套错误、取消和历史释放组合 | 部分 | 部分 | 部分 | 局部用例存在，完整组合矩阵未完成。 |
| C08.3 | 原机执行/历史/资源的综合同状态验收 | 部分 | 未取得交付 | 阻塞 | 真实SDK/VM组合未验证。 |

### D01 sol单操作保存与native写入点

前置：无；完整包未因本表计分关闭。

证据：[SOL-ROUND2-20261019.md](SOL-ROUND2-20261019.md)、[D01-save-callback-batch5.md](reverse/D01-save-callback-batch5.md)、[D01-native-save-batch7.md](reverse/D01-native-save-batch7.md)、[D01-native-save-review-batch7.md](reverse/D01-native-save-review-batch7.md)、[D01-save-callees-batch8.md](reverse/D01-save-callees-batch8.md)

当前实现/工具位置：`tests/Inspect-NativeSolutionSample.ps1`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| D01.1 | 保存加载托管入口、回调及完成标志 | 达成 | 未取得交付 | 未验证 | 原CIL链已取，未运行原SDK保存回调。 |
| D01.2 | 8个原样本外层记录边界的只读解析 | 达成 | 达成 | 达成 | 只读辅助解析，非产品原生导入器；不解释未知首字段。 |
| D01.3 | native writer/reader与单操作字段归因 | 部分 | 未取得交付 | 阻塞 | 缺已配置native provider及受控保存差分。 第七批：两个保存导出原PE指令已取得并独立复核，SDK1段78行、ModuleFrame4段158行；跨模块callee、writer/reader字段及事务仍未闭合。 第八批：保存callee与下一层消费者原指令已取；SDK+2398绑定未知，ModuleFrame事件/回调封装不能视作落盘完成。 |

### D02 独立存储与资源事务补齐

前置：无；完整包未因本表计分关闭。

证据：[SOLUTION-PERSISTENCE-BOUNDARY.md](SOLUTION-PERSISTENCE-BOUNDARY.md)、[D02-storage-batch2.md](reverse/D02-storage-batch2.md)、[D02-asset-history-batch3.md](reverse/D02-asset-history-batch3.md)、[D02-storage-boundary-batch4.md](reverse/D02-storage-boundary-batch4.md)、[D02-interruption-batch6.md](reverse/D02-interruption-batch6.md)

当前实现/工具位置：`src/VisionFramework/SolutionStore.cs`、`src/VisionEditor.Core/GraphEditorDocument.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| D02.1 | 独立JSON结构/版本/大小/深度与拒绝 | 达成 | 达成 | 达成 | 原生格式不在本项；32MiB边界及编辑器状态已验。 |
| D02.2 | 资产嵌入、跨目录和多层历史身份 | 达成 | 部分 | 失败 | FCR-024真实插件SaveAs后Undo读取失败，不能算通过。 |
| D02.3 | 独立写入失败事务与受控进程中断 | 达成 | 达成 | 达成 | 锁/失败及自有中断已验；不是断电/所有阶段保证。 |

### D03 原生容器与对象数据映射

前置：D01、B02、B05；完整包未因本表计分关闭。

证据：[SOL-ROUND2-20261019.md](SOL-ROUND2-20261019.md)

当前实现/工具位置：未取得对应产品实现证据

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| D03.1 | 原XML/UI关系、身份和字段映射 | 部分 | 未取得交付 | 未验证 | 原样本/托管线索存在，尚无完整语义映射。 |
| D03.2 | mdata参数/模型/资源内部语义 | 未取得交付 | 未取得交付 | 阻塞 | 内部writer/reader及类型号未知。 |
| D03.3 | 原未知字段保持和对象图往返 | 未取得交付 | 未取得交付 | 阻塞 | 未实现原容器转换，不以猜测字段代替。 |

### D04 版本、密码和迁移

前置：D03；完整包未因本表计分关闭。

证据：[COMPATIBLETOOL-VERSION-MIGRATION.md](COMPATIBLETOOL-VERSION-MIGRATION.md)

当前实现/工具位置：未取得对应产品实现证据

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| D04.1 | CompatibleTool托管版本转换调度 | 达成 | 未取得交付 | 未验证 | 转换顺序/异常源码已审，原工具未执行。 |
| D04.2 | 版本依赖、缺失插件及转换失败恢复 | 部分 | 未取得交付 | 阻塞 | 没有完整旧版依赖/配置/格式矩阵。 |
| D04.3 | 旧版原样本迁移、执行与再保存 | 未取得交付 | 未取得交付 | 阻塞 | 缺旧版样本和native迁移往返。 |

### D05 原生兼容最小实现与往返

前置：D03、D04、B07；完整包未因本表计分关闭。

证据：[SOLUTION-PERSISTENCE-BOUNDARY.md](SOLUTION-PERSISTENCE-BOUNDARY.md)、[SOL-ROUND2-20261019.md](SOL-ROUND2-20261019.md)

当前实现/工具位置：未取得对应产品实现证据

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| D05.1 | 产品原生容器读取导入 | 部分 | 未取得交付 | 阻塞 | 只读检查器不等于可执行导入，产品仍拒绝。 |
| D05.2 | 产品原生容器写入导出 | 未取得交付 | 未取得交付 | 阻塞 | 无原生writer语义与可用实现。 |
| D05.3 | 原厂打开/执行/再保存最小往返 | 未取得交付 | 未取得交付 | 阻塞 | 原机互通未完成。 |

### D06 SDK/API/ABI与部署依赖

前置：A07、B07、D04；完整包未因本表计分关闭。

证据：[FRAMEWORK-CONTRACTS.md](FRAMEWORK-CONTRACTS.md)、[SOLUTION-PERSISTENCE-BOUNDARY.md](SOLUTION-PERSISTENCE-BOUNDARY.md)、[framework-acceptance.json](framework-acceptance.json)

当前实现/工具位置：`src/VisionEditor/VisionEditor.csproj`、`Start-Flow-Editor.cmd`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| D06.1 | 原SDK导出、结构体与ABI消费合同 | 部分 | 部分 | 部分 | 已有部分托管/CIL/PE坐标，非全部ABI。 |
| D06.2 | 当前win-x64子集打包启动和退出 | 部分 | 达成 | 达成 | 当前发布启动/退出有记录，非完整native部署。 |
| D06.3 | 干净机器全部依赖/设备SDK验收 | 未取得交付 | 未取得交付 | 阻塞 | 无干净机器全库/设备矩阵。 |

### D07 真实工具集成及干净环境验收

前置：D05、D06、A08、B08、C08；完整包未因本表计分关闭。

证据：[PLUGIN-FRAMEWORK-BOUNDARY.md](PLUGIN-FRAMEWORK-BOUNDARY.md)、[ROI-REAL-TOOL.md](ROI-REAL-TOOL.md)、[MAIN-PENDING-REVIEW.md](MAIN-PENDING-REVIEW.md)

当前实现/工具位置：`src/VisionEditor/MainWindow.xaml.cs`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| D07.1 | 用户选定新插件提交的逐文件交接 | 部分 | 阻塞 | 阻塞 | 未选新插件合流，不把来信或脏文件当交付。 |
| D07.2 | main历史真实工具的宿主端到端 | 达成 | 达成 | 达成 | 已有图像源/二值/Group执行及保存，不含新工具线。 |
| D07.3 | 最终干净发布、安装与退出完整矩阵 | 部分 | 部分 | 部分 | 当前子集启动记录不足以关闭全部交付。 |

### D08 全域缺口复查与交付

前置：D07；完整包未因本表计分关闭。

证据：[FRAMEWORK-REVERSE-CHECKLIST.md](FRAMEWORK-REVERSE-CHECKLIST.md)、[FRAMEWORK-PROGRESS-RECONCILIATION.md](FRAMEWORK-PROGRESS-RECONCILIATION.md)、[framework-progress-audit.json](framework-progress-audit.json)

当前实现/工具位置：`tests/ProgressAudit/Reconcile.py`

| 单元 | 验收范围 | 源码 | 实现 | 验证 | 仍缺/边界 |
|---|---|---|---|---|---|
| D08.1 | F-01至F-23原始入口/分支与32包全域对账 | 部分 | 部分 | 部分 | 本次只完成总计划与现有成果对账；完整原始入口清点、全部最终证据及原机验收仍缺。本次统计工具不新增逆向完成项。 |
| D08.2 | 证据、失败、FCR及验收源的最终一致性 | 部分 | 部分 | 部分 | 本次只完成总计划与现有成果对账；完整原始入口清点、全部最终证据及原机验收仍缺。本次统计工具不新增逆向完成项。 |
| D08.3 | 全部必需项收口及最终兼容声明 | 部分 | 未取得交付 | 阻塞 | 其余包未关闭，不能完成最终交付验收。 |

## 精确阻塞与解除条件

| 编号 | 类型/涉及单元 | 解除条件 |
|---|---|---|
| FAIL-ASSET-HISTORY | 已复现产品失败：D02.2 | FCR-024纯重定位合同、用户选择交接后实现并复验原失败场景 |
| FAIL-SHELL-INPUT | 已记录操作失败：A08.2 | 修复默认输入路径并复跑，不以control-events-only代替；桌面矩阵单验 |
| ORIGINAL-INSTANCE | 受控原实例/运行隔离：A01.3, A03.3, B01.3, B02.3, B03.3, C01.3, C03.2, C04.2, C04.3, C08.3 | 完成原日志受控装载/输出约束，专属SDK实例和被动观测后执行固定矩阵 |
| NATIVE-FORMAT | 原生引擎/格式/样本缺证据：D01.3, D03.2, D03.3, D04.2, D04.3, D05.1, D05.2, D05.3 | 提供已配置授权native分析环境和自有单操作原方案/旧版样本，逐字段追读写后往返 |
| PLUGIN-CONTRACT | 跨项目合同/提交交接：B04.3, B07.2, D07.1 | 按FCR责任边界取得已提交合同，用户选择交接后main逐文件消费与验收 |
| MISSING-CAPABILITY | 尚未交付的产品能力：A05.3, A06.3, B05.2, B05.3, C06.1, C06.2 | 原源码和数据流补齐后在所属项目实现，不以占位窗口/改名JSON代替 |
| DEVICE-RELEASE | 真实设备/会话资源条件：C05.3, C06.3 | 提供可归属本任务的受控设备/会话和固定操作/退出矩阵，不默认操作连接设备 |
| ACCEPTANCE-PREREQUISITES | 完整差分/部署前置未满足：A02.3, B08.3, D06.3, D08.3 | 完成相应运行/插件/native前置后按既定矩阵验收，保留失败 |
| FRAMEWORK-UNDO | main自身尚未实现的统一撤销能力：C01.2 | 由main实现统一历史seam并核对真实Group捕获/回放；不能把main职责转嫁给插件项目或仅等待用户交接 |

## 复算与审计

源数据：[framework-progress-audit.json](framework-progress-audit.json)。命令：`python tests/ProgressAudit/Reconcile.py`（本机使用明确的bundled Python路径）。

脚本验证32包/23域/96单元、状态、分母、报告/代码引用，复算百分比与交集，保留证据哈希和摘要。它不从关键词自动判定完成，不运行产品，不访问其他工作树。

本次证据快照、排除旧计分口径的原因和检查结果位于 `analysis/full-audit/progress-reconciliation-20261019/`。不存在以当前审计时间冒充历史运行时间的记录。
