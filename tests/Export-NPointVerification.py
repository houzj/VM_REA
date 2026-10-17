"""Index final no-device checks and retained numerical differences; no parity assertion."""
import hashlib,json,re
from pathlib import Path
root=Path(__file__).resolve().parents[1];base=root/"analysis/full-audit/n-point-calibration"
counts={"VisionFramework.PersistenceChecks":23,"VisionFramework.IntegrationChecks":10,"VisionFramework.ExecutionChecks":38,"VisionEditor.Core.Checks":115,"VisionEditor.WpfChecks":262,"ImageSource.PluginChecks":767,"ImageAcquisition.PluginChecks":327,"ImageBuffer.PluginChecks":1623,"Light.PluginChecks":11836,"SolutionInspector.Checks":17,"VisionCore.Checks":29,"VisionIO.Checks":14,"VisionModules.Checks":22,"VisionPluginDefinitions.Checks":10,"VisionProcedure.Checks":20,"VisionRouting.Checks":20,"VisionWorkflow.CallChecks":34,"VisionWorkflow.Checks":13,"VisionWorkflow.DataChecks":31,"VmRouting.Checks":64}
def identity(path):return {"path":str(path.relative_to(root)),"sha256":hashlib.sha256(path.read_bytes()).hexdigest()}
checks=[]
for name,count in counts.items():
 log=base/f"regression-{name}.log";text=log.read_text(encoding="utf-8-sig");assert re.search(rf"\b{count}\b",text) and "passed" in text.lower() or "PASS" in text
 checks.append({"name":name,"checks":count,"log":identity(log)})
log=base/"plugin-checks-progress.log";assert "5747 checks passed" in log.read_text(encoding="utf-8-sig");checks.append({"name":"NPointCalibration.PluginChecks","checks":5747,"log":identity(log)})
solver=json.loads((root/"docs/plugins/n-point-solver-verification.json").read_text(encoding="utf-8"));assert solver["failures"]==0;checks.append({"name":"NPointSolver.Checks","checks":solver["checks"],"log":identity(base/"solver-final.log"),"documentedDifferingFields":len(solver["differences"]),"illConditionedOrTiedObservations":len(solver["conditioning"])})
paths=list((root/"src/VisionPlugins.Calibration").glob("NPoint*.cs"))+[root/"src/VisionPlugins.Calibration/VisionPlugins.Calibration.csproj",root/"src/VisionPlugins.Calibration/Resources/NPointDefinition.json"]+list((root/"tests/NPointCalibration.PluginChecks").glob("*.*"))+list((root/"tests/NPointSolver.Checks").glob("*.*"))
for directory in ["src/VisionFramework","src/VisionPluginContracts","src/VisionEditor"]: paths+=list((root/directory).glob("*.cs"))
report={"scope":"代码范围验证；原厂受控实例/SDK证据与独立宿主验证分开，不声称完整原生等价", "physicalDevicesTested":False,"build":{"warnings":0,"errors":0,"log":identity(base/"final-build.log")},"programCount":len(checks),"checkCount":sum(x["checks"] for x in checks),"checks":checks,"sourceIdentity":[identity(p) for p in sorted(set(paths)) if p.is_file()],"supplementalFailure":{"name":"VisionUi.Checks tools screenshot","includedInPassingTotals":False,"reason":"离屏WinForms SplitContainer重绘产生GDI+ generic error；完整编译及262真实WPF检查通过，未掩盖截图失败","log":identity(base/"regression-VisionUi.Checks-retry.log")},"limitationsDocument":"docs/plugins/N-POINT-CALIBRATION.md"}
assert report["programCount"]==22 and report["checkCount"]==32088
(root/"docs/plugins/n-point-verification.json").write_text(json.dumps(report,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
print(f"Indexed {len(checks)} programs and {report['checkCount']} checks with explicit numerical/screenshot boundaries")
