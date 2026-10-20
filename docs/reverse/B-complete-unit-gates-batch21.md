# 第二十一批 B：五个未完成单元的实际解除条件复核

2026-10-20，读取HEAD `1174604e5713994bc10eae5209a907d74f302129`。本片只审main及既有原源码，不读取tool-plugins，不改共享合同/产品，不构建或运行SDK。目的是排除重复派发与假关闭，不重写此前B12/B16的完整原方法报告。

|固定单元|当前代码事实|完整关闭缺口|解除条件|
|---|---|---|---|
|B01.1 真实运行Tab根/XmlLoaded/惰性装配|main有ToolConfigurationDefinition/State及可见性/订阅配置，不是原proxy运行Tab树；ModuleDescriptor没有运行Tab候选快照入口|原运行Tab物化、Link/Between展开身份及XmlLoaded后来源映射未交付|FCR019稳定生产快照/生命周期及实际已审计插件生产，main只消费|
|B01.2 候选类型/占用/缓存/确认取消|现窗口快照、DataKind筛选、占用排除、ShowDialog真分支提交已有|DisplayType与ComType两域、ISubInfo/SubscribeVisible、ModuleID+Name真实身份缺失|同上合同；不能用同Kind推断原资格，也不能重复局部cache测试算完整|
|B04.3 全部原生类型/pointset/POLYGON/Region ABI|Contracts没有原pointset/POLYGON载荷；CompoundOutputFields明写managed projection非ABI；Region仅已证矩形/完整圆环叶投影|完整producer结构、字段/基数、native所有权与序列化/执行映射未闭|FCR023的实际原生生产者/载荷合同经审查交付；不能把Point数组或RoiRegionValue改名冒充|
|B06.1 Group候选资格/类别/多目标|类别/关联已有；ParameterDescriptor无资格与CloneSource，候选仍遍历descriptor参数/输入|IGroupRunParam、GroupRunParamEnable、原运行类型/CloneSource、DisplayForGroupType、factory资格缺失|FCR021完整可选能力快照及生产者；原默认与两分支见B12，不复制配置模型|
|B06.2 原ROI/专用模板与写回|Group通过spec.Editor和IWpfToolParameterEditorFactory消费已有限能力|原GetSupportDisplayForGroupData装饰器、多参数/ROI模板及完整展示工厂未交付|FCR018提交级Group专用视图/原模板生命周期；不能在main另起插件配置框架|

本轮已核 `VisionFramework.Contracts` 的ParameterDescriptor/PortDescriptor未新增上述能力字段；`VisionPluginContracts.ToolConfigurationState`有CustomVisible/SubscribeVisible，但不能因此假设原IGroupRunParam或Tab_Run Params成员身份。主框架可复用该配置公共能力，不拥有其第二份实现。Group现IWpfToolParameterEditorFactory seam是明确有限消费，不能因为有一个factory接口就宣称全部原专用模板已交付。

原依据仍为 `RunParamsSelectData`/`RunParamsBaseNode`、`RunParamPickDataExtern`/`ExpendHolder`、`ParamTypeList`、ROISelecter/GetSupportDisplayForGroupData和GroupParam/DynamicOutput类型分支；其准确默认值、占用/首次缓存及未恢复ABI分别保存在B12/B16及FCR019/021/023。此轮没有新原生证据解除条件，不重复跑已通过合成探针。

**五项当前均不能在本线独占main文件内完整实施关闭。** 为遵守用户优先关闭未完成整项，本片不修已done的B04.1/.2，也不把B01缓存/Group搜索的另一小补丁变成完成数。本片实施增量0，障碍是实际缺合同/类型语义，非人工验收。没有必要新增镜像测试制造活动；已即时通知root释放方向给真正依赖齐全的实施/独立审查。

本片审计索引 `analysis/full-audit/batch21-20261020/B/gate-hashes.json`。请求仅登记既有合同解除条件，不向插件项目发指令、不采用他项目在制文件、不改变用户合流权。
