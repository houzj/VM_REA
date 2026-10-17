"""Index the completed code checks, original evidence and adapted build assets."""
import hashlib,json,pathlib,re
root=pathlib.Path(__file__).resolve().parents[1]
base=root/'analysis/full-audit/light'
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
def record(p):return dict(path=str(p.relative_to(root)).replace('\\','/'),sha256=sha(p),length=p.stat().st_size)
checks={'checks.log':11836,'regression-wpf.log':257,'regression-ImageSource.PluginChecks.log':767,'regression-ImageAcquisition.PluginChecks.log':327,'regression-ImageBuffer.PluginChecks.log':1623}
others={'VisionFramework.ExecutionChecks':38,'VisionFramework.PersistenceChecks':23,'VisionFramework.IntegrationChecks':10,'VisionEditor.Core.Checks':115,'VisionWorkflow.Checks':13,'VisionPluginDefinitions.Checks':10,'VisionCore.Checks':29,'VisionIO.Checks':14,'VisionModules.Checks':22,'VisionProcedure.Checks':20,'VisionRouting.Checks':20,'VmRouting.Checks':64,'SolutionInspector.Checks':17,'VisionWorkflow.DataChecks':31,'VisionWorkflow.CallChecks':34}
checks.update({f'regression-{name}.log':count for name,count in others.items()})
logs=[]
for name,count in checks.items():
    p=base/name;text=p.read_text(encoding='utf-8-sig')
    assert re.search(rf'\b{count}\b',text) and not re.search(r'Unhandled exception|System\.Exception|^FAIL',text,re.M),name
    logs.append(dict(**record(p),checks=count))
build=(base/'build-solution.log').read_text(encoding='utf-8-sig')
assert '0 个警告' in build and '0 个错误' in build
evidence=json.loads((root/'docs/plugins/light-evidence.json').read_text(encoding='utf-8'))
assert evidence['source']['sha256']==sha(root/evidence['source']['path'])
assets=[]
for name in ['check-box_on.png','check-box_off.png']:
    original=pathlib.Path('D:/MyWork/REA逆向测试/analysis/decompiled/Apps.Style/resources')/name
    local=root/'src/VisionPlugins.ImageSource/Resources'/name
    assert sha(original)==sha(local)
    assets.append(dict(original=str(original),**record(local)))
paths=['src/VisionPlugins.ImageSource/LightPlugin.cs','src/VisionPlugins.ImageSource/LightWindow.cs','src/VisionPlugins.ImageSource/LightUiRules.cs','src/VisionPlugins.ImageSource/Resources/LightDefinition.json','src/VisionPlugins.ImageSource/Resources/LightControls.xaml','src/VisionPlugins.ImageSource/VisionPlugins.ImageSource.csproj','src/VisionFramework/LightControl.cs','src/VisionFramework/WorkflowSubscriptions.cs','src/VisionFramework/WorkflowRunContext.cs','src/VisionFramework/WorkflowConfiguration.cs','src/VisionPluginContracts/Contracts.cs','src/VisionEditor/MainWindow.xaml.cs','src/VisionEditor/ToolExecutionScope.cs','src/VisionEditor/WorkflowConfigurationWindow.xaml.cs','src/VisionEditor/EditorForm.cs','src/VisionEditor/tool-catalog.json','tests/Light.PluginChecks/Program.cs','tests/NativeEvidence/OriginalLightProbe.cs','tests/NativeEvidence/Invoke-OriginalLightProbe.ps1','tests/Export-LightDefinitions.py','tests/Export-LightEvidence.py','tests/Export-LightVerification.py','tests/VisionEditor.WpfChecks/Program.cs']
definition=json.loads((root/paths[3]).read_text(encoding='utf-8'))
assert definition['sourceSha256']==next(a['sha256'] for a in evidence['artifacts'] if a['path'].endswith('LightModuleAlgorithmTab.xml'))
for ui in evidence['ui']:assert sha(root/ui['path'])==ui['sha256']
data=dict(schema=1,scope='代码范围；设备测试排除；原厂下发结果通过自有进程传输替身控制，不代表物理设备实测',parityEstablished=False,programs=len(checks),totalChecks=sum(checks.values()),nativeRuns=sum('values' in r for r in evidence['rows']),nativeUiCaptures=len(evidence['ui']),build=record(base/'build-solution.log'),logs=logs,sources=[record(root/p) for p in paths],assets=assets,evidence=[record(root/'docs/plugins/light-evidence.json'),record(root/'docs/plugins/light-managed-evidence.json')],native=[record(p) for p in sorted((base/'native').glob('*.c'))]+[record(p) for p in sorted((base/'native').glob('*.asm.txt'))],ui=[record(p) for p in sorted(base.glob('independent-*.png'))]+[record(root/p['path']) for p in evidence['ui']])
(root/'docs/plugins/light-verification.json').write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f"Indexed {len(checks)} programs, {sum(checks.values())} checks, {data['nativeRuns']} native runs")
