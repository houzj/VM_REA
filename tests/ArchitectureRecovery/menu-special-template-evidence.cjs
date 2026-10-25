'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert');
const {root,out,source}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const reportPath='docs/architecture-recovery/menu-universe-next-toolbar-decode.json',report=JSON.parse(fs.readFileSync(path.join(root,reportPath))),xamlPath=report.output.path,bytes=fs.readFileSync(path.join(root,xamlPath));
 assert.equal(sha(bytes),report.output.sha256);assert.equal(sha(fs.readFileSync(report.artifact.path)),report.artifact.sha256);assert(report.byteIdenticalToLocal);assert.equal(sha(fs.readFileSync(path.join(root,report.local.path))),report.local.sha256);
 const identity='SHA256 '+report.artifact.sha256+' / MVID '+report.mvid+' (MVID inherited from existing offline decode manifest; artifact SHA independently rechecked)';
 const ref=(anchor,count,member)=>({...source(xamlPath,member,anchor,count),evidenceType:'original-offline-decoded-BAML-resource',artifact:report.artifact.path,binaryIdentity:identity,reaEvidence:report.reaEvidence,resource:report.resource,identityManifest:reportPath});
 const findings=[],add=(id,from,to,condition,behavior,refs,unknowns)=>findings.push({id,classification:'source-confirmed',from,to,relation:'static-template-binding-and-trigger-declaration',condition,behavior,refs,unknowns});
 const commonUnknown='仅证明所选原BAML离线表示的绑定/触发器声明；未创建原版活控件、未验证Binding实际实例、WPF最终优先级或实际可点击状态，不推定完整行为等价。';
 add('MT01','customDataTemplate选择SingleRunTemplate','单次按钮ItemClickCommand与三条IsEnabled=false触发器',
 'CustomItemTemplateName==SingleRunTemplate；各独立触发器条件分别为true/0/true',
 'SingleRunTemplate的Button.Command绑定当前数据项ItemClickCommand，Style基于ToolBarButtonStyle。三条独立DataTrigger分别读CustomItemData.(IVMRun.IsRunning)==True、CustomItemData.Modules.Count==0、CustomItemData.IsOnExecuteAfterLoad==True；各自Setter都写当前Button.IsEnabled=False。模板本体没有默认TitleToolButtonDataTemplate所声明的Click EventSetter，不能复制默认按钮的焦点Click链。三门是附加禁用声明，不表示其余状态必然启用，基础样式和父级有效门仍可生效。',
 [ref('<DataTemplate x:Key="SingleRunTemplate">',19,'SingleRunTemplate: Button command and three disabling triggers'),ref('<DataTemplate x:Key="customDataTemplate">',30,'customDataTemplate finite selector/default template')],
 [commonUnknown,'GetSolution/CustomItemData缓存及SigRun真实参数准备/停止路径属于另有源码的执行链，不由模板直接证明。']);
 add('MT02','ContinousRunTemplate.ContentPresenter','Items[0]/Items[1]子项切换与容器禁用声明',
 'CustomItemTemplateName==ContinousRunTemplate；ContinuousRunEnable/IsRunning/Modules.Count/IsOnExecuteAfterLoad分别驱动声明',
 'ContentPresenter cp默认Content=Items[0]、ContentTemplate=ToolButtonDataTemplate；CustomItemData.(IVMRun.ContinuousRunEnable)==True时，TargetName=cp的Setter将Content改为Items[1]。另一个MultiDataTrigger同时要求ContinuousRunEnable=False与IsRunning=True，其UIElement.IsEnabled=False Setter未指定TargetName；还有Modules.Count==0和IsOnExecuteAfterLoad==True分别写同名IsEnabled=False的DataTemplate触发器。不能把模板切子操作等同于一个按钮始终调用同一方法，也不能把无TargetName的声明冒称已验证某活按钮的最终状态。',
 [ref('<DataTemplate x:Key="ContinousRunTemplate">',21,'ContinousRunTemplate: content selector and three disabling declarations')],
 [commonUnknown,'ToolButtonDataTemplate来自合并资源，其内部命令/样式与IsEnabled继承需要按实际引用另证；本片段不把它当TitleToolButtonDataTemplate。']);
 add('MT03','LockingTemplate.ContentPresenter','IsWorkAreaLocking驱动Items[0]/Items[1]切换',
 'CustomItemTemplateName==LockingTemplate；CustomItemData.IsWorkAreaLocking==True切第二项',
 'ContentPresenter cp初始Content绑定Items[0]，其Style.Setter令ContentTemplate=ToolButtonDataTemplate。DataTemplate的DataTrigger读取CustomItemData.IsWorkAreaLocking==True，以TargetName=cp把Content替换为Items[1]。本模板没有声明统一反转锁定值的方法；锁定/解锁是不同子项，实际GetMainData、AreaLocking/AreaUnLocking与导入菜单门仍是各自源调用。',
 [ref('<DataTemplate x:Key="LockingTemplate">',14,'LockingTemplate: initial content/template and targeted state switch')],
 [commonUnknown,'模板未定义锁状态生产者、提示偏好、取消副作用或哪个菜单被禁用，不能从图标切换推出这些业务行为。']);
 add('MT04','DownPopTemplate主Undo按钮与Items[0]弹出入口','CanUndo/LockAreaStatus/RunData三门及独立MouseDown命令',
 'CustomItemTemplateName==DownPopTemplate；主Button三条件同时匹配才有IsEnabled=True Setter',
 '主Button oo的Command=当前ItemClickCommand，基于ToolBarButtonStyle先Setter IsEnabled=False。MultiDataTrigger同时要求CustomItemData.CanUndo=True、CustomItemData.LockAreaStatus=False、RunData.(IVMRun.IsRunning)=False，然后Setter IsEnabled=True。与主Button并列的Grid rightBottom另把DataContext绑定Items[0]；其Popup.IsOpen用ElementName=rightBottom的IsMouseOver单向驱动，内部Grid.MouseDown经ParamsEventCommand执行这个子项的ItemClickCommand。三条件门写在主Button.Style，未在这个并列Popup入口声明同一门，不能擅自给两种入口画完全相同的准入路径。',
 [ref('<DataTemplate x:Key="DownPopTemplate">',66,'DownPopTemplate: main-button predicate, child data context, popup and MouseDown command')],
 [commonUnknown,'父级状态、Popup/行为组件的实际有效启用和事件路由未运行验证；缺少同一局部门不代表原机必能越过撤销条件。']);
 add('MT05','RedoTemplate按钮','CanRedo/LockAreaStatus/RunData三门同时启用',
 'CustomItemTemplateName==RedoTemplate；CanRedo=True且LockAreaStatus=False且IsRunning=False',
 'RedoTemplate的Button.Command绑定当前ItemClickCommand；Style基于ToolBarButtonStyle，先Setter IsEnabled=False。唯一MultiDataTrigger的三条Condition依次为CustomItemData.CanRedo=True、CustomItemData.LockAreaStatus=False、RunData.(IVMRun.IsRunning)=False；匹配时Setter IsEnabled=True。此声明读取schedule相关数据，但不证明GetUnRedoStack已成功得到服务或重做动作已执行/恢复成功。',
 [ref('<DataTemplate x:Key="RedoTemplate">',21,'RedoTemplate: command and conjunction predicate')],
 [commonUnknown,'基础样式、父级/动画等WPF有效值优先级未在此片段封闭；不把局部True Setter视为绝对启用保证。']);
 const previous=JSON.parse(fs.readFileSync(path.join(out,'menu-universe-next-audit.json'))),linux=previous.findings.find(f=>f.id==='MU-T01');assert(linux);for(const r of linux.refs){const b=fs.readFileSync(path.resolve(root,r.path));assert.equal(sha(b),r.sha256);assert.equal(b.toString('utf8').split(/\r?\n/).slice(r.start-1,r.end).join('\n'),r.excerpt);}
 assert(!bytes.toString('utf8').includes('Value="SaveLoadSolutionTemplate"'));
 add('MT06','MenuItemsLinux/NeutralMenuItemsLinux的Open/Save声明','SaveLoadSolutionTemplate在所选原TitleToolBar选择器中未声明',
 'Linux两资源选择时存在Open/Save自定义模板名；限所选VisionMasterMain原BAML的有限选择器',
 '两个Linux菜单资源的Lang_OpenSol/Lang_SaveSol把customItemTemplateName写为SaveLoadSolutionTemplate，主methodName分别为OpenSolutionRemote/SaveSolutionRemote，且保留Local子项OpenSolutionNative/SaveSolutionNative。所选TitleToolBar customDataTemplate先默认TitleToolButtonDataTemplate，仅对SingleRunTemplate、ContinousRunTemplate、DownPopTemplate、RedoTemplate、LockingTemplate五个值声明切换，未对SaveLoadSolutionTemplate声明分支。这里只证明资源之间这个具体差异；不能据此断言整个产品不存在该模板、原机必使用默认回退或Linux实际每个按钮已可达。',
 [...linux.refs,ref('<DataTemplate x:Key="customDataTemplate">',30,'Selected original finite template switch; no SaveLoadSolutionTemplate branch')],
 [commonUnknown,'Linux资源实际宿主/其它资源替代、远程/本地按钮真实布局与命令接收者仍未闭合。']);
 const provenance={manifest:{path:reportPath,sha256:sha(fs.readFileSync(path.join(root,reportPath)))},artifact:{...report.artifact,mvid:report.mvid,reaEvidence:report.reaEvidence,shaRechecked:true,mvidReextractedThisSlice:false},resource:report.resource,output:report.output,staticResourceDeclarationsOnly:true,originalRuntimeExecuted:false,browserRendered:false,deviceCalled:false,productModified:false};
 const model={schemaVersion:1,complete:false,findings,provenance,fullBehaviorClosed:false,entryCompletenessProved:false,implementationCredit:0,implementationProgress:'41/96 = 42.71%; no change'};
 fs.writeFileSync(path.join(out,'menu-special-template-evidence.json'),JSON.stringify(model,null,2)+'\n');return model;
};
if(require.main===module){const r=module.exports();console.log(JSON.stringify({findings:r.findings.length,references:r.findings.reduce((n,f)=>n+f.refs.length,0),originalRuntimeExecuted:false}));}
