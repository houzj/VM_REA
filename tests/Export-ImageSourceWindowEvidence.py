"""Record vendor window-class rendering without claiming desktop-process parity."""
import argparse,hashlib,json,struct
from pathlib import Path
p=argparse.ArgumentParser();p.add_argument('--runtime',type=Path,required=True);a=p.parse_args();sha=lambda path:hashlib.sha256(path.read_bytes()).hexdigest()
records=json.loads(a.runtime.read_text(encoding='utf-8-sig'))
assert not any('error' in r for r in records)
windows=[r for r in records if r.get('phase')=='sdk-hosted-ui'];assert len(windows)==5 and all(r['accepted'] for r in windows)
assert all(r['windowType']=='VMControls.WPF.ParamsTabControlBase' for r in windows)
images=[]
for path in sorted(a.runtime.parent.glob('*.png')):
    width,height=struct.unpack('>II',path.read_bytes()[16:24]);images.append(dict(path=str(path),width=width,height=height,sha256=sha(path)))
source=Path('D:/MyWork/REA逆向测试/analysis/decompiled/VMControls.WPF')
result=dict(schema=1,runtimeSource=str(a.runtime),runtimeSha256=sha(a.runtime),producerSha256=sha(a.runtime.parent/'OriginalImageSourceProbe.exe'),windowClass='VMControls.WPF.ParamsTabControlBase',variants=[dict(variant=r['variant'],mode=r['mode'],width=r['windowWidth'],height=r['windowHeight'],imageSha256=r['imageSha256']) for r in windows],subscription=[r for r in records if r.get('phase')=='original-subscription-window'],images=images,sourceAnalysis=[dict(path=str(path),sha256=sha(path)) for path in (source/'VMControls.WPF/ParamsTabControlBase.xaml.cs',source/'VMControls.WPF/ParamsConfigControlBase.xaml.cs',source/'VMControls.WPF.DataForUI/VmModuleDecorator.cs')],completeParity=False,limitations=['Original vendor window and subscription window classes executed inside the isolated SDK probe, not the installed VisionMaster desktop process.','SDK mode remains enabled; inline image list and sizing differ from desktop context. These dimensions are not a desktop UI specification.','Subscription window was rendered but its source tree remained empty in this SDK context.','Desktop capture failed with FrameArrived timeout, then Computer Use was stopped with physical Escape. No desktop interactions are required to reproduce the headless report.'])
Path('docs/plugins/image-source-window-host-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8');print('Exported bounded vendor-window host evidence; desktop parity remains unverified')
