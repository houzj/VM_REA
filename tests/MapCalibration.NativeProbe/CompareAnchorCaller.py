"""Strictly compare captured real SDK 813ca0 -> 813020 sequences.

No product detector output is used as a golden value. Orientation/anchor remain
captured inputs rather than assumed values inferred from an image transform.
"""
import hashlib
import json
import struct
import sys
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration" / (sys.argv[1] if len(sys.argv) > 1 else "image-anchor-caller-trace")
trace_path = out / "tag-trace.jsonl"
rows = [json.loads(x) for x in trace_path.read_text(encoding="utf-8-sig").splitlines()]
sdk = [json.loads(x) for x in (out / "image-results.jsonl").read_text(encoding="utf-8-sig").splitlines()]
errors, summaries = [], []
checked = 0
bits = lambda value: struct.pack("<f", value)
def check(equal, message):
    global checked
    checked += 1
    if not equal:
        errors.append(message)

for sequence in sorted({x["sequence"] for x in rows}):
    events = [x for x in rows if x["sequence"] == sequence]
    phases = [x["phase"] for x in events]
    expected_phases = ["search-entry", "decode-attempt", "selected-tag", "map-entry", "map-return"]
    check(phases == expected_phases, f"sequence {sequence}: unexpected event order {phases}")
    if phases != expected_phases:
        continue
    search, attempt, tag, entry, result = events
    board = entry["board"]
    w, h, direction = entry["width"], entry["height"], entry["orientation"]
    check(tag["status"] == 1 and tag["attempts"] == 1 and attempt["statusBefore"] == 0, f"sequence {sequence}: tag status/attempts")
    check(entry["decodeRotation"] == direction, f"sequence {sequence}: decode direction transport")
    b = tag["bytes"]
    expected_origin = [b[0], b[1]] if board == 7 else [(b[0] & 15) * 64 + (b[1] >> 2), (b[1] & 3) * 256 + b[2]]
    check([entry["originX"], entry["originY"]] == expected_origin, f"sequence {sequence}: decoded origin")
    check(entry["callerReturnRva"] == {7: 0x80f8ed, 9: 0x80cf17}[board], f"sequence {sequence}: native caller")
    grid = {(g[0], g[1]): g for g in entry["grid"]}
    check(len(grid) == w * h, f"sequence {sequence}: full grid coverage")
    check([search["width"], search["height"]] == [w, h], f"sequence {sequence}: search/map dimensions")
    check(search["grid"] == [g[:4] for g in entry["grid"]], f"sequence {sequence}: search grid including sentinels unchanged")
    # Source caller orientation groups adjust the selected search anchor by
    # offsets/radius before handing it to 813020.
    # Historical captures label stack param_5 as column and param_6 as row.
    # Source recovery establishes the opposite. Both were 1 in those captures;
    # retain raw files and interpret explicitly instead of rewriting evidence.
    labels_confirmed = search.get("offsetAxisLabels") == "source-confirmed"
    offset_column = search["offsetColumn" if labels_confirmed else "offsetRow"]
    offset_row = search["offsetRow" if labels_confirmed else "offsetColumn"]
    column = offset_column + tag["anchor"][0] - (2 * entry["searchRadius"] if direction in (1, 4, 5, 8) else 0)
    row = offset_row + tag["anchor"][1] - (2 * entry["searchRadius"] if direction in (1, 2, 5, 6) else 0)
    check([entry["anchorColumn"], entry["anchorRow"]] == [column, row], f"sequence {sequence}: caller anchor adjustment")
    if direction in (1, 5):
        order = [(x, y) for y in range(h) for x in range(w)]
    elif direction in (2, 6):
        order = [(x, y) for x in reversed(range(w)) for y in range(h)]
    elif direction in (3, 7):
        order = [(x, y) for y in reversed(range(h)) for x in reversed(range(w))]
    elif direction in (4, 8):
        order = [(x, y) for x in range(w) for y in reversed(range(h))]
    else:
        raise ValueError(f"Unsupported captured orientation {direction}")
    expected = []
    for slot in order:
        _, _, ix, iy, px, py = grid[slot]
        if ix == -2147483648:
            continue
        dx, dy = ix - entry["anchorColumn"], iy - entry["anchorRow"]
        local = {1: (dx, dy), 2: (dy, -dx), 3: (-dx, -dy), 4: (-dy, dx),
                 5: (dy, dx), 6: (-dx, dy), 7: (-dy, -dx), 8: (dx, -dy)}[direction]
        expected.append([local[0] + expected_origin[0], local[1] + expected_origin[1], px, py])
    check(result["status"] == 1, f"sequence {sequence}: mapper return")
    check(result["dimensions"] == ([h, w] if direction in (2, 4) else [w, h]), f"sequence {sequence}: dimensions")
    check(result["countFromInputSentinels"] == len(expected) == len(result["points"]), f"sequence {sequence}: count")
    for i, (actual, target) in enumerate(zip(result["points"], expected)):
        for field in range(4):
            check(actual[field] == target[field] if field < 2 else bits(actual[field]) == bits(target[field]), f"sequence {sequence}: map point {i} field {field}")
    public = sdk[(sequence - 1) // 2]["source" if sequence % 2 else "target"]
    check(len(public) == len(expected), f"sequence {sequence}: public SDK count")
    for i, (actual, target) in enumerate(zip(public, expected)):
        for field, source_field in enumerate((2, 3, 0, 1)):
            check(actual[source_field] == target[field] if field < 2 else bits(actual[source_field]) == bits(target[field]), f"sequence {sequence}: public SDK point {i} field {field}")
    # Record a geometric association as an observation, not a general anchor rule.
    first = tag["opening"][:2]
    valid = [g for g in entry["grid"] if g[2] != -2147483648]
    nearest = min(valid, key=lambda g: (g[4] - first[0]) ** 2 + (g[5] - first[1]) ** 2)
    summaries.append({"sequence": sequence, "board": board, "orientation": direction, "origin": expected_origin,
                      "tagAnchor": tag["anchor"], "mapAnchor": [entry["anchorColumn"], entry["anchorRow"]],
                      "dimensions": [w, h], "sentinels": w * h - len(expected), "points": len(expected),
                      "searchOffsets": [offset_column, offset_row], "rawOffsetLabelsSourceConfirmed": labels_confirmed, "searchRadius": entry["searchRadius"],
                      "searchMapGridIdentical": search["grid"] == [g[:4] for g in entry["grid"]],
                      "openingFirstNearestValidGrid": nearest,
                      "openingFirstDistance": ((nearest[4] - first[0]) ** 2 + (nearest[5] - first[1]) ** 2) ** .5})

check(len(summaries) == 4, "Expected exactly four real SDK mapping calls")
hash_file = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
report = {"scope": "Real SDK images HIK7/9: payload-origin transport and full mapper/public SDK point order, IDs and float bits; not arbitrary orientation parity",
          "identity": json.loads((out / "identity.json").read_text(encoding="utf-8-sig")),
          "traceSha256": hash_file(trace_path), "sdkResultsSha256": hash_file(out / "image-results.jsonl"),
          "sourceSha256": {a: hash_file(root / f"analysis/map-calibration/ghidra/core/{a}.c") for a in ("18080e690", "18080bcd0", "180813ca0", "180813020")},
          "checkedFields": checked, "calls": summaries, "differences": errors}
(out / "comparison.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"calls": len(summaries), "checkedFields": checked, "differences": len(errors)}))
if errors:
    print("\n".join(errors[:10]))
raise SystemExit(bool(errors))
