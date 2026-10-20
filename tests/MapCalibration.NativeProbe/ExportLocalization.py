import hashlib,json,pathlib,xml.etree.ElementTree as ET
root=pathlib.Path(__file__).resolve().parents[2];resources=root/'src/VisionPlugins.MapCalibration/Resources';keys=set()
for name in ['MapCalibrationAlgorithmTab.xml','ConfigurationGroups.xml']:
 for n in ET.parse(resources/name).getroot().iter():
  if n.tag in ('Tab','Category','GroupInfoItem') and n.get('Name'):keys.add(n.get('Name'))
  if n.tag in ('DisplayName','Description','ToolTipMessage','SelecetedTips') and n.text:keys.add(n.text)
result={};evidence={}
for language,name in [('Chinese','zh-cn.xaml'),('English','en-us.xaml')]:
 path=pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Lang')/name;strings={n.get('{http://schemas.microsoft.com/winfx/2006/xaml}Key'):''.join(n.itertext()) for n in ET.parse(path).getroot() if n.tag.endswith('}String')};result[language]={k:strings[k] for k in sorted(keys) if k in strings};evidence[language]=dict(path=str(path),sha256=hashlib.sha256(path.read_bytes()).hexdigest(),captured=len(result[language]),missing=sorted(keys-strings.keys()))
(resources/'MapCalibrationLocalization.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8');(root/'analysis/map-calibration/localization.json').write_text(json.dumps(evidence,ensure_ascii=False,indent=2),encoding='utf-8');print({name:len(value) for name,value in result.items()})
