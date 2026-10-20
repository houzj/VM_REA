import json,hashlib,pathlib
root=pathlib.Path(__file__).resolve().parents[2];reports=[]
for side in ['source','target']:
 a=(root/f'analysis/map-calibration/image-input/native-{side}.gray').read_bytes();b=(root/f'analysis/map-calibration/image-execution/managed-{side}.gray').read_bytes();reports.append(dict(side=side,nativeBytes=len(a),managedBytes=len(b),nativeSha256=hashlib.sha256(a).hexdigest(),managedSha256=hashlib.sha256(b).hexdigest(),differentPixels=sum(x!=y for x,y in zip(a,b)),maxDifference=max(abs(x-y) for x,y in zip(a,b))))
out=root/'analysis/map-calibration/image-input/comparison.json';out.write_text(json.dumps(reports,indent=2));print(out.read_text())
