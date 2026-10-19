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

2026-10-19 配置上限闭合：原始 `DynamicObjectConfigControl/Apps.XmlParser.ParamTab/ParamConfigDataBase.cs:85–102` 静态构造先设 `MaxRunParamsNum=32`，再以 `int.TryParse(ConfigurationHelper.GetConfig("MaxRunParamsNum"), out result)` 覆盖；`AddGroupCommandExecute` 在 `GroupCollection.Count >= MaxRunParamsNum` 时显示本地化超限警告并返回。相同文件 `:489–496` 的代理添加路径按 `Collection.Count(p => !p.IsForGroup)` 执行同一上限检查。已安装 `D:\Program Files\VisionMaster4.4.0\Applications\VisionMaster.exe.config` 中该键值为32。

重建新增 `VisionFramework.WorkflowRunParameterLimit`，默认32，从当前应用目录的 `<FriendlyName>.config` 的 `appSettings` 读取覆盖；类别添加、代理选择及 `WorkflowRunParameters.Validate` 共用该值。`VisionEditor.exe.config` 发布默认值32。这个读取路径是对已确认键和值的独立XML重建；反编译中 `ConfigurationHelper.GetConfig` 实现和配置优先级/热重载没有证据，故不宣称其完全等价。惰性读取仅在进程首次访问时生效，需重启进程载入新配置。

`VisionEditor.GroupCategoryExpansionChecks` 现以应用配置覆写为2，操作真实WPF类别和运行参数复选框：创建第二类别和第二代理成功，点击第三项后两者各自保持2；非法配置回退32。首次实测第三代理仍能被添加，定位到单项候选复选框回调缺少上限判断（批量选择路径已有检查）；已补齐该回调的超限拒绝并让控件重建回未选状态。修复后重新编译并运行专项20项全通过；随后保存、重开方案并执行Group，输出仍为7。`VisionEditor.exe.config` 在构建输出中存在且值为32。该验证证明当前独立WPF链路读取配置、边界阻止和业务执行不回归；未与VM改写配置后的实时行为做动态差分。此前固定32的范围局限由本次修复消除。

ROI单元格无效数值会拒绝提交、保留原几何且窗口不关闭，修正后可成功运行。截图展示过校验提示的状态。直接写值/同步当前要求相关目标先解除参数订阅，原生订阅中的当前值同步语义仍需动态差分。公共动态参数名/API暴露的完整原生映射尚需进一步核对；当前提供界面与框架方法路由。

最终发布：dist/VisionEditor-WpfGroupAdvancedRuntime；启动入口已切换。Core/Plugin/WPF/EditorClose均exit0，90秒看门狗内退出，详见group-advanced-roi-final-lifecycle和HANDOFF。

## 2026-10-18 MF-04 独立运行输入与失败重试边界复审

本次重新逐项对照 `IMVSGroup.WorkflowAudit/IMVSGroupCs/IMVSGroupTool.cs` 的 `Run(bool)`：托管入口直接调用 `SyncSelfRun(ID, bIsRespondToUI)`，并未暴露输入缓存的构造、释放或重试协议。`VM.Core/VM.Core/VmModule.cs` 的普通模块 `Run()` 调用另一入口 `SelfRun`，在 `VmException` 时清理 `SetInnerModuleIsSelfRuning` 和 `IsRunning`；它不能直接当成 Group 同步入口的回调顺序。原生第一次输入准备仍只能依据上表 ModuleFrame 原生调用链，不得从托管 Group 方法推断“自动执行上游”或具体缓存 ABI。

当前 `VisionFramework/GroupDebugExecutor.cs` 的 `Prepare` 接收显式父级快照，逐个解析连接和订阅，收集目标错误；`RunAsync` 用独立临时流程执行。它在缺值时失败而不复用旧值，是已验收的独立实现。输入类型进一步扩展到原生 `VM.Core/VM.Core/DynamicInput.cs` 的 Line、Polygon、Pointset 等，需要相应 `PortValue`、插件生产者和序列化契约；本线不直接改共享 `Contracts` 或启用无运行支持的复合输入控件。输出 `VmIODecorate.IsAccumulatedOutput` 对原生 Group 输出显示模式开关，但 `IOModes` 的复合叶模式和 `ProcessGroupOutputV44` 数据 ABI 尚不足以安全实现 Components/Region 累积；不能把显示开关当作复合运行已闭合。

