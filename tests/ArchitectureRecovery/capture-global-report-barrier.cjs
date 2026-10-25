'use strict';
const fs=require('fs'),path=require('path'),assert=require('assert/strict'),crypto=require('crypto');const {root}=require('./evidence-view.cjs'),inspect=require('./native-pe-inspection.cjs');
const artifact=inspect('D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll'),bytes=fs.readFileSync(artifact.path),dest='docs/architecture-recovery/global-report-barrier.resources',files=[],methods=[];
const off=r=>{const s=artifact.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
function capture(p,q){const b=fs.readFileSync(path.join(root,p)),target=dest+'/'+q;fs.mkdirSync(path.dirname(path.join(root,target)),{recursive:true});fs.writeFileSync(path.join(root,target),b);const f={path:target,source:p,sha256:crypto.createHash('sha256').update(b).digest('hex')};files.push(f);return f;}
const runs=['frame-completion-queue-build','frame-manager-write-guard','frame-close-guard-exports','frame-result-report-ingress','frame-result-callback-producers','frame-result-report-wait','frame-result-thread-entry'];
for(const run of runs){const folder='analysis/global-variable-followup/'+run,dir=folder+'/output',log=fs.readFileSync(path.join(root,folder,'functions-headless.log'),'utf8');assert(/Save succeeded for processed file/.test(log));capture(folder+'/functions-headless.log',run+'/functions-headless.log');const table=fs.readdirSync(path.join(root,dir)).find(f=>/^functions-.*\.tsv$/.test(f));capture(dir+'/'+table,run+'/'+table);
 for(const row of fs.readFileSync(path.join(root,dir,table),'utf8').trim().split(/\r?\n/).slice(1)){const [address,name,bodyBytes,decompiled]=row.split('\t');if(address==='1800580b0')continue; // Rejected navigation seed: not the PE runtime-function entry. Never cited.
  assert.equal(decompiled,'true');const ff=['.c','.asm.txt'].map(ext=>capture(dir+'/'+address+ext,run+'/'+address+ext));let calls=0;
  for(const c of fs.readFileSync(path.join(root,dir,address+'.asm.txt'),'utf8').matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){const r=Number(BigInt('0x'+c[1])-BigInt(artifact.imageBase)),o=off(r);assert.equal(bytes[o],0xe8);assert.equal(BigInt(artifact.imageBase)+BigInt(r+5+bytes.readInt32LE(o+1)),BigInt('0x'+c[2]));calls++;}
  methods.push({address,name,bodyBytes:Number(bodyBytes),directCallsChecked:calls,files:ff,entryFragment:address==='1800d3076'});
 }
}
const literals=[[0x220158,'CControlDelegate::CloseSolution'],[0x21f4f8,'[%s@%d] Set binary data [%s] failed, closing solution now'],[0x222320,'CControlDelegate::CanRunProcedure']].map(([rva,value])=>{const p=off(rva);assert.equal(bytes.toString('utf8',p,bytes.indexOf(0,p)),value);return{rva,value};});
// Keep navigation coverage separate: absence in this database is not absence in the binary.
const navigation=['functions-headless.log','output/coverage.txt',...fs.readdirSync(path.join(root,'analysis/global-variable-followup/frame-comm-manager-references/output')).filter(f=>f.startsWith('references-')).map(f=>'output/'+f)].map(f=>capture('analysis/global-variable-followup/frame-comm-manager-references/'+f,'navigation/'+f));
const manifest={schemaVersion:1,artifact,files,methods,literals,navigation,directCallsChecked:methods.reduce((n,m)=>n+m.directCallsChecked,0),rejectedNavigationSeeds:['1800580b0: corrected PE entry is 1800580c0'],vendorImageLoaded:false,originalRuntimeExecuted:false,fullExecutionContractClosed:false};
fs.writeFileSync(path.join(root,dest,'manifest.json'),JSON.stringify(manifest,null,2)+'\n');console.log(JSON.stringify({methods:methods.length,calls:manifest.directCallsChecked}));
