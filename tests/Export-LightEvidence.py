"""Keep static identities, controlled native replay and owned-process UI evidence distinct."""
import hashlib,json,pathlib,sys
root=pathlib.Path(__file__).resolve().parents[1]
run=pathlib.Path(sys.argv[1]).resolve()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
rows=json.loads((run/'probe.json').read_text(encoding='utf-8-sig'))
assert not any(r['phase']=='fatal' or r['phase'].startswith('ui-') for r in rows)
assert sum(r['phase']=='sdk-ui' for r in rows)==24
assert any(r['phase']=='transport-interception' for r in rows)
for r in rows:
    for w in r.get('writes',[]):
        assert len(bytes.fromhex(w['hex']))==w['length']==748
        assert w['module']==2147418112 and w['command']=='LightDeviceParamSetTrigger'
original=pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/LightModule')
data=dict(schema=1,authority='original-SDK-replay-with-explicit-owned-process-transport-interception',scope='无设备测试；成功/失败传输是测试替身，原厂算法实际执行；UI为原厂控件的独立SDK宿主',parityEstablished=False,run=str(run.relative_to(root)),source=dict(path='tests/NativeEvidence/OriginalLightProbe.cs',sha256=sha(root/'tests/NativeEvidence/OriginalLightProbe.cs')),artifacts=[dict(path=str(p),sha256=sha(p),length=p.stat().st_size) for p in sorted(original.iterdir()) if p.is_file()],provenance=json.loads((run/'provenance.json').read_text(encoding='utf-8-sig')),rows=rows,ui=[dict(path=str(p.relative_to(root)),sha256=sha(p)) for p in sorted(run.glob('original-sdk-*.png'))])
(root/'docs/plugins/light-evidence.json').write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('Exported',sum('values' in r for r in rows),'native runs,',len(data['ui']),'UI captures')
