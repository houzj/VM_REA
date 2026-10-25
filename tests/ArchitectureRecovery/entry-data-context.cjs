// Analyst navigation associations, never inferred original call edges.
module.exports=function(data){
 const image=require('./image-data-pipeline.cjs')(data).model,parameters=require('./parameter-data-pipeline.cjs')().model,nonimage=require('./nonimage-data-pipeline.cjs')().model,group=require('./group-data-pipeline.cjs')().model,configuration=require('./configuration-data-pipeline.cjs')().model,baseFacts=[...image.facts,...parameters.facts],facts=[...baseFacts,...nonimage.facts,...group.facts,...configuration.facts];
 // Explicit research associations: never infer OP/PST/HP ownership from names,
 // result-display placement, or each registered tool's presence in the toolbox.
 const configurationEntryFacts={
  save:['OP01','OP02','OP05','OP06','PST01','PST02','PST03','PST04','PST05'],
  undo:['OP04','PST05','HP01','HP02','HP04'],
  history:['OP04','PST05','HP01','HP02','HP03','HP04'],
  solution:['OP01','OP03','OP05','PST04','PST05','HP01','HP03','HP04'],
  close:['OP02','OP03','OP04','PST05','HP02','HP03','HP04'],
  data:configuration.facts.map(f=>f.id)
 };
 const configurationIds=new Set(configuration.facts.map(f=>f.id));
 for(const list of Object.values(configurationEntryFacts))for(const id of list)if(!configurationIds.has(id))throw Error('Configuration entry fact missing '+id);
 const extras={variables:['VD01','VD02','VD03','VD05','VD06','VD07','VD08','VN01','VN02','VN03','VN04','VN05','VS01','VS02','VS03','VS04','VZ01','VZ02','VZ03','VZ04','VZ05','VZ06','VZ07','CD01','CD02','CD03','CD04','CD05','CD06','CD07','VX01','VX02','VX03','VX04','VX05','VX06','VP01','VP02','VP03','VP04','VP05','VP06','VB01','VB02','VB03','VB04'],links:['VD04','VD08','CD02','CD03','CD04','CD05','CD06'],node:['VD04','VD08','CD02','CD03','CD04','CD05','CD06'],group:['VD04','VD08','CD02','CD04','CD05','CD06','CD08'],result:['VD04','CD01','CD02','CD04','CD05','CD08'],status:['VD04','CD01','CD02','CD04','CD05','CD08'],data:nonimage.facts.map(f=>f.id)};
 extras.group.push(...group.facts.map(f=>f.id));extras.data.push(...group.facts.map(f=>f.id));
 for(const id of ['run','continuous','tabonce','tabcontinuous'])extras[id]=['VD04','VD08','CD01','CD02','CD03','CD04','CD05','CD06','CD08'];
 const ids=predicate=>baseFacts.filter(predicate).map(f=>f.id),prefix=(f,p)=>p.some(p=>f.id.startsWith(p)),entryFacts={};
 for(const id of ['startup','solution','save','run','continuous','stop','variables','camera','communication','catalog','help','tabs','node','links','undo','group','tabonce','tabcontinuous','viewport','preview','result','close','status','history','data']){
  let select;
  if(id==='data')select=()=>true;
  else if(['solution','node','variables','links','group','catalog'].includes(id))select=f=>prefix(f,['CP','SP','PS']);
  else if(id==='save')select=f=>['CP06','SP05'].includes(f.id)||prefix(f,['PS','IS10']);
  else if(['run','continuous','tabonce','tabcontinuous','tabs','startup'].includes(id))select=f=>prefix(f,['CP','PB','RQ','MF','ON','TD','SP','PS']);
  else if(['preview','result','status'].includes(id))select=f=>f.stages?.some(s=>['results','display'].includes(s))&&!prefix(f,['SP']);
  else if(['stop','close','undo','history'].includes(id))select=f=>f.stages?.includes('retirement')||['CP04','CP05'].includes(f.id);
  else if(id==='camera')select=f=>prefix(f,['IS'])&&f.modes?.some(m=>['sdk','camera'].includes(m));
  else select=()=>false;
  entryFacts[id]=[...new Set([...ids(select),...(extras[id]||[]),...(configurationEntryFacts[id]||[])])];
 }
 return{facts,entryFacts,configurationEntryFacts,configurationMeaning:'配置/撤销/保存证据只按save、undo、history、solution、close、data显式选入；区域可从其入口聚合研究上下文，不构成区域控件直接调用或每工具专属事实。',groupCapabilities:group.capabilities,groupCapabilityEntries:['group','data'],meaning:'按界面问题关联的研究上下文；不证明本控件直接调用所有成员，不把通用框架证据归作每个工具的专属实现。',browserRendered:false};
};
module.exports.render=function(host,context,ids,hrefPrefix){
 const make=(tag,text)=>{const e=document.createElement(tag);if(text!==undefined)e.textContent=text;return e;};
 if(!ids?.length)return;
 const section=make('section');section.append(make('h3','此入口相关的最新数据证据'),make('p',context.meaning));
 for(const id of ids){const f=context.facts.find(f=>f.id===id);if(!f)continue;const d=make('details');d.dataset.evidenceId=f.id;d.append(make('summary',f.id+' · '+f.from+' → '+f.to),make('p',f.classification),make('p',f.behavior));
  for(const [k,label]of [['representation','类型 / 持有 / 复制'],['cache','缓存 / 失效'],['release','退役 / 释放']])d.append(make('p',label+'：'+f[k]));
  for(const r of f.refs){const proof=make('details');proof.append(make('summary',r.member||r.path),make('p',r.binaryIdentity||r.evidenceType||''),make('pre',r.path+':'+r.start+'–'+r.end+'\nSHA256 '+r.sha256+'\n'+r.excerpt));d.append(proof);}
  for(const u of f.unknowns)d.append(make('p','待核：'+u));const a=make('a','在数据页签定位并展开 '+f.id);a.href=hrefPrefix+f.id;d.append(a);section.append(d);
 }host.append(section);
};
