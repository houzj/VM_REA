"""Summarize original evidence without copying binary/decompiler projects into Git."""
import argparse
import hashlib
import json
from pathlib import Path
import struct
import xml.etree.ElementTree as ET

parser = argparse.ArgumentParser()
parser.add_argument('--evidence-root', type=Path, default=Path('D:/MyWork/REA逆向测试/analysis'))
parser.add_argument('--runtime', type=Path, default=Path('analysis/full-audit/image-source-runtime/run-85fcec94893c4a63bd0106b9b2889789/probe.json'))
parser.add_argument('--output', type=Path, default=Path('docs/plugins/image-source-evidence.json'))
args = parser.parse_args()
audit = args.evidence_root / 'full-audit'
decompiled = args.evidence_root / 'decompiled'
definition = audit / 'image-source-audit'
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
sources = [definition / n for n in ['ImageSourceModuleAlgorithm.xml', 'ImageSourceModuleAlgorithmTab.xml', 'ImageSourceModule.xml']]
sources += [audit / n for n in ['image-source-native-0.c', 'image-source-native-2.c', 'image-source-native-3.c', 'image-source-native-7.c', 'image-source-native-9.c', 'image-source-native-10.c', 'image-source-native-extra-5.c', 'image-source-native-extra-25.c', 'image-source-native-extra-26.c', 'image-source-media-decode-implementation.c']]
sources += [decompiled / 'ImageSourceModuleControl' / n for n in ['ImageSourceModuleControl/LocalImageListView.xaml.cs', 'Apps.XmlParser.ParamTab/LocalImageList.cs', 'Apps.XmlParser.ParamTab/EnumerationCameraForImage.cs']]
sources += [decompiled / 'Apps.XmlParser/Hik.Xml.BaseNode.decompiled.cs', decompiled / 'Apps.XmlParser/Apps.XmlParser.ParamTab.AlgorithmTabRoot.decompiled.cs', decompiled / 'Apps.Style/buttonstyle/togglebuttonstyle.xaml']
sources += [decompiled / folder / 'resources' / name for folder, name in [('Apps.Style', 'ic_btn_link切换_disable.png'), ('Apps.XmlUI', 'ic_btn_link_disable.png'), ('Apps.XmlUI', 'ic_modal_camera_normal.png')]]
ui = ET.parse(definition / 'ImageSourceModuleAlgorithmTab.xml').getroot()
algorithm = {n.findtext('Name'): n.findtext('DefaultValue') for n in ET.parse(definition / 'ImageSourceModuleAlgorithm.xml').getroot()}
parameters = []
for node in ui.findall('.//Items/*'):
    if 'Name' not in node.attrib:
        continue
    rules = []
    for trigger in node.findall('./Triggers/*'):
        conditions = [{c.tag: c.text for c in condition} for condition in trigger.findall('./Conditions/Condition')]
        if trigger.find('Property') is not None:
            conditions.append({'Property': trigger.findtext('Property'), 'Value': trigger.findtext('Value')})
        rules.append({'conditions': conditions, 'setters': [{c.tag: c.text for c in setter} for setter in trigger.findall('./Setters/Setter')]})
    parameters.append({'name': node.attrib['Name'], 'controlType': node.tag, 'algorithmDefault': algorithm.get(node.attrib['Name']), 'uiDefault': node.findtext('DefaultValue'), 'uiCurrent': node.findtext('CurValue'), 'initialVisibility': node.findtext('CustomVisible', 'True'), 'access': node.findtext('AccessMode'), 'minimum': node.findtext('MinValue'), 'maximum': node.findtext('MaxValue'), 'rules': rules})

