import json,struct,pathlib
root=pathlib.Path(__file__).resolve().parents[2];reports=[]
f32=lambda x:struct.unpack('<f',struct.pack('<f',x))[0]
for weight,name in [(3,'image-fit-trace'),(1,'image-fit-weight1-trace'),(2,'image-fit-weight2-trace')]:
 d=root/'analysis/map-calibration'/name;rows=[json.loads(s) for s in (d/'image-results.jsonl').read_text(encoding='utf-8-sig').splitlines()];records=[json.loads(s) for s in (d/'fixed-trace.jsonl').read_text().splitlines()]
 for row in rows:
  inputs=[x for x in records if x['phase']=='fixed-fit-input' and x['board']==row['board'] and x['model']==row['model'] and x['options'][3]==50]
  for side,record in zip(['source','target'],inputs):
   n=record['count'];capture=d/f'capture-{record["sequence"]:03d}.bin';values=struct.unpack('<'+str(n*4)+'f',capture.read_bytes());points=row[side];pitch=10 if row['board']==7 else 1000
   expected=[f32(v) for p in points for v in p[:2]]+[f32(f32(v)*pitch) for p in points for v in p[2:]]
   differences=sum(a!=b for a,b in zip(values,expected));reports.append(dict(weight=weight,board=row['board'],model=row['model'],side=side,fields=len(values),differences=differences,sequence=record['sequence']))
report=dict(fields=sum(x['fields'] for x in reports),differences=sum(x['differences'] for x in reports),cases=reports);out=root/'analysis/map-calibration/image-fit/input-comparison.json';out.write_text(json.dumps(report,indent=2));print(f'{len(reports)} main fits, {report["fields"]} fields, {report["differences"]} input differences')

