const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
module.exports=function(){
 const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),dir='docs/architecture-recovery/buffer-native-ownership.resources/',read=p=>fs.readFileSync(path.resolve(root,p));
 const invocation=JSON.parse(read(dir+'invocation.json').toString('utf8').replace(/^\ufeff/,'')),original=JSON.parse(read('docs/architecture-recovery/image-producer-recovery-audit.json')).targets.find(t=>t.key==='buffer');
 if(sha(read(invocation.target))!==original.sha256||invocation.sha256!==original.sha256||invocation.vendorExecuted!==false)throw Error('Buffer native original identity drift');
 const imported=(va,name)=>{if(!original.imports.some(i=>i.iatVa===va&&i.name===name))throw Error('Native IAT mismatch '+va);};
 imported('0x180012208','memcpy_s');imported('0x1800122e8','VM_M_SetImageEx');
 const ref=(stem,anchor,n=1,kind='c')=>({...source(dir+stem+'.'+(kind==='c'?'c':'asm.txt'),anchor,kind==='c'?anchor:anchor+'\t',n),binaryIdentity:original.sha256,evidenceType:kind==='c'?'offline-ghidra-pseudocode':'offline-ghidra-assembly',provider:'existing Ghidra CLI; not REA output'}),facts=[];
 const add=(id,stages,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,stages,modes:['all'],classification:'source-confirmed',from,to,behavior,representation,cache,release,refs,unknowns});
 add('NB01',['processing'],'原缓存图像CopyImage','接口申请目标缓冲后复制像素与元数据',
 '本轮从原DLL静态恢复CopyImage@0xe6a0。入口RDX保存到RBX作为目标描述，R8保存到RDI作为源描述；this+0x50的接口虚表+8接收模块标识、源+c长度、目标+0x10指针槽及附加返回槽。非0返回将目标+0x18置0并返回0xe0000002。成功后memcpy_s由RCX=目标像素、RDX=目标容量、R8=源像素、R9=源+c长度调用（0x18000e710–e720，当前原件IAT0x180012208核为memcpy_s），再复制宽/高/格式/长度字段并保存接口返回值到目标+0x20。memcpy_s返回值在此选定体没有检查。',
 '输入描述借来的源指针 → 接口返回的另一目标缓冲 → 显式像素复制；源与缓存不是仅共享同一像素引用。',
 '本函数创建缓存描述所需像素，但不负责决定哪一轮刷新或淘汰。',
 'NM01–NM03另已核到分配接口与映射创建，+0x20返回共享名地址；它不是已确认的所有权令牌，完整消费者租约仍未闭合。',
 [ref('18000e6a0','  local_res8 = 0;',26),ref('18000e6a0','18000e6af',16,'asm'),ref('18000e6a0','18000e710',18,'asm')],['所选虚表分配体和本地关闭体已由NM01–NM03续证；失败时目标指针、memcpy_s失败后及ModuleFrame接收方资源合同仍未闭合，未执行原版或设备。']);
 add('NB02',['processing','results','retirement'],'原缓存图像Process的RefreshFlag','刷新时追加并按数量淘汰；非刷新保持队列',
 'Process读取BufferCount，拒绝小于0或大于15的值；RefreshFlag读取后只有等于1才GetInImage→CopyImage，任一失败返回错误，不进入该轮追加。成功经helper0x18000f630创建记录并链接到列表尾，增加this+0xd8计数；随后仅在该刷新分支循环删队首直到数量不大于BufferCount。RefreshFlag不等于1会跳过这一整段，所以只降低BufferCount并不在这条路径立即缩容；BufferCount=0而刷新为1仍先取得/复制/追加再淘汰。输出循环不从队列移除元素。',
 'this+0xd0为链表哨兵关联字段、+0xd8为节点数；追加/输出所传描述中含像素指针，不把描述复制当成再次复制像素。',
 'FIFO刷新淘汰与输出读取分开；未刷新时继续输出原队列。',
 '淘汰分支摘链、减计数并operator_delete列表节点；此体未显式调用接口像素归还，不能据此认定像素泄漏或已经回收。',
 [ref('18000de30','  VM_M_GetInt(param_1,"BufferCount"',9),ref('18000de30','  if (0xf < local_438)',23),ref('18000de30','    iVar13 = CopyImage',52),ref('18000de30','18000e001',2,'asm'),ref('18000de30','18000e105',39,'asm')],['追加helper的全部分配/异常语义、外部池释放时机和参数getter失败合同仍待继续逐体核对。']);
 add('NB03',['results','display'],'原SetOutImage','写元数据后提交像素描述及额外栈参数',
 'SetOutImage按输出名前缀依次写Width、Height、PixelFormat，任一失败走返回；成功时组装临时图像描述，将缓存描述+0x10像素指针和+c长度写入，调用VM_M_SetImageEx。0x18000e902–e916还将缓存描述+0x20字段写到调用前[RSP+0x20]，该第五实参未显示在伪码调用签名中，必须保留。当前原件IAT0x1800122e8确认为VM_M_SetImageEx。在该体未再调用像素memcpy；这证明提交侧使用描述/指针，不证明ModuleFrame接收方不复制或不持有它。',
 '缓存中的描述 → 临时提交描述，包含既有像素地址、长度以及ABI额外参数；不是已确认端到端零复制。',
 '循环输出可再次发布相同缓存像素；接收方如何保存或复用待其callee证据。',
 'SetOutImage所释放的临时std::string不能画成图像像素释放；接收方租约和最后读者仍未知。',
 [ref('18000e760','        memset(local_d0',23),ref('18000e760','18000e8ce',27,'asm')],['VM_M_SetImageEx第五实参的确切语义及调用后接收方的复制、持有和释放没有由名称或反编译签名推断。']);
 add('NB04',['retirement'],'原缓存图像DeInit@0xdd20','逐节点摘链与删除，不自动等于像素归还',
 'DeInit已按原PE精确导出地址恢复：循环读取this+0xd8，非0则取this+0xd0的队首、重接前后指针、计数减一并调用operator_delete节点，直至0返回。完整所选94字节Ghidra函数体的汇编只有这一个删除调用，没有由节点像素+0x10加载后调用图像释放接口的动作。范围限定在这个DeInit函数，不能据此否定基类、模块会话或外部分配池的后续清理。',
 '清的是链表节点持有；像素由CopyImage中的接口取得，属于另一个需单独追踪的资源域。',
 '该函数移除队列全部节点，不证明框架结果、显示或外部池已不再持有图像。',
 '节点delete与底层像素最终释放分别记录；不从源码未见直接Free推断已经泄漏。',
 [ref('18000dd20','int __thiscall CImageBufferModule::DeInit',24),ref('18000dd20','18000dd20',25,'asm')],['NM04续证所选模块/基类映射销毁链；结果接收方与在飞显示的最后读者仍须续追。']);
 const captured=[dir+'invocation.json','tests/ArchitectureRecovery/Recover-ImageBufferNative.ps1','tests/NativeEvidence/VmNativeCapability.java',...['18000dd20','18000de30','18000e540','18000e6a0','18000e760'].flatMap(s=>[dir+s+'.c',dir+s+'.asm.txt']),...fs.readdirSync(path.join(root,dir)).filter(n=>/^functions-.*\.tsv$/.test(n)).map(n=>dir+n)];
 const provenance={schemaVersion:1,original:{path:invocation.target,sha256:original.sha256},files:captured.map(p=>({path:p,sha256:sha(read(p))})),provider:'Offline Ghidra 12.1.4 CLI, existing JDK21; not REA retained evidence',selectedFunctions:5,cliExitCode:0,vendorRuntimeExecuted:false,targetAssemblyLoaded:false,deviceCalled:false,globalConfigurationChanged:false,limitations:['Fresh pseudocode requires assembly/ABI checking; selected argument paths checked, whole callee ownership contract remains open.','Ghidra logger reported non-ASCII path XML errors; successful process exit and five c/asm/TSV artifacts, not missing logger output, establish selected recovery.']};
 fs.writeFileSync(path.join(out,'buffer-native-ownership.provenance.json'),JSON.stringify(provenance,null,2)+'\n');
 return{facts,unknowns:['所选五体与另列NM接口/映射清理体已恢复；接收方像素租约和最终读者仍未闭合。'],provenance};
};
