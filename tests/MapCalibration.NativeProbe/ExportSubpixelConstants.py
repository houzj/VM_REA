import pathlib,struct,json,re
root=pathlib.Path(__file__).resolve().parents[2];p=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll');b=p.read_bytes();pe=struct.unpack_from('<I',b,60)[0];n=struct.unpack_from('<H',b,pe+6)[0];opt=struct.unpack_from('<H',b,pe+20)[0]
sections=[struct.unpack_from('<IIII',b,pe+24+opt+i*40+8) for i in range(n)]
addresses=set()
for name in ['1808b6f30','1808babd0','1808b5330']:
 addresses.update(int(s,16) for s in re.findall(r'(?:DAT_|UNK_)(18[0-9a-f]{7})',(root/'analysis/map-calibration/ghidra/core'/f'{name}.c').read_text()))
for name in ['1808b47c0','1808bb3a0','1808bcff0','180873de0']:
 addresses.update(int(s,16) for s in re.findall(r'(?:DAT_|UNK_)(18[0-9a-f]{7})',(root/'analysis/full-audit/n-image-calibration/core-native'/f'{name}.c').read_text()))
for name in ['1808bcba0','1808c1780','1808bdb40','1808c1340','1808c0fa0','1808bdef0','1808c0880','1808bf960','1808c00f0']:
 addresses.update(int(s,16) for s in re.findall(r'(?:DAT_|UNK_)(18[0-9a-f]{7})',(root/'analysis/map-calibration/ghidra/core'/f'{name}.c').read_text()))
rows=[]
for address in sorted(addresses):
 rva=address-0x180000000
 for sz,va,rawsz,raw in sections:
  if va<=rva<va+max(sz,rawsz):
   token=b[raw+rva-va:raw+rva-va+8];rows.append(dict(address=f'{address:x}',bytes=token.hex(),float=struct.unpack('<f',token[:4])[0],double=struct.unpack('<d',token)[0]));break
out=root/'analysis/map-calibration/image-candidate-trace/constants.json';out.write_text(json.dumps(rows,indent=2));print(f'{len(rows)} source constants')




