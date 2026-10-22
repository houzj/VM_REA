// Bounded static search for direct x64 displacements of the procedure wait field.
// A byte match alone is not an instruction; inspect candidate functions' assembly.
const fs = require('fs');
const crypto = require('crypto');
const dll = process.argv.slice(2).find(arg => !arg.startsWith('--')) || 'D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll';
const bytes = fs.readFileSync(dll);
const sha256 = crypto.createHash('sha256').update(bytes).digest('hex');
if (sha256 !== '06d86f51bebfeba357d1b79297e20a3b0a70580a40af7b77c8abdd6c38ca0624')
  throw Error(`Unexpected server DLL: ${sha256}`);
const pe = bytes.readUInt32LE(0x3c), opt = pe + 24;
if (bytes.toString('ascii', pe, pe + 4) !== 'PE\0\0' || bytes.readUInt16LE(opt) !== 0x20b)
  throw Error('Expected PE32+ image');
const base = bytes.readBigUInt64LE(opt + 24), sections = [];
for (let i = 0; i < bytes.readUInt16LE(pe + 6); i++) {
  const p = opt + bytes.readUInt16LE(pe + 20) + 40 * i;
  sections.push({name: bytes.toString('ascii', p, p + 8).replace(/\0.*$/, ''), rva: bytes.readUInt32LE(p + 12), size: bytes.readUInt32LE(p + 16), file: bytes.readUInt32LE(p + 20)});
}
const exceptionRva = bytes.readUInt32LE(opt + 136), exceptionSize = bytes.readUInt32LE(opt + 140);
const exceptionSection = sections.find(s => exceptionRva >= s.rva && exceptionRva < s.rva + s.size);
if (!exceptionSection || exceptionSize % 12) throw Error('Invalid x64 unwind directory');
const exceptionFile = exceptionSection.file + exceptionRva - exceptionSection.rva;
const ranges = [];
for (let p = exceptionFile; p < exceptionFile + exceptionSize; p += 12)
  ranges.push({begin: bytes.readUInt32LE(p), end: bytes.readUInt32LE(p + 4)});
function containing(rva) {
  let lo = 0, hi = ranges.length - 1;
  while (lo <= hi) {
    const mid = (lo + hi) >> 1, x = ranges[mid];
    if (rva < x.begin) hi = mid - 1;
    else if (rva >= x.end) lo = mid + 1;
    else return '0x' + (base + BigInt(x.begin)).toString(16);
  }
  return null;
}
const pattern = Buffer.from([0xa8, 0x02, 0x00, 0x00]), hits = [];
for (const s of sections.filter(s => s.name === '.text')) {
  for (let p = s.file; p < Math.min(s.file + s.size, bytes.length) - 3; p++) {
    if (!bytes.subarray(p, p + 4).equals(pattern)) continue;
    const rva = s.rva + p - s.file;
    hits.push({va: '0x' + (base + BigInt(rva)).toString(16), prefixHex: bytes.subarray(p - 4, p).toString('hex'), functionStart: containing(rva)});
  }
}
const selectedFunctions = new Set(['0x1805dda30', '0x1805eec10', '0x18061a320', '0x18061a8b0', '0x1806a32e0']);
const selected = hits.filter(h => selectedFunctions.has(h.functionStart));
console.log(JSON.stringify({sha256, fieldOffset: '0x2a8', rawCandidateCount: hits.length,
  coverage: 'raw byte matches only; --all prints all candidates; selected functions require assembly review',
  selectedHits: process.argv.includes('--all') ? hits : selected}, null, 2));
