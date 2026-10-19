"""Finite menu-resource inventory; static source audit, never executes vendor code."""
import argparse, hashlib, json, re
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT=Path(__file__).resolve().parents[2]
SOURCE=ROOT/'analysis/decompiled/VisionMaster.Exe.Audit/menuitems.xml'
HANDLERS=ROOT/'analysis/decompiled/VisionMaster.Exe.Audit/VisionMaster.AppMainWindow/TitleMenueItemData.cs'
EXTERNAL=HANDLERS.with_name('ExternTitleMenueItemData.cs')
# status refers to current independent main entry, never to original VM equivalence.
MAP={
 'CreateNewSolution':('implemented-local','MainWindow.xaml.cs','void New('),
 'OpenSolution':('implemented-local','MainWindow.ShellOpenSolution.cs','OpenSolutionFromFileMenu('),
 'GetRecentlySolutions':('implemented-local','MainWindow.ShellRecentSolutions.cs','InitializeRecentSolutionsMenu('),
 'OpenSamples':('implemented-local','MainWindow.ShellSampleSelection.cs','OpenSampleSolution('),
 'SaveSolution':('implemented-local','MainWindow.ShellSaveSolution.cs','bool Save('),
 'SaveAsSolution':('implemented-local','MainWindow.ShellSaveSolution.cs','bool Save('),
 'Exit':('implemented-local','MainWindow.ShellFileMenu.cs','"退出", Close'),
 'ProcRunSet':('implemented-local','MainWindow.ShellRunMode.cs','OpenRunModeSettings('),
 'OnlineHelpDoc':('implemented-local','MainWindow.ShellHelp.cs','OpenOnlineHelpDocument('),
 'VerInfo':('implemented-local','MainWindow.ShellAbout.cs','ShowAbout('),
 'Undo':('implemented-local','MainWindow.ShellHistory.cs','UndoShell('),
 'Redo':('implemented-local','MainWindow.ShellHistory.cs','RedoShell('),
 'AreaLocking':('implemented-local','MainWindow.ShellLock.cs','RequestWorkAreaLock('),
 'AreaUnLocking':('implemented-local','MainWindow.xaml.cs','void ToggleLock('),
 'CameraSet':('partial-backend','MainWindow.xaml.cs','RefreshCameraProvider(true)'),
 'SigRun':('implemented-local','MainWindow.xaml.cs','"运行流程",()=>{_=Run();}'),
 'MutiRun':('implemented-local','MainWindow.xaml.cs','"连续运行",StartContinuous'),
 'StopRun':('implemented-local','MainWindow.xaml.cs','"停止",()=>activeRun?.Cancel()'),
}
MISSING={
 'ExportAlgorithm':'原生算法代码导出后端尚无完整实现',
 'ImportProcess':'原生流程导入及ID/模型落地开放',
 'AuthSet':'账户权限服务及持久化开放',
 'VMSet':'完整软件设置服务开放',
 'Solset':'原方案管理设置数据链开放',
 'CollectSet':'采集策略后端开放',
 'CreateCalibGuide':'标定引导服务与生命周期开放',
 'LogShow':'原日志窗口与日志服务消费未交付',
 'CommSet':'通信全局模块与会话开放，现工具栏禁用',
 'LightSet':'原全局光源管理入口尚未接入（工具插件并不等同此入口）',
 'ChSet':'全局语言切换与全部资源刷新开放',
 'EnSet':'全局语言切换与全部资源刷新开放',
 'HelpDoc':'本地帮助路径及启动链未接入',
 'MoreInfo':'更多版本组件信息窗口未接入',
 'OpenVClub':'V社区入口未接入',
 'WecPage':'原欢迎页及数据链未接入',
 'UndoList':'历史下拉/明细列表尚未实施，不以普通Undo替代',
 'GlobalVariable':'原全局变量模块及专用会话开放，现工具栏禁用',
 'GlobalScript':'全局脚本引擎与会话开放，现工具栏禁用',
 'GlobalTrigger':'全局触发器与会话开放，现工具栏禁用',
 'ComprehensiveConfig':'综合配置服务开放，现工具栏禁用',
 'EditFrontendUI':'真实设计器/缓冲及关闭依赖开放（A06）',
 'OpenExe':'原外部exe检查/启动入口未接入，不启动原厂工具',
 'OpenExeByLang':'原按语言选择外部exe入口未接入，不启动原厂工具',
 'OpenModuleParamTool':'原查找表工具/参数服务入口未接入',
}
def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()
def anchor(path,text):
    lines=path.read_text(encoding='utf-8-sig').splitlines()
    found=[i+1 for i,line in enumerate(lines) if text in line]
    if not found: raise ValueError(f'Current anchor absent: {path}: {text}')
    return {'path':str(path),'line':found[0],'needle':text,'sha256':sha(path)}
