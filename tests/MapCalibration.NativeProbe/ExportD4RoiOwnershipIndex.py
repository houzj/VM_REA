"""Read original SDK PE vtable/import bytes and fingerprint finite ROI bodies."""
import hashlib
import json
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d4-roi-ownership"
out.mkdir(parents=True, exist_ok=True)
binary = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDMapCalibCpp.dll")
data = binary.read_bytes()
digest = hashlib.sha256(data).hexdigest()
assert digest == "fb373e8ef51572f1ea377a1218bc3fcc230e61f8fa72264d6715445467b4b75a"
pe = struct.unpack_from("<I", data, 60)[0]
count, optional = struct.unpack_from("<H", data, pe + 6)[0], struct.unpack_from("<H", data, pe + 20)[0]
image_base = struct.unpack_from("<Q", data, pe + 24 + 24)[0]

def offset(rva):
    for i in range(count):
        _, base, length, raw = struct.unpack_from("<IIII", data, pe + 24 + optional + i * 40 + 8)
        if base <= rva < base + length:
            return raw + rva - base
    raise ValueError(hex(rva))

def string(rva):
    start = offset(rva)
    return data[start:data.index(0, start)].decode("ascii")

imports = {}
import_rva = struct.unpack_from("<I", data, pe + 24 + 112 + 8)[0]
descriptor = offset(import_rva)
while any(data[descriptor:descriptor + 20]):
    lookup, _, _, dll, first_thunk = struct.unpack_from("<5I", data, descriptor)
    i = 0
    while (entry := struct.unpack_from("<Q", data, offset(lookup or first_thunk) + i * 8)[0]):
        imports[image_base + first_thunk + i * 8] = {"dll": string(dll),
            "name": f"ordinal:{entry & 0xffff}" if entry >> 63 else string(entry + 2)}
        i += 1
    descriptor += 20
thunks = []
for va in (0x18000e1f4, 0x18000e250):
    at = offset(va - image_base)
    assert data[at:at + 2] == b"\xff\x25"
    iat = va + 6 + struct.unpack_from("<i", data, at + 2)[0]
    thunks.append({"va": hex(va), "bytes": data[at:at + 6].hex(), "iat": hex(iat), **imports[iat]})
vtable_va = 0x180010f88
vtable = [{"offset": hex(i * 8), "entry": hex(value)} for i, value in enumerate(
    struct.unpack_from("<15Q", data, offset(vtable_va - image_base)))]
sources = sorted((out / "source").glob("*.c"))
records = [{"path": str(path.relative_to(root)), "sha256": hashlib.sha256(path.read_bytes()).hexdigest(),
            "assemblySha256": hashlib.sha256(path.with_suffix(".asm").read_bytes()).hexdigest()}
           for path in sources]
headers = []
for name in ("Algorithms/MapCalibCpp.h", "Common/VisionDesigner/MVDShapeCpp.h", "Common/VisionDesigner/MVD_ShapeParamDefine.h"):
    path = binary.parents[2] / "Includes" / name
    headers.append({"path": str(path), "sha256": hashlib.sha256(path.read_bytes()).hexdigest() if path.exists() else None})
report = {"binarySha256": digest, "provider": "existing same-hash private Ghidra readOnly fallback; no REA Evidence ID",
          "vtableVa": hex(vtable_va), "vtable": vtable, "imports": thunks, "sources": records, "headers": headers,
          "runtimeAdded": False, "productChanged": False}
(out / "source-index.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"imports": thunks, "vtable": vtable, "sources": len(records)}))
