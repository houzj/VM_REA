// Finite original typed-value bridge. Static read-only evidence; no vendor/factory execution.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),{source}=require('./evidence-view.cjs');
const root=path.resolve(__dirname,'../..'),dir='docs/architecture-recovery/variable-native-storage.resources/',prior='docs/architecture-recovery/variable-native-receiver.resources/pe-layout.json',sha=b=>crypto.createHash('sha256').update(b).digest('hex'),read=p=>fs.readFileSync(path.resolve(root,p));
module.exports=function(){
 const pe=JSON.parse(read(prior)),bytes=read(pe.path);if(sha(bytes)!=='20cd716e9471a2687c0cc777548dedb469bcd0356b9da90aa97c42e301e16ff0'||sha(bytes)!==pe.sha256)throw Error('GlobalVariable original differs');
 const needed={'0x180055638':'VM_M_BatchSetGlobalVarInt','0x1800556c8':'VM_M_BatchSetGlobalVarFloat','0x1800556a0':'VM_M_BatchSetGlobalVarString','0x180055680':'VM_M_SetBinayData','0x180055540':'??_V@YAXPEAX@Z'};
 for(const [iat,name]of Object.entries(needed))if(pe.imports.find(x=>x.iatVa===iat)?.name!==name)throw Error('Selected prior PE import identity differs');
 const completionPath=dir+'completion-typed.json',completion=JSON.parse(read(completionPath).toString().replace(/^\ufeff/,''));if(completion.exitCode!==0)throw Error('Owned typed capture not complete');
 const mfPath='docs/architecture-recovery/moduleframe-image.resources/pe-layout.json',mf=JSON.parse(read(mfPath));if(sha(read(mf.path))!==mf.sha256)throw Error('ModuleFrame export identity differs');
 const exports=mf.exports.filter(e=>['VM_M_BatchSetGlobalVarInt','VM_M_BatchSetGlobalVarFloat','VM_M_BatchSetGlobalVarString','VM_M_DelGlobalVar'].includes(e.name));
 const ref=(stem,a,n=1,asm=false)=>({...source(dir+stem+(asm?'.asm.txt':'.c'),stem,asm?a+'\t':a,n),evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode',binaryIdentity:pe.sha256,provider:'offline existing owned Ghidra project -noanalysis; not REA retained evidence'});
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'source-confirmed',stages:['variable','transfer','results','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('VS01','原typed int@0x295b0 / float@0x29eb0','带第五栈字节的ModuleFrame batch调用',
 '两个原体对名称或payload指针为0返回0xe0000001，否则先调用ModuleFrame.VM_M_BatchSetGlobalVarInt/Float。原汇编明确RCX=context+8对象、RDX=变量名、R8=原payload指针、R9D=数量，并从typed入口第五字节实参写到调用前stack+0x20；不能因伪码只显示四个参数就称第五标志未传。IAT分别0x180055638和0x1800556c8绑定同名ModuleFrame导出。所选wrapper没有在batch调用前为数值数组分配/逐字节复制新的本地存储，传的是输入数组地址。',
 'context内部对象/名称/原数值pointer/数量/额外字节是五项ABI载荷；额外字节业务意义未从名称猜测。',
 '这里不是最终数值heap存储体，只是实际typed入口到ModuleFrame的下一跳；不把pointer转发写成深拷贝已完成。',
 'wrapper不释放传入数值数组；实际复制、旧值替换和free必须进入ModuleFrame batch导出再核。',
 [ref('1800295b0','int FUN_1800295b0',5),ref('1800295b0','180029611',13,true),ref('180029eb0','180029f11',13,true)],['ModuleFrame.VM_M_BatchSetGlobalVarInt RVA0x134710及Float RVA0x134880为已绑定下一种子，本轮未取它们原体，底层存储所有权仍未知。']);
 add('VS02','原typed string@0x2a7b0','0x60字节输入描述 → 0x10字节STRING_UNIT临时数组',
 '原体按数量×0x10分配临时数组；逐项从输入记录i×0x60的+0 pointer、+8长度、+0xc容量字段，写到输出i×0x10的同三字段。复制的是pointer/length/size描述，没有在这个循环复制指向的字符串字节。调用ModuleFrame.VM_M_BatchSetGlobalVarString时R8传临时数组、R9D传数量，另传第五栈字节；保存返回码后，无论返回码0还是非0，正常控制流立即对该临时数组delete[]，随后才判断结果/继续联动。IAT0x1800556a0绑定batch string，0x180055540绑定MSVCR120.dll::??_V@YAXPEAX@Z。',
 '调用者ByteDataArray大描述 → 本地紧凑描述副本；两个描述中的pData借用同一个字符串内容地址。',
 '临时紧凑数组不是长期字符串cache，也不是所有字符串内容的deep copy。',
 '本体只delete[]紧凑描述数组，不逐项free调用者字符串pointer；异常跨callee时是否清理需异常片段，不能把正常返回路径当全异常保证。',
 [ref('18002a7b0','local_80 = (ulonglong)param_4;',40),ref('18002a7b0','18002a8ee',25,true),ref('18002a7b0','18002a96a',17,true)],['ModuleFrame.VM_M_BatchSetGlobalVarString RVA0x1349f0是否同步复制字符串内容、是否保留临时描述pointer、旧值free与失败回滚仍缺原体。']);
 add('VS03','原typed设置成功后的运行参数联动','只取第0个值，发送到记录中的linked模块/参数',
 '三个typed原体都仅在batch返回0、数量>0且typed入口第六字节为0时进入linked记录循环；记录中+0x43c是链接数量、+0x440按0x94字节步进，每项取目标模块ID及+0x14的参数名。int在调用格式helper0x1b450前，汇编实际从原payload第0项读EDX；伪码遗漏该实参，不能说无值输入。float同样读第0项到XMM1再调用0x1b650。字符串联动用原输入第0条描述的pData/nDataLen直接调用VM_M_SetBinayData，未按每个linked目标选择不同数组元素。',
 '完整数组先送batch存储入口，随后参数联动使用首项，是两条不同数据边；不能从数组nValueNum推断每个目标都接整数组。',
 'linked记录是另一个保留容器，当前新取值不自动删除它；wrapper临时格式化string退出后析构。',
 'VM_M_SetBinayData收到的数值格式化文本或原字符串内容pointer在调用点仅短期有效，下层是否复制/排队与最后读者仍未知。',
 [ref('1800295b0','if (((local_c8 == 0) && (0 < param_4))',45),ref('1800295b0','1800297bd',6,true),ref('180029eb0','18002a0c2',5,true),ref('18002a7b0','local_70 = 0;',15),ref('18002a7b0','18002ab97',9,true)],['第六字节精确控制是否联动，但其正式参数名/业务语义未取符号；不泛称订阅/copy开关。','ModuleFrame.VM_M_SetBinayData对这些linked参数文本的接收和租约仍未闭合。']);
 add('VS04','原typed返回码与后续链接异常门','batch结果与各linked更新结果分开保留',
 'int/float保存在local_c8的batch结果，string保存在local_c4；batch非0时直接返回该码。batch成功后的linked VM_M_SetBinayData错误保存到另一个local_a0/local_98，仅在日志门下记录，不覆盖最终batch返回值。链接参数名为空时记录后提前返回的也仍是原batch码，剩余链接不再处理。因此函数返回0不能证明每个linked运行参数都已更新；这一结论来自实际局部赋值和返回门，不能把日志存在当作错误向上传播。',
 '主存储调用结果与联动结果是两个结果域，不是统一事务。',
 '没有在这些分支看到失败时恢复旧数组或撤销已成功linked更新；真实下层存储回滚亦不能从本包装推出。',
 '原payload和旧值的free仍在下层未知；所选局部shared_ptr释放只退役包装持有的linked记录引用。',
 [ref('1800295b0','local_9c = local_c8;',4),ref('1800295b0','local_a0 = VM_M_SetBinayData',24),ref('1800295b0','  return local_c8;',2),ref('18002a7b0','local_98 = VM_M_SetBinayData',28),ref('18002a7b0','  return local_c4;',2)],['native最终存储/引用表一致性与跨线程执行顺序未运行验证；正常返回0仅能按所选返回码来源解释。']);
 const files=['tests/ArchitectureRecovery/Recover-GlobalVariableStorage.ps1','tests/NativeEvidence/VmNativeCapability.java',...fs.readdirSync(path.join(root,dir)).filter(n=>/\.(?:c|json|tsv)$|\.asm\.txt$/.test(n)).map(n=>dir+n)].map(p=>({path:p,sha256:sha(read(p)),byteLength:read(p).length}));
 return {schemaVersion:1,complete:false,facts,unknowns:[{id:'VSU01',target:'ModuleFrame batch typed final storage',question:'已取得GlobalVariable三个typed体，但实际值分配/替换/free在ModuleFrame.BatchSetGlobalVarInt/Float/String下层；本轮不能宣称已闭合。'},{id:'VSU02',target:'string descriptor and nested bytes lifetime',question:'紧凑描述在batch正常返回后立即delete[]，嵌套字节仍借用；callee同步copy还是保留pointer未知。'},{id:'VSU03',target:'linked propagation and old payload retirement',question:'first-element联动结果不覆盖batch返回码；下游文字payload复制/排队与旧值最后读者未证。'}],provenance:{original:{path:pe.path,sha256:pe.sha256,byteLength:bytes.length},priorPeImports:{path:prior,sha256:sha(read(prior)),selected:pe.imports.filter(i=>Object.keys(needed).includes(i.iatVa)),meaning:'Reused original import-table evidence bound to same freshly rehashed DLL; not new runtime module-resolution evidence.'},moduleFrameNextSeeds:{path:mf.path,sha256:mf.sha256,exportMetadataSource:mfPath,exports,bodyRecoveredInThisSlice:false},selectedProcedures:3,selectedCaseFragments:0,files,completion:{path:completionPath,...completion},sourceProvider:'offline owned Ghidra project -noanalysis; not REA retained evidence',wholeBinaryAnalysisComplete:false,originalRuntimeExecuted:false,targetCodeExecuted:false,deviceCalled:false,anotherWorktreeRead:false,implementationChanges:false,liveProcessHandle:null,fullOriginalChainClosed:false}};
};
if(require.main===module){const r=module.exports(),report='docs/architecture-recovery/original-variable-native-storage.json';fs.writeFileSync(path.join(root,report),JSON.stringify(r,null,2)+'\n');console.log(JSON.stringify({report,facts:r.facts.length,unknowns:r.unknowns.length,selectedProcedures:r.provenance.selectedProcedures,cliLive:false}));}
