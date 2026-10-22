// Search E8 rel32 call-site bytes in the exact original server PE; disassembly must confirm hits.
const fs = require('fs');
const crypto = require('crypto');
const target = process.argv[2] || 'D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll';
const goals = (process.argv[3] || '1805dda30,1805eec10').split(',').map(x => BigInt('0x' + x.replace(/^0x/, '')));
const bytes = fs.readFileSync(target);
const sha256 = crypto.createHash('sha256').update(bytes).digest('hex');
if (sha256 !== '06d86f51bebfeba357d1b79297e20a3b0a70580a40af7b77c8abdd6c38ca0624') throw Error(`Server PE changed: ${sha256}`);
const pe = bytes.readUInt32LE(0x3c), opt = pe + 24;
if (bytes.toString('ascii', pe, pe + 4) !== 'PE\0\0' || bytes.readUInt16LE(opt) !== 0x20b) throw Error('Expected PE32+ image');
const base = bytes.readBigUInt64LE(opt + 24), sections = [];
for (let i = 0; i < bytes.readUInt16LE(pe + 6); i++) {
  const p = opt + bytes.readUInt16LE(pe + 20) + 40 * i;
  sections.push({name: bytes.toString('ascii', p, p + 8).replace(/\0.*$/, ''), rva: bytes.readUInt32LE(p + 12), size: bytes.readUInt32LE(p + 16), file: bytes.readUInt32LE(p + 20)});
}
const exceptionRva = bytes.readUInt32LE(opt + 136), exceptionSize = bytes.readUInt32LE(opt + 140);
const es = sections.find(s => exceptionRva >= s.rva && exceptionRva < s.rva + s.size);
if (!es || exceptionSize % 12) throw Error('Invalid x64 unwind directory');
const ranges = [];
for (let p = es.file + exceptionRva - es.rva; p < es.file + exceptionRva - es.rva + exceptionSize; p += 12)
  ranges.push({begin: bytes.readUInt32LE(p), end: bytes.readUInt32LE(p + 4)});
function owner(rva) {
  let lo = 0, hi = ranges.length - 1;
  while (lo <= hi) {
    const mid = (lo + hi) >> 1, x = ranges[mid];
    if (rva < x.begin) hi = mid - 1;
    else if (rva >= x.end) lo = mid + 1;
    else return '0x' + (base + BigInt(x.begin)).toString(16);
  }
  return null;
}
const hits = [];
for (const s of sections.filter(s => s.name === '.text')) {
  for (let p = s.file; p < Math.min(s.file + s.size, bytes.length) - 4; p++) {
    if (bytes[p] !== 0xe8) continue;
    const siteRva = s.rva + p - s.file;
    const destination = base + BigInt(siteRva + 5) + BigInt(bytes.readInt32LE(p + 1));
    if (!goals.includes(destination)) continue;
    hits.push({site: '0x' + (base + BigInt(siteRva)).toString(16), callee: '0x' + destination.toString(16), callerStart: owner(siteRva)});
  }
}
const expected = process.argv[3] ? [] :
  ['0x1805ab371>0x1805eec10', '0x1805b86ca>0x1805dda30', '0x180622f41>0x1805dda30'];
for (const edge of expected) {
  if (!hits.some(h => `${h.site}>${h.callee}` === edge)) throw Error(`Missing original direct call ${edge}`);
}
console.log(JSON.stringify({sha256, note: 'raw E8 matches; inspect original disassembly before attribution', hits}, null, 2));
