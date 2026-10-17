"""Record original managed folder selector helpers; popup interaction remains unverified."""
import argparse
import hashlib
import json
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('--runtime', type=Path, default=Path('analysis/full-audit/image-source-runtime/run-8fdfdc1bf96c4561b6d877015860d013/probe.json'))
args = parser.parse_args()
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
original = Path('D:/MyWork/REA逆向测试/analysis/decompiled')
sources = [original / p for p in ['Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/OpenFolderDialog.cs', 'Apps.Style/buttonstyle/normalbuttonstyle.xaml', 'Apps.Style/resources/ic_文件路径_normal.png']]
records = [r for r in json.loads(args.runtime.read_text(encoding='utf-8-sig')) if r.get('phase', '').startswith('folder-')]
assert len(records) == 7
binary = Path('D:/Program Files/VisionMaster4.4.0/Applications/myLibs/Apps.XmlParser.dll')
result = {'schema': 1, 'runtimeSource': str(args.runtime), 'runtimeSha256': sha(args.runtime), 'binaryPath': str(binary), 'binarySha256': sha(binary), 'sources': [{'path': str(p), 'sha256': sha(p)} for p in sources], 'observations': records, 'scope': 'Original IsCorrectDir and IsHaveRWAccess methods invoked by reflection; local owned fixture only', 'limitations': 'Native picker popup, remote context and parameter/report/run callbacks are not runtime-verified; write probe implementation deliberately uses a unique DeleteOnClose file instead of the original fixed test.txt', 'completeParity': False}
Path('docs/plugins/image-source-folder-evidence.json').write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
print(f'Exported {len(records)} original folder helper observations')
