// Bounded static scan of the original x64 PE. Candidate addresses need disassembly review.
const fs = require('fs');
const crypto = require('crypto');
const dll = process.argv[2] || 'D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll';
const data = fs.readFileSync(dll);
const sha256 = crypto.createHash('sha256').update(data).digest('hex');
if (sha256 !== 'b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1')
  throw Error(`Unexpected original DLL: ${sha256}`);
const pe = data.readUInt32LE(0x3c), opt = pe + 24;
if (data.toString('ascii', pe, pe + 4) !== 'PE\0\0' || data.readUInt16LE(opt) !== 0x20b)
  throw Error('Expected PE32+ image');
const base = data.readBigUInt64LE(opt + 24);
const sections = [];
for (let n = 0; n < data.readUInt16LE(pe + 6); n++) {
  const h = opt + data.readUInt16LE(pe + 20) + 40 * n;
  sections.push({name: data.toString('ascii', h, h + 8).replace(/\0.*$/, ''), rva: data.readUInt32LE(h + 12), size: data.readUInt32LE(h + 16), file: data.readUInt32LE(h + 20)});
}
const slotBytes = Buffer.from([0x98, 0x23, 0x00, 0x00]);
function offsetFor(rva) {
  const s = sections.find(s => rva >= s.rva && rva < s.rva + s.size);
  if (!s) throw Error(`Not file backed: 0x${rva.toString(16)}`);
  return s.file + rva - s.rva;
}
function asciiAt(rva) {
  const start = offsetFor(rva), end = data.indexOf(0, start);
  return data.toString('ascii', start, end);
}
function utf16At(rva) {
  const start = offsetFor(rva); let end = start;
  while (end + 1 < data.length && data.readUInt16LE(end) !== 0 && end - start < 512) end += 2;
  return data.toString('utf16le', start, end);
}
const hits = [];
const exceptionRva = data.readUInt32LE(opt + 112 + 3 * 8);
const exceptionSize = data.readUInt32LE(opt + 112 + 3 * 8 + 4);
const exceptionSection = sections.find(s => exceptionRva >= s.rva && exceptionRva < s.rva + s.size);
if (!exceptionSection || exceptionSize % 12) throw Error('Invalid x64 exception directory');
const exceptionFile = exceptionSection.file + exceptionRva - exceptionSection.rva;
function unwindRange(rva) {
  for (let p = exceptionFile; p < exceptionFile + exceptionSize; p += 12) {
    const begin = data.readUInt32LE(p), end = data.readUInt32LE(p + 4);
    if (begin <= rva && rva < end) return {start: '0x' + (base + BigInt(begin)).toString(16), end: '0x' + (base + BigInt(end)).toString(16)};
  }
  return null;
}
for (const s of sections.filter(s => s.name === '.text')) {
  const end = Math.min(s.file + s.size, data.length);
  for (let p = s.file; p <= end - slotBytes.length; p++) {
    if (!data.subarray(p, p + slotBytes.length).equals(slotBytes)) continue;
    const rva = s.rva + p - s.file;
    hits.push({va: '0x' + (base + BigInt(rva)).toString(16), precedingHex: data.subarray(Math.max(s.file, p - 4), p).toString('hex'), unwindRange: unwindRange(rva)});
  }
}
const loaderLiterals = {
  moduleNameA: utf16At(0xaabf90), moduleNameB: utf16At(0xab4c10),
  moduleNameC: utf16At(0xac0aa8), moduleNameD: utf16At(0xac40d0),
  callbackExport: asciiAt(0xac41c8)
};
console.log(JSON.stringify({sha256, searched: '.text exact displacement 0x2398', candidateCount: hits.length, hits, loaderLiterals}, null, 2));
