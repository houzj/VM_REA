from pathlib import Path
import json,hashlib,xml.etree.ElementTree as ET,collections
base=Path('analysis/full-audit/n-image-calibration/configuration-model')
parser=base/'parser/Apps.XmlParser.ParamTab'
ui=base/'ui'
original=Path('D:/Program Files/VisionMaster4.4.0/Applications')
xml=original/'Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModuAlgorithmTab.xml'
run=Path('analysis/full-audit/n-image-calibration/configuration-runtime/run-206ca74b4a0e4a24a3c0147ab8d2fe98')
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def evidence(p,anchor,count=16):
 p=Path(p); lines=p.read_text(encoding='utf-8-sig').splitlines(); hits=[i for i,s in enumerate(lines) if anchor in s]
 if not hits:raise Exception(str(p)+' '+anchor)
 i=hits[0];return {'path':str(p).replace('\\','/'),'sha256':sha(p),'line':i+1,'endLine':min(len(lines),i+count),'excerpt':'\n'.join(lines[i:i+count])}
def item(id,claim,p,anchor,count=16):return {'id':id,'authority':'decompiled-original-source','claim':claim,'source':evidence(p,anchor,count)}
items=[]
def add(id,claim,file,anchor,count=16):items.append(item(id,claim,parser/file,anchor,count))
items.append(item('xml-reflection','XML attributes/children map to public fields; list tags are reflected registered model types, with parent and AfterAddedToArray hooks. Unknown names are not a generic widget schema.','D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser/Hik.Xml.BaseNode.decompiled.cs','public void LoadNodeListE',95))
add('root-init','AlgorithmTabRoot loads XML with parser namespace, then initializes and dispatches XML-loaded hooks.','AlgorithmTabRoot.cs','public override BaseNode Init',32)
add('group-expansion','GroupLinkItem removes itself, clones the named shared GroupInfoItem and appends its child models with the original parent. It is an expansion macro, not a rendered group.','GroupLinkItem.cs','public override void AfterAddedToArray',36)
add('category-inheritance','Category supplies inherited subscription/delete-binding/execution settings; child rows retain group identity.','Category.cs','public bool CanTriggerRun',10)
add('flatten','Tab flattens category items for the grouped renderer and name lookup.','Tab.cs','InitCollection',40)
add('trigger-order','Property changes reset matching working operations before applying the matching triggers; execution is deferred until nested trigger processing finishes.','NameValueItem.cs','protected void TriggerBusiness',40)
add('condition','MultiTrigger resolves local or other-item properties, converts typed expected values and combines equality conditions with AND(default) or OR.','MultiTrigger.cs','public void OnTrigger',72)
add('setter','Setter invokes registered operation and can additionally assign target PropertyName+A; operation parameters are split on semicolon.','Setter.cs','public void Operate',35)
add('operation-dispatch','OperationManager resolves operation classes from ActivatorWrap registration; exact operation spelling is part of XML identity.','OperationManager.cs','CreatInstanceBySetter',45)
add('reversible-visibility','VisibleOperation saves previous visibility and restores it on ReOperate, rather than permanently toggling a row.','VisibleOperation.cs','public void Operate',34)
add('commit','Command click performs its setter operation before SetParamUnit and change notification. Model values, user commits and initialization are distinct.','Command.cs','ClickCommand',45)
add('parameter-binding','ValueBase dynamically creates SubData subscription selector and persists @ComName subscription state; this is separate from tool input IO wiring.','ValueBase.cs','public ButtonSelecter SubData',75)
add('input-binding','SetFilterSourceOperation resolves and validates typed input/output references then mutates RelateIO and RelateIndex; display text alone is not a binding.','SetFilterSourceOperation.cs','RelateIO =',25)
add('change-execution','PreExecuteRun checks global IsParamModifyExecute and inherited CanTriggerExecute, defers nested work, then invokes the module run path with busy guards.','NameValueItem.cs','public void PreExecuteRun()',75)
add('close','The original CloseParamsTabControl delegates to the window close command; this alone does not establish modal versus nonmodal lifetime.','BottomCommand.cs','public void CloseParamsTabControl()',12)
add('explicit-run','Explicit SelfExcuteModule exists in the command model but NImage does not declare it in its footer.','BottomCommand.cs','public void SelfExcuteModule()',39)
add('save','SaveCalibPath searches tabs for hidden SaveFileDialog and invokes its save action.','BottomCommand.cs','public void SaveCalibPath()',28)
add('clear-feedback','Clear writes owner.Params[Clear]=1 then optionally reports success/failure. NeedPopTip is post-action feedback, not Yes/No permission.','BottomCommand.cs','public new void Clear()',28)
add('reset','ResetAlgoParams has its own confirmation, clears run parameter bindings, calls owner.ResetParam and reloads item values under reset guard.','Button_ResetAlgoParams.cs','public ICommand ClickCommand',90)
items.append(item('template-dispatch','ParamUserControl binds ContentTemplate to DisplayType and can load plugin assembly DataTemplate dictionaries. ExternDisplayType explicitly overrides dispatch.',ui/'Apps.XmlUI/ParamUserControl.xaml.cs','private void SetDefaultBinding',36))
items.append(item('template-fallback','UserTempConverter selects an exact resource key; absent keys use Default template.',ui/'Apps.XmlUI/UserTempConverter.cs','public object Convert',16))
items.append(item('host-layout','Common shell is TabsA plus grouped scroll content and fixed 32px footer; left commands are shown only on Tab_Run Params. Footer content comes from declared command collections.',ui/'Apps.XmlUI/ParamsTabUserControl.xaml','<ItemsControl Height="32" HorizontalAlignment="Left"',47))
items.append(item('custom-condition','NImage plugin HidePhysicInfoOperation hides automatic physical coordinate fields whenever angle input or active point/XY inputs are bound. Both XY inputs empty is required to show automatic controls.',Path('analysis/full-audit/n-image-calibration/managed/control/Apps.XmlParser.ParamTab/HidePhysicInfoOperation.cs'),'bool flag =',29))
for key in ['Float','Integer','RadioSelecter','EnumerationG','ButtonSelecter','OpenFileForCalibDialog','BottomCommand']:
 items.append(item('template-'+key,'Exact original '+key+' template bindings and control composition; preserve command and subscription bindings, not merely approximate field appearance.',ui/'styles/paramusercontroldatatemplate.xaml','<DataTemplate x:Key="'+key+'">',min(30, 30)))
