// Read the exact constant bytes passed as keys by PlatformManager's run-record writer.
const fs = require('fs');
const crypto = require('crypto');
const dll = 'D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll';
const bytes = fs.readFileSync(dll);
const sha256 = crypto.createHash('sha256').update(bytes).digest('hex');
if (sha256 !== '06d86f51bebfeba357d1b79297e20a3b0a70580a40af7b77c8abdd6c38ca0624')
  throw Error(`Unexpected PlatformManager DLL: ${sha256}`);
const pe = bytes.readUInt32LE(0x3c), opt = pe + 24;
if (bytes.toString('ascii', pe, pe + 4) !== 'PE\0\0' || bytes.readUInt16LE(opt) !== 0x20b)
  throw Error('Expected PE32+ image');
const base = bytes.readBigUInt64LE(opt + 24);
const sections = [];
for (let i = 0; i < bytes.readUInt16LE(pe + 6); i++) {
  const p = opt + bytes.readUInt16LE(pe + 20) + 40 * i;
  sections.push({name: bytes.toString('ascii', p, p + 8).replace(/\0.*$/, ''),
    rva: bytes.readUInt32LE(p + 12), size: bytes.readUInt32LE(p + 16), file: bytes.readUInt32LE(p + 20)});
}
const keys = new Map([[0x7d1d64, 'Index'], [0x7d1d70, 'DisplayName'],
  [0x7d2030, 'ShieldGlobalCtrl'], [0x7d2170, 'ContinueExecuteTimGap'],
  [0x7d1de8, 'Guid'], [0x7d2160, 'StopWhenNG'], [0x7d2150, 'RunTimeout']]);
for (const [rva, expected] of keys) {
  const section = sections.find(s => rva >= s.rva && rva < s.rva + s.size);
  if (!section) throw Error(`Unmapped RVA ${rva.toString(16)}`);
  const start = section.file + rva - section.rva;
  const end = bytes.indexOf(0, start);
  if (end < start || end - start > 128) throw Error(`Unterminated key ${rva.toString(16)}`);
  const raw = bytes.subarray(start, end);
  if (raw.toString('utf8') !== expected) throw Error(`Unexpected key at ${rva.toString(16)}`);
  console.log(JSON.stringify({va: '0x' + (base + BigInt(rva)).toString(16),
    section: section.name, key: raw.toString('utf8'), hex: raw.toString('hex')}));
}
