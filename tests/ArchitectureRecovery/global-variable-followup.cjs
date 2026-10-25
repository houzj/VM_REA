'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');
const {root}=require('./evidence-view.cjs');
module.exports=function(facts,unknowns,scenarios){
 const manifestPath='docs/architecture-recovery/global-variable-followup.resources/manifest.json',manifest=JSON.parse(fs.readFileSync(path.join(root,manifestPath)));
 function native(address){const m=manifest.methods.find(m=>m.address===address);assert(m);return m.files.map(f=>{const b=fs.readFileSync(path.join(root,f.path));assert.equal(crypto.createHash('sha256').update(b).digest('hex'),f.sha256);const lines=b.toString().split(/\r?\n/);return {path:f.path,member:m.artifact+'!'+m.name+' @ 0x'+address,start:1,end:lines.length,excerpt:lines.join('\n'),sha256:f.sha256,evidenceType:f.path.endsWith('.c')?'offline-ghidra-pseudocode':'offline-ghidra-assembly',binaryIdentity:manifest.artifacts[m.artifact].sha256,limitations:m.pseudocodeCoverage+' No original runtime execution.'};});}
 function add(id,title,behavior,addresses,limits){facts.push({id,program:'original',classification:'source-confirmed',title,behavior,refs:addresses.flatMap(native),limits,verification:{static:true,originalRuntimeExecuted:false,endToEndClosed:false}});}
 add('GEX16','先区分存储，再谈“读哪一轮”',
 '0x8e6c0 的选择并非无条件同一个全局单例：模式字节关闭或参数为 0 时返回映射中 key=0 的存储；模式开启且参数非 0 时改交 0xea800 选择上下文存储。已恢复的绑定源读取 0x96fe0 明确传 0，索引助手 0x99be0 也传 0。独立全局读取则经 0x8e910 返回另一个静态对象 0x1802b2b50。0xcae00 按模块 ID 从所给存储的 +8 映射取得引用并增加共享引用计数；它没有在本体中自动改选静态全局存储。两类存储中的变量模块记录是否由注册路径共享，尚未证实。因此目前能确认选定 API 按当时选中的记录读取，不能把“接口没有轮次参数”直接等同“跨流程必然取得 A 最新轮”。',
 ['18008e6c0','18008e910','1800cae00','1800c8f10'],['下一步必须核查变量模块注册、插入与共享对象身份；0xea800 非零上下文分支未据此宣称已闭合。']);
 add('GEX17','停止等待事件先置位，尾部通知和收尾回调随后执行',
 '0xde620 用 CreateEventW(NULL, TRUE, TRUE, NULL) 创建 +0x80 与 +0x88。0xdcc80 在启动前 ResetEvent(+0x80/+0x88/+0x90)，再提交工作入口 0xdd330。0xdd330 离开循环后先 SetEvent(+0x88)，随后置 +0xe9=0、处理单次状态通知、清连续标志、调用 0xde180(...,3,...)，再调用 +0x1c8 的可选函数对象，最后置 +0x1d0=1 并 SetEvent(+0x98)。0xde180 在已恢复的分支中读取流程 ID +0x50 和执行计数 +0xe0，并经全局函数指针发送类型 6 的状态回调。因此 0xdd030 等待 +0x88 成功，只能证明到达该置位点，不能证明其后的状态通知和收尾函数已经结束。',
 ['1800de620','1800dcc80','1800dd330','1800de180'],['这是同一函数内的静态先后顺序，不是原机已经复现迟到写回。+0x1c8 回调具体目标、托管投递队列的排空，以及最后一次全局写入与此处的关联仍待证；+0x98 也不能未经调用方核查就当产品停止屏障。']);
 add('GEX18','连续运行计数属于所选流程记录，不能拿相等数字配跨流程批次',
 '0xdd330 在所选轮次准备路径取得流程模块记录，对该记录 +0xe4 做原子递增，并把递增后的数值复制到当前执行对象 +0xe0；随后发运行状态、准备报告变量并调用所持函数对象执行本轮工作。该计数的更新点位于本轮工作之前。原体显示的是当前流程记录和执行对象，未出现为所有流程分配同一批次号的共同协调器。与托管 VmIO.ExecuteCount 的比较规则合看时，必须分别说明本流程执行计数、结果镜像计数及方案批次身份，不能因 A、B 都显示数字 5 就认定是同一批数据。',
 ['1800dd330','1800de180'],['仍需追 +0xc8 函数对象的具体目标和上层跨流程等待，才能判断消费者此时读取哪个生产者版本；本条不证明所有执行模式使用相同计数。']);
 add('GEX19','索引助手必须按汇编恢复返回值，不能照抄 void 伪码',
 '0x99be0 的伪代码显示 void，但汇编在描述种类 0 分支执行 XOR EAX,EAX，种类 1 返回入口 R8D，种类 2 返回 [RDX+4]。种类 3 则以描述中的模块 ID 和名称从 0x8e6c0(0) 选定存储查记录，再走具名值解析；失败分支还保留各自错误路径。尾部保留 EAX 返回，不能把这三个提前返回分支当成“没有返回值”，也不能把种类 1 的索引来源误写为被调用者凭空生成。0xf74b0 还包含缺名时构造记录并插入的路径，因而不能把任意调用都概括为纯只读查询。',
 ['180099be0','1800f74b0'],['种类 1 的入口 R8D 含义须与各调用点核对；种类 3 以及各类型读写器的容量、数组缺项和失败语义仍需细化。']);
 add('GEX20','SDK 已追到实际命令构造；服务端接收仍保留身份边界',
 '原 SDK 的 IMVS_StopExecute_V30 经 0x647920 到 0x6702f0，构造 0x408 命令并交 0x6c00d0；IMVS_ContinousRunStart_V30 经 0x647820 到 0x670170，构造 0x407 命令。IMVS_BindGlobalVarParam 经 0x64abd0 到 0x695890，先交 0x680b40 序列化，再组装包含 0x417 和 0xc09 的发送内容。候选 PlatformManager.dll 的 PF_M_MsgDeal 经 0x6a0250 到 0x602df0；后者的大型反编译文本提示 0x408 去 0x619670，0x417 等命令去 0x62b200。该大型函数已列出的汇编范围未覆盖这些 switch 碎片，所以这两条分派目前仅是明确检索线索，不能冒称原字节已验证的跨 DLL 调用边。0x619670 内还出现对象 +0x218 的动态调用，最终 ModuleFrame 接收者尚未确定。',
 ['180647920','1806702f0','180647820','180670170','18064abd0','180695890','1806a0250','180602df0','180619670','18062b200'],['尚未运行核对实际加载副本、PF_M_MsgDeal 指针身份与 ModuleFrame 实例；大型 switch 案例还需独立反汇编覆盖，不能仅用 case 伪码补边。']);
 add('GEX21','其余四种类型已取得处理体，复制与释放继续按类型追',
 '0x94190 的种类 4/5/6/7 目标 0x95f40、0x964e0、0x96a60、0x95230 本轮均已取得原体和汇编。前三级处理分别调用 0x97d10→0x98ae0、0x97f40→0x98e00、0x98150→0x99100；种类 7 调 0x97610→0x98950。处理体包含逐项读取、失败退出及条件报告路径，种类 7 还可见字符串容器复制／赋值和共享引用计数操作。已从“只有分派目标”推进到具体处理体，但未仅凭类型编号给种类 4/5/6/7 虚造图像或结构类型名称，也未把局部引用减计数当成整个像素缓冲区已经释放。',
 ['180095f40','1800964e0','180096a60','180095230'],['上述八个内层读写器尚未全部取得；参数容器、指针别名、深浅复制及最终释放必须继续对照其实现和调用点。']);
 Object.assign(unknowns.find(u=>u.id==='GU01'),{question:'SDK 命令构造、候选服务入口与候选处理体已取到；大型 switch 的目标碎片尚缺汇编覆盖，动态 +0x218 接收者及实际加载副本未证。',nextEvidence:'核对 PlatformManager switch 碎片的原字节分派，恢复对象 +0x218 的创建与虚表，再对到 ModuleFrame 的实际导出和实例；不以候选文件名代替加载身份。'});
 Object.assign(unknowns.find(u=>u.id==='GU02'),{question:'已明确区分 key=0 存储、非零上下文存储与独立全局静态存储；尚缺注册时是否共享记录及消费者的生产就绪／同批规则。',nextEvidence:'追变量模块向这些存储插入记录的路径与对象身份，然后核消费者 GetInput 订阅、执行模式调度和等待失败分支。'});
 Object.assign(unknowns.find(u=>u.id==='GU03'),{question:'+0x88 的创建、启动复位和工作循环置位已闭合；已证置位发生在尾部状态回调与 +0x1c8 收尾之前。最后全局写入、动态子任务和托管队列是否被最终停止屏障覆盖仍未知。',nextEvidence:'追 +0xc8 工作函数与 +0x1c8 收尾的具体目标，串起最后写入和 UI 入队／执行；原机有界停止实验未执行。'});
 Object.assign(unknowns.find(u=>u.id==='GU04'),{question:'四种类型处理体和索引助手均已获取，索引提前返回按汇编恢复；内层各类型读写器和容量／别名／释放契约尚未闭合。',nextEvidence:'取得 0x97d10/0x98ae0、0x97f40/0x98e00、0x98150/0x99100、0x97610/0x98950 并逐调用点核 ABI、复制及失败退出。'});
 Object.assign(unknowns.find(u=>u.id==='GU05'),{question:'自研源码路径结论仍成立，但运行探针未通过构建：现有依赖不含当前源码所需 SDK 契约；旧 Workflow 二进制与当前源码身份不匹配，不能替代运行证据。',nextEvidence:'在本工作区重建相互一致的当前依赖快照后运行探针；已保留 net9 引用包缺失与 net10 契约不匹配的失败日志，本轮没有运行通过计数。'});
 scenarios.find(s=>s.id==='cross-flow').original.push('GEX16','GEX18');
 scenarios.find(s=>s.id==='cross-flow').verdict='原版按名读取路径未显式传批次，但绑定源存储与独立全局存储还需追注册共享，且所见执行计数属于当前流程记录；因此现在不能保证 A、B 同批，也不能直接断言每次都读 A 最新值。自研方案批次的显式前置发布与全部单次／连续的独立入口仍须分开。';
 scenarios.find(s=>s.id==='stop').original.push('GEX17');
 scenarios.find(s=>s.id==='stop').verdict='原版已证：+0x88 停止等待事件先置位，尾部状态通知和收尾函数后执行；结果镜像入口本身不按停止代次拒收。尚不能据此判定原机实际发生迟到写回。自研外层过渡租约能阻止新运行，但就绪队列 finally 只释放剩余任务租约，未等待它们；源码风险已定位，运行探针因依赖身份不一致未构建通过。';
 return {manifest:manifestPath,nativeMethods:manifest.methods.length,rawDirectCallsChecked:manifest.directCallsChecked,probe:manifest.probe,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
};
