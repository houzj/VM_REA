"""Independent source arithmetic at the one 90-degree candidate, no native run."""
import json
import math
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d1-source-chain"
index = json.loads((out / "source-index.json").read_text(encoding="utf-8"))
constants = {row["va"]: row["values"] for row in index["constants"]}
f = lambda value: struct.unpack("<f", struct.pack("<f", value))[0]
pi = constants["0x18161c268"][3]
delta = f(constants["0x1816180d0"][5] / f(30))
start = -f(f(.5) * delta)
window = f(start - pi)
center = f(window + pi)
for _ in range(7):
    center = f(center - delta)
    window = f(window - delta)
low, fold, high, one = constants["0x1816189c0"]

def phase(value):
    result = f(value - int(value))
    if result < low:
        result = f(result + one)
    elif fold < result:
        result = f(result - one)
    return result

rows = []
for cy in range(100):
    for cx in range(100):
        wx = f(f(f(math.cos(center)) * f(29.5)) + f(cx))
        wy = f(f(f(math.sin(center)) * f(29.5)) + f(cy))
        x0 = (wx - 21 * math.cos(window)) + 3 * math.sin(window)
        y0 = (wy - 21 * math.sin(window)) - 3 * math.cos(window)
        px, py = phase(x0), phase(y0)
        rows.append({"precenter": [cx, cy], "windowCenter": [wx, wy],
                     "corner0": [x0, y0], "phase": [px, py],
                     "passX": low <= px <= high, "passY": low <= py <= high})
report = {"kind": "independent source arithmetic, host math sin/cos; not original runtime",
          "binarySha256": index["binarySha256"], "precenterDomain": "integer x/y 0..99",
          "candidateK": 7, "centerAngle": center, "windowAngle": window,
          "rounding": "nearest-even float; original CRT last bits and ambient MXCSR unverified",
          "testedCenters": len(rows), "passBoth": sum(r["passX"] and r["passY"] for r in rows),
          "xPhaseRange": [min(r["phase"][0] for r in rows), max(r["phase"][0] for r in rows)],
          "yPhaseRange": [min(r["phase"][1] for r in rows), max(r["phase"][1] for r in rows)],
          "example": rows[2525]}
(out / "phase-source-replay.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps(report))
