'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert');
const {root,source}=require('./evidence-view.cjs');
const scope='src/VisionFramework/ModuleExecutionScope.cs',parameters='src/VisionFramework/ModuleExecutionScope.Parameters.cs',host='src/VisionEditor/ToolExecutionScope.cs';
const ref=(p,member,anchor,count=1)=>source(p,member,anchor,count);
module.exports=function(){
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'independent-implementation',stages:['retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('CSR01','ModuleExecutionScope.RegisterService<T>替换旧owned服务','先撤owned标记，再Dispose；抛错可留下未回滚的旧映射',
 '条件必须同时满足：同T已有previous、新service不是同引用、ownedServices.Remove(previous)返回true且previous实现IDisposable。该条件链先从ownedServices删除旧对象，再调用previous.Dispose；只有其正常返回才写services[typeof(T)]=新service及按owned标记登记新对象。旧Dispose抛异常时方法无catch/finally回滚：services仍映射旧previous，旧owned标记已移除，新service未写入，异常直接向调用者传播，也不调用RecordCleanupFailure。不能把注册失败描述为原状态完全不变。',
 'services以Type为key保留对象引用；ownedServices是按ReferenceEquality比较的跨Type集合。新旧服务引用与该集合标记分开。',
 'Service<T>()仍可能查回旧previous。后续root Release的服务候选来自ownedServices而非services全值，所以若旧对象没有另在states等候选中，它不再因这个服务映射获得自动Dispose重试。另有state/服务别名时不能据这一条断言永不再被Dispose。',
 '这是服务替换入口的异常合同，区别于root Release逐资源catch/记失败后继续；服务注册并未采用同样的失败资源保留策略。',
 [ref(scope,'RegisterService<T> mutation order',' public void RegisterService<T>('),ref(scope,'Service<T> query',' public T? Service<T>('),ref(scope,'Release candidate construction',' void Release(){'),ref(scope,'RecordCleanupFailure retention',' void RecordCleanupFailure(')],
 ['本条是源码静态事实；另列ScopeRetirementProbe验证有限合成Disposable替换异常。真实相机/工具服务、GUI与设备未观测，不直接宣称现有资源会触发。','ownedServices按对象而非逐Type独立计数；所有多接口别名注册/外部所有者的实际组合未穷尽。']);
 add('CSR02','RemoveNode与RetainProcedureNodes批量裁剪','删除记录在Dispose之前完成；首个异常中断剩余裁剪',
 'RemoveNode在root.gate内先删除effectiveParameters键，再从states.Remove取出value；离锁后仅当value实现IDisposable才Dispose，没有catch和RecordCleanupFailure。Dispose抛错时该node状态及参数已从这些字典移除，调用直接失败；重复RemoveNode找不到state就return，不自动重试旧对象。RetainProcedureNodes先批量删除不在keep中的全部参数键，再快照待移state键并foreach RemoveNode；其中一项抛错将中断剩余foreach，后续state仍可留在states，而其参数键可能已在第一阶段删除。',
 '节点状态字典移除、参数字典移除、对象Dispose完成分别发生，不能用一个成功/失败标记代替。',
 '没有被循环走到的state仍由root.states持有，可在之后的裁剪或root关闭进入其它清理；已移除但Dispose失败对象不由本分支自动放入failedResources。若它另被owned服务/外部持有，则还需单独判断。',
 '单节点Remove/批量Retain与root Release有不同异常屏障；root Release的逐项继续保证不能外推到这两个入口。',
 [ref(scope,'RemoveNode removes then disposes',' public void RemoveNode('),ref(scope,'RetainProcedureNodes two-phase pruning',' void RetainProcedureNodes(IEnumerable<(string Procedure,string Module,string Node)> current,string? only)'),ref(host,'WPF RetainExecutionNodes caller',' void RetainExecutionNodes()')],
 ['WPF doc.Changed等上层对传播异常如何显示/恢复需按调用者另追；本切片不宣称文档编辑或整个solution关闭必然失败。','另列合成ScopeRetirementProbe已观测有限顺序Remove/Retain异常；并发与真实资源异常未观测，本条仍按源码关系取证。']);
 add('CSR03','root closing置位与API准入 / 租约','立即拒新状态与租约，不等于所有查询/删除都禁用',
 'root Dispose在gate下先设置closing=true再调用stopping.Cancel；State<T>、RegisterService、Enter与参数Parameters入口均以closing抛ObjectDisposedException。与之不同，Service<T>只锁gate查询，无closing检查：若活跃租约阻止Release清字典，它仍可在closing期间返回旧服务；Release清空后则返回null。ForProcedure只验证procedure字符串，可以继续生成borrowed view；RequestRun/RequestRefresh在closing直接返回。RemoveNode与RetainProcedureNodes没有closing或leases门，故root完整Release等待租约不能被概括成任何节点移除/替换都受同一租约保护。',
 'closing是操作级准入状态；cancellationComplete与leases控制root Release时机。借用view的存在本身不证明其API可继续使用。',
 '在Release前states/services可能仍保留；State即使只是想取已有值也先拒绝，Service查询则未使用同样策略。',
 'Enter取得的Lease仅参与root TryRelease；Lease自身Interlocked.Exchange确保一次Exit。它不替代调用者在RemoveNode等入口的并发协调。',
 [ref(scope,'State<T> closing gate',' public T State<T>(',3),ref(scope,'RegisterService and Service asymmetry',' public void RegisterService<T>(',2),ref(scope,'ForProcedure borrowed view construction',' public ModuleExecutionScope ForProcedure('),ref(scope,'RequestRun and RequestRefresh closing return',' public void RequestRun(',2),ref(scope,'Enter and RemoveNode guards differ',' public IDisposable Enter()',2),ref(scope,'Dispose/TryRelease/Exit',' public void Dispose(){',4),ref(parameters,'Effective parameter closing gate',' EffectiveParameters Parameters(')],
 ['代码未给出所有方法的全域并发安全保证；不将允许调用某查询/Remove的事实解释为对资源在用状态必然安全。','stopping.Token在CancellationTokenSource已Dispose之后的外部访问合同未在这里运行验证，不作应用级行为推断。']);
 add('CSR04','取消或资源Dispose异常后的root Dispose / Lease.Exit','root清理是一次性尝试；failedResources保留不等于排队重试',
 'Dispose设置closing后，stopping.Cancel抛Exception会RecordCleanupFailure(cancel,stopping,error)，finally仍设置cancellationComplete并TryRelease；不是因取消回调异常就跳过该finally。TryRelease要求closing、cancellationComplete、leases==0且!releaseStarted，在真正Release前置releaseStarted=true。Release对各IDisposable的Exception分别记录后继续下一资源，并单独尝试stopping.Dispose；这点现有CR06已有，本事实新增明确重试边界：root重复Dispose因closing立即return，后续Exit也因releaseStarted不再启动第二遍，failedResources只保留对象，所选类没有重试/清空该表的方法。',
 'CleanupFailures getter返回失败记录数组副本；内部failedResources保存真实资源引用。诊断快照与资源持有是两层。',
 '失败对象及其Exception可继续使资源图可达；保留到root仍可达不等于资源已释放，也不等于有后台重试任务。',
 'Release前先清states/有效参数/services/ownedServices，失败不会把原索引恢复。普通单项Dispose抛Exception不会阻断后续项；但分配快照、RecordCleanupFailure自身异常或进程级故障等未被这一逐资源catch覆盖。',
 [ref(scope,'Dispose cancellation catch and finally',' public void Dispose(){'),ref(scope,'TryRelease once-only gate',' bool TryRelease(){'),ref(scope,'Exit can trigger final release',' void Exit(){'),ref(scope,'Release catches each resource and token',' void Release(){'),ref(scope,'Diagnostic snapshot and actual failed object retention',' public IReadOnlyList<ModuleScopeCleanupFailure> CleanupFailures',2),ref(scope,'releaseStarted and closing fields',' bool closing,cancellationComplete,releaseStarted;int leases;'),ref(scope,'Lease exits once',' sealed class Lease(')],
 ['不把源码Exception catch扩大为任何OOM、进程退出或任意RecordCleanupFailure自身失败时也必完整清理。','本条是源码静态事实；另列合成ScopeRetirementProbe验证有限异常与重复Dispose。GC、真实资源/GUI/设备及native释放未观测，不以幂等调用门推成功释放/原版等价。']);
 const sourceFiles=[...new Set(facts.flatMap(f=>f.refs.map(r=>r.path)))].map(p=>{const b=fs.readFileSync(path.resolve(root,p));return{path:p,sha256:crypto.createHash('sha256').update(b).digest('hex'),byteLength:b.length};});
 for(const f of facts){assert(f.refs.length);assert(f.unknowns.length);}
 return{schemaVersion:1,complete:false,facts,unknowns:[{id:'CSRU01',target:'current exceptional cleanup runtime',question:'这是当前源码级异常/准入分支分析；另列ScopeRetirementProbe合成异常观察，真实资源/GUI/设备、并发删除与GC/native最后释放仍未观测。'},{id:'CSRU02',target:'API-specific retirement guarantees',question:'RegisterService替换、RemoveNode/Retain、root Dispose/Release分别有不同异常屏障，不能把root逐项continue和租约门套用到所有入口。'}],provenance:{sourceFiles,coverageDelta:'New exceptional replacement/removal, closing API asymmetry and once-only retry boundaries; general CR06/CG10 cleanup narration is not counted as a new finding.',actualScopeType:'VisionFramework.ModuleExecutionScope',hostType:'VisionEditor.MainWindow partial in ToolExecutionScope.cs',scopeNamesNotResolvedAsProductTypes:['PluginExecutionScope'],runtimeExecuted:false,originalRuntimeExecuted:false,targetCodeExecuted:false,productModified:false,otherWorktreeRead:false,implementationCredit:0}};
};
if(require.main===module){const r=module.exports(),output='docs/architecture-recovery/current-scope-retirement-evidence.json';fs.writeFileSync(path.resolve(root,output),JSON.stringify(r,null,2)+'\n');console.log(JSON.stringify({output,facts:r.facts.length,refs:r.facts.reduce((n,f)=>n+f.refs.length,0),sources:r.provenance.sourceFiles.length}));}
