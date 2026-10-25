// Formal communication dispatch view. Reads text and PE bytes only; never loads vendor code.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const {root,out,source,writeView}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const read=name=>JSON.parse(fs.readFileSync(path.join(out,name),'utf8'));
const auditName='external-entry-audit.json',originalName='external-entry-audit-original-members.json';
const auditBytes=fs.readFileSync(path.join(out,auditName)),audit=JSON.parse(auditBytes),originalBytes=fs.readFileSync(path.join(out,originalName)),original=JSON.parse(originalBytes);
const identity=read('solution-lifecycle-rea-identity.json');
const expectedIds=Array.from({length:13},(_,i)=>'EA'+String(i+1).padStart(2,'0'));
if(audit.complete||audit.runtimeExecuted||audit.entries.length!==13||JSON.stringify(audit.entries.map(e=>e.id))!==JSON.stringify(expectedIds))throw Error('Communication audit scope changed');
if(original.evidence_id!=='ev_fbe6e3bef56ab98d6f6a7756adbe474d2b6583f820583efa489711898053e0bb'||original.methods.length!==9||original.artifact.sha256!==identity.artifact.sha256||original.module.mvid!==identity.module.mvid)throw Error('Communication original identity changed');
const retainedBytes=fs.readFileSync(path.resolve(root,original.retainedSource.path));
if(sha(retainedBytes)!==original.retainedSource.sha256)throw Error('Retained communication source changed');
const pe=fs.readFileSync(original.artifact.path);if(sha(pe)!==original.artifact.sha256)throw Error('Original EXE changed');
const wanted={
 '0x0600002f':['VisionMaster.MainWindowData','.ctor'],
 '0x06000037':['VisionMaster.MainWindowData','DoClose'],
 '0x06000083':['VisionMaster.MainWindowData','InitComTrigger'],
 '0x06000084':['VisionMaster.MainWindowData','VmSolution_OnCommunicationRecvCallBackEvent'],
 '0x06000268':['VisionMaster.AppMainWindow.SolutionItemsData','LoadSolInfo'],
 '0x06000269':['VisionMaster.AppMainWindow.SolutionItemsData','SaveSolInfo'],
 '0x0600026a':['VisionMaster.AppMainWindow.SolutionItemsData','GetSolItem'],
 '0x06000326':['<>c__DisplayClass157_0','<VmSolution_OnCommunicationRecvCallBackEvent>b__0'],
 '0x06000374':['<>c__DisplayClass39_0','<GetSolItem>b__0']
};
const methods=original.methods.map(m=>{
 const b=m.body,expected=wanted[m.token];if(!expected||m.declaring_type!==expected[0]||m.name!==expected[1]||b.status!=='present'||b.truncated_instructions!==0)throw Error('Communication original member selection changed');
 const at=b.file_offset;if(!Number.isInteger(at)||at<0||at+2>pe.length)throw Error('Original method offset invalid');
 const header=b.header_format==='fat'?(pe.readUInt16LE(at)>>>12)*4:b.header_format==='tiny'?1:0;
 if(header<1||at+header+b.il_size>pe.length)throw Error('Original method bounds invalid');
 const il=pe.subarray(at+header,at+header+b.il_size);if(sha(il)!==b.il_sha256)throw Error('Original method CIL digest changed: '+m.token);
 return {token:m.token,declaringType:m.declaring_type,name:m.name,rawIlSha256:b.il_sha256,rawIlBytes:il.toString('hex'),ilSize:b.il_size,fileOffset:at,headerSize:header,reaBodyInstructionCoverage:'complete retained body summary; original raw CIL bytes rechecked',semanticScope:'The local decompiled source is separately cited; this identity anchor does not bind other assemblies or every neighbouring declaration.'};
});
const evidenceName='communication-dispatch-original-members.json';
fs.writeFileSync(path.join(out,evidenceName),JSON.stringify({schemaVersion:1,complete:false,reaEvidence:original.evidence_id,artifact:original.artifact,module:original.module,source:{path:'docs/architecture-recovery/'+originalName,sha256:sha(originalBytes)},retainedSource:original.retainedSource,methods,runtimeExecuted:false},null,2)+'\n');
const methodRef=token=>{
 const m=methods.find(m=>m.token===token);if(!m)throw Error('Unknown communication method token');
 const ref=source('docs/architecture-recovery/'+evidenceName,m.declaringType+'::'+m.name+' '+token,'"token": "'+token+'"',10);
 ref.evidenceType='original-managed-rea-body-digest';ref.binaryIdentity=original.artifact.sha256+' / MVID '+original.module.mvid;return ref;
};
const bound={EA01:['0x0600002f','0x06000083'],EA03:['0x06000084','0x0600026a','0x06000374'],EA04:['0x06000084'],EA05:['0x06000084','0x06000326'],EA06:['0x06000083','0x06000037'],EA07:['0x06000268','0x06000269','0x0600026a']};
let sourceReferences=0;
const sourceSnapshots=new Map();
const edges=audit.entries.map(e=>{
 if(e.classification!=='source-confirmed'||!e.refs.length||!e.unknowns.length)throw Error('Communication source evidence incomplete');
 const refs=e.refs.map(r=>{
  const bytes=fs.readFileSync(path.resolve(root,r.path)),lines=bytes.toString('utf8').split(/\r?\n/);
  if(sha(bytes)!==r.sha256||lines.slice(r.start-1,r.end).join('\n')!==r.excerpt)throw Error('Communication source citation changed: '+e.id+' '+r.path);
  sourceSnapshots.set(r.path,r.sha256);sourceReferences++;return {...r};
 });
 for(const token of bound[e.id]??[])refs.push(methodRef(token));
 for(const l of e.priorLinks??[]){const model=read(l.view+'.edges.json');if(!model.edges.some(p=>p.id===l.id))throw Error('Missing communication parent edge '+l.view+':'+l.id);}
 return {id:e.id,route:'原版 · '+e.group,classification:'source-confirmed',classificationScope:'Local decompiled source statements; selected EXE method identity and original raw CIL bytes are separately rechecked, without vendor execution.',from:e.from,to:e.to,behavior:e.behavior,refs,unknowns:[...e.unknowns],nextMembers:e.nextMembers,priorLinks:e.priorLinks};
});
edges.push({id:'U01',route:'未知 · 完整入口与当前实现',classification:'unknown',from:'通信注册、报文、方案配置、触发与网关链',to:'跨程序集原体、当前实现及真实生命周期尚未闭合',behavior:'本页把通信接收换方案、触发配置即时写入和网关/脚本边界拆成独立证据边；源码存在或相邻框都不证明所有入口运行可达。原EXE九体保留REA身份并重核原始CIL字节，其它SDK/Core/WPF本地反编译源未在此逐体绑定。未建立当前独立实现对应，不添加推测性的实现边。',refs:[],unknowns:['SDK回调原件、native消息/长度/ABI/线程及事件实投尚未验证。','MainWindowData所有创建/替换/解除路径，轮询结束条件和异常后恢复仍需完整对象生命周期证据。','GlobalTriggerModule实际接收字节、执行换方案、持久化和网关SDK释放未闭合。','当前main实现的菜单/通信/触发/密码/文件字段对应尚未逐操作映射。','尚未取得全域入口全集、浏览器呈现、原机运行或设备验收；完整入口链关闭数为0。']});
for(const [file,digest]of sourceSnapshots)if(sha(fs.readFileSync(path.resolve(root,file)))!==digest)throw Error('Concurrent communication source change');
fs.writeFileSync(path.join(out,'communication-dispatch-review.json'),JSON.stringify({schemaVersion:1,complete:false,auditSource:{path:'docs/architecture-recovery/'+auditName,sha256:sha(auditBytes)},originalMemberEvidence:{path:'docs/architecture-recovery/'+evidenceName,sha256:sha(fs.readFileSync(path.join(out,evidenceName))),bodies:methods.length},sourceReferences,uniqueSourceFiles:sourceSnapshots.size,findingsPromotedToFormalView:expectedIds,closedEntryChains:0,currentImplementationMapped:false,runtimeExecuted:false,meaning:'Finite source and identity mapping; no original/native/runtime equivalence or complete entry enumeration.'},null,2)+'\n');
writeView({slug:'communication-dispatch',title:'通信换方案、触发写入与后台分派',domains:['F-01','F-03','F-04','F-16','F-18','F-21','F-23'],introduction:'从主窗口注册通信回调追到报文匹配、配置路径/密码、方案加载和轮询；另列全局触发的即时写入、错误密码清空与网关/脚本的不同接收者。每条边分别保留条件、状态、失败和未证范围。',edges,provenance:'Thirteen finite local-source findings promoted from external-entry-audit; original EXE SHA256/MVID and nine retained REA raw CIL byte digests rechecked. SDK/Core/WPF source hashes are separately verified and are not promoted to per-body original binary binding. No vendor, SDK, service, gateway or device execution. Current implementation and complete entry chains remain unestablished.'});