此前同工作树的 `VisionEditor.GroupCategoryExpansionChecks` 已在真实 WPF 窗口完成 17 项检查，包含保存失败注入、重试后仅新增一个代理、实际 Group 输出 7 和保存重开；这证明独立窗口事务边界，**不**证明原生 `SyncSelfRun` 失败状态码或缓存清理等价。本次复审没有发现可在 MF-04 独占文件中独立修复、同时具备原始证据和端到端运行依据的新差异，因此没有更改运行代码。原生 Group 缓存 ABI、连续停止时回调时序、所有复合输入/输出叶报告模式仍保持开放。

## 2026-10-19 Group内部状态与ProcessManager回调静态审计

目前不能把Group内部中断/错误整数映射成外层ProcessManager工作状态。证据链分成不同层：

- 原生 `analysis/full-audit/group-native-audit/group-run.c` 的 `FUN_1800a0930`（`CGroupNode::DoWork`）第551行调用 `FUN_1800a55b0`（`IsBreakLoop`）；第630行记录中断索引；第912行日志包含输出准备返回值和 `nSubModuleProcessStatus`；第1154行以 `status` 与 `nErrCode` 记录Group结束；第1180行返回 `iVar15`。这证实Group DoWork内部有子模块状态、错误码和最终返回值，但该文本没有给出返回值枚举定义，也没有解出这些值到管理器消息字段的映射。
- 托管Group入口 `analysis/decompiled/IMVSGroup.WorkflowAudit/IMVSGroupCs/IMVSGroupTool.cs:74–76` 只调用 `mModuleManager.SyncSelfRun(ID, bIsRespondToUI)`。同文件 `:119–122` 另有 `GetModuErrorInfoList` 从ModuleManager查询Group内部错误；这是错误查询API，不是ProcessManager工作状态回调。
- `GroupResult.ModuStatus` (`analysis/decompiled/IMVSGroup.WorkflowAudit/IMVSGroupCs/GroupResult.cs:22–52`) 是内部可写属性，三个构造器均初始化为0；`IMVSGroupTool.ModuResult` 查询 `QueryModuleOneResult` 并交给 `ModuleResultManager.SetModuleResult`。现有C#源码没有显示该结果对象与 `CGroupNode::DoWork` 返回值或ProcessManager回调字段间的转换。
- `VM.Core/VM.Core/VmSolution.cs:4067–4171` 的 `MProcessManager_OnWorkStatusCallBack(IMVS_MODULE_WORK_STAUS)`按流程ID/执行次数处理；`nWorkStatus == 0` 走流程空闲/执行结束分支，其余值进入开始分支。`:3866–3910` 的停止回调按 `nStopAction` 1、2、3更新流程连续运行与等待状态。这些是流程级开始/空闲/停止状态，不是上述Group节点结果码；源码未提供两者的直连映射。
- 本项目 `VisionFramework/GraphExecutor.cs:20–26` 将内部子流程非Success折叠为父Group节点 `ModuleStatus.Failed`；令牌取消/等待超时映射为本地 `Cancelled`/`TimedOut`。Group条件中断按独立的成功终止处理。该数据流可在本项目运行验证，但因原生返回码映射缺失，不能宣称本地枚举与VM所有NG/异常/主动停止逐项等价。

### 托管 SDK 桥接的进一步追踪

2026-10-19 对 `VM.PlatformSDKCS.Audit` 的调用封装与回调注册继续静态追踪。Group 调用与数据读取链可证到托管/PInvoke边界：

