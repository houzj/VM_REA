"""Bounded original directory-event execution sequence."""
import argparse,hashlib,json
from pathlib import Path
parser=argparse.ArgumentParser();parser.add_argument('--runtime',type=Path,required=True);args=parser.parse_args()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads(args.runtime.read_text(encoding='utf-8-sig'));rows=[r for r in records if r.get('phase','').startswith('watch-') and 'nativeErrorCode' in r];assert len(rows)==5
for row in rows:
 path=args.runtime.parent/(row['phase']+'.pixels');assert sha(path)==row['pixelSha256'];row['pixelsHex']=path.read_bytes().hex()
root=Path('D:/MyWork/REA逆向测试/analysis/full-audit')
result={'schema':1,'runtimeSource':str(args.runtime),'runtimeSha256':sha(args.runtime),'sourceAnalysis':[{'path':str(p),'sha256':sha(p)} for p in [root/'image-source-native-extra-25.c',root/'image-source-native-9.c',root/'image-source-native-0.c']],'observations':rows,'limitations':['Original SDK manual runs with AutoPlay=False, cache=0, TriggerProcessRun=False. Polling/event timing is not a deterministic callback-count contract.','Runtime observations preserve cursor index on refresh; original rename actions 4/5 do not dirty the list.','Independent FileSystemWatcher backend and UI request-run dispatch require lifecycle tests; original completion-port implementation is not copied.'],'completeParity':False}
Path('docs/plugins/image-source-watch-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8');print('Exported 5 original directory-event scenarios')
