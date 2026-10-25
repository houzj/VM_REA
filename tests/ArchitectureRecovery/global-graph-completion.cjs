'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');const {root}=require('./evidence-view.cjs');
module.exports=function(facts,unknowns,scenarios){
 const manifestPath='docs/architecture-recovery/global-graph-completion.resources/manifest.json',m=JSON.parse(fs.readFileSync(path.join(root,manifestPath)));
 const refs=addresses=>addresses.flatMap(address=>{const body=m.methods.find(b=>b.address===address);assert(body,address);return body.files.map(f=>{const b=fs.readFileSync(path.join(root,f.path));assert.equal(crypto.createHash('sha256').update(b).digest('hex'),f.sha256);const lines=b.toString().split(/\r?\n/);return {path:f.path,member:'ModuleFrame!'+body.name+' @ 0x'+address,start:1,end:lines.length,excerpt:lines.join('\n'),sha256:f.sha256,evidenceType:f.path.endsWith('.c')?'offline-ghidra-pseudocode':'offline-ghidra-assembly',binaryIdentity:m.artifact.sha256};});});
 const add=(id,title,behavior,addresses,limits)=>facts.push({id,program:'original',classification:'source-confirmed',title,behavior,refs:refs(addresses),limits,verification:{static:true,originalRuntimeExecuted:false,referenceRuntimeExecuted:false,endToEndClosed:false}});
 add('GEX27','执行图的完成条件：前驱任务返回后才递减后继计数',
 '流程对象在 0xe2720 中准备执行器；0x10ae80 的构造路径经 0x20ae0 建立起点和结束节点，再由 0xdcba0 将共享执行器保存到流程 +0xc8。0x20ae0 为结束节点创建手动复位、初始无信号事件，句柄在结束节点 +0x128；原 PE 虚表字节确认外层 +8 为 0x20dd0、结束节点 +8 为 0xb97e0。加边 0x1dfd0 增加目标节点 +0x20/+0x24 前驱计数。调度包装器 0xffd00 在所选正常分支先调用任务 virtual +8，返回后才遍历后继并在锁内递减后继 +0x20；归零后派发后继，类型 2 的结束节点走直接调用。结合 GEX02 已取的模块任务 0xb8740：算法成功后的绑定发布 0x93b20 位于任务返回前，因此该图中受上述前驱关系覆盖的正常任务，其绑定写入先于相应后继就绪。不能将此推为所有跨流程生产者自动属于同一依赖图。',
 ['1800e2720','18010ae80','1800dcba0','180020ae0','18001dfd0','1800ffd00'],
 ['虚表目标按原始 PE 重核，间接调用的实际对象类型仍以所选构造链为范围。图成员、其它执行分支、算法内部另起且未挂接的异步工作，不由该局部先后证明全部排空。']);
 add('GEX28','两个完成事件不同：图结束事件先于流程停止事件，尾部回调仍在后面',
 '结束任务 0xb97e0 先更新节点状态，再 SetEvent(结束节点 +0x128)。外层虚方法 0x20dd0 调 0xb9810，后者 WaitForSingleObject(+0x128,INFINITE)，随后 ResetEvent 并返回；正常等待成功意味着所选执行图已到结束节点。结合 GEX17 的流程工作循环，图工作返回并离开循环后，0xdd330 才置流程 +0x88 停止事件，之后仍执行尾部状态通知、可选 +0x1c8 回调及 +0x98 置位。因此停止等待返回与尾部回调全部结束不是同一时刻。此外 0xb9810 对非成功等待只按条件记录错误，仍走 ResetEvent/返回；0x20dd0 固定返回 0，不能把任意返回都解释为正常图完成。结束节点 SetEvent 失败也仅取错误码，不能据此声称异常路径有成功屏障。',
 ['180020dd0','1800b9810','1800b97e0'],
 ['这是静态正常分支和错误分支的精确顺序，尚非原机停止竞态实验。UI 消息投递、尾部回调中的最终提交及外部异步工作是否带代次拒收，继续留在 GU03。']);
 const stop=scenarios.find(s=>s.id==='stop');stop.original.push('GEX27','GEX28');stop.verdict='原版正常图路径：模块成功后的绑定发布在任务返回前，前驱完成后才派发后继，结束节点置 +0x128 后图等待返回；随后流程 +0x88 停止等待可返回，但尾部通知与收尾回调仍在后面。异常等待不保证图完成。自研有限取消探针已证明私有队列返回时慢工具仍可运行；产品 UI／全局迟到写回尚未运行验证。';
 scenarios.find(s=>s.id==='same-flow').original.push('GEX27');
 Object.assign(unknowns.find(u=>u.id==='GU03'),{question:'正常执行图的前驱递减、结束节点 +0x128 和外层等待已连到流程停止 +0x88：受该图覆盖的成功绑定写入先于正常图完成；+0x88 之后仍有尾部通知及回调。异常等待返回、未挂接异步工作、原生到托管消息排空及最终提交拒收仍未闭合。',nextEvidence:'优先继续实际收尾回调目标与托管提交队列，明确最后一次写入和停止返回的先后；图等待失败路径单列，原机有界停止实验尚未执行。'});
 return {manifest:manifestPath,nativeMethods:m.methods.length,rawDirectCallsChecked:m.directCallsChecked,virtualTargetsChecked:m.virtualTargets.length,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
};
