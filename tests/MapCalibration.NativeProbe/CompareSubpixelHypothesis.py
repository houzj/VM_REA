import math,json,pathlib,statistics
root=pathlib.Path(__file__).resolve().parents[2];pixels=(root/'analysis/map-calibration/image-input/native-source.gray').read_bytes();w=1000
native=(105.408211,110.790932)
def raw(x,y):return pixels[y*w+x]
def median(x,y):return sorted(raw(x+i,y+j) for i in [-1,0,1] for j in [-1,0,1])[4]
def refine(sample,x,y,r):
 xx=xy=yy=bx=by=0.
 for j in range(y-r,y+r+1):
  for i in range(x-r,x+r+1):
   gx=sample(i+1,j)-sample(i-1,j);gy=sample(i,j+1)-sample(i,j-1);a=math.exp(-((i-x)**2+(j-y)**2)/float(r*r));xx+=a*gx*gx;xy+=a*gx*gy;yy+=a*gy*gy;bx+=a*(gx*gx*i+gx*gy*j);by+=a*(gx*gy*i+gy*gy*j)
 d=xx*yy-xy*xy
 return ((yy*bx-xy*by)/d,(xx*by-xy*bx)/d)
rows=[]
for r in range(2,13):
 for seed in [(105,111),(105,110),(106,111),(106,110)]:
  for name,sample in [('raw',raw),('median3',median)]:
   p=refine(sample,*seed,r);rows.append(dict(radius=r,seed=seed,input=name,point=p,maxDelta=max(abs(p[i]-native[i]) for i in [0,1])))
rows.sort(key=lambda r:r['maxDelta']);out=root/'analysis/map-calibration/image-input/subpixel-hypothesis.json';out.write_text(json.dumps(dict(independentHypothesis=True,nativePoint=native,originalSubpixelAlgorithmVerified=False,rows=rows),indent=2));print(json.dumps(rows[:4],indent=2))
