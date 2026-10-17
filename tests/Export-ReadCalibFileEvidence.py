"""Index controlled original ReadCalibFile runs and original managed UI fixtures."""
import hashlib
import json
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
BASE=ROOT/'analysis/full-audit/read-calib-file'
def identity(path):
    return {'path':str(path),'bytes':path.stat().st_size,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()}
def latest(folder,report):
    return max((p for p in folder.glob('run-*') if (p/report).exists() and json.loads((p/'process.json').read_text(encoding='utf-8-sig'))['exitCode']==0),key=lambda p:p.stat().st_mtime)
run=latest(BASE/'runtime','probe.jsonl')
events=[json.loads(line) for line in (run/'probe.jsonl').read_text(encoding='utf-8-sig').splitlines()]
assert events[-1]['phase']=='finished'
provenance=json.loads((run/'provenance.json').read_text(encoding='utf-8-sig'))
for entry in provenance:
    assert identity(Path(entry['path']))['sha256']==entry['sha256']
ui=latest(BASE/'ui','probe.json')
ui_events=json.loads((ui/'probe.json').read_text(encoding='utf-8-sig'))
report={
 'authority':'controlled-original-ctor-instance/synthetic-ModuleFrame-host',
 'platformRuntime':False,'physicalDevicesTested':False,'nativeParityEstablished':False,
 'run':str(run.relative_to(ROOT)),'provenance':provenance,'events':events,
 'fixtures':[{'name':p.name,'identity':identity(p),'xml':p.read_text(encoding='utf-8-sig')} for p in sorted(run.glob('*.xml'))],
 'nativeMethods':[identity(p) for p in sorted((BASE/'native').glob('*.c'))],
 'ui':{'authority':'original-managed-parser-and-control/synthetic-model','run':str(ui.relative_to(ROOT)),'events':ui_events,'screenshots':[identity(ui/e['screenshot']) for e in ui_events if e.get('screenshot')]},
 'limitations':['Owned 0x180 ctor instance invokes source-confirmed LoadFile/GetParam/SetParam/Process; no platform Init.', 'Only original module ModuleFrame input/output/runtime imports are redirected, verified and restored; no algorithm/license patches.', 'Original managed rendering uses synthetic text fixtures and original dictionary/control; it is not desktop operation.', 'Malformed original XML memory faults are not recreated in the independent safe reader.', 'Dynamic schema changes require user confirmation at the independent host contract boundary.']
}
report['ui']['provenance']=json.loads((ui/'provenance.json').read_text(encoding='utf-8-sig'))
if (ui/'native-description.txt').exists():report['ui']['nativeDescription']=identity(ui/'native-description.txt')
(ROOT/'docs/plugins/read-calib-file-evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
extra={
 'authority':report['authority'],'nativeMethods':{'AnalyzeExtraIO':'0x9410','ProcessExtraIO':'0xc100','Process':'0xc520','SaveModuleData':'0xcf10','LoadModuleData':'0xbef0'},
 'xmlExamples':[e['value'] for e in events if e['phase']=='set' and e.get('key')=='DynamicOutData'],
 'controlledScenarios':[e for e in events if e['phase']=='run' and e.get('label','').startswith('extra-result-')],
 'specification':{
  'selection':'Output category / Items entries with Name and ValueType, and Others containing case-sensitive IsResultShow=t or IsResultShow=T substring',
  'types':'Case-insensitive int, float, string; other types do not publish',
  'outputOrder':['loaded typed outputs','nine per-component pose overrides','ModuStatus','extra result display values','runtime info'],
  'missingInput':'No extra write; loaded output of the same name remains; no previous-run display value retained',
  'floatFallback':'GetFloat nonzero return -> GetInt -> float; successful zero count does not fall back',
  'numericArrays':'Original loops getter count; independent descriptor uses IsArray=true and NumberArray for either int or float arrays',
  'string':'First item only; 511-byte initial capacity; 0xe0000012 resizes using returned byte count',
  'persistence':'cf10 always includes DynamicInData and DynamicOutData raw user-data records; bef0 restores them using SetParam',
  'displayResource':'ReadCalibFileModuDisplay.xml only active Data Record/datarecord maps Module Status:{0} to ModuStatus. Show Text is commented out; no TextDisplayGroup in its AlgorithmTab.'},
 'sharedDisplayEvidence':'docs/plugins/rotate-calibration-display-contract.json',
 'limitations':report['limitations']
}
(ROOT/'docs/plugins/read-calib-file-extra-io.json').write_text(json.dumps(extra,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f"ReadCalibFile indexed: {sum(e['phase']=='run' for e in events)} runs, {sum(e['phase']=='load' for e in events)} loads, {len(report['fixtures'])} XML fixtures")
