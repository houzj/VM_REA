# Rebuild native probe matrices from tracked source fixtures, preserving signed zero.
import json, pathlib, struct, sys
root=pathlib.Path(__file__).resolve().parents[2]
rows=json.loads((root/'docs/plugins/distortion-calibration-matrix-parameters-evidence.json').read_text(encoding='utf-8-sig'),parse_int=lambda s:-0.0 if s=='-0' else int(s))['cases']
output=pathlib.Path(sys.argv[1]) if len(sys.argv)>1 else root/'analysis/distortion-calibration/matrix-analysis/matrices.bin'
output.parent.mkdir(parents=True,exist_ok=True)
output.write_bytes(b''.join(struct.pack('<9f',*row['matrix']) for row in rows))
print(f'{len(rows)} raw matrices: {output}')
