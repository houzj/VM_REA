'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root,out}=require('./evidence-view.cjs');
module.exports=function(){
 const read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),m=JSON.parse(read('docs/architecture-recovery/global-background-boundary.resources/manifest.json')),pe=require('./native-pe-inspection.cjs')(m.artifact.path);assert.deepEqual(pe,m.artifact);const bytes=read(pe.path);
 const offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
 for(const f of m.files)assert.equal(sha(read(f.path)),f.sha256);
 let calls=0;for(const f of m.methods){let n=0;for(const q of read(f.files.find(f=>f.path.endsWith('.asm.txt')).path).toString().matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+q[1])-BigInt(pe.imageBase)),p=offset(r);assert.equal(bytes[p],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+bytes.readInt32LE(p+1)),BigInt('0x'+q[2]));n++;}assert.equal(n,f.directCallsChecked);calls+=n;}
 const asm=a=>read(m.methods.find(x=>x.address===a).files.find(f=>f.path.endsWith('.asm.txt')).path).toString();assert.equal(m.methods.length,14);assert.equal(calls,73);
 for(const l of m.literals){const p=offset(l.rva);assert.equal(bytes.subarray(p,p+Buffer.byteLength(l.value)+1).toString(),l.value+'\0');}assert.equal('0x'+bytes.readBigUInt64LE(offset(m.callbackVirtual.tableRva)+m.callbackVirtual.slot).toString(16),m.callbackVirtual.targetVa);
 for(const t of ['MOV EDX,dword ptr [RCX + 0x10]','MOV RCX,qword ptr [RCX + 0x18]','JMP qword ptr [RAX + 0x8]'])assert(asm('1800e3570').includes(t));assert(asm('180080bb0').includes('MOV RAX,RBP'));assert(asm('1800e34f0').includes('MOV RBX,R9'));const create=pe.imports.find(i=>i.name==='CreateEventW');assert(asm('1800810d0').includes('CALL qword ptr ['+create.iatVa+']'));
 const r={passed:true,nativeMethods:m.methods.length,rawDirectCallsRechecked:calls,callbackVirtualTargetRechecked:true,literalsRechecked:3,timerEventImportRechecked:true,originalRuntimeExecuted:false,fullExecutionContractClosed:false};fs.writeFileSync(path.join(out,'global-background-boundary-review.json'),JSON.stringify(r,null,2)+'\n');return r;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
