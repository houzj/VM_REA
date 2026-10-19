"""Read retained logs and source only; never starts WPF, target code or input APIs."""
import argparse, hashlib, json, re
from pathlib import Path

p = argparse.ArgumentParser()
p.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[3])
p.add_argument('--output', type=Path, required=True)
a = p.parse_args()
paths = [
 'analysis/full-audit/framework-retirement-20261019/validation-shell.log',
 'analysis/full-audit/framework-retirement-20261019/validation-shell-control-events-complete.log',
 'tests/VisionEditor.ShellChecks/Program.cs',
 'src/VisionEditor/Styles/ShellFileMenu.xaml',
 'src/VisionEditor/MainWindow.ShellFileMenu.cs',
 'src/VisionEditor/MainWindow.ShellRecentSolutions.cs',
 'analysis/decompiled/Apps.Style/menustyle/menustyle.xaml',
 'analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml',
 'analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs',
 'docs/MF01-HELP-DOC-SOURCE-AUDIT.md',
]
records=[]
texts={}
for rel in paths:
 data=(a.root/rel).read_bytes(); text=data.decode('utf-8-sig')
 texts[rel]=text
 anchors=[{'line':i,'text':line.strip()} for i,line in enumerate(text.splitlines(),1)
          if re.search(r'WPF menu input opens|LIMITATION:|GetRecentlySolutions|OpenRecentlySolution|private static bool OpenRecentSolution|IsSubmenuOpen|ItemClickCommandExecute|Keyboard.Focus|target.RaiseEvent|PreviewMouseLeftButton|GetRecentlySolutions|Shell command and input checks passed|SubmenuOpened',line)]
 records.append({'path':rel,'sha256':hashlib.sha256(data).hexdigest(),'anchors':anchors})
failed=texts[paths[0]]
fixture=texts[paths[2]]
assert 'file=False, recent=False' in failed
assert 'recentLoaded=False, recentVisible=False' in failed
assert 'target.RaiseEvent(keyEvent)' in fixture
assert 'controlEventsOnly' in fixture
assert 'IsSubmenuOpen' in texts[paths[3]] and 'IsSubmenuOpen' in texts[paths[6]]
result={'kind':'static evidence reconciliation, not runtime acceptance','inputs':records,
 'defaultFailure':next(line for line in failed.splitlines() if 'WPF menu input opens' in line),
 'recordedPassesBeforeFailure':sum(line.startswith('PASS:') for line in failed.splitlines()),
 'controlEventPasses':sum(line.startswith('PASS:') for line in texts[paths[1]].splitlines()),
 'nativeInputInjected':False,'desktopIdentityInRetainedFailureLog':False,
 'productDefectProven':False,'sandboxDesktopCauseProven':False,
 'acceptanceStatus':'A08.2 remains failed; real input desktop/DPI/native same-state matrix unverified'}
a.output.parent.mkdir(parents=True,exist_ok=True)
a.output.write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
print(json.dumps({k:v for k,v in result.items() if k not in ['inputs','defaultFailure']},ensure_ascii=False))
