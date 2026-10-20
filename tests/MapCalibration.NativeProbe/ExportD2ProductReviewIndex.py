"""Read-only snapshot of shared consumer methods for the Map-owned review."""
import hashlib
import json
import re
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d2-product-review"
out.mkdir(parents=True, exist_ok=True)
shared = ["NImageSourceGridSeeds.cs", "NImageSourceGridPadding.cs", "NImageSourceGridAssignment.cs",
          "NImageSourceGridExpansion.cs", "NImageSourceGridPruning.cs", "NImageSourceGridSelection.cs",
          "NImageSourceGridPyramid.cs", "NImageSourceLatticeAdapter.cs", "NImageDetectedBoard.cs",
          "NImageCodeDecoder.cs", "NImageBoardDetector.cs"]
paths = [root / "src/VisionPlugins.Calibration" / file for file in shared]
paths += [root / "src/VisionPlugins.MapCalibration/MapCalibrationImageSolver.cs"]
entries = []
for path in paths:
    text = path.read_text(encoding="utf-8-sig")
    entries.append({"path": str(path.relative_to(root)), "sha256": hashlib.sha256(path.read_bytes()).hexdigest(),
                    "anchors": [{"line": number, "text": line.strip()}
                                for number, line in enumerate(text.splitlines(), 1)
                                if re.search(r"public static|static .*Recognize|static .*DetectImage|index>=0|score<=|tags|firstOutputSlots|GridPyramid|SourceLattice", line)]})
report = {"kind": "read-only product source review, no product edits",
          "nativeBinarySha256": "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549",
          "files": entries, "managedBuildOrTestsRun": False,
          "reuseFixtures": ["image-grid-workspace-trace", "image-grid-filter2-trace",
                            "image-preprocess-trace", "detector-boundary", "image-execution-source-grid"]}
(out / "product-index.json").write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding="utf-8")
print(json.dumps({"files": len(entries), "productChanged": False}))
