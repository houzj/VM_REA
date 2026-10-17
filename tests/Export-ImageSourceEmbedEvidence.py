"""Original SaveAs/new-output-only and embedded reload evidence."""
import argparse,hashlib,json
from pathlib import Path
parser=argparse.ArgumentParser();parser.add_argument('--runtime',type=Path,required=True);args=parser.parse_args();sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads(args.runtime.read_text(encoding='utf-8-sig'));names={'embed-before-save','embed-after-source-removed'};rows=[r for r in records if r.get('phase') in names];assert len(rows)==2
for row in rows:
 path=args.runtime.parent/(row['phase']+'.pixels');assert sha(path)==row['pixelSha256'];row['pixelsHex']=path.read_bytes().hex()
assert rows[0]['pixelSha256']==rows[1]['pixelSha256'];saved=next(r for r in records if r.get('phase')=='embed-save-as');assert sha(args.runtime.parent/'owned-embedded.sol')==saved['sha256']
root=Path('D:/MyWork/REA逆向测试/analysis/full-audit');result={'schema':1,'runtimeSource':str(args.runtime),'runtimeSha256':sha(args.runtime),'save':saved,'observations':rows,'sourceAnalysis':[{'path':str(p),'sha256':sha(p)} for p in [root/'image-source-native-extra-7.c',root/'image-source-native-8.c']],'limitations':['Original SDK SaveAs wrote only a new owned .sol. The originally loaded sample was not saved or changed.','Original reload filename gains a numeric module-ID prefix. Independent VMX resources preserve original configured names and do not implement vendor SOL serialization.','Native 120-item limit is source-confirmed (0x78 checks in SaveModuleData), not a 120-item native runtime experiment.'],'completeParity':False}
Path('docs/plugins/image-source-embed-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8');print('Exported original embedded save/reload pixels and source limit')
