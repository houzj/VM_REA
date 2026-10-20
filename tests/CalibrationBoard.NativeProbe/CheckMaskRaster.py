import json,pathlib,struct,math
root=pathlib.Path.cwd();rows=[json.loads(s) for s in (root/'analysis/calibration-board/roi-mask-native/runtime.jsonl').read_text().splitlines()]
f=lambda x:struct.unpack('<f',struct.pack('<f',x))[0]
dll=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDPreproMaskCpp.dll').read_bytes();pe=struct.unpack_from('<I',dll,60)[0];n=struct.unpack_from('<H',dll,pe+6)[0];size=struct.unpack_from('<H',dll,pe+20)[0]
def raw(rva,size):
 for i in range(n):
  _,vsize,va,siz,off,*_=struct.unpack_from('<8sIIIIIIHHI',dll,pe+24+sizeOpt+40*i)
  if va<=rva<va+max(vsize,siz):return dll[off+rva-va:off+rva-va+size]
sizeOpt=size
print('constants',[(hex(a),raw(a,8).hex(),struct.unpack('<f',raw(a,4))[0]) for a in [0x9a58,0x9a5c,0x9b18,0x9b20,0x9ba8,0x9bb0]])
differences=[]
for r in rows:
 out=bytearray(64*48)
 if abs(r['angle'])<.0001:
  cx=f(r['cx']);cy=f(r['cy']);w=f(r['width']);h=f(r['height']);left=f(cx-f(w*.5));top=f(cy-f(h*.5));right=f(cx+w*.5);bottom=f(cy+h*.5)
  x=max(0,int(left+.5));y=max(0,int(top+.5));endx=min(64,int(right+.5));endy=min(48,int(bottom+.5))
  for row in range(y,endy):out[row*64+x:row*64+endx]=bytes([255])*max(0,endx-x)
 else:
  points=[(f(f(x)/64),f(f(y)/48)) for x,y in r['vertices']]
  minx=max(0,int(f(f(64*min(x for x,y in points))+.5)));maxx=min(63,int(f(f(64*max(x for x,y in points))+.5)));miny=max(0,int(f(f(48*min(y for x,y in points))+.5)));maxy=min(47,int(f(f(48*max(y for x,y in points))+.5)))
  for y in range(miny,maxy+1):
   intersections=[]
   for i,(x,py) in enumerate(points):
    px2,py2=points[i-1];ya=f(py*48);yb=f(py2*48)
    if ya<y<=yb or yb<y<=ya:
     value=f(f(f(f(f(y-ya)/f(f(py2-py)*48))*f(px2-x))*64)+f(x*64));intersections.append(value)
   intersections.sort()
   for i in range(0,len(intersections)-1,2):
    left=max(minx,int(f(intersections[i]+1)));right=min(maxx,int(intersections[i+1]))
    if left<=right:out[y*64+left:y*64+right+1]=bytes([255])*(right-left+1)
 native=bytes.fromhex(r['hex']);diff=sum(x!=y for x,y in zip(out,native));differences.append({'index':r['index'],'angle':r['angle'],'difference':diff})
print('different',[(r['index'],r['difference']) for r in differences if r['difference']]);(root/'analysis/calibration-board/roi-mask-native/independent-differences.json').write_text(json.dumps(differences,indent=2))
