'use strict';
// Explicit execution-fact → already-evidenced data-context mapping. No names, adjacency or role aliases are used to infer calls.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert'),{root,out}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const rows=[
 ['workflow-run','current','ERU02',['CP02','CR01'],'主窗发起本次运行时的配置快照，以及旧结果视图与旧session分开的持有关系。'],
 ['workflow-run','current','ERU03',['CG01','CG09'],'SolutionExecutor文档快照与调用返回；CG事实涉及owned Group的部分只适用于该分支，不外推普通节点。'],
 ['workflow-run','current','ERU04',['TD01','CP05','CD05'],'实际GraphExecutor输入解析、有效参数scope缓存与本轮变量/记录；三种容器分别保留。'],
 ['workflow-run','current','ERU05',['TD04','CD05','CR02'],'模块finally与结果过滤、节点记录、流程导出，以及本轮失败/取消记录替换旧会话。'],
 ['tool-run','current','ETU01',['CR05'],'调试宿主、完成Task与宿主记录是独立持有者；仅按本事实明确的调试分支阅读。'],
 ['tool-run','current','ETU02',['CR05'],'ToolDebugSession的active Task、停止等待与完成结果持有，不以停止等同清缓存。'],
 ['tool-run','current','ETU03',['CP03','TD01','TD04'],'隔离节点仍交真实GraphExecutor；展示其公共输入/输出合同，不声称重新执行上游或证明每个工具算法。'],
 ['tool-run','current','ETU04',['CR05','TD08'],'调试记录发布与宿主缓存、当前结果、旧会话的持有关系。'],
 ['tool-run','current','ETU05',['CP04','CR05'],'普通配置自执行的参数副本与独立取消域；不并成ToolDebugSession的同一个实例。'],
 ['tool-run','current','ETU06',['CP05','CR05'],'普通配置执行进入Graph后的scope参数缓存与请求结束边界，分支身份保持独立。'],
 ['results','current','ERS01',['TD08','CR02'],'返回session提交到workflowSessions，容器覆盖与载荷最后读者退出分开。'],
 ['results','current','ERS03',['TD08','CR01'],'当前结果视图恢复同一NodeRunRecord；清视图不等于删除旧session。'],
 ['results','current','ERS04',['TD05','TD06','TD07'],'显示准入三分支、普通Bitmap预览与三个frame presenter分别取证，不把它们合成同一显示管线。'],
 ['stop','current','ESP01',['CR01'],'停止是取消请求；旧session与空白视图可能仍并存，不承诺停止即清旧图。'],
 ['stop','current','ESP02',['CR02'],'节点本轮取消/失败/跳过的输出与本轮session聚合；新旧插件状态另有持有者。'],
 ['stop','current','ESP03',['CR05'],'调试StopAsync等待active；完成Task、LastResult和宿主结果的退役分别核对。'],
 ['stop','current','ESP04',['CR01','CR02'],'主流程finally结束运行状态，但正常返回session与异常无session时保留行为不同。'],
 ['file-exit','current','EFE04',['CR05'],'工具窗口退出、调试调度停止与结果引用解除分开。'],
 ['file-exit','current','EFE06',['CR06','CP05','CSR03','CSR04'],'owner换代、旧scope租约排空、有效参数与失败清理资源的实际退役边界。'],
 ['file-exit','original','shell-lifecycle:L17',['OR05'],'应用退出交方案Dispose(bool)的实际SDK销毁实参；更外层显示退出及native最后读者仍分开。'],
 ['results','original','results-roi:V03',['RS01'],'ModuleResultView与显示装饰器建立的对象持有链；只是已证字段/绑定，不补原生活实例。'],
 ['results','original','results-roi:V04',['RC01','RC02','RC03','RS02'],'模块回调三仓路由/引用交换、计数等待，以及图像对象交给装饰器；这不是逐像素复制。'],
 ['results','original','results-roi:V05',['RS04'],'结果树选中与图像选择是两条分支，此处保留已证的树记录/选择路径。'],
 ['results','original','results-roi:V23',['RS02'],'流程UpdateRender与模块自执行回调的入口不同，只展示RS02分别已证的仓库到图像字段路径。']
];
module.exports=function(execution){
 const files=['parameter-data-pipeline.json','image-data-pipeline.json','nonimage-data-pipeline.json','group-data-pipeline.json'],facts=new Map(),inputs=[];
 for(const name of files){const file='docs/architecture-recovery/'+name,b=fs.readFileSync(path.join(root,file)),m=JSON.parse(b);inputs.push({path:file,sha256:sha(b)});for(const f of m.facts||[])if(rows.some(r=>r[3].includes(f.id))){if(facts.has(f.id))assert.deepStrictEqual(facts.get(f.id),f);facts.set(f.id,f);}}
 const associations=rows.map(([chainId,side,executionFactId,dataFactIds,reason])=>{const edge=execution.chains.find(c=>c.id===chainId)?.[side].find(e=>e.id===executionFactId);assert(edge,'Explicit execution context edge absent '+executionFactId);for(const id of dataFactIds){const f=facts.get(id);assert(f,'Explicit data fact absent '+id);assert.equal(f.classification,side==='original'?'source-confirmed':'independent-implementation','Data program identity '+id);}return{chainId,side,executionFactId,dataFactIds,reason,associationType:'explicit-research-context',sameInvocationEstablished:false,addsRuntimeEdge:false};});
 for(const f of facts.values())for(const r of f.refs){const b=fs.readFileSync(path.resolve(root,r.path));assert.equal(sha(b),r.sha256);assert.equal(b.toString('utf8').split(/\r?\n/).slice(r.start-1,r.end).join('\n'),r.excerpt);}
 return{schemaVersion:1,complete:false,meaning:'这里按明确登记的执行关系ID展开已有数据事实。它解释持有、复制、缓存与退出；不是新增调用箭头，不证明同一次调用、同一帧或最后一个读者已经退出。',unmappedMeaning:'此执行关系尚未登记对应的数据持有与释放正文；保留已有执行证据与未知，不按成员名称补连。',associations,facts:[...facts.values()],inputs,scope:'Original VM and current REA kept separate; SunEye reference is not imported into runtime call graphs.',originalRuntimeExecuted:false,currentRuntimeExecuted:false,implementationCredit:0};
};
function render(host,model,chainId,side,executionFactIds,make,prefix=''){
 if(!model)return;const rows=model.associations.filter(r=>r.chainId===chainId&&r.side===side&&executionFactIds.includes(r.executionFactId)),section=make('section');section.dataset.execDataContext='true';section.append(make('h4','数据层正文：谁持有、如何传递、何时退出'),make('p',model.meaning));
 const selected=new Set();for(const row of rows){section.append(make('p',row.executionFactId+' · '+row.reason));for(const id of row.dataFactIds)selected.add(id);}
 if(!selected.size)section.append(make('p',model.unmappedMeaning));
 for(const id of selected){const f=model.facts.find(v=>v.id===id),d=make('details');d.dataset.execDataFact=id;d.open=true;d.append(make('summary',id+' · '+f.from+' → '+f.to),make('p',(side==='original'?'原版VM源码确认':'REA当前独立实现')+' · '+f.classification),make('p',f.behavior));for(const [key,title]of [['representation','类型、表示与复制'],['cache','持有、缓存与失效'],['release','结束与释放边界']])d.append(make('p',title+'：'+f[key]));if(f.holder)d.append(make('p','明确持有者：'+(Array.isArray(f.holder)?f.holder.join(' / '):f.holder)));for(const r of f.refs){const proof=make('details');proof.append(make('summary',r.member||r.path),make('p',r.evidenceType||''),make('p',r.binaryIdentity||''),make('pre',r.path+':'+r.start+'–'+r.end+'\nSHA256 '+r.sha256+'\n'+r.excerpt));d.append(proof);}for(const u of f.unknowns||[])d.append(make('p','数据链待核：'+(typeof u==='string'?u:JSON.stringify(u))));const a=make('a','在数据页签查看同一事实 '+id);a.href=prefix+'#data-design/fact/'+id;d.append(a);section.append(d);}
 host.append(section);
}
module.exports.render=render;
