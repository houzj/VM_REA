const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const manifest=JSON.parse(fs.readFileSync(path.join(out,'image-notification.artifact-identities.json'))),cil=JSON.parse(fs.readFileSync(path.join(out,'image-notification.cil.json'))),artifact=manifest.artifacts[0];if(sha(fs.readFileSync(artifact.path))!==artifact.sha256)throw Error('Notification original identity changed');
 let methods=0,branches=0;for(const t of cil.targets)for(const m of t.methods){if(m.status!=='decoded'||sha(Buffer.from(m.ilBytes,'hex'))!==m.ilSha256)throw Error('Notification CIL body incomplete');const offsets=new Set(m.instructions.map(i=>i.offset));for(const i of m.instructions)if(/BrTarget|Switch/.test(i.operandKind))for(const target of Array.isArray(i.operand)?i.operand:[i.operand]){if(!offsets.has(target))throw Error('Notification branch target');branches++;}methods++;}
 const O='Apps.XmlParser.Object',F='Apps.XmlParser.FeatureBase',B='Apps.XmlParser.ObjectBase';
 const ref=(type,anchor,count)=>({...source('docs/architecture-recovery/image-notification.resources/'+type+'.decompiled.cs',anchor,anchor,count),evidenceType:'original-offline-decompiled-source',artifact:artifact.path,binaryIdentity:'SHA256 '+artifact.sha256+' / MVID '+artifact.mvid,reaEvidence:artifact.reaEvidence});
 function C(type,name){const m=cil.targets.find(t=>t.type===type)?.methods.filter(m=>m.name===name);if(m?.length!==1)throw Error('Notification method ambiguity '+type+' '+name);return{...source('docs/architecture-recovery/image-notification.cil.json',type+'::'+name+' '+m[0].token,'"ilSha256": "'+m[0].ilSha256+'"',1),evidenceType:'original-sha-mvid-bound-cil',artifact:artifact.path,binaryIdentity:'SHA256 '+artifact.sha256+' / MVID '+artifact.mvid,methodToken:m[0].token,reaEvidence:artifact.reaEvidence};}
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,from,to,behavior,representation,cache,release,refs,unknowns,classification:'source-confirmed',stages:['results','display','retirement'],modes:['all']});
 add('ON01','VmIO.ValueChanged → FeatureBase.IO_ValueChanged','按执行计数聚合Feature.ValueStore → Object.FeatureNotifyValueChanged',
 'FeatureBase.SetOutputFilter为每个vmIO安装WeakEventManager处理器，RemoveOutputFilter对应退订。回调在执行计数变化、ValueStore为空或重建标记为true时新建object[]，收到Array时直接存入该数组引用；待VmIO_Notnull_Count等于已收数后，GetValue组值并调用父Object.FeatureNotifyValueChanged，随后ClearValueStore只设重建标记，不立即清旧数组。',
 'VmIO事件值Array → FeatureBase.ValueStore槽位保留同一引用 → 父Object值；不在这里统一复制像素。','执行计数/重建门决定新建聚合容器；清标记和实际换数组是不同时间点。','弱事件退订不清所有数据；ClearValueStore未逐元素Dispose，旧ValueStore引用要到后续替换或对象退役再减少。',
 [ref(F,'public void SetOutputFilter()',16),ref(F,'private void IO_ValueChanged(',45),ref(F,'internal void ClearValueStore()',4),C(F,'SetOutputFilter'),C(F,'RemoveOutputFilter'),C(F,'IO_ValueChanged'),C(F,'ClearValueStore')],
 ['GetValue的全部数据类型/索引/数组拼接语义与多个IO并发回调的原子性未由所选方法证明；不能从WeakEventManager推断像素已释放。']);
 add('ON02','Object.FeatureNotifyValueChanged的特征到齐门','FillImageValue → 带计数的ImageData载荷',
 '父Object在计数变化、首次计数或已收数为0时清非固定Feature槽并重置计数，然后保留本次Feature值引用。只有已收Feature数等于ValueStore.Length才NotifyValueChanged。图像填充从Image特征取chunk：共享模式保留地址名，非共享模式GetData取数组，赋给新ImageData的ImageBuffer/MemoryAddress并写宽高、格式、NeedCheckExecuteCount=true及ExecuteCount；输入已是ImageData时直接返回原对象。',
 '多个特征 → 同一轮ImageData；创建包装对象不表示共享像素也复制了一份。','固定Feature保留；新轮清非固定槽；对象Value字段与后续通知元组可以额外持有图像。','清Feature槽仅减少对应持有，不归还所有chunk/共享生产者缓冲。',
 [ref(O,'internal void FeatureNotifyValueChanged(',34),ref(O,'private object FillImageValue(',67),C(O,'FeatureNotifyValueChanged'),C(O,'FillImageValue')],
 ['特征计数门不证明原生生产者写入与读取之间有完整帧租约；GetImageValue主动拉取是另一分支，不套用此通知门。']);
 add('ON03','Object.NotifyValueChanged','双数组通知队列 → 后台OnValueChanged分发',
 'NotifyValueChanged先将FillValue返回值赋给Object.value，再以同一value引用构造RenderDataEventArgs，连同Object包装Tuple。静态构造建立两个各1000槽的object[]；接收计数小于1000时入队，满时此体不入队，也没有本体补发循环，最后仍重置已收Feature计数。后台线程交换发送/接收数组，逐项调用OnValueChanged，再在正常返回后把已发送槽设null、更新发送计数，并Sleep(3)。这条通知路径会跨越一次异步排队，不是同步直接把原生回调显示到控件。',
 'Tuple持Object与事件，事件Value持ImageData；传递是引用包装，不是隔离像素快照。','双数组和Object.value是不同持有者；队列满的分支不代表Object.value被清。接收长度检查在锁外，不声称完整并发队列合同。','正常发送后清该发送槽；只减少队列持有，Object.value、显示仓库和消费者仍可继续持有图像。',
 [ref(O,'internal void NotifyValueChanged(',31),ref(O,'static Object()',61),C(O,'NotifyValueChanged'),C(O,'.cctor'),C('.<>c','<.cctor>b__13_0')],
 ['线程while(true)的完整关闭/Join及宿主退出协议尚未闭合；不能依据后台线程标记推断通知已经排空。','只确认入队和发送的本体顺序；队列积压、超过1000项的实际丢通知与并发时序未运行注入。']);
 add('ON04','ObjectBase.OnValueChanged / Object.ClearValueStore','事件接收与聚合清理各有边界',
 'ObjectBase.OnValueChanged调用ValueChanged(this,args)，事件处理异常被该体捕获并记录。Object.ClearValueStore先把临时value替换成新List<object>并清valueExecuteCount，再清非固定Feature槽并要求Feature下轮重建；没有逐图像Dispose。Clear并不会修改已经入队的RenderDataEventArgs.Value，因此旧通知在没有其它门阻止时仍可能继续被接收。接收方仍须按其自己的owner/计数/忙碌门决定入仓与呈现。',
 '事件args.Value、Object.value、Feature.ValueStore与队列Tuple不是同一个持有位置。','清聚合容器不撤销队列中既有引用；数据仓与UI的执行计数门需沿后续链核对。','仅清引用/重建标记；未在本体回收所有ImageData像素或原生共享资源。',
 [ref(B,'internal void OnValueChanged(',15),ref(O,'internal override void ClearTempValue()',5),ref(O,'public void ClearValueStore()',21),C(B,'OnValueChanged'),C(O,'ClearValueStore')],
 ['某一处理器异常对多订阅者分发的具体影响、日志处理自身失败和所有订阅者的生命周期仍需另证；不把异常被记录等同全部处理成功。']);
 const tool=JSON.parse(fs.readFileSync(path.join(out,'data-lifecycle-image.decompile.json'))),provenance={schemaVersion:1,scope:'Original feature aggregation, ImageData wrapping, queued callback and clearing; no vendor assembly loading',artifact,methods,branchTargets:branches,manifest:'image-notification.artifact-identities.json',metadata:'image-notification.cil.json',toolFiles:tool.toolFiles,sources:[O,F,B].map(t=>{const p='docs/architecture-recovery/image-notification.resources/'+t+'.decompiled.cs';return{path:p,sha256:sha(fs.readFileSync(path.join(root,p)))};}),workerResolvedFromCctorLdftn:{token:'0x06001024',type:'.<>c',name:'<.cctor>b__13_0'},targetAssemblyLoaded:false,targetExecuted:false};
 fs.writeFileSync(path.join(out,'image-notification.decompile.json'),JSON.stringify(provenance,null,2)+'\n');return{facts,unknowns:['所选为托管事件链，不证明原生生产者对共享区的完整同步/租约。','通知、仓库、绑定与位图是不同阶段，现场同一活实例和运行时序未执行核实。'],provenance:{methods,branchTargets:branches}};
};
