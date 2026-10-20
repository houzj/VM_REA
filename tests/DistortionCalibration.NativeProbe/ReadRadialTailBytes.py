import pathlib,struct,hashlib,json
p=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll');b=p.read_bytes();assert hashlib.sha256(b).hexdigest()=='85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549'
pe=struct.unpack_from('<I',b,0x3c)[0];n=struct.unpack_from('<H',b,pe+6)[0];opt=struct.unpack_from('<H',b,pe+20)[0];sections=[struct.unpack_from('<IIII',b,pe+24+opt+40*i+8) for i in range(n)]
def raw(va,count):
 rva=va-0x180000000
 for size,start,length,offset in sections:
  if start<=rva<start+max(size,length):return b[offset+rva-start:offset+rva-start+count]
 raise ValueError(hex(va))
rows=[]
for va in [0x1807edb18,0x1807edc8f]:
 data=raw(va,120);assert data[8:13]==bytes.fromhex('b80b301010');rows.append(dict(address=hex(va),hex=data.hex(),movEaxOffset=8,returnCode='0x1010300b'));print(hex(va),data.hex(' '))
constant=raw(0x18161d1d0,8);print('maximum-init',constant.hex(' '),struct.unpack('<d',constant)[0]);rows.append(dict(address='0x18161d1d0',hex=constant.hex(),value=struct.unpack('<d',constant)[0]))
out=pathlib.Path('analysis/distortion-calibration/radial-source-order/output/tail-raw.json');out.write_text(json.dumps(rows,indent=2))


