// Independent static identity/CIL review; no vendor assembly loading or execution.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert');
const {root,out}=require('./evidence-view.cjs');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
module.exports=function(){
 const bundles=[];
 for(const [prefix,expected]of [['image-notification',11],['render-bridge',26],['result-image-selection',21],['display-inflight',30],['result-payload-bridge',12],['result-query-ownership',17]]){
  const read=s=>JSON.parse(fs.readFileSync(path.join(out,prefix+s))),manifest=read('.artifact-identities.json'),cil=read('.cil.json');
  for(const a of manifest.artifacts){assert.equal(sha(fs.readFileSync(a.path)),a.sha256);assert(cil.manifest.artifacts.some(x=>x.key===a.key&&x.sha256===a.sha256&&x.mvid===a.mvid));}
  let methods=0,branches=0;
  for(const t of cil.targets)for(const m of t.methods){
   assert.equal(m.status,'decoded');const bytes=Buffer.from(m.ilBytes||'','hex');assert.equal(bytes.length,m.ilSize);assert.equal(sha(bytes),m.ilSha256);
   const offsets=new Set(m.instructions.map(i=>i.offset));for(const i of m.instructions)if(/BrTarget|Switch/.test(i.operandKind))for(const target of Array.isArray(i.operand)?i.operand:[i.operand]){assert(offsets.has(target));branches++;}methods++;
  }
  assert.equal(methods,expected);bundles.push({prefix,methodBodies:methods,branchTargets:branches});
 }
 const native=JSON.parse(fs.readFileSync(path.join(out,'image-producer-recovery-audit.json')));
 assert.equal(native.retainedSourceRecovery.bodies.length,47);assert.equal(native.cliLaunched,false);assert.equal(native.originalRuntimeExecuted,false);assert.equal(native.targetAssemblyLoaded,false);assert.equal(native.deviceCalled,false);
 const identities=[native.retainedSourceRecovery.retainedExport,...native.retainedSourceRecovery.bodies.flatMap(b=>[b.pseudocode,b.retainedEvidence]),native.retainedSourceRecovery.processAssembly,...native.targets,...native.offlineTools.toolFiles];
 for(const f of identities)assert.equal(sha(fs.readFileSync(path.resolve(root,f.path))),f.sha256);
 for(const b of native.retainedSourceRecovery.bodies){assert.equal(b.exactNormalizedTextMatch,true);assert.equal(b.retainedExportRecordMatch,true);}
 assert.equal(native.retainedSourceRecovery.processAssembly.rawInstructionBytesReverified,false);
 const buffer=JSON.parse(fs.readFileSync(path.join(out,'buffer-native-ownership.provenance.json')));assert.equal(buffer.cliExitCode,0);assert.equal(buffer.selectedFunctions,5);assert.equal(buffer.vendorRuntimeExecuted,false);for(const f of [buffer.original,...buffer.files])assert.equal(sha(fs.readFileSync(path.resolve(root,f.path))),f.sha256);
 const origin=JSON.parse(fs.readFileSync(path.join(out,'buffer-interface-origin.provenance.json')));assert.equal(origin.cliExitCode,0);assert.equal(origin.selectedFunctions,14);for(const f of [origin.original,...origin.files])assert.equal(sha(fs.readFileSync(path.resolve(root,f.path))),f.sha256);const b=fs.readFileSync(origin.original.path);for(const slot of origin.vtableSlots)assert.equal(b.subarray(slot.fileOffset,slot.fileOffset+8).toString('hex'),slot.rawBytes);
 const frame=JSON.parse(fs.readFileSync(path.join(out,'moduleframe-image.provenance.json')));assert.equal(frame.selectedFunctions,14);assert.equal(frame.selectedCaptureExitCode,0);assert.equal(frame.initialWholeAnalysisTimedOut,true);assert.equal(frame.wholeBinaryAnalysisComplete,false);for(const f of [frame.original,...frame.files])assert.equal(sha(fs.readFileSync(path.resolve(root,f.path))),f.sha256);
 for(const [name,counts] of [['moduleframe-image-read',{selectedFunctions:6}],['sdk-image-publication',{selectedProcedures:9,selectedCaseFragments:1}]]){
  const p=JSON.parse(fs.readFileSync(path.join(out,name+'.provenance.json')));for(const [k,v] of Object.entries(counts))assert.equal(p[k],v);assert.equal(p.selectedCaptureExitCode,0);assert.equal(p.wholeBinaryAnalysisComplete,false);assert.equal(p.originalRuntimeExecuted,false);for(const f of [p.original,...p.files])assert.equal(sha(fs.readFileSync(path.resolve(root,f.path))),f.sha256);
 }
 const wire=JSON.parse(fs.readFileSync(path.join(out,'sdk-result-layout.json'))).provenance;assert.equal(wire.metadataLoadedReflectionOnly,true);assert.equal(wire.targetCodeExecuted,false);assert.equal(wire.wireLayouts.length,6);assert.equal(sha(fs.readFileSync(wire.artifact)),wire.sha256);for(const f of wire.resources)assert.equal(sha(fs.readFileSync(path.resolve(root,f.path))),f.sha256);
 const retirement=JSON.parse(fs.readFileSync(path.join(out,'current-result-retirement.review.json'))).provenance;assert.equal(retirement.sourceFiles.length,15);assert.equal(retirement.runtimeExecuted,false);for(const f of retirement.sourceFiles)assert.equal(sha(fs.readFileSync(path.resolve(root,f.path))),f.sha256);
 return{moduleFrameReaderSelectedFunctions:6,sdkPublicationSelectedProcedures:9,sdkPublicationCaseFragments:1,wireLayouts:6,currentRetirementSourceFiles:15,moduleFrameSelectedFunctions:14,wholeModuleFrameAnalysisComplete:false,newBufferInterfaceFunctions:14,newBufferNativeFunctions:5,managedBundles:bundles,methodBodies:bundles.reduce((n,b)=>n+b.methodBodies,0),retainedNativeBodies:47,nativeIdentityFilesChecked:identities.length,nativeAssemblyRawBytesReverified:false,runtimeExecuted:false};
};
