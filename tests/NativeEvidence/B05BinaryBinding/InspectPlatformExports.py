"""Offline identity and two exports from the statically selected PlatformManager."""
from pathlib import Path
import hashlib,json,struct
ROOT=Path(__file__).resolve().parents[3]; OUT=ROOT/'analysis/full-audit/batch9-20261019/B05'
p=Path('D:/Program Files/VisionMaster4.4.0/Applications/Server/PlatformManager.dll'); b=p.read_bytes()
pe=struct.unpack_from('<I',b,0x3c)[0]; opt=pe+24; sz=struct.unpack_from('<H',b,pe+20)[0]; n=struct.unpack_from('<H',b,pe+6)[0]
assert b[pe:pe+4]==b'PE\0\0' and struct.unpack_from('<H',b,opt)[0]==0x20b
def off(rv):
 for i in range(n):
  vs,va,rs,rp=struct.unpack_from('<IIII',b,opt+sz+40*i+8)
  if va<=rv<va+rs: return rp+rv-va
 raise ValueError(rv)
er,es=struct.unpack_from('<II',b,opt+112); fields=struct.unpack_from('<IIHHIIIIIII',b,off(er)); rows=[]
for i in range(fields[7]):
 nr=struct.unpack_from('<I',b,off(fields[9])+4*i)[0]; at=off(nr); name=b[at:b.index(b'\0',at)].decode('ascii')
 if name not in ('PF_M_SetBinaryData','PF_M_MsgDeal'): continue
 ordindex=struct.unpack_from('<H',b,off(fields[10])+2*i)[0]; rv=struct.unpack_from('<I',b,off(fields[8])+4*ordindex)[0]
 rows.append({'name':name,'rva':hex(rv),'ordinal':fields[5]+ordindex,'forwarder':er<=rv<er+es})
assert len(rows)==2 and not any(x['forwarder'] for x in rows)
digest=hashlib.sha256(b).hexdigest(); assert hashlib.sha256(p.read_bytes()).hexdigest()==digest
report={'path':str(p),'sha256':digest,'byteLength':len(b),'exports':rows,'authority':'offline original PE; no LoadLibrary or target execution'}
(OUT/'platform-coordinates.json').write_text(json.dumps(report,indent=2)+'\n',encoding='utf-8'); print(json.dumps(report,indent=2))
