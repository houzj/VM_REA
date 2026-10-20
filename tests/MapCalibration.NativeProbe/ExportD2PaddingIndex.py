"""Index padding address evidence and derive offsets, never dereference a target."""
import hashlib
import json
import re
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d2-padding-address"
binary = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll")
digest = hashlib.sha256(binary.read_bytes()).hexdigest()
assert digest == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
sources = sorted((out / "source").glob("*.c"))
sources.append(root / "analysis/map-calibration/ghidra/core/1807d3e90.c")
entries = []
for source in sources:
    text = source.read_text(encoding="utf-8-sig")
    asm = source.with_suffix(".asm")
    entries.append({"address": source.stem, "source": str(source.relative_to(root)),
                    "sourceSha256": hashlib.sha256(source.read_bytes()).hexdigest(),
                    "assemblySha256": hashlib.sha256(asm.read_bytes()).hexdigest() if asm.exists() else None,
                    "calls": sorted(set(re.findall(r"FUN_([0-9a-f]+)\(", text)) - {source.stem})})

def offsets(cap_width, cap_height, actual_width):
    align = lambda size: (size + 127) & -128
    row_bytes = align(cap_height * 8)
    pair_bytes = align(cap_width * cap_height * 8)
    ids = 0x80 + row_bytes * 2 + pair_bytes * 2
    grid = ids + pair_bytes
    return {"capacity": [cap_width, cap_height], "actualWidth": actual_width,
            "rawPairCapacity": cap_width * cap_height,
            "rawPairBytes": cap_width * cap_height * 8,
            "rowPointerReservationBytes": row_bytes, "pairReservationBytes": pair_bytes,
            "integerOutputListOffset": ids, "integerGridOffset": grid,
            "firstNegativeCellOffset": grid - 8,
            "firstNegativeCellIsOutputListTail": grid - 8 == ids + pair_bytes - 8,
            "tailKind": "last declared pair" if pair_bytes == cap_width * cap_height * 8 else "alignment padding after declared pairs",
            "radius2Column1WindowRows": [{"row": row, "relativeToGridBase": row * actual_width * 8 - 8,
                                         "aliasesPreviousRowLastCell": row > 0}
                                        for row in range(5)]}
report = {"binarySha256": digest, "entryRecords": len(entries), "functions": entries,
          "provider": "private Ghidra 12.1.4 readOnly fallback; no REA Evidence ID",
          "layoutFormula": "R=align128(capH*8), Q=align128(capW*capH*8), IDs=S+128+2R+2Q, Grid=IDs+Q",
          "arithmeticExampleOnly": offsets(300, 300, 14),
          "alignmentExampleOnly": offsets(13, 13, 7),
          "exampleWarning": "300x300 capacity/14-column grid are arithmetic illustration, not observed allocator parameters",
          "runtime": "none; no target address dereference", "allocationTotalAndPrefixValues": "unknown"}
(out / "source-index.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"entryRecords": len(entries), "example": report["arithmeticExampleOnly"]}))
