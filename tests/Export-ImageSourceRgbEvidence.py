"""Export original RGB cache transition, optional mono and stitching bytes."""
import argparse,hashlib,json
from pathlib import Path
parser=argparse.ArgumentParser();parser.add_argument('--runtime',type=Path,required=True);args=parser.parse_args()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads(args.runtime.read_text(encoding='utf-8-sig'));rows=[r for r in records if r.get('phase','').startswith('rgb-') and 'nativeErrorCode' in r];assert len(rows)==7
for row in rows:
 phase=row['phase'];pixels=args.runtime.parent/(phase+'.pixels');assert sha(pixels)==row['pixelSha256'];row['pixelsHex']=pixels.read_bytes().hex()
 extra=next(r for r in records if r.get('phase')==phase+'-extra-mono');pixels=args.runtime.parent/(phase+'.mono.pixels');assert sha(pixels)==extra['pixelSha256'];row['extraMono']=extra;row['extraMono']['pixelsHex']=pixels.read_bytes().hex()
result={'schema':1,'runtimeSource':str(args.runtime),'runtimeSha256':sha(args.runtime),'observations':rows,'completeParity':False}
Path('docs/plugins/image-source-rgb-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8');print('Exported 7 RGB cache/stitch/extra-MONO scenarios')
