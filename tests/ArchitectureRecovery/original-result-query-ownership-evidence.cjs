const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,source}=require('./evidence-view.cjs'),{cilReference}=require('./original-cil-reference.cjs');
module.exports=function(){
 const B='docs/architecture-recovery/result-query-ownership.',P=B+'resources/',M=P+'VM.PlatformSDKCS.ModuleManager.decompiled.cs',A=P+'VM.PlatformSDKCS.ServerSDKAPI.decompiled.cs',R=P+'VM.PlatformSDKCS.ModuleResultManager.decompiled.cs',Q=P+'VM.Core.ModuleParam.decompiled.cs',D=P+'VM.Core.DynamicOutput.decompiled.cs',V=P+'VM.Core.VmModule.decompiled.cs',sha=b=>crypto.createHash('sha256').update(b).digest('hex'),read=p=>JSON.parse(fs.readFileSync(path.resolve(root,p))),provenance=read(B+'provenance.json');
 for(const f of [...provenance.artifacts,...provenance.sources,...provenance.toolFiles,...provenance.inputs])if(sha(fs.readFileSync(path.resolve(root,f.path)))!==f.sha256)throw Error('Result query ownership drift '+f.path);
 const cil=read(B+'cil.json');let methods=0,branches=0;for(const t of cil.targets)for(const m of t.methods){const b=Buffer.from(m.ilBytes||'','hex'),offsets=new Set(m.instructions.map(i=>i.offset));if(m.status!=='decoded'||b.length!==m.ilSize||sha(b)!==m.ilSha256)throw Error('Result query CIL incomplete');for(const i of m.instructions)for(const target of /BrTarget$/.test(i.operandKind)?[i.operand]:i.operandKind==='InlineSwitch'?i.operand:[]){if(!offsets.has(target))throw Error('Result query branch target');branches++;}methods++;}if(methods!==17)throw Error('Result query finite scope changed');
 const ref=(p,a,n=1)=>{const r=source(p,a,a,n),art=provenance.artifacts.find(x=>x.key===(p===Q||p===D||p===V?'core':'sdk-x64'));r.evidenceType='original-managed-offline-ilspy-decompilation';r.binaryIdentity=art.sha256+' / MVID '+art.mvid;return r;},C=(type,name,token)=>cilReference('result-query-ownership.cil.json',type,name,token),MC=(name,token)=>C('VM.PlatformSDKCS.ModuleManager',name,token),facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns,stages=['results','retirement'])=>facts.push({id,classification:'source-confirmed',from,to,behavior,representation,cache,release,refs,unknowns,stages,modes:['all']});
 add('RQ01','ModuleParam字符串/二进制参数读取','GetParamValue字符串返回与GetBinaryData固定托管数组的独立通道',
 'ModuleParam字符串索引getter在Parent未Dispose时，普通参数调用GetParamValue并要求错误抛出；VmModuleSet且键以%开头则走动态运行参数查询。ModuleManager.GetParamValue无长度重载传4096给显式长度重载，再把ref string交IMVS_GetParamValue；它没有查询模块结果或构造ImageData。ModuleParam二进制索引另走GetBinaryData：普通路径先查长度、new byte[length]并固定该数组，把数组地址交IMVS_GetBinaryData写入；调用返回后解除固定，finally再次检查保证异常也解除固定。native错误时把data置null。参数二进制读取不能被画成SDK结果chunk/共享图像名返回的同一路径。',
 'GetParamValue输出string；GetBinaryData(ref byte[])输出此方法分配的托管数组，pin只是临时地址固定，不是像素永久租约。',
 '成功返回数组由调用者继续持有；参数读取自身不写VmIO结果缓存，也不调用PB回调桥。',
 'GCHandle.Free仅解除这次数组固定，不释放托管数组；此ref byte[]分支没有调用历史查询容器Release。',
 [ref(Q,'public virtual string this[string key]',23),ref(Q,'byte[] IUserBytesData.this[string key]',27),ref(M,'public void GetParamValue(uint nModuleID, string strName, int ',16),ref(M,'public int GetBinaryData(uint moduleId, string paramName, ref byte[]',52),C('VM.Core.ModuleParam','get_Item'),MC('GetParamValue','0x0600007f'),MC('GetParamValue','0x06000080'),MC('GetBinaryData','0x060000a5')],
 ['动态参数、out IntPtr二进制重载以及native参数存储的寿命不由此ref byte[]路径代替；未执行native查询。'],['input','results','retirement']);
 add('RQ02','QueryModuleHistoryResultData主动查询','PreProData托管结果数组与正常尾释放；不是无条件像素深拷贝',
 '此方法明确以bResBuffFirst=false、bNeedImage=true、bUseResStructMap=false查询历史页。成功且根指针非零后读取结构、展开原结果数组，为每项调用ModuleManager.PreProData，再调用IMVS_ReleaseDynamicMemory释放历史结果列表，错误码非零则抛异常，最后返回托管结果数组。尽管注释称“深拷贝”，方法没有逐图强制GetData或复制共享映射：图像是否借用、保留共享名或已物化仍由DI04/DI07中LargeObject与chunk分支决定。预处理或结构展开抛异常时，该体没有finally保证走到正常尾Release。',
 '新分配的是托管结果描述符数组；其内部image/chunk载荷的所有权必须逐分支解释，不等于整批像素都独立。',
 '本方法直接返回转换结果，不把它提交给VmSolution结果回调或VmIO；调用方保存数组可独立延长其中引用的可达性。',
 '正常尾请求释放查询根容器；返回后的共享名、复制池指针与外部像素的有效性不能由此Release成功推出。异常路径是否有native外层回收仍未知。',
 [ref(M,'public ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX_Data[] QueryModuleHistoryResultData(',32),MC('QueryModuleHistoryResultData')],
 ['native释放种类的递归释放范围、借用像素在根容器释放后的有效期及异常外部清理尚未闭合；不能仅因无finally就声称原机已发生泄漏。']);
 add('RQ03','历史分页查询 / QueryModuleOneResult','显式所有权页句柄与映射结构返回的不同契约',
 '历史分页重载以bUseResStructMap=false查询，成功时把根pQueryModuResInfoList交ref pstModuleResBuff，并返回浅结构；成功释放才把传入ref置零（容器Release的非幂等边界见DR02）。QueryModuleOneResult则请求index0、count1，只显式传bResBuffFirst（默认true）；ServerSDKAPI默认bNeedImage=true、bUseResStructMap=true。OneResult成功且nNum==1时只返回array[0]，其余正常情况返回null；未把本地查询根指针传给调用方，也没有本体显式Release。两条路径的映射模式不同，不能要求或推定调用方用同一容器释放协议。',
 'OneResult返回包含指针的原始结果结构，尚未PreProData；历史页另向调用者提供查询根句柄。原始结构不是ImageData或已独立byte[]。',
 'OneResult本体不向VmIO提交结果，也不发布PB01事件；映射结构可能由native结果缓存管理，其实际有效期未在托管体说明。',
 '本体没有显式Release并不足以判定泄漏，也不足以保证可永久保留；必须恢复bUseResStructMap=true的native所有权契约。',
 [ref(M,'public ImvsSdkDefine.IMVS_QUERY_MODULE_RES_INFO_LIST? QueryModuleHistoryResult(uint nModuleID, int nIndex, uint ',26),ref(M,'public int ReleaseModuleHistoryResult(',12),ref(M,'public ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_LIST_EX? QueryModuleOneResult(',27),ref(A,'public static int IMVS_QueryModuleHistoryResult(',4),MC('QueryModuleHistoryResult','0x0600008b'),MC('ReleaseModuleHistoryResult'),MC('QueryModuleOneResult'),C('VM.PlatformSDKCS.ServerSDKAPI','IMVS_QueryModuleHistoryResult')],
 ['映射结果是否借用缓存、何时换帧/覆盖、是否按模块保存根容器及并发查询失效规则仍是native未知；不能把映射true等同历史页false的malloc所有权。']);
 add('RQ04','DynamicOutput.GetOutputImage / GetOutputImageV2','主动OneResult物化字节与已有IO载荷按模式构造SDK图像两分支',
 '旧GetOutputImage先验证输出IO存在，再QueryModuleOneResult→PreProData；按type4及去%后的参数名找图像，nValueNum>0时只取第0个chunk.GetData(false)，把所得byte[]及长度直接赋ByteArrayData。V2不走该旧入口：它调用VmModule.GetImageByteData读取IMAGE父IO的第0子项chunk及宽高格式子项；浅复制模式只在MemoryType0/2时取共享名，其余模式调用GetData(false)。V2浅复制新建ImageBaseData、填宽高格式，有共享名才InitImageObjBySharedMem（new SharedMemoryItemVM(name,模块ExecuteCount)）；非浅复制用得到的数组构造ImageBaseData。IO.Value可能先触发DR01按计数刷新，不能把V2误画成无条件重新OneResult或PB回调路径。',
 '旧入口输出ByteArrayData.pData；V2输出SDK ImageBaseData。二者都不是UI Apps.Data.ImageData，也不直接接RenderDataCenter。',
 '旧入口对GetData(false)返回值不再额外复制；V2浅模式保持共享名/映射对象，深模式使用字节数组，后续SDK指针、pin及映射寿命仍分别见DI01–03。浅模式遇非0/2的MemoryType可返回仅填几何元数据、未挂共享对象的ImageBaseData，不能虚构自动回退复制。',
 '这些调用者没有释放查询页容器；旧入口使用映射true的OneResult，不能套用历史分页Release。V2构造对象不等于复制/释放原IO载荷；SDK Dispose解除pin与共享映射关闭仍是分开的机制。',
 [ref(D,'public ByteArrayData GetOutputImage(',39),ref(D,'public ImageBaseData GetOutputImageV2(',34),ref(V,'public IVmImageData GetImageByteData(',35),ref(R,'public void InitImageObjBySharedMem(',4),C('VM.Core.DynamicOutput','GetOutputImage'),C('VM.Core.DynamicOutput','GetOutputImageV2'),C('VM.Core.VmModule','GetImageByteData'),C('VM.PlatformSDKCS.ModuleResultManager','InitImageObjBySharedMem')],
 ['原机同次宽高格式与共享图像的帧一致性、模块ExecuteCount与共享头当前计数的完整校验规则仍需运行/native证据；名字相同不证明同一实例。'],['results','display','retirement']);
 return {facts,unknowns:['参数字符串、参数二进制、历史页结果、映射OneResult、回调和IO缓存读取是不同入口；只有具体调用证据才串联。','查询根容器释放与pixel最后读者释放没有在本切片形成统一租约；native映射true的所有权必须独立恢复。'],provenance:{...provenance,selectedOriginalMethods:methods,branchTargetsChecked:branches,complete:false,runtimeExecuted:false,browserRendered:false}};
};
