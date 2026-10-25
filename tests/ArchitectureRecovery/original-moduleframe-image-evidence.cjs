const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
module.exports=function(){
 const dir='docs/architecture-recovery/moduleframe-image.resources/',read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),layout=JSON.parse(read(dir+'pe-layout.json')),binary=read(layout.path);
 if(sha(binary)!==layout.sha256)throw Error('ModuleFrame original differs');const exported=layout.exports.find(e=>e.name==='VM_M_SetImageEx');if(exported?.rva!=='0x1312f0')throw Error('Image export changed');
 const rva=0x226ab8,sec=layout.sections.find(s=>rva>=s.rva&&rva<s.rva+s.rawSize),off=sec.rawOffset+rva-sec.rva,end=binary.indexOf(0,off),format=binary.toString('ascii',off,end);if(format!=='%d_%p_%s')throw Error('Original image descriptor format differs');
 const protocol={artifact:{path:layout.path,sha256:layout.sha256},export:exported,format:{rva:'0x226ab8',fileOffset:off,rawBytes:binary.subarray(off,end+1).toString('hex'),text:format},analysisTimedOut:true,selectedBodiesRecovered:true,wholeBinaryAnalyzed:false,runtimeModuleResolutionVerified:false};fs.writeFileSync(path.join(root,dir,'protocol.json'),JSON.stringify(protocol,null,2)+'\n');
 const ref=(stem,a,n=1,asm=false)=>({...source(dir+stem+(asm?'.asm.txt':'.c'),a,asm?a+'\t':a,n),binaryIdentity:layout.sha256,evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode',provider:'existing offline Ghidra CLI; not REA output'}),facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,from,to,behavior,representation,cache,release,refs,unknowns,classification:'source-confirmed',stages:['processing','results','display'],modes:['all']});
 add('MF01','原VM_M_SetImageEx导出','像素指针、长度与第五实参共享名继续向内转发',
 '安装目录ModuleProxy/x64/ModuleFrame.dll的原PE将VM_M_SetImageEx绑定RVA0x1312f0。入口检查context、参数名、图像描述和第五实参均非空；从描述+0取像素指针、+8取uint长度。0x180131377–1392汇编把第五入口实参转为后续调用第七参数（调用前[RSP+0x30]），长度放[RSP+0x20]，计算的flag放[RSP+0x28]，随后调用0x122a40。它不是只有伪码显示的四参调用；NB03的额外共享名在这个静态接收候选中未被丢弃。',
 '调用侧临时描述 → ModuleFrame入口 → 内部像素指针/长度/flag/共享名；没有因描述展开就复制像素。',
 'flag按描述+c字节等于1且全局开关为0计算；不据该flag名称或位置推断接收方完整所有权。',
 '入口非空检查不能证明指针有效或像素租约仍在；未运行厂商DLL或确认实际进程DLL解析。',
 [ref('1801312f0','  if ((((param_1',15),ref('1801312f0','180131312',13,true),ref('1801312f0','180131377',9,true)],['两DLL导入/导出名和ABI建立静态候选关系；原运行实例的装载解析、同帧/同计数及SDK接收对象仍未闭合。']);
 add('MF02','原内部0x122a40','条件取共享映射指针，失败回到原像素指针再提交',
 '0x122a40读取context内模块标识及关联对象，模块标识-1不进入有效提交。模块条件函数0x108b20返回非0时，调用0xeee30，传模块标识、参数名、索引、原指针、长度、共享名及返回指针槽；成功可使用输出映射指针，非0错误则回到原像素指针并按日志门记录。0xeee30所选体在已有名称/容量匹配时使用其映射字段+0x80，否则经0xeff60建立记录并替换/追加缓存项。之后0x122a40将实际选中的指针、长度、共享名继续交0xc99f0→模块对象0xf49a0，不能把它画成无条件像素复制。',
 '原指针与接收端共享映射指针是两个地址来源；同一共享映射不意味着虚拟地址相等。',
 '映射记录按模块/参数/索引等上下文持有，命中还核名称和容量；所选记录结构与图像工具FIFO不是同一个容器。',
 '本切片没有闭合映射记录析构、全部回调和最后读者；共享名匹配不替代完整租约证明。',
 [ref('180122a40','      uVar5 = FUN_18008e5d0',27),ref('180122a40','      pcStack_98 = param_7;',8),ref('1800eee30','    if (uVar15 < uStack_100)',9),ref('1800c99f0','      uVar4 = FUN_1800f49a0',2)],['映射建立callee0xeff60、条件函数全部含义、注册表销毁和SDK回调接收的同实例关系仍未闭合；不宣称端到端零复制。']);
 add('MF03','原模块图像结果0xf49a0→0xf41e0','先形成长度/地址/共享名文本，再按索引存入结果槽',
 '0xf49a0在指针、长度和共享名门后取得参数结果对象；对模块对象+0xf0互斥量加锁，在128字节局部缓冲中按原件格式串%d_%p_%s格式化长度、像素地址和共享名，然后调用0xf41e0。格式串从当前原PE RVA0x226ab8读取原字节，0x1800f4b06–4b29汇编确认变参为长度、指针和共享名。0xf41e0拒绝负索引；索引小于当前槽数时写已有字符串槽，否则取得追加槽再写文本。这里存的是图像描述文本，不是把整张像素写入字符串。',
 '图像像素缓冲与结果槽中的长度/指针/共享名文本分开；文本中包含地址不自动延长像素寿命。',
 '所选结果写入受模块锁包围；不把这把锁推成生产者、查询、回调、显示与像素释放的全局屏障。',
 '局部文本退出与结果槽持有、共享映射退役各有边界；SDK如何把此内部槽转换成typed载荷仍需下一段实现证据。',
 [ref('1800f49a0','        lVar3 = param_1 + 0xf0;',10),ref('1800f49a0','        pcStack_100 = param_8;',6),ref('1800f49a0','1800f4b06',8,true),ref('1800f41e0','  if (param_2 < 0)',33),{...source(dir+'protocol.json','Original image descriptor format','"format": {',6),binaryIdentity:layout.sha256,evidenceType:'direct-original-PE-bytes'}],['格式化失败/截断、槽追加helper全部行为、执行计数头写入和SDK输出转换尚未闭合；未因协议相似宣称原生到托管已经端到端验证。']);
 add('MF04','原导出与内部函数返回值','按汇编保留错误码，不按伪码void解释成无返回',
 '本次Ghidra把若干带安全cookie尾调用的函数显示成void；SetImageEx汇编却在内部调用后把EAX保存到R14D，尾部恢复EAX，非法入口分支置0xe0000001，再调用cookie校验。已恢复的0x1e8810在正常返回分支只操作RCX/标志，不改EAX。0xc99f0也显式返回错误码，模块查不到返回0xe0000003。故不能依据void伪码丢失错误传播；本条不补异常/fast-fail返回的假值。',
 '返回码是执行合同的另一条数据；它与像素指针、描述字符串和回调载荷分别追踪。',
 '所选导出SetImage要求入口索引为0，SetImageEx入口没有这条等0门；后续结果槽仍有自己的负索引拒绝。',
 '错误码返回不证明之前已完成的映射缓存或结果写入自动回滚；未取得全局失败清理合同。',
 [ref('1801312f0','180131415',16,true),ref('1801e8810','1801e8810',8,true),ref('1800c99f0','    if (lStack_20 == 0)',10),ref('180131190','  if ((((param_1',8)],['正常返回路径的EAX已核；异常、cookie失败、日志异常和完整回滚仍未验证。']);
 const names=fs.readdirSync(path.join(root,dir)).filter(n=>/\.c$|\.asm\.txt$|\.tsv$/.test(n)),files=[dir+'invocation.json',dir+'pe-layout.json',dir+'protocol.json','tests/ArchitectureRecovery/native-pe-inspection.cjs','tests/ArchitectureRecovery/Recover-ModuleFrameImage.ps1','tests/ArchitectureRecovery/Recover-ModuleFrameImageDetails.ps1','tests/NativeEvidence/VmNativeCapability.java',...names.map(n=>dir+n)];
 const provenance={schemaVersion:1,original:{path:layout.path,sha256:layout.sha256},files:files.map(p=>({path:p,sha256:sha(read(p))})),selectedFunctions:14,initialWholeAnalysisTimedOut:true,selectedCaptureExitCode:0,wholeBinaryAnalysisComplete:false,originalRuntimeExecuted:false,deviceCalled:false,runtimeDllResolutionVerified:false,sourceProvider:'offline Ghidra CLI, not REA evidence',fullOriginalChainClosed:false};fs.writeFileSync(path.join(out,'moduleframe-image.provenance.json'),JSON.stringify(provenance,null,2)+'\n');return{facts,unknowns:['内部结果槽→SDKtyped结构、计数写入与全部映射/像素最后读者仍未闭合。'],provenance};
};
