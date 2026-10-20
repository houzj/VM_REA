"""Read installed declarations without executing original modules."""
import base64,hashlib,json,pathlib,xml.etree.ElementTree as ET
root=pathlib.Path(__file__).resolve().parents[2]
installed=pathlib.Path('D:/Program Files/VisionMaster4.4.0/Applications/Module(sp)/x64/Calibration/IMVSMapCalibModu')
def digest(p):return dict(path=str(p),bytes=p.stat().st_size,sha256=hashlib.sha256(p.read_bytes()).hexdigest())
tab=ET.parse(installed/'IMVSMapCalibModuAlgorithmTab.xml').getroot()
main=ET.parse(installed/'IMVSMapCalibModu.xml').getroot()
controls=[]
for items in tab.findall('.//Category/Items'):
 for e in items:
  controls.append(dict(template=e.tag,name=e.get('Name'),properties={c.tag:c.text for c in e if not len(c)},xml=ET.tostring(e,encoding='unicode')))
report=dict(tool='IMVSMapCalibModu',status='source-and-recorded-sdk-runtime-verified-broader-boundaries-open',reaManagedEvidence='ev_a37ff2fa04c0772765574a1048effb17c0b455e28644362c3de78712fab7a94d',sources=[digest(p) for p in sorted(installed.iterdir()) if p.is_file()],controls=controls,categories={c.get('Name'):[dict(tag=e.tag,name=e.get('Name'),xml=ET.tostring(e,encoding='unicode')) for e in c.findall('./Items/*')] for c in main.findall('./Categorys/Category')},unknown=['native two-file composition and XML compatibility','image detection, paired ROI and teach semantics','file refresh/cache and failure ordering','original desktop visual parity'])
report['sdkSources']=[digest(pathlib.Path('D:/Program Files/VisionMaster4.4.0/MVDAlgorithmSDK')/p) for p in ['Includes/Algorithms/MapCalibCpp.h','Runtime/x64/MVDMapCalibCpp.dll','Runtime/x64/MVD_Algorithm.dll']]
report['imageFixtures']=[digest(root/'analysis/full-audit/n-image-calibration/solver-fixture'/p) for p in ['frame-0.bmp','frame-4.bmp']]
native=root/'analysis/map-calibration/native'
if (native/'results.jsonl').exists():report['sdkRuntime']=[json.loads(s) for s in (native/'results.jsonl').read_text(encoding='utf-8-sig').splitlines()]
if (native/'image-results.jsonl').exists():report['imageSdkRuntime']=[json.loads(s) for s in (native/'image-results.jsonl').read_text(encoding='utf-8-sig').splitlines()]
if (native/'module-file-results.jsonl').exists():report['moduleFileRuntime']=[json.loads(s) for s in (native/'module-file-results.jsonl').read_text(encoding='utf-8-sig').splitlines()]
if (native/'module-runtime-results.jsonl').exists():report['moduleProcessRuntime']=[json.loads(s) for s in (native/'module-runtime-results.jsonl').read_text(encoding='utf-8-sig').splitlines()]
parser=root/'analysis/map-calibration/parser/model/probe.json'
if parser.exists():report['originalParserRuntime']=dict(**digest(parser),phases=json.loads(parser.read_text(encoding='utf-8-sig')),authority='original-parser/synthetic-parameter-host; no original desktop capture')
for key,filename in [('sessionVerification','session-checks.json'),('pluginVerification','plugin-checks.json'),('configurationVerification','configuration/checks.json'),('imageFileVerification','image-file-checks.json'),('roiRuntimeVerification','roi-runtime/checks.json'),('regression','regression/results.json'),('localization','localization.json')]:
 p=root/'analysis/map-calibration'/filename
 if p.exists():
  value=json.loads(p.read_text(encoding='utf-8-sig'))
  report[key]=dict(**digest(p),complete=value.get('complete'),cases=value.get('cases')) if key=='productionImageConclusions' else value
trace=root/'analysis/map-calibration/image-fit-trace/fixed-trace.jsonl'
if trace.exists():
 records=[json.loads(s) for s in trace.read_text(encoding='utf-8-sig').splitlines()]
 for item in records:
  if item['phase']=='fixed-fit-input':
   capture=trace.parent/f'capture-{item["sequence"]:03d}.bin'
   if capture.exists():item['capture']=dict(**digest(capture),base64=base64.b64encode(capture.read_bytes()).decode('ascii'),layout='count float2 source followed by count float2 destination; little endian')
 report['ownedProcessFixedFitTrace']=records
