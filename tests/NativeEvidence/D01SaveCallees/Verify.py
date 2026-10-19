"""Verify exported region bytes and printed instruction prefixes; never execute target."""
import hashlib
import json
from pathlib import Path
import re
import struct
import sys

out = Path(sys.argv[1])
report = json.loads((out / 'functions.json').read_text())
count = 0
facts = []
for f in report['functions']:
    data = Path(f['path']).read_bytes()
    assert hashlib.sha256(data).hexdigest() == f['sha256Before'] == f['sha256After']
    pe = struct.unpack_from('<I', data, 0x3c)[0]
    opt = pe + 24
    n = struct.unpack_from('<H', data, pe + 6)[0]
    size = struct.unpack_from('<H', data, pe + 20)[0]
    def offset(rva):
        for i in range(n):
            _, va, rawsize, raw = struct.unpack_from('<IIII', data, opt + size + i * 40 + 8)
            if va <= rva < va + rawsize:
                return raw + rva - va
        raise ValueError(rva)
    imports = json.loads((out / (f['export'] + '.imports.json')).read_text())
    calls = []
    for branch in f['branches']:
        m = re.search(r'\[([0-9A-Fa-f]{16})h\]', branch['instruction'])
        if m:
            calls.append(dict(branch, importName=imports.get(hex(int(m[1], 16)))))
    facts.append({'function': f['export'], 'resolvedCalls': calls})
    for i, r in enumerate(f['regions']):
        start, end = int(r['beginRva'], 16), int(r['endRvaExclusive'], 16)
        raw = data[offset(start):offset(start) + end - start]
        assert raw == (out / (f['export'] + '.region' + str(i) + '.bin')).read_bytes()
        assert hashlib.sha256(raw).hexdigest() == r['bytesSha256']
        for line in (out / (f['export'] + '.region' + str(i) + '.assembly.txt')).read_text().splitlines():
            m = re.match(r'\s*([0-9A-Fa-f]{16}):\s+((?:[0-9A-Fa-f]{2} )+)', line)
            if m:
                va, b = int(m[1], 16), bytes.fromhex(m[2])
                pos = offset(va - int(f['imageBase'], 16))
                assert data[pos:pos + len(b)] == b
                count += 1
(out / 'resolved-calls.json').write_text(json.dumps(facts, indent=2), encoding='utf-8')
(out / 'verification.json').write_text(json.dumps({
    'originalsUnchanged': True, 'printedInstructionPrefixesChecked': count,
    'allRegionBytesMatch': True, 'runtimeExecuted': False,
    'limitations': 'Printed prefixes checked, no independent opcode decoder or EH funclet verification'
}, indent=2), encoding='utf-8')
print(count)
