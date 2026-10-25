'use strict';
// Preserve offline bodies and recheck direct calls and virtual targets against shipped bytes.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');
const {root}=require('./evidence-view.cjs'),inspect=require('./native-pe-inspection.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),dest='docs/architecture-recovery/global-procedure-scheduling.resources';
const pe=inspect('D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll'),bytes=fs.readFileSync(pe.path);
const offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
const files=[],methods=[];
function capture(p,q){const b=fs.readFileSync(path.join(root,p)),target=dest+'/'+q;fs.mkdirSync(path.dirname(path.join(root,target)),{recursive:true});fs.writeFileSync(path.join(root,target),b);const f={path:target,source:p,sha256:sha(b)};files.push(f);return f;}
for(const run of ['frame-procedure-dispatch','frame-procedure-ready','frame-procedure-root-and-subscribe']){
 const folder='analysis/global-variable-followup/'+run,log=fs.readFileSync(path.join(root,folder,'functions-headless.log'),'utf8');assert(/Import succeeded|Save succeeded for processed file/.test(log),run);
 capture(folder+'/functions-headless.log',run+'/functions-headless.log');
 const dir=folder+'/output',table=fs.readdirSync(path.join(root,dir)).find(f=>/^functions-.*\.tsv$/.test(f));assert(table);capture(dir+'/'+table,run+'/'+table);
 for(const row of fs.readFileSync(path.join(root,dir,table),'utf8').trim().split(/\r?\n/).slice(1)){
  const [address,name,bodyBytes,decompiled]=row.split('\t');assert.equal(decompiled,'true');const captured=['.c','.asm.txt'].map(ext=>capture(dir+'/'+address+ext,run+'/'+address+ext));
  let calls=0;for(const m of fs.readFileSync(path.join(root,dir,address+'.asm.txt'),'utf8').matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+m[1])-BigInt(pe.imageBase)),p=offset(r);assert.equal(bytes[p],0xe8);assert.equal(BigInt(pe.imageBase)+BigInt(r+5+bytes.readInt32LE(p+1)),BigInt('0x'+m[2]));calls++;}
  methods.push({address,name,bodyBytes:Number(bodyBytes),directCallsChecked:calls,files:captured});
 }
}
const manifest={schemaVersion:1,artifact:pe,files,methods,directCallsChecked:methods.reduce((n,m)=>n+m.directCallsChecked,0),vendorImageLoaded:false,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
fs.writeFileSync(path.join(root,dest,'manifest.json'),JSON.stringify(manifest,null,2)+'\n');console.log(JSON.stringify({methods:methods.length,calls:manifest.directCallsChecked}));
