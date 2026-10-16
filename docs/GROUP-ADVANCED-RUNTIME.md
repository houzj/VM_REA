> 后续实现：双栏配置器、图形ROI与真实图像源Group接入见 [GROUP-NATIVE-EDITORS.md](GROUP-NATIVE-EDITORS.md)。原生ROI算法工具、全部复杂模板仍未完成。下文适配器状态保留为历史阶段记录。

# Group 类别、关联、复杂编辑及首次 SelfRun

2026-10-16，框架 main。原始反编译文件哈希：group-advanced-source-manifest.json。当前实现和仍未还原的原生部分分开记录。

## 原始证据与已实现范围

- ParamConfigDataBase.AddGroup/DelGroup/UpGroup/UpDownParam：运行参数类别与变量组不同，默认上限32，当前原厂VisionMaster.exe.config第106行也配置32（原厂可配置覆盖）；类别有稳定ID和顺序，删除确认并删除成员代理。GroupRunParamConfigData 提供类别和组内参数排序。
- RunParamConfigDecorate.RelationModulesConfigCommandExecute/SyncCommandExcute：主关联和多个关联模块；同类型关联；同步取CloneSource值。DynamicNameValueItemOfGroup.SyncGroupRunParamWithMultiParams 对结构参数复制全部字段，不能只写一个值。
- ParamTypeList 明确数值、枚举、开关、单选、ROI、文件及模板训练等类型；ParamGroup.cs 显示范围为SliderArrange/SliderArrangeFloat双端点；Enumeration模板为原始ComboBox。复杂编辑须按明确元数据分派，不能按名字猜测。
- 已接入：独立运行类别列表、名称/顺序/成员设置、类别确认删除；组内代理上下移；关联树回显、添加和解除；同步和编辑原子写入全部目标；订阅/解除同样作用于全部目标。目标类型、复杂编辑器字段不兼容或数值非法时整批回滚。
- ParameterDescriptor.Editor 是可选框架能力：File/Folder/Enumeration/Range/Roi，显式字段、选项和文件过滤器。已有插件构造函数保留；没有修改工具实现或插件工作树。工具未声明元数据时不会自动生成专属编辑器。
- 复杂功能适配器能够实际保存路径/枚举/范围/ROI并进入模块运行；范围所有字段和ROI同时同步多个目标。ROI限制8项、Polygon32点，保存校验几何。新建ROI支持矩形/圆/圆环；已有多边形坐标保留。

## 首次 SelfRun 静态链路

原厂文件均只读分析；本任务使用隔离REA/Ghidra会话。原始JSON、伪代码、汇编及证据包保留于 analysis/full-audit/group-advanced-audit。

|原始模块 / 地址|证据|
|---|---|
|iMVS-6000PlatformSDK.dll IMVS_SelfExecuteOneModule 0x180645a50|校验SDK句柄，调用管理器；无直接执行父流程逻辑|
|SDK CServerManager::SM_SelfExecuteOneModule 0x180664060|发送命令；0x180664116汇编R9D=0x3f5，确认命令1013，避免伪代码漏掉可变参数|
|SDK SendCommand 0x1806c00d0|发送至服务端入口，检查OK/错误回复；不是本项目自动上游调度依据|
|ModuleFrame.dll CControlDelegate::RunNodeSelf 0x180043f60|解析节点与拥有流程，分派目标自执行|
|CProcedure RunModuleSelf 0x1800ddd80|运行忙状态检查、目标节点任务准备|
|CWorkFlow::JustRunModuleSelf 0x180100e00|必要时更新拓扑；先更新运行参数绑定，再调用目标节点；更新参数失败不会进入目标DoWork|
|CTopoDataMgr::UpdateProcedureTopology 0x18010ae80|首次准备分支实际是拓扑更新，不能误解成运行全部上游|
|CModuleParamBindingHandler::ProcessModuleParamBindings 0x1800b98c0|逐项解析、设置运行参数及报告失败|
|CGroupNode::PrepareInput 0x1800a4a00 → PrepareGroupRst 0x1800a4a90|处理input列表；int路径进入ProcessIntIO 0x180010450|
|CAbstractIOObject::ProcessIntIO → CUserModuleDelegate::GetInt 0x18011e580|按订阅记录读取来源、常量及结果；没有订阅/错误常量/结果不可用都有错误路径|

