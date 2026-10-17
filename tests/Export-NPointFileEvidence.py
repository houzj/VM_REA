"""Index original module WriteXmlData; SDK ExportCalibFile is deliberately not substituted."""
import hashlib
import json
import sys
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'analysis/full-audit/n-point-file'

def identity(path):
    return {'path': str(path), 'bytes': path.stat().st_size, 'sha256': hashlib.sha256(path.read_bytes()).hexdigest()}

def fields(node):
    result = {'tag': node.tag, **node.attrib}
    if node.tag == 'CalibParam':
        result['value'] = node.findtext('ParamValue')
    elif node.tag == 'CalibPointFListParam':
        result['points'] = [{p.tag: p.text for p in point} for point in node.findall('PointF')]
    elif node.tag == 'CalibFloatListParam':
        result['values'] = [p.text for p in node.findall('ParamValue')]
    else:
        result['fields'] = {p.tag: p.text for p in node}
    return result

run = Path(sys.argv[1]) if len(sys.argv) > 1 else max((p for p in BASE.glob('run-*') if (p / 'process.json').exists() and json.loads((p / 'process.json').read_text(encoding='utf-8-sig'))['exitCode'] == 0), key=lambda p: p.stat().st_mtime)
run=run.resolve()
events = [json.loads(line) for line in (run / 'probe.jsonl').read_text(encoding='utf-8-sig').splitlines() if line.strip()]
process = json.loads((run / 'process.json').read_text(encoding='utf-8-sig'))
provenance = json.loads((run / 'provenance.json').read_text(encoding='utf-8-sig'))
for item in provenance:
    assert identity(Path(item['path']))['sha256'] == item['sha256']
assert process['exitCode'] == 0 and events[-1]['phase'] == 'finished'
files = []
for event in events:
    if event['phase'] != 'file':
        continue
    path = run / (event['name'] + '.xml')
    row = dict(event)
    if path.exists():
        row['artifact'] = identity(path)
        xml = ET.parse(path)
        row['root'] = xml.getroot().tag
        row['sections'] = {section.tag: [fields(node) for node in section] for section in xml.getroot()}
    files.append(row)
assert len(files) == 33 and sum('artifact' in f for f in files) == 27
by_name = {r['name']: r for r in files}
def output(name):
    return {p['ParamName']: p for p in by_name[name]['sections']['CalibOutputParam']}

for row in files:
    if 'artifact' in row:
        out = output(row['name'])
        assert out['RotCenterImagePoint']['fields']['RotCenterImageR'] == '-999'
        assert out['RotCenterWorldPoint']['fields']['RotCenterWorldR'] == '-999'
for camera in [1, 2, 3]:
    assert by_name[f'rot1-camera{camera}']['code'] == 0x80100011
    assert output(f'rot2-camera{camera}')['RotDirectionState']['value'] == '-999'
assert output('rot3-camera1')['RotDirectionState']['value'] == '1'
assert output('rot3-camera1')['IsRightCoorA']['value'] == '1'
assert output('rot3-camera3')['RotDirectionState']['value'] == '-1'
assert output('rot3-camera3')['IsRightCoorA']['value'] == '-1'
assert 'RotDirectionState' not in output('pattern2-camera1') and 'IsRightCoorA' not in output('pattern2-camera1')
assert output('mirror-rot0')['IsRightCoorA']['value'] == '1'
assert output('mirror-rot3')['IsRightCoorA']['value'] == '-1'
for relative in ['off', 'on']:
    assert by_name[f'relative-{relative}-teach-missing']['code'] == 0xe0000516
    captured = output(f'relative-{relative}-teach-captured')
    assert captured['SnapImage']['fields'] == {'SnapImagePointX': '5', 'SnapImagePointY': '6', 'SnapImageR': '7'}
    assert captured['TeachWorld']['fields'] == {'TeachWorldPointX': '101', 'TeachWorldPointY': '102', 'TeachWorldR': '103'}
    disabled = output(f'relative-{relative}-teach-disabled-after-capture')
    assert disabled['SnapImage']['fields'] == {'SnapImagePointX': '10', 'SnapImagePointY': '10', 'SnapImageR': '0'}
    assert set(disabled['SnapWorld']['fields'].values()) == {'-999'}
    assert set(disabled['TeachWorld']['fields'].values()) == {'-999'}
