// Bounded displacement scan of the exact PlatformManager PE; hits require disassembly review.
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
const base = bytes.readBigUInt64LE(opt + 24), sections = [];
for (let i = 0; i < bytes.readUInt16LE(pe + 6); i++) {
  const p = opt + bytes.readUInt16LE(pe + 20) + 40 * i;
  sections.push({name: bytes.toString('ascii', p, p + 8).replace(/\0.*$/, ''),
    rva: bytes.readUInt32LE(p + 12), size: bytes.readUInt32LE(p + 16), file: bytes.readUInt32LE(p + 20)});
}
function fileOffset(rva) {
  const s = sections.find(x => rva >= x.rva && rva < x.rva + x.size);
  if (!s) throw Error(`Unmapped RVA 0x${rva.toString(16)}`);
  return s.file + rva - s.rva;
}
const exportRva = bytes.readUInt32LE(opt + 112), exportSize = bytes.readUInt32LE(opt + 116);
const directory = fileOffset(exportRva), functions = fileOffset(bytes.readUInt32LE(directory + 28));
const names = fileOffset(bytes.readUInt32LE(directory + 32));
const ordinals = fileOffset(bytes.readUInt32LE(directory + 36));
const targetRvas = new Set([0x631510, 0x6315f0, 0x6203a0]);
const matchingExports = [];
for (let i = 0; i < bytes.readUInt32LE(directory + 24); i++) {
  const rva = bytes.readUInt32LE(functions + bytes.readUInt16LE(ordinals + 2 * i) * 4);
  if (!targetRvas.has(rva)) continue;
  const p = fileOffset(bytes.readUInt32LE(names + 4 * i));
  matchingExports.push({rva: '0x' + rva.toString(16), name: bytes.toString('ascii', p, bytes.indexOf(0, p)),
    forwarded: rva >= exportRva && rva < exportRva + exportSize});
}
const setterVa = base + 0x631510n, setterPointers = [];
for (const s of sections.filter(s => s.name !== '.text'))
  for (let p = s.file; p + 8 <= Math.min(s.file + s.size, bytes.length); p += 1)
    if (bytes.readBigUInt64LE(p) === setterVa)
      setterPointers.push({va: '0x' + (base + BigInt(s.rva + p - s.file)).toString(16), section: s.name});
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
const pattern = Buffer.from([0x48, 0xfc, 0x07, 0x00]), hits = [];
for (const s of sections.filter(s => s.name === '.text'))
  for (let p = s.file; p + 4 <= Math.min(s.file + s.size, bytes.length); p++) {
    if (!bytes.subarray(p, p + 4).equals(pattern)) continue;
    const rva = s.rva + p - s.file;
    hits.push({va: '0x' + (base + BigInt(rva)).toString(16), functionStart: owner(rva),
      precedingHex: bytes.subarray(p - 4, p).toString('hex')});
  }
if (!hits.some(x => x.functionStart === '0x1806203a0')) throw Error('Known callback use absent');
console.log(JSON.stringify({sha256, rawDisplacement: '0x7fc48',
  scope: 'raw bytes only; disassembly required; indirect writes or copies not covered', matchingExports, setterPointers, hits}, null, 2));
