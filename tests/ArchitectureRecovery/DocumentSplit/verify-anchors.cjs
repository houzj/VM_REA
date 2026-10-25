'use strict';
const fs=require('fs'),path=require('path');const site=path.resolve(__dirname,'../../../docs/架构逆向与实现对照文档'),read=p=>fs.readFileSync(path.join(site,p),'utf8'),routes=JSON.parse(read('清单/旧路由映射.json')),cache=new Map();
function page(p){if(!cache.has(p)){const html=read(p),ids=new Set([...html.matchAll(/\b(?:id|data-[\w-]*fact)="([^"]+)"/g)].map(m=>m[1]));cache.set(p,{html,ids});}return cache.get(p);}
const unresolved=[],verified=[];
for(const [old,target]of Object.entries(routes)){const [file,...parts]=target.split('#'),anchor=parts.join('#');if(!anchor){verified.push({old,target,kind:'page'});continue;}const {html,ids}=page(file);if(ids.has(anchor)){verified.push({old,target,kind:'static-anchor'});continue;}const dynamic=html.includes('<script')&&file.startsWith('专题/');unresolved.push({old,target,kind:dynamic?'dynamic-anchor-unverified':'anchor-not-found',containsLiteral:html.includes(anchor)});}
const report={passed:unresolved.length===0,scope:'All registered legacy route fragment targets; dynamic-script presence does not prove an anchor works',routes:Object.keys(routes).length,verified:verified.length,unresolved:unresolved.length,items:unresolved,browserVerified:false};
fs.writeFileSync(path.join(site,'清单/锚点定位检查.json'),JSON.stringify(report,null,2)+'\n');console.log(JSON.stringify({...report,items:unresolved.slice(0,16)}));
