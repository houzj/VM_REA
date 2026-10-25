const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root,out}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
// Supplemental selections overlap registered views. Their arrows are research
// navigation, never evidence that two controls are the same runtime instance.
module.exports=function(){
 const selections=[
  ['algorithm-binary-comparison-audit','已实现算法对比：二值化有限基线与模式缺口',['toolbox','results'],['plugins','rendering','effects','boundary'],'精确工具身份 → 原参数/旧像素基线与当前真实源码隔离回放；仅模式1和两夹具有限匹配，模式缺口与ROI/native边界独立保留。'],
  ['solution-save-native-boundary-audit','方案保存：托管入口、原生导出与ABI边界',['menu','toolbar'],['effects','boundary'],'显式方案保存 → SDK托管包装 → 原生导出与直接调用边界；实际落盘、格式和未恢复callee仍未知。'],
  ['variable-file-entry-audit','变量文件：主菜单、子窗口与原生导入导出',['menu'],['regions','framework','effects','boundary'],'主菜单打开全局变量窗口 → 变量窗文件按钮 → UTF8路径命令/IO重建；gvar内容编码及保护仍未知，当前JSON另列。'],
  ['history-results-entry-audit','结果历史：文本记录、当前选行与回放缺口',['history-help','results'],['regions','framework','rendering','effects'],'历史查询/文本投影 → 计数、清空和txt导出；当前结果表选行另走重画，历史图像/模块帧回放仍待证。'],
  ['rendering-native-loader-audit','原生呈现依赖：搜索规则、架构与候选导出',['results'],['rendering','boundary','shell'],'主入口cwd → 托管初始化/平台与路径搜索 → 文件候选及静态导出；实际进程装载和原生绘字仍未知。'],
  ['menu-authorization-consumers-audit','授权消费者：角色谓词与WPF／输入门',['menu','toolbar','toolbox','workflow'],['shell','regions','framework'],'CodeManager角色字段/通知 → 具体WPF容器、配置编辑与输入门；回调赋值者及逐菜单授权矩阵未证明。'],
  ['workflow-rename-history-audit','改名历史：名称路径身份、录制与重放',['workflow','menu','toolbar'],['framework','effects'],'三字符串快照 → 原名称路径再查找 → 历史栈/委托重放；失败不据此保证回滚，稳定身份和嵌套Group未知保留。'],
  ['export-text-rendering-audit','导出文字：测量、换行与原生字体边界',['results'],['rendering','effects','boundary'],'IMultiText/IText → 整数尺寸与分组行高 → 五参数绘字重载 → OpenCvSharp包装/PInvoke；屏幕绘字另有字体与比例规则。'],
  ['menu-state-producers-audit','菜单状态：锁、语言、运行模板与角色按钮',['menu','toolbar'],['shell','regions','framework'],'字段默认/通知 → 所选状态生产者 → 命令项与WPF模板；各菜单授权、活实例及完整释放继续未知。'],
  ['workflow-title-commit-audit','标题提交：编辑、名称写入与独立保存',['workflow'],['framework','rendering','effects'],'Explicit名称绑定 → HeadReNameTextBox提交 → 装饰器/模块或流程SDK改名；保存另有消费者，不把编辑退出当写盘。'],
  ['workflow-rendering-entry-audit','流程画布：节点、标题与连线',['workflow'],['rendering','framework'],'DiagramItem模板 / HeadReNameTextBox → 节点；Connection Shape → 连线视觉层。'],
  ['image-export-entry-audit','结果图像：原图与叠加图导出',['results'],['rendering','effects'],'图像控件菜单/命令 → 保存参数 → 编码与文件写入；当前Runner另列。'],
  ['menu-entry-materialization-audit','菜单与工具栏：声明到实际接收者',['menu','toolbar'],['shell','regions'],'资源声明 / 集合供应 / 模板与生成挂接 → 命令接收者；动态实例与可达性另验。']
 ];
 const domainContext={
  'workflow-rendering-entry-audit':['F-07','F-08','F-09'],
  'image-export-entry-audit':['F-20'],
  'menu-entry-materialization-audit':['F-03','F-04'],
  'workflow-title-commit-audit':['F-05','F-07','F-10','F-19'],
  'menu-state-producers-audit':['F-04'],
  'export-text-rendering-audit':['F-20','F-23'],
  'workflow-rename-history-audit':['F-05','F-07','F-10','F-21'],
  'menu-authorization-consumers-audit':['F-04'],
  'rendering-native-loader-audit':['F-20','F-23'],
  'history-results-entry-audit':['F-20','F-21'],
  'variable-file-entry-audit':['F-04','F-17','F-23'],
  'solution-save-native-boundary-audit':['F-19','F-23'],
  'algorithm-binary-comparison-audit':['F-20','F-22','F-23']
 };
 const scope=require('./domain-scope.cjs')();
 const branches=selections.map(([slug,title,regions,layers,meaning])=>{
  const file=slug+'.json',bytes=fs.readFileSync(path.join(out,file)),audit=JSON.parse(bytes);
  if(audit.complete!==false||audit.runtimeExecuted!==false||!Array.isArray(audit.findings))throw Error('Unsupported supplemental core scope '+slug);
  if(audit.currentImplementationExecuted===true&&(audit.originalRuntimeExecuted!==false||audit.vendorSdkExecuted!==false||audit.productionDllExecuted!==false||audit.currentExecutionKind!=='isolated-current-production-source'))throw Error('Unreviewed execution scope '+slug);
  const ids=new Set();let references=0;
  const findings=audit.findings.map(e=>{
   if(!e.id||ids.has(e.id)||!['source-confirmed','independent-implementation','unknown'].includes(e.classification)||!Array.isArray(e.refs)||e.classification!=='unknown'&&!e.refs.length)throw Error('Invalid core supplemental relation '+slug+':'+e.id);
   ids.add(e.id);
   for(const r of e.refs){const b=fs.readFileSync(path.resolve(root,r.path));if(!r.start||!r.end||sha(b)!==r.sha256||b.toString('utf8').split(/\r?\n/).slice(r.start-1,r.end).join('\n')!==r.excerpt)throw Error('Core supplemental reference changed '+slug+':'+e.id);references++;}
   if(!fs.readFileSync(path.join(out,slug+'.html'),'utf8').includes('id="'+e.id+'"'))throw Error('Supplemental finding anchor missing '+slug+':'+e.id);
   return {...e,operationComplete:false,stages:{classification:'independent-implementation',sourceRecovery:e.classification==='source-confirmed'?'finite-source-slice; retain each reference evidence type and identity limits':e.classification==='independent-implementation'?'current-source-slice; original behavior not inferred':'unresolved',runtimeCorroboration:'not-established-by-this-static-audit',currentExecution:audit.currentImplementationExecuted===true?'isolated-current-production-source; audit scope only, not every row or full algorithm proof':'not-executed-by-this-audit',currentImplementation:e.classification==='independent-implementation'?'independent-current-source-slice; not VM parity':'not-inferred-from-this-row',hostAcceptance:'separate; not established or counted by this audit',wholeFeatureClosure:'not-inferred'}};
  });
  const domains=domainContext[slug];if(!domains?.length||domains.some(id=>!scope.domains.some(d=>d.id===id)))throw Error('Invalid supplemental domain context '+slug);
  return {domains,domainAssociation:'independent-navigation-context-only; not each finding coverage or a closed task',id:slug,title,regions,layers,meaning,path:slug+'.html',input:{path:'docs/architecture-recovery/'+file,sha256:sha(bytes)},findings,references,executionScope:{newOriginalRuntimeExecuted:false,currentImplementationExecuted:audit.currentImplementationExecuted===true,kind:audit.currentExecutionKind??'not-executed'},complete:false,runtimeVerified:false,entryCompletenessProved:false};
 });
 const extensions={
  'solution-save-native-boundary-audit':[['workflow-title-commit-audit','WTU01']],
  'workflow-rename-history-audit':[['workflow-title-commit-audit','WTU01']],
  'menu-authorization-consumers-audit':[['menu-state-producers-audit','MS-U01']],
  'rendering-native-loader-audit':[['export-text-rendering-audit','U01']]
 };
 for(const b of branches)b.extendsUnknowns=(extensions[b.id]??[]).map(([audit,id])=>{const parent=branches.find(p=>p.id===audit)?.findings.find(e=>e.id===id);if(parent?.classification!=='unknown')throw Error('Missing open parent for evidence extension '+b.id);return {audit,id,classification:'independent-implementation',relation:'partial-evidence-extension',fullBehaviorClosed:false,meaning:'Selected later evidence addresses part of this open research boundary; the original unknown is retained.'};});
 const resourcePath='docs/architecture-recovery/resource-scope-reconciliation.json',resourceBytes=fs.readFileSync(path.join(root,resourcePath)),resource=JSON.parse(resourceBytes);
 if(resource.complete!==false||resource.verification.runtimeExecuted!==false||resource.verification.entryCompletenessProved!==false)throw Error('Unsupported resource scope closure');
 const resourceScope={path:'resource-scope-reconciliation.html',input:{path:resourcePath,sha256:sha(resourceBytes)},counts:resource.counts,complete:false,entryCompletenessProved:false};
 return {planInput:{path:scope.planPath,sha256:scope.planSha256},resourceScope,classification:'independent-implementation',meaning:'Finite supplemental selections grouped by UI region and research layer. Counts overlap registered views and must not be added to them.',scopesAreAdditive:false,closedEntryChains:0,branches};
};
