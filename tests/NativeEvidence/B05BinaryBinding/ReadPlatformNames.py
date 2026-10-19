"""Read constants addressed by verified initializer instructions; never load target."""
from pathlib import Path
import hashlib,json,struct
ROOT=Path(__file__).resolve().parents[3]; OUT=ROOT/'analysis/full-audit/batch9-20261019/B05'
P=Path('D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll')
b=P.read_bytes(); assert hashlib.sha256(b).hexdigest()=='06d86f51bebfeba357d1b79297e20a3b0a70580a40af7b77c8abdd6c38ca0624'
pe=struct.unpack_from('<I',b,0x3c)[0]; opt=pe+24; sz=struct.unpack_from('<H',b,pe+20)[0]; n=struct.unpack_from('<H',b,pe+6)[0]
rows=[]
for rv in (0x7e3ba8,0x7cc60c,0x7dbd10,0x7e3bc8):
 for i in range(n):
  vs,va,rs,rp=struct.unpack_from('<IIII',b,opt+sz+40*i+8)
  if va<=rv<va+rs: at=rp+rv-va; break
 else: raise ValueError(rv)
 wide=False
 if wide:
  end=at
  while b[end:end+2]!=b'\0\0': end+=2
  raw=b[at:end]; value=raw.decode('utf-16-le'); encoding='utf-16-le'
 else:
  raw=b[at:b.index(b'\0',at)]; value=raw.decode('ascii'); encoding='ascii'
 rows.append({'rva':hex(rv),'fileOffset':hex(at),'hex':raw.hex(),'encoding':encoding,'value':value,'authority':'literal at direct LEA target; no runtime resolution observed'})
(OUT/'platform-consumer-names.json').write_text(json.dumps(rows,indent=2)+'\n',encoding='utf-8'); print(json.dumps(rows,indent=2))
