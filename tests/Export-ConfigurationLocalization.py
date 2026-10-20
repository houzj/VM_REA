"""Extract required configuration captions from shipped language resources; no UI execution."""
from pathlib import Path
import hashlib,json,xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[1]
installed=Path('D:/Program Files/VisionMaster4.4.0/Applications')
report=json.loads((root/'docs/plugins/configuration-template-catalog-evidence.json').read_text(encoding='utf-8-sig'))
group_path=installed/'Module(sp)/Group.xml'
groups={n.get('Name'):n for n in ET.parse(group_path).getroot().iter('GroupInfoItem')}
keys={'Language','Lang_Help','Lang_Err','Lang_InfoTips','Lang_ClearImageSuc','Lang_ClearImageErr','Lang_ClearCalibPointSuc','Lang_ClearCalibPointErr','Lang_PlzLoopStop','Param not Complete'}
keys.update('Lang_FilePathEptErr Lang_FilePathFmtErr Lang_FilePathLenErr Lang_FilePathErr Lang_FilePathCharErr Lang_FileRdOnly Lang_DrOpenSuc Lang_DrOpenFail 收起 Lang_Unwarp Lang_FirstImage'.split())
keys.update('Open Lang_SetSuc Lang_FileOpenFail Lang_SysErr Lang_SaveSuccess Lang_FileSaveFail Lang_FilePathTooLong Lang_Import Lang_Export'.split())
keys.update('TransNum RotNum PixelPrecision CalibType ImagePointLst WorldPointLst CalibMatrix SnapImage SnapWorld TeachWorld CalibErrStatus TransError RotError TransWorldError RotWorldError PixelPrecisionX PixelPrecisionY CreateCalibTime Lang_Sure Lang_ResetAlgoParamTips Lang_ResetAlgoParamSuccess Lang_UnSupportAlgoParamReset'.split())
def visit(node,chain=()):
    if node.tag=='GroupLinkItem':
        link=node.findtext('LinkName')
        if link not in chain and link in groups:visit(groups[link],chain+(link,))
    if node.tag in ('Tab','Category','GroupInfoItem') and node.get('Name'):keys.add(node.get('Name'))
    if node.tag in ('DisplayName','Description','ToolTipMessage','SelecetedTips') and node.text:keys.add(node.text)
    for child in node:visit(child,chain)
for entry in report['coverage']:visit(ET.parse(entry['xml']['path']).getroot())
visit(ET.parse(installed/'Module(sp)/x64/Collection/ImageSourceModule/ImageSourceModuleAlgorithmTab.xml').getroot())
keys.update('运行全部 运行选中 图像文件 修改配置 Lang_ImageAdd Lang_ImagePathError Lang_PlzChoose'.split())
def identity(path):return {'path':str(path),'bytes':path.stat().st_size,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()}
languages={};sources={};missing={}
for name,file in [('Chinese','zh-cn.xaml'),('English','en-us.xaml')]:
    path=installed/'Lang'/file;sources[name]=identity(path)
    strings={n.get('{http://schemas.microsoft.com/winfx/2006/xaml}Key'):''.join(n.itertext()) for n in ET.parse(path).getroot() if n.tag.endswith('}String')}
    languages[name]={k:strings[k] for k in sorted(keys) if k in strings}
    missing[name]=sorted(keys-strings.keys())
payload={'scope':'Required captions for source-derived shared configuration framework, eight tools plus N-image; not a desktop comparison','sources':sources,'languages':languages}
target=root/'src/VisionPluginContracts/Resources/ConfigurationLocalization.json'
target.write_text(json.dumps(payload,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
(root/'docs/plugins/configuration-localization-evidence.json').write_text(json.dumps({'source':sources,'group':identity(group_path),'requestedKeys':len(keys),'captured':{k:len(v) for k,v in languages.items()},'missingKeys':missing,'outputSha256':hashlib.sha256(target.read_bytes()).hexdigest(),'fallbackPolicy':'Independent explicit key fallback; missing-key behavior has not been runtime-compared.'},ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print({name:len(values) for name,values in languages.items()})
