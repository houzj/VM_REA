'use strict';
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');
const site=path.resolve(__dirname,'../../../docs/架构逆向与实现对照文档'),read=p=>fs.readFileSync(path.join(site,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),esc=s=>String(s).replaceAll('&','&amp;').replaceAll('<','&lt;').replaceAll('"','&quot;');
const manifest=JSON.parse(read('清单/构建清单.json')),routes=JSON.parse(read('清单/旧路由映射.json')),audit=JSON.parse(read('清单/锚点定位检查.json')),created=new Map(),unknown=[];
if(!audit.items.length){console.log(JSON.stringify({changed:false,reason:'No unresolved targets; existing repair evidence retained.'}));process.exit(0);}
function write(p,s){const b=Buffer.from(s);fs.writeFileSync(path.join(site,p),b);const row={path:p,sha256:sha(b),bytes:b.length},i=manifest.outputs.findIndex(x=>x.path===p);if(i<0)manifest.outputs.push(row);else manifest.outputs[i]=row;}
const template=read('证据/'+JSON.parse(read('清单/证据保留.json')).evidence[0].key+'.html').toString();
function shell(file,title,body){let s=template.replace(/<main id="main-content" class="site-main">[\s\S]*?<\/main>/,'<main id="main-content" class="site-main">'+body+'</main>').replace(/<title>[\s\S]*?<\/title>/,'<title>'+esc(title)+'</title>');return s.replace(/<h1>[\s\S]*?<\/h1>/,'<h1>'+esc(title)+'</h1>');}
let fixed=0,edgeRoutes=0;
for(const row of audit.items){const [file,anchor]=row.target.split('#');
 if(file==='数据/domain-data-obligations.html'&&/^F-\d+$/.test(anchor)){let html=read(file).toString();const tag='data-data-domain="'+anchor+'"';assert(html.includes(tag));if(!html.includes('id="'+anchor+'"'))html=html.replace(tag,tag+' id="'+anchor+'"');write(file,html);fixed++;continue;}
 if(['routeGrid','execution-map-root'].includes(anchor)){let html=read(file).toString();if(!html.includes('id="'+anchor+'"'))html=html.replace('<main id="main-content" class="site-main">','$&<span id="'+anchor+'"></span>');write(file,html);fixed++;continue;}
 const asset='公共资源/原页/architecture-recovery/'+path.basename(file,'.html')+'.edges.json',assetFile=path.join(site,asset);let edge;
 if(anchor?.startsWith('edge-')&&fs.existsSync(assetFile)){const m=JSON.parse(read(asset));edge=m.edges?.find(e=>'edge-'+e.id===anchor);}
 if(edge){const key=sha(Buffer.from(asset+'#'+edge.id)).slice(0,24),dest='证据/调用边-'+key+'.html';
  if(!created.has(dest)){const payload=JSON.stringify(edge,null,2),title=(edge.member||edge.from||edge.id)+' · 调用边证据';
   const body='<section id="'+esc(anchor)+'"><p class="site-notice">事实级别：'+esc(edge.classification||'unknown')+'。本页来自已捕获调用边模型；数组顺序不证明相邻边是同一调用、对象或线程。未进行原机运行验证。</p><h2>'+esc(edge.from||edge.id)+' → '+esc(edge.to||'未记录目标')+'</h2><p>'+esc(edge.behavior||'')+'</p><p><a href="../'+file+'#'+esc(anchor)+'">返回既有关系专题（交互未验）</a> · <a href="../'+asset+'">完整原模型</a></p><p>来源 '+esc(asset)+' · SHA256 '+sha(read(asset))+'</p><h3>完整条目：类型、成员、来源、分支与未知</h3><pre data-edge-payload="'+sha(Buffer.from(payload))+'">'+esc(payload)+'</pre></section>';
   write(dest,shell(dest,title,body));if(!manifest.pages.some(p=>p.path===dest))manifest.pages.push({path:dest,title,kind:'edge-evidence'});created.set(dest,{path:dest,source:asset,sourceSha256:sha(read(asset)),edgeId:edge.id,payloadSha256:sha(Buffer.from(payload))});}
  routes[row.old]=dest+'#'+anchor;edgeRoutes++;continue;
 }
 const id='route-'+sha(Buffer.from(row.old)).slice(0,20);unknown.push({...row,id,reason:'未找到与旧片段精确匹配的已捕获静态成员；模板字符串残留不解释为真实业务入口。'});routes[row.old]='清单/未解析旧入口.html#'+id;
}
if(unknown.length){const file='清单/未解析旧入口.html';write(file,shell(file,'未解析旧入口：保持明确未知','<p class="site-notice">这些登记字符串未能映射到确切正文。保留原键并明确说明，不静默跳到无关主题。它们不计为语义兼容通过。</p>'+unknown.map(r=>'<section id="'+r.id+'"><h2>'+esc(r.old)+'</h2><p>原目标 '+esc(r.target)+'</p><p>'+r.reason+'</p></section>').join('')));if(!manifest.pages.some(p=>p.path===file))manifest.pages.push({path:file,title:'未解析旧入口',kind:'compatibility-unknown'});}
write('清单/旧路由映射.json',JSON.stringify(routes,null,2)+'\n');write('公共资源/routes.js','window.DOC_ROUTES='+JSON.stringify(routes).replaceAll('<','\\u003c')+';\n');write('清单/路由定位修复.json',JSON.stringify({staticAnchorsFixed:fixed,edgeRoutes,edgePages:[...created.values()],unknown,semanticCompatibilityComplete:unknown.length===0,browserVerified:false},null,2)+'\n');manifest.counts.pages=manifest.pages.length;fs.writeFileSync(path.join(site,'清单/构建清单.json'),JSON.stringify(manifest,null,2)+'\n');console.log(JSON.stringify({staticAnchorsFixed:fixed,edgeRoutes,edgePages:created.size,unknown:unknown.length,browserVerified:false}));
