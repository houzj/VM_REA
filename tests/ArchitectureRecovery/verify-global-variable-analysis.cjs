const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto'),vm=require('vm');
const {root,out}=require('./evidence-view.cjs');
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const model=JSON.parse(fs.readFileSync(path.join(out,'global-variable-analysis.json'))),page=fs.readFileSync(path.join(root,'docs/framework-architecture.html'),'utf8'),script=fs.readFileSync(path.join(out,'data-design-tab.js'),'utf8');
 const start=page.indexOf('<section id="global-variable-analysis">'),end=page.indexOf('</section>',start);assert(start>=0&&end>start);
 const text=page.slice(start,end).replaceAll('&lt;','<').replaceAll('&quot;','"').replaceAll('&amp;','&');
 const programs=new Set();
 for(const r of model.refs){const b=fs.readFileSync(path.join(root,r.path));assert.equal(hash(b),r.sha256);assert.equal(b.toString('utf8').split(/\r?\n/).slice(r.start-1,r.end).join('\n'),r.excerpt);assert(text.includes(r.excerpt),'excerpt not embedded '+r.id);assert(text.includes(r.program));assert(text.includes(r.originalPath||r.path));programs.add(r.program);}
 assert.equal(programs.size,3);assert.equal(model.refs.length,17);
 for(const [title,body] of model.sections){assert(text.includes(title));assert(text.includes(body));}
 for(const u of model.unknowns)assert(text.includes(u));
 for(const row of [...model.policies,...model.proposal])for(const v of row)assert(text.includes(v));
 assert(text.includes('设计建议不是已实施功能'));assert(text.includes('尚未实现或验收'));
 assert(page.includes('href="#data-design/global-variable-analysis"'));
 // Execute the actual chapter bookmark handler against a small DOM substitute.
 const tab=JSON.parse(fs.readFileSync(path.join(out,'data-design-tab.json')));
 class E{constructor(tag='DIV'){this.tagName=tag.toUpperCase();this.dataset={};this.classList={toggle:(k,v)=>this.active=v};this.children=[];this.scrolled=0;}append(...v){this.children.push(...v);}replaceChildren(...v){this.children=v;}setAttribute(){}addEventListener(){}scrollIntoView(){this.scrolled++;}}
 const views=['overview','routes','data-design','execution','domains','method'].map(id=>Object.assign(new E(),{id})),tabs=views.map(v=>Object.assign(new E(),{dataset:{tab:v.id}})),chapter=new E('section'),host=new E();
 vm.runInNewContext(script,{document:{createElement:t=>new E(t),getElementById:id=>id==='global-variable-analysis'?chapter:id==='data-step-detail'?host:null,querySelectorAll:s=>s==='.tabs button'?tabs:s==='.view'?views:[],querySelector:()=>null},location:{hash:'#data-design/global-variable-analysis'},history:{replaceState(){}},addEventListener(){}},{timeout:20000});
 assert(views.find(v=>v.id==='data-design').active);assert.equal(chapter.scrolled,1);
 const review={passed:true,sourceReferences:model.refs.length,programs:[...programs],chapterBookmarkExecuted:true,staticDomSubstitute:true,browserRendered:false,productCodeChanged:false,externalSnapshotFiles:model.externalSnapshot.files.length,externalDriftAdvisory:model.externalDriftAdvisory,implementationProgress:model.implementationProgress};
 fs.writeFileSync(path.join(out,'global-variable-analysis-review.json'),JSON.stringify(review,null,2)+'\n');return review;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
