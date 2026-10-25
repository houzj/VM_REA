const fs=require('fs'),path=require('path'),{root,source}=require('./evidence-view.cjs');
module.exports=function(){
 const E='src/VisionEditor/',F='src/VisionFramework/',W=E+'WorkflowExecution.cs',S=E+'ToolExecutionScope.cs',T=E+'ToolEditorHost.cs',M=E+'MainWindow.xaml.cs',facts=[];
 const ref=(p,a,n=1)=>source(p,a,a,n),add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'independent-implementation',stages:['results','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('CR01','新一轮开始 / 停止 / 抛取消异常','隐藏旧结果视图与保存旧会话是两个动作',
 'BeginWorkflowResults增加过程执行计数，仅在count>1、workStatus非0且清旧结果偏好开启时，标记该过程及所调用子过程为cleared并清当前results视图；它不删除workflowSessions。RunCore返回session才StoreWorkflowCallResults覆盖对应会话并移除cleared标记。若Task.Run被取消或执行抛异常而没有返回session，catch只改状态文字，旧session仍在；偏好开启时可处于隐藏状态，未开启时也未由这个catch主动清除。工具栏停止只Cancel activeRun，FinishWorkflowRun清运行标记并Dispose该取消源，不清会话或插件scope状态。',
 'results是可见NodeRunRecord索引，workflowSessions是完整ExecutionSessionResult；隐藏索引不等于移除图像持有者。',
 '旧session可继续保持Nodes、WorkflowOutputs、WorkflowDisplays、LocalVariables和Calls中的PortValue引用；不能将空白显示当零像素缓存。',
 '停止请求不等于清旧结果或FIFO；本轮没有新session时不会自动用空session覆盖旧session。',
 [ref(E+'FrameworkExecutionPolicy.cs',' void BeginWorkflowResults(',17),ref(E+'FrameworkExecutionPreferences.cs','    public bool ShouldClearAtBegin(',2),ref(W,'   var session=await Task.Run(',3),ref(W,'  catch(OperationCanceledException){if(IsCurrentExecutionOwner(owner))',3),ref(W,' void StoreWorkflowCallResults('),ref(W,' void FinishWorkflowRun(',4),ref(M,'stopToolButton=Tool('),ref(F+'Contracts.cs','public sealed record ExecutionSessionResult(')],
 ['此事实仅区分返回session与抛异常两条路径，不把所有取消都判成保留旧会话；返回Cancelled session的覆盖路径见CR02。']);
 add('CR02','GraphExecutor本轮取消 / 失败 / 跳过','本轮混合结果可替换上轮会话；旧插件状态另留',
 'GraphExecutor每轮新建records，不拿上一轮NodeRunRecord补失败节点。节点开始前发现取消会Record(Cancelled)且输出默认Empty；前驱失败或不可用会Record(Skipped)且输出Empty。单节点执行后取消/超时/异常与失败输出过滤按TD04生成本轮记录，已经成功完成的前序节点记录不因此统一清空。最终把全部本轮记录组成session，状态可为Cancelled/Failed/TimedOut。RunCore只检查owner、未以session成功作为发布前提，所以正常返回的取消/失败session也可替换workflowSessions，包含本轮早先成功节点图像与后续空输出。',
 '取消后保留的可能是本轮成功前缀，不是自动合并上一轮成功图像；失败记录与成功记录的图像持有不同。',
 '覆盖旧会话只移除该字典入口的旧引用，不清插件缓存、配置结果、调试记录或独立视图；新session本身仍可保留本轮成功图像。',
 '清输出字典、返回取消状态、取消源Dispose均没有逐帧图像Dispose语义。',
 [ref(F+'GraphExecutor.cs','  var calls=new Dictionary<string,ExecutionSessionResult>();'),ref(F+'GraphExecutor.cs','   void Record(NodeRunStatus s,',3),ref(F+'GraphExecutor.cs','   if(executionEdges.Where('),ref(F+'GraphExecutor.cs','   if(cancellationToken.IsCancellationRequested){Record(NodeRunStatus.Cancelled,code:'),ref(F+'GraphExecutor.cs','  var all=validation.ExecutionOrder.Select(',2),ref(W,'   WorkflowCallbacks.PublishModuleResult(',13)],
 ['Task.Run调度前取消或外层异常可能不产生session，需与CR01分别判断；未执行竞争、GC或内存峰值探针。']);
 add('CR03','GraphEditorDocument.DeleteNodes → Changed','scope节点状态移除不等于裁剪旧session/配置记录',
 'DeleteNodes修改节点、连线与配置引用，Edit提交文档后发Changed；主窗Changed调用ValidateToolConfigurationOwner、RetainExecutionNodes和刷新。RetainExecutionNodes只将现存(过程,模块,节点)集合交scope.RetainProcedureNodes并裁剪pointAliasPresentation。scope移除对应参数缓存与state，state若IDisposable才Dispose。这个删除链没有按被删节点裁剪workflowSessions.Nodes、configurationResults或toolDebugRecords；这些容器先前保存的记录不能因文档节点消失而推定已释放。',
 '编辑文档的节点/JSON历史与运行NodeRunRecord在不同容器；删除图结构不自动改写历史运行session。',
 'configurationResults还可保存已删节点的旧记录；新配置请求available从旧session与configurationResults组装引用视图。具体能否再被有效订阅消费仍受当前文档解析和验证限制。',
 'scope移除仅退役该层state持有；显示不再列出节点不等于所有结果载荷不可达。',
 [ref('src/VisionEditor.Core/GraphEditorDocument.cs','    public void DeleteNodes(',4),ref('src/VisionEditor.Core/GraphEditorDocument.cs','        SolutionStore.ValidateStructure(next); RecordEditHistory();'),ref(M,'doc.Changed+=()=>'),ref(S,' void RetainExecutionNodes()'),ref(F+'ModuleExecutionScope.cs',' public void RemoveNode('),ref(F+'ModuleExecutionScope.cs',' void RetainProcedureNodes(IEnumerable<(string Procedure,string Module,string Node)> current,string? only)'),ref(S,'  var available=(workflowSessions.TryGetValue(',2)],
 ['只证明所选删除/Changed链没有这些裁剪，不宣称任意其它用户动作永远不会清记录；结果字典的最终可达时刻未测。']);
 add('CR04','ImageSource状态缓存 / SDK待提交帧 / ImageBuffer FIFO','scope退役、Reset、像素格式变化分别影响哪些持有者',
 'ImageSource.State除Cache还持PendingFrame及StitchBuffer；其Dispose仅Dispose Folder并置null，不主动Clear Cache、清PendingFrame或StitchBuffer。SDK提交以Interlocked.Exchange替换PendingFrame，执行SDK模式再Exchange为null取走，不克隆。文件模式仅像素格式变化时Clear Cache，路径命中复用同一图像；缓存容量参数只限制新准入，不在此段按减少后的容量主动裁剪。ImageBuffer.State不是IDisposable，FIFO淘汰/Reset及scope移除均只是移除特定持有，旧结果仍可指向已出队图像（复制/发布见TD03）。主窗InvalidateSourceResults清results、调用fallback Reset并移除scope节点，却未在此方法清workflowSessions或configurationResults。',
 '源Cache、PendingFrame、拼接数组、FIFO和NodeRunRecord是不同持有者；State.Dispose并不表示它的每个字段被清空。',
 '即使state从scope字典移除，正在运行的局部state引用或旧结果仍可能继续持有像素；停止不触发这里的Reset。',
 '是否由GC回收取决于其它引用；本切片只报告显式字段/容器变化，不宣称旧字节立刻归还系统。',
 [ref('src/VisionPlugins.ImageSource/ImageSourcePlugin.cs',' sealed class State:IDisposable',18),ref('src/VisionPlugins.ImageSource/ImageSourcePlugin.cs',' public void SubmitImage('),ref('src/VisionPlugins.ImageSource/ImageSourcePlugin.cs',' public void Reset(ModuleExecutionScope'),ref('src/VisionPlugins.ImageSource/ImageSourcePlugin.cs',' public void Reset(string nodeId)',3),ref('src/VisionPlugins.ImageSource/ImageSourcePlugin.cs','    if(state.PixelFormat!=',13),ref('src/VisionPlugins.ImageSource/ImageSourcePlugin.cs','     image=Interlocked.Exchange(ref state.PendingFrame,null)!;',2),ref('src/VisionPlugins.ImageSource/ImageBufferPlugin.cs',' sealed class State {',4),ref(M,' void InvalidateSourceResults(')],
 ['无scope fallback状态的外部调用/所有Reset时序未穷尽；已知的托管持有关系不能替代native相机帧租用与释放证据。']);
 add('CR05','工具调试 / 普通配置自执行 / 关闭编辑器','停止调度与LastResult、Task结果、宿主缓存分别退役',
 '当前实际类型是ToolDebugSession和ExecutionSessionResult，并未发现名为ToolExecutionSession/RunRecord的同一实体。ToolDebugSession.active保存Start返回的Task，完成finally仅清取消源/更新状态，不把active置null；若为RunOnce，该完成Task可继续持有NodeRunRecord直到active被下次Start替换或session不再可达。StopAsync取消并等待active，不清已保存结果。普通配置调度ToolConfigurationExecution.Dispatch开始把LastResult=null，完成再置结果；StopAsync只取消轮询并等loop，不清LastResult；DisposeAsync再等待inFlight也不清它。宿主RunConfiguredTool按owner/配置存活/节点身份准入后缓存record，不限成功状态；关闭配置可阻止晚发布，但CloseToolConfiguration没有清configurationResults。独立调试窗口的关闭另等待session.DisposeAsync后移除session集合项，不等同清宿主toolDebugRecords。',
 '完成Task<T>、配置执行器LastResult、configurationResults、toolDebugRecords和工具窗口LastResult是独立持有者，不能统一命名成一个“会话结果”。',
 '正常失败record可覆盖配置缓存旧记录；配置失活导致不发布时旧configurationResults可留存。调试Invalidate还只做宿主特定失效标记，不消灭其它session旧引用。',
 '停止、Dispose调度器、从窗口集合移除、清LastResult和清宿主记录是不同动作；不能从窗口关闭推断所有输出图像已释放。',
 [ref(F+'ToolDebugSession.cs',' Task? active;'),ref(F+'ToolDebugSession.cs','   async Task<T> Execute()'),ref(F+'ToolDebugSession.cs',' public async Task StopAsync()'),ref('src/VisionPluginContracts/ToolConfigurationExecution.cs',' async Task Dispatch(){',6),ref('src/VisionPluginContracts/ToolConfigurationExecution.cs',' public async Task StopAsync(){',5),ref(S,'  }catch(Exception ex){record=new(nodeId,',15),ref(E+'ToolConfigurationHost.cs',' void CloseToolConfiguration()'),ref(T,' void BindToolEditorLifetime(',5),ref(T,' void StoreToolDebugResult(',3)],
 ['Task对象是否实际继续可达取决于窗口/context/session持有；此处只指出字段可达链，不声称关闭后必然泄漏。工具窗口内部额外结果/图像字段仍按各工具另核。']);
 add('CR06','切方案 / 关闭主窗','先阻止旧owner发布，scope等待租约；主窗关闭不等于全持有者清零',
 '切方案ReplaceExecutionScope先使执行owner失效、取消当前请求，显式Clear调试记录、configurationResults、workflowSessions、results等，再Dispose旧scope并创建新scope。scope.Dispose先closing并Cancel，只有取消完成且leases==0才Release；Release清state/services/参数字典，再逐个Dispose可释放资源，失败资源额外存failedResources。主窗关闭与切方案不同：DrainWorkflowBeforeClose先取消活动流程并等activeRun换出；独立调试session由OnClosing等停。OnClosed关闭owner/回调队列、关配置并Dispose scope，但该体没有Clear workflowSessions/results/configurationResults或逐个清控件图像。只可说关闭启动退役，不能说所有外部对象已同步归零。',
 'scope的租约控制state/services清理；NodeRunRecord与WPF/工具显示引用不由同一引用计数统一管理。',
 '失败资源被保留用于CleanupFailures诊断；旧在飞方法局部引用、关闭窗口/context、外部消费者与结果Task可另外保持载荷可达。FrameworkRunHistory只存摘要（TD08），不能把它当图像历史释放机制。',
 '新owner阻止旧结果进入新方案，但不是销毁所有旧像素；窗口生命周期、scope清理、查询容器释放和最后图像读者退出需分别说明。',
 [ref(S,' void ReplaceExecutionScope()'),ref(F+'ModuleExecutionScope.cs',' public void Dispose(){',4),ref(F+'ModuleExecutionScope.cs',' void Release(){'),ref(F+'ModuleExecutionScope.cs',' void RecordCleanupFailure('),ref(M,' async void DrainWorkflowBeforeClose(',21),ref(T,' protected override async void OnClosing(',4),ref(W,' protected override void OnClosed(',8)],
 ['没有强制GC、原机内存峰值或最终外部读者测量；不得由静态Dispose路径推断释放的精确时刻、无泄漏或native等价。']);
 const sourceFiles=[...new Map(facts.flatMap(f=>f.refs).map(r=>[r.path,{path:r.path,sha256:r.sha256}])).values()];
 return {facts,unknowns:['本轮只核当前WPF主宿主所选链，旧WinForms EditorForm不是同一路由。','清视图、替换记录、状态缓存移除、窗口关闭、等待运行和像素最后读者是不同退役边。'],provenance:{schemaVersion:1,scope:'Current main-tree result/session/cache retirement on new run, cancellation, node deletion, editor/window closure and solution replacement; static source only.',sourceFiles,factCount:facts.length,referenceCount:facts.reduce((n,f)=>n+f.refs.length,0),runtimeExecuted:false,productModified:false,otherWorktreeRead:false,complete:false}};
};
if(require.main===module){const d=module.exports();fs.writeFileSync(path.join(root,'docs/architecture-recovery/current-result-retirement.review.json'),JSON.stringify(d,null,2)+'\n');console.log(JSON.stringify({facts:d.facts.length,refs:d.provenance.referenceCount,sourceFiles:d.provenance.sourceFiles.length}));}
