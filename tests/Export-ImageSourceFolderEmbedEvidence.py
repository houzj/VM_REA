"""Folder subscription takes precedence over restored embedded image list."""
import argparse,hashlib,json
from pathlib import Path
p=argparse.ArgumentParser();p.add_argument('--runtime',type=Path,required=True);a=p.parse_args();records=json.loads(a.runtime.read_text(encoding='utf-8-sig'))
rows=[r for r in records if r.get('phase') in ('embed-before-save','embed-after-source-removed')];assert len(rows)==2
assert rows[0]['nativeErrorCode']==0 and rows[0]['length']==16
assert rows[1]['nativeErrorCode']==0xe0000203 and rows[1]['length']==0
result=dict(schema=1,runtimeSource=str(a.runtime),runtimeSha256=hashlib.sha256(a.runtime.read_bytes()).hexdigest(),observations=rows,behavior='Subscribed folder is rescanned after reload; an empty folder yields NoImage even when SolSaveImageData was enabled. No fallback to embedded list.',completeParity=False)
Path('docs/plugins/image-source-folder-embed-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8')
