"""Index the finite file-direction source slice and original PE constants."""
import hashlib
import json
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d4-file-direction"
binary = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll")
data = binary.read_bytes()
digest = hashlib.sha256(data).hexdigest()
assert digest == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
pe = struct.unpack_from("<I", data, 60)[0]
count = struct.unpack_from("<H", data, pe + 6)[0]
optional = struct.unpack_from("<H", data, pe + 20)[0]
constants = []
for va, size, fmt in ((0x181628530, 16, "<4f"), (0x181628540, 8, "<d"),
                      (0x18162854c, 4, "<f"), (0x181628554, 4, "<f"),
                      (0x181628574, 8, "<2I")):
    rva = va - 0x180000000
    for i in range(count):
        _, base, length, raw = struct.unpack_from("<IIII", data, pe + 24 + optional + i * 40 + 8)
        if base <= rva < base + length:
            offset = raw + rva - base
            constants.append({"va": hex(va), "fileOffset": offset,
                              "bytes": data[offset:offset + size].hex(),
                              "values": struct.unpack_from(fmt, data, offset)})
            break
paths = sorted((out / "source").glob("*.c"))
paths += [root / "analysis/map-calibration/ghidra/module/1800227d0.c",
          root / "analysis/map-calibration/ghidra/module-analyzed/180021c50.c",
          root / "analysis/map-calibration/ghidra/wrapper/180005860.c"]
records = []
for path in paths:
    record = {"path": str(path.relative_to(root)), "sha256": hashlib.sha256(path.read_bytes()).hexdigest()}
    for suffix in (".asm", "-cookie-tails.asm"):
        other = path.with_name(path.stem + suffix)
        record[suffix] = hashlib.sha256(other.read_bytes()).hexdigest() if other.exists() else None
    records.append(record)
report = {"binarySha256": digest, "sourceRecords": records, "constants": constants,
          "provider": "private Ghidra 12.1.4 readOnly fallback; REA provider unavailable; no REA Evidence ID",
          "runtimeAdded": False, "productChanged": False,
          "stage": "D4 S0 approved; this finite source slice does not complete D4 S1/S2"}
(out / "source-index.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"records": len(records), "constants": constants}))
