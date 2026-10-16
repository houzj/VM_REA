"""Export bounded original SDK codec corpus observations, not general parity."""
import argparse
import hashlib
import json
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('--runtime', type=Path, default=Path('analysis/full-audit/image-source-runtime/run-4f8687bcc001476bbd34f5409be3345a/probe.json'))
args = parser.parse_args()
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
records = json.loads(args.runtime.read_text(encoding='utf-8-sig'))
fixtures = {r['name']: r['fileSha256'] for r in records if r.get('phase') == 'codec-fixture'}
observations = []
for r in records:
    name = r.get('phase', '')
    if name not in fixtures:
        continue
    pixels = args.runtime.parent / (name + '.pixels')
    assert sha(pixels) == r['pixelSha256']
    fixture = args.runtime.parent / 'codec-fixtures' / name.removeprefix('codec-')
    assert sha(fixture) == fixtures[name]
    observation = {key: r[key] for key in ['phase', 'nativeErrorCode', 'sn', 'width', 'height', 'format', 'length', 'pixelSha256']}
    observation.update(fixtureSha256=fixtures[name], pixelsHex=pixels.read_bytes().hex())
    observations.append(observation)
assert len(observations) == 13
source_root = Path('D:/MyWork/REA逆向测试/analysis/full-audit')
sources = [source_root / n for n in ['image-source-native-0.c', 'image-source-native-4.c', 'image-source-media-decode-implementation.c']]
rgb_record = next(r for r in records if r.get('phase') == 'codec-rgb-color.jpg')
rgb_pixels = args.runtime.parent / 'codec-rgb-color.jpg.pixels'
assert sha(rgb_pixels) == rgb_record['pixelSha256']
rgb = rgb_pixels.read_bytes()
native_jpeg = next(r for r in observations if r['phase'] == 'codec-color.jpg')
assert bytes((76*rgb[i]+150*rgb[i+1]+30*rgb[i+2]) >> 8 for i in range(0, len(rgb), 3)).hex() == native_jpeg['pixelsHex']
prior = Path('analysis/full-audit/image-source-runtime/run-983ac3347f34461ab7270d943c020272/codec-truncated.bmp.pixels')
current = args.runtime.parent / 'codec-truncated.bmp.pixels'
result = {'schema': 1, 'targetSha256': sha(Path('analysis/native-input/ImageSourceModule.dll')), 'runtimeSource': str(args.runtime), 'runtimeSha256': sha(args.runtime), 'fixtureGeneratorSha256': sha(Path('tests/NativeEvidence/ImageSourceCodecFixtures.cs')), 'nativeSources': [{'path': str(p), 'sha256': sha(p)} for p in sources], 'observations': observations, 'jpegRgbReference': {'pixelSha256': sha(rgb_pixels), 'pixelsHex': rgb.hex(), 'monoRuleMatches': True}, 'malformedRepeat': {'firstSha256': sha(prior), 'secondSha256': sha(current), 'differentPixels': sum(a != b for a, b in zip(prior.read_bytes(), current.read_bytes())), 'limitation': 'Truncated BMP returns success with unstable pixels; plugin rejects it deliberately'}, 'completeParity': False}
Path('docs/plugins/image-source-codec-evidence.json').write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Exported {len(observations)} original codec observations')
