import json
import pathlib
import struct

root = pathlib.Path(__file__).resolve().parents[2]
directory = root / 'analysis/map-calibration/grid-permutations'
assert json.loads((directory/'process-status.json').read_text(encoding='utf-8-sig'))['exitCode'] == 0
rows = [json.loads(line) for line in (directory/'image-results.jsonl').read_text(encoding='utf-8-sig').splitlines() if line.strip()]
assert len(rows) == 4
original = next(json.loads(line) for line in (root/'analysis/map-calibration/image-grid-trace/grid-trace.jsonl').read_text().splitlines()
                if json.loads(line)['sequence'] == 2)
report = []
for row in rows:
    variant = row['variant']
    pixels = (directory/f'grid-{variant}-pixels.bin').read_bytes()
    indices = (directory/f'grid-{variant}-indices.bin').read_bytes()
    prefix = (directory/f'grid-{variant}-prefix.bin').read_bytes()
    xy = sum(struct.pack('<f', point[axis]) == pixels[i*8+axis*4:i*8+axis*4+4]
             for i, point in enumerate(original['points']) for axis in range(2))
    ids = sum(point[axis+2] == struct.unpack_from('<i', indices, i*8+axis*4)[0]
              for i, point in enumerate(original['points']) for axis in range(2))
    assert row['count'] == original['count'] == 168 and xy == ids == 336
    assert [row['width'], row['height']] == original['dimensions']
    assert struct.pack('<f', row['score']) == struct.pack('<f', original['score'])
    changed = [i for i, value in enumerate(prefix) if value != 0xa5]
    assert changed in (list(range(120, 124)), list(range(120, 128)))
    assert all(prefix[i] == 0 for i in changed)
    report.append(dict(variant=variant, count=168, xyBits=xy, indexFields=ids, clearedPrefixOffsets=changed))
(directory/'comparison.json').write_text(json.dumps(dict(cases=4, maxGrid=[300,300], workspacePrefix=128,
    sourceStrategyImplemented=False, productIntegrated=False, report=report), indent=2))
print('4 original-grid order diagnostics: 2688 XY/index fields exact; negative-prefix clearing recorded')
