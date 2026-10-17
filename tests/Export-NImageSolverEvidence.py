"""Index original SDK translation/rotation evidence with real transformed pixels."""
import argparse,json,hashlib
from pathlib import Path
root=Path(__file__).resolve().parents[1];parser=argparse.ArgumentParser();parser.add_argument('run',type=Path);args=parser.parse_args();run=args.run.resolve()
assert run.is_relative_to(root)
process=json.loads((run/'process.json').read_text(encoding='utf-8-sig'));assert process['exitCode']==0
records=[json.loads(line) for line in (run/'probe.jsonl').read_text(encoding='utf-8-sig').splitlines() if line.strip()]
cases=[row for row in records if row.get('phase')=='run'];assert len(cases)==3 and all(row['code']==0 for row in cases)
def identity(p):return {'path':str(p.relative_to(root)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
fixtures=sorted((root/'analysis/full-audit/n-image-calibration/solver-fixture').glob('frame-*.bmp'));assert len(fixtures)==14
report={'authority':'official-NImage-SDK-owned-process','scope':'Help-derived real pixels, nine translations plus two/three/five synthetic rotations at ten-degree steps; no devices','physicalDevicesTested':False,'run':str(run.relative_to(root)),'process':process,'provenance':json.loads((run/'provenance.json').read_text(encoding='utf-8-sig')),'inputs':[identity(p) for p in fixtures],'raw':[identity(run/name) for name in ['probe.jsonl','build.log','process.json']],'cases':cases,'limits':['SDK common-corner output omits corners that are absent from any frame. Original rotation fitting uses adjacent per-frame matches and may therefore use more points than the exported all-frame intersection.','These are synthetic image motions, not camera/device tests.']}
(root/'docs/plugins/n-image-solver-evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8');print('Indexed three original successful translation/rotation SDK runs')
