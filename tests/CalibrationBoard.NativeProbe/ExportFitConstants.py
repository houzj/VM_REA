import pathlib,struct,json
p=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll');b=p.read_bytes();pe=struct.unpack_from('<I',b,60)[0];n=struct.unpack_from('<H',b,pe+6)[0];opt=struct.unpack_from('<H',b,pe+20)[0];rva=0x16299a0
for i in range(n):
 _,vs,va,sz,off,*_=struct.unpack_from('<8sIIIIIIHHI',b,pe+24+opt+i*40)
 if va<=rva<va+max(vs,sz):
  data=b[off+rva-va:off+rva-va+192];row={'rva':hex(rva),'hex':data.hex(),'floats':list(struct.unpack('<48f',data))};print(row);pathlib.Path('analysis/calibration-board/fit-constants.json').write_text(json.dumps(row,indent=2))
