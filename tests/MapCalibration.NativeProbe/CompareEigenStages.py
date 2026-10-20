"""Source-order 8bcba0 reconstruction, checked against owned-process binary doubles."""
import json
import math
import pathlib
import struct

root = pathlib.Path(__file__).resolve().parents[2]
directory = root / 'analysis/map-calibration/image-eigen-trace'
rows = [json.loads(line) for line in (directory / 'eigen-trace.jsonl').read_text().splitlines() if line.strip()]
report = []
for row in rows:
    sequence = row['sequence']
    def payload(kind, count):
        return struct.unpack('<' + str(count) + 'd', (directory / f'eigen-{sequence:03}-{kind}.double').read_bytes())
    a, b, c, d = payload('input', 4)
    trace = a + d
    negative = -trace
    discriminant = negative * negative - (d * a - c * b) * 4.0
    if discriminant < 0:
        assert row['status'] == 0x10000000
        report.append(dict(sequence=sequence, status=row['status'], fields=0, exact=0))
        continue
    assert row['status'] == 0
    if discriminant == 0:
        values = [trace * .5, trace * .5]
    else:
        low = -(negative + math.sqrt(discriminant)) * .5
        high = (math.sqrt(discriminant) - negative) * .5
        values = [low, high] if high <= low else [high, low]
    vectors = [0., 0., 0., 0.]
    if abs(b) <= .0001 or abs(c) <= .0001:
        if abs(a) <= .0001:
            if abs(d) > .0001:
                vectors = [0., 1., 1., 0.]
        else:
            vectors = [1., 0., 0., 1.]
    else:
        for i, value in enumerate(values):
            denominator = -(b * c) if i == 0 else -b * c
            x = ((d - value) * (value - a)) / denominator
            y = ((value - a) * x) / b
            if abs(x) >= .0001 or abs(y) >= .0001:
                norm = math.sqrt(x * x + y * y)
                x = x / norm
                y = y / norm
                if abs(y) > abs(x) and y < 0:
                    x, y = -x, -y
            vectors[i*2:i*2+2] = [x, y]
    actual = values + vectors
    expected = payload('values', 2) + payload('vectors', 4)
    exact = sum(struct.pack('<d', x) == struct.pack('<d', y) for x, y in zip(actual, expected))
    if exact != 6:
        raise AssertionError((sequence, actual, expected, exact))
    report.append(dict(sequence=sequence, status=0, fields=6, exact=exact))
assert len(report) == 96
(directory / 'comparison.json').write_text(json.dumps(dict(cases=96, fields=sum(row['fields'] for row in report),
    exact=sum(row['exact'] for row in report), productIntegrated=False, report=report), indent=2))
print(f"96 native direction-matrix analyses: {sum(row['exact'] for row in report)} double fields exact")
