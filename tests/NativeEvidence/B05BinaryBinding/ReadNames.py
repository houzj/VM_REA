"""Read constants addressed by verified initializer instructions; never load target."""
from pathlib import Path
import hashlib,json,struct
ROOT=Path(__file__).resolve().parents[3]; OUT=ROOT/'analysis/full-audit/batch9-20261019/B05'
P=Path('D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll')
b=P.read_bytes(); assert hashlib.sha256(b).hexdigest()=='b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1'
pe=struct.unpack_from('<I',b,0x3c)[0]; opt=pe+24; sz=struct.unpack_from('<H',b,pe+20)[0]; n=struct.unpack_from('<H',b,pe+6)[0]
rows=[]
for rv in (0xac4198,0xac41c8,0xac0aa8,0xac40d0,0xab4c10,0xaabf90,0xac4000):
 for i in range(n):
  vs,va,rs,rp=struct.unpack_from('<IIII',b,opt+sz+40*i+8)
  if va<=rv<va+rs: at=rp+rv-va; break
 else: raise ValueError(rv)
 wide=rv in (0xac0aa8,0xac40d0,0xab4c10,0xaabf90)
 if wide:
  end=at
  while b[end:end+2]!=b'\0\0': end+=2
  raw=b[at:end]; value=raw.decode('utf-16-le'); encoding='utf-16-le'
 else:
  raw=b[at:b.index(b'\0',at)]; value=raw.decode('ascii'); encoding='ascii'
 rows.append({'rva':hex(rv),'fileOffset':hex(at),'hex':raw.hex(),'encoding':encoding,'value':value,'authority':'literal at direct LEA target; no runtime resolution observed'})
(OUT/'binding-names.json').write_text(json.dumps(rows,indent=2)+'\n',encoding='utf-8'); print(json.dumps(rows,indent=2))
