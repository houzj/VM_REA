"""Read-only PE coordinates, exports and exact string bytes; no code execution/decompilation."""
import argparse
import hashlib
import json
import struct
from pathlib import Path


def inspect(path):
    data = path.read_bytes()
    def unpack(fmt, offset):
        return struct.unpack_from(fmt, data, offset)
    if data[:2] != b'MZ':
        raise ValueError('Not PE/MZ')
    pe, = unpack('<I', 0x3c)
    if data[pe:pe+4] != b'PE\0\0':
        raise ValueError('Invalid PE signature')
    machine, sections = unpack('<HH', pe+4)
    optional_size, = unpack('<H', pe+20)
    optional = pe+24
    magic, = unpack('<H', optional)
    if magic not in (0x10b, 0x20b):
        raise ValueError('Unsupported optional header')
    image_base, = unpack('<Q' if magic == 0x20b else '<I', optional+(24 if magic == 0x20b else 28))
    directories = optional+(112 if magic == 0x20b else 96)
    export_rva, export_size = unpack('<II', directories)
    section_rows = []
    for index in range(sections):
        start = optional+optional_size+index*40
        virtual_size, rva, raw_size, raw = unpack('<IIII', start+8)
        section_rows.append(dict(name=data[start:start+8].rstrip(b'\0').decode('ascii'),
                                 rva=rva, virtualSize=virtual_size, rawOffset=raw, rawSize=raw_size))
    def offset(rva):
        for sec in section_rows:
            delta = rva-sec['rva']
            if 0 <= delta < sec['rawSize']:
                return sec['rawOffset']+delta
        raise ValueError(f'RVA is not file-backed: {rva:x}')
    def coordinates(file_offset):
        for sec in section_rows:
            if sec['rawOffset'] <= file_offset < sec['rawOffset']+sec['rawSize']:
                rva = sec['rva']+file_offset-sec['rawOffset']
                return dict(fileOffset=file_offset, rva=hex(rva), va=hex(image_base+rva))
        return dict(fileOffset=file_offset, rva=None, va=None)
    exports = []
    if export_rva:
        base, count, names, address_table, name_table, ordinal_table = unpack('<IIIIII', offset(export_rva)+16)
        if names > 100000 or count > 100000:
            raise ValueError('Export table exceeds audit limit')
        for index in range(names):
            name_rva, = unpack('<I', offset(name_table)+index*4)
            name_start = offset(name_rva)
            name = data[name_start:data.index(b'\0', name_start)].decode('ascii')
            if not any(word in name for word in ('Solution', 'GroupModule', 'Procedure', 'ModuleParamBinding')):
                continue
            ordinal, = unpack('<H', offset(ordinal_table)+index*2)
            if ordinal >= count:
                raise ValueError('Export ordinal exceeds address table')
            target, = unpack('<I', offset(address_table)+ordinal*4)
            start = offset(target)
            record = dict(name=name, ordinal=base+ordinal, **coordinates(start), prefixHex=data[start:start+64].hex())
            if export_rva <= target < export_rva+export_size:
                record['forwarder'] = data[start:data.index(b'\0', start)].decode('ascii')
            elif data[start] == 0xe9:
                displacement, = unpack('<i', start+1)
                destination = target+5+displacement
                record['entryRel32Jump'] = dict(rva=hex(destination), va=hex(image_base+destination),
                                               prefixHex=data[offset(destination):offset(destination)+64].hex())
            exports.append(record)
    strings = []
    for value in ('MoudleFrame', 'VmServer.xml', 'UiParamData', 'GlobalVarBinding', 'ModuleParamBinding', 'RunningPolicy'):
        for encoding in ('ascii', 'utf-16le'):
            needle = value.encode(encoding)
            begin = 0
            while (found := data.find(needle, begin)) >= 0:
                strings.append(dict(value=value, encoding=encoding, **coordinates(found), rawHex=needle.hex()))
                begin = found+len(needle)
    return dict(path=str(path.resolve()), sha256=hashlib.sha256(data).hexdigest(), byteLength=len(data),
                evidenceType='offline exact PE table/byte observation; not REA decompilation or execution',
                machine=hex(machine), imageBase=hex(image_base), sections=section_rows, exports=exports,
                strings=strings, unknowns=['No function-body or string-xref semantics inferred from these bytes.'])


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--input', action='append', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    if args.output.resolve() in [p.resolve() for p in args.input]:
        raise ValueError('Output must not overwrite an input')
    results = [inspect(path) for path in args.input]
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(results, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')
    print(f'Inspected {len(results)} PE files; {sum(len(x["exports"]) for x in results)} selected exports.')
