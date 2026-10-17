import json,pathlib,hashlib
root=pathlib.Path('.').resolve();folder=root/'analysis/full-audit/n-image-calibration/median-probe';records=[json.loads(s) for s in (folder/'runtime.jsonl').read_text().splitlines()];bad=[]
for r in records:
 w,h=r['width'],r['height'];expected=[]
 for y in range(h):
  for x in range(w):
   values=sorted(r['input'][(y+dy)*w+x+dx] for dy in [-1,0,1] for dx in [-1,0,1] if 0<=x+dx<w and 0<=y+dy<h);n=len(values);expected.append(values[n//2] if n%2 else (values[n//2-1]+values[n//2]+1)//2)
 bad.append(sum(a!=b for a,b in zip(expected,r['output'])))
print('Native median mismatches',bad)
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest();native=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll');output={'authority':'owned process directly invokes original MVD_Algorithm RVA 8afe10 with owned arrays','nativeSha256':sha(native),'producerSha256':sha(root/'tests/NativeEvidence/OriginalNImageMedianProbe.cpp'),'nativeOutputSha256':sha(folder/'runtime.jsonl'),'records':records,'comparison':{'definition':'3x3 median, clipped windows, rounded average of middle two at even-size edges','mismatches':bad},'lifecycle':'flush then TerminateProcess(GetCurrentProcess(),0); unsupported core unloading is not claimed equivalent'};(root/'docs/plugins/n-image-calibration-filter-evidence.json').write_text(json.dumps(output,indent=2)+'\n',encoding='utf-8')

