'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');const {root}=require('./evidence-view.cjs');
module.exports=function(facts,unknowns,scenarios){
 const manifestPath='docs/architecture-recovery/global-procedure-comm.resources/manifest.json',m=JSON.parse(fs.readFileSync(path.join(root,manifestPath)));
 const native=(program,address,range)=>{const method=m.methods.find(x=>x.program===program&&x.address===address);assert(method,program+address);return method.files.map(f=>{const b=fs.readFileSync(path.join(root,f.path));assert.equal(crypto.createHash('sha256').update(b).digest('hex'),f.sha256);const lines=b.toString().split(/\r?\n/),start=f.path.endsWith('.c')&&range?range[0]:1,end=f.path.endsWith('.c')&&range?range[1]:lines.length;return{path:f.path,member:program+'!'+method.name+' @ 0x'+address,start,end,excerpt:lines.slice(start-1,end).join('\n'),sha256:f.sha256,evidenceType:f.path.endsWith('.c')?'offline-ghidra-pseudocode':'offline-ghidra-assembly',binaryIdentity:m.artifacts[program].sha256};});};
 const frame=(a,r)=>native('ModuleFrame',a,r),comm=(a,r)=>native('ProcedureComm',a,r);
 const add=(id,title,behavior,refs,limits)=>facts.push({id,program:'original',classification:'source-confirmed',title,behavior,refs,limits,verification:{static:true,originalRuntimeExecuted:false,endToEndClosed:false}});
 add('GEX42','定时业务提供者：已找到匹配接口的 ProcedureComm 原体',
 '框架 0xb6b70 在管理器锁内按模块 ID 找记录，返回记录后端对象；它是 GEX39 所追接口取得路径的一环。独立取得的 ProcedureComm.CreateModule→0x12bb0 构造体把对象虚表写为 0x1d810，原 PE 的 +0x10 槽确为 0x144b0、+0x18 槽确为 0x14e10，分别实现 GetTimerItems 查询和 ProcessItem/ProcessWorkFinish 命令。这里以构造写入、原虚表字节、调用槽和命令字面量共同确认一个可用的业务接口实现，不能只靠字符串匹配认定活实例。它仍不证明被测方案实际配置了该模块或该实例已经被挂到流程 +0x1e8。自研对照：GEX14/GEX15 的取消租约和就绪队列属于另一实现，不能套用这里的原生定时接口。',
 [...frame('1800b6b70'),...comm('180016b00'),...comm('180012bb0'),...comm('1800144b0'),...comm('180014e10',[474,523])],
 ['原机实例 ID、动态加载副本和流程挂接未运行核实；尚不把候选接口静态匹配称为原机身份闭合。完整命令体保存在资源中，正文展开选定相关分支及完整汇编。']);
 add('GEX43','本轮结束只处理另一类条目，不等于排空定时回调',
 'ProcedureComm.GetTimerItems 在对象 +0x70 锁内枚举 +0x60 条目表，只输出 mode==1 的 itemID/timeout。ProcessWorkFinish 则取得同一锁，只对 mode==2 条目调用 0x12f00；该分支没有遍历等待 mode==1 的已发起处理。ProcessItem 交 0x12dc0；其 0x15b50 在表锁内取得条目并增加共享控制块强引用，然后解锁返回，0x12dc0 才继续处理该条目。因此“结束处理拿同一把表锁”本身不保证此前已经取出条目的定时业务执行完毕。强引用保证选定条目对象继续可用，与停止代次校验不是同一保证。自研对照：GEX25 已运行证明取消外层队列可先于慢工具完成；两边都必须追最终提交，不能把任一局部返回当成完整排空。',
 [...comm('1800144b0'),...comm('180014e10',[474,523]),...comm('180012dc0'),...comm('180015b50'),...comm('180015f20'),...comm('180012f00')],
 ['mode 数值是所取结构字段及分支事实，尚未把所有 UI 模式名称与它逐项映射。0x12f00 的其它协议处理体未在本条恢复；GEX41 的交错仍是条件推导，未原机复现。流程其它尾部通知、可选 +0x1c8 函数和目标工具内部同步仍可能改变具体运行时序。']);
 add('GEX44','定时业务在处理时重新读参数：整数全局来源接到当前存储',
 '选定标量发送分支 0x12f00→0x131d0→0x13f10 在实际处理时格式化 %%condition_%d%%；按条件再取 %%ok_send_%d%% 或 %%ng_send_%d%%，特定定时失败分支改读 %%timeout_send_%d%%。0x14140 依次尝试 VM_M_GetInt、GetFloat、GetString，读取索引为 0；这里的 0 是数组元素索引，不是执行轮次。ProcedureComm 原导入表指向 ModuleFrame.dll。已继续取到整数导出 0x1300f0→0x11e580：后者查询参数绑定描述；种类 3 时解析来源 ID/名称/索引，遇全局来源由 0xbcd20 判定后加全局锁，经 0x8e910→0xca500 读取当前全局记录，失败再回退流程存储。这个整数分支没有向值存储提交消费者 ExecuteCount、生产者轮次或历史版本键。因此选定定时业务不是在登记计时项时冻结这些参数；它读到处理时实际可见的值。自研普通参数 GEX10/GEX24 则从运行请求携带的字典快照读值，方案批次 GEX12 在生产流程完成后发布成功节点值再给后继流程建立快照；这些入口须分开比较。',
 [...comm('180012f00'),...comm('1800131d0'),...comm('180013f10'),...comm('1800140a0'),...comm('180014140'),...frame('1801300f0'),...frame('18011e580')],
 ['本次只把整数分支继续接到全局存储。GetFloat/GetString 的尝试顺序及字符串扩容可见，但其框架内部消费者体不借整数分支代证。绑定名称中的格式转义保留原字面量；处理时读取当前值不等于已在原机观察到读错轮次。不同参数各自读取，也未证明跨参数原子快照。']);
 add('GEX45','业务写出抵达目标模块：这是命令送达，不冒称全局旧值写回',
 '0x131d0 把选到的标量转成发送文本，格式化 WriteString-%d，再调用 VM_M_SetBinayData(目标模块 ID,名称,数据,长度)。ModuleFrame 导出 0x13aad0→0x43120，先检查管理器 +0x22 禁止位；普通目标分支取得模块 +0x128 后端对象，若存在则经虚表 +0x18 同步交付名称、字节和长度，否则交 0xb9390。0xb9390 的选定路径会释放旧名称条目的堆缓冲，分配新缓冲并 memcpy_s 新数据。它既不同于 GEX04 的全局变量记录写入，也不同于 GEX09 的托管结果回调。此链证明定时业务可以走到目标模块命令入口，尚不证明具体目标执行了设备发送、产生全局变量更新或污染 UI。自研最终 UI 写入 GEX26 仍有完整产品入口验证缺口，不用这个原版命令路径替它作证。',
 [...comm('1800131d0'),...frame('18013aad0'),...frame('180043120'),...frame('1800b9390'),...frame('1800e1d20'),...frame('1800e2160'),...frame('180079770'),...frame('1800d33b0'),...frame('18003fde0'),...frame('1800d34a0')],
 ['管理器初始化 0x3fde0 清零包含 +0x22 的字段，但其后全部写入者及其与单流程停止的关系仍未知，不能给它擅自命名为 StopEpoch。普通目标的 +0x18 动态处理体与实际设备行为未运行验证；特殊目标分支单独保留原体。0xe2160 所达 0xd34a0 在锁内维护事件及队列，随后 SetEvent 通知；这段原体不是等待定时回调返回的证明。队列工作者、具体载荷构造、其它业务同步仍需继续核实。']);
 facts.find(f=>f.id==='GEX41').limits.push('GEX42–GEX45 已补匹配的 ProcedureComm 业务原体：结束项与定时项分开，取条目锁不覆盖整个处理。具体目标命令和其它尾部同步仍不能省略。');
 Object.assign(unknowns.find(u=>u.id==='GU03'),{question:'框架定时登记、工作线程和匹配的 ProcedureComm 业务实现已接通到静态原体：结束处理仅处理 mode 2，定时查询仅枚举 mode 1；取条目后解锁再执行业务。选定整数读取接到当前全局记录，写出接到目标模块命令入口。仍须核实际方案接口实例、管理器 +0x22 禁止位的写入者、目标工具处理体、流程 +0x1c8 可选尾函数和最终托管提交；尚未原机复现停止交错，不能宣称已证明整产品旧值写回。',nextEvidence:'按实际方案配置确定提供者与目标工具身份，追 +0x22 写入及 +0x1c8 活动目标，再以受控完整入口运行记录核对停止返回、业务发送、全局发布与托管接收的先后。'});
 scenarios.find(s=>s.id==='stop').original.push('GEX42','GEX43','GEX45');
 const cross=scenarios.find(s=>s.original.includes('GEX36'));if(cross)cross.original.push('GEX44');
 return{manifest:manifestPath,nativeMethods:m.methods.length,rawDirectCallsChecked:m.directCallsChecked,virtualTargets:2,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
};
