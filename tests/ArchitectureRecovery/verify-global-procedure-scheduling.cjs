'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root,out}=require('./evidence-view.cjs');
module.exports=function(){
 const read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),m=JSON.parse(read('docs/architecture-recovery/global-procedure-scheduling.resources/manifest.json')),pe=require('./native-pe-inspection.cjs')(m.artifact.path);assert.deepEqual(pe,m.artifact);const bytes=read(pe.path);
 const offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
 for(const f of m.files)assert.equal(sha(read(f.path)),f.sha256);
 let calls=0;for(const f of m.methods){let n=0;for(const q of read(f.files.find(f=>f.path.endsWith('.asm.txt')).path).toString().matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+q[1])-BigInt(pe.imageBase)),p=offset(r);assert.equal(bytes[p],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+bytes.readInt32LE(p+1)),BigInt('0x'+q[2]));n++;}assert.equal(n,f.directCallsChecked);calls+=n;}
 const asm=a=>read(m.methods.find(x=>x.address===a).files.find(f=>f.path.endsWith('.asm.txt')).path).toString();assert.equal(m.methods.length,10);assert.equal(calls,146);for(const [a,targets]of [['1800e4080',['1800e4330','180128200']],['1800e4330',['18006ddb0','180070160','180077b00']],['180128200',['1800e3fe0','1800dcc80']],['18013ade0',['1800e38a0']]])for(const t of targets)assert(asm(a).includes('CALL 0x'+t));
 const r={passed:true,nativeMethods:m.methods.length,rawDirectCallsRechecked:calls,completedIdRemovalAndResetCallsRechecked:true,originalRuntimeExecuted:false,fullExecutionContractClosed:false};fs.writeFileSync(path.join(out,'global-procedure-scheduling-review.json'),JSON.stringify(r,null,2)+'\n');return r;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
