# 第十六批 B：B01.2 完整实施前置审查

范围为原固定“候选类型、占用、首次/缓存及确认取消”。未实施局部补丁、未构建、未执行原厂/SDK、未接触tool-plugins。原源码证据复用 B01 第二/三/四/五批，身份和CIL摘要均在各报告；本片不重复调用已知预算失败的REA查询。

## 当前 main 不是只缺窗口缓存

`WorkflowLocalVariablesWindow.TargetOptions` 按 descriptor.ParameterKind/DataKind 匹配，并把参数与输入合并，排除被其他来源订阅的目标及已有输入连线。`WorkflowVariableTargetsWindow` 在构造时 ToArray快照，分型只过滤DataKind，选择列表自有拷贝，双击添加按(NodeId,Port,IsInput)幂等；确认DialogResult=true，关闭/取消不提交。调用方BindTargets只在ShowDialog==true后建proposed配置并GraphValidator检查，再save。

这些已经提供main兼容支持域的快照、占用和取消事务，但不能等同原RunParamsSelectData。该原选择器来自装配后的Tab_Run Params树：类型展示按DisplayType，绑定按ISubInfo/SubscribeVisibleA/ComType，身份ModuleID+Name。首次全部类型才形成缓存，首次直接分型不形成缓存；缓存后切分型不重算占用/变量类型。原首次筛选还保留不可订阅项与已选例外；确认才独立拒绝other重复、可订阅但ComType不同，普通最多32、变量绑定无限。当前WorkflowSourceOption没有这些字段，Kind相同不能替代DisplayType/ComType两域。

原GetSelectedRunParams成功后清旧SelectRunParams列表并替换；GlobalVariableItem弹窗前先重建OldSelectedRunParams，再在true分支写回SelectedRunParams。取消不写绑定不代表调用方绝无快照副作用。输入目标另有ModuleInputSelectData，不能合并成单一CanBind。main三标量局部变量支持与原全局模块支持域不同，不能把原全局选择器完整责任悄悄限定成局部窗口已有行为。

## 精确阻塞及可实施锁

完整B01.2需要FCR019稳定候选快照：实际Tab归属、原DisplayType、ComType、ISubInfo支持与SubscribeVisible分别、ModuleID+Name/动态Between展开身份、已选/他占用上下文，以及首次读取的生命周期。main现有ToolConfigurationState不能单凭CustomVisible或XML Tag证明原接口与运行Tab树。共享合同字段仍未交付；本片没有借实施之名复制插件配置系统或默认候选资格。

合同若可消费，独占窗口可为WorkflowVariableTargetsWindow(.xaml/.cs)、新纯SelectionSession与专项测试；根线唯一修改WorkflowSourceOption/调用方及共享合同。没有生产方真实元数据时，仅增加cache/occupied/cancel测试是未完成项内局部修复，不能达到用户本批要求的完整关闭，故本片停止该局部实施。

## 备选B05.3并不存在已证XML格式

`Apps.XmlParser.Variable.VariableManager.ExportVar` 将UTF8+NUL路径写IUserBytesData Params[GlobalValueExport]；ImportVar先写GlobalValueImport，随后解绑旧输出、清索引/订阅、回读GetAllVar重建。真实文件编码由native消费者决定。此前B05第7/9批已追到SetBinaryData/PlatformManager桥，但没有gvar reader/writer语义。现main局部变量JSON导入导出属于独立格式，不能改名gvar或虚构XML helper作为原gvar完成；本片未这样实施。

自审核结论：B01.2与B05.3均不满足完整实施条件，分子增量0；障碍是未交付语义合同/未恢复native格式，不是人工验证。已即时告知根线让资源改派依赖已齐的完整单元，避免重修已完成B单元。本片哈希索引 `analysis/full-audit/batch16-20261019/B/evidence-hashes.json`。