- `IMVSGroupTool.Run(bool)` → `ModuleManager.SyncSelfRun(uint,bool)`（`analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleManager.cs:266–342`）。`SyncSelfRun` 按模块ID区间清理对应 self-execute flag，随后实际调用 `ServerSDKAPI.IMVS_SelfExecuteOneModule(handle,nModuleId)`，再读取 `IMVS_GetModuleState` 并按 Group 对应 flag 进入 `SyncWaitor.PrecisionWait`；它没有在托管代码中接收 `CGroupNode::DoWork` 的返回整数。对应导出封装在 `ServerSDKAPI.cs:229–232`，P/Invoke 声明为 `NativeMethods.IMVS_SelfExecuteOneModule`，库名 `".\\iMVS-6000PlatformSDK.dll"`（`NativeMethods.cs:94–95`）。
- `IMVSGroupTool.ModuResult` → `ModuleManager.QueryModuleOneResult`（`ModuleManager.cs:1045–1069`）→ `ServerSDKAPI.IMVS_QueryModuleHistoryResult`（`ServerSDKAPI.cs:1173–1176`）→ 同名 `NativeMethods` P/Invoke（`NativeMethods.cs:355–356`）。托管端传入模块ID、索引0、请求1项与 `bResBuffFirst`，并从 `IMVS_QUERY_MODULE_RES_INFO_LIST` 解出 `IMVS_MODULE_RESULT_INFO_LIST_EX`。其字段包含 `nStatus`、`nErrorCode`、`nCycleID`、`nModuleID`、模块/算法耗时、自执行标记及执行次数（`ImvsSdkDefine.cs:1200–1232`）。
- `IMVSGroupTool.GetModuErrorInfoList` → `ModuleManager.GetModuErrorInfoList`（`ModuleManager.cs:411–418`）→ `ServerSDKAPI.IMVS_GetModuErrorInfoList`（`ServerSDKAPI.cs:942–945`）→ `NativeMethods.IMVS_GetModuErrorInfoList`（`NativeMethods.cs:559–560`），参数为目标对象ID及 `IMVS_MODULE_RUN_ERROR_INFO_LIST`。托管结构声明 `nNum`、16项 `stModuErrorInfo`；每项有 `nModuleID`、`nErrorCode`、模块名和显示名（`ImvsSdkDefine.cs:202–225`）。这些可证明错误列表的 ABI 形状，不能证明 native Group 怎样填充/排序该列表。
- 回调入口由 `CVmObjectBase` 构造/初始化路径将 `PlatformInfoCallBack` 注册到 `IMVS_RegisterOutputPlatformInfoCallBack`（`CVmObjectBase.cs:493–500`，P/Invoke声明 `NativeMethods.cs:22–23`，库同上）。回调处理 `nInfoType == 14` 时将 `pData` Marshal 为 `IMVS_MODULE_WORK_STAUS` 并触发 `CVmObjectBase.OnWorkStatusCallBack`（`CVmObjectBase.cs:831–838`）。`ProcessManager` 构造函数注册 `GetWorkStatusCallBackInfo`（`ProcessManager.cs:135`）；该处理器只在 `nWorkStatus == 0` 时按 `nProcessID - 10000` 更新流程级 `mProcedureNormalFlag` 与 `mProcessRunFlag`，之后转发整个结构（`:630–650`）。结构注释把 `nWorkStatus` 定义为0空闲、1忙、2停止，另有 `nIsExecuteNormal`（`ImvsSdkDefine.cs:713–733`）。这条路径确认的是流程工作状态通知，不是 `nSubModuleProcessStatus` 到 Group 返回值的映射。

**可证的停止边界：** `NativeMethods` 中上述 SelfExecute、QueryModuleHistoryResult、GetModuErrorInfoList、RegisterOutputPlatformInfoCallBack 均为对 `".\\iMVS-6000PlatformSDK.dll"` 的 DllImport。目前反编译目录提供对应托管封装与 native `CGroupNode::DoWork` 伪代码，但没有找到该 P/Invoke DLL 的实现反编译或 native 符号/入口。故不能证明 `CGroupNode::DoWork` 的 `iVar15`/`nSubModuleProcessStatus` 如何写入 `IMVS_MODULE_RESULT_INFO_LIST_EX.nStatus/nErrorCode`、`GroupResult.ModuStatus` 或 `IMVS_MODULE_RUN_ERROR_INFO_LIST`，也不能证明内部异常、NG、用户停止之间的枚举转换。此缺口保持未知，不用日志值、字段名或独立实现替代证据。

本项目 `VisionFramework/GraphExecutor.cs:20–26` 的 Failed/Cancelled/TimedOut 归并仍是独立实现；不作 VM 状态码等价声明。后续如需闭合，需要该 SDK DLL 的实现级反编译/匹配 native 符号，或在真实 VM 对“首轮中断、内部NG、抛异常、用户Stop”分别采集 `QueryModuleOneResult`、`GetModuErrorInfoList` 与流程回调结构作动态差分。
