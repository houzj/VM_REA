// Find RIP-relative LEA candidates to the exact RunTimeout string in the original server PE.
// Byte matches identify candidate sites; only disassembly establishes instruction boundaries.
const fs = require('fs');
const crypto = require('crypto');
const dll = 'D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll';
const bytes = fs.readFileSync(dll);
const sha256 = crypto.createHash('sha256').update(bytes).digest('hex');
if (sha256 !== '06d86f51bebfeba357d1b79297e20a3b0a70580a40af7b77c8abdd6c38ca0624')
  throw Error(`Unexpected server PE: ${sha256}`);
const pe = bytes.readUInt32LE(0x3c), opt = pe + 24;
if (bytes.toString('ascii', pe, pe + 4) !== 'PE\0\0' || bytes.readUInt16LE(opt) !== 0x20b)
  throw Error('Expected PE32+');
const base = bytes.readBigUInt64LE(opt + 24), target = base + 0x7d2150n;
const sections = [];
for (let i = 0; i < bytes.readUInt16LE(pe + 6); i++) {
  const p = opt + bytes.readUInt16LE(pe + 20) + 40 * i;
  sections.push({name: bytes.toString('ascii', p, p + 8).replace(/\0.*$/, ''),
    rva: bytes.readUInt32LE(p + 12), size: bytes.readUInt32LE(p + 16), file: bytes.readUInt32LE(p + 20)});
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
  for (let p = s.file; p + 7 <= Math.min(s.file + s.size, bytes.length); p++) {
    if (![0x48, 0x4c].includes(bytes[p]) || bytes[p + 1] !== 0x8d || (bytes[p + 2] & 0xc7) !== 0x05) continue;
    const rva = s.rva + p - s.file;
    if (base + BigInt(rva + 7) + BigInt(bytes.readInt32LE(p + 3)) !== target) continue;
    hits.push({site: '0x' + (base + BigInt(rva)).toString(16), functionStart: owner(rva),
      bytes: bytes.subarray(p, p + 7).toString('hex')});
  }
}
const expected = ['0x1805c4878', '0x1805c81f0', '0x1805cc9af', '0x1805ce254'];
for (const site of expected)
  if (!hits.some(x => x.site === site)) throw Error(`Known RunTimeout reference absent: ${site}`);
console.log(JSON.stringify({sha256, target: '0x' + target.toString(16),
  scope: 'raw RIP-relative LEA candidates only, not exhaustive references', hits}, null, 2));