assert by_name['incomplete']['code'] == 0xe0000517
assert output('teach-missing-snap')['SnapImage']['fields'] == {'SnapImagePointX': '0', 'SnapImagePointY': '0', 'SnapImageR': '0'}
native = ROOT / 'analysis/full-audit/n-point-calibration/native'
reviewed = run / 'reviewed-NPointFile.cs'
report = {
    'authority': 'controlled-original-module-WriteXmlData-with-synthetic-input-output-host',
    'run': str(run.relative_to(ROOT)), 'process': process,
    'platformRuntime': False, 'physicalDevicesTested': False, 'sdkExportSubstituted': False,
    'sourceProvenance': provenance,
    'staticEvidence': [identity(native / p) for p in ['18003f990.c', '180031e60.c', '180031080.c', '180031740.c']],
    'events': events, 'files': files,
    'reviewedProductSnapshot': identity(reviewed),
    'constants': [
        {'rva': '0x593c0', 'type': 'float32', 'value': -999.0, 'use': 'local_784 sentinel, including both rotation-center R values'},
        {'rva': '0x593a8', 'type': 'float64', 'value': -99998.0, 'use': 'SnapImage X fallback threshold (<=)'},
        {'rva': '0x540a8', 'type': 'float32', 'value': 90.0},
        {'rva': '0x540b8', 'type': 'float32', 'value': -90.0},
        {'rva': '0x540bc', 'type': 'float32', 'value': -180.0},
        {'rva': '0x535c4', 'type': 'float32', 'value': 360.0},
        {'rva': '0x53534', 'type': 'float32', 'value': 0.0}
    ],
    'sourceConfirmedSemantics': [
        'ImagePointLst R reads member 0x16430 (ImageRotateAngle); WorldPointLst R reads 0x163b0 (WorldRotateAngle/DeltaAngle).',
        'RotDirectionState uses helper 0x31e60 over the first THREE rotation angles in each array; each series must be strictly monotonic with consistent adjacent signs.',
        'Image series wrap correction occurs only when first<-90 and third>90, or first>90 and third<-90; add 360 to image values in [-180,0) before monotonic checks. World series is not normalized by that branch.',
        'Direction helper compares image/world monotonic direction; same direction gives true, opposite false. CameraMode 3 inverts that flag before converting true/false to +1/-1.',
        'If fewer than three rotation points, direction is -999. If helper fails for nonmonotonic angles, both direction and right-coordinate nodes are OMITTED despite successful file save.',
        'IsRightCoorA helper 0x31080 inverts the calibration matrix via 0x31740, compares transformed axis angles, adjusts angular wrap, and combines orientation with the direction flag. It is not the current draft determinant-sign expression.',
        'Both RotCenterImageR and RotCenterWorldR are the same constant -999 in original WriteXmlData; all 26 successful controlled exports corroborate this.',
        'SnapImage/SnapWorld/TeachWorld nodes are written when raw HasTeach member 0x16501 is true, regardless of current manual mode or TeachEnable.',
        'With HasTeach true and TeachEnable true: SnapImage uses captured coordinates unless captured X<=-99998, in which case all SnapImage components use CalibOrigin point; SnapWorld always uses CalibOrigin world point; TeachWorld uses captured physical pose.',
        'With HasTeach true and TeachEnable false: SnapImage uses origin image pose; SnapWorld and TeachWorld are all -999.',
        'Trigger mode with TeachEnable and no HasTeach fails e0000516 before count check. Insufficient captured count fails e0000517.',
        'UseRelativeCoordinates toggle did not change these file-field branches in the identical no-rotation fixture; this is a bounded observation, not proof of no effect in all solve modes.'
    ],
    'reviewRecommendations': [
        {'target': 'NPointFile.RotationDirection', 'issue': 'Summed all-angle deltas accept [10,30,20]; original requires strict monotonicity of first three and omits both direction/right nodes if it fails.', 'evidence': ['pattern2-camera1', 'pattern2-camera3', 'pattern3-camera1']},
        {'target': 'NPointFile.Write IsRightCoorA', 'issue': 'For positive-orientation affine matrix with three same-direction rotations, original right=+1, draft computes -1. Reflection fixture reverses right as expected. Reconstruct original inverse-axis helper including zero/failure guards rather than guessing from determinant.', 'evidence': ['rot3-camera1', 'rot3-camera3', 'mirror-rot0', 'mirror-rot3']},
        {'target': 'NPointFile.Write pose gating', 'issue': 'Use hasTeach to decide whether to emit three poses. Current manual/TeachEnable gate drops original-retained nodes.', 'evidence': ['relative-off-teach-disabled-after-capture', 'relative-off-manual-after-teach']},
        {'target': 'NPointFile.Write SnapImage fallback', 'issue': 'Original checks captured X<=-99998; checking only float.MinValue equality misses the original sentinel threshold.', 'evidence': ['teach-missing-snap']},
        {'target': 'NPointFile.Write RotCenter R', 'issue': 'Keep -999. This request hypothesized nonconstant R, but original source and controlled files support the existing constant.'}
    ],
    'limitations': [
        'Original ctor instance plus normally created official algorithm and synthetic ModuleFrame host; full platform initialization and desktop operation remain outside this evidence.',
        'IAT changes only own-module input/output/runtime callbacks and restore original targets; no algorithm code, licensing, installed DLL or other process modifications.',
        'Rotation-one fixture has no algorithm result and export fails; it does not establish every possible rotation-one behavior.',
        'No claim of full homography right-coordinate parity: inverse-axis helper is source-confirmed; controlled reflection tests use affine matrices.',
        'No physical-device tests or complete native-equivalence claim.'
    ],
    'rawEvidence': [identity(run / p) for p in ['probe.jsonl', 'provenance.json', 'process.json', 'build.log']]
}
(ROOT / 'docs/plugins/n-point-file-evidence.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Indexed {len(files)} original WriteXmlData attempts, 27 XML files; direction/pose/failure assertions passed')
