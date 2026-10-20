"""Read the shipped x64 PE exception metadata without loading the module."""
import hashlib
import json
import struct
from pathlib import Path

target = Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSImageCalibModu/IMVSImageCalibModu.dll')
data = target.read_bytes()
digest = hashlib.sha256(data).hexdigest()
assert digest == '0a6bac886f1aa1ccc41433ef9d35fc1091b3d43354de5794563454d8bba03bcb'
pe = struct.unpack_from('<I', data, 0x3c)[0]
optional = pe + 24
assert struct.unpack_from('<H', data, optional)[0] == 0x20b
count = struct.unpack_from('<H', data, pe + 6)[0]
table = optional + struct.unpack_from('<H', data, pe + 20)[0]

def offset(rva):
    for i in range(count):
        size, start, raw_size, raw = struct.unpack_from('<IIII', data, table + i * 40 + 8)
        if start <= rva < start + max(size, raw_size):
            assert rva - start < raw_size
            return raw + rva - start
    raise ValueError(hex(rva))

def u32(rva):
    return struct.unpack_from('<I', data, offset(rva))[0]

exception_rva, exception_size = struct.unpack_from('<II', data, optional + 112 + 3 * 8)
records = []
for rva in range(exception_rva, exception_rva + exception_size, 12):
    begin, end, unwind = struct.unpack_from('<III', data, offset(rva))
    if begin not in (0x20550, 0x20820, 0x20b70):
        continue
    flags, prolog, codes, frame = struct.unpack_from('<BBBB', data, offset(unwind))
    item = dict(begin=hex(begin), end=hex(end), unwind=hex(unwind), flags=flags >> 3)
    assert flags & 7 == 1
    if flags >> 3 & 3:
        handler_slot = unwind + 4 + ((codes + 1) & ~1) * 2
        item['languageHandler'] = hex(u32(handler_slot))
        info = u32(handler_slot + 4)
        fields = struct.unpack_from('<10I', data, offset(info))
        item['funcInfoRva'] = hex(info)
        item['funcInfoWords'] = [hex(x) for x in fields]
        assert fields[0] & 0xfffffff0 == 0x19930520
        item['ipStates'] = []
        for i in range(fields[5]):
            ip, state = struct.unpack_from('<Ii', data, offset(fields[6] + i * 8))
            item['ipStates'].append(dict(ipRva=hex(ip), state=state))
        item['tryBlocks'] = []
        for i in range(fields[3]):
            low, high, catch_high, n, handlers = struct.unpack_from('<5I', data, offset(fields[4] + i * 20))
            block = dict(tryLow=low, tryHigh=high, catchHigh=catch_high, handlers=[])
            for j in range(n):
                words = struct.unpack_from('<5I', data, offset(handlers + j * 20))
                name = None
                if words[1]:
                    start = offset(words[1]) + 16
                    name = data[start:data.index(b'\0', start)].decode('ascii')
                block['handlers'].append(dict(rawWords=[hex(x) for x in words], handlerRva=hex(words[3]), typeDescriptorName=name))
            item['tryBlocks'].append(block)
    records.append(item)
assert len(records) == 3
assert {int(r['begin'], 16): int(r['end'], 16) for r in records} == {0x20550: 0x2081f, 0x20820: 0x20b70, 0x20b70: 0x21326}
assert sum(len(t['handlers']) for r in records for t in r.get('tryBlocks', [])) == 16
out = Path('analysis/distortion-calibration/c3-catch-source/module-exception-scopes.json')
out.parent.mkdir(parents=True, exist_ok=True)
result = dict(target=str(target), sha256=digest, runtimeExecuted=False, records=records)
out.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
print(json.dumps(result, indent=2))

