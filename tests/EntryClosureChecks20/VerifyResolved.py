"""Offline closure gate over retained evidence. Does not load any assembly."""
import hashlib,json
from pathlib import Path
root=Path(__file__).resolve().parents[2]
audit=root/'analysis/full-audit/batch20-20261020/A'
proof=json.loads((audit/'standard-wpf/resolved-bindings.json').read_text(encoding='utf-8-sig'))
expected={'M':'M','C':'C','F':'F','A':'A','LevelT':'T','LevelD':'D','LevelI':'I','LevelW':'W','LevelE':'E','LevelC':'R','LevelO':'O'}
assert proof['Passed']==11 and len(proof['Bindings'])==11
rows=[]
for b in proof['Bindings']:
 assert b['TypeId']==-337 and b['Modifiers']=='Control'
 assert b['Key']==expected[b['Parameter']]
 assert any(t['value']=='KeyDownCommand' for t in b['RawNestedText'])
 rows.append({'scope':'DiagramItem','key':'Ctrl+'+b['Key'],'parameter':b['Parameter'],'bamlOffset':b['ElementOffset'],'handler':'VmModuleBaseDecorator.KeyDownCommandExecute','consumer':('ServerSDKAPI.IMVS_SetParamValue(LogLevel)' if b['Parameter'].startswith('Level') else {'M':'Process.Start(explorer.exe, XmlPath directory); catch swallowed','C':'CheckRunning -> Clipboard.ClearCopyInfo/AddCopyInfo + Position','F':'except DataQueueModule/GlobalCameraModule -> IsForbidden=true','A':'except DataQueueModule/GlobalCameraModule -> IsForbidden=false'}[b['Parameter']]),'currentMapping':('missing logging shortcut/backend' if b['Parameter'].startswith('Level') else {'M':'missing original XmlPath explorer operation','C':'WorkflowCanvas Ctrl+C, graph-focus gated','F':'WorkflowCanvas Ctrl+F, box-selection gated','A':'WorkflowCanvas Ctrl+A, box-selection gated'}[b['Parameter']])})
result={'passed':11,'scope':'Retained DiagramItem raw binding order, not global dispatcher completeness','implementationStatus':'partial','resolved':rows,'remaining':['Whole-shell/global routed input registration coverage not established by the selected carriers','All source-level context-specific consumers and current mapping are recorded in the report; native dispatcher and SDK semantics remain unknown'],'sourceProofSHA256':hashlib.sha256((audit/'standard-wpf/resolved-bindings.json').read_bytes()).hexdigest()}
(audit/'resolved-entry-gate.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf8')
print('PASS 11 retained DiagramItem bindings; A05.1 remains partial')
