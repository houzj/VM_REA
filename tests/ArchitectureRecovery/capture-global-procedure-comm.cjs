'use strict';
// Offline preservation only. No inspected DLL is loaded or executed.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');
const {root}=require('./evidence-view.cjs'),inspect=require('./native-pe-inspection.cjs');
const dest='docs/architecture-recovery/global-procedure-comm.resources',files=[],methods=[];
const artifacts={ModuleFrame:inspect('D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll'),ProcedureComm:inspect('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Global/ProcedureComm/ProcedureComm.dll')};
const bytes=Object.fromEntries(Object.entries(artifacts).map(([k,v])=>[k,fs.readFileSync(v.path)]));
const offset=(key,r)=>{const s=artifacts[key].sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
function capture(src,target){const b=fs.readFileSync(path.join(root,src)),p=dest+'/'+target;fs.mkdirSync(path.dirname(path.join(root,p)),{recursive:true});fs.writeFileSync(path.join(root,p),b);const f={path:p,source:src,sha256:crypto.createHash('sha256').update(b).digest('hex')};files.push(f);return f;}
const runs={ModuleFrame:['frame-provider-and-finish','frame-procedure-comm-imports','frame-comm-read-delivery','frame-comm-tail-guards','frame-comm-manager-and-submit'],ProcedureComm:['procedure-comm-create','procedure-comm-timer-interface','procedure-comm-process-item','procedure-comm-read-and-gate','procedure-comm-scalar-read','procedure-comm-value-fetch']};
for(const [program,names] of Object.entries(runs))for(const run of names){
 const dir='analysis/global-variable-followup/'+run,log=fs.readFileSync(path.join(root,dir,'functions-headless.log'),'utf8');assert(/Import succeeded|Save succeeded for processed file/.test(log),run);capture(dir+'/functions-headless.log',run+'/functions-headless.log');
 const table=fs.readdirSync(path.join(root,dir,'output')).find(f=>/^functions-.*\.tsv$/.test(f));assert(table);capture(dir+'/output/'+table,run+'/'+table);
 for(const row of fs.readFileSync(path.join(root,dir,'output',table),'utf8').trim().split(/\r?\n/).slice(1)){
  const [address,name,bodyBytes,decompiled]=row.split('\t');assert.equal(decompiled,'true');const ff=['.c','.asm.txt'].map(ext=>capture(dir+'/output/'+address+ext,run+'/'+address+ext));let calls=0;
  for(const m of fs.readFileSync(path.join(root,dir,'output',address+'.asm.txt'),'utf8').matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+m[1])-BigInt(artifacts[program].imageBase)),p=offset(program,r),b=bytes[program];assert.equal(b[p],0xe8);assert.equal(BigInt(artifacts[program].imageBase)+BigInt(r+5+b.readInt32LE(p+1)),BigInt('0x'+m[2]));calls++;}
  methods.push({program,address,name,bodyBytes:Number(bodyBytes),directCallsChecked:calls,files:ff});
 }
}
const virtualTargets=[{program:'ProcedureComm',tableRva:0x1d810,slot:0x10,targetVa:'0x1800144b0'},{program:'ProcedureComm',tableRva:0x1d810,slot:0x18,targetVa:'0x180014e10'}];
for(const v of virtualTargets)assert.equal('0x'+bytes[v.program].readBigUInt64LE(offset(v.program,v.tableRva)+v.slot).toString(16),v.targetVa);
const literalSpecs=[[0x1d008,'%%condition_%d%%'],[0x1d020,'%%ok_send_%d%%'],[0x1d030,'%%ng_send_%d%%'],[0x1d040,'%%timeout_send_%d%%'],[0x1d1c0,'GetTimerItems'],[0x1d408,'ProcessItem'],[0x1d418,'ProcessWorkFinish'],[0x1cd20,'WriteString-%d']];
const literals=literalSpecs.map(([rva,value])=>{const p=offset('ProcedureComm',rva),b=bytes.ProcedureComm;assert.equal(b.toString('utf8',p,b.indexOf(0,p)),value);return{program:'ProcedureComm',rva,value};});
const imports=artifacts.ProcedureComm.imports.filter(i=>i.dll.toLowerCase()==='moduleframe.dll'&&['VM_M_GetInt','VM_M_GetFloat','VM_M_GetString','VM_M_SetBinayData'].includes(i.name));assert.equal(imports.length,4);
const manifest={schemaVersion:1,artifacts,files,methods,virtualTargets,literals,imports,directCallsChecked:methods.reduce((n,m)=>n+m.directCallsChecked,0),vendorImageLoaded:false,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
fs.writeFileSync(path.join(root,dest,'manifest.json'),JSON.stringify(manifest,null,2)+'\n');console.log(JSON.stringify({methods:methods.length,calls:manifest.directCallsChecked}));
