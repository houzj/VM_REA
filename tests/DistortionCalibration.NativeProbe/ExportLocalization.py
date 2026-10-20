"""Export only this tool's actual language keys; source identities retained separately."""
from pathlib import Path
import hashlib,json,xml.etree.ElementTree as E
root=Path(__file__).resolve().parents[2]
resources=root/'src/VisionPlugins.DistortionCalibration/Resources'
keys=set()
for filename in ['DistortionAlgorithmTab.xml','DistortionDisplay.xml']:
 for node in E.parse(resources/filename).getroot().iter():
  if node.tag in ('Tab','Category','GroupInfoItem') and node.get('Name'):keys.add(node.get('Name'))
  if node.tag in ('DisplayName','Description','ToolTipMessage','SelecetedTips') and node.text:keys.add(node.text)
languages={};sources={}
for language,filename in [('Chinese','zh-cn.xaml'),('English','en-us.xaml')]:
 path=Path('D:/Program Files/VisionMaster4.4.0/Applications/Lang')/filename
 strings={node.get('{http://schemas.microsoft.com/winfx/2006/xaml}Key'):''.join(node.itertext()) for node in E.parse(path).getroot() if node.tag.endswith('}String')}
 languages[language]={key:strings[key] for key in sorted(keys) if key in strings}
 sources[language]={'path':str(path),'sha256':hashlib.sha256(path.read_bytes()).hexdigest()}
document=E.Element('Localization')
for language,strings in languages.items():
 node=E.SubElement(document,'Language',Name=language)
 for key,value in strings.items():E.SubElement(node,'String',Key=key).text=value
E.indent(document)
path=resources/'DistortionLocalization.xml';E.ElementTree(document).write(path,encoding='utf-8',xml_declaration=True)
(root/'docs/plugins/distortion-calibration-localization-evidence.json').write_text(json.dumps({'authority':'Original global WPF language resources, tool-specific key extraction','sources':sources,'requested':len(keys),'captured':{key:len(value) for key,value in languages.items()},'sha256':hashlib.sha256(path.read_bytes()).hexdigest(),'missing':{key:sorted(keys-strings.keys()) for key,strings in languages.items()}},ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print({key:len(value) for key,value in languages.items()})
