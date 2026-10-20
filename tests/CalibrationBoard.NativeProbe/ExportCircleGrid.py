import pathlib,json,hashlib,struct,zlib,base64
root=pathlib.Path.cwd();probe=root/'tests/CalibrationBoard.NativeProbe/CircleGridProbe.cpp'
allrows=[json.loads(s) for s in (root/'analysis/calibration-board/circle-grid-native/runtime.jsonl').read_text().splitlines()]
rows=[r for r in allrows if r['phase']=='grid'];adjacency=[r for r in allrows if r['phase']=='adjacency'];pruned=[r for r in allrows if r['phase']=='pruned'];assert len(rows)==70 and len(adjacency)==140 and len(pruned)==70
for row in adjacency+pruned:
 data=bytes.fromhex(row.pop('hex'));assert len(data)==row['count']*84;row['zlibBase64']=base64.b64encode(zlib.compress(data)).decode()
dll=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll').read_bytes();pe=struct.unpack_from('<I',dll,60)[0];n=struct.unpack_from('<H',dll,pe+6)[0];size=struct.unpack_from('<H',dll,pe+20)[0]
def read(rva,size):
 for i in range(n):
  _,vs,va,sz,off,*_=struct.unpack_from('<8sIIIIIIHHI',dll,pe+24+struct.unpack_from('<H',dll,pe+20)[0]+40*i)
  if va<=rva<va+max(vs,sz):return dll[off+rva-va:off+rva-va+size]
constants=[{'rva':hex(rva),'hex':read(rva,4).hex(),'float':struct.unpack('<f',read(rva,4))[0]} for rva in range(0x1648440,0x1648470,4)]
canonical=[r for r in allrows if r['phase']=='canonical'];assert len(canonical)==70
components=[r for r in allrows if r['phase']=='component'];assert len(components)==70
report={'authority':'Original pure 8cd840/8d07c0 called after public SDK creation, exact synthetic candidate buffers. No device/global platform.', 'probeSha256':hashlib.sha256(probe.read_bytes()).hexdigest(),'constants':constants, 'inputLayout':'Candidate x,y float; edgefit int1; fields3/4 int0; id int1-based; size float8.5; trailing int0. Output 6 words: active,row,column,sizeFloatBits,xFloat,yFloat.', 'limits':'Original helper fixtures verify the independent product neighbor/pruning/component/canonical/matcher pipeline for 70 rotated/missing candidate sets. The maximum-density window now has separate 21-case source fixtures in calibration-board-circle-window-fixtures.jsonl. These fixtures are not full tool fitting parity.', 'rows':rows,'adjacency':adjacency,'pruned':pruned,'components':components,'canonical':canonical}
(root/'docs/plugins/calibration-board-circle-grid-fixtures.json').write_text(json.dumps(report,indent=2)+'\n')
print('Exported',len(rows),'grid cases and',len(adjacency),'adjacency cases; constants',constants)
