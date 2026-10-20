import json,pathlib,struct
base=pathlib.Path('analysis/map-calibration/image-grid-trace');rows=[json.loads(x) for x in (base/'grid-trace.jsonl').read_text().splitlines()];groups=[];pending=[]
for r in rows:
 if r['stage']==1:pending.append(r)
 else:
  def bits(x):return struct.pack('<f',x)
  comps=[]
  for p in pending:
   equal=sum(bits(x)==bits(y) for a,b in zip(p['points'],r['points']) for x,y in zip(a[:2],b[:2]));comps.append(dict(sequence=p['sequence'],inputCandidateCount=p['inputCandidateCount'],score=p['score'],count=p['count'],xyEqual=equal,xyFields=r['count']*2))
  groups.append(dict(board=r['board'],sequence=r['sequence'],count=r['count'],candidateStages=comps));pending=[]
out=dict(scope='Actual 873de0 selection among ordered 8bcff0 results; no tag decoder used inside these stages',groups=groups);(base/'selection-comparison.json').write_text(json.dumps(out,indent=2));print(json.dumps(out))
