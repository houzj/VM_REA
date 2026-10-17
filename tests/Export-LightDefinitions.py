"""Extract the shipped light UI declarations into a small independent layout resource."""
import json,pathlib,hashlib,xml.etree.ElementTree as E
root=pathlib.Path(__file__).resolve().parents[1]
source=pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/LightModule/LightModuleAlgorithmTab.xml')
tree=E.parse(source)
items=tree.findall('.//Category/Items/*')
definitions=[]
for item in items:
    definitions.append(dict(name=item.get('Name'),kind=item.tag,label=item.findtext('DisplayName'),value=item.findtext('CurValue'),visible=item.findtext('CustomVisible')!='False',minimum=item.findtext('MinValue'),maximum=item.findtext('MaxValue'),options=[dict(value=e.findtext('Value'),label=e.findtext('DisplayName')) for e in item.findall('EnumEntrys/EnumEntry')]))
rules={}
for trigger in tree.findall('.//Enumeration[@Name="LightDeviceParamGet"]/Triggers/Trigger'):
    rules[trigger.findtext('Value')]=[dict(target=s.findtext('TargetName'),operation=s.findtext('OperationName'),argument=s.findtext('OperationParams')) for s in trigger.findall('Setters/Setter')]
data=dict(schema=1,sourceSha256=hashlib.sha256(source.read_bytes()).hexdigest(),items=definitions,rules=rules)
(root/'src/VisionPlugins.ImageSource/Resources/LightDefinition.json').write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('Extracted',len(items),'controls,',len(rules),'device rules')
