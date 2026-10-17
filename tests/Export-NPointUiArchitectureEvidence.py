"""Source-bound N-point UI architecture ledger; does not execute native software."""
import hashlib
import json
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
DECOMP = Path('D:/MyWork/REA逆向测试/analysis/decompiled')
INSTALL = Path('D:/Program Files/VisionMaster4.4.0/Applications')
PARSER = DECOMP / 'Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab'
CONTROL = ROOT / 'analysis/full-audit/n-point-calibration/managed/control'
XML = INSTALL / 'Module(sp)/x64/Calibration/IMVSNPointCalibModu/IMVSNPointCalibModuAlgorithmTab.xml'

def source(path, needles):
    data = path.read_bytes()
    lines = data.decode('utf-8-sig').splitlines()
    claims = []
    for needle in needles:
        matches = [{'line': i + 1, 'text': line.strip()} for i, line in enumerate(lines) if needle in line]
        if not matches:
            raise ValueError(f'{path}: missing {needle}')
        claims.append({'needle': needle, 'matches': matches})
    return {'path': str(path), 'sha256': hashlib.sha256(data).hexdigest(), 'bytes': len(data), 'claims': claims}

specs = [
 (DECOMP/'VM.Core/VM.Core/VmModule.cs', ['Params.XmlPath =']),
 (PARSER/'VmModuleExtern.cs', ['GetParamRoot', 'param.Data = new AlgorithmTabRoot']),
 (PARSER/'AlgorithmTabRoot.cs', ['CustomTabCHWidth = 420f', 'CustomTabENWidth = 680f', 'CustomTabHeight = 500f', 'BottomExcuteButtons', 'loadXml(path, ignoreUnknown: true)', 'DynamicSetProperty();']),
 (DECOMP/'VMControls.WPF/VMControls.WPF/ParamsConfigControlBase.xaml.cs', ['LoadModuleControlDLL', 'param.GetParamRoot()', 'ZHCustomTabWidth', 'ENCustomTabWidth']),
 (XML, ['<Tab Name=', '<EditDialog Name=', '<Param>IMVSNPointCalibModuControl.NPointList', '<DialogWidth>493', '<DialogHeight>700', '<SaveFileDialog', '<OpenFileForCalibDialog', '<!--', '<EnumEntry Name="Ransac"', '<Integer Name="DistThreshold"', '<Integer Name="SampleRatio"', '<GroupLinkItem Name="TextDisplayGroup"', '<BottomExcuteButtonsLeft>', '<BottomExcuteButtons>', '<NeedPopTip>true']),
 (CONTROL/'Apps.XmlParser.ParamTab/EditButton_Calib.cs', ['class EditButton_Calib : Integer', 'OpenDialog EditDialog', 'EditDialogA']),
 (CONTROL/'datatemplate/paramusercontroldatatemplate.xaml', ['Integer_Sub', 'EditDialogA.ClickCommand', 'edit_normal.png', 'LeftRightControl FrameworkElement.Height="32"']),
 (PARSER/'OpenDialog.cs', ['CurValueA == null', 'ActivatorWrap.CreateInstance', 'baseWindow.Width = DialogWidth', 'baseWindow.Height = DialogHeight', 'SetParentNode(this)', 'baseWindow.ShowDialog()']),
 (CONTROL/'IMVSNPointCalibModuControl/NPointList.xaml.cs', ['(Owner as Window).Height = 399.0', 'GetIntParamValue("CalibPointTotalNum")']),
 (PARSER/'OpenFileForCalibDialog.cs', ['new ButtonSelecter()', 'GetFrontFilterTreeOperation', 'SetModuleParamBindOperation', 'GetModuleParamBindOperation', 'UserData["@" + ComName]', 'CheckFileExists = false', 'CheckPathExists = true', 'CurValueA = null', 'Directory.Exists(text)', 'SetParamUnit()', 'PreExecuteRun()']),
 (PARSER/'SaveFileDialog.cs', ['DisplayType => "Command"', '((IUserStringData)GetModuleTagInfo())[Name] = filePath', 'FindBaseNodeByName("CalibPathName")', 'RestoreDirectory = true', 'EndsWith(array[0])', 'INISetDefaultOpenPath']),
 (PARSER/'BottomCommand.cs', ['public void ClearCalibPoint()', 'owner.Params["Clear"] = "0"', 'Lang_ClearCalibPointSuc', 'Lang_ClearCalibPointErr', 'public void SaveCalibPath()']),
 (PARSER/'NameValueItem.cs', ['trigger3.ReOnTrigger()', 'trigger4.OnTrigger(this)', 'public virtual void InniterBusiness()']),
 (PARSER/'Trigger.cs', ['setter.Operation.Working', 'setter.Operation.ReOperate()', 'if (Value != null)']),
 (PARSER/'BridgeItemBase.cs', ['BindingPathA => "BridgeObjA." + PropertyName + "A"', 'BrigdgeSetter.Operate(null)', 'override int GetParamUnit()', 'override int SetParamUnit()']),
 (PARSER/'StringConfig.cs', ['GetFrontAndSelfFilterWithoutTSTreeOperation', 'text.Insert(curSelectionStart, modulePathName)', 'BindingPathA']),
 (INSTALL/'Module(sp)/Group.xml', ['GroupInfoItem Name="TextDisplayGroup"', 'Show Text;Content', 'Show Text;FontSize', '<PropertyName>okcolor', '<PropertyName>ngcolor', '<PropertyName>opacity']),
 (CONTROL/'Apps.XmlParser.ParamTab/HidePhysicInfoOperation.cs', ['CalibPointGet', 'CalibPointInput', 'WorldRotateAngle', 'PhyPointX', 'PhyPointY']),
 (ROOT/'src/VisionPlugins.Calibration/NPointPlugin.cs', ['new NPointWindow(context,this)']),
 (ROOT/'src/VisionPlugins.Calibration/NPointWindow.cs', ['连续执行', 'tabs.Items.Clear()', 'Header="结果显示"', 'WeightFun']),
]

