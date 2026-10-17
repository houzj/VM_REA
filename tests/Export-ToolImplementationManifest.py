"""Export explicitly registered source metadata for main's PF-012 integration.

This is a test/report helper, not a runtime discovery mechanism or DLL loader.
"""
from pathlib import Path
import hashlib
import json
import re
import subprocess

root = Path(__file__).resolve().parents[1]
host = root / 'src/VisionEditor/MainWindow.xaml.cs'
host_text = host.read_text(encoding='utf-8-sig')
classes = re.findall(r'plugins\.Register\(new (\w+)\(\),modules\)', host_text)
catalog_path = root / 'src/VisionEditor/tool-catalog.json'
catalog = json.loads(catalog_path.read_text(encoding='utf-8-sig'))
items = {i['ModuleId']: (g, i) for g in catalog['Groups'] for i in g['Items'] if i.get('ModuleId')}

def evidence(path):
    return {'path': path.relative_to(root).as_posix(), 'sha256': hashlib.sha256(path.read_bytes()).hexdigest()}

records = []
for name in classes:
    files = [p for p in (root / 'src').glob('VisionPlugins.*/*.cs') if re.search(r'\bclass\s+' + name + r'\b', p.read_text(encoding='utf-8-sig'))]
    manifest_sources = [(p, re.search(r'ToolManifest\s+Manifest\s*\{\s*get;\s*\}\s*=\s*new\(\s*(Id|"[^"]+")\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"', p.read_text(encoding='utf-8-sig'))) for p in files]
    manifest_sources = [(p, m) for p, m in manifest_sources if m]
    if len(manifest_sources) != 1:
        raise ValueError('Expected one explicit manifest for ' + name)
    path, manifest = manifest_sources[0]
    text = path.read_text(encoding='utf-8-sig')
    identity = manifest[1].strip('"')
    if identity == 'Id':
        identity = re.search(r'const string Id\s*=\s*"([^"]+)"', text)[1]
    group, item = items[identity]
    combined = '\n'.join(p.read_text(encoding='utf-8-sig') for p in files)
    namespace = re.search(r'namespace\s+([\w.]+);', text)[1]
    models = [p for p in files if 'IToolConfigurationProvider' in p.read_text(encoding='utf-8-sig')]
    records.append({
        'moduleId': identity, 'version': manifest[2], 'name': manifest[3],
        'assembly': path.parent.name + '.dll', 'factoryType': namespace + '.' + name,
        'originalCatalogId': item['Id'], 'catalogCategory': group['Id'],
        'catalogIcon': item['Icon'], 'manifestCategory': manifest[4], 'manifestIcon': manifest[5],
        'runtimeFactory': 'IVisionToolPlugin.CreateRuntime',
        'configuration': 'IToolConfigurationProvider.CreateConfiguration -> ToolConfigurationSession' if models else 'IWpfToolEditorFactory.CreateWpfEditor (transitional)',
        'dynamicPorts': 'IToolNodeDescriptorProvider' in combined,
        'sources': [evidence(p) for p in files],
        'closure': 'In-progress source integration metadata; not a native-equivalence or manual-acceptance claim',
    })
assert len(records) == len(classes) == len({r['moduleId'] for r in records})
payload = {
    'schema': 1, 'scope': 'Explicit plugin factories registered by this worktree; no assembly discovery/loading',
    'head': subprocess.check_output(['git', 'rev-parse', 'HEAD'], cwd=root, text=True).strip(),
    'workingTreeChangesIncluded': True, 'host': evidence(host), 'catalog': evidence(catalog_path),
    'plugins': records,
    'boundaries': ['main owns discovery and final integration; no second scanner is implemented here.',
                   'N-image algorithm components are not a registered/closed plugin.',
                   'Source hashes identify in-progress files; no delivery commit exists until manual acceptance.'],
}
target = root / 'docs/plugins/tool-implementation-manifest.json'
target.write_text(json.dumps(payload, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Exported {len(records)} explicitly registered plugins; no DLL was loaded.')
