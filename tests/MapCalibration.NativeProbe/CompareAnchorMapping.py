"""Compare controlled native 813020 calls with its explicit scalar mapping.

This does not exercise opening discovery, decoding, or an SDK caller lifecycle.
"""
import hashlib
import json
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/anchor-mapping-native"
rows = [json.loads(line) for line in (out / "results.jsonl").read_text(encoding="utf-8-sig").splitlines()]
errors = []
fields = 0
count = 0
for case in rows:
    w, h, missing, orientation = (case[k] for k in ("width", "height", "missing", "orientation"))
    if orientation not in range(1, 9):
        expected, dimensions, status = [], [111, 222], 0
    else:
        # Row-pointer traversal, not a post-hoc world-index sort.
        if orientation in (1, 5):
            order = [(x, y) for y in range(h) for x in range(w)]
        elif orientation in (2, 6):
            order = [(x, y) for x in reversed(range(w)) for y in range(h)]
        elif orientation in (3, 7):
            order = [(x, y) for y in reversed(range(h)) for x in reversed(range(w))]
        else:
            order = [(x, y) for x in range(w) for y in reversed(range(h))]
        expected = []
        for x, y in order:
            if missing and (x + y) % 3 == 0:
                continue
            dx, dy = x - 1, y - 1
            local = {1: (dx, dy), 2: (dy, -dx), 3: (-dx, -dy), 4: (-dy, dx),
                     5: (dy, dx), 6: (-dx, dy), 7: (-dy, -dx), 8: (dx, -dy)}[orientation]
            expected.append([local[0] + 101, local[1] + 203, x + .125 + y * 10, y + .375 + x * 10])
        # The original swaps dimensions only for 2/4, not 6/8.
        dimensions, status = ([h, w] if orientation in (2, 4) else [w, h]), 1
    for key, value in (("status", status), ("dimensions", dimensions), ("points", expected)):
        if case[key] != value:
            errors.append({"case": {k: case[k] for k in ("width", "height", "missing", "orientation")}, "field": key, "actual": case[key], "expected": value})
    fields += 3 + len(expected) * 4
    count += len(expected)
report = {"scope": "Owned-process direct native 813020; widths/heights 1..4, orientations 0..9, full/sentinel grids; not opening or payload parity",
          "cases": len(rows), "pointRecords": count, "checkedFields": fields, "differences": errors,
          "native": json.loads((out / "process-status.json").read_text(encoding="utf-8-sig")),
          "resultsSha256": hashlib.sha256((out / "results.jsonl").read_bytes()).hexdigest()}
(out / "comparison.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({k: report[k] for k in ("cases", "pointRecords", "checkedFields")}))
print(f"differences: {len(errors)}")
raise SystemExit(bool(errors) or len(rows) != 320)
