const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
module.exports=function(){
 const dir='docs/architecture-recovery/sdk-image-publication.resources/',read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),layout=JSON.parse(read(dir+'pe-layout.json')),b=read(layout.path);
 if(sha(b)!==layout.sha256)throw Error('Native SDK differs');const at=rva=>{const s=layout.sections.find(s=>rva>=s.rva&&rva<s.rva+s.rawSize);if(!s)throw Error('RVA');return s.rawOffset+rva-s.rva;},str=r=>{const o=at(r);return b.toString('ascii',o,b.indexOf(0,o));};
 if(str(0xaac794)!=='_'||str(0xad2c30)!=='%p')throw Error('Image token format differs');const target9=b.readUInt32LE(at(0x6af2ec)+9*4);if(target9!==0x6af272)throw Error('History release selector differs');
 const protocol={original:{path:layout.path,sha256:layout.sha256},selectedExports:layout.exports.filter(e=>['IMVS_RegisterOutputPlatformInfoCallBack','IMVS_QueryModuleHistoryResult','IMVS_ReleaseDynamicMemory'].includes(e.name)),imageTokenSeparator:{rva:'0xaac794',text:str(0xaac794)},pointerFormat:{rva:'0xad2c30',text:str(0xad2c30)},historyRelease:{selector:9,tableRva:'0x6af2ec',targetRva:'0x6af272',rawBytes:b.subarray(at(0x6af2ec)+36,at(0x6af2ec)+40).toString('hex')},nativeOwnershipWordOffset:276,publicMemoryModeOffset:272,wholeBinaryAnalyzed:false,originalRuntimeExecuted:false};fs.writeFileSync(path.join(root,dir,'protocol.json'),JSON.stringify(protocol,null,2)+'\n');
 const ref=(stem,a,n=1,asm=false)=>({...source(dir+stem+(asm?'.asm.txt':'.c'),a,asm?a+'\t':a,n),binaryIdentity:layout.sha256,evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode'}),facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,from,to,behavior,representation,cache,release,refs,unknowns,classification:'source-confirmed',stages:['results','display','retirement'],modes:['all']});
 add('SN01','原无后缀SDK回调注册导出','通过句柄检查后保存callback与user两份字段',
 '实际托管注册的无后缀IMVS_RegisterOutputPlatformInfoCallBack在本次原x64 DLL导出为RVA0x645630；它验证句柄注册记录及关联记录，再交0x6632a0。该体在对象+0x388为0，且旧callback为空或新callback为空时，将callback/user写到对象+0x58/+0x60，也写到关联对象+0x50/+0x58，并更新+0xd238开关。已有非空callback时再注册非空callback走错误0xe0000007；这不是任意覆盖。',
 '委托函数指针与user指针由原生对象字段持有；托管静态委托根另由WL01记录。',
 '注册状态属于原生对象/关联对象，不是某帧图像缓存。',
 '允许传空callback不等于在飞回调排空；没有据这些字段写入推出全局shutdown保证。',
 [ref('180645630','      iVar3 = FUN_1806632a0',8),ref('1806632a0','  if (*(longlong *)(param_1 + 0x388)',28)],['实际桌面DLL装载及原生异步调用者/注销排空仍未执行核验。']);
 add('SN02','原历史查询→0x69df50→0x6b9950','构造结果记录并写入header执行计数字段',
 'IMVS_QueryModuleHistoryResult通过句柄门后转0x69df50；该路径按0xe8字节构造每模块记录，并逐项调用0x6b9950。转换体把中间字段uStack_68高32位写piVar32[0x2d]，即结果记录偏移0xb4=180；原汇编0x1806b9cdf也明确写[RDI+0xb4]。这与WL03实取的x64托管nExecuteCount偏移一致，定位了结果header的一处原生写入；它没有在这里证明图像映射头+80也同时写入相同值。',
 '模块header执行计数、图像chunk字段与映射头计数是不同存储位置。',
 '记录数组/每参数容器分别分配；字段布局一致不证明回调与主动查询取自同一运行实例。',
 'header发布与动态查询容器释放分别追踪，不能以header已填充推断像素已复制。',
 [ref('18064c650','        uVar5 = FUN_18069df50',4),ref('18069df50','      iVar11 = FUN_1806b9950',4),ref('1806b9950','  piVar32[2] =',5),ref('1806b9950','1806b9cd8',4,true)],['中间执行计数的业务生成、与映射头的同帧一致性、回调路径的全部版本分支仍待核。']);
 add('SN03','原type4图像字符串→288字节chunk','长度/指针/共享名、公开模式和内部释放标志分开写',
 '0x6b9950所选type4分支设置参数type=4，在图像数组按0x120=288字节分配元素。有效转换分支将描述按原件分隔符下划线拆分，至少两个token才继续：长度经atoi，第二项按原%p格式解析像素地址，第三项存在时复制到元素+0xc的260字节名称域；写长度+8、公开模式+0x110为2、内部字+0x114为1。所选无有效数据分支则置像素/长度为0、模式为1、内部字为0。模式与内部字并不是同一个字段；所选分支没有把整张图像复制进这个描述元素。',
 '288字节元素是图像载荷描述；pData、nLen、strValue及nUseRealData与WL02的元数据布局分开对应。',
 '描述指向的原像素可能来自共享映射，描述数组自身是另外一次分配；不能将二者寿命混用。',
 '0x114在托管布局属于保留区，但原生释放体确实读取它；不能只凭nUseRealData模式值判断最终free。',
 [ref('1806b9950','                auVar9._8_8_ = 0;',25),ref('1806b9950','1806ba9b1',8,true),ref('1806b9950','                  *(undefined4 *)(lVar21 + 8 + *(longlong *)(puVar36 + 0x2a)) = uVar16;',28),ref('1806b9950','1806baaa5',5,true),ref('1806b9950','1806bab57',3,true),{...source(dir+'protocol.json','Image descriptor protocol and distinct fields','"imageTokenSeparator": {',17),binaryIdentity:layout.sha256,evidenceType:'direct-original-PE-bytes'}],['其它类型/映射模式及分割/数值转换的全部错误语义没有由本切片泛化；中间描述来源到ModuleFrame同一次写入仍未闭合。']);
 add('SN04','原动态释放selector9','图像数组与像素指针按内部字分开清理',
 '原IMVS_RELEASE_MEMORY_TYPE中历史结果列表为值9；本次直接读取原native跳转表9项得到0x6af272。该片段先MOV RDX,RSI再call0x6c1390，其中RSI由分发入口R8保存，证实传入查询根指针；返回后另delete根。0x6c1390按每模块0xe8、每参数0xd0、每图像chunk0x120遍历：pImage中的像素指针非0且元素+0x114为0才调用delete[]，随后清指针；元素数组本身仍另delete[]。因此释放查询描述数组，并不无条件释放每个图像像素，更不等于关闭所有映射与显示读者。',
 '根容器、模块记录、参数数组、chunk数组和pData像素分别有释放门；公开模式字段与内部释放字独立。',
 '该API清调用者查询容器，不意味着原SDK内部cache、托管副本或显示位图清空。',
 '内部字1跳过所选pData删除并非证明最后读者安全；根释放后外部像素的真正owner/复用仍须继续追踪。',
 [ref('1806af090','1806af09c',3,true),ref('1806af272','1806af272',5,true),ref('1806c1390','                if (*(longlong *)(uVar8 + 0xa8',28),ref('1806c1390','1806c14c8',6,true)],['其它selector/异常/部分构造失败的全清理未全部关闭；所选private释放字不能反推全部模式下像素owner。']);
 const files=[dir+'invocation.json',dir+'pe-layout.json',dir+'protocol.json','tests/ArchitectureRecovery/Recover-SdkImagePublication.ps1','tests/ArchitectureRecovery/Recover-SdkImagePublicationDetails.ps1','tests/NativeEvidence/VmNativeCapability.java',...fs.readdirSync(path.join(root,dir)).filter(n=>/\.c$|\.asm\.txt$|\.tsv$/.test(n)).map(n=>dir+n)];const provenance={schemaVersion:1,original:{path:layout.path,sha256:layout.sha256},files:files.map(p=>({path:p,sha256:sha(read(p))})),selectedProcedures:9,selectedCaseFragments:1,initialWholeAnalysisTimedOut:true,selectedCaptureExitCode:0,wholeBinaryAnalysisComplete:false,originalRuntimeExecuted:false,deviceCalled:false,metadataLoadedReflectionOnly:false,sourceProvider:'offline Ghidra CLI; not REA output',fullOriginalChainClosed:false};fs.writeFileSync(path.join(out,'sdk-image-publication.provenance.json'),JSON.stringify(provenance,null,2)+'\n');return{facts,unknowns:['所选header写入/描述构造/条件释放已证，映射头同帧发布、回调全部转发和最后读者仍未闭合。'],provenance};
};