sources = [source(p, needles) for p, needles in specs]
tree = ET.parse(XML).getroot()
report = {
 'scope': 'N-point UI architecture, static source/resource audit against unified baseline 0f3a1d4',
 'evidenceKind': 'original-static-source-and-resources plus independent-implementation-audit',
 'nativeExecutionPerformed': False, 'deviceTestsPerformed': False, 'desktopPixelParityEstablished': False,
 'sources': sources,
 'originalTabs': [t.attrib for t in tree.findall('./Tabs/Tab')],
 'originalBottomLeft': [{'type': e.tag, **e.attrib} for e in tree.findall('./BottomExcuteButtonsLeft/*')],
 'originalBottomRight': [{'type': e.tag, **e.attrib} for e in tree.findall('./BottomExcuteButtons/*')],
 'effectiveWeightFunctions': [{'name':e.get('Name'),'value':e.findtext('Value')} for e in tree.findall('.//Enumeration[@Name="WeightFun"]/EnumEntrys/EnumEntry')],
 'weightFunctionCommentWarning': 'Ransac, DistThreshold and SampleRatio text exists in commented XML. Raw grep is not proof of an active control or supported WeightFun=4.',
 'findings': [
  {'id':'NPUI-001','kind':'original-static-source','finding':'VmModule XML path -> cached AlgorithmTabRoot -> module control DLL/resources -> common ParamsConfigControlBase/ParamsTabUserControl; NPoint independent window currently bypasses this model.'},
  {'id':'NPUI-002','kind':'original-static-source','finding':'Chinese ordinary model defaults width420 height500; English width680. N-point XML does not override dimensions. DPI scaling and desktop outer chrome require separate runtime evidence.'},
  {'id':'NPUI-003','kind':'original-static-source','finding':'RotPointTotalNum is integer+inline edit icon, whose nested OpenDialog owns dynamic NPointList. XML493x700 is overridden to height399 by original control Loaded; final393/399 differences must not be guessed.'},
  {'id':'NPUI-004','kind':'original-static-source','finding':'ResultShow links TextDisplayGroup object-property bridges, not execution output text. Color/Float/StringConfig do not use algorithm parameter Get/Set.'},
  {'id':'NPUI-005','kind':'original-static-source','finding':'Bottom strip is Reset on left; ClearPoint with post-action success/error notification, SaveFile and ExecuteOnce on right. NeedPopTip is not a yes/no prompt.'},
  {'id':'NPUI-006','kind':'original-static-source','finding':'SaveFileDialog uses module user-string command then CalibPathName readback; OpenFileForCalibDialog allows missing file with existing parent and supports dynamic subscription submodel.'},
  {'id':'NPUI-007','kind':'independent-implementation-audit','finding':'NPointWindow rebuilds handwritten pages and controls and substitutes an output TextBlock for display configuration. Ransac is commented out in original XML: effective WeightFun has three values, not four.'},
 ],
 'priorControlledRuntime': {'report':'docs/plugins/n-point-ui-evidence.json','scope':'Original managed controls/XML parser driven by synthetic model, not native module nor actual VM desktop. Reused evidence, not rerun here.'},
 'unknowns': ['Actual original desktop pixel differential for all branches', 'Full native display-object lifetime and formula/feature update equivalence', 'Remote directory/FTP paths and error delivery', 'Full original native algorithm/calibration session behavior'],
}
out = ROOT/'docs/plugins/n-point-ui-architecture-evidence.json'
out.write_text(json.dumps(report, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')
print(f'{len(sources)} source/resource identities -> {out}')
