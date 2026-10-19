# 第十二批 B：完整关闭 B06.1 的实施前审查

基线 f53aa7b。结论：B06.1 仍缺未交付的候选能力合同，本片不实施局部补丁、不增加实施分子。未访问 tool-plugins；未运行原厂或 SDK；未改 Contracts、插件配置公共框架和共享台账。按 REA 技能复用已有本地反编译成员，本轮没有新增 REA Evidence 或原厂运行证据。

## 已核原链及默认值

`DynamicObjectConfigControl.UI.Data.RunParamPickData.Init` → `RunParamPickDataExtern.GetInsideRunParamTree/GetRunParamTreeNode` → `ExpendHolder.EnumEntrysA`。普通模块必须同时满足 `IGroupRunParam`、`CustomVisibleA`、`GroupRunParamEnable`、真实运行类型名属于过滤表、本地化名 `Contains(searchWord)`，并且 `AccessMode?.ToLower()=="rw"` 或非空 `DisplayForGroupType`。Group 模块走另一分支，按 GroupParams 下代理的 `CloneSource.GetType().Name` 过滤，不重复普通参数谓词。

`analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/NameValueItem.cs` 字段默认：GroupRunParamEnable=true、CustomVisible=true、AccessMode=null；并非所有元数据缺失都默认可写。ValueBase、Boolean、Enumeration 的接口 DisplayForGroupType=null；ROISelecter 为 `RoiConfigControl`。这不能外推所有插件自定义类型。

`DynamicObjectConfigControl.Common.ParamTypeList`：All 是七个具体类表的并集，而不是任意 descriptor 参数。数值含 Integer/Float/IntegerBettween/FloatBettween/IntegerBettween_CircleFind/Integer_CircleFind；枚举 Enumeration；开关 Boolean/BooleanTips；单选 RadioSelecter/RadioSelecterEx2；ROI ROISelecter/ROISelecter_Extern；路径与训练分别有原始显式类型表。完整列表保留在原文件。

`GroupRunParamConfigData.AsyncLoadParams` 重新构建类型和搜索树，重置 `_ParamList`，随后通过 `ExpendHolder.CheckedParams` 和 `ParamConfigDataBase.CheckParam` 恢复选择。CheckParam 使用 ParamsSource、原 UniquePath 与 UserData HeadPath；Group 与普通模块身份不同，不能把本地 Relations 简单视为原始占用字段。勾选经 CheckCommandExecute/AddParam/AddModule；批量新增在首次失败后撤后续勾选。删除失败保留勾选状态。

`DynamicObjectConfigControl.RunParamConfigDecorate.SyncCommandExcute` 查询原 SDK 关联数量；已知数量 <=1 时提示退出；多参数能力走 `DynamicNameValueItemOfGroup.SyncGroupRunParamWithMultiParams`，其他参数取 CloneSource.Value 后 SetParamUnitDirect。分别捕获 VmException/普通异常；不能由本地参数类型相同推定原多参数 ABI。

## 当前 main 的已实现范围和缺口

`GroupRunConfigurationWindow` 已有类别、精确主身份勾选、搜索和代理持久化；`GroupRunCategories` 有多目标普通值/复杂字段同步；`WorkflowRunParameters.Validate` 检查关联身份、类型、编辑器字段和 ROI 能力；`WorkflowRunParameter.Targets` 保留主目标与 Relations。上述是既有兼容实施，本次未重复计分。

`VisionFramework.Contracts` 的 ParameterDescriptor 仅 Name/Kind/Required/Default/Min/Max/Editor；PortDescriptor 仅 Name/Kind/Required/AcceptInteger/Cardinality。没有资格、原生类型、本地化名、原始身份/CloneSource 或 Group 展示能力。Kind/Editor 推断不能替代原谓词。

main 已有 `VisionPluginContracts.ToolConfigurationDefinition/State`，包含原 XML、GroupLink 展开、动态节点与 CustomVisible，必须复用，不能另造 XML 配置框架。`IToolConfigurationProvider.CreateConfiguration` 的返回值是有释放生命周期的编辑 session；没有发布 ModuleDescriptor 参数/端口与 live 配置节点之间的稳定 Group 候选身份映射，也不表达 IGroupRunParam、DisplayForGroup 工厂和 CloneSource。为构造候选而盲目启动编辑 session 还会引入业务回调与释放副作用，未据此执行。

完整解除条件为 FCR-021 的可选版本化生产合同：节点/参数稳定身份；装配后的原始类型及 Group CloneSource 类型；能力已知/未知；IGroupRunParam；动态 CustomVisibleA/GroupRunParamEnable；AccessMode；DisplayForGroupType/展示能力；本地化名；刷新与释放边界。main 仅消费该合同并保留不合格存量代理的诊断，不静默删除。仅补 Contracts 字段但没有真实已审计插件生产者，不足以关闭整个 B06.1。

## 关闭选择结论

B06.1 不应在合同缺失下进行新一轮局部修复。其余未完成 B 单元分别依赖真实装配、原 gro/gvar 格式、全局模块服务、SDK 每轮生命周期、完整原生 ABI、完整 ROI 或提交级交接；没有仅在 Group 配置独占文件内可完整关闭的替代 B 单元。已将此阻塞即时反馈根线，让本批资源转到依赖已齐的完整单元；本报告不是实施完成项。人工验收后置与此判断无关，阻塞来自缺实施合同而非人工验收。

本片自审核：逐字段对照原始默认值、两类候选分支和现有合同；没有新增测试镜像已有实现。证据文本哈希索引位于 `analysis/full-audit/batch12-20261019/B/source-hashes.json`。
