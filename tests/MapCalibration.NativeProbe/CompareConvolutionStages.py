"""Verify native byte-to-float descriptor Sobel patches (not general float kernels)."""
import json
import pathlib
import struct

root = pathlib.Path(__file__).resolve().parents[2]
directory = root / 'analysis/map-calibration/image-convolution-trace'
rows = [json.loads(line) for line in (directory / 'convolution-trace.jsonl').read_text(encoding='utf-8-sig').splitlines() if line.strip()]
report = []
for row in rows:
    sequence, width, height = row['sequence'], row['width'], row['height']
    def payload(kind, count):
        return struct.unpack('<' + str(count) + 'f', (directory / f'convolution-{sequence:03}-{kind}.float').read_bytes())
    image, expected = payload('input', width * height), payload('output', width * height)
    kernel = payload('kernel', 9)
    assert all(0 <= value <= 255 and value == int(value) for value in image)
    assert kernel in ((1, 0, -1, 2, 0, -2, 1, 0, -1), (1, 2, 1, 0, 0, 0, -1, -2, -1))
    # 756f10.asm computes both anchors as (kernelDimension-1)>>1.
    # All products/sums in this source byte-domain Sobel operation are exactly representable floats.
    actual = [sum(kernel[ky * 3 + kx] * image[min(height-1, max(0, y+ky-1))*width + min(width-1, max(0, x+kx-1))]
                  for ky in range(3) for kx in range(3)) for y in range(height) for x in range(width)]
    exact = sum(struct.pack('<f', a) == struct.pack('<f', b) for a, b in zip(actual, expected))
    assert exact == len(expected), f'Native convolution differs: {sequence}'
    report.append(dict(sequence=sequence, width=width, height=height, exact=exact, fields=len(expected)))
assert len(report) == 48
(directory / 'comparison.json').write_text(json.dumps(dict(cases=48, exact=sum(row['exact'] for row in report),
    sourceCenterAnchor=True, byteDomainOnly=True, productIntegrated=False, report=report), indent=2))
print(f"48 native byte-domain Sobel patches: {sum(row['exact'] for row in report)} float fields exact")
