import pathlib,json,hashlib
root=pathlib.Path.cwd()
rows=[json.loads(s) for s in (root/'analysis/calibration-board/fit-dispatch-native/runtime.jsonl').read_text().splitlines()]
assert len(rows)==27
(root/'docs/plugins/calibration-board-fit-dispatch-fixtures.jsonl').write_text(''.join(json.dumps(r,separators=(',',':'))+'\n' for r in rows))
mapping=root/'analysis/calibration-board/weight-mapping-native/runtime.jsonl'
(root/'docs/plugins/calibration-board-weight-mapping-fixtures.jsonl').write_bytes(mapping.read_bytes())
report={'authority':'Original core guard 7ecea0 dispatching 7ed0c0, SDK Create initializes owned process. Exact synthetic float points; no device/global platform.',
 'source':{'publicSetter':'7c6830 0x40001 -> engine9c; 0x40002 value4 -> enginea0=3','defaults':'7c9320 engineb4=1','caller':'7f7050 options+18=engineb4; +1c=engine9c','fit':'7ed0c0 +18==1 has normalized-grid-to-image and image-to-world fits; Weight1 8345e0+8006e0, Weight2/3 7ff3c0(20,1e-4)'},
 'center':{'floatX':240,'floatY':200,'packedBits':'0x4348000043700000'},
 'options':{'originX':2,'originY':-3,'angleRadians':.3,'coordinateMode':1,'pitch':2,'calibrationKind':1,'nativeWeightFactor':2},
 'cases':27,'success':sum(r['code']==0 for r in rows),'errors':[{'index':r['index'],'code':hex(r['code'])} for r in rows if r['code']],
 'files':{str(p.relative_to(root)).replace('\\','/'):hashlib.sha256(p.read_bytes()).hexdigest() for p in [root/'tests/CalibrationBoard.NativeProbe/FitDispatchProbe.cpp',root/'tests/CalibrationBoard.NativeProbe/WeightMappingProbe.cpp']},
 'limits':['Core-wrapper probes establish fitting dispatch, not original public image recognition parity.','Older zero-anchor fixtures preserve integer bit-pattern center from preliminary probe; they are separate controlled inputs, not float center240/200.','No product parity follows from helper assertions.']}
(root/'docs/plugins/calibration-board-fit-dispatch-evidence.json').write_text(json.dumps(report,indent=2)+'\n')
print(report['success'],'successes of',len(rows))
