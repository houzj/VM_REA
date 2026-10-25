const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs'),{cilReference}=require('./original-cil-reference.cjs');
module.exports=function(){
 const prefix='result-image-selection',P='docs/architecture-recovery/'+prefix+'.resources/',V=P+'VMControls.WPF.ModuleResultView.decompiled.cs',D=P+'VMControls.WPF.DataForUI.ModuleDisplayDecorator.decompiled.cs',X=P+'moduleresultview.xaml',S=P+'moduleviewstyle.xaml',R='analysis/decompiled/VMControls.WPF/VMControls.WPF.DataForUI/RenderDataCenter.cs';
 const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),read=p=>JSON.parse(fs.readFileSync(path.resolve(root,p))),provenance=read('docs/architecture-recovery/'+prefix+'.provenance.json');
 for(const f of [provenance.artifact,...provenance.toolFiles,...provenance.sources,...provenance.resources.map(r=>r.decoded),...provenance.inputs])if(sha(fs.readFileSync(path.resolve(root,f.path)))!==f.sha256)throw Error('Original result-image evidence drift '+f.path);
 const ci=read('docs/architecture-recovery/'+prefix+'.cil.json');let methodBodies=0;for(const t of ci.targets)for(const m of t.methods){const bytes=Buffer.from(m.ilBytes||'','hex'),offsets=new Set(m.instructions.map(i=>i.offset));if(m.status!=='decoded'||m.ilSize!==bytes.length||sha(bytes)!==m.ilSha256)throw Error('Incomplete result-image CIL');for(const i of m.instructions)if(/BrTarget$/.test(i.operandKind)&&!offsets.has(i.operand)||i.operandKind==='InlineSwitch'&&i.operand.some(n=>!offsets.has(n)))throw Error('Invalid result-image CIL branch');methodBodies++;}
 const artifact=provenance.artifact,ref=(p,a,n=1)=>{const r=source(p,a,a,n);if(p.startsWith(P)){r.evidenceType=p.endsWith('.xaml')?'original-baml-offline-ilspy-extraction':'original-managed-offline-ilspy-decompilation';r.binaryIdentity=artifact.sha256+' / MVID '+artifact.mvid;}return r;},C=(t,n)=>cilReference(prefix+'.cil.json',t,n),W='VMControls.WPF.ModuleResultView',M='VMControls.WPF.DataForUI.ModuleDisplayDecorator',facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns,stages=['results','display'])=>facts.push({id,classification:'source-confirmed',from,to,behavior,representation,cache,release,refs,unknowns,stages,modes:['all']});
 add('RS01','ModuleResultView.ModuleSource → ModuleDisplayDecorator','m_CurrentInfo.ImageDataSource → ImageViewBox.ImageDataSource声明',
 'ModuleResultView的ModuleSource变化经DoSourceChanged释放旧装饰器、清数据仓库，再建立DecorateSource=newValue且ParentUI=this的ModuleDisplayDecorator并Init；resultGrid与localImageGrid使用该装饰器上下文。ImageDataSource的setter仅比较object引用、赋给imageDataSource字段并通知属性变化，没有复制像素。原moduleresultview.baml离线输出中，imageView显式绑定m_CurrentInfo.ImageDataSource，imageViewIn绑定ImageDataSourceIn；InitializeComponent包含该资源LoadComponent入口。这是主模块结果视图自己的显示链，不可误串到RenderControlBase。',
 '装饰器object字段持有ImageData引用；ImageViewBox为另一依赖属性持有者。绑定声明不等于本机活实例绑定已执行。',
 '当前装饰器随ModuleSource重建；同一个ImageData对象重复赋值不通知，不能用每次setter推断每帧都建立新图像。',
 'setter不释放旧像素，也不调用旧图像Dispose；模块切换的Dispose/清视图另由RS05逐步说明。',
 [ref(V,'private void DoSourceChanged(',33),ref(D,'if (imageDataSource != value)',6),ref(X,'Name="imageView" Margin="1"',1),ref(X,'Name="imageViewIn" Margin="1"',1),C(W,'DoSourceChanged'),C(M,'set_ImageDataSource'),C(W,'InitializeComponent')],
 ['BAML载荷与离线XAML分别核hash；没有独立逐条验证全部原BAML语义或运行绑定。','具体ImageData持有的Buffer/MemoryAddress如何显示由ImageViewBox/Byte2ImageSource继续消费，字段赋值本身不完成像素释放。']);
 add('RS02','RenderDataCenter.SelectedImage缓存','模块回调/流程UpdateRender → 装饰器图像引用',
 'RenderDataCenter.SetImageToStore将ValueEventArgs.Value as ImageData直接保存在SelectedImage键，并可能原地写Tag=SelectedImage.DisplayName；TryGetImage仅从displayImageStore返回引用。ModuleResultView自执行回调在忙/渲染门后BeforeExchange，再排到Dispatcher，Exchange指定OwnerProcessID/ExecuteCount并取SelectedImage；自执行分支还比较image.Tag与当前SelectedImage.DisplayName，然后赋ImageDataSource或交ResultViewAddition。流程UpdateRender则在工作线程与UI Dispatcher两处比较捕获的moduleDecorator是否仍为m_CurrentInfo后赋图；没有在此分支复用自执行的同一Tag比较。两条来源不得合并为一个无条件赋值。',
 '缓存字典、回调局部变量、装饰器字段共享同一个ImageData对象；所选写入/读取未逐像素复制。',
 'SelectedImage键来自当前选中的DisplayObject；BeforeExchange/Exchange与OwnerProcessID、执行计数相关，未成功渲染还可ClearDisplayStore。',
 '交换/清字典与装饰器/显示控件引用退出是不同动作；清缓存不能证明已经显示的位图或图像字段被清掉。',
 [ref(R,'private void SetImageToStore(',51),ref(R,'public bool TryGetImage(',4),ref(V,'public void OnModuleCallBackArrived(',89),ref(V,'private void UpdateRender(',95),C(W,'OnModuleCallBackArrived'),C(W,'UpdateRender'),cilReference('render-store-cil-evidence.json','VMControls.WPF.DataForUI.RenderDataCenter','TryGetImage')],
 ['部分真正赋值位于编译器闭包；本次新CIL清单记录入口体，闭包相关原CIL另见已存render-store证据；不声称所有排队回调与当前选择在原机均已实测。','ResultViewAddition可改写显示路径，不能把插件扩展路径视为普通ImageDataSource赋值同义。']);
 add('RS03','图像下拉选择 / 主动刷新','SelectedImage.DisplayObject.GetValue → UpdateImageView',
 '原图像下拉ItemsSource=ImageViewList、SelectedItem双向绑定SelectedImage、SelectedValuePath=DisplayObject，SelectionChanged进imageListCombobox_SelectionChanged。SelectedImage setter解除旧DisplayObject输出过滤与ValueChanged订阅，再为新对象SetOutputFilter/订阅，写renderDataCenter.SelectedImage并通知属性。选择处理最终调用UpdateModuleViewResultShow：按IsClearLastExecuteResult选择ExecuteCount或uint.MaxValue，优先对选中DisplayObject.GetValue；若没有选中项则取显示声明中首个image/pixelinfoimage对象；再清图形并UpdateImageView。后者普通分支赋装饰器与curImageData，同一引用无像素克隆。该拉取分支可在回调缓存没有SelectedImage图像时补取，不能只画回调字典一条通道。',
 '选择身份ImageViewItem/DisplayObject与实际GetValue返回的ImageData分离；curImageData是视图的又一持有者。',
 '自执行缓存无图的fallback另要求所选DisplayValue是当前ModuleSource拥有的IVmIO；流程UpdateRender fallback不具有完全相同Owner谓词。主动刷新读取计数门也与回调计数门不同。',
 '无选中值时选择处理会UpdateImageView(null)清显示；无效尺寸/格式可ClearImage或被ResultViewAddition接管，不代表原数据对象已销毁。',
 [ref(X,'ItemsSource="{Binding ImageViewList}"',1),ref(D,'public ImageViewItem SelectedImage',58),ref(V,'private void imageListCombobox_SelectionChanged(',78),ref(V,'public void UpdateModuleViewResultShow(',42),ref(V,'public void UpdateImageView(',40),C(M,'set_SelectedImage'),C(W,'UpdateModuleViewResultShow'),C(W,'UpdateImageView')],
 ['本报告未把抽象“取图”虚构为名为getImage的已定位成员；实际调用是Object.GetValue、RenderDataCenter.TryGetImage等。','此处不重新证明每个原生图像生产者到Object.GetValue的所有分支；已有ImageData返回与共享内存桥接证据单列。']);
 add('RS04','模块结果树的行点击/刷新','SelectedItemPath与DisplayValue更新；不是图像下拉选择',
 '原moduleresultview BAML离线输出把resultTreeView.ItemsSource绑定到ResultData，模块模板展开Outputs，VmIO模板展开Children；ModuleDisplayDecorator.Init把当前模块加进ResultData。其合并的ModuleViewStyle资源在行MouseLeftButtonDown触发SelectedCommand，SelectedCommandExecute只读取FullName、去掉%字符并设置SelectedItemPath，没有调用GetValue、UpdateImageView或改变SelectedImage。RefreshResultTree仅在模块结果树Tab被选且ModuleSource存在时，对指定类型叶IO触发DisplayValue属性通知。因此结果树点击、树值刷新、图像下拉选择是三个不同入口，不能从“点了结果树”推定切换了主图。',
 '树持有模块/IO及显示值投影；SelectedItemPath是字符串路径，不是ImageData。',
 'ResultData是装饰器持有的结果树根集合；RefreshResultTree不在此体内缓存图像或建立独立像素历史。',
 '清ResultData移除树根引用；不等价ClearImage，也不代表原IO结果载荷或显示位图释放。',
 [ref(X,'<TreeView Name="resultTreeView"',1),ref(X,'HierarchicalDataTemplate x:Key="{DataTemplateKey {x:Type VMCore:VmModule}}"',2),ref(S,'<i:EventTrigger EventName="MouseLeftButtonDown">',3),ref(D,'private void SelectedCommandExecute(',12),ref(D,'ResultData.Add(vmModule);',8),ref(V,'public void RefreshResultTree()',20),C(M,'SelectedCommandExecute'),C(W,'RefreshResultTree')],
 ['结果树样式载荷已独立离线提取并与旧本地XAML核同hash；仍不证明活树Template/事件已在原机命中。','其它插件/自定义结果控件可能提供额外图像操作，不能据所选通用树否定所有自定义行为。']);
 add('RS05','模块切换 / OwnerWin_Closed / 装饰器Dispose','清图像字段、退订事件、清缓存与原始像素寿命分列',
 'OwnerWin_Closed把ModuleSource设null，普通模块切换DoSourceChanged会Dispose旧m_CurrentInfo并置null。装饰器Dispose退订流程/模块、输入和显示对象事件，在Dispatcher调用ParentUI.ClearModuleView，再清dataRecords/ShapeList/ResultData、DeInitImageList、停RecordTimer并RenderDataCenter.DeInit。ClearModuleView调用UpdateImageView(null)，普通校验分支进ClearImage，将装饰器.ImageDataSource与curImageData置null；同时清图形/数据网格/图层。上述选定方法没有逐像素Free、共享内存映射释放或ImageData.Dispose调用，因此只能证明各自持有/订阅退出，不能据此断言所有原生像素或已生成位图立即释放。',
 '装饰器、视图curImageData、ImageViewBox依赖属性、数据中心字典和显示位图是不同持有层；断开这些引用的时间点并非一个。',
 '切模块释放旧装饰器并建新装饰器；Owner窗口关闭触发置ModuleSource为空；ResultViewAddition扩展仍有独立清理行为。',
 '清图像字段/集合与DeInit是可见收尾步骤，最后原生读者、GC回收和WPF位图内部资源释放仍须另证；不据Dispose名称宣称无泄漏或绝对安全。',
 [ref(V,'private void OwnerWin_Closed(',4),ref(V,'private void DoSourceChanged(',33),ref(D,'public void Dispose()',85),ref(V,'public void ClearModuleView()',15),ref(V,'public void ClearImage()',12),C(M,'Dispose'),C(W,'ClearImage'),C(W,'OwnerWin_Closed')],
 ['ResultViewAddition.CheckUpdateImageView/OnModuleCallBackArrived可改变清空/呈现分支，扩展组件全部行为未闭合。','未确认原程序关闭/切换时所有异步工作、消费者与native buffer的最后读者；不得声称本次做过原机内存回收验收。'],['retirement','display']);
 return {facts,unknowns:['本报告是主模块结果视图A链；RenderControlBase的tempImageStore/Object.GetValue显示B链由另一专项独立分析，不能相互串接。','原BAML离线声明与CIL/字段读取分别取证；原机活实例、模板命中、原生最后读者与并发时序仍未验证。'],provenance:{...provenance,methodBodies,complete:false,runtimeExecuted:false,browserRendered:false}};
};
