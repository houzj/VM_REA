'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');const {root}=require('./evidence-view.cjs');
module.exports=function(facts,unknowns,scenarios){
 const manifestPath='docs/architecture-recovery/global-background-boundary.resources/manifest.json',m=JSON.parse(fs.readFileSync(path.join(root,manifestPath))),other=JSON.parse(fs.readFileSync(path.join(root,'docs/architecture-recovery/global-procedure-scheduling.resources/manifest.json')));
 const native=address=>{const method=m.methods.find(x=>x.address===address)||other.methods.find(x=>x.address===address);assert(method,address);return method.files.map(f=>{const b=fs.readFileSync(path.join(root,f.path));assert.equal(crypto.createHash('sha256').update(b).digest('hex'),f.sha256);const lines=b.toString().split(/\r?\n/);return {path:f.path,member:'ModuleFrame!'+method.name+' @ 0x'+address,start:1,end:lines.length,excerpt:lines.join('\n'),sha256:f.sha256,evidenceType:f.path.endsWith('.c')?'offline-ghidra-pseudocode':'offline-ghidra-assembly',binaryIdentity:m.artifact.sha256};});};
 const add=(id,title,behavior,addresses,limits)=>facts.push({id,program:'original',classification:'source-confirmed',title,behavior,refs:addresses.flatMap(native),limits,verification:{static:true,originalRuntimeExecuted:false,endToEndClosed:false}});
 add('GEX38','+0x1e8 条目来自定时项查询，不是通用子流程任务列表',
 '本轮工作前 0xe2d70 对 +0x1e8 容器首部接口调用 GetTimerItems，先给 256 字节缓冲区；若返回 0xE0000012，按返回尺寸重试。成功后按返回项登记定时回调。它构造的函数对象保存 0xe3460、项 ID 与容器地址；原 PE 的虚表 +0x10 指向 0xe3570，汇编从函数对象 +0x18 取 RCX、+0x10 取 EDX，再跳到 +8 的函数地址，因而回调实际携带容器和项 ID。0xe3460 经接口 virtual +0x18 调 ProcessItem。登记 0x80bb0 的伪码显示 void，但汇编将 0x810d0 返回的值保存在 RBP，结尾恢复到 RAX；调用者确实保存该返回值。0x810d0 在池为空时 CreateEventW(NULL,FALSE,FALSE,NULL)，否则取池中事件并 ResetEvent，所以存入容器的是事件句柄，而不是凭空生成的生产轮次号。',
 ['1800e2d70','1800e34f0','1800e3570','1800e3460','180080bb0','1800810d0'],
 ['定时项具体业务取决于提供该接口的对象；不能仅凭 ProcessItem 名称断言是工具算法、UI 呈现或全局变量写入。事件创建/复位失败分支没有由此变成成功；原机定时交错未执行。']);
 add('GEX39','正常停止前还有定时项处理与事件等待，覆盖范围须按容器限定',
 '接口挂接已在 0x119610 的所取类型条件分支定位：查询流程对象和后端接口，二者非空时直接将接口指针写入流程 +0x1e8；相应移除分支清零它，不能由两次赋值推断额外引用所有权。本轮执行图工作结束后，0xdd330 先发送 ProcessWorkFinish，再调用 0xe2fd0。后者按已登记的事件查询/移除计时管理器条目，对返回假所对应的项另通过接口调用 ProcessItem，并清理容器项；该管理器不同分支还可登记撤销标记或将句柄放回池。后续 0xdd330 才提交连接后继、调用 0xe32c0。必须注意：0xe2fd0 已清空同一容器的登记链表；若期间没有再加入条目，0xe32c0 会直接走空表退出，而不会等待刚才已经删除的定时事件。只有仍在表里的条目才会走 WaitForSingleObject(INFINITE)。最后离开流程循环到 +0x88 停止事件。等待失败分支会记录错误后继续，不会自动把异常返回变成成功排空。因此既不能以存在 Wait 调用推断原正常停止已等待所有定时回调，也不能将这个容器替代“所有子流程、原生到托管消息、UI 队列全部结束”的保证；事件究竟在回调返回前还是后置位，仍须核定时工作线程。',
 ['180119610','1800e2fd0','1800e32c0','180080dc0','180081200'],
 ['+0x1e8 内部条目生产端现已取得；前一轮笼统的“可能是子流程句柄”不能作为结论。这里只确认调用点及登记范围，不把定时事件与图结束 +0x128、流程停止 +0x88 或收尾 +0x98 混为同一屏障。']);
 add('GEX40','输入订阅登记复制描述记录；不能把登记成功解释为增加流程连接',
 'VM_M_SubScribe 的选定路径进入 0xfadb0：先在订阅管理器锁内按目标模块 ID 找或创建名称映射，再交 0xd9620。0xd9620 分配 0x110 字节，复制传入描述；按输入名称经 0xd9920 查找或插入条目，用新的对象/共享控制块替换旧记录，并释放旧引用。0xd9920 是按名称查找/创建该条目的路径。所取登记链处理的是参数来源描述，它没有在这几个体内调用流程连接登记 0xe38a0；GEX01 的生产输出绑定及 GEX33/GEX37 的执行连接仍分列。这个局部结论不证明原版整个 UI 永远不会在其它事件中维护执行连接，但已不能把 SubScribe 返回成功本身当作跨流程执行依赖已建立。',
 ['1800fadb0','1800d9620','1800d9920'],
 ['描述对象复制和旧引用释放不等于像素复制或运行值发布；0x110 是所取原生描述分配尺寸，不是所有参数结构的统一尺寸。其它编辑事件和装载恢复是否另建执行连接仍保留待核。']);
 Object.assign(unknowns.find(u=>u.id==='GU03'),{question:'停止前的 +0x1e8 已定位为 GetTimerItems 登记的定时项及其事件：本轮前登记，工作后处理剩余项并清空登记表；后续 Wait 函数可能直接空表返回。它不是通用子流程/UI 队列。清项时计时管理器的锁是否覆盖正在执行的回调、事件置位顺序、+0x1c8 可选尾部函数及最终托管提交仍待闭合。',nextEvidence:'只追定时工作线程何时置位这些事件及归还事件池，再核可选尾部函数与托管队列提交；不再重复恢复 +0x1e8 条目的注册端。'});
 for(const id of ['stop','continuous'])scenarios.find(s=>s.id===id).original.push('GEX38','GEX39');
 scenarios.find(s=>s.id==='same-flow').original.push('GEX40');
 return {manifest:manifestPath,nativeMethods:m.methods.length,rawDirectCallsChecked:m.directCallsChecked,callbackVirtualTargetChecked:true,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
};
