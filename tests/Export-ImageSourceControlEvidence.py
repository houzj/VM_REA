"""Bounded source-correlated filter and fresh SDK submission evidence."""
import argparse,hashlib,json
from pathlib import Path
parser=argparse.ArgumentParser();parser.add_argument('--runtime',type=Path,required=True);args=parser.parse_args()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads(args.runtime.read_text(encoding='utf-8-sig'));inputs=[r for r in records if r.get('phase')=='filter-input'];assert len(inputs)==7
names={r['name'] for r in inputs}|{'sdk-fresh-submission','sdk-submission-consumed','sdk-resubmitted'}
observations=[]
for row in records:
 if row.get('phase') not in names:continue
 phase=row['phase'];assert sha(args.runtime.parent/(phase+'.pixels'))==row['pixelSha256'];extra=next(r for r in records if r.get('phase')==phase+'-extra-mono');assert sha(args.runtime.parent/(phase+'.mono.pixels'))==extra['pixelSha256'];row['extraMono']=extra;observations.append(row)
assert len(observations)==10
result={'schema':1,'runtimeSource':str(args.runtime),'runtimeSha256':sha(args.runtime),'inputSampleSha256':sha(Path('analysis/images/circle-find-sample.bmp')),'inputs':inputs,'observations':observations,'completeParity':False}
Path('docs/plugins/image-source-control-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8');print('Exported 7 filter and 3 fresh SDK submission scenarios')
