import json,pathlib,hashlib
root=pathlib.Path.cwd();probe=root/'tests/CalibrationBoard.NativeProbe/RoiRotatedProbe.cpp'
rows=[json.loads(s) for s in (root/'analysis/calibration-board/roi-rotated-native/runtime.jsonl').read_text().splitlines()]
assert len(rows)==21 and all(r['code']==0 and len(r['points'])==r['count'] for r in rows)
report={'authority':'Original public board SDK with valid rotated rectangular ROI and independent public PreproMask bbox. No devices/platform initialization.', 'probeSha256':hashlib.sha256(probe.read_bytes()).hexdigest(),'limits':'Circle board only. Fully outside rectangle caused original PreproMask fail-fast 0xc0000409 and is independently archived; not an expected recognition result.', 'rows':rows}
(root/'docs/plugins/calibration-board-roi-rotated-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
print('Exported',len(rows),'valid original rotated ROI runs')
