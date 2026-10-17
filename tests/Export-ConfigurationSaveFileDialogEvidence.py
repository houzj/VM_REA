"""Read-only original SaveFileDialog model contract evidence; no picker execution."""
from pathlib import Path
import hashlib,json,xml.etree.ElementTree as E
W=Path.cwd();p=Path('D:/MyWork/REA逆向测试/analysis/decompiled/Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/SaveFileDialog.cs');b=p.read_bytes();lines=b.decode('utf-8-sig').splitlines()
claims=[]
for needle,meaning in [
('public override bool NeedInit => false;','No generic parameter initialization for this model'),
('public override string DisplayType => "Command";','Template alias does not mean CommandSetter execution semantics'),
('DoSaveFile();','Own ClickCommand invokes DoSaveFile, not base CommandSetter'),
('((IUserStringData)GetModuleTagInfo())[Name] = filePath;','Successful selection writes module-tag user string keyed by model Name; plugin-specific save action is below this bridge'),
('tab.FindBaseNodeByName("CalibPathName")','Refresh peer CalibPathName GetParamUnit after native user-string write'),
('saveFileDialog.Filter = FileOption.FilterName;','Save picker takes declared filter exactly'),
('saveFileDialog.FilterIndex = 1;','Local picker filterindex1'),
('saveFileDialog.RestoreDirectory = true;','Local picker restore directory enabled'),
('INIGetDefaultOpenPath(vmModule?.ModuleDir)','Initial local directory uses per-module stored default open path'),
('saveFileDialog.ShowDialog() != true','Cancellation returns before write, history or success notification'),
('array.Count() != 2','Require exactly two pipe-separated filter components'),
('saveFileDialog.FileName.EndsWith(array[0])','Local chosen path must end with filter first component; invalid path reports save failure/path-too-long'),
('SaveFile(saveFileDialog.FileName);','Write module user-string before remembering path or showing success'),
('INISetDefaultOpenPath(Path.GetDirectoryName(saveFileDialog.FileName)','Only successful write remembers chosen local directory'),
('catch (VmException ex)','Native failure notification includes errorcode resolver; managed exceptions report exception message'),
('DirectoryBrowser.SaveFileDialog','Separate remote picker branch uses remote path history; no claim implemented here'),
('public override int GetParamUnit()','Model GetParamUnit/SetParamUnit return0 and do not read/write execution CurValue'),
('SaveTypeA == "CalibPath"','PathInit/PathSave currently empty conditional bodies; SaveType cannot be used to invent an action')]:
 hits=[i+1 for i,l in enumerate(lines) if needle in l];assert hits,needle;claims.append(dict(lines=hits,text=needle,meaning=meaning))
r=E.parse(W/'tests/VisionConfiguration.ModelChecks/Fixtures/IMVSNPointCalibModuAlgorithmTab.xml').getroot();e=next(e for e in r.iter('SaveFileDialog'))
out=dict(status='Source-confirmed shared model requirement; Command renderer exists, SaveFileDialog execution route not implemented',source=dict(path=str(p),sha256=hashlib.sha256(b).hexdigest()),claims=claims,npointInstance=dict(name=e.get('Name'),saveType=e.findtext('SaveType'),filter=e.findtext('FileOption/FilterName'),customVisible=e.findtext('CustomVisible'),accessMode=e.findtext('AccessMode')),
 requiredSharedContract=dict(picker=['SaveFile request, current per-module directory, exact source filter, FilterIndex1, RestoreDirectory true','Selected/Cancelled/Failed result and chosen path; no file write merely from opening picker'],modelDispatch=['Explicit SaveFileDialog kind, not arbitrary XML CLR reflection','Host routes selected path to plugin module-user-string action keyed by source model Name','After successful action refresh source CalibPathName; remember directory and show success; report native/managed errors with source message distinction'],missingInCurrentFramework=['IToolConfigurationFileService.Select currently has open-existing-file semantics (WpfConfigurationFileService uses OpenFileDialog); ConfigurationFileRequest has no save-operation discriminator or SaveFile picker contract','State.Invoke currently requires CommandSetter and diagnoses unsupported for SaveFileDialog','No shared named module-user-string save action + peer refresh/history transaction contract']),
 independentBoundary='This report neither opens a picker nor writes a calibration file. Renderer availability is reported separately from model behavior. Remote mode remains source-confirmed/unimplemented.')
(W/'docs/plugins/configuration-save-file-dialog-contract-evidence.json').write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8');print(f'{len(claims)} source-confirmed save-model claims')
