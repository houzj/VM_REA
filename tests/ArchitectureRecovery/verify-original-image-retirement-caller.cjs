'use strict';
// Independent byte reader: never invokes the producer or loads a vendor assembly.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),assert=require('assert/strict');
const root=path.resolve(__dirname,'../..');
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
const read=p=>fs.readFileSync(path.isAbsolute(p)?p:path.join(root,p));
const json=p=>JSON.parse(read(p));
function parsePE(b){
 const pe=b.readUInt32LE(0x3c);assert.equal(b.toString('ascii',pe,pe+4),'PE\0\0');
 const count=b.readUInt16LE(pe+6),opt=pe+24,magic=b.readUInt16LE(opt),dd=opt+(magic===0x20b?112:96),sections=opt+b.readUInt16LE(pe+20);
 const ranges=Array.from({length:count},(_,i)=>{const q=sections+i*40;return{rva:b.readUInt32LE(q+12),size:b.readUInt32LE(q+8),rawSize:b.readUInt32LE(q+16),offset:b.readUInt32LE(q+20)};});
 const offset=rva=>{const s=ranges.find(s=>rva>=s.rva&&rva<s.rva+Math.max(s.size,s.rawSize));assert(s,`RVA ${rva.toString(16)}`);const x=s.offset+rva-s.rva;assert(x>=0&&x<b.length);return x;};
 const cli=offset(b.readUInt32LE(dd+14*8)),base=offset(b.readUInt32LE(cli+8));assert.equal(b.readUInt32LE(base),0x424a5342);
 let q=(base+16+b.readUInt32LE(base+12)+3)&~3;const n=b.readUInt16LE(q+2),streams={};q+=4;
 for(let i=0;i<n;i++){const rel=b.readUInt32LE(q),size=b.readUInt32LE(q+4),start=q+8;let end=start;while(b[end])end++;streams[b.toString('ascii',start,end)]={offset:base+rel,size};q=(end+4)&~3;}
 const t=streams['#~']||streams['#-'],heap=b[t.offset+6],valid=b.readBigUInt64LE(t.offset+8),rows=Array(64).fill(0);q=t.offset+24;
 for(let i=0;i<64;i++)if(valid&(1n<<BigInt(i))){rows[i]=b.readUInt32LE(q);q+=4;}
 const strSize=heap&1?4:2,guidSize=heap&2?4:2,blobSize=heap&4?4:2,idx=i=>rows[i]>=65536?4:2;
 const coded=(bits,ts)=>Math.max(...ts.map(i=>rows[i]))>=2**(16-bits)?4:2;
 const resolution=coded(2,[0,26,35,1]),typeRef=coded(2,[2,1,27]),parent=coded(3,[2,1,26,6,27]);
 const sizes=[2+strSize+3*guidSize,resolution+2*strSize,4+2*strSize+typeRef+idx(4)+idx(6),idx(4),2+strSize+blobSize,idx(6),8+strSize+blobSize+idx(8),idx(8),4+strSize,idx(2)+typeRef,parent+strSize+blobSize],starts=[];
 for(let i=0;i<sizes.length;i++){starts[i]=q;q+=rows[i]*sizes[i];}
 const num=(p,n)=>n===2?b.readUInt16LE(p):b.readUInt32LE(p);
 const string=id=>{const start=streams['#Strings'].offset+id;let end=start;while(b[end])end++;return b.toString('utf8',start,end);};
 const text=(p,n=strSize)=>string(num(p,n));
 const module=starts[0],gid=num(module+2+strSize,guidSize),g=Buffer.from(b.subarray(streams['#GUID'].offset+(gid-1)*16,streams['#GUID'].offset+gid*16));
 const mvid=g.subarray(0,4).reverse().toString('hex')+'-'+g.subarray(4,6).reverse().toString('hex')+'-'+g.subarray(6,8).reverse().toString('hex')+'-'+g.subarray(8,10).toString('hex')+'-'+g.subarray(10).toString('hex');
 function typeDef(i){assert(i>=1&&i<=rows[2]);const p=starts[2]+(i-1)*sizes[2];return{name:text(p+4),namespace:text(p+4+strSize),firstMethod:num(p+4+2*strSize+typeRef+idx(4),idx(6))};}
 function method(token){assert.equal(token>>>24,6);const i=token&0xffffff;assert(i>=1&&i<=rows[6]);const p=starts[6]+(i-1)*sizes[6],rva=b.readUInt32LE(p);let owner;
  for(let j=1;j<=rows[2];j++){const td=typeDef(j),next=j===rows[2]?rows[6]+1:typeDef(j+1).firstMethod;if(i>=td.firstMethod&&i<next){owner=td;break;}}
  assert(owner);const header=offset(rva),first=b[header];let body,size;
  if((first&3)===2){body=header+1;size=first>>2;}else{assert.equal(first&3,3);body=header+(b.readUInt16LE(header)>>12)*4;size=b.readUInt32LE(header+4);}
  assert(body+size<=b.length);return{type:[owner.namespace,owner.name].filter(Boolean).join('.'),member:text(p+8),rva,headerOffset:header,ilFileOffset:body,il:b.subarray(body,body+size)};
 }
 function memberRef(token){assert.equal(token>>>24,10);const i=token&0xffffff;assert(i>=1&&i<=rows[10]);const p=starts[10]+(i-1)*sizes[10],raw=num(p,parent),tag=raw&7,rid=raw>>>3;assert.equal(tag,1,'Target must be TypeRef');const tr=starts[1]+(rid-1)*sizes[1],scope=num(tr,resolution);return{type:[text(tr+resolution+strSize),text(tr+resolution)].filter(Boolean).join('.'),member:text(p+parent),typeRefToken:'0x'+(0x01000000+rid).toString(16).padStart(8,'0'),resolutionScopeToken:'0x'+(([0,26,35,1][scope&3]<<24)+(scope>>>2)).toString(16).padStart(8,'0')};}
 const resourceRva=b.readUInt32LE(cli+24),resourceSize=b.readUInt32LE(cli+28),resourceOffset=resourceRva?offset(resourceRva):0;
 return{mvid,method,memberRef,resourceOffset,resourceSize,resourceRva};
}
function verify(){
 const modelPath='docs/architecture-recovery/original-image-retirement-evidence.json',auditPath='docs/architecture-recovery/original-image-retirement-caller-audit.json',scanPath='docs/architecture-recovery/image-retirement.resources/caller-scan.json';
 const model=json(modelPath),audit=json(auditPath),scan=json(scanPath);assert.deepEqual(model.provenance.callerAudit,audit);assert.equal(sha(read(scanPath)),audit.scanReport.sha256);assert.equal(scan.artifacts.length,4);
 const artifacts=[],bodies=[],resources=[];let release;
 for(const a of scan.artifacts){const b=read(a.path);assert.equal(sha(b),a.sha256,a.path);assert.equal(b.length,a.byteLength);const pe=parsePE(b);assert.equal(pe.mvid,a.mvid);
  artifacts.push({path:a.path,sha256:sha(b),byteLength:b.length,mvid:pe.mvid});
  for(const selected of a.selectedBodies){const actual=pe.method(Number(selected.token));assert.equal(actual.type,selected.type);assert.equal(actual.member,selected.member);assert.equal(actual.il.toString('hex'),selected.ilBytes);assert.equal(sha(actual.il),selected.ilSha256);
   bodies.push({artifact:a.path,type:actual.type,member:actual.member,token:selected.token,rva:actual.rva,headerOffset:actual.headerOffset,ilFileOffset:actual.ilFileOffset,ilByteLength:actual.il.length,ilSha256:sha(actual.il)});
   if(selected.token==='0x06000038'&&a.path.endsWith('VMControls.WPF.Release.dll')){assert.equal(actual.il[40],0x28);assert.equal(actual.il.readUInt32LE(41),0x0a000086);const target=pe.memberRef(0x0a000086);assert.equal(target.type,'VMControls.WPF.MainViewControl');assert.equal(target.member,'Dispose');release={callerType:actual.type,callerMember:actual.member,methodToken:selected.token,ilOffset:40,opcode:'call',targetToken:'0x0a000086',...target};}
  }
  for(const r of a.selectedRawResources){const capture=read(r.path);assert.equal(sha(capture),r.sha256);assert.equal(capture.length,r.byteLength);const original=b.subarray(pe.resourceOffset,pe.resourceOffset+pe.resourceSize),within=original.indexOf(capture);assert(within>=0,`raw BAML absent: ${r.path}`);assert.equal(original.indexOf(capture,within+1),-1,'Ambiguous raw resource source range');
   resources.push({...r,artifact:a.path,sourceFileOffset:pe.resourceOffset+within,sourceResourceRva:pe.resourceRva+within,sourceRangeLength:capture.length,rawBytesEqual:true,nameAssociation:'Retained scanner name; verifier independently proves exact unique raw byte range within original CLR resource directory, not a fresh BAML semantic decode.'});
  }
 }
 assert.equal(bodies.length,5);assert.equal(resources.length,4);assert(release);assert.equal(audit.actualDirectCaller.methodToken,release.methodToken);assert.equal(audit.actualDirectCaller.targetToken,release.targetToken);assert.equal(audit.actualDirectCaller.ilOffset,release.ilOffset);
 const refs=model.facts.flatMap(f=>f.refs);for(const r of refs){const b=read(r.path);assert.equal(sha(b),r.sha256,r.path);assert(Number.isInteger(r.start)&&r.start>=1&&r.end>=r.start);const lines=b.toString('utf8').replace(/\r\n/g,'\n').split('\n');assert.equal(lines.slice(r.start-1,r.end).join('\n'),r.excerpt,`${r.path}:${r.start}`);}
 for(const f of [...model.provenance.sourceFiles,...audit.files]){const b=read(f.path);assert.equal(sha(b),f.sha256,f.path);if(f.byteLength!==undefined)assert.equal(b.length,f.byteLength);}
 assert.equal(audit.complete,false);assert.equal(scan.completeAllApplicationCallers,false);assert.equal(model.provenance.outerCallerClosed,false);assert.equal(model.provenance.fullOriginalChainClosed,false);assert.equal(audit.originalAssemblyLoaded,false);assert.equal(audit.originalRuntimeExecuted,false);assert.equal(scan.originalAssemblyLoaded,false);assert.equal(scan.originalCodeExecuted,false);assert.equal(audit.initialThreePeScan.methodBodies,5630);assert.equal(audit.initialThreePeScan.directReferences,0);assert.equal(audit.initialThreePeScan.interfaceCandidates,224);
 assert.equal(audit.actualCallerFound,true);assert(audit.unknowns.some(s=>s.includes('更外层')));assert(model.facts.find(f=>f.id==='OR07').unknowns.some(s=>s.includes('尚未定位')));
 const inputs=[modelPath,auditPath,scanPath,'tests/ArchitectureRecovery/verify-original-image-retirement-caller.cjs'].map(p=>({path:p,sha256:sha(read(p))}));
 const report={schemaVersion:1,passed:true,inputs,artifacts,selectedBodies:bodies,rawResources:resources,directCall:release,sourceRefsChecked:refs.length,producerExecuted:false,scannerExecuted:false,originalAssemblyLoaded:false,originalRuntimeExecuted:false,browserRendered:false,fullCallerScanIndependentlyRepeated:false,outerCallerClosed:false,fullOriginalChainClosed:false,implementationCredit:0,limitations:['Independently validates four PE identities, five selected method bodies and the actual Release Dispose call operand; does not re-enumerate all method calls or re-establish the historical 5630/224 counts.','Four BAML captures match unique byte ranges inside original CLR resource directories; resource key association is retained from frozen scanner and BAML semantics/live binding activation are not newly decoded.','Release.Dispose outer callers, interface dispatch, other IsDisposeNotify consumers and final pixel readers remain unknown.']};
 fs.writeFileSync(path.join(root,'docs/architecture-recovery/original-image-retirement-caller-review.json'),JSON.stringify(report,null,2)+'\n');return report;
}
module.exports=verify;
if(require.main===module){const r=verify();console.log(JSON.stringify({passed:r.passed,artifacts:r.artifacts.length,selectedBodies:r.selectedBodies.length,rawResources:r.rawResources.length,refs:r.sourceRefsChecked,outerCallerClosed:false,originalRuntimeExecuted:false}));}