静态入口不支持“为了首次运行直接跑整个父流程”的推断。PrepareGroupData 0x18005ce70实际涉及加载方案文件，不是自执行输入准备；本报告不会混用两者。

本项目 GroupDebugExecutor.Prepare 显式解析父级输入/变量/缓存、运行参数及基数，补齐参数默认值，在进入内部图前验证所有必需输入；错误包含目标名，缺少来源不执行上游、不编造0。该预检及现有GraphValidator提供当前合同范围的首次准备。精确原生缓存布局、全部状态码和失败后继续/停止语义仍未等价；本项目预检失败即停是独立政策。

## 实际验收

- 多目标值同步、订阅与解除均进入实际模块输出；任一目标超范围全部回滚。复制Group后所有关联重映射；类别顺序和成员保存重开不变。
- WPF实际添加类别、移动类别、设置成员、选择关联、点击同步；Range4..8使两个模块各输出12。枚举2、真实BMP路径、ROI一项共同进入实际模块，输出16。
- 实际类别删除弹出确认，只删除代理不删除内部节点。复合范围反向、非法枚举和ROI几何通过独立专项覆盖。
- Prepare单独调用只准备输入，不创建/执行模块；缺少缓存明确报出in0及SelfRun数据准备失败。
- 本轮图像 wpf-group-advancedCategories-relations.png、wpf-group-range-editor.png、wpf-group-roi-editor.png 是tests对本项目WPF窗口的离屏渲染，已读取；不是VM截图，也不是原厂算法等价证据。测试模块仅验证宿主参数和ROI数据流。

## 尚未完成，不能标为原生完成

1. 原生DynamicRunParamConfigPanal的完整双栏树/分组GridView、全部主题、搜索和控件状态；当前类别/关联控件是有真实数据行为的功能重建。
2. 当前Range/ROI窗口是独立功能适配器，尚非原始SliderArrange/图像内ROI编辑器；ROI图形拖动、多边形点编辑、训练模板专属窗口未恢复。不能将它们包装成原生UI还原成功。
3. 现有真实工具还需根据其XML/代码声明Editor元数据或提供专属工厂；已写入CONTRACT-REQUESTS，不在本任务越权修改插件线。
4. 原生首次运行缓存生命周期/失效、全部数据类型、错误状态及实时设备差分未完成。未运行真实VM对照；当前通过的是独立实现验收。
本轮最终专项33项、实际WPF操作309项通过；原有中断54、循环17、调用34、数据31、数组28、调试21、核心115、持久化23回归通过。首次预检失败遗留旧成功结果的问题已通过实际窗口复验修复。最终发布/退出结果见HANDOFF。

额外联动验收：Group公共输入改名/删除会同步更新父级运行参数代理及其关联目标，避免持久化失效引用。

关联总数按ImvsSdkDefine.IMVS_DYNAMIC_PARAM_RELATION_LIST的128项数组界限校验（含主关联），不是按Group输入1024项限制。

ROI单元格无效数值会拒绝提交、保留原几何且窗口不关闭，修正后可成功运行。截图展示过校验提示的状态。直接写值/同步当前要求相关目标先解除参数订阅，原生订阅中的当前值同步语义仍需动态差分。公共动态参数名/API暴露的完整原生映射尚需进一步核对；当前提供界面与框架方法路由。

最终发布：dist/VisionEditor-WpfGroupAdvancedRuntime；启动入口已切换。Core/Plugin/WPF/EditorClose均exit0，90秒看门狗内退出，详见group-advanced-roi-final-lifecycle和HANDOFF。
