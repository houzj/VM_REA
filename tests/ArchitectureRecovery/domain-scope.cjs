const fs=require('fs'),path=require('path'),crypto=require('crypto'),{root}=require('./evidence-view.cjs');
module.exports=function(){
 const planPath='docs/framework-reverse-plan.json',bytes=fs.readFileSync(path.join(root,planPath)),plan=JSON.parse(bytes),baseline=JSON.parse(fs.readFileSync(path.join(root,'docs/architecture-recovery/domain-package-baseline.json'))),ids=Array.from({length:23},(_,i)=>'F-'+String(i+1).padStart(2,'0'));
 if(plan.tasks.length!==32||new Set(plan.tasks.map(t=>t.id)).size!==32||baseline.domains.length!==23||new Set(baseline.domains.map(d=>d.id)).size!==23)throw Error('Domain/task authorized scope changed; review before regeneration');
 for(const task of plan.tasks)if(!/^[ABCD]0[1-8]$/.test(task.id)||!task.domains.length||new Set(task.domains).size!==task.domains.length||task.domains.some(id=>!ids.includes(id))||!task.entry||!task.question||!task.done)throw Error('Invalid authorized task requirements '+task.id);
 const domains=ids.map(id=>{const prior=baseline.domains.find(d=>d.id===id);if(!prior)throw Error('Missing domain label '+id);return {id,name:prior.name,packages:plan.tasks.filter(t=>t.domains.includes(id)).map(t=>t.id)};});
 return {planPath,planSha256:crypto.createHash('sha256').update(bytes).digest('hex'),tasks:plan.tasks,domains,baseline,linkCount:domains.reduce((n,d)=>n+d.packages.length,0)};
};
