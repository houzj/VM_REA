"""Export controlled original tag/codeword records; no original runtime invoked here."""
import argparse,json,hashlib
from pathlib import Path
root=Path(__file__).resolve().parents[1]
parser=argparse.ArgumentParser();parser.add_argument("code_run",type=Path);parser.add_argument("tag_run",type=Path);args=parser.parse_args()
def load(run):
 run=run.resolve()
 if not run.is_relative_to(root):raise ValueError("Keep evidence in current worktree")
 process=json.loads((run/"process.json").read_text(encoding="utf-8-sig"))
 if process["exitCode"]:raise ValueError("Original probe failed")
 rows=[json.loads(line) for line in (run/"probe.jsonl").read_text(encoding="utf-8-sig").splitlines() if line.strip()]
 return run,process,rows
run,process,rows=load(args.code_run);cases=[row for row in rows if "input" in row]
assert len(cases)==40 and all(row["code"]==0 and row["output"]==row["encoded"] for row in cases)
report={"authority":"owned-original-internal-function-replay","function":"MVD_Algorithm.dll RVA0x8b0260","scope":"synthetic RS(24,6), four payloads, zero through nine corrupt bytes","devices":False,"coreUnloadSkipped":True,"shutdown":"Flush then terminate own probe; preliminary direct core unload hung; not lifecycle parity","run":str(run.relative_to(root)),"process":process,"provenance":json.loads((run/"provenance.json").read_text(encoding="utf-8-sig")),"cases":cases}
tag_run,tag_process,tag_rows=load(args.tag_run);tag=next(row for row in tag_rows if row.get("phase")=="result")
assert tag["exception"]==0 and tag["count"]==1 and tag["flag"]==1
fixture=root/"tests/NImageCalibration.PluginChecks/Fixtures/hik-i-help-derived.png"
report["realPixelTag"]={"run":str(tag_run.relative_to(root)),"process":tag_process,"manualDiagnosticQuad":next(row for row in tag_rows if "manualQuad" in row)["manualQuad"],"fixture":str(fixture.relative_to(root)),"fixtureSha256":hashlib.sha256(fixture.read_bytes()).hexdigest(),"original":tag,"rawPixelSha256":hashlib.sha256((root/"analysis/full-audit/n-image-calibration/code-tag/help-gray.raw").read_bytes()).hexdigest(),"scope":"Same real pixels and manual four-circle quad; automatic location and all-board identity are separate work","provenance":json.loads((tag_run/"provenance.json").read_text(encoding="utf-8-sig"))}
(root/"docs/plugins/n-image-code-evidence.json").write_text(json.dumps(report,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
print("40 original codewords and one real-pixel tag indexed")
