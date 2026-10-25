const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out,source}=require('./evidence-view.cjs');
module.exports=function(){
 const plugin='src/VisionPlugins.ImageSource/ImageBufferPlugin.cs',window='src/VisionPlugins.ImageSource/ImageBufferWindow.cs',graph='src/VisionFramework/GraphExecutor.cs',contracts='src/VisionFramework/Contracts.cs',scope='src/VisionFramework/ModuleExecutionScope.cs';
 const ref=(p,anchor,count=1)=>source(p,anchor,anchor,count);
 const facts=[],add=(id,from,to,behavior,representation,cache,release,refs,unknowns)=>facts.push({id,classification:'independent-implementation',from,to,behavior,representation,cache,release,refs,unknowns});
 add('IB01','ImageBufferWindow.Link / 整数提交、订阅','GraphExecutor nodeInputs → Runtime.Read',
 '窗口把图像、RefreshFlag、BufferCount作为输入端口处理。整数文本提交调用SetInputInteger，订阅按钮先Commit再PickSubscription。运行体从c.Inputs取整数，默认数量15、刷新1，不从c.Parameters取这两个值；checked long→int越界不在插件内捕获，由外层执行器记ModuleException。界面配置和运行读取不能合并成普通算法参数字典。',
 '输入字典内PortValue由GraphExecutor连接/订阅解析；此工具Describe的算法参数数组为空。','整数默认仅在对应输入键缺席时使用；刷新标志只认等于1。','本段无图像分配/释放。',
 [ref(window,'void Commit()'),ref(window,'var button=new W.Button{Name="Subscribe"'),ref(plugin,'public ModuleDescriptor Descriptor{get;}',2),ref(plugin,'int Read(string name,int value)',2),ref(graph,'nodeInputs[c.ToPort]=v;')],
 ['当前订阅弹层的完整外观、原版动态输入设置器逐分支尚未在本专项闭合。']);
 add('IB02','数量/刷新门与节点状态','State.Images FIFO变更',
 'count<0或>15在GetState之前返回错误；合法数量后取得状态并加锁，再检查取消。只有flag==1才读取InputImage、复制并裁剪。flag为0、-1、2或其它非1值时保留队列，数量变小也不裁剪。flag==1且缺图时不改既有队列。',
 'Queue<IImageData>按最旧到最新保存；数量0仍先复制入队，然后出队至空。','GetState按scope或fallback选择持久状态；不是每次CreateRuntime建立新队列。','裁剪仅Dequeue引用，无像素Dispose/清零调用。',
 [ref(plugin,'sealed class State'),ref(plugin,'if(count is <0 or >15)',9)],
 ['原所选分配/关闭callee已由NM01–NM04续证；完整池分支、竞争与异常安全仍未闭合，本段只确认当前实现分支。']);
 add('IB03','本轮输入图像','独立拥有的缓存图像',
 '刷新时仅支持GrayImage、RgbImage、RawImage三个具体类型，分别调用构造器复制；其它IImageData实现抛ArgumentException。三种构造器均data.ToArray，因此入队图像和上游图像不是同一对象或同一数组。Gray/Rgb校验精确载荷长度，Raw保留格式、stride及全部非空载荷；此处不调用预览格式转换。',
 '缓存复制尺寸、格式和像素；Raw不解码、不按width*height截断。','复制后Enqueue，再按count裁剪；count0的临时副本也经历分配。','托管数组由引用可达性决定回收；非确定时刻释放。',
 [ref(plugin,'else {var image=input.As<IImageData>();'),ref('src/VisionData/ImageGeometry.cs','public GrayImage(',6),ref('src/VisionData/ColorImage.cs','public RgbImage(',5),ref('src/VisionData/RawImage.cs','public RawImage(',4)],
 ['不模拟损坏原生指针、任意外部IImageData或内存不足后的原版行为；不声称原生CopyImage位等价。']);
 add('IB04','FIFO槽位','ModuleResult → GraphExecutor记录 → 下游',
 '成功时从队首枚举并发布OutImage0Image等键，PortValue.Image仅包装同一缓存图像对象。暂停刷新重复运行可让不同ModuleResult持有相同图像；主流程成功路径只复制输出字典并包只读视图，不复制像素。下游nodeInputs接收同一个PortValue。因此一次入队的深复制不能解释为每层都深复制。',
 '缓存图像对象 → PortValue → 字典/NodeRunRecord；图像像素不随包装再复制。','旧结果可在帧被队列淘汰后继续保持图像可达。','从队列移除不等于销毁被旧结果/显示仍引用的数组。',
 [ref(plugin,'if(code==0){int i=0;foreach'),ref(contracts,'public static PortValue Image(GrayImage'),ref(contracts,'public static PortValue Image(IImageData'),ref(graph,'if(result.Continuation==ModuleContinuation.SuppressSuccessors)'),ref(graph,'nodeInputs[c.ToPort]=v;')],
 ['显示位图、历史容器及所有外部消费者的最终引用释放时点尚未逐一闭合；不能由只读字典保证对象生命周期短。']);
 add('IB05','失败ModuleResult中的状态/耗时','主流程失败NodeRunRecord空输出',
 '插件正常错误返回仍填ModuStatus与ModuRunTime，并设置NativeErrorCode，但未设置PreserveFailedOutputs；其默认bool为false。GraphExecutor在此分支先Record失败再continue，未把result.Outputs传给Record，Record默认Empty。因此插件直接调用可读到的失败状态/耗时，在主流程记录与下游端口中被丢弃；保留的是失败码和诊断字段。插件构造输出与主框架最终发布必须分别陈述。',
 '失败结果与失败记录是两个不同边界；不能仅查插件返回字典判断主窗可读字段。','缺图/非法数量不清已有缓存；失败这一轮不重发缓存图像。','既有缓存仍可在后续合法暂停刷新中发布；失败不等于清缓存。',
 [ref(plugin,'output["ModuStatus"]',2),ref(contracts,'public sealed record ModuleResult('),ref(graph,'void Record(NodeRunStatus'),ref(graph,'if(result.Status==ModuleStatus.Failed&&!result.PreserveFailedOutputs)')],
 ['旧IMAGE-BUFFER文档中“失败发布状态0”只可直接用于插件返回边界，不能据此断言当前主流程保留该端口；本轮记录差异，不修改产品。','普通配置自执行已另由IB09核实相同过滤；另一ToolDebugSession路径在此不合并作同一入口。']);
 add('IB06','插件实例 / scope / 节点','缓存持有与RemoveNode、关闭释放',
 'scope.State的实际键是(ProcedureId,module,node)，状态属于方案root；无scope时fallback键为规范化BaseDirectory加node，保留于插件实例。Runtime只保留owner且不实现Dispose，不随单轮运行器结束清队列。Reset只从fallback移除指定node，并不移除scope状态。RemoveNode移除状态后只对IDisposable调用Dispose，缓存State不实现该接口。根scope关闭先取消，待租约归零后清状态字典；该State也不会执行逐帧Dispose。',
 'State.Queue是独立持有者；scope和fallback两条路径不可混用Reset解释。','RetainProcedureNodes通过RemoveNode清除不再存在的状态键。','移除缓存引用依靠托管GC；旧ModuleResult仍可持图，不能称立即归还全部像素内存。',
 [ref(plugin,'State GetState('),ref(plugin,'public void Reset('),ref(plugin,'sealed class Runtime('),ref(scope,'public T State<T>',3),ref(scope,'public void RemoveNode('),ref(scope,'void Release(){')],
 ['未证明所有历史、UI位图、订阅外部持有者同时清空；实际峰值和回收时刻需独立内存观测。']);
 add('IB07','工具窗口执行/连续/关闭','停止下一轮与缓存生命周期',
 '专用窗口Run传入克隆的Node.Parameters，但缓存输入实际仍由宿主提供。连续按钮在每轮await Execute之后再判断continuous/closed；Closed只设置closed=true、continuous=false。窗口本身没有调用插件Reset、scope.RemoveNode或对在飞context.Run发出取消。因此关闭编辑器不等于清缓存或证明正在执行的本轮已经中断。',
 '窗口参数快照、输入端口和状态队列分属不同持有者。','关闭只阻止本窗口循环启动后续轮；队列仍按scope/fallback持有。','窗口关闭处理未出现像素Dispose或状态移除。',
 [ref(window,'var rows=new W.StackPanel'),ref(window,'async Task Execute()')],
 ['普通配置宿主的活性/发布/取消边界另见IB09；此窗口处理器本身不能证明其它宿主的取消或排空行为。']);
 add('IB09','主窗OpenToolConfiguration → ConnectToolEditorServices','配置自执行快照、发布缓存与关闭准入',
 '当前普通配置入口为OpenToolConfiguration：建立ConfigurationActive后连接服务再创建ImageBufferWindow；ConnectToolEditorServices把Run接到RunConfiguredTool。它先快照流程会话和configurationResults中的上游记录，只允许成功/NoMatch来源，解析常量/订阅后构建单节点isolated流程交给GraphExecutor，不重新执行上游。故IB05的失败输出过滤也适用于这个配置自执行入口。完成后还要核窗口活性、方案、scope实例和节点身份，才写configurationResults及当前results并刷新右侧。关闭窗口令active=false，阻止过期发布，但传给执行器的取消token是ownerScope.Stopping，不能据此声称关闭配置窗会取消在飞执行或回滚已修改缓存。',
 '输入来自本次捕获的上游记录；参数Clone；运行结果记录由身份准入后进入两份结果字典。','配置自执行不会清方案节点状态；失败记录可覆盖该节点旧配置记录，但不等于清ImageBuffer FIFO。','关闭配置窗口撤发布资格；方案scope关闭才触发Stopping。窗口失效不能证明队列修改已撤销。',
 [ref('src/VisionEditor/ToolConfigurationHost.cs','var context=new ToolEditorContext',8),ref('src/VisionEditor/ToolExecutionScope.cs','Run=values=>'),ref('src/VisionEditor/ToolExecutionScope.cs','var available=(workflowSessions.TryGetValue',6),ref('src/VisionEditor/ToolExecutionScope.cs','var run=await Task.Run(()=>new GraphExecutor',3),ref('src/VisionEditor/ToolExecutionScope.cs','if(CanPublish()&&doc.SolutionId',8)],
 ['ToolEditorHost的另一条ToolDebugSession/NodeDebugExecutor路径有独立关闭排空逻辑，不能与普通配置入口混作一条调用链；本事实仅绑定OpenToolConfiguration实际构建的context。','关闭与在飞回调的实际桌面时序未运行验证；原版配置关闭/缓存副作用仍待原函数体与确认运行证据。']);
 const dir='D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/ImageBufferModule',xml=dir+'/ImageBufferModuleAlgorithmTab.xml',bytes=fs.readFileSync(xml),lines=bytes.toString('utf8').split(/\r?\n/);
 facts.push({id:'IB08',classification:'source-confirmed',from:'原ImageBufferModuleAlgorithmTab.xml',to:'整数输入选择/回填声明',behavior:'CacheEnable和BufferCount均声明ButtonSelecter；DropOpenSetter调用GetFrontFilterTreeOperation(int)，CurValue触发SetFilterSourceOperation到RefreshFlag/BufferCount，Initers通过GetSelectedFilterOperation回填。这证明资源声明了输入源选择与回填链，不证明原生缓存执行、真实活绑定或队列释放。',representation:'原资源把刷新和数量声明为输入源操作，不应按普通算法参数值解释。',cache:'资源不定义像素缓存结构或淘汰行为。',release:'资源未声明原生像素释放合同。',refs:[{path:xml,member:'AlgorithmTab resource: ButtonSelecter / setters / initializers',start:1,end:lines.length,sha256:crypto.createHash('sha256').update(bytes).digest('hex'),excerpt:lines.join('\n'),evidenceType:'shipped-resource'}],unknowns:['历史native/*.c与受控运行产物未作为本轮原体证据；本轮已从身份匹配原DLL独立静态恢复五个函数，见NB01–NB04，不以旧报告替代。','此前REA provider_unavailable响应保留为历史；本轮使用已有离线Ghidra恢复所选五体，并以NM01–NM04续证接口/映射清理；ModuleFrame接收端和最终像素读者仍待核。']});
 const nativePath=dir+'/ImageBufferModule.dll',nativeBytes=fs.readFileSync(nativePath),nativeSha=crypto.createHash('sha256').update(nativeBytes).digest('hex');if(nativeSha!=='ab8850d7a747df693152ef7e095a5e0ef87eff9410c2424e9b7c84e435717869')throw Error('Original ImageBuffer native identity changed');
 const result={schemaVersion:1,complete:false,pluginKey:'Collection/ImageBufferModule',facts,originalArtifact:{path:nativePath,sha256:nativeSha,length:nativeBytes.length,reaOpenIdentityMatched:true,identityOnlyNotFunctionBody:true},originalRuntimeExecuted:false,currentRuntimeExecuted:false,browserRendered:false,nativeSourceRecovered:true,nativeSelectedFunctions:5,nativeOwnershipClosed:false,nativeSourceEvidenceReport:'buffer-native-ownership.provenance.json',nativeRetrieval:'image-buffer-native-retrieval.json',historicalReportsRevalidated:false,scope:'当前缓存图像工具跨层静态调用/数据生命周期；原资源声明与另列NB01–NB04原生五体切片分列；未证明全部缓存所有权与像素释放。'};
 fs.writeFileSync(path.join(out,'tool-data-lifecycle.json'),JSON.stringify(result,null,2)+'\n');return result;
};
