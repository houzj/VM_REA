import json, pathlib, hashlib, base64
root=pathlib.Path.cwd()
module=pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu')
ui=sorted((root/'analysis/full-audit/calibration-board-calibration/ui').glob('run-*/probe.json'),key=lambda p:p.stat().st_mtime)[-1]
rows=json.loads(ui.read_text(encoding='utf-8-sig'))
selected=[]
for row in rows:
 if 'items' in row:
  selected.append({'phase':row['phase'],'authority':row['authority'],'nativeParameters':False,'desktopOperation':False,'items':[i for i in row['items'] if i['values'].get('NameA') in ['CalibBoardType','Circularity','EdgeThresh','CircleBoardCircleMode','FilterStatus','PhysicalSize','HomoFreedom','CalibBoardWeightFun']]})
files=[]
for p in module.iterdir():
 if p.is_file():files.append({'path':str(p),'length':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
for p in [pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDCalibBoardCalibCpp.dll'),root/'tests/CalibrationBoard.NativeProbe/Probe.cpp',root/'tests/CalibrationBoard.NativeProbe/OriginalUiProbe.cs']:
 if p.exists():files.append({'path':str(p),'length':p.stat().st_size,'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
result={'status':'in-progress','reaEvidence':'ev_80ccadddacca09c728b4f900dafb275b2664d00c7002ca2a8be96dd1d417dd1e','sourceFiles':files,'originalParser':{'probe':str(ui),'phases':selected},'algorithmRuntime':{'authority':'original-documented-SDK/owned-synthetic-images','fixtureCount':12,'devices':False,'platformModuleInitialized':False,'desktopPixelsCompared':False},'iwcalFiles':[{'fixture':p.name,'bytes':p.stat().st_size,'base64':base64.b64encode(p.read_bytes()).decode(),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()} for p in sorted((root/'analysis/calibration-board/native').glob('board-*.iwcal'))],'remaining':['common ROISelecter integration and actual host ImageViewConfig acceptance','actual product registration and WPF acceptance','native subpixel/robust fitting and arbitrary boards','native platform-module auto-refresh failure semantics']}
(root/'docs/plugins/calibration-board-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('wrote source hashes, original-parser phases',len(selected),'native files',len(result['iwcalFiles']))

result['nativeModuleReplay']={'scope':'owned original module constructor and ProcessAlg; source-confirmed +0x15d98 SDK tool; no platform Init/device/code patch','report':'docs/plugins/calibration-board-module-runtime.jsonl','sourceSha256':hashlib.sha256((root/'tests/CalibrationBoard.NativeProbe/ModuleProbe.cpp').read_bytes()).hexdigest(),'cases':['missing path refresh false creates36','existing path refresh false preserves37 sentinel','existing path refresh true rewrites36','original ProcessOutput captures23 fields through8 synthetic host import slots']}
audit=root/'analysis/calibration-board/native-decompile/output'
result['nativeDecompile']={'provider':'existing Ghidra CLI 12.1.4','automaticAnalysisTimedOutSeconds':60,'selectedFunctionsCompleted':all((audit/name).exists() for name in ['180020240.c','180020630.c','180020d40.c']),'functions':[{'path':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()} for p in sorted(audit.glob('*.c'))],'confirmed':['ProcessOutput Scale comes from MatrixToParam.GetScale','ProcessAlg exports when path configured and file absent or refresh enabled','SetParam SaveCalibPath exports and then changes internal path']}
result['remaining']=['original desktop visual acceptance','native checker subpixel and general inputs beyond recorded fixtures','other board types rotated-mask recognition','native platform-module error propagation beyond recorded module cases']
resource_names={'AlgorithmTab.xml':module/'IMVSCalibBoardCalibModuAlgorithmTab.xml','Algorithm.xml':module/'IMVSCalibBoardCalibModuAlgorithm.xml','Display.xml':module/'IMVSCalibBoardCalibModuDisplay.xml','Module.xml':module/'IMVSCalibBoardCalibModu.xml','Groups.xml':module.parents[2]/'Group.xml'}
result['resourceParity']=[{'resource':name,'original':str(original),'sameBytes':(root/'src/VisionPlugins.CalibrationBoard/Resources'/name).read_bytes()==original.read_bytes(),'resourceSha256':hashlib.sha256((root/'src/VisionPlugins.CalibrationBoard/Resources'/name).read_bytes()).hexdigest(),'originalSha256':hashlib.sha256(original.read_bytes()).hexdigest()} for name,original in resource_names.items()]
result['fittingDispatchEvidence']='docs/plugins/calibration-board-fit-dispatch-evidence.json'
result['boundedRecognitionEvidence']=['docs/plugins/calibration-board-circle-grid-fixtures.json','docs/plugins/calibration-board-circle-window-fixtures.jsonl','docs/plugins/calibration-board-dot-mode-fixtures.jsonl']
(root/'docs/plugins/calibration-board-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')


