"""Index dedicated Translation verification; raw local evidence stays untracked."""
import hashlib
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
BASE = ROOT / 'analysis/full-audit/translation-calibration'

def identity(path):
    data = path.read_bytes()
    return {'path': path.relative_to(ROOT).as_posix(), 'bytes': len(data), 'sha256': hashlib.sha256(data).hexdigest()}

checks = (BASE / 'checks.log').read_text(encoding='utf-8-sig')
count = int(re.search(r'TranslationCalibration.PluginChecks: (\d+) checks passed', checks)[1])
assert count >= 7445 and 'Unhandled exception' not in checks
build = (BASE / 'build.log').read_text(encoding='utf-8-sig')
assert '0 个警告' in build and '0 个错误' in build
evidence = json.loads((ROOT / 'docs/plugins/translation-calibration-evidence.json').read_text(encoding='utf-8'))
files = []
for pattern in ['src/VisionPlugins.Calibration/Translation*', 'src/VisionPlugins.Calibration/Resources/Translation*', 'tests/TranslationCalibration.PluginChecks/*.cs', 'tests/TranslationCalibration.PluginChecks/*.csproj', 'tests/NativeEvidence/*Translation*', 'tests/Export-Translation*']:
    files += [p for p in ROOT.glob(pattern) if p.is_file()]
report = {
    'pluginId': 'vision.translation-calibration', 'implementation': 'VisionPlugins.Calibration.TranslationPlugin',
    'checksPassed': count, 'buildWarnings': 0, 'buildErrors': 0,
    'originalScenarios': len(evidence['scenarios']), 'fileAttempts': len(evidence['files']),
    'successfulOriginalFiles': sum('xml' in f for f in evidence['files']),
    'originalRuntime': evidence['authority'], 'platformDesktopTested': False,
    'physicalDevicesTested': False, 'completeNativeParityEstablished': False,
    'sourceAndHelpers': [identity(p) for p in sorted(set(files))],
    'verificationArtifacts': [identity(p) for p in [BASE / 'build.log', BASE / 'checks.log', ROOT / 'docs/plugins/translation-calibration-evidence.json', ROOT / 'docs/plugins/translation-calibration-definition.json']],
    'independentScreenshots': [identity(p) for p in sorted((BASE / 'checks').glob('independent-*.png'))],
    'limitations': evidence['limitations'] + ['Shared numerical kernel retains its documented degenerate and strong robust-weight boundaries.', 'Host registration/actual WPF integrated checks are indexed below; no full original desktop parity claim.']
}
host_log=BASE/'host-checks.log'
host_build=BASE/'host-build.log'
if host_log.exists() and host_build.exists():
 host_count=int(re.search(r'PASS (\d+) actual WPF checks',host_log.read_text(encoding='utf-8-sig'))[1])
 assert host_count==267
 assert '0 个警告' in host_build.read_text(encoding='utf-8-sig') and '0 个错误' in host_build.read_text(encoding='utf-8-sig')
 report['hostIntegration']={'checks':host_count,'buildWarnings':0,'buildErrors':0,'artifacts':[identity(host_log),identity(host_build)],'sources':[identity(ROOT/p) for p in ['src/VisionEditor/MainWindow.xaml.cs','src/VisionEditor/EditorForm.cs','src/VisionEditor/tool-catalog.json','tests/VisionEditor.WpfChecks/Program.cs','VisionReplica.slnx']]}
(ROOT / 'docs/plugins/translation-calibration-verification.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Translation verification indexed: {count} checks, {len(report["sourceAndHelpers"])} source/helper files')
