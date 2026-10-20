"""Replay current source float operations; never executes the original target."""
import json
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d1-source-chain"
index = json.loads((out / "source-index.json").read_text(encoding="utf-8"))
constants = {row["va"]: row["values"] for row in index["constants"]}
f = lambda value: struct.unpack("<f", struct.pack("<f", value))[0]
pi = constants["0x18161c268"][3]
full = constants["0x1816180d0"][5]
factor, period, low, high = constants["0x1816189b0"]
delta = f(full / f(30))
angle = f(-f(f(.5) * delta) - pi)
rows = []
for k in range(30):
    degree = f(f(factor) * angle)
    rem = f(degree - f(f(int(f(degree / period))) * period))
    normalized = rem if rem >= 0 else f(period + rem)
    bucket = int(normalized)
    fraction = f(normalized - f(bucket))
    generic = fraction <= low or fraction >= high or bucket == -1
    rows.append({"k": k, "windowAngle": angle, "degree": normalized,
                 "bucket": bucket, "fraction": fraction,
                 "angleGeneric": generic,
                 "axisCandidateBeforeCornerPhase": not generic and bucket in (0, 90, 180, 270)})
    angle = f(angle - delta)
report = {"kind": "independent source arithmetic replay; not original runtime",
          "rounding": "nearest-even per float operation; ambient native environment unverified",
          "binarySha256": index["binarySha256"], "delta": delta, "rows": rows}
(out / "angle-source-replay.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"generic": sum(row["angleGeneric"] for row in rows),
                  "nonGeneric": [row for row in rows if not row["angleGeneric"]]}))
