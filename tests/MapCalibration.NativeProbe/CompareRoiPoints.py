import json,pathlib,struct
rows=[json.loads(s) for s in pathlib.Path('analysis/map-calibration/image-roi/image-results.jsonl').read_text(encoding='utf-8-sig').splitlines()];base=rows[0]
for row in rows:
 if 'source' not in row:continue
 reports=[]
 for side in ['source','target']:
  original={(p[2],p[3]):p[:2] for p in base[side]};points=row[side];matched=[(p,original.get((p[2],p[3]))) for p in points];bits=sum(struct.pack('<f',p[j])==struct.pack('<f',q[j]) for p,q in matched if q for j in (0,1));total=sum(2 for p,q in matched if q);mx=max([abs(p[j]-q[j]) for p,q in matched if q for j in (0,1)] or [0]);reports.append(dict(side=side,count=len(points),bits=bits,total=total,max=mx))
 print(row['roiCase'],reports)
