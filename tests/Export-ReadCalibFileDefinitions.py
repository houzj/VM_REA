"""Export original ReadCalibFileModu UI declarations without executing the target."""
import hashlib
import json
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
SOURCE = Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/ReadCalibFileModu')
def identity(path):
    return {'path': str(path), 'bytes': path.stat().st_size, 'sha256': hashlib.sha256(path.read_bytes()).hexdigest()}
def node(element):
    result = {'kind': element.tag, **element.attrib}
    result['properties'] = {c.tag: (c.text or '').strip() for c in element if not len(c)}
    result['entries'] = [{'name': e.get('Name'), **{c.tag: (c.text or '').strip() for c in e}} for e in element.findall('./EnumEntrys/EnumEntry')]
    result['triggers'] = [ET.tostring(t, encoding='unicode') for t in element.findall('./Triggers/Trigger')]
    if element.tag == 'Combination':
        result['fields'] = [node(n) for n in element.findall('./Filters/*')]
    return result
tab = SOURCE / 'ReadCalibFileModuAlgorithmTab.xml'
categories = [{'name': c.get('Name'), 'controls': [node(n) for n in c.findall('./Items/*')]} for c in ET.parse(tab).findall('.//Category')]
definition = {'source': identity(tab), 'categories': categories}
out = ROOT / 'src/VisionPlugins.Calibration/Resources/ReadCalibFileDefinition.json'
out.parent.mkdir(parents=True, exist_ok=True)
out.write_text(json.dumps(definition, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
declaration = ET.parse(SOURCE / 'ReadCalibFileModu.xml')
schema = {c.get('Name'): [node(n) for n in c.findall('./Items/*')] for c in declaration.findall('./Categorys/Category')}
report = {'authority': 'static-original-declarations', 'runtimeExecuted': False, 'artifacts': [identity(p) for p in sorted(SOURCE.iterdir()) if p.is_file()], 'schema': schema, 'ui': definition, 'algorithmDefaults': [{n.findtext('Name'): n.findtext('DefaultValue')} for n in ET.parse(SOURCE / 'ReadCalibFileModuAlgorithm.xml').findall('./ParamItem')], 'limitations': ['Declared UI defaults and schema are not native runtime defaults.', 'Actual Process behavior and numerical results require controlled replay.']}
(ROOT / 'docs/plugins/read-calib-file-definition.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
for c in categories:
    print(c['name'], [(n.get('Name'), n['kind'], n['properties'].get('CurValue')) for n in c['controls']])

