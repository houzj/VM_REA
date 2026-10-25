const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,source}=require('./evidence-view.cjs'),{cilReference}=require('./original-cil-reference.cjs');
module.exports=function(){
 const base='docs/architecture-recovery/result-payload-bridge.',P=base+'resources/',T=P+'VM.Core.TempVmSolution.decompiled.cs',S=P+'VM.Core.VmSolution.decompiled.cs',I=P+'VM.Core.VmIO.decompiled.cs',M=P+'VM.PlatformSDKCS.ModuleManager.decompiled.cs';
 const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),read=p=>JSON.parse(fs.readFileSync(path.resolve(root,p))),provenance=read(base+'provenance.json');
 for(const f of [...provenance.artifacts,...provenance.sources,...provenance.toolFiles,...provenance.inputs])if(sha(fs.readFileSync(path.resolve(root,f.path)))!==f.sha256)throw Error('Payload bridge evidence drift: '+f.path);
 const cil=read(base+'cil.json');let methods=0,branches=0;for(const t of cil.targets)for(const m of t.methods){const b=Buffer.from(m.ilBytes||'','hex'),offsets=new Set(m.instructions.map(i=>i.offset));if(m.status!=='decoded'||b.length!==m.ilSize||sha(b)!==m.ilSha256)throw Error('Payload bridge CIL incomplete');for(const i of m.instructions)for(const target of /BrTarget$/.test(i.operandKind)?[i.operand]:i.operandKind==='InlineSwitch'?i.operand:[]){if(!offsets.has(target))throw Error('Payload bridge branch target missing');branches++;}methods++;}if(methods!==12)throw Error('Payload bridge finite scope changed');
 const ref=(p,a,n=1)=>{const r=source(p,a,a,n),art=provenance.artifacts.find(x=>x.key===(p===M?'sdk-x64':'core'));r.evidenceType='original-managed-offline-ilspy-decompilation';r.binaryIdentity=art.sha256+' / MVID '+art.mvid;return r;},C=(type,name,token)=>cilReference('result-payload-bridge.cil.json',type,name,token),facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns,stages=['results'])=>facts.push({id,classification:'source-confirmed',from,to,behavior,representation,cache,release,refs,unknowns,stages,modes:['all']});
 add('PB01','CVmObjectBase.OnModuleResultExCallBack','ModuleManager → TempVmSolution订阅 → VmSolution回调',
 'ModuleManager构造订阅CVmObjectBase.OnModuleResultExCallBack；GetModuleResultCallBack对原生结果描述符调用PreProData，再发布OnModuleResultCallBack。TempVmSolution静态构造把此事件明确订阅到VmSolution.MModuleManager_OnModuleResultCallBack。VmSolution单参入口先调用OnModuleResultCallbackEvent，再进入ref重载按模块ID查AllModules并写IO，最后调用GetFrameMeanCount。因此外部结果事件发布发生在本入口本轮IO提交之前，不应把两个观察时点画成同一原子提交。',
 '这里传递托管结果描述符及其载荷字段；原生回调描述符经过PreProData，随后事件没有额外逐像素复制操作。',
 '事件订阅建立具体托管调用桥；观察者可能另外保存结果结构里的引用，不能仅从方法返回判断载荷不可达。',
 '回调入口没有走QueryModuleOneResult/其Release；查询容器释放规则不能移植成回调缓冲的寿命承诺。',
 [ref(M,'CVmObjectBase.OnModuleResultExCallBack += GetModuleResultCallBack;'),ref(M,'public void GetModuleResultCallBack(',4),ref(T,'mModuleManager.OnModuleResultCallBack += VmSolution.MModuleManager_OnModuleResultCallBack;'),ref(S,'internal static void MModuleManager_OnModuleResultCallBack(ImvsSdkDefine.',7),ref(S,'internal static void MModuleManager_OnModuleResultCallBack(ref ',32),C('VM.PlatformSDKCS.ModuleManager','.ctor'),C('VM.PlatformSDKCS.ModuleManager','GetModuleResultCallBack'),C('VM.Core.TempVmSolution','.cctor'),C('VM.Core.VmSolution','MModuleManager_OnModuleResultCallBack','0x06000182'),C('VM.Core.VmSolution','MModuleManager_OnModuleResultCallBack','0x06000181')],
 ['此处闭合的是CVmObjectBase已发布事件后的托管桥；VM_M_SetImageEx接收端如何形成该次事件、该次查询的描述符仍未闭合。外部事件处理器异常也不能由ref重载内部catch替代覆盖。']);
 add('PB02','结果nModuleID / pInfo[].strParamName / nExecuteCount','模块输出VmIO的批次重置、循环合并及事件闸门',
 'ref回调先按nModuleID找模块；SetValue按模块是否IVmModuleSet选择叶IO字典，再以参数名GetHashCode查找各结果目标。计数改变、自执行、禁止循环合并或VmProcedure时，先给每个目标IO更新计数、清_value/_valueList并恢复RaiseValueChanged；同批合并且isCallBack时则先禁止叶IO事件。已匹配参数调用VmIO.SetValueFromServer并置isNull=false；缺失参数走SetValueFromDefault。循环未结束继续抑制事件，结束分支才按门恢复并通知。不能把每次原生结果都等同立即显示一张新图。',
 '模块ID、参数名哈希及执行计数分别决定目的地、字段与批次；尚无按每张ImageData逐像素身份匹配的代码。',
 '清本轮IO列表只移除该层持有；循环路径可累积载荷，后续模块回调队列另保存结果与执行计数。',
 '本层清_value/_valueList与缺失参数默认化没有释放ImageData、关闭共享映射或等待显示读者。',
 [ref(S,'private static void SetValue(VmModule module,',105),C('VM.Core.VmSolution','SetValue')],
 ['参数名哈希冲突及全部Group嵌套调度不是本有限切片的验证范围；外部持有者不能由IO批次重置自动清理。']);
 add('PB03','pImageValue / pChunkValue → VmIO.SetValue','图像跳过FillData，chunk先填充；同引用入值与排队',
 '静态SetValueFromServer按ImageType直接把pImageValue传给SetValue，ChunkType传pChunkValue，ByteType传pByteValue。SetValue只在newValue是chunk描述符枚举且TypeName != image时逐项FillData；图像类型跳过这一步。随后_value=newValue，_valueList首次Add同引用，IsImageIOParent时覆盖第0项，否则追加并标记待合并。needRaiseEvent为true才进入InvokeValueChangedEvent；后者还要RaiseValueChanged且IsOpenSetValueToDisplay，调用MergeIOLoopValue后将_value、ExecuteCount、TypeName装进事件参数并排队。该桥不是把所有图像都先转成独立byte[]再通知。',
 '输入为SDK chunk描述符数组或ByteData数组；IO事件仍是类型化载荷，之后FeatureBase/Object才组装ImageData（见ON01/ON02），不是此处直接生成ImageData。',
 'image不强制FillData使其可能继续依赖共享名/借用指针；非image的FillData也不能绕开DI07/DI08已证的缓存/复制池规则。队列事件与IO可同时保持载荷可达。',
 '这里没有释放原生查询结果容器；也没有像素租约或最后读者协议。查询finally Release不能推出image事件载荷已复制或已释放。',
 [ref(I,'internal static void SetValueFromServer(VmIO io,',34),ref(I,'private void SetValue<T>(',29),ref(I,'internal void InvokeValueChangedEvent()',19),C('VM.Core.VmIO','SetValueFromServer','0x06000560'),C('VM.Core.VmIO','SetValue'),C('VM.Core.VmIO','InvokeValueChangedEvent')],
 ['MergeIOLoopValue的合并复制与特定图像父节点配置需独立跟踪；这里不把readonly、队列对象或计数相同作为像素稳定保证。'],['results','display','retirement']);
 add('PB04','SDK NativeData2Data的nParamType 4 / 5 / 6','图像描述符、chunk描述符与普通字节结果三种载荷',
 'NativeData2Data把type4的pImageValue和type5的pChunkValue分别解析为原生chunk描述符数组，再逐项new托管chunk并调用LargeObject；两者都受isFilter影响。type5在LargeObject报告isExcOccur后把nValueNum置0并break，type4虽传该标志却没有同样清计数分支。type6普通ByteData则逐项new byte[nLen]并Marshal.Copy，把完整数组存hexValue，strValue最多转换前4096字节。因此“结果里有byte[]”必须区分普通ByteData、chunk.GetData物化及ImageData.Buffer，不可混为同一个原生图像对象或统一所有权。',
 'type4/5是载荷描述符新包装，不等于其像素深拷贝；type6的hexValue在此处确实新分配复制。',
 'type6展示文本截断不截断hexValue；type4异常标志未清计数与type5清计数是不同失败保留规则，不应合并。',
 '托管数组复制与原生查询容器释放是不同动作；本方法未证明共享映射、借用像素或native生产缓存已释放。',
 [ref(M,'case 4:',70),C('VM.PlatformSDKCS.ModuleManager','NativeData2Data'),C('VM.PlatformSDKCS.ModuleManager','PreProData')],
 ['VM_M_SetImageEx的第五实参、native缓存描述符、SDK chunk的strValue/pData与ImageData.ExecuteCount之间尚缺同次生产/消费实例的直接桥证据；共享头偏移相似不能填补该缺口。'],['results','retirement']);
 return {facts,unknowns:['原生SetImageEx接收者到SDK查询/回调载荷的同帧身份、参数映射及复制/借用协议尚未静态闭合；不得把同名参数或共享头格式相似当同一实例。','查询结果容器Release、SDK chunk FillData/GetData、Feature/Object组装ImageData、显示仓清引用、native像素释放是独立边，分别引用DR/DI/ON/RC，不相互替代。'],provenance:{...provenance,selectedOriginalMethods:methods,branchTargetsChecked:branches,complete:false,runtimeExecuted:false,browserRendered:false}};
};
