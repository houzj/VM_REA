"""Read source-referenced floating constants without loading the original core."""
import hashlib
import json
import pathlib
import struct

root = pathlib.Path(__file__).resolve().parents[2]
target = pathlib.Path("D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll")
data = target.read_bytes()
digest = hashlib.sha256(data).hexdigest()
assert digest == "85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549"
pe = struct.unpack_from("<I", data, 60)[0]
count = struct.unpack_from("<H", data, pe + 6)[0]
start = pe + 24 + struct.unpack_from("<H", data, pe + 20)[0]
sections = [struct.unpack_from("<8sIIII", data, start + 40 * index) for index in range(count)]
rows = []
for rva, kind in [(0x162B7C0, "d"), (0x162B804, "f"), (0x162B808, "f"), (0x162B824, "f"), (0x162B82C, "f"), (0x162B830, "f")]:
    section = next(item for item in sections if item[2] <= rva < item[2] + max(item[1], item[3]))
    offset = section[4] + rva - section[2]
    rows.append({"rva": hex(rva), "format": kind, "raw": data[offset: offset + struct.calcsize(kind)].hex(), "value": struct.unpack_from("<" + kind, data, offset)[0]})
output = root / "analysis/camera-mapping/b2-matrix-source-20261019/constants.json"
output.write_text(json.dumps({"target": str(target), "sha256": digest, "constants": rows}, indent=2), encoding="utf-8")
print(output.read_text(encoding="utf-8"))
