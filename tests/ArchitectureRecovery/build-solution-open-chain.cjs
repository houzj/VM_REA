const fs=require('fs'),path=require('path'),{out}=require('./evidence-view.cjs'),{writeChain}=require('./explicit-chain.cjs');
require('./verify-solution-lifecycle-cil.cjs').verify();
const parents=['solution-lifecycle','entry-routing-security','shell-resources'].flatMap(view=>JSON.parse(fs.readFileSync(path.join(out,view+'.edges.json'))).edges.map(e=>({view,...e})));
const nodes=[
 ['root','主界面','实际事件投递仍未验证',600,60],['menu','打开方案菜单','methodName=OpenSolution',600,180],['handler','OpenSolution包装','创建/保存/加载忙状态门',600,300],['busy','忙状态直接返回','无选择器 / 无加载任务',150,300],
 ['choose','LoadSolution选择路径','提供路径 / 普通 / 远程',600,420],['cancel','选择器取消','返回true；未启动加载',1050,420],['format','CheckSolution格式门','nValidFormat / Check异常',600,540],['fail','格式/检查失败','所选分支返回false',150,540],
 ['pwd','密码参数门','nHasPwd / solPassWord',600,660],['bad','显式密码错误','throw VmException',150,660],['dialog','Decryption.ShowDialog','nullable bool返回',600,780],['result','密码窗结果','true或非true',600,900],
 ['task','构造加载Task','构造不等于启动',600,1020],['prompts','切换 / 保存门','Modules / 来源 / 提示条件',600,1140],['reject','拒绝切换 / 取消保存','返回true；跳过配置调用',150,1140],['save','交SaveSolution(task)','本层忽略保存返回bool',150,1260],
 ['start','Task.Start','任务已交付 ≠ 已加载',600,1260],['worker','加载Task后台体','版本/历史/迁移等条件',600,1380],['load','VmSolution.Load调用','条件满足才执行',600,1500],['native','native完成仍未知','实际回调 / 实例状态未证',600,1620],
 ['config','自动加载配置门','调用点在任务门外',1050,1260],['write','SolPath / SolPassword','满足偏好门才SetAutoLoadSol',1050,1380],['returned','正常返回true','不作为加载完成凭据',1050,1500]
].map(([id,label,detail,x,y])=>({id,label,detail,x,y}));
const edges=[],L=id=>['solution-lifecycle',id],R=id=>['entry-routing-security',id];
function E(id,from,to,relation,condition,behavior,keys,classification='source-confirmed'){
 const selected=keys.map(([view,id])=>{const p=parents.find(e=>e.view===view&&e.id===id);if(!p)throw Error('Missing open-chain parent');return p;});
 const refs=[...new Map(selected.flatMap(p=>p.refs).map(r=>[r.path+':'+r.start+':'+r.end,r])).values()];
 edges.push({id,from,to,relation,condition,behavior,classification,parents:selected.map(p=>({view:p.view,id:p.id,classification:p.classification})),refs,runtimeExecuted:false,fullBehaviorClosed:false});
}
E('A01','root','menu','host-activation-unknown','主界面实际实例/资源激活','静态主窗和菜单资源已定位，原机BAML、实际对象和事件投递未验证。',[R('R03')],'unknown');
E('A02','menu','handler','binding-declaration','IsEnabled且MethodName非null，反射解析成功','XML声明OpenSolution，通过通用命令反射到方法；声明/体不等于运行点击。',[R('R06'),L('O01')]);
E('A03','handler','busy','guarded-return','IsCreating或IsSaving或IsLoading','包装不继续进入选择器。',[L('O01')]);
E('A04','handler','choose','guarded-call','三个忙标志均false','调用StopRun后LoadSolution。StopRun设连续运行标志false，不把它画成已等待当前轮完成。',[L('O01')]);
E('A05','choose','cancel','guarded-return','普通选择器取消；或远程结果非true且正常结束','返回true不代表已选择或加载文件。',[L('O02')]);
E('A06','choose','format','guarded-call','filename已提供，或选择器返回true','本地和DirectoryBrowser远程选择器是不同类型；选择目录可先写偏好。',[L('O02')]);
E('A07','format','fail','guarded-return','CheckSolution抛所捕获VmException，或nValidFormat!=1且错误处理正常返回','本地选定失败路径返回false；不把其它异常统一归为false。',[L('O03')]);
E('A08','format','pwd','guarded-next','检查正常返回且nValidFormat==1','继续密码分支。',[L('O03')]);
E('A09','pwd','dialog','guarded-call','nHasPwd==1且solPassWord==null','创建密码窗并传SolutionPath，再ShowDialog。',[L('O03')]);
E('A10','pwd','task','guarded-next','nHasPwd!=1，或显式密码检查正确；相关调用正常返回','未设置跳过标志的路径进入Task构造；显式密码被传给后续载荷。',[L('O03')]);
E('A11','pwd','bad','guarded-throw','显式密码CheckPassword的nIsCorrect==0','原件抛VmException(-536868864)，位于本地CheckSolution的catch范围之外。',[L('O03')]);
E('A12','dialog','result','modal-result-read','ShowDialog正常返回','读取nullable结果，随后仍读取AccessAdministatorCode；不声称密码框字段值已被原机验证。',[L('O03')]);
E('A13','result','task','guarded-next','nullable结果恰为true','不设置local16跳过标志，正常进入任务门。',[L('O03')]);
E('A14','result','config','guarded-normal-return-cil-path','结果false或null，所遇字段/调用正常返回','原CIL IL_0271设local16，IL_02d4跳IL_03e9，绕过Task构造和Start，进入IL_0439配置调用。不是认证或写盘成功证明。',[L('O05')]);
E('A15','task','prompts','ordered-next','Task构造正常返回','先清剪贴板，再检查已有Modules、AppArgs切换询问和保存提示条件。',[L('O04')]);
E('A16','prompts','reject','guarded-return','AppArgs允许切换询问被拒绝，或保存提示Cancel/default','这些分支返回true，且因显式return跳过尾部自动加载配置调用。',[L('O04')]);
E('A17','prompts','save','guarded-call','已有Modules且保存提示门满足、用户Yes','把尚未启动的Task交SaveSolution(task)，忽略本层返回bool并返回true；尾部配置调用被跳过。',[L('O04')]);
E('A18','prompts','start','guarded-call','切换门未拒绝，且无保存提示或用户No等放行路径','调用Task.Start。任务构造、保存交付和Task启动分别建节点。',[L('O04')]);
E('A19','save','start','save-continuation-unknown','所选保存/另存路径实际启动afterSaveTask时','存在afterSaveTask.Start路径，但保存取消、导出/回调结果与调度未在此图闭合，不画成Yes必然加载。',[L('N02'),L('O04')],'unknown');
E('A20','start','config','ordered-caller-next','Task.Start正常返回，未走前面的显式return','调用者继续到尾部配置门；此关系不等待后台加载成功。',[L('O05')]);
E('A21','config','write','guarded-state-write','GetAutoLoadSolState()==0且IsAutoLoadLast','辅助体写SolPath/SolPassword，再调用SetAutoLoadSol。',[L('O05')]);
E('A22','config','returned','guarded-return','辅助体偏好门不满足且正常返回','不进入所选字段写入支路，调用者仍返回true。',[L('O05')]);
E('A23','write','returned','normal-return-next','字段赋值和SetAutoLoadSol正常返回','调用者返回true；不把存储调用正常返回等同新方案载入。',[L('O05')]);
E('A24','start','worker','task-execution-unknown','运行时实际调度并执行Task委托','源码有加载委托，但此图没有原机调度/线程/完成观测。',[L('O04')],'unknown');
E('A25','worker','load','guarded-call','版本/迁移等先行条件放行，且!VmSolution.Instance.IsLoading','源码加载委托在此条件下调用VmSolution.Load(filename,passWord)。内部其它分支见原加载/兼容页；本图不穷尽worker分支。',[['shell-resources','V03']]);
E('A26','load','native','native-effect-unknown','实际进入原生加载及回调生命周期','托管调用不证明加载完成、实例替换成功或全部UI已回流。',[L('U01')],'unknown');
writeChain('solution-open-chain',{schemaVersion:1,id:'solution-open-branches',title:'主界面打开方案：分支与任务/配置回流',scope:'原版打开方案的有限静态分支。密码窗非true路径、取消、保存交付、Task启动与配置调用分别追踪；不穷尽加载委托/原生内部路径。',entryCompletenessProved:false,fullBehaviorClosed:false,runtimeVerified:false,nodes,edges,invariants:['Return true does not prove load success.','Caller configuration continuation and Task execution are distinct relations, not a completion dependency.','Modal nontrue path is a normal-return CIL witness, not authentication bypass or proven persistence.','Graph relation counts are separate from evidence-view edges and implementation progress.']},parents);
console.log(JSON.stringify({nodes:nodes.length,relations:edges.length,unknown:edges.filter(e=>e.classification==='unknown').length,complete:false}));
