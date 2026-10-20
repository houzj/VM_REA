import pathlib,json,hashlib,struct
root=pathlib.Path.cwd()
probe=root/'tests/CalibrationBoard.NativeProbe/RoiProbe.cpp'
rows=[json.loads(line) for line in (root/'analysis/calibration-board/roi-native/runtime.jsonl').read_text().splitlines()]
f=lambda x:struct.unpack('<f',struct.pack('<f',x))[0]
shapes=[r for r in rows if r['phase']=='shape']
assert len(shapes)==45
assert all(f(f(r['cx'])-f(f(r['width'])*.5))==f(r['left']) and f(f(r['cy'])-f(f(r['height'])*.5))==f(r['top']) for r in shapes)
report={'authority':'Public original IMvdRectangleF getters and CalibBoard SDK Run, synthetic mono image; no platform or devices',
 'sourceSha256':hashlib.sha256(probe.read_bytes()).hexdigest(),
 'leftTopRule':'float(float(center)-float(float(size)*0.5f)); all 45 native getter pairs exactly match float bits',
 'pixelBoundsSource':'MVDCalibBoardCalibCpp SDK Run 180002ad0:151-194: independently truncate double(float(edge))+0.5; clamp origin and far edge to image. Early disjoint 0x80100025; zero rounded extent 0x1000002d.',
 'limits':'Axis-aligned rectangles only. Full SDK recognition crop phase still differs; rotated mask rasterization unverified.', 'rows':rows}
(root/'docs/plugins/calibration-board-roi-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
print('Exported',len(shapes),'getter cases and',len(rows)-len(shapes),'SDK ROI cases')
