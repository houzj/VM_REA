# 节点插件影响面：双线任务与集成验收清单

更新：2026-10-17。用户分别管控两个项目。分工和文件锁见 [边界规则](PLUGIN-FRAMEWORK-BOUNDARY.md)，当前 Git 状态与同步闸门见 [开发基线](TWO-PROJECT-BASELINE.md)。这是 `main` 在用户选择合流时使用的**集成清单**，对应插件线在制 `docs/PLUGIN-FRAMEWORK-CHECKLIST.md` 的 PF-001～014；不复制其未提交实现，也不把插件线测试数记作 main 验收。独立任务分配见 [双项目清单](TWO-PROJECT-WORKLISTS.md)。

状态词：`主线现有子集`、`插件线在制（未合）`、`待原生审计`、`待合流`、`待原厂差分`。通过本项目测试只能关闭托管实现层，不能自动关闭原生等价层。

| ID / 顺序 | 对节点工具的影响及原生依据 | 主责、协作 | 当前状态 | 合流验收条件 |
|---|---|---|---|---|
| NFI-01 / P0 | **配置模型**：`ModuleParam→AlgorithmTabRoot→ParamsConfigControlBase→ParamsTabUserControl`；页签、组、尺寸、底栏、状态不能仅由执行 `ParameterKind` 猜出 | 插件线 PF-001/003；main 审核 | 插件线在制（未合）；main 只有 `ModuleDescriptor` 和各工具自建窗 | 普通工具配置定义完整序列化/读回；隐藏、继承、无效与分组变化作用于真实窗口及运行值 |
| NFI-02 / P0 | **公共模板**：`NameValueItem.DisplayType` 与 `paramusercontroldatatemplate.xaml` 选择控件；专属内容和顶层窗口分离 | 插件线 PF-002/005/010；main 审核 | 插件线模板目录与单值控件在制（未合） | 原生展示键逐项映射；未知键显式报缺口；图像源、标定加载、N点各一例对照 UI/事件/数据 |
| NFI-03 / P0 | **窗口宿主**：普通参数窗和带渲染窗有不同结构；`VmModuleDecorator` 普通窗的 Owner、同节点重用、切换关闭 | 插件线 PF-004；main 负责 WPF Shell 接入 | 插件线普通非模态宿主在制（未合）；main 目前 `IWpfToolEditorFactory` 任意 Window＋`ShowDialog` | 根流程/Group打开、重复打开、切换节点/流程/方案、关闭/取消/释放；专属窗口过渡兼容清楚标注 |
| NFI-04 / P0 | **订阅与动态端口**：`ButtonSelecter`、`SetFilterSourceOperation.RelateIndex`、`VM.Core.DynamicInput/Output` 的标量/数组/叶与动态 schema | 插件线 PF-006 提配置树/动态节点描述；main 审核与 Group/复合端口合流 | 两边均有独立子集；共享文件插件线在制 | 类型和顺序校验、整数索引、Group父输入/局部变量、叶路径、动态增删输出、保存重开和失效引用清理；普通端口不被隐式放宽 |
| NFI-05 / P0 | **单节点调试/配置执行**：`BottomCommand→VmModule.Run→ModuleManager.SelfRun` 与回调/连续/Stop状态 | 插件线 PF-007/008/009 配置执行；main 负责流程/Group会话和结果作用域 | 两边有不同实现子集，待合流 | 同一节点根流程/Group单次、连续、停止、失败、切换方案、关闭；不可读旧缓存、不可跨流程写结果；300ms等原生时序单独差分 |
| NFI-06 / P0 | **ROI、Fixture、Polygon与图像预览**：原生参数模板、ROI XML 与 `DynamicInput/Output` 各自定义数据形状 | main 现有 `VisionUi`/Region与Fixture协议；插件线 PF-011 提真实工具参数/专属控件 | main 部分 ROI/Fixture、独立 Polygon 容器；插件线桥接在制 | 同一个共享模型在参数窗、Group、执行器和预览间往返；动态ROI/Fixture零尺度/Polygon对象边界不得被同名类型粗暴替换；实工具算法消费验证 |
| NFI-07 / P0 | **工具发现与工具箱**：`ToolBoxData.UpdateLoad` 扫 `ToolItemInfo.xml`，`ToolItemGrid` 按模块类型/路径拖放；当前项目硬编码两插件与静态目录 | main 主责；插件线 PF-012 提工具元数据/清单 | 原生目录审计及独立元数据扫描子集已完成，见 `TOOL-DISCOVERY-METADATA.md`；主窗/注册未接入 | 已注册插件能在正确分类展示并拖入；缺失/版本冲突/禁用、图标及类别重载明确；不扫描任意未知 DLL 后立即执行 |
| NFI-08 / P0 | **工具结果、显示与错误**：原生输出叶、图像结果、模块状态和 UI 显示规则并行存在 | 插件线给每工具输出/显示适配；main 负责 `PortValue`、复合叶、图像结果宿主 | main 有少数类型子集；插件线显示框架在制 | 真实插件输出→下游订阅→Group累积/结果显示；NoMatch/失败/取消不泄漏上次结果；原生缺失指标不捏造 |
| NFI-09 / P0 | **存储与资源**：原生 `.sol`、插件独有配置/模型文件和模块目录；当前 `.vmx.json` 为独立格式 | main `SolutionStore`；插件线供专属配置读写/文件语义 | 双方子集；原生 `.sol` 往返未完成 | 同一工具参数、动态端口、文件/ROI、订阅与结果配置保存重开；路径移动/缺文件错误明确；不宣称 `.sol` 等价 |
| NFI-10 / P0 | **真实迁移样本**：先用图像源/标定加载证明公共模板与宿主，再用 N点/N图像覆盖复杂控件 | 插件线 PF-013；main 集成 | 插件线在制（未合），main 现有图像源/二值化子集 | 逐工具原厂资源+受控运行+实际主窗口操作；先构建留人工验收，再按提交合流；不按“窗口能打开”判定完成 |
| NFI-11 / P0 | **共享文件与分支基线**：插件线已授权改自身工作树中的必要共享代码；main 同文件不可并发改 | 用户决定合流；插件线交付提交和差异，main 负责集成实现 | 插件线多个共享文件未提交，暂不合 | 提交号、变更清单、原生证据、合同差异、兼容矩阵齐备；main 逐文件增量集成并复跑完整回归 |
| NFI-12 / P1 | **设备/原生 ABI 差分**：原生 SDK 的相机/光源/通信、原生精度与时序 | 两线按所有权分别取证 | 未完整；本轮设备测试排除 | 不因无设备阻塞托管子集，但明确标原生设备/ABI未验；不得用模拟服务冒充设备证据 |

