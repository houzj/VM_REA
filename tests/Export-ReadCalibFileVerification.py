"""Index dedicated ReadCalibFile verification without claiming platform parity."""
import hashlib
import json
import re
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'analysis/full-audit/read-calib-file'
def identity(path):
    data=path.read_bytes()
    return {'path':path.relative_to(ROOT).as_posix(),'bytes':len(data),'sha256':hashlib.sha256(data).hexdigest()}
checks=(BASE/'checks.log').read_text(encoding='utf-8-sig')
count=int(re.search(r'ReadCalibFile.PluginChecks: (\d+) checks passed',checks)[1])
assert count>=810 and 'Unhandled exception' not in checks
build=(BASE/'build.log').read_text(encoding='utf-8-sig')
assert '0 个警告' in build and '0 个错误' in build
evidence=json.loads((ROOT/'docs/plugins/read-calib-file-evidence.json').read_text(encoding='utf-8'))
sources=[]
for pattern in ['src/VisionPlugins.Calibration/ReadCalibFile*','src/VisionPlugins.Calibration/Resources/ReadCalibFile*','tests/ReadCalibFile.PluginChecks/*.cs','tests/ReadCalibFile.PluginChecks/*.csproj','tests/NativeEvidence/*ReadCalibFile*','tests/Export-ReadCalibFile*']:
    sources.extend(p for p in ROOT.glob(pattern) if p.is_file())
report={
 'pluginId':'vision.read-calib-file','implementation':'VisionPlugins.Calibration.ReadCalibFilePlugin',
 'checksPassed':count,'buildWarnings':0,'buildErrors':0,
 'originalProcessScenarios':sum(e['phase']=='run' for e in evidence['events']),
 'originalFileLoads':sum(e['phase']=='load' for e in evidence['events']),
 'originalCacheLoads':sum(e['phase']=='load-cache' for e in evidence['events']),
 'originalXmlFixtures':len(evidence['fixtures']),'originalManagedUiFixtures':len(evidence['ui']['events']),
 'numericComparison':'float32 exact bits / integer exact / strings exact / output presence and array length exact',
 'platformRuntimeTested':False,'physicalDevicesTested':False,'completeNativeParityEstablished':False,
 'sourceAndHelpers':[identity(p) for p in sorted(set(sources))],
 'verificationArtifacts':[identity(p) for p in [BASE/'build.log',BASE/'checks.log',ROOT/'docs/plugins/read-calib-file-definition.json',ROOT/'docs/plugins/read-calib-file-evidence.json',ROOT/'docs/plugins/read-calib-file-extra-io.json']],
 'independentScreenshots':[identity(p) for p in sorted((BASE/'checks').glob('independent-*.png'))],
 'manualAcceptanceFile':identity(BASE/'checks/acceptance.xml'),
 'manualAcceptanceStatus':'Pending primary-host launch and human acceptance; no commit/push by this subagent.',
 'limitations':evidence['limitations']+['Primary agent owns host registration, integrated regression and manual UI acceptance.']
}
(ROOT/'docs/plugins/read-calib-file-verification.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f'ReadCalibFile verification indexed: {count} checks, {len(report["sourceAndHelpers"])} source/helper files')
