import pathlib,json,hashlib
root=pathlib.Path.cwd();probe=root/'tests/CalibrationBoard.NativeProbe/CircleEdgeProbe.cpp'
rows=[json.loads(line) for line in (root/'analysis/calibration-board/circle-edges/fallback.jsonl').read_text().splitlines()]
assert len(rows)==9
report={'authority':'Original 8620c0 -> 7582e0 -> 8a9f50, owned binary crops after normal public SDK creation; foreground frame with all-white, empty interior or irregular holes','sourceSha256':hashlib.sha256(probe.read_bytes()).hexdigest(),'rows':rows,'limits':'Foreground frame branch only; arbitrary disconnected or nonrectangular foreground remains open.'}
(root/'docs/plugins/calibration-board-fallback-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
print('Exported original frame fallback cases',len(rows))
