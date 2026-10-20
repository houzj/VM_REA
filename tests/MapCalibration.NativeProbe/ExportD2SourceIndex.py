"""Fingerprint the finite D2 grid/collision/selection slice."""
import hashlib
import json
import re
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d2-grid-selection"
old = root / "analysis/map-calibration/ghidra/core"
binary = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll")
sha = hashlib.sha256(binary.read_bytes()).hexdigest()
assert sha == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
sources = [old / f"{address}.c" for address in ("18080e690", "18080bcd0", "180813ca0", "180813020")]
sources += sorted((out / "source").glob("*.c"))
entries = []
for path in sources:
    text = path.read_text(encoding="utf-8-sig")
    calls = sorted(set(re.findall(r"FUN_([0-9a-f]+)\(", text)) - {path.stem})
    entries.append({"address": path.stem, "source": str(path.relative_to(root)),
                    "sourceSha256": hashlib.sha256(path.read_bytes()).hexdigest(),
                    "assemblySha256": hashlib.sha256(path.with_suffix(".asm").read_bytes()).hexdigest() if path.with_suffix(".asm").exists() else None,
                    "origin": "existing export" if path.parent == old else "private Ghidra readOnly fallback",
                    "calls": calls,
                    "anchors": [{"line": number, "text": line.strip()}
                                for number, line in enumerate(text.splitlines(), 1)
                                if re.search(r"80000000|813ca0|8bcff0|8bf960|if \(|param_.*=", line)]})
available = {item["address"] for item in entries}
for item in entries:
    item["calleesNotInThisSlice"] = [callee for callee in item["calls"] if callee not in available]
report = {"task": "D2 finite first slice", "binarySha256": sha, "entryRecords": len(entries),
          "reaEvidenceId": None, "provider": "REA unavailable as D1; existing private Ghidra 12.1.4 readOnly/noanalysis",
          "functions": entries, "runtime": "none added", "implementation": "unchanged",
          "hostAcceptance": "not performed"}
(out / "source-index.json").write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding="utf-8")
print(json.dumps({"entryRecords": len(entries), "binarySha256": sha}))
