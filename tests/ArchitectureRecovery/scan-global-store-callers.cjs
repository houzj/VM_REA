'use strict';
// Candidate enumeration only. Raw E8 matches are not an instruction decoder.
const fs=require('fs'),path=require('path'),inspect=require('./native-pe-inspection.cjs');
const {root}=require('./evidence-view.cjs'),file='D:/Program Files/VisionMaster4.4.0/Applications/ModuleProxy/x64/ModuleFrame.dll',pe=inspect(file),b=fs.readFileSync(file),base=BigInt(pe.imageBase),pd=pe.sections.find(s=>s.name==='.pdata'),functions=[];
for(let p=pd.rawOffset;p<pd.rawOffset+pd.rawSize;p+=12){const start=b.readUInt32LE(p),end=b.readUInt32LE(p+4);if(start&&end>start)functions.push({start,end});}
const requested=process.argv.slice(2).map(s=>Number.parseInt(s,16));
const targets=new Set(requested.length?requested:[0x8e910,0x8e6c0,0xc8f10]),rows=[];
for(const s of pe.sections.filter(s=>s.name==='.text'))for(let p=s.rawOffset;p<s.rawOffset+s.rawSize-5;p++)if(b[p]===0xe8){const r=s.rva+p-s.rawOffset,t=r+5+b.readInt32LE(p+1);if(targets.has(t)){const owner=functions.find(f=>r>=f.start&&r<f.end);rows.push({call:'0x'+(base+BigInt(r)).toString(16),target:'0x'+(base+BigInt(t)).toString(16),owner:owner?'0x'+(base+BigInt(owner.start)).toString(16):null,exports:pe.exports.filter(e=>Number(BigInt(e.va)-base)===owner?.start).map(e=>e.name)});}}
const result={sha256:pe.sha256,scope:'Raw byte candidates requiring instruction and function validation; not confirmed calls',rows};fs.writeFileSync(path.join(root,'analysis/global-variable-followup/'+(requested.length?'consumer-call-candidates':'store-call-candidates')+'.json'),JSON.stringify(result,null,2)+'\n');console.log(JSON.stringify(requested.length?rows:rows.filter(r=>r.target==='0x18008e910')));
