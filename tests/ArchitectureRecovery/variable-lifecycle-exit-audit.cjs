'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert'),{root,out}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),read=p=>JSON.parse(fs.readFileSync(path.join(root,p))),P='docs/architecture-recovery/';
module.exports=function(){
 const originalProvider=require('./original-nonimage-data-evidence.cjs')(),currentProvider=require('./current-nonimage-data-evidence.cjs')(),nativeProvider=require('./original-variable-native-receiver-evidence.cjs')(),storageProvider=require('./original-variable-native-storage-evidence.cjs')(),moduleFrameProvider=require('./original-variable-moduleframe-evidence.cjs')(),extended=require('./extended-variable-evidence.cjs')(),model={facts:[...originalProvider.facts,...nativeProvider.facts,...storageProvider.facts,...moduleFrameProvider.facts,...extended.facts,...currentProvider.facts]},probe=read(P+'current-nonimage-data-probe.json'),facts=new Map(model.facts.map(f=>[f.id,f])),inputs=new Map();
 const bind=p=>{const b=fs.readFileSync(path.resolve(root,p));inputs.set(p,{path:p,sha256:sha(b)});return b;};
 for(const e of extended.sources){bind('tests/ArchitectureRecovery/'+e.provider);for(const f of e.provenance.files||[])bind(f.path);}
 for(const p of ['tests/ArchitectureRecovery/nonimage-data-pipeline.cjs',P+'current-nonimage-data-probe.json','tests/ArchitectureRecovery/current-nonimage-data-evidence.cjs','tests/ArchitectureRecovery/original-nonimage-data-evidence.cjs','tests/ArchitectureRecovery/original-variable-native-receiver-evidence.cjs','tests/ArchitectureRecovery/original-variable-native-storage-evidence.cjs','tests/ArchitectureRecovery/original-variable-moduleframe-evidence.cjs','tests/ArchitectureRecovery/variable-lifecycle-exit-audit.cjs'])bind(p);
 assert.equal(facts.size,model.facts.length);assert.equal(moduleFrameProvider.provenance.selectedProcedures,31);assert.equal(moduleFrameProvider.provenance.completions.length,8);assert.equal(moduleFrameProvider.provenance.failedSelection.admittedAsEvidence,false);const presentation=fs.readFileSync(path.join(root,'tests/ArchitectureRecovery/nonimage-data-pipeline.cjs'),'utf8');assert.equal((presentation.match(/\{id:'(?:numbers|text|points|scope|retire)'/g)||[]).length,5,'Five presentation rows');assert(presentation.includes('originalRuntimeExecuted:false')&&presentation.includes('endToEndWorkflowExecuted:false'),'Finite execution boundaries retained');assert.equal(probe.passed,13);assert.equal(probe.failed,0);assert.equal(probe.vendorRuntimeExecuted,false);
 let refs=0;for(const f of facts.values()){assert.equal(f.classification,f.id.startsWith('CD')?'independent-implementation':'source-confirmed');for(const r of f.refs){const b=bind(r.path);assert.equal(sha(b),r.sha256,'Source identity '+f.id);assert.equal(b.toString('utf8').split(/\r?\n/).slice(r.start-1,r.end).join('\n'),r.excerpt,'Source excerpt '+f.id);refs++;}}
 for(const a of probe.assemblies){assert.equal(sha(bind(a.path)),a.sha256);assert.equal(sha(bind(a.pdbPath)),a.pdbSha256);for(const s of a.sourceDocuments)assert.equal(sha(bind(s.path)),s.sha256);}
 const evidence=(text,factIds)=>{for(const id of factIds)assert(facts.has(id),'Unknown exit audit fact '+id);return{text,factIds};};
 const original=(entry,copy,hold,exit,unknowns)=>({classification:'source-confirmed',scope:'Selected original managed/native bodies; not a complete live-instance contract.',entry,copy,hold,exit,unknowns});
 const current=(entry,copy,hold,exit,unknowns)=>({classification:'independent-implementation',scope:'Current main-tree contract; native equivalence is not inferred.',entry,copy,hold,exit,unknowns});
 const classes=[
 {id:'numbers',title:'数值与数值数组',original:original(
  evidence('SetVarIntValue/SetVarFloatValue以变量名、类型文本、VAR_VALUE动作和nValueNum构造ModifyVar；native接收分支读数量/pointer并按记录类型转发，不把转发当最终存储。',['VD02','VN01','VN02']),
  evidence('托管包装逐元素形成byte[]并Marshal.Copy到临时HGlobal；已证的是这层实际复制。typed native wrapper继续传原payload指针；当前基线尚未证int/float最终存储体是否及如何复制。',['VD02','VN02','VN05']),
  evidence('调用者临时块在包装方法中活跃；native记录与其指向的最终存储需要另追。GetAllVar结构快照中的内嵌pointer并未因外层byte[]自动独立。',['VD01','VD02','VN02']),
  evidence('setter进入try后的finally释放临时数值块；AllocHGlobal/Marshal.Copy/StructToBytes在try前，不能宣称全异常域保证释放。删除通知和本模块map擦除仍不是typed payload真正free的证据。',['VD02','VD06','VN04']),
  [evidence('typed存储0x295b0(int)/0x29eb0(float)及旧值回收、容量和错误恢复未在这21facts基线闭合。',['VN02']),evidence('VM_M_SetGlobalVarLockStatus返回AL和额外字节的业务语义不明；正常尾锁状态调用不证明异常/并发安全。',['VN05'])]),
  current:current(
  evidence('Integer(long)、Number(double)、NumberArray+Scalar及Number+Array是不同托管合同；AcceptInteger的声明准入与实际Coerce步骤分开。',['CD01','CD02','CD03']),
  evidence('数组工厂ToArray复制容器；同类型Coerce保留同PortValue。整型标量提升先经float，实际16777217变16777216；Integer Array即使被类型门接受，真实Coerce会因As<long>抛异常。',['CD01','CD02','CD03']),
  evidence('本轮inputs、records、variables与完成session可以共享PortValue；Group累积先保存每轮引用，Finish再JoinArray物化。',['CD05','CD08']),
  evidence('替换字典项或退出方法只撤相应持有者，没有数值数组逐对象Dispose；改配置/删变量不改写已返回旧session。',['CD05','CD07','CD08']),
  [evidence('13项有限探针未执行GraphExecutor整图，InputTypeMismatch路由仍是源码证据；不从观察一次异常推定全部数组转换路径。',['CD03']),evidence('托管long/double和资源上限不证明原SDK int32/float32 ABI或全部溢出域兼容。',['CD01','CD08'])])},
 {id:'text',title:'文本、字符串数组与byte变量',original:original(
  evidence('SetVarValue按UTF8字节数走短文本SDK或长文本binary；SetVarStringValue走string[]嵌套描述与每项字符块，两者不是同一种地址合同。',['VD03']),
  evidence('string[]为各项复制补零UTF8字符到HGlobal，再复制外层ByteDataArray描述数组；外层结构复制不能递归证明所有pData目标永久独立。',['VD03','VN02']),
  evidence('包装调用期间同时有托管编码数组、各字符HGlobal、外层描述块；native string wrapper随后传递嵌套描述pointer，接收层最终保留方式尚需其存储体。',['VD03','VN02','VN05']),
  evidence('finally释放各项字符块和外层描述块，但前置分配/序列化失败不全受finally保护；删除map或共享记录局部退引用不等于已释放native旧字符串。',['VD03','VN04']),
  [evidence('0x2a7b0最终string存储、短文本与长文本接收端一致性及最后字符读者在当前基线未知。',['VD03','VN02']),evidence('锁状态返回值及传给typed callee的额外字节不命名成copy/ownership标志。',['VN05'])]),
  current:current(
  evidence('Text保存string，TextArray是Text+Array；配置byte变量按偶数位十六进制归一化为Text，不在该链生成byte[]。',['CD01','CD02','CD06']),
  evidence('TextArray复制引用数组但保留不可变string对象；参数订阅另SerializeToElement。延迟读截到首个NUL并检查UTF8字节数+1，不暴露借用字符指针。',['CD02','CD06']),
  evidence('文本可通过PortValue被本轮变量/结果/session共同保留；JoinArray新建容器但继续保留字符串对象引用。',['CD05','CD08']),
  evidence('删除配置变量和订阅不强制清旧session或调试结果；没有对托管string执行native Free的合同。',['CD05','CD07']),
  [evidence('这不是原版字符串布局或byte变量传输等价证明；配置和独立调试入口的延迟resolve读时点需各自核。',['CD06'])])},
 {id:'compound-descriptors',title:'组合描述、PointSet与当前几何',original:original(
  evidence('VAR_TYPE动作可带新子IO描述数量与指针；pointset getter则读取ByteDataArray中第0块的MatchPoint序列，POINT x/y数组是另一结构。',['VD05','VD07','VN03']),
  evidence('已证VAR_TYPE按nValueNum×0x130把子变量描述memcpy_s进记录数组，不能把描述内嵌pointer目标算成深拷贝。pointset getter则另有byte[]复制和逐MatchPoint结构读取。',['VD05','VN03']),
  evidence('record+0x470持子描述数组、+0x450持数量、+0x448标组合分支；调用者也有临时组合描述块。源block、复制后的描述及嵌套payload要分开。',['VD07','VN03','VN04']),
  evidence('托管ModifyType finally释放其临时组合块，原生DeleteVarValue逐子项通知ModuleFrame并仅保留最终返回码。记录map擦除不证明+0x470数组及嵌套payload全部free；pointset getter正常尾Free临时结构，但无finally。',['VD05','VD07','VN04']),
  [evidence('0x28250组合值体、0x25c40/0x27080初始化/清理体、描述数组最终析构以及嵌套pointer所有权尚未在本基线闭合。',['VN02','VN03']),evidence('专用SetVarPointset为空不代表整个产品没有其它写入口；native MatchPoint布局、生产格式及最终写入仍未知。',['VD05'])]),
  current:current(
  evidence('不适用原版VAR_TYPE子描述或pointset二进制合同；本格只列当前自己的Point/PointArray/Components托管几何，不提供等价别名。',['CD02','CD04']),
  evidence('Point2、CircleFit、Component为值结构；ToArray复制各值。Expand浅复制字典并保留复合父PortValue，X/Y或各Component数值列另物化新数组。',['CD02','CD04']),
  evidence('复合父对象与投影叶子可同时保存在NodeRunRecord/流程输出/变量中；Point数组X/Y用NumberArray+Scalar，Components数值列用Number+Array。',['CD04','CD05']),
  evidence('清父结果某处引用不连带清各列数组，清投影不使其它复合父读者退出。不存在从当前几何对象Dispose推定native pointset free的路径。',['CD04','CD07']),
  [evidence('不建立原版pointset↔当前Point/Components同实例或ABI桥；Fixture等内部复合引用不可仅凭数组ToArray声称递归独立。',['CD02','CD04'])])},
 {id:'managed-results',title:'托管结果、变量引用与循环列表',original:original(
  evidence('ModuleManager按nParamType/nValueStatus转原结果；int/float状态0有单元素默认数组，其它支路读取数值数组，string逐项解码。变量订阅描述另以DataSource.ID+名称+ALL索引标识来源。',['VD04','VD08']),
  evidence('数值结果转为nullable数组存在Array.ConvertAll二次容器复制；字符串数组直接交SetValue，没有统一Clone。绑定来源身份/下标不是先复制源值到参数。',['VD04','VD08']),
  evidence('VmIO._value和_valueList可以同时保留同一newValue；非图像循环追加后仍可能保留先前数组。GetAllVar的展示文本只取第0项，不代表全集只有一项。',['VD01','VD04']),
  evidence('SetNoneValueWithoutRaiseEvent仅清_value不清_valueList；DeleteIO解绑/移除也不证明所有结果消费者已放手。native删除门成功与原数组最后读者退出分开。',['VD04','VD06','VN04']),
  [evidence('循环列表完整清理/合并、异步队列、原生结果存储与订阅最后读者不由本21facts专题全域闭合。',['VD04','VD08','VN04'])]),
  current:current(
  evidence('WorkflowRunContext每轮新建变量容器，无来源变量从配置初始化；绑定变量在生产者更新时先Remove再读本轮Success/NoMatch记录。',['CD05','CD06']),
  evidence('Complete为各输出/显示/局部变量建立新容器，但内层PortValue继续共享；参数JSON、配置Snapshot与运行结果容器分开。',['CD05','CD06','CD07']),
  evidence('旧session仍可保存旧类型值；Group中间列表、最后子session和最终JoinArray结果可同时可达。插件ModuleExecutionScope.State不是这张逐轮变量表。',['CD05','CD08']),
  evidence('改型/删除断开配置订阅与来源，不原地改写先前捕获配置和session。方法结束、窗口关闭、scope退役与最后引用不可达各有边界，不宣称立即GC。',['CD05','CD07','CD08']),
  [evidence('有限工厂/Coerce探针不验证整图发布、取消、调试旧结果输入或最后读者；这些应结合CR和具体入口而非仅凭本表宣布全清。',['CD05','CD07','CD08'])])}
 ];
 // VS01-04 close the typed-wrapper retrieval seeds; they do not close ModuleFrame storage.
 const num=classes[0].original;
 num.copy=evidence('托管包装复制数值到临时HGlobal；已取0x295b0/0x29eb0原体确认wrapper不先复制新数值数组，而将原pointer/count直接交ModuleFrame.BatchSetGlobalVarInt/Float。第五栈字节确实传递，业务语义不猜为copy标志。',['VD02','VN02','VS01']);
 num.hold=evidence('调用者临时数组在同步包装调用期间有效，wrapper继续借用该pointer送batch。batch成功后的linked参数更新只读第0项，并不替代数组主存储；两套接收者分别追踪。',['VD02','VS01','VS03']);
 num.exit=evidence('托管进入try后的finally释放临时数值块，前置分配/复制/序列化异常仍有缺口。wrapper返回batch码，linked SetBinayData失败不覆盖它；不能用返回0证明所有联动已消费或payload已深拷贝。map删除也不等于真正free。',['VD02','VS01','VS03','VS04','VN04']);
 num.unknowns=[evidence('ModuleFrame.BatchSetGlobalVarInt RVA0x134710 / Float RVA0x134880及其下层存储的同步复制、旧值退役、容量与失败回滚仍未在VS基线闭合；0x295b0/0x29eb0已取得且只是wrapper，不再列缺体。',['VS01']),evidence('linked VM_M_SetBinayData接收的格式化短期文本如何复制/排队、锁返回AL及额外字节业务语义仍未证。',['VS03','VS04','VN05'])];
 const str=classes[1].original;
 str.copy=evidence('托管string[]复制UTF8内容到每项HGlobal并再复制大描述数组；已取0x2a7b0只把输入每0x60记录的pointer/len/size抄入0x10 STRING_UNIT数组，没有复制字符内容。两份描述仍指向同一调用者字符块。',['VD03','VS02']);
 str.hold=evidence('同时持有托管编码数组、调用者字符HGlobal、大描述HGlobal与wrapper紧凑STRING_UNIT数组。紧凑数组交ModuleFrame.BatchSetGlobalVarString；batch成功后的linked SetBinayData直接用原输入第0条字符串pointer/len，不是每个目标传整个数组。',['VD03','VS02','VS03']);
 str.exit=evidence('wrapper正常返回batch后立即delete[]紧凑描述，无论batch码0或非0；未逐项free原字符pointer。托管finally随后释放原字符及大描述块，但try前异常缺口仍在。callee抛异常或内部保留pointer的合同未据正常delete[]推定。',['VD03','VS02','VS04']);
 str.unknowns=[evidence('ModuleFrame.BatchSetGlobalVarString RVA0x1349f0是否同步复制字符/描述、是否保留pointer、旧值free和回滚仍待其原体；0x2a7b0 wrapper已取得，不再列缺体。',['VS02']),evidence('linked VM_M_SetBinayData字节接收/租约与短文本SetParamValue/长文本binary另一入口仍分别未知；返回batch0不是联动全成功。',['VD03','VS03','VS04'])];
 classes[3].original.entry=evidence('ModuleManager按结果种类和status转换；typed wrapper另有batch主存储与仅第0项linked文本更新，不把这条联动拼成VmIO结果同一对象。变量引用按DataSource.ID+名称+ALL标识。',['VD04','VD08','VS03','VS04']);
 // VZ01-07 independently narrow storage and deletion gaps; snapshots stay verbatim.
 num.entry=evidence('托管ModifyVar的原pointer/count经GlobalVariable typed wrapper进入实际ModuleFrame BatchInt/Float，不以另一个Set3导出冒接。Batch wrapper重排参数并传隐藏字节，内部取得按句柄首int定位的模块/名称共享值对象。',['VD02','VN01','VN02','VN05','VS01','VZ01','VZ02']);
 num.copy=evidence('调用者先复制数值到临时HGlobal；wrapper本身不复制，已取ModuleFrame主写入后才明确按原pointer+i*4逐项读取并写入内部动态数组。字节非零分支还有辅助列表的4字节复制，辅助表与主仓不能并成一份缓存。',['VD02','VS01','VZ01','VZ02','VZ03']);
 num.hold=evidence('原数值临时块、可选辅助数值列表、主共享值对象内的复制数组和消费者各自持有。只有batch成功、数量>0且typed入口第六字节=0的门通过后，linked参数更新才取第0项；不是所有目标收到整数组。',['VD02','VS03','VZ02','VZ03']);
 num.exit=evidence('托管finally清调用者临时HGlobal的已进入try路径；内部数组已有实际值复制，但其旧值虚表+8槽和扩容失败语义仍未全解。删除沿七typed map走erase：节点名称/节点operator delete已证，payload只按强计数降零条件虚析构，不能说每份数值数组都已free或最后读者退出。',['VD02','VS04','VZ03','VZ05','VZ06']);
 num.unknowns=[evidence('不是缺Batch原体，而是共享值对象准备/重置虚槽、实际payload析构动态接收者、扩容错误/负数量域、其它读者与事务回滚未全闭合。',['VZ02','VZ03','VZ06']),evidence('linked VM_M_SetBinayData短期格式化文本的接收租约仍未知。已证真实_Mtx_lock/unlock及状态位分支，但不证明所有请求都加锁或异常必解锁，也不猜隐藏字节为copy标志。',['VS03','VS04','VN05','VZ07'])];
 str.entry=evidence('SetVarStringValue产生每项UTF8字符HGlobal和大描述；GlobalVariable wrapper复制0x60记录的pointer/len/size为0x10 STRING_UNIT，再交实际ModuleFrame BatchString。短文本/长文本SetVarValue仍是另外两条入口。',['VD03','VS02','VZ01']);
 str.copy=evidence('wrapper只复制描述，未复制字符；已取BatchString接收体随后读各描述首pointer并按C尾零扫描构造局部字符串，再经00ffe0/006470对内部目的字符串memcpy字符、更新长度及尾零。实际字符复制已证，但不能把上游长度/容量字段推成此C字符串扫描的统一边界。',['VD03','VS02','VZ04']);
 str.hold=evidence('调用者字符块与大描述、wrapper紧凑描述、接收方C字符串临时对象、内部字符串副本及可选辅助列表分别保留。仅在指定成功/数量/第六字节门通过后，linked SetBinayData直接借原第0条字符pointer/len。',['VD03','VS02','VS03','VZ04']);
 str.exit=evidence('接收体临时字符串的大容量块有operator delete；wrapper在Batch正常返回后delete[]紧凑描述；托管finally释放原字符和大描述。它们是三层不同临时清理，不能互相替代。typed map节点/名称delete已证，内部目的字符串未来payload析构仍受其它共享读者与未解动态虚槽限制。',['VD03','VS02','VZ04','VZ05','VZ06']);
 str.unknowns=[evidence('不再把BatchString/字符复制原体列缺；剩余为尾零扫描的空指针/嵌入NUL/分配失败域、内部字符串payload最后读者和动态析构体。',['VZ04','VZ06']),evidence('linked SetBinayData消费租约，以及SetVarValue短文本与长binary入口如何落入最终存储仍分别未知；batch0不代表全部linked成功。',['VD03','VS03','VS04'])];
 classes[2].original.exit=evidence('托管finally只释放调用者组合临时描述。组合DeleteVarValue先逐子项发通知且只保留最终返回码；ModuleFrame中按七typed map找到范围erase，节点名称/节点真实delete已证，载荷控制块只有降零才走未解动态虚析构。不能把组合描述数组或嵌套payload都判成已free。pointset getter的临时块只有正常尾Free。',['VD05','VD07','VN04','VZ05','VZ06']);
 classes[2].original.unknowns=[evidence('组合值0x28250、0x25c40/0x27080及GlobalVariable record+0x470描述数组自身退役仍未闭合；ModuleFrame typed map节点已解不替代这个数组的析构。',['VN02','VN03','VZ06']),evidence('专用SetVarPointset空体不代表所有写入口均空；原pointset生产/写格式与MatchPoint ABI仍不能拿当前Point/Components补造。',['VD05'])];
 classes[3].original.exit=evidence('VmIO仅清_value仍可能由_valueList/已发布消费者持有数组；native Del已追到七typed map的范围erase和名称/节点delete，但共享payload条件虚析构及其动态目标、结果最后读者仍另列。托管解绑/移除、native节点删除和旧结果放手不是同一动作。',['VD04','VD06','VN04','VZ05','VZ06']);
 classes[3].original.unknowns=[evidence('结果循环列表完整清理/合并、全部订阅读者与payload析构目标未全域恢复。真实mutex已证，但锁记录与typed仓不能仅同名合并，也不保证所有异常路径排空。',['VD04','VD08','VZ06','VZ07'])];
 // New VX bodies narrow compound-copy and retirement gaps; older fact records are retained.
 classes[2].original.entry=evidence('复合描述按0x130步进，类型0/1/2进入typed数值/字符串分支；类型4为图像，不能补成PointSet。typed第六字节在复合入口固定1，正常独立设置的linked分支不可照搬。',['VX01','VX06','VD05','VD07']);
 classes[2].original.copy=evidence('VAR_TYPE整段memcpy包含嵌套指针，只证明描述浅复制。图像type4的独立helper另申请buffer并memcpy字节；这两层复制不能混同。',['VN02','VX04','VX06']);
 classes[2].original.hold=evidence('record+0x470保留复合描述数组，+0x468另持字符串描述。0x27080真实为SetGlobalValue发布体，只建立临时紧凑描述并交Batch，源record仍保留。',['VN03','VX02','VX04','VX05']);
 classes[2].original.exit=evidence('0x25c40清record计数与指针；0x470数组沿cookie逐0x130元素析构，再释放外层。数值非空pointer delete[]；嵌套0x60描述内容只在+0x50非0门通过才释放。它不删除ModuleFrame typed主仓。',['VX02','VX03','VZ05','VZ06']);
 classes[2].original.unknowns=[evidence('复合memcpy输入嵌套地址的生产/转移与分配器协议尚缺，析构体不证明读者全部退出，也不能直接诊断double-free。',['VX03','VX04']),evidence('图像helper新buffer/handle最终拥有者、PointSet生产与专用setter之外的原生写路径仍未知。',['VX06','VD05'])];
 const graphProbe=read(P+'current-nonimage-graph-probe.json');require('./verify-current-nonimage-graph-probe.cjs')();bind(P+'current-nonimage-graph-probe.json');for(const a of graphProbe.assemblies){bind(a.path);bind(a.pdbPath);for(const d of a.sourceDocuments)bind(d.path);}
 num.entry=evidence('托管临时pointer/count经typed wrapper进入ModuleFrame Batch；factory按名称查找或创建0x88值对象及独立共享控制块，不能把两张虚表视作同一对象。',['VD02','VN01','VS01','VZ01','VZ02','VP01']);
 num.hold=evidence('数值临时块、辅助列表、typed值对象、其载荷及共享控制块分别持有。值对象+8槽仅清计数并保留分配；容量不足才扩容，旧header非0时本体不释放旧块。',['VD02','VS03','VZ02','VZ03','VP01','VP02','VP04']);
 num.exit=evidence('临时HGlobal finally退役与typed map删除分开。强归零已绑定fa730→f7410，实际delete包装对象及名称/类型堆块；弱归零0210d0→0210f0实际delete控制块。payload只经callable未知动态槽关联，不因包装对象已delete就判其内容已free。',['VD02','VS04','VZ05','VZ06','VP05','VP06']);
 num.unknowns=[evidence('值对象准备/逻辑清空和控制块析构目标已恢复；剩余callable分配/清理动态目标、旧header非0链、扩容错误域与全部最后读者。',['VP02','VP03','VP04','VP05','VP06']),evidence('linked SetBinayData消费租约和锁状态完整异常域仍待核。',['VS03','VS04','VZ07'])];
 str.hold=evidence('源字符块、两级描述、接收端临时字符串与内部副本分开。typed string+8槽逐字符串清长度/首字符并清计数，保留分配；对象准备另经callable取得载荷。',['VD03','VS02','VS03','VZ04','VP01','VP02','VP03']);
 str.exit=evidence('临时C字符串、紧凑描述、调用者字符块各自正常退役；强归零释放typed包装对象/名称/类型，弱归零释放控制块。字符串载荷及callable最终动态free仍未知，逻辑清空不是释放。',['VD03','VS02','VZ04','VZ05','VP02','VP05','VP06']);
 str.unknowns=[evidence('Batch字符复制、逻辑清空和包装对象/控制块析构原体已取；剩余callable清理目标、内部字符载荷最后读者、尾零/分配失败域。',['VZ04','VP02','VP03','VP05','VP06']),evidence('短文本/长binary接收入口与linked消费租约仍各自待核。',['VD03','VS03','VS04'])];
 classes[3].original.exit=evidence('VmIO字段、循环列表与已发布消费者分别持有数组。typed map擦除可触发强归零包装对象清理和弱归零控制块delete，但最后读者与原始payload/callable释放仍不由此证明。',['VD04','VD06','VN04','VZ05','VZ06','VP05','VP06']);
 classes[3].original.unknowns=[evidence('包装对象/控制块动态目标已经核实；结果循环列表与订阅读者、payload/callable最终回收及锁异常排空仍未闭合。',['VD04','VD08','VP03','VP05','VP06','VZ07'])];
 classes[0].current.unknowns=[evidence('旧13项方法观察之外，新9项合成两节点流程已实际触发InputTypeMismatch并核模块Dispose和session保留；未验证完整Solution/Group、真实工具、GUI或原版。',['CD03','CD05']),evidence('托管long/double和资源上限不证明原SDK ABI或全部溢出域。',['CD01','CD08'])];
 classes[3].original.entry=evidence('SDK实际目标为iMVS-6000PlatformSDK的binary callback，不是直接同名ModuleFrame导出。ModuleFrame一般key若注册的是原GlobalVariable factory返回对象，则record+0x128同pointer→虚表+0x18→VN接收者条件静态闭合；SDK opcode0xbd2实际接收桥仍缺。',['VD01','VD04','VD08','VB01','VB02','VB03','VB04']);
 const checks=[
 {id:'EXIT01',state:'finite-pass',finding:'五行对照与详细事实分别保留配置、运行值、显示文本；没有按显示第一项推完整数组长度。',factIds:['VD01','VD04','CD05']},
 {id:'EXIT02',state:'finite-pass',finding:'组合memcpy限定为0x130子描述；描述与内嵌payload、临时数值块与永久变量存储分开。',factIds:['VN02','VN03','VD02','VD03']},
 {id:'EXIT03',state:'finite-pass',finding:'setter finally只覆盖进入try后的清理；预分配与序列化构造异常未被夸大为已清理。',factIds:['VD02','VD03','VD07']},
 {id:'EXIT04',state:'finite-pass',finding:'删除命令/返回码、map记录擦除和真正payload释放分开；组合循环最终码不冒充所有子项聚合成功。',factIds:['VD06','VN04']},
 {id:'EXIT05',state:'finite-pass',finding:'VmIO字段与循环列表分别持有；当前session容器复制与PortValue共享明确分开。',factIds:['VD04','CD05','CD08']},
 {id:'EXIT06',state:'finite-pass',finding:'旧13项方法观察与新9项合成GraphExecutor观察分开；后者实证部分输入失败/结果引用与模块Dispose，不外推原版、真实工具或完整方案。',factIds:['CD01','CD02','CD03','CD04']}
 ];
 for(const c of checks)for(const id of c.factIds)assert(facts.has(id));
 const unresolvedCallSeeds=[
 {seed:'SDK this+0x23a8 callback producer/opcode0xbd2 handler → ModuleFrame export, live moduleID/path identity and binary-payload lease; registered GlobalVariable factory→record+0x128→receiver slot now conditionally linked',state:'narrowed-by-VB-not-closed',factIds:['VB01','VB02','VB03','VB04']},
 {seed:'ModuleFrame callable allocator/cleanup dynamic targets, payload final readers, nonzero old-header ownership and resize/error paths; preparation/reset and wrapper/control-block deletion recovered',state:'narrowed-by-VP-not-closed',factIds:['VP01','VP02','VP03','VP04','VP05','VP06']},
 {seed:'Composite input nested-pointer producers/transfer protocol, record+0x50 condition provenance, image-buffer handle retirement; 0x25c40 cleanup and 0x27080 publishing now recovered separately',state:'narrowed-by-VX-not-closed',factIds:['VX02','VX03','VX04','VX05','VX06']},
 {seed:'ModuleFrame!VM_M_SetBinayData linked first-element text receiver / consumption lifetime; SetVarValue short/long-text routes',state:'not-closed-after-VZ',factIds:['VD03','VS03','VS04']},
 {seed:'Lock helper scratch/preparation 9a300 / 69c0 semantics, thread ownership/reentrancy and exceptional unlock/drain',state:'not-closed-after-VZ',factIds:['VZ07']}
 ];
 const families=classes.map(row=>{const family={id:row.id,title:row.title,classification:'independent-implementation',meaning:'Analyst organization only; columns do not assert equivalent types, runtime instances or ABI.'};for(const side of ['original','current']){const old=row[side],factIds=[...new Set(['entry','copy','hold','exit'].flatMap(k=>old[k].factIds).concat(old.unknowns.flatMap(u=>u.factIds)))];family[side]={input:old.entry.text,copy:old.copy.text,holders:old.hold.text,cache:[...new Set(old.hold.factIds.map(id=>facts.get(id).cache))].join('；'),retire:old.exit.text,unknowns:old.unknowns.map(u=>u.text),factIds};}return family;});
 const selectedIds=[...new Set(families.flatMap(f=>[...f.original.factIds,...f.current.factIds]))];
 return{schemaVersion:1,title:'非图像变量生命周期：有限退出状态审计',status:'finite-pass-with-explicit-gaps',complete:false,baseline:{facts:facts.size,comparisonRows:5,currentProbeCases:13,currentGraphProbeCases:9,notifiedVSEvidenceReviewed:true,notifiedVZEvidenceReviewed:true},organization:'Four investigation classes, original and current kept separate. A shared row does not assert matching ABI, runtime instances or behavioral parity.',families,checks,unresolvedCallSeeds,facts:selectedIds.map(id=>facts.get(id)),verification:{sourceReferencesChecked:refs,actualMethodProbeReexecuted:false,probeAssemblyPdbSourceHashesRechecked:true,currentProbeCases:13,currentGraphProbeCases:9,nativeModuleFrameSelectedProcedures:31,nativeModuleFrameTerminalRecords:8,failedSelectionAdmittedAsEvidence:false,originalRuntimeExecuted:false,guiStarted:false,devicesCalled:false,htmlRegenerated:false,productModified:false,otherWorktreeRead:false,implementationCredit:0},inputs:[...inputs.values()],limitations:['This audit independently reviews VS01-04 and VZ01-07 including actual Batch storage copies, erase leaves and mutex branches. Older canonical fact unknowns remain verbatim snapshots and are narrowed only by the later explicit facts. New receiver/storage bodies should resolve only their explicit seeds after separate read-only review.','Native invocation linkage, storage ownership, temporary-call memory, map/shared-record lifetime, result containers and last payload readers remain distinct completion states.','Retained 13-method probe and 9 synthetic two-node GraphExecutor observations are reviewed independently. No native/vendor/GUI/device, GC or peak-memory claim; this audit does not rerun probes.']};
};
if(require.main===module){const d=module.exports();fs.writeFileSync(path.join(out,'variable-lifecycle-exit-audit.json'),JSON.stringify(d,null,2)+'\n');console.log(JSON.stringify({families:d.families.length,checks:d.checks.length,facts:d.facts.length,sourceReferences:d.verification.sourceReferencesChecked,seeds:d.unresolvedCallSeeds.length,status:d.status}));}
