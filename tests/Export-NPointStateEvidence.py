"""Index original Process execution in an explicitly synthetic ModuleFrame I/O host."""
import hashlib
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'analysis/full-audit/n-point-state'

def identity(path):
    return {'path': str(path), 'bytes': path.stat().st_size, 'sha256': hashlib.sha256(path.read_bytes()).hexdigest()}

run = Path(sys.argv[1]) if len(sys.argv) > 1 else max((p for p in BASE.glob('run-*') if (p / 'process.json').exists() and json.loads((p / 'process.json').read_text(encoding='utf-8-sig'))['exitCode'] == 0), key=lambda p: p.stat().st_mtime)
rows = [json.loads(line) for line in (run / 'probe.jsonl').read_text(encoding='utf-8-sig').splitlines() if line.strip()]
process = json.loads((run / 'process.json').read_text(encoding='utf-8-sig'))
provenance = json.loads((run / 'provenance.json').read_text(encoding='utf-8-sig'))
for item in provenance:
    assert identity(Path(item['path']))['sha256'] == item['sha256']
assert process['exitCode'] == 0 and rows[-1]['phase'] == 'finished'
assert rows[0]['phase'] == 'synthetic-host-installed' and rows[0]['importCount'] == 9
scenarios = [r for r in rows if 'rawMembers' in r]
assert len(scenarios) == 23
by_name = {r['phase']: r for r in scenarios}
def writes(name):
    return {r['name']: r for r in by_name[name]['writes'] if r['returned'] == 0}

