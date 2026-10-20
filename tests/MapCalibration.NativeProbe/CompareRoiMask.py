import pathlib,json,struct
root=pathlib.Path(__file__).resolve().parents[2];read=lambda folder:[json.loads(s) for s in (root/'analysis/map-calibration'/folder/'image-results.jsonl').read_text(encoding='utf-8-sig').splitlines()];original=read('image-roi');masked=read('image-roi-mask');rows=[]
def floats(v):
 if isinstance(v,list):return [n for i in v for n in floats(i)]
 return [v]
for case in masked:
 expected=next(x for x in original if x['roiCase']==case['roiCase']);fields=[]
 for name in ['matrix','ratio','status','scale','error','counts','source','target','projection']:
  if name in expected:
   a=floats(expected[name]);b=floats(case.get(name,[]));fields.append((name,len(a),sum(struct.pack('<f',x)==struct.pack('<f',y) for x,y in zip(a,b))))
 rows.append(dict(roiCase=case['roiCase'],fields=sum(x[1] for x in fields),equal=sum(x[2] for x in fields),detail=fields))
r=dict(cases=rows,fields=sum(x['fields'] for x in rows),equal=sum(x['equal'] for x in rows),nonemptyGroups=sum(x['roiCase']!=0 for x in rows));out=root/'analysis/map-calibration/image-roi/mask-equivalence.json';out.write_text(json.dumps(r,indent=2));print(json.dumps(r));

assert len(rows)==8 and r['nonemptyGroups']==7, 'Expected baseline plus seven nonempty ROI groups'
assert r['fields']==14584 and r['equal']==r['fields'], 'Native ROI/PreproMask field mismatch'
