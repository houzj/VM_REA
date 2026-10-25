'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');const {root}=require('./evidence-view.cjs');
module.exports=function(facts,unknowns,scenarios){
 const manifestPath='docs/architecture-recovery/global-procedure-scheduling.resources/manifest.json',m=JSON.parse(fs.readFileSync(path.join(root,manifestPath))),old=JSON.parse(fs.readFileSync(path.join(root,'docs/architecture-recovery/global-variable-followup.resources/manifest.json')));
 const native=(address,anchor,count)=>{const method=m.methods.find(x=>x.address===address)||old.methods.find(x=>x.address===address);assert(method,address);return method.files.map(f=>{const bytes=fs.readFileSync(path.join(root,f.path));assert.equal(crypto.createHash('sha256').update(bytes).digest('hex'),f.sha256);const lines=bytes.toString().split(/\r?\n/),start=anchor&&f.path.endsWith('.c')?lines.findIndex(l=>l.includes(anchor)):0;assert(start>=0,anchor);const end=Math.min(lines.length,start+(count&&f.path.endsWith('.c')?count:lines.length));return {path:f.path,member:'ModuleFrame!'+method.name+' @ 0x'+address,start:start+1,end,excerpt:lines.slice(start,end).join('\n'),sha256:f.sha256,evidenceType:f.path.endsWith('.c')?'offline-ghidra-pseudocode':'offline-ghidra-assembly',binaryIdentity:m.artifact.sha256};});};
 const add=(id,title,behavior,refs,limits)=>facts.push({id,program:'original',classification:'source-confirmed',title,behavior,refs,limits,verification:{static:true,originalRuntimeExecuted:false,endToEndClosed:false}});
 add('GEX35','流程连接确实参与运行：本轮工作返回后检查并提交后继',
 '0xdd330 在本轮执行图返回、结果计时及流程结果更新之后调用 0xe4080，并传入当前流程 ID、上下文和运行描述。0xe4080 从 GEX33 所述连接管理器 +8 复制该流程的后继集合；对每个后继调用 0xe4330，把本次完成的前驱 ID 从其等待集合中处理掉。只有检查返回 0 且剩余数量为 0，才交 0x128200 提交后继。0x128200 的普通指定流程分支按 ID 找执行对象，然后调用 0xdcc80；它不是仅发送 UI 状态或写一条日志。随后当前 0xdd330 还调用 0xe32c0，后者逐个等待当前 +0x1e8 容器里的句柄并清理容器；这些句柄的具体生产者仍需分别定位，不能仅凭等待体称为所有子流程或 UI 消息都已排空。',
 [...native('1800dd330','if ((_DAT_1802b3120',30),...native('1800e4080'),...native('180128200')],
 ['只覆盖所取显式连接和正常完成路径；错误/提前退出可绕过该后继通知点。此处源码证明实际提交路径，不是已执行原机并发实验。']);
 add('GEX36','等待的是前驱 ID 集合，并非逐生产者轮次的快照配对',
 '0xe4330 在连接管理器锁内，按后继 ID 取记录，从记录 +0x10 的等待集合删除本次完成的前驱 ID，再把集合数量写给调用者。集合空时立即用记录首部的完整前驱集合恢复等待集合，然后返回；这个重置早于 0xe4080 调用 0x128200。所取方法的参数是后继 ID、完成前驱 ID 和剩余数量指针，没有传入该前驱的 ExecuteCount、结果版本或全局值快照。重复完成同一前驱只重复删除同一个 ID，不保留第二份完成次数。结合 GEX22 的全局消费者读取当前记录，可知这个完成集合门本身不提供“各生产者同一轮”的数值快照；若一条生产链再次更新，消费者究竟读到哪个值仍由实际取值时刻及其它同步条件决定。下表是根据这些已取分支推导的条件时序，不是原机运行日志。',
 [...native('1800e4330'),...native('1800e4570')],
 ['例子假定显式 A→B、C→B，两个前驱能够独立连续完成，且无其它阻止再次生产的门；不声称已在原机实现该交错。完整前驱集的恢复不是数据回滚。全局写入与读取之间仍可有其它生产更新，不能把“触发 B 时最新”当成“B 实际读取时最新”。']);
 facts.find(f=>f.id==='GEX36').operationTrace={classification:'source-derived-conditional-sequence',runtimeExecuted:false,columns:['发生的操作','B 尚待完成的前驱','对启动与数据的影响'],rows:[['连接已建立','A、C','等待前驱完成'],['A 第 1 次完成并发布 X=1','C','删掉 A，尚不提交 B'],['A 第 2 次完成并发布 X=2','C','再次删除 A 不增加完成次数；全局记录可已变为 2'],['C 第 1 次完成','先清空，再恢复为 A、C','本检查允许尝试提交 B；没有保存 A 第 1 次的值'],['B 随后消费全局 X','与完成集合分开','按 GEX22 读取当时记录；不能仅凭连接断言 X=1'] ]};
 add('GEX37','入口和失败边界：全部流程从根启动，后继启动失败未在此恢复完成票据',
 '0x128200 的所取 ID=10999 分支枚举流程，对具备相应可运行条件的项经 0xe3fe0 检查；该检查遍历连接表，发现该流程位于任一后继集合即返回真，因此这条全部流程入口会跳过有前驱的项，让连接完成通知负责后继提交。指定普通流程 ID 的分支则直接尝试 0xdcc80。0xdcc80 有运行状态和模式准入，并可返回错误；例如已处于连续标志分支会返回 0xE0000311。0xe4330 已在提交前重置等待集合，0xe4080 未按启动结果回滚刚消费的前驱完成标记。不能把“集合已凑齐”描述成后继必定成功执行。另一个编辑入口 VM_M_ConnectModule 在普通模块查找不齐、且两个 ID 都能查到流程对象时转交 0xe38a0；不能因此称任意模块订阅或全局输出绑定都会自动增加流程执行连接。自研 RunSolutionAsync 的有序发布、全部单次 Task.WhenAll 与全部连续入口仍按 GEX12/GEX13 分开比较。',
 [...native('18013ade0'),...native('18013b130'),...native('1800e3fe0'),...native('180128200'),...native('1800dcc80','if ((_DAT_1802a56bc',50)],
 ['10999 的路径仅按该原体说明，不代替所有 SDK/UI 入口。准入失败、连续目标已在运行和异常等待需要操作验证；不把尚未观察的票据丢失、死等或迟到写回报告成已复现产品缺陷。']);
 facts.find(f=>f.id==='GEX33').limits.push('运行期使用已继续取到，见 GEX35/GEX36；后继提交与等待集合重置不再是完全未知。');
 Object.assign(unknowns.find(u=>u.id==='GU02'),{question:'显式流程连接的登记→本轮完成通知→删除等待前驱 ID→清空后重置→后继提交已经静态接通。该门不保存生产者数值轮次快照；独立连续交错时读当前记录。仍需确认全局绑定/订阅的上层是否维护流程连接，以及选定原机模式是否允许所列交错。',nextEvidence:'只追全局绑定/订阅编辑到连接维护的实际调用者，并进行选定连续模式交错验证。已取的连接管理器执行体不再重复恢复；本地完成集合不得替代全链版本一致性结论。'});
 Object.assign(unknowns.find(u=>u.id==='GU03'),{nextEvidence:'继续当前流程 +0x1e8 句柄容器的生产端及 +0x1c8 可选收尾目标；0xe4080 的后继提交和 0xe32c0 的逐句柄等待已取，明确它们覆盖的任务再核最终提交。原机停止/迟到实验仍未执行。'});
 for(const id of ['cross-flow','continuous'])scenarios.find(s=>s.id===id).original.push('GEX35','GEX36','GEX37');
 scenarios.find(s=>s.id==='cross-flow').verdict='原版显式流程连接会在前驱本轮工作完成后消费前驱 ID 集合，集合清空后重置并尝试提交后继；它不保存按生产轮次配对的全局值快照。消费者读取当时全局记录，所列快慢生产者交错仍待原机验证。自研方案批次的依赖排序和完成后发布，与独立全部单次/连续入口逐项分列。';
 return {manifest:manifestPath,nativeMethods:m.methods.length,rawDirectCallsChecked:m.directCallsChecked,originalRuntimeExecuted:false,conditionalTraceIsRuntimeEvidence:false,fullExecutionContractClosed:false};
};