binary = Path('analysis/native-input/ImageSourceModule.dll')
b = binary.read_bytes()
target_sha = sha(binary)
assert target_sha == '91ea3659be97e51edeab0c913779d4555d51dcd0d6288086fbf9916d35b942db'
pe = struct.unpack_from('<I', b, 60)[0]
count = struct.unpack_from('<H', b, pe + 6)[0]
optional_size = struct.unpack_from('<H', b, pe + 20)[0]
section_at = pe + 24 + optional_size
sections = [struct.unpack_from('<IIII', b, section_at + i * 40 + 8) for i in range(count)]
def literal(rva):
    for size, address, raw_size, offset in sections:
        if address <= rva < address + min(size, raw_size):
            return b[offset + rva - address:offset + rva - address + 16].split(b'\0')[0].decode('ascii')
    raise ValueError('unmapped RVA')
literals = {hex(0x180000000 + rva): literal(rva) for rva in [0x31960, 0x31964, 0x31968, 0x3196c, 0x31970, 0x2dacc]}
runtime = json.loads(args.runtime.read_text(encoding='utf-8-sig'))
# Exclude absolute fixture paths and large visual trees; retain results and limits.
runtime_summary = [{key: value for key, value in record.items() if key not in {'nodes', 'path', 'original', 'supplied', 'installed', 'visuals', 'exceptions'}} for record in runtime]
for record in runtime_summary:
    for item in record.get('items', []):
        if item.get('fields', {}).get('Name') == 'CurrentImagePath':
            item.get('properties', {}).pop('Value', None)
sample = Path('analysis/images/circle-find-sample.bmp')
helpers = Path('analysis/full-audit/image-source-state')
sdk_binary = Path('D:/Program Files/VisionMaster4.4.0/Development/V4.x/ComControls/Assembly/Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModule.dll')
assert sha(sdk_binary) == target_sha, 'SDK-distribution DLL differs from the analyzed application DLL'
result = {'schema': 1, 'targetSha256': target_sha, 'sdkDistributionIdentity': {'path': str(sdk_binary), 'sha256': sha(sdk_binary), 'limitation': 'SDK installation copy hash checked; not an observed native call-stack module attribution'}, 'sourceEvidence': [{'path': str(p), 'sha256': sha(p)} for p in sources], 'parameters': parameters, 'nativeLiteralBytes': literals, 'helperAnalysis': [{'path': str(p), 'sha256': sha(p)} for p in sorted(helpers.glob('18000*.c'))], 'runtimeHarnessSha256': sha(Path('tests/NativeEvidence/OriginalImageSourceProbe.cs')), 'runtimeSource': str(args.runtime), 'runtimeSourceSha256': sha(args.runtime), 'runtimeResults': runtime_summary, 'sampleSha256': sha(sample), 'desktopUi': {'local': 'historical screenshot only; no fresh complete mode capture', 'camera': 'XML/managed-source rules only; fresh capture unavailable', 'sdk': 'XML/managed-source rules only; fresh capture unavailable'}, 'rejectedUiEvidence': 'SDK hosted controls rendered empty; no mode screenshot accepted', 'completeParity': False}
accepted = [r for r in runtime if r.get('phase') == 'sdk-hosted-ui' and r.get('accepted')]
if accepted:
    result['sdkHostedScreenshots'] = [{'path': str(p), 'sha256': sha(p), 'pixels': list(struct.unpack_from('>II', p.read_bytes(), 16))} for p in sorted(args.runtime.parent.glob('original-sdk-ui-*.png'))]
    result['sdkHostedUi'] = {'captureCount': len(accepted), 'modes': sorted({r['mode'] for r in accepted}), 'context': 'Original SDK control, byte-identical writable XML copy, 420x700 viewport and expanded 420x1600 inspection; not desktop capture', 'blankCaptureRootCause': 'Vendor BaseNode.loadXml changes file attributes on original read-only evidence; exception swallowed in AlgorithmTabRoot constructor', 'limitations': 'Host-local resource fallback exceptions and BooleanConverter binding errors remain; camera is unbound; subscription popups unverified'}
    result['rejectedUiEvidence'] = 'Earlier empty captures remain rejected; later accepted SDK captures do not establish desktop parity'
args.output.parent.mkdir(parents=True, exist_ok=True)
args.output.write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Exported {len(parameters)} UI declarations, {len(sources)} source hashes, {len(runtime_summary)} runtime records')
