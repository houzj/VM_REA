const fs=require('fs'),path=require('path'),crypto=require('crypto');
const {root,out,source}=require('./evidence-view.cjs');
const esc=s=>String(s).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('"','&quot;');
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const folder=path.join(out,'global-variable-analysis.resources'),manifestPath=path.join(folder,'manifest.json');
 fs.mkdirSync(folder,{recursive:true});
 const externalRoot='D:/MyWork/SunEyeVision_V2';
 const files=['src/UI/Services/Workflow/WorkflowExecutionManager.cs','src/Workflow/WorkflowRunRequest.cs','src/Workflow/SolutionExecutionDependencyGraph.cs','src/Workflow/WorkflowExecutionDependencyGraph.cs','src/Workflow/WorkflowExecutionEngine.cs','src/Workflow/GlobalVariable.cs'];
 let manifest;
 if(fs.existsSync(manifestPath))manifest=JSON.parse(fs.readFileSync(manifestPath));
 else {manifest={capturedUtc:new Date().toISOString(),program:'SunEyeVision_V2',readOnlyReference:true,files:files.map(relative=>{const originalPath=externalRoot+'/'+relative,b=fs.readFileSync(originalPath),sha256=hash(b),name=sha256.slice(0,16)+'-'+path.basename(relative)+'.source.txt';fs.writeFileSync(path.join(folder,name),b);return {relative,originalPath,sha256,snapshot:'docs/architecture-recovery/global-variable-analysis.resources/'+name};})};fs.writeFileSync(manifestPath,JSON.stringify(manifest,null,2)+'\n');}
 const refs=[];
 function local(id,program,file,member,anchor,count){const r={id,program,...source(file,member,anchor,count)};refs.push(r);return id;}
 function external(id,file,member,anchor,count){const f=manifest.files.find(f=>f.relative===file);if(!f||hash(fs.readFileSync(path.join(root,f.snapshot)))!==f.sha256)throw Error('Frozen source mismatch '+file);local(id,'自研 SunEyeVision_V2（只读源码快照）',f.snapshot,member,anchor,count);Object.assign(refs.at(-1),{originalPath:f.originalPath,evidenceType:'captured-external-project-source',binaryIdentity:'source only; no binary or runtime equivalence claimed'});}
 const vm='analysis/decompiled/VM.Core/VM.Core/';
 local('GV01','原版 VM',vm+'VmSolution.cs','VmSolution.LoadGlobleModules','internal void LoadGlobleModules(',48);
 local('GV02','原版 VM',vm+'VmModule.cs','VmModule.GetSubscribleModules','GetSubscribleModules(bool isOutput)',65);
 local('GV03','原版 VM',vm+'VmSolution.cs','VmSolution.GloalScriptContinusRunEnable','public bool GloalScriptContinusRunEnable',46);
 local('GV04','原版 VM',vm+'VmSolution.cs','VmSolution.Dispose(bool)','public void Dispose(bool isTakeOver)',22);
 local('GV05','REA 当前独立实现','src/VisionEditor/MainWindow.xaml.cs','MainWindow global variable toolbar','全局变量（变量模型尚未接入）',4);
 local('GV06','REA 当前独立实现','src/VisionFramework/WorkflowRunContext.cs','WorkflowRunContext','internal sealed class WorkflowRunContext',43);
 local('GV07','REA 当前独立实现','src/VisionFramework/SolutionExecutor.cs','SolutionExecutor.CallAsync','CallAsync(',20);
 local('GV08','REA 当前独立实现','src/VisionEditor/WorkflowExecution.cs','StartRunCore / RunCore / ContinuousRun','async Task StartRunCore(',85);
 const manager=files[0];
 external('GV09',manager,'WorkflowExecutionManager.RunSolutionAsync','public async Task RunSolutionAsync(',82);
 external('GV10',manager,'BuildWorkflowPredecessors / ApplyProducedGlobalValues','private static Dictionary<string, HashSet<string>> BuildWorkflowPredecessors(',76);
 external('GV11',manager,'StartAllContinuousRun / StopAllContinuousRun / TryAcquireExecution','public void StartAllContinuousRun(',64);
 external('GV12',manager,'ExecuteWorkflowInternal','private async Task<ExecutionResult> ExecuteWorkflowInternal(',172);
 external('GV13',files[1],'WorkflowRunRequest','public sealed class WorkflowRunRequest',52);
 external('GV14',files[2],'SolutionExecutionDependencyGraph global dependencies','foreach (var variable in solution.GlobalVariables ??',90);
 external('GV15',files[3],'WorkflowExecutionDependencyGraph local dependencies','if (sourceId is "__GlobalVars__"',36);
 external('GV16',files[5],'GlobalVariable.Value','public object? Value',19);
 external('GV17',files[5],'GlobalVariableManager','public class GlobalVariableManager',101);
 const drift=manifest.files.map(f=>({originalPath:f.originalPath,changed:fs.existsSync(f.originalPath)?hash(fs.readFileSync(f.originalPath))!==f.sha256:null}));
 const sections=[
 ['先用一个例子看清难点','流程 A 测量出宽度 X，流程 B 根据 X 判断。关键不是“B 能否找到 X”，而是“B 找到的是 A 哪一轮的 X”。如果 A 本轮失败，B 继续读上轮的 X，类型和界面都可能正常，业务结果却已错配。以下例子用于解释，不是原版运行实测。'],
 ['谁持有：定义、运行值、结果应当分开','变量定义保存 Id、类型、初值以及生产者／消费者关系；本轮运行值保存本次实际取到的内容；节点结果仍由执行结果对象保留。SunEye 的方案批次在 RunSolutionAsync 中建立 runtimeGlobals，ApplyProducedGlobalValues 刻意不修改 Solution.GlobalVariables.Value。WorkflowRunRequest 再复制字典容器交给本轮执行。容器复制不等于对象、数组或图像像素全部深复制。另外 GlobalVariable.Value 已有 ValueGate 锁；GlobalVariableManager 也有按名称增删读写。它们不单独构成跨流程运行值发布和批次一致性协议，不能说自研项目没有全局变量管理器。证据 GV09、GV10、GV13、GV16、GV17。'],
 ['流程内：先让生产节点执行，再读取本轮结果','REA 的 WorkflowRunContext 持有本轮 records 和局部变量表。Update 先删除该变量旧条目，再尝试取已运行生产节点的输出；失败不保留该条目。跨流程 CallAsync 明确把调用方 WorkflowOutputs 映射为被调用方输入。SunEye 的局部依赖图会为同流程内的全局变量生产者建立节点前置关系；这证明执行顺序约束，不单独证明所有参数解析器会用当前节点输出刷新变量。后者仍需逐入口验证。证据 GV06、GV07、GV15。'],
 ['跨流程方案批次：A 完成 → 发布成功输出 → B 再取值','SunEye RunSolutionAsync 编译方案依赖，从无生产绑定且绑定有效的变量初值建立 runtimeGlobals；BuildWorkflowPredecessors 只收集所选流程之间的全局变量依赖。循环每次选一个就绪流程并 await，再发布它的生产结果。发布时先 Remove 旧值，只有对应节点成功、结果 IsSuccess 且输出解析成功才重新赋值。整流程失败仍可能发布其中成功生产节点的值，后继流程也可继续运行独立节点；不能描述为“一处失败全部停止”。证据 GV09、GV10、GV14。'],
 ['全部连续运行：不能直接套用上面的批次语义','SunEye StartAllContinuousRun 逐个调用 StartContinuousRun；其默认执行入口经 ExecuteWorkflowInternal 使用 _globalVariables，而不是自动共享 RunSolutionAsync 的 runtimeGlobals 和发布顺序。TryAcquireExecution 按 workflowId 防重复，不足以证明所有工作流全局串行。本次已引用路径尚未建立独立持续流程之间的同批版本读取／发布屏障；这是一项具体契约缺口，不能据此推断全项目不存在其它更新路径。证据 GV11、GV12。'],
 ['后台和停止：取消请求、执行退出、禁止写回是三个时刻','REA RunCore 在 Task.Run 中执行，返回时检查 IsCurrentExecutionOwner 后才呈现；连续入口逐轮 await，Stop 请求取消。它证明界面后台执行及所有者准入，不代表独立后台服务或任意 native 调用立即可中断。SunEye StopAllContinuousRun 的引用方法发出 Cancel；单凭此方法不能证明全部任务已经退出、全部图像释放或所有延迟结果被拒绝。原版还存在 GlobalScriptStartContinueRun / GlobalScriptStopRun 与 Dispose(isTakeOver)→DestoryHandle 分支，接管与托管后台线程不是同一概念。证据 GV03、GV04、GV08、GV11。']
 ];
 const policies=[['同批配对','A、B 必须处理同一件产品','以 BatchId / FrameId 等明确关联；B 等待本批生产者完成，缺值或失败明确不可用。'],['最新有效值','监控温度、运行设定等允许读最新值','读带版本、来源、时间和有效状态的快照；明确超时以及是否允许沿用旧值。'],['每条都要处理','触发、检测事件、逐帧任务','使用有界队列，明确顺序、背压与丢弃策略；单个“最新值”槽无法保存所有事件。']];
 const proposal=[['变量身份','用稳定 VariableId 寻址；名称用于显示。定义中指定类型、单一生产者或明确的多写者仲裁。'],['发布记录','建议保存 SolutionEpoch、RunId、BatchId、Version、Producer、Status、Payload；版本只是方案建议，未声称现代码已有。'],['一次读取','消费者按所选策略捕获一致快照；同批依赖在调度层建立，不能只给共享字典加锁。'],['一次提交','先在私有结果中构造完整值，再短时校验并发布；执行算法、等待相机或 await 时不持共享提交锁。多字段必须原子快照或事务提交。'],['取消与迟到','停止／切方案先撤销旧发布资格，再取消并等待清理；发布时校验 epoch、run 和取消状态，拒绝旧任务迟到结果。'],['缓存和释放','明确运行表、结果记录、消费者快照和显示各自的持有关系。图像选择不可变共享句柄＋租约或明确复制；替换字典条目仅移除一个引用，最后读者结束才能回收底层资源。跨轮缓存另设容量和淘汰规则。']];
 const unknowns=['原版 SDK 原生变量读写的原子性、可见时刻、跨流程并行一致性和取消后发布规则，不能由托管订阅候选列表推导。','SunEye 普通 __GlobalVars__ 参数的请求快照→合成节点结果→参数解析链已补证；具体工具同轮值探针，以及专用接收变量各入口仍按 GEX10、GEX11、GU05 分列。','SunEye 独立连续流程的跨轮版本、配对、超时、循环依赖及停止后迟到发布规则，当前引用路径不足以确认。','字典中可变对象／图像的全部持有者、深复制和最终释放，必须按具体载荷补证，不能以 IReadOnlyDictionary 名称证明。'];
 const table=(headers,rows)=>'<div class="dd-table-scroll"><table><thead><tr>'+headers.map(v=>'<th>'+esc(v)+'</th>').join('')+'</tr></thead><tbody>'+rows.map(r=>'<tr>'+r.map(v=>'<td>'+esc(v)+'</td>').join('')+'</tr>').join('')+'</tbody></table></div>';
 const execution=require('./global-variable-execution.cjs')();
 const html='<section id="global-variable-analysis"><h3>全局变量与后台执行：谁持有、何时可见、停止后谁还能写</h3><p class="dd-gap">本节分别说明原版 VM、REA 当前恢复实现、自研 SunEyeVision_V2 的只读源码快照。下面的设计建议不是已实施功能，也不是原版行为。无产品代码改动，无原版运行验证。</p><p class="dd-mini-chain">变量定义 → 建立生产／消费依赖 → 捕获本次输入 → 执行生产节点 → 校验并发布运行值 → 消费者读取 → 退出与回收</p>'+sections.map(([title,text])=>'<article><h4>'+esc(title)+'</h4><p>'+esc(text)+'</p></article>').join('')+'<h4>三份程序的边界，不能混用</h4>'+table(['程序','已确认','不能据此声称'],[['原版 VM','SDK 全局模块由 VmSolution.LoadGlobleModules 装载；订阅候选路径包含全局模块 ID 13000（GV01、GV02）。','不能据此声称统一托管共享字典或跨轮版本协议；局部原生mutex与实际更新点见上方本轮补证。'],['REA 当前恢复','全局变量工具栏明确标记“变量模型尚未接入”；已有本轮局部变量和显式跨流程调用（GV05–GV08）。','局部变量与 CallAsync 不等于全局变量完整机制。'],['自研 SunEyeVision_V2','方案批次已有依赖调度与本轮运行值发布；连续运行另有入口（GV09–GV15）。','不能把批次顺序保证自动推广到独立持续流程。']])+'<h4>建议先明确每个变量采用哪种传递语义</h4><p>以下为自研方案建议，尚未实现或验收。</p>'+table(['语义','适合的情形','应约定的行为'],policies)+'<h4>建议的最小运行契约</h4>'+table(['责任','设计建议'],proposal)+'<h4>需要验证的操作</h4><p>A→B 同批成功；A 失败不得静默读上轮；两流程同时写同一变量；消费者超时；停止后旧生产者迟到；切方案后旧结果返回；图像被两个消费者持有后替换；生产消费速率不匹配时队列容量。此列表是待做的验收案例，不是已通过测试。</p><h4>明确保留的未知</h4><ul>'+unknowns.map(s=>'<li>'+esc(s)+'</li>').join('')+'</ul><details><summary>逐项源码证据：成员、原始路径、固定快照与 SHA256（'+refs.length+' 项）</summary><p>原版为本地反编译文本，本节未重新建立二进制与源码的逐方法映射；SunEye 为捕获时的源码，不声称是其最新编译产物。</p>'+refs.map(r=>'<details data-global-variable-proof="'+r.id+'"><summary>'+esc(r.id+' · '+r.program+' · '+r.member)+'</summary><p>'+esc((r.originalPath||r.path)+':'+r.start+'–'+r.end)+'</p><p>'+esc('快照／引用 '+r.path+' · SHA256 '+r.sha256)+'</p><pre>'+esc(r.excerpt)+'</pre></details>').join('')+'</details></section>';
 const model={scope:'finite documentation analysis; no implementation or runtime parity claim',implementationProgress:require('./current-implementation-progress.cjs')(),refs,sections,policies,proposal,unknowns,externalSnapshot:manifest,externalDriftAdvisory:drift};
 fs.writeFileSync(path.join(out,'global-variable-analysis.json'),JSON.stringify(model,null,2)+'\n');return {model,html:html.replace('<p class="dd-mini-chain">变量定义',()=>execution.html+'<details><summary>基础范围说明与设计参考（设计建议仍未实施）</summary><p class="dd-mini-chain">变量定义').replace('</section>','</details></section>')};
};
