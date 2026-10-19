"""Read-only displacement candidates, NOT decoded instructions or confirmed xrefs."""
from pathlib import Path
import hashlib,json,struct
ROOT=Path(__file__).resolve().parents[3]
OUT=ROOT/'analysis/full-audit/batch9-20261019/B05'; OUT.mkdir(parents=True,exist_ok=True)
path=Path('D:/Program Files/VisionMaster4.4.0/Applications/PublicFile/x64/iMVS-6000PlatformSDK.dll')
b=path.read_bytes(); digest=hashlib.sha256(b).hexdigest()
assert digest=='b62d7e15588ccda6671228aebc11fec3e27aca878f0e03067e44b17d5a54a9a1'
pe=struct.unpack_from('<I',b,0x3c)[0]; opt=pe+24; sz=struct.unpack_from('<H',b,pe+20)[0]; count=struct.unpack_from('<H',b,pe+6)[0]
sections=[]
for i in range(count):
 at=opt+sz+40*i; vs,va,rs,rp=struct.unpack_from('<IIII',b,at+8); flags=struct.unpack_from('<I',b,at+36)[0]
 sections.append((va,vs,rp,rs,flags))
def offset(rv):
 for va,vs,rp,rs,flags in sections:
  if va<=rv<va+rs: return rp+rv-va
 raise ValueError(rv)
er,es=struct.unpack_from('<II',b,opt+112+24); tab=offset(er)
bounds=[struct.unpack_from('<III',b,tab+i) for i in range(0,es,12)]
rows=[]
for va,vs,rp,rs,flags in sections:
 if not flags & 0x20000000: continue
 pos=rp
 while True:
  pos=b.find(bytes.fromhex('a8230000'),pos,rp+rs)
  if pos<0: break
  rv=va+pos-rp
  rows.append({'displacementRva':hex(rv),'contextStartRva':hex(rv-8),'contextHex':b[pos-8:pos+12].hex(),'runtimeFunctions':[[hex(x) for x in row] for row in bounds if row[0]<=rv<row[1]],'authority':'byte candidate only; register/base/decoded semantics not established'})
  pos+=4
assert hashlib.sha256(path.read_bytes()).hexdigest()==digest
(OUT/'displacement-candidates.json').write_text(json.dumps({'path':str(path),'sha256':digest,'search':'literal LE32 0x23a8 in executable raw sections','candidates':rows},indent=2)+'\n',encoding='utf-8')
print(json.dumps(rows,indent=2))
