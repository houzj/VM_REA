"""Export static shipped calibration declarations; does not execute any target."""
import hashlib
import json
import re
import struct
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
TARGET = Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration')
DECOMPILED = ROOT / 'analysis/full-audit/calibration-dependencies/decompiled'
MODULES = ['IMVSCalibBoardCalibModu', 'IMVSCameraMapModu', 'IMVSImageCalibModu', 'IMVSMapCalibModu', 'ReadCalibFileModu', 'RotateCalibModu', 'TranslationCalibModu', 'CoordinateModule']

def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

def imports(path):
    data = path.read_bytes()
    pe = struct.unpack_from('<I', data, 60)[0]
    count, optsize = struct.unpack_from('<H12xH', data, pe + 6)
    opt = pe + 24
    dd = opt + (112 if struct.unpack_from('<H', data, opt)[0] == 0x20b else 96)
    imp = struct.unpack_from('<I', data, dd + 8)[0]
    sections = [struct.unpack_from('<8sIIII', data, opt + optsize + i * 40) for i in range(count)]
    def offset(rva):
        for _, size, va, rawsize, raw in sections:
            if va <= rva < va + max(size, rawsize):
                return raw + rva - va
        raise ValueError(rva)
    names = []
    if imp:
        pos = offset(imp)
        while any(data[pos:pos + 20]):
            name = offset(struct.unpack_from('<I', data, pos + 12)[0])
            names.append(data[name:data.index(b'\0', name)].decode('ascii'))
            pos += 20
    return names

def entry(node):
    result = {'tag': node.tag, **node.attrib, 'values': {c.tag: (c.text or '').strip() for c in node if not len(c)}}
    nested = [{'tag': child.tag, **child.attrib} for child in node.iter() if child is not node and child.tag in ('Filter', 'Combination')]
    if nested:
        result['nestedDeclarations'] = nested
    return result

rows = []
for module in MODULES:
    folder = TARGET / module
    row = {'module': module, 'scope': 'dependency-only' if module == 'CoordinateModule' else 'static-inventory', 'artifacts': [], 'categories': {}, 'controls': [], 'sdk': [], 'nativeImports': imports(folder / (module + '.dll'))}
    for path in sorted(folder.iterdir()):
        if path.is_file():
            row['artifacts'].append({'name': path.name, 'path': str(path), 'bytes': path.stat().st_size, 'sha256': digest(path)})
    if module != 'CoordinateModule':
        for xml in sorted(folder.glob('*.xml')):
            try:
                tree = ET.parse(xml)
            except ET.ParseError as exc:
                row.setdefault('xmlIssues', []).append({'file': xml.name, 'error': str(exc)})
                continue
            if xml.name == module + '.xml':
                for category in tree.findall('./Categorys/Category'):
                    row['categories'][category.get('Name')] = [entry(n) for n in category.findall('./Items/*')]
            if xml.name.endswith('AlgorithmTab.xml'):
                row['controls'] = [entry(n) for n in tree.findall('.//Items/*')]
                row['operations'] = sorted({(n.text or '').strip() for n in tree.findall('.//OperationName')})
                row['fileFilters'] = [(n.text or '').strip() for n in tree.findall('.//FilterName')]
        for project in (DECOMPILED / (module + 'Cs'), DECOMPILED / (module + 'Control')):
            if not project.exists():
                continue
            for source in sorted(project.rglob('*.cs')):
                text = source.read_text(encoding='utf-8-sig')
                properties = re.findall(r'public\s+([\w<>., ]+)\s+(\w+)\s*\{', text)
                methods = re.findall(r'(?:public|private|protected)\s+(?:override\s+|static\s+)?[\w<>., ]+\s+(\w+)\s*\(', text)
                row['sdk'].append({'path': str(source.relative_to(ROOT)), 'sha256': digest(source), 'properties': properties, 'methods': methods})
    rows.append(row)
result = {'authority': 'shipped-artifact/static-declarations-and-ilspy-source', 'runtimeExecuted': False, 'limitations': ['XML and SDK declarations do not prove native numerical behavior.', 'No devices or original UI windows executed.', 'Calibration XML disk schema and matrix coefficient order not recovered by this inventory.', 'CoordinateModule dependency inventory only; user priority remains last.'], 'reaEvidence': {'module': 'ReadCalibFileModuControl', 'id': 'ev_ca66bc7e6c4cc7a9571f53344daf7263f2ee1dc9fa37fd17dc18320bb536c94d', 'provider': 'rea-dotnet-static'}, 'modules': rows}
out = ROOT / 'docs/plugins/calibration-dependencies.json'
out.write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Indexed {len(rows)} modules, {sum(len(r["artifacts"]) for r in rows)} artifacts, {sum(len(r["controls"]) for r in rows)} controls')
