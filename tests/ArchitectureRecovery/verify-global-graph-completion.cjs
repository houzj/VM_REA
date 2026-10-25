'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root,out}=require('./evidence-view.cjs');
module.exports=function(){
 const read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),m=JSON.parse(read('docs/architecture-recovery/global-graph-completion.resources/manifest.json')),pe=require('./native-pe-inspection.cjs')(m.artifact.path);assert.deepEqual(pe,m.artifact);const bytes=read(pe.path);
 const offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
 for(const f of m.files)assert.equal(sha(read(f.path)),f.sha256);
 let calls=0;for(const f of m.methods){let n=0;for(const q of read(f.files.find(f=>f.path.endsWith('.asm.txt')).path).toString().matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+q[1])-BigInt(pe.imageBase)),p=offset(r);assert.equal(bytes[p],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+bytes.readInt32LE(p+1)),BigInt('0x'+q[2]));n++;}assert.equal(n,f.directCallsChecked);calls+=n;}
 for(const v of m.virtualTargets)assert.equal('0x'+bytes.readBigUInt64LE(offset(v.tableRva)+v.offset).toString(16),v.va);
 assert.equal(m.methods.length,22);assert.equal(calls,412);assert.equal(m.virtualTargets.length,3);
 const asm=a=>read(m.methods.find(x=>x.address===a).files.find(f=>f.path.endsWith('.asm.txt')).path).toString();
 assert(asm('1800ffd00').includes('CALL qword ptr [RAX + 0x8]'));assert(asm('180020dd0').includes('0x1800b9810'));const set=pe.imports.find(i=>i.name==='SetEvent');assert(set);assert(asm('1800b97e0').includes('CALL qword ptr ['+set.iatVa+']'));const wait=pe.imports.find(i=>i.name==='WaitForSingleObject');assert(asm('1800b9810').includes('CALL qword ptr ['+wait.iatVa+']'));
 const r={passed:true,nativeMethods:m.methods.length,rawDirectCallsRechecked:calls,virtualTargetsRechecked:m.virtualTargets.length,originalRuntimeExecuted:false,fullExecutionContractClosed:false};fs.writeFileSync(path.join(out,'global-graph-completion-review.json'),JSON.stringify(r,null,2)+'\n');return r;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
