"""Bind the code-only regression summary to source, binaries, logs and evidence."""
import hashlib, json, re
from pathlib import Path
root=Path(__file__).resolve().parents[1]
audit=root/'analysis/full-audit/image-acquisition'
def artifact(path):
    path=Path(path)
    return {'path':str(path.relative_to(root)) if path.is_relative_to(root) else str(path),'sha256':hashlib.sha256(path.read_bytes()).hexdigest()}
programs=[('ImageAcquisition.PluginChecks',327,'checks.log'),('ImageSource.PluginChecks',767,'regression-image-source.log'),('VisionEditor.WpfChecks',245,'regression-wpf.log')]
other=[('VisionFramework.ExecutionChecks',38),('VisionFramework.PersistenceChecks',23),('VisionFramework.IntegrationChecks',10),('VisionEditor.Core.Checks',115),('VisionWorkflow.Checks',13),('VisionPluginDefinitions.Checks',10),('VisionCore.Checks',29),('VisionIO.Checks',14),('VisionModules.Checks',22),('VisionProcedure.Checks',20),('VisionRouting.Checks',20),('VmRouting.Checks',64),('SolutionInspector.Checks',17),('VisionWorkflow.DataChecks',31),('VisionWorkflow.CallChecks',34)]
programs += [(name,count,'regression-'+name+'.log') for name,count in other]
checks=[]
for name,count,filename in programs:
    log=audit/filename;text=log.read_text(encoding='utf-8-sig')
    if not re.search(r'\b'+str(count)+r'\b',text) or ('System.Exception:' in text):
        raise RuntimeError('Unconfirmed check result: '+name)
    checks.append({'program':name,'checks':count,'log':artifact(log),'binary':artifact(root/'analysis/build/image-acquisition/bin'/name/'debug'/(name+'.dll')),'lastLine':text.strip().splitlines()[-1]})
build=audit/'build-solution.log';text=build.read_text(encoding='utf-8-sig')
if not re.search(r'\b0 个警告',text) or not re.search(r'\b0 个错误',text):raise RuntimeError('Build did not complete cleanly')
sources=set((root/'src/VisionPlugins.ImageSource').glob('Acquisition*'))|set((root/'src/VisionPlugins.ImageSource').glob('ImageAcquisition*'))|set((root/'tests/ImageAcquisition.PluginChecks').glob('*.cs'))|set((root/'tests/ImageAcquisition.PluginChecks').glob('*.csproj'))
sources.update(root/p for p in ['src/VisionFramework/ImageAcquisition.cs','src/VisionPluginContracts/Contracts.cs','src/VisionPlugins.ImageSource/MvsCameraAcquisition.cs','src/VisionEditor/MainWindow.xaml.cs','src/VisionEditor/EditorForm.cs','src/VisionEditor/tool-catalog.json','tests/VisionEditor.WpfChecks/Program.cs','tests/NativeEvidence/OriginalImageAcquisitionProbe.cs','tests/NativeEvidence/Invoke-OriginalImageAcquisitionProbe.ps1','tests/NativeEvidence/VmAcquisitionAudit.java','tests/Export-ImageAcquisitionEvidence.py','tests/Export-ImageAcquisitionVerification.py','VisionReplica.slnx'])
resources=root/'src/VisionPlugins.ImageSource/Resources'
sources.update(resources/name for name in ['AcquisitionButtons.xaml','add_normal.png','add_click.png','ic_add_disable.png','btn_help.png','ic_modal_视觉控制器_normal.png'])
evidence=[root/'docs/plugins/image-acquisition-evidence.json',root/'docs/plugins/image-acquisition-managed-evidence.json']
raw=json.loads(evidence[0].read_text(encoding='utf-8'))
for group in ('artifacts','sdkUi','desktopUi'):
    for item in raw[group]:
        if hashlib.sha256(Path(item['path']).read_bytes()).hexdigest()!=item['sha256']:raise RuntimeError('Changed evidence: '+item['path'])
for item in [raw['runtime'],raw['producer']]+raw['nativeDecompilation']['functions']:
    if hashlib.sha256(Path(item['path']).read_bytes()).hexdigest()!=item['sha256']:raise RuntimeError('Changed producer/source: '+item['path'])
result={'schema':1,'plugin':'vision.image-acquisition','acceptanceScope':'Code reconstruction. Physical device testing explicitly excluded by user.','completeNativeParity':False,'programCount':len(checks),'totalChecks':sum(c['checks'] for c in checks),'checks':checks,'build':artifact(build),'sources':[artifact(p) for p in sorted(sources) if p.is_file()],'evidence':[artifact(p) for p in evidence],'renders':[artifact(p) for p in sorted(audit.glob('independent-mode-*.png'))],'boundaries':['Original SOL bytes are not the independent VMX format.','Original desktop local-mode popup has not been pixel-compared; original XAML and SDK-hosted modes were inspected.','Global-light module transport is injected by the global resource provider; no controller serial/network protocol is fabricated.','Native undefined decoder damage-input behavior is not replicated.','Passing these tests alone does not establish complete native equivalence.']}
(root/'docs/plugins/image-acquisition-verification.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f"Verified {result['programCount']} programs, {result['totalChecks']} checks, {len(result['sources'])} source/assets; no device tests")
