import pathlib,json,hashlib,struct
root=pathlib.Path.cwd();probe=root/'tests/CalibrationBoard.NativeProbe/CircleCandidateProbe.cpp'
base=root/'analysis/calibration-board/circle-candidates';summary=base/'dataset-summary.json'
if summary.exists():
 manifest=json.loads(summary.read_text(encoding='utf-8-sig'));assert manifest['sourceSha256']==hashlib.sha256(probe.read_bytes()).hexdigest()
 rows=[json.loads(line) for group in range(36) for line in (base/f'runtime-group-{group}.jsonl').read_text().splitlines()]
 assert len(rows)==360 and len({row['index'] for row in rows})==360
else:
 rows=[json.loads(line) for line in (base/'runtime.jsonl').read_text().splitlines()];assert len(rows)==6
assert all(row['factor']==5 and row['sampling']==0 for row in rows)
dll=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll')
with dll.open('rb') as f:
 f.seek(60);pe=struct.unpack('<I',f.read(4))[0];f.seek(pe+6);count=struct.unpack('<H',f.read(2))[0];f.seek(pe+20);size=struct.unpack('<H',f.read(2))[0];f.seek(pe+24+size)
 sections=[struct.unpack('<8sIIIIIIHHI',f.read(40)) for _ in range(count)];rva=0x1627a50;section=next(s for s in sections if s[2]<=rva<s[2]+max(s[1],s[3]));f.seek(section[4]+rva-section[2]);data=f.read(96)
report={'authority':'Original 877230 pure candidate stage after normal public SDK creation; owned buffers, original resize factor5; not the complete board module',
 'sourceSha256':hashlib.sha256(probe.read_bytes()).hexdigest(),'defaultResampling':{'source':'7c9320 copies 181627a50 to engine+170; 7f69e0 mode2 selects engine+194 for options+30; 812930 divides dimensions by that factor','tableRva':'0x1627a50','rawHex':data.hex(),'mode':struct.unpack_from('<I',data,0x18)[0],'factor':struct.unpack_from('<I',data,0x24)[0]},'rows':rows,
 'limits':'Only synthetic shapes/contrasts/circularities/manual thresholds in rows. Initial candidate stage only; unsupported nonrectangular fallback, grid matching, adaptive refinements and complete module parity remain open.'}
(root/'docs/plugins/calibration-board-candidate-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
print('Exported original candidate cases',len(rows))
