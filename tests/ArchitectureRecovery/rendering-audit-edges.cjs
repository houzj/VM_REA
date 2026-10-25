const fs=require('fs'),crypto=require('crypto');
module.exports=function(){
 const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),edges=[];
 for(const [file,route] of [['tool-rendering-entry-audit.json','原版 · Blob工具显示资源与实际操作供应'],['rendering-binding-resource-review.json','原版 · 绘制模板载荷与绑定声明']]){
  const path='docs/architecture-recovery/'+file,bytes=fs.readFileSync(path),d=JSON.parse(bytes);if(d.complete||d.runtimeExecuted||d.browserRendered||!Array.isArray(d.findings))throw Error('Unsupported rendering audit completion');
  for(const e of d.findings){if(!['source-confirmed','independent-implementation','unknown'].includes(e.classification)||e.classification!=='unknown'&&!e.refs.length)throw Error('Invalid rendering audit classification');for(const r of e.refs){const b=fs.readFileSync(r.path);if(sha(b)!==r.sha256||b.toString('utf8').split(/\r?\n/).slice(r.start-1,r.end).join('\n')!==r.excerpt)throw Error('Rendering audit ref drift '+e.id);}edges.push({...e,route,sourceAudit:{path,sha256:sha(bytes)},operationComplete:false});}
 }
 return edges;
};
