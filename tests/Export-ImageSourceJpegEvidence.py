"""Export original JPEG corpus bytes and source/build identities without parity overclaim."""
import argparse
import hashlib
import json
from pathlib import Path

parser=argparse.ArgumentParser();parser.add_argument('--runtime',type=Path,required=True);args=parser.parse_args()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads(args.runtime.read_text(encoding='utf-8-sig'))
fixtures=[r for r in records if r.get('phase')=='jpeg-fixture'];assert len(fixtures)==27
results={r['phase']:r for r in records if r.get('phase','').startswith(('jpeg-mono-','jpeg-rgb-'))}
observations=[]
for fixture in fixtures:
 name=fixture['name'];path=Path('tests/ImageSource.PluginChecks/Fixtures/Jpeg')/name
 assert sha(path)==fixture['fileSha256']
 item={'name':name,'fixtureSha256':sha(path)}
 for mode in ['mono','rgb']:
  record=results['jpeg-'+mode+'-'+name];pixels=args.runtime.parent/('jpeg-'+mode+'-'+name+'.pixels');assert sha(pixels)==record['pixelSha256']
  item[mode]={k:record[k] for k in ['nativeErrorCode','width','height','format','length','pixelSha256']}
  item[mode]['pixelsHex']=pixels.read_bytes().hex()
 observations.append(item)
analysis=Path('analysis/full-audit/image-source-jpeg')
media=Path('analysis/native-input/MVDMediaProcess.dll');assert sha(media)=='eea3f2310ead31fbfebe0dc187f270b11cf686344275d3ddcaeb95e2051868a7'
version=b'libjpeg-turbo version 2.1.3 (build 20220324)';version_offset=media.read_bytes().find(version);assert version_offset>=0
upstream=Path('analysis/downloads/jpeg/libjpeg-turbo-2.1.3');archive=upstream.parent/'libjpeg-turbo-2.1.3.zip'
assert sha(archive)=='cc868c30e5ed5ae08a103ed726e7cd235d8d7208124cea1a4740bd8b94482eb9'
backend=Path('src/VisionPlugins.ImageSource/runtimes/win-x64/native/turbojpeg.dll')
result={'schema':1,'runtimeSource':str(args.runtime),'runtimeSha256':sha(args.runtime),'targetMediaSha256':sha(media),'observedVersion':{'fileOffset':version_offset,'text':version.decode()},'nativeSourceAnalysis':[{'path':str(p),'sha256':sha(p)} for p in [analysis/'18038c8a0.c',analysis/'1802b5590.c',analysis/'1802b5980.c']],'nativeParameters':{'header':'0x1802b5590: tjDecompressHeader3 signature and body correlate with upstream','decode':'0x1802b5980: tjDecompress2 signature and body correlate with upstream','flags':0,'rgbPixelFormat':0,'grayPixelFormat':6,'pitch':0,'warningPolicy':'warning code 0 accepted, fatal code 1 rejected'},'independentBackend':{'path':str(backend),'sha256':sha(backend),'upstream':'https://github.com/libjpeg-turbo/libjpeg-turbo/tree/2.1.3','archiveSha256':sha(archive),'configuration':'Windows x64, MSVC Release, WITH_SIMD=OFF, WITH_CRT_DLL=OFF','sourceHashes':{p.name:sha(p) for p in [upstream/'turbojpeg.c',upstream/'jdapimin.c',upstream/'jdsample.c',upstream/'jidctint.c']},'limitation':'Independent upstream build; scalar backend performance is not equivalent to vendor SIMD build'},'observations':observations,'completeParity':False}
result['nativeSourceAnalysis'] += [{'path':str(p),'sha256':sha(p)} for p in [analysis/'18038b190.c',analysis/'180389440.c',analysis/'1803889b0.c']]
result['conversionBoundary'] = 'Original RGB-to-MONO conversion dispatches through 0x18038b190 -> 0x180389440 -> 0x1800031b0. Observed small-color failures are retained without assuming a general minimum geometry rule; kernel remains unrecovered.'
result['fixtureGenerator']={'path':'tests/NativeEvidence/Create-JpegCorpus.py','sha256':sha(Path('tests/NativeEvidence/Create-JpegCorpus.py')),'encoderSha256':sha(Path('analysis/build/jpeg-backend/cjpeg.exe'))}
Path('docs/plugins/image-source-jpeg-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8')
print('Exported 27 original JPEG MONO8/RGB observations and source/backend identities')
