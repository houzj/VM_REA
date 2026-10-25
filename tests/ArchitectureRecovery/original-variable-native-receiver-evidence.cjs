const fs=require('fs'),path=require('path'),crypto=require('crypto');
const root=path.resolve(__dirname,'../..'),dir='docs/architecture-recovery/variable-native-receiver.resources/';
const target='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Global/GlobalVariableModule/GlobalVariableModule.dll',expected='20cd716e9471a2687c0cc777548dedb469bcd0356b9da90aa97c42e301e16ff0';
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
function locate(){
 const b=fs.readFileSync(target);if(hash(b)!==expected)throw Error('Global variable DLL identity differs');
 const h=b.readUInt32LE(60),o=h+24,ns=b.readUInt16LE(h+6),ss=o+b.readUInt16LE(h+20),sections=[];
 for(let i=0;i<ns;i++){const p=ss+40*i;sections.push({name:b.toString('ascii',p,p+8).replace(/\0.*$/,''),rva:b.readUInt32LE(p+12),rawSize:b.readUInt32LE(p+16),rawOffset:b.readUInt32LE(p+20),flags:b.readUInt32LE(p+36)});}
 const off=r=>{const s=sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);if(!s)throw Error('RVA not file-backed');return s.rawOffset+r-s.rva;};
 const ranges=[],pRva=b.readUInt32LE(o+136),pSize=b.readUInt32LE(o+140);for(let p=off(pRva),end=p+pSize;p+12<=end;p+=12)ranges.push({start:b.readUInt32LE(p),end:b.readUInt32LE(p+4),unwind:b.readUInt32LE(p+8)});
 const strAt=p=>b.toString('ascii',p,b.indexOf(0,p)),imports=[];for(let p=off(b.readUInt32LE(o+120));b.readUInt32LE(p+12)!==0;p+=20){const lookup=b.readUInt32LE(p)||b.readUInt32LE(p+16),iat=b.readUInt32LE(p+16),module=strAt(off(b.readUInt32LE(p+12)));for(let n=0;;n++){const val=b.readBigUInt64LE(off(lookup)+8*n);if(!val)break;imports.push({module,name:val>>63n?'ordinal:'+Number(val&0xffffn):strAt(off(Number(val))+2),iatRva:'0x'+(iat+n*8).toString(16),iatVa:'0x'+(0x180000000n+BigInt(iat+n*8)).toString(16)});}}
 const cases=Array.from({length:10},(_,action)=>({action,tableRva:'0x25b58',targetRva:'0x'+b.readUInt32LE(off(0x25b58)+4*action).toString(16),rawBytes:b.subarray(off(0x25b58)+4*action,off(0x25b58)+4*action+4).toString('hex')}));
 const literals=[];for(const text of ['ModifyVar','DeleteVar','GlobalValueList','int','float','string']){let i=0;while((i=b.indexOf(Buffer.from(text+'\0'),i))>=0){const s=sections.find(s=>i>=s.rawOffset&&i<s.rawOffset+s.rawSize);if(s)literals.push({text,fileOffset:i,rva:s.rva+i-s.rawOffset,bytes:b.subarray(i,i+text.length+1).toString('hex')});i++;}}
 const candidates=[];for(const s of sections.filter(s=>s.flags&0x20000000)){for(let i=s.rawOffset;i+7<s.rawOffset+s.rawSize;i++){if((b[i]&0xf8)!==0x48||b[i+1]!==0x8d||(b[i+2]&0xc7)!==5)continue;const rva=s.rva+i-s.rawOffset,to=rva+7+b.readInt32LE(i+3),literal=literals.find(t=>t.rva===to);if(!literal)continue;const range=ranges.find(r=>rva>=r.start&&rva<r.end);candidates.push({literal:literal.text,instructionRva:rva,targetRva:to,rawBytes:b.subarray(i,i+7).toString('hex'),range,selection:'Raw RIP-relative LEA candidate; must confirm decoded instruction and function body before semantic claims.'});}}
 return {path:target,sha256:expected,byteLength:b.length,imageBase:'0x180000000',sections,ranges,literals,candidates,imports,cases,seeds:[...new Set(candidates.filter(c=>['ModifyVar','DeleteVar'].includes(c.literal)&&c.range).map(c=>'180'+c.range.start.toString(16).padStart(6,'0')))]};
}
module.exports=function(){
 const {source}=require('./evidence-view.cjs'),pe=locate(),read=p=>fs.readFileSync(path.join(root,p));
 const completed=['initial','details'].map(k=>{const p=dir+'completion-'+k+'.json',v=JSON.parse(read(p).toString().replace(/^\ufeff/,''));if(v.exitCode!==0)throw Error('Selected static capture did not exit successfully');return {path:p,...v};});
 if(pe.imports.find(i=>i.iatVa==='0x180055568')?.name!=='memcpy_s'||pe.imports.find(i=>i.iatVa==='0x1800556f0')?.name!=='VM_M_DelGlobalVar')throw Error('Original selected IAT imports differ');
 if(pe.cases[1].targetRva!=='0x245bd'||pe.cases[3].targetRva!=='0x249ea')throw Error('Original VAR_TYPE/VAR_VALUE jump table differs');
 const ref=(stem,a,n=1,asm=false)=>({...source(dir+stem+(asm?'.asm.txt':'.c'),stem,asm?a+'\t':a,n),binaryIdentity:expected,evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode',provider:'existing offline Ghidra CLI, not REA retained evidence'});
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'source-confirmed',stages:['variable','transfer','results','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('VN01','原GlobalVariableModule key分发体RVA0x1f8c0','ModifyVar载荷字段读取与动作跳表',
 '原PE字符串ModifyVar/DeleteVar的RIP-LEA候选落同一runtime范围0x1f8c0..0x25b80；本轮实际取得对应伪码和解码汇编后，确认其比较参数key并进入ModifyVar分支。入口汇编保存RCX=context、RDX=key、R8=payload、R9D=第四整数参数，分支把payload作为描述读取：名称+4、目标文本+0x204、动作+0、数量+0x304、数据pointer+0x308。动作跳表位于RVA0x25b58，原字节直接确认1→0x245bd、3→0x249ea，与托管VAR_TYPE=1/VAR_VALUE=3的描述枚举相对照。',
 '描述byte[]被native解释为带pointer的结构，不是自动深拷贝嵌套数据；只证这个实际分支，不虚构所有动作合法域。',
 '所选分支查context+0xf8的名称容器并取得共享记录；未在此证明全局/局部作用域的所有建表规则。',
 '入口判空与名称存在门不代表payload长度/指针范围已验证；第四参数对整个ModifyVar结构的充分长度检查本轮未证。',
 [ref('18001f8c0','18001f8c0',4,true),ref('18001f8c0','18002404f',17,true),ref('18001f8c0','180024081',8,true),ref('18001f8c0','1800242f3',8,true)],['原SDK/ModuleFrame如何选到此虚成员、该成员正式符号和实际进程DLL解析未闭合；不因key一致宣称端到端调用已证。']);
 add('VN02','原VAR_VALUE动作3','已有类型核验 → pBinaryValue/数量传入typed值分发',
 'case3从payload+0x304读数量，记录+0x448非0时将payload+0x308与数量直接交0x28250；另一分支核目标类型文本：payload+0x204非空且与记录+0x129类型不符时返回0xe000030f。通过门后调用0x28c30，汇编明确RDX=记录名称+0x29、R8=类型文本、R9=payload+0x308内的pointer，第五栈参数为数量；还在调用前stack+0x28写入字节1，不能按伪码五参数显示丢掉这个调用点事实。0x28c30原体按int/float/string分别向0x295b0/0x29eb0/0x2a7b0转发；本轮没取这些最终存储体。',
 '标量/字符串数组的payload地址确实进入native值接收链；这个转发不是值已完成深拷贝的证据。',
 '原记录已有类型与组合标志决定接收分支，不能将所有typed输入按相同存储处理。',
 '既未在所选case释放调用者pBinaryValue，也未证明callee已完成复制；托管finally回收临时块前的安全性仍需最终存储体。',
 [ref('1800249ea','180024a0b',18,true),ref('1800249ea','180024ad3',6,true),ref('1800249ea','180024b49',20,true),ref('180028c30','int FUN_180028c30',66)],['0x28250组合值体及0x295b0/0x29eb0/0x2a7b0 typed存储体尚未取得；copy、旧值释放和越界行为不推测。','额外第六字节的业务含义未取得原符号，不将其命名成copy/ownership标志。']);
 add('VN03','原VAR_TYPE动作1','删除旧值成功门 → 新类型文本与0x130子描述复制',
 'case1先调用0x28110，返回0才继续；随后清记录+0x139的0x100字节与+0x129的0x10字节，并将payload+0x204的类型文本写入记录。数量非0时按nValueNum×0x130构造数组并保存在记录+0x470，目标为空则返回0xe0000002；分配非空时清数组并调用memcpy_s，从payload+0x308指向区域请求复制同样字节数；所选代码未分支检查其返回值，随后设记录+0x448=1、+0x450=数量。数量0分支另调用0x27080并置+0x448=0。原IAT0x180055568绑定MSVCR120.dll::memcpy_s，原汇编核到RCX目标、RDX容量、R8源pointer、R9长度。',
 '此处复制的是0x130字节子变量描述数组；描述中的pointer目标不因此自动成为深拷贝，不能称整套新变量payload都已复制。',
 '旧值删除成功门位于类型更新/分配前；本切片不证明分配或后续失败时恢复原类型和旧值。',
 '所选新描述复制不释放调用者pBinaryValue；记录+0x470未来退役、描述内嵌pointer所有权及0x25c40的完整旧存储清理仍待取体。',
 [ref('1800245bd','1800245bd',12,true),ref('1800245bd','180024643',12,true),ref('1800245bd','1800246e9',12,true),ref('1800245bd','180024880',12,true),ref('18001f8c0','piStack_17d0 = (int *)thunk_FUN_1800502a4(_Var19);',44)],['0x25c40/0x27080初始化清理体、数组构造失败与回滚、嵌套数据深拷贝仍未闭合。']);
 add('VN04','原DeleteVar分支与0x28110 DeleteVarValue体','ModuleFrame删除通知 → 成功才移除名称记录',
 'DeleteVar先按payload字符串查名称容器，未找到返回0xe000001f；找到后保存原索引，调用0x28110，只有返回0才擦除名称记录并递减其他记录中更大索引。0x28110对记录+0x448为0调用VM_M_DelGlobalVar(context+8,记录+0x29名称)，组合记录则按+0x450数量、+0x470数组0x130步长逐项调用同一导出；返回码在循环中逐项覆盖，返回的是最终保存值，不能说已聚合所有子项失败。原PE IAT0x1800556f0核为ModuleFrame.dll::VM_M_DelGlobalVar。',
 '变量存储删除命令与本模块map记录退役分开，组合变量由子描述数组逐项通知。',
 '擦除名称容器不等于所有既有托管数组或订阅消费者同时清空；共享记录的其他引用仍须分别追踪。',
 '真正删除payload发生在尚未取得的ModuleFrame导出内部，不能把VM_M_DelGlobalVar调用或shared_ptr局部释放直接写成已free数值/字符串。',
 [ref('18001f8c0','if (bVar1 != pbVar27[(longlong)"DeleteVar"',73),ref('180028110','int FUN_180028110',40),ref('180028110','180028110',18,true)],['ModuleFrame.VM_M_DelGlobalVar原体与保留结果/绑定最后读者未恢复；最终码0不证明组合每个子项都成功。']);
 add('VN05','原0x28c30值类型包装的额外参数与锁状态调用','显式覆盖后续callee栈字节，不由显示签名推ABI',
 '0x28c30汇编保存前四入参并读取第五数量；先调用VM_M_SetGlobalVarLockStatus(moduleID,moduleID,1)，保存返回AL，分发int/float/string时将该AL作为第五字节参数、显式把第六字节参数写0，再把原数量置R9D、原payload置R8。所选体结束处调用同一锁状态导出(moduleID,moduleID,0)。因此case3调用前写入的第六字节1不能直接当作最终typed存储callee接收到1；包装有具体覆盖写。这个事实只限已读分支，不能从函数名断言它建立了完整线程互斥。',
 '原入口参数、wrapper局部状态与typedcallee参数位置分开记录；字节flag值与业务语义分开。',
 '该wrapper不维护长期payload副本；具体存储和锁状态导出行为在下层。',
 '末尾unlock样调用只证正常控制流，无完整异常/early return并发安全结论。',
 [ref('180028c30','180028c30',20,true),ref('180028c30','180028cba',10,true),ref('180028c30','180028d15',10,true),ref('180028c30','180028d70',10,true),ref('180028c30','180028eaf',9,true)],['VM_M_SetGlobalVarLockStatus返回值语义及typedcallee的额外字节含义未取得原体/符号；不造copy标志解释。']);
 const files=['tests/ArchitectureRecovery/Recover-GlobalVariableReceiver.ps1','tests/NativeEvidence/VmNativeCapability.java',...fs.readdirSync(path.join(root,dir)).filter(n=>/\.(?:c|json|tsv)$|\.asm\.txt$/.test(n)).map(n=>dir+n)].map(p=>({path:p,sha256:hash(read(p)),byteLength:read(p).length}));
 return {schemaVersion:1,complete:false,facts,unknowns:[{id:'VNU01',target:'ModuleFrame/SDK → GlobalVariableModule dispatch',question:'未取得正式虚表映射/实际调用者，当前证明的是原DLL具体key分支，不宣称入口运行链完整。'},{id:'VNU02',target:'0x295b0/0x29eb0/0x2a7b0 and 0x28250',question:'typed值与组合值最终分配/复制/替换/free尚待读取，不能据入口pointer转发声称深复制完成。'},{id:'VNU03',target:'VM_M_DelGlobalVar / old descriptor retirement',question:'实际底层存储销毁、订阅和结果最后读者仍缺ModuleFrame原体；不将map擦除当payload释放。'}],provenance:{original:{path:target,sha256:expected,byteLength:pe.byteLength},selection:{method:'original string bytes → RIP-LEA candidate → PE runtime range → actual selected Ghidra body/decoded assembly',cases:pe.cases,rawCandidatesAreNotFacts:true},selectedProcedures:3,selectedCaseFragments:2,initialAnalysisTimeoutSeconds:90,wholeBinaryAnalysisComplete:false,sourceProvider:'existing offline Ghidra CLI; not REA retained evidence',files,completions:completed,liveProcessHandle:null,originalRuntimeExecuted:false,targetCodeExecuted:false,deviceCalled:false,anotherWorktreeRead:false,implementationChanges:false,fullOriginalChainClosed:false}};
};
if(require.main===module&&process.argv.includes('--locate')){const r=locate();fs.mkdirSync(path.join(root,dir),{recursive:true});fs.writeFileSync(path.join(root,dir,'pe-layout.json'),JSON.stringify(r,null,2)+'\n');console.log(JSON.stringify({seeds:r.seeds,candidates:r.candidates}));}
else if(require.main===module){const r=module.exports(),report='docs/architecture-recovery/original-variable-native-receiver.json';fs.writeFileSync(path.join(root,report),JSON.stringify(r,null,2)+'\n');console.log(JSON.stringify({report,facts:r.facts.length,unknowns:r.unknowns.length,selectedProcedures:r.provenance.selectedProcedures,selectedCaseFragments:r.provenance.selectedCaseFragments,cliLive:false}));}
