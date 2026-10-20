"""D4 identity/reuse/gap inventory only; does not execute installed artifacts."""
import hashlib
import json
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d4-s0"
out.mkdir(parents=True, exist_ok=True)
installed = Path("D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu")
sdk = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK")
def identity(path):
    return {"path": str(path), "present": path.exists(),
            "bytes": path.stat().st_size if path.exists() else None,
            "sha256": hashlib.sha256(path.read_bytes()).hexdigest() if path.exists() else None}

artifacts = [identity(path) for path in sorted(installed.iterdir()) if path.is_file()]
artifacts += [identity(sdk / relative) for relative in
              ("Includes/Algorithms/MapCalibCpp.h", "Runtime/x64/MVDMapCalibCpp.dll", "Runtime/x64/MVD_Algorithm.dll")]
core = next(item for item in artifacts if item["path"].endswith("MVD_Algorithm.dll"))
assert core["sha256"] == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
native = ["module-analyzed/180021690.c", "module-analyzed/180021c50.c", "module-analyzed/1800220b0.c",
          "module-analyzed/180022d50.c", "module-analyzed/180023870.c", "module/1800227d0.c",
          "module/180021340.c", "core/1807d4f20.c", "core/1807d3e90.c",
          "core/1807e3f50.c", "core/180816650.c"]
sources = [identity(root / "analysis/map-calibration/ghidra" / relative) for relative in native]
products = [identity(path) for path in sorted((root / "src/VisionPlugins.MapCalibration").glob("*.cs"))]
managed = Path("D:/MyWork/REA逆向测试/analysis/full-audit/calibration-dependencies/decompiled/IMVSMapCalibModuCs")
gaps = {"historicalManagedDecompileDirectory": {"path": str(managed), "present": managed.exists()},
        "retrieveWhenRequired": ["IMVSMapCalibModuCs MapCalibParam/MapCalibResult bodies at a current readable path",
                                 "CIMVSMapCalibModu ROI vector SetParam/clear/destroy ownership path",
                                 "SDK ITwoImageMapBasicParam SetSourceROI/SetTargetROI concrete setter body",
                                 "SDK GetProcessInput shape conversion and angle/unit/layout bridge",
                                 "platform display/result-history and scope-release path if D4 claims host parity"]}
report = {"task": "D4 S0 inventory submitted for parent audit, no stage promotion",
          "artifacts": artifacts, "nativeReuse": sources, "productFiles": products, "gaps": gaps,
          "finiteSlice": "CIMVSMapCalibModu::ProcessAlg image-mode vtable source/target image and ROI slots",
          "newRuntime": False, "productChanged": False,
          "provider": "reuse existing private Ghidra exports and shipped SDK header; no new REA Evidence ID"}
(out / "identity-index.json").write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding="utf-8")
print(json.dumps({"artifacts": len(artifacts), "nativePresent": sum(item["present"] for item in sources),
                  "nativeRequested": len(sources), "productFiles": len(products),
                  "managedHistoricalPathPresent": managed.exists()}))
