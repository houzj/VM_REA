'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root,out}=require('./evidence-view.cjs');
module.exports=function(){
 const read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),m=JSON.parse(read('docs/architecture-recovery/global-timer-worker.resources/manifest.json')),pe=require('./native-pe-inspection.cjs')(m.artifact.path);assert.deepEqual(pe,m.artifact);const bytes=read(pe.path);
 const offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
 for(const f of m.files)assert.equal(sha(read(f.path)),f.sha256);
 let calls=0;for(const f of m.methods){let n=0;for(const q of read(f.files.find(f=>f.path.endsWith('.asm.txt')).path).toString().matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+q[1])-BigInt(pe.imageBase)),p=offset(r);assert.equal(bytes[p],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+bytes.readInt32LE(p+1)),BigInt('0x'+q[2]));n++;}assert.equal(n,f.directCallsChecked);calls+=n;}
 const asm=a=>read(m.methods.find(x=>x.address===a).files.find(f=>f.path.endsWith('.asm.txt')).path).toString();assert.equal(m.methods.length,8);assert.equal(calls,33);assert.equal('0x'+bytes.readBigUInt64LE(offset(m.taskVirtual.tableRva)+m.taskVirtual.slot).toString(16),m.taskVirtual.targetVa);assert(asm('1800820a0').includes('0x180080990'));assert(asm('180082b90').includes('ADD RCX,0x8'));assert(asm('180082b90').includes('JMP 0x180082e30'));const cb=asm('180080990'),call=cb.indexOf('CALL qword ptr [RAX + 0x10]'),set=pe.imports.find(i=>i.name==='SetEvent');assert(call>0);assert(cb.indexOf('CALL qword ptr ['+set.iatVa+']')>call);assert(cb.includes('CALL 0x180080a30'));
 const r={passed:true,nativeMethods:m.methods.length,rawDirectCallsRechecked:calls,taskVirtualTargetRechecked:true,callbackBeforeEventSetRechecked:true,originalRuntimeExecuted:false,fullExecutionContractClosed:false};fs.writeFileSync(path.join(out,'global-timer-worker-review.json'),JSON.stringify(r,null,2)+'\n');return r;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
