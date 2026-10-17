"""Index local static/runtime provenance and explicitly retained numerical differences."""
import hashlib,json,pathlib
root=pathlib.Path(__file__).resolve().parents[1]
def digest(path):return hashlib.sha256(path.read_bytes()).hexdigest()
evidence=json.loads((root/'docs/plugins/n-point-solver-evidence.json').read_text())
verification=json.loads((root/'docs/plugins/n-point-solver-verification.json').read_text())
assert evidence['probeSha256']==digest(root/'tests/NativeEvidence/OriginalNPointSolverProbe.cpp')
assert len(evidence['cases'])==verification['sourceCases']
assert verification['failures']==0
native=root/'analysis/full-audit/n-point-solver/core-native'
procedures={
 '1800152b0':'official MVD_Algorithm_Process export',
 '18001c4e0':'owned core object process dispatch; vtable slot +0x18',
 '18001d900':'N-point wrapper dispatch 0x65; engine operation 6',
 '1807fccd0':'platform calibration parameter conversion; WorldError=PixelError*precision',
 '1807f3420':'point-count validation, platform/rotation orchestration, status thresholds',
 '1807f2560':'four-point subset selection up to 40 translation points; larger sets fit once',
 '1807ff3c0':'center/scale normalization and initial fit',
 '1807fd4e0':'50-iteration IRLS; 0.0001 normalized coefficient stopping threshold',
 '1807fec40':'inverse-projection residuals and upper median * 1.482579708',
 '180800170':'linear perspective/affine/similarity design matrices',
 '1807075e0':'normal equation least squares wrapper',
 '180730e90':'single precision full-pivot matrix inversion',
 '1807f2170':'two rotation-direction candidates, 0.1/1.5 selection rule',
 '1807f04b0':'rotation center fitting and residuals',
 '180800af0':'normalized inverse matrix decomposition in radians with FLT_EPS denominators'
}
runtime=pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK/Runtime/x64')
paths=['src/VisionPlugins.Calibration/NPointSolver.cs','tests/NPointSolver.Checks/Program.cs','tests/NPointSolver.Checks/NPointSolver.Checks.csproj','tests/NativeEvidence/OriginalNPointSolverProbe.cpp','tests/NativeEvidence/Invoke-OriginalNPointSolverProbe.ps1','tests/NativeEvidence/VmNPointSolverCore.java','tests/Export-NPointSolverEvidence.py','tests/Export-NPointSolverVerification.py','docs/plugins/n-point-solver-evidence.json','docs/plugins/n-point-solver-verification.json']
report={
 'scope':'纯C#独立数值实现；原厂直接SDK自有进程受控回放，无设备；Ghidra CLI静态分析不冒充REA深度提供器运行。通过回归不代表全部原生浮点等价。',
 'binaries':[{'path':str(runtime/name),'sha256':digest(runtime/name)} for name in ['MVDNPointCalibCpp.dll','MVD_Algorithm.dll']],
 'staticProcedures':[{'address':address,'finding':description,'path':str((native/(address+'.c')).relative_to(root)),'sha256':digest(native/(address+'.c'))} for address,description in procedures.items()],
 'implementationSources':[{'path':p,'sha256':digest(root/p)} for p in paths],
 'runtimeCases':verification['sourceCases'],'successfulOriginalMatrices':verification['matrixCases'],
 'fieldChecks':verification['checks'],'unexpectedFailures':verification['failures'],
 'documentedDifferingFields':len(verification['differences']),'conditioningObservations':len(verification['conditioning']),
 'boundaries':[
  '镜像点对无法由保持方向的相似变换唯一表达，多个四点子集有相同均误差；6组原厂单精度选优方向未逐值复现。',
  '四点中三点共线的透视退化样本，原厂输出极大不稳定矩阵；独立实现返回0x10103007，未硬编码垃圾系数。',
  '最低Huber强度1的透视点强制相似样本，归一化迭代终止存在约0.00108平移系数差，严格0.001比较仍列为差异。',
  'Scale/Aspect在原厂cos(theta)+FLT_EPS或sin(theta+skew)*scale+FLT_EPS分母邻近零时对float舍入极敏感；报告保留实际值，未宣称数值一致。',
  '最大误差并列或接近零时原厂float舍入可选择不同点号；报告保留该条件，不能由同一误差容差推导索引一致。',
  '产品仅实现界面公开的三种权重；DistThreshold/SampleRatio只在原厂WeightFun=4的RANSAC分支参与拟合，该分支不在当前工具UI。'
 ],
 'reproduction':['pwsh -File tests/NativeEvidence/Invoke-OriginalNPointSolverProbe.ps1','dotnet run --project tests/NPointSolver.Checks','python tests/Export-NPointSolverVerification.py']
}
(root/'docs/plugins/n-point-solver-findings.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f"Indexed {report['runtimeCases']} native cases, {report['fieldChecks']} checks; explicitly retained {report['documentedDifferingFields']} differing fields")