## 不重复逆向的执行规则

1. 每个新工具先在插件线清单登记原生二进制/资源、参数/UI展示键、执行与结果；用 NFI ID 指向所需公共能力。若已有共享实现，只补工具适配器，不再写公共模板副本。
2. 公共配置框架证据由插件线集中维护；主线只审查与流程、Group、ROI、数据端口、导航集成有关的交界。主线对同一 `ButtonSelecter`/模板所做的既有局部实现记为过渡子集，合流时逐状态比较，不简单累计完成率。
3. 交叉类型或入口先写契约差异：调用方、数据形状、生命周期、旧方案兼容、源码位置及验收样例。指定**一个**共享文件提交所有者；另一线提交证据和测试需求，等提交后再合。
4. 主线按 NFI-01→05→07→08→09→10 的依赖顺序合流；ROI/数据形状 NFI-06 要在真实工具消费前先定形。插件线可继续独立取证和算法，不把未合公共代码视为主程序能力。
5. 插件线每个工具任务必须同时列出依赖的插件公共框架能力、缺口和实际主窗口/Group消费结果；主线每个供插件使用的新接口必须用至少一个真实工具复验。不能出现“工具功能已实现但公共节点框架未接”或“框架接口已做但真实工具未消费”却宣称闭合。

相关原生入口：`analysis/decompiled/VMControls.Data/VMControls.Data/ToolBoxData.cs`、`analysis/decompiled/VMControls.WPF/VMControls.WPF.Base/ToolItemGrid.cs`、`analysis/decompiled/VM.Core/VM.Core/IMVSGroup.cs`、`analysis/decompiled/VM.Core/VM.Core/DynamicInput.cs`、`DynamicOutput.cs`、`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/ButtonSelecter.cs`；插件线配置架构原始路径与哈希见其 `configuration-*-evidence.json`。本表仅记录核对到的源码/在制状态，未在本轮执行原厂桌面差分。
