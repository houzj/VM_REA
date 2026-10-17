"""Retain compact original SDK stitch observations; never infer complete parity."""
import argparse
import hashlib
import json
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('--runtime', type=Path, default=Path('analysis/full-audit/image-source-runtime/run-70d41c7b16174235ab6bd43c8993aa6f/probe.json'))
args = parser.parse_args()
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
records = json.loads(args.runtime.read_text(encoding='utf-8-sig'))
observations = []
for record in records:
    if not record.get('phase', '').startswith('stitch-'):
        continue
    pixels = args.runtime.parent / (record['phase'] + '.pixels')
    assert sha(pixels) == record['pixelSha256']
    observation = {k: record[k] for k in ['phase', 'nativeErrorCode', 'sn', 'width', 'height', 'format', 'length', 'pixelSha256']}
    data = pixels.read_bytes()
    if data:
        width = record['width']
        assert len(data) == width * record['height']
        assert all(len(set(data[i:i+width])) == 1 for i in range(0, len(data), width))
        observation['rowValues'] = list(data[::width])
    observations.append(observation)
assert len(observations) == 11
source = Path('D:/MyWork/REA逆向测试/analysis/full-audit/image-source-native-7.c')
target_sha = sha(Path('analysis/native-input/ImageSourceModule.dll'))
assert target_sha == '91ea3659be97e51edeab0c913779d4555d51dcd0d6288086fbf9916d35b942db'
result = {'schema': 1, 'targetSha256': target_sha, 'runtimeSource': str(args.runtime), 'runtimeSha256': sha(args.runtime), 'probeSha256': sha(Path('tests/NativeEvidence/OriginalImageSourceProbe.cs')), 'nativeSource': str(source), 'nativeSourceSha256': sha(source), 'observations': observations, 'scope': 'Local MONO8, 4x4 uniform rows, clear-trigger disabled; parameter changes, invalid crop, disable/re-enable, zero height, width mismatch/restoration', 'completeParity': False}
Path('docs/plugins/image-source-stitch-evidence.json').write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Exported {len(observations)} original stitch observations')
