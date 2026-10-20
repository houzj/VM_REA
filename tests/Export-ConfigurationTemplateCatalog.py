"""Static template-selection contract and expanded XML coverage; no vendor execution."""
from pathlib import Path
import collections,hashlib,json,re,xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[1];D=Path('D:/MyWork/REA逆向测试/analysis/decompiled');I=Path('D:/Program Files/VisionMaster4.4.0/Applications')
roots=[D/'Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab',D/'ImageSourceModuleControl',D/'ExtensionParamUserControl.CapabilityAudit',ROOT/'analysis/full-audit/image-acquisition/control',ROOT/'analysis/full-audit/light/managed-control',ROOT/'analysis/full-audit/n-point-calibration/managed/control',ROOT/'analysis/full-audit/calibration-dependencies/decompiled/TranslationCalibModuControl',ROOT/'analysis/full-audit/calibration-dependencies/decompiled/ReadCalibFileModuControl',ROOT/'analysis/branch-module/source-20261020/control-decompiled/Apps.XmlParser.ParamTab',D.parent/'full-audit/renderer-gap-projects/IMVSContourMatchModuControl/Apps.XmlParser.ParamTab',ROOT/'analysis/next-batch/condition-managed/IfModuleControl/Apps.XmlParser.ParamTab',ROOT/'analysis/branch-string/source-20261020/control-decompiled/Apps.XmlParser.ParamTab',D.parent/'full-audit/renderer-gap-projects/IMVSFixtureModuControl/Apps.XmlParser.ParamTab']
dictionaries=[('Apps.XmlUI',D/'Apps.XmlUI/styles/paramusercontroldatatemplate.xaml'),('ImageSourceModuleControl',D/'ImageSourceModuleControl/datatemplate/paramusercontroldatatemplate.xaml'),('ExtensionParamUserControl',D/'ExtensionParamUserControl.CapabilityAudit/datatemplate/paramusercontroldatatemplate.xaml'),('ImageAcquisitionModuleControl',ROOT/'analysis/full-audit/image-acquisition/control/datatemplate/paramusercontroldatatemplate.xaml'),('LightModuleControl',ROOT/'analysis/full-audit/light/managed-control/datatemplate/paramusercontroldatatemplate.xaml'),('IMVSNPointCalibModuControl',ROOT/'analysis/full-audit/n-point-calibration/managed/control/datatemplate/paramusercontroldatatemplate.xaml'),('TranslationCalibModuControl',ROOT/'analysis/full-audit/calibration-dependencies/decompiled/TranslationCalibModuControl/datatemplate/paramusercontroldatatemplate.xaml'),('ReadCalibFileModuControl',ROOT/'analysis/full-audit/calibration-dependencies/decompiled/ReadCalibFileModuControl/datatemplate/paramusercontroldatatemplate.xaml'),('BranchModule_STDControl',ROOT/'analysis/branch-module/source-20261020/control-decompiled/datatemplate/paramusercontroldatatemplate.xaml'),('IMVSContourMatchModuControl',D.parent/'full-audit/renderer-gap-projects/IMVSContourMatchModuControl/datatemplate/paramusercontroldatatemplate.xaml'),('IMVSHPFeatureMatchModuControl',D.parent/'full-audit/renderer-gap-projects/IMVSHPFeatureMatchModuControl/datatemplate/paramusercontroldatatemplate.xaml'),('IfModuleControl',ROOT/'analysis/next-batch/condition-managed/IfModuleControl/datatemplate/paramusercontroldatatemplate.xaml'),('BranchStringCpmLControl',ROOT/'analysis/branch-string/source-20261020/control-decompiled/datatemplate/paramusercontroldatatemplate.xaml'),('IMVSFixtureModuControl',D.parent/'full-audit/renderer-gap-projects/IMVSFixtureModuControl/datatemplate/paramusercontroldatatemplate.xaml')]
X='{http://schemas.microsoft.com/winfx/2006/xaml}Key';W='{http://schemas.microsoft.com/winfx/2006/xaml/presentation}'
def info(p):return {'path':str(p.resolve()),'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'bytes':p.stat().st_size}
def balanced(text,start):
 depth=0
 for i in range(start,len(text)):
  if text[i]=='{':depth+=1
  elif text[i]=='}':
   depth-=1
   if depth==0:return text[start:i+1]
 return text[start:]
classes={};classSources=collections.defaultdict(list)
for root in roots:
 for p in sorted(root.rglob('*.cs')):
  text=p.read_text(encoding='utf-8-sig');m=re.search(r'public\s+(?:sealed\s+|abstract\s+|partial\s+)*class\s+(\w+)(?:\s*:\s*([\w.<>]+))?',text)
  if not m:continue
  name=m[1];base=(m[2] or '').split('.')[-1];ns=re.search(r'namespace\s+([\w.]+)',text);qualified=(ns[1]+'.' if ns else '')+name
  member=re.search(r'public\s+(?:override\s+|virtual\s+)?string\s+DisplayType\s*',text)
  kind='inherit';key=None;line=None;body=None
  if member:
   line=text[:member.start()].count('\n')+1;tail=text[member.end():]
   if tail.startswith('=>'):body=tail[2:tail.index(';')].strip();returns=[body]
   else:body=balanced(text,text.index('{',member.end()));returns=re.findall(r'\breturn\s+([^;]+);',body)
   if returns and all(re.fullmatch(r'"[^"\\]*"',e.strip()) for e in returns) and len(set(returns))==1:kind='literal';key=returns[0].strip()[1:-1]
   elif len(returns)==1 and returns[0].strip()=='GetType().Name':kind='type-name'
   elif len(returns)==1 and re.fullmatch(r'(?:\(\(object\)this\)|this)\.GetType\(\)\.BaseType\.Name',returns[0].strip()):kind='base-type-name';key=base
   else:kind='runtime'
  record={'type':name,'qualifiedType':qualified,'base':base,'ownRule':kind,'constant':key,'source':info(p),'line':line,'member':body}
  classSources[name].append(record)
  if name not in classes:classes[name]=record

def rule(name,concrete=None,stack=()):
 if name not in classes or name in stack:return ('unknown',None,[])
 r=classes[name];concrete=concrete or name
 if r['ownRule']=='type-name':return ('type-name',concrete,[r])
 if r['ownRule']=='literal':return ('literal',r['constant'],[r])
 if r['ownRule']=='base-type-name':return ('base-type-name',r['base'],[r])
 if r['ownRule']=='runtime':return ('runtime',None,[r])
 kind,key,chain=rule(r['base'],concrete,stack+(name,));return kind,key,[r]+chain
resources=collections.defaultdict(list);nonTemplates=set();dictionaryInfo=[];unresolvedDictionaries=[];dictionaryEdges=[]
assemblyRoots={assembly:(path.parent.parent if assembly!='Apps.XmlUI' else D/'Apps.XmlUI') for assembly,path in dictionaries}
assemblyRoots.update({'Apps.Style':D/'Apps.Style','Apps.PcUI':D/'Apps.PcUI.ImageSourceAudit','VM.PcUI':D/'VM.PcUI'})
visited=set()
def dictionary(assembly,path,uri,stack=()):
 identity=(assembly,str(path.resolve()).lower())
 if identity in visited:return
 visited.add(identity)
 if not path.exists():unresolvedDictionaries.append({'assembly':assembly,'uri':uri,'path':str(path)});return
 tree=ET.parse(path).getroot();dictionaryInfo.append({'assembly':assembly,'uri':uri,**info(path)})
 for merged in tree.findall('./'+W+'ResourceDictionary.MergedDictionaries/'+W+'ResourceDictionary'):
  target=merged.get('Source')
  if not target:continue
  match=re.match(r'^/([^;]+);component/(.+)$',target)
  if match:
   targetAssembly,relative=match.groups();base=assemblyRoots.get(targetAssembly)
   if base is None:unresolvedDictionaries.append({'from':uri,'uri':target});continue
   targetPath=base/relative;targetUri=target
  else:
   targetAssembly=assembly;targetPath=(path.parent/target).resolve();base=assemblyRoots[assembly]
   targetUri='/'+assembly+';component/'+targetPath.relative_to(base.resolve()).as_posix()
  dictionaryEdges.append({"from":uri,"source":target,"resolvedUri":targetUri,"assembly":targetAssembly})
  dictionary(targetAssembly,targetPath,targetUri,stack+(identity,))
 for node in tree.iter():
  if X not in node.attrib:continue
  key=node.get(X)
  if key.startswith('{'):continue
  if node.tag==W+'DataTemplate':resources[key].append({'assembly':assembly,'uri':uri,'source':info(path),'templateSha256':hashlib.sha256(ET.tostring(node)).hexdigest(),'dependencies':sorted(set(re.findall(r'\{StaticResource\s+([^}]+)\}',ET.tostring(node,encoding='unicode'))))})
  else:nonTemplates.add(key)
for assembly,path in dictionaries:
 dictionary(assembly,path,f'/{assembly};component/'+('Styles' if assembly=='Apps.XmlUI' else 'DataTemplate')+'/ParamUserControlDataTemplate.xaml')
# Local dictionary entries take precedence over merged dictionaries. Retain multiple external providers explicitly.
for key,candidates in resources.items():
 local=[r for r in candidates if r['assembly']=='Apps.XmlUI'];resources[key]=local or candidates
nonTemplates-=resources.keys()

def resolution(node,display=None):
 kind,key,chain=rule(node)
 if display is not None:key=display;kind='evaluated-display-type'
 if key is not None and key in resources:return {'nodeType':node,'effectiveDisplayType':key,'templateKey':key,'support':'Recognized','rule':kind,'nativeFallbackKey':None,'sources':resources[key],'independentRendererVerified':False}
 return {'nodeType':node,'effectiveDisplayType':key,'templateKey':None,'support':'RequiresRuntimeDisplayType' if kind=='runtime' and display is None else 'Unsupported','rule':kind,'nativeFallbackKey':None if key in nonTemplates or (kind=='runtime' and display is None) else 'Default','sources':[],'independentRendererVerified':False}
moduleDefs=[('ImageSourceModule','Collection'),('ImageAcquisitionModule','Collection'),('ImageBufferModule','Collection'),('LightModule','Collection'),('IMVSNPointCalibModu','Calibration'),('TranslationCalibModu','Calibration'),('RotateCalibModu','Calibration'),('ReadCalibFileModu','Calibration'),('IMVSNImageCalibModu','Calibration'),('BranchModule_STD','Logic'),('IfModule','Logic'),('BranchStringCpmL','Logic'),('IMVSFixtureModu','Location')]
groupPath=I/'Module(sp)/Group.xml';groups={n.get('Name'):n for n in ET.parse(groupPath).getroot().iter('GroupInfoItem')}
coverage=[];actual=set()
for mod,cat in moduleDefs:
 p=I/'Module(sp)/x64'/cat/mod/(mod+'AlgorithmTab.xml');tree=ET.parse(p).getroot();controls=[];links=[]
 def scan(root,source,origin,stack=()):
  for element in root.iter():
   if element.tag in ('Items','BottomExcuteButtons','BottomExcuteButtonsLeft','ExternParamsList'):
    for node in element:
     if node.tag in ('Category','GroupLinkItem'):continue
     actual.add(node.tag);res=resolution(node.tag);controls.append({'nodeType':node.tag,'name':node.get('Name'),'namespace':node.get('NameSpace'),'source':source,'origin':origin,'xmlDisplayTypeAttribute':node.get('DisplayType'),'xmlDisplayTypeElement':node.findtext('DisplayType'),'resolution':{k:v for k,v in res.items() if k!='sources'}})
   if element.tag=='GroupLinkItem':
    name=element.findtext('LinkName');links.append({'name':name,'source':source,'origin':origin})
    if name in stack:raise RuntimeError('Group link cycle '+str(stack+(name,)))
    if name not in groups:links[-1]['unsupportedMissingGroup']=True;continue
    scan(groups[name],str(groupPath),f'GroupInfoItem:{name}',stack+(name,))
 scan(tree,str(p),'module XML');coverage.append({'module':mod,'scope':'supplemental-NImage-priority' if mod=='IMVSNImageCalibModu' else 'supplemental-IfModule' if mod=='IfModule' else 'supplemental-BranchString' if mod=='BranchStringCpmL' else 'supplemental-Fixture' if mod=='IMVSFixtureModu' else 'primary-eight','xml':info(p),'controls':controls,'groupLinks':links,'counts':dict(collections.Counter(c['nodeType'] for c in controls))})
usedRules={}
for name in sorted(classes):
 kind,key,chain=rule(name)
 if kind=='unknown':continue
 usedRules[name]={'kind':kind,'defaultTemplateKey':key,'sourceChain':[{'type':r['type'],'qualifiedType':r['qualifiedType'],'ownRule':r['ownRule'],'line':r['line'],'source':r['source']} for r in chain]}
selectionSources=[D/'Apps.XmlUI/Apps.XmlUI/UserTempConverter.cs',D/'Apps.XmlUI/Apps.XmlUI/ParamUserControl.xaml.cs',D/'Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/NameValueItem.cs']
report={'authority':'shipped decompiled source/XAML and expanded module XML; no vendor types instantiated','scope':{'primaryToolCount':8,'supplementalToolCount':4,'distinctXmlNodeTypes':len(actual),'expandedControlInstances':sum(len(m['controls']) for m in coverage)},'selectionSources':[info(p) for p in selectionSources],'dictionarySources':dictionaryInfo,'mergedDictionaryEdges':dictionaryEdges,'unresolvedMergedDictionaries':unresolvedDictionaries,'selectionRules':['Installed original binding property is DisplayType, not DisplayTypeA. NameValueItem getter uses runtime GetType().Name; setter is empty.','Derived source overrides may change template key (SaveFileDialog->Command, OpenFolderDialog->OpenFileDialog, EnumerationLightForLight->EnumerationLight).','Explicit API displayType means an already evaluated native model DisplayType or ParamUserControl.ExternDisplayType; it is not a raw XML property override.','ParamUserControl ExternDisplayType uses non-null string even if empty. Otherwise bind model DisplayType.','Original UserTempConverter does exact case-sensitive ResourceDictionary lookup; missing keys use native Default DataTemplate; existing non-DataTemplate keys cast to null.','Independent catalog reports unknown entries Unsupported with no selected TemplateKey. NativeFallbackKey describes evidence only, never an instruction to create a generic TextBox.','Original local dictionary entries win over merged dictionaries. External merged providers/load order must remain explicit.','Recognized means template-selection metadata is known; it does not prove an independent renderer exists or that source/runtime behavior is fully reproduced.'],'actualNodeTypes':{t:{'resolution':resolution(t),'sourceRule':usedRules.get(t),'sourceCandidates':classSources.get(t,[])} for t in sorted(actual)},'typeRules':usedRules,'templateKeys':dict(sorted(resources.items())),'knownNonTemplateKeys':sorted(nonTemplates),'coverage':coverage,'limits':['Coverage includes declared XML Items and recursively linked groups, not every runtime-created custom control child.','Custom-template dependencies are recorded per DataTemplate; external object creation/interaction is not executed.','Literal controls and data kinds are not used to infer an editor. No window/view implementation is added.']}
(ROOT/'docs/plugins/configuration-template-catalog-evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
# Emit complete immutable metadata source atomically. Shared compilation never sees a partial declaration.
q=lambda s:json.dumps(s,ensure_ascii=False)
source='''// Generated by tests/Export-ConfigurationTemplateCatalog.py from original source/XAML evidence.
using System.Collections.ObjectModel;
namespace VisionPluginContracts;
public enum ConfigurationTemplateSupport { Recognized, RequiresRuntimeDisplayType, Unsupported }
public sealed record ConfigurationTemplateResolution(string NodeType,string? EffectiveDisplayType,string? TemplateKey,ConfigurationTemplateSupport Support,string Rule,string? NativeFallbackKey,IReadOnlyList<string> ResourceUris) {
 public bool IndependentRendererVerified => false;
 public bool IsRecognized => Support==ConfigurationTemplateSupport.Recognized;
}
/// <summary>Pure source-confirmed template selection metadata, not a control factory.
/// The optional displayType is an evaluated model/ExternDisplayType, never a guessed DataKind or raw XML override.
/// Unknowns are Unsupported; NativeFallbackKey records original evidence and must not create a generic editor.</summary>
public static class ConfigurationTemplateCatalog {
 public const string NativeBindingProperty="DisplayType";
 public const string OriginalVersion="VisionMaster 4.4.0";
 static readonly IReadOnlyDictionary<string,(string? Key,string Rule)> types = new ReadOnlyDictionary<string,(string?,string)>(new Dictionary<string,(string?,string)>(StringComparer.Ordinal) {
'''
for name,r in sorted(usedRules.items()):
 key='null' if r['defaultTemplateKey'] is None else q(r['defaultTemplateKey']);source+=f'  [{q(name)}]=({key},{q(r["kind"])}),\n'
 # Exact qualified native type names are also supported, without unqualified-name guesses for unknown namespaces.
 for qualified in sorted({candidate['qualifiedType'] for candidate in classSources[name]}):
  if qualified!=name:source+=f'  [{q(qualified)}]=({key},{q(r["kind"])}),\n'
source+=' });\n static readonly IReadOnlyDictionary<string,IReadOnlyList<string>> templates = new ReadOnlyDictionary<string,IReadOnlyList<string>>(new Dictionary<string,IReadOnlyList<string>>(StringComparer.Ordinal) {\n'
for key,entries in sorted(resources.items()):source+=f'  [{q(key)}]=Array.AsReadOnly(new string[]{{'+','.join(q(e['uri']) for e in entries)+'}),\n'
source+=' });\n static readonly HashSet<string> nonTemplates = new(StringComparer.Ordinal) { '+','.join(q(k) for k in sorted(nonTemplates))+' };\n'
source+=''' public static IReadOnlyDictionary<string,IReadOnlyList<string>> Templates => templates;
 public static ConfigurationTemplateResolution Resolve(string nodeType,string? displayType=null) {
  ArgumentNullException.ThrowIfNull(nodeType);
  string? key=null;string rule="unknown-node-type";
  if(displayType is not null){key=displayType;rule="evaluated-display-type";}
  else if(types.TryGetValue(nodeType,out var type)){key=type.Key;rule=type.Rule;}
  if(key is not null&&templates.TryGetValue(key,out var resources))return new(nodeType,key,key,ConfigurationTemplateSupport.Recognized,rule,null,resources);
  return new(nodeType,key,null,rule=="runtime"&&displayType is null?ConfigurationTemplateSupport.RequiresRuntimeDisplayType:ConfigurationTemplateSupport.Unsupported,rule,rule=="runtime"&&displayType is null||key is not null&&nonTemplates.Contains(key)?null:"Default",Array.Empty<string>());
 }
}
'''
temp=ROOT/'analysis/full-audit/configuration-templates/ConfigurationTemplateCatalog.cs';temp.write_text(source,encoding='utf-8');(ROOT/'src/VisionPluginContracts/ConfigurationTemplateCatalog.cs').write_text(source,encoding='utf-8')
missing=[t for t in actual if resolution(t)['support']!='Recognized'];print('Scope',report['scope'],'; templates',len(resources),'; class rules',len(usedRules),'; unresolved actual',missing)
