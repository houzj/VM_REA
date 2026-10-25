'use strict';
// Finite offline original bodies; no vendor module or factory execution.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert');
const {root,source}=require('./evidence-view.cjs');
const dir='docs/architecture-recovery/variable-composite.resources/',prior='docs/architecture-recovery/variable-native-receiver.resources/';
const read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const pe=JSON.parse(read(prior+'pe-layout.json')),hash='20cd716e9471a2687c0cc777548dedb469bcd0356b9da90aa97c42e301e16ff0';assert.equal(sha(read(pe.path)),hash);assert.equal(pe.sha256,hash);
 for(const [iat,name]of [['0x180055540','??_V@YAXPEAX@Z'],['0x180055568','memcpy_s'],['0x1800556e0','VM_M_SetGlobalVarLockStatus'],['0x180055668','VM_M_SetGlobalVarImageEx']])assert(pe.imports.some(i=>i.iatVa===iat&&i.name===name));
 const captures=['composite','retirement','nested'],completions=captures.map(c=>{const p=dir+'completion-'+c+'.json',v=JSON.parse(read(p).toString().replace(/^\ufeff/,''));assert.equal(v.exitCode,0);assert.equal(v.liveProcessHandle,null);return{path:p,...v};});
 const methods=fs.readdirSync(path.join(root,dir)).filter(n=>/^functions-.*\.tsv$/.test(n)).flatMap(n=>read(dir+n).toString().trim().split(/\r?\n/).slice(1).map(l=>{const [address,name,bodyBytes,decompiled,callees]=l.split('\t');assert.equal(decompiled,'true');return{address,name,bodyBytes:Number(bodyBytes),decompiled:true,callees};}));assert.equal(methods.length,9);
 const ref=(stem,asm=false)=>{const p=dir+stem+(asm?'.asm.txt':'.c'),lines=read(p).toString().split(/\r?\n/),anchor=asm?stem+'\t':lines.find(l=>l.includes('FUN_'+stem)&&!l.startsWith(' '));assert(anchor);return{...source(p,stem,anchor,lines.length),binaryIdentity:'SHA256 '+hash,artifact:pe.path,evidenceType:asm?'fresh-offline-ghidra-assembly':'fresh-offline-ghidra-pseudocode',provider:'existing owned offline Ghidra CLI -noanalysis; not REA Evidence'};};
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'source-confirmed',stages:['variable','transfer','results','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('VX01','原GlobalVariableModule复合设置0x28250','0x130描述逐项分发至已绑定typed入口',
 '入口RDX是描述数组、R8D是条数，按0x130步进读取+0x100类型与+0x104数量。类型0/1/2分别用+0x108/+0x110/+0x118的payload指针调用295b0/29eb0/2a7b0。汇编确认typed第五栈字节来自VM_M_SetGlobalVarLockStatus返回AL，第六栈字节固定1；据VS03已见第六字节非0跳过typed联动分支。起始SetGlobalVarLockStatus(moduleID,moduleID,1)，正常末尾同入口第三参0；这里只证API及实参，不能等同原生互斥锁实现。',
 '复合描述数组引用→名称、typed payload地址、数量；本分发循环没有复制该数值payload。',
 '每个已执行子设置覆盖同一个local_34；没有失败后break或保存第一个错误的分支，未知类型跳过。返回的是最后执行子设置结果，零项/全部跳过仍为初始0，不证明所有子项成功。',
 '不释放传入描述数组或数值pointer；typed及其ModuleFrame实际存储边沿用VS/VZ有限证据，不把这一循环称所有权移交。',
 [ref('180028250'),ref('180028250',true)],['负条数经MOVSXD后无符号比较的异常输入安全性未运行验证；不据正常条数循环推断边界校验。','复合条目的创建/命名及不同订阅者看到更新的原子性没有由此证明。']);
 add('VX02','record退役0x25c40','清零计数，释放+0x440/+0x458/+0x460/+0x468/+0x470',
 '0x25c40真实原体是缓冲清理，不是初始化：先写+0x43c=0、+0x450=0；非空+0x440、+0x458、+0x460分别delete[]，清pointer；+0x468调用1ba20(pointer,3)，+0x470调用1bc70(pointer,3)，返回后清pointer及+0x450。它未调用ModuleFrame删除变量值，不能把record缓冲清理等同全局最终存储删除。',
 '这些record槽持有不同分配；+0x470是复合描述数组，+0x468是0x60描述数组。',
 '计数与pointer分别复位；函数没有把+0x448复合标志清零，不能把它单独当完整record重置。',
 '释放动作沿具体delete链；不是GC、也不是仅移除字典引用。调用者是否仍保留已复制的payload地址必须另核。',
 [ref('180025c40'),ref('180025c40',true)],['record整体析构、并发最后读者和调用0x25c40的全部路径未在本切片遍历。']);
 add('VX03','record+0x470数组析构1bc70(pointer,3)','逐0x130子描述析构，再释放cookie前的数组分配',
 '1bc70参数3同时启用vector析构与delete[]：从pointer-8读数量，以0x130步长调用1b900，然后delete[](pointer-8)。每个1b900对子描述+0x108/+0x110非空指针直接delete[]，清pointer与+0x104；+0x118非空则调用1ba20(pointer,3)。1ba20以0x60步长调用1b830并delete[]其pointer-8。1b830只有描述+0x50的int非0且+0pointer非0时才delete[]内容、清pointer与+8长度，最后始终清+0xc容量。',
 '外层0x130数组拥有子数值分配；内层0x60描述的内容是否释放受+0x50标志控制。正式字段名尚缺符号，仅按偏移记录。',
 '外层和内层数组数量来自各分配前cookie，不来自已被清零record+0x450；因此先清record计数不阻止vector析构。',
 '不是笼统deep-free：内层标志0会跳过内容delete，不能称所有字符串/图像内容都无条件释放。',
 [ref('18001bc70'),ref('18001bc70',true),ref('18001b900'),ref('18001b900',true),ref('18001ba20'),ref('18001b830'),ref('18001b830',true)],['+0x50正式所有权语义、每个生产分支如何置位/转移仍未闭合；不由delete条件猜字段名。']);
 add('VX04','复合描述构造1b890与先前VAR_TYPE浅描述复制','初始化与后续嵌套释放边界分开',
 '本次取到1b890：清名称0x100字节、+0x104=0、+0x100类型=6、+0x108/+0x110/+0x118=0。先前VAR_TYPE分支已见按条数分配带cookie的0x130数组、逐项构造、随后整段memcpy_s输入描述；该整段复制包含指针字段，不能因调用memcpy_s就称嵌套payload已复制。新增析构体证明这些复制进来的非空数值pointer将进入delete[]，内层内容则受+0x50控制。',
 '构造的空状态→描述字节副本→嵌套析构链；三个阶段分别可证。',
 '只确认record+0x470保留描述副本；payload究竟由哪个接收路径分配/移交，需要补生产体及ABI，不能用本地数组分配代替。',
 '清理源体已取到，但复制输入与析构是否对同一有效拥有者配对、分配器是否兼容仍未证；不据静态形状声称无悬挂/双重释放。',
 [ref('18001b890'),{...source(prior+'1800245bd.c','VAR_TYPE selected case fragment','      uVar1 = *(uint *)(in_stack_00000058 + 0x304);',46),binaryIdentity:'SHA256 '+hash,evidenceType:'prior-offline-selected-case-fragment',provider:'prior owned Ghidra case fragment; inherited stack parameters are not standalone ABI'},ref('18001b900')],['VAR_TYPE case是大函数内片段，in_stack字段依赖原整体栈语义，不当成独立函数正式参数。','输入嵌套地址来源与借用/转移协议尚缺，不能直接断言原版存在double-free。']);
 add('VX05','原SetGlobalValue0x27080','record中数值数组或紧凑字符串描述发布到ModuleFrame',
 '0x27080日志实名CVariableModuBase::SetGlobalValue，实际是发布体，不是清理体。record+0x450非0时先调用SetGlobalVarLockStatus(...,1)，int/float分支分别传+0x458/+0x460地址、数量、返回AL到BatchSet；string且+0x468非空时，分配数量×0x10临时数组，把每个0x60描述的pointer/+8长度/+0xc容量复制进去，调用BatchString后正常路径delete[]临时描述，随后SetGlobalVarLockStatus(...,0)。临时分配返回null分支记录日志并提前return，未经过正常末尾的status(...,0)；这只是所见控制流，不断言status就是实际mutex。',
 'string转换复制描述，不复制内容字节；零计数另走初始文本+0x139的scalar设置路径，不能把两者混称空数组。',
 '原record的+0x468及嵌套内容在这个发布循环保留；临时紧凑描述仅到batch返回。',
 '正常分支释放临时描述，不释放record源数组；异常路径和下层最终值生命周期沿VZ证据另列。',
 [ref('180027080'),ref('180027080',true)],['锁状态API本身的同步含义及分配异常unwind尚未恢复；不由null失败分支推断所有异常路径。','本体末尾链接参数发送使用record+0x139初始文本，不能据数组设置成功推断每个链接拿到完整数组。']);
 add('VX06','复合type4分支','图像字节进入2b2e0及VM_M_SetGlobalVarImageEx，排除PointSet误归类',
 '28250的类型4将+0x118视作0x60描述数组，逐项取pointer/+8长度与下标，调用2b2e0。该helper日志set image start/end，正常申请0x145c0缓冲成功且pointer/handle均非0后，memcpy_s复制输入长度字节到新buffer，并设置局部描述长度。汇编确认调用ImageEx前RCX=context+8、RDX名称、R8D下标、R9=&局部描述；第五栈参数是0x145c0返回的handle，第六字节继承helper入口第六字节（上游由LockStatus返回AL写入）。伪码显示四参不等于handle/第六字节丢失。',
 '这里确实复制图像内容，区别于数值pointer转发和string描述浅复制；本分支不能当作PointSet存储体。',
 'helper在所见正常返回路径没有delete新buffer，不能因此断定泄漏或永久缓存；0x145c0与ImageEx接收所有权需另取体。',
 '调用者输入图像bytes没有在本helper释放；新buffer/handle最终释放者尚未在该切片闭合。',
 [ref('180028250'),ref('180028250',true),ref('18002b2e0'),ref('18002b2e0',true)],['0x145c0分配/复用、ImageEx对handle的保留与释放未取；无原版设备/SDK运行佐证。','PointSet原生最终存储路径仍未知，不能由类型4或图像分支补齐。']);
 const files=['tests/ArchitectureRecovery/Recover-VariableComposite.ps1','tests/NativeEvidence/VmNativeCapability.java',...fs.readdirSync(path.join(root,dir)).map(n=>dir+n)].map(p=>({path:p,sha256:sha(read(p)),byteLength:read(p).length}));
 return{schemaVersion:1,complete:false,facts,unknowns:[{id:'VXU01',target:'composite nested payload production',question:'描述浅复制与析构链已证；嵌套payload分配来源、转移协议、+0x50字段全部置位路径未闭合。'},{id:'VXU02',target:'image handle and PointSet',question:'type4已证是图像分支；145c0缓冲/handle→ImageEx最终拥有者与PointSet真实native链仍待取体。'},{id:'VXU03',target:'status and concurrent readers',question:'LockStatus API调用参数可证，不能据名称假定互斥/原子更新、并发最后读者或失败回滚。'}],provenance:{original:{path:pe.path,sha256:hash,byteLength:read(pe.path).length},sourceProvider:'existing owned offline Ghidra CLI -noanalysis; not REA Evidence; REA provider unavailable previously confirmed by root',selectedProcedures:methods,completions,files,originalRuntimeExecuted:false,targetCodeExecuted:false,deviceCalled:false,anotherWorktreeRead:false,implementationChanges:false,liveProcessHandle:null,wholeBinaryAnalysisComplete:false,fullOriginalChainClosed:false}};
};
if(require.main===module){const report=module.exports(),output='docs/architecture-recovery/original-variable-composite.json';fs.writeFileSync(path.join(root,output),JSON.stringify(report,null,2)+'\n');console.log(JSON.stringify({output,facts:report.facts.length,unknowns:report.unknowns.length,procedures:report.provenance.selectedProcedures.length,liveProcessHandle:null}));}