trace=root/'analysis/map-calibration/image-fit-weight1-trace/fixed-trace.jsonl'
if trace.exists():
 records=[json.loads(s) for s in trace.read_text(encoding='utf-8-sig').splitlines()]
 for item in records:
  if item['phase']=='fixed-fit-input':
   capture=trace.parent/f'capture-{item["sequence"]:03d}.bin'
   if capture.exists():item['capture']=dict(**digest(capture),base64=base64.b64encode(capture.read_bytes()).decode('ascii'),layout='count float2 source followed by count float2 destination; little endian')
 report['ownedProcessWeight1FitTrace']=records
trace=root/'analysis/map-calibration/image-fit-weight2-trace/fixed-trace.jsonl'
if trace.exists():
 records=[json.loads(s) for s in trace.read_text(encoding='utf-8-sig').splitlines()]
 for item in records:
  if item['phase']=='fixed-fit-input':
   capture=trace.parent/f'capture-{item["sequence"]:03d}.bin'
   if capture.exists():item['capture']=dict(**digest(capture),base64=base64.b64encode(capture.read_bytes()).decode('ascii'),layout='count float2 source followed by count float2 destination; little endian')
 report['ownedProcessWeight2FitTrace']=records
for key,folder in [('moduleImageRuntime','image-runtime'),('moduleImageRuntimeCurrent','image-runtime-source-grid'),('moduleImageWriter','image-file'),('sdkImageRoi','image-roi'),('sdkImageRoiMask','image-roi-mask')]:
 p=root/'analysis/map-calibration'/folder/'image-results.jsonl'
 if p.exists():report[key]=[json.loads(s) for s in p.read_text(encoding='utf-8-sig').splitlines()]
report['moduleImageExports']=[dict(**digest(p),xml=p.read_text(encoding='utf-8')) for p in sorted((root/'analysis/map-calibration/image-file').glob('image-writer-*.xml'))]
p=root/'analysis/map-calibration/image-runtime-source-grid/normal-process.xml'
if p.exists():report['moduleNormalProcessExport']=dict(**digest(p),xml=p.read_text(encoding='utf-8'))
p=root/'analysis/map-calibration/image-runtime/teach-process.xml'
if p.exists():report['moduleProcessTeachExport']=dict(**digest(p),xml=p.read_text(encoding='utf-8'))
for key,filename in [('imageFitDiagnostics','image-fit/diagnostics.json'),('nativeConstants','native/constants.json'),('imagePostprocessVerification','image-fit/postprocess-checks.json'),('imageFitInputComparison','image-fit/input-comparison.json')]:
 p=root/'analysis/map-calibration'/filename
 if p.exists():
  value=json.loads(p.read_text(encoding='utf-8-sig'))
  report[key]=dict(**digest(p),complete=value.get('complete'),cases=value.get('cases')) if key=='productionImageConclusions' else value
for key,filename in [('imageInputPixelComparison','image-input/comparison.json'),('subpixelHypothesisDiagnostic','image-input/subpixel-hypothesis.json'),('productionImageExecutionBeforeTagFix','image-execution-current/differences.json'),('productionImageExecutionBeforeSourceGrid','image-execution-quadratic/differences.json'),('productionImageExecutionBeforeLegacyIsolation','image-execution-source-grid/differences.json'),('productionImageExecution','image-execution-legacy-isolated/differences.json'),('productionImageConclusions','image-execution-legacy-isolated/conclusions.json'),('actualProcessXmlVerification','image-execution-legacy-isolated/process-xml-comparison.json'),('managedTagBoundary','detector-boundary/rejection.json'),('nativeDetectorStageComparison','image-detector-stage-trace/comparison.json'),('nativeCandidateSelectionComparison','image-grid-trace/selection-comparison.json'),('quadraticSubpixelKernelVerification','subpixel-kernel/all-cases.json')]:
 p=root/'analysis/map-calibration'/filename
 if p.exists():
  value=json.loads(p.read_text(encoding='utf-8-sig'))
  report[key]=dict(**digest(p),complete=value.get('complete'),cases=value.get('cases')) if key=='productionImageConclusions' else value
