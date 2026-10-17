"""Export original N-point UI declarations and immutable input identities; no runtime claims."""
import hashlib,json,pathlib,xml.etree.ElementTree as E
root=pathlib.Path(__file__).resolve().parents[1]
original=pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSNPointCalibModu')
source=original/'IMVSNPointCalibModuAlgorithmTab.xml'
def node(x):return dict(type=x.tag,attributes=x.attrib,text=(x.text or '').strip(),children=[node(y) for y in x])
definition=dict(source=str(source),sourceSha256=hashlib.sha256(source.read_bytes()).hexdigest(),tree=node(E.parse(source).getroot()))
target=root/'src/VisionPlugins.Calibration/Resources/NPointDefinition.json';target.parent.mkdir(parents=True,exist_ok=True);target.write_text(json.dumps(definition,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
artifacts=[dict(path=str(p),length=p.stat().st_size,sha256=hashlib.sha256(p.read_bytes()).hexdigest()) for p in sorted(original.iterdir()) if p.is_file()]
data=dict(schema=1,authority='shipped-artifact',scope='静态声明与文件身份；不表示执行或UI等价',artifacts=artifacts,definition=dict(path=str(target.relative_to(root)),sha256=hashlib.sha256(target.read_bytes()).hexdigest()))
(root/'docs/plugins/n-point-calibration-artifacts.json').write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f'Indexed {len(artifacts)} original files')
