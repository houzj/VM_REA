"""Export original bit-depth, alpha, compression and multi-frame observations."""
import argparse,hashlib,json
from pathlib import Path
parser=argparse.ArgumentParser();parser.add_argument('--runtime',type=Path,required=True);args=parser.parse_args()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads(args.runtime.read_text(encoding='utf-8-sig'));fixtures=[r for r in records if r.get('phase')=='format-fixture'];assert len(fixtures)==26
results={r['phase']:r for r in records if r.get('phase','').startswith(('format-mono-','format-rgb-'))};observations=[]
for fixture in fixtures:
 name=fixture['name'];path=args.runtime.parent/'format-fixtures'/name;assert sha(path)==fixture['fileSha256'];item={'name':name,'fixtureSha256':sha(path)}
 for mode in ['mono','rgb']:
  record=results['format-'+mode+'-'+name];pixels=args.runtime.parent/('format-'+mode+'-'+name+'.pixels');assert sha(pixels)==record['pixelSha256']
  item[mode]={k:record[k] for k in ['nativeErrorCode','width','height','format','length','pixelSha256']};item[mode]['pixelsHex']=pixels.read_bytes().hex()
 observations.append(item)
result={'schema':1,'runtimeSource':str(args.runtime),'runtimeSha256':sha(args.runtime),'generatorSha256':sha(Path('tests/NativeEvidence/ImageSourceCodecFixtures.cs')),'observations':observations,'completeParity':False}
sources=[Path('analysis/full-audit/image-source-formats')/name for name in ['18038cc20.c','18038d080.c']]
result['sourceAnalysis']=[{'path':str(p),'sha256':sha(p)} for p in sources]
result['packedPalettePngLimitation']='Native PNG palette path reads one byte per pixel without low-bit unpacking. Output can depend on bytes outside a packed row. Original MONO output differs from converting the RGB output for the same packed fixture; independent decoder explicitly rejects this path instead of reading beyond rows.'
Path('docs/plugins/image-source-format-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8');print('Exported 26 original format observations')
