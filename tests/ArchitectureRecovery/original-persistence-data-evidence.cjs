// Static managed-source evidence only. Never loads a vendor assembly.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const {root,source}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const d='docs/architecture-recovery/',read=p=>fs.readFileSync(path.join(root,p));
 const manifests=[d+'display-userdata-cil.json',d+'configuration-hosts-artifact-identities.json'];
 const artifacts=[];
 for(const p of manifests){const j=JSON.parse(read(p));for(const a of (j.manifest||j).artifacts){if(sha(fs.readFileSync(a.path))!==a.sha256)throw Error('Original persistence artifact drift '+a.path);if(!artifacts.some(x=>x.sha256===a.sha256))artifacts.push(a);}}
 const C='analysis/decompiled/VM.Core/VM.Core/',S='analysis/decompiled/VM.PlatformSDKCS.Audit/VM.PlatformSDKCS/',W='analysis/decompiled/VMControls.WPF/VMControls.WPF.CalibGuide/CalibrationGuideData.cs',P='analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.Project/ProjectManager.cs';
 function r(file,member,anchor,count){const ref=source(file,member,anchor,count);const key=file.startsWith(C)?'core':file.startsWith(S)?'sdk-x64':file===W?'wpf':'exe';const a=artifacts.find(a=>a.key===key);if(!a)throw Error('Unmapped original artifact '+file);ref.binaryIdentity=a.sha256+' / MVID '+a.mvid+'; retained decompiled source; not newly decoded CIL in this slice';return ref;}
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'source-confirmed',stages:['persistence','retirement'],modes:['all'],from,to,behavior,representation,cache,release,refs,unknowns});
 add('OP01','原UserData[key]','按Parent.ID普通键或动态复合键读写SDK界面数据',
 'UserData不是一个本地Dictionary。getter在Parent非活动或正在Dispose时返回null，否则调用SDK；setter在Parent为空或Dispose时直接返回。isUsedForDynamicUIParam决定路由：动态key用字面分隔符&?;?拆分，array[0]为名字、array[1]（若有）为optional，更多片段没有继续传递；普通路由传完整key和Parent.ID。Parent.IsDynamicModu控制撤销分支，是另一项判断，不能等同isUsedForDynamicUIParam。',
 '对外载荷byte[]；键是字符串，具体编码/布局由每个生产者决定。getter的null还可能表示非活动/Dispose，不能只解释为文件缺键。',
 '所选UserData索引器没有本地值字典缓存；具体模型另有nullable字段或数组持有。',
 'setter的Dispose门只阻止这次写入，不释放先前SDK值或撤销记录。',
 [r(C+'UserData.cs','UserData getter/setter lifecycle gate','public virtual byte[] this[string key]',39),r(C+'UserData.cs','UserData dynamic key routing','private byte[] GetUIParamValue(string key)',25),r(C+'UserData.cs','UserData split delimiter','public const string SplitStr',3)],
 ['SDK界面数据的最终native容器/落盘结构不在这些托管体内；不把写入等同.sol保存完成。']);
 add('OP02','原ModuleManager.SetUIParamValue / SetDynamicUIParamValue','临时非托管字节副本 → native setter；空载荷改走删除',
 '两setter对非null且Length非0的byte[]按实际Length分配HGlobal并Marshal.Copy，传pointer和uint Length调用native；调用正常返回后FreeHGlobal，再检查返回码抛错。null和空数组都走对应删除方法，不能保存为可区分的零长度值。普通路由另有IsForbidden特殊分支：非null载荷先BytesToStruct<bool>再EnableModule，绕过一般UI键存储。',
 '管理数组→独立临时HGlobal按Length复制；key的UTF8 custom marshaler与payload任意bytes是不同合同。',
 '托管体调用后不保留HGlobal；native是否复制、如何保留以及setter何时提交到方案存储仍未知。',
 'FreeHGlobal位于native正常返回后而非finally；native调用或Marshal.Copy抛异常时此体不保证走到Free。仅证明潜在异常路径，不宣称原机已泄漏。Delete成功只证明该SDK操作返回0，不证明所有历史副本已释放。',
 [r(S+'ModuleManager.cs','ModuleManager.SetUIParamValue','public void SetUIParamValue(uint nObjectID',39),r(S+'ModuleManager.cs','ModuleManager.SetDynamicUIParamValue','public void SetDynamicUIParamValue(uint nObjectId',32),r(S+'NativeMethods.cs','native UI setter byte length and key marshaler','public static extern int IMVS_SetUIParamValue(',1)],
 ['native setter保存副本或租用pointer的协议、删除最终析构、原.sol打包时点未闭合；本片不重新推断Writer/mdata。']);
 add('OP03','原普通/动态UserData读取','native pointer+nDataLen → 新托管byte[]；正常尾请求释放返回块',
 '普通GetUIParamValueByKey成功时new byte[nDataLen]并在pData非0时Marshal.Copy；复制异常被吞，随后仍请求RELEASE_MEMORY_BASE_ARRAY，释放失败抛VmException。错误码-536870911返回null，其他非0抛错。动态ModuleManager getter经ServerSDKAPI展开：成功时同样按返回nDataLen新建数组及复制，但Release调用位于pData非0条件内，不能合并成同一无条件尾释放。动态wrapper将byte[]返回给调用者。',
 '返回数组有nDataLen字节，是新分配的托管容器；这不证明内容语义正确，吞掉复制异常后仍可能返回未完整写入的数组。普通成功即使pData为0也会分配数组并请求Release；动态pData为0则不请求Release。',
 '调用者可持有返回数组，SDK临时返回块与模型缓存不是同一持有者；重复读取可生成不同数组。',
 '正常尾Release仅针对native返回块；托管数组继续由引用/GC决定寿命。分配等先行步骤抛异常时没有finally承诺，Release实际递归范围须native原体。',
 [r(S+'ModuleManager.cs','GetUIParamValueByKey','public byte[] GetUIParamValueByKey(',43),r(S+'ModuleManager.cs','GetDynamicUIParamValue','public byte[] GetDynamicUIParamValue(',23),r(S+'ServerSDKAPI.cs','dynamic UI buffer expansion','public static int IMVS_GetDynamicUIParamValue(',30)],
 ['不能由Release成功推断SDK权威存储已删除；本slice未执行SDK，也未恢复该释放类型内部allocator。']);
 add('OP04','原UserData赋值','分支化撤销载荷持有，不是统一原子保存事务',
 '全局模块以及CheckedImages/DESIGNERBUFFER/ConfigAvailable直接Set；动态ModifyConfig先登记AllUserData再Set，DynamicIO普通写绕过记录，其他动态分支有先记录或后记录差异。普通分支先Get旧byte[]、Set新值，再按引用不等及CheckItemParamReady特例登记。RecordSingleData将oldValue/newValue直接赋给UnRedo_UserData字段，没有该处数组深复制。AllUserData先保存表达式，CanRegister在允许的root操作且未重复时才Compile执行抓取；不能把表达式创建时点当快照完成时点。',
 '单项撤销记录保存object载荷引用；字段PropertyKey、ModuleFullName、IsAllUserData是定位元数据。全量快照的内部复制深度不是RecordAllUserData本体定义。',
 '撤销服务可延长旧/新数组可达性；UserData_Update另外查询全量并赋NewValue，Undo/Redo通过Dispatcher按模块全名重新定位后SetUserData。',
 '赋值替换/SDK删除不自动清撤销OldValue/NewValue。撤销服务栈清理及全量userdata内部副本释放没有在本条闭合。',
 [r(C+'UserData.cs','UserData setter history branches','if (Parent.IsGlobalModu || key ==',61),r(C+'UserData.cs','RecordAllUserData','private void RecordAllUserData()',17),r(C+'UserData.cs','RecordSingleData','private void RecordSingleData(',14),r(C+'UserData.cs','CanRegisterUserData','internal bool CanRegisterUserData(',22),r(C+'UserData.cs','Undo / Redo / Update','internal void UserData_Undo(',34)],
 ['不把字节引用登记称深克隆；外部IUnRedoService是否接受/何时清记录、VmModule.GetAllUserData/SetUserData全量格式仍需独立追踪。']);
 const fieldRefs={
 ready:[r(C+'VmModule.cs','VmModule.IsReady','public virtual bool? IsReady',33)],
 guid:[r(C+'VmModule.cs','VmModule.Init GUID','if (ServerSDKManager.serverSDKManager.mModuleManager.CheckUIParamExist(ID, "GUID"))',10)],
 zoom:[r(C+'VmSolution.cs','VmSolution zoom backing default','private int proceZoomIndex = 9;',1),r(C+'VmSolution.cs','VmSolution.ProceZoomIndex','internal int ProceZoomIndex',31)],
 subscribe:[r(C+'VmSolution.cs','VmSolution.IsSubscribeInWindow','bool IVmSolution.IsSubscribeInWindow',45)],
 calib:[r(C+'VmProcedure.cs','VmProcedure CalibData restore','byte[] array = UserData["CalibData"];',15),r(C+'VmProcedure.cs','IVmProcedure.SaveCalibData','void IVmProcedure.SaveCalibData()',12),r(W,'CalibrationGuideData.Parent hook','vmProcedure.SaveCalibDataToUserData = (VmProcedure.SaveCalibrationData)Delegate.Remove',2),r(W,'CalibrationGuideData.SaveData','private void SaveData()',7)]
 };
 add('OP05','原具体UI模型字段','字段级布局、默认值与缓存规则必须分列',
 'IsReady使用1字节0/1，缺键保留nullable未知，null setter删除。GUID初始化先CheckUIParamExist，存在时new Guid(bytes)，不存在创建Guid.NewGuid().ToByteArray再写入。ProceZoomIndex缺键返回backing字段初值9；getter读取并int.Parse后不回填该字段，setter先改字段再尝试写入且吞为消息框，故字段与SDK值不必事务一致。IsSubscribeInWindow按nullable缓存首次读取，缺失或TryParse失败为false；setter先更新缓存和通知，再尝试写入且吞异常。',
 'GUID是Guid.ToByteArray二进制，不是GUID文本；IsReady固定1字节。缩放调用Hik.Util.StringUtil.UTF8GetBytesPadZero/UTF8GetString；订阅窗口调用Hik.Util.ByteUtil.GetBytes/GetStringFromByte，不能拿SDK同名StringUtil源码替代Hik helper确切编码/补零。',
 'IsReady与IsSubscribeInWindow存在模型字段缓存；ProceZoomIndex读SDK但缺失回退backing值。缓存与原生键存储分别持有。',
 '把IsReady设null经UserData删除键，不等于清除外部撤销副本；字段缓存更新可先于SDK写入成功。',
 [...fieldRefs.ready,...fieldRefs.guid,...fieldRefs.zoom,...fieldRefs.subscribe],
 ['Hik.Util两个编码helper完整体未在本片取得，因此缩放/订阅确切字节长度及尾零不宣称已证；异常载荷（空数组IsReady、无效GUID、不可解析缩放）不伪造容错。']);
 add('OP06','原CalibrationGuideData / VmProcedure.CalibData','保存前UI flush到UserData，再进入独立方案Export阶段',
 'VmProcedure恢复时将UserData[CalibData]数组直接赋calibrationData；缺失则清该字段返回，存在则isBootMode=true。SaveCalibData在calibrationData为null时写null删除，否则仅调用可空delegate，不能保证必有写入。CalibrationGuideData.Parent为VmProcedure时去重追加SaveData，SaveData将ToXmlStringWithHead()经ByteUtil.GetBytes写入该键。ProjectManager.SaveSolution在Dispatcher同步遍历各流程SaveCalibData后，才调用VmSolution.Export；仅Export为true后才更新路径通知/启动后续Task。',
 'CalibData是XML字符串经Hik.Util.ByteUtil形成的byte[]，不是原.sol/mdata格式；读取数组与calibrationData字段在这条赋值处共享引用。',
 'calibrationData字段和SaveCalibDataToUserData delegate持有不同状态；缺少delegate时非null数据并不触发重新序列化。本条flush是保存前步骤，非磁盘提交确认。',
 'calibrationData为null的SaveCalibData请求删除SDK键；Export失败并无本段回滚已flush UserData的操作。不把XML生成、SDK键写入、异步保存回调和文件最终写入混成同一事务。',
 [...fieldRefs.calib,r(P,'ProjectManager.SaveCalibUserData + SaveSolution','private void SaveCalibUserData()',31)],
 ['CalibData XML schema、ByteUtil具体编码和长度、delegate在全部关闭路径的解除以及原native writer对userdata的磁盘打包仍未知；既有S01–S06与save-native-boundary保持独立。']);
 const knownFields=[
 {key:'IsReady',owner:'VmModule',encoding:'1字节：0=false、1=true',length:'写入固定1；getter直接读array[0]，未先验空数组',missingDefault:'nullable字段未赋值，保持null',read:'getter在_IsReady无值时读取',write:'setter状态改变时；null删除',saveTiming:'属性写入即SDK操作，非.sol成功',factIds:['OP02','OP03','OP05'],refs:fieldRefs.ready,unknowns:['外部旧数组/撤销记录不随删除自动释放']},
 {key:'GUID',owner:'VmModule.Init',encoding:'Guid.ToByteArray二进制 / new Guid(bytes)',length:'System.Guid.ToByteArray约定16字节；本片未运行',missingDefault:'CheckUIParamExist为false时新建GUID并写回',read:'模块Init且存在键',write:'模块Init且键不存在',saveTiming:'初始化写入，不代表磁盘保存',factIds:['OP05'],refs:fieldRefs.guid,unknowns:['存在键却返回null或畸形长度的原运行结果未测']},
 {key:'ProceZoomIndex',owner:'VmSolution',encoding:'Hik.Util.StringUtil.UTF8GetBytesPadZero(value.ToString())；helper内部尚未取',length:'未证确切长度/尾零，不能借SDK同名helper代替',missingDefault:'当前proceZoomIndex；初值9',read:'每次getter查UserData；成功解析不回填backing',write:'setter值不同先改字段再尝试写入',saveTiming:'属性修改时；写失败字段可能已改',factIds:['OP05'],refs:fieldRefs.zoom,unknowns:['int.Parse异常和helper内部格式不在本片运行验证']},
 {key:'IsSubscribeInWindow',owner:'VmSolution',encoding:'ByteUtil.GetBytes(nullable bool.ToString()) / GetStringFromByte后TryParse',length:'ByteUtil未取，长度未知',missingDefault:'false；异常路径返回nullable ?? false',read:'首次nullable缓存无值时；成功路径缓存结果',write:'先缓存和OnPropertyChanged再try写入；异常吞掉',saveTiming:'属性修改时，与.sol保存阶段分离',factIds:['OP05'],refs:fieldRefs.subscribe,unknowns:['编码/尾零未确认；catch后cache是否仍为空按该分支解释']},
 {key:'CalibData',owner:'VmProcedure + CalibrationGuideData',encoding:'ByteUtil.GetBytes(ToXmlStringWithHead())',length:'XML与helper决定，未固定',missingDefault:'calibrationData=null并返回',read:'流程恢复时直接保存所取数组',write:'SaveCalibData空字段删除；非空经可空delegate触发SaveData',saveTiming:'ProjectManager.SaveSolution在Export前Dispatcher flush',factIds:['OP06'],refs:fieldRefs.calib,unknowns:['XML完整schema/ByteUtil字节与native落盘均未闭合']}
 ];
 return {schemaVersion:1,facts,knownFields,unknowns:[...new Set(facts.flatMap(f=>f.unknowns))],provenance:{scope:'原版F18/F19有限UserData载荷和保存前flush；不定义.sol writer/mdata，不是全量键目录。',originalRuntimeExecuted:false,vendorAssembliesLoaded:false,productModified:false,sourceInspection:true,newCilDecoded:false,artifacts,inputs:manifests.map(p=>({path:p,sha256:sha(read(p))})),limits:['原件SHA重核与源码摘录SHA分别保留，不冒称本片所有方法都已新解码CIL。','字段表并非所有工具userdata枚举；字段编码helper缺体明确留空。','既有execution-storage S/J和solution-save-native-boundary只作边界背景，不新增writer假设。'],implementationProgress:'41/96 = 42.71%; unchanged'}};
};
if(require.main===module){const m=module.exports();fs.writeFileSync(path.join(root,'docs/architecture-recovery/original-persistence-data-evidence.json'),JSON.stringify(m,null,2)+'\n');console.log(JSON.stringify({facts:m.facts.length,knownFields:m.knownFields.length,refs:m.facts.reduce((n,f)=>n+f.refs.length,0),originalRuntimeExecuted:false}));}
