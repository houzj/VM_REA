"""Index installed evidence, own native analysis and genuine algorithm replay."""
from pathlib import Path
import hashlib,json,xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[1]
installed=Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSNImageCalibModu')
audit=root/'analysis/full-audit/n-image-calibration'
def identity(p):return {'path':str(p),'length':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
def tree(e):return {'kind':e.tag,'attributes':e.attrib,'text':(e.text or '').strip(),'children':[tree(c) for c in e]}
tab=installed/'IMVSNImageCalibModuAlgorithmTab.xml'
declaration=installed/'IMVSNImageCalibModu.xml'
doc={'authority':'shipped-artifact + controlled-replay + static native decompilation','managedEvidenceId':'ev_47d8fe724a3d57ef879aa778e0fb1e24e74df258e6f6f9266459af03c8e39ca6','installed':[identity(p) for p in sorted(installed.iterdir()) if p.is_file()], 'declaration':tree(ET.parse(declaration).getroot()),'ui':tree(ET.parse(tab).getroot()),'native':[identity(audit/'native'/f'{a}.c') for a in ['18001f670','1800209b0','180020b90','180021450','180021850','1800229d0','1800230e0']],'probe':identity(root/'tests/NativeEvidence/OriginalNImageAlgorithmProbe.cpp'),'algorithm':identity(Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDNImageCalibV2Cpp.dll')),'runtime':[json.loads(l) for l in (audit/'runtime.jsonl').read_text(encoding='utf-8-sig').splitlines() if l.strip()],'limitations':['Algorithm SDK replay is not platform-module execution.','No physical equipment used.','No successful coded-board calibration observed yet.','No claim of full native parity.']}
doc['fixture']=json.loads((audit/'fixture.json').read_text(encoding='utf-8-sig'))
for row in doc['fixture']['fixtures']:
    if identity(Path(row['path']))['sha256']!=row['sha256']:raise RuntimeError('Fixture identity mismatch')
for name in ['18001f090','18001f0a0','18001f5a0']:doc['native'].append(identity(audit/'native'/f'{name}.c'))
doc['fixtureRuntime']=[json.loads(l) for l in (audit/'runtime-fixture.jsonl').read_text(encoding='utf-8-sig').splitlines() if l.strip()]
doc['moduleRuntime']=[json.loads(l) for l in (audit/'module-runtime.jsonl').read_text(encoding='utf-8-sig').splitlines() if l.strip()]
doc['moduleProvenance']=json.loads((audit/'module-provenance.json').read_text(encoding='utf-8-sig'))
if doc['moduleProvenance']['harnessSha256']!=identity(root/'tests/NativeEvidence/OriginalNImageModuleHarness.cpp')['sha256']:raise RuntimeError('Harness producer differs from recorded replay')
if doc['moduleProvenance']['outputSha256']!=identity(audit/'original-module-calibration.xml')['sha256']:raise RuntimeError('Module export differs from recorded replay')
doc['nativeModuleExport']={'identity':identity(audit/'original-module-calibration.xml'),'xml':tree(ET.parse(audit/'original-module-calibration.xml').getroot())}
doc['limitations']=['Direct SDK and synthetic-owned-instance module replay do not establish complete platform behavior.','No physical equipment used.','Documentation-derived I-type board permits original algorithm success; warning status 0x1010304e remains independently recorded.','Independent coded-board detector/decoder and full editor not completed.','No claim of full native parity.']
out=root/'docs/plugins/n-image-calibration-evidence.json';out.write_text(json.dumps(doc,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
resource=root/'src/VisionPlugins.Calibration/Resources/NImageDefinition.json';resource.parent.mkdir(parents=True,exist_ok=True)
resource.write_text(json.dumps({'source':identity(tab),'ui':doc['ui'],'declaration':doc['declaration']},ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f'Indexed {len(doc["installed"])} installed files, {len(doc["native"])} native functions, {len(doc["runtime"])+len(doc["fixtureRuntime"])} actual SDK records and {len(doc["moduleRuntime"])} owned-instance module records.')
