const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert'),{root,out}=require('./evidence-view.cjs');
module.exports=function(){
 const read=n=>JSON.parse(fs.readFileSync(path.join(out,n))),audit=read('domain-data-obligations-audit.json'),model=read('domain-data-panel.json'),page=fs.readFileSync(path.join(root,'docs/framework-architecture.html'),'utf8'),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),esc=s=>String(s).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('"','&quot;');
 assert.equal(audit.domains.length,23);assert.equal(audit.priorities.length,5);assert.equal(audit.complete,false);assert.equal(model.complete,false);assert.equal(audit.verification.sourceDrift.length,0);assert.deepEqual(model.domainIds,audit.domains.map(d=>d.id));assert.equal(model.sourceReportSha256,sha(fs.readFileSync(path.join(out,model.sourceReport))));
 let obligations=0,facts=0,formal=0,unknowns=0,refs=0;
 for(const d of audit.domains){
  assert.equal(d.requirementClosed,false);assert.equal(d.dataCoverageComplete,false);assert(page.includes('data-data-domain="'+d.id+'"'));assert(page.includes(esc(d.remaining)));
  for(const o of d.obligations){obligations++;assert.equal(o.requirementClosed,false);for(const v of Object.values(o.authorized))assert(page.includes(esc(v)));}
  for(const f of [...d.factAssociations,...d.formalAssociations]){if(f.id){facts++;assert.equal(f.satisfiesWholeDomain,false);}else{formal++;assert.equal(f.requirementClosed,false);}assert(page.includes(esc(f.behavior)));for(const u of f.knownUnknowns||[]){unknowns++;assert(page.includes('具体未决：'+esc(u)));}for(const r of f.evidenceRefs){refs++;const bytes=fs.readFileSync(path.resolve(root,r.path));assert.equal(sha(bytes),r.recordedSha256||r.sha256);const excerpt=bytes.toString('utf8').split(/\r?\n/).slice(r.start-1,r.end).join('\n');assert(page.includes(esc(excerpt)));assert(page.includes(esc('证据类型 '+(r.evidenceType||'未另标'))));assert(page.includes(esc('程序集身份 '+(r.binaryIdentity||'本片段未复核'))));}}
 }
 for(const r of audit.inputs)assert.equal(sha(fs.readFileSync(path.resolve(root,r.path))),r.sha256);
 const files=['docs/framework-architecture.html',...['domain-data-obligations-audit.json','domain-data-panel.json'].map(n=>'docs/architecture-recovery/'+n),'tests/ArchitectureRecovery/domain-data-panel.cjs','tests/ArchitectureRecovery/audit-domain-data-obligations.cjs'];
 const report={domains:23,priorities:5,obligationAppearances:obligations,factAssociations:facts,formalAssociations:formal,knownUnknownAppearances:unknowns,sourceReferenceAppearances:refs,allDomainCoverageUnclosed:true,fullInlineEvidence:true,browserRendered:false,implementationCredit:0,inputHashes:files.map(p=>({path:p,sha256:sha(fs.readFileSync(path.join(root,p)))}))};fs.writeFileSync(path.join(out,'domain-data-panel-review.json'),JSON.stringify(report,null,2)+'\n');return report;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
