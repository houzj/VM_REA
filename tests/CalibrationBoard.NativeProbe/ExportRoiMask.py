import pathlib,json,hashlib,zlib,base64
root=pathlib.Path.cwd();probe=root/'tests/CalibrationBoard.NativeProbe/RoiMaskProbe.cpp'
rows=[json.loads(line) for line in (root/'analysis/calibration-board/roi-mask-native/runtime.jsonl').read_text().splitlines()]
assert len(rows)==64 and all(r['code']==0 for r in rows)
for row in rows:
 pixels=bytes.fromhex(row.pop('hex'));assert len(pixels)==64*48
 points=[(i%64,i//64) for i,b in enumerate(pixels) if b];assert len(points)==row['count']
 x=min(p[0] for p in points);y=min(p[1] for p in points);bounds=[x,y,max(p[0] for p in points)+1-x,max(p[1] for p in points)+1-y];assert bounds==row['bounds']
 row['pixelSha256']=hashlib.sha256(pixels).hexdigest();row['zlibBase64']=base64.b64encode(zlib.compress(pixels)).decode();row['values']=sorted(set(pixels))
report={'authority':'Public original PreproMask SDK SetInputImageSize/AddRegion(rectangle,true)/Run/GetOutputImage. No global platform, device or debug save.', 'probeSha256':hashlib.sha256(probe.read_bytes()).hexdigest(),'imageWidth':64,'imageHeight':48,'encoding':'zlib-compressed row-major bytes, base64','limits':'Shared VisionCore.RectangleRoiMaskRasterizer now reproduces all 64 original masks pixel-for-pixel and bbox; board wrapper also verifies 224 float vertices. This establishes these fixtures only, not other shape rasterizers or complete tool parity.', 'rows':rows}
(root/'docs/plugins/calibration-board-roi-mask-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
print('Exported masks',len(rows),'values',sorted({v for r in rows for v in r['values']}))
