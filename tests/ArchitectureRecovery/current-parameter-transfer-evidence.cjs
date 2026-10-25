'use strict';
const fs=require('fs'),path=require('path');
const {source,out}=require('./evidence-view.cjs');
module.exports=function currentParameterTransfer(){
 const refs=(p,member,anchor,count=10)=>source('src/'+p,member,anchor,count);
 const doc=(member,anchor,count)=>refs('VisionEditor.Core/GraphEditorDocument.cs',member,anchor,count);
 const graph=(member,anchor,count)=>refs('VisionFramework/GraphExecutor.cs',member,anchor,count);
 const facts=[];
 const add=(id,stages,from,to,behavior,representation,cache,release,evidence,unknowns)=>facts.push({id,stages,modes:['all'],classification:'independent-implementation',from,to,behavior,representation,cache,release,refs:evidence,unknowns});
 add('CP01',['input','processing'],'工具配置Apply与编辑文档','经校验的JSON参数副本',
 '常规工具配置窗口的Apply先核方案/目录/窗口与执行所有者，并在activeRun非null时拒绝修改，再取before、调用SetProcedureNodeParameters并更新有效参数缓存。CheckedParameters为每个JsonElement调用Clone，校验参数错误后交给文档编辑；Document getter又调用Copy，Copy重新分配节点参数字典并Clone各JSON值。因此正常全流程运行中不能把此Apply描述为允许实时修改本轮参数。',
 'Dictionary<string,JsonElement>；Clone隔离JSON值的文档存储，字典替换与历史编辑也有自己的文档副本。它不代表任意插件状态、服务、图像对象的深复制。',
 '编辑文档持有已提交参数；配置窗口上下文的Node来自打开时的CurrentProcedure副本。',
 'JSON副本没有显式像素释放协议；文档/历史生命周期与运行scope分开，不能用参数Clone解释图像buffer退役。',
 [refs('VisionEditor/ToolConfigurationHost.cs','OpenToolConfiguration.Apply','   void Apply(Dictionary<string,JsonElement> values)',10),doc('SetProcedureNodeParameters','    public void SetProcedureNodeParameters(',4),doc('CheckedParameters','    Dictionary<string,JsonElement> CheckedParameters(',9),doc('Document','    public SolutionDocument Document => Copy(document);',4),doc('Copy','    static SolutionDocument Copy(',1)],
 ['此门只证明所选常规配置Apply路径；不能扩称所有API、插件内部状态和配置自执行期间都具有同一禁改门。']);
 add('CP02',['input','processing'],'主流程启动与子流程调用','本次请求的JSON配置快照',
 'MainWindow.RunCore先在调用线程用doc.Document构建SolutionWorkspace，再将workspace交Task.Run。SolutionExecutor.RunAsync再复制全部过程/节点参数，内部Execute和Group循环从这个document查子流程；GraphExecutor入口再复制目标图的参数。编辑文档后来替换JSON字段，不会替换已经捕获的本轮参数字典。ContinuousRun每次重新调用RunCore，故各轮具有各自的配置获取点；这不解除CP01的UI禁改门。',
 '编辑文档副本→SolutionWorkspace.Document→SolutionExecutor局部document→GraphExecutor局部p；每层显式ToDictionary+JsonElement.Clone，而Resources仅数组复制。',
 '当前请求及子调用持有局部快照；持续运行没有一次捕获后永久复用同一份方案JSON。',
 '局部快照随异步方法与其子调用生命周期保留；不等于Scope.State或输入图像也被复制或在返回时释放。',
 [refs('VisionEditor/WorkflowExecution.cs','RunCore','  var workspace=new SolutionWorkspace(doc.Document,doc.BaseDirectory);',9),refs('VisionEditor/WorkflowExecution.cs','ContinuousRun',' async Task ContinuousRun(',6),refs('VisionFramework/SolutionExecutor.cs','RunAsync.snapshot','        var document=workspace.Document with{',9),graph('RunAsync.snapshot','  var p=procedure with{',4)],
 ['插件还可读取scope服务、运行状态、延迟输入读取器；这里只闭合JSON配置隔离，不能宣称整个执行环境不可变。','未执行线程竞争探针；不把静态复制链称为所有并发插件行为的运行证明。']);
 add('CP03',['input','processing'],'节点开始前解析默认值和订阅','ModuleExecutionContext.Parameters与Inputs',
 '每个节点先从已复制n.Parameters建立新的字典，补Clone后的默认JSON值，再在执行到此节点时通过workflow.Resolve解析订阅；参数订阅经WorkflowSubscriptions.Parameter检查标量种类并SerializeToElement，覆盖该节点参数。普通连线则直接把上游PortValue赋入nodeInputs。ExecuteAsync收到两套ReadOnlyDictionary包装；ModuleExecutionContext构造器直接持有这两套只读接口。只读包装没有复制PortValue中图像/对象的含义。',
 '参数种类仅Number/Integer/Boolean/Text；图像是PortValue输入，不是把一张图像深克隆进JSON参数。节点字典ToDictionary本身是浅复制，JSON数据隔离来自前述Clone和绑定序列化。',
 '配置种子在请求开始冻结，但订阅所得值在各节点执行时解析，不能称全图所有有效参数同时于启动时冻结。',
 '参数只读视图不授予图像所有权；模块Dispose与参数缓存退役是不同步骤。',
 [graph('RunAsync.inputs-and-parameters','    nodeInputs[c.ToPort]=v;',12),graph('RunAsync.ExecuteAsync','   try{if(n.ModuleId==WorkflowCalls.ModuleId)',3),refs('VisionFramework/WorkflowSubscriptions.cs','Parameter',' public static JsonElement Parameter(',9),refs('VisionFramework/Contracts.cs','ParameterKind','public enum ParameterKind {',2),refs('VisionFramework/Contracts.cs','ModuleExecutionContext.constructor','    public ModuleExecutionContext(',5)],
 ['DataKinds.Coerce及插件执行可有自己的转换/复制规则；仅凭框架字典不能推定任意具体图像在工具内零复制或深复制。','下游动态读取服务与原生参数setter认可状态不在这条托管参数传递事实内。']);
 add('CP04',['input','processing','results'],'工具配置自执行RunConfiguredTool','本次配置参数副本与独立节点请求',
 '配置自执行在调度前取得doc.Document中的procedure/node，Clone传入values；已解析参数订阅和默认值后建立isolated节点，再交GraphExecutor。ParameterSeed显式返回这次捕获node.Parameters，applied则是传入编辑值加本次绑定解析后的parameters，二者不可混称同一个快照。上游available重新建字典但里面仍为NodeRunRecord/PortValue引用；图像没有随JSON参数Clone自动复制。',
 '配置种子、传入参数、绑定后参数、上游记录视图四种对象分别持有；自执行也走GraphExecutor的JSON复制/校验与scope租约。',
 'CanPublish及结束条件核执行所有者、配置存活、方案/scope/节点身份，再缓存record；这些是发布门，不是将所有运行数据倒写文档。',
 '配置关闭可抑制发布，但此片段未以关闭配置窗口建立专属取消源；ownerScope.Stopping才传入执行请求。',
 [refs('VisionEditor/ToolExecutionScope.cs','RunConfiguredTool.capture',' async Task<NodeRunRecord> RunConfiguredTool(',18),refs('VisionEditor/ToolExecutionScope.cs','RunConfiguredTool.resolve','   var boundParameters=new HashSet<string>(StringComparer.Ordinal);',9),refs('VisionEditor/ToolExecutionScope.cs','RunConfiguredTool.dispatch','   var isolated=new ProcedureDocument(',13)],
 ['不能把全流程activeRun门移植成配置自执行期间全部编辑必被拒绝；各配置操作门不同。','此处记录复用不是图像生产者生命周期或插件缓存释放的完整证明。']);
 add('CP05',['processing','results','retirement'],'进入ExecuteAsync前的有效参数','scope按过程/模块/节点保存Seed和Values',
 'GraphExecutor在ExecuteAsync之前调用RememberExecutionParameters；该方法分别Clone seed与applied，所以缓存不依赖模块执行结束才写入，失败/取消也可能留下本次入参。ReadEffectiveParameters先Clone缓存Values，再把当前配置中与Seed的GetRawText不同的字段覆盖进去；RememberConfigurationParameters又可更新已变字段。因此“有效参数”是供编辑器继续配置的托管视图，不能直接标成上次成功算法实际使用的精确历史记录或原生setter确认。',
 'root.effectiveParameters以(Procedure,Module,Node)为键，每项有Gate、Seed字典、Values字典；读取返回新的JSON副本。',
 '正常单次运行返回、单模块runtime Dispose不清除此scope级缓存；后续执行覆盖它，配置读可叠加当前值。',
 'RemoveNode与RetainProcedureNodes移除相应条目；scope Dispose先closing并取消，等待已Enter租约退出才Release，Release清空effectiveParameters。这是托管引用退役，不证明GC时刻或原生像素释放。',
 [graph('RunAsync.RememberExecutionParameters-before-ExecuteAsync','   try{if(n.ModuleId==WorkflowCalls.ModuleId)',2),refs('VisionFramework/ModuleExecutionScope.Parameters.cs','EffectiveParameters and copy/overlay',' sealed class EffectiveParameters',7),refs('VisionFramework/ModuleExecutionScope.cs','RemoveNode and scope retirement',' public void RemoveNode(',9),refs('VisionEditor/ToolExecutionScope.cs','ConnectToolEditorServices.ReadEffectiveParameters','  return context with{ReadEffectiveParameters=',1)],
 ['没有逐工具核其如何消费每个参数、是否忽略字段、何时复制到native；这些不能从托管入参缓存替代取证。','删除节点与旧在飞请求的全部竞争仍需具体调用时序验证，缓存删除不等于全局排空。']);
 add('CP06',['input','results','retirement'],'用户保存方案','插件持久化投影→JSON方案文件→编辑器提交',
 'MainWindow将doc.PrepareSave接到plugins.PrepareSave(document,baseDirectory,executionScope)。GraphEditorDocument.Save向PrepareSave传入Copy(document)，插件注册表只对IToolAssetPersistence参与者调用其PrepareSave(node,catalog,procedureScope)，回收这些插件前缀资源并采用返回Node/Resources。随后重基路径、SolutionStore验证和序列化方案，经同目录临时文件Flush(true)+Move覆盖成功后编辑器才提交rebased文档。框架没有把scope有效参数或运行结果字典统一自动拷回所有节点。',
 '被保存的是SolutionDocument及插件显式返回的ToolAssetSnapshot；不是整个运行scope、图像对象或原生指针堆的序列化。',
 '参与者可通过传入scope选择保留的工具状态；实际保存哪些有效参数/资产取决于各工具PrepareSave实现。',
 '临时方案文件在finally中清理；写出JSON不会主动清空scope有效参数、运行缓存或释放其图像。',
 [refs('VisionEditor/MainWindow.xaml.cs','MainWindow.PrepareSave wiring','PrepareSave=(document,baseDirectory)=>plugins.PrepareSave(',1),doc('Save','    public void Save(string path) {',7),refs('VisionPluginContracts/Contracts.cs','ToolPluginRegistry.PrepareSave',' public SolutionDocument PrepareSave(SolutionDocument document,string baseDirectory,ModuleExecutionScope? scope)',7),refs('VisionFramework/SolutionStore.cs','Save','    public static void Save(string path,SolutionDocument solution)',9)],
 ['本片段不展开每个IToolAssetPersistence参与者；不能宣称所有最后运行参数、缓存图像、SDK内部状态都能保存和重开恢复。','插件PrepareSave在最终方案文件提交前可能有自身资产写入副作用；此处只证明方案文件提交顺序，不宣称全资产跨文件事务。']);
 const sourceFiles=[...new Map(facts.flatMap(f=>f.refs).map(r=>[r.path,{path:r.path,sha256:r.sha256}])).values()];
 const provenance={schemaVersion:1,scope:'Current main-tree parameter edit, request snapshots, node binding, self-run, effective cache and save projection; static source evidence only.',sourceFiles,factCount:facts.length,referenceCount:facts.reduce((n,f)=>n+f.refs.length,0),productModified:false,otherWorktreeRead:false,guiStarted:false,runtimeExecuted:false,nativeEquivalent:false,implementationProgress:'41/96 = 42.71%; analysis adds no implementation units'};
 fs.mkdirSync(out,{recursive:true});fs.writeFileSync(path.join(out,'current-parameter-transfer.review.json'),JSON.stringify({provenance,facts,unknowns:['托管JSON参数副本不等于所有图像/服务/插件状态深复制。','每工具native setter确认、持久化投影和动态服务行为需按工具继续证实。']},null,2)+'\n');
 return {facts,unknowns:['运行配置与逐节点绑定取值时点分开；有效参数缓存与成功运行历史、持久化输出分开。','具体插件native参数认可、图像复制和资源持有仍以各工具证据为准。'],provenance};
};
if(require.main===module){const result=module.exports();console.log(JSON.stringify({facts:result.facts.length,references:result.provenance.referenceCount,sourceFiles:result.provenance.sourceFiles.length}));}
