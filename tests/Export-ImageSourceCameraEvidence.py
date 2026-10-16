"""Bounded ledger for original camera operations and independent real-device checks."""
import argparse, hashlib, json
from pathlib import Path

p=argparse.ArgumentParser();p.add_argument('--runtime',type=Path,required=True);p.add_argument('--independent',type=Path,required=True);a=p.parse_args()
sha=lambda b:hashlib.sha256(b).hexdigest()
original=json.loads(a.runtime.read_text(encoding='utf-8-sig'));independent=json.loads(a.independent.read_text(encoding='utf-8-sig'))
rows=[r for r in original if r.get('phase','').startswith('camera-') and r['phase']!='camera-catalog']
pixel_checks=[]
for r in rows:
    if 'nativeErrorCode' not in r or not r['length']:continue
    phase=r['phase'];rgb=(a.runtime.parent/(phase+'.pixels')).read_bytes();mono=(a.runtime.parent/(phase+'.mono.pixels')).read_bytes()
    assert sha(rgb)==r['pixelSha256'];assert len(rgb)==r['width']*r['height']*3
    expected=bytes((76*x+150*y+30*z)>>8 for x,y,z in zip(rgb[0::3],rgb[1::3],rgb[2::3]))
    differences=sum(x!=y for x,y in zip(expected,mono));assert len(expected)==len(mono)
    pixel_checks.append(dict(phase=phase,pixels=len(mono),differentPixels=differences,expectedSha256=sha(expected),originalMonoSha256=sha(mono)))
    assert differences==0,phase
parameters=[]
for index in range(3):
    native=next(r for r in rows if r['phase']==f'camera-readback-{index}')
    own=next(r for r in independent if r['phase']==f'parameter-readback-{index}')
    matched=all(float(f'{own[k]:.3f}')==native[k] for k in ('exposure','gain'));assert matched
    parameters.append(dict(original=native,independent=own,matchedAtNativeReadbackPrecision=matched))
frames=[r for r in independent if r.get('phase','').startswith('frame-')];assert [r['FrameNum'] for r in frames]==[1,2,3]
assert all((r['Width'],r['Height'],r['format'])==(2448,2048,'Rgb24') for r in frames)
failure=next(r for r in independent if r['phase']=='plugin-no-trigger');recovered=next(r for r in independent if r['phase']=='plugin-recovered')
assert failure['status']=='Failed' and failure['NativeErrorCode']==0 and failure['scalars']['GetCameraFailCount']==1
assert recovered['status']=='Success' and recovered['scalars']['GetCameraFailCount']==1 and recovered['scalars']['SNCode']==9
native_invalid=next(r for r in rows if r['phase']=='camera-invalid-stitch');native_recovered=next(r for r in rows if r['phase']=='camera-recovered');assert native_invalid['nativeErrorCode']==0xe0000502 and native_invalid['sn']==native_recovered['sn']+1
own_invalid=next(r for r in independent if r['phase']=='plugin-invalid-stitch');assert own_invalid['NativeErrorCode']==0xe0000502 and own_invalid['scalars']['SNCode']==10 and own_invalid['scalars']['GetCameraFailCount']==1 and 'FrameNum' in own_invalid['scalars']
settings=next(r for r in independent if r['phase']=='settings');final=next(r for r in independent if r['phase']=='final-settings')
assert all(settings[k]==final[k] for k in ('exposure','gain','triggerMode','triggerSource'))
source_root=Path('D:/MyWork/REA逆向测试/analysis/full-audit');provenance=a.runtime.parent/'provenance.json'
result=dict(schema=2,reaManagedIdentityEvidence='ev_1ffa33e107785938a9ea47bee49b5da70ca4732631308f72abc9e31bad5c8ed6',originalSource=str(a.runtime),originalSha256=sha(a.runtime.read_bytes()),originalProbeSha256=sha((a.runtime.parent/'OriginalImageSourceProbe.exe').read_bytes()),provenance=json.loads(provenance.read_text(encoding='utf-8-sig')) if provenance.exists() else None,sourceAnalysis=[dict(path=str(path),sha256=sha(path.read_bytes())) for path in (source_root/'image-source-native-0.c',source_root/'image-source-native-extra-18.c')],independentSource=str(a.independent),independentSha256=sha(a.independent.read_bytes()),camera=next(r for r in independent if r['phase']=='enumeration'),observations=rows,independent=independent,parameterComparisons=parameters,pixelChecks=pixel_checks,completeParity=False,limitations=['Different live exposures are different sensor frames; no cross-process frame-hash equality is claimed.','Same original RGB frame to its extra MONO output was compared at every pixel.','Gain values compare at original global-camera readback precision of three decimals.','Positive physical packet loss and hardware-trigger input were not generated.','Only the attached MV-CS050-10GC was tested; other camera models and fallback parameter names remain unverified.'])
Path('docs/plugins/image-source-camera-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8')
print(f'Exported {len(pixel_checks)} full-frame pixel checks, 3 parameter comparisons, timeout/recovery and restored settings')
