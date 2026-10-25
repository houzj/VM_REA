'use strict';
// Freeze read-only offline disassembly. No vendor image is loaded or executed.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');
const {root}=require('./evidence-view.cjs'),inspect=require('./native-pe-inspection.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const consumer=process.argv.includes('--consumer');
const dest='docs/architecture-recovery/global-variable-'+(consumer?'consumer':'followup')+'.resources';
const runs=consumer?[['frame-registration','frame'],['frame-consumer','frame']]:[['frame-read-stop','frame'],['frame-stop-producer','frame'],['frame-store-route','frame'],['sdk-bind-stop','sdk'],['sdk-command','sdk'],['platform-dispatch','platform'],['platform-command','platform'],['platform-route','platform']];
const artifacts={frame:inspect('D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll'),sdk:inspect('D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll'),platform:inspect('D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll')};
const files=[],methods=[];
function capture(p,relative){const b=fs.readFileSync(path.join(root,p)),q=dest+'/'+relative;fs.mkdirSync(path.dirname(path.join(root,q)),{recursive:true});fs.writeFileSync(path.join(root,q),b);const row={path:q,source:p,sha256:sha(b)};files.push(row);return row;}
for(const [run,artifact]of runs){
 const folder='analysis/global-variable-followup/'+run,log=fs.readFileSync(path.join(root,folder,'functions-headless.log'),'utf8');assert(log.includes('Import succeeded'),'incomplete import '+run);
 capture(folder+'/functions-headless.log',run+'/functions-headless.log');
 const output=folder+'/output',table=fs.readdirSync(path.join(root,output)).find(f=>/^functions-.*\.tsv$/.test(f));assert(table);capture(output+'/'+table,run+'/'+table);
 const pe=artifacts[artifact],b=fs.readFileSync(pe.path),offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
 for(const line of fs.readFileSync(path.join(root,output,table),'utf8').trim().split(/\r?\n/).slice(1)){
  const [address,name,bodyBytes,decompiled]=line.split('\t');assert.equal(decompiled,'true');const f=['.c','.asm.txt'].map(ext=>capture(output+'/'+address+ext,run+'/'+address+ext));
  const asm=fs.readFileSync(path.join(root,output,address+'.asm.txt'),'utf8');let calls=0;
  for(const m of asm.matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+m[1])-BigInt(pe.imageBase)),p=offset(r);assert.equal(b[p],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+b.readInt32LE(p+1)),BigInt('0x'+m[2]));calls++;}
  methods.push({artifact,address,name,bodyBytes:Number(bodyBytes),directCallsChecked:calls,files:f,pseudocodeCoverage:address==='180602df0'?'Decompiler follows switch fragments beyond listed function body; case pseudocode is a lead, not assembly-confirmed dispatch.':'Captured function; displayed ABI must be checked against assembly.'});
 }
}
const attempts=['suneye-probe-build.log','suneye-probe-build-net10.log'].filter(f=>fs.existsSync(path.join(root,'analysis/global-variable-followup',f))).map(f=>capture('analysis/global-variable-followup/'+f,'probe-attempts/'+f));
const manifest={schemaVersion:1,artifacts,methods,files,directCallsChecked:methods.reduce((n,m)=>n+m.directCallsChecked,0),vendorImageLoaded:false,originalRuntimeExecuted:false,fullExecutionContractClosed:false,probe:{passed:false,buildAttempts:attempts,reason:'Current Workflow source is newer than available Workflow/SDK binaries; linked-source net10 build fails on missing current SDK contracts. Failed builds establish no runtime behavior.'}};
fs.writeFileSync(path.join(root,dest,'manifest.json'),JSON.stringify(manifest,null,2)+'\n');console.log(JSON.stringify({methods:methods.length,calls:manifest.directCallsChecked,probePassed:false}));
