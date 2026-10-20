import pathlib,json,hashlib,gzip,base64
root=pathlib.Path.cwd();base=root/'analysis/calibration-board/circle-edges'
rows=[json.loads(line) for line in (base/'runtime.jsonl').read_text().splitlines()]
def blob(path):
 data=path.read_bytes();return {'length':len(data),'sha256':hashlib.sha256(data).hexdigest(),'gzipBase64':base64.b64encode(gzip.compress(data,mtime=0)).decode()}
gradients=[]
for shape in range(3):
 for contrast in [2,5,10,20,50,255]:
  prefix=f'shape-{shape}-contrast-{contrast}'
  gradients.append({'shape':shape,'contrast':contrast,'gx':blob(base/(prefix+'-gx.bin')),'gy':blob(base/(prefix+'-gy.bin'))})
report={'authority':'Source-addressed original image helpers after normal public SDK instance creation; owned 32x32 crops; no platform/device/code patch',
 'addresses':{'sobelX':'0x1806a7570','sobelY':'0x1806a6c50','edgeContours':'0x1808b3d90','circleFit':'0x180877b20'},
 'sourceSha256':hashlib.sha256((root/'tests/CalibrationBoard.NativeProbe/CircleEdgeProbe.cpp').read_bytes()).hexdigest(),
 'gradients':gradients,'rows':rows,'limits':'Crop helper behavior only; not a substitute for complete board recognition or its candidate selection.'}
(root/'docs/plugins/calibration-board-edge-fixtures.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
print('Exported gradient fixtures',len(gradients),'edge/circle crops',len(rows))
