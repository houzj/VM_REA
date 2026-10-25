const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
module.exports=function(){
 const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),dir='docs/architecture-recovery/moduleframe-image-read.resources/',base='docs/architecture-recovery/moduleframe-image.resources/',read=p=>fs.readFileSync(path.resolve(root,p)),layout=JSON.parse(read(base+'pe-layout.json')),b=read(layout.path);
 if(sha(b)!==layout.sha256)throw Error('ModuleFrame reader original differs');
 const strings=[0x226ab8,0x226ac4].map(rva=>{const s=layout.sections.find(s=>rva>=s.rva&&rva<s.rva+s.rawSize),o=s.rawOffset+rva-s.rva,e=b.indexOf(0,o);return{rva:'0x'+rva.toString(16),fileOffset:o,text:b.toString('ascii',o,e),bytes:b.subarray(o,e+1).toString('hex')};});if(strings[0].text!=='%d_%p_%s'||strings[1].text!=='%d_%p')throw Error('Image parse formats differ');
 if(!layout.imports.some(i=>i.iatVa==='0x180204768'&&/sscanf/.test(i.name)))throw Error('Image parse IAT differs');fs.writeFileSync(path.join(root,dir,'protocol.json'),JSON.stringify({original:{path:layout.path,sha256:layout.sha256},strings,candidateSelection:'Original format-string LEA references, PE runtime ranges and then fresh selected disassembly; byte patterns alone are not treated as function proof.',liveInstanceVerified:false},null,2)+'\n');
 const ref=(stem,a,n=1,asm=false)=>({...source(dir+stem+(asm?'.asm.txt':'.c'),a,asm?a+'\t':a,n),binaryIdentity:layout.sha256,evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode'}),facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,from,to,behavior,representation,cache,release,refs,unknowns,classification:'source-confirmed',stages:['processing','results'],modes:['all']});
 add('MR01','原ModuleFrame内部图像结果文本','按索引解析长度、像素地址与可选共享名',
 '所选0xf66f0先核参数结果对象和索引：负索引、空结果和越界各走错误分支；从已有字符串槽复制描述到局部字符串。没有共享名输出缓冲时按%d_%p解析，有缓冲时按%d_%p_%s解析。原件两个格式串已直接核字节，0x1800f68fe–6915汇编明确给安全scanf传递额外共享名缓冲及长度，不能照伪码省掉的变参推断没有写名称。解析后只有长度和解析出的像素地址均非0才写输出地址；这里复制描述文本，没有复制整张像素。',
 '文本中的数值长度与地址 → 输出槽；描述字符串副本不是图像像素副本。',
 '参数结果槽可重复读取；同一槽含地址不自动建立像素租约。',
 '局部字符串和临时引用控制块退出不等于所解析像素释放，生产者与调用者的最后读者仍独立。',
 [ref('1800f66f0','      if (lStack_b0 == 0)',44),ref('1800f66f0','      if ((*param_5 == 0)',25),ref('1800f66f0','1800f68fe',8,true),{...source(dir+'protocol.json','Original parsing formats','"strings": [',15),binaryIdentity:layout.sha256,evidenceType:'direct-original-PE-bytes'}],['安全scanf全部错误/截断时的目的缓冲状态、结果槽与生产者同帧/同计数关系未完整闭合。']);
 add('MR02','原共享图像记录读取0xef410','返回映射base+0x80，并额外保留共享记录',
 '0xef410在参数与索引门后对自身+400的互斥量加锁，按两个输入上下文和索引取缓存记录；取到时返回记录映射字段+0x80及记录长度，并按名称输出容量条件填写共享名。随后按param_4这个整数上下文键向另一个列表追加该共享记录，再退出局部引用及解锁。不是返回地址后立即清池。这里没有取得该上下文键生成函数的完整语义，不能直接把它命名为执行计数或最后读者计数。',
 '调用者拿到地址/长度，内部保留列表另持共享记录；两个持有者不能合并。',
 '新增加的保留列表与原缓存记录、工具FIFO、托管结果仓库各自独立。',
 '所选体证明增加保留，不证明保留列表在哪个退出门清空；不能据局部引用减计数断言映射销毁。',
 [ref('1800ef410','        if (plStack_98[6] != 0)',28),ref('1800ef410','        plVar8 = (longlong *)FUN_1800f05e0',15)],['上下文整数键、保留列表消费者/退役、所有映射最后读者尚待逐体追踪，未运行原版。']);
 add('MR03','原同步GetImage读取与内部结果写入','共享格式读写已接上，SDK回调转换仍分列',
 '既有GetImage/Ex→0x121720所选路径调用0xcaa60；新恢复0xcaa60核模块记录后调用0xf66f0，并只在成功时回填读取数量。原MF03的0xf49a0→0xf41e0写入长度_地址_共享名格式，本条已取得对应结果对象读取该格式的静态消费者。另一同步读分支走0xef410的映射记录；不能把这两条同步读路径与SDK历史查询/异步callback当作同一个接收对象。',
 '同一内部格式的写入与读取已能对照成员；活实例、同步读取时点和异步SDK typed载荷各有独立身份。',
 '源结果对象、共享映射记录、同步输出槽和异步结果缓存四层保留分别追踪。',
 '静态格式消费者闭合不证明生产缓冲可在回调或显示结束前复用，也不证明全局清理安全。',
 [ref('1800caa60','        iVar5 = FUN_1800f66f0',8),ref('180098ae0','    iVar6 = FUN_1800eee30',6),...['      iVar7 = FUN_1800caa60','      iVar7 = FUN_1800ef410'].map(a=>({...source(base+'180121720.c',a,a,5),binaryIdentity:layout.sha256,evidenceType:'retained-offline-ghidra-pseudocode'}))],['SDKtyped输出的全部调用/版本选择、执行计数与图像映射头一致性及最终释放仍未闭合。']);
 const files=[dir+'invocation.json',dir+'protocol.json','tests/ArchitectureRecovery/Recover-ModuleFrameImageRead.ps1','tests/NativeEvidence/VmNativeCapability.java',...fs.readdirSync(path.join(root,dir)).filter(n=>/\.c$|\.asm\.txt$|\.tsv$/.test(n)).map(n=>dir+n)];const provenance={schemaVersion:1,original:{path:layout.path,sha256:layout.sha256},files:files.map(p=>({path:p,sha256:sha(read(p))})),selectedFunctions:6,selectedCaptureExitCode:0,wholeBinaryAnalysisComplete:false,sourceProvider:'offline Ghidra CLI; not REA retained evidence',originalRuntimeExecuted:false,deviceCalled:false,fullOriginalChainClosed:false};fs.writeFileSync(path.join(out,'moduleframe-image-read.provenance.json'),JSON.stringify(provenance,null,2)+'\n');return{facts,unknowns:['所选同步读取格式已证，异步SDK同帧、执行计数与最后读者未闭合。'],provenance};
};
