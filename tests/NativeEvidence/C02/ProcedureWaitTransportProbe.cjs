// Static evidence check only: reads the shipped DLL bytes; never loads the DLL.
const fs = require('fs');
const crypto = require('crypto');
const path = require('path');

const root = path.resolve(__dirname, '../../..');
const dll = process.argv[2] || 'D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll';
const bytes = fs.readFileSync(dll);
const hash = crypto.createHash('sha256').update(bytes).digest('hex');
if (hash !== 'b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1')
  throw Error(`Original DLL identity changed: ${hash}`);

const pe = bytes.readUInt32LE(0x3c);
if (bytes.toString('ascii', pe, pe + 4) !== 'PE\0\0') throw Error('Not a PE image');
const opt = pe + 24;
if (bytes.readUInt16LE(opt) !== 0x20b) throw Error('Expected PE32+ image');
const imageBase = bytes.readBigUInt64LE(opt + 24);
const sections = [];
for (let i = 0; i < bytes.readUInt16LE(pe + 6); i++) {
  const p = opt + bytes.readUInt16LE(pe + 20) + 40 * i;
  sections.push({ rva: bytes.readUInt32LE(p + 12), size: bytes.readUInt32LE(p + 16), offset: bytes.readUInt32LE(p + 20) });
}
function fileOffset(rva) {
  const s = sections.find(s => rva >= s.rva && rva < s.rva + s.size);
  if (!s) throw Error(`RVA 0x${rva.toString(16)} is not file backed`);
  return s.offset + rva - s.rva;
}
function cString(rva, maxLength = 4096) {
  const start = fileOffset(rva);
  const end = bytes.indexOf(0, start);
  if (end < 0 || end - start > maxLength) throw Error(`Invalid string at RVA 0x${rva.toString(16)}`);
  return bytes.toString('ascii', start, end);
}
const exportRva = bytes.readUInt32LE(opt + 112);
const table = fileOffset(exportRva);
const funcs = fileOffset(bytes.readUInt32LE(table + 28));
const names = fileOffset(bytes.readUInt32LE(table + 32));
const ordinals = fileOffset(bytes.readUInt32LE(table + 36));
const found = {};
for (let i = 0; i < bytes.readUInt32LE(table + 24); i++) {
  const name = cString(bytes.readUInt32LE(names + 4 * i));
  if (name === 'IMVS_SetProcedureWaitTime' || name === 'IMVS_GetProcedureWaitTime') {
    const rva = bytes.readUInt32LE(funcs + 4 * bytes.readUInt16LE(ordinals + 2 * i));
    found[name] = '0x' + (imageBase + BigInt(rva)).toString(16);
  }
}
if (found.IMVS_SetProcedureWaitTime !== '0x18064ed60' || found.IMVS_GetProcedureWaitTime !== '0x18064ee70')
  throw Error(`Procedure wait exports moved: ${JSON.stringify(found)}`);

const strings = {
  setCommand: cString(0xac9560, 128),
  getCommand: cString(0xac5830, 128),
  responseScan: cString(0xac5208, 128),
  responseOk: cString(0xac5258, 128)
};
if (strings.setCommand !== '%d-%u-%u-%u' || strings.getCommand !== '%d-%u-%u' || strings.responseScan !== '%[^*]*%d' || strings.responseOk !== 'OK')
  throw Error(`Procedure wait command strings changed: ${JSON.stringify(strings)}`);

const evidence = path.join(root, 'analysis/full-audit/reverse-c02-20261022/native-wait');
const setAsm = fs.readFileSync(path.join(evidence, 'output/18064ed60.asm.txt'), 'utf8');
const setBody = fs.readFileSync(path.join(root, 'analysis/full-audit/reverse-c02-20261022/native-wait-callees/output/1806abda0.c'), 'utf8');
const getBody = fs.readFileSync(path.join(root, 'analysis/full-audit/reverse-c02-20261022/native-wait-callees/output/1806abf40.c'), 'utf8');
const transportAsm = fs.readFileSync(path.join(root, 'analysis/full-audit/reverse-c02-20261022/native-transport/output/1806c00d0.asm.txt'), 'utf8');
const retryRoot = path.join(root, 'analysis/full-audit/reverse-c02-20261022/native-transport-fallback/output');
const retryAsm = fs.readFileSync(path.join(retryRoot, '180662a70.asm.txt'), 'utf8');
const queryAsm = fs.readFileSync(path.join(retryRoot, '1806c20b0.asm.txt'), 'utf8');
for (const needle of ['MOV EBP,R8D', 'MOV R14D,EDX', 'MOV R8D,EBP', 'MOV EDX,R14D', 'CALL 0x1806abda0'])
  if (!setAsm.includes(needle)) throw Error(`Missing register-flow evidence: ${needle}`);
for (const needle of ['param_2 < 1', '0x41f,param_2,8,param_3', 'FUN_1806c00d0(param_1)'])
  if (!setBody.includes(needle)) throw Error(`Missing set transport evidence: ${needle}`);
for (const needle of ['param_3 == (undefined4 *)0x0', 'uStack_4d8 = 8', 'FUN_1806c1f30(param_1,acStack_438', '*param_3 = uVar6'])
  if (!getBody.includes(needle)) throw Error(`Missing get transport evidence: ${needle}`);
for (const needle of ['MOV RAX,qword ptr [R15 + 0x2398]', 'CALL RAX', 'MOV EDI,EAX', 'CALL 0x180662a70', 'MOV EAX,EDI'])
  if (!transportAsm.includes(needle)) throw Error(`Missing callback or return-code evidence: ${needle}`);
for (const needle of ['MOV R10,qword ptr [RCX + 0x2398]', 'MOV R8,R11', 'MOV RDX,RBX', 'MOV RCX,RDI', 'JMP R10', 'MOV EAX,0xe0000010'])
  if (!retryAsm.includes(needle)) throw Error(`Missing resend trampoline evidence: ${needle}`);
for (const needle of ['MOV R8D,0x2000', 'MOV RAX,qword ptr [R14 + 0x2398]', 'MOV R9D,0x1fff', 'CALL RAX', 'CMP EBX,0xe0000012', 'MOV RDI,RAX', 'MOV EBX,EAX'])
  if (!queryAsm.includes(needle)) throw Error(`Missing query-buffer evidence: ${needle}`);

console.log(JSON.stringify({originalSha256: hash, exports: found, strings, setOpcode: 0x41f, getOpcode: 0x420, fieldCode: 8, retryTransport: 'same +0x2398 callback', status: 'static-evidence-consistent'}, null, 2));
