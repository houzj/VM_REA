"""Index the bounded 18/24 decoder source slice and verify existing evidence consistency."""
import hashlib
import json
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d1-decoder-output"
binary = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll")
data = binary.read_bytes()
digest = hashlib.sha256(data).hexdigest()
assert digest == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
sources = sorted((out / "source").glob("*.c"))
sources += [root / f"analysis/map-calibration/d1-source-chain/source/{entry}.c" for entry in
            ("1808b0260", "1808b11a0", "1808b0670", "18086ff00", "180932370", "1809322a0", "180932480")]
sources += [root / f"analysis/map-calibration/d2-padding-address/source/{entry}.c" for entry in
            ("18080e690", "18080bcd0", "180813ca0")]
sources += [root / f"analysis/map-calibration/ghidra/core/{entry}.c" for entry in ("18080a7f0", "180813020")]
records = []
for source in sources:
    record = {"entry": source.stem, "path": str(source.relative_to(root)),
              "sourceSha256": hashlib.sha256(source.read_bytes()).hexdigest()}
    for suffix in (".asm", "-cookie-tails.asm"):
        path = source.with_name(source.stem + suffix)
        record[suffix] = hashlib.sha256(path.read_bytes()).hexdigest() if path.exists() else None
    records.append(record)
pe = struct.unpack_from("<I", data, 60)[0]
count, optional = struct.unpack_from("<H", data, pe + 6)[0], struct.unpack_from("<H", data, pe + 20)[0]
constants = []
for va in (0x181642e90, 0x181642ec0, 0x181642ec4):
    rva = va - 0x180000000
    for i in range(count):
        _, base, length, raw = struct.unpack_from("<IIII", data, pe + 24 + optional + i * 40 + 8)
        if base <= rva < base + length:
            at = raw + rva - base
            constants.append({"va": hex(va), "bytes": data[at:at + 4].hex(), "float": struct.unpack_from("<f", data, at)[0]})
            break

def multiply(a, b):
    value = 0
    while b:
        if b & 1:
            value ^= a
        a <<= 1
        if a & 256:
            a ^= 0x11d
        b >>= 1
    return value

def evaluate(word, x):
    value = 0
    for coefficient in reversed(word):
        value = multiply(value, x) ^ coefficient
    return value

fixture_path = root / "docs/plugins/n-image-code-evidence.json"
fixture = json.loads(fixture_path.read_text(encoding="utf-8-sig"))
assert fixture["provenance"][0]["sha256"] == digest
cases = []
for row in fixture["cases"]:
    assert len(row["input"]) == len(row["output"]) == 24 and row["code"] == 0
    assert sum(a != b for a, b in zip(row["input"], row["output"])) == row["errors"]
    x, syndromes = 1, []
    for i in range(18):
        syndromes.append(evaluate(row["output"], x))
        x = multiply(x, 2)
    assert not any(syndromes)
    cases.append({"seed": row["seed"], "errors": row["errors"], "nativeCode": row["code"],
                  "payload12": row["output"][::-1][:12], "syndromesZero": True})
products = []
for name in ("NImageCodeReedSolomon.cs", "NImageCodeDecoder.cs", "NImageBoardIdentity.cs", "NImageDetectedBoard.cs", "NImageSourceLatticeAdapter.cs"):
    path = root / "src/VisionPlugins.Calibration" / name
    products.append({"path": str(path.relative_to(root)), "sha256": hashlib.sha256(path.read_bytes()).hexdigest()})
report = {"binarySha256": digest, "functions": records, "constants": constants, "productReadSnapshot": products,
          "fixture": {"path": str(fixture_path.relative_to(root)), "sha256": hashlib.sha256(fixture_path.read_bytes()).hexdigest(),
                      "nativeCases": len(cases), "symbols": len(cases) * 24, "exportedBytes": len(cases) * 12, "cases": cases},
          "validation": "existing native fixture consistency plus independent source arithmetic; no current native/product execution",
          "nullStatus": "0x10000014", "invalidRangeStatus": "0x10000019",
          "provider": "same-hash private Ghidra readOnly plus existing exports; no REA Evidence ID",
          "runtimeAdded": False, "productChanged": False, "wholeToolProgress": "0/10; local decoder slice is not a whole tool"}
(out / "source-index.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"functions": len(records), "existingNativeCases": len(cases), "symbols": len(cases) * 24, "constants": constants}))
