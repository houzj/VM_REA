"""Fingerprint bounded module ROI cross-round state evidence; no execution."""
import hashlib
import json
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d4-module-roi-state"
binary = Path("D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu/IMVSMapCalibModu.dll")
digest = hashlib.sha256(binary.read_bytes()).hexdigest()
assert digest == "658d63e2c4e2378e7eb1776d27b966990c0d0e01e29ca57ed82fe9acc740fde8"
expected = ("180015660", "180015830", "18001f280", "18001f390", "18001fae0", "1800203d0", "180021340", "180021690",
            "180021c50", "180022d50", "180022ec0", "180022f40", "180023870")
records = []
for entry in expected:
    source = out / "source" / f"{entry}.c"
    asm = source.with_suffix(".asm")
    text = asm.read_text(encoding="utf-8-sig")
    assert source.exists() and text.strip(), entry
    records.append({"entry": entry, "path": str(source.relative_to(root)),
                    "sourceSha256": hashlib.sha256(source.read_bytes()).hexdigest(),
                    "assemblySha256": hashlib.sha256(asm.read_bytes()).hexdigest(),
                    "undecodedByteLines": [line for line in text.splitlines() if "UNDECODED_BYTE" in line]})
def pe_bytes(path, va, size):
    data = path.read_bytes()
    pe = struct.unpack_from("<I", data, 60)[0]
    count = struct.unpack_from("<H", data, pe + 6)[0]
    optional = struct.unpack_from("<H", data, pe + 20)[0]
    rva = va - 0x180000000
    for i in range(count):
        _, base, length, raw = struct.unpack_from("<IIII", data, pe + 24 + optional + i * 40 + 8)
        if base <= rva < base + length:
            return data[raw + rva - base:raw + rva - base + size]
    raise ValueError(hex(va))

sdk = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDMapCalibCpp.dll")
assert hashlib.sha256(sdk.read_bytes()).hexdigest() == "fb373e8ef51572f1ea377a1218bc3fcc230e61f8fa72264d6715445467b4b75a"
fast_setters = [{"entry": hex(va), "bytes": pe_bytes(sdk, va, 16).hex(), "meaning": meaning}
                for va, meaning in ((0x180001a90, "mov [rcx+0x10],rdx; mov byte [rcx+8],1; ret"),
                                    (0x180001b50, "mov [rcx+0x28],rdx; mov byte [rcx+9],1; ret"),
                                    (0x180003330, "mov [rcx+0x8],edx; ret; padding"))]
vtable = [hex(x) for x in struct.unpack("<14Q", pe_bytes(binary, 0x1800314e0, 14 * 8))]
switch_cases = []
for case in (1, 2, 3, 0xff):
    bucket = pe_bytes(binary, 0x18001f9e0 + case - 1, 1)[0]
    rva = struct.unpack("<I", pe_bytes(binary, 0x18001f9c0 + bucket * 4, 4))[0]
    switch_cases.append({"case": case, "bucket": bucket, "target": hex(0x180000000 + rva)})
reused = []
for entry in ("180001000", "180001070", "180003990", "180004670", "180003e30", "1800047f0"):
    source = root / "analysis/map-calibration/ghidra/wrapper" / f"{entry}.c"
    reused.append({"entry": entry, "path": str(source.relative_to(root)),
                   "sourceSha256": hashlib.sha256(source.read_bytes()).hexdigest()})
report = {"binarySha256": digest, "provider": "same-hash private Ghidra readOnly; REA provider unavailable; no REA Evidence ID",
          "functions": records, "reusedSdkSources": reused, "sdkFastSetterBytes": fast_setters,
          "moduleVtable": vtable, "analyzeSwitchCases": switch_cases,
          "assemblyDataWarning": "Analyze 1f9c0..1fadf is switch data; raw bounded ASM pseudo-instructions there are not executable source claims",
          "scope": "module empty ROI across rounds; no generic CPU/image expansion",
          "runtimeAdded": False, "productChanged": False, "internalDStageCount": "5/32", "wholeToolProgress": "0/10=0%; stage slices do not count as complete tools"}
(out / "source-index.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
print(json.dumps({"functions": len(records), "undecodedBytes": sum(len(x["undecodedByteLines"]) for x in records)}))
