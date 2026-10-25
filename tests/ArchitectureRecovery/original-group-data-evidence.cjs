'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert');
const {root,out,source}=require('./evidence-view.cjs');
const dir='docs/architecture-recovery/group-original-data.resources/',read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const pe=JSON.parse(read(dir+'pe-layout.json'));assert.deepStrictEqual(require('./native-pe-inspection.cjs')(pe.path),pe);assert.equal(pe.sha256,'45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b');
 const raw=JSON.parse(read(dir+'selected-strings.json')),bytes=read(pe.path);assert.equal(raw.sha256,pe.sha256);for(const r of raw.strings){const a=parseInt(r.rva,16),s=pe.sections.find(s=>a>=s.rva&&a<s.rva+s.rawSize),o=s.rawOffset+a-s.rva;assert.equal(bytes.toString('utf8',o,bytes.indexOf(0,o)),r.text);}
 const captures=['loop-output','input-typed-copy','lifecycle-input','output-descriptor-binding'],completions=captures.map(c=>{const p=dir+'completion-'+c+'.json',v=JSON.parse(read(p).toString().replace(/^\ufeff/,''));assert.equal(v.exitCode,0);assert.equal(v.liveProcessHandle,null);assert.equal(v.vendorExecuted,false);return{path:p,...v};});
 const methods=fs.readdirSync(path.join(root,dir)).filter(n=>/^functions-.*\.tsv$/.test(n)).flatMap(n=>read(dir+n).toString().trim().split(/\r?\n/).slice(1).map(line=>{const[address,name,bodyBytes,decompiled]=line.split('\t');assert.equal(decompiled,'true');assert(+bodyBytes>0);assert(read(dir+address+'.c').length);assert(read(dir+address+'.asm.txt').length);return{address,name,bodyBytes:+bodyBytes,decompiled:true};}));assert.equal(methods.length,16);assert.equal(new Set(methods.map(m=>m.address)).size,16);
 const ref=(stem,anchor,count=32,asm=false)=>{const file=dir+stem+(asm?'.asm.txt':'.c');if(!anchor&&asm)anchor=stem;if(!anchor)anchor=read(file).toString().split(/\r?\n/).find(l=>/^[a-zA-Z]/.test(l)&&l.includes('FUN_'+stem+'('));return{...source(file,stem,asm?anchor+'\t':anchor,count),binaryIdentity:'SHA256 '+pe.sha256,artifact:pe.path,evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode',provider:'copied owned offline Ghidra project; not REA retained Evidence'};};
 const strings={...source(dir+'selected-strings.json','Original PE selected data strings','"sha256"',100),binaryIdentity:'SHA256 '+pe.sha256,evidenceType:'offline original PE bytes, revalidated'};
 const facts=[],add=(id,stage,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'source-confirmed',stages:stage,modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('OG01',['prepare'],'原DoWork进入循环前','一次准备输入与输出描述列表',
 'DoWork先调用a4a00，再调用a74c0(this,this+0x180,&本次输出列表)，二者位于逐轮循环之前。a4a00构造原字符串input并交a4a90(this,this+0x170,input)；后者按类型分派输入准备helper。a74c0逐个读取配置描述，向12e200传递上下文、类型与名称，只有其返回0才复制名称/模式到临时描述并把描述加入本次列表，失败分支记录日志后继续其它条目。输入配置、临时输出描述和每轮结果不是同一对象。',
 'this+0x170输入描述容器、this+0x180输出配置容器、DoWork局部链表分别持有；12e200真实第5参数是目标描述指针，a74c0汇编在栈+0x20设置，不能照四参伪码漏掉它。',
 '所选正常路径显示输入/输出描述在循环前准备一次；不能由此声称每轮所有输入值深快照或冻结。',
 'a74c0每轮配置准备末尾清临时描述，DoWork退出另退役本次列表；具体共享载荷见OG07边界。',
 [ref('1800a0930','    iVar17 = FUN_1800a4a00',4),ref('1800a0930','    FUN_1800a74c0(',3),ref('1800a4a00',null,45),ref('1800a4a90',null,115),ref('1800a74c0',null,170),ref('1800a74c0','1800a75d7',13,true),ref('18012e200',null,120),strings],
 ['输入typed准备helper10450/10ab0/117a0等未在本切片逐体恢复，输入数据复制/共享不以方法名推定。','12e200交12dcb0建立typed目标的具体动态绑定尚未补体；第5参数已核，但不把其载荷与其它专题的活实例直接合并。']);
 add('OG02',['round'],'DoWork每轮开始','StartIndex/EndIndex/LoopIndex的第0项写入与严格上界',
 '正常DoWork以开始字段+0x21c初始化本轮索引，先作有符号index<end(+0x220)门；每轮写this+0x224，再用RCX=this、EDX=start、R8D=end、R9D=index调用a7190。该体从原PE字符串确认StartIndex/EndIndex/LoopIndex，按当前Group模块键取int共享对象，对三个名字分别调用f3ee0(target,0,value)。f3ee0准备对象后，已有第0项时覆盖该4字节，否则增加一项。轮尾32位ADD后再次有符号JL到循环体，不能把这个静态分支扩成数学整数或溢出安全循环。',
 '三个输出是Group键下的独立具名int值；内部索引字段与对外LoopIndex数据不是同一个变量存储位置。',
 '值对象可重复查得并覆写第0项；本体不每轮清除整个模块结果仓。',
 'a7190释放局部shared持有和临时字符串；覆盖第0项不证明旧结果消费者已释放。',
 [ref('1800a0930','1800a13b4',28,true),ref('1800a0930','1800a140d',11,true),ref('1800a0930','1800a1be4',7,true),ref('1800a7190',null,240),ref('1800f3ee0',null,60),strings],
 ['负步长、start>=end、int32溢出以及所有错误分支未运行差分；不从C有符号算式推出语言层未定义行为结果。']);
 add('OG03',['round'],'本轮索引发布后、执行前','IsBreakLoop布尔门与首轮特殊准入',
 'DoWork在a7190之后调用a55b0，汇编立即TEST AL并在非零时跳过本轮执行/输出段。a55b0原字符串为CGroupNode::IsBreakLoop；体先检查EnableBreak相关字节this+0x250，再检查this+0x22c==0和a6730返回的首轮保护门，随后才按this+0x248类型与+0x24c比较符处理。DoWork只在通过break门后增加this+0x22c，所以检测与已执行轮计数不可混用。',
 '本轮索引、已通过break门的计数、比较源/目标与布尔结果分开；原byte返回不能因新伪码写void而忽略。',
 '首轮门可能阻止读取本轮尚未生产的比较数据；本切片仅保留实际门，不泛称所有订阅可用。',
 'break使本轮执行/输出不发生，不等于清空此前累积输出；比较临时文本在对应分支退役。',
 [ref('1800a0930','1800a1425',6,true),ref('1800a0930','1800a15d9',4,true),ref('1800a55b0','  if (*(char *)(param_1 + 0x250)',45),ref('1800a55b0','1800a5daa',10,true),ref('1800a6730',null,110),strings],
 ['a6730完整类型/作用域分支、比较source getter以及float容差/NaN/字符串全部比较合同不在本轮闭合；不以资源枚举代替各操作汇编。','新伪码把a55b0显示void且部分返回值丢失；本事实用调用侧AL与所选返回汇编限定，不重写原行为。']);
 add('OG04',['execute','retain'],'本轮执行对象与ProcessGroupOutputV44','执行后分型处理并按描述模式调用目标虚槽',
 '通过break门后DoWork调用局部执行对象的虚表+8；随后不把相邻指令解释为未知线程池合同，而是保存实际关系：RCX=this、RDX=&本次输出列表、R8=&本轮状态字传入a7830，返回EAX被保存并检查。a7830逐描述按type分派：int/float/string仅mode字段==1时先调用相应目标虚表+8，再调用13450/139f0/13fa0；image及其它所见类型有无条件先调用+8的独立分支。不能把这些类型都画成相同的累积策略。',
 '输出描述持typed目标、来源状态和模式；执行对象虚调用、输出目标虚调用、typed复制helper是三种不同边。',
 'mode!=1的三种标量描述不经此处+8前置调用；本轮输出可能保留前轮结果，细节由各typed helper决定。',
 '本体仅证明虚槽调用条件；12dcb0动态绑定未闭合，不能直接把Group这里+8同一实例等同于VP专题已定位的逻辑清空对象。',
 [ref('1800a0930','1800a1638',9,true),ref('1800a0930','1800a1844',7,true),ref('1800a7830',null,235),ref('1800a7830','1800a7ff1',5,true)],
 ['虚执行对象的具体调度与全部子节点状态传播未在本切片恢复。','图像/复合类型累积、载荷租约与虚槽接收者需分别恢复，不从int路径外推。']);
 add('OG05',['retain'],'三类标量输出helper的正常来源分支','按索引读取一个值，追加进目标；文本另有复制',
 '13450/139f0/13fa0先检查目标typed指针。来源分类字段+0x98为1或2时使用描述中的字面值；为3时验证来源typed对象，再处理选择模式。普通索引分支先调用132e0取得索引，检查来源payload/count，从int/float各4字节元素或string指针数组取一个值，分别交fc50/fd30/100f0追加目标。string先经006470形成临时文本，再复制交100f0；不是把来源指针数组直接作为目标结果。模式+0x170==1另交fcb0/fe00/101a0处理，不能与单元素分支混称已证完整数组拷贝。',
 'int来源payload+0x68/count和+0x70/base，float为+0x40/+0x48；string为+0x30/count和+0x38/指针表。目标对象指针位于描述+0x28/+0x38/+0x48；汇编明确float值走XMM1、目标走RCX，string目标走RCX、临时文本地址走RDX，不能按整数寄存器签名吞掉浮点参数。',
 '目标追加与mode==1前置虚槽共同决定保留/替换，源值按所见分支读取，不能泛称全数组深快照。',
 '临时string的堆块在对应路径delete；来源shared对象、目标结果与调用临时文本的释放责任分开。',
 [ref('180013450',null,245),ref('180013450',null,500,true),ref('1800139f0',null,290),ref('180013fa0',null,310),ref('1800139f0','180013d8f',8,true),ref('180013fa0','180014374',22,true)],
 ['132e0完整索引表达式求值、全数组helper与目标append的底层分配/最后读者未在Group专题闭合。','空来源、越界、不同失败后的先前累积值与原厂运行结果未做差分。']);
 add('OG06',['execute','retain'],'输出处理失败与DoWork收尾','分开保存处理错误码、状态码和ModuStatus布尔值',
 'a7830继续遍历描述；在尚未捕获错误时，来源状态分类为3且来源记录存在、其工作状态不在0x75掩码允许集合时，复制该来源错误码与工作状态到本次错误/状态输出。汇编最终MOV EAX,R13D返回累计错误。DoWork收尾以其最终错误寄存器是否0写ModuStatus(index0)，把错误码写this+0x7c、另一个状态码写this+0x84，清this+0x1b8执行中标记，并另发布ModuRunTime。成功布尔、错误码与状态枚举不是一个字段。',
 '本次局部错误、本次局部状态、this工作字段及模块输出值分开；返回0不构成所有来源/消费者都成功的证明。',
 '失败体未见统一回滚前面已追加输出，不能以最终ModuStatus=false画成累积数据全部撤销。',
 '收尾状态写入不等于结果仓销毁；本次描述节点退役见OG07。',
 [ref('1800a7830','        if (((*(int *)(puVar3 + 0x15)',24),ref('1800a7830','1800a7ff1',5,true),ref('1800a0930','1800a1e95',22,true),ref('1800a0930','  VM_M_SetInt(',17),strings],
 ['所有上游失败映射、mask中每个状态的名称、异常与取消的全路径未枚举；不由数值猜业务枚举语义。']);
 add('OG07',['retire'],'DoWork正常退出的本次描述链表','退役临时节点与shared持有，结果最后读者仍未知',
 'DoWork收尾把本次输出链表头复位，逐节点调用a77a0(节点+0xa8)与a03a0(节点+0x18)，operator delete节点，最后delete链表头。a77a0与a03a0对各shared控制块真实LOCK递减强计数，仅强归零调用虚槽0，再递减弱计数，弱归零调用虚槽+8；a77a0还清文本。节点及符合capacity条件的临时文本有直接delete证据，不能把局部shared撤销写成所有结果payload立即释放。',
 '本次输出订阅描述、shared控制块、typed目标/源载荷、仍可被模块仓或消费者持有的结果分别退役。',
 '本次列表清除只撤销列表持有；前轮累积结果与其它仓库缓存可能仍存。',
 '节点/链表头直接delete已证；具体shared动态析构接收者和结果最后读者未在本专题定位，不冒全局无泄漏。',
 [ref('1800a0930','  plVar22 = (longlong *)*plStack_2d0;',22),ref('1800a77a0',null,65),ref('1800a03a0',null,220),ref('1800a77a0',null,100,true)],
 ['typed结果载荷最终free、异常栈展开与并发最后读者未知；正常收尾不是所有异常必清理证明。']);
 const stages=[{id:'prepare',title:'准备输入与输出描述',summary:'循环前准备一次输入与本次输出描述；值复制合同按具体helper保留边界。',factIds:['OG01']},{id:'round',title:'发布本轮索引并检查中断',summary:'Start/End/LoopIndex写第0项，break门先于执行。',factIds:['OG02','OG03']},{id:'execute',title:'执行并记录错误/状态',summary:'只连已核虚调用和输出处理实参，不能补成完整子调度器。',factIds:['OG04','OG06']},{id:'retain',title:'按描述模式保留或更新结果',summary:'类型分派、前置虚槽与按索引追加分开；图像及全部数组保留未知。',factIds:['OG04','OG05','OG06']},{id:'retire',title:'撤销本次描述持有',summary:'节点delete与shared条件退役已见，结果最后读者未闭合。',factIds:['OG07']}];
 const files=['tests/ArchitectureRecovery/Recover-GroupOriginalData.ps1','tests/NativeEvidence/VmNativeCapability.java','tests/ArchitectureRecovery/native-pe-inspection.cjs','.tools/native-analysis/jdk-21.0.12.1+1/bin/java.exe','.tools/native-analysis/ghidra_12.1.4_PUBLIC/Ghidra/Framework/Utility/lib/Utility.jar',...fs.readdirSync(path.join(root,dir)).map(n=>dir+n)].map(p=>({path:p,sha256:sha(read(p)),byteLength:read(p).length}));
 const result={schemaVersion:1,complete:false,facts,stages,unknowns:[{id:'OGU01',target:'typed input/output target binding and dynamic execution receiver',question:'12dcb0、输入typed helper和虚执行对象具体调度未在本切片补全；不把每轮输入视为深快照。'},{id:'OGU02',target:'all output families and final payload retirement',question:'普通int/float/string索引复制分支已见；图像/复合、全数组、溢出、异常、取消和最后读者仍未知。'}],provenance:{original:{path:pe.path,sha256:pe.sha256,byteLength:pe.byteLength},methods,selectedProcedures:methods.length,completions,files,sourceProvider:'copied owned offline Ghidra project -process -noanalysis; REA provider unavailable previously confirmed; not REA retained Evidence',priorContext:['docs/architecture-recovery/group-control.edges.json','analysis/full-audit/group-native-audit/group-run.c','analysis/full-audit/group-native-audit/break.c','analysis/full-audit/group-native-audit/prepare-output-native.txt'],loggingWarning:'Ghidra logging XML warning was observed; only actual nonempty C/assembly outputs plus decompiled=true TSV rows and terminal-zero completions are admitted.',decompilerReturnBoundary:'Several fresh pseudocode declarations show void despite observed caller TEST AL/EAX; selected assembly call/return registers are cited and no missing return is inferred from C signatures.',liveProcessHandle:null,wholeBinaryAnalysisComplete:false,originalRuntimeExecuted:false,targetCodeExecuted:false,deviceCalled:false,productModified:false,fullOriginalChainClosed:false,browserRendered:false,implementationCredit:0}};
 for(const s of stages)for(const id of s.factIds)assert(facts.some(f=>f.id===id));fs.writeFileSync(path.join(out,'original-group-data.json'),JSON.stringify(result,null,2)+'\n');return result;
};
if(require.main===module){const r=module.exports();console.log(JSON.stringify({facts:r.facts.length,refs:r.facts.reduce((s,f)=>s+f.refs.length,0),methods:r.provenance.selectedProcedures,stages:r.stages.length,completions:r.provenance.completions.length}));}
