"""Generate colored N-image SDK fixtures from owned 8-bit board BMPs; no devices."""
from pathlib import Path
import struct
base=Path(__file__).resolve().parents[2]/'analysis/full-audit/n-image-calibration'
out=base/'color-fixture';out.mkdir(exist_ok=True)
for i in range(9):
 d=(base/'solver-fixture'/f'frame-{i}.bmp').read_bytes()
 off=struct.unpack_from('<I',d,10)[0];w,h,planes,bits=struct.unpack_from('<iiHH',d,18)
 assert h>0 and planes==1 and bits==8
 stride=(w+3)&~3;rgb=bytearray();mono=bytearray()
 for y in range(h):
  for v in d[off+y*stride:off+y*stride+w]:
   r=20+235*v//255;g=80+175*v//255;b=140+115*v//255
   rgb.extend((b,g,r));mono.append((76*r+150*g+30*b)>>8)
  rgb.extend(bytes((-w*3)%4));mono.extend(bytes((-w)%4))
 def save(name,bits,pixels):
  palette=b''.join(bytes((v,v,v,0)) for v in range(256)) if bits==8 else b''
  o=54+len(palette)
  header=struct.pack('<2sIHHI',b'BM',o+len(pixels),0,0,o)+struct.pack('<IiiHHIIiiII',40,w,h,1,bits,0,len(pixels),2835,2835,256 if bits==8 else 0,0)
  (out/f'{name}-{i}.bmp').write_bytes(header+palette+pixels)
 save('rgb',24,rgb);save('gray',8,mono)
