"""Extract board captions from shipped XML and language resources; no target execution."""
from pathlib import Path
import hashlib
import json
import xml.etree.ElementTree as ET

root = Path(__file__).resolve().parents[2]
installed = Path('D:/Program Files/VisionMaster4.4.0/Applications')
tab = installed / 'Module(sp)/x64/Calibration/IMVSCalibBoardCalibModu/IMVSCalibBoardCalibModuAlgorithmTab.xml'
group = installed / 'Module(sp)/Group.xml'
groups = {item.get('Name'): item for item in ET.parse(group).getroot().iter('GroupInfoItem')}
keys = set()

def visit(node, chain=()):
    if node.tag == 'GroupLinkItem':
        link = node.findtext('LinkName')
        if link in groups and link not in chain:
            visit(groups[link], chain + (link,))
    if node.tag in ('Tab', 'Category', 'GroupInfoItem') and node.get('Name'):
        keys.add(node.get('Name'))
    if node.tag in ('DisplayName', 'Description', 'ToolTipMessage', 'SelecetedTips') and node.text:
        keys.add(node.text)
    for child in node:
        visit(child, chain)

def identity(path):
    return dict(path=str(path), sha256=hashlib.sha256(path.read_bytes()).hexdigest())

visit(ET.parse(tab).getroot())
languages, sources, missing = {}, [identity(tab), identity(group)], {}
for language, filename in [('Chinese', 'zh-cn.xaml'), ('English', 'en-us.xaml')]:
    path = installed / 'Lang' / filename
    strings = {node.get('{http://schemas.microsoft.com/winfx/2006/xaml}Key'): ''.join(node.itertext())
               for node in ET.parse(path).getroot() if node.tag.endswith('}String')}
    languages[language] = {key: strings[key] for key in sorted(keys) if key in strings}
    missing[language] = sorted(keys - strings.keys())
    sources.append(identity(path))
output = root / 'src/VisionPlugins.CalibrationBoard/Resources/Localization.json'
output.write_text(json.dumps(languages, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
report = dict(scope='Shipped static caption extraction; no native desktop comparison', sources=sources,
              requestedKeys=len(keys), missing=missing, output=identity(output))
(root / 'docs/plugins/calibration-board-localization-evidence.json').write_text(
    json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print({language: len(values) for language, values in languages.items()})