assert by_name['default-no-input-31c']['code'] == 0xe000031c and not by_name['default-no-input-31c']['writes']
assert by_name['default-no-input-31b']['code'] == 0xe000031b and not by_name['default-no-input-31b']['writes']
assert by_name['bound-empty-images']['code'] == 0 and by_name['bound-empty-images']['rawMembers']['current'] == 0
assert by_name['mismatch-physical-counts']['code'] == 0xe0000001 and not by_name['mismatch-physical-counts']['writes']
assert by_name['batch-cross-translation-boundary']['rawMembers']['current'] == 9
assert by_name['rotation-three']['rawMembers']['current'] == 12
assert not by_name['full-no-input']['getters'] and by_name['full-no-input']['code'] == 0
assert 'Rotate' not in writes('trigger-two') and writes('trigger-two')['WorldRotateAngle']['values'] == [0]
assert 'Rotate' in writes('rotation-three') and 'WorldRotateAngle' not in writes('rotation-three')
assert writes('trigger-two')['Tilt']['values'] == [0]
clear = by_name['after-clear-raw']['rawMembers']
before = by_name['rotation-three']['rawMembers']
assert clear['current'] == 0 and clear['angles163b0'] == [0] * 32
assert clear['angles16430'] == before['angles16430'] and clear['angles16430'][:12] == list(range(1, 13))
assert writes('default-generated-physical')['WorldPointX']['values'] == [-1]
assert writes('default-generated-physical')['WorldPointY']['values'] == [-1]
assert by_name['short-physical-prefix']['rawMembers']['current'] == 4
assert by_name['teach-match-incomplete']['rawMembers']['current'] == 1
assert by_name['teach-match-missing-physical-x']['code'] == 0xe000031c and not by_name['teach-match-missing-physical-x']['writes']
assert by_name['manual-table']['code'] == 0 and not by_name['manual-table']['getters']
native = ROOT / 'analysis/full-audit/n-point-calibration'
static = [native / 'native-focused/18003c2f0.c', native / 'native-focused/18003c2f0.asm.txt', native / 'native-focused/18003b150.c', native / 'native-focused/18003b150.asm.txt', native / 'native/18003d230.c', native / 'native/1800364b0.c', native / 'native/180036690.c', native / 'native/180033260.c']
report = {
    'authority': 'controlled-original-module-process-with-synthetic-input-output-host',
    'platformRuntime': False, 'physicalDevicesTested': False, 'codeOrLicensePatched': False,
    'run': str(run.relative_to(ROOT)), 'process': process,
    'sourceProvenance': provenance,
    'hostImports': ['VM_M_GetFloat', 'VM_M_BatchGetFloat', 'VM_M_GetString', 'VM_M_SetFloat', 'VM_M_SetInt', 'VM_M_BatchSetFloat', 'VM_M_BatchSetInt', 'VM_M_GetModuleRuntimeInfo', 'VM_M_SetModuleRuntimeInfo'],
    'signatures': {
        'GetFloat': '(context,name,index,float* value,int* totalCount)',
        'BatchGetFloat': '(context,name,int capacity,float* values,int* totalCount)',
        'GetString': '(context,name,index,char* buffer,int capacity,int* bytes,int* count)',
        'SetFloat': '(context,name,index,float value,int flags)',
        'SetInt': '(context,name,index,int value)',
        'BatchSetFloat/Int': '(context,name,values*,int count,int flags)',
        'Get/SetModuleRuntimeInfo': '(moduleContext,buffer[0x2b0])'
    },
    'rawMemberInterpretation': {
        '0x164c0': 'current captured count; observed directly, not a published SDK result',
        '0x163b0': '32 float angle slots. WorldRotateAngle fixture 100+i stored here. Clear zeroes these slots.',
        '0x16430': '32 float angle slots. ImageRotateAngle fixture 1+i stored here. Clear leaves these slots unchanged.'
    },
    'findings': [
        'No bound image fields: synthetic getter e000031b/e000031c propagates unchanged with zero output calls. These synthetic codes do not establish which code the full platform chooses for an unbound subscription.',
        'Bound successful zero-count image arrays return success and publish incomplete outputs with current count zero.',
        'Image X/Y or physical X/Y count mismatch returns e0000001 before output publication.',
        'Batch at current 8 with 4 input points consumes only one translation point to count 9; unused input suffix is not forwarded to rotation stage in the same call.',
        'Once full, further input and empty input preserve captured points and publish calibration results without calling input getters.',
        'Incomplete output publishes WorldRotateAngle and Tilt zero; Rotate is absent. Completed output publishes Rotate and Tilt and does not publish WorldRotateAngle.',
        'Clear resets count and angle array 0x163b0, but preserves angle array 0x16430. Labels use member offsets to avoid treating SDK output names as field names.',
        'Image-only successful data supports generated physical coordinates despite absent optional physical/angle getters; default fixture first generated physical coordinate is (-1,-1).',
        'Short equal physical X/Y prefix is accepted; remaining image points receive generated physical coordinates.',
        'Matching nonempty Trigger and TeachFlagInput captures teach coordinates without incrementing incomplete point count; mismatching strings allow ordinary sampling.',
        'Teach match with missing PhysicalPointX propagates getter failure and publishes no output.',
        'Injected synthetic output failure preserves already executed output calls and captured state; failure is not a physical/platform transport observation.',
        'Manual table mode solves without reading synthetic input getters.'
    ],
    'scenarios': scenarios,
    'events': rows,
    'setParameters': [r for r in rows if r['phase'] == 'set'],
    'staticEvidence': [identity(p) for p in static],
    'rawEvidence': [identity(run / p) for p in ['probe.jsonl', 'provenance.json', 'process.json', 'build.log']],
    'limitations': [
        'Own native ctor instance binds a normally created official algorithm at source-confirmed member offset 0x15d98; full platform Init and process registration are not exercised.',
        'Only original module IAT slots are redirected; all original slot targets are verified against ModuleFrame exports and restored. DLL files, algorithm code and licensing remain unchanged.',
        'Successful output callback means the synthetic recorder accepted a value, not that an actual platform output port exists or accepted it.',
        'Runtime info buffer is synthetic zero/non-loop; platform loops, scheduling, thread concurrency and session ownership are unverified.',
        'Raw members are observational fixture data and are not public contracts.',
        'This suite does not establish complete native parity or equipment behavior.'
    ]
}
(ROOT / 'docs/plugins/n-point-state-evidence.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Indexed {len(scenarios)} original Process/raw-state scenarios; boundary, Clear, outputs and teaching checks passed')
