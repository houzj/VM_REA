"""Owned, reproducible cases and source/runtime ledger. No original binary redistribution."""
import argparse,base64,hashlib,json,pathlib,datetime,random,struct
root=pathlib.Path(__file__).resolve().parents[2]
out=root/'analysis/camera-mapping/native';out.mkdir(parents=True,exist_ok=True)
parser=argparse.ArgumentParser();parser.add_argument('--cases-only',action='store_true');args=parser.parse_args()
lines=[];cases=[]
def case(name,f,w,points,k=20):
 lines.append(f'{name} {f} {w} {k} {len(points)}');lines.extend(' '.join(map(str,v)) for v in points);cases.append(dict(name=name,freedom=f,weight=w,factor=k,points=points))
base=[(0,0),(10,0),(0,10),(10,10),(4,3),(8,2)]
for f in [1,2,3]:
 for w in [1,2,3]:
  for n in [0,1,2,3,4,6]:case(f'exact_f{f}_w{w}_n{n}',f,w,[(x,y,2*x+10,2*y-5) for x,y in base[:n]])
  case(f'affine_f{f}_w{w}',f,w,[(x,y,2*x+.5*y+10,-.2*x+3*y-5) for x,y in base])
  case(f'outlier_f{f}_w{w}',f,w,[(x,y,2*x+10+(30 if i==5 else 0),2*y-5) for i,(x,y) in enumerate(base)])
for f in [1,2,3]:case(f'collinear_{f}',f,1,[(i,i,2*i+1,3*i+2) for i in range(6)])
for f in [0,4]:case(f'badfreedom_{f}',f,1,[(x,y,x,y) for x,y in base])
for w in [0,4]:case(f'badweight_{w}',2,w,[(x,y,x,y) for x,y in base])
for k in [0,1,200,201]:case(f'factor_{k}',2,3,[(x,y,x,y) for x,y in base],k)
rng=random.Random(440066)
for f in [1,2,3]:
 for w in [1,2,3]:
  for n in [4,8,32]:
   data=[]
   for i in range(n):
    x=rng.uniform(-100,100);y=rng.uniform(-80,80)
    if f==1:z=.0004*x-.0002*y+1;u=(1.7*x+.3*y+13)/z;v=(-.2*x+2.1*y-7)/z
    elif f==2:u=1.7*x+.3*y+13;v=-.2*x+2.1*y-7
    else:u=1.7*x-.3*y+13;v=.3*x+1.7*y-7
    if n>4:u+=rng.uniform(-.02,.02);v+=rng.uniform(-.02,.02)
    if n==32 and i==0:u+=15;v-=10
    data.append((x,y,u,v))
   case(f'generated_f{f}_w{w}_n{n}',f,w,data)
case('two_object_same',1,1,[(1,2,4,5),(1,2,6,9)])
case('two_target_same',1,1,[(0,0,5,5),(10,0,5,5)])
case('two_both_same',1,1,[(1,2,5,6),(1,2,5,6)])
case('three_object_collinear',1,1,[(0,0,0,0),(1,1,2,0),(2,2,0,2)])
case('three_target_collinear',1,1,[(0,0,0,0),(10,0,1,1),(0,10,2,2)])
case('four_target_collinear',1,1,[(0,0,0,0),(10,0,1,1),(0,10,2,2),(10,10,3,3)])
case('four_repeated_start',1,1,[(0,0,0,0),(0,0,0,0),(10,0,2,0),(0,10,0,2)])
case('four_object_collinear',1,1,[(0,0,0,0),(1,1,2,0),(2,2,0,2),(3,3,2,2)])
(out/'cases.txt').write_text('\n'.join(lines)+'\n',encoding='ascii')
if args.cases_only:raise SystemExit()
def digest(path):
 p=pathlib.Path(path);return dict(path=str(p),bytes=p.stat().st_size,sha256=hashlib.sha256(p.read_bytes()).hexdigest())
