const fs=require('fs'),path=require('path'),{out}=require('./evidence-view.cjs'),{writeChain}=require('./explicit-chain.cjs');
const parents=['entry-routing-security','shell-lifecycle'].flatMap(view=>JSON.parse(fs.readFileSync(path.join(out,view+'.edges.json'))).edges.map(e=>({view,...e})));
const nodes=[
 ['start','InitMainWindow','构造与PreLoad调用点',700,60],['select','MainWindowData构造','AppData / 中性 / 远程资源门',700,180],['root','PreLoad新建菜单根','TitleMenueItemData',700,300],
 ['read','loadXml文件入口','IgnoreUnknown / 文件存在门',700,420],['xml','XML路径','先设原XML属性Normal',700,540],['zip','ZIP后备候选','也先操作原XML属性',1200,540],['fail','加载异常分支','PreLoad提示后Exit调用',200,540],
 ['marker','XML末子节点标识','HikXmlSecurityInfo_A1?',700,660],['decrypt','字符串解密提供器','Base64 / 字节解密 / UTF8',200,780],['stream','ZIP解包读流','多条目拼接 / 不按名称选',1200,780],
 ['parse','BaseNode.Init / 按字段解析','items落IList载体',700,900],['gate','LoadNodeListE普通类门','限定类型名 / TypeTable',700,1020],['skip','缺类型键跳过节点','所选IgnoreUnknown两支',1200,1020],
 ['factory','ActivatorWrap静态初始化','首次类型使用的依赖',200,900],['paths','默认配置缺失才回退','固定HKLM默认目录',200,1020],['libs','GetLibs / LoadAssembly','公开类型 / FullName缺键',200,1140],
 ['create','CreateInstance字符串重载','查表 / Type无参工厂',700,1140],['init','SetParentNode / Init / Add','逐节点物化',700,1260],['language','PreLoad语言门','SDK结果或SoftLanguage',700,1380],['subscribe','LangPropertyChanged += Refresh','语言门之后订阅',700,1500],
 ['host','ShowMainWindow主窗','DataContext = MainData',700,1620],['binding','TitleBar声明绑定','MenuSource.Items / MenuToolSource',700,1740],['runtime','运行时菜单实例与点击','BAML / DataContext / 路由待核',700,1860],
 ['refresh','后续语言Refresh','同一根loadXml + finally恢复',1200,1500],['cache','Items通知 / 工具栏缓存清除','前序失败可中断后续',1200,1620],['dynamic','Items动态方法与字段递归','不能从声明数推动态全集',200,1740]
].map(([id,label,detail,x,y])=>({id,label,detail,x,y}));
const edges=[];
function E(id,from,to,relation,condition,behavior,ids,classification='source-confirmed'){
 const selected=ids.map(id=>{const p=parents.find(e=>e.id===id);if(!p)throw Error('Missing menu chain parent '+id);return p;});
 edges.push({id,from,to,relation,condition,behavior,classification,parents:selected.map(p=>({view:p.view,id:p.id,classification:p.classification})),refs:[...new Map(selected.flatMap(p=>p.refs).map(r=>[r.path+':'+r.start+':'+r.end,r])).values()],runtimeExecuted:false,fullBehaviorClosed:false});
}
E('M01','start','select','constructor-call','InitMainWindow正常进入new MainWindowData','构造内部先取得AppData并CheckIsNeutral；MainData赋值在构造正常返回后。',['R26']);
E('M02','select','root','ordered-call','构造正常返回且继续PreLoad','appData非空才按IsNeutral/IsRemoteMode选四资源；否则保持字段默认MenuItems.xml。',['R01','R26','R27']);
E('M03','root','read','ordered-call','菜单根构造正常返回','赋menuSource后调用loadXml(Resources//+MenuItems,false)，不是每个菜单项直接由主窗new。',['R27','E36']);
E('M04','read','xml','guarded-resource-read','原XML存在且先行检查/属性操作正常','XML存在时走CreateXmlDoc与标识分支。',['E36']);
E('M05','read','zip','guarded-fallback-candidate','原XML不存在而替换路径ZIP存在','这只是后备分支候选，原体在读ZIP前仍设置缺失XML路径Attributes；不能保证会成功到达解包。',['E36']);
E('M06','read','fail','guarded-error-propagation','XML与ZIP都不存在，或先行文件操作抛异常并到达PreLoad catch','异常内链分类，提示/日志后Exit；提示自身异常不保证Exit。',['E36','R29']);
E('M07','xml','marker','ordered-read','CreateXmlDoc正常返回且LastChild可访问','按LastChild.Name识别标识，不是只按扩展名判断受保护内容。',['E36','E34']);
E('M08','marker','decrypt','guarded-call','LastChild.Name等于提供器NodeNameplate','取InnerText.Trim后Decrypt；字符串层Base64解码再字节解密、UTF8及去尾零。',['E34','E35','E36','E67']);
E('M09','marker','parse','guarded-call','标识不匹配且后续正常','不走所选Decrypt分支，直接交原XmlDocument.LastChild给Init。',['E36']);
E('M10','decrypt','parse','ordered-call','解密与XmlDocument.LoadXml正常返回','解密结果解析为XML再交Init；算法提供器/模式和真实资源往返尚未验证。',['E34','E35','E36','E67']);
E('M11','zip','stream','guarded-call','前序XML属性操作仍正常且到达ZIP分支','CompressUtil.DecompressToStream来自VM.Compress；ZIP分支没有再次进入XML标识Decrypt。',['E36','E38']);
E('M12','stream','parse','ordered-call','解包和XmlDocument.Load(stream)正常返回','所选助手拼接条目数组，不按名称筛选；原件读取/CRC边界见资源保护页。',['E36','E38']);
E('M13','decrypt','fail','exception-propagation','解密/解析抛错并传播到首次PreLoad','首次装配错误不是自动回退普通XML或忽略失败；Refresh调用则走不同catch。',['E36','R29']);
E('M14','parse','gate','field-carrier-call','所选items字段走IList且处理普通class节点','字段解析、泛型List载体和动态Items属性不同，未假定XML写入调用属性setter。',['R17','R18']);
E('M15','gate','skip','guarded-skip','所选普通类TypeTable不含拼接类型名','原LoadNodeListE所选IgnoreUnknown真/假分支均继续循环；不扩成全部类型分支都忽略。',['R18']);
E('M16','gate','factory','static-initialization-dependency','首次使用ActivatorWrap静态类型表','表达依赖，不断言此前所有代码都未初始化该类型；触发时点与进程历史仍未知。',['R18','R21']);
E('M17','factory','paths','ordered-initializer-call','ActivatorWrap静态构造到达ConvertDLLPath','默认配置不存在才查固定HKLM键；有效目录才改写strAppDir与LibConfigFile。',['R21','R25']);
E('M18','paths','libs','ordered-call','路径方法正常返回后静态构造继续','在typeTable锁内读取配置并加载；路径返回不证明配置/依赖存在或加载成功。',['R21','R22','R25']);
E('M19','libs','gate','type-availability-dependency','加载成功、GetTypes成功、类型IsPublic且FullName缺键','这类条件会填充TypeTable；不是所有安装文件自动可用，不是对同名类型覆盖。',['R22']);
E('M20','gate','create','guarded-call','所选普通类类型键存在','交CreateInstance字符串重载；无参Type分支优先公开静态GetInstance，否则Assembly.CreateInstance。',['R18','R23']);
E('M21','create','init','ordered-call','工厂返回满足该载体的对象且后续调用正常','所选普通类分支设置父节点、Init并加入集合；不保证每个工厂候选都能创建。',['R18','R23']);
E('M22','init','language','caller-continuation','整个菜单根loadXml正常返回至PreLoad','SDK语言查询成功用返回值，否则取VmSolution.SoftLanguage再DoChangeLanguage。',['R27']);
E('M23','language','subscribe','ordered-subscription','DoChangeLanguage正常返回','此处之后才挂Refresh，因此不能把前面的初次语言设置归因于这次订阅。',['R27']);
E('M24','subscribe','host','guarded-startup-continuation','PreLoad及BeforeStartMian后续正常返回，后台信号汇合/异常处理及AppAfterStart正常返回，loginUser!=3且FrontendFirst=false','OnStartup的静态汇合与显示分支已核；满足所列条件才交ShowMainWindow。这不是运行时达到条件或实际窗口激活证明。',['R26','R30','L24','L25','L26']);
E('M25','host','binding','source-and-resource-contract','ShowMainWindow被调用且提取资源对应实际主窗','原Exe赋DataContext=MainData；提取TitleBar声明两集合绑定。资源与消费者对应为静态合同，非运行绑定验收。',['R30','R03']);
E('M26','binding','runtime','host-activation-unknown','实际BAML实例化、DataContext继承、资源与事件路由成立','全部菜单实例/可见门/点击路由未运行验证，不宣称214声明就是完整可用菜单。',['R20','R30','R05','R06'],'unknown');
E('M27','binding','dynamic','dynamic-projection','Items getter被读取，或工具栏派生缓存首次读取','动态Items可反射替换字段；GetAllChildren直接读字段，不能等同强制刷新全部动态项。',['R07','R19','R04']);
E('M28','subscribe','refresh','event-handler-registration','后续LangPropertyChanged实际触发并正常投递','已证订阅指向Refresh；所有触发者和实际投递仍不由注册声明保证。',['R27','R14','R28']);
E('M29','refresh','read','reload-call','Refresh进入try并调用loadXml','同一menuSource重载，暂设静态命名空间；失败记录日志并finally恢复，不是PreLoad退出处理。',['R28']);
E('M30','refresh','cache','guarded-ordered-state-write','本次loadXml和Items通知均正常返回','先恢复命名空间、通知Items，再清工具栏缓存并通知；后续刷新授权显示文字，不等同重新授权。',['R28','R29']);
E('M31','cache','binding','notification-contract','属性通知实际由运行绑定消费','源码通知与声明绑定配对；所有运行消费者、异常和重入未证，不能画成必定完成重绘。',['R28','R30'],'unknown');
writeChain('menu-materialization-chain',{schemaVersion:1,id:'menu-materialization',title:'主界面菜单：资源、加密读取、物化与刷新总链',viewBox:'0 0 1400 1980',scope:'连通已有静态证据的有限总链。调用顺序、条件分支、类型注册依赖和运行未知分别标记；不穷尽全部XML类型、启动模式或动态菜单。',entryCompletenessProved:false,fullBehaviorClosed:false,runtimeVerified:false,nodes,edges,invariants:['Type registration dependency is not an observed startup timeline.','Encrypted XML and ZIP are conditional alternatives, not universal sequential steps.','A declaration, registered handler, notification or factory call does not prove runtime activation.','Relations inherit existing evidence and do not increase evidence-view or implementation completion counts.']},parents);
console.log(JSON.stringify({nodes:nodes.length,relations:edges.length,unknown:edges.filter(e=>e.classification==='unknown').length,complete:false}));
