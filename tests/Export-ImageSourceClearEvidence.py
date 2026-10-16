"""Original typed clear-input and retained-width observations."""
import argparse,hashlib,json
from pathlib import Path
parser=argparse.ArgumentParser();parser.add_argument('--runtime',type=Path,required=True);args=parser.parse_args()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads(args.runtime.read_text(encoding='utf-8-sig'));rows=[r for r in records if r.get('phase','').startswith('clear-') and 'nativeErrorCode' in r];assert len(rows)==8
for row,value in zip(rows,[None,0,1,1,0,-1,0,1]):
 row['inputValue']=value;path=args.runtime.parent/(row['phase']+'.pixels');assert sha(path)==row['pixelSha256'];row['pixelsHex']=path.read_bytes().hex()
root=Path('D:/MyWork/REA逆向测试/analysis/full-audit')
result={'schema':1,'runtimeSource':str(args.runtime),'runtimeSha256':sha(args.runtime),'sourceAnalysis':[{'path':str(p),'sha256':sha(p)} for p in [root/'image-source-native-0.c',root/'image-source-native-7.c']],'observations':rows,'completeParity':False}
Path('docs/plugins/image-source-clear-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8');print('Exported 8 original typed clear scenarios')
