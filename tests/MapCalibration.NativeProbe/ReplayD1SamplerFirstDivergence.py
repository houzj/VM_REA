"""Current-caller arithmetic only: no image fixture or original DLL execution."""
import hashlib
import json
import math
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d1-source-chain"
index = json.loads((out / "source-index.json").read_text(encoding="utf-8"))
f = lambda value: struct.unpack("<f", struct.pack("<f", value))[0]
constants = {row["va"]: row["values"] for row in index["constants"]}
pi = constants["0x18161c268"][3]
delta = f(constants["0x1816180d0"][5] / f(30))
theta = -f(f(.5) * delta)
angle = f(theta - pi)
cx = f(f(25) + f(f(math.cos(theta)) * f(29.5)))
cy = f(f(50) + f(f(math.sin(theta)) * f(29.5)))
s, c = math.sin(angle), math.cos(angle)
sf, cf = f(s), f(c)
# Exact operation order in 704de0, including the *2/.5 source expression.
x_float = f(f(f(f(cx * f(2)) - f(f(42) * cf)) + f(sf * f(6))) * f(.5))
y_float = f(f(f(f(cy * f(2)) - f(sf * f(42))) - f(f(6) * cf)) * f(.5))
x_double = (cx - 21 * c) + 3 * s
y_double = (cy - 21 * s) - 3 * c
corners = [[x_double, y_double], [x_double + 42 * c, y_double + 42 * s]]
corners += [[2 * cx - px, 2 * cy - py] for px, py in corners[:2]]
all_inside = all(0 < px < 99 and 0 < py < 99 for px, py in corners)
if not all_inside:
    raise RuntimeError("Selected caller rectangle does not satisfy the internal sampler branch")
coordinates = []
for col in range(42):
    coordinates.append({"column": col, "ordinary": [x_float, y_float],
                        "generic": [x_double, y_double],
                        "difference": [x_float - x_double, y_float - y_double]})
    x_float, y_float = f(x_float + cf), f(y_float + sf)
    x_double, y_double = x_double + c, y_double + s

def ordinary(x, y, pixels):
    p00, p10, p01, p11 = pixels
    wx, fy = f(f(int(x) + 1) - f(x)), f(f(y) - f(int(y)))
    top = f(f(f(p00 - p10) * wx) + f(p10))
    bottom = f(f(f(p01 - p11) * wx) + f(p11))
    return int(f(f(f(top * f(f(1) - fy)) + f(.5)) + f(bottom * fy)))

def float_weights_at_double_coordinate(x, y, pixels):
    # Isolate 11-bit quantization; the coordinate and cell are unchanged.
    p00, p10, p01, p11 = pixels
    wx, fy = f((int(x) + 1) - x), f(y - int(y))
    top = f(f(f(p00 - p10) * wx) + f(p10))
    bottom = f(f(f(p01 - p11) * wx) + f(p11))
    return int(f(f(f(top * f(f(1) - fy)) + f(.5)) + f(bottom * fy)))

def fixed(x, y, pixels):
    p00, p10, p01, p11 = pixels
    a = (int(x) + 1) * 2048 - int(2048 * x)
    b = int(2048 * y) - int(y) * 2048
    top = p10 * 2048 + (p00 - p10) * a
    bottom = p11 * 2048 + (p01 - p11) * a
    return ((2048 - b) * top + b * bottom + 0x200000) >> 22

point = coordinates[0]
x, y = point["generic"]
selected = None
# Smallest deterministic two-intensity horizontal stencil at the first sample.
for left in range(256):
    for right in range(256):
        pixels = [left, right, left, right]
        float_value = float_weights_at_double_coordinate(x, y, pixels)
        fixed_value = fixed(x, y, pixels)
        if float_value != fixed_value:
            selected = {"pixelsP00P10P01P11": pixels,
                        "ordinaryAtOrdinaryCoordinate": ordinary(*point["ordinary"], pixels),
                        "floatWeightsAtGenericCoordinate": float_value,
                        "fixedAtGenericCoordinate": fixed_value}
            break
    if selected is not None:
        break
if selected is None:
    raise RuntimeError("No isolated quantization divergence in the selected finite stencil domain")
a = (int(x) + 1) * 2048 - int(2048 * x)
b = int(2048 * y) - int(y) * 2048
report = {"kind": "independent source arithmetic, not original runtime",
          "binarySha256": index["binarySha256"],
          "scriptSha256": hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
          "environment": "nearest-even float emulation, host double math sin/cos; original CRT/MXCSR not corroborated",
          "caller": {"precenter": [25, 50], "imageDimensions": [100, 100],
                     "windowCenter": [cx, cy], "windowDimensions": [42, 6],
                     "k": 0, "centerAngle": theta, "windowAngle": angle,
                     "corners": corners, "allCornersStrictlyInside": all_inside},
          "firstDifferentOperation": {"operation": "sin result float rounding versus retained double",
                                      "ordinary": sf, "generic": s},
          "coordinateSnapshots": [coordinates[i] for i in (0, 1, 41)],
          "sameCellAtFirstPoint": [int(v) for v in point["ordinary"]] == [int(x), int(y)],
          "isolatedQuantization": {"coordinate": [x, y],
                                   "unquantizedFloatWeights": [f((int(x) + 1) - x), f(y - int(y))],
                                   "integerWeights": [a, b], "normalizedIntegerWeights": [a / 2048, b / 2048],
                                   **selected}}
(out / "sampler-first-divergence.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps(report))
