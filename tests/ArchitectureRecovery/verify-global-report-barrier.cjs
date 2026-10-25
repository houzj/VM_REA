'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root,out}=require('./evidence-view.cjs');
module.exports=function(){
 const read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),m=JSON.parse(read('docs/architecture-recovery/global-report-barrier.resources/manifest.json')),pe=require('./native-pe-inspection.cjs')(m.artifact.path);assert.deepEqual(pe,m.artifact);const bytes=read(pe.path),off=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
 for(const f of m.files)assert.equal(sha(read(f.path)),f.sha256);let calls=0;
 for(const method of m.methods){let n=0;for(const c of read(method.files.find(f=>f.path.endsWith('.asm.txt')).path).toString().matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+c[1])-BigInt(pe.imageBase)),o=off(r);assert.equal(bytes[o],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+bytes.readInt32LE(o+1)),BigInt('0x'+c[2]));n++;}assert.equal(n,method.directCallsChecked);calls+=n;}
 assert.equal(m.methods.length,15);assert.equal(calls,208);assert.equal(calls,m.directCallsChecked);assert(!m.methods.some(x=>x.address==='1800580b0'));assert(m.methods.find(x=>x.address==='1800d3076').entryFragment);
 for(const l of m.literals){const p=off(l.rva);assert.equal(bytes.toString('utf8',p,bytes.indexOf(0,p)),l.value);}
 const body=(a,ext='.asm.txt')=>read(m.methods.find(x=>x.address===a).files.find(f=>f.path.endsWith(ext)).path).toString();
 assert.equal(bytes.subarray(off(0x476d6),off(0x476d6)+4).toString('hex'),'c6462201');assert.equal(bytes.subarray(off(0x47957),off(0x47957)+4).toString('hex'),'c6462200');assert(body('18013d440').includes('CALL 0x180047570'));assert(body('18013d550').includes('0x180047570'));
 const wait=pe.imports.find(i=>i.name==='WaitForSingleObject'),set=pe.imports.find(i=>i.name==='SetEvent'),waitBody=body('1800d53c0');assert(waitBody.includes('MOV EDX,0x32'));assert(waitBody.includes('CALL qword ptr ['+wait.iatVa+']'));assert(waitBody.includes('JMP 0x1800d53f0'));
 const worker=body('1800d3dd0'),callback=worker.indexOf('CALL RAX');assert(callback>worker.indexOf('MOV RAX,qword ptr [0x1802a5260]'));assert(worker.indexOf('CALL qword ptr ['+set.iatVa+']')>callback);assert(body('180140610').includes('qword ptr [0x1802a5260]'));
 const r={passed:true,nativeBodies:15,rawDirectCallsRechecked:calls,closingGuardSetAndClearRechecked:true,callbackBeforeCompletionEventRechecked:true,reportWaitRepeatsAfter50ms:true,rejectedSeedNotCited:true,originalRuntimeExecuted:false,fullExecutionContractClosed:false};fs.writeFileSync(path.join(out,'global-report-barrier-review.json'),JSON.stringify(r,null,2)+'\n');return r;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
