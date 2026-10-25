const fs=require('fs'),path=require('path'),crypto=require('crypto');
const {out}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
// Reuse immutable snapshots within one generator process. Metadata is checked
// on every use, and all cached bytes are rehashed before a successful exit.
// This changes lookup cost, not the artifact/body/unique-member checks below.
const snapshots=new Map(),datasets=new Map();
function snapshot(file){
 const stat=fs.statSync(file,{bigint:true}),stamp=[stat.dev,stat.ino,stat.size,stat.mtimeNs,stat.ctimeNs].join(':');
 let cached=snapshots.get(file);if(!cached||cached.stamp!==stamp){const bytes=fs.readFileSync(file);cached={stamp,bytes,sha256:sha(bytes)};snapshots.set(file,cached);}return cached;
}
process.once('beforeExit',()=>{for(const [file,s]of snapshots)if(sha(fs.readFileSync(file))!==s.sha256)throw Error('Original CIL input changed before generator completion: '+file);});
function dataset(file){
 const s=snapshot(file);let cached=datasets.get(file);if(cached?.sha256===s.sha256)return cached;
 const data=JSON.parse(s.bytes),lines=s.bytes.toString('utf8').split(/\r?\n/),headers=new Map();
 const property=(line,key)=>{try{return JSON.parse('{'+line.trim().replace(/,$/,'')+'}')[key];}catch{return undefined;}};
 for(let i=1;i<lines.length-4;i++){const match=/^          "token": "(0x[0-9a-f]+)",$/.exec(lines[i]);if(!match)continue;const name=property(lines[i-1],'name'),type=property(lines[i+4],'type');if(name===undefined||type===undefined)continue;const key=JSON.stringify([type,name,match[1]]),hits=headers.get(key)??[];hits.push(i);headers.set(key,hits);}
 cached={sha256:s.sha256,data,lines,headers};datasets.set(file,cached);return cached;
}
function cilReference(fileName,type,name,token){
 const {sha256,data,lines,headers}=dataset(path.join(out,fileName)),matches=data.targets.filter(t=>t.type===type).flatMap(t=>t.methods.filter(m=>m.name===name&&(!token||m.token===token)).map(m=>({t,m})));
 if(matches.length!==1)throw Error('Original CIL reference target/member selection ambiguous');
 const {t,m}=matches[0],a=data.manifest.artifacts.find(a=>a.key===t.artifact);
 if(!a||snapshot(a.path).sha256!==a.sha256)throw Error('Original CIL reference identity changed');
 const il=Buffer.from(m.ilBytes??'','hex');if(m.status!=='decoded'||il.length!==m.ilSize||sha(il)!==m.ilSha256)throw Error('Original CIL body changed');
 const hits=headers.get(JSON.stringify([type,name,m.token]))??[];
 if(hits.length!==1)throw Error('Original CIL method header ambiguous');const start=hits[0],end=start+20;
 return {path:'docs/architecture-recovery/'+fileName,member:type+'::'+name,sha256,start:start+1,end,excerpt:lines.slice(start,end).join('\n'),evidenceType:'original-managed-cil',binaryIdentity:a.sha256+' / MVID '+a.mvid};
}
module.exports={cilReference};