install=pathlib.Path('D:/Program Files/VisionMaster4.4.0')
module=install/'Applications/Module(sp)/x64/Calibration/IMVSCameraMapModu'
sources=[digest(p) for p in sorted(module.iterdir()) if p.is_file()]
sources += [digest(install/'MVDAlgorithmSDK'/p) for p in ['Includes/Algorithms/CameraMapCpp.h','Includes/Algorithms/MatrixToParamCpp.h','Runtime/x64/MVDCameraMapCpp.dll','Runtime/x64/MVDMatrixToParamCpp.dll','Runtime/x64/MVD_Algorithm.dll']]
sources += [digest(install/'Applications/Lang'/p) for p in ['zh-cn.xaml','en-us.xaml']]
source_root=pathlib.Path('D:/MyWork/REA逆向测试/analysis/decompiled')
sources += [digest(source_root/p) for p in ['Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/PointPair.cs','Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/AddButton_PointPair.cs','Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/OpenFileForCalibDialog.cs','Apps.XmlUI/styles/paramusercontroldatatemplate.xaml']]
results=[json.loads(line) for line in (out/'results.jsonl').read_text(encoding='utf-8-sig').splitlines()]
module_results=[json.loads(line) for line in (out/'module-results.jsonl').read_text(encoding='utf-8-sig').splitlines()]
comparison=json.loads((root/'analysis/camera-mapping/managed/comparison.json').read_text(encoding='utf-8-sig'))
core_files=['1800152b0.c','18001c4e0.c','18001df30.c','1807c6ba0.c','1807f6330.c','1807d2ea0.c','1807ff3c0.c','1807fd4e0.c','1807fec40.c','180800170.c','1807fcf60.c']
core_sources=[digest(root/'analysis/full-audit/n-point-solver/core-native'/p) for p in core_files]
core_sources.append(digest(root/'analysis/camera-mapping/ghidra/wrapper/180002620.c'))
core_sources += [digest(p) for p in sorted((root/'analysis/camera-mapping/ghidra/module').glob('*.c'))]
sources += [digest(source_root/p) for p in ['VM.Core/VM.Core/VmModule.cs','VM.Core/VM.Core/VmIO.cs']]
# Original PE RVA-to-file mapping; this is independent byte inspection, not REA provider output.
b=(install/'MVDAlgorithmSDK/Runtime/x64/MVD_Algorithm.dll').read_bytes();nt=struct.unpack_from('<I',b,0x3c)[0];nsec=struct.unpack_from('<H',b,nt+6)[0];sec=nt+24+struct.unpack_from('<H',b,nt+20)[0];constants=[]
for rva,fmt in [(0x162b60c,'f'),(0x162b690,'d'),(0x162b698,'f'),(0x162b69c,'f'),(0x162b6a8,'f')]:
 for i in range(nsec):
  vs,va,size,ptr=struct.unpack_from('<IIII',b,sec+i*40+8)
  if va<=rva<va+max(size,vs):
   off=ptr+rva-va;length=struct.calcsize(fmt);constants.append(dict(rva=hex(rva),fileOffset=off,bytes=b[off:off+length].hex(),value=struct.unpack_from('<'+fmt,b,off)[0]))
parser_dir=root/'analysis/camera-mapping/parser'
parser_runtime=dict(factory=json.loads((parser_dir/'operation-factory.json').read_text(encoding='utf-8-sig')),modelVisibilityOperations=[r for r in json.loads((parser_dir/'model/probe.json').read_text(encoding='utf-8-sig')) if r.get('phase')=='visibility-trigger-operation'],modelDigest=digest(parser_dir/'model/probe.json'))
report=dict(productVerification=json.loads((root/'analysis/camera-mapping/product/checks.json').read_text(encoding='utf-8-sig')),platformIndexEntry=[json.loads(s) for s in (parser_dir/'platform-entry.jsonl').read_text(encoding='utf-8-sig').splitlines()],additionalOriginalFiles=[dict(**digest(p),base64=base64.b64encode(p.read_bytes()).decode()) for p in sorted(out.glob('module.iwcal.*.iwcal'))],parserRuntime=parser_runtime,nativeKernelCliSources=core_sources,nativeConstants=constants,tool='IMVSCameraMapModu',generatedUtc=datetime.datetime.now(datetime.timezone.utc).isoformat(),status='in-progress-not-closed',managedReaEvidence='ev_0809b590345cf275953fab58b7afe715bbeedb5cfe50881238de03462802e1bb',sources=sources,caseCount=len(cases),cases=cases,sdkResults=results,moduleResults=module_results,numericComparison=comparison,originalGeneratedFile=dict(**digest(out/'module.iwcal'),base64=base64.b64encode((out/'module.iwcal').read_bytes()).decode()),productFiles=[digest(p) for p in sorted((root/'src/VisionPlugins.CameraMapping').glob('*.cs'))],evidenceLayers=dict(source='original XML, SDK headers, decompiled managed PointPair/asset resources',sdkRuntime='normally created official SDK, no device or license patch',moduleRuntime='owned native module, source-confirmed official tools at +0xc0/+0xc8, synthetic ModuleFrame IO imports; no platform Init',managed='independent managed solve and shared production configuration',desktop='original desktop not captured; source-derived screenshot only'),remaining=['Corpus numeric matches do not establish all native degeneracies, floating-point boundaries or original desktop parity','POINT component alias and array graph transport validated through actual tool; indexed binding remains unsupported','Configuration reset, effective parameters, path/save and shared display-position projection validated in actual WPF; complete original display and remote file protocol remain outside current acceptance','Native platform scheduling/complete file/locale error matrix and original desktop pixel comparison unverified','Host catalog registration and final user acceptance pending'])
target=root/'docs/plugins/camera-mapping-evidence.json';target.write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8');print(f'{target}: {len(cases)} SDK cases, {comparison["pass"]} matching, {comparison["fail"]} differing')

