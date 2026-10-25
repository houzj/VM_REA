const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
module.exports=function(){
 const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),dir='docs/architecture-recovery/buffer-interface-origin.resources/',read=p=>fs.readFileSync(path.resolve(root,p));
 const invocation=JSON.parse(read(dir+'invocation.json').toString('utf8').replace(/^\ufeff/,'')),target=JSON.parse(read('docs/architecture-recovery/image-producer-recovery-audit.json')).targets.find(t=>t.key==='buffer'),bytes=read(target.path);
 if(sha(bytes)!==target.sha256||invocation.sha256!==target.sha256)throw Error('Buffer interface original changed');
 const offset=rva=>{const s=target.sections.find(s=>rva>=s.rva&&rva<s.rva+s.rawSize);if(!s)throw Error('RVA not backed');return s.rawOffset+rva-s.rva;},at=offset(0x13a80);
 const slots=[[0,'destructor-thunk','0x18000f9f8'],[8,'allocation','0x180007fa0'],[16,'release-by-name','0x180008380']].map(([slotOffset,purpose,expected])=>{const value='0x'+bytes.readBigUInt64LE(at+slotOffset).toString(16);if(value!==expected)throw Error('Original vtable differs');return{slotOffset,purpose,target:value,fileOffset:at+slotOffset,rawBytes:bytes.subarray(at+slotOffset,at+slotOffset+8).toString('hex')};});
 const layout={target:{path:target.path,sha256:target.sha256},subobjectOffset:'0x50',vtableRva:'0x13a80',vtableExport:target.exports.find(e=>e.rva==='0x13a80'),slots};fs.writeFileSync(path.join(root,dir,'vtable-layout.json'),JSON.stringify(layout,null,2)+'\n');
 const checkImport=(va,name)=>{if(!target.imports.some(i=>i.iatVa===va&&i.name===name))throw Error('IAT identity differs '+va);};checkImport('0x180012070','UnmapViewOfFile');checkImport('0x1800120b8','CloseHandle');
 const ref=(stem,a,n=1,asm=false)=>({...source(dir+stem+(asm?'.asm.txt':'.c'),a,asm?a+'\t':a,n),evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode',binaryIdentity:target.sha256,provider:'existing offline Ghidra CLI; not REA output'}),facts=[];
 const add=(id,stages,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,stages,modes:['all'],classification:'source-confirmed',from,to,behavior,representation,cache,release,refs,unknowns});
 add('NM01',['processing','retirement'],'原缓存图像this+0x50接口','本模块共享内存基类的具体虚表成员',
 '原构造器先初始化this+0x50的CModuleSharedMemoryBase子对象，再写派生类对应虚表。当前原PE导出的共享内存子表RVA0x13a80逐槽读取：+8=0x180007fa0，+0x10=0x180008380。因此NB01中CopyImage的接口申请可静态接到本DLL的0x7fa0原体，而不再泛称未取得分配callee。0x8380原体包含共享名解析、模块标识校验和按名查找/摘链，再进入CloseMapping；其scanf省参尚未逐寄存器闭合，不据显示签名猜字段写入。',
 '工具队列与共享内存基类的池结构是两个持有层；接口对象是模块内子对象，不是当前托管scope。',
 '0x7fa0按类别查询池列表，结合VM_M_GetBufferNodeNum返回量选择复用或新描述；全部类别、计数和并发约束未由此宣称闭合。',
 '取得release-by-name实现不证明每次FIFO淘汰都调用它；NB02/04的节点删除仍与映射归还分开。',
 [ref('18000da70','  pCVar1 = this + 0x50;',19),{...source(dir+'vtable-layout.json','Original shared-memory vtable slots','"slots": [',23),binaryIdentity:target.sha256,evidenceType:'direct-original-PE-bytes'},ref('180007fa0','  if (param_3 == 0)',28),ref('180008380','    if (param_2 == local_res18)',17)],['vtable绑定为静态成员关系，不代表运行实例、共享名解析ABI、全部池分支与最后读者已验证。']);
 add('NM02',['processing','results','display'],'原分配函数返回值与映射头','像素地址base+0x80、共享名和分离的头字段',
 '0x7fa0所选创建/扩容成功支在映射首地址写像素长度，+0xe写ushort0x80，+8写byte0，并把描述中的共享名复制到头+0x10的0x40字节区域。之后公共尾段更新长度和头+0x50的uint0，向第四实参输出base+0x80，向第五实参输出描述+0x14的共享名地址。0x18000827f–0x1800082d9汇编核对了这些写入及两个返回槽。因此CopyImage保存到描述+0x20的附加值可定位为该共享名地址，而非凭名称猜为所有权令牌。',
 '映射头 / 像素区 / 共享名地址不同；两个返回槽也不等于复制了两份像素。',
 '容量足够的复用路径跳过部分创建头初始化，但公共尾段仍更新长度与+0x50；不能把整个映射每轮清零。',
 '头的offset/标志字段与消费者协议可比较，但尚未证明ModuleFrame如何写执行计数、何时允许复用及同帧读取。',
 [ref('180007fa0','    plVar4 = (longlong *)*plVar15;',13),ref('180007fa0','  *param_4 = *plVar15 + 0x80;',4),ref('180007fa0','18000827f',29,true)],['消费者offset>84/计数门另由OD取证；协议对齐不能替代生产者到消费者的同实例、同计数与完整租约证据。']);
 add('NM03',['processing','retirement'],'原CreateOpenMapping / CloseMapping','创建句柄、映射视图、关闭本地视图与句柄',
 '0x8d90在描述大小/共享名门满足后调用CreateFileMappingA，并把句柄保存到描述+8；GetLastError非0走关闭/更名helper与一次重试，仍错误则返回0xe0000015。MapViewOfFile结果保存到描述首字段，为空则关闭并报错。0x90f0先拒绝空描述或容量0；非空视图调用UnmapViewOfFile，非空句柄调用CloseHandle，分别保留错误码，随后即使关闭失败仍清本地地址、句柄和容量字段。所选汇编及原PE IAT分别确认UnmapViewOfFile/CloseHandle。',
 '描述保存本地映射视图、OS句柄、容量、名称；这些不是托管ImageData或WPF位图。',
 '映射创建尺寸含原件常量参与的计算，本条没有按常见策略把它解释成恰好length+128；更名helper与完整碰撞合同仍待核。',
 '清本地描述不证明OS对象已消失、其它进程/显示映射已关闭或原指针的所有使用者已退出；错误时字段归零与真正关闭成功分开。',
 [ref('180008d90','  if (param_3 != (undefined8 *)0x0)',35),ref('180008d90','LAB_180008fad:',8),ref('1800090f0','  if (param_3 ==',63),ref('1800090f0','18000922b',13,true)],['未执行任何Win32映射API；全部创建/碰撞/错误/多读者行为只按所选静态体限定。']);
 add('NM04',['results','retirement'],'原缓存节点、模块析构与共享池析构','描述拷贝、队列删除与关闭映射三层退役',
 '追加helper0xf190分配0x48字节链表节点；0xf630以三次16字节和一次8字节搬运把0x38字节图像描述写到节点+0x10，没有在此复制其像素。模块析构先摘链删除缓存节点及哨兵，再以RCX=this+0x50调用0x7de0。该共享内存基类析构遍历池描述，对每项调用0x90f0关闭映射，然后处理引用控制块和容器。NB04的DeInit只清队列，不能替代这条模块/基类析构链。',
 '缓存节点保存含像素指针/共享名地址的描述；共享池另外保存映射资源。',
 '节点被删时，基类池可能仍持映射；模块销毁再走不同资源域的清理。',
 '现已定位所选共享池映射清理callee，但没有证明模块销毁前所有结果、显示任务或外部消费者排空；不宣称原版生命周期全安全。',
 [ref('18000f190','  puVar1 = operator_new',15),ref('18000f630','18000f647',8,true),ref('18000db90','  FUN_180007de0',3),ref('18000db90','18000dc54',11,true),ref('180007de0','    for (puVar7 =',5)],['基类析构末尾的反编译间接跳转警告保留；引用控制块所有callee、模块Destroy调用时序和最后像素读者仍未闭合。']);
 const files=[dir+'invocation.json',dir+'vtable-layout.json','tests/ArchitectureRecovery/Recover-ImageBufferInterface.ps1','tests/NativeEvidence/VmNativeCapability.java',...fs.readdirSync(path.join(root,dir)).filter(n=>/\.c$|\.asm\.txt$|^functions-.*\.tsv$/.test(n)).map(n=>dir+n)];
 const provenance={schemaVersion:1,original:layout.target,files:files.map(p=>({path:p,sha256:sha(read(p))})),selectedFunctions:14,vtableSlots:slots,cliExitCode:0,provider:'existing offline Ghidra CLI',originalRuntimeExecuted:false,deviceCalled:false,wholeOwnershipClosed:false,remaining:['ModuleFrame submission/receiver lease and execute-count publication','All live consumers and inflight retirement','Pool rename/helper/refcount branches not fully recovered']};fs.writeFileSync(path.join(out,'buffer-interface-origin.provenance.json'),JSON.stringify(provenance,null,2)+'\n');return{facts,unknowns:provenance.remaining,provenance};
};
