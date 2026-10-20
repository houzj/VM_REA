"""Index original function exports and source-bound constants; no target execution."""
import hashlib
import json
import re
import struct
from pathlib import Path

root = Path(__file__).resolve().parents[2]
out = root / "analysis/map-calibration/d1-source-chain"
old = root / "analysis/map-calibration/ghidra/core"
binary = Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll")
data = binary.read_bytes()
digest = hashlib.sha256(data).hexdigest()
assert digest == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
pe = struct.unpack_from("<I", data, 60)[0]
sections = struct.unpack_from("<H", data, pe + 6)[0]
optional = struct.unpack_from("<H", data, pe + 20)[0]
def read_va(va, size):
    rva = va - 0x180000000
    for i in range(sections):
        start = pe + 24 + optional + i * 40
        virtual_size, base, raw_size, raw = struct.unpack_from("<IIII", data, start + 8)
        if base <= rva and rva + size <= base + raw_size:
            offset = raw + rva - base
            return offset, data[offset:offset + size]
    raise ValueError(hex(va))

constants = []
for va, fmt in [(0x181642ef8, "f"), (0x181642e94, "f"), (0x181642d90, "4f"),
                (0x1816445a0, "4I"), (0x1816445b0, "d"), (0x1816445c0, "8f"),
                (0x181647640, "d"), (0x181647650, "2f"),
                (0x1816180d0, "7f"), (0x18161a060, "2f"), (0x18161c8c0, "2f"),
                (0x18164ad20, "2f"), (0x18164ad50, "d"), (0x18164ad58, "f"),
                (0x18161c8b0, "4f"), (0x181644650, "6I"), (0x18161d3e0, "f"),
                (0x18161c268, "7f"), (0x18161c080, "5f"),
                (0x181642ea4, "2f"), (0x18161c040, "3d"), (0x18161c058, "5d"),
                (0x1816179e8, "5f"), (0x1816189a0, "f"),
                (0x1816189b0, "4f"), (0x18161be70, "4f"),
                (0x181618950, "2d"), (0x1816189e0, "d"),
                (0x1816189f0, "f"), (0x181618a04, "f"), (0x1816189c0, "4f")]:
    offset, block = read_va(va, struct.calcsize("<" + fmt))
    constants.append({"va": hex(va), "fileOffset": offset, "bytes": block.hex(), "format": fmt, "values": struct.unpack("<" + fmt, block)})

index = []
sources = [old / f"{a}.c" for a in ("18080e690", "18080bcd0", "180813ca0", "18080a7f0", "180813020")]
sources += sorted((out / "source").glob("*.c"))
for source in sources:
    text = source.read_text(encoding="utf-8-sig")
    address = source.stem
    assembly = source.with_suffix(".asm")
    tails = source.with_name(address + "-cookie-tails.asm")
    calls = sorted(set(re.findall(r"FUN_([0-9a-f]+)\(", text)) - {address})
    anchors = [{"line": i, "text": line.strip()} for i, line in enumerate(text.splitlines(), 1)
               if re.search(r"FUN_180|param_5 \+ 0x11|param_5 \+ 0xf0|param_5 \+ 0x118|if \(|switch\(|param_10 =|param_11 =|param_12\[", line)]
    index.append({"address": address, "origin": "existing Ghidra export, no REA Evidence ID" if source.parent == old else "D1 private Ghidra 12.1.4 headless fallback, no REA Evidence ID",
                  "source": str(source.relative_to(root)), "sourceSha256": hashlib.sha256(source.read_bytes()).hexdigest(),
                  "assemblySha256": hashlib.sha256(assembly.read_bytes()).hexdigest() if assembly.exists() else None,
                  "cookieTailsSha256": hashlib.sha256(tails.read_bytes()).hexdigest() if tails.exists() else None,
                  "calls": calls, "sourceAnchors": anchors,
                  "addressReferences": sorted(set(re.findall(r"(?:FUN_|UNK_|D1_)(180[0-9a-f]+)", text)) - {address}),
                  "temporaryFunctionRecovery": address == "18068b8f0",
                  "classification": "split block of 1808a8040; not a distinct recovered algorithm" if address in {"1808a808c", "1808a811d", "1808a8132", "1808a81c3", "1808a81ce", "1808a82bf", "1808a82ca", "1808a83d4", "1808a83e3"} else "Ghidra function entry"})
available = {item["address"] for item in index}
for item in index:
    if item["address"] == "18068a618":
        item["classification"] = "split block of 18068a2f0; not a distinct recovered algorithm"
for item in index:
    item["calleeBodiesNotInThisD1Index"] = [callee for callee in item["calls"] if callee not in available]
split_blocks = sum(item["classification"].startswith("split block") for item in index)
report = {"task": "D1", "binary": str(binary), "binarySha256": digest,
          "entryRecordCount": len(index), "splitBlockCount": split_blocks,
          "provider": {"reaMcp": "binary_session open=false; Ghidra not_configured: GHIDRA_INSTALL_DIR unset; java runtime_missing",
                       "fallback": "existing private Ghidra 12.1.4 project, readOnly/noanalysis; no global config edits", "reaEvidenceId": None},
          "functions": index, "constants": constants,
          "states": {"sourceRecovery": "indexed bodies only; leaf retrieval gaps listed per function", "runtime": "reuse prior direction-1 caller capture only", "implementation": "unchanged", "hostAcceptance": "not performed by D1"}}
(out / "source-index.json").write_text(json.dumps(report, indent=2, ensure_ascii=False), encoding="utf-8")
print(json.dumps({"entryRecords": len(index), "newEntryRecords": len(index) - 5, "splitBlocks": split_blocks, "constants": constants}, ensure_ascii=False))
