'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert');
const {root,out,source}=require('./evidence-view.cjs');
const dir='docs/architecture-recovery/variable-moduleframe-storage.resources/',hash=b=>crypto.createHash('sha256').update(b).digest('hex'),read=p=>fs.readFileSync(path.resolve(root,p));
module.exports=function(){
 const pe=JSON.parse(read(dir+'pe-layout.json')),fresh=require('./native-pe-inspection.cjs')(pe.path);assert.deepStrictEqual(fresh,pe);assert.equal(pe.sha256,'45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b');assert.equal(pe.loadedOrExecuted,false);
 const expectedExports={VM_M_DelGlobalVar:'0x135660',VM_M_BatchSetGlobalVarInt:'0x134710',VM_M_BatchSetGlobalVarFloat:'0x134880',VM_M_BatchSetGlobalVarString:'0x1349f0',VM_M_SetGlobalVarLockStatus:'0x148130'};
 for(const [name,rva]of Object.entries(expectedExports))assert(pe.exports.some(e=>e.name===name&&e.rva===rva&&!e.forwarder));
 for(const [iat,name]of [['0x180204580','??3@YAXPEAX@Z'],['0x1802042f8','_Mtx_lock'],['0x180204300','_Mtx_unlock'],['0x180204850','memcpy']])assert(pe.imports.some(i=>i.iatVa===iat&&i.name===name));
 const captures=['exports','internal-correct','batch-exports','batch-storage','typed-retirement','leaf-copies','containers','retire-leaves'],completions=captures.map(c=>{const p=dir+'completion-'+c+'.json',v=JSON.parse(read(p).toString('utf8').replace(/^\ufeff/,''));assert.equal(v.exitCode,0);assert.equal(v.liveProcessHandle,null);assert.equal(v.vendorExecuted,false);return{path:p,...v};});
 const tsv=fs.readdirSync(path.join(root,dir)).filter(n=>/^functions-.*\.tsv$/.test(n)),methods=tsv.flatMap(n=>read(dir+n).toString('utf8').trim().split(/\r?\n/).slice(1).map(line=>{const [address,name,bodyBytes,decompiled,callees]=line.split('\t');assert.equal(decompiled,'true','incomplete selected body '+address);assert(Number(bodyBytes)>0);assert(read(dir+address+'.c').length>0);assert(read(dir+address+'.asm.txt').length>0);return{address,name,bodyBytes:Number(bodyBytes),decompiled:true,callees:(callees||'').split(',')};}));assert.equal(methods.length,31);assert.equal(new Set(methods.map(m=>m.address)).size,31);
 const ref=(stem,anchor,count=1,asm=false)=>({...source(dir+stem+(asm?'.asm.txt':'.c'),stem,asm?anchor+'\t':anchor,count),binaryIdentity:'SHA256 '+pe.sha256,artifact:pe.path,evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode',provider:'existing owned offline Ghidra CLI; not REA retained evidence'});
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'source-confirmed',stages:['variable','transfer','results','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('VZ01','原ModuleFrame的Batch int/float/string导出','原指针/数量/第五字节进入各内部接收者',
 '原PE导出表分别确认BatchInt=0x134710、BatchFloat=0x134880、BatchString=0x1349f0，三者不是独立SetGlobalVarInt/Float/String导出。三wrapper汇编保存原RCX/RDX/R8/R9，检查句柄、名称和payload非null；调用内部126f10/127050/127340时，RDX=原句柄指针、R8=原名称、R9=原payload，数量写栈+0x20，原第五字节从当前栈+0xc0读出并写栈+0x28。内部接收体再从句柄首int取得键；不能因伪码callee少显示参数就说原指针或隐藏字节已丢弃。',
 '导出入参、wrapper重排后的内部入参、后续typed存储分开；此边传递pointer/count而未证明wrapper复制payload。',
 'Batch内部在所选后续体接入模块键下的值记录；独立Set导出虽已取体，不冒充GlobalVariableModule typed链实际使用的Batch入口。',
 'wrapper末尾清日志用字符串不等于释放调用者payload；上游临时描述/数值块与下游记录分别核对。',
 [ref('180134710','180134728',30,true),ref('180134880','180134898',30,true),ref('1801349f0','180134a08',30,true),ref('180126f10','  if (((param_2',30),ref('180127050','      FUN_180099550',5),ref('180127340','      FUN_1800997c0',5)],
 ['实际从GlobalVariableModule typed调用进入这些导出的运行实例未执行；已有静态IAT与caller链和本导出体分别保存。','第五字节的具体业务命名不由ABI位置推定；后续非零分支可据所取体说明，但不命名为通用copy或ownership标志。']);
 add('VZ02','Batch内部992e0/99550的附加分支与主仓读取','非零字节分支逐值记录，随后取共享模块记录',
 'int与float接收体均仅把count==0直接判错；字节参数非零时，按上下文键查另一记录，更新标记/名称，然后仅在count>0时循环从原payload按4字节读取并写各自数值列表。之后仍执行公共检查并进入全局共享模块记录查找：记录为空返回0xe0000003，否则交f4d10/f4dc0处理。这里附加记录与主typed存储是两条持有关系，且负数量不等于被count==0门拒绝。',
 'int附加列表字段+0x68/+0x6c/+0x70、float字段+0x40/+0x44/+0x48；实际值逐个写入内部数组，不是仅保存原pointer。',
 '辅助字段和主共享记录可能同时保留数值；未取得全部创建/复用/清理分支，不推定它们是一份缓存。',
 '局部共享记录强引用减计数只撤销本调用持有；记录为空或后续失败不由此证明辅助字段已回滚。',
 [ref('1800992e0','  if (param_5 == 0)',57),ref('180099550','    if (param_6',43),ref('1800992e0','    FUN_1800cae00',48)],
 ['负数量、附加记录与主记录一致性及0x09a7e0完整准入合同没有运行验证；不从本片段推通用线程安全或失败原子性。']);
 add('VZ03','f4d10/f4dc0的数值主写入','输入每个4字节值复制进所取值记录的内部数组',
 'int写入先经f74b0取得共享值对象，空则返回0xe0000003；非空先调用其虚表+8槽，再对正count循环从原pointer+i*4读取4字节并交fc50。fc50经准备函数后取对象+0x10中的载荷，必要时扩容，并向其数组base+8+当前数目*4写入该值、增加计数。float的f4dc0对应fd30，写入另一组+0x40/+0x44/+0x48数组字段。这个层级已有实际标量复制，不能继续说数值始终只是原pointer转发；也不能把虚表+8未解体的行为擅称完整旧值清空。',
 '原数值buffer→逐个4字节值→内部动态数组；输入临时块寿命与已复制记录分开，float/int按实际4字节载体，不外推double。',
 '取得的共享值对象和模块仓库可继续持有复制后的数组；本调用只减自己的控制块计数。',
 '普通退出含共享控制块强/弱计数递减与虚调用；实际载荷析构槽未在此解析，不能宣称输出消费者已经释放或buffer容量立即归还。',
 [ref('1800f4d10','  FUN_1800f74b0',39),ref('1800f4dc0','  FUN_1800f7650',25),ref('18000fc50','  FUN_18000fb90',10),ref('18000fd30','  FUN_18000fb90',10),ref('1800f4d10','1800f4d53',22,true)],
 ['值对象创建、虚表+8目标、扩容失败和所有其它读者的退役仍需独立追体；本轮未运行数值或并发验证。']);
 add('VZ04','BatchString接收体997c0','描述步长16字节→局部字符串→内部字符串记录副本',
 '接收体在正count循环中每次读取16字节描述的首pointer，再按尾零扫描构造局部字符串；还存在字节参数非零时向辅助列表复制字符串的分支。主分支按模块/名称取得字符串值对象后，汇编明确把当前索引和局部字符串作为参数交00ffe0。该体按索引选择已有或新增的内部字符串，再调用006470复制字符；006470在非同对象分支按实际字符串长度调用原IAT memcpy、更新长度并写尾零。00ffe0随后对临时字符串大容量块调用operator delete并重置它。此处释放的是临时字符串存储，不能当作调用者STRING_UNIT数组或其原pointer的释放。',
 '16字节描述只携带pointer/metadata；所选receiver实际读取首pointer并按C字符串扫描，随后已有复制到内部字符串的成员证据。',
 '内部字符串与临时字符串、上游描述数组各自持有；源码不支持把所有同名string当同一个缓冲。',
 '临时字符串delete已见原IAT；内部目的字符串未来退役与嵌套消费者仍未知，不能从局部清理证明最后读者结束。',
 [ref('1800997c0','    if (param_6',35),ref('1800997c0','    iVar6 = 0;',34),ref('1800997c0','180099a59',9,true),ref('18000ffe0','  if (param_2 < 0)',32),ref('180006470','    if (uVar3 != 0)',14)],
 ['STRING_UNIT长度/容量字段没有在这些读取分支中作为统一边界消费；不能把它们推成全部字符串均带长度安全读取。','空指针、嵌入尾零、分配失败和字符串剩余分支没有原机运行验证。']);
 add('VZ05','VM_M_DelGlobalVar → 127e60','依次按七个类型值查名称，再交带锁的共享模块仓',
 'Del导出伪码显示void/少参数，但汇编明确把原句柄和名称重排到RDX/R8交127e60，并保存EAX作为返回值。内部读取句柄首int，非-1时对同名依次调用cb820，类型整数次序1/2/7/3/4/5/6。cb820对仓库+0x58执行实际_Mtx_lock，取得模块共享记录，非空才交f82f0；f82f0把七个类型映射到记录内不同map偏移，再调用f9aa0按name求范围、交6eb00擦除。普通返回的0并不包含逐个erase结果聚合证明，map不存在也不等于所有历史结果引用消失。',
 '删除分为全局变量模块名称map、ModuleFrame模块仓和七个typed map；与托管VmIO解绑/显示缓存不是一处清空。',
 'cb820在查找期间局部持共享模块记录，随后减局部强/弱计数并_Mtx_unlock；其它持有者仍可继续存在。',
 'erase的实际节点/控制块退役见VZ06；不能在本层把VM_M_DelGlobalVar返回0解释为所有数值/字符串payload已free。',
 [ref('180135660','1801356b8',7,true),ref('180135660','18013578e',11,true),ref('180127e60','      FUN_1800cb820(uVar3,iVar1,1,',4),ref('180127e60','      FUN_1800cb820(uVar3,iVar1,2,',4),ref('180127e60','      FUN_1800cb820(uVar3,iVar1,7,',4),ref('180127e60','      FUN_1800cb820(uVar3,iVar1,3,',4),ref('180127e60','      FUN_1800cb820(uVar3,iVar1,4,',4),ref('180127e60','      FUN_1800cb820(uVar3,iVar1,5,',4),ref('180127e60','      FUN_1800cb820(uVar3,iVar1,6,',7),ref('1800cb820','  iVar4 = _Mtx_lock',38),ref('1800f82f0','  switch(param_2)',31)],
 ['句柄有效性、所有map语义、删除后的已发布结果和native绑定最后读者未运行确认；正常控制流mutex操作不是所有异常分支排空证明。']);
 add('VZ06','typed map擦除6eb00→6f290/6e570','释放节点名称/节点内存；载荷经控制块条件虚析构',
 '范围erase在全范围时交6e570递归处理并复位根/计数，局部范围逐节点交6f290。已恢复两叶实际体与汇编：节点+0x48的共享控制块强计数递减，只有降为0才调用其虚表首槽，再递减弱计数，弱计数为0再调用+8槽。节点名称容量>=16时delete其名称堆块，之后重置字符串字段并operator delete节点本身；局部erase还递减容器数目。因此节点与名称的真实释放已可证明，而payload虚析构的动态接收者未定位，不能把该条件虚调用等同于每份数值/字符串buffer都已销毁。',
 'map节点、名称堆块、共享控制块、所指typed载荷四种对象分开。',
 '从此map移除仅撤销此节点的一份shared所有权；其它缓存、结果或订阅持有仍可能阻止最终析构。',
 '明确free/delete证据仅限本体直接释放的节点和名称；payload最后读者、虚析构目标和内部动态数组的真实free仍保留未知。',
 [ref('18006eb00','  if ((param_3',13),ref('18006eb00','    FUN_18006f290',7),ref('18006f290','  plVar8 = (longlong *)param_3[9];',35),ref('18006e570','    plVar4 = (longlong *)param_2[9];',33),ref('18006f290','18006f5ea',20,true),ref('18006e570','18006e5b4',19,true)],
 ['未恢复这些shared控制块在每个typed map中的具体虚表实例与payload析构体；不宣称已证明全局无悬挂引用、无泄漏或所有存储已回收。']);
 add('VZ07','VM_M_SetGlobalVarLockStatus → 9a140','按键查状态记录，再调用真实mutex与标志写入',
 '导出正常分支把两整数参数与原字节传入静态对象的9a140。该体先读对象+0x10，汇编明确TEST该字段后SETZ临时字节，再与请求字节作TEST决定早返0；不能照伪码含糊的位运算优先级另造布尔公式。成功查到键后，非零请求还需对象启用字节与记录首字节通过门，才_Mtx_lock记录+8并写记录+1=1。零请求在记录+1非零时才做额外准备/清理调用、写+1=0后_Mtx_unlock；重复零/不存在记录另有返回1分支。锁/解锁确为原MSVCP120导入，但只有这些分支被静态核实，不把导出名当全局同步或所有异常必解锁保证。',
 '静态管理器状态、按整数键的记录、记录首字节、已锁标志及mutex指针分别读取；参数名称不由函数名填补。',
 '按键查找的记录与typed共享结果仓不是已证同一对象；辅助数值/字符串暂存也不能直接当这把mutex拥有的完整事务。',
 '请求0的部分分支有清理调用及unlock；未取得被调清理函数的全部语义，不称它释放所有变量值。',
 [ref('180148130','1801481b8',7,true),ref('18009a140','18009a14c',14,true),ref('18009a140','  else if (param_4 == 0)',48),ref('18009a140','18009a1bc',8,true),ref('18009a140','18009a258',10,true)],
 ['mutex所属线程、重入/重复请求、异常处理以及9a300/69c0对暂存的具体语义未闭合；未运行并发或原始设备。']);
 const files=['tests/ArchitectureRecovery/Recover-VariableModuleFrameStorage.ps1','tests/ArchitectureRecovery/native-pe-inspection.cjs','tests/NativeEvidence/VmNativeCapability.java',...fs.readdirSync(path.join(root,dir)).filter(n=>/\.(?:c|json|tsv)$|\.asm\.txt$/.test(n)).map(n=>dir+n)].map(p=>({path:p,sha256:hash(read(p)),byteLength:read(p).length}));
 const result={schemaVersion:1,complete:false,facts,unknowns:[{id:'VZU01',target:'typed shared-control-block dynamic destructors',question:'Map节点/名称delete已见，但payload虚析构目标与数组最终free、已发布消费者最后读者仍未知。'},{id:'VZU02',target:'lock-state record and scratch-path semantics',question:'mutex调用已证，不证明整个更新/删除事务原子、重入或异常必排空。'}],provenance:{original:{path:pe.path,sha256:pe.sha256,byteLength:pe.byteLength},exports:Object.entries(expectedExports).map(([name])=>pe.exports.find(e=>e.name===name)),methods,selectedProcedures:methods.length,wholeBinaryAnalysisComplete:false,sourceProvider:'existing owned offline Ghidra project -process -noanalysis; not REA retained output',files,completions,liveProcessHandle:null,failedSelection:{path:dir+'invocation-internal.json',admittedAsEvidence:false,terminalExitCode:0,reason:'Comma-joined seed passed as one address; script reported null address. Corrected capture is internal-correct; terminal zero alone is not decompilation success.'},loggingWarning:'Ghidra logging XML path warning was observed; only nonempty selected C/assembly files and decompiled=true manifest rows are admitted, not terminal status alone.',originalRuntimeExecuted:false,targetCodeExecuted:false,deviceCalled:false,anotherWorktreeRead:false,productModified:false,fullOriginalChainClosed:false,implementationCredit:0}};
 fs.writeFileSync(path.join(out,'original-variable-moduleframe.json'),JSON.stringify(result,null,2)+'\n');return result;
};
if(require.main===module){const r=module.exports();console.log(JSON.stringify({facts:r.facts.length,refs:r.facts.reduce((n,f)=>n+f.refs.length,0),bodies:r.provenance.selectedProcedures,cliTerminal:r.provenance.completions.map(c=>c.exitCode),liveProcessHandle:null}));}
