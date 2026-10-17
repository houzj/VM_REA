"""Export a small, traceable original SDK ledger; raw pixels stay in ignored analysis."""
import hashlib,json,pathlib,sys
root=pathlib.Path(__file__).resolve().parents[1]
run=pathlib.Path(sys.argv[1]).resolve()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
rows=json.loads((run/'probe.json').read_text(encoding='utf-8-sig'))
for row in rows:
    for slot in row.get('images',[]):
        p=run/f"{row['phase']}-{slot['slot']}.pixels"
        assert p.stat().st_size==slot['length'] and sha(p)==slot['sha256']
original=pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Collection/ImageBufferModule')
data=dict(schema=1,authority='controlled-original-SDK-replay',run=str(run.relative_to(root)),
          scope='代码复现；排除设备测试；SDK 内嵌界面不等同原厂桌面窗口',parityEstablished=False,
          source=dict(path='tests/NativeEvidence/OriginalImageBufferProbe.cs',sha256=sha(root/'tests/NativeEvidence/OriginalImageBufferProbe.cs')),
          artifacts=[dict(path=str(p),sha256=sha(p),length=p.stat().st_size) for p in sorted(original.iterdir()) if p.is_file()],
          provenance=json.loads((run/'provenance.json').read_text(encoding='utf-8-sig')),rows=rows,
          ui=dict(path=str((run/'original-sdk-ui.png').relative_to(root)),sha256=sha(run/'original-sdk-ui.png')))
(root/'docs/plugins/image-buffer-evidence.json').write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print('Exported',sum('images' in r for r in rows),'native execution rows')
