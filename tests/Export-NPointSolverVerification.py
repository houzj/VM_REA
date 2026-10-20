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
paths=['src/VisionPlugins.Calibration/NPointSolver.cs','tests/NPointSolver.Checks/Program.cs','tests/NPointSolver.Checks/NPointSolver.Checks.csproj','tests/NativeEvidence/OriginalNPointSolverProbe.cpp','tests/NativeEvidence/Invoke-OriginalNPointSolverProbe.ps1','tests/NativeEvidence/VmNPointSolverCore.java','tests/Export-NPointSolverEvidence.py','tests/Export-NPointSolverVerification.py','tests/NativeEvidence/Export-NPointCount4OrderEvidence.py','docs/plugins/n-point-solver-evidence.json','docs/plugins/n-point-solver-verification.json','docs/plugins/npoint-count4-order-evidence.json']
report={
 'scope':'纯C#独立数值实现；原厂直接SDK自有进程受控回放，无设备；Ghidra CLI静态分析不冒充REA深度提供器运行。通过回归不代表全部原生浮点等价。',
 'binaries':[{'path':str(runtime/name),'sha256':digest(runtime/name)} for name in ['MVDNPointCalibCpp.dll','MVD_Algorithm.dll']],
 'staticProcedures':[{'address':address,'finding':description,'path':str((native/(address+'.c')).relative_to(root)),'sha256':digest(native/(address+'.c'))} for address,description in procedures.items()],
 'implementationSources':[{'path':p,'sha256':digest(root/p)} for p in paths],
 'runtimeCases':verification['sourceCases'],'successfulOriginalMatrices':verification['matrixCases'],
 'fieldChecks':verification['checks'],'unexpectedFailures':verification['failures'],
 'documentedDifferingFields':len(verification['differences']),'conditioningObservations':len(verification['conditioning']),
 'boundaries':[
  '1807f2560 对四点子集评分时调用 180730e90 做 3x3 单精度全主元求逆；候选均误差仅以严格小于替换，等分保留先枚举者。独立实现已将候选评分求逆改为该来源算法，六组镜像样本现纳入普通逐字段 SDK 回归并通过。原厂逐候选 trace 不可用，因此不能声称运行轨迹直接确认具体选中的组合。',
  '四点中三点共线的透视样本由原厂全主元候选评分路径复现其矩阵和状态；OffsetWorldError 与 OffsetWorldMaxError 仍分别有约 0.00166 与 0.00206 的有界数值差，逐字段保留为已知差异。这只确认该 fixture，不推断其他病态输入均稳定。',
  '最终 OffsetError/OffsetMaxError 的原厂路径（1807f2560 → 1807f3420 → 1807fccd0）同样调用 180730e90 对已选 3×3 矩阵做全主元逆投影；独立实现的最终偏移残差此前误用伴随矩阵 FloatInverse，现已改为 NativeFullPivotInverse3x3。对固定 SDK 矩阵，C# 全主元实现得到 mean/max 17.57632828/42.15121078，与 SDK 输出相符；伴随式得到 17.57632446/42.15119934。产品拟合矩阵与 SDK 最大系数差约 1.93e-6，用同一全主元逆得到 17.57632065/42.15119934，剩余两个世界误差字段差值由拟合矩阵与精度计算差共同保留，未扩容差。',
  'count4-f1 精确原厂 fixture（canonical JSON SHA-256 2f14e38e5df82a51108b950edbfc7f8a90d53282e4f75e5663c0a3fd9d5075cd）已做 12 次同序运行及 24 种点排列：同序输出 1 种，排列输出 22 种。重复/排列批输入 SHA-256 为 d6a4306c60d1bd47236441f325540dd9237ca4f3d20049f0755bff197cc0ff85 / 21549a341fb37011a449bc484db0bbfc39090add9d79b7ac16e6f26bbda562a2；stdout SHA-256 为 e93ae89037b6efea739f6a6dd934e33a3972080d960bc2a627b29253f1f1a980 / d4062ffd71ec280b6d3a7ec0aeffb745d27039634fb186d5c6252ab98fd6c0b5。固定输入顺序重复稳定、重排敏感；该 WeightFun=1 fixture 不进入 iStack_60==4 → 180835a20 的 rand 分支，故不能据此判断其随机种子/全局 RNG 状态。详见 npoint-count4-order-evidence.json。',
  '此前最低Huber强度样本约0.00108的平移差在原厂float初始拟合修正后消失；该样本现纳入普通逐字段回归，不再列为已知差异。',
  'Scale/Aspect在原厂cos(theta)+FLT_EPS或sin(theta+skew)*scale+FLT_EPS分母邻近零时对float舍入极敏感；报告保留实际值，未宣称数值一致。',
  '最大误差并列或接近零时原厂float舍入可选择不同点号；报告保留该条件，不能由同一误差容差推导索引一致。',
  '产品仅实现界面公开的三种权重；DistThreshold/SampleRatio只在原厂WeightFun=4的RANSAC分支参与拟合，该分支不在当前工具UI。'
 ],
 'reproduction':['pwsh -File tests/NativeEvidence/Invoke-OriginalNPointSolverProbe.ps1','dotnet run --project tests/NPointSolver.Checks','python tests/Export-NPointSolverVerification.py']
}
(root/'docs/plugins/n-point-solver-findings.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(f"Indexed {report['runtimeCases']} native cases, {report['fieldChecks']} checks; explicitly retained {report['documentedDifferingFields']} differing fields")