for key,filename in [('gaussianStageVerification','gaussian-kernel/prototype.json'),('candidateStageVerification','hessian-candidates/prototype.json'),('descriptorStageBoundary','image-candidate-descriptor-trace/descriptor-summary.json'),('descriptorConvolutionVerification','image-convolution-trace/comparison.json'),('descriptorEigenVerification','image-eigen-trace/comparison.json'),('productionDescriptorVerification','descriptor-kernel/diagnostic.json'),('productionFullCandidateVerification','candidate-chain/median.json'),('gridOrderNativeDiagnostic','grid-permutations/comparison.json'),('gridNeighborVerification','grid-neighbor/comparison.json'),('gridNeighborOddVerification','grid-neighbor-odd/comparison.json'),('gridSeedVerification','grid-seeds/comparison.json'),('gridScoreVerification','grid-score/comparison.json'),('gridPaddingVerification','grid-padding/comparison.json'),('gridAssignmentVerification','grid-assignment/comparison.json'),('gridExpansionVerification','grid-expansion/comparison.json'),('gridPruningVerification','grid-pruning/comparison.json'),('gridSelectionVerification','grid-selection/comparison.json'),('gridPyramidVerification','grid-pyramid/comparison.json'),('gridPyramidBoundaryVerification','image-grid-boundary-trace/boundary-comparison.json'),('gridRoiVerification','grid-roi/comparison.json'),('sourceRecognitionVerification','source-recognition/comparison.json'),('sourceRecognitionFilter2Verification','source-recognition/filter2-comparison.json'),('gridSelectionFilter2Verification','grid-selection/filter2-comparison.json'),('productionImageFilter2Verification','image-execution-filter2-legacy-isolated/differences.json'),('legacyIsolationRegression','legacy-isolation/checks.json')]:
 p=root/'analysis/map-calibration'/filename
 if p.exists():report[key]=json.loads(p.read_text(encoding='utf-8-sig'))
p=root/'analysis/map-calibration/image-tag-trace/tag-trace.jsonl'
if p.exists():report['nativeSelectedTagTrace']=[json.loads(line) for line in p.read_text(encoding='utf-8-sig').splitlines() if line.strip()]
report['unknown']=['independent code-opening proposals and payload/anchor logic; source candidate/grid parity proven only for recorded fixtures','managed safe initialization/capacity rules do not reproduce arbitrary native allocator contexts','projective/rotated fallback retains prior independent compatibility behavior','original desktop visual comparison','POINT indexed subscription semantics and native ROI persistence ABI']
if (native/'module-export.xml').exists():report['moduleFileExport']=dict(**digest(native/'module-export.xml'),xml=(native/'module-export.xml').read_text(encoding='utf-8'))
if (root/'analysis/map-calibration/file-checks.json').exists():report['fileVerification']=json.loads((root/'analysis/map-calibration/file-checks.json').read_text(encoding='utf-8'))
ui=pathlib.Path('D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlUI/styles/paramusercontroldatatemplate.xaml')
if ui.exists():report['originalPathAndCommandTemplate']=dict(**digest(ui),openPathLines=[671,704],commandLines=[1309,1322],openPathHeight=34,commandHeight=32,sourceConfirmed=True,desktopCompared=False)
report['productFiles']=[digest(p) for p in sorted((root/'src/VisionPlugins.MapCalibration').glob('*.cs'))]
report['sharedRecognitionSources']=[digest(root/'src/VisionPlugins.Calibration'/name) for name in ['NImageDetectedBoard.cs','NImageBoardDetector.cs','NImageQuadraticCornerRefiner.cs','NImageSourceCandidateDescriptor.cs','NImageSourceCandidateDetector.cs','NImageSourceGridNeighbor.cs','NImageSourceGridSeeds.cs','NImageSourceGridScoring.cs','NImageSourceGridPadding.cs','NImageSourceGridAssignment.cs','NImageSourceGridExpansion.cs','NImageSourceGridPruning.cs','NImageSourceGridSelection.cs','NImageSourceGridPyramid.cs','NImageSourceLatticeAdapter.cs']]
report['productResources']=[digest(p) for p in sorted((root/'src/VisionPlugins.MapCalibration/Resources').iterdir()) if p.is_file()]
report['testSources']=[digest(p) for folder in sorted((root/'tests').glob('MapCalibration.*')) for p in sorted(folder.iterdir()) if p.is_file()]
if (root/'analysis/map-calibration/file-solver-checks.json').exists():report['fileSolverVerification']=json.loads((root/'analysis/map-calibration/file-solver-checks.json').read_text(encoding='utf-8'))
report['moduleCliSources']=[digest(p) for p in sorted((root/'analysis/map-calibration/ghidra/module').glob('*.c'))]
report['wrapperCliSources']=[digest(p) for p in sorted((root/'analysis/map-calibration/ghidra/wrapper').glob('*.c'))]
report['imageCoreCliSources']=[digest(p) for p in sorted((root/'analysis/map-calibration/ghidra/core').glob('*.c'))]
report['exportedFiles']=[dict(**digest(p),xml=p.read_text(encoding='utf-8')) for p in sorted(native.glob('result-*.xml'))]
out=root/'docs/plugins/map-calibration-evidence.json';out.write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(f'{len(controls)} controls; {len(report["sources"])} source files')

















