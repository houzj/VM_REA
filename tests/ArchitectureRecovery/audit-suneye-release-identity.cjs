'use strict';
// Read-only release inspection: embedded symbols are decompressed into memory, never loaded.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),zlib=require('zlib'),assert=require('assert/strict');
const {root}=require('./evidence-view.cjs'),{raw}=require('./verify-current-nonimage-graph-probe.cjs');
const release='D:/MyWork/SunEyeVision_V2/Release/SunEyeVision_v1.0.0/SunEyeVision',sha=(b,a='sha256')=>crypto.createHash(a).update(b).digest('hex'),rows=[];
for(const name of ['SunEyeVision.UI','SunEyeVision.Workflow','SunEyeVision.Plugin.SDK','SunEyeVision.Core']){
 const file=path.join(release,name+'.dll'),b=fs.readFileSync(file),pe=raw.peData(b),nt=b.readUInt32LE(0x3c),op=nt+24,di=op+(b.readUInt16LE(op)===0x20b?112:96),ss=op+b.readUInt16LE(nt+20),count=b.readUInt16LE(nt+6);
 const off=r=>{for(let i=0;i<count;i++){const q=ss+i*40,vr=b.readUInt32LE(q+12);if(r>=vr&&r<vr+Math.max(b.readUInt32LE(q+8),b.readUInt32LE(q+16)))return r-vr+b.readUInt32LE(q+20);}throw Error('Unbacked RVA');};
 const dr=b.readUInt32LE(di+48),dl=b.readUInt32LE(di+52);let embedded;
 for(let q=off(dr);q<off(dr)+dl;q+=28)if(b.readUInt32LE(q+12)===17){const p=b.readUInt32LE(q+24),n=b.readUInt32LE(q+16);assert.equal(b.toString('ascii',p,p+4),'MPDB');embedded=zlib.inflateRawSync(b.subarray(p+8,p+n));assert.equal(embedded.length,b.readUInt32LE(p+4));}
 assert(embedded,'Embedded PDB required');const symbols=raw.portableDocuments(embedded);assert.equal(pe.cv.guidBytes,symbols.identity.subarray(0,16).toString('hex'));assert.equal(pe.cv.stamp,symbols.identity.readUInt32LE(16));
 const documents=symbols.docs.map(d=>{const original=d.path,mapped=d.path.replace(/^\/_\//,'D:/MyWork/SunEyeVision_V2/'),exists=fs.existsSync(mapped);let actual=null;if(exists)actual=sha(fs.readFileSync(mapped),d.algorithm==='8829d00f-11b8-4213-878b-770e8597ac16'?'sha256':'sha1');return{...d,currentPath:mapped,exists,actualChecksum:actual,matched:exists&&actual===d.checksum};});
 rows.push({path:file,sha256:sha(b),mvid:raw.moduleMvid(b,pe.metadata),lastWriteTime:fs.statSync(file).mtime.toISOString(),embeddedPdbSha256:sha(embedded),embeddedPdbMatchesDll:true,documents,counts:{total:documents.length,matched:documents.filter(d=>d.matched).length,changed:documents.filter(d=>d.exists&&!d.matched).length,missing:documents.filter(d=>!d.exists).length}});
}
const result={release,inspection:'Raw PE and embedded PortablePDB, no target execution',assemblies:rows,allDocumentsMatch:rows.every(r=>r.documents.every(d=>d.matched)),runtimeExecuted:false,productModified:false};
const output=path.join(root,'analysis/global-variable-followup/suneye-release-identity-audit.json');fs.writeFileSync(output,JSON.stringify(result,null,2)+'\n');console.log(JSON.stringify(rows.map(r=>({name:path.basename(r.path),...r.counts,changedExamples:r.documents.filter(d=>d.exists&&!d.matched).slice(0,4).map(d=>d.path),missingExamples:r.documents.filter(d=>!d.exists).slice(0,2).map(d=>d.path)}))));
