"""Read shipped PE slots; never load or execute the SDK."""
import argparse
import hashlib
import json
import struct
from pathlib import Path

p = argparse.ArgumentParser()
p.add_argument('--sdk', default='D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVDCalibBoardCalibCpp.dll')
p.add_argument('--output', required=True)
a = p.parse_args()
sdk = Path(a.sdk)
data = sdk.read_bytes()
digest = hashlib.sha256(data).hexdigest()
assert digest == '0a65d7a9e84a3ec6f662b4f05daa5a88351690ae4fbecd1748427c13c56fa308', 'SDK identity mismatch'
pe = struct.unpack_from('<I', data, 0x3c)[0]
assert data[pe:pe+4] == b'PE\0\0'
sections = struct.unpack_from('<H', data, pe+6)[0]
optional_size = struct.unpack_from('<H', data, pe+20)[0]
optional = pe+24
assert struct.unpack_from('<H', data, optional)[0] == 0x20b
image_base = struct.unpack_from('<Q', data, optional+24)[0]
table = optional+optional_size

def offset(rva):
    for i in range(sections):
        s = table+i*40
        virtual_size, start, raw_size, raw = struct.unpack_from('<IIII', data, s+8)
        if start <= rva < start+max(virtual_size, raw_size):
            assert rva-start < raw_size, 'RVA has no file bytes'
            return raw+rva-start
    raise ValueError(f'unmapped RVA {rva:x}')

# Create@103a loads this table; method order comes from shipped CalibBoardCalibCpp.h.
names = ['destructor','GetInputImage','SetInputImage','GetROI','SetROI','GetBasicParam','GetResult',
         'AddMask','ClearMasks','RemoveMaskAt','GetNumOfMasks','GetRunParam','SetRunParam',
         'SaveConfiguration','LoadConfiguration','Run','ExportCalibFile','GetRegionImage',
         'SetRegionImage','GetAlgRunTime']
entries = []
for i, name in enumerate(names):
    va = struct.unpack_from('<Q', data, offset(0x7b38)+8*i)[0]
    rva = va-image_base
    entries.append({'slot': hex(i*8), 'headerMember': name, 'rva': hex(rva),
                    'first16Bytes': data[offset(rva):offset(rva)+16].hex()})
result = {'sdk': str(sdk), 'sha256': digest, 'imageBase': hex(image_base),
          'vtableRva': '0x7b38', 'evidence': 'Create@103a disassembly and shipped header order',
          'runtimeExecuted': False, 'entries': entries}
out = Path(a.output)
out.parent.mkdir(parents=True, exist_ok=True)
out.write_text(json.dumps(result, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')
print(json.dumps({'entries': len(entries), 'output': str(out)}))
