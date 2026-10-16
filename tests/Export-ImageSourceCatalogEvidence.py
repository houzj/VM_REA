"""Retain bounded native comparator and directory enumeration observations."""
import argparse
import hashlib
import json
from pathlib import Path

parser=argparse.ArgumentParser();parser.add_argument('--runtime',type=Path,required=True);args=parser.parse_args()
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads(args.runtime.read_text(encoding='utf-8-sig'))
comparisons=[r for r in records if r.get('phase')=='folder-less'];assert len(comparisons)==121
catalog=next(r for r in records if r.get('phase')=='folder-catalog')
source=Path('analysis/full-audit/image-source-folder-sort/180023550.c')
assembly=source.with_suffix('.asm.txt')
original=Path('D:/MyWork/REA逆向测试/analysis/full-audit/image-source-native-extra-26.c')
binary=Path('analysis/native-input/ImageSourceModule.dll')
assert sha(binary)=='91ea3659be97e51edeab0c913779d4555d51dcd0d6288086fbf9916d35b942db'
result={'schema':1,'runtimeSource':str(args.runtime),'runtimeSha256':sha(args.runtime),'binarySha256':sha(binary),'sourceAnalysis':[{'path':str(p),'sha256':sha(p)} for p in [source,assembly,original]],'confirmed':{'comparator':'0x180023550 converts ANSI to wide, calls StrCmpLogicalW and returns result<0 (SETS BL in assembly)','enumerator':'0x180022750 top-level files; lowercase jpg/png/bmp/tif accepted; directories and other extensions excluded'},'comparisons':comparisons,'catalog':catalog,'limitations':['Native probe invokes known RVAs in the hash-checked loaded module, in an isolated owned process. Vendor return-vector memory is retained until that process exits.','Tie ordering is not guaranteed: native std::sort and independent stable sorting can order equal comparisons differently.','Independent catalog uses Unicode filesystem APIs; original FindFirstFileA uses process ANSI paths.','Catalog snapshot is not folder event monitoring, refresh, lifecycle or scheduler equivalence.'],'completeParity':False}
Path('docs/plugins/image-source-catalog-evidence.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf8')
print('Exported 121 comparator observations and original catalog snapshot')
