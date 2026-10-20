"""Portable summaries of owned original-SDK probes, never a parity claim."""
import hashlib
import json
import math
from collections import defaultdict
from pathlib import Path

root = Path.cwd()
base = root / 'analysis/calibration-board'
def rows(name):
    return [json.loads(line) for line in (base / name / 'runtime.jsonl').read_text(encoding='utf-8-sig').splitlines() if line.strip()]
circleRows = rows('circle-refinement')
circle = [r for r in circleRows if 'shape' in r and 'code' in r]
readbacks = [r for r in circleRows if r.get('phase') == 'parameter-readback']
edgeGroups = defaultdict(list)
for row in circle:
    if row['code'] == 0: edgeGroups[row['shape'], row['contrast'], row['circularity']].append(row)
edgeDifferences = []
for key, group in edgeGroups.items():
    delta = max(math.hypot(a['x']-b['x'],a['y']-b['y']) for a in group for b in group)
    if delta: edgeDifferences.append({'shape': key[0], 'contrast': key[1], 'circularity': key[2], 'maximumFirstPointDelta': delta})
refine = [r for r in rows('refinement') if 'filter' in r]
fit = [r for r in rows('native-point-fit') if r.get('phase') == 'npoint-native-points']
sizes = [r for r in rows('input-sizes') if 'board' in r]
sources = ['CircleProbe.cpp', 'RefinementProbe.cpp', 'OriginalPointFitProbe.cpp', 'SizeProbe.cpp', 'BoardCoreInteractive.java']
report = {
    'authority': 'Original documented SDK; owned synthetic images; no device or original desktop operation',
    'circle': {'cases': len(circle), 'success': sum(r['code'] == 0 for r in circle), 'rows': circle,
        'parameterReadback': readbacks,
        'edgeCoordinateDifferences': edgeDifferences,
        'limits': 'Only three synthetic shapes, six contrasts, five circularities and four edge threshold pairs; no general threshold-invariance claim.'},
    'checkerWindow': {'cases': len(refine), 'rows': refine,
        'limits': 'Identical results on this clean checker do not establish obsolete controls have no effect on all inputs.'},
    'nativePointFit': {'rows': fit,
        'limits': 'Raw integer grid replay; mode 1 only directly matches the default board world transform. Native solver success does not establish managed solver parity.'},
    'inputSize': {'rows': sizes, 'modeFieldMeans': 'Square side length in pixels', 'confirmed': '32/63 return 0x10103054; 64/65/100 reach board-specific blank recognition failure. Core 0x1807f69e0 checks both axes.'},
    'preprocessFixtures':'docs/plugins/calibration-board-preprocess-fixtures.json',
    'edgeFixtures':'docs/plugins/calibration-board-edge-fixtures.json',
    'freshCircleInstances':{'rows':rows('circle-fresh'),'limits':'Contrast2 failure and contrast5 success persist after independently recreating SDK instances; identical half-size masks do not imply identical full recognition.'},
    'managedNativePointReplay': {
        'rows': json.loads((base / 'native-point-fit-differences.json').read_text(encoding='utf-8-sig')),
        'solverSourceSha256': hashlib.sha256((root / 'src/VisionPlugins.Calibration/NPointSolver.cs').read_bytes()).hexdigest(),
        'authority': 'Exact original float point coordinates and original board world transform; managed shared solver. Compared against board SDK metrics, distinct from separate NPoint SDK replay.',
    },
    'probeSources': [{'path': f'tests/CalibrationBoard.NativeProbe/{name}', 'sha256': hashlib.sha256((root / 'tests/CalibrationBoard.NativeProbe' / name).read_bytes()).hexdigest()} for name in sources],
    'coreFunctions': [{'address': p.stem, 'run': p.parent.parent.name, 'sha256': hashlib.sha256(p.read_bytes()).hexdigest()} for directory in sorted(base.glob('core-live*')) for p in sorted((directory / 'output').glob('*.c'))],
    'sdkBoardDispatch': {'runAddress': '0x180002ad0', 'mapAddress': '0x180003ee0',
        'publicToCore': {'1': 1, '2': 3, '3': 7, '4': 6, '5': 'invalid', '6': 6, '7': 7, '8': 8, '9': 9},
        'authority': 'Original SDK Run decompilation and dumpbin jump table at RVA0x3f9c; core kind2 is not public circle type2.',
        'runSha256': hashlib.sha256((base / 'sdk-decompile/output/180002ad0.c').read_bytes()).hexdigest(),
        'sdkDisassemblySha256': hashlib.sha256((base / 'sdk-disassembly.txt').read_bytes()).hexdigest()},
    'remaining': ['Native checker subpixel coordinates', 'Non-frame enclosing circle fallback', 'General inputs beyond 360 circle/70 grid/27 fit cases', 'Original desktop visual acceptance'],
}
(root / 'docs/plugins/calibration-board-refinements.json').write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Exported {len(circle)} circle, {len(refine)} checker, {len(fit)} original NPoint replay cases')
