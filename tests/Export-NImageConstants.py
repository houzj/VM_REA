"""Read source-referenced constants from the exact installed PE without loading code."""
from pathlib import Path
import struct,json,hashlib
r=Path(__file__).resolve().parents[1]
p=Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll')
b=p.read_bytes();digest=hashlib.sha256(b).hexdigest()
if digest!='85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549':raise RuntimeError('Unexpected original core identity')
pe=struct.unpack_from('<I',b,60)[0];n=struct.unpack_from('<H',b,pe+6)[0];opt=struct.unpack_from('<H',b,pe+20)[0];sections=[]
for i in range(n):
 o=pe+24+opt+40*i;vs,va,rs,rp=struct.unpack_from('<IIII',b,o+8);sections.append((va,rs,rp))
def offset(va):
 rva=va-0x180000000
 return next(rp+rva-v for v,size,rp in sections if v<=rva<v+size)
rows=[]
for va in range(0x1816445a0,0x181644600,4):
 o=offset(va);v=b[o:o+16];rows.append({'address':hex(va),'fileOffset':o,'bytes16':v.hex(),'int32':struct.unpack('<i',v[:4])[0],'float32':struct.unpack('<f',v[:4])[0],'float64':struct.unpack('<d',v[:8])[0]})
out=r/'analysis/full-audit/n-image-calibration/core-constants.json';out.write_text(json.dumps({'path':str(p),'sha256':digest,'constants':rows},indent=2)+'\n',encoding='utf-8');print(out)
