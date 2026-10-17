"""Index the controlled original constructor/parameter/exported-method replay."""
import argparse, hashlib, json
from pathlib import Path
parser=argparse.ArgumentParser()
parser.add_argument("run",type=Path)
args=parser.parse_args()
root=Path(__file__).resolve().parents[1]
run=args.run.resolve()
if not run.is_relative_to(root): raise SystemExit("Evidence must be in this worktree")
process=json.loads((run/"process.json").read_text(encoding="utf-8-sig"))
if process["exitCode"]: raise SystemExit("Original replay did not finish")
events=[json.loads(line) for line in (run/"probe.jsonl").read_text(encoding="utf-8-sig").splitlines() if line.strip()]
files=[]
for name in ["probe.jsonl","module-calibration.xml","build.log","process.json"]:
 p=run/name
 if p.exists(): files.append({"path":str(p.relative_to(root)),"sha256":hashlib.sha256(p.read_bytes()).hexdigest()})
report={"authority":"original-exported-methods-in-owned-instance","platformInit":False,"physicalDevicesTested":False,"scope":"Original constructor defaults, GetParam/SetParam ranges, manual ProcessAlg and module XML; no full desktop/Platform parity claim", "run":str(run.relative_to(root)),"process":process,"provenance":json.loads((run/"provenance.json").read_text(encoding="utf-8-sig")),"files":files,"events":events}
(root/"docs/plugins/n-point-module-evidence.json").write_text(json.dumps(report,ensure_ascii=False,indent=2)+"\n",encoding="utf-8")
print(f"Indexed {len(events)} original exported-method records")
