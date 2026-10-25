'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root,out}=require('./evidence-view.cjs');
module.exports=function(){
 const read=p=>fs.readFileSync(path.resolve(root,p)),sha=b=>crypto.createHash('sha256').update(b).digest('hex'),m=JSON.parse(read('docs/architecture-recovery/global-procedure-comm.resources/manifest.json')),inspect=require('./native-pe-inspection.cjs'),bytes={};
 for(const [key,p] of Object.entries(m.artifacts)){assert.deepEqual(inspect(p.path),p);bytes[key]=read(p.path);}
 const offset=(key,r)=>{const s=m.artifacts[key].sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
 for(const f of m.files)assert.equal(sha(read(f.path)),f.sha256);
 let calls=0;for(const method of m.methods){let n=0;for(const q of read(method.files.find(f=>f.path.endsWith('.asm.txt')).path).toString().matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const p=m.artifacts[method.program],b=bytes[method.program],r=Number(BigInt('0x'+q[1])-BigInt(p.imageBase)),o=offset(method.program,r);assert.equal(b[o],0xe8);assert.equal(BigInt(p.imageBase)+BigInt(r+5+b.readInt32LE(o+1)),BigInt('0x'+q[2]));n++;}assert.equal(n,method.directCallsChecked);calls+=n;}
 assert.equal(m.methods.length,26);assert.equal(calls,363);assert.equal(calls,m.directCallsChecked);
 for(const v of m.virtualTargets)assert.equal('0x'+bytes[v.program].readBigUInt64LE(offset(v.program,v.tableRva)+v.slot).toString(16),v.targetVa);
 for(const l of m.literals){const p=offset(l.program,l.rva),b=bytes[l.program];assert.equal(b.toString('utf8',p,b.indexOf(0,p)),l.value);}
 for(const i of m.imports)assert(m.artifacts.ProcedureComm.imports.some(x=>JSON.stringify(x)===JSON.stringify(i)));
 const body=(program,address,ext='.c')=>read(m.methods.find(x=>x.program===program&&x.address===address).files.find(f=>f.path.endsWith(ext)).path).toString();
 assert(body('ProcedureComm','1800144b0').includes('((int *)plVar7[2])[1] == 1'));const setter=body('ProcedureComm','180014e10');assert(setter.includes('FUN_18001986e')||setter.includes('func_0x00018001986e'));assert(setter.includes('*(int *)(puVar4[2] + 4) == 2'));
 const lookup=body('ProcedureComm','180015b50');assert(lookup.includes('_Mtx_unlock(param_1 + 0x70)'));const readTyped=body('ProcedureComm','180014140');assert(readTyped.indexOf('VM_M_GetInt(')<readTyped.indexOf('VM_M_GetFloat('));assert(readTyped.indexOf('VM_M_GetFloat(')<readTyped.indexOf('VM_M_GetString('));
 for(const name of ['VM_M_GetInt','VM_M_GetFloat','VM_M_GetString'])assert(body('ProcedureComm','180014140','.asm.txt').includes('CALL qword ptr ['+m.imports.find(i=>i.name===name).iatVa+']'));
 assert(body('ModuleFrame','18011e580').includes('FUN_1800bcd20'));assert(body('ModuleFrame','18011e580').includes('FUN_18008e910'));assert(body('ModuleFrame','18011e580').includes('FUN_1800ca500'));
 assert(body('ModuleFrame','180043120').includes('param_1 + 0x22'));assert(body('ModuleFrame','180043120').includes('*plVar3 + 0x18'));assert(body('ModuleFrame','1800b9390').includes('memcpy_s'));
 const result={passed:true,nativeMethods:26,rawDirectCallsRechecked:calls,virtualTargetsRechecked:2,literalsRechecked:m.literals.length,importsRechecked:m.imports.length,vendorImageLoaded:false,originalRuntimeExecuted:false,fullExecutionContractClosed:false};fs.writeFileSync(path.join(out,'global-procedure-comm-review.json'),JSON.stringify(result,null,2)+'\n');return result;
};
if(require.main===module)console.log(JSON.stringify(module.exports()));