root=ET.parse(xml).getroot(); lines=xml.read_text(encoding='utf-8-sig').splitlines()
raw=[]
for elem in root.iter():
 if elem.tag in ['Trigger','MultiTrigger','Condition','Setter','GroupLinkItem','BottomCommand','Button_ResetAlgoParams']:
  text=ET.tostring(elem,encoding='unicode'); raw.append({'tag':elem.tag,'name':elem.get('Name'),'xml':text})
from xml.parsers import expat
locations=[]
xp=expat.ParserCreate()
xp.StartElementHandler=lambda name,attrs: locations.append({'tag':name,'name':attrs.get('Name'),'line':xp.CurrentLineNumber}) if name in ['Trigger','MultiTrigger','Condition','Setter','GroupLinkItem','BottomCommand','Button_ResetAlgoParams'] else None
xp.Parse(xml.read_bytes(),True)
for node,location in zip(raw,locations): node['line']=location['line']
snap=json.loads((run/'probe.json').read_text(encoding='utf-8-sig'))
model=next(x for x in snap if x['phase']=='configuration-model')
states=[]
for s in snap:
 if 'items' in s:states.append({'phase':s['phase'],'categories':s['categories'],'items':[{'type':x['type'],**x['values']} for x in s['items']]})
report={'schemaVersion':1,'scope':'Original XML → parameter model → template dispatch, triggers, subscriptions and commands; NImage complex sample. Framework design evidence, not plugin closure.','authorityLegend':{'decompiled-original-source':'Original shipped managed assemblies decompiled; statement tied to excerpt and binary identity.','controlled-original-parser':'Original parser/custom operations instantiated with explicitly synthetic parameter and IVmModule owners; no real VM IO mutation or desktop window.'},'binaries':[{'path':str(p).replace('\\','/'),'sha256':sha(p)} for p in [original/'myLibs/Apps.XmlParser.dll',original/'myLibs/Apps.XmlUI.dll',original/'myLibs/VM.Util.dll',original/'Module(sp)/x64/Calibration/IMVSNImageCalibModu/IMVSNImageCalibModuControl.dll']], 'evidence':items,'nImageXml':{'path':str(xml).replace('\\','/'),'sha256':sha(xml),'tagCounts':dict(collections.Counter(x.tag for x in root.iter())),'operations':sorted(set(x.findtext('OperationName') for x in root.iter('Setter') if x.findtext('OperationName'))),'rawLogicAndCommands':raw},'controlledRuntime':{'authority':'controlled-original-parser','probeSource':'tests/NativeEvidence/OriginalNImageConfigurationProbe.cs','runner':'tests/NativeEvidence/Invoke-OriginalNImageConfigurationProbe.ps1','run':str(run).replace('\\','/'),'exitCode':0,'records':len(snap),'model':model,'states':states,'nativeParameters':False,'desktopRendered':False},'boundaries':['Full original BAML host rendering failed separately with Baml2006SchemaContext.ResolveBamlType NotImplementedException; model evidence is not a full original desktop screenshot.','Synthetic-owner attempts to assign fake IO labels recurse through subscription triggers and caused StackOverflow in owned probe; stable runner excludes those mutations. Binding semantics are source-confirmed only.','IsAbandon attribute is present in NImage XML but no public parser field was found. Hidden advanced rows also explicitly set CustomVisible False; do not infer universal deletion from IsAbandon.','AccessMode O does not mean disabled UI: UI-only selectors persist state and trigger operations; r/w control parameter transport separately.','No explicit Execute footer is not evidence of nonmodal lifetime. Change-triggered execution and external module commands are separate paths.','Independent mathematical and image recognition implementation remains separate; this report does not claim complete runtime or UI parity.'],'frameworkImplications':[{'status':'derived-design-requirement','requirement':'Preserve declared tabs/categories/control class and exact DisplayType dispatch, with per-plugin template extension and bounded explicit fallback.'},{'status':'derived-design-requirement','requirement':'Represent typed values, initialization/commit distinction, reversible triggers, cross-item conditions and inherited capabilities before rendering.'},{'status':'derived-design-requirement','requirement':'Use typed services for IO relation mutation versus numeric parameter subscription, execute/reset/save/clear/window close; render declared footer only.'},{'status':'derived-design-requirement','requirement':'Lifecycle and execution must come from original host path; do not fix architectural mismatch by per-plugin window styling or guessed modality.'}]}
Path('docs/plugins/configuration-model-evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('written',len(items),'source claims,',len(raw),'logic/command nodes,',len(snap),'runtime records')
