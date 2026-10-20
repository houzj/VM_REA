"""Fingerprint seed cleanup source, constants, and symbolic flag addresses."""
import hashlib
import json
import re
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d2-seed-flags"
binary = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll")
data = binary.read_bytes()
digest = hashlib.sha256(data).hexdigest()
assert digest == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
pe = struct.unpack_from("<I", data, 60)[0]
section_count = struct.unpack_from("<H", data, pe + 6)[0]
optional_size = struct.unpack_from("<H", data, pe + 20)[0]
constants = []
for va in (0x181647dd8, 0x181647dec, 0x181647df0, 0x181647e14):
    rva = va - 0x180000000
    for n in range(section_count):
        _, base, size, raw = struct.unpack_from("<IIII", data, pe + 24 + optional_size + n * 40 + 8)
        if base <= rva < base + size:
            offset = raw + rva - base
            constants.append({"va": hex(va), "fileOffset": offset, "bytes": data[offset:offset + 4].hex(),
                              "float": struct.unpack_from("<f", data, offset)[0]})
            break
sources = sorted((out / "source").glob("*.c"))
sources += [root / f"analysis/map-calibration/d2-grid-selection/source/{a}.c"
            for a in ("1808bcff0", "180873de0")]
entries = []
for source in sources:
    tails = source.with_name(source.stem + "-cookie-tails.asm")
    entries.append({"address": source.stem, "source": str(source.relative_to(root)),
                    "sourceSha256": hashlib.sha256(source.read_bytes()).hexdigest(),
                    "assemblySha256": hashlib.sha256(source.with_suffix(".asm").read_bytes()).hexdigest(),
                    "cookieTailsSha256": hashlib.sha256(tails.read_bytes()).hexdigest() if tails.exists() else None,
                    "calls": sorted(set(re.findall(r"FUN_([0-9a-f]+)\(", source.read_text(encoding="utf-8-sig"))) - {source.stem})})
report = {"binarySha256": digest, "entryRecords": len(entries), "functions": entries, "constants": constants,
          "provider": "private Ghidra 12.1.4 readOnly fallback; no REA Evidence ID",
          "flagBase": "F=S+align128(candidateCapacity*28)+align128(imageWidth*imageHeight)+4*align128(capW*capH*8)",
          "negativeFlagWrites": [{"index": i, "byteOffsetFromF": i * 4, "writeBytes": 4, "value": 0}
                                 for i in (-2, -1)],
          "minusOneOrigin": "not assigned by the recovered seed initializers/neighbor selector; remains unknown outside this slice",
          "runtime": "none; symbolic addresses only"}
(out / "source-index.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"entryRecords": len(entries), "constants": constants}))
