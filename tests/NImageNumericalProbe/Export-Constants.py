"""Targeted original PE constant extraction, no target loading or execution."""
import hashlib,json,pathlib,struct,math
p=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll')
b=p.read_bytes()
assert hashlib.sha256(b).hexdigest()=='85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549'
pe=struct.unpack_from('<I',b,0x3c)[0];n=struct.unpack_from('<H',b,pe+6)[0];opt=struct.unpack_from('<H',b,pe+20)[0];base=struct.unpack_from('<Q',b,pe+24+24)[0]
sections=[struct.unpack_from('<IIII',b,pe+24+opt+i*40+8) for i in range(n)]
rows=[]
for address in [0x181628490,0x1816284a0,0x1816284a4,0x1816284a8,0x1816284ac,0x1816284b0,0x1816284c0,0x1816284c4,0x1816284c8,0x1816284cc,0x1816284d0,0x1816284dc,0x1816284e0]:
 rva=address-base;vs,va,rs,raw=next(s for s in sections if s[1]<=rva<s[1]+max(s[0],s[2]));off=raw+rva-va;f=struct.unpack_from('<f',b,off)[0];d=struct.unpack_from('<d',b,off)[0]
 rows.append(dict(address=hex(address),fileOffset=off,bytes8=b[off:off+8].hex(),float32=f if math.isfinite(f) else str(f),float64=d if math.isfinite(d) else str(d)))
out=pathlib.Path('analysis/full-audit/n-image-calibration/numerical-probe/constants.json');out.write_text(json.dumps(dict(source=str(p),sha256=hashlib.sha256(b).hexdigest(),constants=rows),indent=2))
print(json.dumps(rows,indent=2))
