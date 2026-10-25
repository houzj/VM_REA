const fs=require('fs'),path=require('path'),{out}=require('./evidence-view.cjs'),{writeChain}=require('./explicit-chain.cjs');
const parents=['plugin-sessions','parameter-candidates','entry-routing-security'].flatMap(view=>JSON.parse(fs.readFileSync(path.join(out,view+'.edges.json'))).edges.map(e=>({view,...e})));
const nodes=[
 ['main','主界面工作流模块','实际事件投递未全证',700,60],['area','画布按钮 / MODULE双击样式','DiagramArea.ConfigParamsCommand',250,180],['procedure','流程设置按钮','ProcedureConfigControl命令',1150,180],['open','ProcedureConfigControl.OpenParams','配置入口原方法',700,300],['gate','权限 / ConfigCheck','不是所有角色同一拒绝门',700,420],['files','OpenParamsTabControl','XML完整性 / 历史记录',700,540],['window','扩展窗或通用参数窗','ParamsConfig / ROI / Owner',700,660],
 ['group','Group运行参数编辑按钮','模板与实际DataContext待核',700,780],['data','GroupRunParam.ConfigCommand','专用数据 / 历史抑制 / Invoke',700,900],['modal','DynamicRunParamConfigWin','回调设Owner/DataContext并显示',700,1020],
 ['checkgate','ConfigCheck虚拟getter','角色门后才读取',250,300],['basecheck','选定IMVSGroup继承','VmModule恒true；无覆盖',250,420],['proxycheck','VmProxyModule转发','动态装入目标再检查',250,540],
 ['direct','Group装饰器独立命令','绑定/调用者未定位',1150,540],['password','PasswordA独立校验窗','读取IsPasswordOK',1150,660],['directwin','独立通用模态参数窗','非OpenParams / 无扩展查找',1150,780],
 ['worker','AsyncLoadParams后台任务','Task丢弃 / static CheckedParams',250,1020],['candidates','GetInsideRunParamTree','普通模块 / Group不同过滤',250,1140],['lazy','ExpendHolder首次子枚举','先置isLoaded / 回填可写源路径',250,1260],['check','参数勾选命令','单项 / 批量 Add与Delete',250,1380],['write','集合 / SDK / 索引写入','即时修改 / 无整体事务回滚',250,1500],['userdata','Owner.UserData索引器','生命周期 / 历史 / 动态分支',250,1620],['sdk','动态参数及UI参数SDK','托管调用点不等于原生效果',250,1740],['native','原生状态与方案落盘','调用成功 / 往返仍未证明',250,1860],
 ['scope','Modules / Children','共享缓存与懒装载',700,1140],['first','默认1 / Group20','消费SDK第一层节点',700,1260],['close','忙状态关闭门','忙时Cancel；放行先赋true',700,1380],['return','Group回调正常返回','结果直接pop；不要求true',700,1500],['refresh','恢复历史 / 清缓存 / 重读','仅正常延续；没有finally保障',700,1620],
 ['relation','关联列模板按钮','EditModulesMethod委托',1150,1020],['relwin','专用关联窗口','校验源与窗口DataContext分开',1150,1140],['root','关联候选生产','模块类型名/参数名；单根子项',1150,1260],['relclose','关联窗口Closing','仅true时校验首根子项>128',1150,1380],['true','关联调用者结果门','false/null返回，true才继续',1150,1500],['diff','关系字典差异 / SDK增删','先改字典再调用SDK',1150,1620],['text','RelationModulesStr getter','清字典 / SDK重读 / 吞异常',1150,1740],['relnative','关联native及持久化未知','字典更新不是完整落盘证明',1150,1860]
].map(([id,label,detail,x,y])=>({id,label,detail,x,y}));
const edges=[],P=id=>['parameter-candidates',id],S=id=>['plugin-sessions',id],U=id=>['entry-routing-security',id];
function E(id,from,to,relation,condition,behavior,keys=[],classification='source-confirmed'){
 const selected=keys.map(([view,id])=>{const p=parents.find(p=>p.view===view&&p.id===id);if(!p)throw Error('Missing Group configuration parent');return p;});
 edges.push({id,from,to,relation,condition,behavior,classification,parents:selected.map(p=>({view:p.view,id:p.id,classification:p.classification})),refs:[...new Map(selected.flatMap(p=>p.refs).map(r=>[r.path+':'+r.start+':'+r.end,r])).values()],runtimeExecuted:false,fullBehaviorClosed:false});
}
E('Q01','main','area','host-event-activation-unknown','画布模板、附加双击/命令及委托属性实际激活','按钮/双击样式、附加事件原件体及命令已证；辅助先Handled再CanExecute(null)，通过才Execute实际参数。实际主界面实例、属性/模板激活和DataContext/投递仍未全证。',[S('P08'),S('P11'),S('P16'),S('P17'),S('P19'),S('P21'),S('P22'),S('P31'),S('P32'),S('P33'),S('P34'),S('P35'),S('P38'),S('P39')],'unknown');
E('Q02','open','gate','guarded-configuration-entry','进入该控制器OpenParams','CanOpenConfigurationInterface和ConfigCheck放行才继续；Tech/Operator的ConfigAvailable门不外推所有角色。',[S('P01'),S('P12')]);
E('Q03','gate','files','guarded-ordered-call','权限门通过，旧窗关闭后继续尝试打开','旧窗引用仍不为空则OpenParamsTabControl返回；路径与三类XML完整性检查通过后才继续。',[S('P01'),S('P02')]);
E('Q04','files','window','conditional-window-factory','资源条件满足；GetExternTabWin返回候选或null','先查扩展Control程序集候选；为空才回退通用窗口，赋ParamsConfig/ROI/Owner并Show。',[S('P02'),S('P03'),S('P04')]);
E('Q05','window','group','group-template-activation-unknown','Group参数节点、专用模板及实际绑定在宿主激活','通用/扩展窗口创建不证明Group专用控件在运行实例中已出现；其它程序集副本与模板选择也未全证。',[S('P04'),S('P18'),S('P19'),P('P16'),P('P38'),P('P39'),S('P20'),P('P40'),P('P41')],'unknown');
E('Q06','group','data','binding-declaration','专用模板按钮绑定成功并执行ConfigCommand','声明将EditRunParamMethod作为命令参数；Group命令构造GroupRunParamConfigData，抑制历史再Invoke。',[P('P15'),P('P16')]);
E('Q07','data','modal','delegate-construction-and-call','传入的是专用EditRunParam委托并正常进入','回调new专用窗口、设Owner和数据，Group数据时设RelateControl并启动加载，再ShowDialog。',[P('P16')]);
E('Q08','modal','worker','pre-modal-task-scheduling','Group数据类型命中；显示对话框前调用AsyncLoadParams','Task.Run后丢弃Task，不等其完成再ShowDialog；此边表示调度，不是后台任务必成功。',[P('P16'),P('P17')]);
E('Q09','worker','candidates','candidate-initialization-call','worker推进至RunParamPickData.Init','写Owner/筛选/搜索和ParamList后Init，调用GetInsideRunParamTree；异常没有本体清理finally。',[P('P17'),P('P20')]);
E('Q10','candidates','scope','module-scope-read','枚举IVmModuleSet.Children','Children显式getter转Modules；读取可能触发原模块懒装载，不是纯静态列表。',[P('P20'),P('P29')]);
E('Q11','scope','first','virtual-lazy-load','modules_IsLoaded=false且锁内复查仍未装载','默认LoadModules传1，IMVSGroup覆盖传20；消费SDK第一层返回，再替换缓存，锁外仍有连接与导入。',[P('P29'),P('P30')]);
E('Q12','candidates','lazy','nonempty-holder-production','对应模块过滤结果非空，随后首次读取子枚举','普通模块可见性/启用/类型/搜索/访问门与嵌套Group的CloneSource类型/搜索门分列；worker可主动触发首次子枚举。',[P('P20'),P('P21')]);
E('Q13','lazy','check','selection-command-activation-unknown','控件勾选与CheckCommand实际投递到配置数据','Group专用模板框绑定CheckEnabled和祖先数据CheckCommand，以Holder作参数；容器自带框无此命令且在Group被隐藏。命令getter与选定Mvvm包装已证，默认CanExecute=true且接口Execute不重查，异常交Task/async void构建器；但原BAML/实际程序集/绑定、投递和操作先后仍未知。',[P('P21'),P('P22'),P('P32'),P('P33'),P('P34'),P('P35'),P('P36'),P('P37'),P('P39')],'unknown');
E('Q14','check','write','guarded-immediate-mutation','CheckEnabled.Value决定新增或删除分支','编辑命令即时修改，不等待窗口关闭统一提交；批量新增失败不回滚先前成功项。',[P('P22')]);
E('Q15','write','userdata','ordered-property-write','新增/删除按各自正常前序继续','按动态参数写入顺序调整集合、索引、XML和路径键；删除存在局部VmException拒绝分支。',[P('P22')]);
E('Q16','write','sdk','sdk-method-call','执行到新增/删除动态参数及关联调用点','AddDynamicParam/Relation或DeleteDynamicParam与集合变更非原子；只证明托管调用位置。',[P('P22')]);
E('Q17','userdata','sdk','conditional-storage-dispatch','Parent、生命周期、历史与动态UI参数分支允许','UserData不是内存字典同义词；设置按动态/普通分支调用SDK，空数据在普通分支可走删除。',[P('P22'),U('E66')]);
E('Q18','sdk','native','native-and-persistence-effect-unknown','SDK真实执行及方案保存/重开','本图未执行SDK、厂商程序或设备；native状态、失败回滚与最终方案文件往返均未知。',[P('P22'),U('E66')],'unknown');
E('Q19','modal','close','closing-override','专用窗口进入OnClosing','忙标志false时e.Cancel=true并返回；放行先DialogResult=true，再调用基类，不保证实际关闭成功。',[P('P18')]);
E('Q20','modal','return','framework-modal-return-unknown','ShowDialog正常返回到委托并由Invoke正常返回','原调用点已证；模态回流/基类取消/实际结果和异常仍未运行证明。',[P('P16'),P('P18'),P('P19')],'unknown');
E('Q21','return','refresh','normal-continuation-result-discard','DialogHandler.Invoke正常返回，任意nullable结果','Group参数入口直接pop结果；随后恢复历史、清缓存/通知/GetParamUnit，没有finally保障异常后恢复。',[P('P19')]);
E('Q22','modal','relation','nested-template-activation-unknown','Group数据面板实际显示关联列并激活条目模板','资源存在关联列按钮与委托声明，但运行模板、条目DataContext和事件激活仍未证。',[P('P16'),P('P23')],'unknown');
E('Q23','relation','relwin','command-and-delegate-contract','RelationModulesConfigCommand进入并使用EditRelationModules','命令先创建ModulesPickData；窗口校验绑定源是面板DataContext，窗口数据另设为传入选择数据。',[P('P23'),P('P25')]);
E('Q24','relation','root','pre-dialog-candidate-build','命令解析Owner路径并ModulesPickData.Init','候选在委托打开窗口前已初始化；匹配模块类型名/参数名，第三显示名未参与过滤，结果单根直接子项。',[P('P25'),P('P27'),P('P28')]);
E('Q25','root','scope','shared-module-cache-read','GetParamsNode读取vmModuleSet.Modules','与左侧Children同一getter；调用方候选筛选不同，缓存并非两份独立模块树。',[P('P27'),P('P29'),P('P31')]);
E('Q26','relwin','relclose','guarded-closing-validation','DialogResult.HasValue且Value=true','仅此时校验；Group实现统计第一根直接子Holder勾选数，大于128拒绝，非递归全树上限。',[P('P24')]);
E('Q27','relwin','true','modal-return-to-caller-unknown','关联委托ShowDialog实际正常返回','方法返回nullable结果；原窗口确认/取消及其它Closing处理器顺序未运行证明。',[P('P23'),P('P24'),P('P25')],'unknown');
E('Q28','true','diff','guarded-diff-application','委托结果GetValueOrDefault为true','false/null直接返回；true时按旧快照增删，先修改字典再调SDK；finally仅释放枚举器，无整体回滚。',[P('P25')]);
E('Q29','diff','text','property-notification-to-read-unknown','命令正常通知RelationModulesStr并被实际绑定重新读取','通知代码和资源读取声明均在；具体求值/重入与SDK重读时序仍未知。',[P('P23'),P('P25'),P('P26')],'unknown');
E('Q30','text','relnative','sdk-read-and-display-effect-unknown','显示getter被读取并向SDK查询','getter先清字典，catch可能保留部分状态；重读不证明本轮关系全部生效或完成持久化。',[P('P26')],'unknown');
E('Q31','diff','relnative','native-relation-effect-unknown','执行AddDynamicParamRelation或DeleteDynamicParamRelation','托管先后顺序已证；原生效果、失败中断与文件保存并未闭合。',[P('P25')],'unknown');
E('Q32','main','procedure','host-command-activation-unknown','流程设置按钮实例和Command绑定实际激活','按钮声明和命令到处理器的直接调用已核；Click与Command顺序、BAML和实际投递未知。',[S('P09')],'unknown');
E('Q33','area','open','guarded-delegate-call','前置禁用/加载门放行，ItemDoubleClick属性委托指向声明宿主处理器','画布命令调用委托属性，资源两处声明该处理器；处理器直接OpenParams。依赖属性不是CLR事件，赋值与实际激活另列。',[S('P08'),S('P11')]);
E('Q34','procedure','open','guarded-direct-method-call','前置禁用/加载门放行','流程命令直接构造ObjectEventArgs调用同一处理器，再OpenParams；加载失败时不提示，与画布入口不同。',[S('P09')]);
E('Q35','main','direct','independent-group-entry-reachability-unknown','VmGroupDecorator独立命令实际有调用者','原件体已核，尚未定位该无参命令绑定/完整调用者，不能默认它就是画布或流程按钮。',[S('P10')],'unknown');
E('Q36','direct','password','guarded-modal-password-call','IsPasswordEnabledA=true','构造校验窗并传PasswordA；ShowDialog结果pop，后续读取IsPasswordOK。',[S('P10'),U('E68')]);
E('Q37','password','directwin','guarded-parameter-window-construction','校验窗正常返回且IsPasswordOK=true','创建通用参数窗、赋VMModule.Params并ShowDialog；不是依此证明实际校验/模态成功。',[S('P10')]);
E('Q38','direct','directwin','unguarded-password-disabled-branch','IsPasswordEnabledA=false','直接通用模态参数窗；本体不调用OpenParams或扩展查找，不凭此推其它入口的ConfigCheck也无密码门。',[S('P10')]);
E('Q39','directwin','group','group-template-activation-unknown','该独立通用窗确实物化Group节点和专用模板','原件窗口参数赋值存在；Group模板和另一公共控件副本的实际加载未验。',[S('P10'),P('P16'),P('P38'),P('P39')],'unknown');
E('Q40','gate','checkgate','guarded-virtual-property-read','角色配置门flag=true','Procedure方法以flag&&ConfigCheck短路；直接VmModuleDecorator谓词不读取ConfigCheck，不能统一成一套门。',[S('P12')]);
E('Q41','checkgate','basecheck','conditional-selected-hierarchy-resolution','接收者是所选IMVSGroup，且无更派生覆盖','原件继承/声明getter/MethodImpl证据显示Group与ModuleSet无覆盖，继承VmModule恒true；不是UI实际接收者证明。',[S('P13')]);
E('Q42','checkgate','proxycheck','conditional-proxy-target-dispatch','接收者是VmProxyModule','首次先置装载位再动态找/造目标，随后目标虚拟getter；不能用基类true代替未知动态覆盖或声称失败可重试。',[S('P14')]);
writeChain('group-configuration-chain',{schemaVersion:1,id:'group-configuration',title:'主界面到Group参数与关联：入口、候选、编辑和回流',viewBox:'0 0 1400 1980',scope:'以主界面配置入口为起点，拼接已核实的条件调用与数据流；主界面投递、模板激活、勾选路由、模态回流和native/保存效果逐处留未知。左右两路不是同一种确认或提交合同；不声称入口完整或原机操作成功。',entryCompletenessProved:false,fullBehaviorClosed:false,runtimeVerified:false,nodes,edges,invariants:['Parameter editing mutates during commands; its outer callback result is discarded.','Relation editing applies its diff only when its own nullable callback result is true.','Children aliases Modules; matching predicates differ between consumers.','Shared static evidence does not prove BAML activation, runtime scheduling, transactional success or persistence.','Cross-view relations do not increase evidence edge counts or implementation completion.']},parents);
console.log(JSON.stringify({nodes:nodes.length,relations:edges.length,unknown:edges.filter(e=>e.classification==='unknown').length,complete:false}));