def resolver_anchor(command):
    pattern=re.compile(r'^\s*(?:public|private|protected|internal)\s+[^\n;=]*\b'+re.escape(command)+r'\s*\(',re.M)
    for path in (HANDLERS,EXTERNAL):
        text=path.read_text(encoding='utf-8-sig');match=pattern.search(text)
        if match:return {'path':str(path),'line':text[:match.start()].count('\n')+1,'member':command,'sha256':sha(path)}
    raise ValueError('Original method declaration not found: '+command)
def main():
    parser=argparse.ArgumentParser();parser.add_argument('--output',required=True);args=parser.parse_args()
    output=Path(args.output).resolve();output.mkdir(parents=True,exist_ok=True)
    source_text=SOURCE.read_text(encoding='utf-8-sig');handler_text=HANDLERS.read_text(encoding='utf-8-sig')+'\n'+EXTERNAL.read_text(encoding='utf-8-sig')
    entries=[];seen=set()
    def walk(node,indices=(),parents=(),hidden=False):
        fields={c.tag:(c.text or '').strip() for c in node if c.tag not in ('items','toolGroupItemInfo')}
        name=fields.get('name');is_hidden=hidden or fields.get('isDisplayInMenu','true').lower()=='false'
        identity='.'.join(map(str,indices)) or 'root';location=parents+((name or '[hidden-toolbar]'),)
        if name:
            commands={key:fields[key] for key in ('methodName','dynamicGetItemsMethodName','getCustomItemDataContextMethodName','initMethodName') if fields.get(key)}
            for command in commands.values():
                if not re.search(r'\b'+re.escape(command)+r'\s*\(',handler_text):raise ValueError('Original resolver absent: '+command)
            command=fields.get('methodName') or fields.get('dynamicGetItemsMethodName')
            current={'status':'structural-folder','reason':'资源容器；不是可执行命令'}
            if command:
                seen.add(command)
                if command in MAP:
                    status,file,needle=MAP[command];current={'status':status,'anchor':anchor(ROOT/'src/VisionEditor'/file,needle)}
                elif command in MISSING:current={'status':'not-implemented','reason':MISSING[command]}
                else:raise ValueError('Unclassified source command: '+command)
            group=node.find('toolGroupItemInfo')
            entries.append({'id':identity,'sourceHierarchy':list(location),'name':name,'hiddenInMenu':is_hidden,'fields':fields,'toolbar':({c.tag:(c.text or '').strip() for c in group} if group is not None else None),'sourceResolvers':commands,'sourceAnchors':{k:resolver_anchor(v) for k,v in commands.items()},'current':current})
        children=node.find('items')
        if children is not None:
            for index,child in enumerate(children):walk(child,indices+(index,),location if name or is_hidden else parents,is_hidden)
    walk(ET.fromstring(source_text))
    if seen != set(MAP)|set(MISSING):raise ValueError('Source/mapping fixed set differs: '+str(seen ^ (set(MAP)|set(MISSING))))
    counts={'namedResourceEntries':len(entries),'uniqueExecutableResolvers':len(seen),'toolbarEntries':sum(bool(e['toolbar']) for e in entries),'hiddenNamedEntries':sum(e['hiddenInMenu'] for e in entries)}
    counts['currentStatuses']={s:sum(e['current']['status']==s for e in entries) for s in sorted({e['current']['status'] for e in entries})}
    shortcut={'windowHandler':'VisionMasterMain.MainWindow.BaseWindow_PreviewKeyDown','rva':'0x818C','body':'nop; ret (2-byte CIL)','currentLibrarySha256':'319618b9f82dfc30546c900c1c19a665e0fce1d87d2a7ffa7bb4345915b180c1','evidence':'analysis/full-audit/batch17-20261019/A/MainWindow-library.il','limit':'此窗口方法无快捷键；继承类、画布、SDK/外部触发另列，不能外推全程序无快捷键。','currentCanvas':anchor(ROOT/'src/VisionEditor/WorkflowCanvas.cs','void CanvasKeyDown('),'sourceMenuKeyFields':sorted({key for e in entries for key in e['fields'] if 'key' in key.lower() or 'gesture' in key.lower()})}
    shortcut['baseWindowCommands']=anchor(ROOT/'analysis/decompiled/VM.Controls/VM.Controls/BaseWindow.cs','CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand')
    keyboard=[]
    for directory in ('VMControls.WPF','VisionMaster.Exe.Audit','VisionMasterMain','VM.Controls'):
        for path in sorted((ROOT/'analysis/decompiled'/directory).rglob('*.cs')):
            for number,line in enumerate(path.read_text(encoding='utf-8-sig').splitlines(),1):
                if re.search(r'\bKey\.[A-Z]|Keyboard\.Modifiers|new KeyBinding|new KeyGesture',line):keyboard.append({'path':str(path),'line':number,'expression':line.strip(),'fileSha256':sha(path)})
    shortcut['sourceKeyboardCandidateCount']=len(keyboard)
    shortcut['candidateLimit']='词法候选索引覆盖上述四个原证据树；局部编辑键、选择键和鼠标修饰键不自动等同主窗口全局快捷键。'
    (output/'keyboard-candidate-index.json').write_text(json.dumps(keyboard,ensure_ascii=False,indent=2),encoding='utf-8')
    result={'scope':'全量原menuitems.xml具名条目；main独立入口映射，未执行原厂代码/服务','source':{'path':str(SOURCE),'sha256':sha(SOURCE),'handlers':str(HANDLERS),'handlerSha256':sha(HANDLERS),'externalHandlers':str(EXTERNAL),'externalHandlerSha256':sha(EXTERNAL)},'counts':counts,'entries':entries,'shortcuts':shortcut}
    (output/'menu-entry-map.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
    rows=['# 第十七批完整原菜单资源逐项对照','',f'资源具名节点 {len(entries)}；唯一可执行resolver {len(seen)}。这些是审计分母，不是96项实施进度的新分母。','','|资源ID|完整路径|源方法/动态项|工具栏|当前状态|当前锚点或缺口|','|---|---|---|---|---|---|']
    for e in entries:
        c=e['current'];a=c.get('anchor');detail=(f"{Path(a['path']).relative_to(ROOT).as_posix()}:{a['line']}" if a else c.get('reason',''))
        rows.append('|'+ '|'.join([e['id'],' → '.join(e['sourceHierarchy']),'; '.join(e['sourceResolvers'].values()),str(e['toolbar'] or ''),c['status'],detail.replace('|','/')])+'|')
    rows+=['','窗口PreviewKeyDown当前CIL为nop/ret；不创建F5或CtrlS等未经原证的全局热键。源XML没有声明快捷键字段。当前画布快捷键有焦点/输入控件门槛，按既有画布证据单列。','', '本表完整记录外部服务缺口；不把disabled按钮或假窗口算成服务实现，不将普通Undo等同UndoList。']
    (output/'menu-entry-map.md').write_text('\n'.join(rows)+'\n',encoding='utf-8')
    print(json.dumps(counts,ensure_ascii=False))
if __name__=='__main__':main()
