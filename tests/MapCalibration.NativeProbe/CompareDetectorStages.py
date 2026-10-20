import json,pathlib,struct
root=pathlib.Path(__file__).resolve().parents[2];base=root/'analysis/map-calibration';native=[json.loads(x) for x in (base/'image-detector-stage-trace/image-results.jsonl').read_text(encoding='utf-8-sig').splitlines() if x.strip()];stages=[json.loads(x) for x in (base/'image-detector-stage-trace/stage-trace.jsonl').read_text().splitlines() if x.strip()];rows=[]
for s in stages:
 original=next(r for r in native if r['board']==s['board'])['source' if s['sequence']%2 else 'target'];equal=0;fields=0
 for p,q in zip(s['points'],original):
  for x,y in zip(p[:2],q[:2]):equal+=struct.pack('<f',x)==struct.pack('<f',y);fields+=1
  equal+=p[2:]==q[2:];fields+=1
 rows.append(dict(stage=s['stage'],sequence=s['sequence'],board=s['board'],count=s['count'],fields=fields,equal=equal))
assert len(rows)==4 and all(r['fields']==504 and r['equal']==504 for r in rows)
report=dict(stages=rows,refinement7e4360Called=False,scope='This SDK fixture/options only; native 80e690/80bcd0 returned points are already SDK final',fields=sum(r['fields'] for r in rows),equal=sum(r['equal'] for r in rows));(base/'image-detector-stage-trace/comparison.json').write_text(json.dumps(report,indent=2));print(json.dumps(report))
