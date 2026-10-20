import pathlib,json,hashlib
root=pathlib.Path.cwd();probe=root/'tests/CalibrationBoard.NativeProbe/CircleWrapperProbe.cpp'
rows=[json.loads(line) for line in (root/'analysis/calibration-board/circle-wrapper-native/runtime.jsonl').read_text().splitlines()]
assert len(rows)==15
assert all(r['code']==(0x1000001a if r['reducedHeight']<64 else 0x10000107) for r in rows)
report={'authority':'Original 877f90 invoked after public SDK creation, original/reduced white mono buffers and owned workspace. No devices/platform.', 'probeSha256':hashlib.sha256(probe.read_bytes()).hexdigest(),'sourceRule':'877f90 requires ROI and reduced axes >63. Direct failure 0x1000001a maps at board wrapper to 0x10000107. Default factor5 is source table; factors2..6 are controlled private helper inputs, not exposed user parameters.','rows':rows}
(root/'docs/plugins/calibration-board-circle-wrapper-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
print('Exported',len(rows),'original wrapper size cases')
