'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
module.exports=function(){
 const F='src/VisionFramework/',E='src/VisionEditor/',C=F+'Contracts.cs',A=F+'PortArrays.cs',W=F+'WorkflowRunContext.cs',S=F+'WorkflowSubscriptions.cs',G=F+'GraphExecutor.cs',facts=[],ref=(p,a,n=1)=>source(p,a,a,n);
 const add=(id,stages,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'independent-implementation',stages,modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('CD01',['transfer','results'],'数值、整数、布尔、文本与NumberArray工厂','PortValue的标量载荷与单对象数值序列',
 'Number检查double有限后包装值，Integer(long)与Boolean(bool)包装值，Text拒绝null但保留传入string引用；string本身不可变，不需要为每次传递重新复制字符。Numbers(IEnumerable<double>)则先ToArray、拒绝非有限项，再用只读集合包装副本，Kind为NumberArray但Cardinality仍采用构造器默认Scalar。Components也ToArray，但它是Components整体载荷，不因内部是列表就自动变Cardinality.Array。只读集合是容器接口限制，复制行为来自明确ToArray，不能把readonly一词当所有权证据。',
 'Number(double)/Integer(long)/Boolean(bool)是装箱数值；Text保存string对象；NumberArray保存独立double[]副本的只读视图。NumberArray+Scalar与Number+Array必须分开。',
 'PortValue本身无全局缓存；同一PortValue可随后同时被输入、变量和运行结果字典持有。',
 '这些托管载荷没有逐值Dispose协议；替换字典项仅撤一个持有者，不保证字符/数组马上回收。',
 [ref(C,'    private PortValue(',1),ref(C,'    public static PortValue Number(double',9),ref(C,'    public T As<T>()')],
 ['不把托管long/double及文本合同视为原SDK int32/float32/UTF8缓冲ABI等价；自定义或原生工具内部是否另复制需逐工具取证。']);
 add('CD02',['transfer','results'],'Cardinality.Array工厂与Element / JoinArray','复制数组容器、标量取项与重新物化集合',
 'PortTypes.Valid只允许Integer/Number/Text/Circle/Point/Fixture使用Array基数；并无PointSet枚举。IntegerArray/NumberArray/TextArray及几何数组工厂均ToArray后包装只读视图；Number数组校验有限，Text数组拒绝null，Point/Circle/Fixture数组逐项调用相应校验。Element对Scalar且index0返回同一PortValue，对Array则从只读列表取值并创建标量PortValue。JoinArray先把同Kind输入Elements展平，再调用对应数组工厂，因此输出是新容器；Text只复制字符串引用，Point2/Component等值结构复制其数值字段，不是深复制任意引用对象。',
 'Point2/CircleFit/Component是readonly record struct；PointArray为Point+Array；Numbers为NumberArray+Scalar，二者ElementCount含义不同。',
 '数组工厂隔离传入枚举容器；取项后包装的标量与原数组可并存。JoinArray临时flat数组与最终数组是额外的物化层。',
 '没有数组池归还或逐元素Dispose；集合离开某条路径不等于所有session/订阅读者已退出。',
 [ref(A,' public static bool Valid('),ref(A,' public static PortValue CircleArray(',10),ref('src/VisionData/ImageGeometry.cs','public readonly record struct Component(',3)],
 ['FixtureValue内部可能持有复合引用，数组ToArray本身不证明它们递归深复制；本条不将PointArray/Components解释为原版pointset二进制布局。']);
 add('CD03',['transfer'],'DataKinds.Coerce / PortTypes.AcceptsInput','不转换时同引用；数值提升时新包装并可能降精度',
 'CanFlow同Kind通常允许原值通过；Coerce(value,DataKind)只有Number→NumberArray会Numbers([double])新分配一个序列，其余可流转类型原样返回PortValue。带PortDescriptor的Coerce先保留支持deferred读的原始类型；否则AcceptInteger且Integer输入时，将long先强转float再包装Number或Numbers。这是显式单精度中间转换，不是无损long→double。GraphExecutor先按基数与种类验证每个输入，再Coerce；常规类型门不接受不同Cardinality，支持deferred的输入另有准入例外。还须注意：AcceptInteger的转换体调用As<long>()，不是逐元素转换；Integer+Array即使通过目标Number+Array的声明类型门，仍会在这里读取long失败，并被Graph记录InputTypeMismatch，不能把声明接受等同运行数组提升已实现。',
 '同类型传递是PortValue身份共享；Number→NumberArray与整型提升会创建新PortValue。NumberArray种类与Array基数不是自动互换的两种拼法。',
 '每节点nodeInputs为新字典，字典里的共享值和提升后的新值要分开跟踪；不存在对全部输入逐项深克隆的通用步骤。',
 'Coerce没有清来源结果或修改原PortValue；新值被丢弃不影响其它读者仍持有旧值。',
 [ref(C,' public static bool CanFlow(DataKind source,DataKind target)',4),ref(A,' public static bool DeferredReadAccepts(',4),ref(A,' public static PortValue IntegerArray('),ref(G,'   try{foreach(var input in nodeInputs.ToArray())')],
 ['这里描述当前显式代码的转换点，不声明原版数值精度或原生getter行为；全部数值极值和调用路径未做运行差分。']);
 add('CD04',['transfer','results'],'Point / Components复合结果','投影叶子是新数值容器，复合父载荷仍原样保留',
 'CompoundOutputFields.Expand先浅复制原Outputs，再用TryAdd追加投影字段，已存在同名字段优先保留。Point标量投影X/Y新Number；Point数组的X/Y用Numbers分别物化为NumberArray+Scalar。Components父载荷是Component只读列表，投影BlobNum/TotalArea标量及Area/中心/矩形等Number+Array叶子，叶子数组逐列新建。因此父Point/Components与各投影列同时可被结果持有；不能称所有子字段只是父数组的零复制切片。',
 '当前Point、PointArray、Components是具体托管几何合同；原版pointset/chunk涉及其自身字段、元素布局和所有权，不与此处同名映射或等同。',
 'Expand创建新字典但复合父PortValue仍共享；各列数组是独立投影快照，可能增加内存持有。',
 '释放父载荷某处引用不会自动清结果中已投影的数值数组，反向清投影也不释放父结果的其它持有者。',
 [ref(F+'CompoundOutputFields.cs','  DataKind.Point=>',3),ref(F+'CompoundOutputFields.cs',' public static IReadOnlyDictionary<string,PortValue> Expand(',5),ref(C,'    public static PortValue Components('),ref(A,' public static PortValue PointArray(')],
 ['不据恢复XML字段名推定native点集ABI；当前Region/Fixture等其它复合投影及全部插件自定义输出仍应独立核查。']);
 add('CD05',['variable','transfer','results'],'WorkflowRunContext每轮变量、参数与节点结果','本轮引用解析、生产者失败移除与会话快照',
 '当前逐轮数据容器名为WorkflowRunContext；未定位到名为WorkflowValueScope的同一路由实体。构造器复制inputs字典但保留每个PortValue，严格核输入Kind/Cardinality；未绑定生产者的局部变量每轮由Initial重新建值。Resolve节点输出只读本轮Success/NoMatch记录；局部变量直接返回variables中的同一PortValue。Update在生产者记录后先Remove对应变量，再尝试从本轮Node读值，失败则不恢复上轮/旧值。Complete为Inputs/Outputs/LocalVariables新建只读字典，为显示创建新单元素列表，但内层PortValue不复制；局部变量从ID键转换成名称键后保存在ExecutionSessionResult。',
 '逐轮variables/arguments/records与跨轮ModuleExecutionScope.State不同；会话快照是容器快照，不是整个载荷堆的深快照。',
 '同一数值数组可被节点结果、流程输出、显示列表和局部变量同时保留；已有会话可继续持有上一轮值，见CR01–06，不等于新一轮变量自动继承旧值。',
 'WorkflowRunContext没有逐值Dispose；session覆盖、编辑文档变化和scope state移除不是同一动作，不推断GC时刻。',
 [ref(W,' public WorkflowRunContext(',8),ref(W,' PortValue Node('),ref(W,' public PortValue Resolve(',13),ref(W,' public void Update('),ref(W,' public ExecutionSessionResult Complete(',8),ref(G,'   void Record(NodeRunStatus s,')],
 ['本事实只证明这个主执行路由；配置自执行/调试可另外使用旧结果作为输入，不能移植成所有入口都只允许本轮生产者。']);
 add('CD06',['variable','transfer'],'WorkflowSubscriptions常量/变量/参数与延迟读取','字节变量仍为文本，JSON复制与UTF8容量读另走',
 'VariableKind把int映为Integer、float映为Number、string与byte都映为Text；NormalizeValue把byte限制为偶数长度十六进制并大写，Initial的byte落到Text(v.Value)，没有在这里转byte[]。NumberArrayConstant先JSON反序列化double[]、检查最多1000000元素且有限，再由Resolve调用Numbers复制一次。参数订阅要求Kind匹配且Scalar，并SerializeToElement生成JSON；Integer参数还checked转int32并按参数范围验证。延迟文本读另取Text元素，截到首个NUL，计算UTF8字节数+1，容量不足返回所需字节数；它返回string，不提供原生借用字节指针。',
 '变量byte十六进制文本、TextArray字符串集合、原版ByteData/native UTF8缓冲是不同表示。当前参数JSON与PortValue载荷也不是同一容器。',
 '常量每次解析可产生新值/新数组；延迟输入读取器持有本次inputs与索引解析器，读取时再选元素，不保证全部读在流程启动时一次冻结。',
 'JSON序列化、UTF8长度计算或NUL截断不释放原文本/数组；无此处统一的native缓冲释放合同。',
 [ref(S,' public static DataKind VariableKind(',2),ref(F+'WorkflowConfiguration.cs',' public static string NormalizeValue(',8),ref(S,' public static IReadOnlyList<double> ReadNumberArrayConstant('),ref(S,' public static PortValue Initial(',8),ref(W,'  WorkflowSourceKind.NumberArrayConstant=>'),ref(F+'WorkflowNumericInputRead.cs',' public ModuleTextInputReadResult? ReadText(',14)],
 ['这里不声称原版byte变量也如此传输；原SDK字节值与字符串指针寿命另列。读取器resolve回调是否捕获跨轮可变对象须按具体入口另核。']);
 add('CD07',['variable','retirement'],'局部变量改型/删除/节点删除','断开文档订阅和来源不等于清历史运行值',
 '局部变量窗口Commit改型时归一化或重置Value，并清该变量的SourceNode/SourcePort；类型变化还移除以此LocalVariable为来源的Subscriptions。DeleteVariable删除变量及其消费者订阅；删除分组类似批量移除。GraphEditorDocument.SetWorkflowConfiguration验证并Snapshot，识别删除/改型ID后交PropagateGroupInterface处理关联。删除节点时WorkflowConfiguration.RemoveNodes移除相应来源引用，并把绑定该节点的变量改为无来源，保留配置Value。所选UI写配置经WriteWorkflowConfiguration在activeRun非空时拒绝；改文档不会原地改写已经捕获的本轮配置或旧session.LocalVariables。',
 '变量声明的Type/Value/SourceNode与运行PortValue分别保存；同名或同ID不意味着只有一个可变存储单元。',
 '未来运行根据新配置重新初始化；旧session/结果字典仍可持有旧类型值。文档Snapshot复制集合容器，非回收运行载荷。',
 '删除消费者订阅只撤文档依赖；不以删变量推定历史数组、调试结果或scope插件状态已释放。',
 [ref(E+'WorkflowLocalVariablesWindow.xaml.cs',' void Commit(Row row)',8),ref(E+'WorkflowLocalVariablesWindow.xaml.cs',' void DeleteVariable('),ref('src/VisionEditor.Core/GraphEditorDocument.cs','    public void SetWorkflowConfiguration(',6),ref(F+'WorkflowConfiguration.cs',' public WorkflowConfiguration RemoveNodes('),ref(E+'WorkflowConfigurationHost.cs',' void WriteWorkflowConfiguration(',5),ref('src/VisionEditor.Core/GroupInterfacePropagation.cs','  Apply(changed);var result=')],
 ['父Group比较操作数等传播规则不能仅由变量窗口推定完整，本条只证明确调用到传播清理；其它程序化编辑入口门及跨方案消费者需另核。']);
 add('CD08',['variable','results','retirement'],'Group跨轮累积','先保留各轮PortValue，再JoinArray复制成最终集合',
 'GroupOutputAccumulator按输出名保存List<PortValue>，仅对Accumulate且Accumulatable支持的Integer/Number/Text/Circle/Point/Fixture接收累积。Add直接list.Add(value)，不在每轮立即复制整个载荷，累计元素数受独立1000000上限。Finish先浅复制last.WorkflowOutputs，再对每个累积输出JoinArray，形成新数组容器；FinishDisplays也仅复制列表容器，保留各帧PortValue。SolutionExecutor每次Group调用新建accumulator，每次Execute产生新子session，只有成功iteration才Add；最终返回last with新的累积输出/显示。NumberArray与Components不在该累积种类白名单，不应假定能按相同算法累积。',
 '累积中间列表、最后一轮session和最终数组可能同时可达；数值/点集合与图像缓存并非相同缓存机制。',
 '累积器属于本次Group调用局部对象，不是全方案全局变量表；scope可保存插件状态，但这里没有把累积列表登记进ModuleExecutionScope.State。',
 'Finish不会清中间列表或Dispose载荷；返回后何时不可达取决于调用栈/session及其它读者，不能从循环结束推出立即回收。',
 [ref(F+'GroupOutputAccumulator.cs',' readonly Dictionary<string,List<PortValue>> accumulated=',2),ref(F+'GroupOutputAccumulator.cs',' public IReadOnlyDictionary<string,IReadOnlyList<PortValue>> FinishDisplays()',2),ref(F+'GroupBreak.cs',' public static bool Accumulatable('),ref(F+'SolutionExecutor.cs','                var loop=GroupLoop.Read(node);',14),ref(A,' public static PortValue JoinArray(')],
 ['1000000元素/10000轮等为当前独立资源门，不当作原版保证；native Group跨轮清理及数值ABI的等价性未由本静态切片验证。']);
 const refs=facts.flatMap(f=>f.refs),sourceFiles=[...new Map(refs.map(r=>[r.path,{path:r.path,sha256:r.sha256}])).values()];for(const r of refs){const b=fs.readFileSync(path.resolve(root,r.path));if(crypto.createHash('sha256').update(b).digest('hex')!==r.sha256||b.toString('utf8').split(/\r?\n/).slice(r.start-1,r.end).join('\n')!==r.excerpt)throw Error('Current nonimage reference changed '+r.path);}
 return{facts,unknowns:['Point/Components与原版pointset/native chunk分别取证；当前相近中文名字不是布局或所有权桥。','只读包装、字典快照、参数JSON、插件scope状态和跨轮session是不同持有层；未做GC/峰值或原版运行等价验证。'],provenance:{schemaVersion:1,scope:'Current main-tree nonimage factories, cardinality, scalar/array promotion, compound leaves, per-run variables, subscriptions, edits and group accumulation; static evidence only.',sourceFiles,factCount:facts.length,referenceCount:refs.length,complete:false,runtimeExecuted:false,productModified:false,otherWorktreeRead:false,implementationCredit:0}};
};
if(require.main===module){const d=module.exports();fs.writeFileSync(path.join(out,'current-nonimage-data-evidence.json'),JSON.stringify(d,null,2)+'\n');console.log(JSON.stringify({facts:d.facts.length,references:d.provenance.referenceCount,sourceFiles:d.provenance.sourceFiles.length}));}
