// Current source only. Original-behaviour comments in product sources are not evidence here.
const {source}=require('./evidence-view.cjs');
module.exports=function(){
 const plugin='src/VisionPlugins.ImageSource/ImageSourcePlugin.cs',decoder='src/VisionPlugins.ImageSource/LocalImageDecoder.cs',camera='src/VisionPlugins.ImageSource/MvsCameraAcquisition.cs',folder='src/VisionPlugins.ImageSource/LocalFolderSubscription.cs',scope='src/VisionFramework/ModuleExecutionScope.cs',assets='src/VisionPlugins.ImageSource/ImageSourceAssets.cs',contracts='src/VisionFramework/Contracts.cs';
 const ref=(p,anchor,count=1)=>source(p,anchor,anchor,count),facts=[];
 const add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'independent-implementation',from,to,behavior,representation,cache,release,refs,unknowns});
 add('IS01','ImageSourcePlugin.GetState / ExecuteAsync','方案节点状态与本轮串行执行',
 '有scope时使用scope.State("vision.image-source",NodeId)；真实root状态键另含ProcedureId。无scope时由插件实例按规范化BaseDirectory/NodeId保存State。Runtime只持owner，创建新Runtime不新建全部State。执行先持scope租约、链接调用token与scope.Stopping，参数/触发检查后才取得状态并等待State.Execution信号量；finally仅在成功进入信号量后Release。',
 'State持Cache、PendingFrame、Folder/FolderPaths、Paths/Index/Sequence与StitchBuffer；本轮局部image和ModuleResult是另外的持有者。','同节点执行被State.Execution串行；整个插件没有持全局字典锁等待取图。SubmitImage交换PendingFrame不等于取得该执行信号量。','本轮结束释放信号量和scope租约，不清缓存、拼接缓冲或待提交槽。',
 [ref(plugin,'State GetState(ModuleExecutionContext context)',10),ref(plugin,'sealed class State:IDisposable',17),ref(plugin,'using var lease=c.Scope?.Enter();',17),ref(plugin,'state=owner.GetState(c);',4),ref(plugin,'if(entered)state!.Execution.Release();'),ref(scope,'public T State<T>',3)],
 ['此处只核当前托管同步结构；不证明原版线程/回调调度或所有跨线程读者都经过同一门。']);
 add('IS02','本地路径 / 嵌入编码资源','解码数组 → GrayImage / RgbImage',
 '缓存未命中时OpenEncodedImage打开文件或校验资源流，using在离开块时关闭编码流；LocalImageDecoder的WIC路径使用BitmapCacheOption.OnLoad，CopyPixels取到临时数组，再经GrayImage/RgbImage构造器data.ToArray形成独立像素。JPEG/Gray16/Indexed8/彩色分支有各自临时数组与转换，不能把只读流、解码器、临时像素和最终图像当成同一个缓冲。缓存命中不再次打开或解码。',
 '编码字节/Stream → 解码或转换临时byte[] → 拥有其私有byte[]的图像对象。','只有最终图像可能进入State.Cache；临时解码器/转换数组不是该路径字典的显式缓存项。','编码流通过using关闭；所选托管图像和临时数组未显式逐像素Dispose，按引用可达性回收。',
 [ref(assets,'public Stream OpenEncodedImage(',4),ref(plugin,'using var encoded=owner.OpenEncodedImage',7),ref(decoder,'internal static IImageData ReadImage(Stream stream,bool rgb)',18),ref(decoder,'try {decoder=BitmapDecoder.Create',4),ref(decoder,'var color=new FormatConvertedBitmap',10),ref('src/VisionData/ImageGeometry.cs','public GrayImage(',6),ref('src/VisionData/ColorImage.cs','public RgbImage(',5)],
 ['WIC/各解码后端内部native资源的实际回收时点未沿系统实现继续追踪；不能据OnLoad或using声称整条显示链的所有内存已确定释放。']);
 add('IS03','PathCache / PixelFormat / 路径列表变更','按路径首批保留，不是LRU',
 '本地模式按所选路径字符串查State.Cache，命中直接取同一IImageData；未命中仅在Count<PathCache时Add，不执行LRU替换。当前源码仅在执行到本地分支且PixelFormat参数改变时Clear全部Cache。降低PathCache（含变0）、删除列表项、切换CurrentImagePath、目录dirty刷新或文件内容变化，本段都没有逐项驱逐Cache；旧条目可继续占容量，仍命中的同路径可继续使用旧像素。进入相机或SDK模式本身也不执行这里的Clear。',
 'Dictionary<string,IImageData>以Ordinal路径字符串为键，字典保留最终图像引用，不在Add时再复制像素。','容量是新增准入上限，不是持续强制裁剪；列表Paths和像素Cache是不同容器。','Cache.Clear只移除字典引用；已发布结果仍可能保留同一图像，不能称立即释放像素。',
 [ref(plugin,'if(!state.Paths.SequenceEqual(paths,StringComparer.Ordinal))',6),ref(plugin,'if(state.PixelFormat!=c.Integer("PixelFormat"))',13)],
 ['未运行文件覆盖/参数切换的桌面时序探针；这里是源码确认的当前保留策略，不能用产品源码中的native注释代证原版相同。']);
 add('IS04','LocalFolderSubscription文件事件','合并dirty → 目录路径列表 → 图像源执行',
 '文件监视器仅持dirty标志与scope/node引用；Created/Deleted调用Changed(true)，Changed/Error调用Changed(false)，没有Rename订阅。Changed合并置dirty，只有requestRun且TriggerRun才请求运行。图像源执行ConsumeDirty后重建FolderPaths，RefreshImage为true时请求界面刷新；这不是像素缓存驱逐。离开文件夹模式或改变目录时关闭旧Folder；切换目录新建监视器。',
 'FileSystemWatcher通知 → 一个合并标志与路径列表；不把文件事件当成已经解码的新帧。','FolderPaths刷新独立于Cache命中；同路径已缓存图像不因dirty自动重新解码。','LocalFolderSubscription.Dispose在锁内标disposed并Dispose watcher；它不清图像源Cache。',
 [ref(folder,'internal LocalFolderSubscription(',10),ref(folder,'void Changed(bool requestRun)',3),ref(plugin,'if(!folderMode){state.Folder?.Dispose();state.Folder=null;}',12)],
 ['原版完成端口事件与当前FileSystemWatcher并非已证明等价；事件丢失、并发改名和实际重入时序未运行核实。']);
 add('IS05','SubmitImage(scope,node,image)','单个PendingFrame引用槽 → SDK模式本轮image',
 'SubmitImage使用Interlocked.Exchange直接保存调用者传入的IImageData引用，没有克隆像素，也不排队；连续提交会覆盖尚未消费的旧槽，Exchange返回的旧值未被接收/Dispose。SDK模式执行以Exchange(PendingFrame,null)取走该引用；无帧则失败。取走后才检查取消并进入拼接/输出等后续分支；后续失败不会把该帧退回PendingFrame。SubmitImage的scope键必须与运行实际scope的procedure/module/node一致才能命中同一状态。',
 '提交者与PendingFrame可共用同一图像；消费只是引用转移到局部变量，并不创建图像副本。','单槽最新提交覆盖，不是FIFO，不自动重用上轮已消费帧。','覆盖/消费使槽解除旧引用，但提交者或旧结果仍可持有；当前image接口无统一Dispose合同。',
 [ref(plugin,'public void SubmitImage(',2),ref(plugin,'} else if(sourceMode==3){',13),ref(scope,'public T State<T>',3)],
 ['SDK任意IImageData实现的内部缓冲是否可变/外借、以及调用者提交后继续写入是否安全，不能由引用槽或Interlocked保证。']);
 add('IS06','MvsCameraAcquisition.AcquireAsync','native临时缓冲 → 托管CameraFrame.Image',
 '图像源取得IImageAcquisitionService后使用frame.Image引用，本身不在这里克隆。所选Mvs适配器在Task.Run的gate锁内读取相机；按PayloadSize分配native buffer。目标格式相符时Marshal.Copy到pixels；不符则额外分配converted，调用像素转换后Marshal.Copy，并在inner finally释放converted。返回CameraFrame前GrayImage/RgbImage构造器再复制pixels；outer finally释放buffer，因此此适配器返回的图像不借用已经释放的native buffer。',
 'native采集缓冲 → 临时托管pixels → 构造器私有数组 → CameraFrame → 图像源image引用。','设备句柄按cameraId保留于适配器；采集buffer/converted是每次调用的临时分配，不是源PathCache。','两级native临时分配分别有finally；适配器Dispose停止/关闭/销毁已记入handles的设备并Unload ALC。静态libraries字典仍保留native库句柄，本Dispose没有NativeLibrary.Free。',
 [ref(plugin,'var acquisition=c.Scope?.Service<IImageAcquisitionService>();',5),ref(camera,'public ValueTask<CameraFrame> AcquireAsync',3),ref(camera,'object payload=Create("MVCC_INTVALUE_EX")',12),ref(camera,'public void Dispose(){'),ref(camera,'static readonly System.Collections.Concurrent.ConcurrentDictionary'),ref(camera,'library=libraries.GetOrAdd('),ref('src/VisionData/ImageGeometry.cs','public GrayImage(',6),ref('src/VisionData/ColorImage.cs','public RgbImage(',5),ref('src/VisionFramework/ImageAcquisition.cs','public sealed record CameraFrame')],
 ['未加载相机SDK、未采集设备；这里只核当前适配器源码。其它IImageAcquisitionService可有不同所有权策略，不能把本适配器的拷贝保证推广给全部服务。','超时轮询/取消是合作检查，不证明native调用能被立刻抢占；Dispose中某个设备API抛错后的剩余清理也未由逐句成功保证。']);
 add('IS07','StitchEnable / retained StitchBuffer','拼接图像快照与下一轮前缀分开',
 '拼接有效时State保留StitchWidth及StitchBuffer。ClearTrigger命中会Array.Clear当前数组并把宽度设0，不立即释放数组。总输出字节长度改变才new StitchBuffer；本轮原图先复制到prefix之后，再调用GrayImage/RgbImage构造器复制完整StitchBuffer为输出快照，最后将原图指定条带复制到State.StitchBuffer开头，作为下一轮前缀。这个先构造输出、后改前缀的顺序意味着输出不共享随后更新的State数组。关闭拼接分支不会清掉已保留的StitchBuffer。',
 '原image像素 → State拼接工作数组 → 输出图像构造器私有副本；随后State前缀更新不修改已产生输出像素。','尺寸/宽度/范围失败、触发清空与长度重分配是不同分支；宽度不符或start+height越界在拷贝前失败。','重分配解除State对旧工作数组的引用；禁用拼接并未主动清缓存，最终回收仍由引用可达性决定。',
 [ref(plugin,'if(c.Boolean("StitchEnable")',22),ref('src/VisionData/ImageGeometry.cs','public GrayImage(',6),ref('src/VisionData/ColorImage.cs','public RgbImage(',5)],
 ['这里只证明Gray/Rgb所选当前构造路径；额外外部图像格式/stride/原生拼接内存布局不能据channels分支泛化为兼容。']);
 add('IS08','图像源局部image / 附加Mono8转换','ModuleResult.Image与ImageMono8',
 'Image输出通过PortValue.Image包装当前image；本地缓存命中、SDK单槽、相机frame.Image在未拼接时分别传入各自原对象。宽高/格式/序列等是独立标量输出，不是新图像。OutMono8只有image为RgbImage时生成额外图像：ImageColorConversion先分配灰度临时数组，再经GrayImage构造器复制；主Image不被替换。相机/SDK移除名称与路径字段，SDK还移除SNCode；这些输出缺席不能解释为内部帧/计数被清空。',
 '主图包装不深复制；附加Mono8转换另建独立图像数组；metadata不承载像素。','成功ModuleResult可能与State.Cache、提交者或CameraFrame同时持有主图。后续结果字典复制的深度由框架另核。','输出局部变量退出不释放仍被结果或缓存引用的像素；RGB附加灰度也需追其独立消费者。',
 [ref(plugin,'var outputs=new Dictionary<string,PortValue>',14),ref(contracts,'public static PortValue Image(GrayImage',2),ref('src/VisionData/ImageColorConversion.cs','public static GrayImage ToMono8(',13),ref('src/VisionData/ImageGeometry.cs','public GrayImage(',6)],
 ['主窗只选某端口显示、历史是否保留及结果过滤由消费者另核，不能只见成功返回就宣称所有输出都被界面显示或及时释放。']);
 add('IS09','Reset / State.Dispose / scope退役','撤销状态持有 ≠ 确定释放所有像素',
 'State.Dispose仅关闭Folder并设null，没有清Cache、PendingFrame、StitchBuffer，也没有Dispose Execution信号量。Reset(scope,node)通过scope.RemoveNode移除同procedure状态并调用State.Dispose；无scope的Reset(node)只从插件states字典移除，不调用被移除State.Dispose。root方案scope关闭先取消，等租约归零后清状态/拥有服务并Dispose可释放者；borrowed procedure scope.Dispose直接返回。因此不能把工具窗口关闭、单轮Runtime结束、Reset和方案关闭当成同一个释放事件。',
 'State、结果记录、预览图像和外部提交者是可能独立的引用持有者。','State从字典移除后仍可能暂被在飞执行持有；旧结果可继续持有已产生的图像。','这里确认的是移除状态、关闭watcher与scope清理；没有像素归零、立即GC或所有外部读者终止的保证。',
 [ref(plugin,'public void Dispose(){Folder?.Dispose();Folder=null;}'),ref(plugin,'public void Reset(ModuleExecutionScope'),ref(plugin,'public void Reset(string nodeId)',3),ref(scope,'public void RemoveNode('),ref(scope,'public void Dispose(){'),ref(scope,'void Release(){')],
 ['RemoveNode与在飞执行的所有者门由宿主另核；不从State.Dispose内容直接断言泄漏，也不以root租约退出证明全部显示/历史最后读者已结束。']);
 add('IS10','LoadPreview / PrepareSave','预览临时解码与持久化编码资源是另外两条支路',
 'LoadPreview每次独立OpenEncodedImage并解码，不查询State.Cache。PrepareSave克隆节点参数，取配置列表或文件夹目录列表；SolSaveImageData为true时最多取120条路径，复制编码流到byte[]，计算hash并Base64写ResourceDocument，返回新参数与资源快照。它保存编码文件资源，而不是把正在运行的Cache、PendingFrame或StitchBuffer序列化。预览流在using退出关闭；保存中的stream和MemoryStream各自using，输出ResourceDocument继续持有Base64字符串。',
 '运行像素缓存 / 预览解码像素 / 保存的编码文件字节与Base64是三种载荷。','预览不表示当前工具已经运行；保存列表和首N像素缓存的准入/容量彼此独立。','using关闭编码流，不消灭资源字符串、最终预览图像或主运行缓存；这些由后续文档/显示持有者决定寿命。',
 [ref(plugin,'public VisionCore.IImageData LoadPreview('),ref(assets,'public ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources,ModuleExecutionScope? scope)',16)],
 ['方案加载、资源验证实现、资源删除/去重及主窗预览释放需沿其消费者继续核；这条保存分支不证明原.sol格式兼容。']);
 const organization={IS01:[['input','processing','retirement'],['all']],IS02:[['input'],['local']],IS03:[['input','results','retirement'],['local']],IS04:[['input','retirement'],['local']],IS05:[['input','retirement'],['sdk']],IS06:[['input','retirement'],['camera']],IS07:[['processing','results'],['all']],IS08:[['results'],['all']],IS09:[['retirement'],['all']],IS10:[['input','display','retirement'],['local']]};for(const f of facts){[f.stages,f.modes]=organization[f.id];f.organizationMeaning='stages/modes为研究分组，不是额外已证运行调用边。';}
 return{facts,unknowns:['本组全为当前独立实现静态源码事实；未把源码中的原版注释或历史运行摘要提升为原版源码确认。','未执行当前程序、原版、设备或SDK；实际像素峰值、GC回收时刻与所有显示/历史最后读者仍需另外证据。']};
};
