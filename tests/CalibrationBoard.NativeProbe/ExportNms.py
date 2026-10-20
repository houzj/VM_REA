import pathlib,json,hashlib
root=pathlib.Path.cwd();source=root/'tests/CalibrationBoard.NativeProbe/CircleEdgeProbe.cpp'
rows=[json.loads(line) for line in (root/'analysis/calibration-board/circle-edges/nms.jsonl').read_text().splitlines()]
assert len(rows)==72
report={'authority':'Original pure 0x1808b30c0 with original signed Sobel gradients; normal public SDK instance; owned 32x32 crop buffers','sourceSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'rows':rows,'limits':'Manual thresholds only; no contour connectivity or full board equivalence claim'}
(root/'docs/plugins/calibration-board-nms-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
print('Exported original NMS fixtures',len(rows))
