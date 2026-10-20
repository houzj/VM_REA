import pathlib,struct,json
p=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll');b=p.read_bytes();pe=struct.unpack_from('<I',b,60)[0];n=struct.unpack_from('<H',b,pe+6)[0];opt=struct.unpack_from('<H',b,pe+20)[0];result=[]
for rva in [0x162b60c,0x162b610,0x1628560,0x1628564]:
 for i in range(n):
  o=pe+24+opt+i*40;sz,va,rawsz,raw=struct.unpack_from('<IIII',b,o+8)
  if va<=rva<va+max(sz,rawsz):
   token=b[raw+rva-va:raw+rva-va+4];result.append(dict(va=hex(0x180000000+rva),bytes=token.hex(),float=struct.unpack('<f',token)[0]));break
out=pathlib.Path('analysis/map-calibration/native/constants.json');out.write_text(json.dumps(result,indent=2));print(out.read_text())
