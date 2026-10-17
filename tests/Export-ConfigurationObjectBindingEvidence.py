"""Documents source-confirmed object-property binding requirements; does not implement reflection fallback."""
from pathlib import Path
import xml.etree.ElementTree as E,hashlib,json
D=Path('D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab');W=Path.cwd()
specs={'BridgeItemBase.cs':['BindingPathA =>','if (_BridgeObj == null','BrigdgeSetter.Operate(null)','override int GetParamUnit','override int SetParamUnit'],
'GetDisplayObjectFeatureOperation.cs':['ExtraParams[0]','ExtraParams[1]','GetDisplayList().OfType','GetFeatureByName'],
'GetDisplayObjectOperation.cs':['ExtraParams[0]','GetDisplayList().OfType'],
'StringConfig.cs':['GetFrontAndSelfFilterWithoutTSTreeOperation','OperationParams = "int;float;string"','ItemsHelper.SetBindingProperty','private void FocusedCommandExecute'],
'FloatConfig.cs':['DecimalDigits = 3','double IncValue = 1.0'],
'ColorConfig.cs':['class ColorConfig']}
sources=[]
for name,needles in specs.items():
 p=D/name;b=p.read_bytes();lines=b.decode('utf-8-sig').splitlines();source=dict(path=str(p),sha256=hashlib.sha256(b).hexdigest(),claims=[])
 for needle in needles:
  hits=[i+1 for i,l in enumerate(lines) if needle in l];assert hits,(name,needle);source['claims'].append(dict(lines=hits,text=needle))
 sources.append(source)
r=E.parse(W/'tests/VisionConfiguration.ModelChecks/Fixtures/Group.xml').getroot();group=next(e for e in r.iter('GroupInfoItem') if e.get('Name')=='TextDisplayGroup');routes=[]
for e in group.iter():
 if e.tag in ['StringConfig','FloatConfig','ColorConfig']:
  op=e.find('BrigdgeSetter');prop=e.findtext('PropertyName');routes.append(dict(node=e.get('Name'),nodeType=e.tag,bridgeOperation=op.findtext('OperationName'),operationParameters=op.findtext('OperationParams'),property=prop,originalBindingPath='BridgeObjA.'+prop+'A',nativeUi={'StringConfig':'DeleteTextBox + PopupButton/SubTreeView180x180 focus/brace placeholder editing','FloatConfig':'SliderArrangeFloat StartValue object property binding','ColorConfig':'ColorPickerControlView CurrentColor object property binding'}[e.tag],source='Group.xml/TextDisplayGroup'))
out=dict(status='Source-confirmed contract requirement; unsupported independent config templates',sources=sources,routes=routes,
 requirements=['Resolve module-scoped display object by stable original object name Show Text; resolve feature Content/FontSize when requested.','Typed object-property reads/writes for Feature.Value, Object.okcolor/ngcolor/opacity; these are not execution parameter CurValue writes.','BridgeItemBase resolves object lazily on first getter, caches object; native GetParamUnit/SetParamUnit are no-ops.','Notify property edits to shared display object, persistence and preview without rerunning algorithm, preserve object lifetime across ordinary window reopen.','StringConfig placeholder insertion uses typed front-and-self int/float/string tree and current caret brace location; do not parse displayed labels into identity.','Explicit unsupported for missing object/property/type or feature insertion rules; never reflect arbitrary CLR property names from untrusted XML.'],
 suggestedIndependentContract={'request':['sourceNodeIdentity','objectName','optionalFeatureName','allowlistedPropertyId','expectedValueType'],'operations':['ReadProperty','WriteProperty','EnumerateTextPlaceholderTree','InsertTypedPlaceholderAtCaret'],'lifecycle':'Session resolves stable host-owned display objects; reads/edits trigger source notifications; no native DLL object dependence.'},
 boundary='These requirements are a proposed safe independent contract derived from source routes, not an implemented public API or full parity claim.')
(W/'docs/plugins/configuration-object-binding-contract-evidence.json').write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8');print(f'{len(sources)} source files, {len(routes)} exact TextDisplayGroup property routes')
