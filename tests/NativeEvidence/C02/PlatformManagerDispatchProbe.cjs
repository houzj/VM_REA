// Read-only PE export check for the server module dynamically loaded by the SDK client.
const fs = require('fs');
const crypto = require('crypto');
const path = require('path');
const target = process.argv[2] || 'D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll';
const bytes = fs.readFileSync(target);
const sha256 = crypto.createHash('sha256').update(bytes).digest('hex');
if (sha256 !== '06d86f51bebfeba357d1b79297e20a3b0a70580a40af7b77c8abdd6c38ca0624')
  throw Error(`Server module identity changed: ${sha256}`);
const pe = bytes.readUInt32LE(0x3c), opt = pe + 24;
if (bytes.toString('ascii', pe, pe + 4) !== 'PE\0\0' || bytes.readUInt16LE(opt) !== 0x20b)
  throw Error('Expected PE32+ image');
const base = bytes.readBigUInt64LE(opt + 24);
const sections = [];
for (let i = 0; i < bytes.readUInt16LE(pe + 6); i++) {
  const p = opt + bytes.readUInt16LE(pe + 20) + i * 40;
  sections.push({rva: bytes.readUInt32LE(p + 12), size: bytes.readUInt32LE(p + 16), file: bytes.readUInt32LE(p + 20)});
}
function fileOffset(rva) {
  const s = sections.find(s => rva >= s.rva && rva < s.rva + s.size);
  if (!s) throw Error(`Unmapped RVA 0x${rva.toString(16)}`);
  return s.file + rva - s.rva;
}
function textAt(rva) {
  const p = fileOffset(rva), end = bytes.indexOf(0, p);
  return bytes.toString('ascii', p, end);
}
const exportRva = bytes.readUInt32LE(opt + 112), exportSize = bytes.readUInt32LE(opt + 116);
const dir = fileOffset(exportRva);
const functions = fileOffset(bytes.readUInt32LE(dir + 28));
const names = fileOffset(bytes.readUInt32LE(dir + 32));
const ordinals = fileOffset(bytes.readUInt32LE(dir + 36));
let callback = null;
for (let i = 0; i < bytes.readUInt32LE(dir + 24); i++) {
  if (textAt(bytes.readUInt32LE(names + i * 4)) !== 'PF_M_MsgDeal') continue;
  const rva = bytes.readUInt32LE(functions + bytes.readUInt16LE(ordinals + i * 2) * 4);
  callback = {rva: '0x' + rva.toString(16), va: '0x' + (base + BigInt(rva)).toString(16), forwarded: rva >= exportRva && rva < exportRva + exportSize};
}
if (!callback || callback.forwarded) throw Error(`Unexpected PF_M_MsgDeal export: ${JSON.stringify(callback)}`);
if (callback.va !== '0x1806a0250') throw Error(`Unexpected callback address: ${callback.va}`);
const root = path.resolve(__dirname, '../../..');
function checkEvidence(rel, expectedSha, needles) {
  const evidence = fs.readFileSync(path.join(root, rel));
  const actual = crypto.createHash('sha256').update(evidence).digest('hex');
  if (actual !== expectedSha.toLowerCase()) throw Error(`Analysis output changed: ${rel} ${actual}`);
  const source = evidence.toString('utf8');
  for (const needle of needles) if (!source.includes(needle)) throw Error(`Missing ${needle} in ${rel}`);
}
const audit = 'analysis/full-audit/reverse-c02-20261022/';
checkEvidence(audit + 'server-message-export/output/1806a0250.asm.txt', '4394E7DE22E263778D1A9DC779A258153A6C4C3E37A7EB02A4DD1FBB13B05629', ['CMP RBX,0x200', 'CALL 0x180602df0', 'MOV EAX,0xe0000012']);
checkEvidence(audit + 'server-message-dispatch/output/180602df0.c', 'AF99B707CB47309609C63D83992826728CD1190F27A381A6AD25A811B0517C46', ['case 0x41f:', 'FUN_18061a320(', 'case 0x420:', 'FUN_18061a8b0(']);
checkEvidence(audit + 'server-wait-handlers/output/18061a320.asm.txt', '657E5C24C5142FA923517539C7B02B406070FDE10A67924B85E7D46C3AEFB10C', ['MOV dword ptr [RSI + 0x2a8],EBX']);
checkEvidence(audit + 'server-wait-handlers/output/18061a8b0.asm.txt', '8B6D9FB79152E40F0E88BC80D9E2465D0F3C2FDA1CD07CF88C815FCCF10CD774', ['MOV dword ptr [RAX + 0x2a8],R14D']);
console.log(JSON.stringify({sha256, callback, setOpcode: 0x41f, getOpcode: 0x420, procedureFieldOffset: '0x2a8', status: 'static-evidence-consistent'}, null, 2));
