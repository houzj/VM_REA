"""Join the actual product diagnostic and fixed native inputs without conflating their scopes."""
import pathlib,json,sys
root=pathlib.Path(__file__).resolve().parents[2];base=root/'analysis/map-calibration'
folder=sys.argv[1] if len(sys.argv)>1 else 'image-execution-current'
execution=json.loads((base/folder/'differences.json').read_text(encoding='utf-8-sig'));fixed=json.loads((base/'image-fit/diagnostics.json').read_text(encoding='utf-8-sig'))
rows=[]
for run in execution['reports']:
 key=(run['weight'],run['model'],run['board']);fit=next(r for r in fixed['report'] if (r['weight'],r['model'],r['board'])==key)
 directions={}
 for name in ['source','target','projection']:
  details=run.get(name+'PointDetails',[])
  if details:
   directions[name]=dict(maxAbsXPoint=max(details,key=lambda p:abs(p['deltaX'] or 0)),maxAbsYPoint=max(details,key=lambda p:abs(p['deltaY'] or 0)),exactXBits=sum(p['nativeXBits']==p['managedXBits'] for p in details),exactYBits=sum(p['nativeYBits']==p['managedYBits'] for p in details),positiveX=sum((p['deltaX'] or 0)>0 for p in details),negativeX=sum((p['deltaX'] or 0)<0 for p in details),positiveY=sum((p['deltaY'] or 0)>0 for p in details),negativeY=sum((p['deltaY'] or 0)<0 for p in details))
 exact=run['status']=='Success' and run['matrixEqualBits']==9 and run['sourcePointMax']==0 and run['targetPointMax']==0 and run['errorDelta']==0 and all(d['exactXBits']==168 and d['exactYBits']==168 for d in directions.values())
 rows.append(dict(**run,actualSdkExact=exact,pointDirections=directions,nativeFinalMatrix=fit['expected'],fixedInputManagedFinalMatrix=fit['actual'],fixedInputMatrixEqualBits=fit['bits'],fixedInputMatrixMaxDelta=fit['delta'],conclusion='Actual product points/matrix/error are bit-exact for this fixture; independent tag geometry and desktop remain separate boundaries' if exact else 'Actual product differences remain; inspect per-ID details'))
out=dict(complete=execution['complete'],cases=len(rows),originalDesktopCompared=False,scope='Original SDK vs actual independent plugin runtime; fixed-native-point fit is a separate controlled comparison',sources=dict(execution=folder+'/differences.json',fixedFit='image-fit/diagnostics.json',postprocessing='image-fit/postprocess-checks.json',decoding='image-input/comparison.json',detectorRejection='detector-boundary/rejection.json'),rows=rows)
(base/folder/'conclusions.json').write_text(json.dumps(out,indent=2),encoding='utf-8')
lines=['# 映射标定真实产品18组差分','',f"已完成 {len(rows)}/18；完整状态：{execution['complete']}。固定原厂点对拟合与图像识别分开判定；不宣称桌面等价。",'', '|权重|模型|板型|产品点数|源/目标最大像素差|产品矩阵位一致|产品矩阵最大差|固定点矩阵位一致|结论|','|---|---|---|---|---|---|---|---|---|']
for r in rows:
 points='/'.join(str(x) for x in r['counts']) if r['counts'] else '失败'
 delta='未进入拟合' if r['matrixMax'] is None else format(r['matrixMax'],'.9g')
 coord='拒绝于Recognize原点一致性' if r['sourcePointMax'] is None else f"{r['sourcePointMax']:.9g}/{r['targetPointMax']:.9g}"
 lines.append(f"|{r['weight']}|{r['model']}|{r['board']}|{points}|{coord}|{r['matrixEqualBits']}/9|{delta}|{r['fixedInputMatrixEqualBits']}/9|{'点/矩阵/误差零差' if r['actualSdkExact'] else '仍有差异'}|")
(root/'docs/plugins/MAP-CALIBRATION-IMAGE-COMPARISON.md').write_text('\n'.join(lines)+'\n',encoding='utf-8');print(f"{len(rows)}/18 joined; complete={execution['complete']}")


