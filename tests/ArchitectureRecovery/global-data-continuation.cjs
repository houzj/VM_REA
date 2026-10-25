'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');const {root}=require('./evidence-view.cjs');
module.exports=function(facts,unknowns,scenarios){
 const manifestPath='docs/architecture-recovery/global-data-continuation.resources/manifest.json',m=JSON.parse(fs.readFileSync(path.join(root,manifestPath)));
 function ref(f,member,kind,anchor,count){const b=fs.readFileSync(path.join(root,f.path));assert.equal(crypto.createHash('sha256').update(b).digest('hex'),f.sha256);const lines=b.toString().split(/\r?\n/),start=anchor?lines.findIndex(l=>l.includes(anchor)):0;assert(start>=0,anchor);const end=Math.min(lines.length,start+(count||lines.length));return {path:f.path,member,start:start+1,end,excerpt:lines.slice(start,end).join('\n'),sha256:f.sha256,evidenceType:kind,binaryIdentity:kind==='captured-decompiled-definition'?'decompiled SDK declaration; native operation verified separately':m.artifact.sha256};}
 const native=address=>{const b=m.methods.find(x=>x.address===address);assert(b,address);return b.files.map(f=>ref(f,'ModuleFrame!'+b.name+' @ 0x'+address,f.path.endsWith('.c')?'offline-ghidra-pseudocode':'offline-ghidra-assembly'));};
 const add=(id,title,behavior,refs,limits)=>facts.push({id,program:'original',classification:'source-confirmed',title,behavior,refs,limits,verification:{static:true,originalRuntimeExecuted:false,endToEndClosed:false}});
 add('GEX32','图像全局值分为记录键与映射像素持有者，不能画成字典深复制整图',
 'SDK 的 IMVS_ENUM_MODULE_RESULT_TYPE 声明 4=IMAGE、5=CHUNK、6=BYTEARRAY、7=DOUBLE；这与所取绑定结构的 eGlobalVarType 分派相对照，容器的字符串操作不能直接命名为业务字符串类型。选定种类 4 路径的目的写入 0x98ae0 先交存储管理器 0xeee30；它在模块/名称/索引的缓存中选择或替换共享映射对象。已有对象的名称和容量满足条件时可以复用；否则经 0xeff60 构造对象，OpenFileMappingA 打开具名映射，MapViewOfFile 映射字节范围，对象分别保存映射句柄、视图、容量及数据长度。0xeee30 交出的数据地址为映射视图 +0x80，头部和载荷不能混为同一起点。全局记录写入体 0xf49a0 则在锁内按格式 %d_%p_%s 生成名称键，再由 0xf41e0 覆盖有效索引或追加一个字符串记录；这一步复制的是键，不能据此声称复制像素。映射复用、记录键更新、像素内容写入和最后解除映射必须分别追。',
 [ref(m.definition,'IMVS_ENUM_MODULE_RESULT_TYPE / binding payload','captured-decompiled-definition','public enum IMVS_ENUM_MODULE_RESULT_TYPE',23),...['1800eee30','1800eff60','1800f49a0','1800f41e0'].flatMap(native)],
 ['这里只恢复所选图像绑定的两种持有关系；尚未把全部类型当同一所有权模型。Open/Map 失败分支保留容量清零与后续检查，不能从对象分配就推断有可用像素。像素生产拷贝体、消费者映射及最后拥有者释放继续单列，不以局部共享引用减计数冒称全部释放。']);
 add('GEX33','原版有显式流程连接表；全局值绑定与执行连接是两种操作',
 'VM_M_ConnectProcedure(前一ID,后一ID) 进入 0xe38a0，在连接管理器锁内查询两个流程对象；任一不存在返回 0xE0000316。它向管理器 +8 的前向集合登记前一→后一，向 +0x18 的目标记录登记两组反向前驱集合，并避免已有边重复加入。VM_M_DisConnectProcedure→0xe3dd0 删除对应前向和反向集合记录。这个原体确认“流程之间还有显式的执行连接关系”，不能只画参数订阅或全局变量名。它本身登记 ID 集合，没有读取指定轮次的全局值，也没有在此等待生产者完成。所取绑定发布路径和这个连接登记入口必须分列；是否有上层自动调用它，以及运行器何时消费这些集合，仍需追实际调用者和调度体。自研 RunSolutionAsync 的生产依赖排序与完成后显式发布见 GEX12，普通全部连续入口不能自动套用该保证。',
 ['1801466c0','1800e38a0','180146820','1800e3dd0'].flatMap(native),
 ['集合含义按具体插入/删除方向描述，不将尚未追到的运行期等待写成既定事实。此处不作“全局绑定永远不会自动连流程”或“所有跨流程都同批”的结论。流程连接的错误分支与 SDK 消息失败政策仍需各自保留。']);
 const typeFact=facts.find(f=>f.id==='GEX21');typeFact.behavior+=' 后续取得的 SDK 类型声明见 GEX32：种类 7 声明为 DOUBLE；前文字符串容器操作描述的是底层表示，不可据此把其业务类型标成 STRING。';
 add('GEX34','最后强引用释放才清理映射；更新记录不等于所有图像读者退出',
 '0xeee30 的缓存元素保存对象指针和共享控制块；取用会增加强引用，替换旧元素会减少旧控制块强引用，归零时调用其 virtual +0。0xf1750 构造所选控制块，初始强/弱计数均为 1，并保存映射对象指针到 +0x10；原 PE 的虚表 0x18022e3f8 首项确为 0xf2b80。后者在对象指针非空时调用 0xf01e0，再释放对象内存。0xf01e0 检查视图 +0x30 非空且容量 +0x28 非零后调用 UnmapViewOfFile，并在句柄 +0x20 不等于 -1 时调用 CloseHandle；清理错误只记录，不构成操作必定成功的证明。因此需要区分“全局记录键已覆盖”“缓存替换并减去一份引用”“本映射对象最后强引用归零”“本视图/句柄清理被调用”四个时刻。其它读者独立打开的视图、操作系统映射对象及生产者写指针，不由此局部析构保证全部退出。自研 GEX10 的全局请求字典复制是另一套容器语义，不能直接套成该原生共享映射控制块模型。',
 ['1800f1750','1800f2b80','1800f01e0'].flatMap(native),
 ['上述生命周期按所取映射对象和控制块成立，不把最后一个局部引用等同于全进程/跨进程所有读者。未执行原机映射/关闭实验；像素实际写入、消费者打开键与淘汰触发者仍需继续核。']);
 Object.assign(unknowns.find(u=>u.id==='GU02'),{question:'消费者读当时记录的路径已确认；原版还存在显式 ConnectProcedure/DisConnectProcedure 连接表及内部依赖展开。尚未确认全局绑定是否自动维护流程连接、运行器怎样消费连接表，以及独立连续流程间的生产/消费轮次约束。',nextEvidence:'沿 0xe38a0 写入的连接管理器 +8/+0x18 集合追运行期读取者；同时核对绑定编辑调用者是否调用流程连接。内部图依赖展开 0x111880 等已取，不再把“有递归关系”当跨流程等待证据。'});
 Object.assign(unknowns.find(u=>u.id==='GU04'),{question:'所选图像绑定的名称键、共享映射缓存复用、控制块及最后强引用到 Unmap/Close 的实际路径已取。映射载荷何处复制/写入、消费者按键打开与淘汰触发者，以及其它复杂类型仍未全部闭合。',nextEvidence:'沿生产输出映射管理器返回的视图+0x80 指针追实际写入和消费者取图；缓存移除入口 0xefc60 已定位，继续核删除触发者，不重复已经确认的析构。'});
 scenarios.find(s=>s.id==='cross-flow').original.push('GEX33');scenarios.find(s=>s.id==='continuous').original.push('GEX32','GEX33');
 return {manifest:manifestPath,nativeMethods:m.methods.length,rawDirectCallsChecked:m.directCallsChecked,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
};
