"""Index original N-point UI controls driven by a synthetic model; never native parity."""
import hashlib
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'analysis/full-audit/n-point-ui'
INSTALL = Path('D:/Program Files/VisionMaster4.4.0/Applications')

def identity(path):
    return {'path': str(path), 'bytes': path.stat().st_size, 'sha256': hashlib.sha256(path.read_bytes()).hexdigest()}

run = Path(sys.argv[1]) if len(sys.argv) > 1 else max((p for p in BASE.glob('run-*') if (p / 'process.json').exists() and (p / 'probe.json').exists()), key=lambda p: p.stat().st_mtime)
rows = json.loads((run / 'probe.json').read_text(encoding='utf-8-sig'))
process = json.loads((run / 'process.json').read_text(encoding='utf-8-sig'))
assert process['exitCode'] == 0
provenance = json.loads((run / 'provenance.json').read_text(encoding='utf-8-sig'))
for source in provenance:
    assert identity(Path(source['path']))['sha256'] == source['sha256']
snapshots = [r for r in rows if 'items' in r]
assert len(snapshots) == 10
assert {r['phase'] for r in snapshots} == {'default-trigger-point', 'trigger-input-1', 'trigger-input-0', 'teach-1', 'teach-0', 'freedom-1', 'freedom-2', 'freedom-3', 'manual-input', 'point-list'}
screenshots = [identity(run / r['image']) for r in snapshots if r.get('image')]
assert len(screenshots) == 1
items = {r['phase']: {i['values'].get('NameA'): i['values'] for i in r['items']} for r in snapshots}
assert items['trigger-input-1']['PhyPointX']['CustomVisibleA'] == 'True'
assert items['trigger-input-1']['PhyPoint']['CustomVisibleA'] == 'False'
assert items['teach-1']['TeachFlagInput']['CustomVisibleA'] == 'True'
assert items['teach-0']['TeachFlagInput']['CustomVisibleA'] == 'False'
manual = next(r for r in snapshots if r['phase'] == 'manual-input')
assert next(c for c in manual['categories'] if c['name'] == 'Tab_Phys Coord Params')['visible'] is False
paths = ['myLibs/Apps.XmlParser.dll', 'myLibs/Apps.XmlUI.dll', 'myLibs/VMControls.WPF.dll', 'myLibs/VM.Util.dll', 'myLibs/VMControls.Interface.dll', 'myLibs/VMControls.BaseInterface.dll', 'Module(sp)/Group.xml', 'Module(sp)/x64/Calibration/IMVSNPointCalibModu/IMVSNPointCalibModuControl.dll', 'Module(sp)/x64/Calibration/IMVSNPointCalibModu/IMVSNPointCalibModuAlgorithmTab.xml']
report = {
    'authority': 'original-managed-controls-and-XML-parser-with-synthetic-model',
    'nativeModuleCreated': False, 'physicalDevicesTested': False, 'desktopOperation': False,
    'run': str(run.relative_to(ROOT)), 'process': process,
    'sourceProvenance': provenance, 'installedEvidence': [identity(INSTALL / p) for p in paths],
    'snapshots': snapshots, 'screenshots': screenshots,
    'failures': [r for r in rows if 'error' in r],
    'rawEvidence': [identity(run / p) for p in ['probe.json', 'exceptions.log', 'first-chance.log', 'process.json']],
    'limitations': [
        'Full original parameter panel rendering failed in Baml2006SchemaContext.ResolveBamlType with NotImplementedException; no full-panel screenshots claimed.',
        'Only original NPointList was rendered. It was hosted in an owned offscreen test window, not opened by original desktop operation.',
        'Point count 9+3 and all-zero point values are synthetic fixture data, not native results or defaults.',
        'Enumeration/Boolean CurValueA uses original setters to observe XML visibility; native execution commands deliberately not invoked.',
        'Point/XY radio uses original ValueChangedCommand; subscription picker selection and bound physical coordinate visibility not validated.',
        'Unresolved localization keys and replacement characters in original parser strings are retained without inventing desktop labels.',
        'Earlier failed trials retained only in ignored analysis folder; this index selects the completed exit-zero run.'
    ]
}
(ROOT / 'docs/plugins/n-point-ui-evidence.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Indexed {len(snapshots)} synthetic-model original-parser snapshots, {len(screenshots)} original-control screenshot, {len(report["failures"])} explicit render failure')
