import pathlib,json,hashlib,gzip,base64
root=pathlib.Path.cwd();base=root/'analysis/calibration-board/circle-preprocess'
rows=[json.loads(line) for line in (base/'runtime.jsonl').read_text().splitlines()]
def blob(path):
 data=path.read_bytes();return {'length':len(data),'sha256':hashlib.sha256(data).hexdigest(),'gzipBase64':base64.b64encode(gzip.compress(data,mtime=0)).decode()}
fixtures=[]
for row in rows:
 if 'shape' in row:
  prefix=f"shape-{row['shape']}-contrast-{row['contrast']}"
  if row.get('factor',2)!=2: prefix+=f"-factor-{row['factor']}"
  fixtures.append({**row,'half':blob(base/(prefix+'-half.bin')),'mask':blob(base/(prefix+'-mask.bin')),'rawMask':blob(base/(prefix+'-raw-mask.bin')),'alternateMask':blob(base/(prefix+'-alternate-mask.bin')),'alternateRawMask':blob(base/(prefix+'-alternate-raw-mask.bin'))})
 if row.get('phase')=='boundary':
  prefix=f"pattern-{row['pattern']}-width-{row['width']}"
  fixtures.append({**row,'input':blob(base/(prefix+'-input.bin')),'half':blob(base/(prefix+'-half.bin'))})
report={'authority':'Source-addressed original pure image helpers; normal public SDK instance creation; owned synthetic images; no platform initialization/device/code patch',
 'dispatch':next(row for row in rows if row.get('phase')=='cpu-dispatch'),'featureDispatch':next(row for row in rows if row.get('phase')=='feature-dispatch'),'addresses':{'resize':'0x180727a50','weights':'0x180727470','otsu':'0x180724460','alternateOtsu':'0x18087db00'},
 'sourceSha256':hashlib.sha256((root/'tests/CalibrationBoard.NativeProbe/PreprocessProbe.cpp').read_bytes()).hexdigest(),'fixtures':fixtures,
 'limits':'These fixtures verify only preprocessing, not board recognition, contours, calibration or other CPU dispatches.'}
(root/'docs/plugins/calibration-board-preprocess-fixtures.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
print('Exported original preprocessing fixtures',len(fixtures))
