'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root,source}=require('./evidence-view.cjs');
module.exports=function(facts,unknowns,scenarios){
 const manifestPath='docs/architecture-recovery/global-report-barrier.resources/manifest.json',m=JSON.parse(fs.readFileSync(path.join(root,manifestPath))),comm=JSON.parse(fs.readFileSync(path.join(root,'docs/architecture-recovery/global-procedure-comm.resources/manifest.json'))),follow=JSON.parse(fs.readFileSync(path.join(root,'docs/architecture-recovery/global-variable-followup.resources/manifest.json')));
 const native=address=>{const method=m.methods.find(x=>x.address===address)||comm.methods.find(x=>x.program==='ModuleFrame'&&x.address===address)||follow.methods.find(x=>x.address===address);assert(method,address);return method.files.map(f=>{const b=fs.readFileSync(path.join(root,f.path));assert.equal(crypto.createHash('sha256').update(b).digest('hex'),f.sha256);const lines=b.toString().split(/\r?\n/);return{path:f.path,member:'ModuleFrame!'+method.name+' @ 0x'+address,start:1,end:lines.length,excerpt:lines.join('\n'),sha256:f.sha256,evidenceType:f.path.endsWith('.c')?'offline-ghidra-pseudocode':'offline-ghidra-assembly',binaryIdentity:m.artifact.sha256};});};
 const add=(id,title,behavior,refs,limits)=>facts.push({id,program:'original',classification:'source-confirmed',title,behavior,refs,limits,verification:{static:true,originalRuntimeExecuted:false,endToEndClosed:false}});
 add('GEX46','写入禁止位已定位为关闭方案状态，不能当成停止代次',
 'VM_M_CloseSolution(0x13d440) 以同一静态管理器 0x1802b12d0 调 0x47570。该体在 +0x50 锁内把 +0x22 置 1，通知关闭开始，执行等待、清理和重建特定模块后再清回 0；原二进制日志明确写 CControlDelegate::CloseSolution、Close solution begin/finished。VM_M_AsyncCloseSolution 将同一个 0x47570 和同一个管理器地址封装为异步任务。GEX45 中 0x43120 在这个标志为真时返回 0xE000001D，原日志也明确说明 closing solution now。因此已经确认一条“关闭期间拒绝二进制写入”的真实门；它不是带 RunId 的旧结果拒收，也不是单流程 Stop 必然置位的证据。自研 GEX14 的方案过渡租约在阻止新运行后取消并等待执行，与这条原版关闭状态门逐项分开。',
 ['18013d440','18013d550','180047570','1800580c0','180043120'].flatMap(native),
 ['已定位设置/清除该位的一条真实关闭路径，不声称穷尽所有别名写入。+0x22 清零之后，后续命令是否仍对应旧方案，必须再核实例身份和最终接收者；仅在关闭窗口内拒绝不等于永久拒绝迟到结果。']);
 add('GEX47','结果上报队列：排队键与回调缓冲的持有位置不同',
 'GEX45 所达 0xd34a0 在管理器锁内维护模块/流程关联和完成事件，调用 0xd70a0 把所选 ID 与上下文键的 16 字节描述复制到队列节点，再 SetEvent(+0x10) 唤醒工作者。节点不是在此复制完整结果图像。所取选择片段 0xd3076 按原字段 <7 或 >=7 分别调用 0xd3dd0、0xd45b0；该字段的产品名称仍未知。0xd3dd0 出队后按上下文取存储并找到该 ID 的上报记录，从记录中取缓冲指针、长度及其共享控制块；调用回调前增加强引用，返回后减少引用。它调用的全局函数指针 0x1802a5260，正是 VM_M_SetResultCallback 写入的地址，用户数据为 0x1802b1188。另一工作者 0xd45b0 含直接缓冲回调与经 0xd6010 序列化后回调的路径，后者会按所需尺寸替换共享缓冲。自研 GEX26 的事件投递与 UI 缓存提交同样应分别追踪，不能把计算完成视为最终展示完成。',
 ['1800d34a0','1800d70a0','1800d3076','1800d3dd0','1800d45b0','1800d6010','180140610'].flatMap(native),
 ['0xd3076 是所取选择片段，不能仅凭其 RDI 输入推定完整线程入口 ABI。完整注册实例、回调接收者及其是否再次排队尚未运行核实。原体按上下文与 ID 取记录不等于已经证明所有轮次可互相覆盖；结果记录的生产/替换时机仍须与具体执行分支对齐。']);
 add('GEX48','停止前确有本轮上报等待，50ms 是循环间隔而非总超时',
 '原流程 0xdd330 在 ProcessWorkFinish、定时登记清理和 0xe2160 之后，立即调用 0xd53c0(上报管理器,流程 ID,上下文)，随后才继续后继调度及离开循环到 +0x88。0xd53c0 在锁内检查该键的待处理集合；若集合为空且 +0x90 中也无在途记录则返回，否则取 +0x30 的关联完成事件并 WaitForSingleObject(...,0x32)，随后重新检查集合。0x32 是每次等待 50ms，不是“50ms 超时就放弃整轮”。低字段分支工作者 0xd3dd0 先调用已注册结果回调，返回后维护待处理集合并在归零时置位 +0x30 对应事件；另一工作者 0xd45b0 也在选定回调/序列化处理后更新在途记录并通知。因此正常返回的这道门覆盖其管理器中已登记的本轮上报工作；不能再笼统声称 +0x88 前没有结果回调等待。自研 GEX14/GEX25 的等待范围是执行租约，且有限运行已证外层取消可先于慢工具退出，两种等待对象并不等同。',
 ['1800dd330','1800d53c0','1800d3dd0','1800d45b0'].flatMap(native),
 ['本条保证限定于选定管理器、上下文和登记集合正常收敛的路径。额外定时业务在集合之外的命令、已经返回的原生回调在服务/代理/托管层再次排队、尾部 +0x1c8 函数及 UI Dispatcher 不由此自动排空。原机受控停止实验未执行，不能把本轮上报等待推广为全软件迟到拒绝。']);
 const core='analysis/decompiled/VM.Core/VM.Core/VmModule.cs',sdk='analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/ModuleManager.cs';
 add('GEX49','连接的加载与编辑维护：集合事件实际调用连接 SDK',
 'VmModule.LoadNextAndPreModules 从 GetMultiModuleConnect 返回的后继 ID 填充 _NextModules/_PreModules，然后才给对应模块挂 NextModules_CollectionChanged。后续集合变化时，OldItems 逐个调用 DisConnectOneModule，维护对端 PreModules 并 UnSubModule；NewItems 逐个调用 ConnectOneModule，补齐 PreModules 并 ModuleOperate。AddConnection 撤销重做操作则在 Dispatcher 中修改 NextModules，从而走集合事件。ModuleManager 的两包装器实际把前后 ID 和 mhandle 交 IMVS_ConnectOneModule/IMVS_DisConnectOneModule，错误码非零时抛异常。由此补齐一条实际加载/编辑维护入口，连接维护与 GEX01 的输出绑定应区分；这不证明所有全局绑定都隐式建立流程依赖。自研 GEX12 的生产/消费者依赖是方案批次另行编译，独立连续入口 GEX13 不自动继承。',
 [source(core,'VmModule.LoadNextAndPreModules','protected virtual void LoadNextAndPreModules()',47),source(core,'VmModule.NextModules_CollectionChanged','protected void NextModules_CollectionChanged(',45),source(core,'VmModule.AddConnection','internal void AddConnection(',12),source(sdk,'ConnectOneModule / DisConnectOneModule','public void ConnectOneModule(',26)],
 ['这是已取反编译源码中的集合事件链，未新增原件 CIL 验证。SDK 命令到原生 ConnectModule 的这一特定桥以及流程 ID 分支仍需与 GEX33/35 分列核对；加载填表发生在本方法挂事件之前，不据此宣称每次加载都重复向 SDK 创建连接。']);
 for(const id of ['GEX08','GEX17','GEX28','GEX39','GEX41','GEX45'])facts.find(f=>f.id===id).limits.push('续追更正边界：GEX46 已将 +0x22 定位为关闭方案状态；GEX48 已恢复 +0x88 之前的本轮结果上报等待。这不等于排空额外定时业务或回调返回后再次排队的托管/UI 消息。');
 Object.assign(unknowns.find(u=>u.id==='GU03'),{question:'已补原版关闭期间的 +0x22 写入禁止门，以及 +0x88 之前的本轮结果上报等待：0xd53c0 反复检查待处理/在途集合，工作者回调返回后更新完成信号。仍未闭合的是额外定时业务目标命令的最终效果、实际回调接收者及其再次排队、+0x1c8 可选尾函数与最后托管/UI 提交资格；原机时序未运行验证。',nextEvidence:'沿 VM_M_SetResultCallback 的实际注册者和接收者继续追服务/代理队列与托管提交；以实际方案实例核验定时目标及 +0x1c8 活动函数。保留本轮上报等待已证事实，不再重复恢复该等待体。'});
 const gu2=unknowns.find(u=>u.id==='GU02');gu2.question+=' GEX49 已补已加载模块的连接填表/集合事件维护；仍未把全部全局绑定视为隐式流程依赖，也未运行独立连续交错。';
 scenarios.find(s=>s.id==='stop').original.push('GEX46','GEX47','GEX48');scenarios.find(s=>s.id==='cross-flow').original.push('GEX49');
 scenarios.find(s=>s.id==='stop').verdict='原版本轮上报有实际等待：已登记结果回调返回并清理集合后才能正常越过 0xd53c0；关闭方案期间另有 +0x22 拒写门。额外定时业务、回调再次排队及最终托管/UI 提交仍未全闭合。自研有限队列探针已复现外层取消先于慢工具退出，未据此宣称完整产品旧结果污染。';
 return{manifest:manifestPath,nativeMethods:m.methods.length,rawDirectCallsChecked:m.directCallsChecked,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
};
