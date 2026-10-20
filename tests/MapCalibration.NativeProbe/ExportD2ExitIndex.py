"""D2 source-stage exit inventory; no runtime or score mutation."""
import hashlib
import json
import re
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d2-exit-audit"
out.mkdir(parents=True, exist_ok=True)
paths = {
    "d2-grid-selection/source": ["180873de0", "1808bcff0", "1808bf960"],
    "d2-padding-address/source": ["18080e690", "18080bcd0", "180813ca0"],
    "d2-seed-flags/source": ["1808c1780", "1808c1340", "1808c0fa0"],
    "ghidra/core": ["180813020", "1808bdb40", "1808c00f0", "1808bdef0", "1808c0880"],
}
entries = []
for directory, addresses in paths.items():
    for address in addresses:
        source = root / "analysis/map-calibration" / directory / (address + ".c")
        asm = source.with_suffix(".asm")
        tails = source.with_name(address + "-cookie-tails.asm")
        text = source.read_text(encoding="utf-8-sig")
        entries.append({"address": address, "source": str(source.relative_to(root)),
                        "sourceSha256": hashlib.sha256(source.read_bytes()).hexdigest(),
                        "assemblySha256": hashlib.sha256(asm.read_bytes()).hexdigest() if asm.exists() else None,
                        "cookieTailsSha256": hashlib.sha256(tails.read_bytes()).hexdigest() if tails.exists() else None,
                        "calls": sorted(set(re.findall(r"FUN_([0-9a-f]+)\(", text)) - {address})})
binary = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll")
digest = hashlib.sha256(binary.read_bytes()).hexdigest()
assert digest == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
report = {"task": "D2 bounded source-stage exit audit", "binarySha256": digest,
          "requiredBodyCount": len(entries), "functions": entries,
          "provider": "existing private Ghidra exports; no REA Evidence ID",
          "proposedStagesForParentAudit": ["S1", "S2"],
          "parentApprovedStages": ["S1", "S2"],
          "approvalBasis": "Parent verified all 14 required C hashes and approved the original D2 task source stages on 2026-10-19",
          "recordedCompletion": "D group 4/32 stages, 0/4 cards; D2 S3-S7 remain open",
          "runtimeAdded": False, "productChanged": False}
(out / "source-index.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"requiredBodyCount": len(entries), "binarySha256": digest}))
