// Original non-image data evidence. Only static files/assembly hashes are read by require().
const fs=require('fs'),path=require('path'),crypto=require('crypto'),{source}=require('./evidence-view.cjs');
const root=path.resolve(__dirname,'../..'),dir='docs/architecture-recovery/original-nonimage-data.resources/',sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const artifacts=[{path:'D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlParser.dll',sha256:'78ec7ae1e5adefff29197099c7109b153a3f75d092b7c3e9dc253d8c1b25a8d1',types:['VariableManager']},{path:'D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VM.Core.dll',sha256:'936763f6c8a1bf9fcf59f22acebca8feb09b5f3d5701ae77c82bdc81e626190d',types:['VmIO']},{path:'D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/VM.PlatformSDKCS.dll',sha256:'34c0c90b00cb75a701685b96670b6490f61376f62eacc0ab29d240157db401c1',types:['ModuleManager']}];
 for(const a of artifacts)if(sha(fs.readFileSync(a.path))!==a.sha256)throw Error('Original DLL differs: '+a.path);
 const v=dir+'VariableManager.cs',io=dir+'VmIO.cs',sdk='docs/architecture-recovery/parameter-sdk-bridge.resources/ModuleManager.cs';
 const ref=(p,a,n)=>({...source(p,path.basename(p),a,n),evidenceType:'static-original-managed-decompilation',binaryIdentity:p===v?artifacts[0].sha256:p===io?artifacts[1].sha256:p===sdk?artifacts[2].sha256:'not revalidated in this slice'});
 const cilPath='docs/architecture-recovery/variable-manager-cil-evidence.json',cilBytes=fs.readFileSync(path.join(root,cilPath)),cil=JSON.parse(cilBytes);
 for(const m of cil.targets[0].methods){const b=Buffer.from(m.ilBytes??'','hex');if(m.status!=='decoded'||b.length!==m.ilSize||sha(b)!==m.ilSha256)throw Error('Retained VariableManager CIL differs');}
 if(cil.manifest.artifacts[0].sha256!==artifacts[0].sha256)throw Error('Variable CIL artifact mismatch');
 const facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns,stages=['variable','transfer','results','retirement'])=>facts.push({id,classification:'source-confirmed',from,to,behavior,representation,cache,release,refs,unknowns,stages,modes:['all']});
 add('VD01','原VariableManager绑定的DataSource模块','VarCount/GlobalValueList快照与显示值',
 '构造函数只在dataSource为VmModule时保存DataSource。GetAllVar(ref IMVS_VARIABLE[])通过DataSource.Params先取VarCount：只有正整数才读GlobalValueList，且非空、字节长度可整除IMVS_VARIABLE大小才覆盖调用者ref数组；不是数量相等校验，其他分支保留旧ref。List<VarItem>重载逐条附加元数据，int/float将native数组复制到byte[]再BlockCopy成数值数组，但显示Value只取第0项；string读取描述数组后，仅复制第0项指向的字符串payload作显示。不能把Value文本当成完整数组。',
 '变量身份=所属模块+变量名/Guid；VarItem的Value是展示字符串，ValueArray另有object属性，但所选GetAllVar没有把完整数值数组赋给ValueArray。',
 'GetAllVar是读取快照而非本地变量存储实现；ref数组未覆盖分支可保留调用者旧快照，list重载附加而非自动Clear。',
 '已复制的标量数组/文本由GC持有；IMVS_VARIABLE内嵌pointer指向的原payload所有权未由结构快照获得。',
 [ref(v,'public VariableManager(object dataSource)',8),ref(v,'public void GetAllVar(ref IMVS_VARIABLE[] vARIABLEs)',14),ref(v,'public void GetAllVar(ref List<VarItem> varList)',58),ref('analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.Variable/VarItem.cs','public string Value { get; set; }',5)],['native变量模块GlobalValueList的生产、稳定快照/锁与内嵌payload回收协议缺体；不能由外层byte[]推断内层pointer已深拷贝。']);
 add('VD02','原SetVarIntValue/SetVarFloatValue数组写入口','ModifyVar描述+临时HGlobal数值块',
 '两个setter拒绝空名、null和空数组，修复名字后设置VAR_VALUE动作、类型文本int/float及nValueNum。逐数值BitConverter.GetBytes→List<byte>→ToArray，再AllocHGlobal并Marshal.Copy，pBinaryValue指向该临时块；序列化的是IMVS_MODIFY_VAR描述，调用SetBinaryData("ModifyVar",描述bytes)。进入try后finally释放临时块，但分配、复制、StructToBytes位于try之前，不能宣称所有构造异常都会释放。',
 '调用载荷包含类型/数量和native地址，而非只有独立的描述byte[]；这些typed setter的单元素输入仍按nValueNum=1数组处理。',
 '调用期生成字节数组与HGlobal是实际复制；此包装没有长期缓存该HGlobal。',
 'finally正常/调用异常路径释放临时数值块，要求native消费或复制在该生命周期内；缺callee不能声称异步保留这个地址安全。',
 [ref(v,'public void SetVarIntValue(string varName, int[] intList',43),ref(v,'public void SetVarFloatValue(string varName, float[] floatList',42),ref(v,'private void SetBinaryData(string paramName, byte[] value)',8)],['ModifyVar的native接收、解析、同步复制/异步消费、底层旧值释放及多线程原子性未取得原体。']);
 add('VD03','原字符串变量写入口','短文本SDK路径、长文本binary路径与string[]嵌套块',
 'SetVarValue按UTF8字节数<=256选择ServerSDKAPI.IMVS_SetParamValue，超过256改用SetBinaryData与补零UTF8数组，键的固定前缀为GlobalScipt-（按原拼写）。SetVarStringValue则为每个字符串创建补零UTF8数组、独立HGlobal，写入ByteDataArray(pData/nDataLen/nDataSize)；再为整个描述数组分配HGlobal，最后把其地址写到ModifyVar。finally逐项Free再Free外层；分配与序列化在try之前。',
 '字符串scalar的直接文本/binary路径与string[]的双层pointer描述不同；nDataLen包含补零UTF8字节长度。',
 '这些是临时复制；字符串[]没有在此包装中保存永久native地址。',
 'finally释放每项及描述块，不代表native保存的旧字符串缓存也已释放；try之前部分分配失败的清理不能假定完备。',
 [ref(v,'public int SetVarValue(string varName, string varValue)',12),ref(v,'public void SetVarStringValue(string varName, string[] stringList',65)],['精确键见源引用；native文本/二进制两入口的一致性、截断/编码与存储深拷贝需接收方原体。']);
 add('VD04','原ModuleManager非图像结果转换','VmIO保留数值副本或托管字符串数组引用',
 'NativeData2Data对status=0的int/float构造单元素零值数组，字符串构造单元素空字符串数组；非0调用相应pointer→数值数组helper，字符串将固定UTF8 byte字段逐项解码成新string数组。VmIO.SetValueFromServer对int/float再Array.ConvertAll为nullable数组，对string直接把pStringValue传SetValue。SetValue写_value=newValue并将同一个对象追加或替换到_valueList，不能把这一步描述为统一Clone全部payload。SetNoneValueWithoutRaiseEvent只清_value，没有清_valueList。',
 '结果nParamType1/2/3、nValueNum、nValueStatus与数据数组分别保存；status=0支路采用默认单元素数组，没有在这条分支构造与原数组等长的全null。',
 '_value与循环_valueList可能同时持有同一数组；非IsImageIOParent且列表非空的所选分支继续追加并设needMergeIOLoopValue。',
 '清一个字段仅移除一条托管引用；有_valueList或其他消费者时不能宣称立即GC，更不能宣称native payload释放。',
 [ref(sdk,'int[] pIntValue = new int[1];',3),ref(sdk,'targeData.pIntValue = ExternalCallHelper.IntPtr2Ts<int>',38),ref(io,'internal void SetValueFromServer(ImvsSdkDefine.IMVS_MODULE_RESULT_INFO_EX_Data',31),ref(io,'private void SetValue<T>(IEnumerable<T> newValue',28),ref(io,'internal void SetNoneValueWithoutRaiseEvent()',7)],['数值转换helper的全部异常/长度合法域、循环列表的完整退休门与native结果产生规则不由本切片闭合。']);
 add('VD05','原PointSet变量接口','ByteDataArray复制→MatchPoint列表；专用setter为空体',
 'GetVarPointset读取VarValue-null-加名字的binary结构，检查变量类型pointset；若nValueNum非0，从pByteValue取ByteDataArray[]，仅第0块nDataLen非0才复制其pData到byte[]。随后按Marshal.SizeOf(MatchPoint)步进，将每段复制到单个临时HGlobal再PtrToStructure并Add到MatchOutlinePoints；正常末尾Free临时块。对应SetVarPointset(string,List<MatchOutline>)本轮新静态提取确认为空方法，不能从getter或POINT坐标数组的setter补造pointset写入协议。',
 'pointset在该变量接口由binary MatchPoint序列表示；POINT的x/y float[]是另一结构，不能混用。',
 '读取产生独立byte[]和托管MatchPoint列表；只处理第0个ByteDataArray，不因nValueNum>1假定全块已合并。',
 '临时块正常路径释放且没有finally；未见按MatchPoint大小整除检查，异常退出不保证Free；生产者原块仍独立。',
 [ref(v,'public MatchOutline GetVarPointset(string varName)',38),ref(v,'public void SetVarPointset(string varName, List<MatchOutline>',4)],['native pointset生产者序列格式/版本、MatchPoint ABI和写入入口仍缺体；原空setter不等于整个产品所有pointset入口均不支持。']);
 add('VD06','原DeleteVar/删除IO入口','native删除命令与托管解绑/节点移除分开执行',
 'DeleteVar先验证非空并RepairVarName，withVar=true才向DataSource.Params["DeleteVar"]写变量名，再调用DeleteIO；withVar=false仍做托管清理。DeleteIO取得输出的Index/TypeName，按可见输出且index更大的规则递减索引，再尝试UnBindGlobarVarItem和RemoveSelf，两个异常各记录后继续ChangeToSendReceiveData与SetRunParamBind。删除不是一个统一Dispose(payload)动作，原顺序也不是先全验证再原子提交。',
 '变量名删除命令、订阅/绑定关系、输出节点、通信索引是不同载荷/生命周期域。',
 'RemoveSelf与解绑不证明消费者已经放弃此前数组或结果缓存；native命令发送和UI移除也不能合并成同一成功状态。',
 '本段没有FreeHGlobal变量旧值或遍历数组释放；native删除是否真正退役payload及最后读者另缺原体。',
 [ref(v,'public void DeleteVar(string varName',14),ref(v,'private void DeleteIO(string varName',45)],['DataSource.Params删除命令在native的检查/存储释放/绑定失效语义未证；不要由节点移除推出像素或非图像数组已释放。']);
 add('VD07','原ModifyType入口','组合类型重建或标量改TypeName，然后发送ModifyVar',
 'ModifyType验证名字/类型、查现有输出及支持类型，相同类型直接返回。新旧任一为组合枚举时先DeleteIO(isChangeIndex:false)，按新类型重建并恢复Index；否则先修改现有TypeName，再尝试解绑、更新通信与运行参数绑定。随后序列化VAR_TYPE动作、名称和目标类型发送ModifyVar；新组合类型还附带子IO名称/类型描述块，finally仅释放这个临时块。发送前托管状态已经改变，所选catch没有回滚TypeName或重建节点。',
 '改型命令的目标类型文本，与新子IO描述结构、旧节点/值引用分开；没有可由此推出的标量自动转换规则。',
 '原体未在这里把旧数组逐元素转换为新类型，也未证明旧payload同步回收。',
 '组合临时块释放不等于旧变量值释放；发送失败时UI/绑定已变的具体一致性需运行/原callee证据。',
 [ref(v,'public void ModifyType(string varName',95)],['native VAR_TYPE实际值转换/默认初始化/引用失效与失败回滚仍未知；不得以常规产品设计补齐。']);
 add('VD08','原变量作用域和运行参数引用绑定','DataSource.ID+变量名作为来源身份，参数类型匹配后绑定ALL',
 'SetRunParamBindForName按目标模块ID获取模型，仅IsSupportSub且变量类型文本与参数Type相同的项继续；调用ParamSubManager.SetModuleParamBinding时来源模块是DataSource.ID、来源名varName、类型SUBSCRIBE_VAR_VARIABLE，下标模式ALL且常量文本All。成功才在目标模块UserData["@"+参数名]存true并重置ISubInfo初始化状态。变量作用域不是单独一个裸字符串名；VarItem显示将Owner.ID==13000标为全局，其他模块标为局部，但显示标签不替代实际来源模块ID。',
 '引用绑定传模块身份/名字/下标描述；不是先把源值数组复制到目标参数。',
 '绑定描述与消费者本地显示标志各自保留；同名变量在不同DataSource下不能因此合并。',
 '解绑/删除的载荷退役须看native订阅表，不能把UserData标志清理等同于原值或最后读者释放。',
 [ref(v,'private void SetRunParamBindForName(',32),ref('analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.Variable/VarItem.cs','return (vmModule.ID == 13000)',3)],['原生局部变量模块创建/调用栈作用域、过程结束销毁与ALL订阅求值时序未取得原体。']);
 return {schemaVersion:1,complete:false,facts,unknowns:[{id:'VDU01',target:'native variable module ModifyVar/DeleteVar/GlobalValueList',question:'需取实际接收方body确认标量/数组/字符串存储、同步消费、改型/删除退役与绑定失效。'},{id:'VDU02',target:'PointSet生产/写入',question:'此托管SetVarPointset为空；getter只证明读取复制，不提供native写协议。'},{id:'VDU03',target:'variable scope and last readers',question:'DataSource.ID提供来源身份，但native过程调用作用域、订阅求值、并发与最后持有者未闭合。'}],provenance:{artifacts,sourceCaptures:[{path:v,sha256:sha(fs.readFileSync(path.join(root,v))),command:"& .tools/ilspycmd.exe -t Apps.XmlParser.Variable.VariableManager 'D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlParser.dll' | Out-File -Encoding utf8 "+v,exitCode:0},{path:io,sha256:sha(fs.readFileSync(path.join(root,io))),command:"& .tools/ilspycmd.exe -t VM.Core.VmIO 'D:/Program Files/VisionMaster4.4.0/Applications/myLibs/VM.Core.dll' | Out-File -Encoding utf8 "+io,exitCode:0}],retainedCil:{path:cilPath,sha256:sha(cilBytes),mvid:cil.manifest.artifacts[0].mvid,reaEvidence:cil.manifest.artifacts[0].reaEvidence,methods:cil.targets[0].methods.map(m=>({name:m.name,token:m.token,ilSha256:m.ilSha256})),boundary:'Formal selected CIL covers constructors/GetAllVar/link/RepairVarName; fresh static type captures cover additional cited methods.'},targetCodeExecuted:false,originalRuntimeExecuted:false,nativeCliLaunched:false,liveProcessHandle:null,anotherWorktreeRead:false,implementationChanges:false}};
};
if(require.main===module){const r=module.exports(),report='docs/architecture-recovery/original-nonimage-data.json';fs.writeFileSync(path.join(root,report),JSON.stringify(r,null,2)+'\n');console.log(JSON.stringify({report,facts:r.facts.length,unknowns:r.unknowns.length,refs:r.facts.reduce((n,f)=>n+f.refs.length,0)}));}
