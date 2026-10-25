const fs=require('fs'),path=require('path');
const site=path.resolve(__dirname,'../../../docs/架构逆向与实现对照文档');
for(const name of ['image-pipeline','group-data-pipeline','global-variable-analysis','nonimage-pipeline','retirement-operation-map']){
 const s=fs.readFileSync(path.join(site,'数据',name+'.html'),'utf8');
 console.log(JSON.stringify({name,headings:[...s.matchAll(/<h[234][^>]*>([\s\S]*?)<\/h[234]>/g)].filter(m=>!m[1].includes('当前独立')).map(m=>({title:m[1].replace(/<[^>]+>/g,''),before:s.slice(Math.max(0,m.index-180),m.index)}))}));
}
