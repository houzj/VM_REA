// Static original-PE evidence only. This module never loads a vendor assembly/DLL.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const root=path.resolve(__dirname,'../..'),dir='docs/architecture-recovery/variable-dispatch.resources/';
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
const read=p=>fs.readFileSync(path.join(root,p));
const json=p=>JSON.parse(read(p).toString().replace(/^\ufeff/,''));
module.exports=function(){
 const {source}=require('./evidence-view.cjs'),inspect=require('./native-pe-inspection.cjs');
 const originals={};
 for(const [kind,name] of [['global','globalvariable'],['frame','moduleframe'],['sdk','platformsdk']]){
  const saved=json(dir+name+'.pe.json'),fresh=inspect(saved.path);
  if(fresh.sha256!==saved.sha256||JSON.stringify(fresh)!==JSON.stringify(saved))throw Error('Original PE snapshot differs '+name);
  originals[kind]=fresh;
 }
 const bytes=Object.fromEntries(Object.entries(originals).map(([k,p])=>[k,fs.readFileSync(p.path)]));
 const offset=(kind,rva)=>{const s=originals[kind].sections.find(s=>rva>=s.rva&&rva<s.rva+s.rawSize);if(!s)throw Error('No file-backed RVA');return s.rawOffset+rva-s.rva;};
 const q=(k,r)=>bytes[k].readBigUInt64LE(offset(k,r));
 const text=(k,r)=>{const p=offset(k,r);return bytes[k].toString('ascii',p,bytes[k].indexOf(0,p));};
 const peStrings=[['frame',0x229340,'CreateModule'],['frame',0x229380,'DestroyModule'],['frame',0x21f538,'##ImportModuleData##'],['frame',0x21f550,'DynamicInData'],['frame',0x21f560,'DynamicOutData']].map(([artifact,rva,value])=>{if(text(artifact,rva)!==value)throw Error('Original string differs');return{artifact,rva:'0x'+rva.toString(16),value};});
 const tables=json(dir+'globalvariable-vtable-layout.json');
 for(const t of tables.tables){
  const r=Number(t.startRva);if(q('global',r+0x18)!==0x18001f8c0n||t.entries[3].va!=='0x18001f8c0')throw Error('Receiver slot differs');
  const col=Number(q('global',r-8)-0x180000000n),p=offset('global',col),td=bytes.global.readUInt32LE(p+12);
  if(col!==Number(t.col.rva)||bytes.global.readUInt32LE(p)!==t.col.signature||bytes.global.readUInt32LE(p+4)!==t.col.offset||td!==Number(t.col.typeDescriptorRva)||text('global',td+16)!==t.col.name)throw Error('Original COL/RTTI differs');
  for(const e of t.entries)if(q('global',r+Number(e.offset))!==BigInt(e.va))throw Error('Vtable entry differs');
 }
 if(tables.tables.length!==2||tables.tables[0].col.name!=='.?AVCVariableModuBase@@'||tables.tables[1].col.name!=='.?AVCMyAlgorithmManager@@')throw Error('Selected original RTTI types differ');
 if(originals.global.exports.find(e=>e.name==='CreateModule')?.rva!=='0x4f9b0'||originals.frame.exports.find(e=>e.name==='VM_M_SetBinayData')?.rva!=='0x13aad0'||originals.sdk.exports.find(e=>e.name==='IMVS_SetBinaryData')?.rva!=='0x6471f0')throw Error('Selected original export differs');
 const captures=[];
 for(const n of fs.readdirSync(path.join(root,dir)).filter(n=>/^(global|frame|sdk)-capture-.*\.json$/.test(n))){
  const k=n.split('-')[0],v=json(dir+n);if(v.sha256!==originals[k].sha256)throw Error('Capture identity differs');
  for(const r of v.records){const raw=bytes[k].subarray(offset(k,r.begin),offset(k,r.begin)+r.end-r.begin);if(hash(raw)!==r.bytesSha256||hash(read(dir+k+'-'+r.begin.toString(16)+'.bin'))!==r.bytesSha256)throw Error('Captured bytes differ');}
  captures.push({path:dir+n,sha256:hash(read(dir+n)),...v});
 }
 const completions=[];
 for(const kind of ['global','frame'])for(const n of fs.readdirSync(path.join(root,dir,kind)).filter(n=>/^completion-.*\.json$/.test(n))){const v=json(dir+kind+'/'+n);if(v.exitCode!==0)throw Error('Static CLI capture failed');completions.push({path:dir+kind+'/'+n,...v});}
 const ref=(kind,stem,anchor,count=1,mode='asm')=>({...source(dir+(mode==='dump'?kind+'-'+stem+'.dumpbin.txt':kind+'/'+stem+(mode==='c'?'.c':'.asm.txt')),stem,mode==='asm'?anchor+'\t':anchor,count),binaryIdentity:originals[kind].sha256,evidenceType:mode==='c'?'fresh-offline-ghidra-pseudocode':mode==='dump'?'fresh-original-PE-dumpbin-instructions':'fresh-offline-ghidra-assembly',provider:'offline static tools; not REA retained evidence'});
 const nativeImport=source('analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/NativeMethods.cs','IMVS_SetBinaryData import signature','public static extern int IMVS_SetBinaryData(IntPtr handle, uint nModuleID, ref ImvsSdkDefine.IMVS_SET_BINARY_DATA_INFO strValue, uint nRecvWaitTime);',1);nativeImport.start--;nativeImport.excerpt=read(nativeImport.path).toString().split(/\r?\n/).slice(nativeImport.start-1,nativeImport.end).join('\n');if(!nativeImport.excerpt.includes('DllImport(".\\\\iMVS-6000PlatformSDK.dll"'))throw Error('Managed native import target differs');
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'source-confirmed',stages:['variable','transfer','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('VB01','原GlobalVariableModule.CreateModule RVA0x4f9b0','同一factory返回对象首虚表 → slot+0x18 → VN01接收体0x1f8c0',
  '本轮取得实际CreateModule/构造器原体：factory请求0x10630字节，非空时以同一分配地址为RCX调用0x4fc30；构造器先调用0x1bf40，再把原PE虚表0x5ad60写到该对象首指针，返回同一地址。factory成功路径返回该对象。原PE逐qword核0x5ad60+0x18=0x18001f8c0；基础构造体0x1bf40较早写入的0x5ab38+0x18也指向同一接收体。两表COL/RTTI分别为CMyAlgorithmManager与CVariableModuBase；这不是仅凭类型或key名字拼接。',
  '偏移+0为factory返回完整对象的当前vptr；+0x50另有第二表，不能把第二接口的槽位混作主表+0x18。RVA均为原文件静态地址，原运行实例地址未观测。',
  'factory先通过主表+0x50初始化，再调用导入取得moduleID，保存对象+0x48；确认成功后返回对象。此处建立的是模块对象，不是变量payload的深复制。',
  '初始化或moduleID查询失败的所选路径经首虚表slot0、EDX=1退役对象并返回0；未在本事实把析构调用解释为全部内部payload已free。',
  [ref('global','4f9b0','  000000018004F9CB:',14,'dump'),ref('global','4f9b0','  000000018004FA2D:',25,'dump'),ref('global','4f9b0','  000000018004FA9D:',12,'dump'),ref('global','4f9b0','  000000018004FB67:',5,'dump'),ref('global','4f9b0','  000000018004FC23:',5,'dump'),ref('global','18004fc30','18004fc39',16),ref('global','1bf40','  000000018001BF70:',10,'dump'),{...source(dir+'globalvariable-vtable-layout.json','factory class primary vtable','      "startRva": "0x5ad60",',46),evidenceType:'fresh-original-PE-vtable-COL-decode',binaryIdentity:originals.global.sha256}],
  ['仅证该已识别DLL的静态factory返回类型与接收槽；实际进程采用哪个DLL副本、加载成功的moduleID与活实例未做运行印证。']);
 add('VB02','原ModuleFrame模块创建0x112f40 → loader0xb54e0','factory返回对象注册 → 0xb6b70按moduleID取原pointer → 模块record+0x128',
  '原0xb54e0对选定路径LoadLibraryW，按原字节字符串CreateModule/DestroyModule调用GetProcAddress；两个地址非空时，以创建参数记录调用取得的CreateModule函数，将返回RAX保存在loader记录+8，非0时向返回对象+8写入创建参数pointer。所选注册路径把同一loader记录及控制块放入manager容器。0xb6b70在manager+0x38的真实_Mtx_lock/unlock内遍历：创建参数首int等于传入moduleID且loader记录非空时返回loader记录+8的原factory对象pointer。0x112f40以相同moduleID调用loader成功后调用该getter，保存RAX到R15，再按moduleID查本框架记录，将同一R15写入record+0x128。',
  '这是factory对象pointer在两个注册层之间的保存和回取；没有在这段注册链复制数值/文本payload。loader记录中的DLL句柄、Create/Destroy函数pointer、factory对象pointer和框架record是不同字段，不能当作同一容器。',
  'loader容器保存记录及共享控制块；框架record+0x128保存factory对象地址。getter返回前释放所选查找锁，没有在getter构造另一份模块对象。',
  '所选创建/注册/查找路径不释放factory对象；容器替换处有控制块递减与条件虚调用，不能据此声称当前对象、变量值或最后读者已全部销毁。销毁路由和DLL卸载不属于本切片已闭合内容。',
  [ref('frame','1800b54e0','      puVar21 = (undefined8 *)LoadLibraryW(puVar21);',5,'c'),ref('frame','1800b54e0','      ppuVar11 = (undefined8 **)GetProcAddress(puVar21,&DAT_180229340);',6,'c'),ref('frame','1800b54e0','        ppppppplVar9 = (longlong *******)GetProcAddress(puVar21,&DAT_180229380);',12,'c'),ref('frame','1800b54e0','1800b5f81',6),ref('frame','1800b54e0','          uVar8 = (*(code *)ppuVar11)(puStack_358);',15,'c'),ref('frame','1800b54e0','      plVar4 = (longlong *)puVar18[6];',12,'c'),ref('frame','1800b6b70','  iVar4 = _Mtx_lock(param_1 + 0x38);',29,'c'),ref('frame','180112f40','18011345e',7),ref('frame','180112f40','1801138b5',9),{...source(dir+'moduleframe.pe.json','native factory entry strings source identity','  "sha256": "'+originals.frame.sha256+'",',3),evidenceType:'fresh-original-PE-identity-and-verified-literals',binaryIdentity:originals.frame.sha256}],
  ['loader接受通用模块路径；只有选到VB01识别的GlobalVariableModule原DLL时才把其factory/vtable代入本通用链，不能把每次创建都命名为全局变量模块。','真实SDK对象handle与该ModuleFrame moduleID/record的上游注册关系尚未闭合；没有活进程同实例运行断言。']);
 add('VB03','原ModuleFrame.VM_M_SetBinayData 0x13aad0','一般参数分支 → moduleID查record → record+0x128主虚表+0x18直接派发',
  '导出0x13aad0保存ECX=moduleID、RDX=key、R8=payload、R9D=length；调用0x43120时改为RCX=manager(0x2b12d0)、EDX=原moduleID、R8=原key、R9=原payload、第五栈参数=原length。0x43120先验证key非空及manager+0x22状态；特殊key ##ImportModuleData##、DynamicInData、DynamicOutData有各自路线，一般key经0x10a690按moduleID取得record/控制块。若record+0x128非0，原汇编以RCX=该factory对象pointer，RDX=同一key、R8=同一payload、R9D=同一length调用其首虚表+0x18。因此在VB02注册的是VB01工厂对象这一条件下，实际调用目标静态闭合为VN01的0x1f8c0，而非按SetBinaryData同名推定。',
  '一般分支将原pointer与长度传递到实际接收槽；没有在所选wrapper/dispatch分支出现复制payload的动作。R9D的32位长度与托管描述中嵌套pointer/count不是同一字段。',
  '0x10a690从manager+0x40名称为整型moduleID的树中取record和共享控制块，非空控制块加计数；record+0x128是已注册factory对象。dispatch返回后释放该次record局部共享引用，并返回所选调用状态。',
  '本分支局部控制块DEC与条件虚析构不是调用者payload FreeHGlobal，也不证明变量存储最后读者消失。record缺失返回0xe0000302；record存在但+0x128为空时转0xb9390，不可把这个fallback当作相同plugin接收体。',
  [ref('frame','13aad0','  000000018013AAE8:',7,'dump'),ref('frame','13aad0','  000000018013AB5A:',9,'dump'),ref('frame','180043120','      iVar5 = _stricmp(param_3,&DAT_18021f538);',12,'c'),ref('frame','10a690','  000000018010A6E9:',18,'dump'),ref('frame','43120','  000000018004336D:',15,'dump'),ref('frame','43120','  00000001800433CA:',5,'dump'),ref('frame','43120','  000000018004340C:',16,'dump'),ref('frame','43120','  000000018004343A:',19,'dump')],
  ['ModifyVar/DeleteVar不等于上述三个特殊key，因此属于所选一般分支；全体key、错误码和fallback语义未穷举。','上游SDK的0xbd2 callback如何调用到此ModuleFrame导出仍是VB04明确断点；本条不宣称整个SDK→插件链已闭合。']);
 add('VB04','原托管ModuleManager.SetBinaryData → iMVS-6000PlatformSDK.IMVS_SetBinaryData','SDK实际callback边界0x66f090：opcode0xbd2、binary pointer/length；未证等于ModuleFrame同名接口',
  '托管ModuleManager构造IMVS_SET_BINARY_DATA_INFO并调用ServerSDKAPI→NativeMethods，DllImport实际目标是iMVS-6000PlatformSDK.dll。本轮对同SHA原PE重新截取IMVS_SetBinaryData 0x6471f0与直接callee0x66f090原runtime范围及字节：导出先校验handle记录，再把moduleID、描述pointer、timeout交0x66f090；callee从描述+0x100读取binary pointer，从+0x108读取32位长度，读取this+0x23a8函数pointer。函数pointer非空时，以ECX=0xbd2、RDX=临时格式串、R8=描述中的binary pointer、R9D=描述中的length、第五/第六参数0进行间接调用。该调用的真实callee注册和跨组件接收者尚未取得，不能把它和ModuleFrame.VM_M_SetBinayData仅凭名字合并。',
  '已证SDK描述外层和其中binary pointer/length分别读取；0xbd2间接调用直接携带binary pointer，嵌套payload是否在callback/传输侧复制不由这两段原体决定。',
  'SDK handle记录查找与this+0x23a8 callback槽是SDK自己的状态；与VB02的ModuleFrame模块factory对象/record不是同一地址域证明。',
  '所选SDKwrapper在返回前清本次局部锁对象；不等于释放调用者临时HGlobal或descriptor内的数值/字符数据。callback同步消费/异步租约、超时返回后是否仍持有payload需要真正接收体。',
  [{...source('analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleManager.cs','SetBinaryData(uint,string,IntPtr,int,uint)','public int SetBinaryData(uint moduleId, string paramName, IntPtr dataPtr, int len, uint recvTimeOut = 3000u)',16),evidenceType:'decompiled-managed-source'},{...nativeImport,evidenceType:'decompiled-managed-source'},ref('sdk','6471f0','  00000001806472D3:',8,'dump'),ref('sdk','66f090','  000000018066F1FD:',16,'dump')],
  ['尚缺SDK this+0x23a8 callback的赋值来源、opcode0xbd2真实处理者、它到ModuleFrame.VM_M_SetBinayData的具体调用桥及binary载荷租约；不可用SDK/ModuleFrame/插件三个相似方法名替代该缺口。','原进程handle/moduleID、动态加载路径和内存地址未做运行观测；有限静态对象链不等于运行全等。']);
 const files=[];const walk=p=>{for(const x of fs.readdirSync(path.join(root,p),{withFileTypes:true})){const f=p+x.name;if(x.isDirectory())walk(f+'/');else files.push({path:f,sha256:hash(read(f)),byteLength:read(f).length});}};walk(dir);
 const counts={facts:facts.length,refs:facts.reduce((n,f)=>n+f.refs.length,0),capturedRuntimeRanges:captures.reduce((n,c)=>n+c.records.length,0),successfulStaticCliRuns:completions.length};
 return{schemaVersion:1,complete:false,facts,unknowns:[{id:'VBU01',target:'SDK 0x66f090 this+0x23a8 opcode0xbd2 → ModuleFrame 0x13aad0',question:'取callback安装与实际处理者，核binary pointer的复制/保留/回收及timeout语义；当前ModuleFrame→已注册插件虚成员静态桥已证，SDK这一边仍缺。'},{id:'VBU02',target:'factory object retirement / live identities',question:'加载的真实DLL副本、moduleID、最后一个对象读者、DestroyModule/FreeLibrary顺序与并发退出需另追；不执行原厂程序求证。'}],provenance:{originals,peStrings,vtables:tables.tables,captures,completions,counts,files,sourceProvider:'offline Ghidra CLI and MSVC dumpbin, not REA retained evidence',wholeBinaryAnalysisComplete:false,initialAnalysisTimeoutSeconds:70,upstreamSdkDispatchClosed:false,moduleFrameToRegisteredFactoryVirtualReceiverClosed:true,closureCondition:'The runtime-selected factory must be the exact GlobalVariableModule original artifact identified in VB01; static registration/lookup preserves that factory-return pointer.',originalRuntimeExecuted:false,targetCodeExecuted:false,deviceCalled:false,anotherWorktreeRead:false,implementationChanges:false}};
};
if(require.main===module){const m=module.exports(),out='docs/architecture-recovery/original-variable-dispatch.json';fs.writeFileSync(path.join(root,out),JSON.stringify(m,null,2)+'\n');console.log(JSON.stringify({report:out,...m.provenance.counts,upstreamSdkDispatchClosed:m.provenance.upstreamSdkDispatchClosed}));}
