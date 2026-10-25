'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root,out}=require('./evidence-view.cjs');
module.exports=function(){
 const read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),m=JSON.parse(read('docs/architecture-recovery/global-data-continuation.resources/manifest.json')),pe=require('./native-pe-inspection.cjs')(m.artifact.path);assert.deepEqual(pe,m.artifact);const bytes=read(pe.path);
 const offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
 for(const f of m.files)assert.equal(sha(read(f.path)),f.sha256);
 let calls=0;for(const f of m.methods){let n=0;for(const q of read(f.files.find(f=>f.path.endsWith('.asm.txt')).path).toString().matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+q[1])-BigInt(pe.imageBase)),p=offset(r);assert.equal(bytes[p],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+bytes.readInt32LE(p+1)),BigInt('0x'+q[2]));n++;}assert.equal(n,f.directCallsChecked);calls+=n;}
 const asm=a=>read(m.methods.find(x=>x.address===a).files.find(f=>f.path.endsWith('.asm.txt')).path).toString();
 for(const [address,names] of [['1800eff60',['OpenFileMappingA','MapViewOfFile']],['1800f01e0',['UnmapViewOfFile','CloseHandle']]])for(const name of names){const imp=pe.imports.find(i=>i.name===name);assert(imp);assert(asm(address).includes('CALL qword ptr ['+imp.iatVa+']'));}
 assert.equal('0x'+bytes.readBigUInt64LE(offset(m.imageControlBlock.tableRva)).toString(16),m.imageControlBlock.destroyTarget);assert(asm('1800f1750').includes('0x18022e3f8'));assert(asm('1800f2b80').includes('CALL 0x1800f01e0'));
 assert.equal(m.methods.length,23);assert.equal(calls,m.directCallsChecked);assert(calls>157);assert(asm('1801466c0').includes('CALL 0x1800e38a0'));assert(asm('180146820').includes('CALL 0x1800e3dd0'));
 const r={passed:true,nativeMethods:m.methods.length,rawDirectCallsRechecked:calls,imageControlBlockTargetRechecked:true,importCallsRechecked:4,originalRuntimeExecuted:false,fullExecutionContractClosed:false};fs.writeFileSync(path.join(out,'global-data-continuation-review.json'),JSON.stringify(r,null,2)+'\n');return r;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
