"""Index completed regression logs and source identities, without upgrading parity claims."""
from pathlib import Path
import hashlib
import json
import re
import sys

root = Path(__file__).resolve().parents[1]
run = root / (sys.argv[1] if len(sys.argv) > 1 else 'analysis/configuration-regression/final-model-host-20261017-v2')
followup = root / (sys.argv[2] if len(sys.argv) > 2 else 'analysis/configuration-regression/preview-and-lifetime-final')
summary = json.loads((run / 'summary.json').read_text(encoding='utf-8-sig'))
suite_only = len(sys.argv) > 2 and sys.argv[2] == '--suite-only'
extra = None if suite_only else json.loads((followup / 'summary.json').read_text(encoding='utf-8-sig'))
assert len(summary['records']) == 22 and all(r['exitCode'] == 0 for r in summary['records'])
assert extra is None or extra['exitCode'] == 0
programs = []
for r in summary['records']:
    log = Path(r['stdout'])
    if extra is not None and r['name'] == extra['program']:
        log = followup / 'stdout.log'
    lines = [s.strip() for s in log.read_text(encoding='utf-8-sig', errors='replace').splitlines() if s.strip()]
    result = lines[-1]
    count = re.search(r'\b(\d+)\b', result)
    programs.append({'name': r['name'], 'checks': int(count[1]) if count else None,
                     'result': result, 'stdout': str(log), 'stdoutSha256': hashlib.sha256(log.read_bytes()).hexdigest(),
                     'exitCode': 0, 'followupHostBuild': extra is not None and r['name'] == extra['program']})
sources = list((root / 'src/VisionPluginContracts').rglob('*.cs')) + list((root / 'src/VisionPluginContracts/Resources').glob('*'))
sources += [root / p for p in ['src/VisionEditor/ToolConfigurationHost.cs', 'src/VisionEditor/ToolExecutionScope.cs',
                              'src/VisionEditor/MainWindow.xaml.cs', 'src/VisionEditor/WorkflowExecution.cs',
                              'src/VisionPlugins.Calibration/ReadCalibFileConfiguration.cs',
                              'src/VisionFramework/ModuleExecutionScope.cs', 'src/VisionFramework/GraphExecutor.cs',
                              'src/VisionEditor/ToolDisplays.cs']]
sources += list((root / 'src/VisionPlugins.ImageSource').glob('*Configuration*.cs'))
sources = [p for p in sources if p.is_file() and 'obj' not in p.relative_to(root).parts]
payload = {
    'schema': 1, 'status': 'Independent desktop configuration-chain verification; manual acceptance pending; no delivery commit yet',
    'build': {'solution': 'VisionReplica.slnx', 'warnings': 0, 'errors': 0, 'log': str(run / 'build.log')},
    'programs': programs,
    'binaryAndDependencyIdentities': [str(run / 'summary.json')] + ([] if suite_only else [str(followup / 'summary.json')]),
    'sourceSnapshot': [{'path': p.relative_to(root).as_posix(), 'sha256': hashlib.sha256(p.read_bytes()).hexdigest()} for p in sorted(set(sources))],
    'evidenceLayers': {
        'originalSourceAndResources': ['configuration-model-evidence.json', 'configuration-template-catalog-evidence.json',
                                      'configuration-review-fixes.json', 'image-source-configuration-evidence.json',
                                      'read-calib-file-configuration.json'],
        'originalManagedRuntime': 'VM.Util.ParseInt/reflection probe in configuration-review-fixes.json; 12 integer conversion cases. No device/algorithm/desktop invocation.',
        'independentRuntime': 'Production shared controls/model windows, real GraphExecutor, document transactions and actual MainWindow event operations.',
        'originalDesktopDifferential': 'Not performed in this node; independent render PNGs are explicitly source-derived.',
        'manualAcceptance': 'Pending; launch helper uses actual MainWindow plus image-source/read providers, no test leaf factories.',
    },
    'limits': ['This report does not close every template or migrate every legacy tool window.',
               'Panal complex content, display-object binding/save contracts, ROI/compound/global/index bridges retain explicit gaps.',
               'SDK list width-500 embedding versus ordinary width-420 wrapper is not a validated SDK host layout.',
               'Native readiness, full subscription metadata, device behavior and original desktop pixels remain separate evidence layers.',
               'Procedure-scoped runtime identity is wired and independently tested in this branch; main identity/cache/backend integration remains pending in CONTRACT-REQUESTS.md. No main merge was performed.'],
}
target = root / 'docs/plugins/configuration-verification.json'
target.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Indexed {len(programs)} completed programs; manual acceptance remains pending.')
