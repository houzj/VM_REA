"""Audit native descriptor payloads without treating managed results as goldens."""
import collections
import json
import pathlib
import struct

root = pathlib.Path(__file__).resolve().parents[2]
directory = root / 'analysis/map-calibration/image-candidate-descriptor-trace'
rows = [json.loads(line) for line in (directory / 'candidate-trace.jsonl').read_text().splitlines()]
report = []
for row in rows:
    if row.get('phase') != 'descriptorConfig':
        continue
    sequence = row['sequence']
    before = (directory / f'candidate-{sequence:03}-entry.bin').read_bytes()
    after = (directory / f'candidate-{sequence:03}-return.bin').read_bytes()
    assert len(before) % 28 == len(after) % 28 == 0
    words = row['words']
    configuration = [words[0]] + [struct.unpack('<f', struct.pack('<I', value))[0] for value in words[1:]]
    # Match bytes, preserving input order even when the native phase rejects points.
    cursor = 0
    for offset in range(0, len(after), 28):
        xy = after[offset:offset + 8]
        while cursor < len(before) and before[cursor:cursor + 8] != xy:
            cursor += 28
        assert cursor < len(before), f'Descriptor changed XY/order: {sequence}'
        cursor += 28
    radii = collections.Counter(struct.unpack_from('<i', after, i + 8)[0] for i in range(0, len(after), 28))
    report.append(dict(sequence=sequence, configuration=configuration, input=len(before)//28,
                       accepted=len(after)//28, xyAndOrderUnchanged=True, radiusWords=dict(radii)))
assert len(report) == 24
assert all(row['configuration'] in ([1, 3.0, 30.0, 135.0], [1, 1.75, 65.0, 110.0]) for row in report)
(directory / 'descriptor-summary.json').write_text(json.dumps(report, indent=2))
print(f"24 native descriptor calls: {sum(row['accepted'] for row in report)} accepted XY/order unchanged")
