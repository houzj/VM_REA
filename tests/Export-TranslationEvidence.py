"""Index static resources and controlled original Translation module executions."""
import hashlib
import json
import sys
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'analysis/full-audit/translation-calibration'
def identity(path):
    return {'path': str(path), 'bytes': path.stat().st_size, 'sha256': hashlib.sha256(path.read_bytes()).hexdigest()}
run = Path(sys.argv[1]) if len(sys.argv) > 1 else max((p for p in (BASE / 'runtime').glob('run-*') if (p / 'process.json').exists() and json.loads((p / 'process.json').read_text(encoding='utf-8-sig'))['exitCode'] == 0), key=lambda p: p.stat().st_mtime)
events = [json.loads(line) for line in (run / 'probe.jsonl').read_text(encoding='utf-8-sig').splitlines() if line.strip()]
assert events[-1]['phase'] == 'finished'
provenance = json.loads((run / 'provenance.json').read_text(encoding='utf-8-sig'))
for source in provenance:
    assert identity(Path(source['path']))['sha256'] == source['sha256']
scenarios = [r for r in events if 'rawMembers' in r]
assert len(scenarios) >= 40 and sum(r['phase'] == 'instance-start' for r in events) >= 14
files = []
for row in events:
    if row['phase'] != 'file':
        continue
    item = dict(row)
    path = run / (row['name'] + '.xml')
    if path.exists():
        item['artifact'] = identity(path)
        item['xml'] = path.read_text(encoding='utf-8-sig')
    files.append(item)
ui = max((p for p in (BASE / 'ui').glob('run-*') if (p / 'probe.json').exists()), key=lambda p: p.stat().st_mtime)
ui_rows = json.loads((ui / 'probe.json').read_text(encoding='utf-8-sig'))
screens = [identity(ui / r['image']) for r in ui_rows if r.get('image')]
native = BASE / 'native'
report = {
    'authority': 'controlled-original-instance/synthetic-ModuleFrame-input-output-host',
    'platformRuntime': False, 'physicalDevicesTested': False, 'nativeParityEstablished': False,
    'run': str(run.relative_to(ROOT)), 'sourceProvenance': provenance,
    'events': events, 'scenarios': scenarios, 'files': files,
    'ui': {'authority': 'original-controls-and-parser/synthetic-model', 'run': str(ui.relative_to(ROOT)), 'events': ui_rows, 'screenshots': screens},
    'staticEvidence': [identity(p) for p in sorted(native.iterdir()) if p.is_file()],
    'rawEvidence': [identity(run / p) for p in ['probe.jsonl', 'provenance.json', 'process.json', 'build.log']],
    'limitations': ['Own native ctor instance binds a normally created original algorithm at source-confirmed member 0x15d98; full platform Init is not exercised.', 'Nine own-module ModuleFrame IAT slots supply synthetic input/output/runtime services; original targets verified and restored. No algorithm or licensing patch.', 'Synthetic callback acceptance is not evidence of a real platform output port.', 'Original full parameter-panel BAML rendering is blocked by NotImplementedException; original point-table control rendered independently with synthetic data.', 'No device tests and no complete native-equivalence claim.']
}
(ROOT / 'docs/plugins/translation-calibration-evidence.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Indexed {len(scenarios)} original Process/raw-state scenarios, {len(files)} file attempts and {len(screens)} original-control screenshots')
