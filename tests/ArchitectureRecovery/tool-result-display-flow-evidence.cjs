const {source}=require('./evidence-view.cjs');
// Current-worktree source only. No original/runtime/ABI equivalence is implied.
module.exports=function(){
 const graph='src/VisionFramework/GraphExecutor.cs',contracts='src/VisionFramework/Contracts.cs',binary='src/VisionPlugins.ImageBinary/ImageBinaryPlugin.cs',buffer='src/VisionPlugins.ImageSource/ImageBufferPlugin.cs',main='src/VisionEditor/MainWindow.xaml.cs',display='src/VisionEditor/ToolDisplays.cs',presenter='src/VisionPluginContracts/ToolDisplayPresenter.cs';
 const ref=(p,anchor,count=1)=>source(p,anchor,anchor,count),facts=[];
 function add(id,from,to,behavior,representation,cache,release,refs,unknowns){facts.push({id,classification:'independent-implementation',from,to,behavior,representation,cache,release,refs,unknowns});}
 add('TD01','GraphExecutor连接/订阅解析','ModuleExecutionContext → 工具输入',
 '当前图连接先要求上游状态为Success或NoMatch并找到指定输出，随后nodeInputs[c.ToPort]=v保存同一个PortValue。订阅解析也把解析值写入参数或输入字典，类型/基数检查后才构造只读上下文。对当前图像类型，DataKinds.Coerce走可流通时返回原PortValue；ModuleExecutionContext.Input<T>通过As<T>返回同一Value对象。因此图像从记录经输入字典交给工具时，这些包装层没有逐像素复制。配置JSON/ROI的本轮克隆不能代证图像也被克隆。',
 '上游NodeRunRecord.Outputs → 同一PortValue → 同一IImageData/GrayImage；字典和上下文是新增持有者。',
 'records属于本次RunAsync；上游不可用会跳过当前节点，订阅失败/类型错误各自记录失败，不能取另一轮图像填补。',
 '输入字典退出只减少这一持有关系；上游记录、工具缓存及界面仍可能持有图像。',
 [ref(graph,'foreach(var c in p.Connections.Where(x=>x.ToNode==id))',8),ref(graph,'try{foreach(var binding in p.Configuration.Subscriptions',2),ref(graph,'try{if(n.ModuleId==WorkflowCalls.ModuleId)'),ref(contracts,'public static PortValue Coerce(PortValue value,DataKind target)'),ref(contracts,'public T Input<T>')],
 ['这里只确认当前托管IMAGE路径；其它标量转换、数组构造和原生租约不能统一描述为零复制。']);
 add('TD02','ImageBinaryPlugin.Runtime硬阈值处理','工作数组 → 独立GrayImage输出',
 '二值化运行体仅支持BinaryType=1；它从上下文取GrayImage和可选mask的引用，分配output字节数组及outMask，逐行检查取消并读输入、写工作数组。成功构造GrayImage时，构造器再次data.ToArray：算法工作output到结果像素是一次实际复制；可选OutMask也经过同样构造。输入像素不在该循环原地改写。ProcessedPixels是inside像素数，不是图像对象。',
 '原输入GrayImage与mask（借用引用）→ 新output/outMask工作数组 → 新GrayImage内自有像素数组 → PortValue包装。',
 '运行体无跨轮图像字段，Reset为空；本轮输出由返回结果/后续记录持有，工作数组只在本次方法内使用。',
 'GrayImage不实现IDisposable；工作数组和输出数组依托可达性回收，不能把Runtime返回或Reset称为确定时刻像素释放。取消异常未被此处过滤catch接住，外层执行器处理。',
 [ref(binary,'if(c.Integer("BinaryType")!=1)'),ref(binary,'var input=c.Input<GrayImage>',6),ref('src/VisionData/ImageGeometry.cs','public sealed class GrayImage',21),ref(binary,'public void Reset(string nodeId)')],
 ['只确认硬阈值当前体；其它模式、native算法体、实际分配峰值和取消时序不由此证明。']);
 add('TD03','ImageBufferPlugin刷新/发布','FIFO图像 → 多轮结果引用',
 '缓存图像在刷新标志等于1时复制支持的Gray/Rgb/Raw图像入队，再按数量裁剪；成功输出枚举队列并PortValue.Image(image)，这里不再次复制像素。后续暂停刷新可以让多轮结果包装同一缓存图像对象。队首淘汰仅Dequeue，不能使仍被旧结果引用的对象消失。',
 '入队深复制与发布引用包装是两个不同动作；最多15槽，数量0仍先复制再裁剪为空。',
 'FIFO由scope.State或插件fallback持有；CreateRuntime不新建FIFO，非1刷新值不会裁剪已有队列。',
 'State不是IDisposable，Reset只删fallback键；scope移除也只是移除持有/按接口条件Dispose，不逐帧释放图像。旧结果持有者必须另算。',
 [ref(buffer,'sealed class State'),ref(buffer,'State GetState('),ref(buffer,'public void Reset('),ref(buffer,'if(count is <0 or >15)',10),ref(contracts,'public static PortValue Image(IImageData')],
 ['ImageBuffer原生队列及其析构需要原函数体；此处与既有IB事实相互补充，非原版完全等价结论。']);
 add('TD04','ModuleResult返回与运行器清理','GraphExecutor过滤 / 校验 → NodeRunRecord',
 'GraphExecutor先在finally释放实现IDisposable/IAsyncDisposable的单轮module，随后检查取消、超时、异常、结果状态以及输出描述符。失败且PreserveFailedOutputs=false时直接Record失败，未传Outputs，记录使用Empty；所以插件字典里曾构造的状态输出不一定发布。通过检查的成功/NoMatch结果（以及显式保留失败输出的路径）复制输出字典并包ReadOnlyDictionary，值仍是同一PortValue；CompoundOutputFields.Expand也先浅复制已有键值，只为适用的复合值追加投影字段。',
 'ModuleResult与NodeRunRecord是不同边界；容器复制不等于图像像素复制，readonly也不是内部对象所有权证明。',
 '每轮records以nodeId保存；WorkflowRunContext.Complete再把值投影到流程输出/显示/局部变量，可能增加同一图像的持有者。',
 '单轮运行器Dispose不等于输出图像或跨轮scope状态Dispose；丢弃这一轮失败Outputs也不等于清工具旧缓存。',
 [ref(graph,'try{if(n.ModuleId==WorkflowCalls.ModuleId)'),ref(graph,'if(result.Status==ModuleStatus.Failed&&!result.PreserveFailedOutputs)'),ref(graph,'if(result.Continuation==ModuleContinuation.SuppressSuccessors)'),ref(contracts,'public sealed record ModuleResult('),ref('src/VisionFramework/CompoundOutputFields.cs','public static IReadOnlyDictionary<string,PortValue> Expand('),ref('src/VisionFramework/WorkflowRunContext.cs','public ExecutionSessionResult Complete(',7)],
 ['不会据清理函数名推断所有输出载荷释放；实际原子性、并发读者和native最后读者仍须按真实对象追踪。']);
 add('TD05','当前主窗结果到图像显示的能力准入','图像选择器 / 通用frame / 鼠标结果预览三条路径',
 'RefreshRight只将IToolImageListProvider或IToolImageOutputProvider加入自动图像源列表；ImageBuffer实现后者且声明15个输出，ImageBinary类并未实现这些接口，也未实现IToolDisplayProvider。RefreshToolDisplay仅向IToolDisplayProvider请求LastDisplay。主窗另有Preview.MouseLeftButtonDown入口：图像Tab且已有Preview.Source时，从所选节点结果的首个图像输出取图再ToBitmap。InputImage(selected)这个名字不能解释成图连接输入。因此二值化Image输出可以存在，却不自动成为图像选择器来源或通用图文frame；鼠标结果预览是另一入口。',
 '结果图像、来源列表条目、ToolDisplayFrame是三种载荷/能力，不能按同名Image合并。',
 '图像选择器记activeImageSourceId/activeImageOutputPort；通用provider准入与鼠标选择节点各自独立。',
 '没有provider时清通用presenter并回退普通Preview显示；这只涉及显示引用，不清结果或FIFO。',
 [ref(main,'void RefreshRight()'),ref(main,'IImageData? InputImage('),ref(main,'public MainWindow()'),ref(binary,'public sealed class ImageBinaryPlugin:'),ref(buffer,'public sealed class ImageBufferPlugin:'),ref(buffer,'public IReadOnlyList<ToolImageOutput> ImageOutputs'),ref(display,'var provider=node is null?null:',6)],
 ['本事实与现有B13一致，不声称原版二值化也具有同样三入口或界面交互；未进行桌面视觉验收。']);
 add('TD06','RefreshSelectedImageSourcePreview / ToBitmap','源结果图像 → 普通WPF预览位图',
 '普通预览先清Preview.Source，按已选来源节点及指定port（或首个图像输出）读取结果。如果节点根本没有results记录，且属于本地文件模式，才可能按CurrentImagePath经provider.LoadPreview或LoadBmp回退；已有失败记录但无图像不会走该回退。ToBitmap先ImagePreview.Packed，明确调用g.Pixels.ToArray向BitmapSource.Create提供数组，再Freeze。Gray/Rgb的Packed返回原图；Raw可能另建转换图像。因此显示位图构建有明确像素数组复制，不能把普通Preview当作直接显示上游裸指针。',
 '算法图像引用 → 可选预览格式转换 → Pixels.ToArray → 新BitmapSource；Freeze不是释放算法图像或清缓存。',
 '每次调用ToBitmap新建位图；此方法没有位图池。预览之外的结果对象仍可能保留原像素。',
 'Preview.Source=null只是解除此图像控件的持有；结果、源缓存、工具窗口和其它消费者不因此释放。',
 [ref(main,'void RefreshSelectedImageSourcePreview()'),ref(main,'BitmapSource? ToBitmap('),ref('src/VisionData/RawImage.cs','public static IImageData? Packed(',11)],
 ['这里只断言源码可见的显式ToArray与对象创建，不精确计算WPF内部额外复制/图形驱动内存或回收时点。']);
 add('TD07','IToolDisplayProvider.LastDisplay → 三presenter','frame持有 / 图像层位图 / 文本和结果投影',
 'RefreshToolDisplay按请求代次、execution owner和当前流程判Current，先Show(null)清旧显示，再在node有结果且实现provider时取frame；Current通过后把同一frame传给图像、记录和结果列表presenter。图像presenter把Frame字段设为frame，清文字/几何children与旧image.Source/图像层，然后按选中图像和勾选层重建WPF图像。Bitmap辅助函数同样Pixels.ToArray→BitmapSource.Create→Freeze。记录presenter只取RecordText，列表presenter把ResultList投影为一个字典行；不能把这两者说成持有相同的整帧字段。',
 'image presenter.Frame保留完整ToolDisplayFrame；各图像层另有BitmapSource副本；TextBlock/表行是另一种投影。',
 '清frame后尺寸归1；provider可能仍持有最近输出。最多三勾选图像层及文字/图形按frame重建。',
 'Show(null)置Frame=null并移除该presenter的图像/标签引用，不调用frame.Image.Dispose或清provider缓存；同一图像的其它持有者继续有效。',
 [ref(display,'long request=++toolDisplayRequestGeneration',22),ref(presenter,'Frame = frame; labels.Children.Clear(); image.Source = null;',11),ref(presenter,'var selectedLayer=frame.SelectedImageName',38),ref(presenter,'public void Show(ToolDisplayFrame? frame) { Text'),ref(presenter,'Columns.Clear(); ItemsSource = null;',3)],
 ['不因Current门或Freeze推断所有生产者线程安全；原版文字/几何/native图像的最后读者和生命周期仍独立核对。']);
 add('TD08','会话结果 / 当前结果视图 / 两类历史','多持有者退役与方案切换',
 '当前运行完成后经owner检查提交，StoreWorkflowCallResults把ExecutionSessionResult放进workflowSessions；RestoreWorkflowResults把session中的同一NodeRunRecord赋给results，不深复制图像。切方案ReplaceExecutionScope清configurationResults/workflowSessions/results等并关闭旧scope，这些清理不证明所有外部持有者同时清除。主框架FrameworkRunHistory仅保存序号/时间/状态文字/流程ID/耗时，最多500条，不保存图像；二值化窗口的history也只是最多256条状态摘要，但LastResult单独保留NodeRunRecord。Present先更新LastResult和摘要，再按100ms门决定是否更新图像/表格，故不同显示持有者可能短暂不同步。',
 'workflowSessions与results可指同一记录；主框架历史和二值化摘要不是像素历史，LastResult则可以继续持有输出图像。',
 'session以流程ID索引，覆盖或清除只改变相应容器引用；二值化窗口关闭只标closed，不能据此假设LastResult已置null。',
 '方案切换、清列表、关闭窗口、scope释放和最后图像读者退出必须分别核对；托管图像没有逐帧Dispose协议，不能宣布立即归还全部像素。',
 [ref('src/VisionEditor/WorkflowExecution.cs','var owner=CaptureExecutionOwner();',10),ref('src/VisionEditor/WorkflowExecution.cs','var session=await Task.Run(',3),ref('src/VisionEditor/WorkflowExecution.cs','void StoreWorkflowCallResults('),ref('src/VisionEditor/WorkflowExecution.cs','void RestoreWorkflowResults()',4),ref('src/VisionEditor/ToolExecutionScope.cs','void ReplaceExecutionScope()'),ref('src/VisionEditor.Core/FrameworkRunHistory.cs','public const int MaximumRecords'),ref('src/VisionEditor.Core/FrameworkRunHistory.cs','public void Append(',14),ref('src/VisionEditor.Core/FrameworkRunHistory.cs','public sealed record FrameworkRunHistoryEntry('),ref('src/VisionPlugins.ImageBinary/ImageBinaryWindow.xaml.cs','void Present(NodeRunRecord result)'),ref('src/VisionPlugins.ImageBinary/ImageBinaryWindow.xaml.cs','Closed+=(_,_)=>closed=true;')],
 ['没有把历史文字条数当成保留像素帧数；实际WPF窗口可达性、全部调试引用与内存峰值需要单独运行证据，当前未执行。']);
 add('TD09','ImageBuffer.OutImage0Image通用IMAGE声明','Binary.Image的GrayImage实际输入门',
 '缓存输出描述符是DataKind.Image，二值化输入描述符是DataKind.GrayImage。DataKinds.CanFlow允许通用Image声明流向图像子类型，因此声明上可连线不等于每次值都能执行。缓存不会把RgbImage转成GrayImage；PortValue.Image(RgbImage)的实际Kind是RgbImage，GraphExecutor在本轮输入PortTypes.AcceptsInput检查中不接受RgbImage→GrayImage，记录InputTypeMismatch并不进入二值化运行体。另一个有限边界是RawImage包装Kind=Image可通过通用类型准入，但Binary运行体c.Input<GrayImage>仍因实际Value不是GrayImage而抛InvalidOperationException，并在其过滤catch转换成InvalidRoiOrInput。',
 '描述符类型、PortValue.Kind与Value的具体CLR类型是三层检查，不能用泛型IMAGE输出声明代替本轮灰度像素保证。',
 'FIFO保留原帧具体Gray/Rgb/Raw格式；没有在此路线插入自动转灰缓存。',
 '类型失败不会清缓存节点队列，也不为二值化生成伪造灰度输出；旧结果持有与本轮失败记录分列。',
 [ref(buffer,'public ModuleDescriptor Descriptor{get;}',2),ref(binary,'public ModuleDescriptor Descriptor{get;}',2),ref(contracts,'public static bool CanFlow(DataKind source,DataKind target)'),ref(contracts,'public static PortValue Image(IImageData'),ref('src/VisionFramework/PortArrays.cs','public static bool AcceptsInput('),ref(graph,'try{foreach(var input in nodeInputs.ToArray())'),ref(binary,'var input=c.Input<GrayImage>',6)],
 ['这是源码静态分支结论，未运行该端到端路线；输入灰度、格式一致与成功输出仍要按实际本轮值判断。']);
 const stages={TD01:['input'],TD02:['processing'],TD03:['processing','results'],TD04:['results','retirement'],TD05:['display'],TD06:['display'],TD07:['display','retirement'],TD08:['results','retirement'],TD09:['input','processing']};
 for(const f of facts){f.stages=stages[f.id];f.modes=['all'];}
 return {facts,unknowns:['本报告仅当前工作树静态源码，不是原版调用链或native数值/ABI等价证据。','普通图像来源、鼠标结果预览、通用frame和专业工具窗口不得合并为一条无条件显示路径。','各容器清除只消除对应持有；所有外部读者退出与WPF/GC实际内存回收尚未实测。']};
};
