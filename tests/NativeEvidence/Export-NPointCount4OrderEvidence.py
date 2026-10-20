"""Re-run the degenerate four-point SDK fixture in original and permuted order."""
import hashlib, itertools, json, os, pathlib, subprocess

root = pathlib.Path(__file__).resolve().parents[2]
evidence = json.loads((root / 'docs/plugins/n-point-solver-evidence.json').read_text())
fixture = next(c for c in evidence['cases'] if c['name'] == 'count4-f1')
assert fixture['freedom'] == 1 and fixture['weight'] == 1 and len(fixture['points']) == 4
runtime = pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64')
solver = root / 'analysis/full-audit/n-point-solver/solver.exe'
if not solver.exists():
    raise SystemExit('Build the original SDK probe first: pwsh -File tests/NativeEvidence/Invoke-OriginalNPointSolverProbe.ps1')
work = root / 'analysis/build/npoint-count4-order-evidence'
work.mkdir(parents=True, exist_ok=True)

def scenario(name, points):
    return {'name': name, 'mode': fixture['mode'], 'freedom': fixture['freedom'],
            'weight': fixture['weight'], 'points': points, 'rotation': [],
            'origin': fixture['origin'], 'weightFactor': fixture['weightFactor'],
            'distThreshold': fixture['distThreshold'], 'sampleRatio': fixture['sampleRatio']}

def encode(scenarios):
    lines = []
    for c in scenarios:
        lines.append(' '.join(map(str, [c['name'], c['mode'], c['freedom'], c['weight'],
            len(c['points']), len(c['rotation']), *c['origin'], c['weightFactor'],
            c['distThreshold'], c['sampleRatio']])))
        lines.extend(' '.join(map(str, p)) for p in c['points'] + c['rotation'])
    return ('\n'.join(lines) + '\n').encode()

def run(tag, scenarios):
    input_bytes = encode(scenarios)
    input_path, output_path = work / f'{tag}.txt', work / f'{tag}.jsonl'
    input_path.write_bytes(input_bytes)
    env = os.environ.copy(); env['PATH'] = str(runtime) + ';' + env.get('PATH', '')
    with output_path.open('wb') as output:
        subprocess.run([str(solver), str(runtime / 'MVDNPointCalibCpp.dll'), str(input_path)],
            cwd=root, env=env, stdout=output, stderr=subprocess.PIPE, timeout=120, check=True)
    rows = [json.loads(line) for line in output_path.read_text().splitlines()]
    assert len(rows) == len(scenarios), (tag, len(rows), len(scenarios))
    return input_bytes, output_path.read_bytes(), rows

repeats = [scenario(f'repeat-{i:02d}', fixture['points']) for i in range(12)]
permutations = [scenario('perm-' + ''.join(map(str, p)), [fixture['points'][i] for i in p])
                for p in itertools.permutations(range(4))]
repeat_input, repeat_output, repeat_rows = run('repeat', repeats)
permutation_input, permutation_output, permutation_rows = run('permutations', permutations)

def selected(row):
    return {'status': row.get('status'), 'matrix': row.get('matrix'),
            'offset': row.get('offset'), 'exception': row.get('exception')}

report = {
 'scope': 'Original owned-process SDK; one tool instance per batch; no device; this is a degenerate four-point input-order probe.',
 'sourceFixture': {'name': fixture['name'], 'mode': fixture['mode'], 'HomoFreedom': fixture['freedom'],
                   'WeightFun': fixture['weight'], 'points': fixture['points'],
                   'canonicalJsonSha256': hashlib.sha256(json.dumps(fixture, sort_keys=True, separators=(',', ':')).encode()).hexdigest()},
 'sdkBinaries': {name: hashlib.sha256((runtime/name).read_bytes()).hexdigest()
                 for name in ['MVDNPointCalibCpp.dll','MVD_Algorithm.dll']},
 'repeatBatch': {'count': len(repeat_rows), 'inputSha256': hashlib.sha256(repeat_input).hexdigest(),
                 'stdoutSha256': hashlib.sha256(repeat_output).hexdigest(),
                 'uniqueSerializedOutputs': len({json.dumps(selected(r),sort_keys=True) for r in repeat_rows}),
                 'outputs': [{'iteration': i, **selected(r)} for i,r in enumerate(repeat_rows)]},
 'permutationBatch': {'count': len(permutation_rows), 'inputSha256': hashlib.sha256(permutation_input).hexdigest(),
                      'stdoutSha256': hashlib.sha256(permutation_output).hexdigest(),
                      'uniqueSerializedOutputs': len({json.dumps(selected(r),sort_keys=True) for r in permutation_rows}),
                      'outputs': [{'order': ''.join(map(str,p)), **selected(r)}
                                  for p,r in zip(itertools.permutations(range(4)),permutation_rows)]},
 'interpretation': [
   'Repeated identical ordered input is stable for all reported fields within this same-process batch; this does not prove where the SDK seeds or resets CRT rand state.',
   'Permuting the same four point pairs changes the original SDK matrix and residuals substantially, consistent with point-order-sensitive single-precision fit accumulation on this ill-conditioned fixture.',
   'The probe supplies HomoFreedom=1 and WeightFun=1. NPointDefinition.xml orders HomoFreedom then WeightFun; 1807c6830 stores the corresponding first/second enumeration at +0x9c/+0xa0, and 1807f2560 reads its second enum into iStack_60. Therefore this fixture does not hit iStack_60==4 → 180835a20 (the WeightFun=4 rand-sampling path).',
   'With four points the 1807f2560 combination loop has only one 4-point subset. Candidate tie/order selection therefore cannot explain the coefficient differences in this fixture.',
 ],
 'reproduction': ['pwsh -File tests/NativeEvidence/Invoke-OriginalNPointSolverProbe.ps1',
                 'python tests/NativeEvidence/Export-NPointCount4OrderEvidence.py']
}
(root/'docs/plugins/npoint-count4-order-evidence.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f"count4-f1: {report['repeatBatch']['uniqueSerializedOutputs']} unique repeated outputs / {report['permutationBatch']['uniqueSerializedOutputs']} unique permutation outputs")
print(f"repeat stdout sha256={report['repeatBatch']['stdoutSha256']}")
print(f"permutation stdout sha256={report['permutationBatch']['stdoutSha256']}")
