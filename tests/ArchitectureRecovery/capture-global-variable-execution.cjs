// Freeze this explicitly requested source investigation. Never loads vendor code.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');
const {root,out}=require('./evidence-view.cjs'),inspect=require('./native-pe-inspection.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const dir='docs/architecture-recovery/global-variable-execution.resources/';
const write=(p,b)=>{fs.mkdirSync(path.dirname(path.join(root,p)),{recursive:true});fs.writeFileSync(path.join(root,p),b);return {path:p,sha256:sha(b),byteLength:b.length};};
const nativePath='D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll';
const pe=inspect(nativePath),b=fs.readFileSync(nativePath);assert.equal(pe.sha256,'45c57bc46f72fd1f9e0b3a6f9183ff974d14590cfd05ecf4e9fa97a4ff95ae9b');
const offset=r=>{const s=pe.sections.find(s=>r>=s.rva&&r<s.rva+s.rawSize);assert(s);return s.rawOffset+r-s.rva;};
const runs=['native-lock','native-entry','native-internal','native-flow','native-boundaries','native-production','native-binding-consume','native-observer','native-bound-writers','native-update-callers','native-bound-value','native-final-store','native-index-store'];
const methods=[];let directCallsChecked=0;
for(const run of runs){
 const base=path.join(root,'analysis/global-variable-execution',run),output=path.join(base,'output'),tables=fs.readdirSync(output).filter(n=>/^functions-.*\.tsv$/.test(n));assert.equal(tables.length,1,run);
 const log=fs.readFileSync(path.join(base,'functions-headless.log'),'utf8');assert(log.includes('Import succeeded'),run+' incomplete');
 for(const line of fs.readFileSync(path.join(output,tables[0]),'utf8').trim().split(/\r?\n/).slice(1)){
  const [address,name,bodyBytes,decompiled]=line.split('\t');assert.equal(decompiled,'true',address);
  const files=['.c','.asm.txt'].map(suffix=>write(dir+'native/'+address+suffix,fs.readFileSync(path.join(output,address+suffix))));
  const asm=fs.readFileSync(path.join(output,address+'.asm.txt'),'utf8');let checked=0;
  for(const match of asm.matchAll(/^([a-f0-9]+)\tCALL 0x([a-f0-9]+)$/gm)){
   const rva=Number(BigInt('0x'+match[1])-BigInt(pe.imageBase)),q=offset(rva);assert.equal(b[q],0xe8);assert.equal(BigInt(rva+5+b.readInt32LE(q+1))+BigInt(pe.imageBase),BigInt('0x'+match[2]));checked++;
  }
  directCallsChecked+=checked;methods.push({address,name,bodyBytes:Number(bodyBytes),decompiled:true,run,files,directCallsChecked:checked});
 }
 write(dir+'native/'+run+'.tsv',fs.readFileSync(path.join(output,tables[0])));
}
assert.equal(new Set(methods.map(m=>m.address)).size,methods.length);
const externalRoot='D:/MyWork/SunEyeVision_V2';
const externalPaths=['src/UI/Services/Workflow/WorkflowExecutionManager.cs','src/Workflow/WorkflowExecutionEngine.cs','src/Workflow/WorkflowContext.cs','src/Workflow/WorkflowRunRequest.cs','src/Workflow/WorkflowExecutionDependencyGraph.cs','src/Workflow/SolutionExecutionDependencyGraph.cs','src/Workflow/AlgorithmNode.cs','src/Plugin.SDK/Core/ToolExecutor.cs','src/Plugin.SDK/Execution/Parameters/ParameterResolver.cs','src/Plugin.SDK/Models/Communication/RuntimeVariableReceiveService.cs','src/UI/Shell/Windows/MainWindowViewModel.cs'];
const external=externalPaths.map(relative=>{const originalPath=externalRoot+'/'+relative,bytes=fs.readFileSync(originalPath),r=write(dir+'suneye/'+sha(bytes).slice(0,16)+'-'+path.basename(relative)+'.source.txt',bytes);return{relative,originalPath,...r};});
const tools=['tests/NativeImageBinaryProbe/Run-GhidraFunctions.ps1','tests/NativeEvidence/VmNativeCapability.java','tests/ArchitectureRecovery/capture-global-variable-cil.ps1'].map(p=>({path:p,sha256:sha(fs.readFileSync(path.join(root,p)))}));
const vtable=Array.from({length:6},(_,i)=>({offset:i*8,va:'0x'+b.readBigUInt64LE(offset(0x226d90)+i*8).toString(16)}));
const inputs=['managed-cil.json','rea-core-identity.json','rea-members-unavailable.json'].map(n=>{const p=dir+n,bytes=fs.readFileSync(path.join(root,p));return{path:p,sha256:sha(bytes)};});
const result={schemaVersion:1,capturedUtc:new Date().toISOString(),scope:'Original global-variable execution and exact SunEye reference paths; static evidence, not runtime parity',native:pe,nativeMethods:methods,directCallsChecked,observerVtable:{rva:'0x226d90',entries:vtable},external,tools,inputs,failedAttempts:[{kind:'initial native capture path',admittedAsEvidence:false,reason:'incorrect directory; corrected to manifest-bound ModuleFrame path'},{kind:'REA managed members',admittedAsEvidence:false,reason:'canonical provider metadata representation budget; raw PEReader CIL captured separately'}],runtimeExecuted:false,vendorAssemblyLoaded:false,fullExecutionContractClosed:false};
write(dir+'manifest.json',Buffer.from(JSON.stringify(result,null,2)+'\n'));console.log(JSON.stringify({nativeMethods:methods.length,directCallsChecked,externalSourceFiles:external.length,runtimeExecuted:false}));
