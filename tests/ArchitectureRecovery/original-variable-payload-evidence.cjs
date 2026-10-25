'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert');
const {root,out,source}=require('./evidence-view.cjs');
const dir='docs/architecture-recovery/variable-payload.resources/',read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const pe=JSON.parse(read(dir+'pe-layout.json'));assert.deepStrictEqual(require('./native-pe-inspection.cjs')(pe.path),pe);assert.equal(pe.sha256,'45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b');
 assert(pe.imports.some(i=>i.iatVa==='0x180204580'&&i.name==='??3@YAXPEAX@Z'));
 const binary=read(pe.path),tables=JSON.parse(read(dir+'vtable-selection.json'));assert.equal(tables.artifactSha256,pe.sha256);
 for(const t of tables.tables){const r=parseInt(t.rva,16),s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize),o=s.rawOffset+r-s.rva;t.slots.forEach((v,i)=>assert.equal('0x'+binary.readBigUInt64LE(o+i*8).toString(16),v));}
 const captures=['creation','vtable','prepare-slots','control-dispose','destructor','count-slots'];
 const completions=captures.map(c=>{const p=dir+'completion-'+c+'.json',v=JSON.parse(read(p).toString().replace(/^\ufeff/,''));assert.equal(v.exitCode,0);assert.equal(v.liveProcessHandle,null);assert.equal(v.vendorExecuted,false);return{path:p,...v};});
 const methods=fs.readdirSync(path.join(root,dir)).filter(n=>/^functions-.*\.tsv$/.test(n)).flatMap(n=>read(dir+n).toString().trim().split(/\r?\n/).slice(1).map(l=>{const [address,name,bodyBytes,decompiled]=l.split('\t');assert.equal(decompiled,'true');assert(+bodyBytes>0);assert(read(dir+address+'.c').length>0);assert(read(dir+address+'.asm.txt').length>0);return{address,name,bodyBytes:+bodyBytes,decompiled:true};}));assert.equal(methods.length,new Set(methods.map(m=>m.address)).size);
 const ref=(stem,asm=false)=>({...source(dir+stem+(asm?'.asm.txt':'.c'),stem,asm?stem+'\t':' FUN_'+stem+'(',1000),binaryIdentity:'SHA256 '+pe.sha256,artifact:pe.path,evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode',provider:'owned copied offline Ghidra project; not REA retained evidence'});
 const raw={...source(dir+'vtable-selection.json','raw PE vtable slots','"artifactSha256"',100),binaryIdentity:'SHA256 '+pe.sha256,evidenceType:'offline PE byte table revalidated against original file'};
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'source-confirmed',stages:['variable','transfer','results','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('VP01','VZ03/04 typed map factory f74b0/f7650/f7260','明确构造的0x88值对象与另一份0x18共享控制块',
 '名称未命中时三个factory分别分配0x88对象并写vtable 22e690/22e6b8/22e6e0；命中则复制已有节点+0x40共享对。包装体fa090/fa100/fa020另分配0x18控制块，强/弱计数均初始1，保存对象指针+0x10，写22e538/22e560/22e5b0。原PE字节校验这些实际表槽，不能把typed对象虚表与shared控制块虚表混成同一析构入口。',
 '值对象含原始载荷指针+0x10、callable+0x30、名称+0x38、类型文本+0x58、待准备字节+0x78；控制块另存强/弱计数与对象指针。',
 '新对象进入名称map；后续同名可复用共享对象，局部shared pair不等于新建底层载荷。',
 '本事实仅证明创建与共享形态，不证明原始payload已拥有或释放。',
 [ref('1800f74b0'),ref('1800f7650'),ref('1800f7260'),ref('1800fa090'),ref('1800fa100'),ref('1800fa020'),raw],['初始化所复制callable的动态分配器目标未在本切片定位。']);
 add('VP02','typed对象虚表+8','清逻辑长度，保留既有数值/字符串分配',
 '原表明确int/float/string的+8槽为f3fb0/f40b0/f42e0。int/float在当前数量大于0时仅把payload+0x68/+0x40写0；string逐项将内部字符串长度写0并首字符置零（按capacity选择SSO或heap），最后把payload+0x30计数写0。所选体没有delete/free；VZ03所调用的+8现在可细化为逻辑清空，不能称完整释放。',
 '数值计数与容量/基址分开；string保留内部string对象及其容量。',
 '旧数组容量、string堆分配可在清空后继续保留。',
 '清空不证明最终回收；对象虚表首槽f3f90是检查数量为0，亦不能当析构。',
 [raw,ref('1800f3fb0'),ref('1800f40b0'),ref('1800f42e0'),ref('1800f3f90'),ref('1800f3fa0'),ref('1800f40a0'),ref('1800f42d0')],['所有消费者最后读者以及何时触发最终载荷析构仍未知。']);
 add('VP03','00fb90一次准备门','callable产生原始载荷后调用typed容量准备槽',
 '仅对象+0x78非零时检查callable；空则抛bad_function_call，非空调用其虚槽+0x10并把返回指针写对象+0x10。随后载荷flags写入1/2、处理名称/类型文本，再调用typed对象虚槽+0x10，成功正常返回才将待准备字节写0。该typed槽由原表绑定到f3f80/f4080/f42b0。',
 'callable、typed包装对象、返回的原始载荷是三种实体；原始指针持有不等于shared强计数覆盖该载荷。',
 '准备门使正常后续调用复用已有返回指针；并非每次更新重新分配。',
 '本体没有最终载荷free；异常后门是否保持及并发使用未运行验证。',
 [ref('18000fb90'),ref('18000fb90',true),raw],['callable实际虚表与其返回载荷的分配/共享仓所有权未定位；不补常见智能指针语义。']);
 add('VP04','typed虚槽+0x10的数值准备','扩容复制旧值，并有条件退役旧数组',
 'int/float仅capacity小于对象请求量时扩容，目标容量取请求量、旧容量两倍与最小4的最大值。新块前8字节保存旧块header；数据初始化后按旧count复制4字节值。旧块非null且旧header==0才delete[]，否则本体不释放旧块。string槽仅在数量不足时调用1a39f0追加差额，未将该callee未取体补成数组析构。',
 'int载荷+0x68/count、+0x6c/capacity、+0x70/base；float为+0x40/+0x44/+0x48；数据在base+8。',
 '容量足够时保留块；header非零分支的外部内存关系不能凭形式命名。',
 '已证旧header==0分支的旧块delete[]，不是全部载荷或当前新块释放。',
 [ref('1800f3f80'),ref('1800f4080'),ref('1800f3f80',true),ref('1800f42b0')],['00f3a0与1a39f0未取体；非零header的最终持有/退役责任未闭合。']);
 add('VP05','VZ06强计数归零后的真实控制块slot0','fa730清理包装对象，再delete包装对象本身',
 '三个实际控制块表首槽均为fa730：读取控制块+0x10对象，非null调用f7410，然后汇编把同一对象放RCX并尾跳operator delete IAT180204580。f7410对名称/类型capacity>=16分别delete其堆块，复位字段；若callable存在，RCX传callable、DL=(callable!=对象内联区+0x18)，调用其虚槽+0x20后置null。本体未直接读取对象+0x10以free原始载荷。',
 '对象名称/类型字符串、callable储存、0x88包装对象、callable返回的载荷必须分别描述。',
 '只有VZ06强计数真正归零才到该dispose；仅从某map移除不能证明已执行。',
 '包装对象与其名称/类型堆块释放已证；callable虚析构动态目标与payload最终free仍保留未知。',
 [raw,ref('1800fa730'),ref('1800fa730',true),ref('1800f7410'),ref('1800f7410',true)],['callable实际虚表+0x20接收者未定位；不能由f7410的清理形态推断返回载荷被销毁。']);
 add('VP06','VZ06弱计数归零后的真实控制块slot+8','0210d0再派发slot+0x10并按DL低位delete控制块',
 '三个控制块表+8均为0210d0；它把同一控制块作为RCX、第二参数1转交自身vtable+0x10。原表该槽均为0210f0；其汇编TEST DL,1，非零调用operator delete IAT180204580，RCX仍为控制块。本证据补齐弱归零时控制块本身退役，不把包装对象与原始载荷混同。',
 '共享控制块18字节；强归零对象dispose与弱归零控制块delete是两个条件。',
 '其他shared/weak持有者可延后对应阶段；本切片未枚举所有持有者。',
 '仅确认所选动态控制块及正常条件路径；不证明全局变量删除后立即无读者或所有payload被free。',
 [raw,ref('1800210d0'),ref('1800210f0'),ref('1800210f0',true)],['最终读者、跨仓共享、异常回收与原机运行未确认。']);
 const files=['tests/ArchitectureRecovery/Recover-VariablePayload.ps1','tests/ArchitectureRecovery/native-pe-inspection.cjs','tests/NativeEvidence/VmNativeCapability.java','.tools/native-analysis/jdk-21.0.12.1+1/bin/java.exe','.tools/native-analysis/ghidra_12.1.4_PUBLIC/Ghidra/Framework/Utility/lib/Utility.jar',...fs.readdirSync(path.join(root,dir)).filter(n=>/\.(?:json|c|tsv)$|\.asm\.txt$/.test(n)).map(n=>dir+n)].map(p=>({path:p,sha256:sha(read(p)),byteLength:read(p).length}));
 const result={schemaVersion:1,complete:false,facts,unknowns:[{id:'VPU01',target:'callable virtual+0x10 producer and +0x20 cleanup',question:'已绑定typed包装对象和控制块动态虚表，但callable来源、返回载荷的真正分配/共享仓与最终free尚未定位。'},{id:'VPU02',target:'last payload reader',question:'删除map仅撤一份shared所有权；所有已发布消费者最后读者与退役时序未知。'}],provenance:{original:{path:pe.path,sha256:pe.sha256,byteLength:pe.byteLength},methods,selectedProcedures:methods.length,rawTables:tables,files,completions,sourceProvider:'copied owned offline Ghidra project -process -noanalysis; REA provider unavailable; not REA retained Evidence',loggingWarning:'Ghidra logging XML warning observed; admission also requires nonempty C/assembly and every TSV decompiled=true.',liveProcessHandle:null,wholeBinaryAnalysisComplete:false,originalRuntimeExecuted:false,targetCodeExecuted:false,deviceCalled:false,productModified:false,fullOriginalChainClosed:false,implementationCredit:0}};
 fs.writeFileSync(path.join(out,'original-variable-payload.json'),JSON.stringify(result,null,2)+'\n');return result;
};
if(require.main===module){const r=module.exports();console.log(JSON.stringify({facts:r.facts.length,refs:r.facts.reduce((s,f)=>s+f.refs.length,0),methods:r.provenance.methods.length,completions:r.provenance.completions.length}));}
