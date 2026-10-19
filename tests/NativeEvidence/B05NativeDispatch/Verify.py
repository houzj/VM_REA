"""Check retained dumpbin printed byte prefixes against exact saved PE slices."""
import hashlib
import json
import re
from pathlib import Path

ROOT=Path(__file__).resolve().parents[3]
OUT=ROOT/'analysis/full-audit/batch8-20261019/B05/instructions-abc22ca0df7e4a1782356f1e05017828'
report=json.loads((OUT/'functions.json').read_text(encoding='utf-8'))
checks=[]
for f in report['functions']:
 original=Path(f['path']).read_bytes()
 assert hashlib.sha256(original).hexdigest()==f['sha256Before']==f['sha256After']
 for i,reg in enumerate(f['regions']):
  stem=f['export']+'.region'+str(i)
  raw=(OUT/(stem+'.bin')).read_bytes()
  assert hashlib.sha256(raw).hexdigest()==reg['bytesSha256']
  text=(OUT/(stem+'.assembly.txt')).read_text(encoding='utf-8')
  n=0
  for line in text.splitlines():
   match=re.match(r'^\s*([0-9A-F]{16}):\s*((?:[0-9A-F]{2}\s+)+)',line)
   if not match: continue
   address=int(match[1],16); printed=bytes.fromhex(match[2]); delta=address-int(f['imageBase'],16)-int(reg['beginRva'],16)
   assert 0<=delta<len(raw) and raw[delta:delta+len(printed)]==printed
   n+=1
  assert n==reg['instructionCount']
  checks.append({'member':f['export'],'printedPrefixesVerified':n,'sha256':reg['bytesSha256']})
(OUT/'byte-prefix-checks.json').write_text(json.dumps(checks,indent=2)+'\n',encoding='utf-8')
print(json.dumps(checks))
