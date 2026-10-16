"""Index completed checks and evidence by digest, without packaging raw original binaries."""
import hashlib,json,pathlib,re
root=pathlib.Path(__file__).resolve().parents[1]
base=root/'analysis/full-audit/image-buffer'
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
checks={'checks.log':1623,'regression-wpf.log':252,'regression-image-source.log':767,'regression-image-acquisition.log':327}
others={'VisionFramework.ExecutionChecks':38,'VisionFramework.PersistenceChecks':23,'VisionFramework.IntegrationChecks':10,'VisionEditor.Core.Checks':115,'VisionWorkflow.Checks':13,'VisionPluginDefinitions.Checks':10,'VisionCore.Checks':29,'VisionIO.Checks':14,'VisionModules.Checks':22,'VisionProcedure.Checks':20,'VisionRouting.Checks':20,'VmRouting.Checks':64,'SolutionInspector.Checks':17,'VisionWorkflow.DataChecks':31,'VisionWorkflow.CallChecks':34}
checks.update({f'regression-{name}.log':count for name,count in others.items()})
def record(p):return dict(path=str(p.relative_to(root)).replace('\\','/'),sha256=sha(p),length=p.stat().st_size)
logs=[]
for name,count in checks.items():
    p=base/name
    text=p.read_text(encoding='utf-8-sig')
    assert re.search(rf'\b{count}\b',text) and not re.search(r'Unhandled exception|System\.Exception|^FAIL',text,re.M),name
    logs.append(dict(**record(p),checks=count))
build=(base/'build-solution.log').read_text(encoding='utf-8-sig')
assert '0 个警告' in build and '0 个错误' in build
evidence=json.loads((root/'docs/plugins/image-buffer-evidence.json').read_text(encoding='utf-8'))
assert evidence['source']['sha256']==sha(root/evidence['source']['path'])
source_paths=['src/VisionPlugins.ImageSource/ImageBufferPlugin.cs','src/VisionPlugins.ImageSource/ImageBufferWindow.cs','src/VisionData/RawImage.cs','src/VisionFramework/Contracts.cs','src/VisionFramework/WorkflowSubscriptions.cs','src/VisionFramework/WorkflowRunContext.cs','src/VisionFramework/WorkflowConfiguration.cs','src/VisionPluginContracts/Contracts.cs','src/VisionEditor/MainWindow.xaml.cs','src/VisionEditor/ToolExecutionScope.cs','src/VisionEditor/WorkflowConfigurationWindow.xaml.cs','src/VisionEditor/EditorForm.cs','src/VisionEditor/tool-catalog.json','tests/ImageBuffer.PluginChecks/Program.cs','tests/NativeEvidence/OriginalImageBufferProbe.cs','tests/NativeEvidence/Invoke-OriginalImageBufferProbe.ps1','tests/Export-ImageBufferEvidence.py','tests/VisionEditor.WpfChecks/Program.cs']
data=dict(schema=1,scope='代码复现；无设备测试；不宣称全场景原生等价',parityEstablished=False,programs=len(checks),totalChecks=sum(checks.values()),build=record(base/'build-solution.log'),logs=logs,sources=[record(root/p) for p in source_paths],evidence=[record(root/'docs/plugins/image-buffer-evidence.json'),record(root/'docs/plugins/image-buffer-managed-evidence.json')],native=[record(p) for p in sorted((base/'native').glob('*.c'))],ui=[record(p) for p in sorted(base.glob('independent-*.png'))]+[record(root/evidence['ui']['path'])])
(root/'docs/plugins/image-buffer-verification.json').write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f"Indexed {len(checks)} programs, {sum(checks.values())} checks")
