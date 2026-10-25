'use strict';
// Incremental scene extraction from the already captured document; no live source scan.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');
const site=path.resolve(__dirname,'../../../docs/架构逆向与实现对照文档');
const read=p=>fs.readFileSync(path.join(site,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const manifest=JSON.parse(read('清单/构建清单.json')),proof=JSON.parse(read('清单/证据保留.json'));
const selection=process.argv.find(x=>x.startsWith('--topic='))?.slice(8);
const recipes={image:['image-pipeline','data-pipeline-stage'],group:['group-data-pipeline','data-group-capability'],retirement:['retirement-operation-map','data-retirement-operation'],global:['global-variable-analysis','(?:data-global-execution-scenario|data-global-execution-fact)'],domain:['domain-data-obligations','data-data-domain'],nonimage:['nonimage-pipeline','data-exit-family']};
assert(recipes[selection],'Choose --topic=image|group|retirement');
const [base,attribute]=recipes[selection],parent='数据/'+base+'.html',snapshot='历史/分页输入-'+base+'.source';
function write(p,s){const b=Buffer.from(s);fs.writeFileSync(path.join(site,p),b);const row={path:p,sha256:sha(b),bytes:b.length},i=manifest.outputs.findIndex(x=>x.path===p);if(i<0)manifest.outputs.push(row);else manifest.outputs[i]=row;}
const source=fs.existsSync(path.join(site,snapshot))?read(snapshot).toString():read(parent).toString();
write(snapshot,source);
let result=source;const extracted=[];
const pattern=new RegExp('<details\\b[^>]*\\b'+attribute+'="([^"]+)"[^>]*>','g');
for(const start of source.matchAll(pattern)){
 let depth=0,end;for(const tag of source.slice(start.index).matchAll(/<\/?details\b[^>]*>/g)){depth+=tag[0].startsWith('</')?-1:1;if(!depth){end=start.index+tag.index+tag[0].length;break;}}
 assert(end,'Unclosed scene');const body=source.slice(start.index,end),id=start[1],title=/<summary[^>]*>([\s\S]*?)<\/summary>/.exec(body)[1].replace(/<[^>]*>/g,''),dest='数据/'+base+'--'+id+'.html';
 const content='<p class="site-notice">按原记录中的具体场景独立分页；原版、当前、自研、拟议与未知分类沿用正文，不新增行为推断。</p><p><a href="'+base+'.html">返回专题总览</a></p><div class="source-content">'+body+'</div>';
 let page=source.replace(/<main id="main-content" class="site-main">[\s\S]*?<\/main>/,'<main id="main-content" class="site-main">'+content+'</main>');
 page=page.replace(/<title>[\s\S]*?<\/title>/,'<title>'+title+' · 场景分析</title>');
 write(dest,page);if(!manifest.pages.some(p=>p.path===dest))manifest.pages.push({path:dest,title,kind:'scene'});
 const anchors=[...new Set([...body.matchAll(/(?:\bid|\bdata-[\w-]*fact)="([^"]+)"/g)].map(m=>m[1]))];
 result=result.replace(body,'<article class="site-card">'+anchors.map(id=>'<span id="'+id+'" data-moved-target="'+path.basename(dest)+'#'+id+'"></span>').join('')+'<h3><a href="'+path.basename(dest)+'">'+title+' →</a></h3><p>完整分支、类型成员、未知及证据已按此场景独立分页。</p></article>');
 extracted.push({id,title,path:dest,payloadSha256:sha(Buffer.from(body)),originalBytes:Buffer.byteLength(body),anchors});
}
assert(extracted.length,'No scene found');write(parent,result);
const routes=JSON.parse(read('清单/旧路由映射.json'));let relocated=0;
for(const [key,value]of Object.entries(routes)){for(const scene of extracted){const anchor=value.startsWith(parent+'#')?value.slice(parent.length+1):null;if(anchor&&scene.anchors.includes(anchor)){routes[key]=scene.path+'#'+anchor;relocated++;break;}}}
write('清单/旧路由映射.json',JSON.stringify(routes,null,2)+'\n');write('公共资源/routes.js','window.DOC_ROUTES='+JSON.stringify(routes).replaceAll('<','\\u003c')+';\n');
const changed=new Set([parent,...extracted.map(x=>x.path)]);
proof.occurrences=proof.occurrences.filter(o=>!changed.has(o.page));
for(const e of proof.evidence)e.backlinks=e.backlinks.filter(p=>!changed.has(p));
for(const p of changed){for(const m of read(p).toString().matchAll(/data-evidence-key="([^"]+)"/g)){const e=proof.evidence.find(e=>e.key===m[1]);assert(e);proof.occurrences.push({page:p,key:e.key,kind:'scene-preserved'});if(!e.backlinks.includes(p))e.backlinks.push(p);}}
// Evidence payload is untouched; refresh its human return links for newly extracted scenes.
for(const e of proof.evidence.filter(e=>e.backlinks.some(p=>changed.has(p)))){let html=read(e.path).toString();const links=e.backlinks.map(p=>'<li><a href="../'+p+'">'+p+'</a></li>').join('');html=html.replace(/(<h2>返回引用位置<\/h2>)[\s\S]*?(<\/main>)/,'$1<ul class="site-list">'+links+'</ul>$2');write(e.path,html);}
write('清单/证据保留.json',JSON.stringify(proof,null,2)+'\n');
write('清单/场景分页-'+selection+'.json',JSON.stringify({topic:selection,parent,snapshot,sourceSha256:sha(Buffer.from(source)),scenes:extracted,sourceEvidenceRescanned:false,verification:'Exact details payload copied; browser presentation not verified'},null,2)+'\n');
manifest.counts.pages=manifest.pages.length;manifest.counts.evidenceOccurrences=proof.occurrences.length;
manifest.incrementalUpdates=[...(manifest.incrementalUpdates||[]),{kind:'topic-scenes',topic:selection,files:extracted.map(x=>x.path),sourceEvidenceRescanned:false,implementationCredit:0}];
fs.writeFileSync(path.join(site,'清单/构建清单.json'),JSON.stringify(manifest,null,2)+'\n');
console.log(JSON.stringify({topic:selection,scenes:extracted.length,parentBytes:Buffer.byteLength(result),sourceBytes:Buffer.byteLength(source),sourceEvidenceRescanned:false}));
