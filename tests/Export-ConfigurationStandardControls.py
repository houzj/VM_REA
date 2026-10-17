"""Read-only native resource audit for independent standard control reconstruction."""
from pathlib import Path
import hashlib,json,xml.etree.ElementTree as E
D=Path('D:/MyWork/REA逆向测试/analysis/decompiled');W=Path.cwd()
def record(p):
 b=p.read_bytes();return dict(path=str(p),sha256=hashlib.sha256(b).hexdigest())
sources={
 'templates':D/'Apps.XmlUI/styles/paramusercontroldatatemplate.xaml',
 'leftRight':D/'Apps.XmlUI/Apps.XmlUI.ParamTab/LeftRightControl.xaml',
 'leftRightCode':D/'Apps.XmlUI/Apps.XmlUI.ParamTab/LeftRightControl.xaml.cs',
 'numeric':D/'VM.PcUI/HikPcUI.ListView/NumericControl.xaml',
 'numericCode':D/'VM.PcUI/HikPcUI.ListView/NumericControl.xaml.cs',
 'rangeRule':D/'VM.PcUI/HikPcUI.ListView/RangeRule.cs',
 'arrows':D/'VM.PcUI/controls/commomdictionary.xaml',
 'radio':D/'Apps.Style/radiobuttonstyle/radiobuttonstyle.xaml',
 'radioModel':D/'Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/RadioSelecter.cs',
 'selector':D/'Apps.Style/comboboxstyle/listviewcomboboxstyle.xaml',
 'buttons':D/'Apps.Style/buttonstyle/bottombuttonstyle.xaml',
 'colors':D/'Apps.ColorFun/colorstyle.xaml',
 'colorsOrange':D/'Apps.ColorFun/colorstyle1.xaml',
 'floatSlider':D/'VM.PcUI/HikPcUI.ListView/SliderArrangeFloat.xaml',
 'floatSliderCode':D/'VM.PcUI/HikPcUI.ListView/SliderArrangeFloat.xaml.cs',
 'editLink':D/'Apps.Style/buttonstyle/togglebuttonstyle.xaml',
 'normalButtons':D/'Apps.Style/buttonstyle/normalbuttonstyle.xaml',
 'stringModel':D/'Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/String.cs',
 'expandModel':D/'Apps.XmlParser.CapabilityAudit/Apps.XmlParser.ParamTab/Expand.cs',
 'sliderResources':D/'VM.PcUI/HikPcUI.ListView/SliderArrangeDictionary.xaml'}
claims=[('templates','DataTemplate x:Key="Command_sub"','Command uses LeftRight32 with inline BottomButton24 auto width/padding4 and state Invoke'),('templates','DataTemplate x:Key="Button_ResetAlgoParams"','Reset uses icon-only native ResetBtnStyle and state reset command, source footer visibility handled shell'),('expandModel','item.IsDeleteSubscribeA = value;','Expand changes subscription deletion flags before triggers; user action is model-only, not native write/run'),('stringModel','value.ByteCount() > MaxLen','String UTF8 MaxLen rejects oversized edit, Length independently limits TextBox characters'),('rangeRule','if (((string)value).Length > 0)','Empty numeric input zero; nonfinite/illegal/out-of-range typed edits rejected; independent red border and tooltip rather than native WPF Binding ValidationError adorner'),('templates','DataTemplate x:Key="Boolean"','Boolean uses Boolean_sub with original swith_off/on assets, LeftRightControl Height32'),('leftRightCode','new PropertyMetadata(89.0)','Default left label column89'),('numeric','ColumnDefinition Width="20"','20px arrow column, RepeatButtons20x15, border1 radius2 margin0,2'),('numericCode','ValueText.IsFocused','Mouse wheel alters values only while focused and writable'),('numericCode','Value + Increment < num','Step clamps to declared maximum/minimum'),('radio','ColumnDefinition Width="24"','24px native PNG indicators, label margin3, fontsize14 and height32'),('radioModel','return 165;','English RadioSelecter left width165; other89'),('templates','EnumEntrysA[1].DisplayNameALang','RadioSelecter original explicit two choices'),('selector','Height" Value="30"','Enumeration selector height30 and original dropdown/hover templates'),('buttons','Height" Value="24"','Bottom buttons height24, width/minwidth66, fontsize12'),('templates','ContinuousExecuteBottomCommand','Run/continuous hidden during continuous, disabled procedure running, hidden RunEnable=false; Stop inverse continuous'),('floatSliderCode','new PropertyMetadata(true, NeedSliderChanged)','Native Float NeedSlider=true default; independent Popup opens on text focus/mouse and commits on close or slider release')]
for key,needle,meaning in claims:
 lines=sources[key].read_text(encoding='utf-8-sig').splitlines(); hits=[i+1 for i,l in enumerate(lines) if needle in l];assert hits,(key,needle)
 claims[claims.index((key,needle,meaning))]=dict(source=key,lines=hits,evidence=needle,meaning=meaning)
assets=[]
for name in ['swith_on.png','swith_off.png','check-box_on.png','check-box_off.png','combo_ic_top_normal.png','combo_ic_top_click.png','combo_ic_triangle_normal.png','combo_ic_triangle_click.png']:
 src=W/'src/VisionPlugins.ImageSource/Resources'/name;dst=W/'src/VisionPluginContracts/Resources'/name
 assert src.read_bytes()==dst.read_bytes();assets.append(dict(name=name,trackedSource=str(src.relative_to(W)),sha256=record(dst)['sha256']))
for name in ['ic_btn_initialize_normal.png','ic_btn_initialize_click.png','btn_高级参数_downarrow.png','btn_高级参数_uparrow.png','ic_btn_link切换_normal.png','ic_btn_link切换_click.png','ic_btn_link切换_disable.png','ic_btn_edit切换_normal.png','ic_btn_edit切换_click.png','ic_btn_edit切换_disable.png']:
 src=D/'Apps.Style/resources'/name;dst=W/'src/VisionPluginContracts/Resources'/name
 assert src.read_bytes()==dst.read_bytes();assets.append(dict(name=name,originalSource=str(src),sha256=record(dst)['sha256']))
out=dict(scope='Shared standard control leaf reconstruction; no per-plugin Window',sources={k:record(v) for k,v in sources.items()},claims=claims,assets=assets,
 implemented=['Boolean switch PNG','Integer/Float dedicated numeric spinner, original slider resource closure, Popup, finite bounded commit','Enumeration native style/dropdown','RadioSelecter native two choices','BottomCommand/ExecuteOnce/Continuous/Stop source visibility gates','Command/Reset native inline/icon styles','Expand model-only boolean and original expand/collapse style','String source LostFocus/max characters/UTF8 MaxLen with typed subscription editor'],
 remaining=['Numeric between/beside/clipped multi-thumb variants outside minimum standard leaf scope','Numeric subscription uses shared typed selector/service; missing service remains explicitly Unsupported','Complex ButtonSelecter owned by subscription collection. ColorConfig/FloatConfig/StringConfig require object-property bridge; remain Unsupported. No invented FilterLink template.','Original full desktop pixel comparison not established'],
 independentBoundary='Original resources and source establish metadata/geometry; independent State.CommitUser/Invoke and WPF controls verified separately. Passing checks do not establish complete parity.')
(W/'docs/plugins/configuration-standard-controls-evidence.json').write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8')
print(f'{len(claims)} source claims, {len(assets)} byte-identical source assets')
