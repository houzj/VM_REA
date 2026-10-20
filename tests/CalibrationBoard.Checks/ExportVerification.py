import json, pathlib, hashlib, shutil
root=pathlib.Path.cwd()
latest=max((p for p in (root/'analysis/calibration-board').glob('verification-*/summary.json') if json.loads(p.read_text(encoding='utf-8-sig')).get('scope') not in ('roi-only','grid-only','recognition-only','checker-mask-only','encoded-mask-only','encoded-output-fit-only','encoded-full-product-only','enclosing-circle-only','boundary-candidates-only','boundary-runtime-only','checker-runtime-only','encoded-mask-runtime-only','encoded-axis-runtime-only')),key=lambda p:p.stat().st_mtime)
verification=json.loads(latest.read_text(encoding='utf-8-sig'))
verification['stderr']= (latest.parent/'stderr.log').read_text(encoding='utf-8-sig',errors='replace')
if verification['exitCode']==0:
 shutil.copyfile(root/'analysis/calibration-board/complete-output-differences.json',root/'docs/plugins/calibration-board-output-differences.json')
source=[{'path':str(p.relative_to(root)).replace('\\','/'),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()} for p in sorted((root/'src/VisionPlugins.CalibrationBoard').glob('*.cs'))]
snapshot=latest.parent/'sources.json'
if snapshot.exists(): source=json.loads(snapshot.read_text(encoding='utf-8-sig'))
report={'status':'partial-implementation-awaiting-product-acceptance' if verification['exitCode']==0 else 'regression-failed-see-managedVerification-stderr','managedVerification':verification,'runDirectory':str(latest.parent),'sources':source,'originalSdkCases':12,'additionalEncodedSdkOutputCases':88,'encodedRotatedMaskSdkCases':84,'encodedPreDecodeProjectionCases':21,'originalParserStates':7,'nativeIwcalFiles':10,'pointTolerancePixels':0.03,'knownDifference':'Checker raw recognition still has known float-coordinate differences; current complete-output fields are recorded separately and no subpixel parity is claimed.','manualAccepted':False,'originalDesktopCompared':False,'remaining':['Other board types mask recognition and actual host display acceptance','Product registration and original/UI manual acceptance','General refinement beyond recorded circle/window/grid cases','General board recognition outside recorded source-exact fixtures','General inputs beyond recorded source-exact two-stage fitting and SDK parameter cases','Original platform-module failure propagation and full lifecycle']}
(root/'docs/plugins/calibration-board-verification.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(verification['result'].strip(),latest.parent)



