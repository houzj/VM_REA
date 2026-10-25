const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,source}=require('./evidence-view.cjs'),{cilReference}=require('./original-cil-reference.cjs');
module.exports=function(){
 const P='docs/architecture-recovery/result-store-ownership.resources/',R=P+'VMControls.WPF.DataForUI.RenderDataCenter.decompiled.cs',O=P+'Apps.XmlParser.Object.decompiled.cs',W='VMControls.WPF.DataForUI.RenderDataCenter';
 const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),read=p=>JSON.parse(fs.readFileSync(path.resolve(root,p))),provenance=read('docs/architecture-recovery/result-store-ownership.provenance.json');for(const f of [...provenance.artifacts,...provenance.toolFiles,...provenance.sources,...provenance.inputs])if(sha(fs.readFileSync(path.resolve(root,f.path)))!==f.sha256)throw Error('Render store ownership evidence drift '+f.path);
 const cil=read('docs/architecture-recovery/render-store-cil-evidence.json'),selected=cil.targets.filter(t=>[W,'Apps.XmlParser.Object'].includes(t.type));let methods=0,branches=0;for(const t of selected)for(const m of t.methods){const b=Buffer.from(m.ilBytes||'','hex'),offsets=new Set(m.instructions.map(i=>i.offset));if(m.status!=='decoded'||b.length!==m.ilSize||sha(b)!==m.ilSha256)throw Error('Render store CIL incomplete');for(const i of m.instructions)for(const target of /BrTarget$/.test(i.operandKind)?[i.operand]:i.operandKind==='InlineSwitch'?i.operand:[]){if(!offsets.has(target))throw Error('Render store CIL branch');branches++;}methods++;}if(methods!==20)throw Error('Render store scope changed');
 const ref=(p,a,n=1)=>{const r=source(p,a,a,n);r.evidenceType='original-managed-offline-ilspy-decompilation';const identity=provenance.artifacts.find(a=>a.key===(p===O?'xmlparser':'wpf'));r.binaryIdentity=identity.sha256+' / MVID '+identity.mvid;return r;},C=n=>cilReference('render-store-cil-evidence.json',W,n),facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns,stages=['results','display'])=>facts.push({id,classification:'source-confirmed',from,to,behavior,representation,cache,release,refs,unknowns,stages,modes:['all']});
 add('RC01','ValueEventArgs.ExecuteCount与固定标志','buffer / spare入仓路由及整仓覆盖',
 '三个仓库均有ImageData字典和shape字典：display供TryGetImage/GetShapeStore读取，buffer和spare供回调入仓。SetDataToStore先拒绝sender为空或非ValueEventArgs，再按(NeedFixed && e.ExecuteCount != FixedExcuteCount) || e.ExecuteCount == SpareExecuteCount选择spare；若所选spare批次改变，就更新SpareExecuteCount并Clear整套spare图像/图形仓。其它回调走buffer，批次改变则更新BufferExecuteCount并Clear整套buffer仓。同一批次再次到达只更新相关键，未更新键不在此路径自动清除。NeedFixedSpare并不出现在这个分流谓词里。',
 '仓库装的是ImageData/事件对象引用，不是复制像素帧；ExecuteCount是入仓批次标记，不应画成固定容量逐帧历史列表。',
 '同一执行计数的多次回调在所选仓内合并；新批次在同一仓覆盖旧批次。spare与buffer计数相等时只记录错误日志，未在该体强制去重或丢弃。',
 'Clear字典移除该仓持有的引用，没有对每张ImageData执行Dispose、解除固定或释放共享映射。',
 [ref(R,'private Dictionary<string, ImageData> displayImageStore',15),ref(R,'public void SetDataToStore(',34),ref(R,'private void SetRenderDataToStore(',24),C('SetDataToStore'),C('SetRenderDataToStore'),C('get_BufferExecuteCount'),C('get_SpareExecuteCount')],
 ['实际回调顺序、所有生产者的ExecuteCount分配及调用线程约束未在本切片闭合；不能把此分流谓词推成所有线程间的完整隔离保证。']);
 add('RC02','BeforeExchange / ExchangeBuffer','仓库角色交换后旧display转成待写仓并继续持图',
 'BeforeExchange把FixedExcuteCount设为请求计数：等于SpareExecuteCount时置NeedFixedSpare，否则置NeedFixed；它不检查该请求一定等于BufferExecuteCount，也不先清另一侧已有标志。Exchange按捕获的NeedFixedSpare调用ExchangeBuffer：true时交换display与spare的图像/图形字典引用，false时交换display与buffer引用。不是逐项复制、逐像素复制或转移后清空。原display字典带着旧ImageData变成spare/buffer，继续持有旧值，直到后续相关键被覆盖、该仓新批次Clear、ClearDisplayStore（若它此时又是display）或DeInit。ExchangeBuffer也不交换BufferExecuteCount/SpareExecuteCount字段，因此不能把计数字段当成每个字典对象内所有键都已逐项认证的帧身份。',
 '例：display=D0、buffer=B0；普通交换后display=B0、buffer=D0。旧D0的对象内容仍存在，交换的是字段引用。',
 '已显示的上一批图像可留在角色改变后的仓中；同一批次的后续写入未必触发整仓Clear，必须分别跟踪字典身份、字段角色和计数。',
 '交换本身不消除旧display持图，也不让ModuleDisplayDecorator/ImageViewBox的已取引用自动失效。',
 [ref(R,'public void BeforeExchange(',14),ref(R,'private void ExchangeBuffer(',24),ref(R,'public bool IsBusy =>'),C('BeforeExchange'),C('ExchangeBuffer'),C('get_IsBusy')],
 ['IsBusy只是两个bool的OR；所选体未显示对声明_exchangelock加锁，不能据名字证明跨线程交换原子性，也不能在无运行时序证据时直接宣称实际数据竞争。','外部调用方如何避免重复BeforeExchange和错误计数请求需结合ModuleResultView/回调调度继续核对。']);
 add('RC03','Exchange等待与Object.GetDataSendFinishState','回调计数等待完成不等于最后像素读者退出',
 'Exchange在GetDataSendFinishState(procID,executeCount)返回true时每次Sleep(3)，直到false才交换仓库，并在正常尾部把NeedFixedSpare/NeedFixed都设false。所选体没有超时、取消或finally复位。该helper按executeCount对maxStoreLen取余选计数槽，读取过程的最近发送执行计数；当槽内sent==received，或请求executeCount早于过程最近计数时，清该槽两计数并返回false，否则返回true。因此不能仅按“FinishState”名称把退出循环解释成这张图所有像素消费者都已处理完，或所请求旧批次一定仍是显示仓。',
 '等待对象是发送/接收计数与请求批次，不是图像引用计数、GCHandle、内存映射租约或WPF位图最后读者。',
 '正常交换后两个固定标志复位，但请求旧计数也可能满足helper的退出条件；不能把该门扩展成完整同帧提交保证。',
 '这里没有释放ImageData或其像素；若等待不退出或helper抛异常，不能从正常尾部推断标志已复位或窗口关闭已排空。',
 [ref(R,'public void Exchange(',12),ref(O,'public static bool GetDataSendFinishState(',14),C('Exchange'),cilReference('render-store-cil-evidence.json','Apps.XmlParser.Object','GetDataSendFinishState')],
 ['sent/received计数所有写入点、环槽复用、溢出及内存可见性未在此有限切片恢复；没有执行厂商等待循环或原生读者测量。'],['results','retirement']);
 add('RC04','ClearDisplayStore / Init / DeInit','仅清当前display、清三仓及迟到回调的边界',
 'ClearDisplayStore仅Clear当前display图像/图形字典，不改buffer/spare、三个计数、两个固定标志或SelectedImage等选择引用。Init只设isInit=true。DeInit则设isInit=false，Clear三套图像/图形字典，清计数和固定标志，置CheckImagesName/SelectedImage/SelectedInputImage为空；没有逐图Dispose或像素free。SetRenderDataToStore以isInit阻止继续入值，但SetDataToStore本身并未先检查isInit：反初始化后迟到的有效回调仍可更新仓库计数和走Clear，然后在下层不存值。所以DeInit不等同所有生产回调已经停止或排空。',
 '清仓、清选择身份、禁止下层存值、原生像素释放是四个不同层次；A链ModuleResultView另持有的ImageData不由此字段写入直接清掉。',
 'ClearDisplayStore仍保留两待写仓里的旧图；DeInit删除三仓持有，但已交给装饰器、视图或扩展消费者的对象可继续可达。',
 '所选清理体没有调用ImageData.Dispose、FreeHGlobal或共享映射关闭；不能用DeInit的完成代替最后读者和所有像素已释放的证据。',
 [ref(R,'public void ClearDisplayStore()',5),ref(R,'public void Init()',4),ref(R,'public void DeInit()',20),ref(R,'public void SetDataToStore(',34),ref(R,'private void SetRenderDataToStore(',24),C('ClearDisplayStore'),C('DeInit'),C('Init')],
 ['三仓只属于RenderDataCenter→ModuleResultView/ModuleDisplayDecorator的A链；不得归到没有引用RenderDataCenter的RenderControlBase/tempImageStore B链。','原机异步排空、外部消费者、图像/位图与native资源最终释放仍未验证。'],['retirement','display']);
 return {facts,unknowns:['字段角色交换不是像素复制，也不等同旧图像已消失；外部持有者及A/B显示路径始终分别追踪。','此处是20个相关原版CIL体及新离线源码的有限静态证据，不证明原机并发、原生指针寿命或完全无泄漏。'],provenance:{...provenance,selectedOriginalMethods:methods,branchTargetsChecked:branches,complete:false,runtimeExecuted:false,browserRendered:false}};
};
