from pathlib import Path
import json,hashlib
root=Path(__file__).resolve().parents[1]
audit=root/'analysis/full-audit/n-image-calibration'
def identity(p):return {'path':str(p),'length':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
runs=sorted((audit/'ui').glob('run-*'),key=lambda p:p.stat().st_mtime)
run=runs[-1]
rows=json.loads((run/'probe.json').read_text(encoding='utf-8-sig'))
if any(row.get('error') for row in rows):raise RuntimeError('Latest parser replay has errors')
producer=identity(root/'tests/NativeEvidence/OriginalNImageUiProbe.cs')
provenance=json.loads((run/'provenance.json').read_text(encoding='utf-8-sig'))
if not any(p['sha256']==producer['sha256'] for p in provenance):raise RuntimeError('Parser replay producer changed')
original=Path('D:/Program Files/VisionMaster4.4.0/Applications')
result={'authority':'original parser + original custom operations with synthetic parameter/owner model','nativeParameters':False,'desktopOperation':False,'fullNativeUiRendered':False,'run':str(run),'producer':producer,'provenance':provenance,'parser':identity(original/'myLibs/Apps.XmlParser.dll'),'records':rows,'reconstructedScreenshots':[identity(p) for p in sorted((audit/'reconstructed-ui').glob('*.png'))],'limitations':['No complete original WPF shell image: shared BAML initialization is separately recorded as failing.','Original snapshots execute parser operations, not the native platform module.','Reconstructed images are independent editor output, not original UI pixels.']}
(root/'docs/plugins/n-image-calibration-ui-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f'Indexed {len(rows)} original parser records and {len(result["reconstructedScreenshots"])} independent editor screenshots.')
