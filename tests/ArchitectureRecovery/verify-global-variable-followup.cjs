'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');
const {root,out}=require('./evidence-view.cjs'),read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const model=JSON.parse(read('docs/architecture-recovery/global-variable-execution.json')),p=model.provenance.followup,manifest=JSON.parse(read(p.manifest));let calls=0;
 for(const f of manifest.files)assert.equal(sha(read(f.path)),f.sha256);
 for(const [key,pe]of Object.entries(manifest.artifacts)){
  assert.deepEqual(require('./native-pe-inspection.cjs')(pe.path),pe);const b=read(pe.path),offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
  for(const m of manifest.methods.filter(m=>m.artifact===key)){let n=0;for(const q of read(m.files.find(f=>f.path.endsWith('.asm.txt')).path).toString().matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+q[1])-BigInt(pe.imageBase)),i=offset(r);assert.equal(b[i],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+b.readInt32LE(i+1)),BigInt('0x'+q[2]));n++;}assert.equal(n,m.directCallsChecked);calls+=n;}
 }
 assert.equal(manifest.methods.length,25);assert.equal(calls,454);assert.equal(calls,p.rawDirectCallsChecked);
 const asm=a=>read(manifest.methods.find(m=>m.address===a).files.find(f=>f.path.endsWith('.asm.txt')).path).toString();
 const index=asm('180099be0');for(const line of ['JZ 0x180099fcd','JZ 0x180099fc8','JZ 0x180099fc3','180099fc3\tMOV EAX,dword ptr [RDX + 0x4]','180099fc8\tMOV EAX,R8D','180099fcd\tXOR EAX,EAX'])assert(index.includes(line));
 const loop=read(manifest.methods.find(m=>m.address==='1800dd330').files.find(f=>f.path.endsWith('.c')).path).toString();assert(loop.indexOf('SetEvent(*(undefined8 *)(param_1 + 0x88))')<loop.indexOf('if (*(longlong *)(param_1 + 0x1c8)'));assert(loop.includes('SetEvent(*(undefined8 *)(param_1 + 0x98))'));
 assert.equal(manifest.probe.passed,false);assert(manifest.probe.buildAttempts.length===2);assert.equal(p.originalRuntimeExecuted,false);assert.equal(p.fullExecutionContractClosed,false);
 const report={passed:true,additionalNativeMethods:25,additionalRawCalls:454,focusedIndexAssemblyChecked:true,stopEventOrderChecked:true,failedRuntimeProbePreserved:true,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
 fs.writeFileSync(path.join(out,'global-variable-followup-review.json'),JSON.stringify(report,null,2)+'\n');return report;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
