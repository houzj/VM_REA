"""Export controlled official generator/algorithm evidence; fail on missing successful board runs."""
import json,pathlib,hashlib
root=pathlib.Path(__file__).resolve().parent.parent
audit=root/'analysis/full-audit/n-image-calibration/board-generator'
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
producer=root/'tests/NativeEvidence/OriginalNImageBoardGeneratorProbe.py'
records=[]
for kind,board in [(1,7),(2,6),(3,9),(4,8)]:
 generation=json.loads((audit/f'board-{kind}.json').read_text())
 if generation['producerSha256']!=sha(producer):raise ValueError('Generator producer changed; rerun')
 if generation['imageSha256']!=sha(audit/f'board-{kind}.jpg'):raise ValueError('Generator image changed')
 path=audit/(f'runtime-{kind}.jsonl' if kind in (1,3) else f'runtime-large-{kind}.jsonl')
 rows=[json.loads(s) for s in path.read_text().splitlines()]
 native=next(r for r in rows if r.get('phase')=='run' and r.get('board')==board and r.get('count')==9 and r.get('code')==0)
 prefix=f'generated-{kind}' if kind in (1,3) else f'large-{kind}'
 inputs=[{'frame':i,'sha256':sha(audit/f'{prefix}-{i}.bmp')} for i in range(9)]
 failures=[]
 if kind in (2,4):
  failures=[r for r in [json.loads(s) for s in (audit/f'runtime-{kind}.jsonl').read_text().splitlines()] if r.get('phase')=='run' and r.get('board')==board and r.get('count')==9]
 records.append({'generatorType':kind,'algorithmBoardType':board,'generatorReturn':generation['return'],'generatorCounts':generation['counts'],'generatorSha256':generation['generatorSha256'],'nativeGeneratorSha256':generation['nativeSha256'],'generatorImageSha256':generation['imageSha256'],'locateRadii':generation['locateRadii'],'codeRadius':generation['codeRadius'],'wrapperMarshalSha256':sha(audit/'extracted/DemoGenCalBoard_en.marshal'),'inputCanvas':[1000,1000] if kind in (1,3) else [2000,2000],'inputFixtures':inputs,'nativeOutputSha256':sha(path),'native':native,'smallerCanvas1000Output':failures})
output={'authority':'controlled official board-generator and official algorithm SDK; no physical equipment; not full platform module','generatorAbiEvidence':'shipped Python 3.6 wrapper CALBOARD_INPUT/HKA_Point_F and gen_calibration 12 positional arguments; extracted without modification','parameters':{'rows':16,'columns':16,'gridPitchMillimeters':10,'nativeMarkDistanceMeters':.01,'hikICodeScale':1,'characterMarks':0},'generatorProducerSha256':sha(producer),'algorithmProducerSha256':sha(root/'tests/NativeEvidence/OriginalNImageAlgorithmProbe.cpp'),'focusedAlgorithmProducerSha256':sha(root/'tests/NativeEvidence/OriginalNImageBoardProbe.cpp'),'sampling':'official JPEG -> System.Drawing HighQualityBicubic, white canvas, 90% size, origin margin 5%; frame shifts (i%3)*10,(i//3)*10','records':records,'limits':['independent detector numerical results are separate from these native outputs','1000px HIK-II/expanded-II official failure is preserved, not reclassified as success','arbitrary perspective detection and complete tool lifecycle remain unverified']}
rotation=audit.parent/'rotated-generator'
output['rotated']={'angleDegrees':7,'sourceFixture':'tests/NImageCalibration.PluginChecks/Fixtures/hik-generated-1.png','producer':'tests/NImageCalibration.PluginChecks/NImageGeneratedBoardChecks.cs','producerSha256':sha(root/'tests/NImageCalibration.PluginChecks/NImageGeneratedBoardChecks.cs'),'sampling':'inverse rotation, bilinear byte rounding, 1400 square; then nine 10px translations','native':next(r for r in [json.loads(s) for s in (rotation/'runtime.jsonl').read_text().splitlines()] if r.get('phase')=='run'),'nativeOutputSha256':sha(rotation/'runtime.jsonl'),'inputHashes':[sha(rotation/f'rotated-{i}.bmp') for i in range(9)]}
perspective=audit.parent/'perspective-generator'
output['perspective']={'homography':[1,.08,180,.04,1,150,.00015,.00008,1],'sourceFixture':'tests/NImageCalibration.PluginChecks/Fixtures/hik-generated-1.png','producer':'tests/NImageCalibration.PluginChecks/NImagePerspectiveChecks.cs','producerSha256':sha(root/'tests/NImageCalibration.PluginChecks/NImagePerspectiveChecks.cs'),'sampling':'inverse homography fit, bilinear byte rounding, 1400 square; then nine 10px translations','native':next(r for r in [json.loads(s) for s in (perspective/'runtime.jsonl').read_text().splitlines()] if r.get('phase')=='run'),'nativeOutputSha256':sha(perspective/'runtime.jsonl'),'inputHashes':[sha(perspective/f'perspective-{i}.bmp') for i in range(9)]}
output['fixtureProducers']={str(p.relative_to(root)):sha(p) for p in [root/'tests/NativeEvidence/Export-NImageGeneratedFixtures.ps1',root/'tests/NativeEvidence/Export-NImageGeneratedLargeFixtures.ps1']}
output['trackedSamples']=[{'path':str(p.relative_to(root)),'sha256':sha(p)} for p in sorted((root/'tests/NImageCalibration.PluginChecks/Fixtures').glob('hik-generated-*.png'))]
(root/'docs/plugins/n-image-calibration-generated-evidence.json').write_text(json.dumps(output,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('Exported official generated-board evidence:',[(r['algorithmBoardType'],r['native']['pairs']) for r in records])


