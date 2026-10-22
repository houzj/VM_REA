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
checkEvidence(audit + 'server-wait-field-consumers/output/1805dda30.asm.txt', '6B525056585DEC891011B4F4937C799137E4F86EDFB02C2FA754CAF303ECCCA0', ['MOV ECX,dword ptr [RAX + 0x2a8]', 'CALL 0x1805d2890']);
checkEvidence(audit + 'server-wait-field-consumers/output/1805eec10.asm.txt', '9DEE5580FE02BE7AE65921BC6B491F3777AC361088BBBEF953D3ECD57E8BA33B', ['MOV EAX,dword ptr [R14 + 0x2a8]', 'MOV dword ptr [RDI + 0x10],EAX']);
checkEvidence(audit + 'server-wait-read-callers/output/1805aa6f0.asm.txt', 'CAD64373914E1621CB69DEF8116ACEC508B5D7F3919A4F101BF53C56B9D47F51', ['CALL 0x1805eec10', 'MOV R9D,0x7fe', 'CALL 0x1806203a0', 'CALL 0x1805cd170']);
checkEvidence(audit + 'server-run-record-consumer/output/1805cd170.asm.txt', '77B92B5BDFF0F14D7DA4DDE4D7EA3C1743C61E79841B259678554464C5B590B5', ['CALL 0x1805cce90', 'CALL qword ptr [RAX + 0x80]']);
checkEvidence(audit + 'server-run-record-next/output/1805cce90.asm.txt', 'E5461549E4511BF48465C3D9B0DAEAB5F6A9E2CF1306EE5F8DF7FD9354185908', ['MOV RSI,R8', 'MOV R8,RSI', 'CALL 0x1805cc780']);
checkEvidence(audit + 'server-run-record-field/output/1805cc780.asm.txt', '090A000973E168DDCC0F9298106FCDA7BA79C994FE8E2390ED2402FC19402121', ['MOV R8D,dword ptr [R14 + 0x10]', 'LEA RDX,[0x1807d2150]', 'CALL 0x1806aca10']);
checkEvidence(audit + 'server-run-timeout-refs/output/1805c43d0.asm.txt', '061C4765B1A84BDC57FF4FFC12369AB2D851929273BB52266AFFCFF5AA55C123', ['LEA RDX,[0x1807d2150]', 'MOV dword ptr [RBP + -0x69],EAX', 'CALL 0x1805d2890']);
checkEvidence(audit + 'server-run-timeout-refs/output/1805c7ea0.asm.txt', '581907C1C867EDD389147BF1A3027C493C50CFE0C7061EF84F1712959BB7AF12', ['MOV R8D,dword ptr [RBX + 0x20]', 'LEA RDX,[0x1807d2150]', 'CALL 0x1806aca10']);
checkEvidence(audit + 'server-run-timeout-refs/output/1805ce0e8.asm.txt', '24B12C0943799DC57204D2C70E932C9CF59A93B818E0B3A194A131959AD03723', ['LEA RDX,[0x1807d2150]', 'MOV dword ptr [R14 + 0x10],0x0', 'MOV dword ptr [R14 + 0x10],EAX']);
checkEvidence(audit + 'server-run-timeout-callers/output/1805c5750.asm.txt', '58ADA515BCF67487E3B0B1997678674E6B783F35A73F24F876C3FB648938946F', ['CALL 0x1805c43d0', 'TEST EAX,EAX', 'CALL 0x1805c53b0']);
checkEvidence(audit + 'server-run-timeout-callers/output/1805c5f40.asm.txt', '60745EBD998C4F34D4AEF07B6C186D3CBBEDD1093BEB7EF7E98196DFEA65EF9D', ['CALL 0x1805c7ea0', 'TEST EAX,EAX', 'CALL 0x1805c8fd0']);
checkEvidence(audit + 'server-run-timeout-record-copy/output/1805c2570.asm.txt', '426F3AC967C842C69A12ADEC86CEE6AD6AEEDD70374D7B6B710EBDA8ADE30559', ['MOV dword ptr [RCX + -0x8],EAX', 'CALL 0x180577f30']);
checkEvidence(audit + 'server-run-message-bridge/output/1806203a0.asm.txt', 'A56BDD8E638E12879E5DE03FE14A2C299227D487271CF509E8EFA0D9CAEBDD6A', ['CALL qword ptr [RDI + 0x7fc48]', 'MOV EDI,EAX', 'XOR EAX,EAX', 'MOV EAX,0xe0000001']);
checkEvidence(audit + 'server-run-callback-binding/output/180631510.asm.txt', '8A9C2BC60ADA4879BC5B6352374A30F4F0FBC85D8A8C598A47826B0EF98FF643', ['MOV qword ptr [RDI + 0x7fc48],RBP', 'MOV qword ptr [RDI + 0x7fc50],RSI']);
console.log(JSON.stringify({sha256, callback, setOpcode: 0x41f, getOpcode: 0x420, procedureFieldOffset: '0x2a8', status: 'static-evidence-consistent'}, null, 2));
